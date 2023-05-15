using OpenCvSharp;
using Xunit.Abstractions;
using Point = System.Drawing.Point;

namespace UnitTest
{
    public class AbstractTest
    {
        public ITestOutputHelper TestOutputHelper;

        public AbstractTest(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }

        public virtual void PrintObject(object obj)
        {
            TestOutputHelper.WriteLine(obj.ToString());
        }

        public virtual void PrintObject(IEnumerable<object> objs)
        {
            foreach (var o in objs)
            {
                PrintObject(o);
            }
        }


        #region System 打印

        /// <summary>
        ///     打印二维Double数组
        /// </summary>
        /// <param name="mat"></param>
        public virtual void PrintMatrix(double[,] mat)
        {
            var matrix = Mat.FromArray(mat);
            PrintMatrix(matrix);
        }


        /// <summary>
        ///     打印系统Point类型
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint(Point point)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            TestOutputHelper.WriteLine($"Point:\t[{point.X:F4},{point.Y:F4}]");
            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoints(IEnumerable<Point> points)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point in points)
            {
                TestOutputHelper.WriteLine($"Point_{++index:D2}:\t[{point.X:F4},{point.Y:F4}]");
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }

        #endregion

        #region OpenCV类型 打印扩展

        /// <summary>
        ///     打印64位浮点型Mat
        /// </summary>
        /// <param name="mat"></param>
        public virtual void PrintMatrix(Mat mat)
        {
            var size = mat.Size();
            TestOutputHelper.WriteLine(new string('-', 30));
            TestOutputHelper.WriteLine($"Matrix Size:\t{size.Height}*{size.Width}");
            for (var r = 0; r < size.Height; r++)
            {
                var row = mat.Row(r);
                row.GetArray(out double[] rowArray);
                var rowArrayStr = rowArray.Select(a => $"{a:F4}");
                TestOutputHelper.WriteLine(string.Join("\t", rowArrayStr));
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     打印32位浮点型Mat
        /// </summary>
        /// <param name="mat"></param>
        public virtual void PrintMatrixF(Mat mat)
        {
            var size = mat.Size();
            TestOutputHelper.WriteLine(new string('-', 30));
            TestOutputHelper.WriteLine($"Matrix Size:\t{size.Height}*{size.Width}");
            for (var r = 0; r < size.Height; r++)
            {
                var row = mat.Row(r);
                row.GetArray(out float[] rowArray);
                var rowArrayStr = rowArray.Select(a => $"{a:F4}");
                TestOutputHelper.WriteLine(string.Join("\t", rowArrayStr));
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     打印OpenCV Point2D类型
        /// </summary>
        /// <param name="point2d"></param>
        public virtual void PrintPoint2d(Point2d point2d)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            TestOutputHelper.WriteLine($"Point:\t[{point2d.X:F4},{point2d.Y:F4}]");
            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     打印OpenCV Point3D类型
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint3d(Point3d point3d)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            TestOutputHelper.WriteLine($"Point:\t[{point3d.X:F4},{point3d.Y:F4},{point3d.Z:F4}]");
            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint2Fs(IEnumerable<Point2f> point2ds)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point2d in point2ds)
            {
                TestOutputHelper.WriteLine($"Point_{++index:D2}:\t{point2d.X:F4}\t{point2d.Y:F4}");
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint2ds(IEnumerable<Point2d> point2ds)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point2d in point2ds)
            {
                TestOutputHelper.WriteLine($"Point_{++index:D2}:\t{point2d.X:F4}\t{point2d.Y:F4}");
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint3ds(IEnumerable<Point3d> point3ds)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point3d in point3ds)
            {
                TestOutputHelper.WriteLine($"Point_{++index:D2}:\t{point3d.X:F4}\t{point3d.Y:F4}\t{point3d.Z:F4}");
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     打印旋转矩形框
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintRotatedRects(IEnumerable<RotatedRect> rotatedRects)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            rotatedRects.ToList().ForEach(point => TestOutputHelper.WriteLine(
                $"{point.Center.X:F1}\t{point.Center.Y:F1}\t{point.Angle}\t" +
                $"{point.Size.Width:F2}\t{point.Size.Height:F2}"));

            TestOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     打印KeyPoints
        /// </summary>
        /// <param name="keyPoints"></param>
        public virtual void PrintKeyPoint(IEnumerable<KeyPoint> keyPoints)
        {
            TestOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var keyPoint in keyPoints)
            {
                var p = keyPoint.Pt;
                TestOutputHelper.WriteLine($"Point({++index:D2}):\t[{p.X:F4},{p.Y:F4}]\t{keyPoint.Size}");
            }

            TestOutputHelper.WriteLine(new string('-', 30));
        }

        #endregion
    }
}