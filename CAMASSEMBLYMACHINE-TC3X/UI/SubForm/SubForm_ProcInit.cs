using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CAMASSEMBLYMACHINE.Machine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class SubForm_ProcInit : Form
    {
        bool refreshData = true;

        public SubForm_ProcInit()
        {
            InitializeComponent();
        }

        private void SubForm_ProcInit_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            BuildProcessInfo();
            timer1.Enabled = true;
            refreshData = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateProcessInfo();
            refreshData = false;

            if (Machine.GetProcInitialized())
                Close();

            if (!Machine.Busy())
                Close();
        }

        private void BuildProcessInfo()
        {
            string[] headername = { "Process Name", "Compl" };
            int[] headersize = { 200, 50 };
            string[] headertype = { "TEXT", "IMAGE" };

            dataGridViewAxis.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                int target_column = i % headername.Length;

                if (headertype[target_column] == "TEXT")
                {
                    dataGridViewAxis.Columns.Add(headername[target_column], headername[target_column]);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                if (headertype[target_column] == "IMAGE")
                {
                    DataGridViewImageColumn newColumn = new DataGridViewImageColumn();
                    newColumn.HeaderText = headername[target_column];
                    newColumn.Name = headername[target_column];
                    newColumn.Image = Properties.Resources.LED_GRAY;
                    newColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    dataGridViewAxis.Columns.Add(newColumn);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                dataGridViewAxis.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewAxis.Columns[i].Width = headersize[target_column];
            }

            // ROWS
            foreach (PROCESS process in Enum.GetValues(typeof(PROCESS)))
            {
                if (process == PROCESS.MAXPROC || process == PROCESS.HOME || process == PROCESS.SYS) continue;
                int target_column = 0;
                int target_row = (int)process - (int)Machine.PROCESS.EXTERNALEQUIP;
                dataGridViewAxis.Rows.Add();
                dataGridViewAxis.Rows[target_row].Cells[target_column + 0].Value = Enum.GetName(typeof(Machine.PROCESS), (int)process);
            }

            dataGridViewAxis.CurrentCell = null;
        }

        private void UpdateProcessInfo()
        {
            int Step = 0;
            int Proc = 0;
            Bitmap stateBmp;

            foreach (PROCESS process in Enum.GetValues(typeof(PROCESS)))
            {
                if (process == PROCESS.MAXPROC || process == PROCESS.HOME || process == PROCESS.SYS) continue;
                if (Machine.proclist[(int)process].initCompl)
                    stateBmp = Properties.Resources.LED_GREEN;
                else
                    stateBmp = Properties.Resources.LED_RED;

                int target_row = (int)process - (int)Machine.PROCESS.EXTERNALEQUIP;
                dataGridViewAxis.Rows[target_row].Cells[1].Value = stateBmp;
            }
        }
    }
}
