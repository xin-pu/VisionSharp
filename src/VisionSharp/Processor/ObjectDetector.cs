using VisionSharp.Models.Detect;

namespace VisionSharp.Processor
{
    public abstract class ObjectDetector<T> : FeatureExtractor<ObjRect<T>> where T : Enum
    {
        /// <summary>
        ///     目标检测器
        /// </summary>
        protected ObjectDetector(string name) : base(name)
        {
        }
    }
}