using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Calibration
{
    /// <summary>
    ///     This is a class for Nine Point Model
    ///     [u]     [ a1, a2, a3 ]  [X]
    ///     [v] =   [ b1, b2, b3 ]* |Y|
    ///     ----------------------- [1]
    /// </summary>
    public class NinePoint : Calibrator
    {
        private static readonly Size modelSize = new(3, 2);

        public NinePoint(string name = "Nine Point")
            : base(modelSize, name)
        {
        }


        #region High Level

        /// <summary>
        ///     When using this model, all Z of xyz point3d should be 1.
        /// </summary>
        /// <param name="wordPoint3ds"></param>
        /// <param name="cameraPoint2ds"></param>
        /// <returns></returns>
        internal override Mat Calibrate(
            Point3d[] worldPoint3ds,
            Point2d[] cameraPoint2ds)
        {
            return Calibrate(worldPoint3ds, cameraPoint2ds);
        }

        internal override Point3d[] PredictWorldCoord(
            Point2d[] cameraPoint2ds)
        {
            return PredictWorldCoords(Model, cameraPoint2ds);
        }

        internal override Point2d[] PredictImageCoord(
            Point3d[] worldPoint3ds)
        {
            return PredictCameraCoords(Model, worldPoint3ds);
        }

        #endregion

        #region Static Method

        /// <summary>
        ///     Create a Virtual Model for Nine Point Method [Cos -Sin Px] [Sin Cos Py ]
        /// </summary>
        /// <param name="angle">Angle for Rotate</param>
        /// <param name="shiftx">Px</param>
        /// <param name="shifty">Py</param>
        /// <returns></returns>
        public static double[,] CreateVirtualModel(
            double angle,
            double shiftx,
            double shifty)
        {
            return new[,]
            {
                {Math.Cos(angle), -Math.Sin(angle), shiftx},
                {Math.Sin(angle), Math.Cos(angle), shifty}
            };
        }


        /// <summary>
        ///     solve X * A = B for Nine Point Function
        ///     X * A = B
        ///     At * Xt = Bt
        ///     Xt = solve (At,Bt)
        ///     X= (Xt)t
        /// </summary>
        /// <param name="objectPoints">XYZ position under Word Coordinate</param>
        /// <param name="imagePoints">uv position on image Coordinate</param>
        /// <returns>transform which size is (2,3)</returns>
        public static Mat Calibrate(
            IEnumerable<Point3d> objectPoints,
            IEnumerable<Point2d> imagePoints)
        {
            var uvPoints = imagePoints as Point2d[] ?? imagePoints.ToArray();
            var xyzPoint3ds = objectPoints as Point3d[] ?? objectPoints.ToArray();
            if (xyzPoint3ds.Length != uvPoints.Length) throw new InvalidDataException();

            xyzPoint3ds.ToList().ForEach(a => a.Z = 1);
            var dW = CvCvt.CvtToMat(xyzPoint3ds);
            var dC = CvCvt.CvtToMat(uvPoints);
            var modelRes = new Mat();
            Cv2.Solve(dW, dC, modelRes, DecompTypes.SVD);
            return modelRes.Transpose();
        }


        /// <summary>
        ///     transform 2*3
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point3d PredictWorldCoord(
            Mat tranform,
            Point2d cameraCoord)
        {
            var B = CvCvt.CvtToMat(new[] {cameraCoord}).Transpose();
            var XR = tranform[new Range(0, 2), new Range(0, 2)];
            var XT = tranform[new Range(0, 2), new Range(2, 3)];

            var D = (B - XT).ToMat();

            var A = new Mat();
            Cv2.Solve(XR, D, A, DecompTypes.SVD);
            var point = new Point3d(A.At<double>(0, 0), A.At<double>(1, 0), 1);
            return point;
        }

        public static Point3d PredictWorldCoord(
            Mat tranform,
            Point2f cameraCoord)
        {
            var p = new Point2d(cameraCoord.X, cameraCoord.Y);
            return PredictWorldCoord(tranform, p);
        }

        /// <summary>
        ///     transform 2*3
        ///     Todo Can optimize logic, Xin.Pu
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point3d[] PredictWorldCoords(
            Mat transform,
            Point2d[] cameraCoords)
        {
            return cameraCoords
                .Select(a => PredictWorldCoord(transform, a))
                .ToArray();
        }

        /// <summary>
        ///     transform 2*3
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point3d[] PredictWorldCoords(
            Mat transform,
            Point2f[] cameraCoord)
        {
            return cameraCoord
                .Select(a => PredictWorldCoord(transform, a))
                .ToArray();
        }


        /// <summary>
        ///     transform 2*3
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point2d[] PredictCameraCoords(
            Mat transform,
            Point3d[] wordCoord)
        {
            var size = transform.Size();
            if (size.Width != 3 || size.Height != 2)
                throw new ArgumentException("Mat Size it not Correct for Current Model");

            wordCoord.ToList().ForEach(a => a.Z = 1);
            var world = CvCvt.CvtToMat(wordCoord).Transpose();
            var uv = (transform * world).ToMat().Transpose();
            var matOut = new Mat<double>();
            uv.ConvertTo(matOut, MatType.CV_64F);

            return CvCvt.CvtToPoint2ds(matOut);
        }

        /// <summary>
        ///     transform 2*3
        /// </summary>
        /// <param name="tranform"></param>
        /// <param name="imageUV"></param>
        /// <returns></returns>
        public static Point2d[] PredictCameraCoords(
            Mat transform,
            Point3f[] wordCoord)
        {
            return PredictCameraCoords(transform, CvCvt.CvtToPoint3ds(wordCoord));
        }

        #endregion
    }
}