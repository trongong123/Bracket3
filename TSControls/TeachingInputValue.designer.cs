
namespace TopEng.Controls
{
    partial class TeachingInputValue
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelHome = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panelLimitMinus = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCurrentPos = new System.Windows.Forms.TextBox();
            this.textBoxTargetPos = new System.Windows.Forms.TextBox();
            this.panelAxis = new System.Windows.Forms.Panel();
            this.labelAxis = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panelLimitPlus = new System.Windows.Forms.Panel();
            this.labelHome = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelHome.SuspendLayout();
            this.panelLimitMinus.SuspendLayout();
            this.panelAxis.SuspendLayout();
            this.panelLimitPlus.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26F));
            this.tableLayoutPanel1.Controls.Add(this.panelHome, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelLimitMinus, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxCurrentPos, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxTargetPos, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelAxis, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelLimitPlus, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(335, 28);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelHome
            // 
            this.panelHome.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelHome.BackColor = System.Drawing.Color.Gray;
            this.panelHome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHome.Controls.Add(this.label2);
            this.panelHome.Location = new System.Drawing.Point(107, 6);
            this.panelHome.Margin = new System.Windows.Forms.Padding(4);
            this.panelHome.Name = "panelHome";
            this.panelHome.Size = new System.Drawing.Size(16, 16);
            this.panelHome.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(26, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "HOME";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelLimitMinus
            // 
            this.panelLimitMinus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelLimitMinus.BackColor = System.Drawing.Color.Gray;
            this.panelLimitMinus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLimitMinus.Controls.Add(this.label1);
            this.panelLimitMinus.Location = new System.Drawing.Point(137, 6);
            this.panelLimitMinus.Margin = new System.Windows.Forms.Padding(4);
            this.panelLimitMinus.Name = "panelLimitMinus";
            this.panelLimitMinus.Size = new System.Drawing.Size(16, 16);
            this.panelLimitMinus.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(26, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "HOME";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxCurrentPos
            // 
            this.textBoxCurrentPos.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxCurrentPos.BackColor = System.Drawing.Color.White;
            this.textBoxCurrentPos.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentPos.ForeColor = System.Drawing.Color.Gray;
            this.textBoxCurrentPos.Location = new System.Drawing.Point(161, 3);
            this.textBoxCurrentPos.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxCurrentPos.MaxLength = 9;
            this.textBoxCurrentPos.Name = "textBoxCurrentPos";
            this.textBoxCurrentPos.ReadOnly = true;
            this.textBoxCurrentPos.Size = new System.Drawing.Size(85, 22);
            this.textBoxCurrentPos.TabIndex = 1;
            this.textBoxCurrentPos.Text = "00000.000";
            this.textBoxCurrentPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxTargetPos
            // 
            this.textBoxTargetPos.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxTargetPos.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTargetPos.Location = new System.Drawing.Point(248, 3);
            this.textBoxTargetPos.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxTargetPos.MaxLength = 9;
            this.textBoxTargetPos.Name = "textBoxTargetPos";
            this.textBoxTargetPos.Size = new System.Drawing.Size(86, 22);
            this.textBoxTargetPos.TabIndex = 2;
            this.textBoxTargetPos.Text = "00000.000";
            this.textBoxTargetPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxTargetPos.Click += new System.EventHandler(this.textBoxTargetPos_Click);
            // 
            // panelAxis
            // 
            this.panelAxis.Controls.Add(this.labelAxis);
            this.panelAxis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAxis.Location = new System.Drawing.Point(33, 3);
            this.panelAxis.Name = "panelAxis";
            this.panelAxis.Size = new System.Drawing.Size(34, 22);
            this.panelAxis.TabIndex = 3;
            // 
            // labelAxis
            // 
            this.labelAxis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAxis.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelAxis.Location = new System.Drawing.Point(0, 0);
            this.labelAxis.Margin = new System.Windows.Forms.Padding(3);
            this.labelAxis.Name = "labelAxis";
            this.labelAxis.Size = new System.Drawing.Size(34, 22);
            this.labelAxis.TabIndex = 5;
            this.labelAxis.Text = "X";
            this.labelAxis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(24, 22);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // panelLimitPlus
            // 
            this.panelLimitPlus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelLimitPlus.BackColor = System.Drawing.Color.Gray;
            this.panelLimitPlus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLimitPlus.Controls.Add(this.labelHome);
            this.panelLimitPlus.Location = new System.Drawing.Point(77, 6);
            this.panelLimitPlus.Margin = new System.Windows.Forms.Padding(4);
            this.panelLimitPlus.Name = "panelLimitPlus";
            this.panelLimitPlus.Size = new System.Drawing.Size(16, 16);
            this.panelLimitPlus.TabIndex = 5;
            // 
            // labelHome
            // 
            this.labelHome.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHome.AutoSize = true;
            this.labelHome.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelHome.Location = new System.Drawing.Point(26, 4);
            this.labelHome.Name = "labelHome";
            this.labelHome.Size = new System.Drawing.Size(43, 15);
            this.labelHome.TabIndex = 0;
            this.labelHome.Text = "HOME";
            this.labelHome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TeachingInputValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TeachingInputValue";
            this.Size = new System.Drawing.Size(335, 28);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelHome.ResumeLayout(false);
            this.panelHome.PerformLayout();
            this.panelLimitMinus.ResumeLayout(false);
            this.panelLimitMinus.PerformLayout();
            this.panelAxis.ResumeLayout(false);
            this.panelLimitPlus.ResumeLayout(false);
            this.panelLimitPlus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.TextBox textBoxCurrentPos;
        public System.Windows.Forms.TextBox textBoxTargetPos;
        private System.Windows.Forms.Panel panelAxis;
        private System.Windows.Forms.Label labelAxis;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel panelLimitPlus;
        private System.Windows.Forms.Label labelHome;
        private System.Windows.Forms.Panel panelLimitMinus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelHome;
        private System.Windows.Forms.Label label2;
    }
}
