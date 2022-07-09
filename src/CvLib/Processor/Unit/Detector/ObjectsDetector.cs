using System.Collections.Generic;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     目标探测器，多用于长方形物体的探测
    /// </summary>
    public class ObjectsDetector
        : Processor<Mat, List<DetectObject>>
    {
        public enum ThresholdType
        {
            Triangle,
            Otsu
        }

        public ObjectsDetector(
            Size targetSize,
            double aspectRatio,
            double thresholdRatio = 0.2,
            string name = "ObjectsDetector")
            : base(name)
        {
            TargetSize = targetSize;
            AspectRatio = aspectRatio;

            ThresholdRatio = thresholdRatio;
        }

        public Size TargetSize { set; get; }
        public double AspectRatio { set; get; }

        public double ThresholdRatio { set; get; }

        internal override List<DetectObject> Process(Mat input)
        {
            var c = new VideoCapture();
            /// Step 1 寻找旋转轮廓
            var rotatedRects = FindRect(input.Clone());

            /// Step 2 更新轮廓是模块的置信度
            rotatedRects.ForEach(a => a.IsConfidence = IsAccordWithTarget(a.RotatedRect));

            return rotatedRects;
        }


        /// <summary>
        ///     根据设置阈值获取产品轮廓
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private List<DetectObject> FindRect(Mat mat)
        {
            var Hierarchy = new Mat();
            Cv2.FindContours(mat, out var filtercons, Hierarchy, RetrievalModes.List,
                ContourApproximationModes.ApproxSimple);

            /// Step 1 去除特小轮廓
            filtercons = filtercons
                .Where(a =>
                {
                    var rotatedRect = Cv2.MinAreaRect(a);
                    var rect_area = rotatedRect.Size.Width * rotatedRect.Size.Height;
                    return rect_area >= 80 * 100;
                }).ToArray();

            /// Step 2 转输出结果
            var moduleStructs = filtercons
                .Select(a => new DetectObject(Cv2.MinAreaRect(a)))
                .ToList();

            return moduleStructs;
        }


        /// <summary>
        ///     用于判别是否符合目标一般特征
        ///     用面积加上长宽比
        /// </summary>
        /// <param name="rotatedRect"></param>
        /// <returns></returns>
        private bool IsAccordWithTarget(RotatedRect rotatedRect)
        {
            var minThreshold = 1 - ThresholdRatio;
            var maxThreshold = 1 + ThresholdRatio;

            var rect_area = rotatedRect.Size.Width * rotatedRect.Size.Height;
            var area_con = rect_area >= TargetSize.Height * TargetSize.Width * minThreshold &&
                           rect_area <= TargetSize.Height * TargetSize.Width * maxThreshold;

            var ratio = CvMath.GetRatio(rotatedRect);
            var ratio_con = ratio > AspectRatio * minThreshold && ratio < AspectRatio * maxThreshold;

            return area_con && ratio_con;
        }


        /// <summary>
        ///     模块轮廓与其他轮廓分别绘画
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, List<DetectObject> detectObjects)
        {
            detectObjects
                .ToList()
                .ForEach(a => DrawRotatedRect(mat, a.RotatedRect, a.IsConfidence
                    ? PenColor
                    : Scalar.Aqua));
            return mat;
        }
    }
}