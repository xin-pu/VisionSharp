using System.Text;
using GalaSoft.MvvmLight;
using OpenCvSharp;
using YAXLib.Attributes;

namespace CVLib.Processor
{
    public class CvRect : ViewModelBase
    {
        private int height;
        private int width;
        private int x;
        private int y;

        public CvRect()
        {
        }

        public CvRect(Rect rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public CvRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        [YAXDontSerialize] public Rect Rect => new(X, Y, Width, Height);

        public int X
        {
            set => Set(ref x, value);
            get => x;
        }

        public int Y
        {
            set => Set(ref y, value);
            get => y;
        }

        public int Width
        {
            set => Set(ref width, value);
            get => width;
        }

        public int Height
        {
            set => Set(ref height, value);
            get => height;
        }


        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append($"Rect\t[{X:F4},{Y:F4}],[{Width:F4},{Height:F4}]");
            return str.ToString();
        }
    }
}