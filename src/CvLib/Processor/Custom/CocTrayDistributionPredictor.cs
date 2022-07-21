using System.Collections.Generic;
using System.Linq;
using CVLib.Processor.Unit;
using OpenCvSharp;

namespace CVLib.Processor.Custom
{
    public class CocTrayDistributionPredictor : DistributionDetector
    {
        public CocTrayDistributionPredictor()
            : base("unload.onnx", new Size(3, 8))
        {
        }


        #region Method

        internal override Mat Draw(Mat mat, bool[,] result)
        {
            Enumerable.Range(0, PatternWidth).ToList().ForEach(c =>
            {
                var rect = Rects[c];
                var height = getBoardRectHeight(rect.Height);
                var heightWithInterval = getBoardRectHeightWithInterval(rect.Height);
                Enumerable.Range(0, PatternHeight).ToList().ForEach(r =>
                {
                    var boardRect = new Rect(rect.X, rect.Y + r * heightWithInterval, rect.Width, height);
                    DrawRect(mat, boardRect, result[r, c] ? PenColor : Scalar.GreenYellow, 20, 5);
                });
            });
            return mat;
        }

        #endregion

        #region Built-in

        internal List<Rect> Rects = new()
        {
            new Rect(371, 165, 1491, 3149),
            new Rect(1942, 141, 1463, 3146),
            new Rect(3527, 137, 1463, 3146)
        };

        internal double BoardHeight = 38;
        internal double IntervalHeight = 12;
        internal double IntervalRow => PatternHeight - 1;

        private int getBoardRectHeight(double height)
        {
            var unit = height / (PatternHeight + 1F * IntervalRow * IntervalHeight / BoardHeight);
            return (int) unit;
        }

        private int getBoardRectHeightWithInterval(double height)
        {
            var unit = height / (PatternHeight + 1F * IntervalRow * IntervalHeight / BoardHeight);
            return (int) (unit * (1 + 1F * IntervalHeight / BoardHeight));
        }

        #endregion
    }
}