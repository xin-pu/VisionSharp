using System.Collections.Generic;
using System.Text;
using OpenCvSharp;

namespace CVLib.Models
{
    /// <summary>
    ///     包含格子信息的区域信息
    /// </summary>
    public class DetectGridRect
    {
        public DetectGridRect(int row, int column, Rect rect)
        {
            Row = row;
            Column = column;
            Rect = rect;
        }

        public Rect Rect { set; get; }

        public int Row { set; get; }

        public int Column { set; get; }

        public bool IsEmpty { set; get; }

        public double ObjectScale { set; get; }

        public int DutyCount { set; get; }

        public List<RotatedRect> DutyRect { set; get; }

        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine($"DetectGridRect[{Row},{Column}]");
            strBuild.AppendLine($"\tScale:\t{ObjectScale:P2}");
            strBuild.AppendLine($"\tDuty:\t{DutyCount}");
            strBuild.AppendLine($"\tPoint:\t{Rect.X},{Rect.Y}");
            strBuild.AppendLine($"\tSize:\t{Rect.Width},{Rect.Height}");
            return strBuild.ToString();
        }
    }
}