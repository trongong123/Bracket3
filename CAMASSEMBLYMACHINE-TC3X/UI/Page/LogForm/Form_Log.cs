using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;
using TopEng.Utils;
using System.IO;
using CAMASSEMBLYMACHINE.Define;

namespace CAMASSEMBLYMACHINE.UI
{
    public enum DATE_TYPE
    {
        NOW,
        SELECTED
    }
    public interface ILogForm
    {
        void LoadLogFile(DATE_TYPE type, string[] logIines);
        void ClearListBox();
        void AddLine(string msg);
        void LogCount(string[] strLogFullData);
    }
    public partial class Form_Log : Form
    {
        public enum DATAPAGE
        {
            UNKNOWN = -1,
            ERROR,
            DATA,
            BARCODE,
            EXCEPTION
        }

        List<ButtonEnh> dataButton = new List<ButtonEnh>();
        List<Form> dataForm = new List<Form>();
        DATAPAGE currPage = DATAPAGE.UNKNOWN;
        DATE_TYPE currDateType = DATE_TYPE.NOW;
        string selectedDate = string.Empty;
        bool firstShow = true;
        public Form_Log()
        {
            InitializeComponent();

            dataButton.Add(button_Error);
            dataButton.Add(button_Data);
            dataButton.Add(button_Barcode);

            dataForm.Add(new tabErrorLog());
            dataForm.Add(new tabDataLog());
            dataForm.Add(new tabBarcodeLog(dateTimePicker1));

            foreach (var form in dataForm)
            {
                form.TopLevel = false;
                form.Parent = panelControl;
                form.Dock = DockStyle.Fill;
            }
        }

        private void Form_Data_Load(object sender, EventArgs e)
        {
            buttonCurrent.ButtonPush = true;
            currDateType = DATE_TYPE.NOW;
            selectedDate = selectedDate = DateTime.Now.ToString("yyyy-MM-dd");
            ChangePage(DATAPAGE.ERROR);
            LoadLogFils(currDateType, selectedDate);
        }

        public void ChangePage(DATAPAGE page)
        {
            if (currPage == page || DATAPAGE.UNKNOWN == page)
                return;

            if (currPage != DATAPAGE.UNKNOWN)
            {
                dataButton[(int)currPage].ButtonPush = false;
                dataForm[(int)currPage].Hide();
            }
            currPage = page;
            if (page == DATAPAGE.ERROR)
            {
                btnErrorCount.Visible = true;
                btnErrorCount.Checked = false;
                btnErrorCount.BackColor = Color.White;
            }
            else
            {
                btnErrorCount.Visible = false;
                btnErrorCount.Checked = false;
            }
            dataButton[(int)currPage].ButtonPush = true;
            dataForm[(int)currPage].Show();
        }

        private void button_Data_Click(object sender, EventArgs e)
        {
            ChangePage(DATAPAGE.DATA);
            LoadLogFils(currDateType, selectedDate);
        }

        private void button_Error_Click(object sender, EventArgs e)
        {
            ChangePage(DATAPAGE.ERROR);
            LoadLogFils(currDateType, selectedDate);
        }

        private void button_Barcode_Click(object sender, EventArgs e)
        {
            ChangePage(DATAPAGE.BARCODE);
            LoadLogFils(currDateType, selectedDate);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // 오늘 날짜와 Value 날짜가 같은지 확인
            if ((sender as DateTimePicker).Value == DateTime.Now.Date)
            {
                currDateType = DATE_TYPE.NOW;
                buttonCurrent.ButtonPush = true;
            }
            else
            {
                // 날짜 선택 하였기에 상태값 변경 (로그 이벤트 막기 위해)
                currDateType = DATE_TYPE.SELECTED;
                buttonCurrent.ButtonPush = false;
            }
            // 달력의 날짜 가져옴
            selectedDate = (sender as DateTimePicker).Value.ToString("yyyy-MM-dd");

            if (btnErrorCount.Checked && btnErrorCount.Visible) btnErrorCount_CheckedChanged(null, null);
            // 파일 불러오기, log form 출력하기
            else LoadLogFils(currDateType, selectedDate);
        }

        private void buttonCurrent_Click(object sender, EventArgs e)
        {
            buttonCurrent.ButtonPush = true;
            currDateType = DATE_TYPE.NOW;
            // 오늘 날짜 가져옴 ( dateTimePicker1_ValueChanged 에서 처리)
            dateTimePicker1.Value = DateTime.Now.Date;
            // 날짜 선택 하였기에 상태값 변경 (로그 이벤트 막기 위해)
            // LoadLogFils(currDateType, selectedDate);
        }
        private void LoadLogFils(DATE_TYPE dateType, string date)
        {
            // 현재 페이지의 로그 타입을 알기 위해 선언과 초기화
            LOG_TYPE logType = LOG_TYPE.SYSTEM;
            // 현재 페이지 로그 타입 저장 (파일명을 정하기 위해)
            switch (currPage)
            {
                case DATAPAGE.DATA: logType = LOG_TYPE.DATA; break;
                case DATAPAGE.ERROR: logType = LOG_TYPE.ALARM; break;
                case DATAPAGE.BARCODE: logType = LOG_TYPE.BARCODE; break;
            }
            // 위 내용으로 파일명을 만들어 로그를 출력
            string filePath = SystemDefine.logPath;
            string filName = filePath + $@"\{Enum.GetName(typeof(LOG_TYPE), logType).ToString()}_{date}.txt";
            string[] logLines = null;
            string[] logLinesReverse = null;
            if (File.Exists(filName))
            {
                logLines = File.ReadLines(filName).ToArray();
                logLinesReverse = logLines.Reverse().Take(100).Reverse().ToArray();
                (dataForm[(int)currPage] as ILogForm).LoadLogFile(dateType, logLinesReverse);
            }
            else
            {
                (dataForm[(int)currPage] as ILogForm).ClearListBox();
                (dataForm[(int)currPage] as ILogForm).AddLine(@"로그 파일이 없습니다");
            }

        }

        private void button_PreDate_Click(object sender, EventArgs e)
        {
            DateTime date = dateTimePicker1.Value.Date;
            dateTimePicker1.Value = date.AddDays(-1);
        }

        private void button_NextDate_Click(object sender, EventArgs e)
        {
            DateTime date = dateTimePicker1.Value.Date;
            dateTimePicker1.Value = date.AddDays(+1);
        }

        private void btnErrorCount_CheckedChanged(object sender, EventArgs e)
        {
            if (!btnErrorCount.Visible) return;
            if (!btnErrorCount.Checked)
            {
                btnErrorCount.BackColor = Color.White;
                button_Error_Click(null, null);
                return;
            }
            btnErrorCount.BackColor = Color.SpringGreen;

            string strFilePath = SystemDefine.logPath;
            string strFileName = strFilePath + $@"\{LOG_TYPE.ALARM}_{selectedDate}.txt";
            string[] strLogFullData;
            if (File.Exists(strFileName))
            {
                strLogFullData = File.ReadLines(strFileName).ToArray();
                (dataForm[(int)currPage] as ILogForm).LogCount(strLogFullData);
            }
            else
            {
                LoadLogFils(currDateType, selectedDate);
                return;
            }
        }
    }
}
