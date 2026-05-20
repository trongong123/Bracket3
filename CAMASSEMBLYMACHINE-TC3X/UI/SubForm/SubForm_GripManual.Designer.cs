
namespace CAMASSEMBLYMACHINE.UI
{
    partial class SubForm_GripManual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubForm_GripManual));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Close = new TopEng.Controls.ButtonEnh();
            this.buttonEnh1 = new TopEng.Controls.ButtonEnh();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.button_Grip3Open = new TopEng.Controls.ButtonEnh();
            this.buttonEnh6 = new TopEng.Controls.ButtonEnh();
            this.button_Grip4Open = new TopEng.Controls.ButtonEnh();
            this.button_Grip2Open = new TopEng.Controls.ButtonEnh();
            this.button_Grip1Open = new TopEng.Controls.ButtonEnh();
            this.buttonEnh7 = new TopEng.Controls.ButtonEnh();
            this.buttonEnh8 = new TopEng.Controls.ButtonEnh();
            this.buttonEnh9 = new TopEng.Controls.ButtonEnh();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.button_Close, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonEnh1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 435);
            this.tableLayoutPanel1.TabIndex = 1;
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
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Close.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Close.ForeColor = System.Drawing.Color.White;
            this.button_Close.ImageButton = true;
            this.button_Close.ImageComplete = null;
            this.button_Close.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageDefault")));
            this.button_Close.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageDisable")));
            this.button_Close.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageDown")));
            this.button_Close.ImageDownHOver = null;
            this.button_Close.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageHOver")));
            this.button_Close.Location = new System.Drawing.Point(2, 371);
            this.button_Close.Margin = new System.Windows.Forms.Padding(2);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(605, 62);
            this.button_Close.TabIndex = 35;
            this.button_Close.TabStop = false;
            this.button_Close.UseVisualStyleBackColor = false;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // buttonEnh1
            // 
            this.buttonEnh1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnh1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.buttonEnh1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEnh1.BackgroundImage")));
            this.buttonEnh1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonEnh1.ButtonPush = false;
            this.buttonEnh1.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.buttonEnh1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnh1.Font = new System.Drawing.Font("Franklin Gothic Medium", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnh1.ForeColor = System.Drawing.Color.White;
            this.buttonEnh1.ImageButton = false;
            this.buttonEnh1.ImageComplete = null;
            this.buttonEnh1.ImageDefault = ((System.Drawing.Image)(resources.GetObject("buttonEnh1.ImageDefault")));
            this.buttonEnh1.ImageDisable = null;
            this.buttonEnh1.ImageDown = null;
            this.buttonEnh1.ImageDownHOver = null;
            this.buttonEnh1.ImageHOver = null;
            this.buttonEnh1.Location = new System.Drawing.Point(2, 2);
            this.buttonEnh1.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEnh1.Name = "buttonEnh1";
            this.buttonEnh1.Size = new System.Drawing.Size(605, 70);
            this.buttonEnh1.TabIndex = 33;
            this.buttonEnh1.TabStop = false;
            this.buttonEnh1.Text = "Initialize";
            this.buttonEnh1.UseVisualStyleBackColor = false;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(3, 76);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(603, 115);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "Open the gripper and take out the product.";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 9;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.99813F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.00062F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.Controls.Add(this.button_Grip3Open, 5, 3);
            this.tableLayoutPanel2.Controls.Add(this.buttonEnh6, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.button_Grip4Open, 7, 3);
            this.tableLayoutPanel2.Controls.Add(this.button_Grip2Open, 3, 3);
            this.tableLayoutPanel2.Controls.Add(this.button_Grip1Open, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.buttonEnh7, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonEnh8, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonEnh9, 7, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 196);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 3F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(603, 170);
            this.tableLayoutPanel2.TabIndex = 36;
            // 
            // button_Grip3Open
            // 
            this.button_Grip3Open.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Grip3Open.BackColor = System.Drawing.Color.Transparent;
            this.button_Grip3Open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Grip3Open.BackgroundImage")));
            this.button_Grip3Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Grip3Open.ButtonPush = false;
            this.button_Grip3Open.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Grip3Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Grip3Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Grip3Open.ForeColor = System.Drawing.Color.White;
            this.button_Grip3Open.ImageButton = true;
            this.button_Grip3Open.ImageComplete = null;
            this.button_Grip3Open.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Grip3Open.ImageDefault")));
            this.button_Grip3Open.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Grip3Open.ImageDisable")));
            this.button_Grip3Open.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Grip3Open.ImageDown")));
            this.button_Grip3Open.ImageDownHOver = null;
            this.button_Grip3Open.ImageHOver = null;
            this.button_Grip3Open.Location = new System.Drawing.Point(308, 54);
            this.button_Grip3Open.Margin = new System.Windows.Forms.Padding(2);
            this.button_Grip3Open.Name = "button_Grip3Open";
            this.button_Grip3Open.Size = new System.Drawing.Size(134, 114);
            this.button_Grip3Open.TabIndex = 36;
            this.button_Grip3Open.TabStop = false;
            this.button_Grip3Open.UseVisualStyleBackColor = false;
            this.button_Grip3Open.Click += new System.EventHandler(this.button_Grip3Open_Click);
            // 
            // buttonEnh6
            // 
            this.buttonEnh6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnh6.BackColor = System.Drawing.Color.Transparent;
            this.buttonEnh6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEnh6.BackgroundImage")));
            this.buttonEnh6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonEnh6.ButtonPush = false;
            this.buttonEnh6.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.buttonEnh6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnh6.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnh6.ForeColor = System.Drawing.Color.White;
            this.buttonEnh6.ImageButton = false;
            this.buttonEnh6.ImageComplete = null;
            this.buttonEnh6.ImageDefault = ((System.Drawing.Image)(resources.GetObject("buttonEnh6.ImageDefault")));
            this.buttonEnh6.ImageDisable = null;
            this.buttonEnh6.ImageDown = null;
            this.buttonEnh6.ImageDownHOver = null;
            this.buttonEnh6.ImageHOver = null;
            this.buttonEnh6.Location = new System.Drawing.Point(10, 10);
            this.buttonEnh6.Margin = new System.Windows.Forms.Padding(0);
            this.buttonEnh6.Name = "buttonEnh6";
            this.buttonEnh6.Size = new System.Drawing.Size(138, 39);
            this.buttonEnh6.TabIndex = 33;
            this.buttonEnh6.TabStop = false;
            this.buttonEnh6.Text = "Gripper 1";
            this.buttonEnh6.UseVisualStyleBackColor = false;
            // 
            // button_Grip4Open
            // 
            this.button_Grip4Open.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Grip4Open.BackColor = System.Drawing.Color.Transparent;
            this.button_Grip4Open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Grip4Open.BackgroundImage")));
            this.button_Grip4Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Grip4Open.ButtonPush = false;
            this.button_Grip4Open.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Grip4Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Grip4Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Grip4Open.ForeColor = System.Drawing.Color.White;
            this.button_Grip4Open.ImageButton = true;
            this.button_Grip4Open.ImageComplete = null;
            this.button_Grip4Open.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Grip4Open.ImageDefault")));
            this.button_Grip4Open.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Grip4Open.ImageDisable")));
            this.button_Grip4Open.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Grip4Open.ImageDown")));
            this.button_Grip4Open.ImageDownHOver = null;
            this.button_Grip4Open.ImageHOver = null;
            this.button_Grip4Open.Location = new System.Drawing.Point(456, 54);
            this.button_Grip4Open.Margin = new System.Windows.Forms.Padding(2);
            this.button_Grip4Open.Name = "button_Grip4Open";
            this.button_Grip4Open.Size = new System.Drawing.Size(134, 114);
            this.button_Grip4Open.TabIndex = 36;
            this.button_Grip4Open.TabStop = false;
            this.button_Grip4Open.UseVisualStyleBackColor = false;
            this.button_Grip4Open.Click += new System.EventHandler(this.button_Grip4Open_Click);
            // 
            // button_Grip2Open
            // 
            this.button_Grip2Open.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Grip2Open.BackColor = System.Drawing.Color.Transparent;
            this.button_Grip2Open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Grip2Open.BackgroundImage")));
            this.button_Grip2Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Grip2Open.ButtonPush = false;
            this.button_Grip2Open.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Grip2Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Grip2Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Grip2Open.ForeColor = System.Drawing.Color.White;
            this.button_Grip2Open.ImageButton = true;
            this.button_Grip2Open.ImageComplete = null;
            this.button_Grip2Open.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Grip2Open.ImageDefault")));
            this.button_Grip2Open.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Grip2Open.ImageDisable")));
            this.button_Grip2Open.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Grip2Open.ImageDown")));
            this.button_Grip2Open.ImageDownHOver = null;
            this.button_Grip2Open.ImageHOver = null;
            this.button_Grip2Open.Location = new System.Drawing.Point(160, 54);
            this.button_Grip2Open.Margin = new System.Windows.Forms.Padding(2);
            this.button_Grip2Open.Name = "button_Grip2Open";
            this.button_Grip2Open.Size = new System.Drawing.Size(134, 114);
            this.button_Grip2Open.TabIndex = 36;
            this.button_Grip2Open.TabStop = false;
            this.button_Grip2Open.UseVisualStyleBackColor = false;
            this.button_Grip2Open.Click += new System.EventHandler(this.button_Grip2Open_Click);
            // 
            // button_Grip1Open
            // 
            this.button_Grip1Open.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Grip1Open.BackColor = System.Drawing.Color.Transparent;
            this.button_Grip1Open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Grip1Open.BackgroundImage")));
            this.button_Grip1Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button_Grip1Open.ButtonPush = false;
            this.button_Grip1Open.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_Grip1Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Grip1Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Grip1Open.ForeColor = System.Drawing.Color.White;
            this.button_Grip1Open.ImageButton = true;
            this.button_Grip1Open.ImageComplete = null;
            this.button_Grip1Open.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Grip1Open.ImageDefault")));
            this.button_Grip1Open.ImageDisable = ((System.Drawing.Image)(resources.GetObject("button_Grip1Open.ImageDisable")));
            this.button_Grip1Open.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Grip1Open.ImageDown")));
            this.button_Grip1Open.ImageDownHOver = null;
            this.button_Grip1Open.ImageHOver = null;
            this.button_Grip1Open.Location = new System.Drawing.Point(12, 54);
            this.button_Grip1Open.Margin = new System.Windows.Forms.Padding(2);
            this.button_Grip1Open.Name = "button_Grip1Open";
            this.button_Grip1Open.Size = new System.Drawing.Size(134, 114);
            this.button_Grip1Open.TabIndex = 36;
            this.button_Grip1Open.TabStop = false;
            this.button_Grip1Open.UseVisualStyleBackColor = false;
            this.button_Grip1Open.Click += new System.EventHandler(this.button_Grip1Open_Click);
            // 
            // buttonEnh7
            // 
            this.buttonEnh7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnh7.BackColor = System.Drawing.Color.Transparent;
            this.buttonEnh7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEnh7.BackgroundImage")));
            this.buttonEnh7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonEnh7.ButtonPush = false;
            this.buttonEnh7.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.buttonEnh7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnh7.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnh7.ForeColor = System.Drawing.Color.White;
            this.buttonEnh7.ImageButton = false;
            this.buttonEnh7.ImageComplete = null;
            this.buttonEnh7.ImageDefault = ((System.Drawing.Image)(resources.GetObject("buttonEnh7.ImageDefault")));
            this.buttonEnh7.ImageDisable = null;
            this.buttonEnh7.ImageDown = null;
            this.buttonEnh7.ImageDownHOver = null;
            this.buttonEnh7.ImageHOver = null;
            this.buttonEnh7.Location = new System.Drawing.Point(158, 10);
            this.buttonEnh7.Margin = new System.Windows.Forms.Padding(0);
            this.buttonEnh7.Name = "buttonEnh7";
            this.buttonEnh7.Size = new System.Drawing.Size(138, 39);
            this.buttonEnh7.TabIndex = 33;
            this.buttonEnh7.TabStop = false;
            this.buttonEnh7.Text = "Gripper 2";
            this.buttonEnh7.UseVisualStyleBackColor = false;
            // 
            // buttonEnh8
            // 
            this.buttonEnh8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnh8.BackColor = System.Drawing.Color.Transparent;
            this.buttonEnh8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEnh8.BackgroundImage")));
            this.buttonEnh8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonEnh8.ButtonPush = false;
            this.buttonEnh8.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.buttonEnh8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnh8.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnh8.ForeColor = System.Drawing.Color.White;
            this.buttonEnh8.ImageButton = false;
            this.buttonEnh8.ImageComplete = null;
            this.buttonEnh8.ImageDefault = ((System.Drawing.Image)(resources.GetObject("buttonEnh8.ImageDefault")));
            this.buttonEnh8.ImageDisable = null;
            this.buttonEnh8.ImageDown = null;
            this.buttonEnh8.ImageDownHOver = null;
            this.buttonEnh8.ImageHOver = null;
            this.buttonEnh8.Location = new System.Drawing.Point(306, 10);
            this.buttonEnh8.Margin = new System.Windows.Forms.Padding(0);
            this.buttonEnh8.Name = "buttonEnh8";
            this.buttonEnh8.Size = new System.Drawing.Size(138, 39);
            this.buttonEnh8.TabIndex = 33;
            this.buttonEnh8.TabStop = false;
            this.buttonEnh8.Text = "Gripper 3";
            this.buttonEnh8.UseVisualStyleBackColor = false;
            // 
            // buttonEnh9
            // 
            this.buttonEnh9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEnh9.BackColor = System.Drawing.Color.Transparent;
            this.buttonEnh9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEnh9.BackgroundImage")));
            this.buttonEnh9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonEnh9.ButtonPush = false;
            this.buttonEnh9.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.buttonEnh9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnh9.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnh9.ForeColor = System.Drawing.Color.White;
            this.buttonEnh9.ImageButton = false;
            this.buttonEnh9.ImageComplete = null;
            this.buttonEnh9.ImageDefault = ((System.Drawing.Image)(resources.GetObject("buttonEnh9.ImageDefault")));
            this.buttonEnh9.ImageDisable = null;
            this.buttonEnh9.ImageDown = null;
            this.buttonEnh9.ImageDownHOver = null;
            this.buttonEnh9.ImageHOver = null;
            this.buttonEnh9.Location = new System.Drawing.Point(454, 10);
            this.buttonEnh9.Margin = new System.Windows.Forms.Padding(0);
            this.buttonEnh9.Name = "buttonEnh9";
            this.buttonEnh9.Size = new System.Drawing.Size(138, 39);
            this.buttonEnh9.TabIndex = 33;
            this.buttonEnh9.TabStop = false;
            this.buttonEnh9.Text = "Gripper 4";
            this.buttonEnh9.UseVisualStyleBackColor = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SubForm_GripManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 435);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SubForm_GripManual";
            this.Text = "Reset Gripper";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SubForm_GripManual_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TopEng.Controls.ButtonEnh button_Close;
        private System.Windows.Forms.TextBox textBox1;
        private TopEng.Controls.ButtonEnh buttonEnh1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private TopEng.Controls.ButtonEnh button_Grip1Open;
        private TopEng.Controls.ButtonEnh button_Grip3Open;
        private TopEng.Controls.ButtonEnh buttonEnh6;
        private TopEng.Controls.ButtonEnh button_Grip4Open;
        private TopEng.Controls.ButtonEnh button_Grip2Open;
        private TopEng.Controls.ButtonEnh buttonEnh7;
        private TopEng.Controls.ButtonEnh buttonEnh8;
        private TopEng.Controls.ButtonEnh buttonEnh9;
        private System.Windows.Forms.Timer timer1;
    }
}