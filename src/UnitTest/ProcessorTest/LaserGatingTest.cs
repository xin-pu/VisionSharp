using OpenCvSharp;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class LaserGatingTest : AbstractTest
    {
        public LaserGatingTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) { }


        [Fact]
        public void Test()
        {
            var index = 0;
            var mat = Cv2.ImRead(@"..\..\..\..\testimages\300K 1.bmp");
            var orginalMat = mat.Clone();
            var saveMat = mat.Clone();
            mat.SaveImage($"{++index}.png");

            Cv2.CvtColor(mat, mat, ColorConversionCodes.RGB2HSV);
            mat.SaveImage($"{++index}.png");

            Cv2.InRange(mat, new Scalar(0, 130, 130, 255), new Scalar(255, 255, 255, 255), mat);
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage($"{++index}.png");

            var element9 = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(1, 5),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Close, element9, new Point(-1, -1));
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage($"{++index}.png");

            Cv2.FindContours(mat, out var contours, new Mat(), RetrievalModes.List,
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
            saveMat.SaveImage($"{++index}.png");

            var element = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(1, 31),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Open, element, new Point(-1, -1));
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage($"{++index}.png");


            var element2 = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(11, 11),
                new Point(-1, -1));
            Cv2.MorphologyEx(mat, mat, MorphTypes.Close, element2, new Point(-1, -1));
            Cv2.CvtColor(mat, saveMat, ColorConversionCodes.GRAY2RGB);
            saveMat.SaveImage($"{++index}.png");


            Cv2.FindContours(mat, out contours, new Mat(), RetrievalModes.List,
                             ContourApproximationModes.ApproxSimple);

            var rects = contours.Select(x => Cv2.BoundingRect(x)).ToArray();
            var maxRect = rects.OrderByDescending(x => x.Size.Height * x.Size.Width).First();

            var invertedImage = (255 - mat[maxRect]).ToMat();

            Cv2.FindContours(invertedImage, out contours, new Mat(), RetrievalModes.List,
                             ContourApproximationModes.ApproxSimple);

            var rects2 = contours.Select(x => Cv2.BoundingRect(x)).ToArray();
            var maxRect2 = rects2.OrderByDescending(x => x.Size.Height * x.Size.Width).First();


            var maxHeight = maxRect.Y;
            var imageWidth = orginalMat.Width;
            var minHeight = maxHeight + maxRect2.Height;
            Cv2.Line(orginalMat, new Point(0, maxHeight), new Point(imageWidth, maxHeight), Scalar.DarkRed, 3);
            Cv2.Line(orginalMat, new Point(0, minHeight), new Point(imageWidth, minHeight), Scalar.DarkRed, 3);

            orginalMat.SaveImage($"{++index}.png");

      
        }
    }
}