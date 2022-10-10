using System;
using System.ComponentModel;
using System.Linq;
using CVLib.Models;
using CVLib.Utils;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     分布预测
    /// </summary>
    public class DistributionDetector : MatProcessor<bool[,]>
    {
        public DistributionDetector(string modelFile, Size patternSize)
            : base("DistributionDetector")
        {
            Net = Net.ReadNetFromONNX(modelFile);
            PatternSize = new CvSize(patternSize);
        }


        #region Built-in

        [Category("Built-in")] public CvSize PatternSize { get; }
        internal Net Net { get; }
        internal int PatternWidth => (int) PatternSize.Width;
        internal int PatternHeight => (int) PatternSize.Height;

        #endregion

        #region Option

        private CvSize _inputSize = new(416, 416);
        private double _threshold = 0.8;

        [Category("Option")]
        public CvSize InputSize
        {
            set => Set(ref _inputSize, value);
            get => _inputSize;
        }

        [Category("Option")]
        public double Threshold
        {
            set => Set(ref _threshold, value);
            get => _threshold;
        }

        #endregion

        #region Method

        internal override bool[,] Process(Mat input)
        {
            var gratMat = new Mat();
            if (input.Channels() != 3)
                gratMat = input;
            else
                Cv2.CvtColor(input, gratMat, ColorConversionCodes.BGR2GRAY);

            try
            {
                var inputBlob = CvDnn.BlobFromImage(gratMat,
                    1d / 255,
                    InputSize.Size,
                    swapRB: false,
                    crop: false);

                Net.SetInput(inputBlob);
                var res = Net.Forward();
                var final = Decode(res);
                return final;
            }
            catch (Exception)
            {
                return new bool[PatternHeight, PatternWidth];
            }
        }

        /// <summary>
        ///     解码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool[,] Decode(Mat result)
        {
            var height = PatternHeight;
            var width = PatternWidth;

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
            var height = PatternHeight;
            var width = PatternWidth;

            var pointStart = new Point(186, 278);
            Enumerable.Range(0, height).ToList().ForEach(r =>
                Enumerable.Range(0, width).ToList().ForEach(c =>
                {
                    if (result[r, c])
                        DrawPoint(mat, pointStart + new Point(c * 182, r * 417), PenColor);
                }));
            return mat;
        }

        #endregion
    }
}