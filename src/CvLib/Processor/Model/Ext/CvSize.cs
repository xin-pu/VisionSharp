using System.Text;
using GalaSoft.MvvmLight;
using OpenCvSharp;
using YAXLib.Attributes;

namespace CVLib.Processor
{
    public class CvSize : ViewModelBase
    {
        private double height;
        private double width;

        public CvSize()
        {
        }

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

        [YAXDontSerialize] public Size Size => new(Width, Height);

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

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("CvSize");
            str.AppendLine($"\tSize:\t({Width:F2}*{Height:F2})");
            return str.ToString();
        }
    }
}