using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Add

namespace TopEng.Controls
{
    public partial class Dlg_MessageBox : Form
    {
        private bool _DontAskAgain;
        public bool DontAskAgain
        {
            get { return this._DontAskAgain; }
            set { this._DontAskAgain = value; }
        }

        private bool m_bFlicker;
        private EMESSAGEBOX m_iType;
        static public List<Dlg_MessageBox> listMsgBox = new List<Dlg_MessageBox>();
        public Dlg_MessageBox()
        {
            InitializeComponent();
        }

        public Dlg_MessageBox(EMESSAGEBOX iType, string sMsg, string sPath = "")
        {
            InitializeComponent();

            m_bFlicker = false;
            m_iType = iType;

            InitUI(m_iType, sMsg, sPath);

            if (m_iType == EMESSAGEBOX.MSG)
                listMsgBox.Add(this);
        }

        private void MessageDlg_Load(object sender, EventArgs e)
        {
            if (m_iType == EMESSAGEBOX.ERROR)
            {
                timer1_Tick(sender, e);
                timer1.Interval = 1000;
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_iType == EMESSAGEBOX.ERROR)
            {
                if (m_bFlicker == true)
                {
                    m_bFlicker = false;
                    label_Msg.ForeColor = SystemColors.ControlLight;
                }
                else
                {
                    m_bFlicker = true;
                    label_Msg.ForeColor = Color.Red;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //DontAskAgain = checkBox1.Checked;
        }

        #region Window Setting

        const int WM_MOUSEACTIVATE = 0x21;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x21 && this.CanFocus && !this.Focused)
            {
                this.Focus();
                base.WndProc(ref m);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Y:
                    metroButton_YES.PerformClick();
                    return true;
                case Keys.N:
                    metroButton_NO.PerformClick();
                    return true;
                default:
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion Window Setting

        private void InitUI(EMESSAGEBOX iType, string sMsg, string sPath)
        {
            label_Path.Text = sPath;

            switch (iType)
            {
                case EMESSAGEBOX.EXIT:
                    this.Text = "EXIT";
                    label_Msg.Text = "Do you want to Exit this Program ?";
                    break;
                case EMESSAGEBOX.SAVE:
                    this.Text = "SAVE";
                    label_Msg.Text = string.Format("Do you want to Save [{0}]?", sMsg);
                    break;
                case EMESSAGEBOX.CLOSE:
                    this.Text = "CLOSE";
                    label_Msg.Text = string.Format("Do you want to Close [{0}]?", sMsg);
                    break;
                case EMESSAGEBOX.REMOVE:
                    this.Text = "REMOVE";
                    label_Msg.Text = string.Format("Do you want to Remove this Node ?\r\n[ {0} ]", sMsg);
                    break;
                case EMESSAGEBOX.MSG:
                    this.Text = "MESSAGE";
                    label_Msg.Text = sMsg;
                    metroButton_NO.Visible = false;
                    metroButton_YES.Location = new Point(126, 194);
                    metroButton_YES.Text = "OK";
                    break;
                case EMESSAGEBOX.COMPLETE:
                    this.Text = "COMPLETE";
                    //this.Style = MetroFramework.MetroColorStyle.Green;
                    label_Msg.Text = sMsg;
                    label_Msg.Size = new Size(250, 55);
                    label_Msg.Location = new Point(18, 85);
                    metroButton_NO.Visible = false;
                    metroButton_YES.Location = new Point(75, 160);
                    metroButton_YES.Text = "OK";
                    this.Size = new Size(286, 252);
                    break;
                case EMESSAGEBOX.WARNING:
                    this.Text = "WARNING";
                    //this.Style = MetroFramework.MetroColorStyle.Orange;
                    //metroButton_NO.Visible = false;
                    //metroButton_YES.Location = new Point(126, 194);
                    //metroButton_YES.Text = "OK";
                    label_Msg.Text = sMsg;
                    break;

                case EMESSAGEBOX.QUESTION:
                    this.Text = "QUESTION";
                    //this.Style = MetroFramework.MetroColorStyle.Orange;
                    //metroButton_NO.Visible = false;
                    //metroButton_YES.Location = new Point(126, 194);
                    //metroButton_YES.Text = "OK";
                    //checkBox1.Visible = true;
                    label_Msg.Text = sMsg;
                    break;
                case EMESSAGEBOX.AUTO_STOP:
                    this.Text = "QUESTION";
                    //this.Style = MetroFramework.MetroColorStyle.Orange;
                    label_Msg.Text = sMsg;
                    break;
                case EMESSAGEBOX.ERROR:
                    this.Text = "ERROR";
                    //this.Style = MetroFramework.MetroColorStyle.Yellow;
                    metroButton_NO.Visible = false;
                    metroButton_YES.Location = new Point(126, 194);
                    metroButton_YES.Text = "OK";
                    label_Msg.Text = sMsg;
                    break;
                case EMESSAGEBOX.SKIP_ON:
                    this.Text = "Check";
                    label_Msg.Text = "Do you want to Skip this channel " + sMsg + "?";
                    break;
                case EMESSAGEBOX.SKIP_OFF:
                    this.Text = "Check";
                    label_Msg.Text = "Do you want to make this channel" + sMsg + " Available?";
                    break;
                case EMESSAGEBOX.DISABLE_OFF:
                    this.Text = "Check";
                    label_Msg.Text = "Do you want to USE this PG " + sMsg + "?";
                    break;
                case EMESSAGEBOX.ORIGIN:
                    this.Text = "Origin";
                    label_Msg.Text = sMsg;
                    metroButton_NO.Visible = false;
                    metroButton_YES.Location = new Point(126, 194);
                    metroButton_YES.Text = "Origin Check";
                    break;
            }
        }

        private void button_YES_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            timer1.Stop();
            this.Close();
            //this.Hide();
        }

        private void button_NO_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            timer1.Stop();
            this.Close();
            //this.Hide();
        }

    }

    public enum EMESSAGEBOX
    {
        EXIT ,
        SAVE,
        CLOSE,
        REMOVE ,
        MSG ,
        COMPLETE ,
        WARNING,
        QUESTION,
        AUTO_STOP,
        ERROR,
        SKIP_ON,
        SKIP_OFF,
        DISABLE_OFF,
        ORIGIN,
    }

    public class MESSAGE
    {
        public const string CHECK_XML_FILE = "Check the XML File.";

        public const string ENTER_GROUP_NAME = "Please Enter the Group Name !";
        public const string ENTER_BLOCK_NAME = "Please Enter the Block Name !";
        public const string ENTER_PROP_NAME = "Please Enter the Property Name !";
        public const string ENTER_ITERATION_VALUE = "Please Enter the Interation Value !";
        public const string ENTER_MAXWAIT_VALUE = "Please Enter the Max Wait Value ";

        public const string EDIT_COMPLETE = "Edit Complete !";
        public const string SAVE_COMPLETE = "Save Complete !";

        public const string ERROR_SAVE = "Error occured while Saving.";
    }
}
