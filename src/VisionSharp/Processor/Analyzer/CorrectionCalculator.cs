﻿using OpenCvSharp;
using VisionSharp.Models;
using VisionSharp.Models.Detect;
using VisionSharp.Utils;

namespace VisionSharp.Processor.Analyzer
{
    /// <summary>
    ///     根据模板轮廓，现有轮廓，以及旋转轴心，计算需要补偿的角度，和像素偏移
    /// </summary>
    public class CorrectionCalculator<T> : Processor<ObjRotatedrect<T>, AdjustPara> where T : Enum
    {
        public CorrectionCalculator(
            ObjRotatedrect<T> objRotatedrect,
            Point pivot)
            : base("CorrectionCalculator")
        {
            TemplateObject = objRotatedrect;
            Pivot = new Point2f(pivot.X, pivot.Y);
        }

        public ObjRotatedrect<T> TemplateObject { set; get; }

        public Point2f Pivot { set; get; }

        public RotatedRect InputRect { set; get; }

        public RotatedRect PredictRect { set; get; }

        /// <summary>
        ///     Min   Angle: 0
        ///     Max   Angle: 360
        ///     Delta Angle: (-360,360)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal override AdjustPara Process(ObjRotatedrect<T> input)
        {
            var templatePoint = TemplateObject.RotatedRect.Points();


            var deltaAngle = (float) (input.AngleFix - TemplateObject.AngleFix);


            var imageRotatedAngle = deltaAngle > 0
                ? deltaAngle > 180
                    ? deltaAngle - 360
                    : deltaAngle
                : deltaAngle < -180
                    ? deltaAngle + 360
                    : deltaAngle;

            var dutRotatedAngle = -imageRotatedAngle;

            var rotationMat = Cv2.GetRotationMatrix2D(Pivot, imageRotatedAngle, 1);

            var newPoint = input.RotatedRect.Points()
                .Select(a => rotationMat * Mat.FromArray(new double[] {a.X, a.Y, 1}))
                .Select(a => new Point2d(a.ToMat().Get<double>(0, 0), a.ToMat().Get<double>(1, 0)))
                .ToArray();


            var meanTemplatePoint = CvMath.GetMeanPoint2F(templatePoint);
            var meanInsertPoint = CvMath.GetMeanPoint2F(newPoint);

            var shiftX = meanTemplatePoint.X - meanInsertPoint.X;
            var shiftY = meanTemplatePoint.Y - meanInsertPoint.Y;

            var endNewPoints = newPoint
                .Select(a => a + new Point2d(shiftX, shiftY))
                .Select(a => a.ToPoint()).ToArray();

            PredictRect = Cv2.MinAreaRect(endNewPoints);
            InputRect = input.RotatedRect;

            return new AdjustPara
            {
                Angle = dutRotatedAngle,
                XBias = shiftX,
                YBias = shiftY
            };
        }

        internal override Mat Draw(Mat mat, AdjustPara result, bool reliability)
        {
            var rects = new List<RotatedRect>
            {
                TemplateObject.RotatedRect,
                InputRect,
                PredictRect
            };
            DrawPoint(mat, Pivot.ToPoint(), Scalar.OrangeRed, thickness: 5);
            var contours = rects
                .Select(a => Cv2.BoxPoints(a).Select(p => p.ToPoint()))
                .ToList();
            Cv2.DrawContours(mat, contours, -1, PenColor, 2);
            return mat;
        }
    }
}