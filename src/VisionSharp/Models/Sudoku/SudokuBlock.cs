using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Sudoku
{
    /// <summary>
    ///     数独单元格
    /// </summary>
    public class SudokuBlock : ObservableObject
    {
        public SudokuBlock(bool isCondition, byte number, byte column, byte row)
        {
            IsCondition = isCondition;

            Number = number;
            Column = column;
            Row = row;
        }

        /// <summary>
        ///     填入的数字
        /// </summary>
        public byte Number { set; get; }

        /// <summary>
        ///     X坐标
        /// </summary>
        public byte Column { get; }

        /// <summary>
        ///     Y坐标
        /// </summary>
        public byte Row { get; }


        /// <summary>
        ///     是否为条件（题目）给出数字的单元格
        /// </summary>
        public bool IsCondition { get; }


        public void SetNumber(byte number)
        {
            Number = number;
        }

        public override string ToString()
        {
            return Number == 0 ? "." : Number.ToString();
        }
    }
}