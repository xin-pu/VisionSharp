using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Unit
{
    /// <summary>
    ///     Yolo V3 object detect by DarkNet
    /// </summary>
    public class YoloV3Detector : ObjDetector

    {
        public YoloV3Detector(string name = "YoloV3Detector")
            : base(name)
        {
            DrawInfo = false;
        }

        public string ModelWeights { set; get; } = "voc.weights";
        public string ConfigFile { set; get; } = "voc.cfg";

        public override string[] Categroy => new[]
        {
            "aeroplane",
            "bicycle",
            "bird",
            "boat",
            "bottle",
            "bus",
            "car",
            "cat",
            "chair",
            "cow",
            "diningtable",
            "dog",
            "horse",
            "motorbike",
            "person",
            "pottedplant",
            "sheep",
            "sofa",
            "train",
            "tvmonitor"
        };

        internal override Net InitialNet()
        {
            var darknet = CvDnn.ReadNetFromDarknet(ConfigFile, ModelWeights);
            if (darknet == null)
                throw new ArgumentNullException();

            darknet.SetPreferableBackend(Backend.CUDA);
            darknet.SetPreferableTarget(Target.CUDA);
            return darknet;
        }

        internal override Mat[] FrontNet(Net model, Mat mat)
        {
            var inputBlob = CvDnn.BlobFromImage(mat,
                1F / 255,
                new Size(416, 416),
                new Scalar(0, 0, 0),
                true,
                false);

            model.SetInput(inputBlob, "data");
            var mats = new Mat[] {new(), new(), new()};
            model.Forward(mats, new[] {"yolo_106", "yolo_94", "yolo_82"});
            return mats;
        }

        internal override List<DetectRectObject> Decode(Mat[] mats, Size size)
        {
            var list = new List<DetectRectObject>();

            mats.AsParallel().ToList().ForEach(m =>
            {
                m[new Rect(4, 0, 1, m.Height)].GetArray(out float[] confidence);

                var conList = confidence
                    .Select((c, i) => (c, i))
                    .Where(p => p.c > CONFIDENCE)
                    .Select(p => p.i)
                    .ToList();

                conList.AsParallel().ToList().ForEach(i =>
                {
                    var _ = m[new Rect(0, i, m.Width, 1)].GetArray(out float[] rowdata);
                    var rowInfo = rowdata.ToList();

                    var classify = rowInfo.Skip(5).ToList();
                    var classProb = classify.Max();
                    var classIndex = classify.IndexOf(classProb);
                    if (classProb < CONFIDENCE) return;

                    var center_x = rowInfo[0] * size.Width;
                    var center_y = rowInfo[1] * size.Height;

                    var w = (int) (rowInfo[2] * size.Width);
                    var h = (int) (rowInfo[3] * size.Height);
                    var x = (int) (center_x - w / 2F);
                    var y = (int) (center_y - h / 2F);

                    var rect = new Rect(x, y, w, h);

                    var detectRectObject = new DetectRectObject(rect)
                    {
                        Category = classIndex,
                        ObjectConfidence = classProb
                    };
                    list.Add(detectRectObject);
                });
            });

            return list;
        }


        #region MyRegion

        public RelayCommand SelectCFGCommand => new(SelectCFGCommand_Execute);
        public RelayCommand SelectWeightCommand => new(SelectWeightCommand_Execute);
        public RelayCommand SelectNamesCommand => new(SelectNamesCommand_Execute);
        public RelayCommand PredictCommand => new(PredictCommand_Execute);

        private void PredictCommand_Execute()
        {
            throw new NotImplementedException();
        }

        private void SelectWeightCommand_Execute()
        {
            throw new NotImplementedException();
        }

        private void SelectCFGCommand_Execute()
        {
            throw new NotImplementedException();
        }

        private void SelectNamesCommand_Execute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}