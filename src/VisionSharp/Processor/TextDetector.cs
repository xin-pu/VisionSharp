using OpenCvSharp;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     文本检测器
    /// </summary>
    public abstract class TextDetector : Processor<Mat, string>
    {
        protected TextDetector(string name) : base(name)
        {
        }

        internal override bool GetReliability(string result)
        {
            return !string.IsNullOrEmpty(result);
        }

        internal override Mat Draw(Mat mat, string result, bool reliability)
        {
            return DrawText(mat, new Point(), result, PenColor);
        }
    }
}