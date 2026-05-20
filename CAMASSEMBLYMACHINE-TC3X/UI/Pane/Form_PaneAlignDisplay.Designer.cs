
namespace CAMASSEMBLYMACHINE.UI
{
    partial class Form_PaneAlignDisplay
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
            // Form_PaneAlignDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 350);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_PaneAlignDisplay";
            this.Text = "Form_PaneAlignDisplay";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_PaneAlignDisplay_Paint);
            this.Resize += new System.EventHandler(this.Form_PaneAlignDisplay_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}