using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMASSEMBLYMACHINE.Define
{
    public enum SystemMode
    {
        SystemModeAUTO, // 정상 가능 상태
        SystemModeBYPASS, // JIG 투입 및 TEST 하지 않고 바로 배출 진행
        SystemModeDRYRUN, // SET 없는 상태로 Simulation 동작 진행
        SystemModePASSRUN, // SET 투입하여 TEST 설정 시간 이후 무조건 양품 배출 진행
    };

    public enum SystemState
    {
        SystemStateIDLE,
        SystemStateRUN,
        SystemStateRUNEMPTY,
        SystemStateERROR,
        SystemStateINIT,
        SystemStateSLEEP,
    }

    public class SystemStatus
    {
        public SystemMode mode = SystemMode.SystemModeAUTO;
        public SystemState state = SystemState.SystemStateIDLE;

        public SystemStatus()
        {

        }
    }
}
