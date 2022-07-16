using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     用于寻找抓取产品特征点的处理器
    /// </summary>
    public abstract class PointProcessor : MatProcessor<Point2d>
    {
        protected PointProcessor(string name)
            : base(name)
        {
        }
    }
}