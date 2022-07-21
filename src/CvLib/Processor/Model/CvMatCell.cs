using System.Text;
using GalaSoft.MvvmLight;

namespace CVLib.Processor
{
    /// <summary>
    ///     This is mat cell double for UI corresponding to cell of Mat @ openCV
    /// </summary>
    public class CvMatCell : ViewModelBase
    {
        private int _column;
        private int _row;
        private double _value;

        public CvMatCell()
        {
        }

        public CvMatCell(int row, int column, double value)
        {
            Row = row;
            Column = column;
            Value = value;
        }

        public int Row
        {
            set => Set(ref _row, value);
            get => _row;
        }

        public int Column
        {
            set => Set(ref _column, value);
            get => _column;
        }

        public double Value
        {
            set => Set(ref _value, value);
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