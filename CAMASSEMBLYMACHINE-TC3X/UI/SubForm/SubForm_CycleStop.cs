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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class SubForm_CycleStop : Form
    {
        bool refreshData = true;
        int[] lastStep = new int[(int)Machine.PROCESS.MAXPROC];

        public SubForm_CycleStop()
        {
            InitializeComponent();
        }

        private void SubForm_CycleStop_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            BuildProcessInfo();
            timer1.Enabled = true;
            refreshData = true;
        }

        private void BuildProcessInfo()
        {
            string[] headername = { "Process Name", "Step", "State" };
            int[] headersize = { 150, 400, 50 };
            string[] headertype = { "TEXT", "TEXT", "IMAGE" };

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

            string[] procname = { "PROC_HOME", "PROC_LOAD", "PROC_DECO", "PROC_GRIPPER", "PROC_SIDE_A", "PROC_SIDE_C", "PROC_UNLOAD", "PROC_NG" };

            // ROWS
            for (int i = 0; i < procname.Length; i++)
            {
                int target_column = 0;
                int target_row = i;
                dataGridViewAxis.Rows.Add();
                dataGridViewAxis.Rows[target_row].Cells[target_column + 0].Value = procname[i];
                dataGridViewAxis.Rows[target_row].Cells[target_column + 1].Value = "IDLE";
            }

            dataGridViewAxis.CurrentCell = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateProcessInfo();
            refreshData = false;

            if (!Machine.Busy())
                Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            button_Title.ButtonPush = !button_Title.ButtonPush;
        }

        private void UpdateProcessInfo()
        {
            string[] procname = { "PROC_HOME", "PROC_LOAD", "PROC_DECO", "PROC_GRIPPER", "PROC_SIDE_A", "PROC_SIDE_C", "PROC_UNLOAD", "PROC_NG" };
            int Step = 0;
            int Proc = 0;
            Bitmap stateBmp;

            // PROCESS HOME
            Proc = (int)Machine.PROCESS.HOME;
            Step = Machine.proclist[Proc].GetStep();
            if (lastStep[Proc - 2] != Step || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessHOME.STEP), Step);
                if (Step == (int)ProcessHOME.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessHOME.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = stateBmp;
                lastStep[Proc - 2] = Step;
            }
            // PROCESS TRAY IN
            Proc = (int)Machine.PROCESS.TRAYIN;
            Step = Machine.proclist[Proc].GetStep();
            if (lastStep[Proc - 2] != Step || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayIn.STEP), Step);
                if (Step == (int)ProcessTrayIn.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayIn.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = stateBmp;
                lastStep[Proc - 2] = Step;
            }
            // PROCESS TRAY IN BUF
            Proc = (int)Machine.PROCESS.TRAYINBUF;
            Step = Machine.proclist[Proc].GetStep();
            if (lastStep[Proc - 2] != Step || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayInBuf.STEP), Step);
                if (Step == (int)ProcessTrayInBuf.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayInBuf.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = stateBmp;
                lastStep[Proc - 2] = Step;
            }
            // PROCESS TRAY WORK
            Proc = (int)Machine.PROCESS.TRAYWORK;
            Step = Machine.proclist[Proc].GetStep();
            if (lastStep[Proc - 2] != Step || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayWork.STEP), Step);
                if (Step == (int)ProcessTrayWork.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayWork.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = stateBmp;
                lastStep[Proc - 2] = Step;
            }
            // PROCESS TRAY OUT
            Proc = (int)Machine.PROCESS.TRAYOUTBUF;
            Step = Machine.proclist[Proc].GetStep();
            if (lastStep[Proc - 2] != Step || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayOutBuf.STEP), Step);
                if (Step == (int)ProcessTrayOutBuf.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayOutBuf.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = stateBmp;
                lastStep[Proc - 2] = Step;
            }
            // PROCESS TRAY OUT BUF
            Proc = (int)Machine.PROCESS.TRAYOUT;
            Step = Machine.proclist[Proc].GetStep();
            if (lastStep[Proc - 2] != Step || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayOut.STEP), Step);
                if (Step == (int)ProcessTrayOut.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayOut.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = stateBmp;
                lastStep[Proc - 2] = Step;
            }            
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Machine.EStop(false);
            Close();
        }
    }
}
