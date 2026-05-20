
namespace CAMASSEMBLYMACHINE.UI.SubForm
{
    partial class Form_ProcessTestView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ProcessTestView));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label_MachineState = new System.Windows.Forms.Label();
            this.button_SelectAxis = new TopEng.Controls.ButtonEnh();
            this.button_SelectProcess = new TopEng.Controls.ButtonEnh();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewAxis = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAxis)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.59491F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.40509F));
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewAxis, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1194, 415);
            this.tableLayoutPanel1.TabIndex = 44;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label_MachineState, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.button_SelectAxis, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.button_SelectProcess, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1036, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(155, 201);
            this.tableLayoutPanel2.TabIndex = 44;
            // 
            // label_MachineState
            // 
            this.label_MachineState.AutoSize = true;
            this.label_MachineState.BackColor = System.Drawing.Color.White;
            this.label_MachineState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_MachineState.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_MachineState.Location = new System.Drawing.Point(3, 80);
            this.label_MachineState.Name = "label_MachineState";
            this.label_MachineState.Size = new System.Drawing.Size(149, 40);
            this.label_MachineState.TabIndex = 69;
            this.label_MachineState.Text = "TRAY TOP";
            this.label_MachineState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_SelectAxis
            // 
            this.button_SelectAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectAxis.BackColor = System.Drawing.Color.Transparent;
            this.button_SelectAxis.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SelectAxis.BackgroundImage")));
            this.button_SelectAxis.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_SelectAxis.ButtonPush = false;
            this.button_SelectAxis.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_SelectAxis.FlatAppearance.BorderSize = 0;
            this.button_SelectAxis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SelectAxis.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SelectAxis.ForeColor = System.Drawing.Color.Black;
            this.button_SelectAxis.ImageButton = true;
            this.button_SelectAxis.ImageComplete = null;
            this.button_SelectAxis.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_SelectAxis.ImageDefault")));
            this.button_SelectAxis.ImageDisable = null;
            this.button_SelectAxis.ImageDisableDown = null;
            this.button_SelectAxis.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_SelectAxis.ImageDown")));
            this.button_SelectAxis.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_SelectAxis.ImageDownHOver")));
            this.button_SelectAxis.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_SelectAxis.ImageHOver")));
            this.button_SelectAxis.Location = new System.Drawing.Point(1, 1);
            this.button_SelectAxis.Margin = new System.Windows.Forms.Padding(1);
            this.button_SelectAxis.Name = "button_SelectAxis";
            this.button_SelectAxis.Size = new System.Drawing.Size(153, 38);
            this.button_SelectAxis.TabIndex = 25;
            this.button_SelectAxis.TabStop = false;
            this.button_SelectAxis.UseVisualStyleBackColor = false;
            this.button_SelectAxis.Click += new System.EventHandler(this.button_SelectAxis_Click);
            // 
            // button_SelectProcess
            // 
            this.button_SelectProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectProcess.BackColor = System.Drawing.Color.Transparent;
            this.button_SelectProcess.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SelectProcess.BackgroundImage")));
            this.button_SelectProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_SelectProcess.ButtonPush = false;
            this.button_SelectProcess.ButtonType = TopEng.Controls.ButtonEnh.BUTTONTYPE.Toggle;
            this.button_SelectProcess.FlatAppearance.BorderSize = 0;
            this.button_SelectProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SelectProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SelectProcess.ForeColor = System.Drawing.Color.Black;
            this.button_SelectProcess.ImageButton = true;
            this.button_SelectProcess.ImageComplete = null;
            this.button_SelectProcess.ImageDefault = ((System.Drawing.Image)(resources.GetObject("button_SelectProcess.ImageDefault")));
            this.button_SelectProcess.ImageDisable = null;
            this.button_SelectProcess.ImageDisableDown = null;
            this.button_SelectProcess.ImageDown = ((System.Drawing.Image)(resources.GetObject("button_SelectProcess.ImageDown")));
            this.button_SelectProcess.ImageDownHOver = ((System.Drawing.Image)(resources.GetObject("button_SelectProcess.ImageDownHOver")));
            this.button_SelectProcess.ImageHOver = ((System.Drawing.Image)(resources.GetObject("button_SelectProcess.ImageHOver")));
            this.button_SelectProcess.Location = new System.Drawing.Point(1, 41);
            this.button_SelectProcess.Margin = new System.Windows.Forms.Padding(1);
            this.button_SelectProcess.Name = "button_SelectProcess";
            this.button_SelectProcess.Size = new System.Drawing.Size(153, 38);
            this.button_SelectProcess.TabIndex = 26;
            this.button_SelectProcess.TabStop = false;
            this.button_SelectProcess.UseVisualStyleBackColor = false;
            this.button_SelectProcess.Click += new System.EventHandler(this.button_SelectProcess_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dataGridViewAxis
            // 
            this.dataGridViewAxis.AllowUserToAddRows = false;
            this.dataGridViewAxis.AllowUserToDeleteRows = false;
            this.dataGridViewAxis.AllowUserToResizeColumns = false;
            this.dataGridViewAxis.AllowUserToResizeRows = false;
            this.dataGridViewAxis.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle25.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewAxis.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle25;
            this.dataGridViewAxis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAxis.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle26.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle26.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle26.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle26.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle26.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle26.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle26.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewAxis.DefaultCellStyle = dataGridViewCellStyle26;
            this.dataGridViewAxis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAxis.GridColor = System.Drawing.Color.DimGray;
            this.dataGridViewAxis.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAxis.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridViewAxis.Name = "dataGridViewAxis";
            this.dataGridViewAxis.ReadOnly = true;
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle27.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle27.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewAxis.RowHeadersDefaultCellStyle = dataGridViewCellStyle27;
            this.dataGridViewAxis.RowHeadersVisible = false;
            this.dataGridViewAxis.RowTemplate.Height = 23;
            this.dataGridViewAxis.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewAxis.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewAxis.ShowCellErrors = false;
            this.dataGridViewAxis.ShowEditingIcon = false;
            this.dataGridViewAxis.ShowRowErrors = false;
            this.dataGridViewAxis.Size = new System.Drawing.Size(1033, 207);
            this.dataGridViewAxis.TabIndex = 45;
            this.dataGridViewAxis.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewAxis_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 200;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 200;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 210);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(1027, 202);
            this.textBox1.TabIndex = 71;
            // 
            // Form_ProcessTestView
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1194, 415);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form_ProcessTestView";
            this.Text = "Process Viewer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_ProcessView_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAxis)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TopEng.Controls.ButtonEnh button_SelectAxis;
        private TopEng.Controls.ButtonEnh button_SelectProcess;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_MachineState;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridViewAxis;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
    }
}