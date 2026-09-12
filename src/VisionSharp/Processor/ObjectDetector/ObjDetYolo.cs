using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;
using VisionSharp.Models.EnumType;

namespace VisionSharp.Processor.ObjectDetector
{
    public abstract class ObjDetYolo<T> : ObjectDetector<T> where T : Enum
    {
        private string _configFile = null!;
        private Size _inputPattern;
        private string _modelWeights = null!;

        /// <summary>
        ///     基于Yolo的目标检测器
        /// </summary>
        protected ObjDetYolo(Size inputPattern)
            : base("ObjDetectorYolo")
        {
            InputPattern = inputPattern;
        }

        public YoloModel YoloModel { protected set; get; }

        public string ModelWeights
        {
            internal set => SetProperty(ref _modelWeights, value);
            get => _modelWeights;
        }


        public string ConfigFile
        {
            internal set => SetProperty(ref _configFile, value);
            get => _configFile;
        }

        public Size InputPattern
        {
            internal set => SetProperty(ref _inputPattern, value);
            get => _inputPattern;
        }

        /// <summary>
        ///     校验权重文件存在性并读取 ONNX 网络
        /// </summary>
        /// <param name="weights">ONNX 权重文件路径</param>
        /// <returns>已加载的网络</returns>
        protected Net LoadOnnxNet(string weights)
        {
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }

            if (File.Exists(weights) == false)
            {
                throw new FileNotFoundException("Model weights not found", weights);
            }

            return CvDnn.ReadNetFromOnnx(weights)
                        ?? throw new NullReferenceException("Can't Load Net");
        }

        /// <summary>
        ///     校验文件存在性并读取 DarkNet 网络
        /// </summary>
        /// <param name="weights">DarkNet 权重文件路径</param>
        /// <param name="configFile">DarkNet 配置文件路径</param>
        /// <returns>已加载的网络</returns>
        protected Net LoadDarkNetNet(string weights, string configFile)
        {
            if (weights == null || configFile == null)
            {
                throw new ArgumentNullException(weights == null ? nameof(weights) : nameof(configFile));
            }

            if (File.Exists(weights) == false || File.Exists(configFile) == false)
            {
                throw new FileNotFoundException("Model files not found");
            }

            return CvDnn.ReadNetFromDarknet(configFile, weights)
                        ?? throw new NullReferenceException("Can't Load Net");
        }

        /// <summary>
        ///     构造 1/255 归一化、swapRB 的网络输入 blob
        /// </summary>
        /// <param name="mat">输入图像</param>
        /// <returns>输入 blob（调用方负责释放）</returns>
        protected Mat CreateInputBlob(Mat mat)
        {
            return CvDnn.BlobFromImage(mat,
                1F / 255,
                InputPattern,
                new Scalar(0, 0, 0),
                true,
                false);
        }
    }
}