using System;
using System.Linq;
using CVLib.Utils;
using MathNet.Numerics.Statistics;
using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     This processor find angle by find key point first, then find line.
    ///     At last, fit lines and get mean K of all lines
    /// </summary>
    public abstract class AngleProcessor
        : Processor<KeyPoint[], double>
    {
        /// <summary>
        ///     will find angle between 0 ~360
        /// </summary>
        /// <param name="name"></param>
        protected AngleProcessor(string name = "AngleProcessor")
            : base(name)
        {
        }

        /// <summary>
        ///     filtered  keyPoint for  lines
        /// </summary>
        public KeyPoint[] KeyPoints { set; get; }

        /// <summary>
        ///     lines information (performance by intercept, slope)
        /// </summary>
        public Tuple<double, double>[] KeyLines { set; get; }


        /// <summary>
        ///     This is  a normal function to draw Angle result in mat
        /// </summary>
        /// <param name="grayMat"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat grayMat, double result)
        {
            var mat = grayMat.Clone();
            var height = mat.Height;

            /// draw all keylines
            KeyLines.ToList().ForEach(l =>
            {
                var p1 = CvMath.GetPoint(0, l);
                var p2 = CvMath.GetPoint(height - 1, l);
                mat.Line(p1, p2, Scalar.White);
            });

            /// draw all keypoints
            KeyPoints.ToList().ForEach(keyPoint =>
            {
                var center = new Point(keyPoint.Pt.X, keyPoint.Pt.Y);
                mat.Circle(center, 2, Scalar.Black, -1);
            });


            return mat;
        }

        /// <summary>
        ///     Cal Angle in range: (0~360)
        /// </summary>
        /// <param name="k"></param>
        /// <param name="heads"></param>
        /// <param name="tails"></param>
        /// <returns></returns>
        internal virtual double CalAngle(double[] klist, bool isPositive)
        {
            var k = klist.Mean();
            var angleBasic = 180 * Math.Atan(-k) / Math.PI;
            angleBasic = angleBasic > 0 ? angleBasic : 180 + angleBasic;
            var angleFinal = isPositive ? angleBasic : angleBasic + 180;
            return angleFinal;
        }

        internal override bool CalScore(double result)
        {
            return true;
        }
    }
}