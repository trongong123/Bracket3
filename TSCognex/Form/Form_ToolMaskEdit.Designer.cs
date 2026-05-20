
namespace TOPASSEMBLYMACHINE.UI
{
    partial class Form_ToolMaskEdit
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
            this.cogMaskCreatorEditV22 = new Cognex.VisionPro.ImageProcessing.CogMaskCreatorEditV2();
            ((System.ComponentModel.ISupportInitialize)(this.cogMaskCreatorEditV22)).BeginInit();
            this.SuspendLayout();
            // 
            // cogMaskCreatorEditV22
            // 
            this.cogMaskCreatorEditV22.Location = new System.Drawing.Point(12, 12);
            this.cogMaskCreatorEditV22.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogMaskCreatorEditV22.Name = "cogMaskCreatorEditV22";
            this.cogMaskCreatorEditV22.Size = new System.Drawing.Size(748, 433);
            this.cogMaskCreatorEditV22.SuspendElectricRuns = false;
            this.cogMaskCreatorEditV22.TabIndex = 0;
            this.cogMaskCreatorEditV22.Load += new System.EventHandler(this.cogMaskCreatorEditV22_Load);
            // 
            // Form_ToolMaskEdit
            // 
            this.ClientSize = new System.Drawing.Size(778, 464);
            this.Controls.Add(this.cogMaskCreatorEditV22);
            this.Name = "Form_ToolMaskEdit";
            ((System.ComponentModel.ISupportInitialize)(this.cogMaskCreatorEditV22)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Cognex.VisionPro.ImageProcessing.CogMaskCreatorEditV2 cogMaskCreatorEditV21;
        private Cognex.VisionPro.ImageProcessing.CogMaskCreatorEditV2 cogMaskCreatorEditV22;
    }
}