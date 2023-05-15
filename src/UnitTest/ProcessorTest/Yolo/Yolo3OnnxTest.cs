using OpenCvSharp.Dnn;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    public class Yolo3OnnxTest : AbstractTest
    {
        public Yolo3OnnxTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
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
    }
}