using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Layout;
using VisionSharp.Utils;

namespace VisionSharp.Processor.LayoutDetectors
{
    public class LayoutDlDetector<T> : LayoutDetector<T> where T : Enum
    {
        public LayoutDlDetector(string modelFile, LayoutArgument layoutArgument)
            : base(layoutArgument)
        {
            Net = Net.ReadNetFromONNX(modelFile);
        }


        /// <summary>
        ///     通过深度学习构建的布局检测器
        /// </summary>
        /// <param name="modelFile"></param>
        /// <param name="patternSize"></param>
        /// <param name="inputSize"></param>
        /// <param name="scoreThreshold"></param>
        public LayoutDlDetector(string modelFile,
            Size patternSize,
            Size inputSize,
            double scoreThreshold = 0.8)
            : this(modelFile, new LayoutArgument(patternSize, inputSize, scoreThreshold))
        {
        }

        /// <summary>
        ///     深度模型
        /// </summary>
        public Net Net { internal set; get; }


        internal override Layout<T> Process(Mat input)
        {
            var inputBlob = CvDnn.BlobFromImage(input,
                1d / 255,
                LayoutArgument.InputPattern,
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


        /// <summary>
        ///     解码
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal Layout<T> Decode(Mat result)
        {
            var height = LayoutArgument.LayoutPattern.Height;
            var width = LayoutArgument.LayoutPattern.Width;
            var scoreThreshold = LayoutArgument.ScoreThreshold;

            var res = new Layout<T>(height, width, scoreThreshold);
            var cateCount = res.CategoryCount;

            var resultCh = result.Reshape(1, height * width, cateCount);
            var array = CvCvt.CvtToArray(resultCh);

            resultCh.Dispose();
            GC.Collect();
            GC.WaitForFullGCComplete();


            Enumerable.Range(0, height * width).ToList().ForEach(d =>
            {
                var row = d / width;
                var col = d % width;
                var activeScores = Enumerable.Range(0, cateCount)
                    .Select(c => CvMath.Sigmoid(array[d, c]))
                    .ToArray();
                var summ = activeScores.Sum();
                activeScores = activeScores.Select(s => s / summ).ToArray();
                res.UpdateScore(row, col, activeScores, scoreThreshold);
            });

            return res;
        }
    }
}