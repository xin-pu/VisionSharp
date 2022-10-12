using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Base
{
    public class CvMatCell : ObservableObject
    {
        private int _column;
        private int _row;

        private double _value;

        /// <summary>
        ///     可观测的矩阵单元
        /// </summary>
        public CvMatCell()
        {
        }

        /// <summary>
        ///     可观测的矩阵单元
        /// </summary>
        public CvMatCell(int row, int column, double value)
        {
            Row = row;
            Column = column;
            Value = value;
        }

        public int Row
        {
            set => SetProperty(ref _row, value);
            get => _row;
        }

        public int Column
        {
            set => SetProperty(ref _column, value);
            get => _column;
        }

        public double Value
        {
            set => SetProperty(ref _value, value);
            get => _value;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"CvMatCell\t[{Row},{Column}]: {Value:F4}");
            return str.ToString();
        }
    }
}