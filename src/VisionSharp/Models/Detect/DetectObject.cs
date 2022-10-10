using System.Text;
using OpenCvSharp;
using VisionSharp.Models.Ext;
using VisionSharp.Utils;

namespace VisionSharp.Models.Detect
{
    /// <summary>
    ///     包含更丰富的物体信息的旋转轮廓
    /// </summary>
    public class DetectObject : CvRotatedRect
    {
        public DetectObject()
        {
        }


        public DetectObject(RotatedRect rotatedRect, double angleFix)
            : base(rotatedRect)
        {
            AngleFix = angleFix;
        }

        /// <summary>
        ///     如果构造是不传如修正角度，则表示物体朝上，并且以Opencv旋转区域角度为物体朝向
        /// </summary>
        /// <param name="rotatedRect"></param>
        public DetectObject(RotatedRect rotatedRect)
            : base(rotatedRect)
        {
            AngleFix = rotatedRect.Angle > 0
                ? rotatedRect.Angle
                : rotatedRect.Angle + 180;
        }

        /// <summary>
        /// </summary>
        /// <param name="rect"></param>
        public DetectObject(Rect rect)
            : base(CvCvt.CvtToRotatedRect(rect))
        {
        }


        /// <summary>
        ///     抓取点，目标点
        /// </summary>
        public Point2d GrabPoint { set; get; }

        /// <summary>
        ///     为目标物的置信度
        /// </summary>
        public bool IsConfidence { set; get; }

        public float Confidence { set; get; }

        /// <summary>
        ///     分类
        /// </summary>
        public int Category { set; get; }

        public string Label { set; get; }

        /// <summary>
        ///     占了多少个格子
        /// </summary>
        public int DutyCount { set; get; }

        /// <summary>
        ///     不同物体定义的意义不同的修正角度，但需要反映的角度必须归为（0°~360°)，
        ///     朝向为第一第二象限，定位物体为正向
        ///     朝向为第三第四象限，定义物体为负向
        ///     第一象限  0~~90
        ///     第二象限  90~180
        ///     第三象限  180~~270
        ///     第四象限  270~360
        /// </summary>
        public double AngleFix { set; get; }

        public bool IsPositive => AngleFix is >= 0 and < 180;


        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine("DetectObject");
            strBuild.AppendLine(
                $"\tRotatedRect:\t({RotatedRect.Center.X:F2},{RotatedRect.Center.Y:F2})," +
                $"({RotatedRect.Size.Width},{RotatedRect.Size.Height}),{RotatedRect.Angle},");
            strBuild.AppendLine($"\tAngle:\t{AngleFix:F2}");
            strBuild.AppendLine($"\tDutyCount:\t{DutyCount}");
            strBuild.AppendLine($"\tIsPositive:\t{IsPositive}");
            strBuild.AppendLine($"\tConfidence:\t{IsConfidence}");
            return strBuild.ToString();
        }
    }
}