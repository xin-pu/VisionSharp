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
    public class UnionUnload : General<ObjCategory>
    {
        public UnionUnload(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        public override LayoutDetector<ObjCategory> InitialPreidctor()
        {
            return new LayoutDlDetector<ObjCategory>(
                @"F:\SaveModels\Yolo\unload_2024.onnx",
                new Size(3,   8),
                new Size(640, 640),
                0.7);
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
            var workfolder = @"F:\COC Tray\Union UnloadTray\5291";
            var outfolder = @"F:\COC Tray\Union UnLoadTray Error";
            filterNotMatch(workfolder, outfolder);
        }

        /// <summary>
        ///     直接预测
        /// </summary>
        [Fact]
        public void PredictByFolder()
        {
            var workfolder = @"F:\COC Tray\New 2024\unloading\Images";
            predictFolder(workfolder);
        }

        #endregion

        #region 单例测试

        [Fact]
        public void Predict()
        {
            var layout = predictSingle(new FileInfo(@"F:\COC Tray\Union UnloadTray\Images\5206_1.bmp"));
            PrintObject(layout);
        }


        [Fact]
        public void UnLoadAnswer()
        {
            var answer = Layout<ObjCategory>.LoadFromAnnotation(@"F:\COC Tray\Union LoadTray\Annotations\1_0063.txt");
            PrintObject(answer);
        }

        #endregion
    }
}