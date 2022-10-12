using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace VisionSharp.Models.Base
{
    public class CvRect : ObservableObject
    {
        private int _height;
        private int _width;
        private int _x;
        private int _y;

        /// <summary>
        ///     拓展的矩形框
        /// </summary>
        public CvRect()
        {
        }

        /// <summary>
        ///     从Rect构造
        /// </summary>
        /// <param name="rect"></param>
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


        public Rect Rect => new(X, Y, Width, Height);

        public int X
        {
            set => SetProperty(ref _x, value);
            get => _x;
        }

        public int Y
        {
            set => SetProperty(ref _y, value);
            get => _y;
        }

        public int Width
        {
            set => SetProperty(ref _width, value);
            get => _width;
        }

        public int Height
        {
            set => SetProperty(ref _height, value);
            get => _height;
        }


        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"Rect:\t[{X:F4},{Y:F4}],[{Width:F4},{Height:F4}]");
            return str.ToString();
        }
    }
}