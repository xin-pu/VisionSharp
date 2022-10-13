using OpenCvSharp;

namespace VisionSharp.Processor.Classifiers
{
    /// <summary>
    ///     分类器
    /// </summary>
    /// <typeparam name="T">分类类型</typeparam>
    public class Classifier<T> : Processor<Mat, T> where T : Enum
    {
        public Classifier() : base("Classifier")
        {
        }

        internal override T Process(Mat input)
        {
            throw new NotImplementedException();
        }
    }
}