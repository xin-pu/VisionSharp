using OpenCvSharp;
using VisionSharp.Models.Layout;
using VisionSharp.Processor.Transform;

namespace VisionSharp.Processor.LayoutDetectors
{
    public class MultiLayoutDetectorDl<T> : FeatureExtractor<Layout<T>[]> where T : Enum
    {
        public MultiLayoutDetectorDl(
            string modelFile,
            LayoutArgument layoutArgument,
            RotatedRect[] rois) : base("MultiLayoutDetectorDl")
        {
            LayoutDetector = new LayoutDlDetector<T>(modelFile, layoutArgument);
            Rois = rois;
        }

        public LayoutDetector<T> LayoutDetector { internal set; get; }

        public RotatedRect[] Rois { internal set; get; }

        internal override Layout<T>[] Process(Mat input)
        {
            var rotatedRectCroppers = Rois
                .Select(a => new RotatedRectCropper(a))
                .ToList();
            var roiMat = rotatedRectCroppers
                .Select(r => r.Call(input))
                .ToList();
            var result = new List<Layout<T>>();
            roiMat.ForEach(m =>
            {
                var res = LayoutDetector.Call(m);
                result.Add(res.Rotate());
            });

            return result.ToArray();
        }
    }
}