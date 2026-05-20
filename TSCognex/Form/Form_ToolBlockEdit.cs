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

namespace TopEng.Vision.Forms
{
    public partial class Form_ToolBlockEdit : Form
    {
        public string filepath;
        public ICogImage image;
        string strToolName;
        int nInspNo;

        public Form_ToolBlockEdit(string Filepath, ICogImage Image)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;
        }

        public Form_ToolBlockEdit(string Filepath, ICogImage Image, string toolname = null, int nToolNo = 0)
        {
            InitializeComponent();
            filepath = Filepath;
            image = Image;
            strToolName = toolname;
            nInspNo = nToolNo;
        }

        CogToolBlock CogToolsBlock = new CogToolBlock();
        private void Form_ToolBlockEdit_Load(object sender, EventArgs e)
        {
            try
            {
                var ToolBlock = CogSerializer.LoadObjectFromFile(filepath) as CogToolBlock;
                ToolBlock.Inputs["Input"].Value = image;
                //
                if (strToolName == null)
                    cogToolBlockEditV21.Subject = ToolBlock;
                else
                    cogToolBlockEditV21.Subject = ToolBlock.Tools[strToolName] as CogToolBlock;
                CogToolsBlock = ToolBlock;
                cogToolBlockEditV21.Subject.Run();
            }
            catch
            {

            }
        }

        private void Form_ToolBlockEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cogToolBlockEditV21.Subject.HasChanged)
            {
                if (MessageBox.Show("Do you want to save ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    CogSerializer.SaveObjectToFile(CogToolsBlock, filepath);
                }
            }
        }

        private void Form_ToolBlockEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 250512, Memory
            if (cogToolBlockEditV21 != null)
            {
                cogToolBlockEditV21.Dispose();
                cogToolBlockEditV21 = null;
            }
        }
    }
}
