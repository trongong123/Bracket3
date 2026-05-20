using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopEng.Controls
{
    public partial class ButtonEnh : Button, IDisposable
    {
        private Image imageDefault = null;
        private Image imageComplete = null;
        private Image imageHOver = null;
        private Image imageDown = null;
        private Image imageDownHOver = null;
        private Image imageDisable = null;
        private Image imageDisableDown = null;

        private bool imageButton = false;
        private BUTTONTYPE buttonType = BUTTONTYPE.Normal;
        private bool buttonPushed = false;

        // STATUS
        private bool mouseDown = false;
        private bool mouseHOver = false;
        private bool enabled = true;

        public event EventHandler ClickEvent;

        public uint valueEvent = 0; //0이면 MsgEvent , 1이면 MsgEvnet2를 출력한다.
        public string MsgEvent = "On 버튼에 등록된 함수를 실행하시겠습니까?"; //Up, Run, On, 
        public string MsgEvent2 = "Off 버튼에 등록된 함수를 실행하시겠습니까?"; // Down, Stop, Off
        static public bool doorOpen = false;
        #region property image
        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageDefault
        {
            get { return this.imageDefault; }
            set
            {
                this.imageDefault = value;
                this.BackgroundImage = this.imageDefault;
            }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageComplete
        {
            get { return this.imageComplete; }
            set { this.imageComplete = value; }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageHOver
        {
            get { return this.imageHOver; }
            set { this.imageHOver = value; }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageDown
        {
            get { return this.imageDown; }
            set { this.imageDown = value; }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageDownHOver
        {
            get { return this.imageDownHOver; }
            set { this.imageDownHOver = value; }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageDisable
        {
            get { return this.imageDisable; }
            set { this.imageDisable = value; }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public Image ImageDisableDown
        {
            get { return this.imageDisableDown; }
            set { this.imageDisableDown = value; }
        }
        #endregion

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public bool ImageButton
        {
            get { return this.imageButton; }
            set
            {
                this.imageButton = value;
                if (value)
                    this.FlatAppearance.BorderSize = 0;
            }
        }

        public enum BUTTONTYPE
        {
            Normal,
            Push,
            Toggle,
            Display,
            Check,
            UnCheck
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(true)]
        public BUTTONTYPE ButtonType
        {
            get { return this.buttonType; }
            set 
            { 
                this.buttonType = value;
                if (buttonType == BUTTONTYPE.Check || buttonType == BUTTONTYPE.UnCheck)
                    this.Click += ButtonEnh_Click;
                else
                    this.Click -= ButtonEnh_Click;
            }
        }

        [Category("Enhanced Properties")]
        [Description("")]
        [Browsable(false)]
        public bool ButtonPush
        {
            get { return this.buttonPushed; }
            set
            {
                if (!this.Enabled)
                    return;

                this.buttonPushed = value;
                this.BackgroundImage = buttonPushed ? this.ImageDown : this.imageDefault;
                this.Invalidate();
            }
        }

        public ButtonEnh()
        {
            InitializeComponent();

            this.TabStop = false;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            this.Size = new Size(90, 23);

            this.MouseDown += ButtonEnh_MouseDown;
            this.MouseUp += ButtonEnh_MouseUp;
            this.MouseHover += new System.EventHandler(this.ButtonEnh_HOver);
            this.MouseLeave += new System.EventHandler(this.ButtonEnh_Leave);
            this.TextChanged += new System.EventHandler(this.ButtonEnh_TextChanged);

            this.BackgroundImage = this.imageDefault;
            this.BackgroundImageLayout = ImageLayout.Zoom;

            this.SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (base.Enabled != enabled)
            {
                if (imageDisable != null)
                {
                    var size = imageDisable.Size;
                    if (size.Width > 0 && size.Height > 0)
                    {
                        if (this.Enabled)
                            this.BackgroundImage = this.imageDefault;
                        else
                        {
                            if (this.ButtonPush)
                                this.BackgroundImage = this.imageDisableDown;
                            else
                                this.BackgroundImage = this.imageDisable;
                        }
                    }
                }

                enabled = base.Enabled;
            }

            //if (!base.Enabled)
            //{
            //    // TEXT OUT
            //    StringFormat formatText = new StringFormat(StringFormatFlags.NoClip);
            //    formatText.LineAlignment = StringAlignment.Center;
            //    formatText.Alignment = StringAlignment.Center;
            //    e.Graphics.DrawString(base.Text, base.Font, new SolidBrush(this.ForeColor),
            //    new RectangleF(0F, 0F, base.Width, base.Height), formatText);
            //    formatText.Dispose();
            //}

            //if (this.Focused)
            //{
            //    var soliddBrush = new SolidBrush(base.BackColor);
            //    var pen = new Pen(Color.Transparent, 1);
            //    // Drawing the button yoursel. The background is gray
            //    e.Graphics.FillRectangle(soliddBrush, e.ClipRectangle);
            //    // Draw the line around the button
            //    e.Graphics.DrawRectangle(pen, 0, 0, base.Width - 1, base.Height - 1);
            //    //}
            //    soliddBrush.Dispose();
            //    pen.Dispose();
            //}
        }

        void ButtonEnh_Click(object sender, EventArgs e)
        {
            if (buttonType == BUTTONTYPE.Check)
            {
                string msg = "Test";
                if (valueEvent == 0)
                    msg = MsgEvent;
                else
                    msg = MsgEvent2;

                if (doorOpen)
                {
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                       string.Format("Please Close the door"));
                    msgBox.ShowDialog();
                    return;
                }

                Dlg_MessageBox formMove = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(msg));
                if (formMove.ShowDialog() == DialogResult.Yes)
                {
                    //등록한 함수 실행
                    if (ClickEvent != null)
                    {
                        ClickEvent(this, e);
                    }
                }
            }
            else if (buttonType == BUTTONTYPE.UnCheck)
            {
                if (doorOpen)
                {
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                       string.Format("Please Close the door"));
                    msgBox.ShowDialog();
                    return;
                }

                //등록한 함수 실행
                if (ClickEvent != null)
                {
                    ClickEvent(this, e);
                }
            }
            else
            {
                Dlg_MessageBox formError = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("Change Button Type to Check"));
                formError.ShowDialog();
            }
        }

        void ButtonEnh_Focused(object sender, EventArgs e)
        {

        }

        void ButtonEnh_TextChanged(object sender, EventArgs e)
        {
            if (imageButton)
                this.Text = "";
        }

        void ButtonEnh_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.Enabled || BUTTONTYPE.Display == buttonType)
                return;
            if (BUTTONTYPE.Toggle == buttonType && buttonPushed)
                return;

            if (this.imageDown != null)
                this.BackgroundImage = this.imageDown;
            mouseDown = true;           
        }

        void ButtonEnh_MouseUp(object sender, MouseEventArgs e)
        {
            if (!this.Enabled || BUTTONTYPE.Display == buttonType)
                return;
            if (BUTTONTYPE.Toggle == buttonType && buttonPushed)
                return;

            if (BUTTONTYPE.Push == buttonType && mouseDown)
                buttonPushed = !buttonPushed;
            if (BUTTONTYPE.Toggle == buttonType && mouseDown)
                buttonPushed = !buttonPushed;

            if (this.imageDefault != null && !buttonPushed)
                this.BackgroundImage = imageDefault;
            if (this.ImageDown != null && buttonPushed)
                this.BackgroundImage = ImageDown;

            mouseDown = false;
            if (!this.Enabled || BUTTONTYPE.Check == buttonType)
                return;
            ButtonEnh_HOver(sender, e);
        }

        void ButtonEnh_HOver(object sender, EventArgs e)
        {
            mouseHOver = true;

            if (!this.Enabled || BUTTONTYPE.Display == buttonType)
                return;

            if (this.imageHOver != null && !buttonPushed)
                this.BackgroundImage = imageHOver;
            if (this.imageDownHOver != null && buttonPushed)
                this.BackgroundImage = imageDownHOver;
        }

        void ButtonEnh_Leave(object sender, EventArgs e)
        {
            mouseHOver = false;

            if (!this.Enabled || BUTTONTYPE.Display == buttonType)
                return;

            if (!buttonPushed)
                this.BackgroundImage = imageDefault;
            if (this.ImageDown != null && buttonPushed)
                this.BackgroundImage = ImageDown;
        }
    }
}
