using MathNet.Numerics;
using OpenCvSharp;

namespace VisionSharp.Utils
{
    /// <summary>
    ///     This is math class for execute point, point2x, point3x, rect , rotatedRect
    /// </summary>
    public class CvMath
    {
        /// <summary>
        ///     获取一列点的中心点
        /// </summary>
        /// <param name="point2Fs"></param>
        /// <returns></returns>
        public static Point2d GetMeanPoint2F(IList<Point> points)
        {
            var meanX = points.Select(p => p.X).Average();
            var meanY = points.Select(p => p.Y).Average();
            return new Point2d(meanX, meanY);
        }

        /// <summary>
        ///     获取一列点的中心点
        /// </summary>
        /// <param name="point2Fs"></param>
        /// <returns></returns>
        public static Point2d GetMeanPoint2F(IList<Point2f> point2Fs)
        {
            var meanX = point2Fs.Select(p => p.X).Average();
            var meanY = point2Fs.Select(p => p.Y).Average();
            return new Point2d(meanX, meanY);
        }


        /// <summary>
        /// </summary>
        /// <param name="point2Fs"></param>
        /// <returns></returns>
        public static Point2d GetMeanPoint2F(IList<Point2d> point2Fs)
        {
            var meanX = point2Fs.Select(p => p.X).Average();
            var meanY = point2Fs.Select(p => p.Y).Average();
            return new Point2d(meanX, meanY);
        }

        /// <summary>
        ///     获取矩形框的中心点
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Point2f GetMeanPoint2F(Rect rect)
        {
            var points = new List<Point> {rect.BottomRight, rect.TopLeft};
            var meanPoint2F = GetMeanPoint2F(points);
            return new Point2f((float) meanPoint2F.X, (float) meanPoint2F.Y);
        }

        /// <summary>
        ///     获取旋转矩形框的中心点
        /// </summary>
        /// <param name="rotatedRect"></param>
        /// <returns></returns>
        public static Point2f GetMeanPoint2F(RotatedRect rotatedRect)
        {
            var rect = rotatedRect.BoundingRect();
            return GetMeanPoint2F(rect);
        }


        /// <summary>
        ///     return B,K with Tuple
        /// </summary>
        /// <param name="keyPoints"></param>
        /// <returns></returns>
        public static Tuple<double, double> Linefit(IEnumerable<Point2f> points)
        {
            var x = new List<double>();
            var y = new List<double>();
            foreach (var p in points)
            {
                x.Add(p.X);
                y.Add(p.Y);
            }

            var res = Fit.Line(x.ToArray(), y.ToArray());
            return new Tuple<double, double>(res.A, res.B);
        }

        /// <summary>
        ///     Return Point By Line
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Point GetPoint(double y, Tuple<double, double> kb)
        {
            return new Point((y - kb.Item1) / kb.Item2, y);
        }


        public static Point GetCenter(
            IEnumerable<Point2d> point2ds)
        {
            var array = point2ds.ToArray();
            var x = array.Average(a => a.X);
            var y = array.Average(a => a.Y);
            return new Point(x, y);
        }

        public static Point GetCenter(
            IEnumerable<Point> points)
        {
            var array = points.ToArray();
            var x = array.Average(a => a.X);
            var y = array.Average(a => a.Y);
            return new Point(x, y);
        }


        /// <summary>
        ///     Distance= |A*x+ B*y +C | / Sqrt(A*A+B*B)
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="point2F"></param>
        /// <returns></returns>
        public static double GetDistance(double a, double b, double c, Point2f point2F)
        {
            return Math.Abs(a * point2F.X + b * point2F.Y + c)
                   / Math.Sqrt(a * a + b * b);
        }


        public static double GetDistance(Point3d p1, Point3d p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            var z = p1.Z - p2.Z;

            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static double GetDistance(Point2d p1, Point2d p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;

            return Math.Sqrt(x * x + y * y);
        }

        public static double GetDistance(Point p1, Point2d p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;

            return Math.Sqrt(x * x + y * y);
        }

        public static double GetDistance(Point2f p1, Point2f p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;

            return Math.Sqrt(x * x + y * y);
        }


        /// <summary>
        ///     Generate Pattern Point by top left, top right, bottom left point.
        /// </summary>
        /// <param name="topleft"></param>
        /// <param name="topright"></param>
        /// <param name="bottomleft"></param>
        /// <param name="patternSize">pattern patternSize</param>
        /// <returns></returns>
        public static List<Point2d> GetPatternPoint(
            Point2d topleft,
            Point2d bottomright,
            Size patternSize)
        {
            var dis = bottomright - topleft;

            var width = dis.X / (patternSize.Width - 1);
            var height = dis.Y / (patternSize.Height - 1);

            var all = new List<Point2d>();

            for (var r = 0; r < patternSize.Height; r++)
            for (var c = 0; c < patternSize.Width; c++)
            {
                var x = topleft.X + c * width;
                var y = topleft.Y + r * height;
                all.Add(new Point2d(x, y));
            }

            return all;
        }

        /// <summary>
        ///     Generate Pattern Point by top left, top right, bottom left point.
        /// </summary>
        /// <param name="topleft"></param>
        /// <param name="topright"></param>
        /// <param name="bottomleft"></param>
        /// <param name="patternSize">pattern patternSize</param>
        /// <returns></returns>
        public static List<Point2d> GetPatternPoint(
            Point2d topleft,
            Point2d topright,
            Point2d bottomleft,
            Size patternSize)
        {
            var rdis = bottomleft - topleft;
            var rdisX = rdis.X / (patternSize.Width - 1);
            var rdisY = rdis.Y / (patternSize.Height - 1);

            var cdis = topright - topleft;
            var cdisX = cdis.X / (patternSize.Width - 1);
            var cdisY = cdis.Y / (patternSize.Height - 1);

            var all = new List<Point2d>();
            for (var r = 0; r < patternSize.Height; r++)
            for (var c = 0; c < patternSize.Width; c++)
            {
                var x = topleft.X + r * rdisX + c * cdisX;
                var y = topleft.Y + r * rdisY + c * cdisY;
                all.Add(new Point2d(x, y));
            }

            return all;
        }

        /// <summary>
        /// </summary>
        /// <param name="rotatedRect"></param>
        /// <returns></returns>
        public static double GetRatio(
            RotatedRect rotatedRect)
        {
            var width = rotatedRect.Size.Width;
            var height = rotatedRect.Size.Height;
            var ratio = width / height;
            return ratio > 1 ? ratio : 1 / ratio;
        }


        /// <summary>
        ///     根据两点，获取象限角度
        ///     象限基于图像坐标系
        ///     ----- 3 ----- | ----- 4 -----
        ///     ----- 2 ----- | ----- 1 -----
        ///     第1象限 k>=0 0~~90
        ///     第2象限 k<=0 90~180
        ///     第3象限 k>=0 180~~270
        ///     第4象限 k<=0 270~360
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double GetAngle(
            Point2f startPoint,
            Point2f endPoint)
        {
            if (startPoint == endPoint)
            {
                throw new Exception();
            }

            if (endPoint.X - startPoint.X == 0)
            {
                return endPoint.Y > startPoint.Y ? 270 : 90;
            }

            var k = (endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X);

            var angleBasic = 180 * Math.Atan(k) / Math.PI;

            var angleFinal =
                angleBasic >= 0
                    ? endPoint.X > startPoint.X
                        ? angleBasic
                        : angleBasic + 180
                    : endPoint.Y > startPoint.Y
                        ? angleBasic + 180
                        : angleBasic + 360;
            return angleFinal;
        }


        public static double Sigmoid(double a)
        {
            return 1.0 / (1 + Math.Exp(-a));
        }
    }
}