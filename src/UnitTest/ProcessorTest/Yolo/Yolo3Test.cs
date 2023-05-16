using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    public class Yolo3Test : AbstractTest
    {
        internal string CocoCfg = @"F:\SaveModels\DarkNet\yolov3_coco\yolov3.cfg";
        internal string CocoWeights = @"F:\SaveModels\DarkNet\yolov3_coco\yolov3.weights";

        internal string VocCfg = @"F:\SaveModels\DarkNet\yolov3_voc\yolov3.cfg";
        internal string VocWeights = @"F:\SaveModels\DarkNet\yolov3_voc\yolov3.weights";

        public Yolo3Test(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void CocoTest()
        {
            var objDetector = new ObjDetYolo3<CocoCategory>(CocoWeights, CocoCfg);
            PrintObject(objDetector);


            var image = @"..\..\..\..\testimages\002341.jpg";
            var mat = Cv2.ImRead(image);
            var objRects = objDetector.Call(mat, mat);
            PrintObject(objRects.Result);
        }

        [Fact]
        public void VocTest()
        {
            var objDetector = new ObjDetYolo3<VocCategory>(VocWeights, VocCfg);
            PrintObject(objDetector);


            var image = @"..\..\..\..\testimages\002341.jpg";
            var mat = Cv2.ImRead(image);
            var objRects = objDetector.Call(mat, mat);
            PrintObject(objRects.Result);
        }
    }
}