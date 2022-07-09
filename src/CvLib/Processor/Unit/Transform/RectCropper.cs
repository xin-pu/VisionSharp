using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Processor.Unit
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