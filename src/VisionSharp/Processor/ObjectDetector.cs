using VisionSharp.Models.Detect;

namespace VisionSharp.Processor
{
    public abstract class ObjectDetector : FeatureExtractor<DetectRectObject>
    {
        /// <summary>
        ///     目标检测器
        /// </summary>
        protected ObjectDetector(string name) : base(name)
        {
        }
    }
}