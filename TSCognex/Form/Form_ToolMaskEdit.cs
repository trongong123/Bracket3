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
    public partial class Form_ToolMaskEdit : Form
    {
        public string filepath;
        public ICogImage image;

        CogToolBlock CogTools = new CogToolBlock();
        private Cognex.VisionPro.ImageProcessing.CogMaskCreatorEditV2 cogMaskCreatorEditV21;

        public Form_ToolMaskEdit(string Filepath, ICogImage Image)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;

        }
        public CogToolBase GetResult(string toolname, ref string tooltype)
        {
            tooltype = CogTools.Tools[toolname].GetType().Name;
            return (CogToolBase)CogTools.Tools[toolname];
        }
        

        private void Form_ToolMaskEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (cogToolBlockEditV21.Subject.HasChanged)
            //{
            //    if (MessageBox.Show("Do you want to save ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        CogSerializer.SaveObjectToFile(cogToolBlockEditV21.Subject, filepath);
            //    }
            //}
        }

        private void cogMaskCreatorEditV22_Load(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            this.cogMaskCreatorEditV21 = new Cognex.VisionPro.ImageProcessing.CogMaskCreatorEditV2();
            ((System.ComponentModel.ISupportInitialize)(this.cogMaskCreatorEditV21)).BeginInit();
            this.SuspendLayout();
            // 
            // cogMaskCreatorEditV21
            // 
            this.cogMaskCreatorEditV21.Location = new System.Drawing.Point(13, 13);
            this.cogMaskCreatorEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogMaskCreatorEditV21.Name = "cogMaskCreatorEditV21";
            this.cogMaskCreatorEditV21.Size = new System.Drawing.Size(863, 494);
            this.cogMaskCreatorEditV21.SuspendElectricRuns = false;
            this.cogMaskCreatorEditV21.TabIndex = 0;
            // 
            // Form_ToolMaskEdit
            // 
            this.ClientSize = new System.Drawing.Size(897, 531);
            this.Controls.Add(this.cogMaskCreatorEditV21);
            this.Name = "Form_ToolMaskEdit";
            ((System.ComponentModel.ISupportInitialize)(this.cogMaskCreatorEditV21)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
