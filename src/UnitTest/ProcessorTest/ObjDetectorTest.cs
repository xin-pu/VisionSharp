using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class ObjDetectorTest : AbstractTest
    {
        public ObjDetectorTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void ObjDetectortDarkNetTest()
        {
            var model = @"..\..\..\..\testModels\yolov3.weights";
            var cfg = @"..\..\..\..\testModels\yolov3.cfg";
            var objDetector = new ObjDetectorDarkNet<CocoCategory>(model, cfg);
            PrintObject(objDetector);


            var image = @"E:\OneDrive - II-VI Incorporated\Pictures\Saved Pictures\voc\004545.jpg";
            var mat = Cv2.ImRead(image);
            var objRects = objDetector.Call(mat, mat);
            PrintObject(objRects);
            Cv2.ImShow("Result", objRects.OutMat);
            Cv2.WaitKey();
        }


        [Fact]
        public void ObjDetectortTest()
        {
            var model = @"F:\SaveModels\Yolo\raccoon-tiny.onnx";
            var imageFilePath = @"..\..\..\..\testimages\002341.jpg";
            var d = CvDnn.ReadNetFromOnnx(model).GetLayerNames();
            PrintObject(string.Join("\r", d));
        }

        [Fact]
        public void ObjDetectortTestID()
        {
            var model = @"F:\SaveModels\Yolo\raccoon-tiny.onnx";
            var imageFilePath = @"..\..\..\..\testimages\002341.jpg";
            var d = CvDnn.ReadNetFromOnnx(model).GetLayerId("532");
            PrintObject(d);
        }


        [Fact]
        public void QrTest()
        {
            var model = @"F:\SaveModels\Yolo\voc.onnx";
            var mat = Cv2.ImRead(@"F:\QR\JPEGImages\3043837346.jpg");


            var predictor = new ObjDetectorYoloOnnx<ObjCategory>(model);
            var res = predictor.Call(mat);
            PrintObject(res);
        }
    }
}