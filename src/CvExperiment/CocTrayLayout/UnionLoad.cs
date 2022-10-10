using System.IO;
using CVLib.Models;
using CVLib.Processor.Module;
using OpenCvSharp;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment.CocTrayLayout
{
    public class UnionLoad : General
    {
        public UnionLoad(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        public override TrayLayoutPredictor InitialPreidctor()
        {
            return new TrayLayoutPredictor(
                @"F:\SaveModels\Yolo\load416.onnx",
                new Size(2, 8),
                new Size(416, 416),
                0.7);
        }

        [Fact]
        public void load()
        {
            var p = new TrayDistributionPredictor("load.onnx", new Size(2, 8));
            var res = Cv2.ImRead(@"F:\COC Tray\Union LoadTray\Images\1_0001.bmp", ImreadModes.Grayscale);
            var d = p.CallLight(res);
        }


        #region 数据集准备

        [Fact]
        public void RenameFolderFiles()
        {
            var workfolder = @"F:\COC Tray\Union UnloadTray\5291\Images";
            var searchPattern = "*.bmp";
            var preName = "";
            RenameFiles(workfolder, searchPattern, preName);
        }


        /// <summary>
        ///     转换目录下图片格式
        /// </summary>
        [Fact]
        public void ChangeImageFormat()
        {
            ChangeImagePNG2BMP(@"F:\COC Tray\Union UnloadTray\5291\Images");
        }


        /// <summary>
        ///     绘制标注结果
        /// </summary>
        [Fact]
        public void CheckDataSet()
        {
            GeneraitAnnToImage(@"F:\COC Tray\Union UnloadTray");
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
            var workfolder = @"E:\My Temp\unload";
            predictFolder(workfolder);
        }

        #endregion

        #region 单例测试

        [Fact]
        public void Predict()
        {
            var layout = predictSingle(new FileInfo(@"F:\COC Tray\Union LoadTray\Images\1_0006.bmp"));
            PrintObject(layout);
            PrintObject(layout.ToScoreString());
        }


        [Fact]
        public void LoadAnswer()
        {
            var answer = DetectLayout.LoadFromAnnotation(@"F:\COC Tray\Union LoadTray\Annotations\1_0063.txt");
            PrintObject(answer);
        }

        #endregion
    }
}