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
    public class Recipes
    {
        public ParamUtil position = new ParamUtil();
        public ParamUtil calibration = new ParamUtil();
        public ParamUtil option = new ParamUtil();
        public ParamUtil tray_position = new ParamUtil();

        public bool read_sucs = false;

        public Recipes()
        {
        }

        #region PARAM ACCESS
        public double Position(RecipeDefine.POSITION index)
        {
            string tag = RecipeDefine.position[(int)index];
            return position[tag];
        }
        public void Position(RecipeDefine.POSITION index, double value)
        {
            string tag = RecipeDefine.position[(int)index];
            position[tag] = value;
        }
        public string PositionNAME(RecipeDefine.POSITION index)
        {
            string tag = RecipeDefine.position[(int)index];
            return position.Name(tag);
        }

        public double PositionMIN(RecipeDefine.POSITION index)
        {
            string tag = RecipeDefine.position[(int)index];
            return position.Min(tag);
        }
        public double PositionMAX(RecipeDefine.POSITION index)
        {
            string tag = RecipeDefine.position[(int)index];
            return position.Max(tag);
        }
        public string PositionUNIT(RecipeDefine.POSITION index)
        {
            string tag = RecipeDefine.position[(int)index];
            return position.Unit(tag);
        }

        public double Calibration(RecipeDefine.CALIBRATION index)
        {
            string tag = RecipeDefine.calibration[(int)index];
            return calibration[tag];
        }
        public void Calibration(RecipeDefine.CALIBRATION index, double value)
        {
            string tag = RecipeDefine.calibration[(int)index];
            calibration[tag] = value;
        }
        public string CalibrationNAME(RecipeDefine.CALIBRATION index)
        {
            string tag = RecipeDefine.calibration[(int)index];
            return calibration.Name(tag);
        }

        public double CalibrationMIN(RecipeDefine.CALIBRATION index)
        {
            string tag = RecipeDefine.calibration[(int)index];
            return calibration.Min(tag);
        }
        public double CalibrationMAX(RecipeDefine.CALIBRATION index)
        {
            string tag = RecipeDefine.calibration[(int)index];
            return calibration.Max(tag);
        }
        public string CalibrationUNIT(RecipeDefine.CALIBRATION index)
        {
            string tag = RecipeDefine.calibration[(int)index];
            return calibration.Unit(tag);
        }
        public double Option(RecipeDefine.OPTION index)
        {
            string tag = RecipeDefine.option[(int)index];
            return option[tag];
        }
        #endregion

        public bool Read(string modelname)
        {
            try
            {
                read_sucs = false;

                position.filepath = SystemDefine.recipePath + @"\" + modelname + @"\recipePos.json";
                position.fileName = position.filepath.Split('\\').Last();
                position.modelName = modelname;
                position.Read();
                position.SortByEnum<RecipeDefine.POSITION>();
                if (!RecipeDefine.PositionDataCheck(position.dic, position.filepath))
                {
                    position.DicToList();
                    position.SortByEnum<RecipeDefine.POSITION>();
                    position.Write();
                }
                calibration.filepath = SystemDefine.recipePath + @"\" + modelname + @"\recipeCalib.json";
                calibration.fileName = calibration.filepath.Split('\\').Last();
                calibration.modelName = modelname;
                calibration.Read();
                calibration.SortByEnum<RecipeDefine.CALIBRATION>();
                if (!RecipeDefine.CalibrationDataCheck(calibration.dic, calibration.filepath))
                {
                    calibration.DicToList();
                    calibration.SortByEnum<RecipeDefine.CALIBRATION>();
                    calibration.Write();
                }
                option.filepath = SystemDefine.recipePath + @"\" + modelname + @"\recipeOp.json";
                option.fileName = option.filepath.Split('\\').Last();
                option.modelName = modelname;
                option.Read();
                option.SortByEnum<RecipeDefine.OPTION>();
                if (!RecipeDefine.OptionDataCheck(option.dic, option.filepath))
                {
                    option.DicToList();
                    option.SortByEnum<RecipeDefine.OPTION>();
                    option.Write();
                }
                //tray_position.filepath = SystemDefine.recipePath + @"\" + modelname + @"\recipeTray.json";
                //tray_position.Read();

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
            try
            {
                position.Write();
                calibration.Write();
                option.Write();
                tray_position.Write();
            }
            catch
            {

            }
        }

        public void ClearTrayPosition()
        {
            tray_position.Clear();
        }
    }
}
