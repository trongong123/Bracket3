namespace TopEng.Controls
{
    partial class Dlg_MessageBox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dlg_MessageBox));
            this.metroButton_NO = new System.Windows.Forms.Button();
            this.metroButton_YES = new System.Windows.Forms.Button();
            this.label_Msg = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_Path = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // metroButton_NO
            // 
            this.metroButton_NO.BackColor = System.Drawing.Color.White;
            this.metroButton_NO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.metroButton_NO.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.metroButton_NO.Location = new System.Drawing.Point(201, 194);
            this.metroButton_NO.Name = "metroButton_NO";
            this.metroButton_NO.Size = new System.Drawing.Size(136, 63);
            this.metroButton_NO.TabIndex = 71;
            this.metroButton_NO.Text = "NO";
            this.metroButton_NO.UseVisualStyleBackColor = false;
            this.metroButton_NO.Click += new System.EventHandler(this.button_NO_Click);
            // 
            // metroButton_YES
            // 
            this.metroButton_YES.BackColor = System.Drawing.Color.White;
            this.metroButton_YES.Cursor = System.Windows.Forms.Cursors.Hand;
            this.metroButton_YES.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.metroButton_YES.Font = new System.Drawing.Font("맑은 고딕", 12F);
            this.metroButton_YES.Location = new System.Drawing.Point(50, 194);
            this.metroButton_YES.Name = "metroButton_YES";
            this.metroButton_YES.Size = new System.Drawing.Size(136, 63);
            this.metroButton_YES.TabIndex = 70;
            this.metroButton_YES.Text = "YES";
            this.metroButton_YES.UseVisualStyleBackColor = false;
            this.metroButton_YES.Click += new System.EventHandler(this.button_YES_Click);
            // 
            // label_Msg
            // 
            this.label_Msg.BackColor = System.Drawing.Color.Transparent;
            this.label_Msg.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Msg.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_Msg.Location = new System.Drawing.Point(7, 25);
            this.label_Msg.Name = "label_Msg";
            this.label_Msg.Size = new System.Drawing.Size(377, 114);
            this.label_Msg.TabIndex = 69;
            this.label_Msg.Text = "Do you want to Exit this Program?";
            this.label_Msg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label_Path
            // 
            this.label_Path.BackColor = System.Drawing.Color.Transparent;
            this.label_Path.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_Path.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label_Path.Location = new System.Drawing.Point(8, 139);
            this.label_Path.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Path.Name = "label_Path";
            this.label_Path.Size = new System.Drawing.Size(373, 37);
            this.label_Path.TabIndex = 72;
            this.label_Path.Text = "C:\\FA\\CAMASSEMBLYMACHINE-WC\\Recipe\\M3\\recipeinfo.json";
            this.label_Path.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Dlg_MessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(388, 305);
            this.Controls.Add(this.label_Path);
            this.Controls.Add(this.metroButton_NO);
            this.Controls.Add(this.metroButton_YES);
            this.Controls.Add(this.label_Msg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dlg_MessageBox";
            this.Padding = new System.Windows.Forms.Padding(20, 60, 20, 20);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MessageDlg_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Button metroButton_NO;
        public System.Windows.Forms.Button metroButton_YES;
        public System.Windows.Forms.Label label_Msg;
        public System.Windows.Forms.Label label_Path;
    }
}