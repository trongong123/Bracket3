using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopEng.Controls
{
    public partial class ProcessPictureBox : UserControl
    {
        public enum COLOR_OBJECT
        {
            BG_TEXT,
            TEXT,
            BG_PICTURE
        }
        Color[] originColor = new Color[3];
        bool EnableHideRow = false;
        /// <summary>
        /// 배경의 색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("이름 열을 숨깁니다.")]
        public bool HideNameRow
        {
            get { return EnableHideRow; }
            set 
            {
                if (bUsed)
                {
                    EnableHideRow = value;

                    if (EnableHideRow)
                    {
                        lbName.Visible = false;
                        topTableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Absolute, 0);
                    }
                    else
                    {
                        topTableLayoutPanel1.RowStyles[0] = new RowStyle(SizeType.Percent, 33.33f);
                        lbName.Visible = true;
                    }
                }
            }
        }
        bool bUsed = true;
        /// <summary>
        /// Enabled 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [DefaultValue(true)]
        [Description("배경의 색상을 설정 합니다.")]
        public bool Used
        {
            get { return bUsed; }
            set 
            {
                if( value )
                {
                    bUsed = value;

                    pnlBackGroundName.BackColor = originColor[(int)COLOR_OBJECT.BG_TEXT];
                    lbName.ForeColor = originColor[(int)COLOR_OBJECT.TEXT];
                    pnlBackgroundControl.BackColor = originColor[(int)COLOR_OBJECT.BG_PICTURE];
                }
                else
                {
                    originColor[(int)COLOR_OBJECT.BG_TEXT] = pnlBackGroundName.BackColor;
                    originColor[(int)COLOR_OBJECT.TEXT] = lbName.ForeColor;
                    originColor[(int)COLOR_OBJECT.BG_PICTURE] = pnlBackgroundControl.BackColor;

                    pnlBackGroundName.BackColor = Color.DimGray;
                    lbName.ForeColor = Color.DarkGray;
                    pnlBackgroundControl.BackColor = Color.DarkGray;

                    bUsed = value;
                }                
            }
        }
        /// <summary>
        /// 배경의 색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("배경의 색상을 설정 합니다.")]
        public Color ColorStatus
        {
            get { return pnlBackgroundControl.BackColor; }
            set
            { 
                if(bUsed) 
                    pnlBackgroundControl.BackColor = value; 
            }
        }
        /// <summary>
        /// 이름 Text를 변경 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("이름 Text를 변경 합니다.")]
        public string TextName
        {
            get { return lbName.Text; }
            set
            {
                if (bUsed)
                {
                    lbName.Text = value;
                    CenterLabelOnPanel(lbName, pnlBackGroundName);
                }     
            }
        }
        /// <summary>
        /// Font를 변경 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("TextName의 Font를 변경 합니다.")]
        public Font TextFont
        {
            get { return lbName.Font; }
            set
            {
                if (bUsed)
                {
                    lbName.Font = value;
                    CenterLabelOnPanel(lbName, pnlBackGroundName);
                }
                    
            }
        }

        /// <summary>
        /// TextName의 색상을 변경 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("TextName의 색상을 변경 합니다.")]
        public Color TextColor
        {
            get { return lbName.ForeColor; }
            set 
            {
                if (bUsed)
                    lbName.ForeColor = value;
            }
        }

        /// <summary>
        /// TextName의 색상을 변경 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("TextName의 색상을 변경 합니다.")]
        public Color TextBackgroundColor
        {
            get { return pnlBackGroundName.BackColor; }
            set 
            { 
                if (bUsed) 
                    pnlBackGroundName.BackColor = value; 
            }
        }

        /// <summary>
        /// PictureBox Image를 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("PictureBox Image를 설정 합니다.")]
        public Image Image
        {
            get { return pnlBackGroundPictureBox.BackgroundImage; }
            set 
            {
                if (bUsed)
                {
                    pnlBackGroundPictureBox.BackgroundImage = value;
                    pnlBackGroundPictureBox.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
        }

        public event EventHandler ClickEvent;
        public ProcessPictureBox()
        {
            InitializeComponent();
            this.lbName.Click += new EventHandler(MyUserControl_Click);
            this.pnlBackGroundPicture.Click += new EventHandler(MyUserControl_Click);
            this.pnlBackGroundPictureBox.Click += new EventHandler(MyUserControl_Click);
            originColor[(int)COLOR_OBJECT.BG_TEXT] = Color.Black;
            originColor[(int)COLOR_OBJECT.TEXT] = Color.White;
            originColor[(int)COLOR_OBJECT.BG_PICTURE] = Color.White;
        }

        private void MyUserControl_Click(object sender, EventArgs e)
        {
            if (ClickEvent != null)
            {
                ClickEvent(this, e);
            }
        }

        /// <summary>
        /// Text 위치를 중앙으로 변경한다
        /// </summary>
        /// <param name="label"></param>
        /// <param name="panel"></param>
        private void CenterLabelOnPanel(Label label, Panel panel)
        {
            // Label의 중앙 좌표를 계산
            int labelX = (panel.Width - label.Width) / 2;
            int labelY = ((panel.Height - label.Height) / 2 );

            // Label의 위치를 설정
            label.Location = new Point(labelX, labelY);
        }

        private void ProcessPictureBox_Resize(object sender, EventArgs e)
        {
        //    ChangeFontSizeOnPanel(lbName, pnlBackGroundName);
            CenterLabelOnPanel(lbName, pnlBackGroundName);
        }
        /// <summary>
        /// 크기 변경에 따라 Font도 자동 조절할 수 있도록 변경
        /// </summary>
        /// <param name="label"></param>
        /// <param name="panel"></param>
        private void ChangeFontSizeOnPanel(Label label, Panel panel)
        {
            // 초기 폰트 크기 설정
            float fontSize = 8f;
            string text = label.Text;
            Font font = new Font(label.Font.FontFamily, fontSize, label.Font.Style);
            Size textSize = TextRenderer.MeasureText(text, font);

            // Label의 Width에 맞게 폰트 크기를 조정
            //while (textSize.Width < panel.Width && fontSize < 50f)
            //{
            //    if( textSize.Height > panel.Height )
            //    {
            //        break;
            //    }
            //    fontSize += 0.5f;
            //    font = new Font(label.Font.FontFamily, fontSize, label.Font.Style);
            //    textSize = TextRenderer.MeasureText(text, font);
            //}

            // 초과한 경우 폰트 크기를 줄임
            while (textSize.Width > panel.Width && fontSize > 8f)
            {
                fontSize -= 0.5f;
                font = new Font(label.Font.FontFamily, fontSize, label.Font.Style);
                textSize = TextRenderer.MeasureText(text, font);
            }

            // 최종 폰트 크기를 Label에 적용
            label.Font = new Font(label.Font.FontFamily, fontSize, label.Font.Style);
        }
    }
}
