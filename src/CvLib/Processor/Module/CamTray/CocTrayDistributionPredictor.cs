using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace CVLib.Processor.Module
{
    public class CocTrayDistributionPredictor : Processor<Mat, bool[,]>
    {
        private readonly List<Rect> Rects = new()
        {
            new Rect(1163, 62, 1491, 3149),
            new Rect(2717, 54, 1463, 3146)
        };

        private readonly Size size = new(300, 80);
        internal double BoardHeight = 38;
        internal double IntervalHeight = 12;

        public CocTrayDistributionPredictor(string modelFileName,
            Size pattern_size) : base("Test")
        {
            Model = LogisticRegression.Load(modelFileName);
            PatterSize = pattern_size;
            ModelFileName = modelFileName;
        }

        internal double IntervalRow => PatterSize.Height - 1;

        public string ModelFileName { get; }
        public Size PatterSize { get; }
        public StatModel Model { get; }


        private int getBoardRectHeight(double height)
        {
            var unit = height / (PatterSize.Height + 1F * IntervalRow * IntervalHeight / BoardHeight);
            return (int) unit;
        }

        private int getBoardRectHeightWithInterval(double height)
        {
            var unit = height / (PatterSize.Height + 1F * IntervalRow * IntervalHeight / BoardHeight);
            return (int) (unit * (1 + 1F * IntervalHeight / BoardHeight));
        }

        internal override bool[,] Process(Mat input)
        {
            var res = new bool[PatterSize.Height, PatterSize.Width];

            Enumerable.Range(0, PatterSize.Width).ToList().ForEach(c =>
            {
                var rect = Rects[c];
                var height = getBoardRectHeight(rect.Height);
                var heightWithInterval = getBoardRectHeightWithInterval(rect.Height);
                Enumerable.Range(0, PatterSize.Height).ToList().ForEach(r =>
                {
                    var boardRect = new Rect(rect.X, rect.Y + r * heightWithInterval,
                        rect.Width, height);
                    var mat = input[boardRect];
                    var matRect = mat.Resize(size);

                    matRect.GetArray(out byte[] datas);

                    var testFeature = new Mat(1, size.Height * size.Width, MatType.CV_32F,
                        datas.Select(a => (float) a).ToArray());
                    var tag = (int) Model.Predict(testFeature);
                    res[r, c] = tag == 1;
                });
            });
            return res;
        }

        internal override Mat Draw(Mat mat, bool[,] result)
        {
            Enumerable.Range(0, PatterSize.Width).ToList().ForEach(c =>
            {
                var rect = Rects[c];
                var height = getBoardRectHeight(rect.Height);
                var heightWithInterval = getBoardRectHeightWithInterval(rect.Height);
                Enumerable.Range(0, PatterSize.Height).ToList().ForEach(r =>
                {
                    var boardRect = new Rect(rect.X, rect.Y + r * heightWithInterval, rect.Width, height);
                    DrawRect(mat, boardRect, result[r, c] ? PenColor : Scalar.GreenYellow, 20, 5);
                });
            });
            return mat;
        }
    }
}