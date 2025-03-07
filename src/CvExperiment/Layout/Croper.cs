using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;
using UnitTest;
using VisionSharp.Processor.Transform;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment.Layout
{
    public class Croper : AbstractTest
    {
        public Croper(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        private Dictionary<string, RotatedRect> rotatedRects => new Dictionary<string, RotatedRect>
        {
            ["WUX-E80015291-Load"] =
                new RotatedRect(new Point2f(2689.6f, 1640.6f), new Size2f(2960.48, 3119.01), 89.89066f),
            ["WUX-E80015291-Unload"] =
                new RotatedRect(new Point2f(2704.12f, 1725.80f), new Size2f(4518.14, 3164.22), 89.58f - 90),
            ["WUX-E80015206-Load"] =
                new RotatedRect(new Point2f(2724.64f, 1656.02f), new Size2f(3002.28, 3160.92), 89.90f - 90),
            ["WUX-E80015206-Unload"] =
                new RotatedRect(new Point2f(2524.12f, 1682.79f), new Size2f(4531.85, 3204.53), 89.47f - 90)
        };

        private RotatedRectCropper getRotatedRectCropper(string id)
        {
            return new RotatedRectCropper(rotatedRects[id]);
        }


        [Fact]
        public void Rotated()
        {
            var folder = @"F:\CoolingMud\Images";
            var imageFiles = new DirectoryInfo(folder).GetFiles().ToList();
            imageFiles.ForEach(img =>
            {
                var rotated = new Rotator(RotateDeg.Deg270);
                var mat = Cv2.ImRead(img.FullName, ImreadModes.Grayscale);
                var res = rotated.Call(mat);

                var crop = new RectCropper(new Rect(558, 2381, 2646, 1902));
                res = crop.Call(res);
                var file = Path.Combine(folder, $"img_{Path.GetFileNameWithoutExtension(img.Name)}.png");
                res.SaveImage(file);
            });
        }

        [Fact]
        public void Crop()
        {
            var folder = @"E:\My Temp\unload";
            var image_folder = Path.Combine(folder, "Images");
            var imageFiles = new DirectoryInfo(image_folder).GetFiles().ToList();
            imageFiles.ForEach(img =>
            {
                var mat = Cv2.ImRead(img.FullName, ImreadModes.Grayscale);
                mat = getRotatedRectCropper("WUX-E80015206").Call(mat);
                Cv2.ImWrite(img.FullName, mat);
                GC.Collect();
                GC.WaitForFullGCComplete();
            });
        }

        [Fact]
        public void TestCrop()
        {
            var Crop = new RotatedRectCropper(new RotatedRect(new Point2f(2524.12f, 1682.79f),
                new Size2f(3204.53, 4531.85), 89.47f));

            var mat = Cv2.ImRead(@"E:\My Temp\unload back.bmp", ImreadModes.Grayscale);

            var outmat = Crop.Call(mat);

            Cv2.Resize(outmat, mat, new Size(outmat.Size().Width / 6, outmat.Size().Height / 6));
            Cv2.ImShow("1", mat);
            Cv2.WaitKey();
        }

        [Fact]
        public void GetRotatedRect()
        {
            var mat = Cv2.ImRead(@"E:\My Temp\WUX-E80016940\load.png", ImreadModes.Grayscale);
            Cv2.Threshold(mat, mat, 100, 255, ThresholdTypes.Otsu);
            Cv2.FindContours(mat, out var contours, new Mat(), RetrievalModes.List,
                ContourApproximationModes.ApproxSimple);
            var rotatedRec6s = contours.Select(a => Cv2.MinAreaRect(a)).ToList();
            var res = rotatedRec6s.OrderBy(a => a.Size.Width * a.Size.Height).Last();
            PrintRotatedRects(new[] {res});
        }
    }
}