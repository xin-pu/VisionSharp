using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     图像绘制中心十字标
    /// </summary>
    public class DrawCenter
        : Processor<Mat, Mat>
    {
        public DrawCenter()
            : base("DrawCenter")
        {
        }

        internal override Mat Process(Mat input)
        {
            var size = input.Size();
            var pointXs = new Point(0, size.Height / 2);
            var pointXe = new Point(size.Width - 1, size.Height / 2);
            input.Line(pointXs, pointXe, Scalar.White, 3, shift: 1);
            var pointYs = new Point(size.Width / 2, 0);
            var pointYe = new Point(size.Width / 2, size.Width - 1);
            input.Line(pointYs, pointYe, Scalar.White, 3, shift: 1);
            return input;
        }
    }
}