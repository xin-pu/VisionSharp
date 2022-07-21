using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     寻找圆的处理器
    /// </summary>
    public class CircleFinder
        : Processor<Mat, KeyPoint[]>
    {
        public CircleFinder()
            : base("CircleFinder")
        {
        }


        public bool FilterByArea { set; get; } = false;
        public double MinArea { set; get; } = 0;
        public double MaxArea { set; get; } = 0;

        public bool FilterByCircularity { set; get; } = true;
        public double MinCircularity { set; get; } = 0.8;
        public double MaxCircularity { set; get; } = 1;

        public byte BlobColor { set; get; } = 0;
        public double MinDistBetweenBlobs { set; get; }


        internal override KeyPoint[] Process(Mat input)
        {
            /// Step 3 Set Cumstom Filter
            var paras = new SimpleBlobDetector.Params
            {
                /// 像素面积控制
                FilterByArea = FilterByArea,
                MinArea = (float) MinArea,
                MaxArea = (float) MaxArea,

                FilterByCircularity = FilterByCircularity,
                MinCircularity = (float) MinCircularity,
                MaxCircularity = (float) MaxCircularity,

                FilterByColor = true,
                BlobColor = BlobColor,
                MinDistBetweenBlobs = (float) MinDistBetweenBlobs,

                FilterByInertia = false,
                FilterByConvexity = false
            };
            var simpleBlob = SimpleBlobDetector.Create(paras);

            var key = simpleBlob.Detect(input);
            return key;
        }

        internal override Mat Draw(Mat grayInMat, KeyPoint[] keyPoints)
        {
            if (keyPoints == null) return grayInMat;

            foreach (var keyPoint in keyPoints)
            {
                var p = new Point((int) keyPoint.Pt.X, (int) keyPoint.Pt.Y);
                grayInMat.Line(p + new Point(-50, 0), p + new Point(50, 0), PenColor);
                grayInMat.Line(p + new Point(0, 50), p + new Point(0, -50), PenColor);
                grayInMat.PutText($"{p.X:D}", p + new Point(100, 0), HersheyFonts.HersheyPlain, 2, PenColor);
                grayInMat.PutText($"{p.Y:D}", p + new Point(100, 30), HersheyFonts.HersheyPlain, 2, PenColor);
            }

            return grayInMat;
        }
    }
}