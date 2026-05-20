using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NV_UI;
using System.IO;
using System.Text.Json;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using System.Text.RegularExpressions;
using TopEng.Controls;
using CAMASSEMBLYMACHINE.UI.SubForm;
using System.Threading;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabModelData : Form
    {
        #region VARIABLES

        public List<string> listModel = new List<string>();

        public enum CONFIG_TYPE
        {
            LOGISTIC_BOOLEAN,
            ADDITIONAL_MOTION_PARAM,
            AXIS_PARAM,
            AXIS_POS,
            RESULT,
            ERROR,
            IP,
            TIMEOUT,
            PRODUCT_LOGISTIC_INFO,
            PRODUCT_INSPECTION_INFO,
            MODEL,
            DELAY_CYLINDER,
            DELAY_VACUUM,
            DELAY_MOTION,
            VISION_CALIBRATION,
        }

        public enum BTN_TAB_SELECT
        {
            SELECT,
            CREATE,
            COPY,
            DELETE,
            RENAME
        }

        public enum MODEL_DATA_GRID_FIELD
        {
            NUMBER,
            MODEL_NAME,
        }

        private string strSelectedModelName;
        private int iSelectedRow;
        #endregion

        #region INITIALIZE
        public tabModelData()
        {
            InitializeComponent();

            nV_Button_PB_NS_Select.ClickEvent += ModelClickEventHandler;
            //nV_Button_PB_NS_Create.ClickEvent += ModelClickEventHandler;
            nV_Button_PB_NS_Copy.ClickEvent += ModelClickEventHandler;
            nV_Button_PB_NS_Delete.ClickEvent += ModelClickEventHandler;
            nV_Button_PB_NS_ReName.ClickEvent += ModelClickEventHandler;

            UpdateCurrentModelName();
            Subscribe(CONFIG_TYPE.MODEL, _ => UpdateModelListData());
                        
            UpdateModelListData();
            InitDataGridView();
        }

        private void InitDataGridView()
        {
            this.dataGridView_ModelList.ScrollBars = ScrollBars.Both;
            this.dataGridView_ModelList.Columns[(int)MODEL_DATA_GRID_FIELD.NUMBER].DefaultCellStyle.SelectionBackColor = dataGridView_ModelList.DefaultCellStyle.BackColor;
            this.dataGridView_ModelList.Columns[(int)MODEL_DATA_GRID_FIELD.NUMBER].ReadOnly = true;

            int iTotalWidth = this.dataGridView_ModelList.Width;
            this.dataGridView_ModelList.Columns[(int)MODEL_DATA_GRID_FIELD.NUMBER].Width = (int)(iTotalWidth * 0.2);     // 20%
            this.dataGridView_ModelList.Columns[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Width = (int)(iTotalWidth * 0.8); // 80%

            int iTotalHeight = this.dataGridView_ModelList.Height;
            dataGridView_ModelList.RowTemplate.Height = iTotalHeight / 10;
            dataGridView_ModelList.AllowUserToResizeRows = false;
            dataGridView_ModelList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView_ModelList.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView_ModelList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        #endregion

        #region BUTTON_CLICK_FUNCTION
        private void ModelClickEventHandler(object sender, EventArgs e)
        {
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out BTN_TAB_SELECT selectBtn))
            {
                NV_Button_TG_NS btn = sender as NV_Button_TG_NS;
                switch (selectBtn)
                {
                    case BTN_TAB_SELECT.SELECT:

                        if (dataGridView_ModelList.SelectedCells.Count <= 0)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Please select model for selecting"));
                            msgBox.ShowDialog();
                            break;
                        }
                        if (dataGridView_ModelList.SelectedCells[0].ColumnIndex != 1)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Please select only 1 model"));
                            msgBox.ShowDialog();
                            return;
                        }

                        Dlg_MessageBox msgBoxW = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                       string.Format("Do you want to change model?"));
                        if (msgBoxW.ShowDialog() == DialogResult.Yes)
                        {
                            if (dataGridView_ModelList.SelectedCells[0].Value == null)
                            {
                                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Empty value, please insert valid value"));
                                msgBox.ShowDialog();
                                return;
                            }
                            SelectModel(dataGridView_ModelList.SelectedCells[0].Value.ToString());
                        }
                        break;

                    case BTN_TAB_SELECT.CREATE:
                        this.CreateNewModel();
                        break;
                    case BTN_TAB_SELECT.COPY:
                        if (dataGridView_ModelList.SelectedCells.Count <= 0)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                         string.Format("Please select model to copy"));
                            msgBox.ShowDialog();
                            break;
                        }

                        if (dataGridView_ModelList.SelectedCells[0].ColumnIndex != 1)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                       string.Format("Please select only 1 model"));
                            msgBox.ShowDialog();
                            return;
                        }

                        Dlg_MessageBox msgBoxW2 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                       string.Format("Do you want to copy this model as source?"));
                        if (msgBoxW2.ShowDialog() == DialogResult.Yes)
                        {
                            this.CopyModel();
                        }
                        break;
                    case BTN_TAB_SELECT.DELETE:
                        if (dataGridView_ModelList.SelectedCells.Count <= 0)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                    string.Format("Please select model for deleting"));
                            msgBox.ShowDialog();
                            break;
                        }

                        if (dataGridView_ModelList.SelectedCells[0].ColumnIndex != 1)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                  string.Format("Please select only 1 model"));
                            msgBox.ShowDialog(); 
                            return;
                        }

                        if (dataGridView_ModelList.SelectedCells[0].Value.ToString() == SystemDefine.modelname)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, 
                                string.Format("This model is running, please select other model!"));
                            msgBox.ShowDialog();
                            return;
                        }

                        if (listModel.Count < 2)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                  string.Format("Please keep at lease one model!"));
                            msgBox.ShowDialog();
                            return;
                        }
                        Dlg_MessageBox msgBoxW3 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                       string.Format("Do you want to delete this model?"));
                        if (msgBoxW3.ShowDialog() == DialogResult.Yes)
                        {
                            this.DeleteModel();
                        }
                        break;
                    case BTN_TAB_SELECT.RENAME:
                        if (dataGridView_ModelList.SelectedCells.Count <= 0)
                        {
                            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                    string.Format("Please select model for deleting"));
                            msgBox.ShowDialog();
                            break;
                        }
                        dataGridView_ModelList.BeginEdit(false);
                        System.Diagnostics.Process.Start("osk.exe");
                        //System.Diagnostics.Process.Start(@"C:\Program Files\Common Files\microsoft shared\ink\TabTip.Exe");
                        break; ;
                }
            }
        }
        #endregion

        #region MAIN_FUNCTION
        private void DeleteModel()
        {
            try
            {
                string strSelectedModel = dataGridView_ModelList.SelectedCells[0].Value.ToString();

                if (listModel.Contains(strSelectedModel))
                {
                    string dir = SystemDefine.recipePath + "\\" + strSelectedModel;
                    DeleteDirectory(dir);
                    listModel.Remove(strSelectedModel);

                    UpdateModelListData();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateNewModel()
        {
            using (SubForm_InputText inputTextForm = new SubForm_InputText("CREATE MODEL"))
            {
                inputTextForm.ShowDialog();
                string strNewModelName = string.Empty;
                if (inputTextForm.DialogResult == DialogResult.OK) strNewModelName = inputTextForm.GetInputText();
                else return;
                if (listModel.Contains(strNewModelName))
                {
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                     string.Format("Already exist model"));
                    msgBox.ShowDialog();
                    return;
                }

                string strNewDIr = SystemDefine.recipePath + "\\" + strNewModelName;                
                CreateDirectory(strNewDIr);
                dataGridView_ModelList.ClearSelection();
                foreach (DataGridViewRow row in dataGridView_ModelList.Rows)
                {
                    if (row.Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value != null && row.Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value.ToString().Equals(strNewModelName, StringComparison.OrdinalIgnoreCase))
                    {
                        row.Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Selected = true;
                        dataGridView_ModelList.FirstDisplayedScrollingRowIndex = row.Index;
                    }
                }
                CreateDummyValueAxisPos(strNewModelName);
                //JsonHelper.Instance.CreateDummyVisionCalValue();
                //this.SelectModel(SystemDefine.modelname);

                UpdateModelListData();
            }
        }

        private void CopyModel()
        {
            using (SubForm_InputText inputTextForm = new SubForm_InputText("CREATE MODEL"))
            {
                inputTextForm.ShowDialog();
                string strNewModelName = string.Empty;
                if (inputTextForm.DialogResult == DialogResult.OK) strNewModelName = inputTextForm.GetInputText();
                else return;
                if (listModel.Contains(strNewModelName))
                {
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Already exist model !"));
                    msgBox.ShowDialog();
                    return;
                }

                string strSelectedModel = dataGridView_ModelList.SelectedCells[0].Value.ToString();
                string strSourceDirectory = SystemDefine.recipePath + "\\" + strSelectedModel;
                string strNewDirectory = SystemDefine.recipePath + "\\" + strNewModelName;
                CopyDirectory(strSourceDirectory, strNewDirectory);

                UpdateModelListData();
            }
        }

        private void SelectModel(string strModelName)
        {
            SubForm_Loading.ShowLoadingForm();

            string strModelNameBef = SystemDefine.modelname;

            try
            {
                SubForm_Loading.ReportProgress(30, $"Change model to {strModelName}");
                bool isChanged = Machine.ChangeModel(strModelName);
                SubForm_Loading.ReportProgress(70, $"Change Vision model to {strModelName}");
                bool isChangedVision = Vision.inspection.ChangeModel(strModelName);
                if (isChanged && isChangedVision)
                {
                    SubForm_Loading.ReportProgress(100, "Model Change Success !");
                    Thread.Sleep(1000);

                    SystemDefine.modelname = strModelName;
                    UpdateCurrentModelName();
                    Program.UpdateRegistry();

                    SubForm_Loading.CloseLoadingForm(this);
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                            string.Format("Model is Successfully Changed !"));
                    msgBox.ShowDialog();
                }
                else
                {
                    throw new Exception();
                }

                LogUtil.Instance.Log(LOG_TYPE.DATA, $"Model has been changed from {strModelNameBef} to {strModelName}", CONTENT_TYPE.INFO);

            }
            catch
            {
                SubForm_Loading.ReportProgress(80, $"Rollback model to {strModelNameBef}", true);
                Machine.ChangeModel(strModelNameBef);
                SubForm_Loading.ReportProgress(90, $"Rollback Vision model to {strModelNameBef}", true);
                Vision.inspection.ChangeModel(strModelNameBef);
                SubForm_Loading.ReportProgress(100, "Model Change is Failed !", true);

                SubForm.SubForm_Loading.CloseLoadingForm(this);
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                       string.Format("Model Change is Failed !"));
                msgBox.ShowDialog();
            }
        }

        private void RenameModel()
        {
            try
            {
                string strNewModelName = dataGridView_ModelList.Rows[this.iSelectedRow].Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value.ToString();
                if (this.strSelectedModelName == strNewModelName) return;

                string strCurrentDirectory = SystemDefine.recipePath + "\\" + strSelectedModelName;
                string strNewDirectory = SystemDefine.recipePath + "\\" + strNewModelName;

                string strRunningModelName = SystemDefine.modelname;

                Dlg_MessageBox msgBoxW = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                       string.Format("Do you want to change model Name?"));
                if (msgBoxW.ShowDialog() == DialogResult.No)
                {
                    dataGridView_ModelList.Rows[this.iSelectedRow].Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value = strSelectedModelName;
                    return;
                }

                if (!CheckDirectoryExist(strCurrentDirectory))
                {
                    dataGridView_ModelList.Rows[this.iSelectedRow].Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value = strSelectedModelName;
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                     string.Format("Folder is not exist"));
                    msgBox.ShowDialog();
                    return;
                }

                if (listModel.Contains(strNewModelName))
                {
                    dataGridView_ModelList.Rows[this.iSelectedRow].Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value = strSelectedModelName;
                    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                     string.Format("Changed model already exis"));
                    msgBox.ShowDialog();
                    return;
                }
                if (strSelectedModelName == strRunningModelName)
                {
                    SystemDefine.modelname = strNewModelName;
                    UpdateCurrentModelName();
                    Program.UpdateRegistry();
                }
                RenameDirectory(strCurrentDirectory, strNewDirectory);

                UpdateModelListData();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region UI_RELATED_FUNCTION
        private void UpdateCurrentModelName()
        {
            this.labelCurrentModel.Text = SystemDefine.modelname;
        }

        private void UpdateModelListData()
        {
            string[] headername = { "No", "Model Name"};
            string[] headertype = { "TEXT", "TEXT"};

            dataGridView_ModelList.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                    dataGridView_ModelList.Columns.Add(headername[i], headername[i]);
                if (headertype[i] == "BUTTON")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.Name = headername[i];
                    dataGridView_ModelList.Columns.Add(newColumn);
                }

                dataGridView_ModelList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView_ModelList.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;              
            }

            dataGridView_ModelList.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;

            string[] folders = Directory.GetDirectories(SystemDefine.recipePath);
            listModel.Clear();
            for (int i = 0; i < folders.Length; ++i)
            {
                string[] strArr = folders[i].Split('\\');

                listModel.Add(strArr[strArr.Length - 1]);
            }

            for (int i = 0; i < listModel.Count; i++)
            {
                dataGridView_ModelList.Rows.Add();
                dataGridView_ModelList.Rows[i].Height = 30;
                dataGridView_ModelList.Rows[i].Cells[(int)MODEL_DATA_GRID_FIELD.NUMBER].Value = (i + 1);
                dataGridView_ModelList.Rows[i].Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value = listModel[i];
            }

            int iTotalWidth = this.dataGridView_ModelList.Width;
            dataGridView_ModelList.Columns[(int)MODEL_DATA_GRID_FIELD.NUMBER].Width = (int)(iTotalWidth * 0.2);     // 20%
            dataGridView_ModelList.Columns[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Width = (int)(iTotalWidth * 0.8); // 80%

            dataGridView_ModelList.CurrentCell = dataGridView_ModelList.Rows[0].Cells[1];
            dataGridView_ModelList.ClearSelection();
            dataGridView_ModelList.Refresh();
        }

        private void dataGridView_ModelList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView_ModelList.Rows)
            {
                if (row.Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value != null 
                    && row.Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Value.ToString().
                    Equals(SystemDefine.modelname, StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Selected = true;
                    dataGridView_ModelList.FirstDisplayedScrollingRowIndex = row.Index;
                }
            }

            dataGridView_ModelList.ClearSelection();
            dataGridView_ModelList.Refresh();
        }

        private void dataGridView_ModelList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == (int)MODEL_DATA_GRID_FIELD.NUMBER && e.RowIndex >= 0)
            {
                dataGridView_ModelList.Rows[e.RowIndex].Cells[(int)MODEL_DATA_GRID_FIELD.MODEL_NAME].Selected = true;
                dataGridView_ModelList.Rows[e.RowIndex].Cells[(int)MODEL_DATA_GRID_FIELD.NUMBER].Selected = false;
            }
        }
        #endregion

        #region SUB_FUNCTION_FOR_RENAME
        private void dataGridView_ModelList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (dataGridView_ModelList.SelectedCells.Count > 0 && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    this.iSelectedRow = e.RowIndex;
                    this.strSelectedModelName = dataGridView_ModelList.SelectedCells[0].Value.ToString();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void dataGridView_ModelList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            this.RenameModel();
        }
        #endregion

        #region //Rear의 DataManager를 가져옴
        private readonly Dictionary<CONFIG_TYPE, List<Action<object>>> subscribers = new Dictionary<CONFIG_TYPE, List<Action<object>>>();

        public void Subscribe(CONFIG_TYPE configType, Action<object> handler)
        {
            if (!subscribers.ContainsKey(configType))
            {
                subscribers[configType] = new List<Action<object>>();
            }
            subscribers[configType].Add(handler);
        }

        public void Unsubscribe(CONFIG_TYPE configType, Action<object> handler)
        {
            if (subscribers.ContainsKey(configType))
            {
                subscribers[configType].Remove(handler);
                if (subscribers[configType].Count == 0)
                {
                    subscribers.Remove(configType);
                }
            }
        }

        public void Publish(CONFIG_TYPE configType, object eventToPublish = null)
        {
            if (subscribers.ContainsKey(configType))
            {
                foreach (var handler in subscribers[configType])
                {
                    if (handler.Target is Control control
                        && control.InvokeRequired)
                    {
                        control.Invoke(handler, eventToPublish);
                    }
                    else
                    {
                        handler?.Invoke(eventToPublish);
                    }
                }
            }
        }
        #endregion

        #region //Rear의 IOFUnctionUtil을 가져옴

        public static void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                using (File.Create(path))
                {

                }
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Already exist"));
                msgBox.ShowDialog();
            }
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Already exist"));
                msgBox.ShowDialog();
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Can not find file"));
                msgBox.ShowDialog();
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Can not find folder"));
                msgBox.ShowDialog();
            }
        }

        public static void CopyFile(string sourcePath, string targetPath)
        {
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, targetPath);
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Can not find source file"));
                msgBox.ShowDialog();
            }
        }

        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                string[] files = Directory.GetFiles(sourcePath);
                string[] folders = Directory.GetDirectories(sourcePath);

                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(targetPath, name);
                    File.Copy(file, dest);
                }
                foreach (string folder in folders)
                {
                    string nameFolder = Path.GetFileName(folder);
                    string dest = Path.Combine(targetPath, nameFolder);
                    CopyDirectory(folder, dest);
                }
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Can not find source folder"));
                msgBox.ShowDialog();
            }
        }

        public static void RenameFile(string sourcePath, string targetPath)
        {
            if (File.Exists(sourcePath))
            {
                File.Move(sourcePath, targetPath);
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Can not find source file"));
                msgBox.ShowDialog();
            }
        }

        public static void RenameDirectory(string sourcePath, string targetPath)
        {
            if (Directory.Exists(sourcePath))
            {
                Directory.Move(sourcePath, targetPath);
            }
            else
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                        string.Format("Error: Can not find source file"));
                msgBox.ShowDialog();
            }
        }

        public static bool CheckFileExist(string sourcePath)
        {
            return File.Exists(sourcePath);
        }

        public static bool CheckDirectoryExist(string sourcePath)
        {
            return Directory.Exists(sourcePath);
        }

        #endregion   

        public void CreateDummyValueAxisPos(string strNewModelName)
        {
            //1. Position.Json파일을 가져온다.
            //2. 해당 파일 객체를 복사한다.
            //3. 0으로 바꾸고 저장한다.
            string path = SystemDefine.recipePath + "\\" + strNewModelName + "\\" + "AxisPos.json";
            CreateFile(path);
            ParamUtil newDatas = new ParamUtil(path);

            ParamUtil originDatas = Machine.param.position;

            for (int i = 0; i < originDatas.Count; ++i)
            {
                PARAMSTRUCT pararm = new PARAMSTRUCT();

                pararm.Name = originDatas.getParam(i).Name;
                pararm.Tag = originDatas.getParam(i).Tag;
                pararm.Value = originDatas.getParam(i).Value;
                pararm.TargetAxis = originDatas.getParam(i).TargetAxis;
                pararm.Unit = originDatas.getParam(i).Unit;
                pararm.Min = originDatas.getParam(i).Min;
                pararm.Max = originDatas.getParam(i).Max;

                newDatas.GetParamList().Add(pararm);
            }

            for (int i = 0; i < newDatas.Count; ++i)
            {
                newDatas.GetParamList()[i].Value = 0;
            }

            foreach (var param in newDatas.GetParamList())
                newDatas.dic.Add(param.Tag, param);

            newDatas.Write();
        }
    }
}
