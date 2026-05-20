using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.UI
{
    /**
     * @brief SelectMode에 해당하는 UI
     * @todo 물류 프로그램 제작자가 Form 구성 및 기능 구현 진행 해야함
     */
    public partial class Form_SelectMode : Form
    {
        List<ButtonEnh> buttonMode = new List<ButtonEnh>();
        SystemMode selectMode;
        
        public Form_SelectMode()
        {
            InitializeComponent();
        }

        private void SelectMode_Load(object sender, EventArgs e)
        {
            CenterToScreen();

            buttonMode.Add(button_Auto);
            buttonMode.Add(button_ByPass);
            buttonMode.Add(button_DryRun);

            ChangeSystemMode(Machine.status.mode);
        }

        private void ChangeSystemMode(SystemMode mode)
        {
            foreach (var button in buttonMode)
                button.ButtonPush = false;

            switch (mode)
            {
                case SystemMode.SystemModeAUTO:
                    button_Auto.ButtonPush = true;
                    break;

                case SystemMode.SystemModeBYPASS:
                    button_ByPass.ButtonPush = true;
                    break;

                case SystemMode.SystemModeDRYRUN:
                    button_DryRun.ButtonPush = true;
                    break;

                default:
                    break;
            }

            selectMode = mode;
        }

        private void button_Auto_Click(object sender, EventArgs e)
        {
            ChangeSystemMode(SystemMode.SystemModeAUTO);
        }

        private void button_ByPass_Click(object sender, EventArgs e)
        {
            ChangeSystemMode(SystemMode.SystemModeBYPASS);
        }

        private void button_DryRun_Click(object sender, EventArgs e)
        {
            ChangeSystemMode(SystemMode.SystemModeDRYRUN);
        }

        private void button_PassRun_Click(object sender, EventArgs e)
        {
            ChangeSystemMode(SystemMode.SystemModePASSRUN);
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            var form2 = new SubForm_Warning("Do you want Change Mode?", true);
            form2.ShowDialog();
            if (form2.DialogResult == DialogResult.No)
                return;

            if ((Machine.status.mode == SystemMode.SystemModeDRYRUN && selectMode != SystemMode.SystemModeDRYRUN) ||
                (Machine.status.mode != SystemMode.SystemModeDRYRUN && selectMode == SystemMode.SystemModeDRYRUN))
            {
                var form = new SubForm_Warning("All product information in the device will be deleted.", true);
                form.ShowDialog();
                if (form.DialogResult == DialogResult.No)
                    return;

                Machine.ClearProduct();
            }
            Machine.status.mode = selectMode;
            if (Machine.status.mode == SystemMode.SystemModeDRYRUN) Machine.AloneMode = true;
            else Machine.AloneMode = false;
            Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
