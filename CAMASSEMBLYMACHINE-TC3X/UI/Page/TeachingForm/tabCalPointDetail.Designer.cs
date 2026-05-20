
namespace CAMASSEMBLYMACHINE.UI
{
    partial class tabCalPointDetail
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lblSelected = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.nV_Button_PB_NS2 = new NV_UI.NV_Button_PB_NS();
            this.btnSelectNGLeft = new NV_UI.NV_Button_PB_NS();
            this.btnEditVppLEft = new NV_UI.NV_Button_PB_NS();
            this.btnCalLeft = new NV_UI.NV_Button_PB_NS();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dataGridView, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.547739F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 56.53267F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.9196F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(619, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Gulim", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Gulim", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.GridColor = System.Drawing.Color.DimGray;
            this.dataGridView.Location = new System.Drawing.Point(0, 42);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidth = 51;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.ShowCellErrors = false;
            this.dataGridView.ShowEditingIcon = false;
            this.dataGridView.ShowRowErrors = false;
            this.dataGridView.Size = new System.Drawing.Size(619, 254);
            this.dataGridView.TabIndex = 44;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_CurrentCellDirtyStateChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(619, 42);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.lblSelected, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.lblTotal, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(309, 42);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // lblSelected
            // 
            this.lblSelected.AutoSize = true;
            this.lblSelected.BackColor = System.Drawing.Color.White;
            this.lblSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelected.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSelected.Location = new System.Drawing.Point(158, 1);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(147, 40);
            this.lblSelected.TabIndex = 1;
            this.lblSelected.Text = "Selected: 12";
            this.lblSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.BackColor = System.Drawing.Color.White;
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotal.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTotal.Location = new System.Drawing.Point(4, 1);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(147, 40);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "Total: 12";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 299);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(613, 148);
            this.tableLayoutPanel3.TabIndex = 45;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 4;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.nV_Button_PB_NS2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnSelectNGLeft, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnEditVppLEft, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnCalLeft, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(605, 66);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // nV_Button_PB_NS2
            // 
            this.nV_Button_PB_NS2.BackColor = System.Drawing.Color.Transparent;
            this.nV_Button_PB_NS2.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Default;
            this.nV_Button_PB_NS2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.nV_Button_PB_NS2.ClickImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Click;
            this.nV_Button_PB_NS2.DefaultHover = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Hover;
            this.nV_Button_PB_NS2.DefaultImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Default;
            this.nV_Button_PB_NS2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nV_Button_PB_NS2.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nV_Button_PB_NS2.Location = new System.Drawing.Point(0, 0);
            this.nV_Button_PB_NS2.Margin = new System.Windows.Forms.Padding(0);
            this.nV_Button_PB_NS2.Name = "nV_Button_PB_NS2";
            this.nV_Button_PB_NS2.Size = new System.Drawing.Size(151, 66);
            this.nV_Button_PB_NS2.TabIndex = 953;
            this.nV_Button_PB_NS2.Tag = "SELECT_ALL";
            this.nV_Button_PB_NS2.Text = "Move XY";
            this.nV_Button_PB_NS2.ClickEvent += new NV_UI.NV_Button_PB_NS.ClickEventDelegate(this.nV_Button_PB_NS2_ClickEvent);
            // 
            // btnSelectNGLeft
            // 
            this.btnSelectNGLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectNGLeft.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Default;
            this.btnSelectNGLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSelectNGLeft.ClickImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Click;
            this.btnSelectNGLeft.DefaultHover = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Hover;
            this.btnSelectNGLeft.DefaultImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Default;
            this.btnSelectNGLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSelectNGLeft.Font = new System.Drawing.Font("Malgun Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectNGLeft.Location = new System.Drawing.Point(453, 0);
            this.btnSelectNGLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnSelectNGLeft.Name = "btnSelectNGLeft";
            this.btnSelectNGLeft.Size = new System.Drawing.Size(152, 66);
            this.btnSelectNGLeft.TabIndex = 951;
            this.btnSelectNGLeft.Tag = "SELECT_ALL";
            this.btnSelectNGLeft.Text = "Set Predicted XY";
            this.btnSelectNGLeft.ClickEvent += new NV_UI.NV_Button_PB_NS.ClickEventDelegate(this.btnSetPredictedData_ClickEvent);
            // 
            // btnEditVppLEft
            // 
            this.btnEditVppLEft.BackColor = System.Drawing.Color.Transparent;
            this.btnEditVppLEft.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Main_Click;
            this.btnEditVppLEft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEditVppLEft.ClickImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_On;
            this.btnEditVppLEft.DefaultHover = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_On_Hover;
            this.btnEditVppLEft.DefaultImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_Main_Click;
            this.btnEditVppLEft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEditVppLEft.Font = new System.Drawing.Font("Malgun Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEditVppLEft.Location = new System.Drawing.Point(302, 0);
            this.btnEditVppLEft.Margin = new System.Windows.Forms.Padding(0);
            this.btnEditVppLEft.Name = "btnEditVppLEft";
            this.btnEditVppLEft.Size = new System.Drawing.Size(151, 66);
            this.btnEditVppLEft.TabIndex = 950;
            this.btnEditVppLEft.Tag = "";
            this.btnEditVppLEft.Text = "Edit VPP";
            this.btnEditVppLEft.ClickEvent += new NV_UI.NV_Button_PB_NS.ClickEventDelegate(this.btnEditVpp_ClickEvent);
            // 
            // btnCalLeft
            // 
            this.btnCalLeft.BackColor = System.Drawing.Color.Transparent;
            this.btnCalLeft.BackgroundImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_On;
            this.btnCalLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCalLeft.ClickImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_On;
            this.btnCalLeft.DefaultHover = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_On_Hover;
            this.btnCalLeft.DefaultImage = global::CAMASSEMBLYMACHINE.Properties.Resources.NV_MainUI_Common_Button_On;
            this.btnCalLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCalLeft.Font = new System.Drawing.Font("Malgun Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCalLeft.Location = new System.Drawing.Point(151, 0);
            this.btnCalLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnCalLeft.Name = "btnCalLeft";
            this.btnCalLeft.Size = new System.Drawing.Size(151, 66);
            this.btnCalLeft.TabIndex = 22;
            this.btnCalLeft.Tag = "";
            this.btnCalLeft.Text = "Cal Selected Point";
            this.btnCalLeft.ClickEvent += new NV_UI.NV_Button_PB_NS.ClickEventDelegate(this.btnCal_ClickEvent);
            // 
            // tabCalPointDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "tabCalPointDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "tabCalPointDetail";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.tabCalPointDetail_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private NV_UI.NV_Button_PB_NS btnCalLeft;
        private NV_UI.NV_Button_PB_NS btnEditVppLEft;
        private NV_UI.NV_Button_PB_NS btnSelectNGLeft;
        private NV_UI.NV_Button_PB_NS nV_Button_PB_NS2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lblSelected;
        private System.Windows.Forms.Label lblTotal;
    }
}