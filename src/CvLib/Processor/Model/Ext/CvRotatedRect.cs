using System.Text;
using GalaSoft.MvvmLight;
using OpenCvSharp;
using YAXLib.Attributes;

namespace CVLib.Processor
{
    /// <summary>
    ///     This is RotatedRect for UI corresponding to cell of RotatedRect @ openCV
    /// </summary>
    public class CvRotatedRect : ViewModelBase
    {
        private double angle;
        private double height;
        private double width;
        private double x;
        private double y;

        public CvRotatedRect()
        {
        }

        public CvRotatedRect(RotatedRect rotatedRect)
        {
            X = rotatedRect.Center.X;
            Y = rotatedRect.Center.Y;
            Width = rotatedRect.Size.Width;
            Height = rotatedRect.Size.Height;
            Angle = rotatedRect.Angle;
        }


        [YAXDontSerialize]
        public RotatedRect RotatedRect => new(
            new Point2f((float) x, (float) y),
            new Size2f(width, height),
            (float) angle);

        public double X
        {
            set => Set(ref x, value);
            get => x;
        }

        public double Y
        {
            set => Set(ref y, value);
            get => y;
        }

        public double Width
        {
            set => Set(ref width, value);
            get => width;
        }

        public double Height
        {
            set => Set(ref height, value);
            get => height;
        }

        /// <summary>
        ///     OpenCV 中 RotatedRect 角度（0°~90°)
        ///     只反映轮廓角度
        /// </summary>
        public double Angle
        {
            set => Set(ref angle, value);
            get => angle;
        }


        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("CvRotatedRect");
            str.AppendLine($"\tCenter\t({X:F2},{Y:F2})");
            str.AppendLine($"\tSize:\t({Width:F2}*{Height:F2})");
            str.AppendLine($"\tAngle:\t{Angle:F2} Deg");
            return str.ToString();
        }
    }
}