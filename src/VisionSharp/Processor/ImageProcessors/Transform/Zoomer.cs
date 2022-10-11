using OpenCvSharp;

namespace VisionSharp.Processor.Transform
{
    /// <summary>
    ///     缩放处理器
    /// </summary>
    public class Zoomer : ImageProcessor
    {
        public Zoomer(Size size) : base("Zoomer")
        {
            TargetSize = size;
        }

        public Size TargetSize { set; get; }
        public Size TempSize { set; get; } = new(500, 500);

        internal override Mat Process(Mat input)
        {
            var size = input.Size();
            var temp = new Mat(TempSize, input.Type());

            var shift = new Point(TempSize.Width / 2 - size.Width / 2,
                TempSize.Height / 2 - size.Height / 2);
            var rect = new Rect(shift, size);
            temp[rect] = input;
            return input.Resize(TempSize);
        }
    }
}