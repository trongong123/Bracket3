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
    public partial class Form_ProcessTestView : Form
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

        public Form_ProcessTestView()
        {
            InitializeComponent();
        }

        private void Form_ProcessView_Load(object sender, EventArgs e)
        {
            Point Loc = new Point();
            Loc.X = MainForm.mainForm.Location.X - 7;
            Loc.Y = MainForm.mainForm.Location.Y + MainForm.mainForm.Size.Height - Size.Height + 7;
            Location = Loc;

            //button_SelectAxis.ButtonPush = true;
            //BuildAxisInfo();
            //refreshData = true;
            //timer1.Enabled = true;
            //viewInfo = VIEW.AXISINFO;

            textBox1.Text = "";

            button_SelectAxis.ButtonPush = false;
            viewInfo = VIEW.NONE;
            timer1.Enabled = false;
            BuildProcessInfo();
            refreshData = true;
            timer1.Enabled = true;
            viewInfo = VIEW.PROCESSINFO;
        }

        private void BuildProcessInfo()
        {
            string[] headername = { "Process Name", "Step", "Msg" , "SendMsg", "Step", "SendMsgS"};
            int[] headersize = { 200, 200, 200, 100, 200, 100};
            string[] headertype = { "TEXT", "TEXT", "ComboM", "ButtonM", "ComboS", "ButtonS" };
            dataGridViewAxis.ReadOnly = false;
            dataGridViewAxis.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                int target_column = i % headername.Length;

                if (headertype[target_column] == "TEXT")
                {
                    dataGridViewAxis.Columns.Add(headername[target_column], headername[target_column]);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dataGridViewAxis.Columns[i].ReadOnly = true;
                }
                else if (headertype[target_column] == "ComboM")
                {
                    DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
                    newColumn.HeaderText = headername[target_column];
                    newColumn.Name = headername[target_column];

                    dataGridViewAxis.Columns.Add(newColumn);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (headertype[target_column] == "ComboS")
                {
                    DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn();
                    newColumn.HeaderText = headername[target_column];
                    newColumn.Name = headername[target_column];

                    dataGridViewAxis.Columns.Add(newColumn);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (headertype[target_column] == "ButtonM")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[target_column];
                    newColumn.Name = headername[target_column];
                    newColumn.Text = "Msg Test";
                    newColumn.UseColumnTextForButtonValue = true; // Text 속성을 버튼 값으로 사용
                    dataGridViewAxis.Columns.Add(newColumn);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (headertype[target_column] == "ButtonS")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[target_column];
                    newColumn.Name = headername[target_column];
                    newColumn.Text = "Step Test";
                    newColumn.UseColumnTextForButtonValue = true; // Text 속성을 버튼 값으로 사용
                    dataGridViewAxis.Columns.Add(newColumn);
                    dataGridViewAxis.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                dataGridViewAxis.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;                
                dataGridViewAxis.Columns[i].Width = headersize[target_column];
            }

            string[] procname = { "PROC_HOME", "PROC_LOAD", "PROC_DECO", "PROC_GRIPPER", "PROC_SIDE_A", "PROC_SIDE_C", "PROC_UNLOAD", "PROC_NG" };

            procname = new string[Machine.proclist.Count - 1];

            int pN = 0;
            foreach (var proc in Machine.proclist)
            {
                if (pN == 0 || pN == 1)
                {
                    pN++;
                    continue;
                }
                procname[pN - 1] = ((Machine.PROCESS)pN).ToString();
                ++pN;
            }

            // ROWS
            for (int i = 0; i < procname.Length; i++)
            {
                int target_column = (i / MAXROWS) * headername.Length;
                int target_row = i % MAXROWS;
                dataGridViewAxis.Rows.Add();
                dataGridViewAxis.Rows[target_row].Cells[target_column + 0].Value = procname[i];
                dataGridViewAxis.Rows[target_row].Cells[target_column + 1].Value = "IDLE";

                DataGridViewComboBoxCell comboColM = dataGridViewAxis.Rows[target_row].Cells[target_column + 2] 
                    as DataGridViewComboBoxCell;
                List<string> listMsgName = Enum.GetNames(Machine.proclist[i + 1].MsgEnumType).ToList();
                for (int j = 0; j < listMsgName.Count; ++j)
                    comboColM.Items.Add(listMsgName[j]);
                if (comboColM.Items.Count > 0)
                    dataGridViewAxis.Rows[target_row].Cells[target_column + 2].Value = comboColM.Items[0];

                DataGridViewComboBoxCell comboColS = dataGridViewAxis.Rows[target_row].Cells[target_column + 4] 
                    as DataGridViewComboBoxCell;
                List<string> listSingleName = Enum.GetNames(Machine.proclist[i + 1].StepEnumType).ToList();
                for (int j = 0; j < listSingleName.Count; ++j)
                    comboColS.Items.Add(listSingleName[j]);
                if (comboColS.Items.Count > 0)
                    dataGridViewAxis.Rows[target_row].Cells[target_column + 4].Value = comboColS.Items[0];
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
                    //newColumn.Image = Properties.Resources.LED_GRAY;
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
            }

            dataGridViewAxis.CurrentCell = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label_MachineState.Text = Machine.status.state.ToString();

            if (viewInfo == VIEW.AXISINFO)
                UpdateAxisInfo();
            if (viewInfo == VIEW.PROCESSINFO)
                UpdateProcessInfo();

            refreshData = false;
        }

        private void UpdateProcessInfo()
        {
            for (int i = 1; i < (int)Machine.proclist.Count; ++i)
            {
                UpdateProcessData(i, Machine.proclist[i], refreshData);
            }
        }

        private void UpdateAxisInfo()
        {
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
            textBox1.Text = "";

            button_SelectAxis.ButtonPush = false;
            viewInfo = VIEW.NONE;
            timer1.Enabled = false;
            BuildProcessInfo();
            refreshData = true;
            timer1.Enabled = true;
            viewInfo = VIEW.PROCESSINFO;
        }

        private void UpdateProcessData(int procIndex, IProcess process, bool refreshData)
        {
            // Proc = procIndex; (지역 변수를 사용하므로 전역 Proc 변수 필요 없음)
            int Step = process.GetStep();
            int AutoStep = process.GetAutoStep();

            // procIndex는 Machine.PROCESS Enum 값이므로, 1부터 시작하는 배열 인덱스를 위해 -1 사용
            int rowIndex = procIndex - 1;

            // 업데이트 조건 확인
            if (lastStep[rowIndex] != Step || refreshData)
            {
                // 1. Enum.GetName(): IProcess.StepEnumType을 사용하여 캐스팅 없이 동적으로 처리
                dataGridViewAxis.Rows[rowIndex].Cells[1].Value = Enum.GetName(process.StepEnumType, Step);
                // 마지막 상태 저장
                lastStep[rowIndex] = Step;
                
                textBox1.Text += dataGridViewAxis.Rows[rowIndex].Cells[1].Value + "\r\n";
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Focus();
            }
        }

        private void dataGridViewAxis_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 3 && e.ColumnIndex != 5)
                return;

            if (MessageBox.Show("프로세스 테스트를 실행하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            textBox1.Text = "Process " + ((Machine.PROCESS)e.RowIndex + 1).ToString() + " Start" + "\r\n";

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // 클릭된 컬럼의 이름을 가져옵니다.
                string columnName = dataGridViewAxis.Columns[e.ColumnIndex].Name;

                DataGridViewRow clickedRow = dataGridViewAxis.Rows[e.RowIndex];
                int rowIndex = e.RowIndex;
                var proc = Machine.proclist[e.RowIndex + 1];
                DataGridViewComboBoxCell comboCol = dataGridViewAxis.Rows[e.RowIndex].Cells[e.ColumnIndex - 1]
                    as DataGridViewComboBoxCell;

                if (columnName == "SendMsg")
                {
                    string strM = comboCol.Value.ToString();
                    textBox1.Text += strM + "\r\n";
                    object MsgEnumValue = Enum.Parse(proc.MsgEnumType, strM);
                    int MsgValue = (int)MsgEnumValue;
                    proc.SetMessage(MsgValue, 0);
                }
                else if (columnName == "SendMsgS")
                {
                    string strS = comboCol.Value.ToString();
                    textBox1.Text += "MSG_SINGLE" + "\r\n";
                    object StepEnumValue = Enum.Parse(proc.StepEnumType, strS);
                    int stepValue = (int)StepEnumValue;
                    object MsgEnumValue = Enum.Parse(proc.MsgEnumType, "MSG_SINGLE");
                    proc.SetMessage((int)MsgEnumValue, stepValue);
                }
            }
        }
    }
}
