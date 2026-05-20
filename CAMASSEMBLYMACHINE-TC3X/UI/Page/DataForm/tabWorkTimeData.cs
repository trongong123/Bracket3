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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabWorkTimeData : Form
    {
        public tabWorkTimeData()
        {
            InitializeComponent();
        }

        private void tabWorkTimeTeach_Load(object sender, EventArgs e)
        {
            InitializeParamInfo();
        }

        private void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Name", "StoredPos", "NewPos", "Accept" };
            int[] headersize = { 255, 105, 105, 65 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "BUTTON" };

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
            for (int i = 0; i < Machine.param.workingtimelist.Count; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 30;
                dataGridView1.Rows[i].Cells[0].Value = Machine.param.workingtimelist[i].Name;
                int hour = Machine.param.workingtimelist[i].Hour;
                int minute = Machine.param.workingtimelist[i].Minute;
                int second = Machine.param.workingtimelist[i].Second;
                string value = $"{hour.ToString("00")}:{minute.ToString("00")}:{second.ToString("00")}";
                dataGridView1.Rows[i].Cells[1].Value = value;
                dataGridView1.Rows[i].Cells[2].Value = value;
                dataGridView1.Rows[i].Cells[3].Value = headername[3];
            }

            dataGridView1.CurrentCell = null;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (row >= 0)
                tbxParamDesc.Text = Machine.param.workingtimelist[row].Desc;
            else
                tbxParamDesc.Text = "Unknown";

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)UIWORKINGTIMECOLDATA.ACCEPT)
                    {
                        string newValue = dataGridView1.Rows[row].Cells[(int)UIWORKINGTIMECOLDATA.EDITED].Value.ToString();
                        string curValue = dataGridView1.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString();
                        dataGridView1.Rows[row].Cells[(int)UIWORKINGTIMECOLDATA.STORED].Value = newValue;
                        var time = newValue.Split(':');
                        Machine.param.workingtimelist[row].Hour = Convert.ToInt32(time[0]);
                        Machine.param.workingtimelist[row].Minute = Convert.ToInt32(time[1]);
                        Machine.param.workingtimelist[row].Second = Convert.ToInt32(time[2]);
                        Machine.param.Write();

                        string logText = $"[{Machine.param.workingtimelist[row].Name}] data has changed. [{curValue} → {newValue}]";
                        LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);

                        var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
                        proc.refreshState = true;
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
                    if (col == (int)UIWORKINGTIMECOLDATA.EDITED)
                    {
                        string title = dataGridView1.Rows[row].Cells[(int)UIWORKINGTIMECOLDATA.NAME].Value.ToString();
                        string value = dataGridView1.Rows[row].Cells[(int)UIWORKINGTIMECOLDATA.STORED].Value.ToString();
                        var time = value.Split(':');
                        var form = new SubForm_Time(Convert.ToInt32(time[0]), Convert.ToInt32(time[1]), Convert.ToInt32(time[2]));
                        form.ShowDialog();

                        if (form.DialogResult == DialogResult.OK)
                            dataGridView1.Rows[row].Cells[(int)UIWORKINGTIMECOLDATA.EDITED].Value = form.GetTime();
                    }
                }
                catch
                {

                }
            }
        }
    }
}
