using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OpenCvSharp;
using UnitTest;
using VisionSharp.Processor.Transform;
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
            ChangeImagePNG2BMP(@"F:\COC Tray\New 2024\unloading");
        }

        [Fact]
        public void ChangeFormat()
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
            var trainfolder = @"F:\COC Tray\New 2024\unloading";
            var files = Directory.GetFiles(trainfolder, "*.png").ToList();
            var i = 0;
            files.ForEach(f =>
            {
                i++;
                var newname = f.Replace(new FileInfo(f).Name, $"5_{i:D5}.png");
                File.Copy(f, newname);
            });
        }

        [Fact]
        public void ReformatAnnFiles()
        {
            var trainfolder = @"F:\CoolingMud\Annotations";
            var files = new DirectoryInfo(trainfolder).GetFiles("*.txt").ToList();
            var i = 0;
            files.ForEach(f =>
            {
                i++;

                var res = CVT(f.FullName);
                using var sw = new StreamWriter(f.FullName, false);
                sw.Write(res);
            });
        }

        [Fact]
        public void CheckAnnFiles()
        {
            var trainfolder = @"F:\COC Tray\New 2024\unloading\Annotations";
            var files = new DirectoryInfo(trainfolder).GetFiles("*.txt").ToList();
            var i = 0;
            files.ForEach(f =>
            {
                i++;


                using var sw = new StreamReader(f.FullName);
                var all = sw.ReadToEnd();
                if (all.Contains("?"))
                {
                    PrintObject(f.Name);
                }

                var d = all.Replace("\r\n", ",");
                PrintObject($"{f.Name}\t{d.Length}");
            });
        }

        private string CVT(string file)
        {
            using var s = new StreamReader(file);
            {
                var ss = s.ReadToEnd();
                if (!ss.Contains(","))
                {
                    var lines = ss.Split('\r', '\n').Where(a => a != "");
                    var newlines = lines.Select(l => string.Join(",", l.ToCharArray()));
                    var endlines = string.Join("\r\n", newlines);
                    return endlines;
                }

                return ss;
            }
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


                    if (sourceImage != null)
                    {
                        i++;
                        var newName = $"{source_i}_{i.ToString().PadLeft(4, '0')}";
                        var targetImage = Path.Combine(sourceImage.DirectoryName, $"{newName}{sourceImage.Extension}")
                            .Replace(s, targetFolder);
                        var targetAnn = Path.Combine(ann.DirectoryName, $"{newName}{ann.Extension}")
                            .Replace(s, targetFolder);


                        var mat = Cv2.ImRead(sourceImage.FullName, ImreadModes.Grayscale);
                        mat = croper.Call(mat);
                        var res = rotate.Call(mat);
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