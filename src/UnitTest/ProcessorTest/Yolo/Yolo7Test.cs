using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using VisionSharp.Utils;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    public class Yolo7Test : AbstractTest
    {
        public Yolo7Test(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        internal string ModelPath = @"F:\SaveModels\Yolo\qr.onnx";

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
            var image = @"F:\QR\JPEGImages\0179583169.jpg";
            var mat = Cv2.ImRead(image);

            var d = new ObjDetYolo7<QrCategory>(ModelPath)
            {
                Confidence = 0.5f,
                IouThreshold = 0.3f
            };
            var res = d.Call(mat, mat);
            Cv2.ImShow("1", res.OutMat);
            Cv2.WaitKey();
        }

        [Fact]
        public void TestSigmoid()
        {
            var a = new Mat(2, 2, MatType.CV_64F);
            a.At<double>(0, 0) = 2;
            PrintMatrix(a);
            CvBasic.Sigmoid(a);
            PrintMatrix(a);
        }
    }
}