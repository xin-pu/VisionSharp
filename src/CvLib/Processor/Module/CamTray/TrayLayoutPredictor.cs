using System;
using System.Linq;
using CVLib.Processor.Unit.Painter;
using CVLib.Utils;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Module
{
    public class TrayLayoutPredictor : Processor<Mat, DetectLayout>
    {
        public TrayLayoutPredictor(string modelFile,
            Size patternSize,
            Size inputSize,
            double scoreThreshold = 0.8)
            : base("TrayLayoutPredictor")
        {
            Net = Net.ReadNetFromONNX(modelFile);


            PatternSize = patternSize;
            InputSize = inputSize;
            ScoreThreshoold = scoreThreshold;
        }

        internal Net Net { get; }
        public Size PatternSize { get; }

        public Size InputSize { set; get; }

        public double ScoreThreshoold { set; get; }

        internal override DetectLayout Process(Mat input)
        {
            var inputBlob = CvDnn.BlobFromImage(input,
                1d / 255,
                InputSize,
                swapRB: false,
                crop: false);
            Net.SetPreferableBackend(Backend.OPENCV);
            Net.SetPreferableTarget(Target.CPU);
            Net.SetInput(inputBlob);


            Net.EnableFusion(false);
            var res = Net.Forward();


            var final = Decode(res);
            inputBlob.Dispose();
            res.Dispose();
            GC.Collect();
            GC.WaitForFullGCComplete();

            return final;
        }

        internal override bool CalScore(DetectLayout result)
        {
            return result.GetReliability();
        }

        /// <summary>
        ///     解码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal DetectLayout Decode(Mat result)
        {
            var height = PatternSize.Height;
            var width = PatternSize.Width;
            var result_ch = result.Reshape(1, height * width, 2);
            var array = CvCvt.CvtToArray(result_ch);
            var res = new DetectLayout(height, width, ScoreThreshoold);


            Enumerable.Range(0, height * width).ToList().ForEach(d =>
            {
                var negativeScore = CvMath.Sigmoid(array[d, 0]);
                var positiveScore = CvMath.Sigmoid(array[d, 1]);
                res.SetStatus(d / width, d % width, positiveScore, negativeScore);
            });
            return res;
        }

        internal override Mat Draw(Mat mat, DetectLayout result)
        {
            return new TrayLayoutPainter().CallLight(new Tuple<Mat, DetectLayout>(mat, result));
        }
    }
}