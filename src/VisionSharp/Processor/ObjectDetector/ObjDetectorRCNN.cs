using OpenCvSharp;
using OpenCvSharp.Dnn;
using VisionSharp.Models.Detect;

namespace VisionSharp.Processor.ObjectDetector
{
    public class ObjDetectorRcnn<T> : ObjectDetector<T> where T : Enum
    {
        /// <summary>
        ///     基于RCNN的目标检测器
        /// </summary>
        /// <param name="name"></param>
        public ObjDetectorRcnn(string name) : base(name)
        {
        }

        internal override ObjRect<T>[] Process(Mat input)
        {
            throw new NotImplementedException();
        }

        internal override Net InitialNet()
        {
            throw new NotImplementedException();
        }

        internal override Mat[] FrontNet(Net net, Mat mat)
        {
            throw new NotImplementedException();
        }

        internal override ObjRect<T>[] Decode(Mat[] mats, Size size)
        {
            throw new NotImplementedException();
        }
    }
}