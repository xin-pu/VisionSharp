using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Models.Layout;
using VisionSharp.Utils;

namespace VisionSharp.Processor
{
    public abstract class LayoutDetector<T> : FeatureExtractor<Layout<T>> where T : Enum
    {
        private LayoutArgument _layoutArgument;

        internal Scalar UnReliableScalar = new(0, 106, 255);

        /// <summary>
        ///     布局检测器,输入乳香，返回布局对象
        /// </summary>
        protected LayoutDetector(LayoutArgument layoutArgument)
            : base("LayoutDetector")
        {
            LayoutArgument = layoutArgument;
            Colors = CvCvt.GetColorDict<T>();
        }

        internal Dictionary<T, Scalar> Colors { set; get; }

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
        internal override bool GetReliability(Layout<T> result)
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
        internal override Mat Draw(Mat mat, Layout<T> result, bool reliability)
        {
            var patternSize = new Size(result.Column, result.Row);
            var space = mat.Height / 100;
            var fontscale = 2 * mat.Height / 1000;
            var thickness = 5;
            var gridWidth = mat.Size().Width / patternSize.Width;
            var gridHeight = mat.Size().Height / patternSize.Height;
            var gridSize = new Size(gridWidth - space, gridHeight - space);

            var pointStart = new Point(0, 0);

            Enumerable.Range(0, patternSize.Height).ToList().ForEach(r =>
                Enumerable.Range(0, patternSize.Width).ToList().ForEach(c =>
                {
                    var cell = result[r, c];
                    var color = cell.Reliable == Reliable.Reliable
                        ? Colors[cell.Category]
                        : UnReliableScalar;
                    var info = cell.Reliable == Reliable.Reliable
                        ? $"{cell.Category}:{cell.GetScore():F4}"
                        : $"{cell.Reliable}:{cell.GetScore():F4}";


                    var currentPoint = pointStart +
                                       new Point(gridWidth * c, gridHeight * r) +
                                       new Point(space / 2, space / 2);

                    mat = DrawRect(mat, new Rect(currentPoint, gridSize), color, thickness);

                    DrawText(mat, currentPoint, info, color, fontscale, thickness);
                }));
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

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"LayoutPattern\t{LayoutPattern}");
            str.AppendLine($"InputPattern\t{InputPattern}");
            str.AppendLine($"ScoreThreshold\t{ScoreThreshold}");
            return str.ToString();
        }
    }
}