
namespace TopEng.Controls
{
    partial class ProductInfoJIG
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
            this.labelName = new System.Windows.Forms.Label();
            this.productCountValueH_Input = new TopEng.Controls.ProductCountValueH();
            this.productCountValueH_Output = new TopEng.Controls.ProductCountValueH();
            this.productCountValueH_NG = new TopEng.Controls.ProductCountValueH();
            this.productCountValueH_NgRate = new TopEng.Controls.ProductCountValueH();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.productCountValueH_Input, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.productCountValueH_Output, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.productCountValueH_NG, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.productCountValueH_NgRate, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.23899F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.75472F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.38365F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.49685F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.49685F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(184, 186);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.BackColor = System.Drawing.Color.LightGray;
            this.labelName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelName.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelName.Location = new System.Drawing.Point(1, 1);
            this.labelName.Margin = new System.Windows.Forms.Padding(1);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(182, 32);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "JIG #01";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // productCountValueH_Input
            // 
            this.productCountValueH_Input.Color = TopEng.Controls.ProductCountValueH.BG_COLOR.TOTAL;
            this.productCountValueH_Input.CountName = "Input";
            this.productCountValueH_Input.CountText = "0";
            this.productCountValueH_Input.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productCountValueH_Input.Location = new System.Drawing.Point(0, 34);
            this.productCountValueH_Input.Margin = new System.Windows.Forms.Padding(0);
            this.productCountValueH_Input.Name = "productCountValueH_Input";
            this.productCountValueH_Input.Size = new System.Drawing.Size(184, 38);
            this.productCountValueH_Input.TabIndex = 1;
            // 
            // productCountValueH_Output
            // 
            this.productCountValueH_Output.Color = TopEng.Controls.ProductCountValueH.BG_COLOR.PASS;
            this.productCountValueH_Output.CountName = "Output";
            this.productCountValueH_Output.CountText = "0";
            this.productCountValueH_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productCountValueH_Output.Location = new System.Drawing.Point(0, 72);
            this.productCountValueH_Output.Margin = new System.Windows.Forms.Padding(0);
            this.productCountValueH_Output.Name = "productCountValueH_Output";
            this.productCountValueH_Output.Size = new System.Drawing.Size(184, 40);
            this.productCountValueH_Output.TabIndex = 1;
            // 
            // productCountValueH_NG
            // 
            this.productCountValueH_NG.Color = TopEng.Controls.ProductCountValueH.BG_COLOR.FAIL;
            this.productCountValueH_NG.CountName = "NG";
            this.productCountValueH_NG.CountText = "0";
            this.productCountValueH_NG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productCountValueH_NG.Location = new System.Drawing.Point(0, 112);
            this.productCountValueH_NG.Margin = new System.Windows.Forms.Padding(0);
            this.productCountValueH_NG.Name = "productCountValueH_NG";
            this.productCountValueH_NG.Size = new System.Drawing.Size(184, 36);
            this.productCountValueH_NG.TabIndex = 1;
            // 
            // productCountValueH_NgRate
            // 
            this.productCountValueH_NgRate.Color = TopEng.Controls.ProductCountValueH.BG_COLOR.FAIL_RATE;
            this.productCountValueH_NgRate.CountName = "NG Rate";
            this.productCountValueH_NgRate.CountText = "0%";
            this.productCountValueH_NgRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productCountValueH_NgRate.Location = new System.Drawing.Point(0, 148);
            this.productCountValueH_NgRate.Margin = new System.Windows.Forms.Padding(0);
            this.productCountValueH_NgRate.Name = "productCountValueH_NgRate";
            this.productCountValueH_NgRate.Size = new System.Drawing.Size(184, 38);
            this.productCountValueH_NgRate.TabIndex = 1;
            // 
            // ProductInfoJIG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ProductInfoJIG";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(186, 188);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Label labelName;
        public ProductCountValueH productCountValueH_Input;
        public ProductCountValueH productCountValueH_Output;
        public ProductCountValueH productCountValueH_NG;
        public ProductCountValueH productCountValueH_NgRate;
    }
}
