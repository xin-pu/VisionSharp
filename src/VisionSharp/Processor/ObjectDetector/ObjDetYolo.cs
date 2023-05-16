using OpenCvSharp;
using VisionSharp.Models.Detect;
using VisionSharp.Models.EnumType;

namespace VisionSharp.Processor.ObjectDetector
{
    public abstract class ObjDetYolo<T> : ObjectDetector<T> where T : Enum
    {
        private string _configFile;
        private Size _inputPattern;
        private string _modelWeights;

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

        internal override ObjRect<T>[] Process(Mat input)
        {
            var mats = FrontNet(Net, input);
            var candidate = Decode(mats, input.Size());
            var final = NonMaximalSuppression(candidate);
            return final;
        }
    }
}