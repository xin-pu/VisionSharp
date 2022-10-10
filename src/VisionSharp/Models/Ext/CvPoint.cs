using System.Text;
using GalaSoft.MvvmLight;
using OpenCvSharp;

namespace VisionSharp.Models.Ext
{
    /// <summary>
    ///     This is Point class for UI corresponding to point2d @ openCV
    /// </summary>
    public class CvPoint : ViewModelBase
    {
        private double x;
        private double y;

        public CvPoint()
        {
        }

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
            set => Set(ref x, value);
            get => x;
        }

        public double Y
        {
            set => Set(ref y, value);
            get => y;
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