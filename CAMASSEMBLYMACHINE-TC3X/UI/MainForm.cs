using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.UI.SubForm;
using TopEng.Controls;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    /**
     * @brief Auto tab에 해당하는 UI
     * @todo 물류 프로그램 제작자가 Form 구성 및 기능 구현 진행 해야함
     */
    public partial class MainForm : Form
    {
        public static MainForm mainForm;

        // PANELS
        Form_PaneTop paneTop;
        Form_PaneBottom paneBottom;
        Form_PaneMenu paneMenu;

        // PAGES
        public enum PAGEID
        {
            PAGE_AUTO,
            PAGE_MANUAL,
            PAGE_DATA,
            PAGE_TEACH,
            PAGE_LOG,
            PAGE_EXIT,

            PAGE_UNKNOWN
        }
        PAGEID currPage = PAGEID.PAGE_UNKNOWN;
        PAGEID currPagePrev = PAGEID.PAGE_UNKNOWN;

        // PAGES
        private Form_Auto pageAuto;
        private Form_Manual pageManual;
        private Form_Data2 pageData;
        private Form_Teaching pageTeach;
        private Form_Log pageLog;


        private Thread logThread;
        private Thread proclogThread;
        private Thread edmThread;

        private SubForm_Warning warningForm;

        public MainForm()
        {
            SubForm_Loading.ShowLoadingForm();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mainForm = this;
            bool isCheck = true;

            SubForm_Loading.ReportProgress(0, "Init Threads for Logs, Capture, Data Recovery");
            InitLogThread();
            SubForm_Loading.ReportProgress(20, "Recovery Data, Init Motion n IO, Auto Threads");
            isCheck = Machine.Initialize();
            SubForm_Loading.ReportProgress(30, $"Change model to {SystemDefine.modelname}");
            isCheck = Machine.ChangeModel(SystemDefine.modelname);
            SubForm_Loading.ReportProgress(40, "Open Camera, Init Vision & Valid License");
            Vision.Initialize();
            SubForm_Loading.ReportProgress(70, $"Change Vision model to {SystemDefine.modelname}");
            Vision.inspection.ChangeModel(SystemDefine.modelname);
            SubForm_Loading.ReportProgress(90, "Create Forms, Pages");

            if (!isCheck)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, "File Load Error!\n To continue, Close the following applications");
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();

                SubForm_Loading.CloseLoadingForm(this);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Application.ExitThread();
                Environment.Exit(0);
                return;
            }

            CreatePane();
            CreatePage();
            ChangePage(PAGEID.PAGE_AUTO);

            SubForm_Loading.ReportProgress(100, "Program Start");

            paneTop.MachineName = SystemDefine.systemName;
            paneTop.Version = SystemDefine.pgmVersion;

            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Program Start", CONTENT_TYPE.INFO);
            Machine.sysMode = Machine.SYSMODE.AUTO;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            SubForm_Loading.CloseLoadingForm(this);
            //if (!Machine.DeveloperMode)
            //{
            //    var form = new Form_Origin2();
            //    form.AutoOrigin();
            //}
        }

        public void MinimalizeWindow()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void ProgramExit()
        {
            string msg = SystemDefine.machineName + "\nDo you want to quit the application?";
            if (DialogResult.Yes != MessageBox.Show(this, msg, SystemDefine.pgmVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                return;

            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Program Exit", CONTENT_TYPE.INFO);

            System.Diagnostics.Process.GetCurrentProcess().Kill();

            Application.ExitThread();
            Environment.Exit(0);
        }

        private void CreatePane()
        {
            // TOP
            paneTop = new Form_PaneTop();
            paneTop.TopLevel = false;
            paneTop.Parent = panel_mainTop;
            paneTop.Dock = DockStyle.Fill;
            paneTop.Show();

            paneTop.MachineName = Program.GetAppTitle();

            // BOTTOM
            paneBottom = new Form_PaneBottom();
            paneBottom.TopLevel = false;
            paneBottom.Parent = panel_mainBottom;
            paneBottom.Dock = DockStyle.Fill;
            paneBottom.Show();
            paneBottom.ChangePage += new Form_PaneBottom.cbChangePageEvent(ChangePage);
            paneBottom.MinimalizeWindow += new Form_PaneBottom.cbMinimalizeWindowEvent(MinimalizeWindow);

            foreach (var proc in Machine.proclist)
                proc.m_cbMessageEventFunc += new Process.IProcess.MessageCallbackEvent(paneBottom.UpdateLogUI);

            // MENU
            paneMenu = new Form_PaneMenu();
            paneMenu.TopLevel = false;
            paneMenu.Parent = panel_mainMenu;
            paneMenu.Dock = DockStyle.Fill;
            paneMenu.Show();
            paneMenu.ChangePage += new Form_PaneMenu.cbChangePageEvent(ChangePage);

            Machine.proclist[(int)Machine.PROCESS.JIGWORK].m_cbUpdateTacTime += new Process.IProcess.cbUpdateTactTimeEvent(paneMenu.UpdateTactTime);
        }

        private void CreatePage()
        {
            pageAuto = new Form_Auto();
            pageAuto.TopLevel = false;
            pageAuto.Parent = panel_mainPage;
            pageAuto.Dock = DockStyle.Fill;

            pageManual = new Form_Manual();
            pageManual.TopLevel = false;
            pageManual.Parent = panel_mainPage;
            pageManual.Dock = DockStyle.Fill;

            pageData = new Form_Data2();
            pageData.TopLevel = false;
            pageData.Parent = panel_mainPage;
            pageData.Dock = DockStyle.Fill;

            pageTeach = new Form_Teaching();
            pageTeach.TopLevel = false;
            pageTeach.Parent = panel_mainPage;
            pageTeach.Dock = DockStyle.Fill;

            pageLog = new Form_Log();
            pageLog.TopLevel = false;
            pageLog.Parent = panel_mainPage;
            pageLog.Dock = DockStyle.Fill;
        }

        public void ChangePage(PAGEID page)
        {
            if (currPage == page/* || Machine.Busy()*/)
            {
                if (page == PAGEID.PAGE_LOG)
                {
                    ChangePage(currPagePrev);
                    currPagePrev = PAGEID.PAGE_UNKNOWN;
                    return;
                }
                else if (page != PAGEID.PAGE_EXIT)
                    return;
            }

            // 개발자 모드에서는 패스워드 작업을 하지 않는다.
            if (!Machine.DeveloperMode)
            {
                switch (page)
                {
                    case PAGEID.PAGE_DATA:
                    case PAGEID.PAGE_TEACH:
                        SubForm_Login dlg = new SubForm_Login(SystemDefine.USER_LEVEL.AUTH_DATA);
                        if (DialogResult.OK != dlg.ShowDialog())
                            return;
                        break;
                }
            }
            pageAuto.StopTimerAll();
            pageAuto.Hide();
            pageData.Hide();
            pageManual.StopTimerAll();
            pageManual.Hide();
            pageTeach.StopTimerAll();
            pageTeach.Hide();
            pageLog.Hide();

            switch (page)
            {
                case PAGEID.PAGE_AUTO:
                    pageAuto.Show();
                    pageAuto.StartTimer();
                    pageAuto.ConnectDisplay();
                    Machine.sysMode = Machine.SYSMODE.AUTO;
                    break;

                case PAGEID.PAGE_DATA:
                    pageData.Show();
                    pageData.ChangePage(Form_Data2.DATAPAGE.DATA_MENU);
                    Machine.sysMode = Machine.SYSMODE.TEACH;
                    break;

                case PAGEID.PAGE_MANUAL:
                    pageManual.Show();
                    pageManual.ChangePage(Form_Manual.MANUALPAGE.TRAY_CONV);
                    pageManual.StartTimer();
                    Machine.sysMode = Machine.SYSMODE.MANUAL;
                    break;

                case PAGEID.PAGE_TEACH:
                    Machine.sysMode = Machine.SYSMODE.TEACH;
                    pageTeach.Show();
                    pageTeach.ChangePage(Form_Teaching.TEACHPAGE.TRAY);
                    pageTeach.StartTimer(true);
                    break;

                case PAGEID.PAGE_LOG:
                    pageLog.Show();
                    pageLog.ChangePage(Form_Log.DATAPAGE.ERROR);
                    currPagePrev = currPage;
                    break;

                case PAGEID.PAGE_EXIT:
                    ProgramExit();
                    break;
            }

            currPage = page;

            if (currPage != PAGEID.PAGE_AUTO)
                Machine.IO.SetOut((int)DO.BYPASS_ON, 1);
        }

        private void InitLogThread()
        {
            try
            {
                logThread = new Thread(LogUtil.Instance.ThreadRun);
                logThread.IsBackground = true;
                logThread.Start();

                proclogThread = new Thread(ProcLogUtil.Instance.ThreadRun);
                proclogThread.IsBackground = true;
                proclogThread.Start();

                edmThread = new Thread(EDMUtil.Instance(SystemDefine.EDMLogPath).ThreadRun);
                edmThread.IsBackground = true;
                edmThread.Start();
            }
            catch (Exception ex)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.EXCEPTION);
                return;
            }
        }

        public void ShowWarning(string message)
        {
            if (warningForm != null)
                warningForm.Dispose();
            warningForm = new SubForm_Warning(message);
            warningForm.Show();
        }

        /**
         * @brief MainForm 종료시 호출되는 함수
         * @details observer 구독 취소 및 thread 종료를 수행하고 자원을 반환한다
         */
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "PGM exit", CONTENT_TYPE.DEBUG);
            Application.ExitThread();
            Environment.Exit(0);
        }

        public void AppForceClose(string ErrorMsg)
        {
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, ErrorMsg);
            formErr.TopLevel = true;
            formErr.TopMost = true;
            formErr.ShowDialog();

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.ExitThread();
            Environment.Exit(0);
        }

        public void ShowHiddenGroup()
        {
            if (currPage == PAGEID.PAGE_DATA)
                pageData.ShowHiddenGroup();
        }
    }
}
