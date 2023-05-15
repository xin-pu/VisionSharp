using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    public class Yolo3DarkNetTest : AbstractTest
    {
        public Yolo3DarkNetTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        internal string CocoWeights = @"F:\SaveModels\DarkNet\yolov3_coco\yolov3.weights";
        internal string CocoCfg = @"F:\SaveModels\DarkNet\yolov3_coco\yolov3.cfg";


        internal string VocWeights = @"F:\SaveModels\DarkNet\yolov3_voc\yolov3.weights";
        internal string VocCfg = @"F:\SaveModels\DarkNet\yolov3_voc\yolov3.cfg";

        [Fact]
        public void CocoTest()
        {
            var objDetector = new ObjDetectorDarkNet<CocoCategory>(CocoWeights, CocoCfg);
            PrintObject(objDetector);


            var image = @"E:\OneDrive - II-VI Incorporated\Pictures\Saved Pictures\voc\004545.jpg";
            var mat = Cv2.ImRead(image);
            var objRects = objDetector.Call(mat, mat);
            PrintObject(objRects.Result);
        }

        [Fact]
        public void VocTest()
        {
            var objDetector = new ObjDetectorDarkNet<VocCategory>(VocWeights, VocCfg);
            PrintObject(objDetector);


            var image = @"E:\OneDrive - II-VI Incorporated\Pictures\Saved Pictures\voc\004545.jpg";
            var mat = Cv2.ImRead(image);
            var objRects = objDetector.Call(mat, mat);
            PrintObject(objRects.Result);
        }
    }
}