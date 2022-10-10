using System.Collections.Generic;
using System.Linq;
using CVLib.Models;
using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     预分割格子，计算每个格子的占空比
    /// </summary>
    public class DutyCovObjCounter
        : Processor<IEnumerable<RotatedRect>, List<DetectGridRect>>
    {
        public DutyCovObjCounter(
            Size gridSize,
            Size matSize)
            : base("DutyCovObjCounter")
        {
            GridSize = gridSize;
            MatSize = matSize;
            GridWidth = matSize.Width / gridSize.Width;
            GridHeight = matSize.Height / gridSize.Height;
            GridRects = CvBasic.GetDetectRects(MatSize, GridSize);
        }

        public Size MatSize { get; }
        public Size GridSize { get; }
        public int GridWidth { set; get; }
        public int GridHeight { set; get; }
        public List<DetectGridRect> GridRects { set; get; }


        internal override List<DetectGridRect> Process(IEnumerable<RotatedRect> input)
        {
            var area = GridWidth * GridHeight;
            var rectBoxes = GridRects
                .Where(a => a.IsEmpty)
                .ToList();

            var rotatedRects = input as RotatedRect[] ?? input.ToArray();

            rectBoxes.ForEach(a =>
            {
                var rect = a.Rect;
                var center = new Point2f((rect.Left + rect.Right) / 2f,
                    (rect.Top + rect.Bottom) / 2f);
                var size = new Size2f(rect.Size.Width * 1.1, rect.Size.Height * 1.1);
                var rotatdRect = new RotatedRect(center, size, 0);
                a.ObjectScale = rotatedRects.Select(i =>
                {
                    Cv2.RotatedRectangleIntersection(i, rotatdRect, out var p);
                    return Cv2.ContourArea(p) / area;
                }).Sum();
                a.DutyRect = rotatedRects
                    .Where(i =>
                        Cv2.RotatedRectangleIntersection(i, rotatdRect, out _) != RectanglesIntersectTypes.None)
                    .ToList();
                a.DutyCount = a.DutyRect.Count;
            });

            return rectBoxes;
        }


        internal override Mat Draw(Mat mat, List<DetectGridRect> detectGridRects)
        {
            detectGridRects.ForEach(rect => { Cv2.Rectangle(mat, rect.Rect, PenColor, -1); });
            return mat;
        }
    }
}