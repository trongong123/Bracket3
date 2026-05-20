using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;
using CAMASSEMBLYMACHINE.Define;
using NV_UI;
using CAMASSEMBLYMACHINE.UI.SubForm;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_Data2 : Form
    {
        public enum DATAPAGE
        {
            UNKNOWN = -1,
            DATA_MENU,
            MODEL,
            MODEL_POSITION,
            MODEL_CALIBRATION,
            MODEL_OPTION,
            POSITION,
            VELOCITY,
            CALIBRATION,
            OPTION,
            INTERFERENCE,
            TIME,
            WORKINGTIME,
            VISION_TRAY,
            VISION_UNDER,
            VISION_JIG,
            VISION_PICKER,
            VISION_JIG2,
            REG_OPTION,
            SOFT_LIMIT
        }

        List<NV_UI.NV_Button_PB_NS> dataButton = new List<NV_UI.NV_Button_PB_NS>();
        List<Form> dataForm = new List<Form>();
        DATAPAGE currPage = DATAPAGE.UNKNOWN;


        public Form_Data2()
        {
            InitializeComponent();

            dataButton.Add(button_Menu);
            dataButton.Add(button_Model);
            dataButton.Add(button_ModelPosition);
            dataButton.Add(button_ModelCalibration);
            dataButton.Add(button_ModelOption);
            dataButton.Add(button_Position);
            dataButton.Add(button_Velocity);
            dataButton.Add(button_Calibration);
            dataButton.Add(button_Option);
            dataButton.Add(button_Interference);
            dataButton.Add(button_Time);
            dataButton.Add(button_WorkTime);
            dataButton.Add(button_VisionTRAY);
            dataButton.Add(button_VisionUNDER);
            dataButton.Add(button_VisionJIG);
            dataButton.Add(button_VisionPICKER);
            dataButton.Add(button_RegOption);
            dataButton.Add(button_SoftLimit);

            dataForm.Add(new tabModelData());
            dataForm.Add(new tabParamterData("Recipe Data [Position]", Machine.recipe.position));
            dataForm.Add(new tabCalibrationData("Recipe Data [Calibration]", Machine.recipe.calibration));
            dataForm.Add(new tabOptionData("Recipe Data [Option]", Machine.recipe.option));
            dataForm.Add(new tabParamterData("Parameter Data [Position]", Machine.param.position));
            dataForm.Add(new tabOptionData("Parameter Data [Velocity]", Machine.param.velocity));
            dataForm.Add(new tabCalibrationData("Parameter Data [Calibration]", Machine.param.calibration));
            dataForm.Add(new tabOptionData("Parameter Data [Option]", Machine.param.option));
            dataForm.Add(new tabParamterData("Parameter Data [Interference]", Machine.param.interference));
            dataForm.Add(new tabOptionData("Parameter Data [Time]", Machine.param.time));
            dataForm.Add(new tabWorkTimeData());
            dataForm.Add(new tabVisionProp((int)SystemDefine.CAMERA.TRAY));
            dataForm.Add(new tabVisionProp((int)SystemDefine.CAMERA.UNDER));
            dataForm.Add(new tabVisionProp((int)SystemDefine.CAMERA.JIG));
            dataForm.Add(new tabVisionProp((int)SystemDefine.CAMERA.PICKER));
            dataForm.Add(new tabVisionProp((int)SystemDefine.CAMERA.JIG2));
            dataForm.Add(new tabRegisterData("Registry Data [Option]"));
            dataForm.Add(new tabSoftLimitData());

            foreach (Form form in dataForm)
            {
                AddNewTabControlBoard(form);
            }
            foreach(NV_UI.NV_Button_PB_NS btn in dataButton)
            {
                btn.ClickEvent += SelectButton_ClickEvent;
            }

            // Hide TAB Button
            tabControlBoard.Appearance = TabAppearance.Buttons;
            tabControlBoard.SizeMode = TabSizeMode.Fixed;
            tabControlBoard.ItemSize = new Size(0, 1);
        }
        private void SelectButton_ClickEvent(object sender, EventArgs e)
        {
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out DATAPAGE selectPage))
            {
                if (selectPage == DATAPAGE.REG_OPTION)
                {
                    SubForm_Login dlg = new SubForm_Login(SystemDefine.USER_LEVEL.AUTH_DATA);
                    if (DialogResult.OK != dlg.ShowDialog())
                        return;
                }

                NV_Button_PB_NS btn = (NV_Button_PB_NS)sender;
                nV_Button_PB_NS_Title.Text = btn.Text == "" ? "Data Menu" : btn.Text;
                tabControlBoard.SelectedIndex = (int)selectPage < tabControlBoard.TabCount ? (int)selectPage : tabControlBoard.SelectedIndex;
                currPage = selectPage;
                UpdatePage();
            }
        }
        public void ChangePage(DATAPAGE page)
        {
            groupBox6.Visible = Machine.DeveloperMode;
            nV_Button_PB_NS_Title.Text = "Data Menu";
            tabControlBoard.SelectedIndex = (int)page;
            currPage = page;
            UpdatePage();
        }
        public void UpdatePage()
        {
            if (currPage > (int)DATAPAGE.DATA_MENU)
            {
                tabParamterData paramData = dataForm[(int)currPage - 1] as tabParamterData;
                if (paramData != null)
                {
                    switch (currPage)
                    {
                        case DATAPAGE.MODEL_POSITION:
                            paramData.ReloadDataList("Recipe Data [Position]", Machine.recipe.position);
                            break;                        
                        case DATAPAGE.POSITION:
                            paramData.ReloadDataList("Parameter Data [Position]", Machine.param.position);
                            break;
                        case DATAPAGE.INTERFERENCE:
                            paramData.ReloadDataList("Parameter Data [Interference]", Machine.param.interference);
                            break;
                    }
                }
                tabCalibrationData caliData = dataForm[(int)currPage - 1] as tabCalibrationData;
                if (caliData != null)
                {
                    switch (currPage)
                    {
                        case DATAPAGE.MODEL_CALIBRATION:
                            caliData.ReloadDataList("Recipe Data [Calibration]", Machine.recipe.calibration);
                            break;
                        case DATAPAGE.CALIBRATION:
                            caliData.ReloadDataList("Parameter Data [Calibration]", Machine.param.calibration);
                            break;
                    }
                }
                tabOptionData optionData = dataForm[(int)currPage - 1] as tabOptionData;
                if (optionData != null)
                {
                    switch (currPage)
                    {
                        case DATAPAGE.VELOCITY:
                            optionData.ReloadDataList("Parameter Data [Velocity]", Machine.param.velocity);
                            break;
                        case DATAPAGE.MODEL_OPTION:
                            optionData.ReloadDataList("Recipe Data [Option]", Machine.recipe.option);
                            break;
                        case DATAPAGE.OPTION:
                            optionData.ReloadDataList("Parameter Data [Option]", Machine.param.option);
                            break;
                        case DATAPAGE.TIME:
                            optionData.ReloadDataList("Parameter Data [Time]", Machine.param.time);
                            break;
                    }
                }
                else
                {
                    tabVisionProp visionData = dataForm[(int)currPage - 1] as tabVisionProp;
                    if (visionData != null)
                        visionData.UpdateVisionDataLoad();
                }
            }
        }

        private void AddNewTabControlBoard(Form _frm)
        {
            TabPage tab = new TabPage(_frm.Text);
            _frm.TopLevel = false;
            _frm.Parent = tab;
            _frm.Visible = true;
            tabControlBoard.TabPages.Add(tab);
            _frm.Dock = DockStyle.Fill;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x2000000;
                return cp;
            }
        }

        private void Form_Data2_Shown(object sender, EventArgs e)
        {
            tabControlBoard.SelectedIndex = 0;
        }

        private void button_Export_ClickEvent(object sender, EventArgs e)
        {
            Program.errorProc.alarm.ExportCSV();
        }

        public void ShowHiddenGroup()
        {
            groupBox6.Visible = true;
        }
    }
}
