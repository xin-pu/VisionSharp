using OpenCvSharp;

namespace VisionSharp.Processor.FeatureExtractors
{
    public class DiameterDetector : FeatureExtractor<Mat>
    {
        public DiameterDetector() : base("DiameterDetector")
        {
        }

        internal override Mat Process(Mat input)
        {
            // Step 1 转8bit图像
            using var image = Convert(input);
            var color = image.CvtColor(ColorConversionCodes.GRAY2RGB);
            // Step 2 创建Mask
            using var gray = image.MedianBlur(5);

            // Step 3 二值化
            using var binary = gray.Threshold(50, 255, ThresholdTypes.Binary);


            // Step 5 提取轮廓
            using var hierarchy = new Mat();
            binary.FindContours(out var contours, hierarchy, RetrievalModes.List,
                ContourApproximationModes.ApproxSimple);

            foreach (var i in Enumerable.Range(0, contours.Length))
            {
                using var maskC = Mat.Zeros(input.Size(), MatType.CV_8U).ToMat();
                Cv2.DrawContours(maskC, contours, i, new Scalar(255, 255, 255), -1);

                using var locC = gray.Clone().BitwiseAnd(maskC).ToMat();

                var moment = Cv2.Moments(locC);

                var cx = moment.M10 / moment.M00;
                var cy = moment.M01 / moment.M00;
                Cv2.Circle(color, (int) cx, (int) cy, 5, Scalar.Red, -1);


                var sigma4_x = 4 * Math.Pow(moment.Mu20 / moment.M00, 0.5);
                var sigma4_y = 4 * Math.Pow(moment.Mu02 / moment.M00, 0.5);

                var rotatedRect = Cv2.FitEllipse(contours[i]);

                var axes = new Size(sigma4_x / 2, sigma4_y / 2);

                Cv2.Ellipse(color, new Point(cx, cy), axes, rotatedRect.Angle + 90, 0, 360, Scalar.Red, 2);
            }


            return color;
        }

        private Mat Convert(Mat input)
        {
            // 克隆后原地归一化，不修改调用方传入的 Mat
            var normalized = input.Clone();
            using (normalized)
            {
                normalized.GetArray(out ushort[] data);
                var min = data.Min();
                var max = data.Max();

                for (var i = 0; i < normalized.Size().Height; i++)
                {
                    for (var j = 0; j < normalized.Size().Width; j++)
                    {
                        normalized.At<ushort>(i, j) = (byte) (255F * (normalized.At<ushort>(i, j) - min) / (max - min));
                    }
                }

                var outmat = new Mat();
                normalized.ConvertTo(outmat, MatType.CV_8U);
                return outmat;
            }
        }
    }
}
