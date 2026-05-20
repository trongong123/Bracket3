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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabDataLog : Form, ILogForm
    {
        bool skipAddLogListBox = false;
        public tabDataLog()
        {
            InitializeComponent();
            LogUtil.DataLogEvent += InsertLog;
        }
        public void ClearListBox()
        {
            listBox1.Items.Clear();
        }
        public void AddLine(string msg)
        {
            listBox1.Items.Add(msg);
        }
        public void LoadLogFile(DATE_TYPE type, string[] logIines)
        {
            // 로그가 쌓이는것을 막고
            skipAddLogListBox = true;

            // 이벤트가 늦게 실행되어 Listbox에 카운트를 올릴수 있기에 반복 초기화
            do
            {
                listBox1.Items.Clear();
                System.Threading.Thread.Sleep(10);
            } while (listBox1.Items.Count != 0);

            // 로그파일 내용 ListBox 추가
            foreach (string line in logIines)
                listBox1.Items.Add(line);
            // 포커스 이동
            int maxCount = listBox1.Items.Count;
            if (maxCount > 0)
            {
                listBox1.SelectedIndex = maxCount - 1;
                listBox1.TopIndex = maxCount - 1;
            }
            // 상태에따라 Log 출력 상태를 변경
            skipAddLogListBox = (type == DATE_TYPE.NOW) ? false : true;
        }

        private void InsertLog(string logText)
        {
            try
            {
                if (listBox1.InvokeRequired)
                {
                    Action action = delegate
                    {
                        listBox1.Items.Add(logText);

                        if (listBox1.Items.Count > 100)
                            listBox1.Items.RemoveAt(0);
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    };
                    listBox1.BeginInvoke(action);
                }
                else
                {
                    listBox1.Items.Add(logText);

                    if (listBox1.Items.Count > 100)
                        listBox1.Items.RemoveAt(0);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }
            }
            catch// (Exception e)
            {
            }
        }

        public void LogCount(string[] strLogFullData) { }
    }
}
