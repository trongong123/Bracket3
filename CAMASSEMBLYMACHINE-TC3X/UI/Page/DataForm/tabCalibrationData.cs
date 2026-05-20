using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Utils;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabCalibrationData : Form
    {
        enum CALIBUNIT
        {
            PROD_LOADER,
            ASSEMBLER,

            TOTAL
        }

        ParamUtil dataListFirst = new ParamUtil();
        ParamUtil[] dataLists = new ParamUtil[2];
        DataGridView[] dataGridViews = new DataGridView[2];

        public tabCalibrationData(string title, ParamUtil datas)
        {
            InitializeComponent();

            dataListFirst = datas;
            label_Title.Text = title;

            dataGridView1.Tag = CALIBUNIT.PROD_LOADER.ToString();
            dataGridView2.Tag = CALIBUNIT.ASSEMBLER.ToString();
            dataGridViews[(int)CALIBUNIT.PROD_LOADER] = dataGridView1;
            dataGridViews[(int)CALIBUNIT.ASSEMBLER] = dataGridView2;

            for (int i = 0; i < dataLists.Length; ++i)
            {
                dataLists[i] = new ParamUtil();
            }
            DivideDataList();
        }

        public void ReloadDataList(string title, ParamUtil datas)
        {
            label_Title.Text = title;
            DivideDataList();

            ReloadData();
        }

        void DivideDataList()
        {
            dataLists[0].GetParamList().Clear();
            dataLists[1].GetParamList().Clear();

            for (int i = 0; i < dataListFirst.GetParamList().Count; ++i)
            {
                if (dataListFirst.GetParamList()[i].Tag.ToUpper().Contains("PROD")
                    || dataListFirst.GetParamList()[i].Tag.ToUpper().Contains("TRAY"))
                {
                    dataLists[0].GetParamList().Add(dataListFirst.GetParamList()[i]);
                }
                else if (dataListFirst.GetParamList()[i].Tag.ToUpper().Contains("ASSEMBLE")
                    || dataListFirst.GetParamList()[i].Tag.ToUpper().Contains("UNDER")
                    || dataListFirst.GetParamList()[i].Tag.ToUpper().Contains("JIG"))
                {
                    dataLists[1].GetParamList().Add(dataListFirst.GetParamList()[i]);
                }
            }
        }

        private void tabCalibrationData_Load(object sender, EventArgs e)
        {
            for (int count = 0; count < (int)CALIBUNIT.TOTAL; ++count)
            {
                InitializeParamInfo(dataLists[count], dataGridViews[count]);
            }
        }

        private void tabCalibrationData_Shown(object sender, EventArgs e)
        {
            UpdateData();
        }

        public void InitializeParamInfo(ParamUtil dataListN, DataGridView dataGridViewN)
        {
            // COLUMNS
            string[] headername = { "Name", "StoredVal", "NewVal", "Unit", "Accept" };
            int[] headersize = { 255, 75, 75, 65, 65 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "TEXT", "BUTTON" };

            dataGridViewN.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                    dataGridViewN.Columns.Add(headername[i], headername[i]);
                if (headertype[i] == "BUTTON")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.Name = headername[i];
                    dataGridViewN.Columns.Add(newColumn);
                }

                dataGridViewN.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewN.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGridViewN.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewN.Columns[i].Width = headersize[i];
            }

            dataGridViewN.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;

            // ROWS
            for (int i = 0; i < dataListN.Count; i++)
            {
                dataGridViewN.Rows.Add();
                dataGridViewN.Rows[i].Height = 30;
                dataGridViewN.Rows[i].Cells[0].Value = dataListN.getParam(i).Name;
                dataGridViewN.Rows[i].Cells[1].Value = dataListN.getParam(i).Value.ToString("0.000");
                dataGridViewN.Rows[i].Cells[2].Value = dataListN.getParam(i).Value.ToString("0.000");
                dataGridViewN.Rows[i].Cells[3].Value = dataListN.getParam(i).Unit;
                dataGridViewN.Rows[i].Cells[4].Value = headername[4];
            }

            dataGridViewN.CurrentCell = null;
        }

        public void UpdateData()
        {
            for (int count = 0; count < (int)CALIBUNIT.TOTAL; ++count)
            {
                // ROWS
                for (int i = 0; i < dataLists[count].Count; i++)
                {
                    if (dataGridViews[count].RowCount >= i)
                        continue;

                    dataGridViews[count].Rows[i].Cells[0].Value = dataLists[count].getParam(i).Name;
                    dataGridViews[count].Rows[i].Cells[1].Value = dataLists[count].getParam(i).Value.ToString("0.000");
                    dataGridViews[count].Rows[i].Cells[2].Value = dataLists[count].getParam(i).Value.ToString("0.000");
                    dataGridViews[count].Rows[i].Cells[3].Value = dataLists[count].getParam(i).Unit;
                }

                dataGridViews[count].CurrentCell = null;
            }
        }
        
        public void ReloadData()
        {
            for (int count = 0; count < (int)CALIBUNIT.TOTAL; ++count)
            {
                if (dataLists[count].Count != dataGridViews[count].RowCount)
                {
                    InitializeParamInfo(dataLists[count], dataGridViews[count]);
                    return;
                }
                // ROWS
                for (int i = 0; i < dataLists[count].Count; i++)
                {
                    dataGridViews[count].Rows[i].Cells[0].Value = dataLists[count].getParam(i).Name;
                    dataGridViews[count].Rows[i].Cells[1].Value = dataLists[count].getParam(i).Value.ToString("0.000");
                    dataGridViews[count].Rows[i].Cells[2].Value = dataLists[count].getParam(i).Value.ToString("0.000");
                    dataGridViews[count].Rows[i].Cells[3].Value = dataLists[count].getParam(i).Unit;
                }

                dataGridViews[count].CurrentCell = null;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            ParamUtil dataList = new ParamUtil();

            if (dataGridView.Tag.ToString() == CALIBUNIT.PROD_LOADER.ToString())
            {
                dataList = dataLists[(int)CALIBUNIT.PROD_LOADER];
            }
            else if (dataGridView.Tag.ToString() == CALIBUNIT.ASSEMBLER.ToString())
            {
                dataList = dataLists[(int)CALIBUNIT.ASSEMBLER];
            }

            if (row >= 0)
                tbxParamDesc.Text = dataList.getParam(row).Desc;
            else
                tbxParamDesc.Text = "Unknown";

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)UIPARAMCOLDATA.ACCEPT)
                    {
                        string newValue = dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString();
                        string curValue = dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString();
                        string unit = dataList.getParam(row).Unit;

                        double dDifValue = Convert.ToDouble(newValue) - Convert.ToDouble(curValue);
                        Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifValue.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "값을 저장하시겠습니까?"));
                        if (formCurrent.ShowDialog() == DialogResult.Yes)
                        {
                            dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value = newValue;
                            dataList.getParam(row).Value = Convert.ToDouble(newValue);
                            Machine.param.Write();
                            Machine.recipe.Write();

                            string logText = $"[{dataList.getParam(row).Name}] data has changed. [{curValue} {unit} → {newValue} {unit}]";
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
                        string title = dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.NAME].Value.ToString();
                        double min = dataList.getParam(row).Min;
                        double max = dataList.getParam(row).Max;
                        string unit = dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.UNIT].Value.ToString();
                        string[] value = { dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString() };
                        SubForm_TenKey keyPad = new SubForm_TenKey(ref value, title, unit, min, max, 300, 300, true);
                        keyPad.ShowDialog();
                        dataGridView.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value = value[0];
                    }
                }
                catch
                {

                }
            }
        }

    }
}
