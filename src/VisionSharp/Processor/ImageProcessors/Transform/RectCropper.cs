using OpenCvSharp;
using VisionSharp.Utils;

namespace VisionSharp.Processor.Transform
{
    /// <summary>
    ///     Rect 裁剪器
    /// </summary>
    public class RectCropper : RotatedRectCropper
    {
        public RectCropper(Rect rect)
            : base(CvCvt.CvtToRotatedRect(rect))
        {
            Name = "RectCropper";
        }
    }
}