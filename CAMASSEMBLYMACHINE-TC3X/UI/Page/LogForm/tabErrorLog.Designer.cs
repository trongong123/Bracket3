
namespace CAMASSEMBLYMACHINE.UI
{
    partial class tabErrorLog
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
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.timerAssemble = new System.Windows.Forms.Timer(this.components);
            this.pageControl = new System.Windows.Forms.TabControl();
            this.tabNormalLog = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabCountError = new System.Windows.Forms.TabPage();
            this.CountListView = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1.SuspendLayout();
            this.pageControl.SuspendLayout();
            this.tabNormalLog.SuspendLayout();
            this.tabCountError.SuspendLayout();
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
            this.label3.Size = new System.Drawing.Size(622, 30);
            this.label3.TabIndex = 36;
            this.label3.Text = "Error Log";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pageControl, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(622, 480);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // timerAssemble
            // 
            this.timerAssemble.Enabled = true;
            // 
            // pageControl
            // 
            this.pageControl.Controls.Add(this.tabNormalLog);
            this.pageControl.Controls.Add(this.tabCountError);
            this.pageControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageControl.Location = new System.Drawing.Point(3, 33);
            this.pageControl.Name = "pageControl";
            this.pageControl.SelectedIndex = 0;
            this.pageControl.Size = new System.Drawing.Size(616, 444);
            this.pageControl.TabIndex = 42;
            // 
            // tabNormalLog
            // 
            this.tabNormalLog.Controls.Add(this.listBox1);
            this.tabNormalLog.Location = new System.Drawing.Point(4, 22);
            this.tabNormalLog.Name = "tabNormalLog";
            this.tabNormalLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabNormalLog.Size = new System.Drawing.Size(608, 418);
            this.tabNormalLog.TabIndex = 0;
            this.tabNormalLog.Text = "tabPage1";
            this.tabNormalLog.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(602, 412);
            this.listBox1.TabIndex = 38;
            // 
            // tabCountError
            // 
            this.tabCountError.Controls.Add(this.CountListView);
            this.tabCountError.Location = new System.Drawing.Point(4, 22);
            this.tabCountError.Name = "tabCountError";
            this.tabCountError.Padding = new System.Windows.Forms.Padding(3);
            this.tabCountError.Size = new System.Drawing.Size(608, 418);
            this.tabCountError.TabIndex = 1;
            this.tabCountError.Text = "tabPage2";
            this.tabCountError.UseVisualStyleBackColor = true;
            // 
            // CountListView
            // 
            this.CountListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CountListView.HideSelection = false;
            this.CountListView.Location = new System.Drawing.Point(3, 3);
            this.CountListView.Name = "CountListView";
            this.CountListView.Size = new System.Drawing.Size(602, 412);
            this.CountListView.TabIndex = 0;
            this.CountListView.UseCompatibleStateImageBehavior = false;
            this.CountListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CountListView_KeyDown);
            // 
            // tabErrorLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 480);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "tabErrorLog";
            this.Text = "tabPositionData";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pageControl.ResumeLayout(false);
            this.tabNormalLog.ResumeLayout(false);
            this.tabCountError.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Timer timerAssemble;
        private System.Windows.Forms.TabControl pageControl;
        private System.Windows.Forms.TabPage tabNormalLog;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabPage tabCountError;
        private System.Windows.Forms.ListView CountListView;
    }
}