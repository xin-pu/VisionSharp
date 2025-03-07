using System;
using System.IO;
using System.Linq;
using OpenCvSharp;
using UnitTest;
using VisionSharp.Models.Category;
using VisionSharp.Models.Layout;
using VisionSharp.Processor;
using Xunit.Abstractions;

namespace CvExperiment.Layout
{
    public abstract class General<T> : AbstractTest where T : Enum
    {
        protected General(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            Initial();
        }

        public LayoutDetector<T> Predictor { set; get; }

        public void Initial()
        {
            Predictor = InitialPreidctor();
        }

        public abstract LayoutDetector<T> InitialPreidctor();


        #region 重命名

        public void RenameFiles(string workfolder, string searchPattern, string prename = "")
        {
            var files = Directory.GetFiles(workfolder, searchPattern).ToList();
            var i = 0;
            files.ForEach(f =>
            {
                i++;
                var newname = f.Replace(new FileInfo(f).Name,
                                        prename == ""
                                            ? ""
                                            : prename + "_"
                                                      + $"{i}.bmp");
                File.Copy(f, newname);
            });
        }

        #endregion

        #region 转换目录下图片格式

        public void ChangeImagePNG2BMP(string workfolder)
        {
            var files = Directory.GetFiles(workfolder, "*.png").ToList();
            files.ForEach(f =>
            {
                var newname = f.Replace("png", "bmp");
                File.Copy(f, newname);
                File.Delete(f);
            });
        }

        #endregion

        #region 绘制标注结果

        public void GeneraitAnnToImage(string workfolder)
        {
            var annFolder = Path.Combine(workfolder, "Annotations");
            var imgFolder = Path.Combine(workfolder, "Images");


            new DirectoryInfo(annFolder).GetFiles("*.txt")
                .ToList()
                .ForEach(ann =>
                {
                    var id = Path.GetFileNameWithoutExtension(ann.Name);
                    var img = Path.Combine(imgFolder, $"{id}.bmp");
                    var detectLayer = Layout<ObjCategory>.LoadFromAnnotation(ann.FullName);
                    var mat = Cv2.ImRead(img);

                    PrintObject(detectLayer);
                    mat.Dispose();
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                });
        }

        #endregion

        #region 核心

        internal Layout<T> predictSingle(FileInfo image_path)
        {
            var id = Path.GetFileNameWithoutExtension(image_path.Name);
            var input = Cv2.ImRead(image_path.FullName, ImreadModes.Grayscale);
            var res = Predictor.Call(input, input, id).Result;
            return res;
        }

        internal void predictFolder(string imageFolder)
        {
            var images = new DirectoryInfo(imageFolder).GetFiles("*.bmp").ToList();
            images.ForEach(img =>
            {
                var imgFile = img.FullName;
                var annfile = imgFile.Replace("bmp", "txt");
                var layoutPredict = predictSingle(new FileInfo(imgFile));

                Layout<T>.SaveAnnotation(annfile, layoutPredict);
            });
        }

        internal void filterNotMatch(string workfolder, string outfolder)
        {
            var imgFolder = Path.Combine(workfolder, "Images");
            var annFolder = Path.Combine(workfolder, "Annotations");
            var allann = new DirectoryInfo(annFolder).GetFiles("*.txt").ToList();
            var error = 0;
            var i = 0;
            allann.ForEach(ann =>
            {
                //PrintObject(i++);
                var id = Path.GetFileNameWithoutExtension(ann.Name);
                var annfile = ann.FullName;
                var imgFile = Path.Combine(imgFolder, $"{id}.bmp");


                var layoutTarget = Layout<T>.LoadFromAnnotation(annfile);
                var layoutPredict = predictSingle(new FileInfo(imgFile));
                var isEqual = layoutPredict.Equals(layoutTarget);

                if (!isEqual)
                {
                    error++;
                    PrintObject($"{id} is {false}.");
                    var outImageFile = Path.Combine(outfolder, $"{id}.bmp");
                    var outAnnFile = Path.Combine(outfolder, $"{id}.txt");
                    File.Copy(imgFile, outImageFile);
                    Layout<T>.SaveAnnotation(outAnnFile, layoutPredict);
                }
            });
            PrintObject($"Error:{error}/All:{allann.Count}");
        }

        #endregion
    }
}