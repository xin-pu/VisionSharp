using OpenCvSharp;
using VisionSharp.Models.Detect;
using Size = System.Drawing.Size;

namespace VisionSharp.Utils
{
    public class CvBasic
    {
        /// <summary>
        ///     Get Detect Grid Size from size of mat, and target pattern size.
        /// </summary>
        /// <param name="matsize"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public static List<DetectGridRect> GetDetectRects(Size matsize, Size gridSize)
        {
            var gridWidth = matsize.Width / gridSize.Width;
            var gridHeight = matsize.Height / gridSize.Height;
            var gridRects = new List<DetectGridRect>();

            foreach (var row in Enumerable.Range(0, gridSize.Height))
            foreach (var column in Enumerable.Range(0, gridSize.Width))
            {
                var x = column * gridWidth;
                var y = row * gridHeight;
                var rect = new Rect(x, y, gridWidth, gridHeight);
                gridRects.Add(new DetectGridRect(row + 1, column + 1, rect)
                {
                    IsEmpty = true
                });
            }

            return gridRects;
        }


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