using OpenCvSharp;
using VisionSharp.Models.Layout;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     布局检测器
    /// </summary>
    public abstract class LayoutDetector : Processor<Mat, Layout>
    {
        protected LayoutDetector(string name) : base(name)
        {
        }
    }
}