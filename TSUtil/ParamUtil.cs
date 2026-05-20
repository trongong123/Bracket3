using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using TopEng.Controls;
using System.Windows.Forms;

namespace TopEng.Utils
{
    public class ParamUtil
    {
        public Dictionary<string, PARAMSTRUCT> dic = new Dictionary<string, PARAMSTRUCT>();
        private List<PARAMSTRUCT> paramlist = new List<PARAMSTRUCT>();
        private List<PARAMSTRUCT> paramlistOld = new List<PARAMSTRUCT>();

        public string filepath;
        public string fileName;
        public string modelName; //Only File in Recipe 

        public ParamUtil()
        {

        }

        public ParamUtil(string filepath)
        {
            this.filepath = filepath;
        }

        public double this[string sKey]
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

        public void Add(PARAMSTRUCT param)
        {
            paramlist.Add(param);
        }

        public PARAMSTRUCT getParam(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index];
            return null;
        }
        public PARAMSTRUCT getParam(string tag)
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

        public double Min(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].Min;
            return -999999999999999999;
        }
        public double Min(string tag)
        {
            if (!dic.ContainsKey(tag))
                return -999999999999999999;
            return dic[tag].Min;
        }
        public void Min(int index, double value)
        {
            if (index < paramlist.Count)
                paramlist[index].Min = value;
        }
        public void Min(string tag, double value)
        {
            if (!dic.ContainsKey(tag))
                return;
            dic[tag].Min = value;
        }

        public double Max(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].Max;
            return 999999999999999999;
        }
        public double Max(string tag)
        {
            if (!dic.ContainsKey(tag))
                return 9999999999999999999;
            return dic[tag].Max;
        }
        public void Max(int index, double value)
        {
            if (index < paramlist.Count)
                paramlist[index].Max = value;
        }
        public void Max(string tag, double value)
        {
            if (!dic.ContainsKey(tag))
                return;
            dic[tag].Max = value;
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

        public int targetAxis(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].TargetAxis;
            return -1;
        }
        public int targetAxis(string tag)
        {
            if (!dic.ContainsKey(tag))
                return -1;
            return dic[tag].TargetAxis;
        }
        public void targetAxis(int index, int value)
        {
            if (index < paramlist.Count)
                paramlist[index].TargetAxis = value;
        }
        public void targetAxis(string tag, int value)
        {
            if (!dic.ContainsKey(tag))
                return;
            dic[tag].TargetAxis = value;
        }

        public double X(string tag)
        {
            if (!dic.ContainsKey(tag))
                return -99999999999999;
            return dic[tag].X;
        }

        public double X(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].X;
            return -99999999999999;
        }

        public double Y(string tag)
        {
            if (!dic.ContainsKey(tag))
                return -99999999999999;
            return dic[tag].Y;
        }

        public double Y(int index)
        {
            if (index < paramlist.Count)
                return paramlist[index].Y;
            return -99999999999999;
        }

        public void UpdateDictionary()
        {
            dic.Clear();
            foreach (var param in paramlist)
                dic.Add(param.Tag, param);
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
                    paramlist = JsonSerializer.Deserialize<List<PARAMSTRUCT>>(streamJson);

                    paramlistOld.Clear();
                    foreach (var param in paramlist)
                    {
                        var newParam = new PARAMSTRUCT();
                        newParam.Name = param.Name;
                        newParam.Tag = param.Tag;
                        newParam.Value = param.Value;
                        newParam.TargetAxis = param.TargetAxis;
                        newParam.Unit = param.Unit;
                        newParam.Min = param.Min;
                        newParam.Max = param.Max;
                        newParam.X = param.X;
                        newParam.Y = param.Y;
                        newParam.Desc = param.Desc;

                        paramlistOld.Add(newParam);
                    }

                    foreach (var param in paramlist)
                    {
                        if (dic.ContainsKey(param.Tag))
                        {
                            Dlg_MessageBox formErr =
                                  new Dlg_MessageBox(EMESSAGEBOX.MSG,
                                  string.Format($"[{param.Tag}] is Duplicated\nProgram Exit"),
                                  filepath);
                            formErr.TopLevel = true;
                            formErr.TopMost = true;
                            formErr.ShowDialog();

                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                            Application.ExitThread();
                            Environment.Exit(0);
                            return;
                        }
                        else 
                            dic.Add(param.Tag, param);
                    }
                }
                else
                    throw new Exception("File is not Exist");
            }
            catch (Exception e)
            {
                string errorMsg = "";
                if (e.ToString().Contains("Json"))
                    errorMsg = "JSON Parse ERROR";
                else if (e.ToString().Contains("Exist"))
                    errorMsg = "No File in FIlePath";
                else
                    errorMsg = e.ToString();

                Dlg_MessageBox formErr =
                       new Dlg_MessageBox(EMESSAGEBOX.MSG,
                       errorMsg + string.Format($" To File Load Failed\nProgram Exit"),
                       filepath);
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Application.ExitThread();
                Environment.Exit(0);
                return;
            }
        }

        public void DicToList()
        {
            paramlist = dic.Values.ToList();
        }

        public List<PARAMSTRUCT> SortByEnum<T>() where T : struct, Enum
        {
            return paramlist = paramlist.OrderBy(item =>
            {
                if (Enum.TryParse<T>(item.Tag, out var result))
                    return Convert.ToInt32(result);
                else
                    return int.MaxValue;
            }).ToList();
        }

        public void Write()
        {
            try
            {
                Backup();
                var options = new JsonSerializerOptions { WriteIndented = true };
                string streamJson = JsonSerializer.Serialize(paramlist, options);
                streamJson = Regex.Unescape(streamJson);
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson)) return;
                File.WriteAllText(filepath, streamJson);                
            }
            catch
            {

            }
        }
        public List<PARAMSTRUCT> GetParamList()
        {
            return paramlist;
        }

        public bool CheckParamListSame()
        {
            bool isEqual = 
                paramlist.Count == paramlistOld.Count &&
                paramlist.Zip(paramlistOld, (first, second) =>
                first.Value == second.Value && first.Name == second.Name).All(result => result);

            return isEqual;
        }

        public void Backup()
        {
            try
            {
                if (fileName == null) 
                    return;
                
                if (CheckParamListSame())
                    return;

                string fileNameOnly = fileName.Split('.')[0];
                string folderPath = @"C:\FA\" + "DataBackup" + "\\";
                if(modelName != null && modelName != "")
                    folderPath += "Recipe" + "\\" + modelName + "\\" + fileNameOnly + "\\";
                else
                    folderPath += "Param" + "\\" + fileNameOnly + "\\";

                string filePathBackup = folderPath + fileNameOnly + "_" + 
                    DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss_fff") + ".json";

                if (Directory.Exists(folderPath) == false)
                {
                    Directory.CreateDirectory(folderPath);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string streamJson = JsonSerializer.Serialize(paramlistOld, options);
                streamJson = Regex.Unescape(streamJson);
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filePathBackup, streamJson)) return;
                File.WriteAllText(filePathBackup, streamJson);

                paramlistOld.Clear();
                foreach (var param in paramlist)
                {
                    var newParam = new PARAMSTRUCT();
                    newParam.Name = param.Name;
                    newParam.Tag = param.Tag;
                    newParam.Value = param.Value;
                    newParam.TargetAxis = param.TargetAxis;
                    newParam.Unit = param.Unit;
                    newParam.Min = param.Min;
                    newParam.Max = param.Max;
                    newParam.X = param.X;
                    newParam.Y = param.Y;
                    newParam.Desc = param.Desc;

                    paramlistOld.Add(newParam);
                }
            }
            catch (Exception e)
            {
                Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format(e + "\nBackup Failed"));
            }
        }
    }

    public class PARAMSTRUCT
    {
        public string Name { get; set; } = "unknown";
        public string Tag { get; set; } = "n";
        public double Value { get; set; } = 0.0;
        public int TargetAxis { get; set; } = -1;
        public string Unit { get; set; } = "?";
        public double Min { get; set; } = 0.0;
        public double Max { get; set; } = 0.0;
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;
        public string Desc { get; set; } = "Unknown Param Description";
    }
}
