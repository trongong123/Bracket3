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
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabBarcodeLog : Form, ILogForm
    {
        int listMax = 100; //listbox Max Line
        List<string> logList = new List<string>();

        DateTimePicker dateTimePicker = null;

        public tabBarcodeLog(DateTimePicker parentPicker)
        {
            InitializeComponent();
            LogUtil.BarcodeLogEvent += InsertLog;

            //dateTimePickerLog.Value = DateTime.Now;
            button_Clear.ClickEvent += button_Clear_Click;
            button_Open.ClickEvent += button_Open_Click;
            button_Load.ClickEvent += button_Load_Click;

            dateTimePicker = parentPicker;
        }
        public void AddLine(string msg) { }
        private void tabUILog_Load(object sender, EventArgs e)
        {
        }

        private void InsertLog(string logText)
        {
            try
            {
                if (listBox1.InvokeRequired)
                {
                    Action action = delegate
                    {
                        logList.Add((logList.Count + 1) + " / " + logText);
                        listBox1.Items.Add((logList[logList.Count - 1]));

                        if (listBox1.Items.Count > listMax)
                            listBox1.Items.RemoveAt(0);
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    };
                    listBox1.BeginInvoke(action);
                }
                else
                {
                    logList.Add((logList.Count + 1) + " / " + logText);
                    listBox1.Items.Add((logList[logList.Count - 1]));

                    if (listBox1.Items.Count > listMax)
                        listBox1.Items.RemoveAt(0);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }
            }
            catch// (Exception e)
            {
            }
        }
        private void button_Clear_Click(object sender, EventArgs e)
        {
            logList.Clear();
            listBox1.Items.Clear();
        }

        private void button_Open_Click(object sender, EventArgs e)
        {
            string filePath = Define.SystemDefine.productPath + @"\" + dateTimePicker.Value.ToString("yyyy-MM-dd") + ".csv";
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(filePath);
            }
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            int listMaxLoad = 100;

            int.TryParse(label2.Text, out listMaxLoad);

            if (listMaxLoad <= 0)
            {
                MessageBox.Show("Please Input Correct Value");
                return;
            }

            string filePath = Define.SystemDefine.productPath + @"\" + dateTimePicker.Value.ToString("yyyy-MM-dd") + ".csv";
            if (File.Exists(filePath))
            {
                logList.Clear();
                listBox1.Items.Clear();
                StreamReader stream = new StreamReader(filePath, false);

                while (!stream.EndOfStream)
                {
                    string strLine = stream.ReadLine();
                    string[] split = strLine.Split(',');

                    logList.Add(split[0]);

                    //listBox1.Items.Add(logList.Count + " / " + split[0]);
                    //if (listBox1.Items.Count > listMaxLoad)
                    //    listBox1.Items.RemoveAt(0);
                }

                if (listMaxLoad > logList.Count)
                    listMaxLoad = logList.Count;

                for (int i = logList.Count - listMaxLoad; i < logList.Count; ++i)
                    listBox1.Items.Add(i + " / " + logList[i]);

                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("Error: Can not find source file");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            string[] value = { label2.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "Max Line", "Lv", 0, 10000, 300, 300, false);
            keyPad.ShowDialog();
            label2.Text = value[0];
        }

        public void LoadLogFile(DATE_TYPE type, string[] logIines)
        {
            return;
        }

        public void ClearListBox()
        {
            return;
        }

        public void LogCount(string[] strLogFullData) { }
    }
}
