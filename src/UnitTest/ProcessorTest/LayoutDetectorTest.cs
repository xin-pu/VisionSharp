using OpenCvSharp;
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
        public void TrayLayoutDetectorTest()
        {
            var onnx = @"..\..\..\..\testonnx\load800.onnx";
            var testImage = @"F:\COC Tray\Union LoadTray\Images\1_0003.bmp";

            var argument = new LayoutArgument(new Size(2, 8), new Size(800, 800), 0.7);
            var layoutDetector = new LayoutDlDetector(onnx, argument);

            var mat = Cv2.ImRead(testImage, ImreadModes.Grayscale);
            var res = layoutDetector.Call(mat);
            PrintObject(res);
            PrintObject(res.ToAnnotationString());
        }
    }
}