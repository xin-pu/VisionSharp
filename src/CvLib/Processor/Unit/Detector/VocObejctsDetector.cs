using System;
using System.Collections.Generic;
using System.Linq;
using CVLib.Models;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace CVLib.Processor.Unit
{
    /// <summary>
    /// </summary>
    public class VocObejctsDetector : Processor<Mat, List<DetectObject>>
    {
        public int[] Pattern =
        {
            52, 26, 13
        };


        public Size[] Size =
        {
            new(10, 13),
            new(16, 30),
            new(33, 23),

            new(30, 61),
            new(62, 45),
            new(59, 119),

            new(116, 90),
            new(156, 198),
            new(373, 326)
        };

        public VocObejctsDetector(string name = "VocObejctsDetector")
            : base(name)
        {
        }

        public string ModelWeights { set; get; } = "voc.weights";
        public string ConfigFile { set; get; } = "voc.cfg";
        public float CONFIDENCE { set; get; } = 0.8F;
        public float THRESHOLD { set; get; } = 0.8F;
        private Net net { set; get; }

        public string[] Names { get; } =
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


        internal override List<DetectObject> Process(Mat input)
        {
            net ??= CvDnn.ReadNetFromDarknet(ConfigFile, ModelWeights);

            var detectObjectS = TestDarkNetModel(input);
            CvDnn.NMSBoxes(
                detectObjectS.Select(a => a.RotatedRect.BoundingRect()),
                detectObjectS.Select(a => a.Confidence),
                CONFIDENCE,
                THRESHOLD,
                out var boxIndex);
            var filter = detectObjectS.Select((a, i) => (a, i)).Where(p => boxIndex.Contains(p.i)).Select(p => p.a)
                .ToList();
            return filter;
        }


        public List<DetectObject> TestDarkNetModel(Mat mat)
        {
            if (net == null) throw new ArgumentException();
            net.SetPreferableBackend(Backend.OPENCV);
            net.SetPreferableTarget(Target.OPENCL_FP16);


            var size = mat.Size();
            var inputBlob = CvDnn.BlobFromImage(mat, 1F / 255, new Size(416, 416),
                new Scalar(0, 0, 0),
                true,
                false);

            net.SetInput(inputBlob, "data");
            var blobName = new[] {"yolo_106", "yolo_94", "yolo_82"};
            var mats = blobName.Select(net.Forward).ToList();

            var list = new List<DetectObject>();


            mats.ForEach(m =>
            {
                foreach (var i in Enumerable.Range(0, m.Height))
                {
                    var _ = m[new Rect(5, i, 20, 1)].GetArray(out float[] a);
                    if (a.Max() > CONFIDENCE)
                    {
                        var index = a.ToList().IndexOf(a.Max());
                        var label = Names[index];
                        m[new Rect(0, i, 4, 1)].GetArray(out float[] box);

                        var center_x = box[0] * size.Width;
                        var center_y = box[1] * size.Height;
                        var w = (int) (box[2] * size.Width);
                        var h = (int) (box[3] * size.Height);

                        var x = (int) (center_x - w / 2F);
                        var y = (int) (center_y - h / 2F);

                        var rect = new DetectObject(new Rect(x, y, w, h))
                        {
                            Category = index,
                            Label = label,
                            Confidence = a.Max()
                        };
                        list.Add(rect);
                    }
                }
            });


            return list;
        }

        /// <summary>
        ///     模块轮廓与其他轮廓分别绘画
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal override Mat Draw(Mat mat, List<DetectObject> detectObjects)
        {
            detectObjects
                .ToList()
                .ForEach(a =>
                {
                    DrawRect(mat, a.RotatedRect.BoundingRect(), PenColor, thickness: 1);
                    mat.PutText($"{a.Label} {a.Confidence:P2}", a.RotatedRect.BoundingRect().TopLeft,
                        HersheyFonts.HersheyPlain,
                        2, PenColor, 2);
                });
            return mat;
        }
    }
}