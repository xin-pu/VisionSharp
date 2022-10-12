using OpenCvSharp;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor.ObjectDetector
{
    public abstract class ObjDetectorYolo<T> : ObjectDetector<T> where T : Enum
    {
        /// <summary>
        ///     基于Yolo的目标检测器
        /// </summary>
        protected ObjDetectorYolo() : base("ObjDetectorYolo")
        {
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

            mats.AsParallel().ToList().ForEach(m =>
            {
                m[new Rect(4, 0, 1, m.Height)].GetArray(out float[] confidence);

                var conList = confidence
                    .Select((c, i) => (c, i))
                    .Where(p => p.c > Confidence)
                    .Select(p => p.i)
                    .ToList();

                conList.AsParallel().ToList().ForEach(i =>
                {
                    var _ = m[new Rect(0, i, m.Width, 1)].GetArray(out float[] rowdata);
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
            });

            return list.ToArray();
        }
    }
}