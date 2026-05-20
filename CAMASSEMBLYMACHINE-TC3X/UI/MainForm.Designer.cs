
namespace CAMASSEMBLYMACHINE.UI
{
    partial class MainForm
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
            this.panel_mainTop = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_mainBottom = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_mainPage = new System.Windows.Forms.Panel();
            this.panel_mainMenu = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel_mainBottom.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_mainTop
            // 
            this.panel_mainTop.BackColor = System.Drawing.SystemColors.Control;
            this.panel_mainTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_mainTop.Location = new System.Drawing.Point(0, 0);
            this.panel_mainTop.Margin = new System.Windows.Forms.Padding(0);
            this.panel_mainTop.Name = "panel_mainTop";
            this.panel_mainTop.Size = new System.Drawing.Size(1022, 89);
            this.panel_mainTop.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel_mainBottom, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel_mainTop, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 1);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1022, 766);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel_mainBottom
            // 
            this.panel_mainBottom.BackColor = System.Drawing.SystemColors.Control;
            this.panel_mainBottom.Controls.Add(this.panel5);
            this.panel_mainBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_mainBottom.Location = new System.Drawing.Point(0, 691);
            this.panel_mainBottom.Margin = new System.Windows.Forms.Padding(0);
            this.panel_mainBottom.Name = "panel_mainBottom";
            this.panel_mainBottom.Size = new System.Drawing.Size(1022, 75);
            this.panel_mainBottom.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.Control;
            this.panel5.Location = new System.Drawing.Point(189, 170);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(200, 100);
            this.panel5.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 191F));
            this.tableLayoutPanel2.Controls.Add(this.panel_mainPage, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel_mainMenu, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 89);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1021, 601);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // panel_mainPage
            // 
            this.panel_mainPage.BackColor = System.Drawing.SystemColors.Control;
            this.panel_mainPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_mainPage.Location = new System.Drawing.Point(0, 0);
            this.panel_mainPage.Margin = new System.Windows.Forms.Padding(0);
            this.panel_mainPage.Name = "panel_mainPage";
            this.panel_mainPage.Size = new System.Drawing.Size(830, 601);
            this.panel_mainPage.TabIndex = 1;
            // 
            // panel_mainMenu
            // 
            this.panel_mainMenu.BackColor = System.Drawing.SystemColors.Control;
            this.panel_mainMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_mainMenu.Location = new System.Drawing.Point(830, 0);
            this.panel_mainMenu.Margin = new System.Windows.Forms.Padding(0);
            this.panel_mainMenu.Name = "panel_mainMenu";
            this.panel_mainMenu.Size = new System.Drawing.Size(191, 601);
            this.panel_mainMenu.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel_mainBottom.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_mainTop;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel_mainBottom;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel_mainPage;
        private System.Windows.Forms.Panel panel_mainMenu;
    }
}