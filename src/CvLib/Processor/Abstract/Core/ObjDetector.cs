using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor
{
    /// <summary>
    ///     用于目标检测的探测器
    ///     输入一张图像
    ///     返回多个含有目标物的检测框
    /// </summary>
    public abstract class ObjDetector : MatProcessor<List<DetectRectObject>>

    {
        private float _confidence = 0.8f;

        private float _iouThreshold = 0.5f;

        protected ObjDetector(string name)
            : base(name)
        {
        }

        [Category("Option")]
        public float Confidence
        {
            set => Set(ref _confidence, value);
            get => _confidence;
        }

        [Category("Option")]
        public float IOUThreshold
        {
            set => Set(ref _iouThreshold, value);
            get => _iouThreshold;
        }


        internal string[] Categroy { get; set; }
        internal Net Model { set; get; }
        internal Scalar[] Colors { get; set; }

        internal override List<DetectRectObject> Process(Mat input)
        {
            Model ??= InitialNet();
            var mats = FrontNet(Model, input);
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
        internal abstract List<DetectRectObject> Decode(Mat[] mats, Size size);

        /// <summary>
        ///     非最大值抑制预测框
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        internal virtual List<DetectRectObject> NonMaximalSuppression(List<DetectRectObject> candidate)
        {
            CvDnn.NMSBoxes(
                candidate.Select(a => a.Rect),
                candidate.Select(a => a.ObjectConfidence),
                Confidence,
                IOUThreshold,
                out var boxIndex);
            var filter = candidate
                .Where((_, b) => boxIndex.Contains(b))
                .ToList();

            return filter;
        }


        /// <summary>
        ///     绘制最终预测框
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, List<DetectRectObject> detectObjects)
        {
            detectObjects
                .ToList()
                .ForEach(a =>
                {
                    DrawRect(mat, a.Rect, Colors[a.Category], thickness: 2, size: 5);
                    mat.PutText($"{Categroy[a.Category]} {a.ObjectConfidence:P2}",
                        a.Rect.TopLeft,
                        HersheyFonts.HersheyPlain,
                        2, Colors[a.Category], 2);
                });
            return mat;
        }
    }
}