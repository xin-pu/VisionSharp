using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor.ObjectDetector
{
    public class ObjDetYolo7<T> : ObjDetYolo<T> where T : Enum
    {
        public ObjDetYolo7(string onnxFile)
            : base(new Size(640, 640))
        {
            ModelWeights = onnxFile;
            Net = InitialNet();
            Anchors = new Point2f[]
            {
                new(142, 110),
                new(192, 243),
                new(459, 401),
                new(36, 75),
                new(76, 55),
                new(72, 146),
                new(12, 16),
                new(19, 36),
                new(40, 28)
            };
        }

        public Point2f[] Anchors { set; get; }

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
            var inputBlob = CvDnn.BlobFromImage(mat,
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
            //mats = mats.Where(a => a.Size(0) == 1).ToArray();
            //var i = 0;
            //foreach (var mat in mats)
            //{
            //    var input_width = mat.Size(2);
            //    var input_height = mat.Size(3);

            //    var stride_h = InputPattern.Height / input_height;
            //    var stride_w = InputPattern.Width / input_width;


            //    var netsize = Enum.GetNames(typeof(T)).Length + 5;

            //    var r1 = mat.Reshape(0, 3, netsize, input_width * input_height);
            //    var r2=r1.Reshape(0, 3, 6, input_width, input_height);
            //    var j = 0;

            //    foreach (var VARIABLE in Enumerable.Range(0, 3))
            //    {
            //        var anchorWidth = Anchors[i * 3 + j].X / stride_w;
            //        var anchorHeight = Anchors[i * 3 + j].Y / stride_h;

            //        var gridX = (int) (input_width / anchorWidth);
            //        var gridY = (int) (input_height / anchorHeight);

            //        j++;
            //    }

            //    i++;
            //}


            return list.ToArray();
        }
    }
}