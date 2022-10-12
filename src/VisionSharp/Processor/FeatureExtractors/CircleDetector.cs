using OpenCvSharp;

namespace VisionSharp.Processor.FeatureExtractors
{
    public class CircleDetector : FeatureExtractor<KeyPoint[]>
    {
        private byte _blodColor;
        private bool _filterByArea;
        private bool _filterByCircularity = true;

        private double _maxArea;
        private double _maxCircularity = 1;
        private double _minArea = double.MaxValue;
        private double _minCircularity;
        private double _minDistBetweenBlobs;

        /// <summary>
        ///     寻找圆的处理器,对灰度图像
        /// </summary>
        public CircleDetector()
            : base("CircleFinder")
        {
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


        internal override KeyPoint[] Process(Mat input)
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

            var key = simpleBlob.Detect(input);
            return key;
        }

        internal override Mat Draw(Mat mat, KeyPoint[] result, bool reliability)
        {
            if (result == null)
            {
                return mat;
            }

            foreach (var keyPoint in result)
            {
                var point = keyPoint.Pt.ToPoint();
                mat = DrawPoint(mat, point, PenColor);
                mat = DrawText(mat, point, $"[{point.X:D},{point.Y:D}]", PenColor);
            }

            return mat;
        }

        /// <summary>
        ///     找到KeyPoints即认为可靠
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal override bool GetReliability(KeyPoint[] result)
        {
            return result is {Length: > 0};
        }
    }
}