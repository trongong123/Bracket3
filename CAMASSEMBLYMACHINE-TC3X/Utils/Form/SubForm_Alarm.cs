using CAMASSEMBLYMACHINE;
using CAMASSEMBLYMACHINE.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;

namespace TopEng.Utils
{
    public partial class SubForm_Alarm : Form
    {
        int SoundTime = 0;
        bool SoundOn = false;
        StopWatch stopWatch = new StopWatch();
        private Dictionary<int, ALARMINFO> dic_alarm = new Dictionary<int, ALARMINFO>();
        public ButtonEnh[] arrDispAlarm = new ButtonEnh[(int)ALARMINFO.UNIT.MAXUNIT];

        public delegate void BuzzerSoundOffDelegate();
        public event BuzzerSoundOffDelegate BuzzerSoundOffEvent;
        public delegate void BuzzerSoundOnDelegate();
        public event BuzzerSoundOffDelegate BuzzerSoundOnEvent;

        public SubForm_Alarm()
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

            dataGridView1.Rows.Insert(0);
            dataGridView1.Rows[0].Cells[0].Value = alramInfo.Code;
            dataGridView1.Rows[0].Cells[1].Value = alramInfo.Message;

            if(arrDispAlarm[(int)alramInfo.Unit] != null)
                arrDispAlarm[(int)alramInfo.Unit].ButtonPush = false;
        }

        public void Remove(int alarmCode)
        {
            if (!dic_alarm.ContainsKey(alarmCode))
                return;
            dic_alarm.Remove(alarmCode);
        }

        public void Clear()
        {
            if (hasSafetyError())
                return;

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
            Machine.AlarmClear();

            if (BuzzerSoundOffEvent != null)
                BuzzerSoundOffEvent();
            SoundOn = false;
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
            if (hasSafetyError())
                return;

            if (BuzzerSoundOffEvent != null)
                BuzzerSoundOffEvent();
            SoundOn = false;
        }

        public void InitDispSignal()
        {
            for (int i = (int)ALARMINFO.UNIT.TRAYINBUF; i < (int)ALARMINFO.UNIT.MAXUNIT; ++i)
                arrDispAlarm[i].ButtonPush = true;
        }
    }
}