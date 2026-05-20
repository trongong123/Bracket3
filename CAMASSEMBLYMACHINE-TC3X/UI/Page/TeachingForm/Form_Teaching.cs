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

namespace CAMASSEMBLYMACHINE.UI
{    
    public partial class Form_Teaching : Form
    {
        public enum TEACHPAGE
        {
            UNKNOWN = -1,
            TRAY,
            PROD_LOADER, //MID
            ASSEMBLER, //Assemble
            JIG,
            LEFT_BUF,
            RIGHT_BUF,
            //OFFSET
        }

        List<ButtonEnh> teachButton = new List<ButtonEnh>();
        List<Form> teachForm = new List<Form>();
        TEACHPAGE currPage = TEACHPAGE.UNKNOWN;

        public Form_Teaching()
        {
            InitializeComponent();

            teachButton.Add(button_Tray);
            teachButton.Add(button_PROD_LOADER);
            teachButton.Add(button_ASSEMBLER);
            teachButton.Add(button_Jig);
            teachButton.Add(button_LeftBuffer);
            teachButton.Add(button_RightBuffer);
            //teachButton.Add(button_Offset);

            teachForm.Add(new tabTrayTeach());
            teachForm.Add(new tabProdLoaderTeach());
            teachForm.Add(new tabAssemblerTeach());
            teachForm.Add(new tabJigTeach());
            teachForm.Add(new tabBufferTeach2(tabBufferTeach2.BufferStation.LEFT));
            teachForm.Add(new tabBufferTeach2(tabBufferTeach2.BufferStation.RIGHT));
            //teachForm.Add(new tabOffsetTeach());

            foreach (var form in teachForm)
            {
                form.TopLevel = false;
                form.Parent = panelControl;
                form.Dock = DockStyle.Fill;
            }
        }

        private void Form_Teaching_Load(object sender, EventArgs e)
        {
            // Hide TAB Button
            tabControlPicture.Appearance = TabAppearance.Buttons;
            tabControlPicture.SizeMode = TabSizeMode.Fixed;
            tabControlPicture.ItemSize = new Size(0, 1);

            ChangePage(TEACHPAGE.TRAY);
        }

        public void StopTimerAll()
        {
            for (int i = 0; i < teachForm.Count; i++)
            {
                IForm form = teachForm[i] as IForm;
                form.StartTimer(false);
            }
        }
        public void StartTimer(bool enable)
        {
            if (enable)
            {
                var form2 = teachForm[(int)currPage] as IForm;
                form2.StartTimer(true);
            }
            else
                StopTimerAll();
        }

        public void ChangePage(TEACHPAGE page)
        {
            if (currPage == page || TEACHPAGE.UNKNOWN == page)
                return;

            if (currPage != TEACHPAGE.UNKNOWN)
            {
                teachButton[(int)currPage].ButtonPush = false;
                teachForm[(int)currPage].Hide();
                var form1 = teachForm[(int)currPage] as IForm;
                form1.StartTimer(false);
            }
            currPage = page;
            teachButton[(int)currPage].ButtonPush = true;
            teachForm[(int)currPage].Show();
            var form2 = teachForm[(int)currPage] as IForm;
            form2.StartTimer(true);

            tabControlPicture.SelectedIndex = (int)page;
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            Machine.EStop(false);
        }

        private void button_Tray_Click(object sender, EventArgs e)
        {
           ChangePage(TEACHPAGE.TRAY);
        }

        private void button_PROD_LOADER_Click(object sender, EventArgs e)
        {
            ChangePage(TEACHPAGE.PROD_LOADER);
        }

        private void button_ASSEMBLER_Click(object sender, EventArgs e)
        {
            ChangePage(TEACHPAGE.ASSEMBLER);
        }
        
        //private void button_Jig_Click(object sender, EventArgs e)
        //{
        //    ChangePage(TEACHPAGE.JIG);
        //}

        private void button_LeftBuffer_Click(object sender, EventArgs e)
        {
            ChangePage(TEACHPAGE.LEFT_BUF);
        }

        private void button_RightBuffer_Click(object sender, EventArgs e)
        {
            ChangePage(TEACHPAGE.RIGHT_BUF);
        }

        private void button_Offset_Click(object sender, EventArgs e)
        {
            //ChangePage(TEACHPAGE.OFFSET);
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

        private void button_Jig_Click(object sender, EventArgs e)
        {
            ChangePage(TEACHPAGE.JIG);
        }
    }
}
