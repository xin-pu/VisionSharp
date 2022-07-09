using System.Text;
using OpenCvSharp;
using YAXLib.Attributes;

namespace CVLib.Processor
{
    /// <summary>
    ///     This is Point class for UI corresponding to point2d @ openCV
    /// </summary>
    public class CvPoint : CvViewModelBase
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

        [YAXDontSerialize] public Point Point => new(X, Y);


        public double X
        {
            set => UpdateProperty(ref x, value);
            get => x;
        }

        public double Y
        {
            set => UpdateProperty(ref y, value);
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