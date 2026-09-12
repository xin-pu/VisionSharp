using FluentAssertions;
using OpenCvSharp;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    /// <summary>
    ///     激光闸门定位：依赖仓库内 300K 1.bmp 大图，中间结果输出到临时目录
    /// </summary>
    [Trait("Category", "RequiresLargeAssets")]
    public class LaserGatingTest : AbstractTest
    {
        public LaserGatingTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) { }


        [Fact]
        public void Test()
        {
            var saveDir = Path.Combine(Path.GetTempPath(), "LaserGatingTest");
            Directory.CreateDirectory(saveDir);
            var index = 0;
            string NextPng() => Path.Combine(saveDir, $"{++index}.png");

            using var mat = Cv2.ImRead(TestImage("300K 1.bmp"));
            mat.Should().NotBeNull();
            using var orginalMat = mat.Clone();
            using var saveMat = mat.Clone();
            mat.SaveImage(NextPng());

            Cv2.CvtColor(mat, mat, ColorConversionCodes.RGB2HSV);
            mat.SaveImage(NextPng());

            Cv2.InRange(mat, new Scalar(0, 130, 130, 255), new Scalar(255, 255, 255, 255), mat);
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage(NextPng());

            using var element9 = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(1, 5),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Close, element9, new Point(-1, -1));
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage(NextPng());

            using var hierarchy = new Mat();
            Cv2.FindContours(mat, out var contours, hierarchy, RetrievalModes.List,
                             ContourApproximationModes.ApproxSimple);
            // 设置面积阈值
            double areaThreshold = 100; // 可根据需要调整阈值

            // 遍历轮廓
            foreach (var contour in contours)
            {
                // 计算轮廓面积
                var area = Cv2.ContourArea(contour);
                // 如果面积小于阈值，则将该轮廓区域设置为黑色
                if (area < areaThreshold)
                {
                    Cv2.DrawContours(mat, new[] {contour}, -1, Scalar.Black, -1); // 填充黑色
                }
            }

            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage(NextPng());

            using var element = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(1, 31),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Open, element, new Point(-1, -1));
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage(NextPng());


            using var element2 = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(11, 11),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Close, element2, new Point(-1, -1));
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage(NextPng());


            Cv2.FindContours(mat, out contours, hierarchy, RetrievalModes.List,
                             ContourApproximationModes.ApproxSimple);

            var rects = contours.Select(x => Cv2.BoundingRect(x)).ToArray();
            rects.Should().NotBeEmpty();
            var maxRect = rects.OrderByDescending(x => x.Size.Height * x.Size.Width).First();

            using var maxRoi = mat[maxRect];
            using var invertedImage = (255 - maxRoi).ToMat();

            Cv2.FindContours(invertedImage, out contours, hierarchy, RetrievalModes.List,
                             ContourApproximationModes.ApproxSimple);

            var rects2 = contours.Select(x => Cv2.BoundingRect(x)).ToArray();
            rects2.Should().NotBeEmpty();
            var maxRect2 = rects2.OrderByDescending(x => x.Size.Height * x.Size.Width).First();


            var maxHeight = maxRect.Y;
            var imageWidth = orginalMat.Width;
            var minHeight = maxHeight + maxRect2.Height;
            minHeight.Should().BeGreaterThan(maxHeight);
            Cv2.Line(orginalMat, new Point(0, maxHeight), new Point(imageWidth, maxHeight), Scalar.DarkRed, 3);
            Cv2.Line(orginalMat, new Point(0, minHeight), new Point(imageWidth, minHeight), Scalar.DarkRed, 3);

            orginalMat.SaveImage(NextPng());
            TestOutputHelper.WriteLine($"output: {saveDir}");
        }
    }
}
