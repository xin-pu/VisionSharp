using System.Text;
using OpenCvSharp;
using VisionSharp.Models.Base;

namespace VisionSharp.Models.Detect
{
    public class ObjRect<T> : CvRect where T : Enum
    {
        private T _category;

        private float _categoryConfidence;
        private float _objectConfidence;

        /// <summary>
        ///     常用带分类的识别框
        /// </summary>
        public ObjRect(Rect rect) : base(rect)
        {
        }

        /// <summary>
        ///     常用带分类的识别框
        /// </summary>
        /// <param name="rect"></param>
        public ObjRect(T category, Rect rect) : base(rect)
        {
            Category = category;
        }

        /// <summary>
        ///     分类
        /// </summary>
        public T Category
        {
            internal set => SetProperty(ref _category, value);
            get => _category;
        }

        /// <summary>
        ///     包含物体置信度
        /// </summary>
        public float ObjectConfidence
        {
            internal set => SetProperty(ref _objectConfidence, value);
            get => _objectConfidence;
        }

        /// <summary>
        ///     最终分类置信度
        /// </summary>
        public float CategoryConfidence
        {
            internal set => SetProperty(ref _categoryConfidence, value);
            get => _categoryConfidence;
        }

        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine($"{Category}\t[{ObjectConfidence:P2}]");
            strBuild.AppendLine(base.ToString());
            return strBuild.ToString();
        }
    }
}