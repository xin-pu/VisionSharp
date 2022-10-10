using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace VisionSharp.Models.Ext
{
    /// <summary>
    ///     对应OpenCV中RotatedRect
    /// </summary>
    public class CvRotatedRect : ObservableObject
    {
        private double _angle;
        private double _height;
        private bool _horizontal;
        private double _width;
        private double _x;
        private double _y;

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
            Horizontal = Width > Height;
        }


        public RotatedRect RotatedRect => new(
            new Point2f((float) _x, (float) _y),
            new Size2f(_width, _height),
            (float) _angle);

        public double X
        {
            set => SetProperty(ref _x, value);
            get => _x;
        }

        public double Y
        {
            set => SetProperty(ref _y, value);
            get => _y;
        }

        public double Width
        {
            set => SetProperty(ref _width, value);
            get => _width;
        }

        public double Height
        {
            set => SetProperty(ref _height, value);
            get => _height;
        }

        /// <summary>
        ///     OpenCV 中 RotatedRect 角度（0°~90°)
        ///     只反映轮廓角度
        /// </summary>
        public double Angle
        {
            set => SetProperty(ref _angle, value);
            get => _angle;
        }

        public bool Horizontal
        {
            set => SetProperty(ref _horizontal, value);
            get => _horizontal;
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

        #region Static Method

        public static CvRotatedRect ConvertFromRotatedRect(RotatedRect rotatedRect)
        {
            return new CvRotatedRect
            {
                X = rotatedRect.Center.X,
                Y = rotatedRect.Center.Y,
                Width = rotatedRect.Size.Width,
                Height = rotatedRect.Size.Height,
                Angle = rotatedRect.Angle
            };
        }

        #endregion
    }
}