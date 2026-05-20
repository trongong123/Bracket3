
namespace CAMASSEMBLYMACHINE.UI.SubForm
{
    partial class SubForm_Product
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubForm_Product));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Ok = new TopEng.Controls.ButtonEnh();
            this.button_Cancel = new TopEng.Controls.ButtonEnh();
            this.textBox_ColorCode = new System.Windows.Forms.TextBox();
            this.textBox_Model = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_IMEI = new System.Windows.Forms.TextBox();
            this.comboBox_Judge = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.button_Ok, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.button_Cancel, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.textBox_ColorCode, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox_Model, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox_IMEI, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBox_Judge, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(336, 201);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // button_Ok
            // 
            this.button_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Ok.BackColor = System.Drawing.Color.Transparent;
            this.button_Ok.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Ok.BackgroundImage")));
            this.button_Ok.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Ok.ButtonPush = false;
            this.button_Ok.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Ok.FlatAppearance.BorderSize = 0;
            this.button_Ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Ok.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Ok.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_Ok.ImageButton = true;
            this.button_Ok.ImageComplete = null;
            this.button_Ok.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Ok.ImageDefault")));
            this.button_Ok.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Ok.ImageDisable")));
            this.button_Ok.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Ok.ImageDown")));
            this.button_Ok.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_Ok.ImageDownHOver")));
            this.button_Ok.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Ok.ImageHOver")));
            this.button_Ok.Location = new System.Drawing.Point(0, 160);
            this.button_Ok.Margin = new System.Windows.Forms.Padding(0);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(168, 41);
            this.button_Ok.TabIndex = 38;
            this.button_Ok.TabStop = false;
            this.button_Ok.UseVisualStyleBackColor = false;
            this.button_Ok.Click += new System.EventHandler(this.button_Ok_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel.BackgroundImage")));
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Cancel.ButtonPush = false;
            this.button_Cancel.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Cancel.FlatAppearance.BorderSize = 0;
            this.button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Cancel.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Cancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_Cancel.ImageButton = true;
            this.button_Cancel.ImageComplete = null;
            this.button_Cancel.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Cancel.ImageDefault")));
            this.button_Cancel.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Cancel.ImageDisable")));
            this.button_Cancel.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Cancel.ImageDown")));
            this.button_Cancel.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_Cancel.ImageDownHOver")));
            this.button_Cancel.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Cancel.ImageHOver")));
            this.button_Cancel.Location = new System.Drawing.Point(168, 160);
            this.button_Cancel.Margin = new System.Windows.Forms.Padding(0);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(168, 41);
            this.button_Cancel.TabIndex = 37;
            this.button_Cancel.TabStop = false;
            this.button_Cancel.UseVisualStyleBackColor = false;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // textBox_ColorCode
            // 
            this.textBox_ColorCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ColorCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_ColorCode.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ColorCode.Location = new System.Drawing.Point(171, 83);
            this.textBox_ColorCode.Name = "textBox_ColorCode";
            this.textBox_ColorCode.Size = new System.Drawing.Size(162, 33);
            this.textBox_ColorCode.TabIndex = 35;
            this.textBox_ColorCode.DoubleClick += new System.EventHandler(this.textBox_ColorCode_DoubleClick);
            // 
            // textBox_Model
            // 
            this.textBox_Model.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Model.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Model.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_Model.Location = new System.Drawing.Point(171, 43);
            this.textBox_Model.Name = "textBox_Model";
            this.textBox_Model.Size = new System.Drawing.Size(162, 33);
            this.textBox_Model.TabIndex = 34;
            this.textBox_Model.DoubleClick += new System.EventHandler(this.textBox_Model_DoubleClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 36);
            this.label1.TabIndex = 32;
            this.label1.Text = "IMEI";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 36);
            this.label2.TabIndex = 32;
            this.label2.Text = "MODEL";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(2, 82);
            this.label3.Margin = new System.Windows.Forms.Padding(2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 36);
            this.label3.TabIndex = 32;
            this.label3.Text = "COLOR CODE";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(2, 122);
            this.label4.Margin = new System.Windows.Forms.Padding(2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 36);
            this.label4.TabIndex = 32;
            this.label4.Text = "JUDGE";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox_IMEI
            // 
            this.textBox_IMEI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_IMEI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_IMEI.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_IMEI.Location = new System.Drawing.Point(171, 3);
            this.textBox_IMEI.Name = "textBox_IMEI";
            this.textBox_IMEI.Size = new System.Drawing.Size(162, 33);
            this.textBox_IMEI.TabIndex = 33;
            this.textBox_IMEI.DoubleClick += new System.EventHandler(this.textBox_IMEI_DoubleClick);
            // 
            // comboBox_Judge
            // 
            this.comboBox_Judge.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_Judge.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_Judge.FormattingEnabled = true;
            this.comboBox_Judge.Items.AddRange(new object[] {
            "NG",
            "GOOD"});
            this.comboBox_Judge.Location = new System.Drawing.Point(171, 123);
            this.comboBox_Judge.Name = "comboBox_Judge";
            this.comboBox_Judge.Size = new System.Drawing.Size(162, 34);
            this.comboBox_Judge.TabIndex = 36;
            // 
            // SubForm_Product
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 201);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SubForm_Product";
            this.Text = "SubForm_Product";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SubForm_Product_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox_ColorCode;
        private System.Windows.Forms.TextBox textBox_Model;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_IMEI;
        private System.Windows.Forms.ComboBox comboBox_Judge;
        private TopEng.Controls.ButtonEnh button_Ok;
        private TopEng.Controls.ButtonEnh button_Cancel;
    }
}