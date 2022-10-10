using System.Linq;
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

        internal override bool CalScore(KeyPoint[] result)
        {
            if (result.Length != Count) return false;
            return true;
        }
    }
}