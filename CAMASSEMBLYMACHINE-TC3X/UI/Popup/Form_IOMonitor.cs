using TopEng.Device;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Utils;
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
     * @brief IO Monitor 에 해당하는 UI
     */
    public partial class Form_IOMonitor : Form
    {
        public const string NAME = "IOMonitor";
        //UI Framework
        private IIO iIO;

        private List<string> inputText = new List<string>();
        private int inputPage = 0;
        private int currInPage = -1;
        private List<string> outputText = new List<string>();
        private int outputPage = 0;
        private int currOutPage = -1;

        private List<TopEng.Controls.ButtonEnh> inputButton = new List<TopEng.Controls.ButtonEnh>();
        private List<TopEng.Controls.ButtonEnh> outputButton = new List<TopEng.Controls.ButtonEnh>();

        public object lockControl = new object();
        private uint[] inputState = new uint[16];
        private uint[] outputState = new uint[16];
        private bool refreshData = true;
        private bool refreshData2 = false;
        private bool miniMode = false;
        private bool updatingNow = false;

        bool isViewEMode = false;

        /**
         * @brief 생성자
         * @param[in] io IO interface
         */
        public Form_IOMonitor(IIO IO)
        {
            InitializeComponent();
            CenterToScreen();

            iIO = IO;

            inputButton.Add(input0);
            inputButton.Add(input1);
            inputButton.Add(input2);
            inputButton.Add(input3);
            inputButton.Add(input4);
            inputButton.Add(input5);
            inputButton.Add(input6);
            inputButton.Add(input7);
            inputButton.Add(input8);
            inputButton.Add(input9);
            inputButton.Add(input10);
            inputButton.Add(input11);
            inputButton.Add(input12);
            inputButton.Add(input13);
            inputButton.Add(input14);
            inputButton.Add(input15);

            outputButton.Add(output0);
            outputButton.Add(output1);
            outputButton.Add(output2);
            outputButton.Add(output3);
            outputButton.Add(output4);
            outputButton.Add(output5);
            outputButton.Add(output6);
            outputButton.Add(output7);
            outputButton.Add(output8);
            outputButton.Add(output9);
            outputButton.Add(output10);
            outputButton.Add(output11);
            outputButton.Add(output12);
            outputButton.Add(output13);
            outputButton.Add(output14);
            outputButton.Add(output15);

            if (!Machine.DeveloperMode)
                button_ChangeE.Visible = false;
        }

        private void Form_IOMonitor_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < (int)DI.IOMAX; i++)
            {
                string name = Enum.GetName(typeof(DI), i);
                if (name.Contains("SP0x"))
                    name = "";
                inputText.Add(name);
            }
            inputPage = (int)DI.IOMAX / 16;
            if ((int)DI.IOMAX % 16 > 0) inputPage++;

            for (int i = 0; i < (int)DO.IOMAX; i++)
            {
                string name = Enum.GetName(typeof(DO), i);
                if (name.Contains("SP0x"))
                    name = "";
                outputText.Add(name);
            }
            outputPage = (int)DO.IOMAX / 16;
            if ((int)DO.IOMAX % 16 > 0) outputPage++;

            ChagePage(0, 0);

            if (Machine.sysMode == Machine.SYSMODE.AUTO || Machine.status.state != SystemState.SystemStateIDLE)
            {
                foreach (var button in outputButton)
                    button.ButtonType = ButtonEnh.BUTTONTYPE.Display;
            }
        }

        private void ChagePage(int inPage, int outPage)
        {
            if (inPage >= inputPage) inPage = 0;
            if (inPage < 0) inPage = inputPage - 1;
            if (outPage >= outputPage) outPage = 0;
            if (outPage < 0) outPage = outputPage - 1;

            if (currInPage != inPage || refreshData || refreshData2)
            {
                int startIn = inPage * 16;
                for (int i = 0; i < inputButton.Count; i++)
                {
                    if (startIn + i >= inputText.Count)
                        break;

                    inputButton[i].Text = "";
                    if (!miniMode)
                    {
                        if (isViewEMode)
                        {
                            int numI1 = (startIn + i) / 16 + 1;
                            int numI2 = (startIn + i) % 16;

                            inputButton[i].Text = $"[E{numI1:D2}" + "- " +  $"X{numI2:X2}]\n";
                        }
                        else
                        {
                            inputButton[i].Text = $"[X{startIn + i:X3}] ";
                        }
                    }
                    inputButton[i].Text += inputText[startIn + i].Replace("_", " ");
                    inputButton[i].Enabled = inputText[startIn + i] == "" ? false : true;
                }
                currInPage = inPage;
                labelInputRange.Text = $"X{startIn + (0):X3}" +
                      " ~ " + $"X{startIn + (inputButton.Count - 1):X3}";
                label_InPage.Text = $"{currInPage + 1} / {inputPage}";
            }

            if (currOutPage != outPage || refreshData || refreshData2)
            {
                int startOut = outPage * 16;
                for (int i = 0; i < outputButton.Count; i++)
                {
                    if (startOut + i >= outputText.Count)
                        break;

                    outputButton[i].Text = "";
                    if (!miniMode)
                    {
                        if (isViewEMode)
                        {
                            int numO1 = (startOut + i) / 16 + 1 + 13;
                            int numO2 = (startOut + i) % 16;

                            outputButton[i].Text = $"[E{numO1:D2}" + "- " + $"Y{numO2:X2}]\n";
                        }
                        else
                        {
                            outputButton[i].Text = $"[Y{startOut + i:X3}] ";
                        }
                    }
                    outputButton[i].Text += outputText[startOut + i].Replace("_", " ");
                    outputButton[i].Enabled = outputText[startOut + i] == "" ? false : true;
                }
                currOutPage = outPage;
                labelOutputRange.Text = $"Y{startOut + (0):X3}" +
                         " ~ " + $"Y{startOut + (outputButton.Count - 1):X3}";
                label_OutPage.Text = $"{currOutPage + 1} / {outputPage}";
            }
        }

        private void button_InPrev_Click(object sender, EventArgs e)
        {
            ChagePage(currInPage - 1, currOutPage);
        }
        private void button_InHome_Click(object sender, EventArgs e)
        {
            ChagePage(0, currOutPage);
        }
        private void button_InNext_Click(object sender, EventArgs e)
        {
            ChagePage(currInPage + 1, currOutPage);
        }
        private void button_OutPrev_Click(object sender, EventArgs e)
        {
            ChagePage(currInPage, currOutPage - 1);
        }
        private void button_OutHome_Click(object sender, EventArgs e)
        {
            ChagePage(currInPage, 0);
        }
        private void button_OutNext_Click(object sender, EventArgs e)
        {
            ChagePage(currInPage, currOutPage + 1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updatingNow)
                return;
            updatingNow = true;
            UpdateIOState();
            updatingNow = false;
        }

        private void UpdateIOState()
        {
            for (int i = 0; i < 16; i++)
            {
                if (iIO == null)
                {
                    inputButton[i].ButtonPush = false;
                    continue;
                }

                int inBit = currInPage * 16 + i;
                uint value = 0;
                iIO.GetIn(inBit, ref value);

                if (inputState[i] != value || refreshData)
                {
                    inputButton[i].ButtonPush = Convert.ToBoolean(value);
                    inputState[i] = value;
                }
            }

            lock (lockControl)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (iIO == null)
                    {
                        outputButton[i].ButtonPush = false;
                        continue;
                    }

                    int outBit = currOutPage * 16 + i;
                    uint value = 0;
                    iIO.GetOut(outBit, ref value);

                    if (outputState[i] != value || refreshData
                        || outputState[i] != Convert.ToUInt16(outputButton[i].ButtonPush))
                    {
                        outputButton[i].ButtonPush = Convert.ToBoolean(value);
                        outputState[i] = value;
                    }
                }
            }

            refreshData = false;
        }

        private void OutputClick(int Bit)
        {
            if (Machine.status.state == SystemState.SystemStateRUN || Machine.status.state == SystemState.SystemStateRUNEMPTY)
                return;

            if (iIO == null)
                return;

            lock (lockControl)
            {
                int outBit = currOutPage * 16 + Bit;

                uint value = 0;
                iIO.GetOut(outBit, ref value);
                iIO.SetOut(outBit, (uint)(value == 0 ? 1 : 0));
            }
        }

        private void output0_Click(object sender, EventArgs e)
        {
            OutputClick(0);
        }
        private void output1_Click(object sender, EventArgs e)
        {
            OutputClick(1);
        }
        private void output2_Click(object sender, EventArgs e)
        {
            OutputClick(2);
        }
        private void output3_Click(object sender, EventArgs e)
        {
            OutputClick(3);
        }
        private void output4_Click(object sender, EventArgs e)
        {
            OutputClick(4);
        }
        private void output5_Click(object sender, EventArgs e)
        {
            OutputClick(5);
        }
        private void output6_Click(object sender, EventArgs e)
        {
            OutputClick(6);
        }
        private void output7_Click(object sender, EventArgs e)
        {
            OutputClick(7);
        }
        private void output8_Click(object sender, EventArgs e)
        {
            OutputClick(8);
        }
        private void output9_Click(object sender, EventArgs e)
        {
            OutputClick(9);
        }
        private void output10_Click(object sender, EventArgs e)
        {
            OutputClick(10);
        }
        private void output11_Click(object sender, EventArgs e)
        {
            OutputClick(11);
        }
        private void output12_Click(object sender, EventArgs e)
        {
            OutputClick(12);
        }
        private void output13_Click(object sender, EventArgs e)
        {
            OutputClick(13);
        }
        private void output14_Click(object sender, EventArgs e)
        {
            OutputClick(14);
        }
        private void output15_Click(object sender, EventArgs e)
        {
            OutputClick(15);
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button_MinMax_Click(object sender, EventArgs e)
        {
            if (button_MinMax.ButtonPush)
            {
                Size = new Size(1020, 753);
                CenterToScreen();
                miniMode = false;
            }
            else
            {
                Size = new Size(350, 753);
                Location = new Point(Location.X + 670, Location.Y);
                miniMode = true;
            }

            refreshData = true;
            ChagePage(currInPage, currOutPage);
        }

        private void button_ChangeE_Click(object sender, EventArgs e)
        {
            isViewEMode = !isViewEMode;
            refreshData2 = true;
            ChagePage(currInPage, currOutPage);
            refreshData2 = false;
        }
    }
}
