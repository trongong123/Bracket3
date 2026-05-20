
namespace TopEng.Vision.Forms
{
    partial class Form_CogView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_CogView));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label_PosRight = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label_PosLeft = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label_Title = new System.Windows.Forms.Label();
            this.button_FullScreen = new TopEng.Controls.ButtonEnh();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cogRecordDisplay1 = new Cognex.VisionPro.CogRecordDisplay();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label_Title, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_FullScreen, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(442, 38);
            this.tableLayoutPanel2.TabIndex = 941;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel3.Controls.Add(this.label_PosRight, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label15, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label_PosLeft, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label14, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(91, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(215, 32);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // label_PosRight
            // 
            this.label_PosRight.AutoSize = true;
            this.label_PosRight.BackColor = System.Drawing.Color.White;
            this.label_PosRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_PosRight.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_PosRight.Location = new System.Drawing.Point(24, 16);
            this.label_PosRight.Name = "label_PosRight";
            this.label_PosRight.Size = new System.Drawing.Size(188, 16);
            this.label_PosRight.TabIndex = 37;
            this.label_PosRight.Text = "{-10,000}, {-10.000}, {-10.000}";
            this.label_PosRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.LightGray;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(0, 16);
            this.label15.Margin = new System.Windows.Forms.Padding(0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(21, 16);
            this.label15.TabIndex = 36;
            this.label15.Text = "R";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_PosLeft
            // 
            this.label_PosLeft.AutoSize = true;
            this.label_PosLeft.BackColor = System.Drawing.Color.White;
            this.label_PosLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_PosLeft.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_PosLeft.Location = new System.Drawing.Point(24, 0);
            this.label_PosLeft.Name = "label_PosLeft";
            this.label_PosLeft.Size = new System.Drawing.Size(188, 16);
            this.label_PosLeft.TabIndex = 34;
            this.label_PosLeft.Text = "{-10,000}, {-10.000}, {-10.000}";
            this.label_PosLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.LightGray;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(21, 16);
            this.label14.TabIndex = 33;
            this.label14.Text = "L";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Title.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Title.Location = new System.Drawing.Point(3, 0);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(82, 38);
            this.label_Title.TabIndex = 1;
            this.label_Title.Text = "No Camera";
            this.label_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_FullScreen
            // 
            this.button_FullScreen.BackColor = System.Drawing.SystemColors.Control;
            this.button_FullScreen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_FullScreen.BackgroundImage")));
            this.button_FullScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_FullScreen.ButtonPush = false;
            this.button_FullScreen.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_FullScreen.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_FullScreen.FlatAppearance.BorderSize = 0;
            this.button_FullScreen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_FullScreen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_FullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_FullScreen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_FullScreen.ImageButton = true;
            this.button_FullScreen.ImageComplete = null;
            this.button_FullScreen.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_FullScreen.ImageDefault")));
            this.button_FullScreen.ImageDisable = null;
            this.button_FullScreen.ImageDisableDown = null;
            this.button_FullScreen.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_FullScreen.ImageDown")));
            this.button_FullScreen.ImageDownHOver = null;
            this.button_FullScreen.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_FullScreen.ImageHOver")));
            this.button_FullScreen.Location = new System.Drawing.Point(312, 3);
            this.button_FullScreen.Name = "button_FullScreen";
            this.button_FullScreen.Size = new System.Drawing.Size(127, 32);
            this.button_FullScreen.TabIndex = 2;
            this.button_FullScreen.TabStop = false;
            this.button_FullScreen.UseVisualStyleBackColor = false;
            this.button_FullScreen.Click += new System.EventHandler(this.button_FullScreen_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cogRecordDisplay1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(444, 299);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cogRecordDisplay1
            // 
            this.cogRecordDisplay1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cogRecordDisplay1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay1.ColorMapLowerRoiLimit = 0D;
            this.cogRecordDisplay1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecordDisplay1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay1.ColorMapUpperRoiLimit = 1D;
            this.cogRecordDisplay1.DoubleTapZoomCycleLength = 2;
            this.cogRecordDisplay1.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecordDisplay1.Location = new System.Drawing.Point(0, 40);
            this.cogRecordDisplay1.Margin = new System.Windows.Forms.Padding(0);
            this.cogRecordDisplay1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecordDisplay1.MouseWheelSensitivity = 1D;
            this.cogRecordDisplay1.Name = "cogRecordDisplay1";
            this.cogRecordDisplay1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecordDisplay1.OcxState")));
            this.cogRecordDisplay1.Size = new System.Drawing.Size(444, 259);
            this.cogRecordDisplay1.TabIndex = 942;
            // 
            // Form_CogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 299);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_CogView";
            this.Text = "Form_CogView";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Cognex.VisionPro.CogRecordDisplay cogRecordDisplay1;
        private TopEng.Controls.ButtonEnh button_FullScreen;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label_PosLeft;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label_PosRight;
        private System.Windows.Forms.Timer timer1;
    }
}