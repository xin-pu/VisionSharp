using System.Text;
using GalaSoft.MvvmLight;

namespace CVLib.Processor
{
    /// <summary>
    ///     用于同一轮廓，角度和像素偏移量
    /// </summary>
    public class AdjustPara : ViewModelBase
    {
        private double _angle;
        private double _xbias;
        private double _ybias;

        public double Angle
        {
            set => Set(ref _angle, value);
            get => _angle;
        }

        public double XBias
        {
            set => Set(ref _xbias, value);
            get => _xbias;
        }

        public double YBias
        {
            set => Set(ref _ybias, value);
            get => _ybias;
        }

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