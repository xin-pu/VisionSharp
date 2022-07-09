using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     旋转区域 裁剪器
    /// </summary>
    public class RotatedRectCropper
        : Processor<Mat, Mat>
    {
        public RotatedRectCropper(RotatedRect rect)
            : base("RotatedRectCropper")
        {
            RotatedRect = rect;
        }

        public RotatedRect RotatedRect { set; get; }

        internal override Mat Process(Mat input)
        {
            if (RotatedRect == new RotatedRect()) return input;

            /// Step 1 旋转
            var needResver = RotatedRect.Size.Width < RotatedRect.Size.Height;

            var angle = needResver
                ? RotatedRect.Angle - 90
                : RotatedRect.Angle;
            var rotMat = Cv2.GetRotationMatrix2D(RotatedRect.Center, angle, 1);
            var tempMat = new Mat();
            Cv2.WarpAffine(input, tempMat, rotMat, input.Size());

            var centerX = RotatedRect.Center.X;
            var centerY = RotatedRect.Center.Y;
            var width = needResver ? RotatedRect.Size.Height : RotatedRect.Size.Width;
            var height = needResver ? RotatedRect.Size.Width : RotatedRect.Size.Height;

            var newCenter = new Point(centerX - width / 2, centerY - height / 2);

            var newSize = new Size(width, height);

            return CvBasic.GetRectMat(tempMat, new Rect(newCenter, newSize));
        }
    }
}