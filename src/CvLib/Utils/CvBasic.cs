using OpenCvSharp;

namespace CVLib.Utils
{
    public class CvBasic
    {
        /// <summary>
        ///     Get Mat from Original Mat by ROI Rect
        /// </summary>
        /// <param name="mat">Original Mat</param>
        /// <param name="roiRect">Roi Rect</param>
        /// <returns></returns>
        public static Mat GetRectMat(
            Mat mat,
            Rect roiRect)
        {
            var left = roiRect.X;
            var right = roiRect.X + roiRect.Width;
            var top = roiRect.Y;
            var bottom = roiRect.Y + roiRect.Height;

            var xmin = left >= 0 && left < mat.Width
                ? left
                : left < 0
                    ? 0
                    : mat.Width - 1;
            var xmax = right >= 0 && right < mat.Width
                ? right
                : right < 0
                    ? 0
                    : mat.Width - 1;

            var ymin = top >= 0 && top < mat.Height
                ? top
                : top < 0
                    ? 0
                    : mat.Height - 1;

            var ymax = bottom >= 0 && bottom < mat.Height
                ? bottom
                : bottom < 0
                    ? 0
                    : mat.Height - 1;
            var newRect = new Rect(xmin, ymin, xmax - xmin, ymax - ymin);

            return mat[newRect];
        }
    }
}