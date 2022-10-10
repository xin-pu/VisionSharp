using System.Text;
using GalaSoft.MvvmLight;

namespace VisionSharp.Models.Ext
{
    /// <summary>
    ///     OpenCV中矩阵 Cell对象
    /// </summary>
    public class CvMatCell : ViewModelBase
    {
        public CvMatCell()
        {
        }

        public CvMatCell(int row, int column, double value)
        {
            Row = row;
            Column = column;
            Value = value;
        }

        public int Row { set; get; }
        public int Column { set; get; }
        public double Value { set; get; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"CvMatCell\t[{Row},{Column}]: {Value:F4}");
            return str.ToString();
        }
    }
}