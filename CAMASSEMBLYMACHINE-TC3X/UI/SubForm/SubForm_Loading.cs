/* 
 * Copyright (C) Samsung Electronics Co., Ltd., Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * 
 * Written by Donghyun Ko <dh79.ko@samsung.com>, June 2022
 * Written by Sanghyeon Lee <sirano06.lee@samsung.com>, June 2022
 * 
 */
using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CAMASSEMBLYMACHINE.UI.SubForm
{
    public partial class SubForm_Loading : Form
    {
        private bool isCloseEnable;
        private static bool isCloseCall;
        private static double dCurrentProgressValue = 0;
        private static double dTargetProgressValue = 0;
        private static string strProgressDesc = string.Empty;
        private static bool isMsgError = false;

        public SubForm_Loading()
        {
            InitializeComponent();
            lbMachineName.Text = SystemDefine.machineName;
            lbVersion.Text = SystemDefine.pgmVersion;
            isCloseEnable = false;
            isCloseCall = false;
        }

        public static void ShowLoadingForm()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            Control mainWindow = Control.FromHandle(process.MainWindowHandle);
            dCurrentProgressValue = 0;
            dTargetProgressValue = 0;
            isCloseCall = false;
            Thread thread = new Thread(new ParameterizedThreadStart(ThreadShowWait));
            thread.Start(new object[] { mainWindow });
        }

        public static void ReportProgress(double _dProgressValue, string _strProgressDesc = "", bool isError = false)
        {
            dTargetProgressValue = _dProgressValue;
            if (_strProgressDesc != string.Empty) strProgressDesc = _strProgressDesc;
            isMsgError = isError;
        }

        public static void CloseLoadingForm(Form formFront)
        {
            //Thread의 loop 를 멈춘다.
            isCloseCall = true;

            //주어진 폼을 맨 앞으로
            SetForegroundWindow(formFront.Handle);
            formFront.BringToFront();
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static void ThreadShowWait(object obj)
        {
            object[] objParam = obj as object[];
            SubForm_Loading loadingForm = new SubForm_Loading();
            Control mainWindow = objParam[0] as Control;

            if (mainWindow != null)
            {
                //메인 윈도를 알 때에는 메인 윈도의 중앙
                loadingForm.StartPosition = FormStartPosition.Manual;
                loadingForm.Location = new Point(
                    mainWindow.Location.X + (mainWindow.Width - loadingForm.Width) / 2,
                    mainWindow.Location.Y + (mainWindow.Height - loadingForm.Height) / 2);
            }
            else
            {
                //메인 윈도를 모를 땐 스크린 중앙
                loadingForm.StartPosition = FormStartPosition.CenterScreen;
            }

            loadingForm.Show();
            loadingForm.BringToFront();

            //닫기 명령이 올 때 가지 0.03 초 단위로 루프
            while (!isCloseCall)
            {
                if (dCurrentProgressValue < dTargetProgressValue) dCurrentProgressValue++;
                //else dCurrentProgressValue += 0.03;
                if (dCurrentProgressValue > 100) dCurrentProgressValue = 100;
                loadingForm.progressBar1.Value = (int)dCurrentProgressValue;
                loadingForm.lblPercent.Text = $"{(int)dCurrentProgressValue}%";
                loadingForm.lblProgressDesc.Text = strProgressDesc;
                if (isMsgError && loadingForm.lblProgressDesc.ForeColor != Color.Red)
                {
                    loadingForm.lblProgressDesc.ForeColor = Color.Red;
                }
                else if(!isMsgError && loadingForm.lblProgressDesc.ForeColor != Color.Black)
                {
                    loadingForm.lblProgressDesc.ForeColor = Color.Black;
                }
                Application.DoEvents();
                Thread.Sleep(30);
            }

            //닫는다.
            if (loadingForm != null)
            {
                loadingForm.ForceClose();
                loadingForm = null;
            }
        }

        // 사용자에 의한 강제 종료 방지
        protected override void OnClosing(CancelEventArgs e)
        {
            if (isCloseEnable)
            {
                e.Cancel = false;
                return;
            }

            base.OnClosing(e);
        }

        public void ForceClose()
        {
            isCloseEnable = true;
            this.Close();
        }
    }
}
