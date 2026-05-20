
namespace TopEng.Controls
{
    partial class ProcessPictureBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessPictureBox));
            this.pnlBackGroundPictureBox = new System.Windows.Forms.PictureBox();
            this.topTableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlBackGroundName = new System.Windows.Forms.Panel();
            this.lbName = new System.Windows.Forms.Label();
            this.pnlBackGroundPicture = new System.Windows.Forms.Panel();
            this.pnlBackgroundControl = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBackGroundPictureBox)).BeginInit();
            this.topTableLayoutPanel1.SuspendLayout();
            this.pnlBackGroundName.SuspendLayout();
            this.pnlBackGroundPicture.SuspendLayout();
            this.pnlBackgroundControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBackGroundPictureBox
            // 
            this.pnlBackGroundPictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlBackGroundPictureBox.BackgroundImage")));
            this.pnlBackGroundPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlBackGroundPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackGroundPictureBox.Location = new System.Drawing.Point(4, 4);
            this.pnlBackGroundPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBackGroundPictureBox.Name = "pnlBackGroundPictureBox";
            this.pnlBackGroundPictureBox.Size = new System.Drawing.Size(38, 39);
            this.pnlBackGroundPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pnlBackGroundPictureBox.TabIndex = 3;
            this.pnlBackGroundPictureBox.TabStop = false;
            // 
            // topTableLayoutPanel1
            // 
            this.topTableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.topTableLayoutPanel1.ColumnCount = 1;
            this.topTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topTableLayoutPanel1.Controls.Add(this.pnlBackGroundName, 0, 0);
            this.topTableLayoutPanel1.Controls.Add(this.pnlBackGroundPicture, 0, 1);
            this.topTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.topTableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.topTableLayoutPanel1.Name = "topTableLayoutPanel1";
            this.topTableLayoutPanel1.RowCount = 2;
            this.topTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.topTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.topTableLayoutPanel1.Size = new System.Drawing.Size(48, 73);
            this.topTableLayoutPanel1.TabIndex = 11;
            // 
            // pnlBackGroundName
            // 
            this.pnlBackGroundName.BackColor = System.Drawing.Color.Black;
            this.pnlBackGroundName.Controls.Add(this.lbName);
            this.pnlBackGroundName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackGroundName.Location = new System.Drawing.Point(1, 1);
            this.pnlBackGroundName.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBackGroundName.Name = "pnlBackGroundName";
            this.pnlBackGroundName.Size = new System.Drawing.Size(46, 23);
            this.pnlBackGroundName.TabIndex = 11;
            // 
            // lbName
            // 
            this.lbName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbName.AutoSize = true;
            this.lbName.BackColor = System.Drawing.Color.Transparent;
            this.lbName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbName.ForeColor = System.Drawing.Color.White;
            this.lbName.Location = new System.Drawing.Point(7, 3);
            this.lbName.Margin = new System.Windows.Forms.Padding(0);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(33, 15);
            this.lbName.TabIndex = 0;
            this.lbName.Text = "Assy";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlBackGroundPicture
            // 
            this.pnlBackGroundPicture.BackColor = System.Drawing.Color.Transparent;
            this.pnlBackGroundPicture.Controls.Add(this.pnlBackGroundPictureBox);
            this.pnlBackGroundPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackGroundPicture.Location = new System.Drawing.Point(1, 25);
            this.pnlBackGroundPicture.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBackGroundPicture.Name = "pnlBackGroundPicture";
            this.pnlBackGroundPicture.Padding = new System.Windows.Forms.Padding(4);
            this.pnlBackGroundPicture.Size = new System.Drawing.Size(46, 47);
            this.pnlBackGroundPicture.TabIndex = 0;
            // 
            // pnlBackgroundControl
            // 
            this.pnlBackgroundControl.BackColor = System.Drawing.Color.White;
            this.pnlBackgroundControl.Controls.Add(this.topTableLayoutPanel1);
            this.pnlBackgroundControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackgroundControl.Location = new System.Drawing.Point(0, 0);
            this.pnlBackgroundControl.Margin = new System.Windows.Forms.Padding(0);
            this.pnlBackgroundControl.Name = "pnlBackgroundControl";
            this.pnlBackgroundControl.Size = new System.Drawing.Size(48, 73);
            this.pnlBackgroundControl.TabIndex = 17;
            // 
            // ProcessPictureBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlBackgroundControl);
            this.Name = "ProcessPictureBox";
            this.Size = new System.Drawing.Size(48, 73);
            this.Resize += new System.EventHandler(this.ProcessPictureBox_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pnlBackGroundPictureBox)).EndInit();
            this.topTableLayoutPanel1.ResumeLayout(false);
            this.pnlBackGroundName.ResumeLayout(false);
            this.pnlBackGroundName.PerformLayout();
            this.pnlBackGroundPicture.ResumeLayout(false);
            this.pnlBackgroundControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.PictureBox pnlBackGroundPictureBox;
        public System.Windows.Forms.TableLayoutPanel topTableLayoutPanel1;
        public System.Windows.Forms.Panel pnlBackGroundName;
        public System.Windows.Forms.Label lbName;
        public System.Windows.Forms.Panel pnlBackGroundPicture;
        private System.Windows.Forms.Panel pnlBackgroundControl;
    }
}
