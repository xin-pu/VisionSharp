using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;
using VisionSharp.Utils;

namespace VisionSharp.Processor.ObjectDetector
{
    /// <summary>
    ///     基于Yolo3的目标检测
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjDetYolo3<T> : ObjDetYolo<T> where T : Enum
    {
        public ObjDetYolo3(string weightDarkNet, string configFile)
            : base(new Size(416, 416))
        {
            YoloModel = YoloModel.DarkNet;

            ModelWeights = weightDarkNet;
            ConfigFile = configFile;
            Colors = CvCvt.GetColorDict<T>();
            Net = InitialNet();
        }

        public ObjDetYolo3(string weightOnnx)
            : base(new Size(416, 416))
        {
            YoloModel = YoloModel.Onnx;

            ModelWeights = weightOnnx;
            ConfigFile = null;
            Colors = CvCvt.GetColorDict<T>();
            Net = InitialNet();
        }


        public YoloModel YoloModel { protected set; get; }

        /// <summary>
        ///     加载网络
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        internal sealed override Net InitialNet()
        {
            var net = YoloModel switch
            {
                YoloModel.DarkNet => InitialDarkNet(),
                YoloModel.Onnx => InitialOnnx(),
                _ => throw new Exception()
            };
            if (net == null)
            {
                throw new NullReferenceException("Can't Load Net");
            }

            net.SetPreferableBackend(Backend.DEFAULT);
            net.SetPreferableTarget(Target.CPU);
            return net;
        }


        internal Net InitialDarkNet()
        {
            if (ModelWeights == null || ConfigFile == null)
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(ModelWeights) == false || File.Exists(ConfigFile) == false)
            {
                throw new FileNotFoundException();
            }


            var net = ConfigFile == null
                ? CvDnn.ReadNetFromOnnx(ModelWeights)
                : CvDnn.ReadNetFromDarknet(ConfigFile, ModelWeights);

            return net;
        }

        internal Net InitialOnnx()
        {
            if (ModelWeights == null)
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(ModelWeights) == false)
            {
                throw new FileNotFoundException();
            }

            var net = CvDnn.ReadNetFromOnnx(ModelWeights);

            return net;
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


        /// <summary>
        ///     Yolo3以上的解码过程是一样的
        /// </summary>
        /// <param name="mats"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override ObjRect<T>[] Decode(Mat[] mats, Size size)
        {
            var list = new List<ObjRect<T>>();

            foreach (var mat in mats)
            {
                mat[new Rect(4, 0, 1, mat.Height)].GetArray(out float[] confidence);

                var conList = confidence
                    .Select((c, i) => (c, i))
                    .Where(p => p.c > Confidence)
                    .Select(p => p.i)
                    .ToList();

                conList.AsParallel().ToList().ForEach(i =>
                {
                    var _ = mat[new Rect(0, i, mat.Width, 1)].GetArray(out float[] rowdata);
                    var rowInfo = rowdata.ToList();

                    var classify = rowInfo.Skip(5).ToList();
                    var classProb = classify.Max();
                    var classIndex = classify.IndexOf(classProb);
                    var category = (T) Enum.ToObject(typeof(T), classIndex);

                    if (classProb < Confidence)
                    {
                        return;
                    }

                    var centerX = rowInfo[0] * size.Width;
                    var centerY = rowInfo[1] * size.Height;

                    var w = (int) (rowInfo[2] * size.Width);
                    var h = (int) (rowInfo[3] * size.Height);
                    var x = (int) (centerX - w / 2F);
                    var y = (int) (centerY - h / 2F);

                    var rect = new Rect(x, y, w, h);

                    var detectRectObject = new ObjRect<T>(rect)
                    {
                        Category = category,
                        ObjectConfidence = classProb
                    };
                    list.Add(detectRectObject);
                });
            }


            return list.ToArray();
        }
    }

    public enum YoloModel
    {
        DarkNet,
        Onnx
    }
}