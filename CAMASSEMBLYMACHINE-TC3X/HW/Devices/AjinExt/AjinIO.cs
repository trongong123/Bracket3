using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopEng.Device.AjinExt
{
    public class AjinIO : IIO
    {
        public AjinIO()
        {

        }

        public override void Open()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if (CAXL.AxlIsOpened() != 1)
                {
                    if (CAXL.AxlOpen(7) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        throw new Exception("Failed to read library of AjinIO.");

                    uint ret = 0;
                    if (CAXD.AxdInfoIsDIOModule(ref ret) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        throw new Exception("Failed to read library of AjinIO.");

                    if ((AXT_EXISTENCE)ret != AXT_EXISTENCE.STATUS_EXIST)
                        throw new Exception("IO module does not exist.");

                    int moduleCount = 0;
                    if (CAXD.AxdInfoGetModuleCount(ref moduleCount) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        throw new Exception("Failed to read library of AjinIO.");

                    IOCONFIG module = new IOCONFIG();

                    for (int i = 0; i < moduleCount; i++)
                    {
                        if (CAXD.AxdInfoGetModule(i, ref module.boardNo, ref module.pos, ref module.id) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                            continue;

                        switch ((AXT_MODULE)module.id)
                        {
                            case AXT_MODULE.AXT_SIO_DI32: module.name = "AXT_SIO_DI32"; break;
                            case AXT_MODULE.AXT_SIO_DO32P: module.name = "AXT_SIO_DO32P"; break;
                            case AXT_MODULE.AXT_SIO_DB32P: module.name = "AXT_SIO_DB32P"; break;
                            case AXT_MODULE.AXT_SIO_DO32T: module.name = "AXT_SIO_DO32T"; break;
                            case AXT_MODULE.AXT_SIO_DB32T: module.name = "AXT_SIO_DB32T"; break;
                            case AXT_MODULE.AXT_SIO_RDI32: module.name = "AXT_SIO_RDI32"; break;
                            case AXT_MODULE.AXT_SIO_RDO32: module.name = "AXT_SIO_RDO32"; break;
                            case AXT_MODULE.AXT_SIO_RDB128MLII: module.name = "AXT_SIO_RDB128MLII"; break;
                            case AXT_MODULE.AXT_SIO_RSIMPLEIOMLII: module.name = "AXT_SIO_RSIMPLEIOMLII"; break;
                            case AXT_MODULE.AXT_SIO_RDO16AMLII: module.name = "AXT_SIO_RDO16AMLII"; break;
                            case AXT_MODULE.AXT_SIO_RDO16BMLII: module.name = "AXT_SIO_RDO16BMLII"; break;
                            case AXT_MODULE.AXT_SIO_RDB96MLII: module.name = "AXT_SIO_RDB96MLII"; break;
                            case AXT_MODULE.AXT_SIO_RDO32RTEX: module.name = "AXT_SIO_RDO32RTEX"; break;
                            case AXT_MODULE.AXT_SIO_RDI32RTEX: module.name = "AXT_SIO_RDI32RTEX"; break;
                            case AXT_MODULE.AXT_SIO_RDB32RTEX: module.name = "AXT_SIO_RDB32RTEX"; break;
                            case AXT_MODULE.AXT_SIO_DI32_P: module.name = "AXT_SIO_DI32_P"; break;
                            case AXT_MODULE.AXT_SIO_DO32T_P: module.name = "AXT_SIO_DO32T_P"; break;
                            case AXT_MODULE.AXT_SIO_RDB32T: module.name = "AXT_SIO_RDB32T"; break;
                            case AXT_MODULE.AXT_ECAT_DIO: module.name = "AXT_ECAT_DIO"; break;
                        }
                    }

                    openned = true;
                }
                else
                    openned = true;
            }
            catch
            {
                throw new Exception("Failed to read library of AjinIO.");
            }
        }

        public override void Close()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            CAXL.AxlClose();
            openned = false;
        }

        public override void GetIn(int address, ref uint value)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if (address < 0) return;
                if (CAXD.AxdiReadInport(address, ref value) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    throw new Exception($"GetIn[{address}] operation failed.");
            }
            catch
            {
                //throw new Exception($"GetIn[{address}] operation failed.");
            }
        }

        public override void GetOut(int address, ref uint value)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if (CAXD.AxdoReadOutport(address, ref value) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    throw new Exception($"GetOut[{address}] operation failed.");
            }
            catch
            {
                //throw new Exception($"GetOut[{address}] operation failed.");
            }
        }

        public override void SetOut(int address, uint value)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    if (CAXD.AxdoWriteOutport(address, value) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        throw new Exception($"SetOut[{address}] operation failed.");
                }
            }
            catch
            {
                //throw new Exception($"SetOut[{address}] operation failed.");
            }
        }
    }
}
