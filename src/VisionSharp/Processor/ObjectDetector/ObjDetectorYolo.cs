using OpenCvSharp;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor.ObjectDetector
{
    public class ObjDetectorYolo<T> : ObjectDetector<T> where T : Enum
    {
        /// <summary>
        ///     基于Yolo的目标检测器
        /// </summary>
        public ObjDetectorYolo() : base("ObjDetectorYolo")
        {
        }

        internal override ObjRect<T> Process(Mat input)
        {
            throw new NotImplementedException();
        }
    }

    public enum YoloVer
    {
        YoloV1,
        YoloV2,
        YoloV3
    }

    public enum ModelType
    {
        DarkNet,
        Onnx
    }
}