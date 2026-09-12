using FluentAssertions;
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
            using var mat = Cv2.ImRead(TestImage("barcode.png"));
            var rotated = new Rotator(RotateDeg.Deg90)
            {
                EnableSaveMat = true
            };
            using var r = rotated.Call(mat, mat, "rotated");
            r.OutMat.Should().NotBeNull();
        }

        [Fact]
        public void LetterBoxTest()
        {
            using var input = Cv2.ImRead(TestImage("dog.png"));
            var letter = new LetterBox(new Size(640, 640));
            using var res = letter.Call(input);
            res.Size().Should().Be(new Size(640, 640));
        }
    }
}
