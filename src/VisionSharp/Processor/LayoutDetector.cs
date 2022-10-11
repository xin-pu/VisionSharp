using OpenCvSharp;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     布局检测器
    /// </summary>
    public abstract class LayoutDetector : Processor<Mat, DetectLayout>
    {
        protected LayoutDetector(string name) : base(name)
        {
        }
    }
}