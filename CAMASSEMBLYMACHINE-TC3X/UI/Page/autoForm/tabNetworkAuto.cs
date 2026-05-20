using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;
using Cognex.VisionPro;
using TopEng.Controls;
using TopEng.Vision.Forms;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabNetworkAuto : Form, IForm
    {
        Image imgLED_Gray = Properties.Resources.LED_GRAY;
        Image imgLED_Red = Properties.Resources.LED_RED;
        Image imgLED_Yellow = Properties.Resources.LED_YELLOW;
        Image imgLED_Green = Properties.Resources.LED_GREEN;

        public int highlightDisp = -1;
        bool updatingNow = false;

        int printCount = 0; 

        enum MACHINENAMEINFO
        {
            MACHINE_TURN,
            TOTAL
        }

        enum NETWORKGRIDCOLUMN
        {
            MACHINENAME,
            CHECKCONNECT,
            IP,
            PORT
        }

        public tabNetworkAuto()
        {
            InitializeComponent();
            InitializeNetworkInfo();
        }

        public void InitializeNetworkInfo()
        {
            // COLUMNS
            string[] headername = { "MachineName", "Connect", "IP Address", "Port" };
            int width = dataGridView1.Width / 4;
            int[] headersize = { width, width, width, width };
            string[] headertype = { "TEXT", "SIGNAL", "TEXT", "TEXT" };

            dataGridView1.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                    dataGridView1.Columns.Add(headername[i], headername[i]);
                if (headertype[i] == "SIGNAL")
                {
                    DataGridViewImageColumn newColumn = new DataGridViewImageColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    newColumn.Name = headername[i];
                    dataGridView1.Columns.Add(newColumn);
                }
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[i].Width = headersize[i];
            }

            for (int i = 0; i < (int)MACHINENAMEINFO.TOTAL; ++i)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[(int)i].Height = 30;
                dataGridView1.Rows[(int)i].Cells[(int)NETWORKGRIDCOLUMN.MACHINENAME].Value = ((MACHINENAMEINFO)i).ToString();
                dataGridView1.Rows[(int)i].Cells[(int)NETWORKGRIDCOLUMN.CHECKCONNECT].Value = imgLED_Gray;
                dataGridView1.Rows[(int)i].Cells[(int)NETWORKGRIDCOLUMN.IP].Value = SystemDefine.externalEquipServerIP;
                dataGridView1.Rows[(int)i].Cells[(int)NETWORKGRIDCOLUMN.PORT].Value = SystemDefine.externalEquipServerPort;
            }

            dataGridView1.CurrentCell = null;
        }

        private void tabNetworkAuto_Load(object sender, EventArgs e)
        {
        }

        public void StartTimer(bool enable) => timer1.Enabled = enable;


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updatingNow)
                return;
            updatingNow = true;

            var procEx = Machine.proclist[(int)Machine.PROCESS.EXTERNALEQUIP] as ProcessExternalEquip;

            for (int i = 0; i < dataGridView1.RowCount; ++i)
            {
                dataGridView1.Rows[i].
               Cells[(int)NETWORKGRIDCOLUMN.CHECKCONNECT].Value = procEx.GetConnected() == true ? imgLED_Green : imgLED_Gray;
            }

            //선택한 PO Machine 값 업데이트
            label_POType.Text = Vision.inspection.recipeInfo.PO_TYPE.ToString();
            label_PODataNum.Text = Vision.inspection.recipeInfo.PO_DATA_NUMBER.ToString();
            label_POData.Text = Vision.inspection.recipeInfo.PO_DATA.ToString();
            updatingNow = false;
        }

        private void button_CheckSetPO_Click(object sender, EventArgs e)
        {
            Dlg_MessageBox msgBoxW2 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                             string.Format("Do you want to Check PO ?"));
            if (msgBoxW2.ShowDialog() == DialogResult.Yes)
            {
                var proc1 = Machine.proclist[(int)Machine.PROCESS.EXTERNALEQUIP];
                proc1.SetMessage((int)ProcessExternalEquip.MSG.MSG_CHECK_PO);
            }
        }

    }
}
