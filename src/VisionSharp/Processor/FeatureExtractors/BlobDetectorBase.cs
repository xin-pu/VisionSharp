using OpenCvSharp;

namespace VisionSharp.Processor.FeatureExtractors
{
    /// <summary>
    ///     SimpleBlobDetector 参数化基类
    ///     提供面积/圆度/颜色等通用过滤条件，派生类通过 CreateBlobDetector 构造检测器
    /// </summary>
    /// <typeparam name="T">检测结果类型</typeparam>
    public abstract class BlobDetectorBase<T> : FeatureExtractor<T>
    {
        private byte _blobColor;
        private bool _filterByArea;
        private bool _filterByCircularity = true;

        private double _maxArea;
        private double _maxCircularity = 1;
        private double _minArea = double.MaxValue;
        private double _minCircularity;
        private double _minDistBetweenBlobs;

        protected BlobDetectorBase(string name) : base(name)
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
        ///     目标斑点颜色（0 为黑，255 为白）
        /// </summary>
        public byte BlobColor
        {
            set => SetProperty(ref _blobColor, value);
            get => _blobColor;
        }

        /// <summary>
        ///     斑点最小间距
        /// </summary>
        public double MinDistBetweenBlobs
        {
            set => SetProperty(ref _minDistBetweenBlobs, value);
            get => _minDistBetweenBlobs;
        }

        /// <summary>
        ///     按当前过滤条件构造 SimpleBlobDetector
        /// </summary>
        /// <returns>检测器（调用方负责释放）</returns>
        protected SimpleBlobDetector CreateBlobDetector()
        {
            var paras = new SimpleBlobDetector.Params
            {
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
            return SimpleBlobDetector.Create(paras);
        }
    }
}
