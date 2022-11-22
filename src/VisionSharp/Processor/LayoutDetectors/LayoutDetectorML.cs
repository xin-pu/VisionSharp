using OpenCvSharp;
using OpenCvSharp.ML;
using VisionSharp.Models.Category;
using VisionSharp.Models.Layout;
using VisionSharp.Utils;

namespace VisionSharp.Processor.LayoutDetectors
{
    public class LayoutDetectorMl : LayoutDetector<MudCategory>
    {
        public LayoutDetectorMl(string modelfile, LayoutArgument layoutArgument) : base(layoutArgument)
        {
            Model = RTrees.Load(modelfile);
        }

        public RTrees Model { get; }
        public int Row1St { set; get; } = 100;
        public int Row2Nd { set; get; } = 1420;

        internal override Layout<MudCategory> Process(Mat input)
        {
            var height = LayoutArgument.LayoutPattern.Height;
            var width = LayoutArgument.LayoutPattern.Width;
            var scoreThreshold = LayoutArgument.ScoreThreshold;

            var mat = GetFeatures(input);
            var predict = mat.Select(m => Predict(m)).ToList();

            var res = new Layout<MudCategory>(LayoutArgument.LayoutPattern.Height,
                LayoutArgument.LayoutPattern.Width, 1);

            Enumerable.Range(0, height * width).ToList().ForEach(d =>
            {
                var row = d / width;
                var col = d % width;

                var activeScores = predict[d];
                res.UpdateScore(row, col, activeScores, scoreThreshold);
            });
            return res;
        }


        private List<Mat> GetFeatures(Mat mat)
        {
            var width = LayoutArgument.InputPattern.Width;
            var height = LayoutArgument.InputPattern.Height;

            var row = LayoutArgument.LayoutPattern.Height;
            var column = LayoutArgument.LayoutPattern.Width;
            var start = new[] {Row1St, Row2Nd};
            var mats = new List<Mat>();
            foreach (var h in Enumerable.Range(0, row))
            {
                foreach (var w in Enumerable.Range(0, column))
                {
                    var matRoi = mat[new Rect(width * w, start[h], width, height)];

                    mats.Add(matRoi);
                }
            }

            return mats;
        }

        private double[] Predict(Mat single)
        {
            var height = LayoutArgument.InputPattern.Height;
            var width = LayoutArgument.InputPattern.Width;
            var data = CvCvt.CvtToFloatArray(single);
            var trainFeatures = new Mat(1, height * width, MatType.CV_32F, data);
            var detectedClass = (int) Model.Predict(trainFeatures);
            return detectedClass == 1
                ? new double[] {1, 0}
                : new double[] {0, 1};
        }

        internal override Mat Draw(Mat mat, Layout<MudCategory> result, bool reliability)
        {
            var height = LayoutArgument.InputPattern.Height;
            mat = DrawRect(mat, new Rect(0, Row1St, mat.Width, height), Scalar.OrangeRed);
            mat = DrawRect(mat, new Rect(0, Row2Nd, mat.Width, height), Scalar.OrangeRed);
            return base.Draw(mat, result, reliability);
        }
    }
}