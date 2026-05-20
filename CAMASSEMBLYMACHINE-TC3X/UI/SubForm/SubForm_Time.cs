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
    public partial class SubForm_Time : Form
    {
        int Hour = 0;
        int Minute = 0;
        int Second = 0;
        string time = "";

        public SubForm_Time(int hour, int minute, int second)
        {
            InitializeComponent();

            Hour = hour;
            Minute = minute;
            Second = second;
        }

        private void SubForm_Time_Load(object sender, EventArgs e)
        {
            InitializeUI();
            CenterToScreen();

            cbo_Hour.SelectedIndex = Hour;
            cbo_Minute.SelectedIndex = Minute;
            cbo_Second.SelectedIndex = Second;
        }

        public string GetTime()
        {
            return time;
        }

        private void InitializeUI()
        {
            for (int i = 0; i < 60; i++)
            {
                if (i < 24)
                    cbo_Hour.Items.Insert(i, i.ToString());
                cbo_Minute.Items.Insert(i, i.ToString());
                cbo_Second.Items.Insert(i, i.ToString());
            }
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            int idxHour, idxMinute, idxSecond;
            if (!int.TryParse(cbo_Hour.Text, out idxHour) || !int.TryParse(cbo_Minute.Text, out idxMinute) || !int.TryParse(cbo_Second.Text, out idxSecond)
                || idxHour < 0 || idxMinute < 0 || idxSecond < 0)
            {
                SubForm_Warning dlg = new SubForm_Warning("0 이상의 정수를 넣으세요");
                dlg.ShowDialog();
                return;
            }

            idxHour = idxHour % 24; idxMinute = idxMinute % 60; idxSecond = idxSecond % 60;

            if (idxHour != cbo_Hour.SelectedIndex)
                cbo_Hour.SelectedIndex = idxHour;
            if (idxMinute != cbo_Minute.SelectedIndex)
                cbo_Minute.SelectedIndex = idxMinute;
            if (idxSecond != cbo_Second.SelectedIndex)
                cbo_Second.SelectedIndex = idxSecond;

            Hour = cbo_Hour.SelectedIndex;
            Minute = cbo_Minute.SelectedIndex;
            Second = cbo_Second.SelectedIndex;
            time = $"{Hour.ToString("00")}:{Minute.ToString("00")}:{Second.ToString("00")}";
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


    }
}
