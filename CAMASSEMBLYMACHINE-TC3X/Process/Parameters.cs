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
    public class WORKTIME
    {
        public string Name { get; set; } = "unknown";
        public string Tag { get; set; } = "n";
        public int Hour { get; set; } = 0;
        public int Minute { get; set; } = 0;
        public int Second { get; set; } = 0;
        public string Desc { get; set; } = "Unknown Time Description";
    }

    public class Parameters
    {
        public Dictionary<string, WORKTIME> workingtime = new Dictionary<string, WORKTIME>();

        public ParamUtil position = new ParamUtil();
        public ParamUtil calibration = new ParamUtil();
        public ParamUtil velocity = new ParamUtil();
        public ParamUtil time = new ParamUtil();
        public ParamUtil interference = new ParamUtil();
        public ParamUtil option = new ParamUtil();

        public bool read_sucs = false;

        public List<WORKTIME> workingtimelist = new List<WORKTIME>();

        public Parameters()
        {
        }

        public double Position(ParameterDefine.POSITION index)
        {
            string tag = ParameterDefine.position[(int)index];
            return position[tag];
        }
        public void Position(ParameterDefine.POSITION index, double value)
        {
            string tag = ParameterDefine.position[(int)index];
            position[tag] = value;
        }

        public string PositionNAME(ParameterDefine.POSITION index)
        {
            string tag = ParameterDefine.position[(int)index];
            return position.Name(tag);
        }

        public double PositionMIN(ParameterDefine.POSITION index)
        {
            string tag = ParameterDefine.position[(int)index];
            return position.Min(tag);
        }
        public double PositionMAX(ParameterDefine.POSITION index)
        {
            string tag = ParameterDefine.position[(int)index];
            return position.Max(tag);
        }
        public string PositionUNIT(ParameterDefine.POSITION index)
        {
            string tag = ParameterDefine.position[(int)index];
            return position.Unit(tag);
        }

        public double Velocity(ParameterDefine.VELOCITY index)
        {
            string tag = ParameterDefine.velocity[(int)index];
            return velocity[tag];
        }
        public void Velocity(ParameterDefine.VELOCITY index, double value)
        {
            string tag = ParameterDefine.velocity[(int)index];
            velocity[tag] = value;
        }

        public double Calibration(ParameterDefine.CALIBRATION index)
        {
            string tag = ParameterDefine.calibration[(int)index];
            return calibration[tag];
        }
        public void Calibration(ParameterDefine.CALIBRATION index, double value)
        {
            string tag = ParameterDefine.calibration[(int)index];
            calibration[tag] = value;
        }

        public double Time(ParameterDefine.TIME index)
        {
            string tag = ParameterDefine.time[(int)index];
            return time[tag];
        }
        public void Time(ParameterDefine.TIME index, double value)
        {
            string tag = ParameterDefine.time[(int)index];
            time[tag] = value;
        }

        public double Option(ParameterDefine.OPTION index)
        {
            string tag = ParameterDefine.option[(int)index];
            return option[tag];
        }
        public void Option(ParameterDefine.OPTION index, double value)
        {
            string tag = ParameterDefine.option[(int)index];
            option[tag] = value;
        }

        public double Interference(ParameterDefine.INTERFERENCE index)
        {
            string tag = ParameterDefine.interference[(int)index];
            return interference[tag];
        }
        public void Interference(ParameterDefine.INTERFERENCE index, double value)
        {
            string tag = ParameterDefine.interference[(int)index];
            interference[tag] = value;
        }

        public bool Read()
        {
            string filepath;

            try
            {
                read_sucs = false;
                position.filepath = SystemDefine.paramPath + @"\Position.json";
                position.fileName = position.filepath.Split('\\').Last();
                position.Read();
                position.SortByEnum<ParameterDefine.POSITION>();
                if (!ParameterDefine.PositionDataCheck(position.dic, position.filepath))
                {
                    position.DicToList(); 
                    position.SortByEnum<ParameterDefine.POSITION>();
                    position.Write();
                }
                calibration.filepath = SystemDefine.paramPath + @"\Calibration.json";
                calibration.fileName = calibration.filepath.Split('\\').Last();
                calibration.Read();
                calibration.SortByEnum<ParameterDefine.CALIBRATION>();
                if (!ParameterDefine.CalibrationDataCheck(calibration.dic, calibration.filepath))
                {
                    calibration.DicToList();
                    calibration.SortByEnum<ParameterDefine.CALIBRATION>();
                    calibration.Write();
                }
                time.filepath = SystemDefine.paramPath + @"\Time.json";
                time.fileName = time.filepath.Split('\\').Last();
                time.Read();
                time.SortByEnum<ParameterDefine.TIME>();
                if (!ParameterDefine.TimeDataCheck(time.dic, time.filepath))
                {
                    time.DicToList();
                    time.SortByEnum<ParameterDefine.TIME>();
                    time.Write();
                }
                velocity.filepath = SystemDefine.paramPath + @"\Velocity.json";
                velocity.fileName = velocity.filepath.Split('\\').Last();
                velocity.Read();
                velocity.SortByEnum<ParameterDefine.VELOCITY>();
                if (!ParameterDefine.VelocityDataCheck(velocity.dic, velocity.filepath))
                {
                    velocity.DicToList();
                    velocity.SortByEnum<ParameterDefine.VELOCITY>();
                    velocity.Write();
                }
                interference.filepath = SystemDefine.paramPath + @"\InterferencePos.json";
                interference.fileName = interference.filepath.Split('\\').Last();
                interference.Read();
                interference.SortByEnum<ParameterDefine.INTERFERENCE>();
                if (!ParameterDefine.InterferenceDataCheck(interference.dic, interference.filepath))
                {
                    interference.DicToList();
                    interference.SortByEnum<ParameterDefine.INTERFERENCE>();
                    interference.Write();
                }
                option.filepath = SystemDefine.paramPath + @"\Option.json";
                option.fileName = option.filepath.Split('\\').Last();
                option.Read();
                option.SortByEnum<ParameterDefine.OPTION>();
                if (!ParameterDefine.OptionDataCheck(option.dic, option.filepath))
                {
                    option.DicToList();
                    option.SortByEnum<ParameterDefine.OPTION>();
                    option.Write();
                }
                // WORKING TIME
                workingtimelist.Clear();
                workingtime.Clear();
                filepath = SystemDefine.paramPath + @"\WorkingTime.json";
                ReadWorkingTime(filepath, ref workingtimelist);
                foreach (var param in workingtimelist)
                    workingtime.Add(param.Tag, param);

                read_sucs = true;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReadWorkingTime(string filepath, ref List<WORKTIME> workingtimelist)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    string streamJson = File.ReadAllText(filepath);
                    streamJson = Regex.Unescape(streamJson);
                    workingtimelist = JsonSerializer.Deserialize<List<WORKTIME>>(streamJson);
                }
            }
            catch
            {

            }
        }

        public void Write()
        {
            string filepath;

            try
            {
                position.Write();
                calibration.Write();
                velocity.Write();
                time.Write();
                interference.Write();
                option.Write();
                // WORKING TIME
                filepath = SystemDefine.paramPath + @"\WorkingTime.json";
                WriteWorkingTime(filepath, workingtimelist);
            }
            catch
            {

            }
        }

        private void WriteWorkingTime(string filepath, List<WORKTIME> workingtimelist)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    // RECIPE INFO
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string streamJson = JsonSerializer.Serialize(workingtimelist, options);
                    streamJson = Regex.Unescape(streamJson);
                    if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson)) return;
                    File.WriteAllText(filepath, streamJson);
                }
            }
            catch
            {

            }
        }
    }
}
