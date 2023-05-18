using System.ComponentModel;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor
{
    public abstract class ObjectDetector<T> : FeatureExtractor<ObjRect<T>[]> where T : Enum
    {
        private float _confidence = 0.8f;

        private float _iouThreshold = 0.5f;

        /// <summary>
        ///     目标检测器
        /// </summary>
        protected ObjectDetector(string name) : base(name)
        {
        }

        /// <summary>
        ///     预测网络，不需要观测
        /// </summary>
        public Net Net { internal set; get; }

        /// <summary>
        ///     用于标记分类的颜色字体
        /// </summary>
        internal Dictionary<T, Scalar> Colors { set; get; }

        [Category("Option")]
        public float Confidence
        {
            set => SetProperty(ref _confidence, value);
            get => _confidence;
        }

        [Category("Option")]
        public float IouThreshold
        {
            set => SetProperty(ref _iouThreshold, value);
            get => _iouThreshold;
        }

        public int CategoryCount => Enum.GetNames(typeof(T)).Length;

        internal override ObjRect<T>[] Process(Mat input)
        {
            var mats = FrontNet(Net, input);
            var candidate = Decode(mats, input.Size());
            var final = NonMaximalSuppression(candidate);
            return final;
        }

        /// <summary>
        ///     从文件恢复模型网络
        /// </summary>
        /// <returns></returns>
        internal abstract Net InitialNet();

        /// <summary>
        ///     前向传递
        /// </summary>
        /// <param name="net"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal abstract Mat[] FrontNet(Net net, Mat mat);

        /// <summary>
        ///     解码获取候选预测框
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal abstract ObjRect<T>[] Decode(Mat[] mats, Size size);

        /// <summary>
        ///     非最大值抑制预测框，输入ObjRect，所以非极大值抑制算法通用
        ///     Todo 可以加入其他非极大值抑制算法 目前使用OpenCV DNN模块的算法
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        internal virtual ObjRect<T>[] NonMaximalSuppression(ObjRect<T>[] candidate)
        {
            CvDnn.NMSBoxes(
                candidate.Select(a => a.Rect),
                candidate.Select(a => a.ObjectConfidence),
                Confidence,
                IouThreshold,
                out var boxIndex);
            var filter = candidate
                .Where((_, b) => boxIndex.Contains(b))
                .ToArray();

            return filter;
        }

        /// <summary>
        ///     绘制最终预测框
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, ObjRect<T>[] result, bool reliability)
        {
            result
                .ToList()
                .ForEach(a =>
                {
                    var info = $"{a.Category} {a.ObjectConfidence:P2}";
                    var color = Colors[a.Category];
                    var fontscale = 1d * mat.Height / 600;
                    mat = DrawRect(mat, a.Rect, color, 1);
                    mat = DrawText(mat, a.Rect.TopLeft, info, color, fontscale);
                });
            return mat;
        }
    }
}