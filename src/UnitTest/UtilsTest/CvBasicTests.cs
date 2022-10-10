using OpenCvSharp;
using VisionSharp.Utils;
using Xunit.Abstractions;

namespace UnitTest.UtilsTest
{
    public class CvBasicTests : AbstractTest
    {
        public CvBasicTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void GetDetectRectsTest()
        {
            var res = CvBasic.GetDetectRects(
                new Size(10, 10),
                new Size(2, 2));
            res.ForEach(PrintObject);
        }

        [Fact]
        public void GetRectMatTest()
        {
            var mat = Mat.FromArray(new[,] {{0d, 0d}, {1d, 1d}, {2d, 2d}});
            var roiMat = CvBasic.GetRectMat(mat, new Rect(0, 0, 1, 2));
            PrintMatrix(roiMat);
        }
    }
}