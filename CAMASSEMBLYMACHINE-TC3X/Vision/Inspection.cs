using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE
{
    public class INSPECTINFO
    {
        public string Name { get; set; } = "Unknown Inspection";
        public int targetCam { get; set; } = 0;
        public bool ignoreLightOff { get; set; } = false;
    }

    public class Inspection
    {
        public List<INSPECTINFO> InspInfo = new List<INSPECTINFO>();
        public Dictionary<string, int> recipeIndex = new Dictionary<string, int>();
        public List<InspectionRcp> recipeData = new List<InspectionRcp>();
        public RECIPEINFO recipeInfo = new RECIPEINFO();
        public int procCount = 0;
        public string CurrentModel;

        public Inspection()
        {

        }

        public bool Read(string filepath)
        {
            bool ret = true;

            // INSPECTION INFO
            if (File.Exists(filepath + @"\InspectionInfo.json"))
            {
                string streamJson = File.ReadAllText(filepath + @"\InspectionInfo.json");
                streamJson = Regex.Unescape(streamJson);
                InspInfo = JsonSerializer.Deserialize<List<INSPECTINFO>>(streamJson);
            }
            else
                ret = false;

            return ret;
        }

        public void Write(string filepath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            // INSPECTION INFO
            string streamInsp = JsonSerializer.Serialize(InspInfo, options);
            streamInsp = Regex.Unescape(streamInsp);
            if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamInsp)) return;
            File.WriteAllText(filepath + @"\InspectionInfo.json", streamInsp);
        }

        public void WriteRecipeInfo()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                // INSPECTION INFO
                string streamRecipe = JsonSerializer.Serialize(recipeInfo, options);
                streamRecipe = Regex.Unescape(streamRecipe);
                string filepath = SystemDefine.recipePath + @"\" + SystemDefine.modelname + @"\recipeinfo.json";
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamRecipe)) return;
                File.WriteAllText(filepath, streamRecipe);
            }
            catch (Exception ex) { }
        }

        public void InitProcess()
        {
            Read(SystemDefine.systemPath);

            procCount = InspInfo.Count;
        }

        public int GetIndex(string name)
        {
            return recipeIndex.ContainsKey(name) ? recipeIndex[name] : -1;
        }

        public int GetCamera(string name)
        {
            int index = GetIndex(name);
            return index < 0 ? -1 : InspInfo[index].targetCam;
        }

        public string GetModelDesc(string modelname)
        {
            try
            {
                string filepath = SystemDefine.recipePath + @"\" + modelname + @"\recipeinfo.json";

                if (File.Exists(filepath))
                {
                    var desc = new RECIPEINFO();
                    string streamJson = File.ReadAllText(filepath);
                    streamJson = Regex.Unescape(streamJson);
                    desc = JsonSerializer.Deserialize<RECIPEINFO>(streamJson);

                    return desc.desc;
                }
            }
            catch
            {
            }

            return "Unknown Model";
        }

        public void CreateModel(string modelname)
        {
            try
            {
                string filepath = SystemDefine.recipePath + @"\" + modelname;

                if (Directory.Exists(filepath))
                {
                    MessageBox.Show(null, "This model already exists." + Environment.NewLine,
                        SystemDefine.pgmVersion, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CREATE DIRECTORY
                Directory.CreateDirectory(filepath);

                // RECIPE INFO
                var desc = new RECIPEINFO();
                var options = new JsonSerializerOptions { WriteIndented = true };
                string streamJson = JsonSerializer.Serialize(desc, options);
                streamJson = Regex.Unescape(streamJson);
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson)) return;
                File.WriteAllText(filepath + @"\recipeinfo.json", streamJson);

                // RECIPE DATA ...
                for (int i = 0; i < InspInfo.Count; i++)
                {
                    string insppath = filepath + @"\" + InspInfo[i].Name;
                    Directory.CreateDirectory(insppath);

                    var newInsp = new InspectionRcp(InspInfo[i].targetCam);
                    newInsp.Create(insppath);
                }
            }
            catch
            {
            }
        }

        public void RemoveModel(string modelname)
        {
            try
            {
                string filepath = SystemDefine.recipePath + @"\" + modelname;

                if (!Directory.Exists(filepath))
                {
                    MessageBox.Show(null, "This model not exists." + Environment.NewLine,
                        SystemDefine.pgmVersion, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // REMOVE DIRECTORY
                Directory.Delete(filepath, true);
                recipeIndex.Remove(modelname);
            }
            catch
            {
            }
        }

        public bool ChangeModel(string modelname)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE && SystemDefine.NO_VISION_KEY) return true;
            try
            {
                // RECIPE INFO
                string filepath = SystemDefine.recipePath + @"\" + modelname;

                if (!Directory.Exists(filepath))
                    return false;

                if (File.Exists(filepath + @"\recipeinfo.json"))
                {
                    string streamJson = File.ReadAllText(filepath + @"\recipeinfo.json");
                    streamJson = Regex.Unescape(streamJson);
                    recipeInfo = JsonSerializer.Deserialize<RECIPEINFO>(streamJson);
                }

                recipeData.Clear();
                recipeIndex.Clear();

                if (Directory.Exists(filepath))
                {
                    for (int i = 0; i < InspInfo.Count; i++)
                    {
                        InspectionRcp newRecipe = new InspectionRcp(InspInfo[i].targetCam);

                        string insppath = filepath + @"\" + InspInfo[i].Name;
                        newRecipe.inspectName = InspInfo[i].Name;
                        newRecipe.Read(insppath);
                        recipeData.Add(newRecipe);
                        recipeIndex.Add(InspInfo[i].Name, i);

                        if (InspInfo[i].targetCam == (int)SystemDefine.CAMERA.TRAY)
                        {
                            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
                            newRecipe.InspectionResult += new InspectionRcp.cbInspectionResultEvent(proc1.cbInspectionResultEvent);
                        }
                        if (InspInfo[i].targetCam == (int)SystemDefine.CAMERA.UNDER)
                        {
                            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
                            newRecipe.InspectionResult += new InspectionRcp.cbInspectionResultEvent(proc.cbInspectionResultEvent);
                        }
                        if (InspInfo[i].targetCam == (int)SystemDefine.CAMERA.JIG)
                        {
                            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;
                            newRecipe.InspectionResult += new InspectionRcp.cbInspectionResultEvent(proc.cbInspectionResultEvent);
                        }
                        if (InspInfo[i].targetCam == (int)SystemDefine.CAMERA.PICKER)
                        {
                            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
                            newRecipe.InspectionResult += new InspectionRcp.cbInspectionResultEvent(proc.cbInspectionResultEvent);
                        }
                        if (InspInfo[i].targetCam == (int)SystemDefine.CAMERA.JIG2)
                        {
                            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessProdLoader;
                            newRecipe.InspectionResult += new InspectionRcp.cbInspectionResultEvent(proc.cbInspectionResultEvent);
                        }
                    }

                    CurrentModel = modelname;
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool UpdateModel()
        {
            try
            {
                // RECIPE INFO
                string filepath = SystemDefine.recipePath + @"\" + CurrentModel;

                if (!Directory.Exists(filepath))
                    return false;

                // RECIPE INFO
                var options = new JsonSerializerOptions { WriteIndented = true };
                string streamJson = JsonSerializer.Serialize(recipeInfo, options);
                streamJson = Regex.Unescape(streamJson);
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson)) return false;
                File.WriteAllText(filepath + @"\recipeinfo.json", streamJson);

                // RECIPE DATA
                if (recipeData == null)
                    return false;

                if (Directory.Exists(filepath))
                {
                    for (int i = 0; i < InspInfo.Count; i++)
                    {
                        if (recipeData[i] == null)
                            return false;

                        string insppath = filepath + @"\" + InspInfo[i].Name;
                        recipeData[i].Write(insppath);
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
