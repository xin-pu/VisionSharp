using System.IO;
using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Models.Layout;
using VisionSharp.Processor;
using VisionSharp.Processor.LayoutDetectors;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment.Layout
{
    public class UnionLoad : General<ObjCategory>
    {
        public UnionLoad(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        public override LayoutDetector<ObjCategory> InitialPreidctor()
        {
            return new LayoutDlDetector<ObjCategory>(
                @"F:\SaveModels\Yolo\load_2024.onnx",
                new Size(2,   8),
                new Size(640, 640),
                0.6);
        }

        [Fact]
        public void load()
        {
            var p = new LayoutDlDetector<ObjCategory>("load.onnx", new Size(2, 8), new Size(416, 416), 0.6);
            var res = Cv2.ImRead(@"F:\COC Tray\Union LoadTray\Images\1_0001.bmp", ImreadModes.Grayscale);
            var d = p.Call(res);
        }


        #region 数据集准备

        [Fact]
        public void RenameFolderFiles()
        {
            var workfolder = @"F:\COC Tray\New 2024\loading";
            var searchPattern = "*.bmp";
            var preName = "4";
            RenameFiles(workfolder, searchPattern, preName);
        }


        /// <summary>
        ///     转换目录下图片格式
        /// </summary>
        [Fact]
        public void ChangeImageFormat()
        {
            ChangeImagePNG2BMP(@"F:\COC Tray\New 2024\loading");
        }


        /// <summary>
        ///     绘制标注结果
        /// </summary>
        [Fact]
        public void CheckDataSet()
        {
            GeneraitAnnToImage(@"F:\COC Tray\New 2024\loading");
        }

        #endregion

        #region 验证

        /// <summary>
        ///     对已经裁剪后的图像预测，将与标注不符的过滤
        /// </summary>
        [Fact]
        public void FilterError()
        {
            var workfolder = @"F:\COC Tray\Union loadTray";
            var outfolder = @"F:\COC Tray\Union LoadTray Error";
            filterNotMatch(workfolder, outfolder);
        }

        /// <summary>
        ///     直接预测
        /// </summary>
        [Fact]
        public void PredictByFolder()
        {
            var workfolder = @"F:\COC Tray\New 2024\loading";
            predictFolder(workfolder);
        }

        #endregion

        #region 单例测试

        [Fact]
        public void Predict()
        {
            var layout = predictSingle(new FileInfo(@"F:\COC Tray\Union LoadTray\Images\1_0006.bmp"));
            PrintObject(layout);
        }


        [Fact]
        public void LoadAnswer()
        {
            var answer = Layout<ObjCategory>.LoadFromAnnotation(@"F:\COC Tray\Union LoadTray\Annotations\1_0063.txt");
            PrintObject(answer);
        }

        #endregion
    }
}