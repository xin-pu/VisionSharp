using FluentAssertions;
using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Models.Layout;
using VisionSharp.Processor;
using VisionSharp.Processor.LayoutDetectors;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class LayoutDetectorTest : AbstractTest
    {
        public LayoutDetectorTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void TestLayout()
        {
            var layout = new Layout<ObjCategory>(3, 5);
            PrintObject(layout);
            PrintObject(layout.ToAnnotationString());
        }


        [Fact]
        public void TestLayoutSave()
        {
            var layout = new Layout<ObjCategory>(3, 5);
            layout.Save("save.txt");
            var res = File.Exists("save.txt");
            res.Should().BeTrue();
        }

        [Fact]
        public void TestLayoutArgument()
        {
            var argument = new LayoutArgument(
                new Size(2, 8),
                new Size(800, 800),
                0.7);
            PrintObject(argument);
        }

        [Fact]
        public void TrayLayoutDetectorTest()
        {
            var onnx = @"..\..\..\..\testonnx\load800.onnx";
            var testImage = @"F:\COC Tray\Union LoadTray\Images\1_0003.bmp";

            var argument = new LayoutArgument(new Size(2, 8), new Size(800, 800), 0.7);
            var layoutDetector = new LayoutDlDetector<ObjCategory>(onnx, argument);

            var mat = Cv2.ImRead(testImage, ImreadModes.Grayscale);
            var res = layoutDetector.Call(mat, mat).Result;
            PrintObject(res);
        }

        [Fact]
        public void MudLayoutDetectorTest()
        {
            var onnx = @"F:\SaveModels\Yolo\mud640.onnx";
            var testImage = @"F:\CoolingMud\Verify\00005.bmp";

            var argument = new LayoutArgument(new Size(6, 3), new Size(640, 640), 0.7);
            var layoutDetector = new LayoutDlDetector<MudCategory>(onnx, argument);

            var mat = Cv2.ImRead(testImage, ImreadModes.Grayscale);
            var res = layoutDetector.Call(mat, mat).Result;
            PrintObject(res);
        }
    }
}