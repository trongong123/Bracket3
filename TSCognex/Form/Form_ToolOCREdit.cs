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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_ToolOCREdit : Form
    {
        public string filepath;
        public ICogImage image;
        string strToolName;
        int nInspNo;

        CogToolBlock CogTools = new CogToolBlock();
        private CogOCRMaxEditV2 cogOCRMaxEditV21;

        public Form_ToolOCREdit(string Filepath, ICogImage Image, string toolname, int nToolNo)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;
            strToolName = toolname;
            nInspNo = nToolNo;
        }
    
        private void InitializeComponent()
        {
            this.cogOCRMaxEditV21 = new Cognex.VisionPro.OCRMax.CogOCRMaxEditV2();
            ((System.ComponentModel.ISupportInitialize)(this.cogOCRMaxEditV21)).BeginInit();
            this.SuspendLayout();
            // 
            // cogOCRMaxEditV21
            // 
            this.cogOCRMaxEditV21.Location = new System.Drawing.Point(13, -2);
            this.cogOCRMaxEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogOCRMaxEditV21.Name = "cogOCRMaxEditV21";
            this.cogOCRMaxEditV21.Size = new System.Drawing.Size(848, 566);
            this.cogOCRMaxEditV21.SuspendElectricRuns = false;
            this.cogOCRMaxEditV21.TabIndex = 0;
            // 
            // Form_ToolOCREdit
            // 
            this.ClientSize = new System.Drawing.Size(873, 576);
            this.Controls.Add(this.cogOCRMaxEditV21);
            this.Name = "Form_ToolOCREdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolOCREdit_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_ToolOCREdit_FormClosed);
            this.Load += new System.EventHandler(this.Form_ToolOCREdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogOCRMaxEditV21)).EndInit();
            this.ResumeLayout(false);

        }
        private void Form_ToolOCREdit_Load(object sender, EventArgs e)
        {
             try
            {
                CogTools = CogSerializer.LoadObjectFromFile(filepath) as CogToolBlock;
                CogTools.Inputs["Input"].Value = image;
                //cogToolBlockEditV21.Subject = ToolBlock;
                CogTools.Run();
                cogOCRMaxEditV21.Subject = CogTools.Tools[strToolName] as CogOCRMaxTool;

                
            }
            catch
            {

            }
            // CreateToolClassify();

            // classifyEdi
            //ToolBlock.Inputs["Input"].Value = image;
            ////cogToolBlockEditV21.Subject = ToolBlock;

            //vppInfo.Clear();
            //vppInfo.Add(new VPPFILEINFO());
            //// WriteInfo(filepath);

            //if (File.Exists(filepath + @"\Inspection.vpp"))
            //    cogToolBlkProc.Create(filepath + @"\Inspection.vpp");

            //foreach (string toolName in cogToolBlkProc.GetToolListName())
            //{
            //    tooltype = CogTools.Tools[toolName].GetType().Name;
            //    var result = GetResult(toolName, ref tooltype);
            //    if (tooltype == "CogPMAlignTool")
            //    {
            //        var aligntool = (CogPMAlignTool)result;
            //        cogPMAlignEditV21.Subject = aligntool;
            //    }
            //}


            ///*
            // *             try
            //{
            //    vppInfo.Clear();
            //    vppInfo.Add(new VPPFILEINFO());
            //    WriteInfo(filepath);

            //    if (File.Exists(filepath + @"\Inspection.vpp"))
            //        cogToolBlkProc.Create(filepath + @"\Inspection.vpp");
            //    if (File.Exists(filepath + @"\Classify.vpp"))
            //    {
            //        cogClassifyProc.Create(filepath + @"\Classify.vpp");
            //        classifyExist = true;
            //    }
            //}
            //catch (Exception e)
            //{
            //    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            //}
            // * 
            // * 
            // */
            //for (int i = 0; i < vppInfo.Count; i++)
            //{
            //    string toolname = vppInfo[i].resultToolName;
            //    tooltype = "";
            //    var result = GetResult(toolname, ref tooltype);

            //    if (tooltype == "CogFindCircleTool")
            //    {
            //        var circletool = (CogFindCircleTool)result;
            //        CogToolFindCircle tool = new CogToolFindCircle(circletool);

            //    }
            //    else if (tooltype == "CogClassifyTool")
            //    {
            //        var aligntool = (CogClassifyTool)result;
            //        CogToolClassify tool = new CogToolClassify(aligntool);

            //    }
            //    else if (tooltype == "CogBlobTool")
            //    {
            //        var blobtool = (CogBlobTool)result;
            //        CogToolBlob tool = new CogToolBlob(blobtool);

            //    }
            //    else if (tooltype == "CogPMAlignTool")
            //    {
            //        var aligntool = (CogPMAlignTool)result;
            //        CogToolPMAlign tool = new CogToolPMAlign(aligntool);

            //    }
            //    else if (tooltype == "CogIDTool")
            //    {
            //        var idtool = (CogIDTool)result;
            //        CogToolID tool = new CogToolID(idtool);


            //    }
            //    else if (tooltype == "CogOCRMaxTool")  // 250416 By Gyu Add
            //    {
            //        var ocrtool = (CogOCRMaxTool)result;
            //        CogToolOCR tool = new CogToolOCR(ocrtool);

            //    }
            //}
        }
        private void Form_ToolOCREdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (cogToolBlockEditV21.Subject.HasChanged)
            //{
            if (MessageBox.Show("Do you want to save ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CogSerializer.SaveObjectToFile(CogTools, filepath);
            }
            //}
        }
        private void Form_ToolOCREdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 250512, Memory
            if (CogTools != null)
            {
                CogTools.Dispose();
                CogTools = null;
            }

            if (cogOCRMaxEditV21 != null)
            {
                cogOCRMaxEditV21.Dispose();
                cogOCRMaxEditV21 = null;
            }
        }

        private void CreateToolClassify()
        {  //
            CogClassifyEditV2 classifyEditV2 = new CogClassifyEditV2();
            this.Controls.Add(classifyEditV2);
            classifyEditV2.Dock = System.Windows.Forms.DockStyle.Fill;

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
