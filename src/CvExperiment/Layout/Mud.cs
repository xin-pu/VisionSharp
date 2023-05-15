using System.Collections.Generic;
using OpenCvSharp;
using VisionSharp.Models.Category;
using VisionSharp.Processor;
using VisionSharp.Processor.LayoutDetectors;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment.Layout
{
    public class Mud : General<MudCategory>
    {
        public Mud(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        /// <summary>
        ///     对已经裁剪后的图像预测，将与标注不符的过滤
        /// </summary>
        [Fact]
        public void FilterError()
        {
            var workfolder = @"F:\CoolingMud";
            var outfolder = @"F:\CoolingMud\Error";
            filterNotMatch(workfolder, outfolder);
        }

        public override LayoutDetector<MudCategory> InitialPreidctor()
        {
            var onnx = @"F:\SaveModels\Yolo\mud.onnx";
            var argument = new LayoutArgument(new Size(12, 3), new Size(640, 640), 0.7);
            var layoutDlDetector = new LayoutDlDetector<MudCategory>(onnx, argument);
            layoutDlDetector.Colors = new Dictionary<MudCategory, Scalar>
            {
                [MudCategory.MissMud] = Scalar.White,
                [MudCategory.Other] = Scalar.Green
            };
            return layoutDlDetector;
        }
    }
}