
namespace CAMASSEMBLYMACHINE.UI
{
    partial class SubForm_CycleStop
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubForm_CycleStop));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewAxis = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_Close = new TopEng.Controls.ButtonEnh();
            this.button_Title = new TopEng.Controls.ButtonEnh();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAxis)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewAxis, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_Close, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.button_Title, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(599, 445);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // dataGridViewAxis
            // 
            this.dataGridViewAxis.AllowUserToAddRows = false;
            this.dataGridViewAxis.AllowUserToDeleteRows = false;
            this.dataGridViewAxis.AllowUserToResizeColumns = false;
            this.dataGridViewAxis.AllowUserToResizeRows = false;
            this.dataGridViewAxis.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewAxis.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewAxis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAxis.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewAxis.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewAxis.GridColor = System.Drawing.Color.DimGray;
            this.dataGridViewAxis.Location = new System.Drawing.Point(0, 70);
            this.dataGridViewAxis.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridViewAxis.Name = "dataGridViewAxis";
            this.dataGridViewAxis.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewAxis.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewAxis.RowHeadersVisible = false;
            this.dataGridViewAxis.RowTemplate.Height = 23;
            this.dataGridViewAxis.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewAxis.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewAxis.ShowCellErrors = false;
            this.dataGridViewAxis.ShowEditingIcon = false;
            this.dataGridViewAxis.ShowRowErrors = false;
            this.dataGridViewAxis.Size = new System.Drawing.Size(599, 307);
            this.dataGridViewAxis.TabIndex = 46;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
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
            this.button_Close.ImageDisable = null;
            this.button_Close.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageDown")));
            this.button_Close.ImageDownHOver = null;
            this.button_Close.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_Close.ImageHOver")));
            this.button_Close.Location = new System.Drawing.Point(2, 379);
            this.button_Close.Margin = new System.Windows.Forms.Padding(2);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(595, 64);
            this.button_Close.TabIndex = 35;
            this.button_Close.TabStop = false;
            this.button_Close.UseVisualStyleBackColor = false;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_Title
            // 
            this.button_Title.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Title.BackColor = System.Drawing.SystemColors.Window;
            this.button_Title.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Title.BackgroundImage")));
            this.button_Title.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Title.ButtonPush = false;
            this.button_Title.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Display;
            this.button_Title.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Title.ForeColor = System.Drawing.Color.White;
            this.button_Title.ImageButton = true;
            this.button_Title.ImageComplete = null;
            this.button_Title.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_Title.ImageDefault")));
            this.button_Title.ImageDisable = null;
            this.button_Title.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_Title.ImageDown")));
            this.button_Title.ImageDownHOver = null;
            this.button_Title.ImageHOver = null;
            this.button_Title.Location = new System.Drawing.Point(0, 0);
            this.button_Title.Margin = new System.Windows.Forms.Padding(0);
            this.button_Title.Name = "button_Title";
            this.button_Title.Size = new System.Drawing.Size(599, 70);
            this.button_Title.TabIndex = 33;
            this.button_Title.TabStop = false;
            this.button_Title.UseVisualStyleBackColor = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // SubForm_CycleStop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 445);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SubForm_CycleStop";
            this.Text = "CycleStop";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SubForm_CycleStop_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAxis)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridViewAxis;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private TopEng.Controls.ButtonEnh button_Close;
        private TopEng.Controls.ButtonEnh button_Title;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}