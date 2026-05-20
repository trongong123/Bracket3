using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using CAMASSEMBLYMACHINE.Process;
using NV_UI;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_Initialize2 : Form
    {
        public enum BTN_SELECT
        {
            EXIT,
            TRAY,
            PROD_LOADER,
            BUFFER,
            ASSEMBLY,
            SELECT_ALL,
            UNSELECT_ALL,
            INITIALIZE,
        }

        public enum INIT_STEP
        {
            NONE,
            INIT_START,
            INIT_WAIT_UNTIL_COMPLETE,
            INIT_DONE,
            ERROR,
        }

        private bool[] bSelectedUnitArray;
        private INIT_STEP eCurrentStep;
        //private ECODE eErrorCode;
        private bool bIsInitializing;
        //private CustomTimer customTimer;
        private MainForm main;
        //private SystemManager systemManager;
        Machine.SYSMODE sysmodeLast = Machine.SYSMODE.MANUAL;
        bool updatingNow = false;

        public Form_Initialize2()
        {
            InitializeComponent();
            this.Location = new Point(MainForm.mainForm.Location.X, MainForm.mainForm.Location.Y + 89);

            sysmodeLast = Machine.sysMode;
            Machine.sysMode = Machine.SYSMODE.MANUAL;

            nV_Button_TG_NS_UnSelectAll.ClickEvent += PushButton_ClickEvent;
            nV_Button_TG_NS_SelectAll.ClickEvent += PushButton_ClickEvent;
            nV_Button_TG_NS_Initialize.ClickEvent += PushButton_ClickEvent;
            nV_Button_PB_NS_Exit.ClickEvent += PushButton_ClickEvent;
        }

        private void UnitSelectButton_ClickEvent(object sender, EventArgs e)
        {
            //string sTag = (sender as Control).Tag.ToString();
            //if (Enum.TryParse(sTag, out MACHINE_SUB_UNIT unitName))
            //{
            //    bool selected = (sender as NV_Button_TG_NS).IsSelected();
            //    bSelectedUnitArray[(int)unitName] = selected;
            //}
        }

        private void PushButton_ClickEvent(object sender, EventArgs e)
        {
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out BTN_SELECT selectedBtn))
            {
                switch (selectedBtn)
                {
                    case BTN_SELECT.EXIT:
                        Machine.sysMode = sysmodeLast;

                        //this.StopTimer();
                        this.Close();
                        //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "User clicked Exit button", CONTENT_TYPE.INF, MACHINE_SUB_UNIT.NONE, (int)ACTION_TYPE.INIT);
                        break;
                    case BTN_SELECT.UNSELECT_ALL:
                        nV_Button_TG_NS_Tray.UserOff();
                        nV_Button_TG_NS_ProdLoader.UserOff();
                        nV_Button_TG_NS_Buffer.UserOff();
                        nV_Button_TG_NS_Assembly.UserOff();
                        break;
                    case BTN_SELECT.SELECT_ALL:
                        nV_Button_TG_NS_Tray.UserOn();
                        nV_Button_TG_NS_ProdLoader.UserOn();
                        nV_Button_TG_NS_Buffer.UserOn();
                        nV_Button_TG_NS_Assembly.UserOn();
                        break;
                    case BTN_SELECT.INITIALIZE:
                        Dlg_MessageBox formMove = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("Do you want to Initialize ?"));
                        if (formMove.ShowDialog() == DialogResult.Yes)
                        {
                            var proc = Machine.proclist[(int)Machine.PROCESS.HOME] as ProcessHOME;
                            proc.init_tray = nV_Button_TG_NS_Tray.check;
                            proc.init_prodLoader = nV_Button_TG_NS_ProdLoader.check;
                            proc.init_jig = nV_Button_TG_NS_Assembly.check;
                            proc.init_buffer = nV_Button_TG_NS_Buffer.check;
                            proc.SetMessage((int)ProcessHOME.MSG.MSG_INITIALIZE);
                        }
                        break;
                }
            }
        }
        private void AllSelectInitPartButtons(Panel pnl, bool bSelectOrDeselect)
        {
            foreach (NV_Button_TG_NS toggle in pnl.Controls)
            {
                if (bSelectOrDeselect)
                {
                    toggle.UserOn();
                }
                else
                {
                    toggle.UserOff();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updatingNow)
                return;
            updatingNow = true;

            updatingNow = false;
        }

        public void SetInitialize()
        {
            //새로 만든거

            bool isInitTrayLoader = nV_Button_TG_NS_Tray.IsSelected();
            bool isInitBuffer = nV_Button_TG_NS_Buffer.IsSelected();
            bool isInitAssembly = nV_Button_TG_NS_Assembly.IsSelected();

            if (isInitTrayLoader)
            {
                Machine.ClearTray();
                Machine.Parts[(int)UNITPART.PROD_PICK1].exist = false;
                Machine.Parts[(int)UNITPART.PROD_PICK2].exist = false;
            }

            if (isInitBuffer)
            {
                for (int i = (int)UNITPART.CAM_TRF_L1; i <= (int)UNITPART.BUF2_R2; ++i)
                {
                    Machine.Parts[i].exist = false;
                }
            }

            if (isInitAssembly)
            {
                Machine.Parts[(int)UNITPART.ASSEMBLER1].exist = false;
                Machine.Parts[(int)UNITPART.ASSEMBLER2].exist = false;
                Machine.ClearJigs();
            }
        }

        private void ButtonEnable(bool bEnable)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => ButtonEnable(bEnable)));
            }
            else
            {
                nV_Button_TG_NS_Tray.Enabled = bEnable;
                nV_Button_TG_NS_Buffer.Enabled = bEnable;

                nV_Button_TG_NS_UnSelectAll.Enabled = bEnable;
                nV_Button_TG_NS_SelectAll.Enabled = bEnable;
                nV_Button_TG_NS_Initialize.Enabled = bEnable;
                nV_Button_PB_NS_Exit.Enabled = bEnable;
            }
        }

        private void ResetTaskDone()
        {
            ButtonEnable(true);
            this.eCurrentStep = INIT_STEP.NONE;
        }
    }
}
