
using Cognex.VisionPro.ViDiEL;

namespace TOPASSEMBLYMACHINE.UI
{
    partial class Form_ToolOCREdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cogOCRMaxEditV21 = new Cognex.VisionPro.OCRMax.CogOCRMaxEditV2();
            this.cogOCRMaxEditV22 = new Cognex.VisionPro.OCRMax.CogOCRMaxEditV2();
            ((System.ComponentModel.ISupportInitialize)(this.cogOCRMaxEditV21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogOCRMaxEditV22)).BeginInit();
            this.SuspendLayout();
            // 
            // cogOCRMaxEditV21
            // 
            this.cogOCRMaxEditV21.Location = new System.Drawing.Point(0, 0);
            this.cogOCRMaxEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogOCRMaxEditV21.Name = "cogOCRMaxEditV21";
            this.cogOCRMaxEditV21.Size = new System.Drawing.Size(750, 459);
            this.cogOCRMaxEditV21.SuspendElectricRuns = false;
            this.cogOCRMaxEditV21.TabIndex = 0;
            // 
            // cogOCRMaxEditV22
            // 
            this.cogOCRMaxEditV22.Location = new System.Drawing.Point(0, 0);
            this.cogOCRMaxEditV22.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogOCRMaxEditV22.Name = "cogOCRMaxEditV22";
            this.cogOCRMaxEditV22.Size = new System.Drawing.Size(750, 459);
            this.cogOCRMaxEditV22.SuspendElectricRuns = false;
            this.cogOCRMaxEditV22.TabIndex = 1;
            // 
            // Form_ToolOCREdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 450);
            this.Controls.Add(this.cogOCRMaxEditV22);
            this.Controls.Add(this.cogOCRMaxEditV21);
            this.Name = "Form_ToolOCREdit";
            this.Text = "Form_ToolOCREdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolOCREdit_FormClosing);
            this.Load += new System.EventHandler(this.Form_ToolOCREdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogOCRMaxEditV21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogOCRMaxEditV22)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Cognex.VisionPro.OCRMax.CogOCRMaxEditV2 cogOCRMaxEditV21;
        private Cognex.VisionPro.OCRMax.CogOCRMaxEditV2 cogOCRMaxEditV22;
    }
}