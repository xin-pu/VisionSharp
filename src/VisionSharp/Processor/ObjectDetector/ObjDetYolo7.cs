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
            var matLetter = new LetterBox().Call(mat);
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
        internal override unsafe ObjRect<T>[] Decode(Mat[] mats, Size size)
        {
            var list = new List<ObjRect<T>>();
            var netStride = new float[] {8, 16, 32, 64};
            var NetAnchors = new float[3, 6]
            {
                {12, 16, 19, 36, 40, 28},
                {36, 75, 76, 55, 72, 146},
                {142, 110, 192, 243, 459, 401}
            };
            var netHeight = InputPattern.Height;
            var netWidth = InputPattern.Width;
            var ratioH = (float) SrcSize.Height / netHeight;
            var ratioW = (float) SrcSize.Width / netWidth;
            const int net_width = 1 + 5;


            for (var stride = 0; stride < 3; stride++)
            {
                var pdata = (float*) mats[stride].Data;
                var grid_x = netWidth / netStride[stride];
                var grid_y = netHeight / netStride[stride];


                for (var anchor = 0; anchor < 3; anchor++)
                {
                    var anchor_w = NetAnchors[stride, anchor * 2];
                    var anchor_h = NetAnchors[stride, anchor * 2 + 1];

                    for (var i = 0; i < grid_y; i++)
                    {
                        for (var j = 0; j < grid_x; j++)
                        {
                            var box_score = (float) CvMath.Sigmoid(pdata[4]); //获取每一行的box框中含有某个物体的概率
                            if (box_score >= Confidence)
                            {
                                var scores = new List<float> {pdata[5]};

                                var classProb = scores.Max() * box_score;
                                var classIndex = scores.IndexOf(classProb);
                                var category = (T) Enum.ToObject(typeof(T), classIndex);

                                if (classProb >= Confidence)
                                {
                                    var x = (int) ((CvMath.Sigmoid(pdata[0]) * 2 - 0.5f + j) * netStride[stride]); //x
                                    var y = (int) ((CvMath.Sigmoid(pdata[1]) * 2 - 0.5f + i) * netStride[stride]); //y
                                    var w = (int) (Math.Pow(CvMath.Sigmoid(pdata[2]) * 2, 2) * anchor_w); //w
                                    var h = (int) (Math.Pow(CvMath.Sigmoid(pdata[3]) * 2, 2) * anchor_h); //h

                                    var rect = new Rect(x, y, w, h);

                                    var detectRectObject = new ObjRect<T>(rect)
                                    {
                                        Category = category,
                                        ObjectConfidence = classProb
                                    };
                                    list.Add(detectRectObject);
                                }
                            }

                            pdata += net_width; //下一行
                        }
                    }
                }
            }

            return list.ToArray();
        }
    }
}