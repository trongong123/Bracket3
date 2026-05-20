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
using Cognex.VisionPro;
using TopEng.Vision.Forms;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabVisionAuto : Form, IForm
    {
        public List<Form_CogView> cogDisplay = new List<Form_CogView>();
        List<Panel> paneDisplay = new List<Panel>();
        public int highlightDisp = -1;
        List<Label> label_tray_alignPos = new List<Label>();
        List<Label> label_cam_alignPos = new List<Label>();
        List<Label> label_jig_alignPos = new List<Label>();
        List<Label> label_picker_alignPos = new List<Label>();
        bool updatingNow = false;

        public tabVisionAuto()
        {
            InitializeComponent();

            paneDisplay.Add(panel1);
            paneDisplay.Add(panel2);
            paneDisplay.Add(panel3);
            paneDisplay.Add(panel4);

            ChangeCameraCount();
        }

        private void tabVisionAuto_Load(object sender, EventArgs e)
        {
        }

        public void ConnectDisplay()
        {
            for (int i = 0; i < Vision.cogTool.Count; i++)
            {
                Vision.cogTool[i].m_cogRecordDisplay = cogDisplay[i].Display;
            }
        }

        public CogRecordDisplay GetDisplay(int Id)
        {
            if (Id < cogDisplay.Count)
                return cogDisplay[Id].Display;
            return null;
        }

        private void ChangeCameraCount()
        {
            for (int i = 0; i < 5; i++)
            {
                Form_CogView cog = new Form_CogView();
                cog.viewId = i;
                cog.TopLevel = false;
                cog.Parent = paneDisplay[i];
                cog.Dock = DockStyle.Fill;
                cog.Title = Vision.Camera.Name(i);
                cog.Show();
                cog.AlignViewTile += new Form_CogView.cbAlignViewTile(AlignViewTile);
                cog.AlignViewFull += new Form_CogView.cbAlignViewFull(AlignViewFull);
                cogDisplay.Add(cog);

                if (i < Vision.cogTool.Count)
                    cog.Visible = true;
                else
                    cog.Visible = false;

                if (cog.viewId == 0) 
                {
                    label_tray_alignPos.Add(cog.Pos_Left);
                    label_tray_alignPos.Add(cog.Pos_Right);
                }
                else if (cog.viewId == 1)
                {
                    label_cam_alignPos.Add(cog.Pos_Left);
                    label_cam_alignPos.Add(cog.Pos_Right);
                }
                else if (cog.viewId == 2)
                {
                    label_jig_alignPos.Add(cog.Pos_Left);
                    label_jig_alignPos.Add(cog.Pos_Right);
                }
                else if (cog.viewId == 3)
                {
                    label_picker_alignPos.Add(cog.Pos_Left);
                    label_picker_alignPos.Add(cog.Pos_Right);
                }
            }
        }

        public void AlignViewTile()
        {
            for (int i = 0; i < cogDisplay.Count; i++)
                cogDisplay[i].Parent = paneDisplay[i];

            for (int i = 0; i < cogDisplay.Count; i++)
                paneDisplay[i].Visible = true;
            tableLayoutPanel1.Visible = true;
        }

        public void AlignViewFull(int Id)
        {
            if (Id >= Vision.cogTool.Count)
                return;

            for (int i = 0; i < cogDisplay.Count; i++)
            {
                cogDisplay[i].Parent = paneDisplay[i];
                paneDisplay[i].Visible = false;
            }

            cogDisplay[Id].Parent = this;
            tableLayoutPanel1.Visible = false;
        }

        private void Form_PaneMain_Resize(object sender, EventArgs e)
        {
            tableLayoutPanel1.Size = Size;
        }

        public void AddResultText(int Id, string text, CogColorConstants color)
        {
            //cogDisplay[Id].AddText(text, color);
        }
        public void StartTimer(bool enable) => timer1.Enabled = enable;

        //실시간 업데이트가 아니라 찍혔을때만 업데이트를 하는걸로?
        private void UpdateProcTrayData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_tray_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_tray_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }

            //if (proc.updateSearchData)
            //{
            //    trayDisp.focusIndex.Clear();
            //    if (proc.alignSucs[0] >= 0) trayDisp.focusIndex.Add(proc.alignSucs[0], 0);
            //    if (proc.alignSucs[1] >= 0) trayDisp.focusIndex.Add(proc.alignSucs[1], 0);
            //    trayDisp.Invalidate();
            //    proc.updateSearchData = false;
            //}
        }

        private void UpdateProcCamData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_cam_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_cam_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void UpdateProcJigData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_jig_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_jig_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void UpdateProcPickerData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_picker_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_picker_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (updatingNow)
                return;
            updatingNow = true;

            UpdateProcTrayData();
            UpdateProcCamData();
            UpdateProcJigData();
            UpdateProcPickerData();

            updatingNow = false;
        }
    }
}
