using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.Process
{
    public class MachineInfo
    {
        public StatusUtil product = new StatusUtil(SystemDefine.infoPath + @"\Product.json");
        public List<StatusUtil> jig = new List<StatusUtil>();

        public bool read_sucs = false;
        private object write_lock;

        public MachineInfo()
        {

        }

        public MachineInfo(int jigCount = 1)
        {
            jig.Clear();

            for (int i = 0; i < jigCount; i++)
                jig.Add(new StatusUtil(SystemDefine.infoPath + $"\\Jig{i}.json"));
        }

        public void CreateJig(int jigCount)
        {
            jig.Clear();

            for (int i = 0; i < jigCount; i++)
                jig.Add(new StatusUtil(SystemDefine.infoPath + $"\\Jig{i + 1}.json"));
        }

        public int Product(StatusDefine.PRODUCT index)
        {
            string name = StatusDefine.product[(int)index];
            return product[name];
        }
        public void Product(StatusDefine.PRODUCT index, int value)
        {
            string name = StatusDefine.product[(int)index];
            product[name] = value;
            product.Write();
        }
        public void ProductInc(StatusDefine.PRODUCT index, int value)
        {
            string name = StatusDefine.product[(int)index];
            product[name] += value;
            product.Write();
        }

        public int Jig(int jig_no, StatusDefine.PRODUCT index)
        {
            string name = StatusDefine.product[(int)index];
            return jig[jig_no][name];
        }
        public void Jig(int jig_no, StatusDefine.PRODUCT index, int value)
        {
            string name = StatusDefine.product[(int)index];
            jig[jig_no][name] = value;
            jig[jig_no].Write();
        }
        public void JigInc(int jig_no, StatusDefine.PRODUCT index, int value)
        {
            if (jig_no < 0)
                return;

            string name = StatusDefine.product[(int)index];
            jig[jig_no][name] += value;
            jig[jig_no].Write();
        }

        public void JigClear()
        {
            for (int i = 0; i < jig.Count; i++)
            {
                for (int j = 0; j < jig[i].Count; j++)
                    jig[i][j] = 0;
                jig[i].Write();
            }
        }

        public bool Read()
        {
            string filepath;

            try
            {
                read_sucs = false;
                product.Read();
                if (!StatusDefine.ProductDataCheck(product.dic, product.filepath))
                    return false;
                for (int i = 0; i < jig.Count; i++)
                {
                    jig[i].Read();
                    StatusDefine.ProductDataCheck(jig[i].dic, jig[i].filepath);
                }

                read_sucs = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Write()
        {
            string filepath;

            try
            {
                lock (write_lock)
                {
                    product.Write();
                }
            }
            catch
            {

            }
        }
    }
}
