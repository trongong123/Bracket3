
namespace TopEng.Controls
{
    partial class AxisOrigin
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
            this.panelAlarm = new System.Windows.Forms.Panel();
            this.labelAlarm = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.panelHome = new System.Windows.Forms.Panel();
            this.labelHome = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panelLimitPlus = new System.Windows.Forms.Panel();
            this.labelLimitPlus = new System.Windows.Forms.Label();
            this.panelLimitMinus = new System.Windows.Forms.Panel();
            this.labelLimitMinus = new System.Windows.Forms.Label();
            this.panelServo = new System.Windows.Forms.Panel();
            this.labelServo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelAlarm.SuspendLayout();
            this.panelHome.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panelLimitPlus.SuspendLayout();
            this.panelLimitMinus.SuspendLayout();
            this.panelServo.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelAlarm, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelHome, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.panelServo, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24.75248F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.81188F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.81188F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.81188F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.81188F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(101, 106);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panelAlarm
            // 
            this.panelAlarm.BackColor = System.Drawing.SystemColors.Control;
            this.panelAlarm.Controls.Add(this.labelAlarm);
            this.panelAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAlarm.Location = new System.Drawing.Point(1, 64);
            this.panelAlarm.Margin = new System.Windows.Forms.Padding(0);
            this.panelAlarm.Name = "panelAlarm";
            this.panelAlarm.Size = new System.Drawing.Size(99, 18);
            this.panelAlarm.TabIndex = 3;
            // 
            // labelAlarm
            // 
            this.labelAlarm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelAlarm.AutoSize = true;
            this.labelAlarm.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelAlarm.Location = new System.Drawing.Point(25, 3);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Size = new System.Drawing.Size(43, 13);
            this.labelAlarm.TabIndex = 0;
            this.labelAlarm.Text = "ERROR";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelName
            // 
            this.labelName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelName.Location = new System.Drawing.Point(14, 4);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(72, 17);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "PICK UP X";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelHome
            // 
            this.panelHome.BackColor = System.Drawing.Color.Lime;
            this.panelHome.Controls.Add(this.labelHome);
            this.panelHome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHome.Location = new System.Drawing.Point(1, 26);
            this.panelHome.Margin = new System.Windows.Forms.Padding(0);
            this.panelHome.Name = "panelHome";
            this.panelHome.Size = new System.Drawing.Size(99, 18);
            this.panelHome.TabIndex = 1;
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
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.panelLimitPlus, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panelLimitMinus, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1, 83);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(99, 22);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // panelLimitPlus
            // 
            this.panelLimitPlus.BackColor = System.Drawing.SystemColors.Control;
            this.panelLimitPlus.Controls.Add(this.labelLimitPlus);
            this.panelLimitPlus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLimitPlus.Location = new System.Drawing.Point(0, 0);
            this.panelLimitPlus.Margin = new System.Windows.Forms.Padding(0);
            this.panelLimitPlus.Name = "panelLimitPlus";
            this.panelLimitPlus.Size = new System.Drawing.Size(49, 22);
            this.panelLimitPlus.TabIndex = 0;
            // 
            // labelLimitPlus
            // 
            this.labelLimitPlus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelLimitPlus.AutoSize = true;
            this.labelLimitPlus.BackColor = System.Drawing.Color.Transparent;
            this.labelLimitPlus.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLimitPlus.Location = new System.Drawing.Point(2, 5);
            this.labelLimitPlus.Margin = new System.Windows.Forms.Padding(0);
            this.labelLimitPlus.Name = "labelLimitPlus";
            this.labelLimitPlus.Size = new System.Drawing.Size(45, 13);
            this.labelLimitPlus.TabIndex = 0;
            this.labelLimitPlus.Text = "Limit +";
            this.labelLimitPlus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelLimitMinus
            // 
            this.panelLimitMinus.BackColor = System.Drawing.SystemColors.Control;
            this.panelLimitMinus.Controls.Add(this.labelLimitMinus);
            this.panelLimitMinus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLimitMinus.Location = new System.Drawing.Point(49, 0);
            this.panelLimitMinus.Margin = new System.Windows.Forms.Padding(0);
            this.panelLimitMinus.Name = "panelLimitMinus";
            this.panelLimitMinus.Size = new System.Drawing.Size(50, 22);
            this.panelLimitMinus.TabIndex = 1;
            // 
            // labelLimitMinus
            // 
            this.labelLimitMinus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelLimitMinus.AutoSize = true;
            this.labelLimitMinus.BackColor = System.Drawing.Color.Transparent;
            this.labelLimitMinus.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLimitMinus.Location = new System.Drawing.Point(4, 5);
            this.labelLimitMinus.Margin = new System.Windows.Forms.Padding(0);
            this.labelLimitMinus.Name = "labelLimitMinus";
            this.labelLimitMinus.Size = new System.Drawing.Size(42, 13);
            this.labelLimitMinus.TabIndex = 1;
            this.labelLimitMinus.Text = "Limit -";
            this.labelLimitMinus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelServo
            // 
            this.panelServo.BackColor = System.Drawing.SystemColors.Control;
            this.panelServo.Controls.Add(this.labelServo);
            this.panelServo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelServo.Location = new System.Drawing.Point(1, 45);
            this.panelServo.Margin = new System.Windows.Forms.Padding(0);
            this.panelServo.Name = "panelServo";
            this.panelServo.Size = new System.Drawing.Size(99, 18);
            this.panelServo.TabIndex = 1;
            // 
            // labelServo
            // 
            this.labelServo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelServo.AutoSize = true;
            this.labelServo.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelServo.Location = new System.Drawing.Point(16, 3);
            this.labelServo.Name = "labelServo";
            this.labelServo.Size = new System.Drawing.Size(64, 13);
            this.labelServo.TabIndex = 0;
            this.labelServo.Text = "SERVO ON";
            this.labelServo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AxisOrigin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AxisOrigin";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.Size = new System.Drawing.Size(113, 118);
            this.Click += new System.EventHandler(this.OriginMortor_Click);
            this.Resize += new System.EventHandler(this.OriginMortor_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelAlarm.ResumeLayout(false);
            this.panelAlarm.PerformLayout();
            this.panelHome.ResumeLayout(false);
            this.panelHome.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panelLimitPlus.ResumeLayout(false);
            this.panelLimitPlus.PerformLayout();
            this.panelLimitMinus.ResumeLayout(false);
            this.panelLimitMinus.PerformLayout();
            this.panelServo.ResumeLayout(false);
            this.panelServo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Panel panelHome;
        private System.Windows.Forms.Label labelHome;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panelLimitPlus;
        private System.Windows.Forms.Panel panelLimitMinus;
        private System.Windows.Forms.Label labelLimitMinus;
        private System.Windows.Forms.Panel panelServo;
        private System.Windows.Forms.Label labelServo;
        private System.Windows.Forms.Label labelLimitPlus;
        private System.Windows.Forms.Panel panelAlarm;
        private System.Windows.Forms.Label labelAlarm;
    }
}
