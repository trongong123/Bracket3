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
    public partial class ProductCountValueV : UserControl
    {
        public enum BG_COLOR
        {
            FAIL,
            FAIL_RATE,
            PASS,
            TIME,
            TOTAL
        }
        private BG_COLOR bgColor = BG_COLOR.TIME;

        public ProductCountValueV()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Count 값 설정합니다
        /// </summary>
        [Category("User 속성")]
        [Description("Count 값 설정합니다")]
        public BG_COLOR Color
        {
            get { return bgColor; }
            set
            {
                bgColor = value;
                switch (bgColor)
                {
                    case BG_COLOR.FAIL:         buttonName.BackgroundImage = Properties.Resources.NV_Title_Fail; break;
                    case BG_COLOR.FAIL_RATE:    buttonName.BackgroundImage = Properties.Resources.NV_Title_FailRate; break;
                    case BG_COLOR.PASS:         buttonName.BackgroundImage = Properties.Resources.NV_Title_Pass; break;
                    case BG_COLOR.TOTAL:        buttonName.BackgroundImage = Properties.Resources.NV_Title_Total; break;
                    case BG_COLOR.TIME:         buttonName.BackgroundImage = Properties.Resources.NV_Title_Time; break;
                }
            }
        }

        /// <summary>
        /// Count 값 설정합니다
        /// </summary>
        [Category("User 속성")]
        [DefaultValue("99999")]
        [Description("Count 값 설정합니다")]
        public string CountText
        {
            get { return buttonCount.Text; }
            set { buttonCount.Text = value; }
        }

        /// <summary>
        /// 이름을 설정합니다
        /// </summary>
        [Category("User 속성")]
        [DefaultValue("Total")]
        [Description("이름을 설정합니다")]
        public string CountName
        {
            get { return buttonName.Text; }
            set { buttonName.Text = value; }
        }


    }
}
