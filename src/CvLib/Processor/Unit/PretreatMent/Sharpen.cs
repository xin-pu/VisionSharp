using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     锐化处理器
    /// </summary>
    public class Sharpen
        : Processor<Mat, Mat>
    {
        public Sharpen()
            : base("Sharpen")
        {
        }

        internal override Mat Process(Mat input)
        {
            var matout = new Mat();
            var kernel = Mat.FromArray(new float[,]
            {
                {0, -1, 0},
                {-1, 5, -1},
                {0, -1, 0}
            });
            Cv2.Filter2D(input, matout, input.Type(), kernel, new Point(-1, -1));
            return matout;
        }
    }
}