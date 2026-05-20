using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Utils;
using TopEng.Controls;
using CAMASSEMBLYMACHINE.Define;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_PanePickerJog : Form
    {
        UIJOGVELINFO veltype = UIJOGVELINFO.NORMAL;
        List<ButtonEnh> buttonVel = new List<ButtonEnh>();

        enum DIR
        {
            X,
            Y,
            Z,
            R1,
            R2,

            MAX_DIRECTION
        }

        enum PICKER
        {
            LD = 0,
            ULD = 1
        }

        int[,] axislist =
        {
            { (int)AXIS.PROD_PICKUP_X, (int)AXIS.ASSEMBLER_X },
            { (int)AXIS.PROD_PICKUP_Y, (int)AXIS.ASSEMBLER_Y },
            { (int)AXIS.PROD_PICKUP_Z, (int)AXIS.ASSEMBLER_Z },
            { (int)AXIS.PROD_PICKUP_R1, (int)AXIS.ASSEMBLER_R1 },
            { (int)AXIS.PROD_PICKUP_R2, (int)AXIS.ASSEMBLER_R2 }
        };

        int unitNo = (int)PICKER.LD;

        public Form_PanePickerJog(int picker)
        {
            InitializeComponent();
            unitNo = picker;

            buttonVel.Add(button_VelSlowest);
            buttonVel.Add(button_VelSlower);
            buttonVel.Add(button_VelNormal);
            buttonVel.Add(button_VelFaster);
            buttonVel.Add(button_VelFastest);

            ChangeVelocity(UIJOGVELINFO.NORMAL);
        }

        #region VELOCITY SELECTION
        private void ChangeVelocity(UIJOGVELINFO newVel)
        {
            buttonVel[(int)veltype].ButtonPush = false;
            veltype = newVel;
            buttonVel[(int)veltype].ButtonPush = true;
        }

        private void button_VelSlowest_Click(object sender, EventArgs e)
        {
            ChangeVelocity(UIJOGVELINFO.SLOWEST);
        }

        private void button_VelSlower_Click(object sender, EventArgs e)
        {
            ChangeVelocity(UIJOGVELINFO.SLOWER);
        }

        private void button_VelNormal_Click(object sender, EventArgs e)
        {
            ChangeVelocity(UIJOGVELINFO.NORMAL);
        }

        private void button_VelFaster_Click(object sender, EventArgs e)
        {
            ChangeVelocity(UIJOGVELINFO.FASTER);
        }

        private void button_VelFastest_Click(object sender, EventArgs e)
        {
            ChangeVelocity(UIJOGVELINFO.FASTEST);
        }
        #endregion
        #region JOG
        private void MoveJogAxis(int axis, bool direction)
        {
            string dirStr = direction == true ? "Plus" : "Minus";
            LogUtil.Instance.Log(LOG_TYPE.UI, ((AXIS)axis).ToString() + " Move Jog to " + dirStr, CONTENT_TYPE.INFO);

            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            double velocity = UIJOGVEL[(int)veltype];
            double acceleration = 2 * velocity;
            double delceleration = 2 * velocity;
            if (!direction)
                velocity *= (-1.0);

            Machine.motion.MoveAxisJog(axis, velocity, acceleration, delceleration);
            Machine.usingMoveJog = true;
        }

        private void button_PickerX_NJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (unitNo == (int)PICKER.LD) 
                Machine.interfer_release_x_safe_area = true;
            MoveJogAxis(axislist[(int)DIR.X, unitNo], false);
        }

        private void button_PickerX_NJog_MouseUp(object sender, MouseEventArgs e)
        {
            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_x_safe_area = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerX_PJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_x_safe_area = true;
            MoveJogAxis(axislist[(int)DIR.X, unitNo], true);
        }

        private void button_PickerX_PJog_MouseUp(object sender, MouseEventArgs e)
        {
            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_x_safe_area = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerY_NJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_y_crash = true;
            MoveJogAxis(axislist[(int)DIR.Y, unitNo], false);
        }

        private void button_PickerY_NJog_MouseUp(object sender, MouseEventArgs e)
        {
            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_y_crash = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerY_PJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_x_safe_area = true;
            else if (unitNo == (int)PICKER.ULD)
                Machine.interfer_release_y_crash = true;
            MoveJogAxis(axislist[(int)DIR.Y, unitNo], true);
        }

        private void button_PickerY_PJog_MouseUp(object sender, MouseEventArgs e)
        {
            if (unitNo == (int)PICKER.LD)
                Machine.interfer_release_x_safe_area = false;
            else if (unitNo == (int)PICKER.ULD)
                Machine.interfer_release_y_crash = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerZ_NJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            MoveJogAxis(axislist[(int)DIR.Z, unitNo], false);
        }

        private void button_PickerZ_NJog_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerZ_PJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            MoveJogAxis(axislist[(int)DIR.Z, unitNo], true);
        }

        private void button_PickerZ_PJog_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerR1_NJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            MoveJogAxis(axislist[(int)DIR.R1, unitNo], false);
        }

        private void button_PickerR1_NJog_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerR1_PJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            MoveJogAxis(axislist[(int)DIR.R1, unitNo], true);
        }

        private void button_PickerR1_PJog_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerR2_NJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            MoveJogAxis(axislist[(int)DIR.R2, unitNo], false);
        }

        private void button_PickerR2_NJog_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_PickerR2_PJog_MouseDown(object sender, MouseEventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            MoveJogAxis(axislist[(int)DIR.R2, unitNo], true);
        }

        private void button_PickerR2_PJog_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }
        #endregion
    }
}
