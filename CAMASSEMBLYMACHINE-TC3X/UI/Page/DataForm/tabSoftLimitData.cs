using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Utils;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabSoftLimitData : Form
    {
        uint[] isUse;

        public enum LIMITPARAMCOLDATA
        {
            NAME,
            USE,
            STOREDP,
            EDITEDP,
            GETPOSP,
            STOREDN,
            EDITEDN,
            GETPOSN,
            ACTPOS,
            ACCEPT,

            LIMITPARAMCOLTEACHMAX
        };

        public tabSoftLimitData()
        {
            InitializeComponent();
        }

        private void tabParamterData_Load(object sender, EventArgs e)
        {
            InitializeParamInfo();
        }

        private void tabParamterData_Shown(object sender, EventArgs e)
        {
        }

        public void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = {"NAME","USE",
                "Stored (+)", "New (+)", "GET",
                "Stored (-)", "New (-)", "GET",
                "ActPos", "Save"
            };
            int[] headersize = { 170, 80,
                90, 90, 40,
                90, 90, 40, 
                90, 90
            };
            string[] headertype = { "TEXT", "BUTTON",
                "TEXT", "TEXT", "BUTTON", 
                "TEXT", "TEXT", "BUTTON", 
                "TEXT", "BUTTON" };

            dataGridView1.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                {
                    dataGridView1.Columns.Add(headername[i], headername[i]);
                }
                if (headertype[i] == "BUTTON")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.Name = headername[i];
                    newColumn.Text = headername[i];
                    dataGridView1.Columns.Add(newColumn);
                }

                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[i].Width = headersize[i];
            }

            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;

            uint upUse = 0; uint upStopMode = 0; uint upSelection = 0;
            double dpPositivePos = 0; double dpNegativePos = 0;

            isUse = new uint[(int)AXIS.AXISMAX];

            // ROWS
            for (int i = 0; i < (int)AXIS.AXISMAX; ++i)
            {
                CAXM.AxmSignalGetSoftLimit
                    (i, ref upUse, ref upStopMode, ref upSelection,
                    ref dpPositivePos, ref dpNegativePos);
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 30;
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.NAME].Value =
                    ((AXIS)i).ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.USE].Value =
                upUse == 1 ? "YES" : "NO";
                isUse[i] = upUse;
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.STOREDP].Value =
                    (dpPositivePos / 1000.0).ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.EDITEDP].Value =
                    (dpPositivePos / 1000.0).ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.STOREDN].Value =
                    (dpNegativePos / 1000.0).ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.EDITEDN].Value =
                    (dpNegativePos / 1000.0).ToString();
            }

            dataGridView1.CurrentCell = null;
        }

        public void UpdateData()
        {
            uint upUse = 0; uint upStopMode = 0; uint upSelection = 0;
            double dpPositivePos = 0; double dpNegativePos = 0;

            for (int i = 0; i < (int)AXIS.AXISMAX; ++i)
            {
                CAXM.AxmSignalGetSoftLimit
                    (i, ref upUse, ref upStopMode, ref upSelection,
                    ref dpPositivePos, ref dpNegativePos);

                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.NAME].Value =
                    ((AXIS)i).ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.USE].Value =
                upUse == 1 ? "YES" : "NO";
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.STOREDP].Value =
                    dpPositivePos.ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.EDITEDP].Value =
                    dpPositivePos.ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.STOREDN].Value =
                    dpNegativePos.ToString();
                dataGridView1.Rows[i].Cells[(int)LIMITPARAMCOLDATA.EDITEDN].Value =
                    dpNegativePos.ToString();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)LIMITPARAMCOLDATA.GETPOSP)
                    {
                        int targetAxis = row;
                        double actualPos = 0;
                        Machine.motion.GetAxisActualPos(targetAxis, ref actualPos);
                        dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDP].Value = (0.001 * actualPos).ToString("0.000");
                    }
                    if (col == (int)LIMITPARAMCOLDATA.GETPOSN)
                    {
                        int targetAxis = row;
                        double actualPos = 0;
                        Machine.motion.GetAxisActualPos(targetAxis, ref actualPos);
                        dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDN].Value = (0.001 * actualPos).ToString("0.000");
                    }
                    if (col == (int)LIMITPARAMCOLDATA.USE)
                    {
                        if (dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Value.ToString() == "YES")
                        {
                            dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Value = "NO";
                            dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Style.ForeColor = Color.Black;
                        }
                        else if (dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Value.ToString() == "NO")
                        {
                            dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Value = "YES";
                            dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Style.ForeColor = Color.Black;
                        }
                    }

                    if (col == (int)LIMITPARAMCOLDATA.ACCEPT)
                    {
                        string newValueP = dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDP].Value.ToString();
                        string curValueP = dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.STOREDP].Value.ToString();
                        string newValueN = dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDN].Value.ToString();
                        string curValueN = dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.STOREDN].Value.ToString();

                        double dDifValueP = Convert.ToDouble(newValueP) - Convert.ToDouble(curValueP);
                        double dDifValueN = Convert.ToDouble(newValueN) - Convert.ToDouble(curValueN);
                        Dlg_MessageBox formCurrent = new Dlg_MessageBox
                            (EMESSAGEBOX.WARNING,
                            string.Format(
                               ((AXIS)row).ToString() + "의\n" +
                                "(+) Limit " + dDifValueP.ToString("N3") + "\n" +
                            "(-) Limit " + dDifValueN.ToString("N3") + "\n" +
                            " is Changed\n" + "Do you want to Save?"));
                        
                        if (formCurrent.ShowDialog() == DialogResult.Yes)
                        {                            
                            Dlg_MessageBox formCheck = new Dlg_MessageBox
                            (EMESSAGEBOX.WARNING,
                            string.Format(
                                "정말로 저장하시겠습니까?"));

                            dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.STOREDP].Value = newValueP;
                            dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.STOREDN].Value = newValueN;

                            // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
                            // uSelection : COMMAND(0), ACTUAL(1)

                            uint upUseOld = isUse[row];
                            string upUseOldStr= upUseOld == 1 ? "USE" : "NOT USE";
                            uint upUse =
                                dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.USE].Value.ToString() == "YES"
                                ? (uint)1 : (uint)0;
                            string upUseStr = upUse == 1 ? "USE" : "NOT USE";
                            isUse[row] = upUse;

                            uint upStopMode = 0; uint upSelection = 1;
                            double dpPositivePos = Convert.ToDouble(newValueP);
                            double dpNegativePos = Convert.ToDouble(newValueN);

                            CAXM.AxmSignalSetSoftLimit(row, upUse, upStopMode, upSelection,
                                dpPositivePos * 1000, dpNegativePos * 1000);
                            string logText = $"[Soft Limit : " +
                                $"[{upUseOldStr} → {upUseStr}] " +
                                $"(+) :[{curValueP} → {newValueP}] " +
                                $"(-) :[{curValueN} → {newValueN}] ";
                            LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    if (col == (int)LIMITPARAMCOLDATA.EDITEDP)
                    {
                        string[] value = { dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDP].Value.ToString() };

                        SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "Limit", "", -999, 999, 300, 300, true);
                        keyPad.ShowDialog();
                        dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDP].Value = value[0];
                    }
                    if (col == (int)LIMITPARAMCOLDATA.EDITEDN)
                    {
                        string[] value = { dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDN].Value.ToString() };

                        SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "Limit", "", -999, 999, 300, 300, true);
                        keyPad.ShowDialog();
                        dataGridView1.Rows[row].Cells[(int)LIMITPARAMCOLDATA.EDITEDN].Value = value[0];
                    }
                }
                catch
                {

                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount <= 0) return;

            double actualPos = 0;
            for (int axis = 0; axis < (int)AXIS.AXISMAX; ++axis)
            {
                Machine.motion.GetAxisActualPos((int)axis, ref actualPos);
                dataGridView1.Rows[axis].Cells[(int)LIMITPARAMCOLDATA.ACTPOS].Value =
                    (actualPos / 1000.0).ToString("N3");
            }
        }
    }
}
