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
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessProdLoader : IProcess
    {
        public enum AUTOSTEP
        {
            ERROR,
            IDLE,
            STOP,
            COMPLETE,
            INIT,
            INIT_CHECK,
            OTHER_PROCESS_INIT_WAIT,
            PROC_SELECT_FOR_RESTART,
            LOADING_CHECK,
            LOADING,
            LOADING_COMPL_CHECK,
            LOADING_READY_LOC,
            LOADING_READY_LOC_COMPL_CHECK,
            LOADING_AVOID_LOC,
            LOADING_AVOID_LOC_COMPL_CHECK,
            ALIGNMENT,
            ALIGNMENT_COMPL_CHECK,
            ALIGNMENT2,
            ALIGNMENT2_COMPL_CHECK,
            UNLOADING_CHECK,
            UNLOADING,
            UNLOADING_COMPL_CHECK,
            UNLOADING_READY_LOC,
            UNLOADING_READY_LOC_COMPL_CHECK,
            UNLOADING_AVOID_LOC,
            UNLOADING_AVOID_LOC_COMPL_CHECK,
        }

        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,
            NEXT_STEP,

            INIT_PRODUCT_DATA_MISMATCH_CHECK,

            VAC_ON_WAIT,
            VAC_ON_WAIT_CHECK,
            PURGE_OFF_WAIT,
            PURGE_OFF_WAIT_CHECK,

            INF_PICKER_ON,
            INF_PICKER_OFF,
            INF_TRAY_ON,
            INF_TRAY_OFF,
            INF_BUFFER_ON,
            INF_BUFFER_OFF,

            MOVE_XYR_POS_CHECK,
            MOVE_XY_POS_CHECK,
            MOVE_Z_POS_CHECK,
            MOVE_XYR_READY_POS,
            MOVE_XYR_TRAY_VISION_AVOID_POS,
            MOVE_XYR_TRAY_VISION_AVOID_POS_OVERLAP,
            MOVE_XYR_TRAY_TRF_AVOID_POS,
            MOVE_XYR_TRAY_TRF_AVOID_POS_OVERLAP,
            MOVE_XYR_PICKUP_POS,
            MOVE_XYR_PICKUP_POS_OVERLAP,
            MOVE_XYR_PLACE_L_POS,
            MOVE_XYR_PLACE_R_POS,
            MOVE_XYR_PLACE_L_POS_OVERLAP,
            MOVE_XYR_PLACE_R_POS_OVERLAP,
            MOVE_XYR_ALIGN_POS,
            MOVE_Z_READY_POS,
            MOVE_Z_READY_POS_OVERLAP,
            MOVE_Z_PICKUP_READY_POS,
            MOVE_Z_PICKUP_UP_READY_POS,
            MOVE_Z_PICKUP_UP_READY_POS_FAST,
            MOVE_Z_PICKUP_POS,
            MOVE_Z_PLACE_READY_POS,
            MOVE_Z_PLACE_UP_READY_POS,
            MOVE_Z_PLACE_UP_READY_POS_FAST,
            MOVE_Z_PLACE_UP_READY_POS_FAST_OVERLAP,
            MOVE_Z_PLACE_POS,
            MOVE_Z_UNDER_ALIGN_POS,

            PICKER_UP,
            PICKER_UP_BYPASS,
            PICKER_UP_CHECK,
            PICKER_UP_ALL,
            PICKER_UP_ALL_BYPASS,
            PICKER_UP_ALL_CHECK,
            PICKER_DOWN,
            PICKER_DOWN_BYPASS,
            PICKER_DOWN_CHECK,

            PICKER_DOWN_L,
            PICKER_DOWN_L_BYPASS,
            PICKER_DOWN_L_CHECK,
            PICKER_DOWN_R,
            PICKER_DOWN_R_BYPASS,
            PICKER_DOWN_R_CHECK,

            PICKER_DOWN_ALL,
            PICKER_DOWN_ALL_BYPASS,
            PICKER_DOWN_ALL_CHECK,
            PICKER_VACON,
            PICKER_VACON_CHECK,
            PICKER_VACON_ALL,
            PICKER_VACON_ALL_CHECK,
            PICKER_VACOFF,
            PICKER_VACOFF_CHECK,
            PICKER_VACOFF_L,
            PICKER_VACOFF_L_CHECK,
            PICKER_VACOFF_R,
            PICKER_VACOFF_R_CHECK,
            PICKER_VACON_L,
            PICKER_VACON_L_CHECK,
            PICKER_VACON_R,
            PICKER_VACON_R_CHECK,
            PICKER_VACOFF_ALL,
            PICKER_VACOFF_ALL_CHECK,

            CAM_BUF_VACON,
            CAM_BUF_VACON_CHECK,
            CAM_BUF_VACOFF,
            CAM_BUF_VACOFF_CHECK,

            CAM_BUF_DETECT_ON_CHECK,
            CAM_BUF_DETECT_ON_CHECK_L,
            CAM_BUF_DETECT_ON_CHECK_R,

            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_LOADING_EMPTY_CHECK,
            IF_UNLOADING_REQUEST,
            IF_UNLOADING_BUSY,
            IF_UNLOADING_COMPLETE_CHECK,
            IF_UNLOADING_SEND_DATA,
            IF_UNLOADING_SEND_DATA_L,
            IF_UNLOADING_SEND_DATA_R,

            // CALIBRATION STEP
            REQUEST_TRAY_CAL_PIXEL,
            REQUEST_TRAY_CALIBRATION_CHECK,

            GENERATE_TRAY_MAP,
            MOVE_XY_MAPPING_TRAY,
            IF_MAPPING_DONE_CHECK_TRAY,
            SAVE_MAPPING_FILE_TRAY,

            MOVE_Z_TRAY_CAL_POS,

            MOVE_XY_MAPPING_TRAY_START_XY,
            MOVE_XY_MAPPING_TRAY_END_XY,

            MOVE_XY_RECAL_POS_XY,
            IF_RECAL_COMPLETE_TRAY,

            // PICKER VISION
            LIGHT_ON,
            LIGHT_OFF,
            PROC_ALIGN_START,
            PROC_ALIGN_COMPL_CHECK,
            PROC_ALIGN_ERROR_CHECK,

            REQUEST_PICKER_CAL_PIXEL,
            REQUEST_PICKER_CALIBRATION_CHECK,

            GENERATE_PICKER_MAP,
            MOVE_XY_MAPPING_PICKER,
            IF_MAPPING_DONE_CHECK_PICKER,
            SAVE_MAPPING_FILE_PICKER,

            MOVE_Z_PICKER_CAL_POS,

            MOVE_XY_MAPPING_PICKER_START_XY,
            MOVE_XY_MAPPING_PICKER_END_XY,

            IF_RECAL_COMPLETE_PICKER,
        }

        public enum MSG
        {
            MSG_SINGLE,
            MSG_PROCESS_INIT,
            MSG_LOADING_PRODUCT,
            MSG_ALIGN_PRODUCT,
            MSG_ALIGN2_PRODUCT,
            MSG_UNLOADING_PRODUCT,

            MSG_MOVE_READY_POS,
            MSG_MOVE_READY_POS_OVERLAP,
            MSG_MOVE_TRAYTRF_AVOID_POS,
            MSG_MOVE_TRAYTRF_AVOID_POS_OVERLAP,
            MSG_MOVE_PICKUP_L_POS,
            MSG_MOVE_PICKUP_R_POS,
            MSG_MOVE_ALIGN_L_POS,
            MSG_MOVE_ALIGN_R_POS,
            MSG_MOVE_PLACE_L_POS,
            MSG_MOVE_PLACE_R_POS,

            MSG_MAPPING_CAL_TRAY_SINGLE,
            MSG_MAPPING_CAL_TRAY_DUAL,
            MSG_MOVE_MAP_TRAY_START_XY,
            MSG_MOVE_MAP_TRAY_END_XY,
            MSG_MAPPING_RECAL_TRAY_POINTS,
            MSG_MOVE_RECAL_POINT_XY_TRAY,

            MSG_MAPPING_CAL_PICKER_SINGLE,
            MSG_MOVE_MAP_PICKER_START_XY,
            MSG_MOVE_MAP_PICKER_END_XY,
            MSG_MAPPING_RECAL_PICKER_POINTS,
            MSG_MOVE_RECAL_POINT_XY_PICKER,
        }

        public enum XYRPOS
        {
            READY,
            TRAY_VISION_AVOID,
            TRAY_TRF_AVOID,
            PICK1,
            PICK2,
            BUF_L_L,
            BUF_L_R,
            BUF_R_L,
            BUF_R_R,
            BUF2SAFE,
            ALIGN1,
            ALIGN2,
            ALIGN3,//이름바꿀거임 L_L식로
            ALIGN4
        }

        public enum ZPOS
        {
            READY,
            PICK_READY,
            PICK_UP_READY,
            PICK_UP_READY_FAST,
            PICK,
            PLACEREADY,
            PLACEUPREADY,
            PLACE,
            UNDER_ALIGN,
            TRAYCAL,
            PICKERCAL,
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

            Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 0);
            Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 0);
            Machine.Parts[(int)UNITPART.PROD_PICK1].ClearInterface();
            Machine.Parts[(int)UNITPART.PROD_PICK2].ClearInterface();
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 0);
            Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 0);
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

        private TOOL_TYPE calibToolType;
        public double[] pickupPos = { 0.0, 0.0, 0.0, 0.0 };
        private Point2d[] PickupPos = new Point2d[2];
        private double[] PickupAngle = new double[2];
        public int targetCurr = 0;
        public bool pickupOn = false;
        private bool receivedData = false;
        private int[] upstream = { (int)UNITPART.TRAY, (int)UNITPART.TRAY };
        private int[] current = { (int)UNITPART.PROD_PICK1, (int)UNITPART.PROD_PICK2 };
        private int[] downstream = { (int)UNITPART.BUF1_L1, (int)UNITPART.BUF1_R1 };
        private bool[] isFixAngle180 = { false, false };
        public Point2d[] productPosition = new Point2d[2];
        public double[] productAngle = new double[2];
        public Point2d[] adjustPos = new Point2d[2];
        public double[] adjustAngle = { 0, 0 };
        public Point2d[] alignCenterPos = new Point2d[2];
        public double[] alignCenterAngle = { 0, 0 };
        public int[] alignSucs = { -1, -1 };
        private int iAlignErrorCheckCount = 0;
        private int iRetryVision = 0;

        Point2d currCalibrationPos = new Point2d();
        int pickRetryCount;
        bool isPauseCal = false;
        int calPauseMSG = -1;
        List<CalibPoint> reCalPointList = new List<CalibPoint>();
        int reCalIndex;
        int retryCount;
        private bool isVacOnComplete = false;
        private int finalPickRetryCount = 0;

        public ProcessProdLoader()
        {
            for (int i = 0; i < PickupPos.Length; i++)
            {
                PickupPos[i] = new Point2d();
                productPosition[i] = new Point2d();
                adjustPos[i] = new Point2d();
                alignCenterPos[i] = new Point2d();
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

                if (Vision.IsSameWithLastData(CAMERA.PICKER, (TOOL_TYPE)targetCurr, new Point2d(productPosition[targetCurr].x, productPosition[targetCurr].y)))
                    throw new Exception("New data is same with last data");
            }
            catch (Exception ex)
            {
                productPosition[targetCurr].x = 0.001 * 0.0;
                productPosition[targetCurr].y = 0.001 * 0.0;
                productAngle[targetCurr] = 0.0;
                alignSucs[targetCurr] = -1;
            }
            receivedData = true;
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
                case ECODE.TIMEOUT_PROD_PICK1_VACON:
                case ECODE.TIMEOUT_PROD_PICK2_VACON:
                    var processErrorHandler = new List<STEP>()
                    {
                        STEP.PICKER_VACOFF,
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.ERROR,
                    };

                    Vision.PauseTrainCapture("TRAY", false);
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
            double actualPos = 0;
            bool isManualMode = Machine.status.state == SystemState.SystemStateIDLE;

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
                    finalPickRetryCount = 0;
                    var processInit = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.MOVE_XYR_TRAY_TRF_AVOID_POS,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    Machine.Parts[(int)UNITPART.PROD_PICK1].ClearInterface();
                    //Machine.interfer_prod_loader_unloading = true;
                    Machine.interfer_prod_loader_on_buffer[targetCurr] = true;
                    pickRetryCount = 0;
                    isVacOnComplete = false;

                    var processLoading = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.IF_LOADING_POSSIBLE_WAIT, // 트레이 상태 확인
                        STEP.IF_LOADING_BUSY,
                        STEP.MOVE_XYR_PICKUP_POS,
                        //STEP.INF_PICKER_OFF,
                        STEP.INF_BUFFER_OFF,
                        STEP.PICKER_DOWN_BYPASS,
                        STEP.MOVE_Z_PICKUP_READY_POS,
                        STEP.MOVE_XYR_POS_CHECK,
                        STEP.MOVE_Z_PICKUP_POS,
                        STEP.PICKER_DOWN_CHECK,
                        STEP.PICKER_VACON,
                        STEP.PICKER_UP_BYPASS,
                        STEP.MOVE_Z_PICKUP_UP_READY_POS,
                        STEP.PICKER_UP_CHECK,
                        STEP.PICKER_VACON,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.IDLE,
                    };

                    if (Machine.sysMode == Machine.SYSMODE.AUTO && Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                    {
                        Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actualPos);
                        if (actualPos > 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_RELEASE_Y_POS))
                            processLoading[1] = STEP.MOVE_Z_PICKUP_UP_READY_POS;
                    }

                    StepList = processLoading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_ALIGN_PRODUCT:
                case MSG.MSG_ALIGN2_PRODUCT:
                    if (message == (int)MSG.MSG_ALIGN_PRODUCT) targetCurr = 0;
                    if (message == (int)MSG.MSG_ALIGN2_PRODUCT) targetCurr = 1;

                    ResetUnderVisionData();

                    var processAlignProduct = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_UNDER_ALIGN_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.LIGHT_ON,
                        STEP.MOVE_XYR_ALIGN_POS,
                        STEP.INF_BUFFER_OFF,
                        STEP.PROC_ALIGN_START,
                        STEP.PROC_ALIGN_ERROR_CHECK,
                        //STEP.LIGHT_OFF,              
                        STEP.IDLE,
                    };
                    double apos = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Z, ref apos);
                    bool isZInAlignPos = Math.Abs(apos / 1000 - (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_ALIGN_POS))) <= 0.2;
                    if (Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist
                    && !isManualMode && isZInAlignPos && targetCurr == 1)
                    {
                        processAlignProduct[GetStepIndex(processAlignProduct, STEP.MOVE_Z_UNDER_ALIGN_POS)] = STEP.NEXT_STEP;
                    }

                    StepList = processAlignProduct;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Parts[(int)UNITPART.PROD_PICK1].ClearInterface();
                    Machine.Parts[(int)UNITPART.PROD_PICK2].ClearInterface();
                    //Machine.interfer_prod_loader_unloading = true;
                    Machine.interfer_prod_loader_on_buffer[targetCurr] = true;

                    var processUnloading = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.IF_UNLOADING_REQUEST,
                        STEP.IF_UNLOADING_BUSY,

                        STEP.PICKER_VACON_L,
                        STEP.MOVE_XYR_PLACE_L_POS_OVERLAP,
                        STEP.PICKER_DOWN_L_BYPASS,
                        STEP.MOVE_Z_PLACE_READY_POS,
                        STEP.MOVE_XYR_POS_CHECK,
                        STEP.MOVE_Z_PLACE_POS,
                        STEP.PICKER_DOWN_L_CHECK,
                        STEP.PICKER_VACOFF_L,
                        STEP.CAM_BUF_DETECT_ON_CHECK_L,
                        STEP.IF_UNLOADING_SEND_DATA_L,
                        STEP.PICKER_UP_ALL,

                        STEP.PICKER_VACON_R,
                        STEP.MOVE_XYR_PLACE_R_POS_OVERLAP,
                        STEP.PICKER_DOWN_R_BYPASS,
                        STEP.MOVE_XYR_POS_CHECK,
                        STEP.MOVE_Z_PLACE_POS,
                        STEP.PICKER_DOWN_R_CHECK,
                        STEP.PICKER_VACOFF_R,
                        STEP.CAM_BUF_DETECT_ON_CHECK_R,
                        STEP.IF_UNLOADING_SEND_DATA_R,

                        STEP.PICKER_UP_ALL_BYPASS,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_UP_ALL_CHECK,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        STEP.CAM_BUF_DETECT_ON_CHECK_L,
                        STEP.CAM_BUF_DETECT_ON_CHECK_R,
                        STEP.IDLE,
                    };

                    if (!Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                    {
                        processUnloading = new List<STEP>()
                        {
                            STEP.PICKER_UP_ALL_BYPASS,
                            STEP.MOVE_Z_READY_POS,
                            STEP.PICKER_UP_ALL_CHECK,
                            STEP.IF_UNLOADING_REQUEST,
                            STEP.IF_UNLOADING_BUSY,

                            STEP.PICKER_VACON_R,
                            STEP.MOVE_XYR_PLACE_R_POS_OVERLAP,
                            STEP.PICKER_DOWN_R_BYPASS,
                            STEP.MOVE_Z_PLACE_READY_POS,
                            STEP.MOVE_XYR_POS_CHECK,
                            STEP.MOVE_Z_PLACE_POS,
                            STEP.PICKER_DOWN_R_CHECK,
                            STEP.PICKER_VACOFF_R,
                            STEP.CAM_BUF_DETECT_ON_CHECK_R,
                            STEP.IF_UNLOADING_SEND_DATA_R,

                            STEP.PICKER_UP_ALL_BYPASS,
                            STEP.MOVE_Z_READY_POS,
                            STEP.PICKER_UP_ALL_CHECK,
                            STEP.IF_UNLOADING_COMPLETE_CHECK,
                            STEP.CAM_BUF_DETECT_ON_CHECK_R,
                            STEP.IDLE,
                        };
                    }
                    if (!Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                    {
                        processUnloading = new List<STEP>()
                        {
                            STEP.PICKER_UP_ALL_BYPASS,
                            STEP.MOVE_Z_READY_POS,
                            STEP.PICKER_UP_ALL_CHECK,
                            STEP.IF_UNLOADING_REQUEST,
                            STEP.IF_UNLOADING_BUSY,

                            STEP.PICKER_VACON_L,
                            STEP.MOVE_XYR_PLACE_L_POS_OVERLAP,
                            STEP.PICKER_DOWN_L_BYPASS,
                            STEP.MOVE_Z_PLACE_READY_POS,
                            STEP.MOVE_XYR_POS_CHECK,
                            STEP.MOVE_Z_PLACE_POS,
                            STEP.PICKER_DOWN_L_CHECK,
                            STEP.PICKER_VACOFF_L,
                            STEP.CAM_BUF_DETECT_ON_CHECK_L,
                            STEP.IF_UNLOADING_SEND_DATA_L,
                            STEP.PICKER_UP_ALL,

                            STEP.PICKER_UP_ALL_BYPASS,
                            STEP.MOVE_Z_READY_POS,
                            STEP.PICKER_UP_ALL_CHECK,
                            STEP.IF_UNLOADING_COMPLETE_CHECK,
                            STEP.CAM_BUF_DETECT_ON_CHECK_L,
                            STEP.IDLE,
                        };
                    }
                    StepList = processUnloading;
                    if (isManualMode)
                    {
                        StepList.Insert(0, STEP.MOVE_Z_READY_POS);
                        StepList.Insert(1, STEP.PICKER_UP_ALL);
                    }
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_READY_POS:
                case MSG.MSG_MOVE_READY_POS_OVERLAP:
                case MSG.MSG_MOVE_TRAYTRF_AVOID_POS:
                case MSG.MSG_MOVE_TRAYTRF_AVOID_POS_OVERLAP:
                case MSG.MSG_MOVE_PICKUP_L_POS:
                case MSG.MSG_MOVE_PICKUP_R_POS:
                case MSG.MSG_MOVE_ALIGN_L_POS:
                case MSG.MSG_MOVE_ALIGN_R_POS:
                case MSG.MSG_MOVE_PLACE_L_POS:
                case MSG.MSG_MOVE_PLACE_R_POS:
                    var moveXYRPosition = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XYR_PICKUP_POS,
                        STEP.MOVE_Z_POS_CHECK,
                        STEP.IDLE,
                    };

                    if ((MSG)message == MSG.MSG_MOVE_READY_POS) moveXYRPosition[2] = STEP.MOVE_XYR_READY_POS;
                    if ((MSG)message == MSG.MSG_MOVE_READY_POS_OVERLAP) { moveXYRPosition[1] = STEP.MOVE_Z_READY_POS_OVERLAP; moveXYRPosition[2] = STEP.MOVE_XYR_READY_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_TRAYTRF_AVOID_POS) moveXYRPosition[2] = STEP.MOVE_XYR_TRAY_VISION_AVOID_POS;
                    if ((MSG)message == MSG.MSG_MOVE_TRAYTRF_AVOID_POS_OVERLAP) { moveXYRPosition[1] = STEP.MOVE_Z_READY_POS_OVERLAP; moveXYRPosition[2] = STEP.MOVE_XYR_TRAY_VISION_AVOID_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_PICKUP_L_POS) { targetCurr = 0; moveXYRPosition[2] = STEP.MOVE_XYR_PICKUP_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_PICKUP_R_POS) { targetCurr = 1; moveXYRPosition[2] = STEP.MOVE_XYR_PICKUP_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_ALIGN_L_POS) { targetCurr = 0; moveXYRPosition[2] = STEP.MOVE_XYR_ALIGN_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_ALIGN_R_POS) { targetCurr = 1; moveXYRPosition[2] = STEP.MOVE_XYR_ALIGN_POS; }
                    if ((MSG)message == MSG.MSG_MOVE_PLACE_L_POS) moveXYRPosition[2] = STEP.MOVE_XYR_PLACE_L_POS;
                    if ((MSG)message == MSG.MSG_MOVE_PLACE_R_POS) moveXYRPosition[2] = STEP.MOVE_XYR_PLACE_R_POS;

                    StepList = moveXYRPosition;
                    Step = StepList[StepIndex = 0];
                    break;

                #region MESSAGE_CALIBRATION
                case MSG.MSG_MOVE_MAP_TRAY_START_XY:
                case MSG.MSG_MOVE_MAP_TRAY_END_XY:
                case MSG.MSG_MOVE_MAP_PICKER_START_XY:
                case MSG.MSG_MOVE_MAP_PICKER_END_XY:
                    var processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_MAPPING_TRAY_START_XY,
                        STEP.IDLE,
                    };
                    if ((MSG)message == MSG.MSG_MOVE_MAP_TRAY_START_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_TRAY_START_XY;
                    if ((MSG)message == MSG.MSG_MOVE_MAP_TRAY_END_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_TRAY_END_XY;
                    if ((MSG)message == MSG.MSG_MOVE_MAP_PICKER_START_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_PICKER_START_XY;
                    if ((MSG)message == MSG.MSG_MOVE_MAP_PICKER_END_XY) processMappingList[2] = STEP.MOVE_XY_MAPPING_PICKER_END_XY;
                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_CAL_TRAY_SINGLE:
                    calPauseMSG = message;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    if (!isPauseCal)
                    {
                        Vision.ClearBeforeMapping(CAMERA.TRAY, calibToolType);
                    }
                    Vision.SetHeadTarget("TRAY", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_DOWN,
                        STEP.GENERATE_TRAY_MAP,
                        STEP.MOVE_XY_MAPPING_TRAY,
                        STEP.MOVE_Z_TRAY_CAL_POS,
                        STEP.REQUEST_TRAY_CAL_PIXEL,
                        STEP.IF_MAPPING_DONE_CHECK_TRAY,
                        STEP.SAVE_MAPPING_FILE_TRAY,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    if (isPauseCal) Step = StepList[StepIndex = 4];
                    else Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_CAL_TRAY_DUAL:
                    calPauseMSG = message;
                    calibToolType = TOOL_TYPE.MAX;
                    if (!isPauseCal)
                    {
                        Vision.ClearBeforeMapping(CAMERA.TRAY, TOOL_TYPE.LEFT);
                        Vision.ClearBeforeMapping(CAMERA.TRAY, TOOL_TYPE.RIGHT);
                    }
                    Vision.SetHeadTarget("TRAY", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.PICKER_DOWN_ALL,
                        STEP.GENERATE_TRAY_MAP,
                        STEP.MOVE_XY_MAPPING_TRAY,
                        STEP.MOVE_Z_TRAY_CAL_POS,
                        STEP.REQUEST_TRAY_CAL_PIXEL,
                        STEP.IF_MAPPING_DONE_CHECK_TRAY,
                        STEP.SAVE_MAPPING_FILE_TRAY,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    if (isPauseCal) Step = StepList[StepIndex = 4];
                    else Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_RECAL_TRAY_POINTS:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    Vision.SetHeadTarget("TRAY", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.PICKER_DOWN,
                        STEP.MOVE_Z_TRAY_CAL_POS,
                        STEP.REQUEST_TRAY_CAL_PIXEL,
                        STEP.IF_RECAL_COMPLETE_TRAY,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_RECAL_POINT_XY_TRAY:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;

                    processMappingList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.PICKER_DOWN,
                        STEP.MOVE_Z_TRAY_CAL_POS,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_CAL_PICKER_SINGLE:
                    calPauseMSG = message;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    if (!isPauseCal)
                    {
                        Vision.ClearBeforeMapping(CAMERA.PICKER, calibToolType);
                    }
                    Vision.SetHeadTarget("PICKER", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.PICKER_UP_ALL,
                        STEP.MOVE_Z_READY_POS,
                        STEP.GENERATE_PICKER_MAP,
                        STEP.MOVE_XY_MAPPING_PICKER,
                        STEP.MOVE_Z_PICKER_CAL_POS,
                        STEP.REQUEST_PICKER_CAL_PIXEL,
                        STEP.IF_MAPPING_DONE_CHECK_PICKER,
                        STEP.SAVE_MAPPING_FILE_PICKER,
                        STEP.PICKER_UP_ALL,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    if (isPauseCal) Step = StepList[StepIndex = 3];
                    else Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MAPPING_RECAL_PICKER_POINTS:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;
                    Vision.SetHeadTarget("PICKER", calibToolType);

                    processMappingList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.MOVE_Z_PICKER_CAL_POS,
                        STEP.REQUEST_PICKER_CAL_PIXEL,
                        STEP.IF_RECAL_COMPLETE_PICKER,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_RECAL_POINT_XY_PICKER:
                    reCalIndex = 0;
                    if (targetCurr == 0) calibToolType = TOOL_TYPE.LEFT;
                    else if (targetCurr == 1) calibToolType = TOOL_TYPE.RIGHT;

                    processMappingList = new List<STEP>()
                    {
                        STEP.MOVE_Z_READY_POS,
                        STEP.MOVE_XY_RECAL_POS_XY,
                        STEP.MOVE_Z_PICKER_CAL_POS,
                        STEP.IDLE,
                    };

                    StepList = processMappingList;
                    Step = StepList[StepIndex = 0];
                    break;
                    #endregion
            }
        }

        protected override void OnProcessing()
        {
            uint ret1 = 0;
            uint ret2 = 0;

            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    AutoStep = AUTOSTEP.ERROR;
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    error_proc = false;
                    stopBit = false;
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
                        Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_VACON, ref ret1);
                        Machine.Parts[(int)UNITPART.PROD_PICK1].exist = Convert.ToBoolean(ret1);
                        Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_VACON, ref ret1);
                        Machine.Parts[(int)UNITPART.PROD_PICK2].exist = Convert.ToBoolean(ret1);

                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_1, ref ret1);
                        Machine.Parts[(int)UNITPART.BUF1_L1].exist = Convert.ToBoolean(ret1);
                        Thread.Sleep(300);
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_3, ref ret2);
                        Machine.Parts[(int)UNITPART.BUF1_R1].exist = Convert.ToBoolean(ret2);
                        //if (ret1 == 0 || ret2 == 0)
                        //{
                        //    if (ret1 == 0)
                        //    {
                        //        if (Machine.Parts[(int)UNITPART.BUF1_L2].exist == true)
                        //            SetError(ECODE.MISMATCH_BETWEEN_DATA_AND_PRODUCT_LEFT_BUF1_L1);
                        //    }
                        //    if (ret2 == 0)
                        //    {
                        //        if (Machine.Parts[(int)UNITPART.BUF1_R2].exist == true)
                        //            SetError(ECODE.MISMATCH_BETWEEN_DATA_AND_PRODUCT_RIGHT_BUF1_R1);
                        //    }
                        //}

                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_2, ref ret1);
                        Machine.Parts[(int)UNITPART.BUF1_L2].exist = Convert.ToBoolean(ret1);
                        Thread.Sleep(300);
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_4, ref ret2);
                        Machine.Parts[(int)UNITPART.BUF1_R2].exist = Convert.ToBoolean(ret2);
                        //if (ret1 == 0 || ret2 == 0)
                        //{
                        //    if (ret1 == 0)
                        //    {
                        //        if (Machine.Parts[(int)UNITPART.BUF1_L1].exist == true)
                        //            SetError(ECODE.MISMATCH_BETWEEN_DATA_AND_PRODUCT_LEFT_BUF1_L2);
                        //    }
                        //    if (ret2 == 0)
                        //    {
                        //        if (Machine.Parts[(int)UNITPART.BUF1_R1].exist == true)
                        //            SetError(ECODE.MISMATCH_BETWEEN_DATA_AND_PRODUCT_RIGHT_BUF1_R2);
                        //    }
                        //}
                    }

                    NextStep();
                    break;

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

                //case STEP.INF_PICKER_ON:
                //    Machine.interfer_prod_loader_unloading = true;
                //    NextStep();
                //    break;

                //case STEP.INF_PICKER_OFF:
                //    Machine.interfer_prod_loader_unloading = false;
                //    NextStep();
                //    break;

                //case STEP.INF_TRAY_ON:
                //    Machine.interfer_prod_picker_loading_to_tray = true;
                //    NextStep();
                //    break;

                //case STEP.INF_TRAY_OFF:
                //    Machine.interfer_prod_picker_loading_to_tray = false;
                //    NextStep();
                //    break;

                case STEP.INF_BUFFER_ON:
                    Machine.interfer_prod_loader_on_buffer[targetCurr] = true;
                    NextStep();
                    break;

                case STEP.INF_BUFFER_OFF:
                    Machine.interfer_prod_loader_on_buffer[0] = false;
                    Machine.interfer_prod_loader_on_buffer[1] = false;
                    NextStep();
                    break;
            }

            OnProcessOfAutoRun();
            OnProcessOfMotion();
            OnProcessOfIO();
            OnProcessOfInterface();
            OnProcessOfCalibration();
            OnProcessOfPickerVision();

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

        private void OnProcessOfAutoRun()
        {
            bool isManualMode = Machine.status.state == SystemState.SystemStateIDLE;

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;

            //string logText = "";
            uint ret1 = 0;

            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
                    break;

                case AUTOSTEP.STOP:
                    break;

                case AUTOSTEP.COMPLETE:
                    AutoStep = AUTOSTEP.LOADING_CHECK;

                    CheckStopBit();
                    break;

                case AUTOSTEP.INIT:
                    //Machine.interfer_prod_loader_unloading = true;
                    Machine.interfer_prod_loader_on_buffer[0] = true;
                    Machine.interfer_prod_loader_on_buffer[1] = true;
                    Machine.Parts[(int)UNITPART.PROD_PICK1].ClearInterface();
                    Machine.Parts[(int)UNITPART.PROD_PICK2].ClearInterface();

                    initCompl = false;
                    stopBit = false;
                    pickupOn = true;

                    pickRetryCount = 0;

                    if (Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                        pickupOn = false;

                    if (!Machine.proclist[(int)Machine.PROCESS.ASSEMBLER].initCompl)
                        break;
                    //if (!Machine.proclist[(int)Machine.PROCESS.LEFTBUF].initCompl)
                    //    break;
                    //if (!Machine.proclist[(int)Machine.PROCESS.RIGHTBUF].initCompl)
                    //    break;

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
                    if (Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                    {
                        AutoStep = AUTOSTEP.ALIGNMENT;
                    }
                    else if (!Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                    {
                        AutoStep = AUTOSTEP.ALIGNMENT2;
                    }
                    else
                        AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.LOADING_CHECK:
                    if (CheckStopBit())
                        break;

                    if (Machine.PickupStop)
                    {
                        AutoStep = AUTOSTEP.LOADING_READY_LOC;
                        break;
                    }

                    if (Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                    {
                        AutoStep = AUTOSTEP.ALIGNMENT;
                        break;
                    }

                    pickupOn = true;

                    if (!Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist)
                    {
                        AutoStep = AUTOSTEP.LOADING_READY_LOC;
                        break;
                    }
                    if (!Machine.Parts[(int)UNITPART.TRAY].exist && !Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist)
                    {
                        AutoStep = AUTOSTEP.LOADING_READY_LOC;
                        break;
                    }
                    if (!Machine.Parts[(int)UNITPART.TRAY].exist || !proc.doingAlignment)
                    {
                        AutoStep = AUTOSTEP.LOADING_AVOID_LOC;
                        break;
                    }

                    targetCurr = 0;
                    if (!Machine.Parts[(int)UNITPART.PROD_PICK1].exist && proc.alignSucs[0] >= 0) targetCurr = 0;
                    else if (!Machine.Parts[(int)UNITPART.PROD_PICK2].exist && proc.alignSucs[1] >= 0) targetCurr = 1;
                    else
                    {
                        if (Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                        {
                            AutoStep = AUTOSTEP.UNLOADING_CHECK;
                        }
                        else
                        {
                            if (!Machine.TrayHolding && Machine.Parts[(int)UNITPART.TRAY].productCount <= 0)
                            {
                                //if (Machine.Parts[(int)UNITPART.BUF1_L1].loadingEnable || Machine.Parts[(int)UNITPART.BUF1_L1].loadingEnable)
                                AutoStep = AUTOSTEP.LOADING_AVOID_LOC;
                            }
                            else
                                AutoStep = AUTOSTEP.LOADING_READY_LOC;
                        }
                        break;
                    }

                    if (proc.productPosition[(int)TOOL_TYPE.LEFT].x == proc.productPosition[(int)TOOL_TYPE.RIGHT].x
                        && proc.productPosition[(int)TOOL_TYPE.LEFT].y == proc.productPosition[(int)TOOL_TYPE.RIGHT].y)
                    {
                        Machine.Parts[(int)UNITPART.TRAY].productCount--;
                        if (!Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                        {
                            proc.productPosition[(int)TOOL_TYPE.RIGHT].x = 0.0;
                            proc.productPosition[(int)TOOL_TYPE.RIGHT].y = 0.0;
                            proc.productAngle[(int)TOOL_TYPE.RIGHT] = 0.0;
                            proc.alignSucs[(int)TOOL_TYPE.RIGHT] = -1;
                        }
                        else if (!Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                        {
                            proc.productPosition[(int)TOOL_TYPE.LEFT].x = 0.0;
                            proc.productPosition[(int)TOOL_TYPE.LEFT].y = 0.0;
                            proc.productAngle[(int)TOOL_TYPE.LEFT] = 0.0;
                            proc.alignSucs[(int)TOOL_TYPE.LEFT] = -1;
                        }
                    }

                    AutoStep = AUTOSTEP.LOADING;
                    break;

                case AUTOSTEP.LOADING:
                    if (CheckStopBit() || !Machine.trayTransferReady)
                        break;

                    if (Machine.Parts[(int)UNITPART.TRAY].productCount > 0)
                    {
                        SetMessage((int)MSG.MSG_LOADING_PRODUCT);
                        AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    }
                    else
                    {
                        Machine.Parts[(int)UNITPART.TRAY].exist = false;
                        //if (Machine.TrayHolding || !Machine.Parts[(int)UNITTRAY.TRAY].exist)
                        AutoStep = AUTOSTEP.LOADING_CHECK;
                    }
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    pickRetryCount = 0;

                    Machine.interfer_prod_loader_on_buffer[0] = false;
                    Machine.interfer_prod_loader_on_buffer[1] = false;
                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.LOADING_READY_LOC:
                    if (CheckStopBit())
                        break;

                    SetMessage((int)MSG.MSG_MOVE_READY_POS);

                    AutoStep = AUTOSTEP.LOADING_READY_LOC_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_READY_LOC_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;
                    Machine.interfer_prod_loader_on_buffer[0] = false;
                    Machine.interfer_prod_loader_on_buffer[1] = false;
                    //Machine.interfer_prod_loader_unloading = false;
                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.LOADING_AVOID_LOC:
                    if (CheckStopBit())
                        break;

                    //Machine.interfer_prod_loader_unloading = true;
                    SetMessage((int)MSG.MSG_MOVE_TRAYTRF_AVOID_POS);

                    AutoStep = AUTOSTEP.LOADING_AVOID_LOC_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_AVOID_LOC_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;
                    Machine.interfer_prod_loader_on_buffer[0] = false;
                    Machine.interfer_prod_loader_on_buffer[1] = false;
                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_CHECK:
                    if (CheckStopBit())
                        break;
                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        AutoStep = AUTOSTEP.IDLE;
                        break;
                    }

                    pickupOn = false;
                    Machine.interfer_prod_loader_on_buffer[0] = false;
                    Machine.interfer_prod_loader_on_buffer[1] = false;

                    bool existUnloadingParts = Machine.Parts[(int)UNITPART.BUF2_L1].exist || Machine.Parts[(int)UNITPART.BUF2_R1].exist;
                    //if (existUnloadingParts && !Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist)
                    //{
                    //    AutoStep = AUTOSTEP.UNLOADING_READY_LOC;
                    //    break;
                    //}
                    if (Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist && proc.doingAlignment
                        && (!Machine.Parts[current[0]].exist || !Machine.Parts[current[1]].exist))
                    {
                        AutoStep = AUTOSTEP.UNLOADING_AVOID_LOC;
                        break;
                    }

                    if (CheckStopBit())
                        break;

                    int partInPicker = 0;
                    int partInBufL = 0;
                    int partInBufR = 0;
                    partInPicker += Machine.Parts[(int)UNITPART.PROD_PICK1].exist ? 1 : 0;
                    partInPicker += Machine.Parts[(int)UNITPART.PROD_PICK2].exist ? 1 : 0;
                    partInBufL += Machine.Parts[(int)UNITPART.BUF1_L1].exist ? 1 : 0;
                    partInBufL += Machine.Parts[(int)UNITPART.BUF1_L2].exist ? 1 : 0;
                    partInBufR += Machine.Parts[(int)UNITPART.BUF1_R1].exist ? 1 : 0;
                    partInBufR += Machine.Parts[(int)UNITPART.BUF1_R2].exist ? 1 : 0;
                    if (partInPicker == 1)
                    {
                        if (Machine.buffer_ready_to_place[(int)TOOL_TYPE.LEFT] && partInBufL == 1)
                        {
                            if (Machine.Parts[(int)UNITPART.BUF1_L1].exist && Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF1_L1_DETECT); return;
                            }
                            if (Machine.Parts[(int)UNITPART.BUF1_L2].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF1_L2_DETECT); return;
                            }
                            targetCurr = 0;
                            Machine.prod_loader_start_place[targetCurr] = true;
                        }
                        else if (Machine.buffer_ready_to_place[(int)TOOL_TYPE.RIGHT] && partInBufR == 1)
                        {
                            if (Machine.Parts[(int)UNITPART.BUF1_R1].exist && Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF1_R1_DETECT); return;
                            }
                            if (Machine.Parts[(int)UNITPART.BUF1_R2].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                            {
                                SetError(ECODE.TIMEOUT_BUF1_R2_DETECT); return;
                            }
                            targetCurr = 1;
                            Machine.prod_loader_start_place[targetCurr] = true;
                        }
                        else break;
                    }
                    else if (Machine.buffer_ready_to_place[(int)TOOL_TYPE.LEFT])
                    {
                        if (Machine.Parts[(int)UNITPART.BUF1_L1].exist && Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                        {
                            SetError(ECODE.TIMEOUT_BUF1_L1_DETECT); return;
                        }
                        if (Machine.Parts[(int)UNITPART.BUF1_L2].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                        {
                            SetError(ECODE.TIMEOUT_BUF1_L2_DETECT); return;
                        }
                        targetCurr = 0;
                        Machine.prod_loader_start_place[targetCurr] = true;
                    }
                    else if (Machine.buffer_ready_to_place[(int)TOOL_TYPE.RIGHT])
                    {
                        if (Machine.Parts[(int)UNITPART.BUF1_R1].exist && Machine.Parts[(int)UNITPART.PROD_PICK1].exist)
                        {
                            SetError(ECODE.TIMEOUT_BUF1_R1_DETECT); return;
                        }
                        if (Machine.Parts[(int)UNITPART.BUF1_R2].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                        {
                            SetError(ECODE.TIMEOUT_BUF1_R2_DETECT); return;
                        }
                        targetCurr = 1;
                        Machine.prod_loader_start_place[targetCurr] = true;
                    }
                    else if (Machine.Parts[(int)UNITPART.BUF1_L1].loadingEnable) targetCurr = 0;
                    else if (Machine.Parts[(int)UNITPART.BUF1_R1].loadingEnable) targetCurr = 1;
                    else
                    {
                        AutoStep = AUTOSTEP.UNLOADING_READY_LOC;
                        break;
                    }

                    AutoStep = AUTOSTEP.UNLOADING;
                    break;

                case AUTOSTEP.UNLOADING:
                    if (CheckStopBit())
                        break;

                    //if (Machine.interfer_assembler_loading)
                    //{
                    //    AutoStep = AUTOSTEP.UNLOADING_CHECK;
                    //    break;
                    //}

                    Machine.interfer_prod_loader_on_buffer[targetCurr] = true;
                    SetMessage((int)MSG.MSG_UNLOADING_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;
                    Machine.prod_loader_start_place[targetCurr] = false;
                    Machine.buffer_ready_to_place[targetCurr] = false;
                    AutoStep = AUTOSTEP.COMPLETE;
                    break;

                case AUTOSTEP.UNLOADING_READY_LOC:
                    if (CheckStopBit())
                        break;

                    SetMessage((int)MSG.MSG_MOVE_READY_POS);
                    AutoStep = AUTOSTEP.UNLOADING_READY_LOC_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_READY_LOC_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    //Machine.interfer_prod_loader_unloading = false;
                    AutoStep = AUTOSTEP.UNLOADING_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_AVOID_LOC:
                    if (CheckStopBit())
                        break;

                    //Machine.interfer_prod_loader_unloading = true;
                    SetMessage((int)MSG.MSG_MOVE_TRAYTRF_AVOID_POS);

                    AutoStep = AUTOSTEP.UNLOADING_AVOID_LOC_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_AVOID_LOC_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.UNLOADING_CHECK;
                    break;

                case AUTOSTEP.ALIGNMENT:
                    if (Machine.param.Option(ParameterDefine.OPTION.USE_PICKER_UNDER_VISION) == 1) SetMessage((int)MSG.MSG_ALIGN_PRODUCT);
                    else ResetUnderVisionData();
                    AutoStep = AUTOSTEP.ALIGNMENT_COMPL_CHECK;
                    break;

                case AUTOSTEP.ALIGNMENT_COMPL_CHECK:
                    if (!Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.ALIGNMENT2;
                    CheckStopBit();
                    break;

                case AUTOSTEP.ALIGNMENT2:
                    if (Machine.param.Option(ParameterDefine.OPTION.USE_PICKER_UNDER_VISION) == 1) SetMessage((int)MSG.MSG_ALIGN2_PRODUCT);
                    else ResetUnderVisionData();
                    AutoStep = AUTOSTEP.ALIGNMENT2_COMPL_CHECK;
                    break;

                case AUTOSTEP.ALIGNMENT2_COMPL_CHECK:
                    if (!Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.UNLOADING_CHECK;
                    CheckStopBit();
                    break;
            }

            if (AutoStep != AUTOSTEP.IDLE && AutoStep != AUTOSTEP.ERROR && AutoStep != AUTOSTEP.STOP)
                lastAutoStep = (int)AutoStep;
        }

        private void OnProcessOfInterface()
        {
            double actpos = 0;
            double paramVal = 0;

            switch ((STEP)Step)
            {
                case STEP.IF_LOADING_POSSIBLE_WAIT:
                    if (!Machine.Parts[(int)UNITPART.TRAY].unloadingRequest || !Machine.trayTransferReady)
                        break;

                    NextStep();
                    break;

                case STEP.IF_LOADING_BUSY:
                    if (!Machine.Parts[(int)UNITPART.TRAY].unloading)
                        break;

                    Machine.Parts[(int)UNITPART.PROD_PICK1].loading = true;
                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (Machine.Parts[(int)UNITPART.TRAY].unloading)
                        break;

                    Machine.Parts[(int)UNITPART.PROD_PICK1].loading = false;

                    if (isVacOnComplete)
                    {
                        var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
                        if (!Machine.trayNGListX.Contains(proc.productPosition[targetCurr].x)
                            && !Machine.trayNGListY.Contains(proc.productPosition[targetCurr].y))
                        {
                            Machine.trayNGListX.Add(proc.productPosition[targetCurr].x);
                            Machine.trayNGListY.Add(proc.productPosition[targetCurr].y);
                            ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] " +
                                $"Add to Tray NG List {targetCurr}: [{proc.productPosition[targetCurr].x}, {proc.productPosition[targetCurr].y}]");
                        }
                        Machine.Parts[current[targetCurr]].exist = true;
                        finalPickRetryCount = 0;
                    }
                    else
                    {
                        if (finalPickRetryCount <= 3)
                        {
                            finalPickRetryCount++;
                            Util.Delay(300);
                        }
                        else
                        {
                            finalPickRetryCount = 0;
                            if (targetCurr == 0)
                            {
                                SetError(ECODE.TIMEOUT_PROD_PICK1_VACON);
                                break;
                            }
                            if (targetCurr == 1)
                            {
                                SetError(ECODE.TIMEOUT_PROD_PICK2_VACON);
                                break;
                            }
                        }
                    }
                    NextStep();
                    break;

                case STEP.IF_LOADING_EMPTY_CHECK:
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Z, ref actpos);
                    paramVal = 1000 * (Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Z_READY_POS) + 10.0);

                    if (Machine.Parts[(int)UNITPART.TRAY].productCount > 2 || actpos > paramVal)
                        NextStep();
                    else
                        Step = STEP.MOVE_XYR_READY_POS;
                    break;

                case STEP.IF_UNLOADING_REQUEST:
                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_UNLOADING_BUSY:
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:
                    Machine.Parts[(int)UNITPART.PROD_PICK1].exist = false;
                    Machine.Parts[(int)UNITPART.PROD_PICK2].exist = false;
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_SEND_DATA:
                    if (targetCurr == 0)
                    {
                        Machine.Parts[(int)UNITPART.BUF1_L1].exist = Machine.Parts[(int)UNITPART.PROD_PICK1].exist;
                        Machine.Parts[(int)UNITPART.BUF1_L2].exist = Machine.Parts[(int)UNITPART.PROD_PICK2].exist;
                    }
                    if (targetCurr == 1)
                    {
                        Machine.Parts[(int)UNITPART.BUF1_R1].exist = Machine.Parts[(int)UNITPART.PROD_PICK1].exist;
                        Machine.Parts[(int)UNITPART.BUF1_R2].exist = Machine.Parts[(int)UNITPART.PROD_PICK2].exist;
                    }

                    NextStep();
                    break;

                case STEP.IF_UNLOADING_SEND_DATA_L:
                    if (targetCurr == 0)
                        Machine.Parts[(int)UNITPART.BUF1_L1].exist = Machine.Parts[(int)UNITPART.PROD_PICK1].exist;
                    if (targetCurr == 1)
                        Machine.Parts[(int)UNITPART.BUF1_R1].exist = Machine.Parts[(int)UNITPART.PROD_PICK1].exist;

                    NextStep();
                    break;

                case STEP.IF_UNLOADING_SEND_DATA_R:
                    if (targetCurr == 0)
                        Machine.Parts[(int)UNITPART.BUF1_L2].exist = Machine.Parts[(int)UNITPART.PROD_PICK2].exist;
                    if (targetCurr == 1)
                        Machine.Parts[(int)UNITPART.BUF1_R2].exist = Machine.Parts[(int)UNITPART.PROD_PICK2].exist;

                    NextStep();
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            double actual_ypos = 0;
            double actual_xpos = 0;
            double target_pos = 0;
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            switch ((STEP)Step)
            {
                case STEP.MOVE_XYR_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_X))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_Y))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_R1))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_R2))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_XY_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_X))
                        break;
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_Y))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_Z_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.PROD_PICKUP_Z))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_XYR_READY_POS:
                case STEP.MOVE_XYR_TRAY_VISION_AVOID_POS:
                case STEP.MOVE_XYR_TRAY_TRF_AVOID_POS:
                    if (Step == STEP.MOVE_XYR_READY_POS) MovePickerXYR(0, XYRPOS.READY);
                    if (Step == STEP.MOVE_XYR_TRAY_VISION_AVOID_POS) MovePickerXYR(0, XYRPOS.TRAY_VISION_AVOID);
                    if (Step == STEP.MOVE_XYR_TRAY_TRF_AVOID_POS) MovePickerXYR(0, XYRPOS.TRAY_TRF_AVOID);
                    Step = STEP.MOVE_XYR_POS_CHECK;
                    break;

                case STEP.MOVE_XYR_PICKUP_POS:
                case STEP.MOVE_XYR_PICKUP_POS_OVERLAP:
                    if (Machine.trayTransferReady == false) break;

                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref actual_xpos);
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actual_ypos);
                    double ifX_R = 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_R_POS);
                    double ifX_L = 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_L_POS);
                    if (actual_xpos >= ifX_R || actual_xpos <= ifX_L)
                    {
                        if (actual_ypos < 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_RELEASE_Y_POS))
                        {
                            Step = STEP.MOVE_XYR_TRAY_VISION_AVOID_POS;
                            StepIndex--;
                            break;
                        }
                    }

                    if (targetCurr == 0) MovePickerXYR(targetCurr, XYRPOS.PICK1);
                    if (targetCurr == 1) MovePickerXYR(targetCurr, XYRPOS.PICK2);

                    if (Step == STEP.MOVE_XYR_PICKUP_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_XYR_POS_CHECK;
                    else if (Step == STEP.MOVE_XYR_PICKUP_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_XYR_PLACE_L_POS:
                case STEP.MOVE_XYR_PLACE_R_POS:
                case STEP.MOVE_XYR_PLACE_L_POS_OVERLAP:
                case STEP.MOVE_XYR_PLACE_R_POS_OVERLAP:
                    bool isLeftSide = Step == STEP.MOVE_XYR_PLACE_L_POS || Step == STEP.MOVE_XYR_PLACE_L_POS_OVERLAP;
                    if (Machine.interfer_buffer_transfer_prod_loader[targetCurr])
                    {
                        if (Machine.sysMode != Machine.SYSMODE.AUTO)
                            SetError(ECODE.INTERFERENCE_CAM_TRF);
                        break;
                    }
                    if (!Machine.buffer_ready_to_place[targetCurr]) break;

                    if (targetCurr == 0)
                    {
                        if (isLeftSide)
                            target_pos = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_L_POS);
                        else
                            target_pos = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_R_POS);
                    }
                    if (targetCurr == 1)
                    {
                        if (isLeftSide) target_pos = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_L_POS);
                        else target_pos = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_R_POS);
                    }

                    ifX_R = Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_R_POS) + 20;
                    ifX_L = Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_L_POS);

                    if (target_pos >= ifX_R || target_pos <= ifX_L)
                    {
                        Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actual_ypos);
                        if (actual_ypos > 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_RELEASE_Y_POS))
                        {
                            MovePickerXYR(0, XYRPOS.BUF2SAFE);
                            Step = STEP.MOVE_XYR_POS_CHECK;
                            StepIndex--;
                            break;
                        }
                    }

                    if (targetCurr == 0)
                    {
                        if (isLeftSide) MovePickerXYR(0, XYRPOS.BUF_L_L);
                        else MovePickerXYR(0, XYRPOS.BUF_L_R);
                    }
                    if (targetCurr == 1)
                    {
                        if (isLeftSide) MovePickerXYR(0, XYRPOS.BUF_R_L);
                        else MovePickerXYR(0, XYRPOS.BUF_R_R);
                    }

                    if (targetCurr == 0 || Step == STEP.MOVE_XYR_PLACE_L_POS || Step == STEP.MOVE_XYR_PLACE_R_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_XYR_POS_CHECK;
                    else if (targetCurr == 0 && (Step == STEP.MOVE_XYR_PLACE_L_POS_OVERLAP || Step == STEP.MOVE_XYR_PLACE_R_POS_OVERLAP))
                        NextStep();
                    else Step = STEP.MOVE_XYR_POS_CHECK;
                    break;
                case STEP.MOVE_XYR_ALIGN_POS:
                    if (targetCurr == 0) MovePickerXYR(0, XYRPOS.ALIGN1);
                    if (targetCurr == 1) MovePickerXYR(0, XYRPOS.ALIGN2);
                    Step = STEP.MOVE_XYR_POS_CHECK;
                    break;

                case STEP.MOVE_Z_READY_POS:
                case STEP.MOVE_Z_READY_POS_OVERLAP:
                    MovePickerZ(ZPOS.READY);

                    if (Step == STEP.MOVE_Z_READY_POS || Machine.sysMode != Machine.SYSMODE.AUTO) Step = STEP.MOVE_Z_POS_CHECK;
                    else if (Step == STEP.MOVE_Z_READY_POS_OVERLAP) NextStep();
                    break;

                case STEP.MOVE_Z_PICKUP_READY_POS:
                    MovePickerZ(ZPOS.PICK_READY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PICKUP_UP_READY_POS:
                    MovePickerZ(ZPOS.PICK_UP_READY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PICKUP_UP_READY_POS_FAST:
                    MovePickerZ(ZPOS.PICK_UP_READY_FAST);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PLACE_READY_POS:
                    if (Machine.interfer_buffer_transfer_prod_loader[targetCurr])
                    {
                        if (Machine.sysMode != Machine.SYSMODE.AUTO)
                            SetError(ECODE.INTERFERENCE_CAM_TRF);
                        break;
                    }

                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actual_ypos);
                    if (actual_ypos > 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_L_POS) + 70)
                        || actual_ypos > 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_R_POS) + 70))
                    {
                        if (Machine.sysMode == Machine.SYSMODE.AUTO)
                            break;
                        else
                        {
                            SetError(ECODE.POSITION_ERROR_PROD_PICKER_PLACE_POS);
                            break;
                        }
                    }

                    MovePickerZ(ZPOS.PLACEREADY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PLACE_UP_READY_POS:
                    if (Machine.interfer_buffer_transfer_prod_loader[targetCurr])
                    {
                        if (Machine.sysMode != Machine.SYSMODE.AUTO)
                            SetError(ECODE.INTERFERENCE_CAM_TRF);
                        break;
                    }

                    MovePickerZ(ZPOS.PLACEUPREADY);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_TRAY_CAL_POS:
                    MovePickerZ(ZPOS.TRAYCAL);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PICKER_CAL_POS:
                    MovePickerZ(ZPOS.PICKERCAL);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PICKUP_POS:
                    MovePickerZ(ZPOS.PICK);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_PLACE_POS:
                    if (Machine.interfer_buffer_transfer_prod_loader[targetCurr])
                    {
                        if (Machine.sysMode != Machine.SYSMODE.AUTO)
                            SetError(ECODE.INTERFERENCE_CAM_TRF);
                        break;
                    }

                    MovePickerZ(ZPOS.PLACE);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_UNDER_ALIGN_POS:
                    MovePickerZ(ZPOS.UNDER_ALIGN);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;
            }
        }

        private void OnProcessOfIO()
        {
            int[] outToolUp = { (int)DO.CAM_PICKER_Z1_UP, (int)DO.CAM_PICKER_Z2_UP };
            int[] outToolDown = { (int)DO.CAM_PICKER_Z1_DOWN, (int)DO.CAM_PICKER_Z2_DOWN };
            int[] outToolVac = { (int)DO.CAM_PICKER_Z1_VACON, (int)DO.CAM_PICKER_Z2_VACON };
            int[] outToolPurge = { (int)DO.CAM_PICKER_Z1_PURGE, (int)DO.CAM_PICKER_Z2_PURGE };
            int[] inToolUp = { (int)DI.CAM_PICKER_Z1_UP, (int)DI.CAM_PICKER_Z2_UP };
            int[] inToolDown = { (int)DI.CAM_PICKER_Z1_DOWN, (int)DI.CAM_PICKER_Z2_DOWN };
            int[] inToolVac = { (int)DI.CAM_PICKER_Z1_VACON, (int)DI.CAM_PICKER_Z2_VACON };

            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            switch ((STEP)Step)
            {
                case STEP.PICKER_UP:
                case STEP.PICKER_UP_BYPASS:
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
                        if (targetCurr == 0) SetError(ECODE.TIMEOUT_PROD_PICK1_UP);
                        if (targetCurr == 1) SetError(ECODE.TIMEOUT_PROD_PICK2_UP);
                        break;
                    }

                    Machine.IO.GetIn(inToolUp[targetCurr], ref ret1);
                    Machine.IO.GetIn(inToolDown[targetCurr], ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.PICKER_UP_ALL:
                case STEP.PICKER_UP_ALL_BYPASS:
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_UP_ALL) Step = STEP.PICKER_UP_ALL_CHECK;
                    if (Step == STEP.PICKER_UP_ALL_BYPASS) NextStep();
                    break;

                case STEP.PICKER_UP_ALL_CHECK:
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_DOWN, ref ret2);
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_UP, ref ret3);
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_DOWN, ref ret4);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (ret1 != 1 || ret2 != 0)
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK1_UP);
                            break;
                        }
                        if (ret3 != 1 || ret4 != 0)
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK2_UP);
                            break;
                        }
                    }

                    if (ret1 == 1 && ret2 == 0 && ret3 == 1 && ret4 == 0)
                        NextStep();
                    break;

                case STEP.PICKER_DOWN:
                case STEP.PICKER_DOWN_BYPASS:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut(outToolUp[targetCurr], 0);
                    Machine.IO.SetOut(outToolDown[targetCurr], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_DOWN) Step = STEP.PICKER_DOWN_CHECK;
                    if (Step == STEP.PICKER_DOWN_BYPASS) NextStep();
                    break;

                case STEP.PICKER_DOWN_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (targetCurr == 0) SetError(ECODE.TIMEOUT_PROD_PICK1_DOWN);
                        if (targetCurr == 1) SetError(ECODE.TIMEOUT_PROD_PICK2_DOWN);
                        break;
                    }


                    Machine.IO.GetIn(inToolUp[targetCurr], ref ret1);
                    Machine.IO.GetIn(inToolDown[targetCurr], ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_DOWN_L:
                case STEP.PICKER_DOWN_L_BYPASS:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut(outToolUp[0], 0);
                    Machine.IO.SetOut(outToolDown[0], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    if (Step == STEP.PICKER_DOWN_L) Step = STEP.PICKER_DOWN_L_CHECK;
                    if (Step == STEP.PICKER_DOWN_L_BYPASS) NextStep();
                    break;

                case STEP.PICKER_DOWN_L_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.GetIn(inToolUp[0], ref ret1);
                    Machine.IO.GetIn(inToolDown[0], ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_DOWN_R:
                case STEP.PICKER_DOWN_R_BYPASS:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut(outToolUp[1], 0);
                    Machine.IO.SetOut(outToolDown[1], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    if (Step == STEP.PICKER_DOWN_R) Step = STEP.PICKER_DOWN_R_CHECK;
                    if (Step == STEP.PICKER_DOWN_R_BYPASS) NextStep();
                    break;

                case STEP.PICKER_DOWN_R_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.GetIn(inToolUp[1], ref ret1);
                    Machine.IO.GetIn(inToolDown[1], ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_DOWN_ALL:
                case STEP.PICKER_DOWN_ALL_BYPASS:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.PICKER_DOWN_ALL) Step = STEP.PICKER_DOWN_ALL_CHECK;
                    if (Step == STEP.PICKER_DOWN_ALL_BYPASS) NextStep();
                    break;

                case STEP.PICKER_DOWN_ALL_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_DOWN, ref ret2);
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_UP, ref ret3);
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_DOWN, ref ret4);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        if (ret1 != 0 || ret2 != 1)
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK1_DOWN);
                            break;
                        }
                        if (ret3 != 0 || ret4 != 1)
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK2_DOWN);
                            break;
                        }
                    }

                    if (ret1 == 0 && ret2 == 1 && ret3 == 0 && ret4 == 1)
                        NextStep();
                    break;

                case STEP.PICKER_VACON:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    Machine.IO.SetOut(outToolVac[targetCurr], 1);
                    Machine.IO.SetOut(outToolPurge[targetCurr], 0);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACON_CHECK;
                    break;

                case STEP.PICKER_VACON_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.GetIn(inToolVac[targetCurr], ref ret1);

                    if (ret1 == 0)
                    {
                        if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_WAIT_TIME) && Machine.sysMode == Machine.SYSMODE.AUTO)//&& !Machine.Parts[(int)UNITPART.PROD_PICK1].adjust)
                        {
                            if (pickRetryCount > 7)
                            {
                                isVacOnComplete = false;
                                NextStep();
                                break;
                            }

                            pickRetryCount++;

                            Step = STEP.MOVE_Z_PICKUP_POS;
                            StepIndex--;
                            //Machine.Parts[(int)UNITPART.PROD_PICK1].adjust = true;
                        }
                        break;
                    }

                    isVacOnComplete = true;
                    NextStep();
                    break;

                case STEP.PICKER_VACOFF:
                    Machine.IO.SetOut(outToolVac[targetCurr], 0);
                    Machine.IO.SetOut(outToolPurge[targetCurr], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACOFF_CHECK;
                    break;

                case STEP.PICKER_VACOFF_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        if (targetCurr == 0) SetError(ECODE.TIMEOUT_ASSY_PICK1_VACOFF);
                        if (targetCurr == 1) SetError(ECODE.TIMEOUT_ASSY_PICK2_VACOFF);
                        break;
                    }

                    Machine.IO.GetIn(outToolVac[targetCurr], ref ret1);

                    if (ret1 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));

                        Machine.IO.SetOut(outToolPurge[targetCurr], 0);
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACON_L:
                    Machine.IO.SetOut(outToolVac[0], 1);
                    Machine.IO.SetOut(outToolPurge[0], 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACON_L_CHECK;
                    break;

                case STEP.PICKER_VACON_L_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_ASSY_PICK1_VACON);
                        break;
                    }

                    Machine.IO.GetIn(inToolVac[0], ref ret1);

                    if (ret1 == 1)
                    {
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACON_R:
                    Machine.IO.SetOut(outToolVac[1], 1);
                    Machine.IO.SetOut(outToolPurge[1], 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACON_R_CHECK;
                    break;

                case STEP.PICKER_VACON_R_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_ASSY_PICK2_VACON);
                        break;
                    }

                    Machine.IO.GetIn(inToolVac[1], ref ret1);

                    if (ret1 == 1)
                    {
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACOFF_L:
                    Machine.IO.SetOut(outToolVac[0], 0);
                    Machine.IO.SetOut(outToolPurge[0], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.PICKER_VACOFF_L_CHECK;
                    break;

                case STEP.PICKER_VACOFF_L_CHECK:
                    Machine.IO.GetIn(outToolVac[0], ref ret1);

                    if (ret1 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(TIME.PURGE_OFF_WAIT_TIME));

                        Machine.IO.SetOut(outToolPurge[0], 0);
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACOFF_R:
                    Machine.IO.SetOut(outToolVac[1], 0);
                    Machine.IO.SetOut(outToolPurge[1], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.PICKER_VACOFF_R_CHECK;
                    break;

                case STEP.PICKER_VACOFF_R_CHECK:
                    Machine.IO.GetIn(outToolVac[1], ref ret1);

                    if (ret1 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(TIME.PURGE_OFF_WAIT_TIME));

                        Machine.IO.SetOut(outToolPurge[1], 0);
                        NextStep();
                    }
                    break;

                case STEP.PICKER_VACOFF_ALL:
                    Machine.IO.SetOut(outToolVac[0], 0);
                    Machine.IO.SetOut(outToolPurge[0], 1);
                    Machine.IO.SetOut(outToolVac[1], 0);
                    Machine.IO.SetOut(outToolPurge[1], 1);

                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PICKER_VACOFF_ALL_CHECK;
                    break;

                case STEP.PICKER_VACOFF_ALL_CHECK:
                    Machine.IO.GetIn(inToolVac[0], ref ret1);
                    Machine.IO.GetIn(inToolVac[1], ref ret2);

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        if (ret1 != 0) SetError(ECODE.TIMEOUT_ASSY_PICK1_VACOFF);
                        if (ret2 != 0) SetError(ECODE.TIMEOUT_ASSY_PICK2_VACOFF);
                        break;
                    }
                    if (ret1 == 1 || ret2 == 1)
                        break;
                    if (ret1 == 0 && ret2 == 0)
                    {
                        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));

                        Machine.IO.SetOut(outToolPurge[0], 0);
                        Machine.IO.SetOut(outToolPurge[1], 0);
                        Step = STEP.PURGE_OFF_WAIT;
                    }
                    break;

                case STEP.PICKER_VACON_ALL:
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

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT) && Machine.sysMode == Machine.SYSMODE.AUTO)//&& !Machine.Parts[(int)UNITPART.PROD_PICK1].adjust)
                    {
                        if (ret1 != 1)
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK1_VACON);
                            break;
                        }
                        if (ret2 != 1)
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK2_VACON);
                            break;
                        }
                    }

                    if (ret1 == 0 || ret2 == 0)
                        break;

                    NextStep();
                    break;

                case STEP.CAM_BUF_DETECT_ON_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep(); break;
                    }
                    if (targetCurr == 0)
                    {
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_1, ref ret1);
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_2, ref ret2);

                        if (ret1 == 1 && ret2 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                if (ret1 == 0)
                                {
                                    SetError(ECODE.TIMEOUT_BUF1_L1_DETECT);
                                    break;
                                }
                                else
                                {
                                    SetError(ECODE.TIMEOUT_BUF1_L2_DETECT);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_3, ref ret1);
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_4, ref ret2);

                        if (ret1 == 1 && ret2 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                if (ret1 == 0)
                                {
                                    SetError(ECODE.TIMEOUT_BUF1_R1_DETECT);
                                    break;
                                }
                                else
                                {
                                    SetError(ECODE.TIMEOUT_BUF1_R2_DETECT);
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case STEP.CAM_BUF_DETECT_ON_CHECK_L:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep(); break;
                    }
                    if (targetCurr == 0)
                    {
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_1, ref ret1);

                        if (ret1 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                SetError(ECODE.TIMEOUT_BUF1_L1_DETECT);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_3, ref ret1);

                        if (ret1 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                SetError(ECODE.TIMEOUT_BUF1_R1_DETECT);
                                break;
                            }
                        }
                    }
                    break;

                case STEP.CAM_BUF_DETECT_ON_CHECK_R:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep(); break;
                    }
                    if (targetCurr == 0)
                    {
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_2, ref ret2);

                        if (ret2 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                SetError(ECODE.TIMEOUT_BUF1_L2_DETECT);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Machine.IO.GetIn((int)DI.DETACH_SENSOR_4, ref ret2);

                        if (ret2 == 1)
                            NextStep();
                        else
                        {
                            if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                            {
                                SetError(ECODE.TIMEOUT_BUF1_R2_DETECT);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        private void OnProcessOfPickerVision()
        {
            switch ((STEP)Step)
            {
                case STEP.LIGHT_ON:
                    Vision.Camera.LightSet((int)CAMERA.PICKER, Vision.inspection.recipeData[(int)SystemDefine.CAMERA.PICKER].inspInfo.lightSet);
                    Vision.Camera.LightOn((int)CAMERA.PICKER, true);
                    NextStep();
                    break;

                case STEP.LIGHT_OFF:
                    Vision.Camera.LightOn((int)CAMERA.PICKER, false);
                    NextStep();
                    break;
                case STEP.PROC_ALIGN_START:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        alignSucs[0] = 0;
                        alignSucs[1] = 0;
                        NextStep();
                    }
                    else
                    {
                        receivedData = false;
                        if (!Vision.RunGrab("PICKER", false, true))
                            break;
                        bool saveTrainImage = Machine.param.Option(ParameterDefine.OPTION.USE_IMAGE_GATHERING_FOR_ASSEMBLY_DEFECT) == 1 ? true : false;
                        if (!Vision.Run("PICKER", true, false, saveTrainImage))
                            break;
                        timeWait[(int)TIMER.TIMEOUT].Start();
                        Step = STEP.PROC_ALIGN_COMPL_CHECK;
                    }
                    break;

                case STEP.PROC_ALIGN_COMPL_CHECK:
                    if (CheckStopBit())
                        break;

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VISION_TIME_OUT))
                    {
                        if (iRetryVision >= 10)
                        {
                            if (targetCurr == 0 && alignSucs[0] < 0)
                            {
                                SetError(ECODE.ALIGN_ERROR_PICKER_SEARCH_FAIL_LEFT_TARGET);
                                break;
                            }

                            if (targetCurr == 1 && alignSucs[1] < 0)
                            {
                                SetError(ECODE.ALIGN_ERROR_PICKER_SEARCH_FAIL_RIGHT_TARGET);
                                break;
                            }
                        }

                        iRetryVision++;
                        Step = STEP.PROC_ALIGN_START;
                        break;
                    }
                    if (!receivedData)
                        break;

                    if (iRetryVision < 5)
                    {
                        if (targetCurr == 0 && alignSucs[0] < 0)
                        {
                            iRetryVision++;
                            Step = STEP.PROC_ALIGN_START;
                            break;
                        }
                        if (targetCurr == 1 && alignSucs[1] < 0)
                        {
                            iRetryVision++;
                            Step = STEP.PROC_ALIGN_START;
                            break;
                        }
                    }
                    else
                    {
                        if (targetCurr == 0 && alignSucs[0] < 0)
                        {
                            SetError(ECODE.ALIGN_ERROR_PICKER_SEARCH_FAIL_LEFT_TARGET);
                            break;
                        }
                        if (targetCurr == 1 && alignSucs[1] < 0)
                        {
                            SetError(ECODE.ALIGN_ERROR_PICKER_SEARCH_FAIL_RIGHT_TARGET);
                            break;
                        }
                    }
                    NextStep();
                    break;

                case STEP.PROC_ALIGN_ERROR_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                        NextStep();
                    else
                    {
                        if (iAlignErrorCheckCount > 20)
                        {
                            iAlignErrorCheckCount = 0;

                            Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.RESULT, false);
                            Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.FAIL, true);
                            SetError(ECODE.ALIGN_ERROR_PICKER_ALIGN_CHECK_COUNT_OVER);
                            break;
                        }
                        ++iAlignErrorCheckCount;

                        Point2d camCenter = Vision.GetCamCenter("PICKER");
                        double gapXpx = camCenter.x - productPosition[targetCurr].x;
                        double gapYpx = camCenter.y - productPosition[targetCurr].y;

                        if (Machine.param.Option(ParameterDefine.OPTION.USE_PICKER_XY_SATURATION) == 0)
                        {
                            if (Math.Abs(productAngle[targetCurr]) > 5)
                            {
                                adjustAngle[targetCurr] -= productAngle[targetCurr];
                                if (adjustAngle[targetCurr] >= 185) adjustAngle[targetCurr] -= 360;
                                else if (adjustAngle[targetCurr] <= -185) adjustAngle[targetCurr] += 360;

                                int nIndex = GetStepIndex(StepList, STEP.MOVE_XYR_ALIGN_POS);
                                if (nIndex >= 0)
                                    Step = StepList[StepIndex = nIndex];
                                break;

                            }
                            adjustAngle[targetCurr] = 0;
                            (double gapXmm, double gapYmm) = Vision.ConvertPxToMmByDistance(CAMERA.PICKER, (TOOL_TYPE)targetCurr, gapXpx, gapYpx);
                            adjustAngle[targetCurr] -= productAngle[targetCurr];
                            if (adjustAngle[targetCurr] >= 185) adjustAngle[targetCurr] -= 360;
                            else if (adjustAngle[targetCurr] <= -185) adjustAngle[targetCurr] += 360;

                            double actualPos = 0;
                            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref actualPos);
                            alignCenterPos[targetCurr].x = actualPos / 1000;

                            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actualPos);
                            alignCenterPos[targetCurr].y = actualPos / 1000;

                            (double alignX, double alignY) = Vision.InterpolateMM(CAMERA.PICKER, (TOOL_TYPE)targetCurr, productPosition[targetCurr].x, productPosition[targetCurr].y);

                            Point2d rot = Vision.RotationTransformationForSingle(adjustAngle[targetCurr], alignX, alignY, alignCenterPos[targetCurr].x, alignCenterPos[targetCurr].y);

                            alignCenterPos[targetCurr].x = alignCenterPos[targetCurr].x + gapXmm + rot.x;
                            alignCenterPos[targetCurr].y = alignCenterPos[targetCurr].y + gapYmm + rot.y;


                            if (targetCurr == 0) Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_R1, ref actualPos);
                            if (targetCurr == 1) Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_R2, ref actualPos);
                            alignCenterAngle[targetCurr] = actualPos / 1000;
                            alignCenterAngle[targetCurr] += adjustAngle[targetCurr];

                            Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.RESULT, false);
                            Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.SUCCESS, true);
                            NextStep();
                            return;
                        }

                        if (Math.Abs(productAngle[targetCurr]) > 0.1
                            || (Math.Abs(gapXpx) > 5)
                            || (Math.Abs(gapYpx) > 5))
                        {
                            (double gapXmm, double gapYmm) = Vision.ConvertPxToMmByDistance(CAMERA.PICKER, (TOOL_TYPE)targetCurr, gapXpx, gapYpx);
                            adjustAngle[targetCurr] -= productAngle[targetCurr];
                            if (adjustAngle[targetCurr] >= 185) adjustAngle[targetCurr] -= 360;
                            else if (adjustAngle[targetCurr] <= -185) adjustAngle[targetCurr] += 360;

                            adjustPos[targetCurr].x += gapXmm;
                            adjustPos[targetCurr].y += gapYmm;

                            int nIndex = GetStepIndex(StepList, STEP.MOVE_XYR_ALIGN_POS);
                            if (nIndex >= 0)
                                Step = StepList[StepIndex = nIndex];
                        }
                        else
                        {
                            double pos;
                            double actualPos = 0;

                            if (targetCurr == 0)
                            {
                                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref actualPos);
                                alignCenterPos[0].x = actualPos / 1000;

                                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actualPos);
                                alignCenterPos[0].y = actualPos / 1000;

                                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_R1, ref actualPos);
                                alignCenterAngle[0] = actualPos / 1000;
                            }
                            else
                            {
                                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref actualPos);
                                alignCenterPos[1].x = actualPos / 1000;

                                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actualPos);
                                alignCenterPos[1].y = actualPos / 1000;

                                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_R2, ref actualPos);
                                alignCenterAngle[1] = actualPos / 1000;
                            }
                            Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.RESULT, false);
                            Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.SUCCESS, true);
                            NextStep();
                        }
                    }
                    break;
            }
        }

        private void OnProcessOfCalibration()
        {
            double dActualPosX = 0, dActualPosY = 0;
            double[] xy = new double[2];
            switch ((STEP)Step)
            {
                #region MOTION
                case STEP.GENERATE_TRAY_MAP:
                    double startX = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_START_POS_X);
                    double startY = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_START_POS_Y);
                    double endX = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_END_POS_X);
                    double endY = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_END_POS_Y);
                    double step = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_STEP);
                    Vision.GenerateMoveStep(startX, startY, endX, endY, step);
                    retryCount = 0;
                    NextStep();
                    break;

                case STEP.MOVE_XY_MAPPING_TRAY:
                    if (Vision.IsMapCalDone())
                    {
                        Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IF_MAPPING_DONE_CHECK_TRAY)];
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

                case STEP.MOVE_XY_MAPPING_TRAY_START_XY:
                    xy[0] = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_START_POS_X);
                    xy[1] = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_START_POS_Y);
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_MAPPING_TRAY_END_XY:
                    xy[0] = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_END_POS_X);
                    xy[1] = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_END_POS_Y);
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_RECAL_POS_XY:
                    xy[0] = this.reCalPointList[reCalIndex].Xmm;
                    xy[1] = this.reCalPointList[reCalIndex].Ymm;
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.GENERATE_PICKER_MAP:
                    if (targetCurr == 0)
                    {
                        startX = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_START_POS_X);
                        startY = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_START_POS_Y);
                        endX = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_END_POS_X);
                        endY = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_END_POS_Y);
                    }
                    else
                    {
                        startX = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_START_POS_X);
                        startY = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_START_POS_Y);
                        endX = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_END_POS_X);
                        endY = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_END_POS_Y);
                    }
                    step = Machine.param.Calibration(CALIBRATION.PICKER_CAL_MAP_STEP);
                    Vision.GenerateMoveStep(startX, startY, endX, endY, step);
                    retryCount = 0;
                    NextStep();
                    break;

                case STEP.MOVE_XY_MAPPING_PICKER:
                    if (Vision.IsMapCalDone())
                    {
                        Step = StepList[StepIndex = GetStepIndex(StepList, STEP.IF_MAPPING_DONE_CHECK_PICKER)];
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

                case STEP.MOVE_XY_MAPPING_PICKER_START_XY:
                    if (targetCurr == 0)
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_START_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_START_POS_Y);
                    }
                    else
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_START_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_START_POS_Y);
                    }
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;

                case STEP.MOVE_XY_MAPPING_PICKER_END_XY:
                    if (targetCurr == 0)
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_END_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_END_POS_Y);
                    }
                    else
                    {
                        xy[0] = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_END_POS_X);
                        xy[1] = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_END_POS_Y);
                    }
                    MovePickerXYForMapping(xy);
                    Step = STEP.MOVE_XY_POS_CHECK;
                    break;
                #endregion

                #region CHECK_MAP_DATA
                case STEP.IF_MAPPING_DONE_CHECK_TRAY:
                    if (Vision.IsMissedPointReachLimit(CAMERA.TRAY, TOOL_TYPE.LEFT))
                    {
                        SetError(ECODE.CAL_ERROR_TRAY_LEFT_MISSEED_POINT_MAX);
                        break;
                    }
                    if (Vision.IsMissedPointReachLimit(CAMERA.TRAY, TOOL_TYPE.RIGHT))
                    {
                        SetError(ECODE.CAL_ERROR_TRAY_RIGHT_MISSEED_POINT_MAX);
                        break;
                    }
                    Vision.StopTTStopWatch();
                    if (Vision.IsMapCalDone())
                    {
                        NextStep();
                        break;
                    }

                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(OPTION.VISION_CAL_CENTER_LENGTH_TRAY);

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;
                        if (Vision.GetPixelCamPos("TRAY", tool) == null)
                        {
                            if (retryCount < 3)
                            {
                                retryCount++;
                                Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_TRAY)];
                                return;
                            }
                        }
                    }

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;

                        if (Vision.GetPixelCamPos("TRAY", tool) == null)
                            Vision.AddNGPointToCalMap(CAMERA.TRAY, tool, dActualPosX, dActualPosY);
                        else
                            Vision.AddOKPointToCalMap(CAMERA.TRAY, tool, dActualPosX, dActualPosY, Vision.GetPixelCamPos("TRAY", tool).x, Vision.GetPixelCamPos("TRAY", tool).y);
                    }

                    Vision.IncreaseCalMapIndex();
                    retryCount = 0;
                    Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_TRAY)];
                    break;

                case STEP.IF_RECAL_COMPLETE_TRAY:
                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_TRAY);
                    if (Vision.GetPixelCamPos("TRAY", calibToolType) == null)
                    {
                        SetError(ECODE.CAL_ERROR_CANT_SEARCH_TOOL); break;
                    }
                    else Vision.ChangePointDataInCalMap(CAMERA.TRAY,
                            calibToolType,
                            dActualPosX,
                            dActualPosY,
                            Vision.GetPixelCamPos("TRAY", calibToolType).x,
                            Vision.GetPixelCamPos("TRAY", calibToolType).y);
                    reCalIndex++;
                    if (reCalIndex == reCalPointList.Count()) NextStep();
                    else
                    {
                        int nIndex = GetStepIndex(StepList, STEP.MOVE_XY_RECAL_POS_XY);
                        if (nIndex >= 0)
                            Step = StepList[StepIndex = nIndex];
                    }
                    break;

                case STEP.IF_MAPPING_DONE_CHECK_PICKER:
                    if (Vision.IsMissedPointReachLimit(CAMERA.PICKER, TOOL_TYPE.LEFT))
                    {
                        SetError(ECODE.CAL_ERROR_PICKER_LEFT_MISSEED_POINT_MAX);
                        break;
                    }
                    if (Vision.IsMissedPointReachLimit(CAMERA.PICKER, TOOL_TYPE.RIGHT))
                    {
                        SetError(ECODE.CAL_ERROR_PICKER_RIGHT_MISSEED_POINT_MAX);
                        break;
                    }
                    Vision.StopTTStopWatch();
                    if (Vision.IsMapCalDone())
                    {
                        NextStep();
                        break;
                    }

                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_PICKER);

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;
                        if (Vision.GetPixelCamPos("PICKER", tool) == null)
                        {
                            if (retryCount < 3)
                            {
                                retryCount++;
                                Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_PICKER)];
                                return;
                            }
                        }
                    }

                    foreach (var tool in new[] { TOOL_TYPE.LEFT, TOOL_TYPE.RIGHT })
                    {
                        if (calibToolType != TOOL_TYPE.MAX && calibToolType != tool)
                            continue;

                        if (Vision.GetPixelCamPos("PICKER", tool) == null)
                            Vision.AddNGPointToCalMap(CAMERA.PICKER, tool, dActualPosX, dActualPosY);
                        else
                            Vision.AddOKPointToCalMap(CAMERA.PICKER, tool, dActualPosX, dActualPosY, Vision.GetPixelCamPos("PICKER", tool).x, Vision.GetPixelCamPos("PICKER", tool).y);
                    }

                    Vision.IncreaseCalMapIndex();
                    retryCount = 0;
                    Step = StepList[StepIndex = GetStepIndex(StepList, STEP.MOVE_XY_MAPPING_PICKER)];
                    break;

                case STEP.IF_RECAL_COMPLETE_PICKER:
                    dActualPosX = 0; dActualPosY = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref dActualPosX);
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref dActualPosY);
                    dActualPosX *= 0.001;
                    dActualPosY *= 0.001;
                    dActualPosY -= Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_PICKER);
                    if (Vision.GetPixelCamPos("PICKER", calibToolType) == null)
                    {
                        SetError(ECODE.CAL_ERROR_CANT_SEARCH_TOOL); break;
                    }
                    else Vision.ChangePointDataInCalMap(CAMERA.PICKER,
                            calibToolType,
                            dActualPosX,
                            dActualPosY,
                            Vision.GetPixelCamPos("PICKER", calibToolType).x,
                            Vision.GetPixelCamPos("PICKER", calibToolType).y);
                    reCalIndex++;
                    if (reCalIndex == reCalPointList.Count()) NextStep();
                    else
                    {
                        int nIndex = GetStepIndex(StepList, STEP.MOVE_XY_RECAL_POS_XY);
                        if (nIndex >= 0)
                            Step = StepList[StepIndex = nIndex];
                    }
                    break;

                case STEP.SAVE_MAPPING_FILE_TRAY:
                    string messageContent = string.Empty;
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.LEFT)
                    {
                        Vision.SaveCalibList(CAMERA.TRAY, TOOL_TYPE.LEFT);
                        if (Vision.IsAllCalPointOK(CAMERA.TRAY, TOOL_TYPE.LEFT))
                            messageContent += $"{CAMERA.TRAY}_{TOOL_TYPE.LEFT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.TRAY}_{TOOL_TYPE.LEFT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.TRAY, TOOL_TYPE.LEFT).miss} NG Point). Please ReCheck all NG Point!";
                    }
                    messageContent += ("\n\n");
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.RIGHT)
                    {
                        Vision.SaveCalibList(CAMERA.TRAY, TOOL_TYPE.RIGHT);
                        if (Vision.IsAllCalPointOK(CAMERA.TRAY, TOOL_TYPE.RIGHT))
                            messageContent += $"{CAMERA.TRAY}_{TOOL_TYPE.RIGHT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.TRAY}_{TOOL_TYPE.RIGHT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.TRAY, TOOL_TYPE.RIGHT).miss} NG Point). Please ReCheck all NG Point!";
                    }

                    MessageBox.Show(messageContent);
                    NextStep();
                    break;

                case STEP.SAVE_MAPPING_FILE_PICKER:
                    messageContent = string.Empty;
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.LEFT)
                    {
                        Vision.SaveCalibList(CAMERA.PICKER, TOOL_TYPE.LEFT);
                        if (Vision.IsAllCalPointOK(CAMERA.PICKER, TOOL_TYPE.LEFT))
                            messageContent += $"{CAMERA.PICKER}_{TOOL_TYPE.LEFT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.PICKER}_{TOOL_TYPE.LEFT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.PICKER, TOOL_TYPE.LEFT).miss} NG Point). Please ReCheck all NG Point!";
                    }
                    messageContent += ("\n\n");
                    if (calibToolType == TOOL_TYPE.MAX || calibToolType == TOOL_TYPE.RIGHT)
                    {
                        Vision.SaveCalibList(CAMERA.PICKER, TOOL_TYPE.RIGHT);
                        if (Vision.IsAllCalPointOK(CAMERA.PICKER, TOOL_TYPE.RIGHT))
                            messageContent += $"{CAMERA.PICKER}_{TOOL_TYPE.RIGHT} Calibration completed (0 NG Point). Data will be saved automatically!";
                        else
                            messageContent += $"{CAMERA.PICKER}_{TOOL_TYPE.RIGHT} Calibration completed " +
                                $"({Vision.GetMapStatus(CAMERA.PICKER, TOOL_TYPE.RIGHT).miss} NG Point). Please ReCheck all NG Point!";
                    }

                    MessageBox.Show(messageContent);
                    NextStep();
                    break;
                #endregion

                #region VISION
                case STEP.REQUEST_TRAY_CAL_PIXEL:
                    Vision.Calibration("TRAY", pickRetryCount, currCalibrationPos, true);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.REQUEST_TRAY_CALIBRATION_CHECK;
                    break;

                case STEP.REQUEST_TRAY_CALIBRATION_CHECK:
                    if (CheckStopBit())
                        break;
                    if (Vision.IsCalDataReceived("TRAY"))
                        NextStep();
                    break;

                case STEP.REQUEST_PICKER_CAL_PIXEL:
                    Vision.Calibration("PICKER", pickRetryCount, currCalibrationPos, true);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.REQUEST_PICKER_CALIBRATION_CHECK;
                    break;

                case STEP.REQUEST_PICKER_CALIBRATION_CHECK:
                    if (CheckStopBit())
                        break;
                    if (Vision.IsCalDataReceived("PICKER"))
                        NextStep();
                    Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.RESULT, false);
                    Vision.Capture("PICKER", CaptureUtil.CAPTURETYPE.RAW, true);
                    break;
                    #endregion
            }
        }

        private void MovePickerXYR(int headNo, XYRPOS Pos, double offset_x = 0)
        {
            double[] position = { 0.0, 0.0, 0.0, 0.0 };
            double gapX = 0, gapY = 0, gapR = 0;

            if (Pos == XYRPOS.BUF_L_L || Pos == XYRPOS.BUF_R_L)
            {
                double basePosX = 0, basePosY = 0, basePosR = 0;
                if (Pos == XYRPOS.BUF_L_L)
                {
                    basePosX = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_L_L_POS);
                    basePosY = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_L_L_POS);
                    basePosR = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_L_L_POS);
                }
                else
                {
                    basePosX = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_R_L_POS);
                    basePosY = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_R_L_POS);
                    basePosR = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_R_L_POS);
                }

                if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                {
                    gapX = alignCenterPos[0].x - basePosX;
                    gapY = alignCenterPos[0].y - basePosY;
                    gapR = alignCenterAngle[0] - basePosR;
                }
            }
            else if (Pos == XYRPOS.BUF_L_R || Pos == XYRPOS.BUF_R_R)
            {
                double basePosX = 0, basePosY = 0, basePosR = 0;
                if (Pos == XYRPOS.BUF_L_R)
                {
                    basePosX = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_L_R_POS);
                    basePosY = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_L_R_POS);
                    basePosR = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_L_R_POS);
                }
                else
                {
                    basePosX = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_R_R_POS);
                    basePosY = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_R_R_POS);
                    basePosR = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_R_R_POS);
                }

                if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                {
                    gapX = alignCenterPos[1].x - basePosX;
                    gapY = alignCenterPos[1].y - basePosY;
                    gapR = alignCenterAngle[1] - basePosR;
                }
            }
            if (Machine.param.Option(ParameterDefine.OPTION.USE_PICKER_UNDER_VISION) == 0)
            {
                gapX = 0;
                gapY = 0;
                gapR = 0;
            }
            switch (Pos)
            {
                case XYRPOS.READY:
                    position[0] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_X_READY_POS);
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Y_READY_POS);
                    position[2] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    position[3] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.TRAY_VISION_AVOID:
                    position[0] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_X_READY_POS);
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Y_TRAY_VISION_AVOID_POS);
                    position[2] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    position[3] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.TRAY_TRF_AVOID:
                    position[0] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_X_READY_POS);
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Y_TRAY_TRF_AVOID_POS);
                    position[2] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    position[3] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    MovePickerXYR(position);
                    break;

                case XYRPOS.BUF_L_L:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_L_POS) + gapX;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_L_POS) + gapY;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_L_L_POS) + gapR;
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_L_R_POS) + gapR;
                    MovePickerXYR(position);
                    break;

                case XYRPOS.BUF_L_R:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_R_POS) + gapX;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_R_POS) + gapY;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_L_L_POS) + gapR;
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_L_R_POS) + gapR;
                    MovePickerXYR(position);
                    break;

                case XYRPOS.BUF_R_L:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_L_POS) + gapX;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_R_L_POS) + gapY;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_R_L_POS) + gapR;
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_R_R_POS) + gapR;
                    MovePickerXYR(position);
                    break;

                case XYRPOS.BUF_R_R:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_R_POS) + gapX;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_R_R_POS) + gapY;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_R_L_POS) + gapR;
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_R_R_POS) + gapR;
                    MovePickerXYR(position);
                    break;

                case XYRPOS.BUF2SAFE:
                    position[0] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_X_READY_POS);
                    position[1] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Y_TRAY_VISION_AVOID_POS);
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_R_L_POS) + alignCenterAngle[0] + Convert.ToDouble(isFixAngle180[0]) * 180.0;
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PLACE_R_R_POS) + alignCenterAngle[1] + Convert.ToDouble(isFixAngle180[1]) * 180.0;
                    MovePickerXYR(position);
                    break;

                case XYRPOS.ALIGN1:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_L_L_POS) + adjustPos[0].x;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_L_L_POS) + adjustPos[0].y;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_L_L_POS) + adjustAngle[0];
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_L_R_POS) + adjustAngle[1];
                    MovePickerXYR(position);
                    break;

                case XYRPOS.ALIGN2:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_L_R_POS) + adjustPos[1].x;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_L_R_POS) + adjustPos[1].y;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_L_L_POS) + adjustAngle[0];
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_L_R_POS) + adjustAngle[1];
                    MovePickerXYR(position);
                    break;

                case XYRPOS.ALIGN3:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_R_L_POS) + adjustPos[0].x;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_R_L_POS) + adjustPos[0].y;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_R_L_POS) + adjustAngle[0];
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_R_R_POS) + adjustAngle[1];
                    MovePickerXYR(position);
                    break;

                case XYRPOS.ALIGN4:
                    position[0] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_ALIGN_R_R_POS) + adjustPos[1].x;
                    position[1] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_ALIGN_R_R_POS) + adjustPos[1].y;
                    position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_R_L_POS) + adjustAngle[0];
                    position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_ALIGN_R_R_POS) + adjustAngle[1];
                    MovePickerXYR(position);
                    break;

                case XYRPOS.PICK1:
                case XYRPOS.PICK2:
                    if (Machine.status.mode == SystemMode.SystemModeAUTO)
                    {
                        pickupPos = position = CalculatePickPosXYR();
                        if (position is null || position.Count() != 4) break;
                        MovePickerXYR(position);
                        break;
                    }
                    else if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        break;
                    }
                    else
                    {
                        position[0] = Machine.param.Calibration(ParameterDefine.CALIBRATION.TRAY_CAL_MAP_END_POS_X);
                        position[1] = Machine.param.Calibration(ParameterDefine.CALIBRATION.TRAY_CAL_MAP_END_POS_Y);
                        position[2] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                        position[3] = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_R_READY_POS);
                    }
                    MovePickerXYR(position);
                    break;
            }
        }

        private double[] CalculatePickPosXYR()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
            double[] position = { 0.0, 0.0, 0.0, 0.0 };
            TOOL_TYPE toolType = (TOOL_TYPE)targetCurr;
            double offsetX = 0; double offsetY = 0;
            double directionOffsetX = 0; double directionOffsetY = 0;
            (double dTrayX, double dTrayY) = Vision.InterpolateMM(CAMERA.TRAY, toolType, proc.productPosition[targetCurr].x, proc.productPosition[targetCurr].y);
            if (dTrayX == Vision.INVALID_DATA || dTrayY == Vision.INVALID_DATA)
            {
                if (toolType == TOOL_TYPE.LEFT) SetError(ECODE.CAL_ERROR_TRAY_LEFT_DATA_OUT_OF_MAP);
                else if (toolType == TOOL_TYPE.RIGHT) SetError(ECODE.CAL_ERROR_TRAY_RIGHT_DATA_OUT_OF_MAP);
                return null;
            }

            if (toolType == TOOL_TYPE.LEFT)
            {
                offsetX = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_LEFT_X);
                offsetY = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_LEFT_Y);
                if (Math.Abs(proc.productAngle[targetCurr]) >= 135)
                {
                    directionOffsetX = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_LEFT_DEG180_X);
                    directionOffsetY = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_LEFT_DEG180_Y);
                }
            }
            else if (toolType == TOOL_TYPE.RIGHT)
            {
                offsetX = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_RIGHT_X);
                offsetY = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_RIGHT_Y);
                if (Math.Abs(proc.productAngle[targetCurr]) >= 135)
                {
                    directionOffsetX = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_RIGHT_DEG180_X);
                    directionOffsetY = Machine.recipe.Calibration(RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_RIGHT_DEG180_Y);
                }
            }

            PickupAngle[targetCurr] = proc.productAngle[targetCurr];
            isFixAngle180[targetCurr] = (Machine.recipe.Option(RecipeDefine.OPTION.LD_PICKER_ANGLE_FIX) == 1) && Math.Abs(PickupAngle[targetCurr]) > 90;

            position[0] = dTrayX + offsetX + directionOffsetX;
            position[1] = dTrayY + offsetY + directionOffsetY;
            position[2] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PICKUP_POS) + PickupAngle[0];
            position[3] = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_R_PICKUP_POS) + PickupAngle[1];
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

            vel[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_X_MOVE_VEL);
            acc[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_X_MOVE_ACC);
            dec[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_X_MOVE_DEC);
            vel[1] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Y_MOVE_VEL);
            acc[1] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Y_MOVE_ACC);
            dec[1] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Y_MOVE_DEC);
            vel[2] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_R_MOVE_VEL);
            acc[2] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_R_MOVE_ACC);
            dec[2] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_R_MOVE_DEC);

            Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_X, 1000 * position[0], vel[0], acc[0], dec[0]);
            Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_Y, 1000 * position[1], vel[1], acc[1], dec[1]);
            Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_R1, 1000 * position[2], vel[2], acc[2], dec[2]);
            Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_R2, 1000 * position[3], vel[2], acc[2], dec[2]);
            ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] " +
        $"{AutoStep}  {Step} Product Picker Moving X: {position[0]};Y: {position[1]}; Picker Left R:{position[2]}: Picker Right R: {position[3]} ");

        }

        private void MovePickerXYForMapping(double[] position)
        {
            double vel_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_X_CAL_MOVE_VEL);
            double acc_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_X_CAL_MOVE_ACC);
            double dec_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_X_CAL_MOVE_DEC);
            double vel_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Y_CAL_MOVE_VEL);
            double acc_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Y_CAL_MOVE_ACC);
            double dec_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Y_CAL_MOVE_DEC);

            Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_X, 1000 * position[0], vel_x, acc_x, dec_x);
            Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_Y, 1000 * position[1], vel_y, acc_y, dec_y);
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
                case ZPOS.PICK_READY:
                case ZPOS.PICK_UP_READY:
                case ZPOS.PLACEREADY:
                case ZPOS.UNDER_ALIGN:
                case ZPOS.TRAYCAL:
                case ZPOS.PICKERCAL:
                    vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_MOVE_VEL);
                    acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_MOVE_ACC);
                    dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_MOVE_DEC);

                    if (Pos == ZPOS.READY)
                        pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Z_READY_POS);
                    if (Pos == ZPOS.PICK_READY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PICKUP_POS) - 5.0);
                    if (Pos == ZPOS.PICK_UP_READY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PICKUP_POS) - 5.0);
                    if (Pos == ZPOS.PLACEREADY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PLACE_POS) - 5.0);
                    if (Pos == ZPOS.UNDER_ALIGN)
                        pos = 1000 * Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_ALIGN_POS);
                    if (Pos == ZPOS.TRAYCAL)
                    {
                        pos = 1000 * Machine.param.Calibration(ParameterDefine.CALIBRATION.TRAY_CAL_MAP_POS_Z);
                        vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_CAL_MOVE_VEL);
                        acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_CAL_MOVE_ACC);
                        dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_CAL_MOVE_DEC);
                    }
                    if (Pos == ZPOS.PICKERCAL)
                    {
                        pos = 1000 * Machine.param.Calibration(ParameterDefine.CALIBRATION.PICKER_CAL_MAP_POS_Z);
                        vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_CAL_MOVE_VEL);
                        acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_CAL_MOVE_ACC);
                        dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_CAL_MOVE_DEC);
                    }

                    Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_Z, pos, vel, acc, dec);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case ZPOS.PICK:
                case ZPOS.PLACE:
                case ZPOS.PLACEUPREADY:
                    vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_APPROACH_VEL);
                    acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_APPROACH_VEL);
                    dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.PROD_LOADER_Z_APPROACH_VEL);

                    if (Pos == ZPOS.PICK)
                    {
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PICKUP_POS));
                        double step = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PICKUP_STEP));
                        if (Machine.sysMode == Machine.SYSMODE.AUTO)
                        {
                            pos += step * pickRetryCount;
                            ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] Cam Picker Z Pos: {pos / 1000}");
                        }
                    }
                    if (Pos == ZPOS.PLACE)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PLACE_POS));
                    if (Pos == ZPOS.PLACEUPREADY)
                        pos = 1000 * (Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Z_PLACE_POS) - 3.0);

                    Machine.motion.MoveAxisAbs((int)AXIS.PROD_PICKUP_Z, pos, vel, acc, dec);
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] {AutoStep} {Step} {Pos} Cam Picker Z: {pos / 1000}");
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;
            }
        }


        //private void CameraBufferVacOn(int targetPlace)
        //{
        //    timeWait[(int)TIMER.TIMEOUT].Reset();
        //    if (targetPlace == 0)
        //    {
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L1_VACON, 1);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L1_PURGE, 0);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L2_VACON, 1);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L2_PURGE, 0);
        //    }
        //    if (targetPlace == 1)
        //    {
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R1_VACON, 1);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R1_PURGE, 0);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R2_VACON, 1);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R2_PURGE, 0);
        //    }
        //}

        //private bool CameraBufferVacOnCheck(int targetPlace)
        //{
        //    uint ret1 = 0;

        //    bool returnValue = true;
        //    bool timeOut = false;

        //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > 3000)
        //        timeOut = true;

        //    if (targetPlace == 0)
        //    {
        //        Machine.IO.GetIn((int)DI.CAM_BUF_L1_VACON, ref ret1);
        //        if (ret1 == 0)
        //        {
        //            returnValue = false;
        //            if (timeOut) SetError(ECODE.TIMEOUT_BUF2_L1_VACON);
        //        }
        //        Machine.IO.GetIn((int)DI.CAM_BUF_L2_VACON, ref ret1);
        //        if (ret1 == 0)
        //        {
        //            returnValue = false;
        //            if (timeOut) SetError(ECODE.TIMEOUT_BUF2_L2_VACON);
        //        }
        //    }
        //    if (targetPlace == 1)
        //    {
        //        Machine.IO.GetIn((int)DI.CAM_BUF_R1_VACON, ref ret1);
        //        if (ret1 == 0)
        //        {
        //            returnValue = false;
        //            if (timeOut) SetError(ECODE.TIMEOUT_BUF2_R1_VACON);
        //        }
        //        Machine.IO.GetIn((int)DI.CAM_BUF_R2_VACON, ref ret1);
        //        if (ret1 == 0)
        //        {
        //            returnValue = false;
        //            if (timeOut) SetError(ECODE.TIMEOUT_BUF2_R2_VACON);
        //        }
        //    }

        //    return returnValue;
        //}

        //private void CameraBufferVacOff(int targetPlace)
        //{
        //    if (targetPlace == 0)
        //    {
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L1_VACON, 0);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L1_PURGE, 1);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L2_VACON, 0);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L2_PURGE, 1);
        //    }
        //    if (targetPlace == 1)
        //    {
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R1_VACON, 0);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R1_PURGE, 1);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R2_VACON, 0);
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R2_PURGE, 1);
        //    }
        //}

        //private bool CameraBufferVacOffCheck(int targetPlace)
        //{
        //    uint ret1 = 0;

        //    if (targetPlace == 0)
        //    {
        //        Machine.IO.GetIn((int)DI.CAM_BUF_L1_VACON, ref ret1);
        //        if (ret1 == 0) return false;
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L1_PURGE, 0);
        //        Machine.IO.GetIn((int)DI.CAM_BUF_L2_VACON, ref ret1);
        //        if (ret1 == 0) return false;
        //        Machine.IO.SetOut((int)DO.CAM_BUF_L2_PURGE, 0);
        //    }
        //    if (targetPlace == 1)
        //    {
        //        Machine.IO.GetIn((int)DI.CAM_BUF_R1_VACON, ref ret1);
        //        if (ret1 == 0) return false;
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R1_PURGE, 0);
        //        Machine.IO.GetIn((int)DI.CAM_BUF_R2_VACON, ref ret1);
        //        if (ret1 == 0) return false;
        //        Machine.IO.SetOut((int)DO.CAM_BUF_R2_PURGE, 0);
        //    }

        //    return true;
        //}

        private void ResetUnderVisionData()
        {
            iAlignErrorCheckCount = 0;
            iAlignErrorCheckCount = 0;
            iRetryVision = 0;
            alignSucs[targetCurr] = -1;
            productPosition[targetCurr].x = 0;
            productPosition[targetCurr].y = 0;
            productAngle[targetCurr] = 0;
            adjustPos[targetCurr].x = 0;
            adjustPos[targetCurr].y = 0;
            adjustAngle[targetCurr] = 0;
            alignCenterPos[targetCurr].x = 0;
            alignCenterPos[targetCurr].y = 0;
            alignCenterAngle[targetCurr] = 0;
        }

        public override void SetHeadTarget(int iTarget)
        {
            this.targetCurr = iTarget;
            //throw new NotImplementedException();
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

