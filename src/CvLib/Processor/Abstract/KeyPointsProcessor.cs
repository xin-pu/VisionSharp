using System;
using System.Linq;
using MathNet.Numerics.Statistics;
using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     This processor find angle by find key point first, then find line.
    /// </summary>
    public abstract class KeyPointsProcessor
        : Processor<KeyPoint[], KeyPoint[]>
    {
        protected KeyPointsProcessor(int count, string name = "MeanPoint")
            : base(name)
        {
            Count = count;
        }


        public int Count { set; get; }


        internal override Mat Draw(Mat grayMat, KeyPoint[] result)
        {
            var mat = grayMat.Clone();

            /// draw all keypoints
            result.ToList().ForEach(keyPoint =>
            {
                var center = new Point(keyPoint.Pt.X, keyPoint.Pt.Y);
                mat.Circle(center, 2, Scalar.Black, -1);
            });

            return mat;
        }

        internal override double CalScore(KeyPoint[] result)
        {
            if (result.Length != Count)
            {
                return 0;
            }

            var rectPointSize = result.Take(4)
                .Select(a => a.Size)
                .ToArray();
            var var = 1 - Math.Abs(rectPointSize.Variance() /
                                   rectPointSize.Mean());
            return var;
        }
    }
}