using OpenCvSharp;

namespace VisionSharp.Utils
{
    public class CvIou
    {
        public static double GetIoU(Rect rect1, Rect rect2)
        {
            var area1 = 1.0 * rect1.Width * rect1.Height;
            var area2 = 1.0 * rect2.Width * rect2.Height;

            var top = new[] {rect1.Top, rect2.Top}.Max();
            var left = new[] {rect1.Left, rect2.Left}.Max();

            var bottom = new[] {rect1.Bottom, rect2.Bottom}.Min();
            var right = new[] {rect1.Right, rect2.Right}.Min();

            if (left > right || top > bottom)
            {
                return -1;
            }

            var areaIn = 1.0 * (right - left) * (bottom - top);

            var areaUnion = area1 + area2 - areaIn;

            return areaIn / areaUnion;
        }
    }
}