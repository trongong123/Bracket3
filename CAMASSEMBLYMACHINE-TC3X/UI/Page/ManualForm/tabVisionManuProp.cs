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
using System.Security;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using System.Threading;
using TopEng.Vision;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Vision.Forms;
using TopEng.Controls;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabVisionManuProp : Form, IForm
    {
        private int InspNo = -1;
        private int targetCam = 0;
        List<Label> label_Pos = new List<Label>();

        public tabVisionManuProp(int Id)
        {
            InitializeComponent();
            InspNo = Id;

            label_Pos.Add(disp_alignPos1);
            label_Pos.Add(disp_alignPos2);
        }

        private void tabVisionManuProp_Load(object sender, EventArgs e)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE && SystemDefine.NO_VISION_KEY) return;
            cogRecordDisplay1.AutoFit = true;

            if (SystemDefine.manualGrab)
                Grab();
            else
            {
                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

                if (targetCam < Vision.cogTool.Count)
                    cogRecordDisplay1.Image = Vision.cogTool[targetCam].m_cogImage;
            }

            initDataLoad();
        }

        public void initDataLoad()
        {
            targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

            LIGHTDATA lightdata = Vision.inspection.recipeData[targetCam].inspInfo.lightSet;

            textBox_RLv.Text = lightdata.a.ToString();// Vision.Camera.cameraInfo[targetCam].lightSet.a.ToString();
            textBox_GLv.Text = lightdata.b.ToString();//Vision.Camera.cameraInfo[targetCam].lightSet.b.ToString();
            textBox_BLv.Text = lightdata.c.ToString();//Vision.Camera.cameraInfo[targetCam].lightSet.c.ToString();
            textBox_BLv2.Text = lightdata.d.ToString();//Vision.Camera.cameraInfo[targetCam].lightSet.d.ToString();
            numericUpDown1.Value =(decimal)lightdata.Exposure;
        }

        public void StartTimer(bool enable)
        {
            if (enable)
                timer1.Start();
            else
                timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Vision.Camera.IsCameraConnected(targetCam))
                ledCamConnection.Image = global::CAMASSEMBLYMACHINE.Properties.Resources.LED_GREEN;
            else
                ledCamConnection.Image = global::CAMASSEMBLYMACHINE.Properties.Resources.LED_GRAY;

            if (Vision.Camera.IsLightConnected(targetCam))
                ledLightConnection.Image = global::CAMASSEMBLYMACHINE.Properties.Resources.LED_GREEN;
            else
                ledLightConnection.Image = global::CAMASSEMBLYMACHINE.Properties.Resources.LED_GRAY;

            if ((SystemDefine.CAMERA)InspNo == SystemDefine.CAMERA.TRAY)
            {
                UpdateProcTrayData();
            }
            else if ((SystemDefine.CAMERA)InspNo == SystemDefine.CAMERA.UNDER)
            {
                UpdateProcCamData();
            }
            else if ((SystemDefine.CAMERA)InspNo == SystemDefine.CAMERA.JIG)
            {
                UpdateProcJigData();
            }
            else if ((SystemDefine.CAMERA)InspNo == SystemDefine.CAMERA.PICKER)
            {
                UpdateProcPickerData();
            }
            else if ((SystemDefine.CAMERA)InspNo == SystemDefine.CAMERA.JIG2)
            {
                UpdateProcJigData();
            }
        }

        private void Grab()
        {
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.Record = null;

            targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

            if (targetCam < Vision.cogTool.Count)
            {
                Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
                Vision.Grab(targetCam);
            }

            Vision.GrabCompleted(targetCam);
        }

        private void button_Test_Click(object sender, EventArgs e)
        {
            if (!Vision.Camera.IsCameraConnected(targetCam))
            {
                MessageBox.Show("Camera is disconnected! \nPlease check camera connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Vision.Camera.IsLightConnected(targetCam))
            {
                MessageBox.Show("Light is disconnected! \nPlease check light connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.a;
            level.b = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.b;
            level.c = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.c;
            level.d = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            LogUtil.Instance.Log(LOG_TYPE.UI, $"Test Button Click", CONTENT_TYPE.INFO);
            bool saveTrainImage = Machine.param.option["USE_IMAGE_GATHERING_FOR_EL"] == 1 ? true : false;
            Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
            Vision.RunGrab(InspNo, false);
            Vision.Run(InspNo, true, true, saveTrainImage);
        }

        private void button_Grab_Click(object sender, EventArgs e)
        {
            if (!Vision.Camera.IsCameraConnected(targetCam))
            {
                MessageBox.Show("Camera is disconnected! \nPlease check camera connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Vision.Camera.IsLightConnected(targetCam))
            {
                MessageBox.Show("Light is disconnected! \nPlease check light connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.a;
            level.b = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.b;
            level.c = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.c;
            level.d = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            LogUtil.Instance.Log(LOG_TYPE.UI, $"Grab Button Click", CONTENT_TYPE.INFO);
            Grab();
            button_Live.ButtonPush = false;
        }

        private void button_Live_Click(object sender, EventArgs e)
        {
            if (!Vision.Camera.IsCameraConnected(targetCam))
            {
                MessageBox.Show("Camera is disconnected! \nPlease check camera connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Vision.Camera.IsLightConnected(targetCam))
            {
                MessageBox.Show("Light is disconnected! \nPlease check light connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.a;
            level.b = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.b;
            level.c = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.c;
            level.d = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            LogUtil.Instance.Log(LOG_TYPE.UI, $"Live Button Click", CONTENT_TYPE.INFO);
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.Record = null;

            if (targetCam < Vision.cogTool.Count)
            {
                Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
                Vision.LiveStart(targetCam);
                button_Live.ButtonPush = true;
            }
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                FileName = "Select a Image file",
                Filter = "Image files (*.bmp)|*.bmp",
                Title = "Open Image file"
            };

            if (DialogResult.OK == dlg.ShowDialog())
            {
                try
                {
                    var filepath = dlg.FileName;

                    CogImageFile file = new CogImageFile();
                    file.Open(filepath, CogImageFileModeConstants.Read);
                    var cogImage = file[0];
                    file.Close();

                    targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                    cogRecordDisplay1.Image = CogImageConvert.GetIntensityImage(cogImage, 0, 0, cogImage.Width, cogImage.Height);
                    Vision.cogTool[targetCam].m_cogImageNoDevice = cogRecordDisplay1.Image;
                    Vision.cogTool[targetCam].m_cogImage = cogRecordDisplay1.Image;

                    cogRecordDisplay1.InteractiveGraphics.Clear();
                    cogRecordDisplay1.StaticGraphics.Clear();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            string name = Vision.inspection.InspInfo[InspNo].Name;
            Vision.cogTool[targetCam].Capture(name, CaptureUtil.CAPTURETYPE.USER, cogRecordDisplay1.Image);
        }

        private void textBox_RLv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_RLv.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_RLv.Text = level.ToString();
            }
            catch
            {
                textBox_RLv.Text = "0";
            }
        }

        private void textBox_GLv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_GLv.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_GLv.Text = level.ToString();
            }
            catch
            {
                textBox_GLv.Text = "0";
            }
        }

        private void textBox_BLv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_BLv.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_BLv.Text = level.ToString();
            }
            catch
            {
                textBox_BLv.Text = "0";
            }
        }

        private void textBox_BLv2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_BLv2.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_BLv2.Text = level.ToString();
            }
            catch
            {
                textBox_BLv2.Text = "0";
            }
        }

        private void textBox_RLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_RLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "RED Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_RLv.Text = value[0];
        }

        private void textBox_GLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_GLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "GREEN Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_GLv.Text = value[0];
        }

        private void textBox_BLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_BLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "BLUE Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_BLv.Text = value[0];
        }

        private void textBox_BLv2_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_BLv2.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "BLUE Light 2(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_BLv2.Text = value[0];
        }

        private void button_LightOn_Click(object sender, EventArgs e)
        {
            if (!Vision.Camera.IsLightConnected(targetCam))
            {
                MessageBox.Show("Light is disconnected! \nPlease check light connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogUtil.Instance.Log(LOG_TYPE.UI, $"targetCam:{Vision.inspection.InspInfo[InspNo].targetCam} / Light On Click", CONTENT_TYPE.INFO);
            try
            {
                targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                //int r = Convert.ToInt16(textBox_RLv.Text);
                //int g = Convert.ToInt16(textBox_GLv.Text);
                //int b = Convert.ToInt16(textBox_BLv.Text);

                var level = new LIGHTDATA();
                level.a = Convert.ToInt16(textBox_RLv.Text);
                level.b = Convert.ToInt16(textBox_GLv.Text);
                level.c = Convert.ToInt16(textBox_BLv.Text);
                level.d = Convert.ToInt16(textBox_BLv2.Text);

                Vision.Camera.LightSet(targetCam, level);
                Vision.Camera.LightOn(targetCam, true);
            }
            catch
            {

            }
        }

        private void button_LightOff_Click(object sender, EventArgs e)
        {
            if (!Vision.Camera.IsLightConnected(targetCam))
            {
                MessageBox.Show("Light is disconnected! \nPlease check light connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogUtil.Instance.Log(LOG_TYPE.UI, $"targetCam:{Vision.inspection.InspInfo[InspNo].targetCam} / Light Off Click", CONTENT_TYPE.INFO);
            try
            {
                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                Vision.Camera.LightOn(targetCam, false);
            }
            catch
            {

            }
        }

        private void Ex_Save_Click(object sender, EventArgs e)
        {
            if (!Vision.Camera.IsCameraConnected(targetCam))
            {
                MessageBox.Show("Camera is disconnected! \nPlease check camera connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int oRlV = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.a;
            //double oExp = Vision.Camera.cameraInfo[targetCam].Exposure;
            double oExp = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.Exposure;

            int nRLv = 0; double nExp = 0;

            bool Check1 = int.TryParse(textBox_RLv.Text, out nRLv);
            if (!Check1)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                    "Light Value is Only Integer Number");
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }
            nExp = Convert.ToDouble(numericUpDown1.Value);

            string typeVisionStr = ((SystemDefine.CAMERA)InspNo).ToString();
            Vision.inspection.recipeData[InspNo].inspInfo.lightSet.a = nRLv;
            Vision.inspection.recipeData[InspNo].inspInfo.lightSet.b = nRLv;
            Vision.inspection.recipeData[InspNo].inspInfo.lightSet.c = nRLv;
            Vision.inspection.recipeData[InspNo].inspInfo.lightSet.d = nRLv;

            Vision.inspection.recipeData[InspNo].inspInfo.lightSet.Exposure = nExp;


            LogUtil.Instance.Log(LOG_TYPE.UI, $"targetCam:{targetCam} / Exposure Save Button Click", CONTENT_TYPE.INFO);
            string logText = $"[{typeVisionStr} Light Setting W data has changed. [{oRlV} → {nRLv}]";
            LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
            //string logText2 = $"[{typeVisionStr} Light Expsoure data has changed. [{oExp} → {nExp}]";
            //LogUtil.Instance.Log(LOG_TYPE.DATA, logText2, CONTENT_TYPE.INFO);

            targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

            //Camera Info Update
            Vision.Camera.Exposure(targetCam, Vision.inspection.recipeData[InspNo].inspInfo.lightSet.Exposure);
            //Camera write
            
            Vision.Camera.SetExposure(targetCam, Convert.ToDouble(Vision.inspection.recipeData[InspNo].inspInfo.lightSet.Exposure));

            Vision.inspection.UpdateModel();
            Vision.Camera.Write();

            if (Vision.Camera.GetExposure(targetCam) == Convert.ToDouble(Vision.inspection.recipeData[InspNo].inspInfo.lightSet.Exposure))
            {
                string logText2 = $"[{typeVisionStr} Light Expsoure data has changed. [{oExp} → {nExp}]";
                LogUtil.Instance.Log(LOG_TYPE.DATA, logText2, CONTENT_TYPE.INFO);
            }
            else
            {
                string logText3 = $"[{typeVisionStr} Light Expsoure data wrong source";
                LogUtil.Instance.Log(LOG_TYPE.DATA, logText3, CONTENT_TYPE.INFO);
            }
            Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                    "Vision Setting is Successfully Saved");
            form.TopLevel = true;
            form.TopMost = true;
            form.ShowDialog();
        }

        private void UpdateProcCamData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_Pos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_Pos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void UpdateProcTrayData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_Pos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_Pos[i].Text = $"( {xpos} , {ypos} , {angle} )";
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
                    label_Pos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_Pos[i].Text = $"{xpos}, {ypos}, {angle}";
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
                    label_Pos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_Pos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }  
        
        private void btnCamConnect_Click(object sender, EventArgs e)
        {
            Vision.Camera.CameraManualOpen(targetCam);
        }

        private void btnCamDisconnect_Click(object sender, EventArgs e)
        {
            Vision.Camera.CameraManualClose(targetCam);
        }

        private void btnLightConnect_Click(object sender, EventArgs e)
        {
            Vision.Camera.LightManualOpen(targetCam);
        }

        private void btnLightDisconnect_Click(object sender, EventArgs e)
        {
            Vision.Camera.LightManualClose(targetCam);
        }
    }
}
