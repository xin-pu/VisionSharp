using OpenCvSharp;
using VisionSharp.Models.Detect;
using VisionSharp.Utils;

namespace VisionSharp.Processor.Analyzer
{
    /// <summary>
    ///     产品占比计算器
    ///     指定图图像大小，格子划分行列数目，计算每个轮廓占用的格子数量
    /// </summary>
    public class DutyCovBoxCounter<T> : Processor<IEnumerable<ObjRotatedrect<T>>, IEnumerable<ObjRotatedrect<T>>>
        where T : Enum
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

        public List<GridRect> GridRects { set; get; }

        internal override IEnumerable<ObjRotatedrect<T>> Process(IEnumerable<ObjRotatedrect<T>> input)
        {
            var dutStructs = (input as ObjRotatedrect<T>[] ?? input.ToArray()).ToList();
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
                //Mark Delete DutyCount
            });
            return dutStructs;
        }
    }
}