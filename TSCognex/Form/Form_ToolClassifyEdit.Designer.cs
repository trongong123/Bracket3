
namespace TopEng.Vision.Forms
{
    partial class Form_ToolClassifyEdit
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
            this.SuspendLayout();
            // 
            // Form_ToolClassifyEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Name = "Form_ToolClassifyEdit";
            this.Text = "Form_ToolClassifyEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolClassifyEdit_FormClosing);
            this.Load += new System.EventHandler(this.Form_ToolClassifyEdit_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Cognex.VisionPro.ViDiEL.CogClassifyEditV2 cogClassifyEditV2;
    }
}