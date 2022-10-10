using System;
using System.Linq;
using CVLib.Utils;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Module
{
    public class TrayDistributionPredictor : Processor<Mat, bool[,]>
    {
        /// <summary>
        ///     用于预测分布的通用处理器
        ///     基于ONNX
        /// </summary>
        /// </summary>
        /// <param name="modelFile"></param>
        /// <param name="patternSize"></param>
        public TrayDistributionPredictor(string modelFile, Size patternSize, double threshold = 0.8)
            : base("TrayDistributionPredictor")
        {
            Net = Net.ReadNetFromONNX(modelFile);
            PatternSize = patternSize;
            Threshold = threshold;
        }


        internal Net Net { get; }
        public Size PatternSize { get; }

        public Size InputSize { set; get; } = new(416, 416);

        public double Threshold { set; get; }

        internal override bool[,] Process(Mat input)
        {
            try
            {
                var inputBlob = CvDnn.BlobFromImage(input,
                    1d / 255,
                    InputSize,
                    swapRB: false,
                    crop: false);

                Net.SetInput(inputBlob);
                var res = Net.Forward();
                var final = Decode(res);
                inputBlob.Dispose();
                res.Dispose();
                GC.Collect();
                GC.WaitForFullGCComplete();
                return final;
            }
            catch (Exception)
            {
                return new bool[PatternSize.Height, PatternSize.Width];
            }
        }

        /// <summary>
        ///     解码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool[,] Decode(Mat result)
        {
            var height = PatternSize.Height;
            var width = PatternSize.Width;
            var result_ch = result.Reshape(1, height, width);
            var array = CvCvt.CvtToArray(result_ch);
            var res = new bool[height, width];
            Enumerable.Range(0, height).ToList()
                .ForEach(r => Enumerable.Range(0, width).ToList()
                    .ForEach(c => res[r, c] = CvMath.Sigmoid(array[r, c]) > Threshold));
            return res;
        }

        internal override Mat Draw(Mat mat, bool[,] result)
        {
            var pointStart = new Point(186, 278);
            Enumerable.Range(0, PatternSize.Height).ToList().ForEach(r =>
                Enumerable.Range(0, PatternSize.Width).ToList().ForEach(c =>
                {
                    if (result[r, c])
                        DrawPoint(mat, pointStart + new Point(c * 182, r * 417), PenColor, thickness: 50);
                }));
            return mat;
        }
    }
}