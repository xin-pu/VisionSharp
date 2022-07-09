using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Module
{
    public class ModuleGrabMultiPredictor
        : Processor<IEnumerable<Mat>, IEnumerable<Point2d>>
    {
        private static readonly Lazy<ModuleGrabMultiPredictor> lazy =
            new(() => new ModuleGrabMultiPredictor());

        public ModuleGrabMultiPredictor(
            int patternSize = 416)
            : base("ModuleGrabMultiPredictor")
        {
            PatternSize = patternSize;
            Models = GetNet();
        }

        public static ModuleGrabMultiPredictor Instance => lazy.Value;

        private int PatternSize { get; }

        public List<Net> Models { set; get; }

        private List<Net> GetNet()
        {
            var onnxFile = new DirectoryInfo(Environment.CurrentDirectory)
                .GetFiles("*.onnx")
                .OrderByDescending(a => a.CreationTime)
                .ToList();
            return onnxFile
                .Select(f => CvDnn.ReadNetFromOnnx(f.FullName))
                .ToList();
        }


        internal override IEnumerable<Point2d> Process(IEnumerable<Mat> input)
        {
            var matsInput = input.ToArray();
            var result = Models
                .Select(net => PredictDL(net, matsInput))
                .ToList();

            var finalRes = Enumerable
                .Range(0, matsInput.Count())
                .ToList()
                .Select(i =>
                {
                    var size = matsInput[i].Size();
                    var pointNets = result
                        .Select(d => new Point2d(d[i].X * size.Width, d[i].Y * size.Height))
                        .ToList();
                    return ChooseBest(pointNets);
                }).ToList();
            return finalRes;
        }


        private Point2d[] PredictDL(Net model, Mat[] input)
        {
            var inputBlobs = CvDnn.BlobFromImages(input,
                1F / 255,
                new Size(PatternSize, PatternSize),
                swapRB: false, crop: false);

            model.SetInput(inputBlobs);
            var result = model.Forward();
            return CvCvt.CvtToPoint2ds(result);
        }

        /// <summary>
        ///     Choose Best Predict Point by Average
        /// </summary>
        /// <param name="predictPoint2ds"></param>
        /// <returns></returns>
        private Point2d ChooseBest(IEnumerable<Point2d> predictPoint2ds)
        {
            var allPoints = predictPoint2ds.ToArray();
            var x = allPoints.Select(a => a.X).Average();
            var y = allPoints.Select(a => a.Y).Average();
            return new Point2d(x, y);
        }
    }
}