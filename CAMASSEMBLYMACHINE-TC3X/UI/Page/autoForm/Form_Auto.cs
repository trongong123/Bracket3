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
    public partial class Form_Auto : Form
    {
        public enum AUTOPAGE
        {
            UNKNOWN = -1,
            CONTROL,
            //VISION,
            NETWORK,

            AUTOPAGEMAX
        }

        List<Form> autoForm = new List<Form>();
        AUTOPAGE currentAutoPage = AUTOPAGE.UNKNOWN;

        public Form_Auto()
        {
            InitializeComponent();
            autoForm.Add(new tabSummaryAuto());
            //autoForm.Add(new tabVisionAuto());
            autoForm.Add(new tabNetworkAuto());

            tabControl1.TabPages.Clear();

            ImageList tabImageList = new ImageList();
            tabImageList.Images.Add("machine", Properties.Resources.TITLE_MACHINE);
            tabImageList.Images.Add("machineSel", Properties.Resources.TITLE_MACHINE_SEL);
            //tabImageList.Images.Add("vision", Properties.Resources.TITLE_VISION);
            //tabImageList.Images.Add("visionSel", Properties.Resources.TITLE_VISION_SEL);
            //tabImageList.Images.Add("machine", Properties.Resources.TITLE_NETWORK);
            //tabImageList.Images.Add("machineSel", Properties.Resources.TITLE_NETWORK_SEL);
            tabImageList.ColorDepth = ColorDepth.Depth32Bit;
            tabImageList.ImageSize = new Size(140, 35);
            tabImageList.TransparentColor = Color.Transparent;
            tabControl1.ImageList = tabImageList;

            for (int i = 0; i < (int)AUTOPAGE.AUTOPAGEMAX; i++)
            {
                TabPage newTabPage = new TabPage("");
                newTabPage.ImageIndex = 2 * i;
                newTabPage.Margin = new Padding(0);
                tabControl1.TabPages.Add(newTabPage);

                if (autoForm.Count > i)
                {
                    autoForm[i].TopLevel = false;
                    autoForm[i].Parent = newTabPage;
                    autoForm[i].Dock = DockStyle.Fill;
                    autoForm[i].Show();
                }
            }
            tabControl1.SelectedIndex = 0;
            tabControl1.TabPages[0].ImageIndex = 1;
            currentAutoPage = (AUTOPAGE)tabControl1.SelectedIndex;
        }
        public void ConnectDisplay()
        {
            //var form = autoForm[(int)AUTOPAGE.VISION] as tabVisionAuto;
            //form.ConnectDisplay();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopTimerAll();
            int selTab = (sender as TabControl).SelectedIndex;
            currentAutoPage = (AUTOPAGE)selTab;
            for (int i = 0; i < (int)AUTOPAGE.AUTOPAGEMAX; i++)
            {
                if (selTab == i)
                    (sender as TabControl).TabPages[i].ImageIndex = 2 * i + 1;
                else
                    (sender as TabControl).TabPages[i].ImageIndex = 2 * i;
            }
            StartTimer();
        }
        public void StartTimer()
        {
            IForm form = autoForm[(int)currentAutoPage] as IForm;
            form.StartTimer(true);
        }
        public void StopTimerAll()
        {
            foreach(IForm form in autoForm)
            {
                form.StartTimer(false);
            }
        }
    }
}
