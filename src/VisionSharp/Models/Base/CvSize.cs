using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace VisionSharp.Models.Base
{
    public class CvSize : ObservableObject
    {
        private double _height;
        private double _width;

        /// <summary>
        ///     可观测的尺度
        /// </summary>
        public CvSize()
        {
        }

        /// <summary>
        ///     可观测的尺度
        /// </summary>
        public CvSize(Size2f s)
        {
            Width = s.Width;
            Height = s.Height;
        }

        public CvSize(Size2d s)
        {
            Width = s.Width;
            Height = s.Height;
        }

        public CvSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public Size Size => new(Width, Height);

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

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("CvSize");
            str.AppendLine($"\tSize:\t({Width:F2}*{Height:F2})");
            return str.ToString();
        }
    }
}