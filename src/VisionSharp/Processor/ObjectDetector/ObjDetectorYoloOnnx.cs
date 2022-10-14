using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace VisionSharp.Processor.ObjectDetector
{
    public class ObjDetectorYoloOnnx<T> : ObjDetectorYolo<T> where T : Enum
    {
        public ObjDetectorYoloOnnx(string onnxFile) : base(new Size(640, 640))
        {
            ModelWeights = onnxFile;

            InitialNet();
        }

        internal override Net InitialNet()
        {
            if (ModelWeights == null)
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(ModelWeights) == false)
            {
                throw new FileNotFoundException();
            }


            var darknet = CvDnn.ReadNetFromOnnx(ModelWeights);
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
            Net.Forward(mats, new[] {"yolo_head_P3", "yolo_head_P4", "yolo_head_P5"});
            return mats;
        }
    }
}