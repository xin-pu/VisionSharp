using System.Linq;
using CVLib.Utils;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Module
{
    /// <summary>
    ///     预测 tray 分布的处理器
    ///     TODO
    /// </summary>
    public class TrayDistributionPredictor : Processor<Mat, bool[,]>
    {
        public TrayDistributionPredictor(
            string modelFileName,
            Size image_size,
            Size pattern_size,
            double threshold = 0.8)
            : base("TrayDistributionPredictor")
        {
            ModelFileName = modelFileName;
            ImageSize = image_size;
            PatterSize = pattern_size;
            Threshold = threshold;
            Net = Precheck();
        }

        public string ModelFileName { get; }
        public Size ImageSize { get; }
        public Size PatterSize { get; }
        public double Threshold { get; }
        public Net Net { get; }


        internal Net Precheck()
        {
            //if (!File.Exists(ModelFileName))
            //    throw new FileNotFoundException($"Can find {ModelFileName}");

            //var net = CvDnn.ReadNetFromOnnx(ModelFileName);
            //if (net == null)
            //    throw new ArgumentException($"Can load {ModelFileName}");
            //return net;
            return null;
        }


        internal override bool[,] Process(Mat input)
        {
            //var inputBlob = CvDnn.BlobFromImage(input,
            //    1d / 255,
            //    ImageSize,
            //    swapRB: false, crop: false);
            //Net.SetInput(inputBlob);
            //var result = Net.Forward();
            //return Resolve(result);
            return new bool[8, 5]
            {
                {true, true, false, false, false},
                {true, true, false, false, false},
                {true, true, false, false, false},
                {true, true, false, false, false},
                {true, true, false, false, false},
                {true, true, false, false, false},
                {true, true, false, false, false},
                {true, true, false, false, false}
            };
        }

        internal bool[,] Resolve(Mat result)
        {
            var height = PatterSize.Height;
            var width = PatterSize.Width;
            var result_ch = result.Reshape(1, height, width);
            var array = CvCvt.CvtToArray(result_ch);
            var res = new bool[height, width];
            Enumerable.Range(0, height).ToList().ForEach(r =>
            {
                Enumerable.Range(0, width).ToList()
                    .ForEach(c => { res[r, c] = CvMath.Sigmoid(array[r, c]) > Threshold; });
            });
            return res;
        }

        internal override Mat Draw(Mat mat, bool[,] result)
        {
            var pointStart = new Point(186, 278);
            Enumerable.Range(0, PatterSize.Height).ToList().ForEach(r =>
            {
                Enumerable.Range(0, PatterSize.Width).ToList().ForEach(c =>
                {
                    if (result[r, c]) DrawPoint(mat, pointStart + new Point(c * 182, r * 417), PenColor);
                });
            });
            return mat;
        }
    }
}