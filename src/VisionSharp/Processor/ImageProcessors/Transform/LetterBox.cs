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

            var resizeSize = new Size(resizeWidth, resizeHeight);
            if (TargetSize != resizeSize)
            {
                Cv2.Resize(input, input, resizeSize, interpolation: InterpolationFlags.Linear);
            }

            var top = (int) Math.Round(dh - 0.1);
            var bottom = (int) Math.Round(dh + 0.1);
            var left = (int) Math.Round(dw - 0.1);
            var right = (int) Math.Round(dw + 0.1);
            Cv2.CopyMakeBorder(input, input, top, bottom, left, right, BorderTypes.Constant, FillColor);
            return input;
        }
    }
}
