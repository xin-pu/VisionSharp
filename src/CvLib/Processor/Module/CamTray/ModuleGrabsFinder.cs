using System.Collections.Generic;
using System.Linq;
using CVLib.Models;
using CVLib.Processor.Unit;
using OpenCvSharp;

namespace CVLib.Processor.Module
{
    /// <summary>
    ///     多产品轮廓以及抓取点预测器
    /// </summary>
    public class ModuleGrabsFinder
        : Processor<Mat, List<DetectObject>>
    {
        public ModuleGrabsFinder(
            string name = "ModuleGrabsFinder")
            : base(name)
        {
            var targetSize = new Size(350, 150);

            NormalPretreatment = new NormalPretreatment(ThresholdTypes.Otsu, 122, 3, MorphShapes.Rect);
            ModuleDetector = new ObjectsDetector(targetSize, 2.4, 0.2, "ModuleDetector");
        }

        public ObjectsDetector ModuleDetector { set; get; }
        public NormalPretreatment NormalPretreatment { set; get; }

        internal override List<DetectObject> Process(Mat input)
        {
            input = new Sharpen().CallLight(input);
            var bitewise = new Mat();
            Cv2.BitwiseNot(input, bitewise);
            var temp = NormalPretreatment.CallLight(bitewise);

            var allDetectObjects = ModuleDetector
                .CallLight(temp);

            var moduleRotatedRects = allDetectObjects
                .Where(a => a.IsConfidence)
                .ToList();

            var mats = moduleRotatedRects
                .Select(m =>
                {
                    var rect = m.RotatedRect.BoundingRect();
                    var mat = input[rect];
                    return mat;
                });

            var points = ModuleGrabMultiPredictor.Instance
                .CallLight(mats)
                .ToList();

            var zip = moduleRotatedRects
                .Zip(points, (detectObj, point) => (detectObj, point))
                .ToList();

            zip.ForEach(pair =>
            {
                var rect = pair.detectObj.RotatedRect.BoundingRect();
                pair.detectObj.GrabPoint = rect.TopLeft + pair.point;
            });

            return moduleRotatedRects;
        }

        internal override Mat Draw(Mat mat, List<DetectObject> detectObjects)
        {
            detectObjects
                .Where(a => a.IsConfidence)
                .ToList()
                .ForEach(a =>
                {
                    DrawRotatedRect(mat, a.RotatedRect, PenColor);
                    DrawPoint(mat, a.GrabPoint.ToPoint(), PenColor);
                });
            return mat;
        }
    }
}