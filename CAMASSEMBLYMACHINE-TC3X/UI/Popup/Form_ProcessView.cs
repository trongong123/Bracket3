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

namespace CAMASSEMBLYMACHINE.UI.SubForm
{
    public partial class Form_ProcessView : Form
    {
        bool refreshData = true;
        double[] actualPosOLD = new double[(int)AXIS.AXISMAX];
        bool[] neglimitOLD = new bool[(int)AXIS.AXISMAX];
        bool[] poslimitOLD = new bool[(int)AXIS.AXISMAX];
        bool[] homeDoneOLD = new bool[(int)AXIS.AXISMAX];
        bool[] servoOnOLD = new bool[(int)AXIS.AXISMAX];
        bool[] alarmOLD = new bool[(int)AXIS.AXISMAX];

        int[] lastStep = new int[20];
        int[] lastAutoStep = new int[20];

        const int MAXROWS = 20;
        int HEADERCOUNT = 8;

        enum VIEW
        {
            NONE,
            AXISINFO,
            PROCESSINFO,
        }

        VIEW viewInfo = VIEW.NONE;

        public Form_ProcessView()
        {
            InitializeComponent();
        }

        private void Form_ProcessView_Load(object sender, EventArgs e)
        {
            Point Loc = new Point();
            Loc.X = MainForm.mainForm.Location.X - 7;
            Loc.Y = MainForm.mainForm.Location.Y + MainForm.mainForm.Size.Height - Size.Height + 7;
            Location = Loc;

            button_SelectAxis.ButtonPush = true;
            BuildAxisInfo();
            refreshData = true;
            timer1.Enabled = true;
            viewInfo = VIEW.AXISINFO;
        }

        private void BuildProcessInfo()
        {
            string[] headername = { "Process Name", "Step", "AutoStep", "State" };
            int[] headersize = { 150, 200, 200, 50 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "IMAGE" };

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


            List<string> procname = new List<string>();

            foreach (Machine.PROCESS prc in Enum.GetValues(typeof(Machine.PROCESS)))
            {
                if (prc is Machine.PROCESS.SYS || prc is Machine.PROCESS.HOME || prc is Machine.PROCESS.MAXPROC) continue;
                else procname.Add(prc.ToString());
            }

            // ROWS
            for (int i = 0; i < procname.Count; i++)
            {
                int target_column = (i / MAXROWS) * headername.Length;
                int target_row = i % MAXROWS;
                dataGridViewAxis.Rows.Add();
                dataGridViewAxis.Rows[target_row].Cells[target_column + 0].Value = procname[i];
                dataGridViewAxis.Rows[target_row].Cells[target_column + 1].Value = "IDLE";
            }

            dataGridViewAxis.CurrentCell = null;
        }

        private void BuildAxisInfo()
        {
            // COLUMNS
            string[] headername = { "No", "Name", "Act.Pos", "-", "+", "H", "Sv", "AL" };
            int[] headersize = { 30, 217, 75, 26, 26, 26, 26, 26 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "IMAGE", "IMAGE", "IMAGE", "IMAGE", "IMAGE" };

            HEADERCOUNT = headername.Length;

            dataGridViewAxis.Columns.Clear();

            for (int i = 0; i < 2 * headername.Length; i++)
            {
                int target_column = i % headername.Length;

                if (headertype[target_column] == "TEXT")
                    dataGridViewAxis.Columns.Add(headername[target_column], headername[target_column]);
                if (headertype[target_column] == "IMAGE")
                {
                    DataGridViewImageColumn newColumn = new DataGridViewImageColumn();
                    newColumn.HeaderText = headername[target_column];
                    newColumn.Name = headername[target_column];
                    newColumn.Image = Properties.Resources.LED_GRAY;
                    newColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    dataGridViewAxis.Columns.Add(newColumn);
                }

                dataGridViewAxis.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridViewAxis.Columns[i].Width = headersize[target_column];
            }

            // ROWS
            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                int target_column = (i / MAXROWS) * headername.Length;
                int target_row = i % MAXROWS;
                dataGridViewAxis.Rows.Add();
                dataGridViewAxis.Rows[target_row].Cells[target_column + 0].Value = i;
                dataGridViewAxis.Rows[target_row].Cells[target_column + 1].Value = Enum.GetName(typeof(AXIS), i);
                dataGridViewAxis.Rows[target_row].Cells[target_column + 2].Value = "0.000";
            }

            dataGridViewAxis.CurrentCell = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label_MachineState.Text = Machine.status.state.ToString();

            if (viewInfo == VIEW.AXISINFO)
                UpdateAxisInfo();
            if (viewInfo == VIEW.PROCESSINFO)
                UpdateProcessInfo();

            refreshData = false;
        }

        private void UpdateProcessInfo()
        {
            int Step = 0;
            int AutoStep = 0;
            int Proc = 0;
            Bitmap stateBmp;

            // PROCESS EXTERNALEQUIP
            Proc = (int)Machine.PROCESS.EXTERNALEQUIP;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessExternalEquip.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = Enum.GetName(typeof(ProcessExternalEquip.STEP), Step);
                if (Step == (int)ProcessExternalEquip.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessExternalEquip.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS TRAY IN
            Proc = (int)Machine.PROCESS.TRAYIN;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayIn.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessTrayIn)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessTrayIn.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayIn.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS TRAY IN BUF
            Proc = (int)Machine.PROCESS.TRAYINBUF;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayInBuf.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessTrayInBuf)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessTrayInBuf.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayInBuf.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS TRAY WORK
            Proc = (int)Machine.PROCESS.TRAYWORK;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 1] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayWork.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessTrayWork)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessTrayWork.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayWork.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS TRAY OUT BUF
            Proc = (int)Machine.PROCESS.TRAYOUTBUF;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayOutBuf.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessTrayOutBuf)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessTrayOutBuf.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayOutBuf.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS TRAY OUT
            Proc = (int)Machine.PROCESS.TRAYOUT;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessTrayOut.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessTrayOut)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessTrayOut.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessTrayOut.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS LD Picker
            Proc = (int)Machine.PROCESS.PROD_LOADER;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessProdLoader.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessProdLoader)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessProdLoader.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessProdLoader.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS ULD Picker
            Proc = (int)Machine.PROCESS.ASSEMBLER;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessAssembler.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessAssembler)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessAssembler.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessAssembler.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS Left Buffer
            Proc = (int)Machine.PROCESS.LEFTBUF;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessBufStageOne.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessBufStageOne)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessBufStageOne.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessBufStageOne.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS Right Buffer
            Proc = (int)Machine.PROCESS.RIGHTBUF;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessBufStageOne.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessBufStageOne)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessBufStageOne.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessBufStageOne.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS JIGINBUF
            Proc = (int)Machine.PROCESS.JIGINBUF;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessJigInBuf.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessJigInBuf)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessJigInBuf.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessJigInBuf.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS JIGWORK
            Proc = (int)Machine.PROCESS.JIGWORK;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessJigWork.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessJigWork)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessJigWork.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessJigWork.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS JIGOUTBUF
            Proc = (int)Machine.PROCESS.JIGOUTBUF;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessJigOutBuf.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessJigOutBuf)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessJigOutBuf.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessJigOutBuf.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS JIGINCARRIER
            Proc = (int)Machine.PROCESS.JIGINCARRIER;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessJigInCarrier.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessJigInCarrier)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessJigInCarrier.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessJigInCarrier.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
            // PROCESS JIGOUTCARRIER
            Proc = (int)Machine.PROCESS.JIGOUTCARRIER;
            Step = Machine.proclist[Proc].GetStep();
            AutoStep = Machine.proclist[Proc].GetAutoStep();
            if (lastStep[Proc - 2] != Step || lastAutoStep[Proc - 2] != AutoStep || refreshData)
            {
                dataGridViewAxis.Rows[Proc - 2].Cells[1].Value = Enum.GetName(typeof(ProcessJigOutCarrier.STEP), Step);
                dataGridViewAxis.Rows[Proc - 2].Cells[2].Value = ((ProcessJigOutCarrier)Machine.proclist[Proc]).AutoStep;
                if (Step == (int)ProcessJigOutCarrier.STEP.IDLE) stateBmp = Properties.Resources.LED_YELLOW;
                else if (Step == (int)ProcessJigOutCarrier.STEP.ERROR) stateBmp = Properties.Resources.LED_RED;
                else
                    stateBmp = Properties.Resources.LED_GREEN;
                dataGridViewAxis.Rows[Proc - 2].Cells[3].Value = stateBmp;
                lastStep[Proc - 2] = Step;
                lastAutoStep[Proc - 2] = AutoStep;
            }
        }

        private void UpdateAxisInfo()
        {
            if (Machine.motion == null)
                return;

            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                int axis = i;
                int target_column = (i / MAXROWS) * HEADERCOUNT;
                int target_row = i % MAXROWS;

                // ACTUAL POSITION
                double actualPos = 0;
                Machine.motion.GetAxisActualPos(axis, ref actualPos);
                if (actualPos != actualPosOLD[i] || refreshData)
                {
                    dataGridViewAxis.Rows[target_row].Cells[target_column + (int)UIAXISINFO.ACTUALPOS].Value = (0.001 * actualPos).ToString("0.000");
                    actualPosOLD[i] = actualPos;
                }
                // NEGATIVE LIMIT
                bool neglimit = false;
                Machine.motion.GetAxis_NEGLimit(axis, ref neglimit);
                if (neglimit != neglimitOLD[i] || refreshData)
                {
                    Bitmap negBmp = neglimit ? Properties.Resources.LED_RED : Properties.Resources.LED_GREEN;
                    dataGridViewAxis.Rows[target_row].Cells[target_column + (int)UIAXISINFO.NLIMIT].Value = negBmp;
                    neglimitOLD[i] = neglimit;
                }
                // POSITIVE LIMIT
                bool poslimit = false;
                Machine.motion.GetAxis_POSLimit(axis, ref poslimit);
                if (poslimit != poslimitOLD[i] || refreshData)
                {
                    Bitmap posBmp = poslimit ? Properties.Resources.LED_RED : Properties.Resources.LED_GREEN;
                    dataGridViewAxis.Rows[target_row].Cells[target_column + (int)UIAXISINFO.PLIMIT].Value = posBmp;
                    poslimitOLD[i] = poslimit;
                }
                // HOME STATE
                //bool homeDone = false;
                //Machine.motion.GetAxisHomeComplete(axis, ref homeDone);
                //if (homeDone != homeDoneOLD[i] && refreshData)
                //{
                //    Bitmap homeBmp = homeDone ? Properties.Resources.LED_GREEN : Properties.Resources.LED_RED;
                //    dataGridViewAxis.Rows[target_row].Cells[target_column + (int)UIAXISINFO.HOME].Value = homeBmp;
                //    homeDoneOLD[i] = homeDone;
                //}
                // SERVO ENABLE
                bool servoOn = false;
                Machine.motion.GetAxisServoOn(axis, ref servoOn);
                if (servoOn != servoOnOLD[i] || refreshData)
                {
                    Bitmap servoBmp = servoOn ? Properties.Resources.LED_GREEN : Properties.Resources.LED_GRAY;
                    dataGridViewAxis.Rows[target_row].Cells[target_column + (int)UIAXISINFO.SERVO].Value = servoBmp;
                    servoOnOLD[i] = servoOn;
                }
                // GET ALARM
                bool alarm = false;
                Machine.motion.GetAlarm(axis, ref alarm);
                if (alarm != alarmOLD[i] || refreshData)
                {
                    Bitmap alarmBmp = alarm ? Properties.Resources.LED_RED : Properties.Resources.LED_GREEN;
                    dataGridViewAxis.Rows[target_row].Cells[target_column + (int)UIAXISINFO.ALARM].Value = alarmBmp;
                    alarmOLD[i] = alarm;
                }
            }

            refreshData = false;
        }

        private void button_SelectAxis_Click(object sender, EventArgs e)
        {
            button_SelectProcess.ButtonPush = false;
            viewInfo = VIEW.NONE;

            timer1.Enabled = false;
            BuildAxisInfo();
            refreshData = true;
            timer1.Enabled = true;
            viewInfo = VIEW.AXISINFO;
        }

        private void button_SelectProcess_Click(object sender, EventArgs e)
        {
            button_SelectAxis.ButtonPush = false;
            viewInfo = VIEW.NONE;

            timer1.Enabled = false;
            BuildProcessInfo();
            refreshData = true;
            timer1.Enabled = true;
            viewInfo = VIEW.PROCESSINFO;
        }


    }
}
