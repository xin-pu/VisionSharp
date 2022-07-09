using System;
using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     翻转处理器-逆时针，只处理90,180,270度
    /// </summary>
    public class Rotator
        : Processor<Mat, Mat>
    {
        public Rotator(RotateDeg rotateDeg)
            : base("Rotator")
        {
            RotateDeg = rotateDeg;
        }

        public RotateDeg RotateDeg { set; get; }


        internal override Mat Process(Mat input)
        {
            switch (RotateDeg)
            {
                case RotateDeg.Deg0:
                    return input;
                case RotateDeg.Deg90:
                    Cv2.Transpose(input, input);
                    Cv2.Flip(input, input, FlipMode.X);
                    return input;
                case RotateDeg.Deg180:
                    Cv2.Flip(input, input, FlipMode.XY);
                    return input;
                case RotateDeg.Deg270:
                    Cv2.Transpose(input, input);
                    Cv2.Flip(input, input, FlipMode.Y);
                    return input;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum RotateDeg
    {
        Deg0,
        Deg90,
        Deg180,
        Deg270
    }
}