using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace VisionSharp.Processor.TextDetectors
{
    /// <summary>
    ///     车牌检测器识别网络
    ///     Todo
    /// </summary>
    /// <remarks>https://github.com/sirius-ai/LPRNet_Pytorch</remarks>
    /// <remarks>https://arxiv.org/abs/1806.10447v1</remarks>
    public class LicensePlateDetector : TextDetector
    {
        public LicensePlateDetector(string pth) : base("LicensePlateDetector")
        {
            var d = CvDnn.ReadNetFromTorch(pth);
        }

        internal override string Process(Mat input)
        {
            return string.Empty;
        }
    }
}