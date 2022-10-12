using OpenCvSharp;
using VisionSharp.Models.Detect;
using VisionSharp.Utils;

namespace VisionSharp.Processor.Analyzer
{
    /// <summary>
    ///     模块轮廓模板生成器
    /// </summary>
    public class ModuleTemplateFinder : Processor<Mat, ObjRotatedrect>

    {
        /// <summary>
        /// </summary>
        /// <param name="diameter">目标定位圆的直径，单位pixel</param>
        /// <param name="tolerance">允许的范围</param>
        /// <param name="name"></param>
        public ModuleTemplateFinder(
            double diameter = 28,
            double tolerance = 1,
            double rectWith = 1800,
            double rectHeight = 625,
            string name = "ModuleTemplateFinder")
            : base(name)
        {
            Diameter = diameter;
            Tolerance = tolerance;
            var s = new[] {rectWith, rectHeight};
            RectSize = new Size(s.Max(), s.Min());
            ModuleFpFinder = new ModuleFpFinder(Diameter, Tolerance);
        }

        public ModuleFpFinder ModuleFpFinder { set; get; }
        public double Diameter { set; get; }
        public double Tolerance { set; get; }

        public Size RectSize { set; get; }

        public Point2f PS { set; get; }

        public Point2f PE { set; get; }

        internal override ObjRotatedrect Process(Mat input)
        {
            var fps = ModuleFpFinder.Call(input);

            var points = fps
                .Select(a => a.Pt)
                .ToList();

            var rotatedRectPoints = getRectPoint(points);

            var rotatedRect = Cv2.MinAreaRect(rotatedRectPoints);

            PS = rotatedRectPoints[0];
            PE = rotatedRectPoints[2];
            var angle = CvMath.GetAngle(rotatedRectPoints[0], rotatedRectPoints[2]);

            return new ObjRotatedrect(rotatedRect, angle);
        }

        private List<Point2f> getRectPoint(List<Point2f> fps)
        {
            var pairs = new List<(Point2f, Point2f)>();
            fps.ForEach(f1 =>
            {
                fps.ForEach(f2 =>
                {
                    if (f1 != f2 &&
                        !pairs.Contains((f1, f2)) &&
                        !pairs.Contains((f2, f1)))
                    {
                        pairs.Add((f1, f2));
                    }
                });
            });
            var shortSide = pairs.OrderBy(a => CvMath.GetDistance(a.Item1, a.Item2)).First();

            var shortSides = new[] {shortSide.Item1, shortSide.Item2};
            var thirdSide = fps
                .Except(new[] {shortSide.Item1, shortSide.Item2})
                .First();

            var kb = CvMath.Linefit(shortSides);

            var k2 = -1 / kb.Item2;
            var b2 = thirdSide.Y - k2 * thirdSide.X;
            var kb2 = new Tuple<double, double>(b2, k2);

            var x = new[,] {{kb.Item2, -1}, {kb2.Item2, -1}};
            var y = new[,] {{-kb.Item1}, {-kb2.Item1}};

            var X = Mat.FromArray(x);
            var Y = Mat.FromArray(y);
            var modelRes = new Mat();
            Cv2.Solve(X, Y, modelRes, DecompTypes.SVD);
            modelRes.GetArray(out double[] p);

            var topLeft = new Point2f((float) p[0], (float) p[1]);

            var topRight = shortSides.OrderByDescending(a => CvMath.GetDistance(a, topLeft)).First();

            var bottomRight = topRight + thirdSide - topLeft;

            return new List<Point2f> {topRight, topLeft, bottomRight, thirdSide};
        }

        internal override bool GetReliability(ObjRotatedrect result)
        {
            return CheckIsTargetRect(result.RotatedRect);
        }


        private bool CheckIsTargetRect(RotatedRect rotatedRect)
        {
            var s = new double[] {rotatedRect.Size.Width, rotatedRect.Size.Height};
            var w = s.Max();
            var h = s.Min();
            return w > RectSize.Width * 0.9 &&
                   w < RectSize.Width * 1.1 &&
                   h > RectSize.Height * 0.9 &&
                   h < RectSize.Height * 1.1;
        }

        internal override Mat Draw(Mat mat, ObjRotatedrect result, bool reliability)
        {
            mat = DrawRotatedRect(mat, result.RotatedRect, PenColor);
            mat = DrawPoint(mat, PS.ToPoint(), PenColor);
            mat = DrawPoint(mat, PE.ToPoint(), Scalar.White, 10);
            mat = DrawText(mat, result.RotatedRect.Center.ToPoint(), $"Angle:{result.AngleFix:F2}", PenColor, 2, 2);
            return mat;
        }
    }
}