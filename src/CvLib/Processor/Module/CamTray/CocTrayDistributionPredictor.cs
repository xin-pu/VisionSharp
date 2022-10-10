using System;
using System.Collections.Generic;
using System.Linq;
using CVLib.Processor.Unit;
using OpenCvSharp;

namespace CVLib.Processor.Module
{
    public class CocTrayDistributionPredictor : Processor<Mat, bool[,]>
    {
        internal double BoardHeight = 38;
        internal double IntervalHeight = 12;

        public CocTrayDistributionPredictor(string modelName, Size pattern_size) : base("CocTrayDistributionPredictor")
        {
            PatterSize = pattern_size;
            CocBoardRecognizer = new CocBoardRecognizer(modelName);
        }

        public CocBoardRecognizer CocBoardRecognizer { set; get; }

        public List<Rect> Rects { set; get; }
            = new()
            {
                new Rect(1163, 62, 1491, 3149),
                new Rect(2717, 54, 1463, 3146)
            };

        internal double IntervalRow => PatterSize.Height - 1;


        public Size PatterSize { get; }


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
            if (Rects.Count != PatterSize.Width)
                throw new ArgumentException();

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
                    var status = CocBoardRecognizer.CallLight(mat);
                    res[r, c] = status == CocBoardStatus.Object;
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