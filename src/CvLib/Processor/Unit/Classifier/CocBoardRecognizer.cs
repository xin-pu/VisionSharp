using System;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace CVLib.Processor.Unit
{
    public class CocBoardRecognizer : Processor<Mat, CocBoardStatus>
    {
        public CocBoardRecognizer(string modelFileName)
            : base("CocBoardRecognizer")
        {
            ModelFileName = modelFileName;
            Model = LogisticRegression.Load(modelFileName);
        }
        //private static readonly Lazy<CocBoardRecognizer> lazy = new(() => new CocBoardRecognizer());

        private string ModelFileName { get; }

        //public static CocBoardRecognizer Instance => lazy.Value;


        public StatModel Model { get; }
        public Size InputShape { get; } = new(300, 60);

        internal override CocBoardStatus Process(Mat input)
        {
            var resize = input.Resize(InputShape);
            var mean = Cv2.Mean(resize);
            if (mean[0] < 40)
                return CocBoardStatus.Object;
            resize.GetArray(out byte[] datas);
            var testFeature = new Mat(1, InputShape.Width * InputShape.Height, MatType.CV_32F,
                datas.Select(a => (float) a).ToArray());

            var res = Model.Predict(testFeature);
            var finalStatus = (int) res == 1
                ? CocBoardStatus.Object
                : CocBoardStatus.Empty;
            return finalStatus;
        }

        public void Train(string datafolder)
        {
            var files = Directory.GetFiles(datafolder, "*.png").ToList();
            var features = files
                .SelectMany(file =>
                {
                    var mat = Cv2.ImRead(file, ImreadModes.Grayscale);
                    mat = mat.Resize(InputShape);
                    mat.GetArray(out byte[] datas);
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                    return datas.Select(a => (float) a);
                })
                .ToList();

            var labels = files
                .Select(file =>
                {
                    var fileinfo = new FileInfo(file);
                    return float.Parse(fileinfo.Name.Split('_')[0]);
                }).ToList();


            var trainFeatures = new Mat(files.Count, InputShape.Height * InputShape.Width, MatType.CV_32F,
                features.ToArray());
            var trainLabels = new Mat(files.Count, 1, MatType.CV_32F, labels.ToArray());

            using var model = LogisticRegression.Create();
            model.LearningRate = 1E-4;
            model.Iterations = 1000;
            model.Regularization = LogisticRegression.RegKinds.RegDisable;
            model.TrainMethod = LogisticRegression.Methods.Batch;


            model.Train(trainFeatures, SampleTypes.RowSample, trainLabels);

            model.Save(FileName);
        }
    }

    public enum CocBoardStatus
    {
        Empty = 0,
        Object = 1
    }
}