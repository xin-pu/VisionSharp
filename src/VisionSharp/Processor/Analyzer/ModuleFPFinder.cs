using OpenCvSharp;
using VisionSharp.Processor.FeatureExtractors;

namespace VisionSharp.Processor.Analyzer
{
    /// <summary>
    ///     模块定位圆寻找器
    /// </summary>
    public class ModuleFpFinder : Processor<Mat, List<KeyPoint>>
    {
        /// <summary>
        /// </summary>
        /// <param name="diameter">目标定位圆的直径，单位pixel</param>
        /// <param name="tolerance">允许的范围</param>
        /// <param name="name"></param>
        public ModuleFpFinder(
            double diameter = 28,
            double tolerance = 1,
            int hitmissSize = 2,
            int open_size = 7,
            string name = "ModuleFPFinder")
            : base(name)
        {
            Diameter = diameter;
            Tolerance = tolerance;
            HitmissSize = hitmissSize;
            OpenSize = open_size;
        }

        public double Diameter { set; get; }
        public double Tolerance { set; get; }
        public int HitmissSize { set; get; }
        public int OpenSize { set; get; }

        /// <summary>
        ///     Otsu 二值化
        ///     击中击不中操作
        ///     闭运算
        ///     半点检测
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal override List<KeyPoint> Process(Mat input)
        {
            var temp = new Mat();
            Cv2.Threshold(input.Clone(), temp, 200, 255, ThresholdTypes.Otsu);

            var element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse,
                new Size(HitmissSize, HitmissSize),
                new Point(-1, -1));

            var element2 = Cv2.GetStructuringElement(MorphShapes.Ellipse,
                new Size(OpenSize, OpenSize),
                new Point(-1, -1));

            Cv2.MorphologyEx(temp, temp, MorphTypes.HitMiss, element1, new Point(-1, -1));

            Cv2.MorphologyEx(temp, temp, MorphTypes.Open, element2, new Point(-1, -1));

            var circleFinder = new CircleDetector
            {
                BlobColor = 255,
                FilterByCircularity = true,
                MinCircularity = 0.85,
                MaxCircularity = 1,

                FilterByArea = false
            };


            var res = circleFinder
                .Call(temp);

            var finalres = res
                .Where(a => a.Size >= Diameter - Tolerance &&
                            a.Size <= Diameter + Tolerance)
                .ToList();

            return finalres;
        }

        /// <summary>
        ///     绘制特征点
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, List<KeyPoint> result, bool reliability)
        {
            if (result == null)
            {
                return mat;
            }

            foreach (var keyPoint in result)
            {
                var p = new Point((int) keyPoint.Pt.X, (int) keyPoint.Pt.Y);
                mat.Line(p + new Point(-50, 0), p + new Point(50, 0), PenColor, 3);
                mat.Line(p + new Point(0, 50), p + new Point(0, -50), PenColor, 3);
                mat.PutText($"{p.X:D}", p + new Point(100, 0), HersheyFonts.HersheyPlain, 2, PenColor);
                mat.PutText($"{p.Y:D}", p + new Point(100, 30), HersheyFonts.HersheyPlain, 2, PenColor);
            }

            return mat;
        }
    }
}