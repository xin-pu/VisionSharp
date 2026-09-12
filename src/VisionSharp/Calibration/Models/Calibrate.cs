using OpenCvSharp;
using VisionSharp.Utils;

namespace VisionSharp.Calibration
{
    public class Calibrate
    {
        /// <summary>
        ///     OpenCV Need Z should be 0 when calibration plane
        /// </summary>
        /// <param name="boardSize"></param>
        /// <param name="squareSize"></param>
        /// <returns></returns>
        private static IEnumerable<Point3f> Create3DChessboardCorners(
            Size boardSize,
            float squareSize = 1)
        {
            var res = new List<Point3f>();
            for (var y = 0; y < boardSize.Height; y++)
            for (var x = 0; x < boardSize.Width; x++)
            {
                res.Add(new Point3f(x * squareSize, y * squareSize, 0));
            }

            return res;
        }

        /// <summary>
        ///     Calibrate Camera Matrix by Many subscript target photos at different positions
        /// </summary>
        /// <param name="calFolder">Image folder for calibrate </param>
        /// <param name="patternSize">pattern size</param>
        /// <param name="imageSize"></param>
        /// <param name="pointSpacing">distance for point</param>
        /// <returns>Camera Matrix and distortion factor</returns>
        public static Tuple<Mat, double[]> CalibrateCameraByChessboard(
            string calFolder,
            Size patternSize,
            Size imageSize,
            float pointSpacing = 1f)
        {
            return CalibrateCamera(
                calFolder,
                patternSize,
                imageSize,
                "*.jpg",
                (image, size, corners) => Cv2.FindChessboardCorners(image, size, corners),
                pointSpacing);
        }

        /// <summary>
        ///     Calibrate Camera Matrix by Many subscript target photos at different positions
        /// </summary>
        /// <param name="calFolder">Image folder for calibrate </param>
        /// <param name="patternSize">pattern size</param>
        /// <param name="imageSize"></param>
        /// <param name="pointSpacing">distance for point</param>
        /// <returns>Camera Matrix and distortion factor</returns>
        public static Tuple<Mat, double[]> CalibrateCameraByCirclesGrid(
            string calFolder,
            Size patternSize,
            Size imageSize,
            float pointSpacing = 1f)
        {
            return CalibrateCamera(
                calFolder,
                patternSize,
                imageSize,
                "*.bmp",
                (image, size, corners) => Cv2.FindCirclesGrid(image, size, corners),
                pointSpacing);
        }

        /// <summary>
        ///     相机标定通用流程：遍历标定图片，提取角点后求解内参矩阵与畸变系数
        /// </summary>
        private static Tuple<Mat, double[]> CalibrateCamera(
            string calFolder,
            Size patternSize,
            Size imageSize,
            string searchPattern,
            Func<Mat, Size, Mat<Point2f>, bool> findCorners,
            float pointSpacing)
        {
            var files = Directory.CreateDirectory(calFolder).GetFiles(searchPattern);
            var objectPoints = new List<IEnumerable<Point3f>>();
            var imagePoints = new List<IEnumerable<Point2f>>();

            foreach (var fileInfo in files)
            {
                using var image = Cv2.ImRead(fileInfo.FullName, ImreadModes.Grayscale);
                using var corners = new Mat<Point2f>();
                if (!findCorners(image, patternSize, corners))
                {
                    continue;
                }

                objectPoints.Add(Create3DChessboardCorners(patternSize, pointSpacing));
                imagePoints.Add(corners.ToArray());
            }

            var cameraMatrix = new double[3, 3];
            var distCoeffs = new double[5];
            Cv2.CalibrateCamera(
                objectPoints.ToArray(),
                imagePoints.ToArray(),
                imageSize,
                cameraMatrix,
                distCoeffs,
                out _,
                out _,
                CalibrationFlags.FixK5);

            return new Tuple<Mat, double[]>(Mat.FromArray(cameraMatrix), distCoeffs);
        }

        /// <summary>
        ///     After cal Internal Camera Matrix
        ///     Cal External Matrix by Provide Camera Matrix and series
        /// </summary>
        /// <param name="objectPoints"></param>
        /// <param name="imagePoints"></param>
        /// <param name="cameraMat"></param>
        /// <param name="distCoff"></param>
        /// <returns></returns>
        public static Mat CalExternal(
            IEnumerable<Point3f> objectPoints,
            IEnumerable<Point2f> imagePoints,
            double[,] cameraMat,
            double[] distCoff)
        {
            var rvec = new double[3];
            var tvec = new double[3];
            Cv2.SolvePnP(objectPoints, imagePoints, cameraMat, distCoff, ref rvec, ref tvec);
            Cv2.Rodrigues(rvec, out var mat, out _);

            using var rotation = Mat.FromArray(mat);
            using var translation = Mat.FromArray(tvec);
            var RT = new Mat(new Size(4, 3), MatType.CV_64F)
            {
                [0, 3, 0, 3] = rotation,
                [0, 3, 3, 4] = translation
            };

            return RT;
        }

        /// <summary>
        ///     Cal Integrated Matrix
        /// </summary>
        /// <param name="objectPoints"></param>
        /// <param name="imagePoints"></param>
        /// <param name="cameraMat"></param>
        /// <param name="distCoff"></param>
        /// <returns></returns>
        public static Mat CalIntegrated(
            IEnumerable<Point3f> objectPoints,
            IEnumerable<Point2f> imagePoints,
            double[,] cameraMat,
            double[] distCoff)
        {
            using var rt = CalExternal(objectPoints, imagePoints, cameraMat, distCoff);
            using var c = Mat.FromArray(cameraMat);
            return (c * rt).ToMat();
        }

        public static Point3d Predict(
            Mat transform,
            Point2d imageUv,
            double z)
        {
            var bRaw = CvCvt.CvtToMat(new[] {imageUv});
            using var b = bRaw.Transpose();
            bRaw.Dispose();
            using var m01 = transform[0, 2, 0, 2];
            using var m23 = transform[0, 2, 2, 4];
            using var p = Mat.FromArray(new[,] {{z}, {1}});
            using var d = (b - m23 * p).ToMat();
            using var a = new Mat();
            Cv2.Solve(m01, d, a, DecompTypes.SVD);
            return new Point3d(a.At<double>(0, 0), a.At<double>(1, 0), z);
        }
    }
}
