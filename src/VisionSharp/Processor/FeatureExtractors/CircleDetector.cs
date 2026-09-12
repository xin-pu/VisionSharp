using OpenCvSharp;

namespace VisionSharp.Processor.FeatureExtractors
{
    /// <summary>
    ///     寻找圆的处理器，输入灰度图像，返回斑点关键点
    /// </summary>
    public class CircleDetector : BlobDetectorBase<KeyPoint[]>
    {
        public CircleDetector()
            : base("CircleFinder")
        {
        }

        internal override KeyPoint[] Process(Mat input)
        {
            using var simpleBlob = CreateBlobDetector();
            return simpleBlob.Detect(input);
        }

        internal override Mat Draw(Mat mat, KeyPoint[] result, bool reliability)
        {
            if (result == null)
            {
                return mat;
            }

            foreach (var keyPoint in result)
            {
                var point = keyPoint.Pt.ToPoint();
                mat = DrawCircle(mat, point, (int) keyPoint.Size, PenColor);
            }

            return mat;
        }

        /// <summary>
        ///     找到KeyPoints即认为可靠
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal override bool GetReliability(KeyPoint[] result)
        {
            return result is {Length: > 0};
        }
    }
}
