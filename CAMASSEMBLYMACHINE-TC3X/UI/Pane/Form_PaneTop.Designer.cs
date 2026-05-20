
namespace CAMASSEMBLYMACHINE.UI
{
    partial class Form_PaneTop
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
            this.button_Department = new TopEng.Controls.ButtonEnh();
            this.button_MachineName = new TopEng.Controls.ButtonEnh();
            this.button_Version = new TopEng.Controls.ButtonEnh();
            this.button_DateTime = new TopEng.Controls.ButtonEnh();
            this.button_ModelName = new TopEng.Controls.ButtonEnh();
            this.timer_DateTime = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // button_Department
            // 
            this.button_Department.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button_Department.BackColor = System.Drawing.Color.Transparent;
            this.button_Department.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Department.ButtonPush = false;
            this.button_Department.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_Department.FlatAppearance.BorderSize = 0;
            this.button_Department.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Department.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Department.ForeColor = System.Drawing.Color.White;
            this.button_Department.ImageButton = false;
            this.button_Department.ImageComplete = null;
            this.button_Department.ImageDefault = null;
            this.button_Department.ImageDisable = null;
            this.button_Department.ImageDisableDown = null;
            this.button_Department.ImageDown = null;
            this.button_Department.ImageDownHOver = null;
            this.button_Department.ImageHOver = null;
            this.button_Department.Location = new System.Drawing.Point(1, 47);
            this.button_Department.Margin = new System.Windows.Forms.Padding(2);
            this.button_Department.Name = "button_Department";
            this.button_Department.Size = new System.Drawing.Size(312, 32);
            this.button_Department.TabIndex = 30;
            this.button_Department.TabStop = false;
            this.button_Department.Text = "Manufaturing Automation Group (MX)";
            this.button_Department.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button_Department.UseVisualStyleBackColor = false;
            this.button_Department.Click += new System.EventHandler(this.button_Department_Click);
            // 
            // button_MachineName
            // 
            this.button_MachineName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_MachineName.BackColor = System.Drawing.Color.Transparent;
            this.button_MachineName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_MachineName.ButtonPush = false;
            this.button_MachineName.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_MachineName.FlatAppearance.BorderSize = 0;
            this.button_MachineName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_MachineName.Font = new System.Drawing.Font("맑은 고딕", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_MachineName.ForeColor = System.Drawing.Color.White;
            this.button_MachineName.ImageButton = false;
            this.button_MachineName.ImageComplete = null;
            this.button_MachineName.ImageDefault = null;
            this.button_MachineName.ImageDisable = null;
            this.button_MachineName.ImageDisableDown = null;
            this.button_MachineName.ImageDown = null;
            this.button_MachineName.ImageDownHOver = null;
            this.button_MachineName.ImageHOver = null;
            this.button_MachineName.Location = new System.Drawing.Point(221, 1);
            this.button_MachineName.Margin = new System.Windows.Forms.Padding(2);
            this.button_MachineName.Name = "button_MachineName";
            this.button_MachineName.Size = new System.Drawing.Size(586, 47);
            this.button_MachineName.TabIndex = 30;
            this.button_MachineName.TabStop = false;
            this.button_MachineName.Text = "Apearance IMEI ";
            this.button_MachineName.UseVisualStyleBackColor = false;
            // 
            // button_Version
            // 
            this.button_Version.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button_Version.BackColor = System.Drawing.Color.Transparent;
            this.button_Version.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Version.ButtonPush = false;
            this.button_Version.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_Version.FlatAppearance.BorderSize = 0;
            this.button_Version.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Version.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Version.ForeColor = System.Drawing.Color.White;
            this.button_Version.ImageButton = false;
            this.button_Version.ImageComplete = null;
            this.button_Version.ImageDefault = null;
            this.button_Version.ImageDisable = null;
            this.button_Version.ImageDisableDown = null;
            this.button_Version.ImageDown = null;
            this.button_Version.ImageDownHOver = null;
            this.button_Version.ImageHOver = null;
            this.button_Version.Location = new System.Drawing.Point(670, 52);
            this.button_Version.Margin = new System.Windows.Forms.Padding(2);
            this.button_Version.Name = "button_Version";
            this.button_Version.Size = new System.Drawing.Size(350, 25);
            this.button_Version.TabIndex = 30;
            this.button_Version.TabStop = false;
            this.button_Version.Text = "[ Ver. 1.0.0.0 ]";
            this.button_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_Version.UseVisualStyleBackColor = false;
            // 
            // button_DateTime
            // 
            this.button_DateTime.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button_DateTime.BackColor = System.Drawing.Color.Transparent;
            this.button_DateTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_DateTime.ButtonPush = false;
            this.button_DateTime.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_DateTime.FlatAppearance.BorderSize = 0;
            this.button_DateTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_DateTime.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_DateTime.ForeColor = System.Drawing.Color.White;
            this.button_DateTime.ImageButton = false;
            this.button_DateTime.ImageComplete = null;
            this.button_DateTime.ImageDefault = null;
            this.button_DateTime.ImageDisable = null;
            this.button_DateTime.ImageDisableDown = null;
            this.button_DateTime.ImageDown = null;
            this.button_DateTime.ImageDownHOver = null;
            this.button_DateTime.ImageHOver = null;
            this.button_DateTime.Location = new System.Drawing.Point(826, 30);
            this.button_DateTime.Margin = new System.Windows.Forms.Padding(2);
            this.button_DateTime.Name = "button_DateTime";
            this.button_DateTime.Size = new System.Drawing.Size(194, 25);
            this.button_DateTime.TabIndex = 30;
            this.button_DateTime.TabStop = false;
            this.button_DateTime.Text = "0000 - 00 - 00, 00 : 00 : 00";
            this.button_DateTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_DateTime.UseVisualStyleBackColor = false;
            // 
            // button_ModelName
            // 
            this.button_ModelName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button_ModelName.BackColor = System.Drawing.Color.Transparent;
            this.button_ModelName.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_ModelName.ButtonPush = false;
            this.button_ModelName.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_ModelName.FlatAppearance.BorderSize = 0;
            this.button_ModelName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ModelName.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_ModelName.ForeColor = System.Drawing.Color.White;
            this.button_ModelName.ImageButton = false;
            this.button_ModelName.ImageComplete = null;
            this.button_ModelName.ImageDefault = null;
            this.button_ModelName.ImageDisable = null;
            this.button_ModelName.ImageDisableDown = null;
            this.button_ModelName.ImageDown = null;
            this.button_ModelName.ImageDownHOver = null;
            this.button_ModelName.ImageHOver = null;
            this.button_ModelName.Location = new System.Drawing.Point(837, 0);
            this.button_ModelName.Margin = new System.Windows.Forms.Padding(2);
            this.button_ModelName.Name = "button_ModelName";
            this.button_ModelName.Size = new System.Drawing.Size(183, 30);
            this.button_ModelName.TabIndex = 30;
            this.button_ModelName.TabStop = false;
            this.button_ModelName.Text = "Model : Name";
            this.button_ModelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_ModelName.UseVisualStyleBackColor = false;
            // 
            // timer_DateTime
            // 
            this.timer_DateTime.Enabled = true;
            this.timer_DateTime.Interval = 400;
            this.timer_DateTime.Tick += new System.EventHandler(this.timer_DateTime_Tick);
            // 
            // Form_PaneTop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Navy;
            this.ClientSize = new System.Drawing.Size(1024, 80);
            this.Controls.Add(this.button_MachineName);
            this.Controls.Add(this.button_ModelName);
            this.Controls.Add(this.button_DateTime);
            this.Controls.Add(this.button_Version);
            this.Controls.Add(this.button_Department);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Form_PaneTop";
            this.Text = "Form_PaneTop";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputText_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private TopEng.Controls.ButtonEnh button_Department;
        private TopEng.Controls.ButtonEnh button_MachineName;
        private TopEng.Controls.ButtonEnh button_Version;
        private TopEng.Controls.ButtonEnh button_DateTime;
        private TopEng.Controls.ButtonEnh button_ModelName;
        private System.Windows.Forms.Timer timer_DateTime;
    }
}