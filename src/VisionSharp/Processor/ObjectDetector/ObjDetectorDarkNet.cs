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
        public ObjDetectorDarkNet(string modelWeights, string configFile)
            : base(new Size(416, 416))
        {
            ModelWeights = modelWeights;
            ConfigFile = configFile;
            Colors = CvCvt.GetColorDict<T>();
            Net = InitialNet();
        }

        public ObjDetectorDarkNet(string onnxWeight, Size inputPattern)
            : base(new Size(416, 416))
        {
            ModelWeights = onnxWeight;
            InputPattern = inputPattern;
            Colors = CvCvt.GetColorDict<T>();
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

        /// <summary>
        ///     加载网络
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        internal Net InitialOnnx()
        {
            if (ModelWeights == null)
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
                InputPattern,
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