using System.Text;

namespace CVLib.Processor
{
    /// <summary>
    ///     用于同一轮廓，角度和像素偏移量
    /// </summary>
    public struct AdjustPara
    {
        public double Angle { set; get; }
        public double XBias { set; get; }
        public double YBias { set; get; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("AdjustPara");
            str.AppendLine($"\tAngle:\t{Angle:F4}");
            str.AppendLine($"\tShift:\t({XBias:F4},{YBias:F4})");
            return str.ToString();
        }
    }
}