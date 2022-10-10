using System;
using System.Collections.Generic;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    public class OffsetProcessor
        : Processor<Tuple<Point2d[], Point2d[]>, Point2d>
    {
        public OffsetProcessor()
            : base("OffsetProcessor")
        {
        }

        public Point2d[] OriginalFeatures { set; get; }

        public Point2d[] MovedFeatures { set; get; }

        public Point2d[] AdjustFeaturs { set; get; }

        public double Threshold { set; get; } = 5;

        internal override Point2d Process(Tuple<Point2d[], Point2d[]> input)
        {
            var target = OriginalFeatures = input.Item1;
            var moved = MovedFeatures = input.Item2;

            var deltaX = target.Average(a => a.X) - moved.Average(a => a.X);
            var deltaY = target.Average(a => a.Y) - moved.Average(a => a.Y);

            var res = new Point2d(deltaX, deltaY);
            AdjustFeaturs = MovedFeatures.Select(a => a + res).ToArray();
            return res;
        }

        internal override Mat Draw(Mat grayMat, Point2d result)
        {
            var mat = grayMat.Clone();
            var featureZip = MovedFeatures
                .Zip(AdjustFeaturs, (a, b) => ((Point) a, (Point) b))
                .ToList();

            featureZip.ForEach(valueTuple =>
            {
                var (p1, p2) = (valueTuple.Item1, valueTuple.Item2);
                mat.Circle(p1, 2, Scalar.Black, -1);
                mat.Circle(p2, 2, Scalar.White, -1);
                mat.Line(p1, p2, Scalar.LightGray);
            });

            OriginalFeatures
                .ToList()
                .ForEach(point => { mat.Circle((Point) point, 10, Scalar.White); });

            Cv2.BitwiseNot(mat, mat);
            return mat;
        }

        internal override bool CalScore(Point2d result)
        {
            var temp = OriginalFeatures.ToList();
            var pair = new Dictionary<Point2d, Point2d>();
            foreach (var point2d in AdjustFeaturs)
            {
                var distance = temp
                    .ToDictionary(a => CvMath.GetDistance(a, point2d), a => a);
                var mindis = distance.Keys.Min();
                pair[point2d] = distance[mindis];
                temp.Remove(distance[mindis]);
            }

            var pairDistance = pair
                .Select(a => CvMath.GetDistance(a.Key, a.Value))
                .ToArray();
            return !pairDistance.Any(a => a > Threshold);
        }
    }
}