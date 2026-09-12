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

        /// <summary>
        ///     执行处理器,并在传入MAT上绘制相关处理结果
        ///     绘制结果包含在返回的RichInfo中
        ///     图像处理器直接返回处理后图像到RichInfo
        ///     异常直接抛出，不再吞入RichInfo
        /// </summary>
        /// <param name="input">输入对象</param>
        /// <param name="mat">传入图像</param>
        /// <param name="saveName">保存文件名（EnableSaveMat 为 true 时生效）</param>
        /// <returns></returns>
        public override RichInfo<Mat> Call(Mat input, Mat mat, string saveName = "")
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (mat == null) throw new ArgumentNullException(nameof(mat));

            // Step 1 执行处理
            var result = Process(input);

            // Step 2 分析结果
            var confi = GetReliability(result);

            // Step 3 绘制输出图像
            var matOut = DrawMat(mat, result, confi, saveName);

            return new RichInfo<Mat>(result, confi, matOut);
        }

        /// <summary>
        ///     图像处理直接绘制处理后的结果
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="result"></param>
        /// <param name="reliability"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, Mat result, bool reliability)
        {
            return result;
        }
    }
}