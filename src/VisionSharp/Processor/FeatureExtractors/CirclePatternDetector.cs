using OpenCvSharp;
using VisionSharp.Utils;

namespace VisionSharp.Processor.FeatureExtractors
{
    public class CirclePatternDetector : FeatureExtractor<Point2d[]>
    {
        private byte _blodColor;
        private bool _filterByArea;
        private bool _filterByCircularity = true;

        private double _maxArea;
        private double _maxCircularity = 1;
        private double _minArea = double.MaxValue;
        private double _minCircularity;
        private double _minDistBetweenBlobs;

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

        /// <summary>
        ///     是否按面积过滤
        /// </summary>
        public bool FilterByArea
        {
            set => SetProperty(ref _filterByArea, value);
            get => _filterByArea;
        }

        /// <summary>
        ///     面积下限
        /// </summary>
        public double MinArea
        {
            set => SetProperty(ref _minArea, value);
            get => _minArea;
        }

        /// <summary>
        ///     面积上限
        /// </summary>
        public double MaxArea
        {
            set => SetProperty(ref _maxArea, value);
            get => _maxArea;
        }

        /// <summary>
        ///     是否按圆度过滤
        /// </summary>
        public bool FilterByCircularity
        {
            set => SetProperty(ref _filterByCircularity, value);
            get => _filterByCircularity;
        }

        /// <summary>
        ///     圆度下限
        /// </summary>
        public double MinCircularity
        {
            set => SetProperty(ref _minCircularity, value);
            get => _minCircularity;
        }

        /// <summary>
        ///     圆度上限
        /// </summary>
        public double MaxCircularity
        {
            set => SetProperty(ref _maxCircularity, value);
            get => _maxCircularity;
        }

        /// <summary>
        ///     颜色
        /// </summary>
        public byte BlobColor
        {
            set => SetProperty(ref _blodColor, value);
            get => _blodColor;
        }

        /// <summary>
        ///     最小间距
        /// </summary>
        public double MinDistBetweenBlobs
        {
            set => SetProperty(ref _minDistBetweenBlobs, value);
            get => _minDistBetweenBlobs;
        }


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
            var mat2 = new Mat();
            Cv2.Threshold(image, mat2, 122, 255, ThresholdTypes.Otsu);

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
                mat2, pattern,
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