using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor.ObjectDetector
{
    public class ObjDetYolo7<T> : ObjDetYolo<T> where T : Enum
    {
        public ObjDetYolo7(string onnxFile) : base(new Size(640, 640))
        {
            ModelWeights = onnxFile;

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
        ///     Yolo3以上的解码过程是一样的
        /// </summary>
        /// <param name="mats"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override ObjRect<T>[] Decode(Mat[] mats, Size size)
        {
            var list = new List<ObjRect<T>>();

            foreach (var mat in mats)
            {
                mat[new Rect(4, 0, 1, mat.Height)].GetArray(out float[] confidence);

                var conList = confidence
                    .Select((c, i) => (c, i))
                    .Where(p => p.c > Confidence)
                    .Select(p => p.i)
                    .ToList();

                conList.AsParallel().ToList().ForEach(i =>
                {
                    var _ = mat[new Rect(0, i, mat.Width, 1)].GetArray(out float[] rowdata);
                    var rowInfo = rowdata.ToList();

                    var classify = rowInfo.Skip(5).ToList();
                    var classProb = classify.Max();
                    var classIndex = classify.IndexOf(classProb);
                    var category = (T) Enum.ToObject(typeof(T), classIndex);

                    if (classProb < Confidence)
                    {
                        return;
                    }

                    var centerX = rowInfo[0] * size.Width;
                    var centerY = rowInfo[1] * size.Height;

                    var w = (int) (rowInfo[2] * size.Width);
                    var h = (int) (rowInfo[3] * size.Height);
                    var x = (int) (centerX - w / 2F);
                    var y = (int) (centerY - h / 2F);

                    var rect = new Rect(x, y, w, h);

                    var detectRectObject = new ObjRect<T>(rect)
                    {
                        Category = category,
                        ObjectConfidence = classProb
                    };
                    list.Add(detectRectObject);
                });
            }


            return list.ToArray();
        }
    }
}