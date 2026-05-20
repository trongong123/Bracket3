
namespace CAMASSEMBLYMACHINE.UI
{
    partial class Form_PaneBottom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_PaneBottom));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Manual = new TopEng.Controls.ButtonEnh();
            this.button_Auto = new TopEng.Controls.ButtonEnh();
            this.button_Exit = new TopEng.Controls.ButtonEnh();
            this.button_Teach = new TopEng.Controls.ButtonEnh();
            this.button_Data = new TopEng.Controls.ButtonEnh();
            this.button_Log = new TopEng.Controls.ButtonEnh();
            this.button_Hide = new TopEng.Controls.ButtonEnh();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.button_Manual, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Auto, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Exit, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Teach, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Data, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Log, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_Hide, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.listBox_Log, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1024, 75);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // button_Manual
            // 
            this.button_Manual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Manual.BackColor = System.Drawing.Color.Transparent;
            this.button_Manual.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Manual.BackgroundImage")));
            this.button_Manual.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Manual.ButtonPush = false;
            this.button_Manual.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Manual.FlatAppearance.BorderSize = 0;
            this.button_Manual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Manual.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Manual.ForeColor = System.Drawing.Color.Black;
            this.button_Manual.ImageButton = false;
            this.button_Manual.ImageComplete = null;
            this.button_Manual.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Manual.ImageDefault")));
            this.button_Manual.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Manual.ImageDisable")));
            this.button_Manual.ImageDisableDown = ((System.Drawing.Image)(resources.GetObject("button_Manual.ImageDisableDown")));
            this.button_Manual.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Manual.ImageDown")));
            this.button_Manual.ImageDownHOver = null;
            this.button_Manual.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Manual.ImageHOver")));
            this.button_Manual.Location = new System.Drawing.Point(85, 0);
            this.button_Manual.Margin = new System.Windows.Forms.Padding(0);
            this.button_Manual.Name = "button_Manual";
            this.button_Manual.Size = new System.Drawing.Size(85, 75);
            this.button_Manual.TabIndex = 2;
            this.button_Manual.TabStop = false;
            this.button_Manual.UseVisualStyleBackColor = false;
            this.button_Manual.Click += new System.EventHandler(this.button_Manual_Click);
            // 
            // button_Auto
            // 
            this.button_Auto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Auto.BackColor = System.Drawing.Color.Transparent;
            this.button_Auto.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Auto.BackgroundImage")));
            this.button_Auto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Auto.ButtonPush = false;
            this.button_Auto.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Auto.FlatAppearance.BorderSize = 0;
            this.button_Auto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Auto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Auto.ForeColor = System.Drawing.Color.Black;
            this.button_Auto.ImageButton = false;
            this.button_Auto.ImageComplete = null;
            this.button_Auto.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Auto.ImageDefault")));
            this.button_Auto.ImageDisable = null;
            this.button_Auto.ImageDisableDown = null;
            this.button_Auto.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Auto.ImageDown")));
            this.button_Auto.ImageDownHOver = null;
            this.button_Auto.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Auto.ImageHOver")));
            this.button_Auto.Location = new System.Drawing.Point(0, 0);
            this.button_Auto.Margin = new System.Windows.Forms.Padding(0);
            this.button_Auto.Name = "button_Auto";
            this.button_Auto.Size = new System.Drawing.Size(85, 75);
            this.button_Auto.TabIndex = 1;
            this.button_Auto.TabStop = false;
            this.button_Auto.UseVisualStyleBackColor = false;
            this.button_Auto.Click += new System.EventHandler(this.button_Auto_Click);
            // 
            // button_Exit
            // 
            this.button_Exit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Exit.BackColor = System.Drawing.Color.Transparent;
            this.button_Exit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Exit.BackgroundImage")));
            this.button_Exit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Exit.ButtonPush = false;
            this.button_Exit.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Exit.FlatAppearance.BorderSize = 0;
            this.button_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Exit.ForeColor = System.Drawing.Color.Black;
            this.button_Exit.ImageButton = true;
            this.button_Exit.ImageComplete = null;
            this.button_Exit.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Exit.ImageDefault")));
            this.button_Exit.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Exit.ImageDisable")));
            this.button_Exit.ImageDisableDown = null;
            this.button_Exit.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Exit.ImageDown")));
            this.button_Exit.ImageDownHOver = null;
            this.button_Exit.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Exit.ImageHOver")));
            this.button_Exit.Location = new System.Drawing.Point(939, 0);
            this.button_Exit.Margin = new System.Windows.Forms.Padding(0);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new System.Drawing.Size(85, 75);
            this.button_Exit.TabIndex = 3;
            this.button_Exit.TabStop = false;
            this.button_Exit.UseVisualStyleBackColor = false;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // button_Teach
            // 
            this.button_Teach.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Teach.BackColor = System.Drawing.Color.Transparent;
            this.button_Teach.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Teach.BackgroundImage")));
            this.button_Teach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Teach.ButtonPush = false;
            this.button_Teach.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Teach.FlatAppearance.BorderSize = 0;
            this.button_Teach.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Teach.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Teach.ForeColor = System.Drawing.Color.Black;
            this.button_Teach.ImageButton = false;
            this.button_Teach.ImageComplete = null;
            this.button_Teach.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Teach.ImageDefault")));
            this.button_Teach.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Teach.ImageDisable")));
            this.button_Teach.ImageDisableDown = ((System.Drawing.Image)(resources.GetObject("button_Teach.ImageDisableDown")));
            this.button_Teach.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Teach.ImageDown")));
            this.button_Teach.ImageDownHOver = null;
            this.button_Teach.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Teach.ImageHOver")));
            this.button_Teach.Location = new System.Drawing.Point(255, 0);
            this.button_Teach.Margin = new System.Windows.Forms.Padding(0);
            this.button_Teach.Name = "button_Teach";
            this.button_Teach.Size = new System.Drawing.Size(85, 75);
            this.button_Teach.TabIndex = 3;
            this.button_Teach.TabStop = false;
            this.button_Teach.UseVisualStyleBackColor = false;
            this.button_Teach.Click += new System.EventHandler(this.button_Teach_Click);
            // 
            // button_Data
            // 
            this.button_Data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Data.BackColor = System.Drawing.Color.Transparent;
            this.button_Data.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Data.BackgroundImage")));
            this.button_Data.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Data.ButtonPush = false;
            this.button_Data.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Data.FlatAppearance.BorderSize = 0;
            this.button_Data.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Data.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Data.ForeColor = System.Drawing.Color.Black;
            this.button_Data.ImageButton = false;
            this.button_Data.ImageComplete = null;
            this.button_Data.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Data.ImageDefault")));
            this.button_Data.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Data.ImageDisable")));
            this.button_Data.ImageDisableDown = ((System.Drawing.Image)(resources.GetObject("button_Data.ImageDisableDown")));
            this.button_Data.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Data.ImageDown")));
            this.button_Data.ImageDownHOver = null;
            this.button_Data.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Data.ImageHOver")));
            this.button_Data.Location = new System.Drawing.Point(170, 0);
            this.button_Data.Margin = new System.Windows.Forms.Padding(0);
            this.button_Data.Name = "button_Data";
            this.button_Data.Size = new System.Drawing.Size(85, 75);
            this.button_Data.TabIndex = 3;
            this.button_Data.TabStop = false;
            this.button_Data.UseVisualStyleBackColor = false;
            this.button_Data.Click += new System.EventHandler(this.button_Data_Click);
            // 
            // button_Log
            // 
            this.button_Log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Log.BackColor = System.Drawing.Color.Transparent;
            this.button_Log.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Log.BackgroundImage")));
            this.button_Log.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Log.ButtonPush = false;
            this.button_Log.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Log.FlatAppearance.BorderSize = 0;
            this.button_Log.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Log.ForeColor = System.Drawing.Color.Black;
            this.button_Log.ImageButton = false;
            this.button_Log.ImageComplete = null;
            this.button_Log.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Log.ImageDefault")));
            this.button_Log.ImageDisable = null;
            this.button_Log.ImageDisableDown = null;
            this.button_Log.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Log.ImageDown")));
            this.button_Log.ImageDownHOver = null;
            this.button_Log.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Log.ImageHOver")));
            this.button_Log.Location = new System.Drawing.Point(769, 0);
            this.button_Log.Margin = new System.Windows.Forms.Padding(0);
            this.button_Log.Name = "button_Log";
            this.button_Log.Size = new System.Drawing.Size(85, 75);
            this.button_Log.TabIndex = 3;
            this.button_Log.TabStop = false;
            this.button_Log.UseVisualStyleBackColor = false;
            this.button_Log.Click += new System.EventHandler(this.button_Log_Click);
            // 
            // button_Hide
            // 
            this.button_Hide.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Hide.BackColor = System.Drawing.Color.Transparent;
            this.button_Hide.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Hide.BackgroundImage")));
            this.button_Hide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Hide.ButtonPush = false;
            this.button_Hide.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Hide.FlatAppearance.BorderSize = 0;
            this.button_Hide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Hide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Hide.ForeColor = System.Drawing.Color.Black;
            this.button_Hide.ImageButton = true;
            this.button_Hide.ImageComplete = null;
            this.button_Hide.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Hide.ImageDefault")));
            this.button_Hide.ImageDisable = null;
            this.button_Hide.ImageDisableDown = null;
            this.button_Hide.ImageDown = null;
            this.button_Hide.ImageDownHOver = null;
            this.button_Hide.ImageHOver = null;
            this.button_Hide.Location = new System.Drawing.Point(854, 0);
            this.button_Hide.Margin = new System.Windows.Forms.Padding(0);
            this.button_Hide.Name = "button_Hide";
            this.button_Hide.Size = new System.Drawing.Size(85, 75);
            this.button_Hide.TabIndex = 3;
            this.button_Hide.TabStop = false;
            this.button_Hide.UseVisualStyleBackColor = false;
            this.button_Hide.Click += new System.EventHandler(this.button_Hide_Click);
            // 
            // listBox_Log
            // 
            this.listBox_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_Log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox_Log.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.Location = new System.Drawing.Point(342, 2);
            this.listBox_Log.Margin = new System.Windows.Forms.Padding(2);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.Size = new System.Drawing.Size(425, 67);
            this.listBox_Log.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form_PaneBottom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 75);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_PaneBottom";
            this.Text = "Form_PaneBottom";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TopEng.Controls.ButtonEnh button_Auto;
        private TopEng.Controls.ButtonEnh button_Teach;
        private TopEng.Controls.ButtonEnh button_Manual;
        private TopEng.Controls.ButtonEnh button_Exit;
        private TopEng.Controls.ButtonEnh button_Log;
        private TopEng.Controls.ButtonEnh button_Data;
        private TopEng.Controls.ButtonEnh button_Hide;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.Timer timer1;
    }
}