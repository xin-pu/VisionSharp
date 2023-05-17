using Numpy;
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


        [Fact]
        public void NumpyTestX()
        {
            var inputWidth = 5;
            var inputHeight = 5;
            var res = CreateGridX(inputWidth, inputHeight, 3);
            PrintObject(res);
        }


        [Fact]
        public void NumpyTestY()
        {
            var inputWidth = 5;
            var inputHeight = 5;
            var res = CreateGridY(inputWidth, inputHeight, 3);
            PrintObject(res);
        }

        private NDarray CreateGridX(int width, int height, int layer)
        {
            var basic = np.linspace(0, width - 1, width, dtype: np.float32);
            var final = basic
                .expand_dims(0)
                .repeat(new[] {height}, 0)
                .expand_dims(0)
                .repeat(new[] {layer}, 0);
            return final;
        }


        private NDarray CreateGridY(int width, int height, int layer)
        {
            var basic = np.linspace(0, height - 1, height, dtype: np.float32);
            var final = basic
                .expand_dims(1)
                .repeat(new[] {width}, 1)
                .expand_dims(0)
                .repeat(new[] {layer}, 0);
            return final;
        }
    }
}