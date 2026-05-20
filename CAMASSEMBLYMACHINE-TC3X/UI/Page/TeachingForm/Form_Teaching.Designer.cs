
namespace CAMASSEMBLYMACHINE.UI
{
    partial class Form_Teaching
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Teaching));
            this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.button_LeftBuffer = new TopEng.Controls.ButtonEnh();
            this.button_Tray = new TopEng.Controls.ButtonEnh();
            this.button_PROD_LOADER = new TopEng.Controls.ButtonEnh();
            this.button_RightBuffer = new TopEng.Controls.ButtonEnh();
            this.button_ASSEMBLER = new TopEng.Controls.ButtonEnh();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlPicture = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelControl = new System.Windows.Forms.Panel();
            this.button_Jig = new TopEng.Controls.ButtonEnh();
            this.tableLayoutPanelButtons.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControlPicture.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelButtons
            // 
            this.tableLayoutPanelButtons.ColumnCount = 3;
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelButtons.Controls.Add(this.button_LeftBuffer, 0, 1);
            this.tableLayoutPanelButtons.Controls.Add(this.button_Tray, 0, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.button_PROD_LOADER, 1, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.button_RightBuffer, 1, 1);
            this.tableLayoutPanelButtons.Controls.Add(this.button_ASSEMBLER, 2, 0);
            this.tableLayoutPanelButtons.Controls.Add(this.button_Jig, 2, 1);
            this.tableLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelButtons.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
            this.tableLayoutPanelButtons.RowCount = 3;
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanelButtons.Size = new System.Drawing.Size(220, 241);
            this.tableLayoutPanelButtons.TabIndex = 1;
            // 
            // button_LeftBuffer
            // 
            this.button_LeftBuffer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_LeftBuffer.BackColor = System.Drawing.Color.Transparent;
            this.button_LeftBuffer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_LeftBuffer.BackgroundImage")));
            this.button_LeftBuffer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_LeftBuffer.ButtonPush = false;
            this.button_LeftBuffer.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_LeftBuffer.FlatAppearance.BorderSize = 0;
            this.button_LeftBuffer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_LeftBuffer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_LeftBuffer.ForeColor = System.Drawing.Color.Black;
            this.button_LeftBuffer.ImageButton = false;
            this.button_LeftBuffer.ImageComplete = null;
            this.button_LeftBuffer.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_LeftBuffer.ImageDefault")));
            this.button_LeftBuffer.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_LeftBuffer.ImageDisable")));
            this.button_LeftBuffer.ImageDisableDown = null;
            this.button_LeftBuffer.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_LeftBuffer.ImageDown")));
            this.button_LeftBuffer.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_LeftBuffer.ImageDownHOver")));
            this.button_LeftBuffer.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_LeftBuffer.ImageHOver")));
            this.button_LeftBuffer.Location = new System.Drawing.Point(2, 82);
            this.button_LeftBuffer.Margin = new System.Windows.Forms.Padding(2);
            this.button_LeftBuffer.Name = "button_LeftBuffer";
            this.button_LeftBuffer.Size = new System.Drawing.Size(69, 76);
            this.button_LeftBuffer.TabIndex = 31;
            this.button_LeftBuffer.TabStop = false;
            this.button_LeftBuffer.Text = "Left \r\nBuffer";
            this.button_LeftBuffer.UseVisualStyleBackColor = false;
            this.button_LeftBuffer.Click += new System.EventHandler(this.button_LeftBuffer_Click);
            // 
            // button_Tray
            // 
            this.button_Tray.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Tray.BackColor = System.Drawing.Color.Transparent;
            this.button_Tray.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Tray.BackgroundImage")));
            this.button_Tray.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Tray.ButtonPush = false;
            this.button_Tray.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Tray.FlatAppearance.BorderSize = 0;
            this.button_Tray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Tray.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Tray.ForeColor = System.Drawing.Color.Black;
            this.button_Tray.ImageButton = false;
            this.button_Tray.ImageComplete = null;
            this.button_Tray.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Tray.ImageDefault")));
            this.button_Tray.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Tray.ImageDisable")));
            this.button_Tray.ImageDisableDown = null;
            this.button_Tray.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Tray.ImageDown")));
            this.button_Tray.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_Tray.ImageDownHOver")));
            this.button_Tray.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Tray.ImageHOver")));
            this.button_Tray.Location = new System.Drawing.Point(2, 2);
            this.button_Tray.Margin = new System.Windows.Forms.Padding(2);
            this.button_Tray.Name = "button_Tray";
            this.button_Tray.Size = new System.Drawing.Size(69, 76);
            this.button_Tray.TabIndex = 26;
            this.button_Tray.TabStop = false;
            this.button_Tray.Text = "Tray";
            this.button_Tray.UseVisualStyleBackColor = false;
            this.button_Tray.Click += new System.EventHandler(this.button_Tray_Click);
            // 
            // button_PROD_LOADER
            // 
            this.button_PROD_LOADER.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_PROD_LOADER.BackColor = System.Drawing.Color.Transparent;
            this.button_PROD_LOADER.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_PROD_LOADER.BackgroundImage")));
            this.button_PROD_LOADER.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_PROD_LOADER.ButtonPush = false;
            this.button_PROD_LOADER.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_PROD_LOADER.FlatAppearance.BorderSize = 0;
            this.button_PROD_LOADER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_PROD_LOADER.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_PROD_LOADER.ForeColor = System.Drawing.Color.Black;
            this.button_PROD_LOADER.ImageButton = false;
            this.button_PROD_LOADER.ImageComplete = null;
            this.button_PROD_LOADER.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_PROD_LOADER.ImageDefault")));
            this.button_PROD_LOADER.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_PROD_LOADER.ImageDisable")));
            this.button_PROD_LOADER.ImageDisableDown = null;
            this.button_PROD_LOADER.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_PROD_LOADER.ImageDown")));
            this.button_PROD_LOADER.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_PROD_LOADER.ImageDownHOver")));
            this.button_PROD_LOADER.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_PROD_LOADER.ImageHOver")));
            this.button_PROD_LOADER.Location = new System.Drawing.Point(75, 2);
            this.button_PROD_LOADER.Margin = new System.Windows.Forms.Padding(2);
            this.button_PROD_LOADER.Name = "button_PROD_LOADER";
            this.button_PROD_LOADER.Size = new System.Drawing.Size(69, 76);
            this.button_PROD_LOADER.TabIndex = 28;
            this.button_PROD_LOADER.TabStop = false;
            this.button_PROD_LOADER.Text = "CAM Picker\r\n";
            this.button_PROD_LOADER.UseVisualStyleBackColor = false;
            this.button_PROD_LOADER.Click += new System.EventHandler(this.button_PROD_LOADER_Click);
            // 
            // button_RightBuffer
            // 
            this.button_RightBuffer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_RightBuffer.BackColor = System.Drawing.Color.Transparent;
            this.button_RightBuffer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RightBuffer.BackgroundImage")));
            this.button_RightBuffer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_RightBuffer.ButtonPush = false;
            this.button_RightBuffer.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_RightBuffer.FlatAppearance.BorderSize = 0;
            this.button_RightBuffer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_RightBuffer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RightBuffer.ForeColor = System.Drawing.Color.Black;
            this.button_RightBuffer.ImageButton = false;
            this.button_RightBuffer.ImageComplete = null;
            this.button_RightBuffer.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_RightBuffer.ImageDefault")));
            this.button_RightBuffer.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_RightBuffer.ImageDisable")));
            this.button_RightBuffer.ImageDisableDown = null;
            this.button_RightBuffer.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_RightBuffer.ImageDown")));
            this.button_RightBuffer.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_RightBuffer.ImageDownHOver")));
            this.button_RightBuffer.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_RightBuffer.ImageHOver")));
            this.button_RightBuffer.Location = new System.Drawing.Point(75, 82);
            this.button_RightBuffer.Margin = new System.Windows.Forms.Padding(2);
            this.button_RightBuffer.Name = "button_RightBuffer";
            this.button_RightBuffer.Size = new System.Drawing.Size(69, 76);
            this.button_RightBuffer.TabIndex = 32;
            this.button_RightBuffer.TabStop = false;
            this.button_RightBuffer.Text = "Right\r\nBuffer";
            this.button_RightBuffer.UseVisualStyleBackColor = false;
            this.button_RightBuffer.Click += new System.EventHandler(this.button_RightBuffer_Click);
            // 
            // button_ASSEMBLER
            // 
            this.button_ASSEMBLER.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ASSEMBLER.BackColor = System.Drawing.Color.Transparent;
            this.button_ASSEMBLER.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ASSEMBLER.BackgroundImage")));
            this.button_ASSEMBLER.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_ASSEMBLER.ButtonPush = false;
            this.button_ASSEMBLER.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_ASSEMBLER.FlatAppearance.BorderSize = 0;
            this.button_ASSEMBLER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ASSEMBLER.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ASSEMBLER.ForeColor = System.Drawing.Color.Black;
            this.button_ASSEMBLER.ImageButton = false;
            this.button_ASSEMBLER.ImageComplete = null;
            this.button_ASSEMBLER.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_ASSEMBLER.ImageDefault")));
            this.button_ASSEMBLER.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_ASSEMBLER.ImageDisable")));
            this.button_ASSEMBLER.ImageDisableDown = null;
            this.button_ASSEMBLER.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_ASSEMBLER.ImageDown")));
            this.button_ASSEMBLER.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_ASSEMBLER.ImageDownHOver")));
            this.button_ASSEMBLER.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_ASSEMBLER.ImageHOver")));
            this.button_ASSEMBLER.Location = new System.Drawing.Point(148, 2);
            this.button_ASSEMBLER.Margin = new System.Windows.Forms.Padding(2);
            this.button_ASSEMBLER.Name = "button_ASSEMBLER";
            this.button_ASSEMBLER.Size = new System.Drawing.Size(70, 76);
            this.button_ASSEMBLER.TabIndex = 29;
            this.button_ASSEMBLER.TabStop = false;
            this.button_ASSEMBLER.Text = "Assembler\r\n";
            this.button_ASSEMBLER.UseVisualStyleBackColor = false;
            this.button_ASSEMBLER.Click += new System.EventHandler(this.button_ASSEMBLER_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tabControlPicture, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanelButtons, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(226, 619);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tabControlPicture
            // 
            this.tabControlPicture.Controls.Add(this.tabPage1);
            this.tabControlPicture.Controls.Add(this.tabPage2);
            this.tabControlPicture.Controls.Add(this.tabPage4);
            this.tabControlPicture.Controls.Add(this.tabPage3);
            this.tabControlPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPicture.Location = new System.Drawing.Point(3, 250);
            this.tabControlPicture.Name = "tabControlPicture";
            this.tabControlPicture.SelectedIndex = 0;
            this.tabControlPicture.Size = new System.Drawing.Size(220, 303);
            this.tabControlPicture.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage1.BackgroundImage")));
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(212, 277);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tray";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage2.BackgroundImage")));
            this.tabPage2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(214, 276);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "CAM";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.White;
            this.tabPage4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage4.BackgroundImage")));
            this.tabPage4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(214, 276);
            this.tabPage4.TabIndex = 5;
            this.tabPage4.Text = "Assembler";
            // 
            // tabPage3
            // 
            this.tabPage3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPage3.BackgroundImage")));
            this.tabPage3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(214, 276);
            this.tabPage3.TabIndex = 6;
            this.tabPage3.Text = "Jig";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.875F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.125F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelControl, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(686, 625);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panelControl
            // 
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(235, 3);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(448, 619);
            this.panelControl.TabIndex = 1;
            // 
            // button_Jig
            // 
            this.button_Jig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Jig.BackColor = System.Drawing.Color.Transparent;
            this.button_Jig.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Jig.BackgroundImage")));
            this.button_Jig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Jig.ButtonPush = false;
            this.button_Jig.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Jig.FlatAppearance.BorderSize = 0;
            this.button_Jig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Jig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Jig.ForeColor = System.Drawing.Color.Black;
            this.button_Jig.ImageButton = false;
            this.button_Jig.ImageComplete = null;
            this.button_Jig.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Jig.ImageDefault")));
            this.button_Jig.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Jig.ImageDisable")));
            this.button_Jig.ImageDisableDown = null;
            this.button_Jig.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Jig.ImageDown")));
            this.button_Jig.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_Jig.ImageDownHOver")));
            this.button_Jig.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Jig.ImageHOver")));
            this.button_Jig.Location = new System.Drawing.Point(148, 82);
            this.button_Jig.Margin = new System.Windows.Forms.Padding(2);
            this.button_Jig.Name = "button_Jig";
            this.button_Jig.Size = new System.Drawing.Size(70, 76);
            this.button_Jig.TabIndex = 34;
            this.button_Jig.TabStop = false;
            this.button_Jig.Text = "Jig";
            this.button_Jig.UseVisualStyleBackColor = false;
            this.button_Jig.Click += new System.EventHandler(this.button_Jig_Click);
            // 
            // Form_Teaching
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(686, 625);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Teaching";
            this.Text = "Page_Teaching";
            this.Load += new System.EventHandler(this.Form_Teaching_Load);
            this.tableLayoutPanelButtons.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tabControlPicture.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControlPicture;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.TabPage tabPage3;
        private TopEng.Controls.ButtonEnh button_Tray;
        private TopEng.Controls.ButtonEnh button_PROD_LOADER;
        private TopEng.Controls.ButtonEnh button_ASSEMBLER;
        private TopEng.Controls.ButtonEnh button_LeftBuffer;
        private TopEng.Controls.ButtonEnh button_RightBuffer;
        private TopEng.Controls.ButtonEnh button_Jig;
    }
}