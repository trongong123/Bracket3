
namespace CAMASSEMBLYMACHINE.UI
{
    partial class Form_Log
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Log));
            this.panelControl = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.button_Barcode = new TopEng.Controls.ButtonEnh();
            this.btnErrorCount = new System.Windows.Forms.CheckBox();
            this.button_Data = new TopEng.Controls.ButtonEnh();
            this.button_Error = new TopEng.Controls.ButtonEnh();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.nV_Button_PB_NS_Title = new NV_UI.NV_Button_PB_NS();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonCurrent = new TopEng.Controls.ButtonEnh();
            this.button_NextDate = new TopEng.Controls.ButtonEnh();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.button_PreDate = new TopEng.Controls.ButtonEnh();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl
            // 
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(205, 66);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(622, 491);
            this.panelControl.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanelButtons, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 66);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 419F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(196, 491);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanelButtons
            // 
            this.tableLayoutPanelButtons.ColumnCount = 1;
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelButtons.Controls.Add(this.button_Barcode, 0, 2);
            this.tableLayoutPanelButtons.Controls.Add(this.btnErrorCount, 0, 6);
            this.tableLayoutPanelButtons.Controls.Add(this.button_Data, 0, 1);
            this.tableLayoutPanelButtons.Controls.Add(this.button_Error, 0, 0);
            this.tableLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelButtons.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            this.tableLayoutPanelButtons.RowCount = 7;
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelButtons.Size = new System.Drawing.Size(190, 413);
            this.tableLayoutPanelButtons.TabIndex = 1;
            // 
            // button_Barcode
            // 
            this.button_Barcode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Barcode.BackColor = System.Drawing.SystemColors.Control;
            this.button_Barcode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Barcode.BackgroundImage")));
            this.button_Barcode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Barcode.ButtonPush = false;
            this.button_Barcode.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Barcode.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_Barcode.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_Barcode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Barcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Barcode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_Barcode.ImageButton = false;
            this.button_Barcode.ImageComplete = null;
            this.button_Barcode.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Barcode.ImageDefault")));
            this.button_Barcode.ImageDisable = null;
            this.button_Barcode.ImageDisableDown = null;
            this.button_Barcode.ImageDown = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_Click_Resource;
            this.button_Barcode.ImageDownHOver = null;
            this.button_Barcode.ImageHOver = null;
            this.button_Barcode.Location = new System.Drawing.Point(3, 113);
            this.button_Barcode.Name = "button_Barcode";
            this.button_Barcode.Size = new System.Drawing.Size(184, 49);
            this.button_Barcode.TabIndex = 36;
            this.button_Barcode.TabStop = false;
            this.button_Barcode.Text = "Barcode";
            this.button_Barcode.UseVisualStyleBackColor = false;
            this.button_Barcode.Click += new System.EventHandler(this.button_Barcode_Click);
            // 
            // btnErrorCount
            // 
            this.btnErrorCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnErrorCount.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnErrorCount.AutoSize = true;
            this.btnErrorCount.BackColor = System.Drawing.Color.White;
            this.btnErrorCount.Font = new System.Drawing.Font("굴림", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnErrorCount.Location = new System.Drawing.Point(4, 352);
            this.btnErrorCount.Name = "btnErrorCount";
            this.btnErrorCount.Size = new System.Drawing.Size(181, 39);
            this.btnErrorCount.TabIndex = 35;
            this.btnErrorCount.Text = "Error Count";
            this.btnErrorCount.UseVisualStyleBackColor = false;
            this.btnErrorCount.CheckedChanged += new System.EventHandler(this.btnErrorCount_CheckedChanged);
            // 
            // button_Data
            // 
            this.button_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Data.BackColor = System.Drawing.SystemColors.Control;
            this.button_Data.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Data.BackgroundImage")));
            this.button_Data.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Data.ButtonPush = false;
            this.button_Data.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Data.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_Data.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_Data.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Data.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Data.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_Data.ImageButton = false;
            this.button_Data.ImageComplete = null;
            this.button_Data.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Data.ImageDefault")));
            this.button_Data.ImageDisable = null;
            this.button_Data.ImageDisableDown = null;
            this.button_Data.ImageDown = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_Click_Resource;
            this.button_Data.ImageDownHOver = null;
            this.button_Data.ImageHOver = null;
            this.button_Data.Location = new System.Drawing.Point(3, 58);
            this.button_Data.Name = "button_Data";
            this.button_Data.Size = new System.Drawing.Size(184, 49);
            this.button_Data.TabIndex = 31;
            this.button_Data.TabStop = false;
            this.button_Data.Text = "Data";
            this.button_Data.UseVisualStyleBackColor = false;
            this.button_Data.Click += new System.EventHandler(this.button_Data_Click);
            // 
            // button_Error
            // 
            this.button_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Error.BackColor = System.Drawing.SystemColors.Control;
            this.button_Error.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Error.BackgroundImage")));
            this.button_Error.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Error.ButtonPush = false;
            this.button_Error.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Error.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_Error.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_Error.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Error.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Error.ForeColor = System.Drawing.Color.Crimson;
            this.button_Error.ImageButton = false;
            this.button_Error.ImageComplete = null;
            this.button_Error.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Error.ImageDefault")));
            this.button_Error.ImageDisable = null;
            this.button_Error.ImageDisableDown = null;
            this.button_Error.ImageDown = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_Click_Resource;
            this.button_Error.ImageDownHOver = null;
            this.button_Error.ImageHOver = null;
            this.button_Error.Location = new System.Drawing.Point(3, 3);
            this.button_Error.Name = "button_Error";
            this.button_Error.Size = new System.Drawing.Size(184, 49);
            this.button_Error.TabIndex = 31;
            this.button_Error.TabStop = false;
            this.button_Error.Text = "Error";
            this.button_Error.UseVisualStyleBackColor = false;
            this.button_Error.Click += new System.EventHandler(this.button_Error_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.45783F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.54217F));
            this.tableLayoutPanel1.Controls.Add(this.nV_Button_PB_NS_Title, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelControl, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(830, 560);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // nV_Button_PB_NS_Title
            // 
            this.nV_Button_PB_NS_Title.BackColor = System.Drawing.Color.Transparent;
            this.nV_Button_PB_NS_Title.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.SubTitle;
            this.nV_Button_PB_NS_Title.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.nV_Button_PB_NS_Title.ClickImage = global::CAMASSEMBLYMACHINE.Properties.Resources.SubTitle;
            this.nV_Button_PB_NS_Title.DefaultHover = global::CAMASSEMBLYMACHINE.Properties.Resources.SubTitle;
            this.nV_Button_PB_NS_Title.DefaultImage = global::CAMASSEMBLYMACHINE.Properties.Resources.SubTitle;
            this.nV_Button_PB_NS_Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nV_Button_PB_NS_Title.Font = new System.Drawing.Font("맑은 고딕", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.nV_Button_PB_NS_Title.ForeColor = System.Drawing.Color.White;
            this.nV_Button_PB_NS_Title.Location = new System.Drawing.Point(6, 8);
            this.nV_Button_PB_NS_Title.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.nV_Button_PB_NS_Title.Name = "nV_Button_PB_NS_Title";
            this.nV_Button_PB_NS_Title.Size = new System.Drawing.Size(190, 47);
            this.nV_Button_PB_NS_Title.TabIndex = 37;
            this.nV_Button_PB_NS_Title.Text = "LOG";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 6;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.76942F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.099226F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.26938F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.86197F));
            this.tableLayoutPanel3.Controls.Add(this.buttonCurrent, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.button_NextDate, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.dateTimePicker1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.button_PreDate, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(205, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(622, 57);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // buttonCurrent
            // 
            this.buttonCurrent.BackColor = System.Drawing.SystemColors.Control;
            this.buttonCurrent.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_UnCheck;
            this.buttonCurrent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonCurrent.ButtonPush = false;
            this.buttonCurrent.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.buttonCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCurrent.FlatAppearance.BorderSize = 0;
            this.buttonCurrent.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonCurrent.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonCurrent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCurrent.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCurrent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonCurrent.ImageButton = false;
            this.buttonCurrent.ImageComplete = null;
            this.buttonCurrent.ImageDefault = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_UnCheck;
            this.buttonCurrent.ImageDisable = null;
            this.buttonCurrent.ImageDisableDown = null;
            this.buttonCurrent.ImageDown = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Check;
            this.buttonCurrent.ImageDownHOver = null;
            this.buttonCurrent.ImageHOver = null;
            this.buttonCurrent.Location = new System.Drawing.Point(362, 3);
            this.buttonCurrent.Name = "buttonCurrent";
            this.buttonCurrent.Size = new System.Drawing.Size(197, 51);
            this.buttonCurrent.TabIndex = 34;
            this.buttonCurrent.TabStop = false;
            this.buttonCurrent.Text = "Today";
            this.buttonCurrent.UseVisualStyleBackColor = false;
            this.buttonCurrent.Click += new System.EventHandler(this.buttonCurrent_Click);
            // 
            // button_NextDate
            // 
            this.button_NextDate.BackColor = System.Drawing.SystemColors.Control;
            this.button_NextDate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_NextDate.BackgroundImage")));
            this.button_NextDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_NextDate.ButtonPush = false;
            this.button_NextDate.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_NextDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_NextDate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_NextDate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_NextDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_NextDate.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_NextDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_NextDate.ImageButton = false;
            this.button_NextDate.ImageComplete = null;
            this.button_NextDate.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_NextDate.ImageDefault")));
            this.button_NextDate.ImageDisable = null;
            this.button_NextDate.ImageDisableDown = null;
            this.button_NextDate.ImageDown = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_Click_Resource;
            this.button_NextDate.ImageDownHOver = null;
            this.button_NextDate.ImageHOver = null;
            this.button_NextDate.Location = new System.Drawing.Point(241, 3);
            this.button_NextDate.Name = "button_NextDate";
            this.button_NextDate.Size = new System.Drawing.Size(94, 51);
            this.button_NextDate.TabIndex = 31;
            this.button_NextDate.TabStop = false;
            this.button_NextDate.Text = ">";
            this.button_NextDate.UseVisualStyleBackColor = false;
            this.button_NextDate.Click += new System.EventHandler(this.button_NextDate_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePicker1.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dateTimePicker1.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(3, 12);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(132, 33);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // button_PreDate
            // 
            this.button_PreDate.BackColor = System.Drawing.SystemColors.Control;
            this.button_PreDate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_PreDate.BackgroundImage")));
            this.button_PreDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_PreDate.ButtonPush = false;
            this.button_PreDate.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_PreDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_PreDate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_PreDate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_PreDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_PreDate.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_PreDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_PreDate.ImageButton = false;
            this.button_PreDate.ImageComplete = null;
            this.button_PreDate.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_PreDate.ImageDefault")));
            this.button_PreDate.ImageDisable = null;
            this.button_PreDate.ImageDisableDown = null;
            this.button_PreDate.ImageDown = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_Click_Resource;
            this.button_PreDate.ImageDownHOver = null;
            this.button_PreDate.ImageHOver = null;
            this.button_PreDate.Location = new System.Drawing.Point(141, 3);
            this.button_PreDate.Name = "button_PreDate";
            this.button_PreDate.Size = new System.Drawing.Size(94, 51);
            this.button_PreDate.TabIndex = 31;
            this.button_PreDate.TabStop = false;
            this.button_PreDate.Text = "<";
            this.button_PreDate.UseVisualStyleBackColor = false;
            this.button_PreDate.Click += new System.EventHandler(this.button_PreDate_Click);
            // 
            // Form_Log
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 560);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Log";
            this.Text = "Form_Data";
            this.Load += new System.EventHandler(this.Form_Data_Load);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanelButtons.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private TopEng.Controls.ButtonEnh buttonCurrent;
        private NV_UI.NV_Button_PB_NS nV_Button_PB_NS_Title;
        private TopEng.Controls.ButtonEnh button_PreDate;
        private TopEng.Controls.ButtonEnh button_NextDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private TopEng.Controls.ButtonEnh button_Data;
        private TopEng.Controls.ButtonEnh button_Error;
        private System.Windows.Forms.CheckBox btnErrorCount;
        private TopEng.Controls.ButtonEnh button_Barcode;
    }
}