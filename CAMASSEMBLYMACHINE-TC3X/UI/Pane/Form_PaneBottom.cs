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
using TopEng.Utils;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_PaneBottom : Form
    {
        public delegate void cbChangePageEvent(MainForm.PAGEID page);
        public event cbChangePageEvent ChangePage;
        public delegate void cbMinimalizeWindowEvent();
        public event cbMinimalizeWindowEvent MinimalizeWindow;
        private List<ButtonEnh> ButtonsEnhs;
        public Form_PaneBottom()
        {
            InitializeComponent();
            ButtonsEnhs = new List<ButtonEnh>();
            ButtonsEnhs.Add(button_Manual);
            ButtonsEnhs.Add(button_Data);
            ButtonsEnhs.Add(button_Teach);
            ButtonsEnhs.Add(button_Auto);
            ButtonsEnhs.Add(button_Log);
            button_Auto_Click(button_Auto, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool running = ((Machine.status.state == SystemState.SystemStateRUN || Machine.status.state == SystemState.SystemStateRUNEMPTY) || Machine.Busy()) ? true : false;
            button_Manual.Enabled = !running;
            button_Data.Enabled = !running;
            button_Teach.Enabled = !running;
            button_Exit.Enabled = !running;
        }
        private void ChangeButton(object sender)
        {
            ButtonEnh btn = sender as ButtonEnh;
            foreach(ButtonEnh forbtn in ButtonsEnhs)
            {
                forbtn.ButtonPush = forbtn.Name == btn.Name ? true: false;
            }
        }
        public void UpdateLogUI(string msg)
        {
            try
            {
                StopWatch timerDwell = new StopWatch();
                while (timerDwell.Elapsed <= 2)
                    Application.DoEvents();

                string date = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");

                if (listBox_Log.InvokeRequired)
                {
                    Action action = delegate
                    {
                        listBox_Log.Items.Add("[" + date + "]" + msg);

                        if (listBox_Log.Items.Count > 100)
                            listBox_Log.Items.Remove(0);
                        listBox_Log.SelectedIndex = listBox_Log.Items.Count - 1;
                    };
                    listBox_Log.BeginInvoke(action);
                }
                else
                {
                    listBox_Log.Items.Add("[" + date + "]" + msg);

                    if (listBox_Log.Items.Count > 100)
                        listBox_Log.Items.Remove(0);
                    listBox_Log.SelectedIndex = listBox_Log.Items.Count - 1;
                }
            }
            catch// (Exception e)
            {
            }
        }

        private void button_Auto_Click(object sender, EventArgs e)
        {
            ChangeButton(sender);
            ChangePage?.Invoke(MainForm.PAGEID.PAGE_AUTO);
        }

        private void button_Manual_Click(object sender, EventArgs e)
        {
            ChangeButton(sender);
            ChangePage?.Invoke(MainForm.PAGEID.PAGE_MANUAL);
        }

        private void button_Data_Click(object sender, EventArgs e)
        {
            ChangeButton(sender);
            ChangePage?.Invoke(MainForm.PAGEID.PAGE_DATA);
        }

        private void button_Teach_Click(object sender, EventArgs e)
        {
            ChangeButton(sender);
            ChangePage?.Invoke(MainForm.PAGEID.PAGE_TEACH);
        }

        private void button_Log_Click(object sender, EventArgs e)
        {
            ChangeButton(sender);
            ChangePage?.Invoke(MainForm.PAGEID.PAGE_LOG);
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            ChangePage?.Invoke(MainForm.PAGEID.PAGE_EXIT);
        }

        private void button_Hide_Click(object sender, EventArgs e)
        {
            MinimalizeWindow();
        }
    }
}
