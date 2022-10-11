using OpenCvSharp;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     目标检测器
    /// </summary>
    public abstract class ObjectDetector : Processor<Mat, DetectRectObject>
    {
        protected ObjectDetector(string name) : base(name)
        {
        }
    }
}