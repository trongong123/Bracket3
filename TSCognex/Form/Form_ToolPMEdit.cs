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
    public partial class Form_ToolPMEdit : Form
    {
        public string filepath;
        public ICogImage image;
        string strToolName;
        int nInspNo;

        CogToolBlock CogTools = new CogToolBlock();
        private CogPMAlignEditV2 cogPMAlignEditV21;

        public Form_ToolPMEdit(string Filepath, ICogImage Image, string toolname,int nToolNo)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;
            strToolName = toolname;
            nInspNo= nToolNo;

        }

        private void InitializeComponent()
        {
            this.cogPMAlignEditV21 = new Cognex.VisionPro.PMAlign.CogPMAlignEditV2();
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEditV21)).BeginInit();
            this.SuspendLayout();
            // 
            // cogPMAlignEditV21
            // 
            this.cogPMAlignEditV21.Location = new System.Drawing.Point(13, 13);
            this.cogPMAlignEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogPMAlignEditV21.Name = "cogPMAlignEditV21";
            this.cogPMAlignEditV21.Size = new System.Drawing.Size(814, 507);
            this.cogPMAlignEditV21.SuspendElectricRuns = false;
            this.cogPMAlignEditV21.TabIndex = 0;
            // 
            // Form_ToolPMEdit
            // 
            this.ClientSize = new System.Drawing.Size(828, 520);
            this.Controls.Add(this.cogPMAlignEditV21);
            this.Name = "Form_ToolPMEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolPMEdit_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_ToolPMEdit_FormClosed);
            this.Load += new System.EventHandler(this.Form_ToolPMEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEditV21)).EndInit();
            this.ResumeLayout(false);

        }
        private void Form_ToolPMEdit_Load(object sender, EventArgs e)
        {
            try
            {
                CogTools = CogSerializer.LoadObjectFromFile(filepath) as CogToolBlock;
                CogTools.Inputs["Input"].Value = image;
                CogTools.Run();
                //cogToolBlockEditV21.Subject = ToolBlock;
                cogPMAlignEditV21.Subject = CogTools.Tools[strToolName] as CogPMAlignTool;
            }
            catch
            {

            }

        }
        private void Form_ToolPMEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (cogPMAlignEditV21.Subject.HasChanged)
            {
                if (MessageBox.Show("Do you want to save ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CogSerializer.SaveObjectToFile(CogTools, filepath);
                }
            }
        }
        private void Form_ToolPMEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 250512, Memory
            if (CogTools != null)
            {
                CogTools.Dispose();
                CogTools = null;
            }

            if (cogPMAlignEditV21 != null)
            {
                cogPMAlignEditV21.Dispose();
                cogPMAlignEditV21 = null;
            }
        }

        private void button_Run_Click(object sender, EventArgs e)
        {
            CogTools.Run();
        }
    }
}
