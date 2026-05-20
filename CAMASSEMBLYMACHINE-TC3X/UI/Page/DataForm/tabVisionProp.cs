using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Utils;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using System.IO;
using CAMASSEMBLYMACHINE;
using TopEng.Vision;
using TopEng.Vision.Forms;
using TopEng.Controls;
using CAMASSEMBLYMACHINE.UI;
using static CAMASSEMBLYMACHINE.Define.UIDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabVisionProp : Form
    {
        int inspno;

        public tabVisionProp(int inspno)
        {
            InitializeComponent();
            tbxParamDesc.GotFocus += (s, ex) => this.ActiveControl = null;
            this.inspno = inspno;
        }

        private void tabVisionProp_Load(object sender, EventArgs e)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE && SystemDefine.NO_VISION_KEY) return;

            tbxParamDesc.GotFocus += (s, ex) => this.ActiveControl = null;
            tbxParamDesc.Text = Vision.inspection.recipeData[inspno].vppInfo[0].visionDesc;
            label_Title.Text = Vision.inspection.recipeData[inspno].inspectName;

            InitializeParamInfo();
            InitializeRecipeInfo();
        }

        public void UpdateVisionDataLoad()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE && SystemDefine.NO_VISION_KEY) return;

            label_Title.Text = Vision.inspection.recipeData[inspno].inspectName;

            InitializeParamInfo();
            InitializeRecipeInfo();
        }

        private void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Name", "StoredPos", "NewPos", "Unit", "Accept" };
            int[] headersize = { 255, 75, 75, 65, 65 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "TEXT", "BUTTON" };

            dataGrid_Light.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                    dataGrid_Light.Columns.Add(headername[i], headername[i]);
                if (headertype[i] == "BUTTON")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.Name = headername[i];
                    dataGrid_Light.Columns.Add(newColumn);
                }

                dataGrid_Light.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGrid_Light.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGrid_Light.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGrid_Light.Columns[i].Width = headersize[i];
            }

            dataGrid_Light.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;

            // ROWS
            int targetCam = Vision.inspection.InspInfo[inspno].targetCam;
            //if (0 < Vision.Camera.cameraInfo[targetCam].targetLight.a)
            //{
            dataGrid_Light.Rows.Add();
            dataGrid_Light.Rows[0].Cells[0].Value = "a";
            dataGrid_Light.Rows[0].Cells[1].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.a.ToString();
            dataGrid_Light.Rows[0].Cells[2].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.a.ToString();
            dataGrid_Light.Rows[0].Cells[3].Value = "LEVEL";
            //}
            //if (0 < Vision.Camera.cameraInfo[targetCam].targetLight.b)
            //{
            dataGrid_Light.Rows.Add();
            dataGrid_Light.Rows[1].Cells[0].Value = "b";
            dataGrid_Light.Rows[1].Cells[1].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.b.ToString();
            dataGrid_Light.Rows[1].Cells[2].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.b.ToString();
            dataGrid_Light.Rows[1].Cells[3].Value = "LEVEL";
            //}
            //if (0 < Vision.Camera.cameraInfo[targetCam].targetLight.c)
            //{
            dataGrid_Light.Rows.Add();
            dataGrid_Light.Rows[2].Cells[0].Value = "c";
            dataGrid_Light.Rows[2].Cells[1].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.c.ToString();
            dataGrid_Light.Rows[2].Cells[2].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.c.ToString();
            dataGrid_Light.Rows[2].Cells[3].Value = "LEVEL";
            //}
            //if (0 < Vision.Camera.cameraInfo[targetCam].targetLight.d)
            //{
            dataGrid_Light.Rows.Add();
            dataGrid_Light.Rows[3].Cells[0].Value = "d";
            dataGrid_Light.Rows[3].Cells[1].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.d.ToString();
            dataGrid_Light.Rows[3].Cells[2].Value = Vision.inspection.recipeData[inspno].inspInfo.lightSet.d.ToString();
            dataGrid_Light.Rows[3].Cells[3].Value = "LEVEL";
            //}


            for (int i = 0; i < dataGrid_Light.Rows.Count; i++)
            {
                dataGrid_Light.Rows[i].Height = 30;
                dataGrid_Light.Rows[i].Cells[4].Value = headername[4];
            }

            dataGrid_Light.CurrentCell = null;
        }

        private void InitializeRecipeInfo()
        {
            CogToolBlockHandler tools = Vision.inspection.recipeData[inspno].cogToolBlkProc;
            if (tools == null)
                return;
            string[] toolList = tools.GetToolListName();

            dataGrid_Tools.Columns.Clear();
            dataGrid_Tools.Columns.Add("No", "NO.");

            DataGridViewComboBoxCell newComboBox1 = new DataGridViewComboBoxCell();
            for (int i = 0; i < toolList.Length; i++)
                newComboBox1.Items.Add(toolList[i]);
            DataGridViewColumn newColumn1 = new DataGridViewColumn(newComboBox1);
            dataGrid_Tools.Columns.Add(newColumn1);
            dataGrid_Tools.Columns[1].HeaderText = "RESULT TOOL NAME";

            dataGrid_Tools.Columns[0].Width = 50;
            dataGrid_Tools.Columns[1].Width = 250;

            // ROWS
            for (int i = 0; i < Vision.inspection.recipeData[inspno].vppInfo.Count; i++)
            {
                dataGrid_Tools.Rows.Add();
                dataGrid_Tools.Rows[i].Cells[0].Value = i;

                string checkName = Vision.inspection.recipeData[inspno].vppInfo[i].resultToolName;
                bool isChecked = false;

                for (int j = 0; j < toolList.Length; ++j)
                {
                    if (toolList[j] == checkName)
                    {
                        isChecked = true;
                        break;
                    }
                }
                if (isChecked)
                    dataGrid_Tools.Rows[i].Cells[1].Value = Vision.inspection.recipeData[inspno].vppInfo[i].resultToolName;
                else
                {
                    Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, 
                        string.Format($"{Vision.inspection.recipeData[inspno].inspectName} " +
                        $"Vision 내 [{checkName}]가 없음 vppinfo.Json 파일을 수정 필요."));
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();
                    return;
                }
            }
        }

        public void UpdateRecipe()
        {
            dataGrid_Tools.EndEdit();

            for (int i = 0; i < dataGrid_Light.Rows.Count; i++)
            {
                if ("a" == dataGrid_Light.Rows[i].Cells[0].Value.ToString())
                    Vision.inspection.recipeData[inspno].inspInfo.lightSet.a = Convert.ToInt16(dataGrid_Light.Rows[i].Cells[1].Value);
                if ("b" == dataGrid_Light.Rows[i].Cells[0].Value.ToString())
                    Vision.inspection.recipeData[inspno].inspInfo.lightSet.b = Convert.ToInt16(dataGrid_Light.Rows[i].Cells[1].Value);
                if ("c" == dataGrid_Light.Rows[i].Cells[0].Value.ToString())
                    Vision.inspection.recipeData[inspno].inspInfo.lightSet.c = Convert.ToInt16(dataGrid_Light.Rows[i].Cells[1].Value);
                if ("d" == dataGrid_Light.Rows[i].Cells[0].Value.ToString())
                    Vision.inspection.recipeData[inspno].inspInfo.lightSet.d = Convert.ToInt16(dataGrid_Light.Rows[i].Cells[1].Value);
            }

            Vision.inspection.recipeData[inspno].vppInfo.Clear();

            try
            {
                for (int i = 0; i < dataGrid_Tools.Rows.Count; i++)
                {
                    VPPFILEINFO vppinfo = new VPPFILEINFO();
                    vppinfo.resultToolName = dataGrid_Tools.Rows[i].Cells[1].Value.ToString();
                    vppinfo.resultCount = Convert.ToInt32(dataGrid_Tools.Rows[i].Cells[2].Value);
                    vppinfo.resultAll = vppinfo.resultCount <= 0 ? true : false;

                    Vision.inspection.recipeData[inspno].vppInfo.Add(vppinfo);
                }
            }
            catch
            {
                MessageBox.Show("Save Exception ", "SYSTEM", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            int rowCount = dataGrid_Tools.Rows.Count;
            dataGrid_Tools.Rows.Add();
            dataGrid_Tools.Rows[rowCount].Cells[0].Value = rowCount;
            if (rowCount > 0)
                dataGrid_Tools.Rows[rowCount].Cells[1].Value = dataGrid_Tools.Rows[rowCount - 1].Cells[1].Value;
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            int rowCount = dataGrid_Tools.Rows.Count;
            if (rowCount <= 1)
                return;

            int index = dataGrid_Tools.SelectedRows.Count < 1 ? rowCount - 1 : dataGrid_Tools.SelectedRows[0].Index;

            dataGrid_Tools.Rows.RemoveAt(index);

            for (int i = 0; i < dataGrid_Tools.Rows.Count; i++)
            {
                dataGrid_Tools.Rows[i].Cells[0].Value = i;
            }
        }

        private void button_Edit_Click(object sender, EventArgs e)
        {
            string title = Vision.inspection.InspInfo[inspno].Name;
            string filepath = SystemDefine.recipePath + @"\" + Vision.inspection.CurrentModel + @"\" + title + @"\Inspection.vpp";

            int targetCam = Vision.inspection.InspInfo[inspno].targetCam;
            Vision.Camera.LightSet(targetCam, Vision.inspection.recipeData[inspno].inspInfo.lightSet);
            Vision.Camera.LightOn(targetCam, true);
            Vision.Grab(targetCam);
            if (!Vision.GrabCompleted(targetCam))
                return;
            ICogImage image = Vision.cogTool[targetCam].m_cogImage;

            var jobEdit = new Form_ToolBlockEdit(filepath, image);
            jobEdit.ShowDialog();
            Vision.inspection.recipeData[inspno].Reload();
            InitializeRecipeInfo();
            Vision.Camera.LightOn(targetCam, false);

            // 250512, Memory
            jobEdit.Dispose();
            image = null;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            UpdateRecipe();
            Vision.inspection.UpdateModel();
        }

        private void button_IndivisualEdit_Click(object sender, EventArgs e)
        {
            if (dataGrid_Tools.CurrentRow != null)
            {
                string strToolName = (string)dataGrid_Tools.CurrentRow.Cells[1].Value;

                ///
                string title = Vision.inspection.InspInfo[inspno].Name;
                string filepath = SystemDefine.recipePath + @"\" + Vision.inspection.CurrentModel + @"\" + title + @"\Inspection.vpp";

                int targetCam = Vision.inspection.InspInfo[inspno].targetCam;

                Vision.Camera.LightSet(targetCam, Vision.inspection.recipeData[inspno].inspInfo.lightSet);
                Vision.Camera.LightOn(targetCam, true);

                Vision.Grab(targetCam);
                if (!Vision.GrabCompleted(targetCam))
                    return;
                ICogImage image = Vision.cogTool[targetCam].m_cogImage;

                if (strToolName.Contains("CogPMAlignTool"))
                {
                    var jobEdit = new Form_ToolPMEdit(filepath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                else if (strToolName.Contains("CogFindCircleTool"))
                {
                    var jobEdit = new Form_ToolFindCircleEdit(filepath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                else if (strToolName.Contains("CogClassifyTool"))
                {
                    var jobEdit = new Form_ToolClassifyEdit(filepath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                else if (strToolName.Contains("CogOCRMaxTool"))
                {
                    var jobEdit = new Form_ToolOCREdit(filepath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                Vision.inspection.recipeData[inspno].Reload();
                InitializeRecipeInfo();
                Vision.Camera.LightOn(targetCam, false);
            }
        }

        private void dataGrid_Tools_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (/*e.ColumnIndex == 2 && */e.RowIndex >= 0)
            {
                CogToolBlockHandler tools = Vision.inspection.recipeData[inspno].cogToolBlkProc;
                if (tools == null)
                    return;
                string[] toolList = tools.GetToolListName();

                int rowIndex = dataGrid_Tools.CurrentRow.Index;
                string name = dataGrid_Tools.Rows[rowIndex].Cells[1].Value.ToString();

                if(toolList.Contains(name))
                   tools.SetEnable(name);

                tbxParamDesc.Text = Vision.inspection.recipeData[inspno].vppInfo[e.RowIndex].visionDesc;
            }
        }

        private void dataGrid_Light_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)UIPARAMCOLDATA.ACCEPT)
                    {
                        string newValue = dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString();
                        string curValue = dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString();

                        double dDifValue = Convert.ToDouble(newValue) - Convert.ToDouble(curValue);
                        Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifValue.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "값을 저장하시겠습니까?"));
                        if (formCurrent.ShowDialog() == DialogResult.Yes)
                        {
                            int targetCam = Vision.inspection.InspInfo[inspno].targetCam;

                            dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value = newValue;
                            //세이브
                            LIGHTDATA data = Vision.inspection.recipeData[inspno].inspInfo.lightSet;

                            if ("a" == dataGrid_Light.Rows[row].Cells[0].Value.ToString())
                                Vision.inspection.recipeData[inspno].inspInfo.lightSet.a = Convert.ToInt16(dataGrid_Light.Rows[row].Cells[1].Value);
                            if ("b" == dataGrid_Light.Rows[row].Cells[0].Value.ToString())
                                Vision.inspection.recipeData[inspno].inspInfo.lightSet.b = Convert.ToInt16(dataGrid_Light.Rows[row].Cells[1].Value);
                            if ("c" == dataGrid_Light.Rows[row].Cells[0].Value.ToString())
                                Vision.inspection.recipeData[inspno].inspInfo.lightSet.c = Convert.ToInt16(dataGrid_Light.Rows[row].Cells[1].Value);
                            if ("d" == dataGrid_Light.Rows[row].Cells[0].Value.ToString())
                                Vision.inspection.recipeData[inspno].inspInfo.lightSet.d = Convert.ToInt16(dataGrid_Light.Rows[row].Cells[1].Value);

                            Vision.inspection.UpdateModel();

                            string logText = $" LightSet [{dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.NAME].Value}] data has changed. [{curValue} → {newValue}]";
                            LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    if (col == (int)UIPARAMCOLDATA.EDITED)
                    {
                        string title = dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.NAME].Value.ToString();
                        double min = 0;
                        double max = 255; //RGB
                        string unit = dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.UNIT].Value.ToString();
                        string[] value = { dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString() };
                        SubForm_TenKey keyPad = new SubForm_TenKey(ref value, title, unit, min, max, 300, 300, false);
                        keyPad.ShowDialog();
                        dataGrid_Light.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value = value[0];
                    }
                }
                catch
                {

                }
            }
        }

        private void button_LoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string strCamName = Vision.inspection.InspInfo[inspno].Name;
                string strInitPath = "";
                strInitPath = SystemDefine.capturePath + @"\";
                strInitPath += DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                strInitPath += strCamName;

                if (!Directory.Exists(strInitPath)) strInitPath = SystemDefine.capturePath;
                OpenFileDialog dlg = new OpenFileDialog()
                {
                    FileName = "Select a Image file",
                    Filter = "Image files (*.bmp)|*.bmp",
                    Title = "Open Image file",
                    InitialDirectory = strInitPath,
                    RestoreDirectory = true
                };

                if (DialogResult.OK != dlg.ShowDialog()) return;
                var imagePath = dlg.FileName;

                CogImageFile file = new CogImageFile();
                file.Open(imagePath, CogImageFileModeConstants.Read);
                var cogImage = file[0];
                file.Close();

                string strVPPPath = SystemDefine.recipePath + @"\" + Vision.inspection.CurrentModel + @"\" + strCamName + @"\Inspection.vpp";
                ICogImage image = null;
                if (inspno == 0) image = CogImageConvert.GetRGBImage(cogImage, 0, 0, cogImage.Width, cogImage.Height);
                else image = CogImageConvert.GetIntensityImage(cogImage, 0, 0, cogImage.Width, cogImage.Height);

                var jobEdit = new Form_ToolBlockEdit(strVPPPath, image);
                jobEdit.ShowDialog();
                Vision.inspection.recipeData[inspno].Reload();
                InitializeRecipeInfo();

                // 250512, Memory
                jobEdit.Dispose();
                image = null;
            }
            catch (Exception ex)
            {

            }
        }

        private void button_LoadFileIndividual_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGrid_Tools.CurrentRow == null) return;
                string strToolName = (string)dataGrid_Tools.CurrentRow.Cells[1].Value;

                string strCamName = Vision.inspection.InspInfo[inspno].Name;
                string strInitPath = "";
                strInitPath = SystemDefine.capturePath + @"\";
                strInitPath += DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                strInitPath += strCamName;

                if (!Directory.Exists(strInitPath)) strInitPath = SystemDefine.capturePath;
                OpenFileDialog dlg = new OpenFileDialog()
                {
                    FileName = "Select a Image file",
                    Filter = "Image files (*.bmp)|*.bmp",
                    Title = "Open Image file",
                    InitialDirectory = strInitPath,
                    RestoreDirectory = true
                };

                if (DialogResult.OK != dlg.ShowDialog()) return;
                var imagePath = dlg.FileName;

                CogImageFile file = new CogImageFile();
                file.Open(imagePath, CogImageFileModeConstants.Read);
                var cogImage = file[0];
                file.Close();

                string strVPPPath = SystemDefine.recipePath + @"\" + Vision.inspection.CurrentModel + @"\" + strCamName + @"\Inspection.vpp";
                ICogImage image = CogImageConvert.GetIntensityImage(cogImage, 0, 0, cogImage.Width, cogImage.Height); ;

                if (strToolName.Contains("CogPMAlignTool"))
                {
                    var jobEdit = new Form_ToolPMEdit(strVPPPath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                else if (strToolName.Contains("CogFindCircleTool"))
                {
                    var jobEdit = new Form_ToolFindCircleEdit(strVPPPath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                else if (strToolName.Contains("CogClassifyTool"))
                {
                    var jobEdit = new Form_ToolClassifyEdit(strVPPPath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                else if (strToolName.Contains("CogOCRMaxTool"))
                {
                    var jobEdit = new Form_ToolOCREdit(strVPPPath, image, strToolName, inspno);
                    jobEdit.ShowDialog();

                    // 250512, Memory
                    jobEdit.Dispose();
                    jobEdit = null;
                }
                Vision.inspection.recipeData[inspno].Reload();
                InitializeRecipeInfo();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
