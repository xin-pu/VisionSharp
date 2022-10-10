using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CVLib.Models;
using CVLib.Utils;
using MathNet.Numerics.Random;
using OpenCvSharp;

namespace CVLib.Processor.Unit.Painter
{
    /// <summary>
    ///     绘制用于目标检测的图像，减少前期数照片收集和标注的工作
    ///     返回再图片上增加的目标物信息，和生成的图像
    /// </summary>
    public class ObjAnnotationCreator : Processor<int, Annotation[]>
    {
        /// <summary>
        /// </summary>
        /// <param name="objInput">用于绘制再图像上的目标物，构造时便传入</param>
        public ObjAnnotationCreator(
            string objFolder,
            string backGroundFolder,
            string outFolder)
            : base("ObjAnnotationCreator")
        {
            ObjInput = InitialObj(objFolder);
            BackGround = InitialBackGround(backGroundFolder);
            OutFolder = outFolder;

            if (!Directory.Exists(OutImageFolder))
                Directory.CreateDirectory(OutImageFolder);
            if (!Directory.Exists(OutAnnFolder))
                Directory.CreateDirectory(OutAnnFolder);
        }

        public Dictionary<string, Mat[]> ObjInput { get; }

        public Mat[] BackGround { get; }

        public string OutFolder { protected set; get; }

        public string OutImageFolder => Path.Combine(OutFolder, "images");

        public string OutAnnFolder => Path.Combine(OutFolder, "annotations");

        public double MatRation { set; get; } = 3;

        public void ChangeOutFolder(string folder)
        {
            OutFolder = folder;

            if (!Directory.Exists(OutImageFolder))
                Directory.CreateDirectory(OutImageFolder);
            if (!Directory.Exists(OutAnnFolder))
                Directory.CreateDirectory(OutAnnFolder);
        }

        internal override Annotation[] Process(int dataLength)
        {
            Enumerable.Range(1, dataLength).ToList().ForEach(i => CreateSingle(i));
            return new Annotation[] { };
        }

        internal Annotation CreateSingle(int index)
        {
            var randomBackGroudIndex = SystemRandomSource.Default.Next(0, BackGround.Length);
            var backGround = BackGround[randomBackGroudIndex].Clone();
            var image_width = backGround.Width;
            var image_heiht = backGround.Height;

            var listObj = new List<AnnObject>();
            var count = SystemRandomSource.Default.Next(5, 15);
            Enumerable.Range(0, count).ToList().ForEach(_ =>
            {
                /// Step 1，随机获取检测目标
                var name = getRandomObj(out var randomObjMat);

                /// Step 2,随机缩放检测目标  三尺度，0.5~2
                randomResize(ref randomObjMat);

                /// Step 3, 随机旋转检测目标
                var bbox = randomRotated(randomObjMat);

                /// Step 4 随机定位目标中心点
                var rect = randomCenter(image_width, image_heiht, bbox);
                if (listObj.All(a => CvIOU.getIoU(a.Rect, rect) <= 0) || listObj.Count == 0)
                {
                    /// Step 5 绘制目标
                    var mask = new Mat();
                    Cv2.CvtColor(randomObjMat, mask, ColorConversionCodes.BGR2GRAY);
                    Cv2.Threshold(mask, mask, 250, 255, ThresholdTypes.Binary);
                    var mask1 = 255 - mask;
                    var imageRoi = backGround[rect];
                    randomObjMat.CopyTo(imageRoi, mask1);

                    listObj.Add(new AnnObject(name, rect.TopLeft, rect.BottomRight));


                    mask.Dispose();
                    mask1.Dispose();
                    imageRoi.Dispose();
                }

                randomObjMat.Dispose();
            });
            var ind = index.ToString().PadLeft(6, '0');
            var annotation = new Annotation
            {
                Folder = OutFolder,
                FileName = $"{ind}.png",
                Size = new Size(image_width, image_heiht),
                ObjectInfos = listObj
            };
            annotation.SaveXml(Path.Combine(OutAnnFolder, $"{ind}.xml"));
            //var path = Path.Combine(OutImageFolder, $"{ind}.png");
            //backGround.SaveImage(path);
            backGround.Dispose();
            GC.Collect();
            GC.WaitForFullGCComplete();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            return annotation;
        }

        private Dictionary<string, Mat[]> InitialObj(string folder)
        {
            var res = new Dictionary<string, Mat[]>();
            var dire = new DirectoryInfo(folder);
            dire.GetDirectories().ToList().ForEach(f =>
            {
                var objType = f.Name;
                var objs = f.GetFiles("*.png");
                res[objType] = objs.Select(a => Cv2.ImRead(a.FullName)).ToArray();
            });
            return res;
        }

        private Mat[] InitialBackGround(string folder)
        {
            var backGrounds = new DirectoryInfo(folder).GetFiles("*.png");
            return backGrounds.Select(f => Cv2.ImRead(f.FullName)).ToArray();
        }

        private static Rect randomCenter(int image_width, int image_heiht, Rect bbox)
        {
            var half_x = bbox.Width / 2;
            var half_y = bbox.Height / 2;
            var randomX = (int) (SystemRandomSource.Default.NextDouble() * (image_width - bbox.Width)) +
                          half_x;
            var randomY = (int) (SystemRandomSource.Default.NextDouble() * (image_heiht - bbox.Height)) +
                          half_y;
            var rect = new Rect(new Point(randomX - half_x, randomY - half_y), bbox.Size);
            return rect;
        }

        private static Rect randomRotated(Mat randomObjMat)
        {
            var randomAngle = SystemRandomSource.Default.NextDouble() * 360;
            var rot = Cv2.GetRotationMatrix2D(
                new Point2f((randomObjMat.Width - 1) / 2f, (randomObjMat.Height - 1) / 2f), randomAngle, 1);
            var bbox = new RotatedRect(new Point2f(), new Size2f(randomObjMat.Width * 1f, randomObjMat.Height * 1f),
                (float) randomAngle).BoundingRect();

            rot.At<double>(0, 2) += bbox.Width * 0.5 - randomObjMat.Width * 0.5;
            rot.At<double>(1, 2) += bbox.Height * 0.5 - randomObjMat.Height * 0.5;

            Cv2.WarpAffine(randomObjMat, randomObjMat, rot, bbox.Size, borderValue: Scalar.White);
            return bbox;
        }

        private void randomResize(ref Mat randomObjMat)
        {
            var sizeScale = SystemRandomSource.Default.NextDouble() * 0.5 + SystemRandomSource.Default.Next(1, 4);
            var objSize = new Size(sizeScale * randomObjMat.Width, sizeScale * randomObjMat.Height);
            randomObjMat = randomObjMat.Resize(objSize);
        }

        private string getRandomObj(out Mat randomObjMat)
        {
            var randomType = SystemRandomSource.Default.Next(0, ObjInput.Count);
            var pair = ObjInput.ToList()[randomType];

            var name = pair.Key;
            var randomIndex = SystemRandomSource.Default.Next(0, pair.Value.Length);
            randomObjMat = pair.Value[randomIndex].Clone();
            return name;
        }
    }
}