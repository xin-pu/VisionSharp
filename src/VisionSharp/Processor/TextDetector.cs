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
    }
}