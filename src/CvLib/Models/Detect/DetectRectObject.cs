using System.Text;
using OpenCvSharp;

namespace CVLib.Models
{
    public class DetectRectObject : CvRect
    {
        /// <summary>
        ///     常用带分类的识别框
        /// </summary>
        public DetectRectObject()
        {
        }


        /// <summary>
        ///     常用带分类的识别框
        /// </summary>
        /// <param name="rect"></param>
        public DetectRectObject(Rect rect)
            : base(rect)
        {
        }


        public float ObjectConfidence { set; get; }


        public float CategoryConfidence { set; get; }

        /// <summary>
        /// </summary>
        public int Category { set; get; }


        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine($"DetectRectObject:{Category}({ObjectConfidence:P2})");
            strBuild.AppendLine(base.ToString());
            return strBuild.ToString();
        }
    }
}