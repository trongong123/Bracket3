using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopEng
{
    namespace Type
    {
        public class Point2d
        {
            public double x = 0;
            public double y = 0;

            public Point2d()
            {
                x = 0;
                y = 0;
            }
            public Point2d(double _x, double _y)
            {
                x = _x;
                y = _y;
            }

            public Point2d(Point2d pt)
            {
                x = pt.x;
                y = pt.y;
            }
        }

        public class Size2d
        {
            public double width = 0;
            public double height = 0;

            public Size2d()
            {
                width = 0;
                height = 0;
            }

            public Size2d(double w, double h)
            {
                width = w;
                height = h;
            }
        }

        public class Rect4d
        {
            public double left = 0;
            public double top = 0;
            public double right = 0;
            public double bottom = 0;

            public Rect4d()
            {
                left = 0;
                top = 0;
                right = 0;
                bottom = 0;
            }

            public Rect4d(double l, double t, double r, double b)
            {
                left = l;
                top = t;
                right = r;
                bottom = b;
            }

            public double Width()
            {
                return right - left;
            }

            public double Height()
            {
                return bottom - top;
            }
        }

        public class Mat22
        {
            public double[,] mat = new double[2, 2];

            public Mat22(double[,] m)
            {
                mat = m;
            }

            public bool Inv()
            {
                double det = mat[0, 0] * mat[1, 1] - mat[0, 1] * mat[1, 0];
                if (det == 0)
                    return false;

                double[,] inv = new double[2, 2];

                inv[0, 0] = mat[1, 1] / det;
                inv[0, 1] = -mat[0, 1] / det;
                inv[1, 0] = -mat[1, 0] / det;
                inv[1, 1] = mat[0, 0] / det;

                mat = inv;
                return true;
            }
        }

        public class Mat33
        {
            public double[,] mat = new double[3, 3];

            public Mat33()
            {

            }
            public Mat33(double[,] m)
            {
                mat = m;
            }

            public static Mat33 operator +(Mat33 rhs1, Mat33 rhs2)
            {
                double[,] v = new double[3, 3];

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        v[row, col] = rhs1.mat[row, col] + rhs2.mat[row, col];
                return new Mat33(v);
            }
            public static Mat33 operator -(Mat33 rhs1, Mat33 rhs2)
            {
                double[,] v = new double[3, 3];

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        v[row, col] = rhs1.mat[row, col] - rhs2.mat[row, col];
                return new Mat33(v);
            }
            public static Mat33 operator *(Mat33 rhs1, Mat33 rhs2)
            {
                double[,] v = new double[3, 3];

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        v[row, col] = rhs1.mat[row, 0] * rhs2.mat[0, col] + rhs1.mat[row, 1] * rhs2.mat[1, col] + rhs1.mat[row, 2] * rhs2.mat[2, col];
                return new Mat33(v);
            }
            public static Mat33 operator *(Mat33 rhs1, double[,] m)
            {
                double[,] v = new double[3, 3];
                Mat33 rhs2 = new Mat33(m);

                for (int row = 0; row < 3; row++)
                    for (int col = 0; col < 3; col++)
                        v[row, col] = rhs1.mat[row, 0] * rhs2.mat[0, col] + rhs1.mat[row, 1] * rhs2.mat[1, col] + rhs1.mat[row, 2] * rhs2.mat[2, col];
                return new Mat33(v);
            }
        }
    }
}
