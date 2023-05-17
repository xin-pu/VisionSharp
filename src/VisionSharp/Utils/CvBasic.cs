using Numpy;
using OpenCvSharp;
using VisionSharp.Models.Detect;

namespace VisionSharp.Utils
{
    public class CvBasic
    {
        /// <summary>
        ///     将图像划分为棋盘格
        /// </summary>
        /// <param name="matsize"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public static List<GridRect> GetDetectRects(Size matsize, Size gridSize)
        {
            var gridWidth = matsize.Width / gridSize.Width;
            var gridHeight = matsize.Height / gridSize.Height;
            var gridRects = new List<GridRect>();

            foreach (var row in Enumerable.Range(0, gridSize.Height))
            foreach (var column in Enumerable.Range(0, gridSize.Width))
            {
                var x = column * gridWidth;
                var y = row * gridHeight;
                var rect = new Rect(x, y, gridWidth, gridHeight);
                gridRects.Add(new GridRect(row + 1, column + 1, rect)
                {
                    IsEmpty = true
                });
            }

            return gridRects;
        }


        /// <summary>
        ///     从图像中截取感兴趣区域
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


        public static unsafe void Sigmoid(Mat mat)
        {
            void SigmoidForeach(double* v, int* position)
            {
                *v = 1 / (1 + Math.Exp(-*v));
            }

            mat.ForEachAsDouble(SigmoidForeach);
        }


        public static NDarray CreateGridX(int width, int height, int layer)
        {
            var basic = np.linspace(0, width - 1, width, dtype: np.float32);
            var final = basic
                .expand_dims(0)
                .repeat(new[] {height}, 0)
                .expand_dims(0)
                .repeat(new[] {layer}, 0);
            return final;
        }


        public static NDarray CreateGridY(int width, int height, int layer)
        {
            var basic = np.linspace(0, height - 1, height, dtype: np.float32);
            var final = basic
                .expand_dims(1)
                .repeat(new[] {width}, 1)
                .expand_dims(0)
                .repeat(new[] {layer}, 0);
            return final;
        }
    }
}