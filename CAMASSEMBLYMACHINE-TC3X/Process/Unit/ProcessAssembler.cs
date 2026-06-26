using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.UI;
using TopEng.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Type;
using TopEng.Utils;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;
using TopEng.Controls;
using TopEng.Device;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessAssembler : IProcess
    {
        public enum AUTOSTEP
        {
            // 시스템 프로세스
            ERROR,
            IDLE,
            STOP,
            COMPLETE,

            // 초기화 프로세스
            INIT,
            INIT_CHECK,
            OTHER_PROCESS_INIT_WAIT,
            PROC_SELECT_FOR_RESTART,

            // 실제 자동 프로세스
            LOADING_CHECK,
            LOADING,
            LOADING_COMPL_CHECK,
            LOADING_APPROACH,
            LOADING_APPROACH_COMPL_CHECK,
            ALIGNMENT,
            ALIGNMENT_COMPL_CHECK,
            ALIGNMENT2,
            ALIGNMENT2_COMPL_CHECK,
            DISCARD_LEFT,
            DISCARD_LEFT_CHECK,
            DISCARD_RIGHT,
            DISCARD_RIGHT_CHECK,
            UNLOADING_CHECK,
            UNLOADING,
            UNLOADING_COMPL_CHECK,
        }

        public enum STEP
        {
            // 시스템 스텝
            ERROR = -1,
            IDLE,
            STOP,
            NEXT_STEP,

            // INIT
            INIT_VACOFF,
            INIT_PRODUCT_DATA_MISMATCH_CHECK,

            // TIME
            ASSEMBLE_WAIT,
            ASSEMBLE_WAIT_CHECK,
            VAC_ON_WAIT,
            VAC_ON_WAIT_CHECK,
            PURGE_OFF_WAIT,
            PURGE_OFF_WAIT_CHECK,

            // INTERFERENCE
            INF_PICKER_ON,
            INF_PICKER_OFF,
            INF_BUFFER_ON,
            INF_BUFFER_OFF,
            INF_JIG_ON,
            INF_JIG_OFF,

            // INTERFACE
            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_UNLOADING_REQUEST,
            IF_UNLOADING_BUSY,
            IF_UNLOADING_COMPLETE_CHECK,
            SENDING_PRODUCT_DATA_TO_JIG,
            IF_INCREASE_NG_COUNT,

            PROC_ALIGN_START,
            PROC_ALIGN_COMPL_CHECK,
            PROC_ALIGN_ERROR_CHECK,

            MOVE_XYR_POS_CHECK,
            MOVE_XY_POS_CHECK,
            MOVE_Z_POS_CHECK,

            MOVE_XYR_READY_POS,
            MOVE_XYR_PICKUP_POS,
            MOVE_XYR_PICKUP_POS_OVERLAP,
            MOVE_XYR_PICKUP_READY_POS,
            MOVE_XYR_ALIGN_POS,
            MOVE_XYR_ALIGN_POS_READY,
            MOVE_XYR_PLACE_POS,
            MOVE_XYR_PLACE_POS_OVERLAP,
            MOVE_XYR_TRASH_POS,
            MOVE_XYR_SCAN_POS,
            MOVE_XYR_SCAN_POS_OVERLAP,

            MOVE_Z_READY_POS,
            MOVE_Z_PICKUP_READY_POS,
            MOVE_Z_PICKUP_UP_READY_POS,
            MOVE_Z_PICKUP_UP_READY_POS_FAST,
            MOVE_Z_PICKUP_UP_READY_POS_FAST_OVERLAP,
            MOVE_Z_PICKUP_POS,
            MOVE_Z_UNDER_ALIGN_POS,
            MOVE_Z_UNDER_ALIGN_POS_OVERLAP,
            MOVE_Z_PLACE_READY_POS,
            MOVE_Z_PLACE_UP_READY_POS,
            MOVE_Z_PLACE_UP_READY_POS_FAST,
            MOVE_Z_PLACE_UP_READY_POS_FAST_OVERLAP,
            MOVE_Z_PLACE_POS,
            MOVE_Z_SCAN_POS,
            MOVE_Z_SCAN_POS_OVERLAP,
            MOVE_Z_DISCARD_POS,

            PICKER_UP,
            PICKER_UP_BYPASS,
            PICKER_UP_CHECK,
            PICKER_UP_ALL,
            PICKER_UP_ALL_BYPASS,
            PICKER_UP_ALL_CHECK,
            PICKER_DOWN,
            PICKER_DOWN_BYPASS,
            PICKER_DOWN_CHECK,
            PICKER_DOWN_ALL,
            PICKER_DOWN_ALL_BYPASS,
            PICKER_DOWN_ALL_CHECK,
            PICKER_VACON,
            PICKER_VACON_CHECK,
            PICKER_VACON_ALL,
            PICKER_VACON_ALL_CHECK,
            PICKER_VACOFF,
            PICKER_VACOFF_BYPASS,
            PICKER_VACOFF_DELAY,
            PICKER_VACOFF_CHECK,
            PICKER_VACOFF_ALL,
            PICKER_VACOFF_ALL_CHECK,
            PICKER_PURGEOFF,

            BUF_VACOFF,
            BUF_VACOFF_CHECK,

            CAM_BUF_DETECT_ON_CHECK,
            CAM_BUF_DETECT_OFF_CHECK,

            // CALIBRATION STEP
            REQUEST_UNDER_CAL_PIXEL,
            REQUEST_UNDER_CALIBRATION_CHECK,
            REQUEST_JIG_CAL_PIXEL,
            REQUEST_JIG_CALIBRATION_CHECK,

            GENERATE_JIG_MAP,
            GENERATE_UNDER_MAP,
            MOVE_XY_MAPPING_JIG,
            MOVE_XY_MAPPING_UNDER,
            IF_MAPPING_DONE_CHECK_JIG,
            SAVE_MAPPING_FILE_JIG,
            IF_MAPPING_DONE_CHECK_UNDER,
            SAVE_MAPPING_FILE_UNDER,

            MOVE_Z_UNDER_CAL_POS,
            MOVE_Z_JIG_CAL_POS,

            MOVE_XY_MAPPING_JIG_START_XY,
            MOVE_XY_MAPPING_JIG_END_XY,
            MOVE_XY_MAPPING_UNDER_START_XY,
            MOVE_XY_MAPPING_UNDER_END_XY,

            MOVE_XY_RECAL_POS_XY,
            IF_RECAL_COMPLETE_JIG,
            IF_RECAL_COMPLETE_UNDER,
        }

        public enum MSG
        {
            MSG_SINGLE,

            MSG_PROCESS_INIT,
            MSG_LOADING_PRODUCT,
            MSG_ALIGN_PRODUCT,
            MSG_ALIGN2_PRODUCT,
            MSG_DISCARD_NG_PRODUCT,
            MSG_UNLOADING_PRODUCT,

            MSG_MOVE_READY_POS,
            MSG_MOVE_PICKUP_POS,
            MSG_MOVE_PICKUP_READY_POS,
            MSG_MOVE_PLACE_POS,
            MSG_MOVE_PLACE2_POS,
            MSG_MOVE_ALIGN_POS,
            MSG_MOVE_ALIGN2_POS,
            MSG_MOVE_SCAN_POS,
            MSG_MOVE_SCAN2_POS,
            MSG_MOVE_TRASH_POS,
            MSG_MOVE_TRASH2_POS,

            MSG_MAPPING_CAL_JIG_SINGLE,
            MSG_MAPPING_CAL_JIG_DUAL,
            MSG_MAPPING_CAL_UNDER_SINGLE,
            MSG_MOVE_MAP_JIG_START_XY,
            MSG_MOVE_MAP_JIG_END_XY,
            MSG_MOVE_MAP_UNDER_START_XY,
            MSG_MOVE_MAP_UNDER_END_XY,
            MSG_MAPPING_RECAL_JIG_POINTS,
            MSG_MAPPING_RECAL_UNDER_POINTS,
            MSG_MOVE_RECAL_POINT_XY_JIG,
            MSG_MOVE_RECAL_POINT_XY_UNDER,
        }

        public enum XYRPOS
        {
            READY,
            BUF1,
            BUF2,
            BUF1READY,
            BUF2READY,
            ALIGN1,
            ALIGN2,
            SCAN1,
            SCAN2,

            PLACE1,
            PLACE2,
            TRASH1,
            TRASH2,
        }

        public enum ZPOS
        {
            READY,
            SCAN,
            PICKREADY,
            PICKUPREADY,
            PICKUPREADY_FAST,
            PICK,
            UNDERALIGN,
            PLACEREADY,
            PLACEUPREADY,
            PLACEUPREADY_FAST,
            PLACE1,
            PLACE2,
            UNDERCAL,
            JIGCAL,
            DISCARD,
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
        public STEP Step = STEP.IDLE;
        public int StepIndex = 0;
        public AUTOSTEP AutoStep = AUTOSTEP.IDLE;
        public override Type StepEnumType => typeof(STEP);
        public override Type MsgEnumType => typeof(MSG);
        public override bool Busy()
        {
            return (AutoStep != AUTOSTEP.IDLE || Step != STEP.IDLE) && (AutoStep != AUTOSTEP.STOP || Step != STEP.STOP) && (AutoStep != AUTOSTEP.ERROR || Step != STEP.ERROR);
        }

        public override bool Ready()
        {
            return Step == STEP.IDLE || Step == STEP.STOP;
        }

        public override bool Stopped()
        {
            return (AutoStep == AUTOSTEP.STOP && Step == STEP.STOP);
        }

        public override bool Error()
        {
            return (AutoStep == AUTOSTEP.ERROR || Step == STEP.ERROR);
        }

        public STEP NextStep()
        {
            if (++StepIndex < StepList.Count())
                Step = StepList[StepIndex];
            else
                Step = STEP.IDLE;
            return Step;
        }

        public override int GetStep() { return (int)Step; }
        public override int GetAutoStep() { return (int)AutoStep; }

        public override void AutoStart()
        {
            stopBit = false;
            AutoStep = AUTOSTEP.INIT;
        }

        public override void EStop(bool pause)
        {
            PauseCal();
            stopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;

            AutoStep = AUTOSTEP.IDLE;
            Step = STEP.IDLE;

            Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
            Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
            Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
        }

        public bool CheckStopBit()
        {
            if (stopBit)
            {
                Step = STEP.IDLE;
                AutoStep = AUTOSTEP.IDLE;
                return true;
            }

            return false;
        }
        #endregion

        // SYSTEM SETTING
        private int[] upstream = { (int)UNITPART.BUF2_L1, (int)UNITPART.BUF2_R1 };
        private int[] current = { (int)UNITPART.ASSEMBLER1, (int)UNITPART.ASSEMBLER2 };
        private int[] downstream = { (int)UNITPART.JIG1, (int)UNITPART.JIG2 };

        // PROCESS
        public int targetCurr = 0;
        private bool receivedData = false;

        // ALIGNMENT DATA
        private TOOL_TYPE calibToolType;
        public Point2d[] productPosition = new Point2d[2];
        public double[] productAngle = new double[2];
        public Point2d[] productPositionOLD = new Point2d[2];
        public double[] productAngleOLD = new double[2];
        public bool[] first_alignment = { true, true };
        public double[] placePos = { 0.0, 0.0, 0.0, 0.0 };
        private Point2d[] PlacePos = new Point2d[2];
        private Point2d[] adjustPos = new Point2d[2];
        private double[] adjustAngle = { 0, 0 };
        public int[] alignSucs = { -1, -1 };
        public bool[] prev_product_exist = { true, true };
        public double[] alignPosView = { 0.0, 0.0, 0.0, 0.0 };

        public DataMan DataReader = new DataMan();
        public bool OnReceivedBarcode = false;
        public string barcodeString = "";
        public string[] productID = { " ", " " };
        private RecordUtil recordUtil;
        int id_reading_count = 0;
        bool duplated_qrcode = false;
        private string DATA_READER_TRIGGER_STRING = "+";
        Point2d currCalibrationPos = new Point2d();
        int calibCount;
        bool isPauseCal = false;
        int calPauseMSG = -1;
        List<CalibPoint> reCalPointList = new List<CalibPoint>();
        int reCalIndex;
        int retryCount;
        int last_pickup_buffer_index = 0;
        int vision_fail_count = 0;
        int targetCam = 0;
        public int[] currentNGCount = new int[(int)TOOL_TYPE.MAX];
        public int[] currentNGCountInX = new int[(int)TOOL_TYPE.MAX];
        public int[] currentNGCountInY = new int[(int)TOOL_TYPE.MAX];


        private int underErrorCheckCount = 0;
        private int underErrorFinalCheckCount = 0;
        private TOOL_TYPE lastPickedSide = TOOL_TYPE.MAX;


        public ProcessAssembler()
        {
            for (int i = 0; i < productPosition.Length; i++)
            {
                productPosition[i] = new Point2d();
                adjustPos[i] = new Point2d();
            }
            for (int i = 0; i < productPositionOLD.Length; i++)
                productPositionOLD[i] = new Point2d();
            for (int i = 0; i < PlacePos.Length; i++)
                PlacePos[i] = new Point2d();

            currentNGCountInX[0] = 0;
            currentNGCountInX[1] = 0;
            currentNGCountInY[0] = 0;
            currentNGCountInY[1] = 0;

            currentNGCount[(int)TOOL_TYPE.LEFT] = 0;
            currentNGCount[(int)TOOL_TYPE.RIGHT] = 0;
        }

        public void ClearAlignData()
        {
            for (int i = 0; i < 2; i++)
            {
                productPosition[0].x = 0;
                productPosition[0].y = 0;
                productAngle[i] = 0;
            }
        }

        public void cbInspectionResultEvent(string inspname, Dictionary<string, List<VisionResult>> list, bool success)
        {
            try
            {
                if (list["OutPuts"].Count < 1) throw new Exception("Outputs data empty");

                var tempresult = list["OutPuts"][0] as OutPutsResult;

                if (targetCurr == (int)TOOL_TYPE.RIGHT && (tempresult.Rcenter.x == Vision.INVALID_DATA || tempresult.Rcenter.y == Vision.INVALID_DATA)) throw new Exception("Cognex not return data");
                if (targetCurr == (int)TOOL_TYPE.LEFT && (tempresult.Lcenter.x == Vision.INVALID_DATA || tempresult.Lcenter.y == Vision.INVALID_DATA)) throw new Exception("Cognex not return data");

                productPosition[targetCurr].x = tempresult.Rcenter.x;
                productPosition[targetCurr].y = tempresult.Rcenter.y;
                productAngle[targetCurr] = tempresult.Rangle;
                alignSucs[targetCurr] = 0;

                if (Vision.IsSameWithLastData(CAMERA.UNDER, (TOOL_TYPE)targetCurr, new Point2d(productPosition[targetCurr].x, productPosition[targetCurr].y)))
                    throw new Exception("New data is same with last data");
                LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"JIG HEAD_{targetCurr}: ClassifyClass OK, ClassifyScore 1", CONTENT_TYPE.INFO);
            }
            catch (Exception ex)
            {
                productPosition[targetCurr].x = 0.001 * 0.0;
                productPosition[targetCurr].y = 0.001 * 0.0;
                productAngle[targetCurr] = 0.0;
                alignSucs[targetCurr] = -1;
                LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"JIG HEAD_{targetCurr}: ClassifyClass NG, ClassifyScore 1", CONTENT_TYPE.INFO);
            }
            receivedData = true;
            LogUtil.Instance.Log(LOG_TYPE.INSPECTION,
                $"Under Inspection Data: " +
                $"[alignSucs: {alignSucs[targetCurr]}]" +
                $"[target Head: {targetCurr}]" +
                $"[x: {productPosition[targetCurr].x}, y: {productPosition[targetCurr].y}, r: {productAngle[targetCurr]}]", CONTENT_TYPE.INFO);
        }

        public void PauseCal()
        {
            isPauseCal = true;
        }

        public void ResumeCal()
        {
            if (calPauseMSG == -1) return;
            SetMessage(calPauseMSG);
        }
        public void ResetPauseVar() => isPauseCal = false;

        public void SetReCalPoint(List<CalibPoint> pointList)
        {
            reCalIndex = 0;
            this.reCalPointList = pointList;
        }

        public override void TimeOut()
        {

        }

        private void SetError(ECODE error)
        {
            if (error_proc)
                return;
            error_proc = true;

            // ERROR MESSAGE
            switch ((ECODE)error)
            {
                case ECODE.TIMEOUT_ASSEMBLER1_VACON:
                case ECODE.TIMEOUT_ASSEMBLER2_VACON:
                    var processErrorHandler = new List<STEP>()
                    {
                        //STEP.PICKER_VACOFF,
                        STEP.MOVE_Z_READY_POS,
                        STEP.ERROR
                    };

                    StepList = processErrorHandler;
                    Step = StepList[StepIndex = 0];
                    break;

                default:
                    var defaultError = new List<STEP>()
                    {
                        STEP.ERROR,
                    };

                    StepList = defaultError;
                    Step = StepList[StepIndex = 0];
                    break;

            }

            AutoStep = AUTOSTEP.STOP;
            Machine.Alarm(error);
        }

        public override void SetMessage(int message, int step = 0)
        {
            if (!Ready() || Error())
            {
                bool MessageBoxShown = false;
                if (!MessageBoxShown)
                {
                    string msg;
                    msg = $"The equipment is running or in an error state.[{Enum.GetName(typeof(MSG), message)}]";
                    MessageBox.Show(msg);
                    MessageBoxShown = true;
                    return;
                }
            }

            //leedh 2025/10/13
            stopBit = false;
            bool isManualMode = Machine.status.state == SystemState.SystemStateIDLE;
            double apos = 0;

            switch ((MSG)message)
            {
                case MSG.MSG_SINGLE:
                    var processSingle = new List<STEP>()
                    {
                        STEP.IDLE,
                        STEP.IDLE,
                    };

                    StepList = processSingle;
                    StepList[0] = (STEP)step;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_PROCESS_INIT:
                    var processInit = new List<STEP>()
                    {
                        //STEP.LIGHT
                        STEP.INIT_VACOFF,
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.MOVE_XYR_READY_POS,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    //Machine.interfer_assembler_loading = true;
                    Machine.interfer_assembler_on_buffer[targetCurr] = true;
                    var processLoadingProduct = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_PURGEOFF,
                        STEP.IF_LOADING_POSSIBLE_WAIT, // 트레이 상태 확인
                        STEP.IF_LOADING_BUSY,
                        STEP.MOVE_XYR_PICKUP_POS,
                        STEP.PICKER_DOWN_ALL_BYPASS,
                        STEP.INF_JIG_OFF,
                        STEP.CAM_BUF_DETECT_ON_CHECK,
                        STEP.MOVE_Z_PICKUP_READY_POS,
                        STEP.MOVE_Z_PICKUP_POS,
                        STEP.PICKER_DOWN_ALL_CHECK,
                        STEP.PICKER_VACON_ALL,
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_PICKUP_READY_POS,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.PICKER_VACON_ALL,
                        STEP.IDLE,
                    };

                    StepList = processLoadingProduct;
                    if (isManualMode)
                    {
                        StepList.Insert(0, STEP.MOVE_Z_READY_POS);
                        StepList.Insert(1, STEP.PICKER_UP_ALL);
                    }
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_ALIGN_PRODUCT:
                case MSG.MSG_ALIGN2_PRODUCT:
                    if (message == (int)MSG.MSG_ALIGN_PRODUCT) targetCurr = 0;
                    if (message == (int)MSG.MSG_ALIGN2_PRODUCT) targetCurr = 1;

                    vision_fail_count = 0;
                    underErrorCheckCount = 0;
                    underErrorFinalCheckCount = 0;

                    alignSucs[targetCurr] = -1;
                    productPosition[targetCurr].x = 0;
                    productPosition[targetCurr].y = 0;
                    adjustPos[targetCurr].x = 0;
                    adjustPos[targetCurr].y = 0;
                    adjustAngle[targetCurr] = 0;
                    prev_product_exist[targetCurr] = false;

                    //Machine.interfer_assembler_loading = true;
                    Machine.interfer_assembler_on_buffer[targetCurr] = true;

                    var processAlignProduct = new List<STEP>()
                    {
                        STEP.MOVE_Z_UNDER_ALIGN_POS_OVERLAP,
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_XYR_ALIGN_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.MOVE_Z_POS_CHECK,
                        STEP.INF_JIG_OFF,
                        STEP.INF_BUFFER_OFF,
                        STEP.MOVE_Z_UNDER_ALIGN_POS,
                        STEP.PROC_ALIGN_START,
                        STEP.PROC_ALIGN_ERROR_CHECK,
                        STEP.IDLE,
                    };
                    StepList = processAlignProduct;
                    if (isManualMode)
                    {
                        StepList.Insert(0, STEP.MOVE_Z_READY_POS);
                        StepList.Insert(1, STEP.PICKER_UP_ALL);
                    }
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_DISCARD_NG_PRODUCT:
                    var processNG = new List<STEP>()
                    {
                        STEP.MOVE_Z_DISCARD_POS,
                        STEP.MOVE_XYR_TRASH_POS,
                        STEP.PICKER_DOWN,
                        STEP.PICKER_VACOFF,
                        STEP.IF_INCREASE_NG_COUNT,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processNG;
                    if (isManualMode)
                    {
                        StepList.Insert(0, STEP.MOVE_Z_READY_POS);
                        StepList.Insert(1, STEP.PICKER_UP_ALL);
                    }
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    //Machine.interfer_assembler_loading = false;
                    Machine.interfer_jig_align = true;
                    Machine.Parts[(int)UNITPART.ASSEMBLER1].ClearInterface();
                    Machine.Parts[(int)UNITPART.ASSEMBLER2].ClearInterface();

                    var processUnloadingProduct = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_UNDER_ALIGN_POS,
                        STEP.MOVE_XYR_PLACE_POS_OVERLAP,
                        STEP.INF_BUFFER_OFF,
                        STEP.MOVE_Z_PLACE_READY_POS,
                        STEP.MOVE_Z_PLACE_POS,
                        STEP.MOVE_XYR_POS_CHECK,
                        STEP.IF_UNLOADING_REQUEST,
                        STEP.IF_UNLOADING_BUSY,
                        STEP.PICKER_DOWN,
                        STEP.PICKER_VACOFF_BYPASS,
                        STEP.PICKER_VACOFF_DELAY,
                        STEP.SENDING_PRODUCT_DATA_TO_JIG,
                        STEP.PICKER_UP_ALL,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        STEP.IDLE,
                    };
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Z, ref apos);
                    bool isZInPlacePos = Math.Abs(apos / 1000 - (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS))) <= 0.2;
                    if (!Machine.Parts[(int)UNITPART.ASSEMBLER1].exist && Machine.Parts[(int)UNITPART.ASSEMBLER2].exist
                    && Machine.Parts[(int)UNITPART.JIG1].exist && !Machine.Parts[(int)UNITPART.JIG2].exist
                    && !isManualMode && isZInPlacePos)
                    {
                        processUnloadingProduct[GetStepIndex(processUnloadingProduct, STEP.MOVE_Z_UNDER_ALIGN_POS)] = STEP.NEXT_STEP;
                        processUnloadingProduct[GetStepIndex(processUnloadingProduct, STEP.MOVE_Z_PLACE_READY_POS)] = STEP.MOVE_XYR_POS_CHECK;
                    }

                    StepList = processUnloadingProduct;
                    if (isManualMode)
                    {
                        StepList.Insert(0, STEP.MOVE_Z_READY_POS);
                        StepList.Insert(1, STEP.PICKER_UP_ALL);
                    }
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_READY_POS:
                case MSG.MSG_MOVE_PICKUP_POS:
                case MSG.MSG_MOVE_PICKUP_READY_POS:
                case MSG.MSG_MOVE_PLACE_POS:
                case MSG.MSG_MOVE_PLACE2_POS:
                case MSG.MSG_MOVE_ALIGN_POS:
                case MSG.MSG_MOVE_ALIGN2_POS:
                case MSG.MSG_MOVE_SCAN_POS:
                case MSG.MSG_MOVE_SCAN2_POS:
                case MSG.MSG_MOVE_TRASH_POS:
                case MSG.MSG_MOVE_TRASH2_POS:
                    var moveXYRPosition = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XYR_PICKUP_POS,
                        STEP.IDLE,
                    };

                    if ((MSG)message == MSG.MSG_MOVE_READY_POS) moveXYRPosition[2] = STEP.MOVE_XYR_READY_POS;
                    if ((MSG)message == MSG.MSG_MOVE_PICKUP_POS) moveXYRPosition[2] = STEP.MOVE_XYR_PICKUP_POS;
                    if ((MSG)message == MSG.MSG_MOVE_PICKUP_READY_POS) moveXYRPosition[2] = STEP.MOVE_XYR_PICKUP_READY_POS;
                    if ((MSG)message == MSG.MSG_MOVE_PLACE_POS) { targetCurr = 0; moveXYRPosition[2] = STEP.MOVE_XYR_PLACE_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_PLACE2_POS) { targetCurr = 1; moveXYRPosition[2] = STEP.MOVE_XYR_PLACE_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_ALIGN_POS) { targetCurr = 0; moveXYRPosition[2] = STEP.MOVE_XYR_ALIGN_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_ALIGN2_POS) { targetCurr = 1; moveXYRPosition[2] = STEP.MOVE_XYR_ALIGN_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_SCAN_POS) { targetCurr = 0; moveXYRPosition[2] = STEP.MOVE_XYR_SCAN_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_SCAN2_POS) { targetCurr = 1; moveXYRPosition[2] = STEP.MOVE_XYR_SCAN_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_TRASH_POS) { targetCurr = 0; moveXYRPosition[2] = STEP.MOVE_XYR_TRASH_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_TRASH2_POS) { targetCurr = 1; moveXYRPosition[2] = STEP.MOVE_XYR_TRASH_POS; }

                    StepList = moveXYRPosition;
                    Step = StepList[StepIndex = 0];
                    break;

                #region MESSAGE CALIBRATION
                case MSG.MSG_MOVE_MAP_JIG_START_XY:
                case MSG.MSG_MOVE_MAP_JIG_END_XY:
                case MSG.MSG_MOVE_MAP_UNDER_START_XY:
                case MSG.MSG_MOVE_MAP_UNDER_END_XY:
                    var processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_MAPPING_JIG_START_XY,
                        STEP.IDLE,
                    };
                    if ((MSG)message == MSG.MSG_MOVE_MAP_JIG_START_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_JIG_START_XY;
                    if ((MSG)message == MSG.MSG_MOVE_MAP_JIG_END_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_JIG_END_XY;
                    if ((MSG)message == MSG.MSG_MOVE_MAP_UNDER_START_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_UNDER_START_XY;
                    if ((MSG)message == MSG.MSG_MOVE_MAP_UNDER_END_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_UNDER_END_XY;
                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_CAL_JIG_SINGLE:
                    calPauseMSG = message;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    if (!isPauseCal)
                    {
                        Vision.ClearBeforeMapping(CAMERA.JIG, calibToolType);
                    }
                    Vision.SetHeadTarget("JIG", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_DOWN,
                        STEP.GENERATE_JIG_MAP,
                        STEP.MOVE_XY_MAPPING_JIG,
                        STEP.MOVE_Z_JIG_CAL_POS,
                        STEP.REQUEST_JIG_CAL_PIXEL,
                        STEP.IF_MAPPING_DONE_CHECK_JIG,
                        STEP.SAVE_MAPPING_FILE_JIG,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    if (isPauseCal) Step = StepList[StepIndex = 4];
                    else Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_CAL_JIG_DUAL:
                    calPauseMSG = message;
                    calibToolType = TOOL_TYPE.MAX;
                    if (!isPauseCal)
                    {
                        Vision.ClearBeforeMapping(CAMERA.JIG, TOOL_TYPE.LEFT);
                        Vision.ClearBeforeMapping(CAMERA.JIG, TOOL_TYPE.RIGHT);
                    }
                    Vision.SetHeadTarget("JIG", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_DOWN_ALL,
                        STEP.GENERATE_JIG_MAP,
                        STEP.MOVE_XY_MAPPING_JIG,
                        STEP.MOVE_Z_JIG_CAL_POS,
                        STEP.REQUEST_JIG_CAL_PIXEL,
                        STEP.IF_MAPPING_DONE_CHECK_JIG,
                        STEP.SAVE_MAPPING_FILE_JIG,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    if (isPauseCal) Step = StepList[StepIndex = 4];
                    else Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_RECAL_JIG_POINTS:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    Vision.SetHeadTarget("JIG", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.PICKER_DOWN,
                        STEP.MOVE_Z_JIG_CAL_POS,
                        STEP.REQUEST_JIG_CAL_PIXEL,
                        STEP.IF_RECAL_COMPLETE_JIG,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_RECAL_POINT_XY_JIG:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;

                    processMappingList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.PICKER_DOWN,
                        STEP.MOVE_Z_JIG_CAL_POS,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_CAL_UNDER_SINGLE:
                    calPauseMSG = message;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    if (!isPauseCal)
                    {
                        Vision.ClearBeforeMapping(CAMERA.UNDER, calibToolType);
                    }
                    Vision.SetHeadTarget("UNDER", calibToolType);

                    var processMappingUNDERList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.GENERATE_UNDER_MAP,
                        STEP.MOVE_XY_MAPPING_UNDER,
                        STEP.MOVE_Z_UNDER_CAL_POS,
                        STEP.REQUEST_UNDER_CAL_PIXEL,
                        STEP.IF_MAPPING_DONE_CHECK_UNDER,
                        STEP.SAVE_MAPPING_FILE_UNDER,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processMappingUNDERList;
                    if (isPauseCal) Step = StepList[StepIndex = 3];
                    else Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_RECAL_UNDER_POINTS:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    Vision.SetHeadTarget("UNDER", calibToolType);

                    processMappingUNDERList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.MOVE_Z_UNDER_CAL_POS,
                        STEP.REQUEST_UNDER_CAL_PIXEL,
                        STEP.IF_RECAL_COMPLETE_UNDER,
                        STEP.IDLE,
                    };

                    StepList = processMappingUNDERList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_RECAL_POINT_XY_UNDER:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;

                    processMappingUNDERList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.MOVE_Z_UNDER_CAL_POS,
                        STEP.REQUEST_UNDER_CAL_PIXEL,
                        STEP.IDLE,
                    };

                    StepList = processMappingUNDERList;
                    Step = StepList[StepIndex = 0];
                    break;
                    #endregion
            }
        }

        protected override void OnProcessing()
        {
            bool isUseSaturation = Machine.param.Option(ParameterDefine.OPTION.USE_XY_SATURATION) == 1 ? true : false;
            uint ret1 = 0;

            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    AutoStep = AUTOSTEP.ERROR;
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.STOP:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.NEXT_STEP:
                    NextStep();
                    break;
                case STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_VACON, ref ret1);
                        Machine.Parts[(int)UNITPART.ASSEMBLER1].exist = Convert.ToBoolean(ret1);
                        if (ret1 == 0)
                            Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 0);

                        Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_VACON, ref ret1);
                        Machine.Parts[(int)UNITPART.ASSEMBLER2].exist = Convert.ToBoolean(ret1);
                        if (ret1 == 0)
                            Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 0);
                    }

                    NextStep();
                    break;
                //case STEP.INF_PICKER_ON:
                //    Machine.interfer_assembler_loading = true;
                //    NextStep();
                //    break;

                //case STEP.INF_PICKER_OFF:
                //    Machine.interfer_assembler_loading = false;
                //    NextStep();
                //    break;

                case STEP.INF_BUFFER_ON:
                    Machine.interfer_assembler_on_buffer[targetCurr] = true;
                    NextStep();
                    break;

                case STEP.INF_BUFFER_OFF:
                    Machine.interfer_assembler_on_buffer[targetCurr] = false;
                    NextStep();
                    break;

                case STEP.INF_JIG_ON:
                    Machine.interfer_jig_align = true;
                    NextStep();
                    break;

                case STEP.INF_JIG_OFF:
                    Machine.interfer_jig_align = false;
                    NextStep();
                    break;

                case STEP.PROC_ALIGN_START:
                    receivedData = false;

                    if (Machine.status.mode == Define.SystemMode.SystemModeDRYRUN)
                    {
                        alignSucs[0] = 0;
                        alignSucs[1] = 0;
                        NextStep();
                    }
                    else
                    {
                        if (!Vision.RunGrab("UNDER", false, true))
                            break;
                        bool saveTrainImage = Machine.param.Option(ParameterDefine.OPTION.USE_IMAGE_GATHERING_FOR_EL) == 1 ? true : false;
                        if (!Vision.Run("UNDER", true, false, saveTrainImage, 300))
                            break;
                        Step = STEP.PROC_ALIGN_COMPL_CHECK;
                        timeWait[(int)TIMER.TIMEOUT].Reset();
                    }
                    break;

                case STEP.PROC_ALIGN_COMPL_CHECK:
                    if (CheckStopBit())
                        break;

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VISION_TIME_OUT))
                    {
                        if (vision_fail_count >= 5)
                        {
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.RESULT, false);
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.FAIL, true);
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.CLASSIFY_NG, true);
                            if (targetCurr == 0 && alignSucs[0] < 0)
                            {
                                Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IDLE)];
                                AutoStep = AUTOSTEP.DISCARD_LEFT;
                                break;
                            }

                            if (targetCurr == 1 && alignSucs[1] < 0)
                            {
                                Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IDLE)];
                                AutoStep = AUTOSTEP.DISCARD_LEFT;
                                break;
                            }
                        }

                        vision_fail_count++;
                        Step = STEP.PROC_ALIGN_START;
                        break;
                    }
                    if (!receivedData)
                        break;

                    if (vision_fail_count < 5)
                    {
                        if (targetCurr == 0 && alignSucs[0] < 0)
                        {
                            vision_fail_count++;
                            Step = STEP.PROC_ALIGN_START;
                            break;
                        }
                        if (targetCurr == 1 && alignSucs[1] < 0)
                        {
                            vision_fail_count++;
                            Step = STEP.PROC_ALIGN_START;
                            break;
                        }
                    }
                    else
                    {
                        Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.RESULT, false);
                        Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.FAIL, true);
                        Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.CLASSIFY_NG, true);
                        if (targetCurr == 0 && alignSucs[0] < 0)
                        {
                            Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IDLE)];
                            AutoStep = AUTOSTEP.DISCARD_LEFT;
                            break;
                        }
                        if (targetCurr == 1 && alignSucs[1] < 0)
                        {
                            Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IDLE)];
                            AutoStep = AUTOSTEP.DISCARD_LEFT;
                            break;
                        }

                        break;
                    }

                    NextStep();
                    break;

                case STEP.PROC_ALIGN_ERROR_CHECK:

                    int maxCheckCount = (int)Machine.param.Option(ParameterDefine.OPTION.ALIGNMENT_ERROR_CHECK_COUNT);
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    if (CheckStopBit())
                        break;
                    if (underErrorCheckCount >= maxCheckCount)
                    {
                        if (underErrorFinalCheckCount > 3)
                        {
                            ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"Assembler Under Vision Error Check Count: {underErrorCheckCount}");
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.RESULT, false);
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.RETRY_FAIL, true);
                            Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IDLE)];
                            AutoStep = AUTOSTEP.DISCARD_LEFT;
                        }
                        else
                        {
                            adjustPos[targetCurr].x = 0;
                            adjustPos[targetCurr].y = 0;
                            adjustAngle[targetCurr] = 0;
                            underErrorCheckCount = 0;
                            underErrorFinalCheckCount++;
                            Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_Z_UNDER_ALIGN_POS_OVERLAP)];
                        }
                        break;
                    }
                    Point2d camCenter = Vision.GetCamCenter("UNDER");
                    double gapXpx = camCenter.x - productPosition[targetCurr].x;
                    double gapYpx = camCenter.y - productPosition[targetCurr].y;

                    double targetAngle = Machine.param.Option(ParameterDefine.OPTION.UNDER_VISION_TARGET_ANGLE_RANGE);
                    if (Math.Abs(productAngle[targetCurr]) > targetAngle
                        || (Math.Abs(gapXpx) > 100 && isUseSaturation)
                        || (Math.Abs(gapYpx) > 100 && isUseSaturation))
                    {
                        adjustAngle[targetCurr] += productAngle[targetCurr];
                        if (adjustAngle[targetCurr] >= 185) adjustAngle[targetCurr] -= 360;
                        else if (adjustAngle[targetCurr] <= -185) adjustAngle[targetCurr] += 360;

                        if (isUseSaturation)
                        {
                            (double gapXmm, double gapYmm) = Vision.ConvertPxToMmByDistance(CAMERA.UNDER, (TOOL_TYPE)targetCurr, gapXpx, gapYpx);

                            adjustPos[targetCurr].x += gapXmm;
                            adjustPos[targetCurr].y += gapYmm;
                        }
                        Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_Z_UNDER_ALIGN_POS_OVERLAP)];
                        this.underErrorCheckCount++;
                    }
                    else
                    {
                        ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"Assembler Under Vision Error Check Count: {underErrorCheckCount}");
                        if (targetCurr == 0)
                        {
                            double apos = 0;
                            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_R1, ref apos);
                            adjustAngle[targetCurr] = apos / 1000;
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.UNDER_LEFT, true);

                        }
                        if (targetCurr == 1)
                        {
                            double apos = 0;
                            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_R2, ref apos);
                            adjustAngle[targetCurr] = apos / 1000;
                            Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.UNDER_RIGHT, true);

                        }

                        Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.RESULT, false);
                        Vision.Capture("UNDER", CaptureUtil.CAPTURETYPE.CLASSIFY_OK, true);
                        NextStep();
                    }
                    break;
            }

            OnProcessOfAutoRun();
            OnProcessOfTime();
            OnProcessOfMotion();
            OnProcessOfIO();
            OnProcessOfInterface();
            OnProcessOfCalibration();

            if (Machine.param.Option(ParameterDefine.OPTION.USE_PROC_LOG) == 1)
            {
                if ((int)AutoStep != lastAutoStep)
                {
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] {Enum.GetName(typeof(AUTOSTEP), AutoStep)}");
                    lastAutoStep = (int)AutoStep;
                }
                if ((int)Step != lastProcStep)
                {
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] {Enum.GetName(typeof(STEP), Step)}");
                    lastProcStep = (int)Step;
                }
            }
        }

        private void OnProcessOfTime()
        {
            switch ((STEP)Step)
            {
                case STEP.VAC_ON_WAIT:
                    timeWait[(int)TIMER.DELAY].Reset();
                    Step = STEP.VAC_ON_WAIT_CHECK;
                    break;

                case STEP.VAC_ON_WAIT_CHECK:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_WAIT_TIME))
                        NextStep();
                    break;

                case STEP.PURGE_OFF_WAIT:
                    timeWait[(int)TIMER.DELAY].Reset();
                    Step = STEP.PURGE_OFF_WAIT_CHECK;
                    break;

                case STEP.PURGE_OFF_WAIT_CHECK:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME))
                        NextStep();
                    break;
            }
        }

        private void OnProcessOfAutoRun()
        {
            uint ret1 = 0;
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;
            bool isManualMode = Machine.status.state == SystemState.SystemStateIDLE;
            bool req1 = false;
            bool req2 = false;

            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
                    if (CheckStopBit())
                        stopBit = false;
                    break;

                case AUTOSTEP.STOP:
                    break;

                case AUTOSTEP.COMPLETE:
                    if (Machine.sysMode != Machine.SYSMODE.AUTO)
                        AutoStep = AUTOSTEP.IDLE;
                    else
                        AutoStep = AUTOSTEP.LOADING_CHECK;

                    CheckStopBit();
                    break;

                case AUTOSTEP.INIT:
                    Machine.Parts[(int)UNITPART.ASSEMBLER1].ClearInterface();
                    Machine.Parts[(int)UNITPART.ASSEMBLER2].ClearInterface();

                    initCompl = false;
                    stopBit = false;

                    Machine.interfer_release_y_crash = true;

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        initCompl = true;
                        AutoStep = AUTOSTEP.IDLE;
                    }

                    SetMessage((int)MSG.MSG_PROCESS_INIT);
                    AutoStep = AUTOSTEP.INIT_CHECK;
                    break;

                case AUTOSTEP.INIT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    initCompl = true;
                    Machine.interfer_release_y_crash = false;
                    //Machine.interfer_assembler_loading = false;
                    first_alignment[0] = true;
                    first_alignment[1] = true;

                    adjustPos[0].x = 0;
                    adjustPos[0].y = 0;
                    adjustAngle[0] = 0;
                    adjustPos[1].x = 0;
                    adjustPos[1].y = 0;
                    adjustAngle[1] = 0;

                    AutoStep = AUTOSTEP.OTHER_PROCESS_INIT_WAIT;
                    break;

                case AUTOSTEP.OTHER_PROCESS_INIT_WAIT:
                    if (CheckStopBit())
                        break;
                    if (Machine.GetProcInitialized() || Machine.sysMode != Machine.SYSMODE.AUTO)
                        AutoStep = AUTOSTEP.PROC_SELECT_FOR_RESTART;
                    break;

                case AUTOSTEP.PROC_SELECT_FOR_RESTART:
                    if (CheckStopBit())
                        break;

                    // 피커가 모두 차 있을 경우에만 언로딩 한다.
                    if (!Machine.Parts[(int)UNITPART.ASSEMBLER1].exist && !Machine.Parts[(int)UNITPART.ASSEMBLER2].exist)
                    {
                        AutoStep = AUTOSTEP.LOADING_CHECK;
                    }
                    else
                        AutoStep = AUTOSTEP.ALIGNMENT;
                    break;

                case AUTOSTEP.LOADING_CHECK:
                    if (CheckStopBit())
                        break;
                    targetCurr = 0;

                    if (Machine.buffer_ready_to_pick[(int)TOOL_TYPE.LEFT] || Machine.buffer_ready_to_pick[(int)TOOL_TYPE.RIGHT])
                    {
                        if (Machine.buffer_ready_to_pick[(int)TOOL_TYPE.LEFT])
                        {
                            if (!Machine.Parts[(int)UNITPART.BUF2_L1].exist && !Machine.Parts[(int)UNITPART.ASSEMBLER1].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF2_L1_DETECT); return;
                            }
                            if (!Machine.Parts[(int)UNITPART.BUF2_L2].exist && !Machine.Parts[(int)UNITPART.ASSEMBLER2].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF2_L2_DETECT); return;
                            }
                            targetCurr = 0;
                        }
                        if (Machine.buffer_ready_to_pick[(int)TOOL_TYPE.RIGHT])
                        {
                            if (!Machine.Parts[(int)UNITPART.BUF2_R1].exist && !Machine.Parts[(int)UNITPART.ASSEMBLER1].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF2_R1_DETECT); return;
                            }
                            if (!Machine.Parts[(int)UNITPART.BUF2_R2].exist && !Machine.Parts[(int)UNITPART.ASSEMBLER2].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF2_R2_DETECT); return;
                            }
                            targetCurr = 1;
                        }
                        if (Machine.buffer_ready_to_pick[(int)TOOL_TYPE.LEFT] && Machine.buffer_ready_to_pick[(int)TOOL_TYPE.RIGHT])
                        {
                            if (lastPickedSide != TOOL_TYPE.LEFT)
                            {
                                targetCurr = 0;
                                lastPickedSide = TOOL_TYPE.LEFT;
                            }
                            else if (lastPickedSide != TOOL_TYPE.RIGHT)
                            {
                                targetCurr = 1;
                                lastPickedSide = TOOL_TYPE.RIGHT;
                            }
                        }
                        Machine.assembler_start_pick[targetCurr] = true;
                        AutoStep = AUTOSTEP.LOADING;
                        break;
                    }
                    else
                    {
                        req1 = Machine.Parts[(int)UNITPART.BUF2_L1].exist;
                        req2 = Machine.Parts[(int)UNITPART.BUF2_R1].exist;

                        if (req1) targetCurr = 0;
                        if (req2) targetCurr = 1;

                        AutoStep = AUTOSTEP.LOADING_APPROACH;
                        break;
                    }

                    AutoStep = AUTOSTEP.LOADING;
                    break;

                case AUTOSTEP.LOADING:
                    if (CheckStopBit())
                        break;

                    //if (Machine.interfer_prod_loader_unloading)
                    //{
                    //    AutoStep = AUTOSTEP.LOADING_APPROACH;
                    //    break;
                    //}
                    //Machine.interfer_assembler_loading = true;

                    last_pickup_buffer_index = targetCurr;

                    SetMessage((int)MSG.MSG_LOADING_PRODUCT);

                    AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.assembler_start_pick[targetCurr] = false;
                    Machine.buffer_ready_to_pick[targetCurr] = false;
                    AutoStep = AUTOSTEP.ALIGNMENT;
                    break;

                case AUTOSTEP.LOADING_APPROACH:
                    SetMessage((int)MSG.MSG_MOVE_PICKUP_POS);
                    AutoStep = AUTOSTEP.LOADING_APPROACH_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_APPROACH_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    //Machine.interfer_assembler_loading = false;
                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.ALIGNMENT:
                    if (CheckStopBit())
                        break;

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        AutoStep = AUTOSTEP.IDLE;
                        break;
                    }

                    if (Machine.Parts[(int)UNITPART.ASSEMBLER1].exist || isManualMode)
                        SetMessage((int)MSG.MSG_ALIGN_PRODUCT);

                    AutoStep = AUTOSTEP.ALIGNMENT_COMPL_CHECK;
                    break;

                case AUTOSTEP.ALIGNMENT_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    productPositionOLD[0].x = productPosition[0].x;
                    productPositionOLD[0].y = productPosition[0].y;
                    productAngleOLD[0] = productAngle[0];

                    AutoStep = AUTOSTEP.ALIGNMENT2;
                    break;

                case AUTOSTEP.ALIGNMENT2:
                    if (Machine.Parts[(int)UNITPART.ASSEMBLER2].exist || isManualMode)
                        SetMessage((int)MSG.MSG_ALIGN2_PRODUCT);
                    AutoStep = AUTOSTEP.ALIGNMENT2_COMPL_CHECK;
                    break;

                case AUTOSTEP.ALIGNMENT2_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    productPositionOLD[1].x = productPosition[1].x;
                    productPositionOLD[1].y = productPosition[1].y;
                    productAngleOLD[1] = productAngle[1];

                    AutoStep = AUTOSTEP.UNLOADING_CHECK;
                    break;

                case AUTOSTEP.DISCARD_LEFT:
                    int maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
                    int maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
                    int maxPartIn_X_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
                    int maxPartIn_Y_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);
                    if (currentNGCount[(int)TOOL_TYPE.LEFT] >= maxPartIn_X_Left*maxPartIn_Y_Left )
                    {
                        currentNGCountInX[0] = 0;
                        currentNGCountInX[1] = 0;
                        currentNGCountInY[0] = 0;
                        currentNGCountInY[1] = 0;

                        currentNGCount[0] = 0;
                        currentNGCount[1] = 0;
                        int maxCount = maxPartIn_X_Left * maxPartIn_Y_Left + maxPartIn_X_Right * maxPartIn_Y_Right;
                        EDM.SetNGBoxStatus(0, maxCount, false);
                        SetError(ECODE.NG_BOX_FULL);
                        break;
                    }
                    targetCurr = 0;
                    SetMessage((int)MSG.MSG_DISCARD_NG_PRODUCT);

                    AutoStep = AUTOSTEP.DISCARD_LEFT_CHECK;
                    break;

                case AUTOSTEP.DISCARD_LEFT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.Parts[(int)UNITPART.ASSEMBLER1].exist = false;

                    AutoStep = AUTOSTEP.DISCARD_RIGHT;
                    break;

                case AUTOSTEP.DISCARD_RIGHT:

                    maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
                    maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
                    maxPartIn_X_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
                    maxPartIn_Y_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);

                    if (currentNGCount[(int)TOOL_TYPE.RIGHT] >= maxPartIn_X_Right* maxPartIn_Y_Right)
                    {
                        currentNGCountInX[0] = 0;
                        currentNGCountInX[1] = 0;
                        currentNGCountInY[0] = 0;
                        currentNGCountInY[1] = 0;

                        currentNGCount[0] = 0;
                        currentNGCount[1] = 0;
                        int maxCount = maxPartIn_X_Left * maxPartIn_Y_Left + maxPartIn_X_Right * maxPartIn_Y_Right;
                        EDM.SetNGBoxStatus(0, maxCount, false);
                        SetError(ECODE.NG_BOX_FULL);
                        break;
                    }
                    targetCurr = 1;
                    SetMessage((int)MSG.MSG_DISCARD_NG_PRODUCT);

                    AutoStep = AUTOSTEP.DISCARD_RIGHT_CHECK;
                    break;

                case AUTOSTEP.DISCARD_RIGHT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.Parts[(int)UNITPART.ASSEMBLER2].exist = false;

                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_CHECK:
                    if (CheckStopBit())
                        break;

                    if (!Machine.Jigs[(int)UNITJIG.JIG_WORK].exist)
                        break;

                    targetCurr = 0;
                    if (Machine.Parts[(int)UNITPART.ASSEMBLER1].exist) targetCurr = 0;
                    else if (Machine.Parts[(int)UNITPART.ASSEMBLER2].exist) targetCurr = 1;
                    else
                    {
                        AutoStep = AUTOSTEP.LOADING_CHECK;
                        break;
                    }

                    if (!proc.doneAlignment) break;

                    int assemblerPartCount = 0;
                    int jigPartCount = 0;
                    assemblerPartCount += (Machine.Parts[(int)UNITPART.ASSEMBLER1].exist ? 1 : 0);
                    assemblerPartCount += (Machine.Parts[(int)UNITPART.ASSEMBLER2].exist ? 1 : 0);
                    jigPartCount += (Machine.Parts[(int)UNITPART.JIG1].exist ? 1 : 0);
                    jigPartCount += (Machine.Parts[(int)UNITPART.JIG2].exist ? 1 : 0);

                    if (Math.Abs(assemblerPartCount - jigPartCount) == 1)
                    {
                        SetError(ECODE.ALIGN_ERROR_DETECT_LEFT_JIG_NOT_OK_TO_ASSEMBLE);
                        break;
                    }
                    else if (assemblerPartCount == 1 && jigPartCount == 1
                        && (Machine.Parts[(int)UNITPART.ASSEMBLER1].exist == Machine.Parts[(int)UNITPART.JIG1].exist)
                        && (Machine.Parts[(int)UNITPART.ASSEMBLER2].exist == Machine.Parts[(int)UNITPART.JIG2].exist))
                    {
                        SetError(ECODE.ALIGN_ERROR_DETECT_LEFT_JIG_NOT_OK_TO_ASSEMBLE);
                        break;
                    }

                    if (targetCurr == 0 &&
                        (Machine.Parts[(int)UNITPART.JIG1].exist
                        || proc.jigCLResult[(int)TOOL_TYPE.LEFT] != JIG_CLASSIFY_RESULT.OK)) break;
                    if (targetCurr == 1 &&
                        (Machine.Parts[(int)UNITPART.JIG2].exist
                        || proc.jigCLResult[(int)TOOL_TYPE.RIGHT] != JIG_CLASSIFY_RESULT.OK)) break;

                    AutoStep = AUTOSTEP.UNLOADING;
                    break;

                case AUTOSTEP.UNLOADING:
                    if (CheckStopBit())
                        break;

                    SetMessage((int)MSG.MSG_UNLOADING_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.UNLOADING_CHECK;
                    break;
            }

            if (AutoStep != AUTOSTEP.IDLE && AutoStep != AUTOSTEP.ERROR && AutoStep != AUTOSTEP.STOP)
                lastAutoStep = (int)AutoStep;
        }

        private void OnProcessOfInterface()
        {
            switch ((STEP)Step)
            {
                case STEP.IF_LOADING_POSSIBLE_WAIT:
                    NextStep();
                    break;

                case STEP.IF_LOADING_BUSY:
                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (CheckStopBit())
                        break;

                    if (targetCurr == 0)
                    {
                        Machine.Parts[(int)UNITPART.BUF2_L1].exist = false;
                        Machine.Parts[(int)UNITPART.BUF2_L2].exist = false;
                    }
                    else if (targetCurr == 1)
                    {
                        Machine.Parts[(int)UNITPART.BUF2_R1].exist = false;
                        Machine.Parts[(int)UNITPART.BUF2_R2].exist = false;
                    }
                    Machine.Parts[(int)UNITPART.ASSEMBLER1].exist = true;
                    Machine.Parts[(int)UNITPART.ASSEMBLER2].exist = true;
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_REQUEST:
                    resumeStepIndex = StepIndex;
                    Machine.Parts[current[targetCurr]].unloadingRequest = true;

                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_UNLOADING_BUSY:
                    Machine.Parts[current[targetCurr]].unloading = true;
                    if (!Machine.Parts[downstream[targetCurr]].loading)
                        break;
                    Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.ASSEMBLING_DELAY_TIME));
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:
                    Machine.Parts[current[targetCurr]].unloadingRequest = false;
                    Machine.Parts[current[targetCurr]].unloading = false;

                    if (Machine.Parts[downstream[targetCurr]].loading)
                        break;

                    Machine.Parts[current[targetCurr]].exist = false;
                    NextStep();
                    resumeStepIndex = StepIndex;
                    break;

                case STEP.SENDING_PRODUCT_DATA_TO_JIG:
                    Machine.Parts[current[targetCurr]].exist = false;
                    Machine.Parts[downstream[targetCurr]].exist = true;
                    NextStep();
                    break;

                case STEP.IF_INCREASE_NG_COUNT:
                    int maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
                    int maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
                    int maxPartIn_X_Right= (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
                    int maxPartIn_Y_Right= (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);

                    currentNGCountInX[targetCurr]++;
                    currentNGCount[targetCurr]++;
                    if(targetCurr == 0)
                    {
                        if (currentNGCountInX[targetCurr] >= maxPartIn_X_Left)
                        {
                            currentNGCountInY[targetCurr]++;
                            currentNGCountInX[targetCurr] = 0;
                        }
                    }
                    else
                    {
                        if (currentNGCountInX[targetCurr] >= maxPartIn_X_Right)
                        {
                            currentNGCountInY[targetCurr]++;
                            currentNGCountInX[targetCurr] = 0;
                        }
                    }
                    
                    EDM.UnloadingSetsNG("Vision Error", false);
                    int currentCount = currentNGCount[0] + currentNGCount[1];
                    int maxCount = maxPartIn_X_Left * maxPartIn_Y_Left + maxPartIn_X_Right*maxPartIn_Y_Right;
                    EDM.SetNGBoxStatus(currentCount, maxCount, false);
                    NextStep();
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            double apos = 0;

            switch ((STEP)Step)
            {
                case STEP.MOVE_XYR_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_X))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_Y))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_R1))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_R2))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_XY_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_X))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_Y))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_Z_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.ASSEMBLER_Z))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_XYR_READY_POS:
                    resumeStepIndex = StepIndex;

                    MovePickerXYR(0, XYRPOS.READY);
                    Step = STEP.MOVE_XYR_POS_CHECK;
                    break;

                case STEP.MOVE_XYR_SCAN_POS:
                case STEP.MOVE_XYR_SCAN_POS_OVERLAP:
                    if (targetCurr == 0) MovePickerXYR(0, XYRPOS.SCAN1);
                    if (targetCurr == 1) MovePickerXYR(0, XYRPOS.SCAN2);

                    if (Step == STEP.MOVE_XYR_SCAN_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_XYR_POS_CHECK;
                    else if (Step == STEP.MOVE_XYR_SCAN_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_XYR_PICKUP_POS:
                case STEP.MOVE_XYR_PICKUP_POS_OVERLAP:
                    resumeStepIndex = StepIndex;

                    //Machine.interfer_assembler_loading = true;

                    if (targetCurr == 0) MovePickerXYR(0, XYRPOS.BUF1);
                    if (targetCurr == 1) MovePickerXYR(0, XYRPOS.BUF2);

                    if (Step == STEP.MOVE_XYR_PICKUP_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_XYR_POS_CHECK;
                    else if (Step == STEP.MOVE_XYR_PICKUP_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_XYR_PICKUP_READY_POS:
                    resumeStepIndex = StepIndex;

                    if (targetCurr == 0) MovePickerXYR(0, XYRPOS.BUF1READY);
                    if (targetCurr == 1) MovePickerXYR(0, XYRPOS.BUF2READY);
                    Step = STEP.MOVE_XYR_POS_CHECK;
                    break;

                case STEP.MOVE_XYR_ALIGN_POS:
                case STEP.MOVE_XYR_ALIGN_POS_READY:
                    resumeStepIndex = StepIndex;

                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Z, ref apos);
                    if (apos > 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_UNDER_ALIGN_POS) + 25))
                    {
                        if (Machine.sysMode == Machine.SYSMODE.AUTO)
                            break;
                        //else
                        //    Machine.Alarm(ECODE.POSITION_ERROR_ASSYPICKER_PICKUP_POS);
                    }

                    if (targetCurr == 0 || Step == STEP.MOVE_XYR_ALIGN_POS_READY) MovePickerXYR(0, XYRPOS.ALIGN1);
                    else if (targetCurr == 1) MovePickerXYR(0, XYRPOS.ALIGN2);
                    Step = STEP.MOVE_XYR_POS_CHECK;
                    break;

                case STEP.MOVE_XYR_PLACE_POS:
                case STEP.MOVE_XYR_PLACE_POS_OVERLAP:
                    resumeStepIndex = StepIndex;

                    if (targetCurr == 0) MovePickerXYR(0, XYRPOS.PLACE1);
                    if (targetCurr == 1) MovePickerXYR(0, XYRPOS.PLACE2);

                    if (Step == STEP.MOVE_XYR_PLACE_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_XYR_POS_CHECK;
                    else if (Step == STEP.MOVE_XYR_PLACE_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_XYR_TRASH_POS:
                    resumeStepIndex = StepIndex;

                    if (targetCurr == 0) MovePickerXYR(0, XYRPOS.TRASH1);
                    else if (targetCurr == 1) MovePickerXYR(0, XYRPOS.TRASH2);
                    Step = STEP.MOVE_XYR_POS_CHECK;
                    break;

                case STEP.MOVE_Z_READY_POS:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.READY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_SCAN_POS:
                case STEP.MOVE_Z_SCAN_POS_OVERLAP:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.SCAN);
                    if (Step == STEP.MOVE_Z_SCAN_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_Z_POS_CHECK;
                    else if (Step == STEP.MOVE_Z_SCAN_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_Z_PICKUP_READY_POS:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.PICKREADY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PICKUP_UP_READY_POS_FAST:
                case STEP.MOVE_Z_PICKUP_UP_READY_POS_FAST_OVERLAP:
                    resumeStepIndex = StepIndex;

                    if (Machine.interfer_assembler_on_buffer[targetCurr])
                    {
                        if (Machine.sysMode != Machine.SYSMODE.AUTO)
                            SetError(ECODE.INTERFERENCE_CAM_TRF);
                        break;
                    }

                    MovePickerZ(ZPOS.PICKUPREADY_FAST);

                    if (Step == STEP.MOVE_Z_PICKUP_UP_READY_POS_FAST || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_Z_POS_CHECK;
                    else if (Step == STEP.MOVE_Z_PICKUP_UP_READY_POS_FAST_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_Z_PLACE_READY_POS:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.PLACEREADY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PLACE_UP_READY_POS:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.PLACEUPREADY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PLACE_UP_READY_POS_FAST:
                case STEP.MOVE_Z_PLACE_UP_READY_POS_FAST_OVERLAP:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.PLACEUPREADY_FAST);
                    if (Step == STEP.MOVE_Z_PLACE_UP_READY_POS_FAST || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_Z_POS_CHECK;
                    else if (Step == STEP.MOVE_Z_PLACE_UP_READY_POS_FAST_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_Z_PICKUP_POS:
                    MovePickerZ(ZPOS.PICK);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_UNDER_ALIGN_POS:
                case STEP.MOVE_Z_UNDER_ALIGN_POS_OVERLAP:
                    resumeStepIndex = StepIndex;

                    MovePickerZ(ZPOS.UNDERALIGN);
                    if (Step == STEP.MOVE_Z_UNDER_ALIGN_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_Z_POS_CHECK;
                    else if (Step == STEP.MOVE_Z_UNDER_ALIGN_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_Z_PLACE_POS:
                    if (targetCurr == 0) MovePickerZ(ZPOS.PLACE1);
                    if (targetCurr == 1) MovePickerZ(ZPOS.PLACE2);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_DISCARD_POS:
                    MovePickerZ(ZPOS.DISCARD);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                #region MOVE CALIBRATION POS
                case STEP.MOVE_Z_UNDER_CAL_POS:
                    MovePickerZ(ZPOS.UNDERCAL);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_JIG_CAL_POS:
                    MovePickerZ(ZPOS.JIGCAL);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;
                    #endregion
            }
        }

        private void OnProcessOfIO()
        {
            int[] outToolUp = { (int)DO.ASSEMBLER_Z1_UP, (int)DO.ASSEMBLER_Z2_UP };
            int[] outToolDown = { (int)DO.ASSEMBLER_Z1_DOWN, (int)DO.ASSEMBLER_Z2_DOWN };
            int[] outToolVac = { (int)DO.ASSEMBLER_Z1_VACON, (int)DO.ASSEMBLER_Z2_VACON };
            int[] outToolPurge = { (int)DO.ASSEMBLER_Z1_PURGE, (int)DO.ASSEMBLER_Z2_PURGE };
            int[] inToolUp = { (int)DI.ASSEMBLER_Z1_UP, (int)DI.ASSEMBLER_Z2_UP };
            int[] inToolDown = { (int)DI.ASSEMBLER_Z1_DOWN, (int)DI.ASSEMBLER_Z2_DOWN };
            int[] inToolVac = { (int)DI.ASSEMBLER_Z1_VACON, (int)DI.ASSEMBLER_Z2_VACON };

            //int[] inUpVac1 = { (int)DI.CAM_BUF_L1_VACON, (int)DI.CAM_BUF_R1_VACON };
            //int[] inUpVac2 = { (int)DI.CAM_BUF_L2_VACON, (int)DI.CAM_BUF_R2_VACON };
            //int[] outUpVac1 = { (int)DO.CAM_BUF_L1_VACON, (int)DO.CAM_BUF_R1_VACON };
            //int[] outUpVac2 = { (int)DO.CAM_BUF_L2_VACON, (int)DO.CAM_BUF_R2_VACON };
            //int[] outUpPurge1 = { (int)DO.CAM_BUF_L1_PURGE, (int)DO.CAM_BUF_R1_PURGE };
            //int[] outUpPurge2 = { (int)DO.CAM_BUF_L2_PURGE, (int)DO.CAM_BUF_R2_PURGE };

            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            switch ((STEP)Step)
            {
                case STEP.INIT_VACOFF:
                    bool isInPlaceProcess = true;

                    double currentPos = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Z, ref currentPos);
                    double placePosZLeft = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS);
                    double placePosZRight = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_R_POS);
                    bool isZinPlacePos = (Math.Abs(currentPos / 1000 - placePosZLeft) <= 0.2) || (Math.Abs(currentPos / 1000 - placePosZRight) <= 0.2);
                    isInPlaceProcess &= isZinPlacePos;

                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref currentPos);
                    double alignPosY = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN_POS);
                    isInPlaceProcess &= (currentPos / 1000 < alignPosY);

                    uint cyl1 = 0, cyl2 = 0;
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_DOWN, ref cyl1);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_DOWN, ref cyl2);
                    if (cyl1 == 1 && isInPlaceProcess)
                    {
                        Machine.IO.SetOut(outToolVac[0], 0);
                        Machine.IO.SetOut(outToolPurge[0], 1);
                    }
                    if (cyl2 == 1 && isInPlaceProcess)
                    {
                        Machine.IO.SetOut(outToolVac[1], 0);
                        Machine.IO.SetOut(outToolPurge[1], 1);
                    }
                    Util.Delay(500);
                    Machine.IO.SetOut(outToolPurge[0], 0);
                    Machine.IO.SetOut(outToolPurge[1], 0);
                    NextStep();
                    break;

                case STEP.PICKER_UP:
                case STEP.PICKER_UP_BYPASS:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut(outToolUp[targetCurr], 1);
                    Machine.IO.SetOut(outToolDown[targetCurr], 0);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_UP) Step = STEP.PICKER_UP_CHECK;
                    if (Step == STEP.PICKER_UP_BYPASS) NextStep();
                    break;

                case STEP.PICKER_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (targetCurr == 0) SetError(ECODE.TIMEOUT_ASSEMBLER1_UP);
                        if (targetCurr == 1) SetError(ECODE.TIMEOUT_ASSEMBLER2_UP);
                        break;
                    }

                    Machine.IO.GetIn(inToolUp[targetCurr], ref ret1);
                    Machine.IO.GetIn(inToolDown[targetCurr], ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.PICKER_UP_ALL:
                case STEP.PICKER_UP_ALL_BYPASS:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_UP_ALL) Step = STEP.PICKER_UP_ALL_CHECK;
                    if (Step == STEP.PICKER_UP_ALL_BYPASS) NextStep();
                    break;

                case STEP.PICKER_UP_ALL_CHECK:
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_DOWN, ref ret2);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref ret3);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_DOWN, ref ret4);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (ret1 != 1 || ret2 != 0)
                        {
                            SetError(ECODE.TIMEOUT_ASSEMBLER1_UP);
                            break;

                        }
                        else if (ret3 != 1 || ret4 != 0)
                        {
                            SetError(ECODE.TIMEOUT_ASSEMBLER2_UP);
                            break;
                        }
                    }

                    if (ret1 == 1 && ret2 == 0 && ret3 == 1 && ret4 == 0)
                        NextStep();
                    break;

                case STEP.PICKER_DOWN:
                case STEP.PICKER_DOWN_BYPASS:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut(outToolUp[targetCurr], 0);
                    Machine.IO.SetOut(outToolDown[targetCurr], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_DOWN) Step = STEP.PICKER_DOWN_CHECK;
                    if (Step == STEP.PICKER_DOWN_BYPASS) NextStep();
                    break;

                case STEP.PICKER_DOWN_CHECK:
                    Machine.IO.GetIn(inToolUp[targetCurr], ref ret1);
                    Machine.IO.GetIn(inToolDown[targetCurr], ref ret2);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (targetCurr == 0) SetError(ECODE.TIMEOUT_ASSEMBLER1_DOWN);
                        if (targetCurr == 1) SetError(ECODE.TIMEOUT_ASSEMBLER2_DOWN);
                        break;
                    }

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_DOWN_ALL:
                case STEP.PICKER_DOWN_ALL_BYPASS:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_DOWN_ALL) Step = STEP.PICKER_DOWN_ALL_CHECK;
                    if (Step == STEP.PICKER_DOWN_ALL_BYPASS) NextStep();
                    break;

                case STEP.PICKER_DOWN_ALL_CHECK:
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_DOWN, ref ret2);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref ret3);
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_DOWN, ref ret4);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (ret1 != 0 || ret2 != 1)
                        {
                            SetError(ECODE.TIMEOUT_ASSEMBLER1_DOWN);
                            break;

                        }
                        else if (ret3 != 0 || ret4 != 1)
                        {
                            SetError(ECODE.TIMEOUT_ASSEMBLER2_DOWN);
                            break;
                        }
                    }

                    if (ret1 == 0 && ret2 == 1 && ret3 == 0 && ret4 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_VACON:
                    resumeStepIndex = StepIndex;

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut(outToolVac[targetCurr], 1);
                    Machine.IO.SetOut(outToolPurge[targetCurr], 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACON_CHECK;
                    break;

                case STEP.PICKER_VACON_CHECK:
                    Machine.IO.GetIn(inToolVac[targetCurr], ref ret1);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        if (targetCurr == 0)
                            Machine.Alarm(ECODE.TIMEOUT_ASSEMBLER1_VACON);
                        if (targetCurr == 1)
                            Machine.Alarm(ECODE.TIMEOUT_ASSEMBLER2_VACON);
                    }

                    if (ret1 == 1)
                        Step = STEP.VAC_ON_WAIT;

                    break;

                case STEP.PICKER_VACON_ALL:
                    resumeStepIndex = StepIndex;

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut(outToolVac[0], 1);
                    Machine.IO.SetOut(outToolPurge[0], 0);
                    Machine.IO.SetOut(outToolVac[1], 1);
                    Machine.IO.SetOut(outToolPurge[1], 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACON_ALL_CHECK;
                    break;

                case STEP.PICKER_VACON_ALL_CHECK:
                    Machine.IO.GetIn(inToolVac[0], ref ret1);
                    Machine.IO.GetIn(inToolVac[1], ref ret2);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT) && Machine.sysMode == Machine.SYSMODE.AUTO)
                    {
                        if (ret1 == 0) SetError(ECODE.TIMEOUT_ASSEMBLER1_VACON);
                        if (ret2 == 0) SetError(ECODE.TIMEOUT_ASSEMBLER2_VACON);
                        break;
                    }

                    if (ret1 == 1 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_VACOFF:
                case STEP.PICKER_VACOFF_BYPASS:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut(outToolVac[targetCurr], 0);
                    Machine.IO.SetOut(outToolPurge[targetCurr], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    if (Step == STEP.PICKER_VACOFF) Step = STEP.PICKER_VACOFF_CHECK;
                    if (Step == STEP.PICKER_VACOFF_BYPASS) NextStep();
                    break;

                case STEP.PICKER_PURGEOFF:
                    Machine.IO.SetOut(outToolPurge[targetCurr], 0);
                    NextStep();
                    break;

                case STEP.PICKER_VACOFF_CHECK:
                    Machine.IO.GetIn(inToolVac[targetCurr], ref ret1);

                    if (ret1 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));

                        Machine.IO.SetOut(outToolPurge[targetCurr], 0);
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACOFF_DELAY:
                    Machine.IO.GetIn(inToolVac[targetCurr], ref ret1);

                    if (ret1 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACOFF_ALL:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut(outToolVac[0], 0);
                    Machine.IO.SetOut(outToolPurge[0], 1);
                    Machine.IO.SetOut(outToolVac[1], 0);
                    Machine.IO.SetOut(outToolPurge[1], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.PICKER_VACOFF_CHECK;
                    break;

                case STEP.PICKER_VACOFF_ALL_CHECK:
                    Machine.IO.GetIn(inToolVac[0], ref ret1);
                    Machine.IO.GetIn(inToolVac[1], ref ret2);

                    if (ret1 == 0 && ret2 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));

                        Machine.IO.SetOut(outToolPurge[0], 0);
                        Machine.IO.SetOut(outToolPurge[1], 0);
                        NextStep();
                    }
                    break;

                case STEP.CAM_BUF_DETECT_ON_CHECK:
                    if (Machine.status.mode == Define.SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    if (targetCurr == 0)
                    {
                        Machine.IO.GetIn((int)DI.CAM_TRF_L1_DETECT_ON, ref ret1);
                        Machine.IO.GetIn((int)DI.CAM_TRF_L2_DETECT_ON, ref ret2);

                        if (ret1 == 1 && ret2 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                if (ret1 == 0)
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_L1_DETECT);
                                    break;
                                }
                                else
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_L2_DETECT);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Machine.IO.GetIn((int)DI.CAM_TRF_R1_DETECT_ON, ref ret1);
                        Machine.IO.GetIn((int)DI.CAM_TRF_R2_DETECT_ON, ref ret2);

                        if (ret1 == 1 && ret2 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                if (ret1 == 0)
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_R1_DETECT);
                                    break;
                                }
                                else
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_R2_DETECT);
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case STEP.CAM_BUF_DETECT_OFF_CHECK:
                    if (Machine.status.mode == Define.SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    if (targetCurr == 0)
                    {
                        Machine.IO.GetIn((int)DI.CAM_TRF_L1_DETECT_ON, ref ret1);
                        Machine.IO.GetIn((int)DI.CAM_TRF_L2_DETECT_ON, ref ret2);

                        if (ret1 == 0 && ret2 == 0)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                if (ret1 != 0)
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_L1_DETECT);
                                    break;
                                }
                                else
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_L2_DETECT);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Machine.IO.GetIn((int)DI.CAM_TRF_R1_DETECT_ON, ref ret1);
                        Machine.IO.GetIn((int)DI.CAM_TRF_R2_DETECT_ON, ref ret2);

                        if (ret1 == 0 && ret2 == 0)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                if (ret1 != 0)
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_R1_DETECT);
                                    break;
                                }
                                else
                                {
                                    SetError(ECODE.TIMEOUT_BUF2_R2_DETECT);
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void OnProcessOfCalibration()
        {
            double dActualPosX = 0; double dActualPosY = 0;
            double[] xy = new double[2];
            switch ((STEP)Step)
            {
                #region MOTION
                case STEP.GENERATE_JIG_MAP:
                    double startX = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_START_POS_X);
                    double startY = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_START_POS_Y);
                    double endX = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_END_POS_X);
                    double endY = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_END_POS_Y);
                    double step = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_STEP);
                    Vision.GenerateMoveStep(startX, startY, endX, endY, step);
                    retryCount = 0;
                    NextStep();
                    break;

                case STEP.MOVE_XY_MAPPING_JIG:
                    if (Vision.IsMapCalDone())
                    {
                        Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IF_MAPPING_DONE_CHECK_JIG)];
                        break;
                    }
                    xy[0] = Vision.GetMoveStep().X;
                    xy[1] = Vision.GetMoveStep().Y;
                    if (xy[0] == Vision.INVALID_DATA || xy[1] == Vision.INVALID_DATA)
                    {
                        SetError(ECODE.CAL_ERROR_GENERATE_MAP_ERROR);
                        break;
                    }
                    if (Vision.GetMapNIndex().index == 1) Vision.StartTTStopWatch();
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_MAPPING_JIG_START_XY:
                    xy[0] = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_START_POS_X);
                    xy[1] = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_START_POS_Y);
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_MAPPING_JIG_END_XY:
                    xy[0] = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_END_POS_X);
                    xy[1] = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_END_POS_Y);
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_RECAL_POS_XY:
                    xy[0] = this.reCalPointList[reCalIndex].Xmm;
                    xy[1] = this.reCalPointList[reCalIndex].Ymm;
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.GENERATE_UNDER_MAP:
                    if (targetCurr == 0)
                    {
                        startX = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_START_POS_X);
                        startY = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_START_POS_Y);
                        endX = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_END_POS_X);
                        endY = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_END_POS_Y);
                    }
                    else
                    {
                        startX = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_START_POS_X);
                        startY = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_START_POS_Y);
                        endX = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_END_POS_X);
                        endY = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_END_POS_Y);
                    }
                    step = Machine.param.Calibration(CALIBRATION.UNDER_CAL_MAP_STEP);
                    Vision.GenerateMoveStep(startX, startY, endX, endY, step);
                    retryCount = 0;
                    NextStep();
                    break;

                case STEP.MOVE_XY_MAPPING_UNDER:
                    if (Vision.IsMapCalDone())
                    {
                        Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IF_MAPPING_DONE_CHECK_UNDER)];
                        break;
                    }
                    double[] xyUNDER = new double[2];
                    xyUNDER[0] = Vision.GetMoveStep().X;
                    xyUNDER[1] = Vision.GetMoveStep().Y;
                    if (xy[0] == Vision.INVALID_DATA || xy[1] == Vision.INVALID_DATA)
                    {
                        SetError(ECODE.CAL_ERROR_GENERATE_MAP_ERROR);
                        break;
                    }
                    if (Vision.GetMapNIndex().index == 1) Vision.StartTTStopWatch();
                    MovePickerXYForMapping(xyUNDER);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_MAPPING_UNDER_START_XY:
                    if (targetCurr == 0)
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_START_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_START_POS_Y);
                    }
                    else
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_START_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_START_POS_Y);
                    }
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_MAPPING_UNDER_END_XY:
                    if (targetCurr == 0)
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_END_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_END_POS_Y);
                    }
                    else
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_END_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_END_POS_Y);
                    }
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;
                #endregion

                #region CHECK_MAP_DATA
                case STEP.IF_MAPPING_DONE_CHECK_JIG:
                    if (Vision.IsMissedPointReachLimit(CAMERA.JIG, TOOL_TYPE.LEFT))
                    {
                        SetError(ECODE.CAL_ERROR_JIG_LEFT_MISSEED_POINT_MAX);
                        break;
                    }
                    if (Vision.IsMissedPointReachLimit(CAMERA.JIG, TOOL_TYPE.RIGHT))
                    {
                        SetError(ECODE.CAL_ERROR_JIG_RIGHT_MISSEED_POINT_MAX);
                        break;
                    }
                    Vision.StopTTStopWatch();
                    if (Vision.IsMapCalDone())
                    {
                        NextStep();
                        break;
                    }
                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_JIG);
                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;
                        if (Vision.GetPixelCamPos("JIG", tool) == null)
                        {
                            if (retryCount < 3)
                            {
                                retryCount++;
                                Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_JIG)];
                                return;
                            }
                        }
                    }

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;

                        if (Vision.GetPixelCamPos("JIG", tool) == null)
                            Vision.AddNGPointToCalMap(CAMERA.JIG, tool, dActualPosX, dActualPosY);
                        else
                            Vision.AddOKPointToCalMap(CAMERA.JIG, tool, dActualPosX, dActualPosY, Vision.GetPixelCamPos("JIG", tool).x, Vision.GetPixelCamPos("JIG", tool).y);
                    }

                    Vision.IncreaseCalMapIndex();
                    retryCount = 0;
                    Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_JIG)];
                    break;

                case STEP.IF_RECAL_COMPLETE_JIG:
                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_JIG);
                    if (Vision.GetPixelCamPos("JIG", calibToolType) == null)
                    {
                        SetError(ECODE.CAL_ERROR_CANT_SEARCH_TOOL); break;
                    }
                    else Vision.ChangePointDataInCalMap(CAMERA.JIG,
                            calibToolType,
                            dActualPosX,
                            dActualPosY,
                            Vision.GetPixelCamPos("JIG", calibToolType).x,
                            Vision.GetPixelCamPos("JIG", calibToolType).y);
                    reCalIndex++;
                    if (reCalIndex == reCalPointList.Count()) NextStep();
                    else
                    {
                        int nIndex = GetStepIndex(StepList, STEP.MOVE_XY_RECAL_POS_XY);
                        if (nIndex >= 0)
                            Step = StepList[StepIndex = nIndex];
                    }
                    break;

                case STEP.IF_MAPPING_DONE_CHECK_UNDER:
                    if (Vision.IsMissedPointReachLimit(CAMERA.UNDER, TOOL_TYPE.LEFT))
                    {
                        SetError(ECODE.CAL_ERROR_UNDER_LEFT_MISSEED_POINT_MAX);
                        break;
                    }
                    if (Vision.IsMissedPointReachLimit(CAMERA.UNDER, TOOL_TYPE.RIGHT))
                    {
                        SetError(ECODE.CAL_ERROR_UNDER_RIGHT_MISSEED_POINT_MAX);
                        break;
                    }
                    Vision.StopTTStopWatch();
                    if (Vision.IsMapCalDone())
                    {
                        NextStep();
                        break;
                    }

                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_UNDER);

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;
                        if (Vision.GetPixelCamPos("UNDER", tool) == null)
                        {
                            if (retryCount < 3)
                            {
                                retryCount++;
                                Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_UNDER)];
                                return;
                            }
                        }
                    }

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;

                        if (Vision.GetPixelCamPos("UNDER", tool) == null)
                            Vision.AddNGPointToCalMap(CAMERA.UNDER, tool, dActualPosX, dActualPosY);
                        else
                            Vision.AddOKPointToCalMap(CAMERA.UNDER, tool, dActualPosX, dActualPosY, Vision.GetPixelCamPos("UNDER", tool).x, Vision.GetPixelCamPos("UNDER", tool).y);
                    }

                    Vision.IncreaseCalMapIndex();
                    retryCount = 0;
                    Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_UNDER)];
                    break;

                case STEP.IF_RECAL_COMPLETE_UNDER:
                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_UNDER);
                    if (Vision.GetPixelCamPos("UNDER", calibToolType) == null)
                    {
                        SetError(ECODE.CAL_ERROR_CANT_SEARCH_TOOL); break;
                    }
                    else Vision.ChangePointDataInCalMap(CAMERA.UNDER,
                            calibToolType,
                            dActualPosX,
                            dActualPosY,
                            Vision.GetPixelCamPos("UNDER", calibToolType).x,
                            Vision.GetPixelCamPos("UNDER", calibToolType).y);
                    reCalIndex++;
                    if (reCalIndex == reCalPointList.Count()) NextStep();
                    else
                    {
                        int nIndex = GetStepIndex(StepList, STEP.MOVE_XY_RECAL_POS_XY);
                        if (nIndex >= 0)
                            Step = StepList[StepIndex = nIndex];
                    }
                    break;

                case STEP.SAVE_MAPPING_FILE_JIG:
                    string messageContent = string.Empty;
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.LEFT)
                    {
                        Vision.SaveCalibList(CAMERA.JIG, TOOL_TYPE.LEFT);
                        if (Vision.IsAllCalPointOK(CAMERA.JIG, TOOL_TYPE.LEFT))
                            messageContent += $"{CAMERA.JIG}_{TOOL_TYPE.LEFT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.JIG}_{TOOL_TYPE.LEFT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.JIG, TOOL_TYPE.LEFT).miss} NG Point). Please ReCheck all NG Point!";
                    }
                    messageContent += ("\n\n");
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.RIGHT)
                    {
                        Vision.SaveCalibList(CAMERA.JIG, TOOL_TYPE.RIGHT);
                        if (Vision.IsAllCalPointOK(CAMERA.JIG, TOOL_TYPE.RIGHT))
                            messageContent += $"{CAMERA.JIG}_{TOOL_TYPE.RIGHT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.JIG}_{TOOL_TYPE.RIGHT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.JIG, TOOL_TYPE.RIGHT).miss} NG Point). Please ReCheck all NG Point!";
                    }

                    MessageBox.Show(messageContent);
                    NextStep();
                    break;
                case STEP.SAVE_MAPPING_FILE_UNDER:
                    messageContent = string.Empty;
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.LEFT)
                    {
                        Vision.SaveCalibList(CAMERA.UNDER, TOOL_TYPE.LEFT);
                        if (Vision.IsAllCalPointOK(CAMERA.UNDER, TOOL_TYPE.LEFT))
                            messageContent += $"{CAMERA.UNDER}_{TOOL_TYPE.LEFT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.UNDER}_{TOOL_TYPE.LEFT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.UNDER, TOOL_TYPE.LEFT).miss} NG Point). Please ReCheck all NG Point!";
                    }
                    messageContent += ("\n\n");
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.RIGHT)
                    {
                        Vision.SaveCalibList(CAMERA.UNDER, TOOL_TYPE.RIGHT);
                        if (Vision.IsAllCalPointOK(CAMERA.UNDER, TOOL_TYPE.RIGHT))
                            messageContent += $"{CAMERA.UNDER}_{TOOL_TYPE.RIGHT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.UNDER}_{TOOL_TYPE.RIGHT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.UNDER, TOOL_TYPE.RIGHT).miss} NG Point). Please ReCheck all NG Point!";
                    }

                    MessageBox.Show(messageContent);
                    NextStep();
                    break;
                #endregion

                #region VISION
                case STEP.REQUEST_UNDER_CAL_PIXEL:
                    Vision.Calibration("UNDER", calibCount, currCalibrationPos, true);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.REQUEST_UNDER_CALIBRATION_CHECK;
                    break;

                case STEP.REQUEST_UNDER_CALIBRATION_CHECK:
                    if (CheckStopBit())
                        break;
                    if (Vision.IsCalDataReceived("UNDER"))
                        NextStep();
                    break;

                case STEP.REQUEST_JIG_CAL_PIXEL:
                    Vision.Calibration("JIG", calibCount, currCalibrationPos, true);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.REQUEST_JIG_CALIBRATION_CHECK;
                    break;

                case STEP.REQUEST_JIG_CALIBRATION_CHECK:
                    if (CheckStopBit())
                        break;
                    if (Vision.IsCalDataReceived("JIG"))
                        NextStep();
                    break;
                    #endregion
            }
        }

        private void MovePickerXYR(int headNo, XYRPOS Pos)
        {
            bool isUseSaturation = Machine.param.Option(ParameterDefine.OPTION.USE_XY_SATURATION) == 1 ? true : false;
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;
            double[] position = { 0.0, 0.0, 0.0, 0.0 };
            Point2d center = new Point2d();
            Point2d alignPos = new Point2d();
            double dx = 0;
            double dy = 0;
            double currentPos = 0;

            switch (Pos)
            {
                case XYRPOS.READY:
                    position[0] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_X_READY_POS);
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Y_READY_POS);
                    position[2] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_READY_POS);
                    position[3] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_READY_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.SCAN1:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_SCANNER_L_POS);
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_SCANNER_L_POS);
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_SCANNER_L_POS);
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_SCANNER_R_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.SCAN2:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_SCANNER_R_POS);
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_SCANNER_R_POS);
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_SCANNER_L_POS);
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_SCANNER_R_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.BUF1:
                case XYRPOS.BUF1READY:
                case XYRPOS.BUF2:
                case XYRPOS.BUF2READY:
                    if (Pos == XYRPOS.BUF1)
                    {
                        position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_BUF_L_POS);
                        position[1] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_BUF_L_POS);
                    }
                    if (Pos == XYRPOS.BUF1READY)
                    {
                        position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_BUF_L_POS);
                        position[1] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Y_READY_POS);
                    }
                    if (Pos == XYRPOS.BUF2)
                    {
                        position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_BUF_R_POS);
                        position[1] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_BUF_R_POS);
                    }
                    if (Pos == XYRPOS.BUF2READY)
                    {
                        position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_BUF_R_POS);
                        position[1] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Y_READY_POS);
                    }
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_PICKUP_POS);
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_PICKUP_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.ALIGN1:
                    alignPosView[0] = position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_UNDER_ALIGN_POS)
                        + (isUseSaturation ? adjustPos[targetCurr].x : 0);
                    alignPosView[1] = position[1] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN_POS)
                        + (isUseSaturation ? adjustPos[targetCurr].y : 0);

                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_UNDER_ALIGN_POS) + adjustAngle[0];
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_UNDER_ALIGN_POS) + adjustAngle[1];
                    MovePickerXYR(position);
                    break;

                case XYRPOS.ALIGN2:
                    alignPosView[0] = position[0] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_UNDER_ALIGN2_POS)
                        + (isUseSaturation ? adjustPos[targetCurr].x : 0);
                    alignPosView[1] = position[1] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN2_POS)
                        + (isUseSaturation ? adjustPos[targetCurr].y : 0);
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_R1, ref currentPos);

                    position[2] = currentPos / 1000;
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_R_UNDER_ALIGN_POS) + adjustAngle[1];
                    MovePickerXYR(position);
                    break;

                case XYRPOS.PLACE1:
                case XYRPOS.PLACE2:
                    if (Machine.status.mode == SystemMode.SystemModeAUTO)
                    {
                        placePos = position = CalculateAssemplePosXYR();
                        if (position is null || position.Count() != 4) break;
                    }
                    else if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        NextStep();
                        break;
                    }
                    else
                    {
                        placePos[0] = position[0] = Machine.param.Calibration(ParameterDefine.CALIBRATION.JIG_CAL_MAP_END_POS_X);
                        placePos[1] = position[1] = Machine.param.Calibration(ParameterDefine.CALIBRATION.JIG_CAL_MAP_END_POS_Y);
                        placePos[2] = position[2] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_READY_POS);
                        placePos[3] = position[3] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_READY_POS);
                    }
                    MovePickerXYR(position);
                    break;

                case XYRPOS.TRASH1:
                    double discardDistance_X = Machine.recipe.Option(RecipeDefine.OPTION.LEFT_NG_DISTANCE_X);
                    double discardDistance_Y = Machine.recipe.Option(RecipeDefine.OPTION.LEFT_NG_DISTANCE_Y);

                    position[0] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_X_NG_BOX_LEFT_POS) + currentNGCountInX[(int)TOOL_TYPE.LEFT] * discardDistance_X;
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Y_NG_BOX_LEFT_POS) + currentNGCountInY[(int)TOOL_TYPE.LEFT] * discardDistance_Y;
                    position[2] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_NG_BOX_LEFT_POS);
                    position[3] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_NG_BOX_LEFT_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.TRASH2:
                    discardDistance_X = Machine.recipe.Option(RecipeDefine.OPTION.RIGHT_NG_DISTANCE_X);
                    discardDistance_Y = Machine.recipe.Option(RecipeDefine.OPTION.RIGHT_NG_DISTANCE_Y);

                    position[0] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_X_NG_BOX_RIGHT_POS) + currentNGCountInX[(int)TOOL_TYPE.RIGHT] * discardDistance_X;
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Y_NG_BOX_RIGHT_POS) + currentNGCountInY[(int)TOOL_TYPE.RIGHT] * discardDistance_Y;
                    position[2] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_NG_BOX_RIGHT_POS);
                    position[3] = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_R_NG_BOX_RIGHT_POS);
                    MovePickerXYR(position);
                    break;
            }
        }

        private double[] CalculateAssemplePosXYR()
        {
            var procJig = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;
            bool isUseSaturation = Machine.param.Option(OPTION.USE_XY_SATURATION) == 1 ? true : false;
            bool isUseJigTheta = Machine.param.Option(OPTION.USE_JIG_THETA) == 1 ? true : false;
            double[] position = { 0.0, 0.0, 0.0, 0.0 };
            double dTeachedAlignPosX = 0; double dTeachedAlignPosY = 0;
            double dOffsetX = 0; double dOffsetY = 0; double dOffsetR1 = 0; double dOffsetR2 = 0;
            TOOL_TYPE toolType = (TOOL_TYPE)targetCurr;

            (double dUnderX, double dUnderY) = Vision.InterpolateMM(CAMERA.UNDER, toolType, productPosition[targetCurr].x, productPosition[targetCurr].y);
            if (dUnderX == Vision.INVALID_DATA || dUnderY == Vision.INVALID_DATA)
            {
                if (toolType == TOOL_TYPE.LEFT) SetError(ECODE.CAL_ERROR_UNDER_LEFT_DATA_OUT_OF_MAP);
                else if (toolType == TOOL_TYPE.RIGHT) SetError(ECODE.CAL_ERROR_UNDER_RIGHT_DATA_OUT_OF_MAP);
                return null;
            }

            (double dJigX, double dJigY) = Vision.InterpolateMM(CAMERA.JIG, toolType, procJig.productPosition[targetCurr].x, procJig.productPosition[targetCurr].y);
            if (dJigX == Vision.INVALID_DATA || dJigY == Vision.INVALID_DATA)
            {
                if (toolType == TOOL_TYPE.LEFT) SetError(ECODE.CAL_ERROR_JIG_LEFT_DATA_OUT_OF_MAP);
                else if (toolType == TOOL_TYPE.RIGHT) SetError(ECODE.CAL_ERROR_JIG_RIGHT_DATA_OUT_OF_MAP);
                return null;
            }

            if (toolType == TOOL_TYPE.LEFT)
            {
                dTeachedAlignPosX = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_UNDER_ALIGN_POS);
                dTeachedAlignPosY = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN_POS);
                dOffsetX = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET1_X);
                dOffsetY = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET1_Y);
                dOffsetR1 = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET1_R);
            }
            else if (toolType == TOOL_TYPE.RIGHT)
            {
                dTeachedAlignPosX = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_UNDER_ALIGN2_POS);
                dTeachedAlignPosY = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN2_POS);
                dOffsetX = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET2_X);
                dOffsetY = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET2_Y);
                dOffsetR2 = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET2_R);
            }

            dTeachedAlignPosX += (isUseSaturation ? adjustPos[targetCurr].x : 0);
            dTeachedAlignPosY += (isUseSaturation ? adjustPos[targetCurr].y : 0);

            double camAngleJigRad = Vision.cameraAngle[(int)CAMERA.JIG, (int)toolType];
            double camAngleUnderRad = Vision.cameraAngle[(int)CAMERA.UNDER, (int)toolType];

            Point2d dDevationUnder = Vision.RotationTransformationForSingle(productAngle[targetCurr], dUnderX, dUnderY, dTeachedAlignPosX, dTeachedAlignPosY);
            Point2d dDevationAll = Vision.RotationTransformation(toolType, camAngleJigRad, camAngleUnderRad, procJig.productAngle[targetCurr], dUnderX, dUnderY, dTeachedAlignPosX, dTeachedAlignPosY);

            dUnderX -= dTeachedAlignPosX;
            dUnderY -= dTeachedAlignPosY;


            position[0] = dJigX - dUnderX + dOffsetX + (isUseJigTheta ? dDevationAll.x : 0) + dDevationUnder.x;
            position[1] = dJigY - dUnderY + dOffsetY + (isUseJigTheta ? dDevationAll.y : 0) + dDevationUnder.y;
            double camAngleJigDegree = camAngleJigRad * 180.0 / Math.PI;
            double camAngleUnderDegree = camAngleUnderRad * 180.0 / Math.PI;
            position[2] = (adjustAngle[0] + productAngle[0] + camAngleJigDegree - camAngleUnderDegree + procJig.productAngle[0] + dOffsetR1);
            position[3] = (adjustAngle[1] + productAngle[1] + camAngleJigDegree - camAngleUnderDegree + procJig.productAngle[1] + dOffsetR2);
            LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"" +
                 $"[dJigX: {dJigX}, dJixY: {dJigY}]" +
                 $"[dUnderX: {dUnderX}, dUnderY: {dUnderY}]" +
                 $"[dDevationX: {dDevationAll.x}, dDevationY: {dDevationAll.y}]" +
                 $"[camJigAngle: {camAngleJigRad}, camUnderAngle: {camAngleUnderRad}]" +
                 $"[adjustAngle1: {adjustAngle[0]}, adjustAngle2: {adjustAngle[1]}]" +
                 $"[jigAngle1: {procJig.productAngle[0]}, jigAngle1: {procJig.productAngle[1]}" +
                 $"[finalX: {position[0]}, finalY: {position[1]}]" +
                 $"[finalR1: {position[2]}, finalR2: {position[3]}]", CONTENT_TYPE.INFO);
            return position;
        }

        private void MovePickerXYR(double[] position)
        {
            if (position.Length != 4)
            {
                SetError(ECODE.INVALID_MOTION_PARAMETER);
                return;
            }

            double[] vel = { 0.0, 0.0, 0.0 };
            double[] acc = { 0.0, 0.0, 0.0 };
            double[] dec = { 0.0, 0.0, 0.0 };

            vel[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_MOVE_VEL);
            acc[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_MOVE_ACC);
            dec[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_MOVE_DEC);
            vel[1] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_MOVE_VEL);
            acc[1] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_MOVE_ACC);
            dec[1] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_MOVE_DEC);
            vel[2] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_R_MOVE_VEL);
            acc[2] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_R_MOVE_ACC);
            dec[2] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_R_MOVE_DEC);

            Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_X, 1000 * position[0], vel[0], acc[0], dec[0]);
            Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_Y, 1000 * position[1], vel[1], acc[1], dec[1]);
            Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_R1, 1000 * position[2], vel[2], acc[2], dec[2]);
            Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_R2, 1000 * position[3], vel[2], acc[2], dec[2]);
        }

        private void MovePickerXYForMapping(double[] position)
        {
            double vel_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_CAL_MOVE_VEL);
            double acc_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_CAL_MOVE_ACC);
            double dec_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_CAL_MOVE_DEC);
            double vel_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_CAL_MOVE_VEL);
            double acc_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_CAL_MOVE_ACC);
            double dec_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_CAL_MOVE_DEC);

            Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_X, 1000 * position[0], vel_x, acc_x, dec_x);
            Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_Y, 1000 * position[1], vel_y, acc_y, dec_y);
        }

        private void MovePickerZ(ZPOS Pos)
        {
            double pos = 0.0;
            double vel = 0.0;
            double acc = 0.0;
            double dec = 0.0;

            switch (Pos)
            {
                case ZPOS.READY:
                case ZPOS.DISCARD:
                case ZPOS.SCAN:
                case ZPOS.PICKREADY:
                case ZPOS.PICKUPREADY_FAST:
                case ZPOS.PLACEREADY:
                case ZPOS.PLACEUPREADY_FAST:
                case ZPOS.UNDERALIGN:
                case ZPOS.UNDERCAL:
                case ZPOS.JIGCAL:
                    vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_VEL);
                    acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_ACC);
                    dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_DEC);

                    if (Pos == ZPOS.READY)
                        pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Z_READY_POS);
                    if (Pos == ZPOS.SCAN)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_SCAN_POS));
                    if (Pos == ZPOS.DISCARD)
                        pos = 1000 * (Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Z_DISCARD_POS));
                    if (Pos == ZPOS.PICKREADY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS) - 3.0);
                    if (Pos == ZPOS.PICKUPREADY_FAST)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS) - 10.0);
                    if (Pos == ZPOS.PLACEREADY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS) - 10.0);
                    if (Pos == ZPOS.PLACEUPREADY_FAST)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS) - 12.0);
                    if (Pos == ZPOS.UNDERALIGN)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_UNDER_ALIGN_POS));
                    if (Pos == ZPOS.UNDERCAL)
                    {
                        pos = 1000 * (Machine.param.Calibration(ParameterDefine.CALIBRATION.UNDER_CAL_MAP_POS_Z));
                        vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_CAL_MOVE_VEL);
                        acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_CAL_MOVE_ACC);
                        dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_CAL_MOVE_DEC);
                    }
                    if (Pos == ZPOS.JIGCAL)
                    {
                        pos = 1000 * (Machine.param.Calibration(ParameterDefine.CALIBRATION.JIG_CAL_MAP_POS_Z));
                        vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_CAL_MOVE_VEL);
                        acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_CAL_MOVE_ACC);
                        dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_CAL_MOVE_DEC);
                    }

                    Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_Z, pos, vel, acc, dec);
                    break;


                case ZPOS.PLACEUPREADY:
                case ZPOS.PICKUPREADY:
                case ZPOS.PICK:
                case ZPOS.PLACE1:
                case ZPOS.PLACE2:
                    vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_APPROACH_VEL);
                    acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_ACC);
                    dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_DEC);

                    if (Pos == ZPOS.PLACEUPREADY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS) - 5.0);
                    if (Pos == ZPOS.PICKUPREADY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS) - 5.0);
                    if (Pos == ZPOS.PICK)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS));
                    if (Pos == ZPOS.PLACE1)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS));
                    if (Pos == ZPOS.PLACE2)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_R_POS));

                    Machine.motion.MoveAxisAbs((int)AXIS.ASSEMBLER_Z, pos, vel, acc, dec);
                    break;
            }
        }
        public override void SetHeadTarget(int iTarget)
        {
            this.targetCurr = iTarget;
        }

        private int GetStepIndex(List<STEP> stepList, STEP eStep)
        {
            for (int i = 0; i < stepList.Count; i++)
            {
                if (stepList[i].ToString() == eStep.ToString())
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
