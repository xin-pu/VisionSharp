using System.Text;
using OpenCvSharp;
using VisionSharp.Models.Base;
using VisionSharp.Utils;

namespace VisionSharp.Models.Detect
{
    /// <summary>
    ///     包含更丰富的物体信息的旋转轮廓
    /// </summary>
    public class ObjRotatedrect<T> : CvRotatedRect where T : Enum
    {
        private double _angleFix;
        private T _category;

        private float _categoryConfidence;
        private float _objectConfidence;

        public ObjRotatedrect()
        {
        }


        public ObjRotatedrect(RotatedRect rotatedRect, double angleFix)
            : base(rotatedRect)
        {
            AngleFix = angleFix;
        }

        /// <summary>
        ///     如果构造是不传如修正角度，则表示物体朝上，并且以Opencv旋转区域角度为物体朝向
        /// </summary>
        /// <param name="rotatedRect"></param>
        public ObjRotatedrect(RotatedRect rotatedRect)
            : base(rotatedRect)
        {
            AngleFix = rotatedRect.Angle > 0
                ? rotatedRect.Angle
                : rotatedRect.Angle + 180;
        }

        /// <summary>
        /// </summary>
        /// <param name="rect"></param>
        public ObjRotatedrect(Rect rect)
            : base(CvCvt.CvtToRotatedRect(rect))
        {
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

        /// <summary>
        ///     不同物体定义的意义不同的修正角度，但需要反映的角度必须归为（0°~360°)，
        ///     朝向为第一第二象限，定位物体为正向
        ///     朝向为第三第四象限，定义物体为负向
        ///     第一象限  0~~90
        ///     第二象限  90~180
        ///     第三象限  180~~270
        ///     第四象限  270~360
        /// </summary>
        public double AngleFix
        {
            internal set => SetProperty(ref _angleFix, value);
            get => _angleFix;
        }

        public bool IsPositive => AngleFix is >= 0 and < 180;


        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine($"{Category}\t[{ObjectConfidence:P2}]");
            strBuild.AppendLine(base.ToString());
            return strBuild.ToString();
        }
    }
}