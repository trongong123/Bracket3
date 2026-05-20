using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.UI;
using TopEng.Device;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Type;
using System.Threading;
using System.Windows.Forms;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.Process
{
    class ProcessBufStageOne : IProcess
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
            TRANSFER_LOAD_PRODUCT,
            TRANSFER_LOAD_PRODUCT_COMPL_CHECK,
            //PEELING,
            //PEELING_COMPL_CHECK,
            UNLOAD_PRODUCT,
            UNLOAD_PRODUCT_CHECK,
            UNLOADING_CHECK,
        }

        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,
            NEXT_STEP,

            // INITIALIZE
            INIT_PRODUCT_EXISTENCE_SEARCH,

            // VAC, PURGE DELAY
            VAC_ON_WAIT,
            VAC_ON_WAIT_CHECK,
            PURGE_OFF_WAIT,
            PURGE_OFF_WAIT_CHECK,
            PURGE_OFF,
            PURGE_OFF_CHECK,

            // TRANSFER IO
            //TRF_UP,
            //TRF_UP_CHECK,
            //TRF_DOWN,
            //TRF_DOWN_CHECK,
            //TRF_CLAMP,
            //TRF_CLAMP_CHECK,
            //TRF_UNCLAMP,
            //TRF_UNCLAMP_CHECK,
            TRF_VACON_SELF,
            TRF_VACON_SELF_CHECK,
            TRF_VACON_RECEIVE,
            TRF_VACON_RECEIVE_CHECK,
            TRF_VACOFF_SELF,
            TRF_VACOFF_SELF_BYPASS,
            TRF_VACOFF_SELF_CHECK,

            // TRANSFER AXIS MOVE
            TRF_READY_POS,
            TRF_READY_POS_OVERLAP,
            TRF_READY_POS_CHECK,
            TRF_LOADING_POS,
            TRF_LOADING_POS_CHECK,
            //TRF_PEELING_POS,
            //TRF_PEELING_POS_CHECK,
            TRF_UNLOADING_AVOID_POS,
            TRF_UNLOADING_AVOID_POS_CHECK,
            TRF_UNLOADING_POS,
            TRF_UNLOADING_POS_CHECK,

            // PEELING
            //PEELING_RETURN,
            //PEELING_RETURN_BYPASS,
            //PEELING_RETURN_CHECK,
            //PEELING_TURN,
            //PEELING_TURN_CHECK,
            //PEELING_CLAMP,
            //PEELING_CLAMP_CHECK,
            //PEELING_UNCLAMP,
            //PEELING_UNCLAMP_BYPASS,
            //PEELING_UNCLAMP_CHECK,
            //PEELING_UP,
            //PEELING_UP_CHECK,
            //PEELING_READY,
            //PEELING_READY_BYPASS,
            //PEELING_READY_CHECK,
            //PEELING_DOWN,
            //PEELING_DOWN_CHECK,
            //PEELING_BLOW,
            //PEELING_BLOW_OFF,

            // BUFFER SENSOR
            LD_BUF_DETECT_ON_CHECK,
            LD_BUF_DETECT_OFF_CHECK,
            ULD_BUF_DETECT_ON_CHECK,
            ULD_BUF_DETECT_OFF_CHECK,

            // BUFFER VAC (UNLOADING STAGE)
            BUF_VACOFF_SELF,
            BUF_VACOFF_SELF_BYPASS,
            BUF_VACOFF_SELF_CHECK,

            BUF_VACON_SELF,
            BUF_VACON_SELF_BYPASS,
            BUF_VACON_SELF_CHECK,
            BUF_VACON_RECEIVE,
            BUF_VACON_RECEIVE_BYPASS,
            BUF_VACON_RECEIVE_CHECK,

            // DATA SEND
            SENDING_PRODUCT_DATA_TO_TRF,
            SENDING_PRODUCT_DATA_TO_BUF,
        }

        public enum MSG
        {
            MSG_SINGLE,

            MSG_PROCESS_INIT,
            MSG_TRANSFER_LOAD_PRODUCT, //MSG_TRANSFER_LOAD_PRODUCT? => TRF가 짚고 들어올리는거까지
            //MSG_PEELING_UP_PRODUCT, //여기서 TRF가 Peeling 위치로 이동 이동
            //MSG_PEELING_DOWN_PRODUCT, //여기서 TRF가 Peeling 위치로 이동
            MSG_TRANSFER_UNLOAD_PRODUCT,
            MSG_MOVE_READY_POS,
            MSG_MOVE_LOADING_POS,
            MSG_MOVE_PEELING_POS,
            MSG_MOVE_UNLOADING_POS,
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
            stopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;

            AutoStep = AUTOSTEP.IDLE;
            Step = STEP.IDLE;

            //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);
        }

        public override void Stop()
        {
            stopBit = true;

            //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);
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

        public enum IOIn
        {
            //TRF_Z_UP, TRF_Z_DOWN,
            //TRF1_CLAMP, TRF1_UNCLAMP,
            //TRF2_CLAMP, TRF2_UNCLAMP,
            //TRF1_VACON, TRF2_VACON,
            //PEELING1_Z1_UP, PEELING1_Z1_DOWN,
            //PEELING1_Z2_UP, PEELING1_Z2_DOWN,
            //PEELING2_Z1_UP, PEELING2_Z1_DOWN,
            //PEELING2_Z2_UP, PEELING2_Z2_DOWN,
            //PEELING1_RETURN, PEELING1_TURN,
            //PEELING2_RETURN, PEELING2_TURN,
            //PEELING1_UNCLAMP,
            //PEELING2_UNCLAMP,
            DETACH1, DETACH3,
            DETACH2, DETACH4,

            IOInMax,
        }

        int[,] inIo =
        {
            // TRF UP / DOWN
            //{(int)DI.CAM_TRF_L_Z_UP,   (int)DI.CAM_TRF_R_Z_UP },
            //{(int)DI.CAM_TRF_L_Z_DOWN, (int)DI.CAM_TRF_R_Z_DOWN },
            //// TRF CLAMP / UNCLAMP
            //{(int)DI.CAM_TRF_L1_CLAMP,   (int)DI.CAM_TRF_R1_CLAMP },
            //{(int)DI.CAM_TRF_L1_UNCLAMP, (int)DI.CAM_TRF_R1_UNCLAMP },
            //{(int)DI.CAM_TRF_L2_CLAMP,   (int)DI.CAM_TRF_R2_CLAMP },
            //{(int)DI.CAM_TRF_L2_UNCLAMP, (int)DI.CAM_TRF_R2_UNCLAMP},
            //// TRF VAC ON
            //{(int)DI.CAM_TRF_L1_VACON, (int)DI.CAM_TRF_R1_VACON},
            //{(int)DI.CAM_TRF_L2_VACON, (int)DI.CAM_TRF_R2_VACON},
            //// PEELING UP / DOWN
            //{(int)DI.PEELING_L1_Z1_UP,   (int)DI.PEELING_R1_Z1_UP},
            //{(int)DI.PEELING_L1_Z1_DOWN, (int)DI.PEELING_R1_Z1_DOWN},
            //{(int)DI.PEELING_L1_Z2_UP,   (int)DI.PEELING_R1_Z2_UP},
            //{(int)DI.PEELING_L1_Z2_DOWN, (int)DI.PEELING_R1_Z2_DOWN},
            //{(int)DI.PEELING_L2_Z1_UP,   (int)DI.PEELING_R2_Z1_UP},
            //{(int)DI.PEELING_L2_Z1_DOWN, (int)DI.PEELING_R2_Z1_DOWN},
            //{(int)DI.PEELING_L2_Z2_UP,   (int)DI.PEELING_R2_Z2_UP},
            //{(int)DI.PEELING_L2_Z2_DOWN, (int)DI.PEELING_R2_Z2_DOWN},
            //// PEELING RETURN / TURN
            //{(int)DI.PEELING_L1_RETURN, (int)DI.PEELING_R1_RETURN},
            //{(int)DI.PEELING_L1_TURN,   (int)DI.PEELING_R1_TURN},
            //{(int)DI.PEELING_L2_RETURN, (int)DI.PEELING_R2_RETURN},
            //{(int)DI.PEELING_L2_TURN,   (int)DI.PEELING_R2_TURN},
            //// PEELING CLAMP / UNCLAMP
            //{(int)DI.PEELING_L1_UNCLAMP, (int)DI.PEELING_R1_UNCLAMP},
            //{(int)DI.PEELING_L2_UNCLAMP, (int)DI.PEELING_R2_UNCLAMP},
            //// BUF DETECT
            //{(int)DI.CAM_BUF_L1_DETECT_ON,  (int)DI.CAM_BUF_R1_DETECT_ON},
            //{(int)DI.CAM_BUF_L2_DETECT_ON,  (int)DI.CAM_BUF_R2_DETECT_ON},
            {(int)DI.DETACH_SENSOR_1, (int)DI.DETACH_SENSOR_3},
            {(int)DI.DETACH_SENSOR_2, (int)DI.DETACH_SENSOR_4},
        };

        public enum IOOut
        {
            //TRF_Z_UP, TRF_Z_DOWN,
            //TRF1_UNCLAMP, TRF1_CLAMP,
            //TRF2_UNCLAMP, TRF2_CLAMP,
            //TRF1_VACON, TRF1_PURGE,
            //TRF2_VACON, TRF2_PURGE,
            //PEELING1_Z1_UP, PEELING1_Z1_DOWN,
            //PEELING1_Z2_UP, PEELING1_Z2_DOWN,
            //PEELING2_Z1_UP, PEELING2_Z1_DOWN,
            //PEELING2_Z2_UP, PEELING2_Z2_DOWN,
            //PEELING1_RETURN, PEELING1_TURN,
            //PEELING2_RETURN, PEELING2_TURN,
            //PEELING1_UNCLAMP, PEELING1_CLAMP,
            //PEELING2_UNCLAMP, PEELING2_CLAMP,
            PEELING_BLOW,
            DUST_SUCTION,
            IOOutMax,
        }

        int[,] outIo =
        {
            // TRF UP / DOWN
            
            // PEELING BLOW
            //{(int)DO.LEFT_PEELING_BLOW, (int)DO.RIGHT_PEELING_BLOW},
            //// DUST SUCTION
            //{(int)DO.DUST_SUCTION, (int)DO.DUST_SUCTION},
        };

        int[] transferAxis = { (int)AXIS.CAM_TRANSFER_LEFT_Y, (int)AXIS.CAM_TRANSFER_RIGHT_Y };
        public enum YPOS
        {
            READY,
            LOAD,
            PEELING,
            UNLOAD
        }
        RecipeDefine.POSITION[,] transferPos =
        {
            { RecipeDefine.POSITION.CAM_TRANSFER_Y_READY_L_POS,             RecipeDefine.POSITION.CAM_TRANSFER_Y_READY_R_POS },
            { RecipeDefine.POSITION.CAM_TRANSFER_Y_LOADING_L_POS,         RecipeDefine.POSITION.CAM_TRANSFER_Y_LOADING_R_POS },
            //{ RecipeDefine.POSITION.CAM_TRANSFER_Y_PEELING_L_POS,         RecipeDefine.POSITION.CAM_TRANSFER_Y_PEELING_R_POS },
            { RecipeDefine.POSITION.CAM_TRANSFER_Y_UNLOADING_L_POS,       RecipeDefine.POSITION.CAM_TRANSFER_Y_UNLOADING_R_POS }
        };

        // BUFFER LOADING 쪽
        int[,] place1 =
        {
            {(int)UNITPART.BUF1_L1, (int)UNITPART.BUF1_R1},
            {(int)UNITPART.BUF1_L2, (int)UNITPART.BUF1_R2},
        };

        // BUFFER UNLOADING 쪽
        int[,] place2 =
        {
            {(int)UNITPART.BUF2_L1, (int)UNITPART.BUF2_R1},
            {(int)UNITPART.BUF2_L2, (int)UNITPART.BUF2_R2}
        };

        //int[,] transfer1 =
        //{
        //    {(int)UNITPART.CAM_TRF_L1, (int)UNITPART.CAM_TRF_R1 },
        //};

        //int[,] transfer2 =
        //{
        //    {(int)UNITPART.CAM_TRF_L2, (int)UNITPART.CAM_TRF_R2 },
        //};

        int[,] transfer =
        {
            {(int)UNITPART.CAM_TRF_L1, (int)UNITPART.CAM_TRF_R1 },
            {(int)UNITPART.CAM_TRF_L2, (int)UNITPART.CAM_TRF_R2 }
        };

        int[] upstream = { (int)UNITPART.PROD_PICK1, (int)UNITPART.PROD_PICK2 };
        int[] downstream = { (int)UNITPART.ASSEMBLER1, (int)UNITPART.ASSEMBLER2 };

        int unitNo = 0;

        public ProcessBufStageOne(int UnitNo)
        {
            unitNo = UnitNo;
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
                case ECODE.TIMEOUT_BUF1_L1_DETECT:
                case ECODE.TIMEOUT_BUF1_L2_DETECT:
                case ECODE.TIMEOUT_BUF1_R1_DETECT:
                case ECODE.TIMEOUT_BUF1_R2_DETECT:
                case ECODE.TIMEOUT_BUF2_L1_DETECT:
                case ECODE.TIMEOUT_BUF2_L2_DETECT:
                case ECODE.TIMEOUT_BUF2_R1_DETECT:
                case ECODE.TIMEOUT_BUF2_R2_DETECT:
                    var processError = new List<STEP>()
                    {
                        STEP.ERROR,
                    };

                    StepList = processError;
                    Step = StepList[StepIndex = 0];
                    break;

                case ECODE.TIMEOUT_CAM_TRF_L1_VACON:
                case ECODE.TIMEOUT_CAM_TRF_R1_VACON:
                case ECODE.TIMEOUT_CAM_TRF_L2_VACON:
                case ECODE.TIMEOUT_CAM_TRF_R2_VACON:
                    var processErrorHandler = new List<STEP>()
                    {
                        STEP.TRF_VACOFF_SELF,
                        //STEP.TRF_UP,
                        STEP.ERROR,
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
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

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
                    Machine.Parts[place1[0, unitNo]].ClearInterface();
                    Machine.Parts[place1[1, unitNo]].ClearInterface();
                    Machine.Parts[place2[0, unitNo]].ClearInterface();
                    Machine.Parts[place2[1, unitNo]].ClearInterface();
                    Machine.Parts[transfer[0, unitNo]].ClearInterface();
                    var processInit = new List<STEP>()
                    {
                        //STEP.TRF_UP,
                        //STEP.PEELING_UNCLAMP,
                        //STEP.PEELING_RETURN,
                        //STEP.PEELING_DOWN,
                        STEP.TRF_READY_POS,
                        STEP.INIT_PRODUCT_EXISTENCE_SEARCH,
                        STEP.IDLE,
                    };

                    //if (Machine.recipe.Option(RecipeDefine.OPTION.PEELING_USE) == 0)
                    //{
                    //    processInit[GetStepIndex(processInit, STEP.PEELING_UNCLAMP)] = STEP.NEXT_STEP;
                    //}

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;


                case MSG.MSG_TRANSFER_LOAD_PRODUCT:
                    Machine.Parts[place1[0, unitNo]].ClearInterface();
                    Machine.Parts[place1[1, unitNo]].ClearInterface();

                    var processLoadTransfer = new List<STEP>()
                    {
                        STEP.LD_BUF_DETECT_ON_CHECK,
                        //STEP.TRF_UP,
                        //STEP.PEELING_RETURN_BYPASS,
                        //STEP.PEELING_DOWN,
                        STEP.TRF_VACOFF_SELF,
                        //STEP.TRF_UNCLAMP,
                        STEP.TRF_LOADING_POS,
                        //STEP.TRF_DOWN,
                        //STEP.TRF_CLAMP,
                        STEP.TRF_VACON_RECEIVE,
                        STEP.SENDING_PRODUCT_DATA_TO_TRF,
                        //STEP.TRF_UP,
                        //STEP.PEELING_RETURN_BYPASS,
                        //STEP.PEELING_DOWN,
                        //STEP.TRF_PEELING_POS,
                        STEP.LD_BUF_DETECT_OFF_CHECK,
                        STEP.IDLE,
                    };

                    //if (Machine.recipe.Option(RecipeDefine.OPTION.PEELING_USE) == 0)
                    //{
                    //    processLoadTransfer[GetStepIndex(processLoadTransfer, STEP.PEELING_DOWN)] = STEP.NEXT_STEP;
                    //    processLoadTransfer[GetStepIndex(processLoadTransfer, STEP.PEELING_RETURN_BYPASS)] = STEP.NEXT_STEP;
                    //    processLoadTransfer[GetStepIndex(processLoadTransfer, STEP.PEELING_DOWN)] = STEP.NEXT_STEP;
                    //    processLoadTransfer[GetStepIndex(processLoadTransfer, STEP.PEELING_RETURN_BYPASS)] = STEP.NEXT_STEP;
                    //    processLoadTransfer[GetStepIndex(processLoadTransfer, STEP.TRF_PEELING_POS)] = STEP.TRF_READY_POS;
                    //}
                    StepList = processLoadTransfer;
                    Step = StepList[StepIndex = 0];
                    break;

                //case MSG.MSG_PEELING_UP_PRODUCT:
                //case MSG.MSG_PEELING_DOWN_PRODUCT:
                //    var processPeeling = new List<STEP>()
                //    {
                //        STEP.PEELING_UNCLAMP_BYPASS,
                //        STEP.PEELING_RETURN_BYPASS,
                //        STEP.PEELING_DOWN,
                //        STEP.TRF_PEELING_POS,
                //        STEP.PEELING_READY,
                //        STEP.PEELING_TURN,
                //        STEP.PEELING_CLAMP,
                //        STEP.PEELING_UP, // 위로 박리
                //        STEP.PEELING_RETURN,
                //        STEP.PEELING_DOWN,
                //        STEP.PEELING_BLOW,
                //        STEP.PEELING_UNCLAMP_BYPASS,
                //        STEP.IDLE,
                //    };

                //    if (message == (int)MSG.MSG_PEELING_UP_PRODUCT) processPeeling[GetStepIndex(processPeeling, STEP.PEELING_UP)] = STEP.PEELING_UP; // 위로 박리;
                //    if (message == (int)MSG.MSG_PEELING_DOWN_PRODUCT) processPeeling[GetStepIndex(processPeeling, STEP.PEELING_UP)] = STEP.PEELING_DOWN; // 아래로 박리

                //    StepList = processPeeling;
                //    Step = StepList[StepIndex = 0];
                //    break;

                case MSG.MSG_TRANSFER_UNLOAD_PRODUCT:
                    Machine.Parts[place2[0, unitNo]].ClearInterface();
                    Machine.Parts[place2[1, unitNo]].ClearInterface();

                    var processTransferUnload = new List<STEP>()
                    {
                        //STEP.TRF_UP,
                        STEP.ULD_BUF_DETECT_OFF_CHECK,
                        //STEP.PEELING_RETURN_BYPASS,
                        //STEP.PEELING_DOWN,
                        STEP.TRF_UNLOADING_POS,
                        //STEP.TRF_DOWN,
                        //STEP.TRF_UNCLAMP,
                        STEP.TRF_VACOFF_SELF,
                        STEP.SENDING_PRODUCT_DATA_TO_BUF,
                        STEP.TRF_VACOFF_SELF_BYPASS,
                        STEP.TRF_VACOFF_SELF_CHECK,
                        //STEP.TRF_CLAMP,
                        //STEP.TRF_UNCLAMP,
                        //STEP.TRF_UP,
                        STEP.TRF_READY_POS,
                        STEP.ULD_BUF_DETECT_ON_CHECK,
                        STEP.IDLE,
                    };

                    //if (Machine.recipe.Option(RecipeDefine.OPTION.PEELING_USE) == 0)
                    //{
                    //    processTransferUnload[GetStepIndex(processTransferUnload, STEP.PEELING_DOWN)] = STEP.NEXT_STEP;
                    //    processTransferUnload[GetStepIndex(processTransferUnload, STEP.PEELING_RETURN_BYPASS)] = STEP.NEXT_STEP;
                    //}

                    StepList = processTransferUnload;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_READY_POS:
                case MSG.MSG_MOVE_LOADING_POS:
                case MSG.MSG_MOVE_PEELING_POS:
                case MSG.MSG_MOVE_UNLOADING_POS:
                    var moveXYRPosition = new List<STEP>()
                    {
                        STEP.TRF_READY_POS,
                        STEP.IDLE,
                    };

                    if ((MSG)message == MSG.MSG_MOVE_READY_POS) moveXYRPosition[0] = STEP.TRF_READY_POS;
                    if ((MSG)message == MSG.MSG_MOVE_LOADING_POS) moveXYRPosition[0] = STEP.TRF_LOADING_POS;
                    //if ((MSG)message == MSG.MSG_MOVE_PEELING_POS) moveXYRPosition[0] = STEP.TRF_PEELING_POS;
                    if ((MSG)message == MSG.MSG_MOVE_UNLOADING_POS) moveXYRPosition[0] = STEP.TRF_UNLOADING_POS;

                    StepList = moveXYRPosition;
                    Step = StepList[StepIndex = 0];
                    break;
            }
        }

        protected override void OnProcessing()
        {
            uint ret1 = 0;
            uint ret2 = 0;

            switch ((STEP)Step)
            {
                case STEP.NEXT_STEP:
                    NextStep();
                    break;

                case STEP.ERROR:
                    error_proc = false;
                    AutoStep = AUTOSTEP.ERROR;
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

                case STEP.INIT_PRODUCT_EXISTENCE_SEARCH:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        //uint outSignal1 = 0;
                        //uint outSignal2 = 0;
                        //Machine.IO.GetOut(outIo[(int)IOOut.TRF1_VACON, unitNo], ref outSignal1);
                        //Machine.IO.GetOut(outIo[(int)IOOut.TRF2_VACON, unitNo], ref outSignal2);

                        //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_VACON, unitNo], 1);
                        //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_VACON, unitNo], 1);

                        //Util.Delay(2000);
                        //Machine.IO.GetIn(inIo[(int)IOIn.TRF1_VACON, unitNo], ref ret1);
                        //Machine.IO.GetIn(inIo[(int)IOIn.TRF2_VACON, unitNo], ref ret2);
                        //if (Machine.Parts[transfer[0, unitNo]].exist && !Convert.ToBoolean(ret1) && outSignal1 == 1)
                        //{
                        //    Machine.EStop(false);
                        //    Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.ERROR, string.Format("Loading Transfer Left mismatching material data.\nPlease Check Data State."));
                        //    form.TopLevel = true;
                        //    form.TopMost = true;
                        //    form.ShowDialog();
                        //    break;
                        //}
                        //else Machine.Parts[transfer[0, unitNo]].exist = Convert.ToBoolean(ret1);
                        //if (ret1 == 0)
                        //    Machine.IO.SetOut(outIo[(int)IOOut.TRF1_VACON, unitNo], 0);

                        //if (Machine.Parts[transfer[1, unitNo]].exist && !Convert.ToBoolean(ret2) && outSignal2 == 1)
                        //{
                        //    Machine.EStop(false);
                        //    Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.ERROR, string.Format("Loading Transfer Right mismatching material data.\nPlease Check Data State."));
                        //    form.TopLevel = true;
                        //    form.TopMost = true;
                        //    form.ShowDialog();
                        //    break;
                        //}
                        //else Machine.Parts[transfer[1, unitNo]].exist = Convert.ToBoolean(ret2);
                        //if (ret2 == 0)
                        //    Machine.IO.SetOut(outIo[(int)IOOut.TRF2_VACON, unitNo], 0);

                        //Machine.IO.GetIn(inIo[(int)IOIn.DETACH1, unitNo], ref ret1);
                        //Machine.Parts[place1[0, unitNo]].exist = Convert.ToBoolean(ret1);

                        //Machine.IO.GetIn(inIo[(int)IOIn.DETACH2, unitNo], ref ret1);
                        //Machine.Parts[place1[1, unitNo]].exist = Convert.ToBoolean(ret1);

                        //Machine.IO.GetIn(inIo[(int)IOIn.DETACH3, unitNo], ref ret1);
                        //Machine.Parts[place2[0, unitNo]].exist = Convert.ToBoolean(ret1);

                        //Machine.IO.GetIn(inIo[(int)IOIn.DETACH4, unitNo], ref ret1);
                        //Machine.Parts[place2[1, unitNo]].exist = Convert.ToBoolean(ret1);
                    }

                    NextStep();
                    break;

                //case STEP.VAC_ON_WAIT:
                //    timeWait[(int)TIMER.DELAY].Reset();
                //    Step = STEP.VAC_ON_WAIT_CHECK;
                //    break;

                //case STEP.VAC_ON_WAIT_CHECK:
                //    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_WAIT_TIME))
                //        NextStep();
                //    break;

                //case STEP.PURGE_OFF:
                //    timeWait[(int)TIMER.DELAY].Reset();
                //    Step = STEP.PURGE_OFF_CHECK;
                //    break;

                //case STEP.PURGE_OFF_CHECK:
                //    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_TIME))
                //    {
                //        //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);
                //        //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);
                //        NextStep();
                //    }
                //    break;

                //case STEP.PURGE_OFF_WAIT:
                //    timeWait[(int)TIMER.DELAY].Reset();
                //    Step = STEP.PURGE_OFF_WAIT_CHECK;
                //    break;

                //case STEP.PURGE_OFF_WAIT_CHECK:
                //    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME))
                //        NextStep();
                //    break;
            }

            OnProcessOfAutoRun();
            OnProcessOfMotion();
            OnProcessOfIO();
            OnProcessOfInterface();

            if (Machine.param.Option(ParameterDefine.OPTION.USE_PROC_LOG) == 1)
            {
                if ((int)AutoStep != lastAutoStep)
                {
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}{unitNo}] {Enum.GetName(typeof(AUTOSTEP), AutoStep)}");
                    lastAutoStep = (int)AutoStep;
                }
                if ((int)Step != lastProcStep)
                {
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}{unitNo}] {Enum.GetName(typeof(STEP), Step)}");
                    lastProcStep = (int)Step;
                }
            }
        }

        private void OnProcessOfAutoRun()
        {
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            bool upstreamExist = false;
            bool downstreamExist = false;

            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
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
                    Machine.Parts[place1[0, unitNo]].ClearInterface();
                    Machine.Parts[place1[1, unitNo]].ClearInterface();
                    Machine.Parts[place2[0, unitNo]].ClearInterface();
                    Machine.Parts[place2[1, unitNo]].ClearInterface();

                    initCompl = false;
                    stopBit = false;

                    if (!Machine.proclist[(int)Machine.PROCESS.ASSEMBLER].initCompl)
                        break;
                    if (!Machine.proclist[(int)Machine.PROCESS.PROD_LOADER].initCompl)
                        break;

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        initCompl = true;
                        AutoStep = AUTOSTEP.IDLE;
                    }

                    SetMessage((int)MSG.MSG_PROCESS_INIT);
                    timeWait[(int)TIMER.STATUS].Reset();
                    AutoStep = AUTOSTEP.INIT_CHECK;
                    break;

                case AUTOSTEP.INIT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    initCompl = true;
                    Machine.interfer_buffer_transfer_prod_loader[unitNo] = false;
                    Machine.interfer_buffer_transfer_assembler[unitNo] = false;
                    Machine.buffer_ready_to_place[unitNo] = false;
                    Machine.buffer_ready_to_pick[unitNo] = false;
                    Machine.prod_loader_start_place[unitNo] = false;
                    Machine.assembler_start_pick[unitNo] = false;

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        AutoStep = AUTOSTEP.IDLE;
                        break;
                    }

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
                    if (Machine.param.Option(ParameterDefine.OPTION.USE_LEFT_BUFFER) == 0 && unitNo == 0)
                        break;
                    if (Machine.param.Option(ParameterDefine.OPTION.USE_RIGHT_BUFFER) == 0 && unitNo == 1)
                        break;

                    if (Machine.Parts[place2[0, unitNo]].exist || Machine.Parts[place2[1, unitNo]].exist)
                        Machine.buffer_ready_to_pick[unitNo] = true;
                    if (!Machine.Parts[place1[0, unitNo]].exist || !Machine.Parts[place1[1, unitNo]].exist)
                        Machine.buffer_ready_to_place[unitNo] = true;
                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.LOADING_CHECK:
                    if (CheckStopBit())
                        break;

                    if (Machine.Parts[transfer[0, unitNo]].exist || Machine.Parts[transfer[1, unitNo]].exist)
                    {
                        AutoStep = AUTOSTEP.UNLOAD_PRODUCT;
                    }
                    else if (Machine.Parts[place1[0, unitNo]].exist && Machine.Parts[place1[1, unitNo]].exist)
                    {
                        AutoStep = AUTOSTEP.TRANSFER_LOAD_PRODUCT;
                    }
                    break;

                case AUTOSTEP.TRANSFER_LOAD_PRODUCT:
                    if (!Machine.Parts[place1[0, unitNo]].exist && !Machine.Parts[place2[1, unitNo]].exist)
                        break;
                    if (Machine.prod_loader_start_place[unitNo]) break;
                    Machine.buffer_ready_to_place[unitNo] = false;
                    SetMessage((int)MSG.MSG_TRANSFER_LOAD_PRODUCT);
                    AutoStep = AUTOSTEP.TRANSFER_LOAD_PRODUCT_COMPL_CHECK;
                    break;

                case AUTOSTEP.TRANSFER_LOAD_PRODUCT_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.buffer_ready_to_place[unitNo] = true;
                    AutoStep = AUTOSTEP.UNLOAD_PRODUCT;
                    break;

                //case AUTOSTEP.PEELING:
                //    if (CheckStopBit())
                //        break;

                //    if (Machine.Parts[transfer[0, unitNo]].exist || Machine.Parts[transfer[1, unitNo]].exist)
                //    {
                //        if (Machine.recipe.Option(RecipeDefine.OPTION.PEELING_USE) == 1)
                //        {
                //            if (Machine.recipe.Option(RecipeDefine.OPTION.USE_PEELING_DOWN) == 1)
                //            {
                //                SetMessage((int)MSG.MSG_PEELING_DOWN_PRODUCT);
                //                AutoStep = AUTOSTEP.PEELING_COMPL_CHECK;
                //            }
                //            else if (Machine.recipe.Option(RecipeDefine.OPTION.USE_PEELING_UP) == 1)
                //            {
                //                SetMessage((int)MSG.MSG_PEELING_UP_PRODUCT);
                //                AutoStep = AUTOSTEP.PEELING_COMPL_CHECK;
                //            }
                //        }
                //        else
                //        {
                //            AutoStep = AUTOSTEP.UNLOAD_PRODUCT;
                //        }
                //        break;
                //    }
                //    break;

                //case AUTOSTEP.PEELING_COMPL_CHECK:
                //    if (CheckStopBit() || !Ready() || Error())
                //        break;

                //    if (Machine.Parts[transfer[0, unitNo]].exist || Machine.Parts[transfer[1, unitNo]].exist)
                //    {
                //        AutoStep = AUTOSTEP.UNLOAD_PRODUCT;
                //    }
                //    break;

                case AUTOSTEP.UNLOAD_PRODUCT:
                    if (Machine.Parts[place2[0, unitNo]].exist || Machine.Parts[place2[1, unitNo]].exist)
                        break;
                    if (Machine.assembler_start_pick[unitNo]) break;
                    Machine.buffer_ready_to_pick[unitNo] = false;
                    SetMessage((int)MSG.MSG_TRANSFER_UNLOAD_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOAD_PRODUCT_CHECK;
                    break;

                case AUTOSTEP.UNLOAD_PRODUCT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;
                    Machine.buffer_ready_to_pick[unitNo] = true;
                    AutoStep = AUTOSTEP.COMPLETE;
                    break;
            }

            if (AutoStep != AUTOSTEP.IDLE && AutoStep != AUTOSTEP.ERROR && AutoStep != AUTOSTEP.STOP)
                lastAutoStep = (int)AutoStep;
        }

        private void OnProcessOfInterface()
        {
            //uint ret1 = 0;

            switch ((STEP)Step)
            {
                case STEP.SENDING_PRODUCT_DATA_TO_TRF:
                    //processTransferLoad Place1 -> TRF
                    for (int i = 1; i >= 0; i--)
                    {
                        if (Machine.Parts[place1[i, unitNo]].exist)
                        {
                            Machine.Parts[transfer[i, unitNo]].CopyFrom(Machine.Parts[place1[i, unitNo]]);
                            Machine.Parts[place1[i, unitNo]].exist = false;
                        }
                    }
                    NextStep();
                    break;

                case STEP.SENDING_PRODUCT_DATA_TO_BUF:
                    //processTransferUnload TRF -> place2
                    for (int i = 1; i >= 0; i--)
                    {
                        if (Machine.Parts[transfer[i, unitNo]].exist)
                        {
                            Machine.Parts[place2[i, unitNo]].CopyFrom(Machine.Parts[transfer[i, unitNo]]);
                            Machine.Parts[transfer[i, unitNo]].exist = false;
                        }
                    }
                    NextStep();
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            uint ret3 = 0;
            uint ret4 = 0;
            uint ret1 = 0;
            uint ret2 = 0;

            //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_TURN, unitNo], ref ret1);
            //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_TURN, unitNo], ref ret2);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF_Z_UP, unitNo], ref ret3);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF_Z_DOWN, unitNo], ref ret4);

            switch ((STEP)Step)
            {

                case STEP.TRF_READY_POS:
                case STEP.TRF_READY_POS_OVERLAP:
                    //if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                    //    SetError(ECODE.INTERFERENCE_CAM_TRF);
                    //else
                    //{
                        MoveTransferY(YPOS.READY);
                        if (Step == STEP.TRF_READY_POS) Step = STEP.TRF_READY_POS_CHECK;
                        if (Step == STEP.TRF_READY_POS_OVERLAP) NextStep();
                    //}
                    break;

                case STEP.TRF_READY_POS_CHECK:
                    //if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                    //    SetError(ECODE.INTERFERENCE_CAM_TRF);

                    if (!Machine.motion.MoveAxisDoneCheck(transferAxis[unitNo]))
                        break;

                    Machine.interfer_buffer_transfer_prod_loader[unitNo] = false;
                    Machine.interfer_buffer_transfer_assembler[unitNo] = false;

                    NextStep();
                    break;

                case STEP.TRF_LOADING_POS:
                    //if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                    //    SetError(ECODE.INTERFERENCE_CAM_TRF);
                    //else
                    //{
                        Machine.interfer_buffer_transfer_prod_loader[unitNo] = true;
                        if (!CheckInterlockBufferLoader()) break;
                        if (!MoveTransferY(YPOS.LOAD)) break;
                        Step = STEP.TRF_LOADING_POS_CHECK;
                    //}
                    break;

                case STEP.TRF_LOADING_POS_CHECK:
                    //if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                    //    SetError(ECODE.INTERFERENCE_CAM_TRF);

                    if (!Machine.motion.MoveAxisDoneCheck(transferAxis[unitNo]))
                        break;

                    Machine.interfer_buffer_transfer_assembler[unitNo] = false;
                    NextStep();
                    break;

                //case STEP.TRF_PEELING_POS:
                //    if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                //        SetError(ECODE.INTERFERENCE_CAM_TRF);
                //    else
                //    {
                //        MoveTransferY(YPOS.PEELING);
                //        Step = STEP.TRF_PEELING_POS_CHECK;
                //    }
                //    break;

                //case STEP.TRF_PEELING_POS_CHECK:
                //    if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                //        SetError(ECODE.INTERFERENCE_CAM_TRF);

                //    if (!Machine.motion.MoveAxisDoneCheck(transferAxis[unitNo]))
                //        break;

                //    Machine.interfer_buffer_transfer_prod_loader[unitNo] = false;
                //    NextStep();
                //    break;

                case STEP.TRF_UNLOADING_POS:
                    //if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                    //    SetError(ECODE.INTERFERENCE_CAM_TRF);
                    //else
                    //{
                        Machine.interfer_buffer_transfer_assembler[unitNo] = true;
                        if (!CheckInterlockBufferAssembler()) break;
                        if (!MoveTransferY(YPOS.UNLOAD)) break;
                        Step = STEP.TRF_UNLOADING_POS_CHECK;
                    //}

                    break;

                case STEP.TRF_UNLOADING_POS_CHECK:
                    //if (ret1 == 1 || ret2 == 1 || ret3 == 0 || ret4 == 1)
                    //    SetError(ECODE.INTERFERENCE_CAM_TRF);

                    if (!Machine.motion.MoveAxisDoneCheck(transferAxis[unitNo]))
                        break;

                    Machine.interfer_buffer_transfer_prod_loader[unitNo] = false;
                    NextStep();
                    break;
            }
        }

        private void OnProcessOfIO()
        {
            bool SetValue = false;
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            switch ((STEP)Step)
            {
                #region TRANSFER I/O
                //case STEP.TRF_UP:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.TRF_Z_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.TRF_Z_UP, unitNo], 1);
                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.TRF_UP_CHECK;
                //    break;

                //case STEP.TRF_UP_CHECK:
                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (unitNo == 0)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_L_UP);
                //        else if (unitNo == 1)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_R_UP);
                //        break;
                //    }

                //    //Machine.IO.GetIn(inIo[(int)IOIn.TRF_Z_UP, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.TRF_Z_DOWN, unitNo], ref ret2);

                //    if (ret1 == 1 && ret2 == 0)
                //        NextStep();
                //    break;

                //case STEP.TRF_DOWN:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.TRF_Z_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.TRF_Z_DOWN, unitNo], 1);
                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.TRF_DOWN_CHECK;
                //    break;

                //case STEP.TRF_DOWN_CHECK:
                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (unitNo == 0)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_L_DOWN);
                //        else if (unitNo == 1)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_R_DOWN);
                //        break;
                //    }

                //    //Machine.IO.GetIn(inIo[(int)IOIn.TRF_Z_UP, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.TRF_Z_DOWN, unitNo], ref ret2);

                //    if (ret1 == 0 && ret2 == 1)
                //    {
                //        Util.Delay(300);
                //        NextStep();
                //    }
                //    break;

                //case STEP.TRF_CLAMP:
                //    if (Machine.recipe.Option(RecipeDefine.OPTION.CAM_TRF_GRIP_USE) == 0)
                //    {
                //        NextStep();
                //        break;
                //    }

                //    TransferClamp();

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.TRF_CLAMP_CHECK;
                //    break;

                //case STEP.TRF_CLAMP_CHECK:
                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (unitNo == 0)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_L1_CLAMP);
                //        else if (unitNo == 1)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_R1_CLAMP);
                //        break;
                //    }

                //    if (TransferClampCheck())
                //    {
                //        Util.Delay(300);
                //        NextStep();
                //    }
                //    break;

                //case STEP.TRF_UNCLAMP:
                //    TransferUnclamp();

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.TRF_UNCLAMP_CHECK;
                //    break;

                //case STEP.TRF_UNCLAMP_CHECK:
                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (unitNo == 0)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_L1_UNCLAMP);
                //        else if (unitNo == 1)
                //            SetError(ECODE.TIMEOUT_CAM_TRF_R1_UNCLAMP);
                //        break;
                //    }

                //    if (TransferUnclampCheck())
                //        NextStep();
                //    break;
                #endregion
                #region TRANSFER VACUUM
                case STEP.TRF_VACON_RECEIVE:
                case STEP.TRF_VACON_SELF:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    if (Step == STEP.TRF_VACON_RECEIVE) SetValue = true;
                    if (Step == STEP.TRF_VACON_SELF) SetValue = false;

                    TransferVacOn(SetValue);

                    if (Step == STEP.TRF_VACON_RECEIVE) Step = STEP.TRF_VACON_RECEIVE_CHECK;
                    if (Step == STEP.TRF_VACON_SELF) Step = STEP.TRF_VACON_SELF_CHECK;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRF_VACON_RECEIVE_CHECK:
                case STEP.TRF_VACON_SELF_CHECK:
                    if (Step == STEP.TRF_VACON_RECEIVE_CHECK) SetValue = true;
                    if (Step == STEP.TRF_VACON_SELF_CHECK) SetValue = false;

                    if (TransferVacOnCheck(SetValue))
                        Step = STEP.VAC_ON_WAIT;
                    break;

                case STEP.TRF_VACOFF_SELF:
                case STEP.TRF_VACOFF_SELF_BYPASS:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    TransferVacOff();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    if (Step == STEP.TRF_VACOFF_SELF) Step = STEP.TRF_VACOFF_SELF_CHECK;
                    if (Step == STEP.TRF_VACOFF_SELF_BYPASS) NextStep();
                    break;

                case STEP.TRF_VACOFF_SELF_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        if (unitNo == 0)
                            SetError(ECODE.TIMEOUT_BUFFER_L1_VACOFF);
                        else if (unitNo == 1)
                            SetError(ECODE.TIMEOUT_BUFFER_R1_VACOFF);
                        break;
                    }

                    if (TransferVacOffCheck())
                        NextStep();
                    break;
                #endregion
                #region BUFFER
                case STEP.BUF_VACON_RECEIVE:
                case STEP.BUF_VACON_RECEIVE_BYPASS:
                case STEP.BUF_VACON_SELF:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    if (Step == STEP.BUF_VACON_RECEIVE || Step == STEP.BUF_VACON_RECEIVE_BYPASS) SetValue = true;
                    if (Step == STEP.BUF_VACON_SELF) SetValue = false;

                    BufferVacOn(SetValue);

                    if (Step == STEP.BUF_VACON_RECEIVE) Step = STEP.BUF_VACON_RECEIVE_CHECK;
                    if (Step == STEP.BUF_VACON_RECEIVE_BYPASS) NextStep();
                    if (Step == STEP.BUF_VACON_SELF) Step = STEP.BUF_VACON_SELF_CHECK;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.BUF_VACON_RECEIVE_CHECK:
                case STEP.BUF_VACON_SELF_CHECK:
                    if (Step == STEP.BUF_VACON_RECEIVE_CHECK) SetValue = true;
                    if (Step == STEP.BUF_VACON_SELF_CHECK) SetValue = false;

                    if (BufferVacOnCheck(SetValue))
                        Step = STEP.VAC_ON_WAIT;
                    break;

                case STEP.BUF_VACOFF_SELF:
                case STEP.BUF_VACOFF_SELF_BYPASS:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    BufferVacOff();

                    if (Step == STEP.BUF_VACOFF_SELF) Step = STEP.BUF_VACOFF_SELF_CHECK;
                    if (Step == STEP.BUF_VACOFF_SELF_BYPASS) NextStep();

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.BUF_VACOFF_SELF_CHECK:
                    if (BufferVacOffCheck())
                        Step = STEP.PURGE_OFF_WAIT;
                    break;

                case STEP.LD_BUF_DETECT_ON_CHECK:
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH1, unitNo], ref ret1);
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH2, unitNo], ref ret2);

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        ret1 = 1;
                        ret2 = 1;
                    }
                    if (ret1 != 1)
                    {
                        if (unitNo == 0) SetError(ECODE.BUF_LEFT_REAR_LEFT_DETECT_SENSOR_ERROR);
                        if (unitNo == 1) SetError(ECODE.BUF_RIGHT_REAR_LEFT_DETECT_SENSOR_ERROR);
                    }
                    else if (ret2 != 1)
                    {
                        if (unitNo == 0) SetError(ECODE.BUF_LEFT_REAR_RIGHT_DETECT_SENSOR_ERROR);
                        if (unitNo == 1) SetError(ECODE.BUF_RIGHT_REAR_RIGHT_DETECT_SENSOR_ERROR);
                    }
                    else NextStep();
                    break;

                case STEP.LD_BUF_DETECT_OFF_CHECK:
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH1, unitNo], ref ret1);
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH2, unitNo], ref ret2);

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        ret1 = 0;
                        ret2 = 0;
                    }
                    if (ret1 != 0)
                    {
                        if (unitNo == 0) SetError(ECODE.BUF_LEFT_REAR_LEFT_DETECT_SENSOR_ERROR);
                        if (unitNo == 1) SetError(ECODE.BUF_RIGHT_REAR_LEFT_DETECT_SENSOR_ERROR);
                    }
                    else if (ret2 != 0)
                    {
                        if (unitNo == 0) SetError(ECODE.BUF_LEFT_REAR_RIGHT_DETECT_SENSOR_ERROR);
                        if (unitNo == 1) SetError(ECODE.BUF_RIGHT_REAR_RIGHT_DETECT_SENSOR_ERROR);
                    }
                    else NextStep();
                    break;

                case STEP.ULD_BUF_DETECT_ON_CHECK:
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH3, unitNo], ref ret1);
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH4, unitNo], ref ret2);

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        ret1 = 1;
                        ret2 = 1;
                    }

                    if (ret1 == 1)
                    {
                        Machine.Parts[place2[0, unitNo]].exist = true;
                        Machine.Parts[transfer[0, unitNo]].exist = false;
                    }
                    else
                    {
                        if (unitNo == 0) SetError(ECODE.BUF_LEFT_FRONT_LEFT_DETECT_SENSOR_ERROR);
                        if (unitNo == 1) SetError(ECODE.BUF_RIGHT_FRONT_LEFT_DETECT_SENSOR_ERROR);
                        break;
                    }

                    if (ret2 == 1)
                    {
                        Machine.Parts[place2[1, unitNo]].exist = true;
                        Machine.Parts[transfer[1, unitNo]].exist = false;
                    }
                    else
                    {
                        if (unitNo == 0) SetError(ECODE.BUF_LEFT_FRONT_RIGHT_DETECT_SENSOR_ERROR);
                        if (unitNo == 1) SetError(ECODE.BUF_RIGHT_FRONT_RIGHT_DETECT_SENSOR_ERROR);
                        break;
                    }
                    NextStep();
                    break;

                case STEP.ULD_BUF_DETECT_OFF_CHECK:
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH3, unitNo], ref ret1);
                    Machine.IO.GetIn(inIo[(int)IOIn.DETACH4, unitNo], ref ret2);

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        ret1 = 0;
                        ret2 = 0;
                    }
                    if (ret1 != 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > 5000)
                        {
                            if (unitNo == 0) SetError(ECODE.BUF_LEFT_FRONT_LEFT_DETECT_SENSOR_ERROR);
                            if (unitNo == 1) SetError(ECODE.BUF_RIGHT_FRONT_LEFT_DETECT_SENSOR_ERROR);
                        }
                    }
                    else if (ret2 != 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > 5000)
                        {
                            if (unitNo == 0) SetError(ECODE.BUF_LEFT_FRONT_RIGHT_DETECT_SENSOR_ERROR);
                            if (unitNo == 1) SetError(ECODE.BUF_RIGHT_FRONT_RIGHT_DETECT_SENSOR_ERROR);
                        }
                    }
                    else NextStep();
                    break;
                #endregion
                //#region PEELING I/O
                //case STEP.PEELING_RETURN:
                //case STEP.PEELING_RETURN_BYPASS:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_RETURN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_RETURN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_TURN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_TURN, unitNo], 0);

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    if (Step == STEP.PEELING_RETURN_BYPASS) NextStep();
                //    else Step = STEP.PEELING_RETURN_CHECK;
                //    break;

                //case STEP.PEELING_RETURN_CHECK: //Cylinder 오동작 알람 필요
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_RETURN, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_RETURN, unitNo], ref ret2);

                //    //if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    //{
                //    //    if (ret1 == 0)
                //    //    {
                //    //        if (unitNo == 0)
                //    //            SetError(ECODE.TIMEOUT_PEELING_L1_RETURN);
                //    //        else if (unitNo == 1)
                //    //            SetError(ECODE.TIMEOUT_PEELING_R1_RETURN);
                //    //    }
                //    //    else if (ret2 == 0)
                //    //    {
                //    //        if (unitNo == 0)
                //    //            SetError(ECODE.TIMEOUT_PEELING_L2_RETURN);
                //    //        else if (unitNo == 1)
                //    //            SetError(ECODE.TIMEOUT_PEELING_R2_RETURN);
                //    //    }
                //    //    break;
                //    //}

                //    //if (ret1 == 1 && ret2 == 1)
                //    //{
                //    //    Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PEELING_RETURN_DELAY_TIME));
                //    //    NextStep();
                //    //}
                //    NextStep();
                //    break;

                //case STEP.PEELING_TURN:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_TURN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_TURN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_RETURN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_RETURN, unitNo], 0);

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.PEELING_TURN_CHECK;
                //    break;

                //case STEP.PEELING_TURN_CHECK: //Cylinder 오동작 알람 필요
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_TURN, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_TURN, unitNo], ref ret2);

                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (ret1 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_TURN);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R1_TURN);
                //        }
                //        else if (ret2 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L2_TURN);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R2_TURN);
                //        }
                //        break;
                //    }

                //    if (ret1 == 1 && ret2 == 1)
                //    {
                //        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PEELING_TURN_DELAY_TIME));
                //        NextStep();
                //    }

                //    break;

                //case STEP.PEELING_CLAMP:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_CLAMP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_CLAMP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_UNCLAMP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_UNCLAMP, unitNo], 0);

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.PEELING_CLAMP_CHECK;
                //    break;

                //case STEP.PEELING_CLAMP_CHECK: //Cylinder 오동작 알람 필요
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_UNCLAMP, unitNo], ref ret3);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_UNCLAMP, unitNo], ref ret4);

                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (ret3 != 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_CLAMP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R1_CLAMP);
                //        }
                //        else if (ret4 != 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L2_CLAMP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R2_CLAMP);
                //        }
                //        break;
                //    }

                //    if (ret3 == 0 && ret4 == 0)
                //    {
                //        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PEELING_CLAMP_DELAY_TIME));
                //        NextStep();
                //    }
                //    break;

                //case STEP.PEELING_UNCLAMP:
                //case STEP.PEELING_UNCLAMP_BYPASS:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_UNCLAMP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_UNCLAMP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_CLAMP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_CLAMP, unitNo], 0);

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    if (Step == STEP.PEELING_UNCLAMP_BYPASS) NextStep();
                //    else Step = STEP.PEELING_UNCLAMP_CHECK;
                //    break;

                //case STEP.PEELING_UNCLAMP_CHECK: //Cylinder 오동작 알람 필요
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_UNCLAMP, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_UNCLAMP, unitNo], ref ret2);

                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (ret1 != 1)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_UNCLAMP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R1_UNCLAMP);
                //        }
                //        else if (ret2 != 1)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L2_UNCLAMP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R2_UNCLAMP);
                //        }
                //        break;
                //    }

                //    if (ret1 == 1 && ret2 == 1)
                //    {
                //        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PEELING_UNCLAMP_DELAY_TIME));
                //        NextStep();
                //    }
                //    break;

                //case STEP.PEELING_UP:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z1_UP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z2_UP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z1_UP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z2_UP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z1_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z1_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z2_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z2_DOWN, unitNo], 0);

                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    Step = STEP.PEELING_UP_CHECK;
                //    break;

                //case STEP.PEELING_UP_CHECK:
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_Z1_UP, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_Z2_UP, unitNo], ref ret2);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_Z1_UP, unitNo], ref ret3);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_Z2_UP, unitNo], ref ret4);

                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed >
                //        Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (ret1 == 0 || ret2 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_UP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R1_UP);
                //        }
                //        else if (ret3 == 0 || ret4 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L2_UP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R2_UP);
                //        }
                //        break;
                //    }

                //    if (ret1 == 1 && ret2 == 1 && ret3 == 1 && ret4 == 1)
                //    {
                //        Util.Delay(300);
                //        NextStep();
                //    }
                //    break;

                //case STEP.PEELING_READY:
                //case STEP.PEELING_READY_BYPASS:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z2_UP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z2_UP, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z1_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z1_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z1_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z1_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z2_DOWN, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z2_DOWN, unitNo], 0);
                //    if (Step == STEP.PEELING_READY_BYPASS) NextStep();
                //    else Step = STEP.PEELING_READY_CHECK;
                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    break;

                //case STEP.PEELING_READY_CHECK: // 하부 Z축 Cylinder가 up 상태가 Ready
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_Z2_UP, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_Z2_UP, unitNo], ref ret2);

                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed >
                //       Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (ret1 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_UP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_UP);
                //        }
                //        else if (ret2 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L2_UP);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R2_UP);
                //        }
                //        break;
                //    }

                //    if (ret1 == 1 && ret2 == 1)
                //    {
                //        Util.Delay(300);
                //        NextStep();
                //    }
                //    break;

                //case STEP.PEELING_DOWN:
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z2_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z2_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z1_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z1_UP, unitNo], 0);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z1_DOWN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_Z2_DOWN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z1_DOWN, unitNo], 1);
                //    //Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_Z2_DOWN, unitNo], 1);

                //    Step = STEP.PEELING_DOWN_CHECK;
                //    timeWait[(int)TIMER.TIMEOUT].Start();
                //    break;

                //case STEP.PEELING_DOWN_CHECK:
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_Z1_DOWN, unitNo], ref ret1);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING1_Z2_DOWN, unitNo], ref ret2);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_Z1_DOWN, unitNo], ref ret3);
                //    //Machine.IO.GetIn(inIo[(int)IOIn.PEELING2_Z2_DOWN, unitNo], ref ret4);

                //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed >
                //       Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                //    {
                //        if (ret1 == 0 || ret2 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L1_DOWN);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R1_DOWN);
                //        }
                //        else if (ret3 == 0 || ret4 == 0)
                //        {
                //            if (unitNo == 0)
                //                SetError(ECODE.TIMEOUT_PEELING_L2_DOWN);
                //            else if (unitNo == 1)
                //                SetError(ECODE.TIMEOUT_PEELING_R2_DOWN);
                //        }
                //        break;
                //    }

                //    if (ret1 == 1 && ret2 == 1 && ret3 == 1 && ret4 == 1)
                //    {
                //        NextStep();
                //    }
                //    break;

                //case STEP.PEELING_BLOW:
                //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING_BLOW, unitNo], 1);
                //    Machine.IO.SetOut(outIo[(int)IOOut.DUST_SUCTION, unitNo], 1);
                //    Task.Run(() =>
                //    {
                //        int count = (int)Machine.param.Option(ParameterDefine.OPTION.PEELING_UNCLAMP_REPEAT_COUNT);
                //        for (int i = 0; i < count; i++)
                //        {
                //            ClampAction(false);
                //            Util.Delay(100);
                //            ClampAction(true);
                //            Util.Delay(100);
                //        }
                //        ClampAction(false);
                //        Util.Delay(3000);
                //        Machine.IO.SetOut(outIo[(int)IOOut.PEELING_BLOW, unitNo], 0);
                //        Machine.IO.SetOut(outIo[(int)IOOut.DUST_SUCTION, unitNo], 0);
                //    });
                //    NextStep();
                //    break;

                //case STEP.PEELING_BLOW_OFF:
                //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING_BLOW, unitNo], 0);
                //    Machine.IO.SetOut(outIo[(int)IOOut.DUST_SUCTION, unitNo], 0);
                //    NextStep();
                //    break;
                //    #endregion
            }
        }

        #region TRANSFER
        private void TransferClamp()
        {
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_CLAMP, unitNo], 1);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_CLAMP, unitNo], 1);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_UNCLAMP, unitNo], 0);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_UNCLAMP, unitNo], 0);
        }

        private bool TransferClampCheck() // 실린더 오동작 알람 추가 필요
        {
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            //Machine.IO.GetIn(inIo[(int)IOIn.TRF1_CLAMP, unitNo], ref ret1);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF2_CLAMP, unitNo], ref ret2);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF1_UNCLAMP, unitNo], ref ret3);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF2_UNCLAMP, unitNo], ref ret4);

            if (Machine.Parts[transfer[0, unitNo]].exist || Machine.Parts[transfer[1, unitNo]].exist)
            {
                if (ret1 == 0 || ret2 == 0 || ret3 == 1 || ret4 == 1)
                    return false;
            }
            return true;
        }

        private void TransferUnclamp()
        {
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_UNCLAMP, unitNo], 1);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_UNCLAMP, unitNo], 1);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_CLAMP, unitNo], 0);
            //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_CLAMP, unitNo], 0);
        }

        private bool TransferUnclampCheck() // 실린더 오동작 알람 추가 필요
        {
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            //Machine.IO.GetIn(inIo[(int)IOIn.TRF1_CLAMP, unitNo], ref ret1);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF2_CLAMP, unitNo], ref ret2);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF1_UNCLAMP, unitNo], ref ret3);
            //Machine.IO.GetIn(inIo[(int)IOIn.TRF2_UNCLAMP, unitNo], ref ret4);

            return (ret1 == 0 && ret2 == 0 && ret3 == 1 && ret4 == 1);
        }

        private void TransferVacOff() //확인필요 
        {
            if (Machine.Parts[transfer[0, unitNo]].exist)
            {
                //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_VACON, unitNo], 0);
                //Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 1);
            }
            if (Machine.Parts[transfer[1, unitNo]].exist)
            {
                //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_VACON, unitNo], 0);
                //Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 1);
            }
        }

        private bool TransferVacOffCheck()
        {
            //uint ret1 = 0;
            //uint ret2 = 0;

            //if (Machine.Parts[transfer[0, unitNo]].exist)
            //{
            //    Machine.IO.GetIn(inIo[(int)IOIn.TRF1_VACON, unitNo], ref ret1);
            //    if (ret1 == 1) return false;
            //    Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));
            //    Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);
            //}
            //else
            //    Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);

            //if (Machine.Parts[transfer[1, unitNo]].exist)
            //{
            //    Machine.IO.GetIn(inIo[(int)IOIn.TRF2_VACON, unitNo], ref ret2);
            //    if (ret2 == 1) return false;
            //    Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));
            //    Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);
            //}
            //else
            //    Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);

            return true;
        }

        private void TransferVacOn(bool receive)
        {
            //if (receive)
            //{
            //    if (Machine.Parts[place1[0, unitNo]].exist)
            //    {
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF1_VACON, unitNo], 1);
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);
            //    }
            //    if (Machine.Parts[place1[1, unitNo]].exist)
            //    {
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF2_VACON, unitNo], 1);
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);
            //    }
            //}
            //else
            //{
            //    if (Machine.Parts[transfer[0, unitNo]].exist)
            //    {
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF1_VACON, unitNo], 1);
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF1_PURGE, unitNo], 0);
            //    }
            //    if (Machine.Parts[transfer[1, unitNo]].exist)
            //    {
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF2_VACON, unitNo], 1);
            //        Machine.IO.SetOut(outIo[(int)IOOut.TRF2_PURGE, unitNo], 0);
            //    }
            //}

            timeWait[(int)TIMER.TIMEOUT].Reset();
        }

        private bool TransferVacOnCheck(bool receive)
        {
            //if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
            //    return true;

            //bool timeOut = false;
            //bool returnValue = true;

            //if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
            //    timeOut = true;

            //ECODE[,] errTrf1VacOn = { { ECODE.TIMEOUT_CAM_TRF_L1_VACON, ECODE.TIMEOUT_CAM_TRF_R1_VACON } };
            //ECODE[,] errTrf2VacOn = { { ECODE.TIMEOUT_CAM_TRF_L2_VACON, ECODE.TIMEOUT_CAM_TRF_R2_VACON } };

            //uint ret1 = 0;
            //uint ret2 = 0;

            //if (receive)
            //{
            //    if (Machine.Parts[place1[0, unitNo]].exist) //Buf1 좌측 -> TRF 좌측
            //    {
            //        Machine.IO.GetIn(inIo[(int)IOIn.TRF1_VACON, unitNo], ref ret1);
            //        if (ret1 == 0)
            //        {
            //            returnValue = false;
            //            if (timeOut)
            //                SetError(errTrf1VacOn[0, unitNo]);
            //        }
            //    }
            //    if (Machine.Parts[place1[1, unitNo]].exist) //Buf1 우측 -> TRF 우측
            //    {
            //        Machine.IO.GetIn(inIo[(int)IOIn.TRF2_VACON, unitNo], ref ret1);
            //        if (ret1 == 0)
            //        {
            //            returnValue = false;
            //            if (timeOut)
            //                SetError(errTrf2VacOn[0, unitNo]);
            //        }
            //    }
            //}
            //else
            //{
            //    if (Machine.Parts[transfer[0, unitNo]].exist)
            //    {
            //        Machine.IO.GetIn(inIo[(int)IOIn.TRF1_VACON, unitNo], ref ret1);
            //        if (ret1 == 0)
            //        {
            //            returnValue = false;
            //            if (timeOut)
            //                SetError(errTrf1VacOn[0, unitNo]);
            //        }
            //    }
            //    if (Machine.Parts[transfer[1, unitNo]].exist)
            //    {
            //        Machine.IO.GetIn(inIo[(int)IOIn.TRF2_VACON, unitNo], ref ret2);
            //        if (ret2 == 0)
            //        {
            //            returnValue = false;
            //            if (timeOut)
            //                SetError(errTrf2VacOn[0, unitNo]);
            //        }
            //    }
            //}

            return true;
        }

        private bool MoveTransferY(YPOS Pos)
        {
            if (Pos == YPOS.UNLOAD)
            {
                if (!CheckInterlockBufferAssembler()) return false;
            }
            if (Pos == YPOS.LOAD)
            {
                if (!CheckInterlockBufferLoader()) return false;
            }

            double[] pos = { 0.0 };
            double[] vel = { 0.0 };
            double[] acc = { 0.0 };
            double[] dec = { 0.0 };

            pos[0] = 1000 * Machine.recipe.Position(transferPos[(int)Pos, unitNo]);
            vel[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_MOVE_VEL);
            acc[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_MOVE_ACC);
            dec[0] = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_MOVE_DEC);

            Machine.motion.MoveAxisAbs(transferAxis[unitNo], pos[0], vel[0], acc[0], dec[0]);
            return true;
        }
        #endregion

        private bool CheckInterlockBufferLoader()
        {
            bool isManualMode = Machine.status.state == SystemState.SystemStateIDLE;

            double xPos = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref xPos);
            double yPos = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref yPos);
            double zPos = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Z, ref zPos);
            double placePosX = 0;
            double placePosY = 0;
            double readyPosZ = 0;
            if (unitNo == 0)
            {
                placePosX = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_L_POS);
                placePosY = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_L_POS);
            }
            if (unitNo == 1)
            {
                placePosX = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_L_POS);
                placePosY = Machine.recipe.Position(RecipeDefine.POSITION.PROD_LOADER_Y_BUF_R_L_POS);
            }
            readyPosZ = Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Z_READY_POS);
            if (Math.Abs(xPos / 1000 - placePosX) < 5
                && Math.Abs(yPos / 1000 - placePosY) < 5
                && zPos / 1000 > readyPosZ) return false;
            if (Machine.prod_loader_start_place[unitNo] && !isManualMode) return false;
            return true;
        }
        private bool CheckInterlockBufferAssembler()
        {
            bool isManualMode = Machine.status.state == SystemState.SystemStateIDLE;

            double xPos = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref xPos);
            double yPos = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref yPos);
            double zPos = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Z, ref zPos);
            double placePosX = 0;
            double placePosY = 0;
            double readyPosZ = 0;
            if (unitNo == 0)
            {
                placePosX = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_BUF_L_POS);
                placePosY = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_BUF_L_POS);
            }
            if (unitNo == 1)
            {
                placePosX = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_X_BUF_R_POS);
                placePosY = Machine.recipe.Position(RecipeDefine.POSITION.ASSEMBLER_Y_BUF_R_POS);
            }
            readyPosZ = Machine.param.Position(ParameterDefine.POSITION.ASSEMBLER_Z_READY_POS);
            if (Math.Abs(xPos / 1000 - placePosX) < 0.2
                && Math.Abs(yPos / 1000 - placePosY) < 0.2
                && zPos / 1000 > readyPosZ)
                return false;
            if (Machine.assembler_start_pick[unitNo] && !isManualMode) return false;
            return true;
        }
        #region PEELING
        #endregion

        #region BUFFER
        private void BufferVacOn(bool receive)
        {
            //    Machine.IO.SetOut(outIo[(int)IOOut.BUF1_PURGE, unitNo], 0);
            //    Machine.IO.SetOut(outIo[(int)IOOut.BUF2_PURGE, unitNo], 0);

            //    if (receive)
            //    {
            //        if (Machine.Parts[transfer[0, unitNo]].exist)
            //        {
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF1_VACON, 0], 1);
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF2_VACON, 0], 1);

            //        }
            //        if (Machine.Parts[transfer[1, unitNo]].exist)
            //        {
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF1_VACON, 1], 1);
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF2_VACON, 1], 1);
            //        }
            //    }
            //    else
            //    {
            //        // UNLOADING BUFFER 1
            //        if (Machine.Parts[place2[0, unitNo]].exist)
            //        {
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF1_VACON, unitNo], 1);
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF1_PURGE, unitNo], 0);
            //        }

            //        // UNLOADING BUFFER 2
            //        if (Machine.Parts[place2[1, unitNo]].exist)
            //        {
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF2_VACON, unitNo], 1);
            //            Machine.IO.SetOut(outIo[(int)IOOut.BUF2_PURGE, unitNo], 0);
            //        }
            //    }

            //    timeWait[(int)TIMER.TIMEOUT].Reset();
        }

        private bool BufferVacOnCheck(bool receive)
        {
            //    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
            //        return true;

            //    bool timeOut = false;
            bool returnValue = true;

            //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
            //        timeOut = true;

            //    ECODE[,] errBufVacOn = { { ECODE.TIMEOUT_BUF2_L1_VACON, ECODE.TIMEOUT_BUF2_R1_VACON },
            //        { ECODE.TIMEOUT_BUF2_L2_VACON, ECODE.TIMEOUT_BUF2_R2_VACON } };

            //    uint ret1 = 0;
            //    uint ret2 = 0;

            //    if (receive)
            //    {
            //        // UNLOADING BUFFER
            //        if (Machine.Parts[transfer[0, unitNo]].exist)
            //        {
            //            Machine.IO.GetIn(inIo[(int)IOIn.BUF1_VAC, unitNo], ref ret1);
            //            //Machine.IO.GetIn(inIo[(int)IOIn.BUF2_VAC, unitNo], ref ret2);
            //            if (ret1 == 0 /*|| ret2 == 0*/)
            //            {
            //                returnValue = false;
            //                if (timeOut)
            //                    SetError(errBufVacOn[0, unitNo]);
            //            }
            //        }
            //        if (Machine.Parts[transfer[1, unitNo]].exist)
            //        {
            //            //Machine.IO.GetIn(inIo[(int)IOIn.BUF1_VAC, unitNo], ref ret1);
            //            Machine.IO.GetIn(inIo[(int)IOIn.BUF2_VAC, unitNo], ref ret2);
            //            if (/*ret1 == 0 || */ret2 == 0)
            //            {
            //                returnValue = false;
            //                if (timeOut)
            //                    SetError(errBufVacOn[1, unitNo]);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // UNLOADING BUFFER
            //        if (Machine.Parts[place2[0, unitNo]].exist)
            //        {
            //            Machine.IO.GetIn(inIo[(int)IOIn.BUF1_VAC, unitNo], ref ret1);
            //            if (ret1 == 0)
            //            {
            //                returnValue = false;
            //                if (timeOut)
            //                    SetError(errBufVacOn[0, unitNo]);
            //            }
            //        }
            //        if (Machine.Parts[place2[1, unitNo]].exist)
            //        {
            //            Machine.IO.GetIn(inIo[(int)IOIn.BUF2_VAC, unitNo], ref ret1);
            //            if (ret1 == 0)
            //            {
            //                returnValue = false;
            //                if (timeOut)
            //                    SetError(errBufVacOn[1, unitNo]);
            //            }
            //        }
            //    }

            return returnValue;
        }

        private void BufferVacOff()
        {
            //    // UNLOADING BUFFER
            //    if (Machine.Parts[place2[0, unitNo]].exist)
            //    {
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF1_VACON, unitNo], 0);
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF1_PURGE, unitNo], 1);
            //        //Machine.IO.SetOut(outIo[(int)IOOut.BUF2_VACON, unitNo], 0);
            //        //Machine.IO.SetOut(outIo[(int)IOOut.BUF2_PURGE, unitNo], 1);
            //    }
            //    if (Machine.Parts[place2[1, unitNo]].exist)
            //    {
            //        //Machine.IO.SetOut(outIo[(int)IOOut.BUF1_VACON, unitNo], 0);
            //        //Machine.IO.SetOut(outIo[(int)IOOut.BUF1_PURGE, unitNo], 0);
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF2_VACON, unitNo], 0);
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF2_PURGE, unitNo], 1);
            //    }
        }

        private bool BufferVacOffCheck()
        {
            //    uint ret1 = 0;
            //    uint ret2 = 0;

            //    // UNLOADING BUFFER
            //    if (Machine.Parts[place2[0, unitNo]].exist)
            //    {
            //        Machine.IO.GetIn(inIo[(int)IOIn.BUF1_VAC, unitNo], ref ret1);
            //        if (ret1 == 1) return false;
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF1_PURGE, unitNo], 0);
            //    }
            //    else
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF1_PURGE, unitNo], 0);

            //    if (Machine.Parts[place2[1, unitNo]].exist)
            //    {
            //        Machine.IO.GetIn(inIo[(int)IOIn.BUF2_VAC, unitNo], ref ret1);
            //        if (ret1 == 1) return false;
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF2_PURGE, unitNo], 0);
            //    }
            //    else
            //        Machine.IO.SetOut(outIo[(int)IOOut.BUF2_PURGE, unitNo], 0);

            return true;
        }
        #endregion

        private void ClampAction(bool onOff)
        {
            //if (onOff)
            //{
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_UNCLAMP, unitNo], 0);
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_UNCLAMP, unitNo], 0);
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_CLAMP, unitNo], 1);
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_CLAMP, unitNo], 1);
            //}
            //else
            //{
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_UNCLAMP, unitNo], 1);
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_UNCLAMP, unitNo], 1);
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING1_CLAMP, unitNo], 0);
            //    Machine.IO.SetOut(outIo[(int)IOOut.PEELING2_CLAMP, unitNo], 0);
            //}
        }

        public override void SetHeadTarget(int iTarget)
        {
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
