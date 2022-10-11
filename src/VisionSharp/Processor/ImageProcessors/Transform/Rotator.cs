using OpenCvSharp;

namespace VisionSharp.Processor.Transform
{
    public class Rotator : ImageProcessor
    {
        private RotateDeg _rotateDeg;

        /// <summary>
        ///     翻转处理器-逆时针，只处理90,180,270度
        /// </summary>
        public Rotator(RotateDeg rotateDeg)
            : base("Rotator")
        {
            RotateDeg = rotateDeg;
        }

        public RotateDeg RotateDeg
        {
            internal set => SetProperty(ref _rotateDeg, value);
            get => _rotateDeg;
        }


        internal override Mat Process(Mat input)
        {
            return Rotate(input, RotateDeg);
        }

        /// <summary>
        ///     公开的旋转方法
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rotateDeg"></param>
        /// <returns></returns>
        public Mat Call(Mat input, RotateDeg rotateDeg)
        {
            return Rotate(input, rotateDeg);
        }

        internal Mat Rotate(Mat input, RotateDeg rotateDeg)
        {
            var clone = input.Clone();
            switch (rotateDeg)
            {
                case RotateDeg.Deg0:
                    return clone;
                case RotateDeg.Deg90:
                    Cv2.Transpose(clone, clone);
                    Cv2.Flip(clone, clone, FlipMode.X);
                    return clone;
                case RotateDeg.Deg180:
                    Cv2.Flip(clone, clone, FlipMode.XY);
                    return clone;
                case RotateDeg.Deg270:
                    Cv2.Transpose(clone, clone);
                    Cv2.Flip(clone, clone, FlipMode.Y);
                    return clone;
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