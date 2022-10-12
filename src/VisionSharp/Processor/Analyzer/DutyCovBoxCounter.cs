using OpenCvSharp;
using VisionSharp.Models.Detect;
using VisionSharp.Utils;

namespace VisionSharp.Processor.Analyzer
{
    /// <summary>
    ///     产品占比计算器
    ///     指定图图像大小，格子划分行列数目，计算每个轮廓占用的格子数量
    /// </summary>
    public class DutyCovBoxCounter
        : Processor<IEnumerable<DetectObject>, IEnumerable<DetectObject>>
    {
        public DutyCovBoxCounter(
            Size gridSize,
            Size matSize)
            : base("DutyCovBoxCounter")
        {
            GridSize = gridSize;
            MatSize = matSize;
            GridRects = CvBasic.GetDetectRects(MatSize, GridSize);
        }


        public Size MatSize { get; }
        public Size GridSize { get; }

        public List<DetectGridRect> GridRects { set; get; }

        internal override IEnumerable<DetectObject> Process(IEnumerable<DetectObject> input)
        {
            var dutStructs = (input as DetectObject[] ?? input.ToArray()).ToList();
            dutStructs.ForEach(a =>
            {
                var count = GridRects
                    .Count(box =>
                    {
                        var rect = box.Rect;
                        var center = CvMath.GetMeanPoint2F(rect);
                        var size = new Size2f(rect.Size.Width, rect.Size.Height);
                        var rotatdRect = new RotatedRect(center, size, 0);
                        return Cv2.RotatedRectangleIntersection(rotatdRect, a.RotatedRect, out _) !=
                               RectanglesIntersectTypes.None;
                    });
                a.DutyCount = count;
            });
            return dutStructs;
        }
    }
}