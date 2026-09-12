using FluentAssertions;
﻿using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Category;
using VisionSharp.Processor.ObjectDetector;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest.Yolo
{
    /// <summary>
    ///     依赖本机模型与图像资产（F:/E: 盘），CI 中按 Category!=RequiresLocalAssets 过滤
    /// </summary>
    [Trait("Category", "RequiresLocalAssets")]
    public class Yolo7Test : AbstractTest
    {
        internal string QRModelPath = @"F:\SaveModels\Yolo\qr\qr_best.onnx";
        internal string VocModelPath = @"F:\SaveModels\Yolo\Voc\voc.onnx";
        internal string RaccoonTinyModelPath = @"F:\SaveModels\Yolo\raccoon\raccoon.onnx";

        public Yolo7Test(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void ObjDetectorTest()
        {
            using var net = CvDnn.ReadNetFromOnnx(QRModelPath);
            net.Should().NotBeNull();

            var layersNames = net!.GetLayerNames();

            foreach (var layerName in layersNames)
            {
                if (layerName == null)
                {
                    continue;
                }

                var id = net.GetLayerId(layerName);
                PrintObject($"{id}:\t{layerName}");
            }
        }

        [Fact]
        public void QrDetectorTest()
        {
            using var d = new ObjDetYolo7<QrCategory>(QRModelPath)
            {
                Confidence = 0.6f,
                IouThreshold = 0.5f
            };
            var image = @"F:\QR\JPEGImages\0179583169.jpg";
            using var mat = Cv2.ImRead(image);
            using var res = d.Call(mat, mat);
            PrintObject(res.Result);
        }

        [Fact]
        public void RaccoonDetectorTest()
        {
            using var d = new ObjDetYolo7<Raccoon>(@"E:\ObjectDetect\yolov7_pytorch\logs\best_epoch_weights.onnx")
            {
                Confidence = 0.5f,
                IouThreshold = 0.5f
            };
            var image = @"E:\OneDrive\Pictures\Saved Pictures\raccoon\Racccon (1).jpg";
            using var mat = Cv2.ImRead(image);
            using var res = d.Call(mat, mat);
            PrintObject(res.Result);
        }

        [Fact]
        public void VocDetectorTest()
        {
            using var d = new ObjDetYolo7<VocCategory>(VocModelPath)
            {
                Confidence = 0.4f,
                IouThreshold = 0.5f
            };
            var image = @"E:\OneDrive\Pictures\Saved Pictures\voc\dog.jpg";
            using var mat = Cv2.ImRead(image);
            using var res = d.Call(mat, mat);
            PrintObject(res.Result);
        }
    }
}
