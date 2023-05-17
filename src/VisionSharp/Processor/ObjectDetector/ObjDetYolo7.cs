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
        internal List<int[]> AnchorMaskLayer = new()
        {
            new[] {6, 7, 8},
            new[] {3, 4, 5},
            new[] {0, 1, 2}
        };

        public ObjDetYolo7(string onnxFile)
            : base(new Size(640, 640))
        {
            ModelWeights = onnxFile;
            Colors = CvCvt.GetColorDict<T>();
            Net = InitialNet();
        }


        internal Size SourceSize { set; get; }

        public Anchor[] Anchors { internal set; get; } =
        {
            new(12, 16),
            new(19, 36),
            new(40, 28),
            new(36, 75),
            new(76, 55),
            new(72, 146),
            new(142, 110),
            new(192, 243),
            new(459, 401)
        };

        internal int NetInputWidth => InputPattern.Width;
        internal int NetInputHeight => InputPattern.Height;


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

        internal override Mat[] FrontNet(Net net, Mat mat)
        {
            var matLetter = new LetterBox(InputPattern).Call(mat, mat).Result;
            SourceSize = matLetter.Size();

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
        ///     输入Mats
        /// </summary>
        /// <param name="mats"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override ObjRect<T>[] Decode(Mat[] mats, Size size)
        {
            var list = new List<ObjRect<T>>();

            var i = 0;
            foreach (var mat in mats)
            {
                /// 20x20,40x40,80*80
                var gridSize = new Size(mat.Size(3), mat.Size(2));
                ///32*32,16*16,8*8
                var strideSize = CvBasic.Div(InputPattern, gridSize);


                var pred = CvCvt.CvtToNDarray(mat).reshape(3, -1, gridSize.Height, gridSize.Width);
                pred = CvBasic.Sigmoid(pred.transpose(0, 2, 3, 1)); // sigmoid

                /// 拆分数据
                var predInfo = SplitDecodeInfo(pred);

                var gridX = CvBasic.CreateGridX(gridSize, 3);
                var gridY = CvBasic.CreateGridY(gridSize, 3);
                var (anchorW, anchorH) =
                    GetAnchorGridWidth(predInfo.W, predInfo.H, strideSize, gridSize, AnchorMaskLayer[i]);

                ///归一化
                var preBoxes = np.empty_like(pred[":,:,:,:4"]);
                preBoxes[":,:,:,0"] = (predInfo.X * 2f - 0.5 + gridX) / gridSize.Width;
                preBoxes[":,:,:,1"] = (predInfo.Y * 2f - 0.5 + gridY) / gridSize.Height;
                preBoxes[":,:,:,2"] = (predInfo.W * 2).power(np.array(2)) * anchorW / gridSize.Width;
                preBoxes[":,:,:,3"] = (predInfo.H * 2).power(np.array(2)) * anchorH / gridSize.Height;
                preBoxes = preBoxes.reshape(-1, 4);

                /// 置信度 和 
                var confR = predInfo.Confidence.reshape(-1, 1);
                var confClass = predInfo.Labels.max(new[] {-1}).reshape(-1, 1); // Todo
                var id = predInfo.Labels.argmax(-1).reshape(-1, 1);

                var res = np.concatenate(new[] {preBoxes, confR, confClass}, -1);

                var confidencePred = res[(res[":,4"] > Confidence).where()];

                var len = confidencePred.shape[0];
                foreach (var l in Enumerable.Range(0, len))
                {
                    var rowData = confidencePred[l].GetData<float>();
                    if (rowData[4] > Confidence)
                    {
                        var rect = Restore(rowData, InputPattern, SourceSize);
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

        private DecodeInfo SplitDecodeInfo(NDarray pred)
        {
            var (x, y, w, h) =
                (pred[":,:,:,0"], pred[":,:,:,1"], pred[":,:,:,2"], pred[":,:,:,3"]);
            var conf = pred[":,:,:,4"];
            var predCLs = pred[":,:,:,5:"];
            return new DecodeInfo
            {
                X = x,
                Y = y,
                W = w,
                H = h,
                Confidence = conf,
                Labels = predCLs
            };
        }


        private Tuple<NDarray, NDarray> GetAnchorGridWidth(NDarray w, NDarray h, Size strideSize,
            Size gridSize, int[] anchorMask)
        {
            var gridArea = gridSize.Width * gridSize.Height;
            var anchors = anchorMask.Select(d => Anchors[d])
                .Select(p => new Anchor(p.Width / strideSize.Width, p.Height / strideSize.Height))
                .ToList();

            var anchorWidth = np.array(anchors.Select(a => a.Width).ToArray());
            var anchorHeight = np.array(anchors.Select(a => a.Height).ToArray());

            var anchorW = anchorWidth
                .expand_dims(0).expand_dims(-1)
                .repeat(new[] {gridArea}, -1)
                .reshape(w.shape);
            var anchorH = anchorHeight
                .expand_dims(0).expand_dims(-1)
                .repeat(new[] {gridArea}, -1)
                .reshape(h.shape);
            return new Tuple<NDarray, NDarray>(anchorW, anchorH);
        }


        public struct DecodeInfo
        {
            public NDarray X { set; get; }
            public NDarray Y { set; get; }
            public NDarray W { set; get; }
            public NDarray H { set; get; }
            public NDarray Confidence { set; get; }
            public NDarray Labels { set; get; }
        }
    }
}