
namespace CAMASSEMBLYMACHINE.UI
{
    partial class tabBarcodeLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tabBarcodeLog));
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Open = new NV_UI.NV_Button_PB_NS();
            this.button_Load = new NV_UI.NV_Button_PB_NS();
            this.button_Clear = new NV_UI.NV_Button_PB_NS();
            this.timerAssemble = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.LightGray;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(650, 24);
            this.label3.TabIndex = 36;
            this.label3.Text = "Barcode Log";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(650, 600);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(3, 27);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(644, 474);
            this.listBox1.TabIndex = 38;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_Open, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_Load, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_Clear, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 507);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(644, 90);
            this.tableLayoutPanel2.TabIndex = 39;
            // 
            // button_Open
            // 
            this.button_Open.BackColor = System.Drawing.Color.White;
            this.button_Open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Open.BackgroundImage")));
            this.button_Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Open.ClickImage = ((System.Drawing.Image)(resources.GetObject("button_Open.ClickImage")));
            this.button_Open.DefaultHover = ((System.Drawing.Image)(resources.GetObject("button_Open.DefaultHover")));
            this.button_Open.DefaultImage = ((System.Drawing.Image)(resources.GetObject("button_Open.DefaultImage")));
            this.button_Open.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Open.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Open.Location = new System.Drawing.Point(163, 2);
            this.button_Open.Margin = new System.Windows.Forms.Padding(2);
            this.button_Open.Name = "button_Open";
            this.button_Open.Size = new System.Drawing.Size(157, 86);
            this.button_Open.TabIndex = 32;
            this.button_Open.Tag = "Open";
            this.button_Open.Text = "Open";
            // 
            // button_Load
            // 
            this.button_Load.BackColor = System.Drawing.Color.White;
            this.button_Load.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Load.BackgroundImage")));
            this.button_Load.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Load.ClickImage = ((System.Drawing.Image)(resources.GetObject("button_Load.ClickImage")));
            this.button_Load.DefaultHover = ((System.Drawing.Image)(resources.GetObject("button_Load.DefaultHover")));
            this.button_Load.DefaultImage = ((System.Drawing.Image)(resources.GetObject("button_Load.DefaultImage")));
            this.button_Load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Load.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Load.Location = new System.Drawing.Point(485, 2);
            this.button_Load.Margin = new System.Windows.Forms.Padding(2);
            this.button_Load.Name = "button_Load";
            this.button_Load.Size = new System.Drawing.Size(157, 86);
            this.button_Load.TabIndex = 30;
            this.button_Load.Tag = "Load";
            this.button_Load.Text = "Load";
            // 
            // button_Clear
            // 
            this.button_Clear.BackColor = System.Drawing.Color.White;
            this.button_Clear.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Clear.BackgroundImage")));
            this.button_Clear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Clear.ClickImage = ((System.Drawing.Image)(resources.GetObject("button_Clear.ClickImage")));
            this.button_Clear.DefaultHover = ((System.Drawing.Image)(resources.GetObject("button_Clear.DefaultHover")));
            this.button_Clear.DefaultImage = ((System.Drawing.Image)(resources.GetObject("button_Clear.DefaultImage")));
            this.button_Clear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Clear.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_Clear.Location = new System.Drawing.Point(2, 2);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(2);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(157, 86);
            this.button_Clear.TabIndex = 29;
            this.button_Clear.Tag = "CLEAR";
            this.button_Clear.Text = "Clear";
            // 
            // timerAssemble
            // 
            this.timerAssemble.Enabled = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(325, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(155, 84);
            this.tableLayoutPanel4.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(81, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 82);
            this.label2.TabIndex = 36;
            this.label2.Text = "100";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 82);
            this.label1.TabIndex = 35;
            this.label1.Text = "Max Line";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabBarcodeLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "tabBarcodeLog";
            this.Text = "tabPositionData";
            this.Load += new System.EventHandler(this.tabUILog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Timer timerAssemble;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private NV_UI.NV_Button_PB_NS button_Load;
        private NV_UI.NV_Button_PB_NS button_Open;
        private NV_UI.NV_Button_PB_NS button_Clear;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}