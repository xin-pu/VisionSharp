using OpenCvSharp;
using VisionSharp.Processor.Transform;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class ImageProcessorTest : AbstractTest
    {
        public ImageProcessorTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) { }

        [Fact]
        public void RotatedTest()
        {
            var mat = Mat.FromArray(new double[,] {{1, 2}, {3, 4}});
            var rotated = new Rotator(RotateDeg.Deg0);
            PrintMatrix(rotated.Call(mat));

            PrintMatrix(rotated.Call(mat, RotateDeg.Deg90));
            PrintMatrix(rotated.Call(mat, RotateDeg.Deg180));
            PrintMatrix(rotated.Call(mat, RotateDeg.Deg270));
        }

        [Fact]
        public void RotatedTestSave()
        {
            var mat = Cv2.ImRead(@"..\..\..\..\testimages\barcode.png");
            var rotated = new Rotator(RotateDeg.Deg90);
            var r = rotated.Call(mat, mat);
        }

        [Fact]
        public void LetterBoxTest()
        {
            var input = Cv2.ImRead(@"F:\QR\JPEGImages\0179583169.jpg");
            var letter = new LetterBox(new Size(640, 640));
            var res = letter.Call(input);
            Cv2.ImShow("Letter", res);
            Cv2.WaitKey();
        }
    }
}