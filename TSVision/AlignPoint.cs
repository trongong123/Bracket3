using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopEng.Type;

namespace TopEng.Vision
{
    public class CameraDataCalc
    {
        // SETTING
        public Mat33 CamOrig = new Mat33();
        public Mat33 CamFlip = new Mat33();
        public Mat33 CamScale = new Mat33();
        public Mat33 TarShift = new Mat33();

        // DATA
        public Mat33 Source = new Mat33();
        public Mat33 Target = new Mat33();
        public Mat33 combinedSource = new Mat33();
        public Mat33 combinedTarget = new Mat33();

        public void SetCamOrigin(double width, double height)
        {
            // 카메라 센터 위치를 위한 변환 행렬
            CamOrig.mat[0, 0] = 1.0; CamOrig.mat[0, 1] = 0.0; CamOrig.mat[0, 2] = 0.0;
            CamOrig.mat[1, 0] = 0.0; CamOrig.mat[1, 1] = 1.0; CamOrig.mat[1, 2] = 0.0;
            CamOrig.mat[2, 0] = -width * 0.5;
            CamOrig.mat[2, 1] = -height * 0.5;
            CamOrig.mat[2, 2] = 1.0;
        }
        public void SetFlip(bool xflip, bool yflip)
        {
            // 카메라 방향을 위한 미러 행렬
            double fx = xflip ? -1.0 : 1.0;
            double fy = yflip ? -1.0 : 1.0;
            CamFlip.mat[0, 0] = fx; CamFlip.mat[0, 1] = 0.0; CamFlip.mat[0, 2] = 0.0;
            CamFlip.mat[1, 0] = 0.0; CamFlip.mat[1, 1] = fy; CamFlip.mat[1, 2] = 0.0;
            CamFlip.mat[2, 0] = 0.0; CamFlip.mat[2, 1] = 0.0; CamFlip.mat[2, 2] = 1.0;
        }
        public void SetScale(double xsize, double ysize)
        {
            // 카메라 해상도 설정
            CamScale.mat[0, 0] = xsize; CamScale.mat[0, 1] = 0.0; CamScale.mat[0, 2] = 0.0;
            CamScale.mat[1, 0] = 0.0; CamScale.mat[1, 1] = ysize; CamScale.mat[1, 2] = 0.0;
            CamScale.mat[2, 0] = 0.0; CamScale.mat[2, 1] = 0.0; CamScale.mat[2, 2] = 1.0;
        }
        public void SetTragetShift(double dx, double dy)
        {
            // 카메라 해상도 설정
            TarShift.mat[0, 0] = 1.0; TarShift.mat[0, 1] = 0.0; TarShift.mat[0, 2] = 0.0;
            TarShift.mat[1, 0] = 0.0; TarShift.mat[1, 1] = 1.0; TarShift.mat[1, 2] = 0.0;
            TarShift.mat[2, 0] = dx;
            TarShift.mat[2, 1] = dy;
            TarShift.mat[2, 2] = 1.0;
        }
        public void Combine()
        {
            combinedSource = Source * CamOrig * CamFlip * CamScale;
            combinedTarget = Target * CamOrig * CamFlip * CamScale * TarShift;
        }
        public void Mul(Mat33 m)
        {
            combinedSource = combinedSource * m;
            combinedTarget = combinedTarget * m;
        }
        public void Mul(double[,] m)
        {
            Mat33 m33 = new Mat33(m);
            combinedSource = combinedSource * m33;
            combinedTarget = combinedTarget * m33;
        }
    }

    public class AlignPoint
    {
        public enum OPTION
        {
            NORMAL,
            ACTUAL,
            ORIGIN,
            FIXED
        }

        public CameraDataCalc[] CamCalc;

        public double[] SourceX = new double[2] { 0.0, 0.0 };
        public double[] SourceY = new double[2] { 0.0, 0.0 };
        public double[] TargetX = new double[2] { 0.0, 0.0 };
        public double[] TargetY = new double[2] { 0.0, 0.0 };

        public double mStageCenterX;
        public double mStageCenterY;


        public double[] mCameraXPixel = new double[2] { 1, 1 };
        public double[] mCameraYPixel = new double[2] { 1, 1 };
        public double[] ActualPosX = new double[2] { 0.0, 0.0 };
        public double[] ActualPosY = new double[2] { 0.0, 0.0 };

        public AlignPoint()
        {
            CamCalc = new CameraDataCalc[2];

            for (int nCam = 0; nCam < 2; nCam++)
            {
                CamCalc[nCam] = new CameraDataCalc();
                Clear(nCam);
            }
        }

        public void Clear(int CamNo)
        {
            SourceX[CamNo] = 0.5 * mCameraXPixel[CamNo];
            SourceY[CamNo] = 0.5 * mCameraYPixel[CamNo];
            TargetX[CamNo] = 0.5 * mCameraXPixel[CamNo];
            TargetY[CamNo] = 0.5 * mCameraYPixel[CamNo];

            ActualPosX[CamNo] = 0.0;
            ActualPosY[CamNo] = 0.0;
        }

        public bool SourcePerpendicular(ref double gradient, ref double intercept)
        {
            // 두 점의 중심을 구한다.
            ConvolutionData();

            double cenx = 0.5 * (CamCalc[1].combinedSource.mat[0, 0] + CamCalc[0].combinedSource.mat[0, 0]);
            double ceny = 0.5 * (CamCalc[1].combinedSource.mat[0, 1] + CamCalc[0].combinedSource.mat[0, 1]);

            // 두 점의 중점을 지나는 직각된 선의 기울기
            double sourceDX = CamCalc[1].combinedSource.mat[0, 0] - CamCalc[0].combinedSource.mat[0, 0];
            double sourceDY = CamCalc[1].combinedSource.mat[0, 1] - CamCalc[0].combinedSource.mat[0, 1];

            // DY가 0이라는 것은 X축과 평행한 직선이라는 뜻이다.
            // 직각선의 기울기는 DX/DY가 된다.
            if (sourceDY == 0)
                return false;
            gradient = - sourceDX / sourceDY;

            // 절편을 구한다.
            intercept = ceny - cenx * gradient;
            return true;
        }

        public bool UpdateTableCenter(double grad1, double intcpt1, double grad2, double intcpt2)
        {
            mStageCenterX = (intcpt1 - intcpt2) / (grad2 - grad1);
            mStageCenterY = grad1 * mStageCenterX + intcpt1;

            return true;
        }

        private void ConvolutionData(OPTION option = OPTION.NORMAL)
        {
            for (int nCam = 0; nCam < 2; nCam++)
            {
                CamCalc[nCam].SetCamOrigin(0, 0);
                CamCalc[nCam].SetFlip(false, false);
                CamCalc[nCam].SetScale(1, 1);
                CamCalc[nCam].SetTragetShift(0, 0);

                // SOURCE DATA
                CamCalc[nCam].Source.mat[0, 0] = SourceX[nCam];
                CamCalc[nCam].Source.mat[0, 1] = SourceY[nCam];
                CamCalc[nCam].Source.mat[0, 2] = 1.0;

                // TARGET DATA
                CamCalc[nCam].Target.mat[0, 0] = TargetX[nCam];
                CamCalc[nCam].Target.mat[0, 1] = TargetY[nCam];
                CamCalc[nCam].Target.mat[0, 2] = 1.0;

                CamCalc[nCam].Combine();
            }
        }

        public void Align(ref double dx, ref double dy, ref double dr, OPTION option = OPTION.NORMAL)
        {
            ConvolutionData(option);

            // TARGET 데이터
            double targetDX = CamCalc[1].combinedTarget.mat[0, 0] - CamCalc[0].combinedTarget.mat[0, 0];
            double targetDY = CamCalc[1].combinedTarget.mat[0, 1] - CamCalc[0].combinedTarget.mat[0, 1];
            double targetCenX = 0.5 * (CamCalc[1].combinedTarget.mat[0, 0] + CamCalc[0].combinedTarget.mat[0, 0]);
            double targetCenY = 0.5 * (CamCalc[1].combinedTarget.mat[0, 1] + CamCalc[0].combinedTarget.mat[0, 1]);

            // ALIGNMENT 데이터
            double sourceDX = CamCalc[1].combinedSource.mat[0, 0] - CamCalc[0].combinedSource.mat[0, 0];
            double sourceDY = CamCalc[1].combinedSource.mat[0, 1] - CamCalc[0].combinedSource.mat[0, 1];
            double sourceCenX = 0.5 * (CamCalc[1].combinedSource.mat[0, 0] + CamCalc[0].combinedSource.mat[0, 0]);
            double sourceCenY = 0.5 * (CamCalc[1].combinedSource.mat[0, 1] + CamCalc[0].combinedSource.mat[0, 1]);

            // 각도 계산
            double offsetRad = Math.PI / 180.0;
            double targetRad = Math.Atan2(targetDY, targetDX) + offsetRad;
            double sourceRad = Math.Atan2(sourceDY, sourceDX);

            double FinalRad = targetRad - sourceRad;
            double FinalDeg = 180 * FinalRad / Math.PI;

            double[,] Rotation = new double[3, 3];
            double CosVal = Math.Cos(FinalRad);
            double SinVal = Math.Sin(FinalRad);

            Rotation[0, 0] = CosVal; Rotation[0, 1] = -SinVal; Rotation[0, 2] = 0.0;
            Rotation[1, 0] = SinVal; Rotation[1, 1] = CosVal; Rotation[1, 2] = 0.0;
            Rotation[2, 0] = 0.0; Rotation[2, 1] = 0.0; Rotation[2, 2] = 0.0;

            // 회전
            double[,] Transfom = new double[3, 3];
            Transfom[0, 0] = sourceCenX; Transfom[0, 1] = sourceCenY; Transfom[0, 2] = 0.0;
            Transfom[1, 0] = targetCenX; Transfom[1, 1] = targetCenY; Transfom[1, 2] = 0.0;
            Transfom[2, 0] = 0.0; Transfom[2, 1] = 0.0; Transfom[2, 2] = 0.0;
            Mat33 final = new Mat33(Transfom);

            final = final * Rotation;

            double FinalX = final.mat[0, 0] - final.mat[1, 0];
            double FinalY = final.mat[0, 1] - final.mat[1, 1];

            dx = 0.001 * FinalX;
            dy = 0.001 * FinalY;
            dr = FinalDeg;


            double dyy = CamCalc[0].combinedTarget.mat[0, 1] - CamCalc[0].combinedSource.mat[0, 1];
        }
    }
}
