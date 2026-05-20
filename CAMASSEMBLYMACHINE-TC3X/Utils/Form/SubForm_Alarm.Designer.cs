

namespace TopEng.Utils
{
    partial class SubForm_Alarm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubForm_Alarm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Close = new TopEng.Controls.ButtonEnh();
            this.button_Mute = new TopEng.Controls.ButtonEnh();
            this.button_Title = new TopEng.Controls.ButtonEnh();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.disp_JigCarrier = new TopEng.Controls.ButtonEnh();
            this.label12 = new System.Windows.Forms.Label();
            this.disp_JigOutBuf = new TopEng.Controls.ButtonEnh();
            this.label11 = new System.Windows.Forms.Label();
            this.disp_JigWork = new TopEng.Controls.ButtonEnh();
            this.label10 = new System.Windows.Forms.Label();
            this.disp_JigInBuf = new TopEng.Controls.ButtonEnh();
            this.label8 = new System.Windows.Forms.Label();
            this.disp_RightBuf = new TopEng.Controls.ButtonEnh();
            this.label7 = new System.Windows.Forms.Label();
            this.disp_LeftBuf = new TopEng.Controls.ButtonEnh();
            this.label5 = new System.Windows.Forms.Label();
            this.disp_TrayOut = new TopEng.Controls.ButtonEnh();
            this.label6 = new System.Windows.Forms.Label();
            this.disp_TrayOutBuf = new TopEng.Controls.ButtonEnh();
            this.label4 = new System.Windows.Forms.Label();
            this.disp_TrayInBuf = new TopEng.Controls.ButtonEnh();
            this.label3 = new System.Windows.Forms.Label();
            this.disp_TrayWork = new TopEng.Controls.ButtonEnh();
            this.label1 = new System.Windows.Forms.Label();
            this.disp_ProdLoader = new TopEng.Controls.ButtonEnh();
            this.label2 = new System.Windows.Forms.Label();
            this.disp_Assembler = new TopEng.Controls.ButtonEnh();
            this.disp_TrayIn = new TopEng.Controls.ButtonEnh();
            this.label9 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.button_Close, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_Mute, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 640);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1018, 45);
            this.tableLayoutPanel2.TabIndex = 42;
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Close.BackColor = System.Drawing.Color.Transparent;
            this.button_Close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Close.BackgroundImage")));
            this.button_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Close.ButtonPush = false;
            this.button_Close.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Close.FlatAppearance.BorderSize = 0;
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Close.ForeColor = System.Drawing.Color.White;
            this.button_Close.ImageButton = true;
            this.button_Close.ImageComplete = null;
            this.button_Close.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageDefault")));
            this.button_Close.ImageDisable = null;
            this.button_Close.ImageDisableDown = null;
            this.button_Close.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageDown")));
            this.button_Close.ImageDownHOver = null;
            this.button_Close.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageHOver")));
            this.button_Close.Location = new System.Drawing.Point(764, 2);
            this.button_Close.Margin = new System.Windows.Forms.Padding(2);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(252, 41);
            this.button_Close.TabIndex = 36;
            this.button_Close.TabStop = false;
            this.button_Close.UseVisualStyleBackColor = false;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_Mute
            // 
            this.button_Mute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Mute.BackColor = System.Drawing.Color.Transparent;
            this.button_Mute.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Mute.BackgroundImage")));
            this.button_Mute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Mute.ButtonPush = false;
            this.button_Mute.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Normal;
            this.button_Mute.FlatAppearance.BorderSize = 0;
            this.button_Mute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Mute.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Mute.ForeColor = System.Drawing.Color.White;
            this.button_Mute.ImageButton = true;
            this.button_Mute.ImageComplete = null;
            this.button_Mute.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Mute.ImageDefault")));
            this.button_Mute.ImageDisable = null;
            this.button_Mute.ImageDisableDown = null;
            this.button_Mute.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Mute.ImageDown")));
            this.button_Mute.ImageDownHOver = null;
            this.button_Mute.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Mute.ImageHOver")));
            this.button_Mute.Location = new System.Drawing.Point(510, 2);
            this.button_Mute.Margin = new System.Windows.Forms.Padding(2);
            this.button_Mute.Name = "button_Mute";
            this.button_Mute.Size = new System.Drawing.Size(250, 41);
            this.button_Mute.TabIndex = 36;
            this.button_Mute.TabStop = false;
            this.button_Mute.UseVisualStyleBackColor = false;
            this.button_Mute.Click += new System.EventHandler(this.button_Mute_Click);
            // 
            // button_Title
            // 
            this.button_Title.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Title.BackColor = System.Drawing.Color.Transparent;
            this.button_Title.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Title.BackgroundImage")));
            this.button_Title.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Title.ButtonPush = false;
            this.button_Title.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_Title.FlatAppearance.BorderSize = 0;
            this.button_Title.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Title.ForeColor = System.Drawing.Color.White;
            this.button_Title.ImageButton = true;
            this.button_Title.ImageComplete = null;
            this.button_Title.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Title.ImageDefault")));
            this.button_Title.ImageDisable = null;
            this.button_Title.ImageDisableDown = null;
            this.button_Title.ImageDown = null;
            this.button_Title.ImageDownHOver = null;
            this.button_Title.ImageHOver = null;
            this.button_Title.Location = new System.Drawing.Point(2, 2);
            this.button_Title.Margin = new System.Windows.Forms.Padding(2);
            this.button_Title.Name = "button_Title";
            this.button_Title.Size = new System.Drawing.Size(1020, 70);
            this.button_Title.TabIndex = 33;
            this.button_Title.TabStop = false;
            this.button_Title.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.button_Title, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1024, 688);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.dataGridView1, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 77);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1018, 557);
            this.tableLayoutPanel3.TabIndex = 43;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.GridColor = System.Drawing.Color.DimGray;
            this.dataGridView1.Location = new System.Drawing.Point(613, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(402, 551);
            this.dataGridView1.TabIndex = 42;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 449.2386F;
            this.Column1.HeaderText = "Code";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 221;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column2.FillWeight = 8.068221F;
            this.Column2.HeaderText = "Desc";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 120;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.disp_TrayIn);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.disp_JigCarrier);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.disp_JigOutBuf);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.disp_JigWork);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.disp_JigInBuf);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.disp_RightBuf);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.disp_LeftBuf);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.disp_TrayOut);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.disp_TrayOutBuf);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.disp_TrayInBuf);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.disp_TrayWork);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.disp_ProdLoader);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.disp_Assembler);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(604, 551);
            this.panel1.TabIndex = 43;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.White;
            this.label13.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label13.Location = new System.Drawing.Point(70, 385);
            this.label13.Margin = new System.Windows.Forms.Padding(0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 20);
            this.label13.TabIndex = 91;
            this.label13.Text = "Jig Carrier";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_JigCarrier
            // 
            this.disp_JigCarrier.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_JigCarrier.BackColor = System.Drawing.Color.Transparent;
            this.disp_JigCarrier.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_JigCarrier.BackgroundImage")));
            this.disp_JigCarrier.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_JigCarrier.ButtonPush = false;
            this.disp_JigCarrier.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_JigCarrier.FlatAppearance.BorderSize = 0;
            this.disp_JigCarrier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_JigCarrier.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_JigCarrier.ForeColor = System.Drawing.Color.Black;
            this.disp_JigCarrier.ImageButton = true;
            this.disp_JigCarrier.ImageComplete = null;
            this.disp_JigCarrier.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_JigCarrier.ImageDefault")));
            this.disp_JigCarrier.ImageDisable = null;
            this.disp_JigCarrier.ImageDisableDown = null;
            this.disp_JigCarrier.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_JigCarrier.ImageDown")));
            this.disp_JigCarrier.ImageDownHOver = null;
            this.disp_JigCarrier.ImageHOver = null;
            this.disp_JigCarrier.Location = new System.Drawing.Point(152, 374);
            this.disp_JigCarrier.Margin = new System.Windows.Forms.Padding(2);
            this.disp_JigCarrier.Name = "disp_JigCarrier";
            this.disp_JigCarrier.Size = new System.Drawing.Size(40, 40);
            this.disp_JigCarrier.TabIndex = 90;
            this.disp_JigCarrier.TabStop = false;
            this.disp_JigCarrier.UseVisualStyleBackColor = false;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.White;
            this.label12.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label12.Location = new System.Drawing.Point(249, 353);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 20);
            this.label12.TabIndex = 89;
            this.label12.Text = "Jig Out Buffer";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_JigOutBuf
            // 
            this.disp_JigOutBuf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_JigOutBuf.BackColor = System.Drawing.Color.Transparent;
            this.disp_JigOutBuf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_JigOutBuf.BackgroundImage")));
            this.disp_JigOutBuf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_JigOutBuf.ButtonPush = false;
            this.disp_JigOutBuf.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_JigOutBuf.FlatAppearance.BorderSize = 0;
            this.disp_JigOutBuf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_JigOutBuf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_JigOutBuf.ForeColor = System.Drawing.Color.Black;
            this.disp_JigOutBuf.ImageButton = true;
            this.disp_JigOutBuf.ImageComplete = null;
            this.disp_JigOutBuf.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_JigOutBuf.ImageDefault")));
            this.disp_JigOutBuf.ImageDisable = null;
            this.disp_JigOutBuf.ImageDisableDown = null;
            this.disp_JigOutBuf.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_JigOutBuf.ImageDown")));
            this.disp_JigOutBuf.ImageDownHOver = null;
            this.disp_JigOutBuf.ImageHOver = null;
            this.disp_JigOutBuf.Location = new System.Drawing.Point(206, 342);
            this.disp_JigOutBuf.Margin = new System.Windows.Forms.Padding(2);
            this.disp_JigOutBuf.Name = "disp_JigOutBuf";
            this.disp_JigOutBuf.Size = new System.Drawing.Size(40, 40);
            this.disp_JigOutBuf.TabIndex = 88;
            this.disp_JigOutBuf.TabStop = false;
            this.disp_JigOutBuf.UseVisualStyleBackColor = false;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.White;
            this.label11.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label11.Location = new System.Drawing.Point(70, 315);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 20);
            this.label11.TabIndex = 87;
            this.label11.Text = "Jig Work";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_JigWork
            // 
            this.disp_JigWork.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_JigWork.BackColor = System.Drawing.Color.Transparent;
            this.disp_JigWork.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_JigWork.BackgroundImage")));
            this.disp_JigWork.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_JigWork.ButtonPush = false;
            this.disp_JigWork.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_JigWork.FlatAppearance.BorderSize = 0;
            this.disp_JigWork.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_JigWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_JigWork.ForeColor = System.Drawing.Color.Black;
            this.disp_JigWork.ImageButton = true;
            this.disp_JigWork.ImageComplete = null;
            this.disp_JigWork.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_JigWork.ImageDefault")));
            this.disp_JigWork.ImageDisable = null;
            this.disp_JigWork.ImageDisableDown = null;
            this.disp_JigWork.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_JigWork.ImageDown")));
            this.disp_JigWork.ImageDownHOver = null;
            this.disp_JigWork.ImageHOver = null;
            this.disp_JigWork.Location = new System.Drawing.Point(152, 304);
            this.disp_JigWork.Margin = new System.Windows.Forms.Padding(2);
            this.disp_JigWork.Name = "disp_JigWork";
            this.disp_JigWork.Size = new System.Drawing.Size(40, 40);
            this.disp_JigWork.TabIndex = 86;
            this.disp_JigWork.TabStop = false;
            this.disp_JigWork.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.White;
            this.label10.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label10.Location = new System.Drawing.Point(9, 273);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 20);
            this.label10.TabIndex = 85;
            this.label10.Text = "Jig In Buffer";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_JigInBuf
            // 
            this.disp_JigInBuf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_JigInBuf.BackColor = System.Drawing.Color.Transparent;
            this.disp_JigInBuf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_JigInBuf.BackgroundImage")));
            this.disp_JigInBuf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_JigInBuf.ButtonPush = false;
            this.disp_JigInBuf.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_JigInBuf.FlatAppearance.BorderSize = 0;
            this.disp_JigInBuf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_JigInBuf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_JigInBuf.ForeColor = System.Drawing.Color.Black;
            this.disp_JigInBuf.ImageButton = true;
            this.disp_JigInBuf.ImageComplete = null;
            this.disp_JigInBuf.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_JigInBuf.ImageDefault")));
            this.disp_JigInBuf.ImageDisable = null;
            this.disp_JigInBuf.ImageDisableDown = null;
            this.disp_JigInBuf.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_JigInBuf.ImageDown")));
            this.disp_JigInBuf.ImageDownHOver = null;
            this.disp_JigInBuf.ImageHOver = null;
            this.disp_JigInBuf.Location = new System.Drawing.Point(91, 262);
            this.disp_JigInBuf.Margin = new System.Windows.Forms.Padding(2);
            this.disp_JigInBuf.Name = "disp_JigInBuf";
            this.disp_JigInBuf.Size = new System.Drawing.Size(40, 40);
            this.disp_JigInBuf.TabIndex = 84;
            this.disp_JigInBuf.TabStop = false;
            this.disp_JigInBuf.UseVisualStyleBackColor = false;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(297, 261);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 20);
            this.label8.TabIndex = 83;
            this.label8.Text = "Right Buffer";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_RightBuf
            // 
            this.disp_RightBuf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_RightBuf.BackColor = System.Drawing.Color.Transparent;
            this.disp_RightBuf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_RightBuf.BackgroundImage")));
            this.disp_RightBuf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_RightBuf.ButtonPush = false;
            this.disp_RightBuf.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_RightBuf.FlatAppearance.BorderSize = 0;
            this.disp_RightBuf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_RightBuf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_RightBuf.ForeColor = System.Drawing.Color.Black;
            this.disp_RightBuf.ImageButton = true;
            this.disp_RightBuf.ImageComplete = null;
            this.disp_RightBuf.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_RightBuf.ImageDefault")));
            this.disp_RightBuf.ImageDisable = null;
            this.disp_RightBuf.ImageDisableDown = null;
            this.disp_RightBuf.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_RightBuf.ImageDown")));
            this.disp_RightBuf.ImageDownHOver = null;
            this.disp_RightBuf.ImageHOver = null;
            this.disp_RightBuf.Location = new System.Drawing.Point(276, 219);
            this.disp_RightBuf.Margin = new System.Windows.Forms.Padding(2);
            this.disp_RightBuf.Name = "disp_RightBuf";
            this.disp_RightBuf.Size = new System.Drawing.Size(40, 40);
            this.disp_RightBuf.TabIndex = 82;
            this.disp_RightBuf.TabStop = false;
            this.disp_RightBuf.UseVisualStyleBackColor = false;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(150, 185);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 20);
            this.label7.TabIndex = 81;
            this.label7.Text = "Left Buffer";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_LeftBuf
            // 
            this.disp_LeftBuf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_LeftBuf.BackColor = System.Drawing.Color.Transparent;
            this.disp_LeftBuf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_LeftBuf.BackgroundImage")));
            this.disp_LeftBuf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_LeftBuf.ButtonPush = false;
            this.disp_LeftBuf.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_LeftBuf.FlatAppearance.BorderSize = 0;
            this.disp_LeftBuf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_LeftBuf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_LeftBuf.ForeColor = System.Drawing.Color.Black;
            this.disp_LeftBuf.ImageButton = true;
            this.disp_LeftBuf.ImageComplete = null;
            this.disp_LeftBuf.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_LeftBuf.ImageDefault")));
            this.disp_LeftBuf.ImageDisable = null;
            this.disp_LeftBuf.ImageDisableDown = null;
            this.disp_LeftBuf.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_LeftBuf.ImageDown")));
            this.disp_LeftBuf.ImageDownHOver = null;
            this.disp_LeftBuf.ImageHOver = null;
            this.disp_LeftBuf.Location = new System.Drawing.Point(232, 188);
            this.disp_LeftBuf.Margin = new System.Windows.Forms.Padding(2);
            this.disp_LeftBuf.Name = "disp_LeftBuf";
            this.disp_LeftBuf.Size = new System.Drawing.Size(40, 40);
            this.disp_LeftBuf.TabIndex = 80;
            this.disp_LeftBuf.TabStop = false;
            this.disp_LeftBuf.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(467, 104);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 20);
            this.label5.TabIndex = 79;
            this.label5.Text = "Tray Out";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_TrayOut
            // 
            this.disp_TrayOut.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_TrayOut.BackColor = System.Drawing.Color.Transparent;
            this.disp_TrayOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_TrayOut.BackgroundImage")));
            this.disp_TrayOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_TrayOut.ButtonPush = false;
            this.disp_TrayOut.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_TrayOut.FlatAppearance.BorderSize = 0;
            this.disp_TrayOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_TrayOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_TrayOut.ForeColor = System.Drawing.Color.Black;
            this.disp_TrayOut.ImageButton = true;
            this.disp_TrayOut.ImageComplete = null;
            this.disp_TrayOut.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_TrayOut.ImageDefault")));
            this.disp_TrayOut.ImageDisable = null;
            this.disp_TrayOut.ImageDisableDown = null;
            this.disp_TrayOut.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_TrayOut.ImageDown")));
            this.disp_TrayOut.ImageDownHOver = null;
            this.disp_TrayOut.ImageHOver = null;
            this.disp_TrayOut.Location = new System.Drawing.Point(447, 62);
            this.disp_TrayOut.Margin = new System.Windows.Forms.Padding(2);
            this.disp_TrayOut.Name = "disp_TrayOut";
            this.disp_TrayOut.Size = new System.Drawing.Size(40, 40);
            this.disp_TrayOut.TabIndex = 78;
            this.disp_TrayOut.TabStop = false;
            this.disp_TrayOut.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.Location = new System.Drawing.Point(322, 82);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 20);
            this.label6.TabIndex = 77;
            this.label6.Text = "Tray Out Buffer";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_TrayOutBuf
            // 
            this.disp_TrayOutBuf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_TrayOutBuf.BackColor = System.Drawing.Color.Transparent;
            this.disp_TrayOutBuf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_TrayOutBuf.BackgroundImage")));
            this.disp_TrayOutBuf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_TrayOutBuf.ButtonPush = false;
            this.disp_TrayOutBuf.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_TrayOutBuf.FlatAppearance.BorderSize = 0;
            this.disp_TrayOutBuf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_TrayOutBuf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_TrayOutBuf.ForeColor = System.Drawing.Color.Black;
            this.disp_TrayOutBuf.ImageButton = true;
            this.disp_TrayOutBuf.ImageComplete = null;
            this.disp_TrayOutBuf.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_TrayOutBuf.ImageDefault")));
            this.disp_TrayOutBuf.ImageDisable = null;
            this.disp_TrayOutBuf.ImageDisableDown = null;
            this.disp_TrayOutBuf.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_TrayOutBuf.ImageDown")));
            this.disp_TrayOutBuf.ImageDownHOver = null;
            this.disp_TrayOutBuf.ImageHOver = null;
            this.disp_TrayOutBuf.Location = new System.Drawing.Point(403, 100);
            this.disp_TrayOutBuf.Margin = new System.Windows.Forms.Padding(2);
            this.disp_TrayOutBuf.Name = "disp_TrayOutBuf";
            this.disp_TrayOutBuf.Size = new System.Drawing.Size(40, 40);
            this.disp_TrayOutBuf.TabIndex = 76;
            this.disp_TrayOutBuf.TabStop = false;
            this.disp_TrayOutBuf.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(376, 172);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 20);
            this.label4.TabIndex = 75;
            this.label4.Text = "Tray Work";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_TrayInBuf
            // 
            this.disp_TrayInBuf.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_TrayInBuf.BackColor = System.Drawing.Color.Transparent;
            this.disp_TrayInBuf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_TrayInBuf.BackgroundImage")));
            this.disp_TrayInBuf.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_TrayInBuf.ButtonPush = false;
            this.disp_TrayInBuf.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_TrayInBuf.FlatAppearance.BorderSize = 0;
            this.disp_TrayInBuf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_TrayInBuf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_TrayInBuf.ForeColor = System.Drawing.Color.Black;
            this.disp_TrayInBuf.ImageButton = true;
            this.disp_TrayInBuf.ImageComplete = null;
            this.disp_TrayInBuf.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_TrayInBuf.ImageDefault")));
            this.disp_TrayInBuf.ImageDisable = null;
            this.disp_TrayInBuf.ImageDisableDown = null;
            this.disp_TrayInBuf.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_TrayInBuf.ImageDown")));
            this.disp_TrayInBuf.ImageDownHOver = null;
            this.disp_TrayInBuf.ImageHOver = null;
            this.disp_TrayInBuf.Location = new System.Drawing.Point(463, 185);
            this.disp_TrayInBuf.Margin = new System.Windows.Forms.Padding(2);
            this.disp_TrayInBuf.Name = "disp_TrayInBuf";
            this.disp_TrayInBuf.Size = new System.Drawing.Size(40, 40);
            this.disp_TrayInBuf.TabIndex = 74;
            this.disp_TrayInBuf.TabStop = false;
            this.disp_TrayInBuf.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(467, 223);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 20);
            this.label3.TabIndex = 73;
            this.label3.Text = "Tray In Buffer";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_TrayWork
            // 
            this.disp_TrayWork.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_TrayWork.BackColor = System.Drawing.Color.Transparent;
            this.disp_TrayWork.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_TrayWork.BackgroundImage")));
            this.disp_TrayWork.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_TrayWork.ButtonPush = false;
            this.disp_TrayWork.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_TrayWork.FlatAppearance.BorderSize = 0;
            this.disp_TrayWork.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_TrayWork.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_TrayWork.ForeColor = System.Drawing.Color.Black;
            this.disp_TrayWork.ImageButton = true;
            this.disp_TrayWork.ImageComplete = null;
            this.disp_TrayWork.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_TrayWork.ImageDefault")));
            this.disp_TrayWork.ImageDisable = null;
            this.disp_TrayWork.ImageDisableDown = null;
            this.disp_TrayWork.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_TrayWork.ImageDown")));
            this.disp_TrayWork.ImageDownHOver = null;
            this.disp_TrayWork.ImageHOver = null;
            this.disp_TrayWork.Location = new System.Drawing.Point(359, 130);
            this.disp_TrayWork.Margin = new System.Windows.Forms.Padding(2);
            this.disp_TrayWork.Name = "disp_TrayWork";
            this.disp_TrayWork.Size = new System.Drawing.Size(40, 40);
            this.disp_TrayWork.TabIndex = 72;
            this.disp_TrayWork.TabStop = false;
            this.disp_TrayWork.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(194, 150);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 20);
            this.label1.TabIndex = 71;
            this.label1.Text = "Product Loader";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_ProdLoader
            // 
            this.disp_ProdLoader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_ProdLoader.BackColor = System.Drawing.Color.Transparent;
            this.disp_ProdLoader.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_ProdLoader.BackgroundImage")));
            this.disp_ProdLoader.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_ProdLoader.ButtonPush = false;
            this.disp_ProdLoader.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_ProdLoader.FlatAppearance.BorderSize = 0;
            this.disp_ProdLoader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_ProdLoader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_ProdLoader.ForeColor = System.Drawing.Color.Black;
            this.disp_ProdLoader.ImageButton = true;
            this.disp_ProdLoader.ImageComplete = null;
            this.disp_ProdLoader.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_ProdLoader.ImageDefault")));
            this.disp_ProdLoader.ImageDisable = null;
            this.disp_ProdLoader.ImageDisableDown = null;
            this.disp_ProdLoader.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_ProdLoader.ImageDown")));
            this.disp_ProdLoader.ImageDownHOver = null;
            this.disp_ProdLoader.ImageHOver = null;
            this.disp_ProdLoader.Location = new System.Drawing.Point(304, 150);
            this.disp_ProdLoader.Margin = new System.Windows.Forms.Padding(2);
            this.disp_ProdLoader.Name = "disp_ProdLoader";
            this.disp_ProdLoader.Size = new System.Drawing.Size(40, 40);
            this.disp_ProdLoader.TabIndex = 70;
            this.disp_ProdLoader.TabStop = false;
            this.disp_ProdLoader.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(83, 219);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 20);
            this.label2.TabIndex = 69;
            this.label2.Text = "Assembler";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disp_Assembler
            // 
            this.disp_Assembler.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_Assembler.BackColor = System.Drawing.Color.Transparent;
            this.disp_Assembler.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_Assembler.BackgroundImage")));
            this.disp_Assembler.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_Assembler.ButtonPush = false;
            this.disp_Assembler.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_Assembler.FlatAppearance.BorderSize = 0;
            this.disp_Assembler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_Assembler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_Assembler.ForeColor = System.Drawing.Color.Black;
            this.disp_Assembler.ImageButton = true;
            this.disp_Assembler.ImageComplete = null;
            this.disp_Assembler.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_Assembler.ImageDefault")));
            this.disp_Assembler.ImageDisable = null;
            this.disp_Assembler.ImageDisableDown = null;
            this.disp_Assembler.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_Assembler.ImageDown")));
            this.disp_Assembler.ImageDownHOver = null;
            this.disp_Assembler.ImageHOver = null;
            this.disp_Assembler.Location = new System.Drawing.Point(177, 223);
            this.disp_Assembler.Margin = new System.Windows.Forms.Padding(2);
            this.disp_Assembler.Name = "disp_Assembler";
            this.disp_Assembler.Size = new System.Drawing.Size(40, 40);
            this.disp_Assembler.TabIndex = 68;
            this.disp_Assembler.TabStop = false;
            this.disp_Assembler.UseVisualStyleBackColor = false;
            // 
            // disp_TrayIn
            // 
            this.disp_TrayIn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.disp_TrayIn.BackColor = System.Drawing.Color.Transparent;
            this.disp_TrayIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("disp_TrayIn.BackgroundImage")));
            this.disp_TrayIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.disp_TrayIn.ButtonPush = false;
            this.disp_TrayIn.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.disp_TrayIn.FlatAppearance.BorderSize = 0;
            this.disp_TrayIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disp_TrayIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disp_TrayIn.ForeColor = System.Drawing.Color.Black;
            this.disp_TrayIn.ImageButton = true;
            this.disp_TrayIn.ImageComplete = null;
            this.disp_TrayIn.ImageDefault = ((System.Drawing.Image)(resources.GetObject("disp_TrayIn.ImageDefault")));
            this.disp_TrayIn.ImageDisable = null;
            this.disp_TrayIn.ImageDisableDown = null;
            this.disp_TrayIn.ImageDown = ((System.Drawing.Image)(resources.GetObject("disp_TrayIn.ImageDown")));
            this.disp_TrayIn.ImageDownHOver = null;
            this.disp_TrayIn.ImageHOver = null;
            this.disp_TrayIn.Location = new System.Drawing.Point(507, 152);
            this.disp_TrayIn.Margin = new System.Windows.Forms.Padding(2);
            this.disp_TrayIn.Name = "disp_TrayIn";
            this.disp_TrayIn.Size = new System.Drawing.Size(40, 40);
            this.disp_TrayIn.TabIndex = 92;
            this.disp_TrayIn.TabStop = false;
            this.disp_TrayIn.UseVisualStyleBackColor = false;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.White;
            this.label9.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(517, 194);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 20);
            this.label9.TabIndex = 93;
            this.label9.Text = "Tray In";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SubForm_Alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 688);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "SubForm_Alarm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alarm";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Alarm_FormClosed);
            this.Load += new System.EventHandler(this.SubForm_Alarm_Load);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private TopEng.Controls.ButtonEnh button_Close;
        private TopEng.Controls.ButtonEnh button_Mute;
        private TopEng.Controls.ButtonEnh button_Title;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label13;
        private Controls.ButtonEnh disp_JigCarrier;
        private System.Windows.Forms.Label label12;
        private Controls.ButtonEnh disp_JigOutBuf;
        private System.Windows.Forms.Label label11;
        private Controls.ButtonEnh disp_JigWork;
        private System.Windows.Forms.Label label10;
        private Controls.ButtonEnh disp_JigInBuf;
        private System.Windows.Forms.Label label8;
        private Controls.ButtonEnh disp_RightBuf;
        private System.Windows.Forms.Label label7;
        private Controls.ButtonEnh disp_LeftBuf;
        private System.Windows.Forms.Label label5;
        private Controls.ButtonEnh disp_TrayOut;
        private System.Windows.Forms.Label label6;
        private Controls.ButtonEnh disp_TrayOutBuf;
        private System.Windows.Forms.Label label4;
        private Controls.ButtonEnh disp_TrayInBuf;
        private System.Windows.Forms.Label label3;
        private Controls.ButtonEnh disp_TrayWork;
        private System.Windows.Forms.Label label1;
        private Controls.ButtonEnh disp_ProdLoader;
        private System.Windows.Forms.Label label2;
        private Controls.ButtonEnh disp_Assembler;
        private System.Windows.Forms.Label label9;
        private Controls.ButtonEnh disp_TrayIn;
    }
}