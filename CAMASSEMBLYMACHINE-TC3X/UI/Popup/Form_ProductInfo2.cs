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
using Microsoft.Win32;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Vision;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_ProductInfo2 : Form
    {
        #region VARIABLES
        public enum INFO_TYPE
        {
            CURRENT,
            SELECTED_DATE
        }

        public enum BUTTON_TYPE
        {
            EXIT,
            RESET_INFO
        }

        private bool refreshData = true;
        private List<TopEng.Controls.ProductInfoJIG> listProductInfoJig = new List<TopEng.Controls.ProductInfoJIG>();
        private List<VisionResult> visionResults = new List<VisionResult>();
        private int curPage = 0;
        private int maxPage = 0;
        public int maxJigCount = 15;
        #endregion

        #region INITIALIZE
        public Form_ProductInfo2()
        {
            InitializeComponent();
            nV_Button_PB_NS_Reset.ClickEvent += FunctionButton_ClickEvent;
            nV_Button_PB_NS_Exit.ClickEvent += FunctionButton_ClickEvent;

            listProductInfoJig.Add(productInfoJIG1);
            listProductInfoJig.Add(productInfoJIG2);
            listProductInfoJig.Add(productInfoJIG3);
            listProductInfoJig.Add(productInfoJIG4);
            listProductInfoJig.Add(productInfoJIG5);
            listProductInfoJig.Add(productInfoJIG6);
            listProductInfoJig.Add(productInfoJIG7);
            listProductInfoJig.Add(productInfoJIG8);
            listProductInfoJig.Add(productInfoJIG9);
            listProductInfoJig.Add(productInfoJIG10);

            for (int i = 0; i < listProductInfoJig.Count; ++i)
            {
                listProductInfoJig[i].SetName(string.Format("JIG #{0:00}", curPage * listProductInfoJig.Count + (i + 1)));
                listProductInfoJig[i].id = curPage * listProductInfoJig.Count + (i + 1);
            }

            maxJigCount = (int)Machine.recipe.Option(Define.RecipeDefine.OPTION.USAGE_JIGS_COUNTS);
            maxPage = (maxJigCount - 1) / 10;
            label_InPage.Text = "<" + (curPage + 1).ToString() + "/" + (maxPage + 1).ToString() + ">";

            ChangePage(0);
        }

        public void Show()
        {
            this.ShowDialog();
        }

        private void FunctionButton_ClickEvent(object sender, EventArgs e)
        {
            string sTag = (sender as NV_UI.NV_Button_PB_NS).Tag.ToString();
            if (Enum.TryParse<BUTTON_TYPE>(sTag, out BUTTON_TYPE type))
            {
                switch (type)
                {
                    case BUTTON_TYPE.EXIT:
                        this.Close();
                        break;
                    case BUTTON_TYPE.RESET_INFO:
                        LogUtil.Instance.Log(LOG_TYPE.UI, "Product Count Reset Button Click", CONTENT_TYPE.INFO);

                        if (Machine.status.state != SystemState.SystemStateRUN)
                        {
                            Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("모든 생산 수량을 초기화 하시겠습니까?"));
                            if (form.ShowDialog() == DialogResult.Yes)
                            {

                                Machine.ClearProductCount();
                                productCountValueFailRate.CountText = "None";
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        #region DISPLAY_DATA
        private void DisplayCurrentProductInfor()
        {
            productCountValueTotal.CountText = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_TOTAL_COUNT).ToString();
            productCountValueInput.CountText = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT).ToString();
            productCountValueOutput.CountText = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT).ToString();
            productCountValueFail.CountText = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT).ToString();
            double outOk = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT);
            double outNg = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT);
            double outNgRate = (outNg / outOk) * 100.0f;
            productCountValueFailRate.CountText = $"{outNgRate.ToString("N2")}%";
            UpdateJigInfo();
        }
        #endregion


        #region TIMER
        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplayCurrentProductInfor();

            if (Machine.totalCount > 0)
                disp_TactAvg.Text = (Machine.totalTime / Machine.totalCount).ToString("0.00");
        }
        #endregion
        private void ChangePage(int inPage)
        {
            if (inPage > maxPage) inPage = 0;
            if (inPage < 0) inPage = maxPage;

            curPage = inPage;

            for (int i = 0; i < listProductInfoJig.Count; ++i)
            {
                listProductInfoJig[i].SetName(string.Format("JIG #{0:00}", curPage * listProductInfoJig.Count + (i + 1)));
                listProductInfoJig[i].id = curPage * listProductInfoJig.Count + (i + 1);

                if (listProductInfoJig[i].id > maxJigCount && listProductInfoJig[i].Visible)
                    listProductInfoJig[i].Visible = false;
                else if (listProductInfoJig[i].id <= maxJigCount && !listProductInfoJig[i].Visible)
                    listProductInfoJig[i].Visible = true;
            }

            label_InPage.Text = "<" + (curPage + 1).ToString() + "/" + (maxPage + 1).ToString() + ">";
        }
        private void button_InPrev_Click(object sender, EventArgs e)
        {
            ChangePage(curPage - 1);
        }
        private void button_InHome_Click(object sender, EventArgs e)
        {
            ChangePage(0);
        }
        private void button_InNext_Click(object sender, EventArgs e)
        {
            ChangePage(curPage + 1);
        }

        //지그 값 받아오는거 업데이트 되면 넣읍시다.
        void UpdateJigInfo()
        {
            for (int i = 0; i < listProductInfoJig.Count; ++i)
            {
                int JigID = listProductInfoJig[i].id - 1;
                if (JigID >= maxJigCount) break;

                double outOk = Machine.info.Jig(JigID, StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT);
                double outNg = Machine.info.Jig(JigID, StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT);
                double outNgRate = (outNg / outOk) * 100.0f;

                //데이터 가져와서 
                listProductInfoJig[i].productCountValueH_Input.CountText = Machine.info.Jig(JigID, StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT).ToString();
                listProductInfoJig[i].productCountValueH_Output.CountText = Machine.info.Jig(JigID, StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT).ToString();
                listProductInfoJig[i].productCountValueH_NG.CountText = Machine.info.Jig(JigID, StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT).ToString();
                listProductInfoJig[i].productCountValueH_NgRate.CountText = $"{outNgRate.ToString("N2")}%";
            }
        }

        private void Form_ProductInfo2_Load(object sender, EventArgs e)
        {
            ChangePage(0);
        }

        private void disp_TactAvg_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tact Average Clear Button Click", CONTENT_TYPE.INFO);

            Dlg_MessageBox formTarget = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("Do you want to Reset TactAvg?"));
            if (formTarget.ShowDialog() == DialogResult.Yes)
            {
                Machine.ClearTacTimeInfo();
                disp_TactAvg.Text = (0).ToString("0.00");
            }
        }
    }
}
