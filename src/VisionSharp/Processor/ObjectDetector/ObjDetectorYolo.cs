using OpenCvSharp;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor.ObjectDetector
{
    public abstract class ObjDetectorYolo<T> : ObjectDetector<T> where T : Enum
    {
        private string _configFile;
        private Size _inputPattern;
        private string _modelWeights;

        /// <summary>
        ///     基于Yolo的目标检测器
        /// </summary>
        protected ObjDetectorYolo(Size inputPattern)
            : base("ObjDetectorYolo")
        {
            InputPattern = inputPattern;
        }

        public string ModelWeights
        {
            internal set => SetProperty(ref _modelWeights, value);
            get => _modelWeights;
        }


        public string ConfigFile
        {
            internal set => SetProperty(ref _configFile, value);
            get => _configFile;
        }

        public Size InputPattern
        {
            internal set => SetProperty(ref _inputPattern, value);
            get => _inputPattern;
        }

        internal override ObjRect<T>[] Process(Mat input)
        {
            var mats = FrontNet(Net, input);
            var candidate = Decode(mats, input.Size());
            var final = NonMaximalSuppression(candidate);
            return final;
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
                if (mat.Height < 0)
                {
                    continue;
                }

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