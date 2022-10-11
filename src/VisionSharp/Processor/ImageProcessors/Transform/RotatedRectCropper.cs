using OpenCvSharp;
using VisionSharp.Models.Base;

namespace VisionSharp.Processor.Transform
{
    /// <summary>
    ///     旋转区域 裁剪器
    /// </summary>
    public class RotatedRectCropper : ImageProcessor
    {
        public RotatedRectCropper(RotatedRect rect)
            : base("RotatedRectCropper")
        {
            RotatedRect = rect;
        }

        public RotatedRectCropper(CvRotatedRect rect)
            : base("RotatedRectCropper")
        {
            RotatedRect = rect.RotatedRect;
            Horizon = rect.Horizontal;
        }

        public RotatedRect RotatedRect { set; get; }
        public bool Horizon { set; get; } = true;

        internal override Mat Process(Mat input)
        {
            if (RotatedRect == new RotatedRect())
            {
                return input;
            }

            var width = RotatedRect.Size.Width;
            var height = RotatedRect.Size.Height;
            var size = new List<float> {width, height};
            width = Horizon ? size.Max() : size.Min();
            height = Horizon ? size.Min() : size.Max();

            var points = RotatedRect.Points()
                .OrderBy(a => a.X)
                .ThenBy(a => a.Y).ToArray();

            var m = Cv2.GetPerspectiveTransform(points,
                new List<Point2f>
                {
                    new(0, 0),
                    new(0, height - 1),
                    new(width - 1, 0),
                    new(width - 1, height - 1)
                });
            var outmat = new Mat();
            Cv2.WarpPerspective(input, outmat, m, new Size(width, height));

            return outmat;
        }
    }
}