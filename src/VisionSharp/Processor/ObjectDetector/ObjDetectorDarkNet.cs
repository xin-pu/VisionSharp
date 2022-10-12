using System.ComponentModel;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Utils;

namespace VisionSharp.Processor.ObjectDetector
{
    /// <summary>
    ///     使用DarkNet网络
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjDetectorDarkNet<T> : ObjDetectorYolo<T> where T : Enum
    {
        private string _configFile;
        private string _modelWeights;

        public ObjDetectorDarkNet(string modelWeights, string configFile)
        {
            ModelWeights = modelWeights;
            ConfigFile = configFile;
            Colors = CvBasic.GetColorDict<T>();
            Net = InitialNet();
        }

        [Category("Option")]
        public string ModelWeights
        {
            internal set => SetProperty(ref _modelWeights, value);
            get => _modelWeights;
        }

        [Category("Option")]
        public string ConfigFile
        {
            internal set => SetProperty(ref _configFile, value);
            get => _configFile;
        }

        /// <summary>
        ///     加载网络
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        internal override Net InitialNet()
        {
            if (ModelWeights == null || ConfigFile == null)
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(ModelWeights) == false || File.Exists(ConfigFile) == false)
            {
                throw new FileNotFoundException();
            }


            var darknet = CvDnn.ReadNetFromDarknet(ConfigFile, ModelWeights);
            if (darknet == null)
            {
                throw new NullReferenceException("Can't Load Net");
            }

            darknet.SetPreferableBackend(Backend.OPENCV);
            darknet.SetPreferableTarget(Target.CPU);
            return darknet;
        }

        internal override Mat[] FrontNet(Net net, Mat mat)
        {
            var inputBlob = CvDnn.BlobFromImage(mat,
                1F / 255,
                new Size(416, 416),
                new Scalar(0, 0, 0),
                true,
                false);

            Net.SetInput(inputBlob, "data");
            var mats = new Mat[] {new(), new(), new()};
            Net.Forward(mats, new[] {"yolo_106", "yolo_94", "yolo_82"});
            return mats;
        }
    }
}