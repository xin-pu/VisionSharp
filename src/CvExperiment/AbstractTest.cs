using System.Collections.Generic;
using System.Linq;
using CVLib.Processor;
using OpenCvSharp;
using Xunit.Abstractions;

namespace CvExperiment
{
    public class AbstractTest
    {
        public ITestOutputHelper _testOutputHelper;

        public AbstractTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public virtual void PrintObject(object obj)
        {
            _testOutputHelper.WriteLine(obj.ToString());
        }


        /// <summary>
        ///     Print OpenCV Mat
        /// </summary>
        /// <param name="mat"></param>
        public virtual void PrintMatrix(Mat mat)
        {
            var size = mat.Size();
            _testOutputHelper.WriteLine(new string('-', 30));
            _testOutputHelper.WriteLine($"Matrix Size:\t{size.Height}*{size.Width}");
            for (var r = 0; r < size.Height; r++)
            {
                var row = mat.Row(r);
                row.GetArray(out double[] rowArray);
                var rowArrayStr = rowArray.Select(a => $"{a:F4}");
                _testOutputHelper.WriteLine(string.Join("\t", rowArrayStr));
            }

            _testOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print OpenCV Mat
        /// </summary>
        /// <param name="mat"></param>
        public virtual void PrintMatrixF(Mat mat)
        {
            var size = mat.Size();
            _testOutputHelper.WriteLine(new string('-', 30));
            _testOutputHelper.WriteLine($"Matrix Size:\t{size.Height}*{size.Width}");
            for (var r = 0; r < size.Height; r++)
            {
                var row = mat.Row(r);
                row.GetArray(out float[] rowArray);
                var rowArrayStr = rowArray.Select(a => $"{a:F4}");
                _testOutputHelper.WriteLine(string.Join("\t", rowArrayStr));
            }

            _testOutputHelper.WriteLine(new string('-', 30));
        }

        public virtual void PrintMatrix(double[,] mat)
        {
            var matrix = Mat.FromArray(mat);
            PrintMatrix(matrix);
        }

        public virtual void PrintRichInfo<T>(RichInfo<T> richInfo)
        {
            _testOutputHelper.WriteLine(new string('-', 30));

            _testOutputHelper.WriteLine($"Result:{richInfo.Result}");
            _testOutputHelper.WriteLine($"Confidence:{richInfo.Confidence}");

            _testOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint(Point point)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            _testOutputHelper.WriteLine($"Point:\t{point.X:F4}\t{point.Y:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint3d(Point3d point3d)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            _testOutputHelper.WriteLine($"Point:\t{point3d.X:F4}\t{point3d.Y:F4}\t{point3d.Z:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point2d"></param>
        public virtual void PrintPoint2d(Point2d point2d)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            _testOutputHelper.WriteLine($"Point2d:\t{point2d.X:F4}\t{point2d.Y:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoints(IEnumerable<Point> points)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point in points)
                _testOutputHelper.WriteLine($"Point_{++index:D2}:\t{point.X:F4}\t{point.Y:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint2fs(IEnumerable<Point2f> point2ds)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point2d in point2ds)
                _testOutputHelper.WriteLine($"Point_{++index:D2}:\t{point2d.X:F4}\t{point2d.Y:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint2ds(IEnumerable<Point2d> point2ds)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point2d in point2ds)
                _testOutputHelper.WriteLine($"Point_{++index:D2}:\t{point2d.X:F4}\t{point2d.Y:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }

        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintPoint3ds(IEnumerable<Point3d> point3ds)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var point3d in point3ds)
                _testOutputHelper.WriteLine($"Point_{++index:D2}:\t{point3d.X:F4}\t{point3d.Y:F4}\t{point3d.Z:F4}");
            _testOutputHelper.WriteLine(new string('-', 30));
        }


        public virtual void PrintKeyPoint(IEnumerable<KeyPoint> keyPoints)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            var index = 0;
            foreach (var keyPoint in keyPoints)
            {
                var p = keyPoint.Pt;
                _testOutputHelper.WriteLine($"Point_{++index:D2}:\t{p.X:F4}\t{p.Y:F4}\t{keyPoint.Size}");
            }

            _testOutputHelper.WriteLine(new string('-', 30));
        }


        /// <summary>
        ///     Print Point3d
        /// </summary>
        /// <param name="point3d"></param>
        public virtual void PrintRotatedRects(IEnumerable<RotatedRect> rotatedRects)
        {
            _testOutputHelper.WriteLine(new string('-', 30));
            rotatedRects.ToList().ForEach(point =>
            {
                _testOutputHelper.WriteLine(
                    $"{point.Center.X:F1}\t{point.Center.Y:F1}\t{point.Angle}\t" +
                    $"{point.Size.Width:F2}\t{point.Size.Height:F2}");
            });

            _testOutputHelper.WriteLine(new string('-', 30));
        }
    }
}