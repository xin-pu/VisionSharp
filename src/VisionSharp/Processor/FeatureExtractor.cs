using OpenCvSharp;

namespace VisionSharp.Processor
{
    public abstract class FeatureExtractor<T> : Processor<Mat, T>
    {
        /// <summary>
        ///     通用特征提取抽象类
        ///     输入图像，返回指定类型特征
        /// </summary>
        /// <param name="name"></param>
        protected FeatureExtractor(string name) : base(name)
        {
        }
    }
}