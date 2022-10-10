using System.Text;
using OpenCvSharp;

namespace VisionSharp.Utils
{
    /// <summary>
    ///     Should use PointXd
    /// </summary>
    public class CvCvt
    {
        #region Convert Mat to float[,]

        public static float[,] CvtToArray(Mat mat)
        {
            if (mat.Type() != MatType.CV_32F)
            {
                throw new ArgumentException();
            }

            var final = new Mat<float>();
            mat.ConvertTo(final, MatType.CV_32F);
            return final.ToRectangularArray();
        }

        #endregion

        #region Cut Point

        public static Point2d CutZToPoint2d(Point3d point3ds)
        {
            return new Point2d(point3ds.X, point3ds.Y);
        }

        #endregion


        #region Convert bool[,]

        public static string CvtToStr(bool[,] mat)
        {
            var row = mat.GetLength(0);
            var column = mat.GetLength(1);
            var strBuild = new StringBuilder();
            Enumerable.Range(0, row)
                .ToList()
                .ForEach(r =>
                {
                    var line = Enumerable.Range(0, column)
                        .ToList()
                        .Select(c => mat[r, c] ? 1 : 0);
                    strBuild.AppendLine(string.Join(",", line));
                });
            return strBuild.ToString();
        }


        public static Mat CvtToMat(bool[,] input)
        {
            var row = input.GetLength(0);
            var column = input.GetLength(1);
            var array = new double[row, column];
            Enumerable.Range(0, row)
                .ToList()
                .ForEach(r => Enumerable.Range(0, column)
                    .ToList()
                    .ForEach(c => array[r, c] = input[r, c] ? 1 : 0));
            return Mat.FromArray(array);
        }

        #endregion

        #region Conver From PointXd to PointXf

        public static Point2f CvtToPoint2f(Point point)
        {
            return new Point2f(point.X, point.Y);
        }

        public static Point2f CvtToPoint2f(Point2d point2F)
        {
            return new Point2f((float) point2F.X, (float) point2F.Y);
        }

        public static Point3f CvtToPoint3f(Point3d point3F)
        {
            return new Point3f((float) point3F.X, (float) point3F.Y, (float) point3F.Z);
        }


        /// <summary>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point2f[] CvtToPoint2fs(IEnumerable<Point> points)
        {
            return points.Select(CvtToPoint2f).ToArray();
        }


        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=3, X,Y,Z</param>
        /// <returns></returns>
        public static Point3f[] CvtToPoint3fs(IEnumerable<Point3d> positions)
        {
            return positions.Select(CvtToPoint3f).ToArray();
        }

        #endregion

        #region Conver From PointXf to PointXd

        public static Point2d CvtToPoint2d(Point point)
        {
            return new Point2d(point.X, point.Y);
        }

        public static Point2d CvtToPoint2d(Point2f point2F)
        {
            return new Point2d(point2F.X, point2F.Y);
        }

        public static Point3d CvtToPoint3d(Point3f point3F)
        {
            return new Point3d(point3F.X, point3F.Y, point3F.Z);
        }


        /// <summary>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point2d[] CvtToPoint2ds(IEnumerable<Point> points)
        {
            return points.Select(CvtToPoint2d).ToArray();
        }


        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=3, X,Y,Z</param>
        /// <returns></returns>
        public static Point3d[] CvtToPoint3ds(IEnumerable<Point3f> positions)
        {
            return positions.Select(CvtToPoint3d).ToArray();
        }

        #endregion

        #region Convert From Mat to PointXd

        public static Point2d[] CvtToPoint2ds(Mat word)
        {
            var final = new Mat<double>();
            word.ConvertTo(final, MatType.CV_64F);
            return CvtToPoint2ds(final);
        }

        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=2, u,v,</param>
        /// <returns></returns>
        public static Point2d[] CvtToPoint2ds(Mat<double> word)
        {
            var size = word.Size();
            if (size.Width != 2)
            {
                throw new ArgumentException("Width is not 2 for Parse");
            }

            var array = word.ToRectangularArray();

            return Enumerable.Range(0, size.Height).ToList()
                .Select(row => new Point2d(array[row, 0], array[row, 1]))
                .ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=3, X,Y,Z</param>
        /// <returns></returns>
        public static Point3d[] CvtToPoint3ds(Mat<double> world)
        {
            var size = world.Size();
            if (size.Width != 3)
            {
                throw new ArgumentException("Width is not 3 for Parse");
            }

            var array = world.ToRectangularArray();

            return Enumerable.Range(0, size.Height).ToList()
                .Select(row => new Point3d(array[row, 0], array[row, 1], array[row, 2]))
                .ToArray();
        }

        #endregion

        #region Convert From Point to Mat

        public static Mat CvtToMat(double[,] points)
        {
            return Mat.FromArray(points);
        }

        public static Mat CvtToMat(Point3d[] point3ds)
        {
            var len = point3ds.Length;
            var size = new Size(3, len);
            var mat = new Mat(size, MatType.CV_64F);
            foreach (var i in Enumerable.Range(0, len))
            {
                var p = point3ds[i];
                mat.Set(i, 0, p.X);
                mat.Set(i, 1, p.Y);
                mat.Set(i, 2, p.Z);
            }

            return mat;
        }

        public static Mat CvtToMat(Point2d[] point2ds)
        {
            var len = point2ds.Length;
            var size = new Size(2, len);
            var mat = new Mat(size, MatType.CV_64F);
            foreach (var i in Enumerable.Range(0, len))
            {
                var p = point2ds[i];
                mat.Set(i, 0, p.X);
                mat.Set(i, 1, p.Y);
            }

            return mat;
        }


        public static Mat CvtToMat(Point[] point2Fs)
        {
            var point2ds = point2Fs.Select(CvtToPoint2d).ToArray();
            return CvtToMat(point2ds);
        }

        #endregion

        #region Covert for Rect / RotatedRect

        public static RotatedRect CvtToRotatedRect(Rect rect)
        {
            var topRight = new Point(rect.Right, rect.Top);
            var bottomLeft = new Point(rect.Left, rect.Bottom);
            var points = new[] {rect.TopLeft, rect.BottomRight, topRight, bottomLeft};
            return Cv2.MinAreaRect(points);
        }


        public static Rect CvtToRect(RotatedRect rotatedRect)
        {
            return rotatedRect.BoundingRect();
        }

        #endregion
    }
}