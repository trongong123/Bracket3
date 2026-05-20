using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopEng.Device
{
    public class IOCONFIG
    {
        public int boardNo = 0;
        public int pos = 0;
        public uint id = 0;
        public string name = "";
    }

    public class IODATA
    {

    }

    public abstract partial class IIO
    {
        protected bool openned = false;
        public object lockControl = new object();

        //public AXISDATA[] axisData = null;

        public IIO()
        {
        }

        public bool IsOpenned() { return openned; }
        public abstract void Open();
        public abstract void Close();
        public abstract void GetIn(int address, ref uint value);
        public abstract void GetOut(int address, ref uint value);
        public abstract void SetOut(int address, uint value);
    }
}
