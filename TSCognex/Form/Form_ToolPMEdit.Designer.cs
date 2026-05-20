
namespace TOPASSEMBLYMACHINE.UI
{
    partial class Form_ToolPMEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ToolPMEdit));
            this.cogPMAlignEditV22 = new Cognex.VisionPro.PMAlign.CogPMAlignEditV2();
            this.button_Run = new TopEng.Controls.ButtonEnh();
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEditV22)).BeginInit();
            this.SuspendLayout();
            // 
            // cogPMAlignEditV22
            // 
            this.cogPMAlignEditV22.Location = new System.Drawing.Point(3, 5);
            this.cogPMAlignEditV22.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogPMAlignEditV22.Name = "cogPMAlignEditV22";
            this.cogPMAlignEditV22.Size = new System.Drawing.Size(748, 456);
            this.cogPMAlignEditV22.SuspendElectricRuns = false;
            this.cogPMAlignEditV22.TabIndex = 0;
            // 
            // button_Run
            // 
            this.button_Run.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Run.BackColor = System.Drawing.Color.Transparent;
            this.button_Run.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Run.BackgroundImage")));
            this.button_Run.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Run.ButtonPush = false;
            this.button_Run.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Run.FlatAppearance.BorderSize = 0;
            this.button_Run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Run.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Run.ForeColor = System.Drawing.Color.Black;
            this.button_Run.ImageButton = false;
            this.button_Run.ImageComplete = null;
            this.button_Run.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Run.ImageDefault")));
            this.button_Run.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Run.ImageDisable")));
            this.button_Run.ImageDisableDown = null;
            this.button_Run.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Run.ImageDown")));
            this.button_Run.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_Run.ImageDownHOver")));
            this.button_Run.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Run.ImageHOver")));
            this.button_Run.Location = new System.Drawing.Point(766, 23);
            this.button_Run.Margin = new System.Windows.Forms.Padding(2);
            this.button_Run.Name = "button_Run";
            this.button_Run.Size = new System.Drawing.Size(74, 38);
            this.button_Run.TabIndex = 33;
            this.button_Run.TabStop = false;
            this.button_Run.Text = "Run";
            this.button_Run.UseVisualStyleBackColor = false;
            this.button_Run.Click += new System.EventHandler(this.button_Run_Click);
            // 
            // Form_ToolPMEdit
            // 
            this.ClientSize = new System.Drawing.Size(851, 450);
            this.Controls.Add(this.button_Run);
            this.Controls.Add(this.cogPMAlignEditV22);
            this.Name = "Form_ToolPMEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ToolPMEdit_FormClosing);
            this.Load += new System.EventHandler(this.Form_ToolPMEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEditV22)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Cognex.VisionPro.PMAlign.CogPMAlignEditV2 cogPMAlignEditV22;
        private TopEng.Controls.ButtonEnh button_Run;
    }
}