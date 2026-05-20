using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using System.Text.Json;
using System.Text.RegularExpressions;

namespace TopEng.Utils
{
    public class RegistryUtil
    {
        public Dictionary<string, REGISTRYSTRUCT> dic = new Dictionary<string, REGISTRYSTRUCT>();
        private List<REGISTRYSTRUCT> paramlist = new List<REGISTRYSTRUCT>();

        public string filepath;

        public RegistryUtil()
        {

        }

        public RegistryUtil(string filepath)
        {
            this.filepath = filepath;
        }

        public object this[string sKey]
        {
            get
            {
                return this.dic[sKey].Value;
            }
            set
            {
                this.dic[sKey].Value = value;
            }
        }

        public int Count
        {
            get { return paramlist.Count; }
        }

        public void Clear()
        {
            paramlist.Clear();
        }

        public void Add(REGISTRYSTRUCT param)
        {
            paramlist.Add(param);
        }

        public REGISTRYSTRUCT getParam(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index];
            return null;
        }

        public REGISTRYSTRUCT getParam(string tag)
        {
            if (dic.ContainsKey(tag))
                return dic[tag];
            return null;
        }

        public string Name(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].Name;
            return "";
        }
        public string Name(string tag)
        {
            if (!dic.ContainsKey(tag))
                return "";
            return dic[tag].Name;
        }
        public void Name(int index, string value)
        {
            if (index < paramlist.Count)
                paramlist[index].Name = value;
        }
        public void Name(string tag, string value)
        {
            if (!dic.ContainsKey(tag))
                return;
            dic[tag].Name = value;
        }
        public string Unit(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].Unit;
            return "";
        }
        public string Unit(string tag)
        {
            if (!dic.ContainsKey(tag))
                return "";
            return dic[tag].Unit;
        }
        public void Unit(int index, string value)
        {
            if (index < paramlist.Count)
                paramlist[index].Unit = value;
        }
        public void Unit(string tag, string value)
        {
            if (!dic.ContainsKey(tag))
                return;
            dic[tag].Unit = value;
        }

        public void UpdateDictionary()
        {
            dic.Clear();
            foreach (var param in paramlist)
                dic.Add(param.Name, param);
        }

        public List<REGISTRYSTRUCT> GetParamList()
        {
            return paramlist;
        }
    }

    public class REGISTRYSTRUCT
    {
        public string Name { get; set; } = "unknown";
        public object Value { get; set; } = 0.0;
        public string Unit { get; set; } = "";
        public string Desc { get; set; } = "Unknown Register Description";
    }
}
