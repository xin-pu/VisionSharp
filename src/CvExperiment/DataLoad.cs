using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CVLib.Processor.Unit;
using OpenCvSharp;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment
{
    public class DataLoad : AbstractTest
    {
        public string WorkFolder = @"F:\COC Tray";

        public DataLoad(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        [Fact]
        public void Run()
        {
            /// 转换图像格式
            ChangeImagePNG2BMP(@"F:\COC Tray\Union UnloadTray\Images");
        }

        [Fact]
        public void changeFormat()
        {
            var ann = new DirectoryInfo(@"F:\COC Tray\Union LoadTray\Annotations").GetFiles();
            foreach (var fileInfo in ann)
            {
                var newlines = new List<string>();
                using var sr = new StreamReader(fileInfo.FullName);
                {
                    var lines = sr.ReadToEnd().Split('\n', '\r').Where(a => a != "");


                    foreach (var line in lines)
                    {
                        var linenew = line.Insert(1, ",");
                        newlines.Add(linenew);
                    }
                }
                var newfile = Path.Combine(@"F:\COC Tray\Union LoadTray\Annotations2", fileInfo.Name);
                using var sw = new StreamWriter(newfile);
                var newdata = string.Join("\r\n", newlines);
                sw.Write(newdata);
            }
        }


        [Fact]
        public void RenameFiles()
        {
            var trainfolder = @"F:\COC Tray\WUX-E80015206 UnloadTray\Crop";
            var files = Directory.GetFiles(trainfolder, "*.bmp").ToList();
            var i = 0;
            files.ForEach(f =>
            {
                i++;
                var newname = f.Replace(new FileInfo(f).Name, $"5206_{i}.bmp");
                File.Copy(f, newname);
            });
        }


        [Fact]
        public void CombineDataSet()
        {
            var sourceFolder = new List<string> {"WUX-E80015206 LoadTray", "WUX-E80015291 LoadTray"};
            var targetFolder = "Union LoadTray";

            var rotatedRects = new List<RotatedRect>
            {
                new RotatedRect(new Point2f(2724.64f, 1656.02f), new Size2f(3160.92, 3002.28), 89.90f),
                new RotatedRect(new Point2f(2689.64f, 1640.64f), new Size2f(3119.01, 2960.48), 89.89f)
            };
            var rotate = new Rotator(RotateDeg.Deg270);
            var source_i = 0;
            sourceFolder.ForEach(s =>
            {
                source_i++;
                var i = 0;
                var annfolder = Path.Combine(WorkFolder, s, "Annotations");
                var imagefolder = Path.Combine(WorkFolder, s, "Images");
                var annfiles = new DirectoryInfo(annfolder).GetFiles().ToList();
                var imagefiles = new DirectoryInfo(imagefolder).GetFiles().ToList();

                var croper = new RotatedRectCropper(rotatedRects[source_i - 1]);

                annfiles.ForEach(ann =>
                {
                    var name = Path.GetFileNameWithoutExtension(ann.FullName);
                    var sourceImage =
                        imagefiles.FirstOrDefault(a => Path.GetFileNameWithoutExtension(a.FullName) == name);
                    if (i == 61) ;

                    if (sourceImage != null)
                    {
                        i++;
                        var newName = $"{source_i}_{i.ToString().PadLeft(4, '0')}";
                        var targetImage = Path.Combine(sourceImage.DirectoryName, $"{newName}{sourceImage.Extension}")
                            .Replace(s, targetFolder);
                        var targetAnn = Path.Combine(ann.DirectoryName, $"{newName}{ann.Extension}")
                            .Replace(s, targetFolder);


                        var mat = Cv2.ImRead(sourceImage.FullName, ImreadModes.Grayscale);
                        mat = croper.CallLight(mat);
                        var res = rotate.CallLight(mat);
                        res.SaveImage(targetImage);
                        res.Dispose();
                        mat.Dispose();
                        res = null;
                        mat = null;
                        GC.Collect();
                        GC.WaitForFullGCComplete();

                        Thread.Sleep(TimeSpan.FromSeconds(0.2));

                        File.Copy(ann.FullName, targetAnn, true);
                    }
                });
            });
        }

        private void ChangeImagePNG2BMP(string folder)
        {
            var files = Directory.GetFiles(folder, "*.png").ToList();
            var i = 0;
            files.ForEach(f =>
            {
                i++;
                var newname = f.Replace("png", "bmp");
                File.Copy(f, newname);
                File.Delete(f);
            });
        }
    }
}