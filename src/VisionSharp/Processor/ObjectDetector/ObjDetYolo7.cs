using Numpy;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;
using VisionSharp.Processor.Transform;
using VisionSharp.Utils;

namespace VisionSharp.Processor.ObjectDetector
{
    public class ObjDetYolo7<T> : ObjDetYolo<T> where T : Enum
    {
        public ObjDetYolo7(string onnxFile)
            : base(new Size(640, 640))
        {
            ModelWeights = onnxFile;
            Colors = CvCvt.GetColorDict<T>();
            Net = InitialNet();
        }


        internal sealed override Net InitialNet()
        {
            if (ModelWeights == null)
            {
                throw new ArgumentNullException();
            }

            if (File.Exists(ModelWeights) == false)
            {
                throw new FileNotFoundException();
            }


            var darknet = CvDnn.ReadNetFromOnnx(ModelWeights);
            if (darknet == null)
            {
                throw new NullReferenceException("Can't Load Net");
            }

            darknet.SetPreferableBackend(Backend.OPENCV);
            darknet.SetPreferableTarget(Target.CPU);
            return darknet;
        }

        internal Size SrcSize { set; get; }

        internal override Mat[] FrontNet(Net net, Mat mat)
        {
            SrcSize = mat.Size();
            var matLetter = new LetterBox(InputPattern).Call(mat, mat).Result;
            SrcSize = matLetter.Size();

            var inputBlob = CvDnn.BlobFromImage(matLetter,
                1F / 255,
                InputPattern,
                new Scalar(0, 0, 0),
                true,
                false);

            Net.SetInput(inputBlob);
            var dd = net.GetUnconnectedOutLayersNames();
            var mats = new Mat[] {new(), new(), new()};
            Net.Forward(mats, dd);
            return mats;
        }

        /// <summary>
        ///     Yolo7以上的解码过程是一样的
        ///     Batch Size * (3 * ([x,y,w,h] + conf + Pred classes),20,20 )
        ///     Batch Size * (3 * ([x,y,w,h] + conf + Pred classes),40,40 )
        ///     Batch Size * (3 * ([x,y,w,h] + conf + Pred classes),80,80 )
        /// </summary>
        /// <param name="mats"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override ObjRect<T>[] Decode(Mat[] mats, Size size)
        {
            var list = new List<ObjRect<T>>();
            var netAnchors = new List<float[]>
            {
                new float[] {12, 16},
                new float[] {19, 36},
                new float[] {40, 28},

                new float[] {36, 75},
                new float[] {76, 55},
                new float[] {72, 146},

                new float[] {142, 110},
                new float[] {192, 243},
                new float[] {459, 401}
            };
            var anchorMaskLayer = new List<int[]>
            {
                new[] {6, 7, 8},
                new[] {3, 4, 5},
                new[] {0, 1, 2}
            };

            var netHeight = InputPattern.Height;
            var netWidth = InputPattern.Width;

            var categoryCount = CategoryCount + 5;
            var i = 0;
            foreach (var mat in mats)
            {
                var inputHeight = mat.Size(2);
                var inputWidth = mat.Size(3);
                var strideHeight = netHeight / inputHeight;
                var strideWidth = netWidth / inputWidth;

                var anchorMask = anchorMaskLayer[i];

                var ancors = anchorMask.Select(d => netAnchors[d])
                    .Select(p => new {A = p[0] / strideWidth, B = p[1] / strideHeight})
                    .ToList();

                mat.Reshape(0, 3 * categoryCount * inputHeight * inputWidth).GetArray(out float[] data);

                var dim = new NDarray<float>(data);
                var reshape = dim.reshape(3, categoryCount, inputHeight, inputWidth);
                var prediction = reshape.transpose(0, 2, 3, 1);
                prediction = 1.0 / (1 + (-prediction).exp());

                var x = prediction[":,:,:,0"];
                var y = prediction[":,:,:,1"];
                var w = prediction[":,:,:,2"];
                var h = prediction[":,:,:,3"];
                var conf = prediction[":,:,:,4"];
                var predCLs = prediction[":,:,:,5:"];


                var gridX = CvBasic.CreateGridX(inputWidth, inputHeight, 3);
                var gridY = CvBasic.CreateGridY(inputWidth, inputHeight, 3);

                var anchorW = ancors.Select(a => a.A).ToArray();
                var anchorH = ancors.Select(a => a.B).ToArray();
                var anchor_W = np.array(anchorW).expand_dims(0).expand_dims(-1)
                    .repeat(new[] {inputHeight * inputWidth}, -1)
                    .reshape(w.shape);
                var anchor_H = np.array(anchorH).expand_dims(0).expand_dims(-1)
                    .repeat(new[] {inputHeight * inputWidth}, -1)
                    .reshape(h.shape);

                var preBoxes = np.empty_like(prediction[":,:,:,:4"]);
                preBoxes[":,:,:,0"] = x * 2f - 0.5 + gridX;
                preBoxes[":,:,:,1"] = y * 2f - 0.5 + gridY;
                preBoxes[":,:,:,2"] = (w * 2).power(np.array(2)) * anchor_W;
                preBoxes[":,:,:,3"] = (h * 2).power(np.array(2)) * anchor_H;
                var _scale = np.array(new float[] {inputWidth, inputHeight, inputWidth, inputHeight});


                var final = preBoxes.reshape(-1, 4) / _scale;
                var confR = conf.reshape(-1, 1);
                var confClass = predCLs.max(new[] {-1}).reshape(-1, 1);

                var res = np.concatenate(new[] {final, confR, confClass}, -1);

                var ress = res.GetData<float>();
                var len = ress.Length / 6;
                foreach (var l in Enumerable.Range(0, len))
                {
                    var row = res[$"{l},:"];
                    var rowData = row.GetData<float>();
                    if (rowData[4] > Confidence)
                    {
                        var rect = Restore(rowData, InputPattern, SrcSize);
                        var detectRectObject = new ObjRect<T>(rect)
                        {
                            Category = (T) Enum.ToObject(typeof(T), 0),
                            ObjectConfidence = rowData[4]
                        };
                        list.Add(detectRectObject);
                    }
                }


                i++;
            }

            return list.ToArray();
        }


        private Rect Restore(float[] xywh, Size inputShape, Size orginalShape)
        {
            var d = new[]
            {
                1f * inputShape.Width / orginalShape.Width,
                1f * inputShape.Height / orginalShape.Height
            }.Min();
            var newsize = new Size(Math.Round(orginalShape.Width * d, MidpointRounding.AwayFromZero),
                Math.Round(orginalShape.Height * d, MidpointRounding.AwayFromZero));

            var offsetX = (inputShape.Width - newsize.Width) / 2 / inputShape.Width;
            var offsetY = (inputShape.Height - newsize.Height) / 2 / inputShape.Height;

            var scaleX = 1f * inputShape.Width / newsize.Width;
            var scaleY = 1f * inputShape.Height / newsize.Height;

            var x = (xywh[0] - offsetX) * scaleX;
            var y = (xywh[1] - offsetY) * scaleY;
            var w = xywh[2] * scaleX;
            var h = xywh[3] * scaleY;

            var left = x - w / 2;
            var top = y - h / 2;

            return new Rect((int) (left * orginalShape.Width),
                (int) (top * orginalShape.Width),
                (int) (w * orginalShape.Width),
                (int) (h * orginalShape.Width));
        }
    }
}