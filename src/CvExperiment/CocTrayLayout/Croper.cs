using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVLib.Processor.Unit;
using OpenCvSharp;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment.CocTrayLayout
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
                new RotatedRect(new Point2f(2704.12f, 1725.80f), new Size2f(4518.14, 3164.22), 89.58f),
            ["WUX-E80015206-Load"] =
                new RotatedRect(new Point2f(2724.64f, 1656.02f), new Size2f(3002.28, 3160.92), 89.90f),
            ["WUX-E80015206-Unload"] =
                new RotatedRect(new Point2f(2524.12f, 1682.79f), new Size2f(4531.85, 3204.53), 89.47f),
            ["WUX-E80015205-Load"] =
                new RotatedRect(new Point2f(2785.8f, 1748.5f), new Size2f(2950.50, 3112.90), 0.05617232f),
            ["WUX-E80015205-Unload"] =
                new RotatedRect(new Point2f(2666.5f, 1723.7f), new Size2f(4412.14, 3103.55), 0.05980768f)
        };

        private RotatedRectCropper getRotatedRectCropper(string id)
        {
            return new RotatedRectCropper(rotatedRects[id]);
        }

        [Fact]
        public void GetRotatedRect()
        {
            var mat = Cv2.ImRead(@"F:\COC Tray\unload.png", ImreadModes.Grayscale);
            Cv2.Threshold(mat, mat, 100, 255, ThresholdTypes.Otsu);
            Cv2.FindContours(mat, out var contours, new Mat(), RetrievalModes.List,
                ContourApproximationModes.ApproxSimple);
            var rotatedRec6s = contours.Select(a => Cv2.MinAreaRect(a)).ToList();
            var res = rotatedRec6s.OrderBy(a => a.Size.Width * a.Size.Height).Last();
            PrintRotatedRects(new[] {res});
        }

        [Fact]
        public void TestCrop()
        {
            var Crop = getRotatedRectCropper("WUX-E80015205-Unload");

            var mat = Cv2.ImRead(@"F:\COC Tray\unload.png", ImreadModes.Grayscale);

            var outmat = Crop.CallLight(mat);

            Cv2.Resize(outmat, mat, new Size(outmat.Size().Width / 6, outmat.Size().Height / 6));
            Cv2.ImShow("1", mat);
            Cv2.WaitKey();
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
                mat = getRotatedRectCropper("WUX-E80015205-Load").CallLight(mat);
                Cv2.ImWrite(img.FullName, mat);
                GC.Collect();
                GC.WaitForFullGCComplete();
            });
        }
    }
}