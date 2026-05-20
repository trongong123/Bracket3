using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopEng.Type;

namespace TopEng.Vision
{
    public class VisionResult
    {
        public enum RESULTTYPE
        {
            UNKNOWN = -1,
            CALCPOS,
            CIRCLE,
            CLASSIFY,
            PMRESULT,
            BLOB,
            ID,
            OCR,
            OUTPUTS
        }

        public RESULTTYPE type = RESULTTYPE.UNKNOWN;
        public bool sucess = true;
        public string name = "";
        public string tags = "";
    }

    public class CalcPointReuslt : VisionResult
    {
        public Point2d pos = new Point2d();

        public CalcPointReuslt()
        {
            type = RESULTTYPE.CALCPOS;
        }
    }

    public class CircleResult : VisionResult
    {
        public Point2d center = new Point2d();
        public double radius = 0;

        public CircleResult()
        {
            type = RESULTTYPE.CIRCLE;
        }

        public CircleResult(CircleResult result)
        {
            type = RESULTTYPE.CIRCLE;

            center.x = result.center.x;
            center.y = result.center.y;
            radius = result.radius;
        }
    }

    public class ClassificationResult : VisionResult
    {
        public string classname = "";
        public double score = 0;

        public ClassificationResult()
        {
            type = RESULTTYPE.CLASSIFY;
        }

        public ClassificationResult(ClassificationResult result)
        {
            type = RESULTTYPE.CLASSIFY;

            classname = result.classname;
            score = result.score;
        }
    }

    public class PMAlignResult : VisionResult
    {
        public Point2d pos = new Point2d();
        public double angle = 0;
        public double score = 0;
        public string classname = "";

        public PMAlignResult()
        {
            type = RESULTTYPE.PMRESULT;
        }

        public PMAlignResult(PMAlignResult result)
        {
            type = RESULTTYPE.PMRESULT;

            pos.x = result.pos.x;
            pos.y = result.pos.y;
            angle = result.angle;
            score = result.score;
            tags = result.tags;
            classname = result.classname;
        }
    }

    public class BlobResult : VisionResult
    {
        public Point2d center = new Point2d();
        public double area = 0;

        public BlobResult()
        {
            type = RESULTTYPE.BLOB;
        }

        public BlobResult(BlobResult result)
        {
            type = RESULTTYPE.BLOB;

            center.x = result.center.x;
            center.y = result.center.y;
            area = result.area;
        }
    }

    public class IDResult : VisionResult
    {
        public Point2d center = new Point2d();
        public double angle;
        public string text;

        public IDResult()
        {
            type = RESULTTYPE.ID;
        }

        public IDResult(IDResult result)
        {
            type = RESULTTYPE.ID;

            center.x = result.center.x;
            center.y = result.center.y;
            angle = result.angle;
            text = result.text;
        }
    }

    public class OCRResult : VisionResult
    {
        public string text;

        public OCRResult()
        {
            type = RESULTTYPE.OCR;
        }

        public OCRResult(OCRResult result)
        {
            type = RESULTTYPE.OCR;

            text = result.text;
        }
    }

    public class OutPutsResult : VisionResult
    {
        public Point2d Lcenter = new Point2d();
        public double Langle;
        public string Ltext;
        public string Ldirection;
        public double LdirectionScore;
        public int LIndex;

        public Point2d Rcenter = new Point2d();
        public double Rangle;
        public string Rtext;
        public string Rdirection;
        public double RdirectionScore;
        public int RIndex;

        public string LClassifyClass;
        public double LClassifyScore;
        public string RClassifyClass;
        public double RClassifyScore;

        public ICogImage LClassifyImage;
        public ICogImage RClassifyImage;

        public string sOCR_ID = "";
        public int currentProductCount = 0;
        public int targetProductCount = 0;

        public OutPutsResult()
        {
            type = RESULTTYPE.OUTPUTS;
        }
        public OutPutsResult(OutPutsResult result)
        {
            type = RESULTTYPE.OUTPUTS;

            Lcenter.x = result.Lcenter.x;
            Lcenter.y = result.Lcenter.y;
            Langle = result.Langle;
            LIndex = result.LIndex;

            Rcenter.x = result.Rcenter.x;
            Rcenter.y = result.Rcenter.y;
            Rangle = result.Rangle;
            RIndex = result.RIndex;

            LClassifyClass = result.LClassifyClass;
            LClassifyScore = result.LClassifyScore;

            RClassifyClass = result.RClassifyClass;
            RClassifyScore = result.RClassifyScore;

            sOCR_ID = result.sOCR_ID;

            LClassifyImage = result.LClassifyImage;
            RClassifyImage = result.RClassifyImage;

            currentProductCount = result.currentProductCount;
            targetProductCount = result.targetProductCount;
        }
    }
}
