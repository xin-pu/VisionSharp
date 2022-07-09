using OpenCvSharp;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     Pretreatment by Threshold + Close + Open
    ///     Close and Open use same element size
    /// </summary>
    public class NormalPretreatment
        : Processor<Mat, Mat>
    {
        public NormalPretreatment(ThresholdTypes thresholdType, double threshold, int elementSize,
            MorphShapes elementShape,
            string name = "NormalPretreatment")
            : base(name)
        {
            ThresholdType = thresholdType;
            Threshold = threshold;
            ElementSize = elementSize;
            ElementShape = elementShape;
        }

        public ThresholdTypes ThresholdType { set; get; }
        public double Threshold { set; get; }

        public int ElementSize { set; get; }
        public MorphShapes ElementShape { set; get; }

        internal override Mat Process(Mat input)
        {
            var mat = input.Clone();
            Cv2.Threshold(mat, mat, Threshold, 255, ThresholdType);

            var element = Cv2.GetStructuringElement(
                ElementShape,
                new Size(ElementSize, ElementSize),
                new Point(-1, -1));

            Cv2.MorphologyEx(mat, mat, MorphTypes.Close, element, new Point(-1, -1));

            Cv2.MorphologyEx(mat, mat, MorphTypes.Open, element, new Point(-1, -1));

            return mat;
        }
    }
}