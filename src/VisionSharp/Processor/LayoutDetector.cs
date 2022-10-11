using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using VisionSharp.Models.Layout;

namespace VisionSharp.Processor
{
    public abstract class LayoutDetector : Processor<Mat, Layout>
    {
        private LayoutArgument _layoutArgument;

        /// <summary>
        ///     布局检测器
        /// </summary>
        protected LayoutDetector(LayoutArgument layoutArgument)
            : base("LayoutDetector")
        {
            LayoutArgument = layoutArgument;
        }

        public LayoutArgument LayoutArgument
        {
            internal set => SetProperty(ref _layoutArgument, value);
            get => _layoutArgument;
        }

        /// <summary>
        ///     返回布局是否可靠
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal override bool GetReliability(Layout result)
        {
            return result.GetReliability();
        }

        /// <summary>
        ///     绘制布局检测结果
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="result"></param>
        /// <param name="reliability"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, Layout result, bool reliability)
        {
            return mat;
        }
    }


    public class LayoutArgument : ObservableObject
    {
        private Size _inputPattern;
        private Size _layoutPattern;
        private double _scoreThreshoold;

        public LayoutArgument(Size layoutPattern, Size inputPattern, double scoreThreshoold)
        {
            InputPattern = inputPattern;
            LayoutPattern = layoutPattern;
            _scoreThreshoold = scoreThreshoold;
        }

        public Size LayoutPattern
        {
            internal set => SetProperty(ref _layoutPattern, value);
            get => _layoutPattern;
        }

        public Size InputPattern
        {
            internal set => SetProperty(ref _inputPattern, value);
            get => _inputPattern;
        }

        public double ScoreThreshold
        {
            internal set => SetProperty(ref _scoreThreshoold, value);
            get => _scoreThreshoold;
        }
    }
}