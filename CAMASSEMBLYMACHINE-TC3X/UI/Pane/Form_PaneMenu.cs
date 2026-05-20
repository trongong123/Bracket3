using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.UI.SubForm;
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
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using Microsoft.Win32;
using TopEng.Controls;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_PaneMenu : Form
    {
        public delegate void cbChangePageEvent(MainForm.PAGEID page);
        public event cbChangePageEvent ChangePage;
        public delegate void cbChangeSystemInfoEvent();
        public event cbChangeSystemInfoEvent ChangeSystemInfo;

        public Form_PaneMenu()
        {
            InitializeComponent();

            btnStart.Enabled = true;
            btnStop.ButtonPush = false;

            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathInfoReg, true);
            if (wregKey == null)
                wregKey = Registry.CurrentUser.CreateSubKey(SystemDefine.pathInfoReg);

            Machine.totalTime = Convert.ToDouble(wregKey.GetValue("Tact Time Total", 0.00));
            Machine.totalCount = Convert.ToInt32(wregKey.GetValue("Pass Count Total", 0));
        }

        public void UpdateTactTime(int unit, int ms)
        {
            try
            {
                if (disp_TactTime.InvokeRequired)
                {
                    Action action = delegate
                    {
                        disp_TactTime.Text = (0.001 * ms).ToString("0.00");
                        Machine.UpdateTactTime(0.001 * ms);
                    };
                    disp_TactTime.BeginInvoke(action);
                }
                else
                {
                    disp_TactTime.Text = (0.001 * ms).ToString("0.00");
                    Machine.UpdateTactTime(0.001 * ms);
                }
            }
            catch// (Exception e)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ChangeSystemOperate();
        }

        public void ChangeSystemOperate()
        {
            if (Machine.sysMode == Machine.SYSMODE.AUTO)
            {
                bool running = (Machine.status.state == SystemState.SystemStateRUN || Machine.status.state == SystemState.SystemStateRUNEMPTY || Machine.status.state == SystemState.SystemStateSLEEP);

                if (btnSelectMode.Enabled == running) btnSelectMode.Enabled = !running;
                if (btnOrigin.Enabled == running) btnOrigin.Enabled = !running;
                if (btnInitialize.Enabled == running) btnInitialize.Enabled = !running;

                if (btnStart.ButtonPush != running)
                {
                    bool enable = btnStart.Enabled;
                    btnStart.ButtonPush = running;
                    btnStart.Enabled = true;
                    btnStart.Enabled = enable;
                }
                if (btnStart.Enabled == running) btnStart.Enabled = !running;

                if (btnStop.ButtonPush == running) btnStop.ButtonPush = !running;
            }
            else
            {
                btnSelectMode.Enabled = true;
                btnOrigin.Enabled = true;
                btnInitialize.Enabled = true;
                btnStart.Enabled = false;
                btnStart.ButtonPush = false;
                btnStop.Enabled = true;
            }
        }

        private void btnSelectMode_Click(object sender, EventArgs e)
        {
            var form = new Form_SelectMode();
            form.ShowDialog();

            switch (Machine.status.mode)
            {
                case SystemMode.SystemModeAUTO:
                    btnSelectMode.ImageDefault = Properties.Resources.MODE_AUTO;
                    break;

                case SystemMode.SystemModeBYPASS:
                    btnSelectMode.ImageDefault = Properties.Resources.MODE_BYPASS;
                    break;

                case SystemMode.SystemModeDRYRUN:
                    btnSelectMode.ImageDefault = Properties.Resources.MODE_DRYRUN;
                    break;

                case SystemMode.SystemModePASSRUN:
                    btnSelectMode.ImageDefault = Properties.Resources.MODE_PASSRUN;
                    break;
            }
        }

        private void btnIOMonitoring_Click(object sender, EventArgs e)
        {
            var form = new Form_IOMonitor(Machine.IO);
            form.Show();
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            var form = new Form_Initialize2();
            form.ShowDialog();
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {
            //if (!Machine.DeveloperMode)
            //{
            //    SubForm_Login dlg = new SubForm_Login(SystemDefine.USER_LEVEL.AUTH_ORIGIN);
            //    if (DialogResult.OK != dlg.ShowDialog())
            //        return;
            //}

            var form = new Form_Origin2();
            form.ShowForm();
        }

        private void btnProductInfo_Click(object sender, EventArgs e)
        {
            var form = new Form_ProductInfo2();
            form.ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Machine.sysMode != Machine.SYSMODE.AUTO)
                return;

            //Form_SelectUnit unitForm = new Form_SelectUnit();
            //DialogResult dr = unitForm.ShowDialog();
            //if (dr == DialogResult.No) return;
            //else
            //{
            //btnStart.Enabled = false;
            //btnStop.ButtonPush = false;
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Start Button Click", CONTENT_TYPE.INFO);
            Machine.Start();

            //var form = new SubForm_ProcInit();
            //form.Show();

            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            proc.towerLampOff = false;
            //}
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Stop Button Click", CONTENT_TYPE.INFO);
            Machine.EStop(false, false);

            //Machine.status.state = SystemState.SystemStateIDLE;
            ////Program.errorProc.AlarmClear();
            //Machine.EStop(false);
            //btnStart.Enabled = true;
            //btnStop.ButtonPush = true;
            //var form = new SubForm_CycleStop();
            //form.ShowDialog();
        }

        private void button_ProcessView_Click(object sender, EventArgs e)
        {
            var form = new Form_ProcessView();
            form.Show();
        }

        private void button_InputStop_ClickEvent(object sender, EventArgs e)
        {
            Machine.PickupStop = (sender as NV_UI.NV_Button_TG).IsSelected();
        }

        private void button_OutputStop_ClickEvent(object sender, EventArgs e)
        {
            Machine.UnloadStop = (sender as NV_UI.NV_Button_TG).IsSelected();
        }

        private void disp_TactTime_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tact Time Clear Button Click", CONTENT_TYPE.INFO);

            Dlg_MessageBox formTarget = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("Do you want to Reset TactTime?"));
            if (formTarget.ShowDialog() == DialogResult.Yes)
                disp_TactTime.Text = (0.00).ToString("0.00");
        }
    }
}
