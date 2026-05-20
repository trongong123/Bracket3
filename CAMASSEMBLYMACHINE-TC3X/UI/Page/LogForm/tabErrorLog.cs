using CAMASSEMBLYMACHINE.Define;
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
    public partial class tabErrorLog : Form, ILogForm
    {
        bool skipAddLogListBox = false;
        public tabErrorLog()
        {
            InitializeComponent();
            LogUtil.AlarmLogEvent += InsertLog;

            // Hide TAB Button
            pageControl.Appearance = TabAppearance.Buttons;
            pageControl.SizeMode = TabSizeMode.Fixed;
            pageControl.ItemSize = new Size(0, 1);

            CountListView.View = View.Details;
            CountListView.FullRowSelect = true;
            CountListView.GridLines = true;
            CountListView.Columns.Add("Count", this.CountListView.Width * 10 / 100);
            CountListView.Columns.Add("Error", this.CountListView.Width * 90 / 100);
        }
        public void ClearListBox()
        {
            listBox1.Items.Clear();
        }
        public void LoadLogFile(DATE_TYPE type, string[] logIines)
        {
            pageControl.SelectedTab = tabNormalLog;

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
        public void AddLine(string msg)
        {
            pageControl.SelectedTab = tabNormalLog;
            listBox1.Items.Add(msg);
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

        public async void LogCount(string[] strInputData)
        {
            try
            {
                pageControl.SelectedTab = tabCountError;
                await Task.Run(() =>
                {
                    var logCountDict = new Dictionary<string, int>();
                    string strLine;
                    foreach (string line in strInputData)
                    {
                        if (line.Length <= 26) continue;
                        strLine = line.Remove(0, 26); // remove datetime data
                        if (logCountDict.ContainsKey(strLine)) logCountDict[strLine]++;
                        else logCountDict[strLine] = 1;
                    }
                    var sortedLogCountDict = logCountDict.OrderByDescending(kvp => kvp.Value);
                    this.Invoke((Action)(() =>
                    {
                        this.CountListView.Items.Clear();
                        this.CountListView.Items.Add(new ListViewItem(new[] { strInputData.Count().ToString(), "ALL ERROR COUNT" }));
                        foreach (var logCount in sortedLogCountDict)
                        {
                            this.CountListView.Items.Add(new ListViewItem(new[] { logCount.Value.ToString(), logCount.Key }));
                        }
                    }));
                });
            }
            catch (Exception ex)
            {

            }
        }

        #region CONTENT_COPY_FUNCTION
        private void CountListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopySelectItem();
            }
        }

        private void CopySelectItem()
        {
            List<(string, string)> selectedLogList = new List<(string, string)>();
            foreach (int iIndex in CountListView.SelectedIndices)
            {
                selectedLogList.Add((CountListView.Items[iIndex].SubItems[0].Text, CountListView.Items[iIndex].SubItems[1].Text));
            }
            string strParsedLog = string.Empty;
            for (int i = 0; i < selectedLogList.Count(); i++)
            {
                strParsedLog += $"{selectedLogList[i].Item1},{selectedLogList[i].Item2} \n";
            }
            Clipboard.SetText(strParsedLog);
        }
        #endregion
    }
}
