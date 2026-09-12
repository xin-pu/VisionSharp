using OpenCvSharp;
using VisionSharp.Utils;

namespace VisionSharp.Processor.FeatureExtractors
{
    /// <summary>
    ///     寻找阵列圆的处理器，返回圆心坐标
    /// </summary>
    public class CirclePatternDetector : BlobDetectorBase<Point2d[]>
    {
        private Size _pattern;

        /// <summary>
        ///     寻找阵列圆的处理器，返回圆心坐标
        /// </summary>
        public CirclePatternDetector(Size pattern)
            : base("CirclePatternFinder")
        {
            Pattern = pattern;
        }

        /// <summary>
        ///     目标阵列圆的大小
        /// </summary>
        public Size Pattern
        {
            set => SetProperty(ref _pattern, value);
            get => _pattern;
        }

        internal override Point2d[] Process(Mat input)
        {
            using var simpleBlob = CreateBlobDetector();

            var res = Cv2.FindCirclesGrid(
                input, Pattern,
                out var points,
                FindCirclesGridFlags.SymmetricGrid | FindCirclesGridFlags.Clustering,
                simpleBlob);
            return res
                ? points.Select(CvCvt.CvtToPoint2d).ToArray()
                : Array.Empty<Point2d>();
        }

        internal override Mat Draw(Mat mat, Point2d[] result, bool reliability)
        {
            foreach (var point2d in result)
            {
                var point = point2d.ToPoint();
                mat = DrawPoint(mat, point, PenColor);
                mat = DrawText(mat, point, $"[{point.X:D},{point.Y:D}]", PenColor);
            }

            return mat;
        }

        /// <summary>
        ///     找到的圆心数量等于阵列规模时认为可靠
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal override bool GetReliability(Point2d[] result)
        {
            if (result == null)
            {
                return false;
            }

            var reliability = result.Length == Pattern.Height * Pattern.Width;
            return reliability;
        }
    }
}