using CAMASSEMBLYMACHINE;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;
using CAMASSEMBLYMACHINE.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;

namespace TopEng.Utils
{
    public partial class SubForm_Alarm2 : Form
    {
        int SoundTime = 0;
        bool SoundOn = false;
        StopWatch stopWatch = new StopWatch();
        private Dictionary<int, ALARMINFO> dic_alarm = new Dictionary<int, ALARMINFO>();
        public ButtonEnh[] arrDispAlarm = new ButtonEnh[(int)ALARMINFO.UNIT.MAXUNIT];
        public bool[] arrNoAlarm = new bool[(int)ALARMINFO.UNIT.MAXUNIT];

        public delegate void BuzzerSoundOffDelegate();
        public event BuzzerSoundOffDelegate BuzzerSoundOffEvent;
        public delegate void BuzzerSoundOnDelegate();
        public event BuzzerSoundOffDelegate BuzzerSoundOnEvent;

        public SubForm_Alarm2()
        {
            InitializeComponent();
            CenterToScreen();
        }

        public void ResetEvent()
        {
            try
            {
                if (dataGridView1.InvokeRequired)
                {
                    Action action = delegate
                    {
                        dataGridView1.Rows.Clear();
                    };
                    dataGridView1.BeginInvoke(action);
                }
                else
                {
                    dataGridView1.Rows.Clear();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public int GetCount()
        {
            return dataGridView1.Rows.Count;
        }

        public bool Find(int code)
        {
            return dic_alarm.ContainsKey(code);
        }

        private void SubForm_Alarm_Load(object sender, EventArgs e)
        {
            InitializeParamInfo();

            arrDispAlarm[(int)ALARMINFO.UNIT.TRAYIN] = disp_TrayIn;
            arrDispAlarm[(int)ALARMINFO.UNIT.TRAYINBUF] = disp_TrayInBuf;
            arrDispAlarm[(int)ALARMINFO.UNIT.TRAYWORK] = disp_TrayWork;
            arrDispAlarm[(int)ALARMINFO.UNIT.TRAYOUTBUF] = disp_TrayOutBuf;
            arrDispAlarm[(int)ALARMINFO.UNIT.TRAYOUT] = disp_TrayOut;
            arrDispAlarm[(int)ALARMINFO.UNIT.LD_PICKER] = disp_ProdLoader;
            arrDispAlarm[(int)ALARMINFO.UNIT.ULD_PICKER] = disp_Assembler;
            arrDispAlarm[(int)ALARMINFO.UNIT.LEFTBUF] = disp_LeftBuf;
            arrDispAlarm[(int)ALARMINFO.UNIT.RIGHTBUF] = disp_RightBuf;
            arrDispAlarm[(int)ALARMINFO.UNIT.JIGINBUF] = disp_JigInBuf;
            arrDispAlarm[(int)ALARMINFO.UNIT.JIGWORK] = disp_JigWork;
            arrDispAlarm[(int)ALARMINFO.UNIT.JIGOUTBUF] = disp_JigOutBuf;
            arrDispAlarm[(int)ALARMINFO.UNIT.JIGCARRIER] = disp_JigCarrier;
            InitDispSignal();
        }

        private void Alarm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Code", "Alarm Histories" };
            int[] headersize = { 60, dataGridView1.Size.Width - 60 };

            dataGridView1.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                dataGridView1.Columns.Add(headername[i], headername[i]);
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[i].Width = headersize[i];
                dataGridView1.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        public bool hasSafetyError()
        {
            foreach (var alarm in dic_alarm)
            {
                if (alarm.Value.Category == ALARMINFO.CATEGORY.SAFE)
                    return true;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorErrorState)
                return true;
            if (proc.lightcurtainError)
                return true;

            return false;
        }

        public void Add(ALARMINFO alramInfo)
        {
            if (dic_alarm.ContainsKey(alramInfo.Code))
                return;

            dic_alarm.Add(alramInfo.Code, alramInfo);

            if (alramInfo.SoundTime >= 0)
            {
                SoundTime = alramInfo.SoundTime;
                stopWatch.Reset();
                if (BuzzerSoundOnEvent != null)
                    BuzzerSoundOnEvent();
                SoundOn = true;
            }

            bool isSafety = false;

            switch ((ECODE)alramInfo.Code)
            {
                case ECODE.EMEGENCY_STOP1_ON:
                    label_EName.Text = "FRONT EMO ON\n" +
                        "Đã nhấn nút EMO phía trước";
                    break;
                case ECODE.EMEGENCY_STOP2_ON:
                    label_EName.Text = "REAR EMO ON\n + " +
                        "Đã nhấn nút EMO phía sau";
                    break;
                case ECODE.MACHINE_LIGHT_CURTAIN_SENSED:
                    label_EName.Text = "LIGHT CURTAIN SENSED\n" +
                        "Light Curtain đã bị cảm biến";
                    break;
                case ECODE.LIGHT_CURTAIN1_SENSED_SOMETHING:
                    //CAM 1,2,3,4,6
                    label_EName.Text = "LIGHT CURTAIN IN SENSED\n" +
                        "Light Curtain ĐẦU VÀO đã bị cảm biến";
                    //Only CAM 5 or Delete
                    //label_EName.Text = "LIGHT CURTAIN LEFT SENSED\n" +
                    //   "Light Curtain TRÁI đã bị cảm biến"; //5번 TAPE 설비
                    break;
                case ECODE.LIGHT_CURTAIN2_SENSED_SOMETHING:
                    //CAM 1,2,3,4,6
                    label_EName.Text = "LIGHT CURTAIN OUT SENSED\n +" +
                        "Light Curtain ĐẦU RA đã bị cảm biến";
                    //Only CAM 5 or Delete
                    //label_EName.Text = "LIGHT CURTAIN RIGHT SENSED\n +" +
                    //    "Light Curtain PHẢI đã bị cảm biến"; //5번 TAPE 설비
                    break;
                case ECODE.MACHINE_DOOR_OPENED:
                    label_EName.Text = "DOOR OPENED\n + " +
                        "Cửa máy đang mở";
                    break;
                case ECODE.DOOR_FRONT_TOP1_OPEN:
                    label_EName.Text = "FRONT DOOR TOP 1 OPENED\n" +
                        "Cửa TRƯỚC TRÊN 1 đang mở";
                    break;
                case ECODE.DOOR_FRONT_TOP2_OPEN:
                    label_EName.Text = "FRONT DOOR TOP 2 OPENED\n" +
                        "Cửa TRƯỚC TRÊN 2 đang mở";
                    break;
                case ECODE.DOOR_FRONT_BOTTOM1_OPEN:
                    label_EName.Text = "FRONT DOOR BOTTOM 1 OPENED\n" +
                        "Cửa TRƯỚC DƯỚI 1 đang mở";
                    break;
                case ECODE.DOOR_FRONT_BOTTOM2_OPEN:
                    label_EName.Text = "FRONT DOOR BOTTOM 2 OPENED\n" +
                        "Cửa TRƯỚC DƯỚI 2 đang mở";
                    break;
                case ECODE.DOOR_REAR_TOP1_OPEN:
                    label_EName.Text = "REAR DOOR TOP 1 OPENED\n" +
                        "Cửa SAU TRÊN 1 đang mở";
                    break;
                case ECODE.DOOR_REAR_TOP2_OPEN:
                    label_EName.Text = "REAR DOOR TOP 2 OPENED\n" +
                        "Cửa SAU TRÊN 2 đang mở";
                    break;
                case ECODE.DOOR_SIDE_TOP1_OPEN:
                    label_EName.Text = "SIDE DOOR TOP 1 OPENED\n" +
                        "Cửa HÔNG TRÊN 1 đang mở";
                    break;
                case ECODE.DOOR_SIDE_TOP2_OPEN:
                    label_EName.Text = "SIDE DOOR TOP 2 OPENED\n" +
                        "Cửa HÔNG TRÊN 2 đang mở";
                    break;
                case ECODE.DOOR_SIDE_TOP3_OPEN:
                    label_EName.Text = "SIDE DOOR TOP 3 OPENED\n" +
                        "Cửa HÔNG TRÊN 3 đang mở";
                    break;
                case ECODE.DOOR_SIDE_TOP4_OPEN:
                    label_EName.Text = "SIDE DOOR TOP 4 OPENED\n" +
                        "Cửa HÔNG TRÊN 4 đang mở";
                    break;
                case ECODE.DOOR_SIDE_TOP5_OPEN:
                    label_EName.Text = "SIDE DOOR TOP 5 OPENED\n" +
                        "Cửa HÔNG TRÊN 5 đang mở";
                    break;
                default:
                    break;
            }

            if ((ECODE)alramInfo.Code == ECODE.EMEGENCY_STOP1_ON
                || (ECODE)alramInfo.Code == ECODE.EMEGENCY_STOP2_ON)
            {
                isSafety = true;
                panel1.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.WARNING_EMGSTOP;
                panel1.BackgroundImageLayout = ImageLayout.Zoom;
            }
            else if ((ECODE)alramInfo.Code == ECODE.MACHINE_DOOR_OPENED
                || (ECODE)alramInfo.Code == ECODE.DOOR_FRONT_TOP1_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_FRONT_TOP2_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_FRONT_BOTTOM1_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_FRONT_BOTTOM2_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_REAR_TOP1_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_REAR_TOP2_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_SIDE_TOP1_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_SIDE_TOP2_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_SIDE_TOP3_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_SIDE_TOP4_OPEN
                || (ECODE)alramInfo.Code == ECODE.DOOR_SIDE_TOP5_OPEN
                )
            {
                isSafety = true;
                panel1.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.WARNING_DOOR;
                panel1.BackgroundImageLayout = ImageLayout.Zoom;
            }
            else if ((ECODE)alramInfo.Code == ECODE.MACHINE_LIGHT_CURTAIN_SENSED
                || (ECODE)alramInfo.Code == ECODE.LIGHT_CURTAIN1_SENSED_SOMETHING
                || (ECODE)alramInfo.Code == ECODE.LIGHT_CURTAIN2_SENSED_SOMETHING)
            {
                isSafety = true;
                panel1.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.WARNING_LIGHTCURTAIN;
                panel1.BackgroundImageLayout = ImageLayout.Zoom;
            }
            else
                isSafety = false;

            if (isSafety)
            {
                tableLayoutPanel3.SetRow(tableLayoutPanel2, 1);
                tableLayoutPanel3.SetRowSpan(tableLayoutPanel2, 1);
                tableLayoutPanel2.Hide();

                tableLayoutPanel4.Controls.Remove(label_EName);
                tableLayoutPanel3.Controls.Add(label_EName, 0, 0);
                label_EName.Dock = DockStyle.Fill;
                tableLayoutPanel3.SetColumn(label_EName, 0);
                tableLayoutPanel3.SetRow(label_EName, 0);
                label_EName.Show();
            }
            else
            {
                tableLayoutPanel3.Controls.Remove(label_EName);
                tableLayoutPanel4.Controls.Add(label_EName, 0, 0);
                label_EName.Hide();
                panel1.BackgroundImage = null;
                tableLayoutPanel3.SetRow(tableLayoutPanel2, 0);
                tableLayoutPanel3.SetRowSpan(tableLayoutPanel2, 2);
                tableLayoutPanel2.Show();
            }

            dataGridView1.Rows.Insert(0);
            dataGridView1.Rows[0].Cells[0].Value = alramInfo.Code;
            dataGridView1.Rows[0].Cells[1].Value = alramInfo.Message;

            if (arrDispAlarm[(int)alramInfo.Unit] != null)
            {
                arrDispAlarm[(int)alramInfo.Unit].ButtonPush = false;
                arrNoAlarm[(int)alramInfo.Unit] = false;
            }

            Thread blinkAlarm = new Thread(() => this.OnBlinkThread((int)alramInfo.Unit));
            blinkAlarm.Start();
        }

        public void Remove(int alarmCode)
        {
            if (!dic_alarm.ContainsKey(alarmCode))
                return;
            dic_alarm.Remove(alarmCode);
        }

        public void Clear()
        {
            dataGridView1.Rows.Clear();
            dic_alarm.Clear();
            InitDispSignal();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            if (hasSafetyError())
                return;

            Action action = delegate
            {
                for (int i = 0; i < Dlg_MessageBox.listMsgBox.Count; ++i)
                {
                    Dlg_MessageBox dlg = Dlg_MessageBox.listMsgBox[i];
                    dlg.DialogResult = DialogResult.Yes;
                    dlg.Close();
                }
                Dlg_MessageBox.listMsgBox.Clear();
            };
            MainForm.mainForm.BeginInvoke(action);
            Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (stopWatch.Elapsed > 1000 * SoundTime && SoundTime > 0 && SoundOn)
            {
                if (BuzzerSoundOffEvent != null)
                    BuzzerSoundOffEvent();
                SoundOn = false;
            }
        }

        private void button_Mute_Click(object sender, EventArgs e)
        {            
            if (BuzzerSoundOffEvent != null)
                BuzzerSoundOffEvent();
            SoundOn = false;
        }

        public void InitDispSignal()
        {
            for (int i = (int)ALARMINFO.UNIT.TRAYIN; i < (int)ALARMINFO.UNIT.MAXUNIT; ++i)
            {
                if (arrDispAlarm[i] != null)
                {
                    arrDispAlarm[i].ButtonPush = true;
                    arrNoAlarm[i] = true;
                }
            }
        }

        public void OnBlinkThread(int num)
        {
            try
            {
                while (!arrNoAlarm[num])
                {
                    if (arrDispAlarm[num] != null)
                    {
                        Thread.Sleep(100);
                        arrDispAlarm[num].ButtonPush = false;
                        Thread.Sleep(100);
                        arrDispAlarm[num].ButtonPush = true;
                    }
                    else
                        break;
                }
            }
            catch (Exception) { }
        }
    }
}