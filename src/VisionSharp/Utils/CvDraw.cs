using OpenCvSharp;
using VisionSharp.Models.Base;

namespace VisionSharp.Utils
{
    /// <summary>
    ///     CV绘画拓展类
    /// </summary>
    public class CvDraw
    {
        /// <summary>
        ///     绘制点
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="point"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawPoint(Mat mat, Point point, Scalar color, int size = 20, int thickness = 3)
        {
            mat.Line(point + new Point(-size, 0), point + new Point(size, 0), color, thickness);
            mat.Line(point + new Point(0, size), point + new Point(0, -size), color, thickness);
            return mat;
        }

        /// <summary>
        ///     绘制线
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="pointStart"></param>
        /// <param name="pointEnd"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawLine(Mat mat, Point pointStart, Point pointEnd, Scalar color, int thickness = 3)
        {
            mat.Line(pointStart, pointEnd, color, thickness);
            return mat;
        }

        /// <summary>
        ///     绘制矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawRect(Mat mat, Rect rect, Scalar color, int size = 10, int thickness = 3)
        {
            var topRight = new Point(rect.Right, rect.Top);
            var bottomLeft = new Point(rect.Left, rect.Bottom);
            var points = new[] {rect.TopLeft, rect.BottomRight, topRight, bottomLeft};
            var rotatcedRect = Cv2.MinAreaRect(points);
            return DrawRotatedRect(mat, rotatcedRect, color, size);
        }

        /// <summary>
        ///     绘制矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawRect(Mat mat, CvRect rect, Scalar color, int size = 10, int thickness = 3)
        {
            return DrawRect(mat, rect.Rect, color, size, thickness);
        }

        /// <summary>
        ///     绘制旋转矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rotatedRect"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawRotatedRect(Mat mat, RotatedRect rotatedRect, Scalar color, int thickness = 3)
        {
            var points = Cv2.BoxPoints(rotatedRect).ToList();
            var cons = points.Select(a => a.ToPoint());
            Cv2.DrawContours(mat, new[] {cons}, -1, color, thickness);

            points.Add(rotatedRect.Center.ToPoint());
            return mat;
        }

        /// <summary>
        ///     绘制旋转矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rotatedRect"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawRotatedRect(Mat mat, CvRotatedRect rotatedRect, Scalar color, int thickness = 3)
        {
            return DrawRotatedRect(mat, rotatedRect.RotatedRect, color, thickness);
        }

        /// <summary>
        ///     绘制文字
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="point"></param>
        /// <param name="info"></param>
        /// <param name="color"></param>
        /// <param name="fontScale"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Mat DrawText(Mat mat, Point point, string info, Scalar color, double fontScale = 1,
            int thickness = 1)
        {
            var size = Cv2.GetTextSize(info, HersheyFonts.HersheyPlain, fontScale, thickness, out _);

            var newpoint = point + new Point(0, size.Height);

            var rectSize = new Size(size.Width, size.Height * 1.2);
            Cv2.Rectangle(mat, new Rect(point, rectSize), color, -1);
            Cv2.PutText(mat, info, newpoint, HersheyFonts.HersheyPlain,
                fontScale,
                new Scalar(255, 255, 255),
                thickness);
            return mat;
        }
    }
}