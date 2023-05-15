using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    public class Yolo7OnnxTest : AbstractTest
    {
        public Yolo7OnnxTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        internal string ModelPath = @"F:\SaveModels\Yolo\qr_best.onnx";

        [Fact]
        public void ObjDetectortTest()
        {
            var net = CvDnn.ReadNetFromOnnx(ModelPath);

            if (net == null)
            {
                return;
            }

            var layersNames = net.GetLayerNames();

            foreach (var layerName in layersNames)
            {
                var id = net.GetLayerId(layerName);
                PrintObject($"{id}:\t{layerName}");
            }
        }

        [Fact]
        public void QrDetectortTest()
        {
            var image = @"E:\OneDrive - II-VI Incorporated\Pictures\Saved Pictures\QR.png";
            var mat = Cv2.ImRead(image);

            var d = new ObjDetectorYoloOnnx<QrCategory>(ModelPath)
            {
                Confidence = 0.5f,
                IouThreshold = 0.3f
            };
            var res = d.Call(mat);
            PrintObject(res);
        }
    }
}