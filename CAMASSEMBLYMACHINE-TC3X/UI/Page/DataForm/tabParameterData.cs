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
    public partial class tabParamterData : Form
    {
        ParamUtil dataList;

        public tabParamterData(string title, ParamUtil datas)
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

        private void tabParamterData_Load(object sender, EventArgs e)
        {
            InitializeParamInfo();
        }

        private void tabParamterData_Shown(object sender, EventArgs e)
        {
            UpdateData();
        }

        public void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Name", "StoredVal", "NewVal", "GetPos", "Accept", "Unit" };
            int[] headersize = { 350, 75, 75, 65, 65, 65, 65 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "BUTTON", "BUTTON", "TEXT" };

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
                dataGridView1.Rows[i].Cells[0].Value = dataList.getParam(i).Name;
                dataGridView1.Rows[i].Cells[1].Value = dataList.getParam(i).Value.ToString("0.000");
                dataGridView1.Rows[i].Cells[2].Value = dataList.getParam(i).Value.ToString("0.000");
                dataGridView1.Rows[i].Cells[3].Value = headername[3];
                dataGridView1.Rows[i].Cells[4].Value = headername[4];
                dataGridView1.Rows[i].Cells[5].Value = dataList.getParam(i).Unit;
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

                dataGridView1.Rows[i].Cells[0].Value = dataList.getParam(i).Name;
                dataGridView1.Rows[i].Cells[1].Value = dataList.getParam(i).Value.ToString("0.000");
                dataGridView1.Rows[i].Cells[2].Value = dataList.getParam(i).Value.ToString("0.000");
                dataGridView1.Rows[i].Cells[5].Value = dataList.getParam(i).Unit;
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
                dataGridView1.Rows[i].Cells[0].Value = dataList.getParam(i).Name;
                dataGridView1.Rows[i].Cells[1].Value = dataList.getParam(i).Value.ToString("0.000");
                dataGridView1.Rows[i].Cells[2].Value = dataList.getParam(i).Value.ToString("0.000");
                dataGridView1.Rows[i].Cells[5].Value = dataList.getParam(i).Unit;
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
                    if (col == (int)UIPARAMINFO.GETPOS)
                    {
                        int targetAxis = dataList.getParam(row).TargetAxis;
                        double actualPos = 0;
                        Machine.motion.GetAxisActualPos(targetAxis, ref actualPos);
                        dataGridView1.Rows[row].Cells[(int)UIPARAMINFO.EDITED].Value = (0.001 * actualPos).ToString("0.000");
                    }

                    if (col == (int)UIPARAMCOLDATA.ACCEPT)
                    {
                        string newValue = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString();
                        string curValue = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString();
                        string unit = dataList.getParam(row).Unit;

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
                        SubForm_TenKey keyPad = new SubForm_TenKey(ref value, title, unit, min, max, 300, 300, true);
                        keyPad.ShowDialog();
                        dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value = value[0];
                    }
                }
                catch
                {

                }
            }
        }

    }
}
