using System.Collections.Generic;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     寻找阵列圆的处理器
    /// </summary>
    public class CirclePatternFinder
        : Processor<Mat, Point2d[]>
    {
        public CirclePatternFinder(Size pattern)
            : base("CirclePatternFinder")
        {
            Pattern = pattern;
        }

        public Size Pattern { set; get; }

        public bool FilterByArea { set; get; }
        public double MinArea { set; get; }
        public double MaxArea { set; get; }

        public bool FilterByCircularity { set; get; }
        public double MinCircularity { set; get; } = 0;
        public double MaxCircularity { set; get; } = 1;

        public byte BlobColor { set; get; } = 0;
        public double MinDistBetweenBlobs { set; get; }


        internal override Point2d[] Process(Mat input)
        {
            /// Step 3 Set Cumstom Filter
            var paras = new SimpleBlobDetector.Params
            {
                /// 像素面积控制
                FilterByArea = FilterByArea,
                MinArea = (float) MinArea,
                MaxArea = (float) MaxArea,

                FilterByCircularity = FilterByCircularity,
                MinCircularity = (float) MinCircularity,
                MaxCircularity = (float) MaxCircularity,

                FilterByColor = true,
                BlobColor = BlobColor,
                MinDistBetweenBlobs = (float) MinDistBetweenBlobs,


                FilterByInertia = false,
                FilterByConvexity = false
            };
            var simpleBlob = SimpleBlobDetector.Create(paras);

            var res = Cv2.FindCirclesGrid(
                input, Pattern,
                out var points,
                FindCirclesGridFlags.SymmetricGrid | FindCirclesGridFlags.Clustering,
                simpleBlob);
            var point2ds = points.Select(CvCvt.CvtToPoint2d).ToArray();
            return res ? point2ds : null;
        }

        internal override Mat Draw(Mat mat, Point2d[] point2ds)
        {
            if (point2ds == null)
                return mat;
            foreach (var p in point2ds)
            {
                var point = p.ToPoint();
                mat.Line(point + new Point(-50, 0), point + new Point(50, 0), PenColor);
                mat.Line(point + new Point(0, 50), point + new Point(0, -50), PenColor);
                mat.PutText($"{point.X:D}", point + new Point(100, 0), HersheyFonts.HersheyPlain, 2, PenColor);
                mat.PutText($"{point.Y:D}", point + new Point(100, 30), HersheyFonts.HersheyPlain, 2, PenColor);
            }

            return mat;
        }

        internal override double CalScore(Point2d[] point2ds)
        {
            if (point2ds == null)
                return 0;
            return point2ds.Length == Pattern.Height * Pattern.Width
                ? 1
                : 0;
        }


        #region Static Method

        /// <summary>
        /// </summary>
        /// <param name="imageSource"></param>
        /// <param name="minArea"></param>
        /// <param name="maxArea"></param>
        /// <param name="minDist"></param>
        /// <param name="blodColor"> 0: Black Circle, 255, white Circle</param>
        /// <returns></returns>
        public static IEnumerable<Point2f> FindBlackCircleByArea(
            string imageSource,
            Size pattern,
            double minArea,
            double maxArea,
            double minDist,
            byte blodColor = 0)
        {
            /// Step 1 Read Image
            var image = Cv2.ImRead(imageSource, ImreadModes.Grayscale);

            /// Step 2 Threshold
            var Mat2 = new Mat();
            Cv2.Threshold(image, Mat2, 122, 255, ThresholdTypes.Otsu);

            /// Step 3 Set Cumstom Filter
            var paras = new SimpleBlobDetector.Params
            {
                FilterByArea = true,
                MinArea = (float) minArea,
                MaxArea = (float) maxArea,

                /// 像素面积控制
                FilterByColor = true,
                BlobColor = blodColor,
                MinDistBetweenBlobs = (float) minDist,

                FilterByCircularity = false,
                FilterByInertia = false,
                FilterByConvexity = false
            };
            var simpleBlob = SimpleBlobDetector.Create(paras);

            var res = Cv2.FindCirclesGrid(
                Mat2, pattern,
                out var points,
                FindCirclesGridFlags.SymmetricGrid | FindCirclesGridFlags.Clustering,
                simpleBlob);

            return res ? points : null;
        }


        public static Mat CircelPatternCalPretreatment(Mat mat, int kernelSize)
        {
            Cv2.Threshold(mat, mat, 122, 255, ThresholdTypes.Otsu);
            var ke = Cv2.GetStructuringElement(MorphShapes.Ellipse,
                new Size(kernelSize, kernelSize),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Open, ke);
            return mat;
        }

        #endregion
    }
}