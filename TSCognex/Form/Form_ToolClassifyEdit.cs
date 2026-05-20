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
using static System.Net.WebRequestMethods;

namespace TopEng.Vision.Forms
{
    public partial class Form_ToolClassifyEdit : Form
    {
        public string filepath;
        public ICogImage image;
        string strToolName;
        int nInspNo;

        CogClassifyEditV2 classifyEditV2;
        CogToolBlock CogTools = new CogToolBlock();

        public Form_ToolClassifyEdit(string Filepath, ICogImage Image, string toolname, int nToolNo)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;
            strToolName = toolname;
            nInspNo = nToolNo;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form_ToolClassifyEdit
            // 
            this.ClientSize = new System.Drawing.Size(1094, 688);
            this.Name = "Form_ToolClassifyEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolClassifyEdit_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_ToolClassifyEdit_FormClosed);
            this.Load += new System.EventHandler(this.Form_ToolClassifyEdit_Load);
            this.ResumeLayout(false);

        }
        private void Form_ToolClassifyEdit_Load(object sender, EventArgs e)
        {
            CogTools = CogSerializer.LoadObjectFromFile(filepath) as CogToolBlock;
            CogTools.Inputs["Input"].Value = image;

            classifyEditV2 = new CogClassifyEditV2();
            this.Controls.Add(classifyEditV2);
            classifyEditV2.Dock = System.Windows.Forms.DockStyle.Fill;

            CogTools.Run();

            classifyEditV2.Subject = CogTools.Tools[strToolName] as CogClassifyTool;

        }
        private void Form_ToolClassifyEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if (classifyEditV2.Subject.HasChanged)
            {
                if (MessageBox.Show("Do you want to save ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CogSerializer.SaveObjectToFile(CogTools, filepath);
                }
            }
        }
        private void Form_ToolClassifyEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 250512, Memory
            if (CogTools != null)
            {
                CogTools.Dispose();
                CogTools = null;
            }

            if (classifyEditV2 != null)
            {
                classifyEditV2.Dispose();
                classifyEditV2 = null;
            }
        }

        public static ICogImage ImageFileLoad(string pFileName)
        {
            ICogImage cogImage = null;

            try
            {
                //----------
                Cognex.VisionPro.ImageFile.CogImageFile cogImageFile = new Cognex.VisionPro.ImageFile.CogImageFile();

                cogImageFile.Open(pFileName, Cognex.VisionPro.ImageFile.CogImageFileModeConstants.Read);

                cogImage = (ICogImage)cogImageFile[0];

                cogImageFile.Close();

                //----------
                cogImage = CogImageConvert.GetIntensityImage(cogImage, 0, 0, cogImage.Width, cogImage.Height);

            }
            catch (Exception exception)
            {
                //MFILE.ExceptionFileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }

            return cogImage;
        }

    }
}
