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
        /// <param name="patterSize">patter size</param>
        /// <param name="pointSpacing">distance for point</param>
        /// <returns>Camera Matrix and distortion factor</returns>
        public static Tuple<Mat, double[]> CalibrateCameraByChessboard(
            string calFolder,
            Size patterSize,
            Size imageSize,
            float pointSpacing = 1f)
        {
            var files = Directory.CreateDirectory(calFolder).GetFiles("*.jpg");
            var objectPoints = new List<IEnumerable<Point3f>>();
            var imagePoints = new List<IEnumerable<Point2f>>();

            foreach (var fileInfo in files)
            {
                var image = Cv2.ImRead(fileInfo.FullName, ImreadModes.Grayscale);
                var corners = new Mat<Point2f>();
                var res = Cv2.FindChessboardCorners(image, patterSize, corners);

                if (!res)
                {
                    continue;
                }

                objectPoints.Add(Create3DChessboardCorners(patterSize, pointSpacing));
                imagePoints.Add(corners.ToArray());
            }

            var objArray = objectPoints.ToArray();
            var imgArray = imagePoints.ToArray();

            var cameraMatrix = new double[3, 3];
            var distCoeffs = new double[5];
            Cv2.CalibrateCamera(
                objArray,
                imgArray,
                imageSize,
                cameraMatrix,
                distCoeffs,
                out _,
                out _,
                CalibrationFlags.FixK5);

            var cameraMat = Mat.FromArray(cameraMatrix);

            return new Tuple<Mat, double[]>(cameraMat, distCoeffs);
        }


        /// <summary>
        ///     Calibrate Camera Matrix by Many subscript target photos at different positions
        /// </summary>
        /// <param name="calFolder">Image folder for calibrate </param>
        /// <param name="patterSize">patter size</param>
        /// <param name="pointSpacing">distance for point</param>
        /// <returns>Camera Matrix and distortion factor</returns>
        public static Tuple<Mat, double[]> CalibrateCameraByCirclesGrid(
            string calFolder,
            Size patterSize,
            Size imageSize,
            float pointSpacing = 1f)
        {
            var files = Directory.CreateDirectory(calFolder).GetFiles("*.bmp");
            var objectPoints = new List<IEnumerable<Point3f>>();
            var imagePoints = new List<IEnumerable<Point2f>>();

            foreach (var fileInfo in files)
            {
                var image = Cv2.ImRead(fileInfo.FullName, ImreadModes.Grayscale);
                var corners = new Mat<Point2f>();
                var res = Cv2.FindCirclesGrid(image, patterSize, corners);

                if (!res)
                {
                    continue;
                }

                objectPoints.Add(Create3DChessboardCorners(patterSize, pointSpacing));
                imagePoints.Add(corners.ToArray());
            }

            var objArray = objectPoints.ToArray();
            var imgArray = imagePoints.ToArray();

            var cameraMatrix = new double[3, 3];
            var distCoeffs = new double[5];
            Cv2.CalibrateCamera(
                objArray,
                imgArray,
                imageSize,
                cameraMatrix,
                distCoeffs,
                out _,
                out _,
                CalibrationFlags.FixK5);

            var cameraMat = Mat.FromArray(cameraMatrix);

            return new Tuple<Mat, double[]>(cameraMat, distCoeffs);
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

            var size = new Size(4, 3);
            var RT = new Mat(size, MatType.CV_64F)
            {
                [0, 3, 0, 3] = Mat.FromArray(mat),
                [0, 3, 3, 4] = Mat.FromArray(tvec)
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
            var rt = CalExternal(objectPoints, imagePoints, cameraMat, distCoff);
            var C = Mat.FromArray(cameraMat);
            var M = (C * rt).ToMat();
            return M;
        }

        public static Point3d Predict(
            Mat tranform,
            Point2d imageUv,
            double z)
        {
            var B = CvCvt.CvtToMat(new[] {imageUv}).Transpose();
            var M01 = tranform[0, 2, 0, 2];
            var M23 = tranform[0, 2, 2, 4];
            var P = Mat.FromArray(new[,] {{z}, {1}});
            var D = (B - M23 * P).ToMat();
            var A = new Mat();
            Cv2.Solve(M01, D, A, DecompTypes.SVD);
            var point = new Point3d(A.At<double>(0, 0), A.At<double>(1, 0), z);
            return point;
        }
    }
}