using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Module
{
    /// <summary>
    ///     产品轮廓预测器
    /// </summary>
    public class ModuleGrabPredictor
        : PointProcessor
    {
        private static readonly Lazy<ModuleGrabPredictor> lazy =
            new(() => new ModuleGrabPredictor());

        public ModuleGrabPredictor(
            int patternSize = 416)
            : base("ModuleGrabPredictor")
        {
            PatternSize = patternSize;
            Models = GetNet();
        }


        public List<Net> Models { set; get; }

        public static ModuleGrabPredictor Instance => lazy.Value;

        private int PatternSize { get; }

        private List<Point2d> TempPoint { set; get; }

        public List<Scalar> Colors => new() {Scalar.OrangeRed, Scalar.Blue};

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

        internal override Point2d Process(Mat input)
        {
            TempPoint = PreidctByDL(input.Clone()).ToList();

            var x = TempPoint.Select(a => a.X).Average();
            var y = TempPoint.Select(a => a.Y).Average();


            return new Point2d(x, y);
        }

        internal override Mat Draw(Mat mat, Point2d result)
        {
            mat = DrawPoint(mat, result.ToPoint(), PenColor, thickness: 3);
            var i = 0;
            TempPoint.ForEach(a => { DrawPoint(mat, a.ToPoint(), Colors[i++], thickness: 3); });
            return mat;
        }

        internal override bool CalScore(Point2d result)
        {
            return result != new Point2d(0, 0);
        }

        private Point2d[] PreidctByDL(Mat input)
        {
            var inputBlob = CvDnn.BlobFromImage(input,
                1F / 255,
                new Size(PatternSize, PatternSize),
                swapRB: false, crop: false);


            var result = Models.Select(a => PredictDL(a, inputBlob, input.Size())).ToArray();
            return result;
        }

        private Point2d PredictDL(Net model, Mat input, Size size)
        {
            model.SetInput(input);
            var result = model.Forward();
            result.GetArray(out float[] data);

            var final = new Point2d(
                data[0] * size.Width,
                data[1] * size.Height);

            return final;
        }

        [Obsolete]
        private Point2d PredictByCV(Mat input)
        {
            var range_start = 20;
            var range_end = 80;
            var range_step = 5;

            var count = (range_end - range_start) / range_step + 1;
            foreach (var i in Enumerable.Range(0, count))
            {
                Cv2.Threshold(input, input, range_end - range_step * i, 255, ThresholdTypes.Binary);

                var element = Cv2.GetStructuringElement(MorphShapes.Rect,
                    new Size(3, 3),
                    new Point(-1, -1));

                Cv2.MorphologyEx(input, input, MorphTypes.HitMiss, element, new Point(-1, -1));
                var Hierarchy = new Mat();
                Cv2.FindContours(input, out var cons, Hierarchy, RetrievalModes.List,
                    ContourApproximationModes.ApproxSimple);

                var d = cons.FirstOrDefault(a =>
                {
                    var rotatedRect = Cv2.MinAreaRect(a);
                    var dd = new[] {rotatedRect.Size.Width, rotatedRect.Size.Height};
                    var width = dd.Max();
                    var height = dd.Min();
                    var floats = new[] {width, height};
                    var final = floats.Max() > 85 && floats.Max() < 95 && floats.Min() > 68 && floats.Min() < 78;
                    return final;
                });
                if (d != null)
                {
                    var rotatedRect = Cv2.MinAreaRect(d);
                    var center = rotatedRect.Center;
                    return CvCvt.CvtToPoint2d(center);
                }
            }

            return new Point2d(0, 0);
        }
    }
}