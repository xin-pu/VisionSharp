using OpenCvSharp;

namespace VisionSharp.Processor.Transform
{
    public class LetterBox : ImageProcessor
    {
        public LetterBox(Size targetSize, string name = "LetterBox")
            : base(name)
        {
            TargetSize = targetSize;
        }

        public Size TargetSize { internal set; get; }

        internal int OriginalHeight { set; get; }
        internal int OriginalWidth { set; get; }

        public Scalar FillColor { set; get; } = new(255, 255, 255);

        /// <summary>
        ///     将图像等比缩放并补边到 TargetSize（信封变换）
        ///     返回新 Mat，不修改调用方传入的 input
        /// </summary>
        /// <param name="input">输入图像</param>
        /// <returns>信封变换后的新 Mat</returns>
        internal override Mat Process(Mat input)
        {
            OriginalHeight = input.Size().Height;
            OriginalWidth = input.Size().Width;
            var ratio = new[]
            {
                1d * TargetSize.Width / OriginalWidth,
                1d * TargetSize.Height / OriginalHeight
            };
            var minRatio = ratio.Min();
            var resizeWidth = (int) Math.Round(OriginalWidth * minRatio);
            var resizeHeight = (int) Math.Round(OriginalHeight * minRatio);
            var dw = (TargetSize.Width - resizeWidth) / 2;
            var dh = (TargetSize.Height - resizeHeight) / 2;

            var output = input.Clone();
            var resizeSize = new Size(resizeWidth, resizeHeight);
            if (TargetSize != resizeSize)
            {
                Cv2.Resize(output, output, resizeSize, interpolation: InterpolationFlags.Linear);
            }

            // ±0.1 的不对称取整保证 top+bottom 恰好填满剩余空间
            var top = (int) Math.Round(dh - 0.1);
            var bottom = (int) Math.Round(dh + 0.1);
            var left = (int) Math.Round(dw - 0.1);
            var right = (int) Math.Round(dw + 0.1);
            Cv2.CopyMakeBorder(output, output, top, bottom, left, right, BorderTypes.Constant, FillColor);
            return output;
        }
    }
}
