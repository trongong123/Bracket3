using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;

namespace TopEng.Vision.Forms
{
    public partial class Form_CogView : Form
    {
        enum ALIGNTYPE
        {
            TILE,
            FULL,
        }

        private Form_CogViewFS fullSizeImageForm;
        public delegate void cbAlignViewTile();
        public event cbAlignViewTile AlignViewTile;
        public delegate void cbAlignViewFull(int Id);
        public event cbAlignViewFull AlignViewFull;
        private ALIGNTYPE alignType = ALIGNTYPE.TILE;
        public int viewId = 0;

        public Form_CogView()
        {
            InitializeComponent();
            fullSizeImageForm = new Form_CogViewFS(this.Display);
        }

        public void EnableWindow(bool enable)
        {
            label_Title.Enabled = enable;
            button_FullScreen.Enabled = enable;

            cogRecordDisplay1.BackColor = enable ? Color.FromArgb(255, 10, 10, 100) : Color.FromArgb(255, 222, 222, 222);
        }

        public string Title
        {
            get { return this.label_Title.Text; }
            set { this.label_Title.Text = value; }
        }

        public Size ViewSize
        {
            get { return cogRecordDisplay1.Size; }
            set { cogRecordDisplay1.Size = value; }
        }

        public ICogImage Image
        {
            get { return cogRecordDisplay1.Image; }
            set { cogRecordDisplay1.Image = value; }
        }

        public CogRecordDisplay Display
        {
            get { return cogRecordDisplay1; }
            set { cogRecordDisplay1 = value; }
        }

        public Label Pos_Left
        {
            get { return label_PosLeft; }
            set { label_PosLeft = value; }
        }

        public Label Pos_Right
        {
            get { return label_PosRight; }
            set { label_PosRight = value; }
        }

        public void AddStaticGraphcis(ICogGraphic graphics, string group) => cogRecordDisplay1.StaticGraphics.Add(graphics, group);
        public void AddInteractiveGraphics(ICogGraphic graphics, string group)
            => cogRecordDisplay1.InteractiveGraphics.Add((ICogGraphicInteractive)graphics, group, false);
        public void RemoveStaticGraphics(string group)
        {
            if (cogRecordDisplay1.StaticGraphics.ZOrderGroups.IndexOf(group) > -1)
                cogRecordDisplay1.StaticGraphics.Remove(group);
        }
        public void ClearImageGraphics()
        {
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.StaticGraphics.Clear();
        }

        public void AddRectangle(string groupname, double LocX, double LocY, double Width, double Height, bool redarw = true)
        {
            if (redarw)
                ClearImageGraphics();

            CogRectangle rect = new CogRectangle();
            rect.Interactive = true;
            rect.SetXYWidthHeight(LocX, LocY, Width, Height);
            rect.GraphicDOFEnable = CogRectangleDOFConstants.All;
            rect.Color = CogColorConstants.Cyan;
            rect.DragColor = CogColorConstants.Cyan;
            rect.SelectedColor = CogColorConstants.Cyan;
            rect.Selected = true;

            cogRecordDisplay1.InteractiveGraphics.Add(rect, groupname, false);
            cogRecordDisplay1.Fit(false);
        }

        public void GetRectangle(string groupname, ref double LocX, ref double LocY, ref double Width, ref double Height, bool clear = true)
        {
            int index = cogRecordDisplay1.InteractiveGraphics.FindItem(groupname, CogDisplayZOrderConstants.Front);

            var graphics = cogRecordDisplay1.InteractiveGraphics[index];
            if (graphics.GetType().Name != "CogRectangle")
                return;

            CogRectangle rect = (CogRectangle)graphics;

            LocX = rect.X;
            LocY = rect.Y;
            Width = rect.Width;
            Height = rect.Height;

            cogRecordDisplay1.InteractiveGraphics.Remove(index);
        }

        public void AddText(string groupname, string text, double LocX, double LocY, CogColorConstants color, string fontname = "Arial", double height = 10)
        {
            CogGraphicLabel label = new CogGraphicLabel();
            label.Font = new Font(new FontFamily(fontname), (float)height, FontStyle.Regular, GraphicsUnit.Point);
            label.Text = text;
            label.Color = color;
            label.X = LocX;
            label.Y = LocY;
            label.Alignment = CogGraphicLabelAlignmentConstants.BaselineCenter;
            label.Interactive = false;
            label.SelectedSpaceName = ".";

            cogRecordDisplay1.InteractiveGraphics.Add(label, groupname, false);
            cogRecordDisplay1.Fit(false);
        }

        private void button_FullScreen_Click(object sender, EventArgs e)
        {
            fullSizeImageForm.TopMost = true;
            fullSizeImageForm.TopLevel = true;
            fullSizeImageForm.ShowForm();
        }
    }
}
