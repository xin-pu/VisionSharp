using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    public class Yolo7Test : AbstractTest
    {
        public Yolo7Test(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        internal string QRModelPath = @"F:\SaveModels\Yolo\qr\qr_best.onnx";
        internal string VocModelPath = @"F:\SaveModels\Yolo\Voc\voc.onnx";
        internal string RaccoonTinyModelPath = @"F:\SaveModels\Yolo\raccoon\raccoon.onnx";

        [Fact]
        public void ObjDetectortTest()
        {
            var net = CvDnn.ReadNetFromOnnx(QRModelPath);

            if (net == null)
            {
                return;
            }

            var layersNames = net.GetLayerNames();

            foreach (var layerName in layersNames)
            {
                if (layerName == null)
                {
                    continue;
                }

                var id = net.GetLayerId(layerName);
                PrintObject($"{id}:\t{layerName}");
            }
        }

        [Fact]
        public void QrDetectortTest()
        {
            var d = new ObjDetYolo7<QrCategory>(QRModelPath)
            {
                Confidence = 0.6f,
                IouThreshold = 0.5f
            };
            var image = @"F:\QR\JPEGImages\0179583169.jpg";
            var mat = Cv2.ImRead(image);
            var res = d.Call(mat, mat);
            PrintObject(res.Result);

            Cv2.ImShow("result", res.OutMat);
            Cv2.WaitKey();
        }

        [Fact]
        public void RaccoonDetectortTest()
        {
            var d = new ObjDetYolo7<Raccoon>(@"E:\ObjectDetect\yolov7_pytorch\logs\best_epoch_weights.onnx")
            {
                Confidence = 0.5f,
                IouThreshold = 0.5f
            };
            var image = @"E:\OneDrive - II-VI Incorporated\Pictures\Saved Pictures\raccoon\Racccon (1).jpg";
            var mat = Cv2.ImRead(image);
            var res = d.Call(mat, mat);
            PrintObject(res.Result);

            Cv2.ImShow("result", res.OutMat);
            Cv2.WaitKey();
        }

        [Fact]
        public void VocDetectortTest()
        {
            var d = new ObjDetYolo7<VocCategory>(VocModelPath)
            {
                Confidence = 0.4f,
                IouThreshold = 0.5f
            };
            var image = @"E:\OneDrive - II-VI Incorporated\Pictures\Saved Pictures\voc\dog.jpg";
            var mat = Cv2.ImRead(image);
            var res = d.Call(mat, mat);
            PrintObject(res.Result);

            Cv2.ImShow("result", res.OutMat);
            Cv2.WaitKey();
        }
    }
}