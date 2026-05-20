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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabOptionData : Form
    {
        ParamUtil dataList;

        public tabOptionData(string title, ParamUtil datas)
        {
            InitializeComponent();

            label_Title.Text = title;
            dataList = datas;
        }

        public void ReloadDataList(string title, ParamUtil datas)
        {
            label_Title.Text = title;
            dataList = datas;

            ReloadData();
        }

        private void tabOptionData_Load(object sender, EventArgs e)
        {
            InitializeParamInfo();
        }

        private void tabOptionData_Shown(object sender, EventArgs e)
        {
            UpdateData();
        }

        public void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Name", "StoredVal", "NewVal", "Unit", "Accept" };
            int[] headersize = { 350, 75, 100, 70, 100 };
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
                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.NAME].Value = dataList.getParam(i).Name;

                if (dataList.getParam(i).Unit == "On/Off")
                {
                    if (dataList.getParam(i).Value.ToString() == "1")
                    {
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.STORED].Value = "USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED] .Value = "USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.STORED].Value = "Not USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED] .Value = "Not USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                    }
                }
                else
                {
                    dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Value = dataList.getParam(i).Value.ToString("0.000");
                    dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.STORED].Value = dataList.getParam(i).Value.ToString("0.000");
                }

                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.UNIT].Value = dataList.getParam(i).Unit;
                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.ACCEPT].Value = headername[(int)UIPARAMCOLDATA.ACCEPT];
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

                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.NAME].Value = dataList.getParam(i).Name;

                if (dataList.getParam(i).Unit == "On/Off")
                {
                    if (dataList.getParam(i).Value.ToString() == "1")
                    {
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Value = "USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Value = "Not USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                    }
                }

                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.UNIT].Value = dataList.getParam(i).Unit;
            }

            dataGridView1.CurrentCell = null;
        }

        public void ReloadData()
        {
            if (dataList.Count != dataGridView1.RowCount)
            {
                InitializeParamInfo();
                return;
            }
            // ROWS
            for (int i = 0; i < dataList.Count; i++)
            {
                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.NAME].Value = dataList.getParam(i).Name;

                if (dataList.getParam(i).Unit == "On/Off")
                {
                    if (dataList.getParam(i).Value.ToString() == "1")
                    {
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Value = "USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Value = "Not USE";
                        dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                    }
                }

                dataGridView1.Rows[i].Cells[(int)UIPARAMCOLDATA.UNIT].Value = dataList.getParam(i).Unit;
            }

            dataGridView1.CurrentCell = null;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (row >= 0)
                tbxParamDesc.Text = dataList.getParam(row).Desc + "_" + row.ToString();
            else
                tbxParamDesc.Text = "Unknown";

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)UIPARAMCOLDATA.ACCEPT)
                    {
                        string newValue = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString();
                        string curValue = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString();
                        string unit = dataList.getParam(row).Unit;
                        
                        if (dataList.getParam(row).Unit == "On/Off")
                        {
                            dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value = newValue;

                            if (newValue == "USE")
                                dataList.getParam(row).Value = Convert.ToDouble(1);
                            else
                                dataList.getParam(row).Value = Convert.ToDouble(0);

                            Machine.param.Write();
                            Machine.recipe.Write();

                            string logText = $"[{dataList.getParam(row).Name}] data has changed. [{curValue} {unit} → {newValue} {unit}]";
                            LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
                        }
                        else
                        {
                            double dDifValue = Convert.ToDouble(newValue) - Convert.ToDouble(curValue);
                            Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifValue.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "값을 저장하시겠습니까?"));
                            if (formCurrent.ShowDialog() == DialogResult.Yes)
                            {
                                dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value = newValue;
                                dataList.getParam(row).Value = Convert.ToDouble(newValue);
                                Machine.param.Write();
                                Machine.recipe.Write();

                                string logText = $"[{dataList.getParam(row).Name}] data has changed. [{curValue} {unit} → {newValue} {unit}]";
                                LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
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
                    if (col == (int)UIPARAMCOLDATA.EDITED)
                    {
                        string title = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.NAME].Value.ToString();
                        double min = dataList.getParam(row).Min;
                        double max = dataList.getParam(row).Max;
                        string unit = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.UNIT].Value.ToString();
                        string[] value = { dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString() };

                        if (dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString() == "USE")
                        {
                            dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value = "Not USE";
                            dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                        }
                        else if (dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString() == "Not USE")
                        {
                            dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value = "USE";
                            dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Style.ForeColor = Color.Black;
                        }
                        else
                        {
                            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, title, unit, min, max, 300, 300, true);
                            keyPad.ShowDialog();
                            dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value = value[0];
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }
}
