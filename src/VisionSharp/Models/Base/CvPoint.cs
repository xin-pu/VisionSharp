using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace VisionSharp.Models.Base
{
    public class CvPoint : ObservableObject
    {
        private double _x;
        private double _y;

        /// <summary>
        ///     可观测的点对象
        /// </summary>
        public CvPoint()
        {
        }

        /// <summary>
        ///     可观测的点对象
        /// </summary>
        /// <param name="p">浮点精度坐标</param>
        public CvPoint(Point2d p)
        {
            X = p.X;
            Y = p.Y;
        }

        public CvPoint(Point2f p)
        {
            X = p.X;
            Y = p.Y;
        }

        public CvPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point Point => new(X, Y);


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


        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("CvPoint");
            str.AppendLine($"\tCenter\t({X:F4},{Y:F4})");
            return str.ToString();
        }
    }
}