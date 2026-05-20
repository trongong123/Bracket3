using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Utils;
using TopEng.Controls;
using Microsoft.Win32;
using CAMASSEMBLYMACHINE.Define;

namespace CAMASSEMBLYMACHINE.UI
{
    public enum UIREGISTRYCOLDATA
    {
        NAME,
        STORED,
        EDITED,
        UNIT,
        ACCEPT,

        UIPARAMCOLTEACHMAX
    };

    public partial class tabRegisterData : Form
    {
        RegistryUtil dataList;

        public tabRegisterData(string title)
        {
            InitializeComponent();

            label_Title.Text = title;
            dataList = new RegistryUtil();

            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg, true);
            string[] valueNames = wregKey.GetValueNames();

            for (int i = 0; i <valueNames.Length; ++i)
            {
                REGISTRYSTRUCT newParam = new REGISTRYSTRUCT();
                newParam.Name = valueNames[i];
                newParam.Value = wregKey.GetValue(valueNames[i], "0");
                if (newParam.Value.ToString() == "True" || newParam.Value.ToString() == "False")
                    newParam.Unit = "True/False";
                else
                {
                    newParam.Unit = "String";
                }

                dataList.dic[valueNames[i]] = newParam;
                dataList.GetParamList().Add(newParam);
            }
        }

        private void tabRegisterData_Load(object sender, EventArgs e)
        {
            InitializeParamInfo();
        }

        private void tabRegisterData_Shown(object sender, EventArgs e)
        {
            UpdateData();
        }

        public void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Name", "StoredVal", "NewVal", "Unit", "Accept" };
            int[] headersize = { 255, 160, 160, 100, 100 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "TEXT", "BUTTON" };

            dataGridView1.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                    dataGridView1.Columns.Add(headername[i], headername[i]);
                if (headertype[i] == "BUTTON")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.Name = headername[i];
                    dataGridView1.Columns.Add(newColumn);
                }
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[i].Width = headersize[i];
            }

            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;

            // ROWS
            for (int i = 0; i < dataList.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 30;
                dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.NAME].Value = dataList.getParam(i).Name;

                if (dataList.getParam(i).Unit == "True/False")
                {
                    DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();

                    if (dataList.getParam(i).Value.ToString() == "True")
                    {
                        dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.STORED].Value = "True";
                        buttonCell.Value = "True";
                        buttonCell.Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.STORED].Value = "False";
                        buttonCell.Value = "False";
                        buttonCell.Style.ForeColor = Color.DarkGray;
                    }
                    dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.EDITED] = buttonCell;
                }
                else
                {
                    dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value = dataList.getParam(i).Value.ToString();
                    dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.STORED].Value = dataList.getParam(i).Value.ToString();
                }

                dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.UNIT].Value = dataList.getParam(i).Unit;
                dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.ACCEPT].Value = headername[(int)UIREGISTRYCOLDATA.ACCEPT];
            }
            dataGridView1.CurrentCell = null;
        }

        public void UpdateData()
        {
            // ROWS
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataGridView1.RowCount >= i)
                    continue;

                dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.NAME].Value = dataList.getParam(i).Name;

                if (dataList.getParam(i).Unit == "On/Off")
                {
                    if (dataList.getParam(i).Value.ToString() == "True")
                    {
                        dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value = "True";
                        dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.EDITED].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value = "False";
                        dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.EDITED].Style.ForeColor = Color.DarkGray;
                    }
                }

                dataGridView1.Rows[i].Cells[(int)UIREGISTRYCOLDATA.UNIT].Value = dataList.getParam(i).Unit;
            }

            dataGridView1.CurrentCell = null;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (row >= 0)
                tbxParamDesc.Text = dataList.getParam(row).Desc;
            else
                tbxParamDesc.Text = "Unknown";

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)UIREGISTRYCOLDATA.ACCEPT)
                    {
                        RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg, true);

                        string newValue = dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value.ToString();
                        string curValue = dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.STORED].Value.ToString();
                        string unit = dataList.getParam(row).Unit;
                        
                        if (dataList.getParam(row).Unit == "True/False")
                        {
                            dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.STORED].Value = newValue;

                            if (newValue == "True")
                                dataList.getParam(row).Value = "True";
                            else
                                dataList.getParam(row).Value = "False";

                            wregKey.SetValue(dataList.getParam(row).Name, dataList.getParam(row).Value);

                            string logText = $"[{dataList.getParam(row).Name}] data has changed. [{curValue} {unit} → {newValue} {unit}]";
                            LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);

                            ReadRegistery();
                        }
                        else 
                        {
                            Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("Registry Value is Changed. Is it Okay?"));
                            if (formCurrent.ShowDialog() == DialogResult.Yes)
                            {
                                dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value = newValue;
                                dataList.getParam(row).Value = newValue;
                                wregKey.SetValue(dataList.getParam(row).Name, dataList.getParam(row).Value);

                                string logText = $"[{dataList.getParam(row).Name}] data has changed. [{curValue} {unit} → {newValue} {unit}]";
                                LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);

                                ReadRegistery();
                            }
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
                    if (col == (int)UIREGISTRYCOLDATA.EDITED)
                    {
                        string title = dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.NAME].Value.ToString();
                        string unit = dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.UNIT].Value.ToString();
                        string[] value = { dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.STORED].Value.ToString() };

                        if (dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED] is DataGridViewButtonCell)
                        {
                            if (dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value == "True")
                            {
                                dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value = "False";
                                dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Style.ForeColor = Color.DarkGray;
                            }
                            else
                            {
                                dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value = "True";
                                dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Style.ForeColor = Color.Green;
                            }
                        }
                        else
                        {
                            using (SubForm_InputText inputTextForm =
                                 new SubForm_InputText(title, dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value.ToString()))
                            {
                                inputTextForm.ShowDialog();
                                if (inputTextForm.DialogResult == DialogResult.OK)
                                {
                                    dataGridView1.Rows[row].Cells[(int)UIREGISTRYCOLDATA.EDITED].Value = inputTextForm.GetInputText();
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        static void ReadRegistery()
        {
            string language = "en-US";
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg);
            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg, true);

            if (wregKey == null)
                wregKey = Registry.CurrentUser.CreateSubKey(SystemDefine.pathSystemReg);

            if (wregKey != null)
            {
                language = (string)wregKey.GetValue("Language", "en-US");
                // SYSTEM SETTING
                SystemDefine.machineName = (string)wregKey.GetValue("Machine Name", "CAMASSEMBLYMACHINE");
                wregKey.SetValue("Machine Name", SystemDefine.machineName);
                SystemDefine.systemName = (string)wregKey.GetValue("System Name", "CAMASSEMBLYMACHINE-C-V1");
                wregKey.SetValue("System Name", SystemDefine.systemName);
                SystemDefine.externalEquipServerIP = (string)wregKey.GetValue("Server IP", "192.168.10.100");
                wregKey.SetValue("Server IP", SystemDefine.externalEquipServerIP);
                SystemDefine.externalEquipServerPort = (int)wregKey.GetValue("Server Port", 8888);
                wregKey.SetValue("Server Port", SystemDefine.externalEquipServerPort);
                // SYSTEM DATA
                SystemDefine.modelname = (string)wregKey.GetValue("Model Name", "none");
                wregKey.SetValue("Model Name", SystemDefine.modelname);
                SystemDefine.passwordOrigin = (string)wregKey.GetValue("Authority Origin Password", "1234");
                wregKey.SetValue("Developer Password", SystemDefine.passwordOrigin);
                SystemDefine.passwordData = (string)wregKey.GetValue("Authority Data Password", "1234");
                wregKey.SetValue("Data Password", SystemDefine.passwordData);
                // OPTION
                Machine.DeveloperMode = Convert.ToBoolean(wregKey.GetValue("Developer Mode", false));
                wregKey.SetValue("Developer Mode", Machine.DeveloperMode);
                Machine.DoorOpenDisregard = Convert.ToBoolean(wregKey.GetValue("Door-Open Disregard", false));
                wregKey.SetValue("Door-Open Disregard", Machine.DoorOpenDisregard);
                Machine.MutingOnDisregard = Convert.ToBoolean(wregKey.GetValue("Muting On Disregard", false));
                wregKey.SetValue("Door-Open Disregard", Machine.DoorOpenDisregard);
                Machine.AloneMode = false;
                wregKey.SetValue("Alone Mode", Machine.AloneMode);
                Machine.TrayHolding = Convert.ToBoolean(wregKey.GetValue("Tray Holding", false));
                wregKey.SetValue("Tray Holding", Machine.TrayHolding);
                Machine.RandomTest = Convert.ToBoolean(wregKey.GetValue("Random Input Test", false));
                wregKey.SetValue("Random Input Test", Machine.RandomTest);
                // OPTION - VISION
                SystemDefine.CamCountSim = Convert.ToInt16(wregKey.GetValue("Camera Count", "0"));
                wregKey.SetValue("Camera Count", SystemDefine.CamCountSim.ToString());
                SystemDefine.UpdateCameraInfo = Convert.ToBoolean(wregKey.GetValue("Update Camera Info", true));
                wregKey.SetValue("Update Camera Info", SystemDefine.UpdateCameraInfo);
                SystemDefine.deletelogperiod = Convert.ToInt16(wregKey.GetValue("Delete Log Period", "7"));
                wregKey.SetValue("Delete Log Period", SystemDefine.deletelogperiod.ToString());
                SystemDefine.deleteimageperiod = Convert.ToInt16(wregKey.GetValue("Delete Image Period", "1"));
                wregKey.SetValue("Delete Image Period", SystemDefine.deleteimageperiod.ToString());
                SystemDefine.UseLightOff = Convert.ToBoolean(wregKey.GetValue("Use Light Off", true));
                wregKey.SetValue("Use Light Off", SystemDefine.UseLightOff);
                SystemDefine.UseImageSave = Convert.ToBoolean(wregKey.GetValue("Use Image Save", true));
                wregKey.SetValue("Use Image Save", SystemDefine.UseImageSave);
                SystemDefine.UseLowQualityImage = Convert.ToBoolean(wregKey.GetValue("Use Low Quality Image", true));
                wregKey.SetValue("Use Low Quality Image", SystemDefine.UseLowQualityImage);
                SystemDefine.manualGrab = Convert.ToBoolean(wregKey.GetValue("Manual Grab", false));
                wregKey.SetValue("Manual Grab", SystemDefine.manualGrab);
                SystemDefine.motorVelocity = Convert.ToDouble(wregKey.GetValue("Motor Velocity", 100));
                wregKey.SetValue("Motor Velocity", SystemDefine.motorVelocity);

                SystemDefine.UpdateData();
            }


        }
    }
}
