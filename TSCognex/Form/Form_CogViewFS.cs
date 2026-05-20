using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;

namespace TopEng.Vision.Forms
{
    public partial class Form_CogViewFS : Form
    {
        private CogRecordDisplay mainCogRecord;
        public Form_CogViewFS(CogRecordDisplay CogRecord)
        {
            InitializeComponent();
            timer1.Stop();
            this.mainCogRecord = CogRecord;
        }

        public void ShowForm()
        {
            timer1.Start();
            this.ShowDialog();
        }
        private void Form_CogViewFS_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            label1.Parent = cogRecordDisplay1;
        }

        private void cogRecordDisplay1_DoubleClick(object sender, EventArgs e)
        {
            timer1.Stop();
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                if (this.mainCogRecord.Image != cogRecordDisplay1.Image)
                {
                    cogRecordDisplay1.Image = this.mainCogRecord.Image;
                    cogRecordDisplay1.Record = this.mainCogRecord.Record;
                }
            }
            catch (Exception ex)
            { }
            timer1.Start();
        }
    }
}
