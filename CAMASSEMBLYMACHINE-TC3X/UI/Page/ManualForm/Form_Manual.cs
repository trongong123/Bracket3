using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;
// user add

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_Manual : Form
    {
        public enum MANUALPAGE
        {
            UNKNOWN = -1,
            TRAY_CONV,
            LD_PICKER,
            ULD_PICKER,
            LEFT_BUF,
            RIGHT_BUF,
            JIG_CONV,
            VISION_TRAY,
            VISION_UNDER,
            VISION_JIG,
            VISION_PICKER,
            VISION_JIG2
        }

        List<ButtonEnh> manualButton = new List<ButtonEnh>();
        List<Form> manualForm = new List<Form>();
        MANUALPAGE currPage = MANUALPAGE.UNKNOWN;

        public delegate void BtnEnableCallbackEvent(bool bEnable);


        public Form_Manual()
        {
            InitializeComponent();

            manualButton.Add(button_TrayConv);
            manualButton.Add(button_PROD_LOADER);
            manualButton.Add(button_ASSEMBLER);
            manualButton.Add(button_LeftBuffer);
            manualButton.Add(button_RightBuffer);
            manualButton.Add(button_JigConv);
            manualButton.Add(button_VisionTray);
            manualButton.Add(button_VisionUnder);
            manualButton.Add(button_VisionJig);
            manualButton.Add(button_VisionPicker);
            manualButton.Add(button_VisionJig2);

            manualForm.Add(new tabTrayConvManu3());
            manualForm.Add(new tabPROD_LOADERManu3());
            manualForm.Add(new tabAssemblerManu3());
            manualForm.Add(new tabLeftBufferManu3());
            manualForm.Add(new tabRightBufferManu3());
            manualForm.Add(new tabJigInOutManu3());
            manualForm.Add(new tabVisionManuProp(0));
            manualForm.Add(new tabVisionManuProp(1));
            manualForm.Add(new tabVisionManuProp(2));
            manualForm.Add(new tabVisionManuProp(3));
            manualForm.Add(new tabVisionManuProp(4));

            foreach (var form in manualForm)
            {
                form.TopLevel = false;
                form.Parent = panelControl;
                form.Dock = DockStyle.Top;
            }
        }

        private void Form_Manual_Load(object sender, EventArgs e)
        {
            // Hide TAB Button
            tabControlPicture.Appearance = TabAppearance.Buttons;
            tabControlPicture.SizeMode = TabSizeMode.Fixed;
            tabControlPicture.ItemSize = new Size(0, 1);

            ChangePage(MANUALPAGE.TRAY_CONV);
        }

        public void StopTimerAll()
        {
            for (int i = 0; i < manualForm.Count; i++)
            {
                IForm form = manualForm[i] as IForm;
                form.StartTimer(false);
            }
        }
        public void StartTimer()
        {
            IForm form = manualForm[(int)currPage] as IForm;
            form.StartTimer(true);
        }
        public void ChangePage(MANUALPAGE page)
        {
            if (currPage == page || MANUALPAGE.UNKNOWN == page)
                return;

            if (currPage != MANUALPAGE.UNKNOWN)
            {
                manualButton[(int)currPage].ButtonPush = false;
                manualForm[(int)currPage].Hide();
                var form1 = manualForm[(int)currPage] as IForm;
                form1.StartTimer(false);
            }

            currPage = page;
            if (currPage == MANUALPAGE.VISION_TRAY ||
                currPage == MANUALPAGE.VISION_UNDER ||
                currPage == MANUALPAGE.VISION_JIG ||
                currPage == MANUALPAGE.VISION_PICKER ||
                currPage == MANUALPAGE.VISION_JIG2)
            {
                tabVisionManuProp vision = manualForm[(int)currPage] as tabVisionManuProp;
                vision.initDataLoad();
            }
            manualButton[(int)currPage].ButtonPush = true;
            manualForm[(int)currPage].Show();
            var form2 = manualForm[(int)currPage] as IForm;
            form2.StartTimer(true);

            tabControlPicture.SelectedIndex = (int)page;
        }

        private void button_TrayConv_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.TRAY_CONV);
        }

        private void button_VisionTransfer_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.VISION_JIG2);
        }

        private ButtonEnh CreateVisionTransferButton()
        {
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Clear();
            for (int i = 0; i < 5; i++)
                tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            var button = new ButtonEnh
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                BackgroundImageLayout = ImageLayout.Stretch,
                ButtonPush = false,
                ButtonType = ButtonEnh.BUTTONTYPE.Normal,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.Black,
                ImageButton = false,
                TabStop = false,
                Text = "Vision\r\nTransfer",
                UseVisualStyleBackColor = false,
                Margin = new Padding(2),
                Name = "button_VisionTransfer"
            };

            tableLayoutPanel3.Controls.Add(button, 4, 0);
            button.Click += new EventHandler(this.button_VisionTransfer_Click);
            return button;
        }

        private void button_PROD_LOADER_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.LD_PICKER);
        }

        private void button_ASSEMBLER_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.ULD_PICKER);
        }

        private void button_LeftBuffer_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.LEFT_BUF);
        }

        private void button_RightBuffer_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.RIGHT_BUF);
        }

        private void button_JigConv_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.JIG_CONV);
        }

        private void button_VisionTray_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.VISION_TRAY);
        }

        private void button_VisionUnder_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.VISION_UNDER);
        }

        private void button_VisionJig_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.VISION_JIG);
        }

        private void button_VisionJig2_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.VISION_JIG2);
        }

        private void button_VisionPicker_Click(object sender, EventArgs e)
        {
            ChangePage(MANUALPAGE.VISION_PICKER);
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            Machine.EStop(false);
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x2000000;
                return cp;
            }
        }
    }
}
