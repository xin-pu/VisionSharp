using OpenCvSharp;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     图像处理器，包含图像增强，图像转换，绘画等处理器
    /// </summary>
    public abstract class ImageProcessor : Processor<Mat, Mat>
    {
        protected ImageProcessor(string name) : base(name)
        {
        }
    }
}