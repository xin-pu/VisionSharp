using OpenCvSharp;
using VisionSharp.Utils;

namespace VisionSharp.Calibration
{
    /// <summary>
    ///     This is a class for Relative Shift Model
    ///     [du]   =    [ a1, b1 ] *  [dX]
    ///     [dv]        [ a2, b2 ]    [dY]
    /// </summary>
    public class RelativeShift : Calibrator
    {
        private static readonly Size modelSize = new(2, 2);

        public RelativeShift(string name = "Relative Shift")
            : base(modelSize, name)
        {
        }

        #region High Level

        /// <summary>
        ///     When using this model, We will not use Z of wordPoint
        /// </summary>
        /// <param name="wordPoint3ds"></param>
        /// <param name="cameraPoint2ds"></param>
        /// <returns></returns>
        internal override Mat Calibrate(
            Point3d[] wordPoint3ds,
            Point2d[] cameraPoint2ds)
        {
            return Calibrate(wordPoint3ds, cameraPoint2ds);
        }

        internal override Point3d[] PredictWorldCoord(
            Point2d[] cameraPoint2ds)
        {
            return PredictWorldCoord(Model, cameraPoint2ds);
        }

        internal override Point2d[] PredictImageCoord(
            Point3d[] worldPoint3ds)
        {
            return PredictCameraCoord(Model, worldPoint3ds);
        }

        #endregion

        #region Static Method

        public static double[,] CreateVirtualModel(
            double angle,
            double kx,
            double ky)
        {
            return new[,]
            {
                {Math.Cos(angle) * kx, -Math.Sin(angle) * kx},
                {Math.Sin(angle) * ky, Math.Cos(angle) * ky}
            };
        }


        /// <summary>
        ///     [du]   [ a1    a2 ]     [dxw]
        ///     [dv] = [ b1    b2 ]  *  [dyw]
        /// </summary>
        /// <param name="objectPoints">each Point is: {dXw,dYw }</param>
        /// <param name="imagePoints">each Point is: {du,dv }</param>
        /// <returns></returns>
        public static Mat Calibrate(
            IEnumerable<Point3d> objectPoints,
            IEnumerable<Point2d> imagePoints)
        {
            var uvPoints = imagePoints as Point2d[] ?? imagePoints.ToArray();
            var xyzPoint3ds = objectPoints as Point3d[] ?? objectPoints.ToArray();

            if (xyzPoint3ds.Length != uvPoints.Length)
            {
                throw new InvalidDataException();
            }

            var pointWithOutZ = xyzPoint3ds.Select(CvCvt.CutZToPoint2d).ToArray();
            var dW = CvCvt.CvtToMat(pointWithOutZ.ToArray());
            var dC = CvCvt.CvtToMat(uvPoints.ToArray());
            var modelRes = new Mat();
            Cv2.Solve(dW, dC, modelRes, DecompTypes.SVD);
            return modelRes.Transpose();
        }

        /// <summary>
        ///     transform 2*2
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point3d PredictWorldCoord(
            Mat tranform,
            Point2d cameraCoord)
        {
            var final = PredictWorldCoord(tranform, new[] {cameraCoord});
            return final.First();
        }


        /// <summary>
        ///     transform 2*3
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point3d PredictWorldCoord(
            Mat tranform,
            Point2f cameraCoord)
        {
            var p = new Point2d(cameraCoord.X, cameraCoord.Y);
            var final = PredictWorldCoord(tranform, new[] {p});
            return final.First();
        }

        /// <summary>
        ///     Todo Can optimize logic, Xin.Pu
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="cameraPoint2ds"></param>
        /// <returns></returns>
        public static Point3d[] PredictWorldCoord(Mat tranform, Point2d[] cameraPoint2ds)
        {
            var row = cameraPoint2ds.Length;
            var pointN2 = CvCvt.CvtToMat(cameraPoint2ds);

            var point2N = pointN2.Transpose();
            var world = (tranform.Inv() * point2N).ToMat();

            var size = new Size(3, row);
            var res = Mat.Zeros(size, MatType.CV_64F).ToMat();

            res[0, row, 0, 2] = world.Transpose();
            var final = new Mat<double>();
            res.ConvertTo(final, MatType.CV_64F);
            var finalPoint3ds = CvCvt.CvtToPoint3ds(final);
            return finalPoint3ds;
        }


        /// <summary>
        ///     transform 2*2
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point2d[] PredictCameraCoord(Mat transform, Point3d[] wordCoord)
        {
            var size = transform.Size();
            if (size.Width != 2 || size.Height != 2)
            {
                throw new ArgumentException("Mat Size it not Correct for Current Model");
            }

            var dw = wordCoord.Select(CvCvt.CutZToPoint2d).ToArray();

            var world = CvCvt.CvtToMat(dw).Transpose();
            var uv = (transform * world).ToMat().Transpose();
            var final = new Mat<double>();
            uv.ConvertTo(final, MatType.CV_64F);

            return CvCvt.CvtToPoint2ds(final);
        }

        #endregion
    }
}