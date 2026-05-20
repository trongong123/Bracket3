using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TopEng.Utils
{
    public class StatusUtil
    {
        public Dictionary<string, STATUSSTRUCT> dic = new Dictionary<string, STATUSSTRUCT>();
        private List<STATUSSTRUCT> statuslist = new List<STATUSSTRUCT>();

        public string filepath;

        public StatusUtil()
        {

        }

        public StatusUtil(string filepath)
        {
            this.filepath = filepath;
        }

        public int this[string sKey]
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

        public int this[int sKey]
        {
            get
            {
                return this.statuslist[sKey].Value;
            }
            set
            {
                this.statuslist[sKey].Value = value;
            }
        }

        public int Count
        {
            get { return statuslist.Count; }
        }

        public void Add(STATUSSTRUCT status)
        {
            this.statuslist.Add(status);
            this.dic.Add(status.Name, status);
        }

        public STATUSSTRUCT getParam(int index)
        {
            if (index < statuslist.Count)
                return statuslist[index];
            return null;
        }
        public STATUSSTRUCT getParam(string tag)
        {
            if (dic.ContainsKey(tag))
                return dic[tag];
            return null;
        }

        public string Name(int index)
        {
            if (index < statuslist.Count)
                return statuslist[index].Name;
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
            if (index < statuslist.Count)
                statuslist[index].Name = value;
        }
        public void Name(string tag, string value)
        {
            if (!dic.ContainsKey(tag))
                return;
            dic[tag].Name = value;
        }

        public void Read()
        {
            try
            {
                dic.Clear();

                if (File.Exists(filepath))
                {
                    string streamJson = File.ReadAllText(filepath);
                    streamJson = Regex.Unescape(streamJson);
                    statuslist = JsonSerializer.Deserialize<List<STATUSSTRUCT>>(streamJson);

                    foreach (var param in statuslist)
                        dic.Add(param.Name, param);
                }
            }
            catch
            {

            }
        }

        public void Write()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string streamJson = JsonSerializer.Serialize(statuslist, options);
                streamJson = Regex.Unescape(streamJson);
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson)) return;
                File.WriteAllText(filepath, streamJson);
            }
            catch
            {

            }
        }
        public List<STATUSSTRUCT> GetParamList()
        {
            return statuslist;
        }
    }

    public class STATUSSTRUCT
    {
        public string Name { get; set; } = "unknown";
        public int Value { get; set; } = 0;
    }
}
