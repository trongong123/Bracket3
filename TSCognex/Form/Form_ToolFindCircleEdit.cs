using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.OCRMax;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.ViDiEL;
using TopEng.Type;
using TopEng.Utils;
using Cognex.VisionPro.Implementation;
using System.IO;
using Cognex.Vision.ViDiEL;
using Cognex.Vision.Implementation;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_ToolFindCircleEdit : Form
    {
        public string filepath;
        public ICogImage image;
        string strToolName;
        int nInspNo;

        CogToolBlock CogTools = new CogToolBlock();
        private CogFindCircleEditV2 cogFindCircleEditV21;

        public Form_ToolFindCircleEdit(string Filepath, ICogImage Image, string toolname, int nToolNo)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;
            strToolName = toolname;
            nInspNo = nToolNo;

        }
      
        private void InitializeComponent()
        {
            this.cogFindCircleEditV21 = new Cognex.VisionPro.Caliper.CogFindCircleEditV2();
            ((System.ComponentModel.ISupportInitialize)(this.cogFindCircleEditV21)).BeginInit();
            this.SuspendLayout();
            // 
            // cogFindCircleEditV21
            // 
            this.cogFindCircleEditV21.Location = new System.Drawing.Point(3, 13);
            this.cogFindCircleEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogFindCircleEditV21.Name = "cogFindCircleEditV21";
            this.cogFindCircleEditV21.Size = new System.Drawing.Size(917, 568);
            this.cogFindCircleEditV21.SuspendElectricRuns = false;
            this.cogFindCircleEditV21.TabIndex = 0;
            // 
            // Form_ToolFindCircleEdit
            // 
            this.ClientSize = new System.Drawing.Size(932, 612);
            this.Controls.Add(this.cogFindCircleEditV21);
            this.Name = "Form_ToolFindCircleEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolFindCircleEdit_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_ToolFindCircleEdit_FormClosed);
            this.Load += new System.EventHandler(this.Form_ToolFindCircleEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogFindCircleEditV21)).EndInit();
            this.ResumeLayout(false);

        }
        private void Form_ToolFindCircleEdit_Load(object sender, EventArgs e)
        {

            try
            {
                CogTools = CogSerializer.LoadObjectFromFile(filepath) as CogToolBlock;
                CogTools.Inputs["Input"].Value = image;
                //cogToolBlockEditV21.Subject = ToolBlock;
                CogTools.Run();
                cogFindCircleEditV21.Subject = CogTools.Tools[strToolName] as CogFindCircleTool;
            }
            catch
            {

            }
        }
        private void Form_ToolFindCircleEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
           // if (cogFindCircleEditV21.Subject.HasChanged)
            {
                if (MessageBox.Show("Do you want to save ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CogSerializer.SaveObjectToFile(CogTools, filepath);
                }
            }
        }
        private void Form_ToolFindCircleEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 250512, Memory
            if (CogTools != null)
            {
                CogTools.Dispose();
                CogTools = null;
            }

            if (cogFindCircleEditV21 != null)
            {
                cogFindCircleEditV21.Dispose();
                cogFindCircleEditV21 = null;
            }
        }

        private void button_Run_Click(object sender, EventArgs e)
        {
            CogTools.Run();
        }



    }
}
