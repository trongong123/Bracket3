using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProductData
    {
        public bool exist = false;
        public bool loadingEnable = false;
        public bool loading = false;
        public bool unloadingRequest = false;
        public bool unloading = false;
        public bool reverseRequest = false; // 역이재 요청
        public bool reversing = false; // 역이재 진행 중
        public bool ready = false;
        public bool adjust = false;
        public int productCount = 0;
        public bool captureReq = false;

        public int from_id = -1;

        public ProductData()
        {

        }

        public void CopyFrom(ProductData data)
        {
            exist = data.exist;
            from_id = data.from_id;
        }

        public void ClearInterface()
        {
            loadingEnable = false;
            loading = false;
            unloadingRequest = false;
            unloading = false;
            reverseRequest = false;
            reversing = false;
            adjust = false;
        }

        public void ClearData()
        {
            exist = false;
            from_id = -1;
        }
    }
}
