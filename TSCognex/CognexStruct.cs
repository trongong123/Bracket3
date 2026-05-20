using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.ImageFile;
using TopEng.Type;
using System.IO;

namespace TopEng.Vision
{
    public class COGROI
    {
        public bool use = false;
        public double LocX = 20;
        public double LocY = 20;
        public double SizeX = 100;
        public double SizeY = 100;
    }

    public class COGTRAIN
    {
        public ICogImage pattern = null;
        public Point2d re_pos;
    }

    public class PARAMBLOB
    {
        public class SEGMENT
        {
            public class HARDTHRESHOLD
            {
                public class FIXED
                {
                    public int threshold = 128;
                }
                public class RELATIVE
                {
                    public int thresholdPercent = 50;
                    public int highTail = 0;
                    public int lowTail = 0;
                }

                FIXED fix = new FIXED();
                RELATIVE relative = new RELATIVE();
            }
            public class SOFTTHRESHOLD
            {
                public class FIXED
                {
                    public int min_threshold = 100;
                    public int max_threshold = 128;
                }

                public class RELATIVE
                {
                    public int min_thresholdPercent = 40;
                    public int max_thresholdPercent = 60;
                    public int lowTailPercent = 0;
                    public int highTailPercent = 0;
                }

                FIXED fix = new FIXED();
                RELATIVE relative = new RELATIVE();
                public int flexibility = 254;
            }

            public CogBlobSegmentationModeConstants mode;
            public CogBlobSegmentationPolarityConstants polarity;
            HARDTHRESHOLD hard = new HARDTHRESHOLD();
            SOFTTHRESHOLD soft = new SOFTTHRESHOLD();
        }


        public class FILTER
        {
            public bool use = false;
            public int highRange = 30000;
            public int lowRange = 30000;
        }

        public SEGMENT segment = new SEGMENT();
        public COGROI roi = new COGROI();
        public FILTER filter = new FILTER();
    }

    public class PARAMPMALIGN
    {
        public List<COGTRAIN> trains = new List<COGTRAIN>();
        public COGROI roi = new COGROI();

        public void SetPatternImage(int id, string filepath)
        {
            if (!File.Exists(filepath))
                return;

            COGTRAIN train = null;

            if (id >= trains.Count)
                train = new COGTRAIN();
            train = trains[id];

            var file = new CogImageFile();
            file.Open(filepath, CogImageFileModeConstants.Read);
            ICogImage pattern = (ICogImage)file[0];
            file.Close();
            train.pattern = CogImageConvert.GetIntensityImage(pattern, 0, 0, pattern.Width, pattern.Height);
        }
    }
}
