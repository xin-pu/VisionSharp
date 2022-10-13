using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Sudoku
{
    public class SudokuSubject : ObservableObject
    {
        private List<SudokuBlock> _cellList;

        /// <summary>
        ///     数独题
        /// </summary>
        /// <param name="subject">0,9, 0代表未知</param>
        public SudokuSubject(byte[,] subject)
        {
            Subject = subject;
            CellList = new List<SudokuBlock>();

            RowCount = subject.GetLength(0);
            ColumnCount = subject.GetLength(1);
            if (RowCount != 9 && ColumnCount != 9)
            {
                throw new ArgumentException();
            }

            Initial();
        }

        public int RowCount { internal set; get; }
        public int ColumnCount { internal set; get; }
        public byte[,] Subject { internal set; get; }

        public List<SudokuBlock> CellList
        {
            protected set => SetProperty(ref _cellList, value);
            get => _cellList;
        }

        public SudokuBlock[] this[int row] => GetRow(row);
        public SudokuBlock this[int row, int column] => GetSudokuBlock(row, column);

        private void Initial()
        {
            foreach (var r in Enumerable.Range(0, RowCount))
            {
                foreach (var c in Enumerable.Range(0, ColumnCount))
                {
                    var value = Subject[r, c];
                    CellList.Add(new SudokuBlock(value != 0, value, (byte) c, (byte) r));
                }
            }
        }

        /// <summary>
        ///     获取一行
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public SudokuBlock[] GetRow(int row)
        {
            return CellList
                .Where(a => a.Row == row)
                .OrderBy(a => a.Column)
                .ToArray();
        }

        /// <summary>
        ///     获取一列
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public SudokuBlock[] GetColumn(int column)
        {
            return CellList
                .Where(a => a.Column == column)
                .OrderBy(a => a.Row)
                .ToArray();
        }

        /// <summary>
        ///     获取单元格
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public SudokuBlock GetSudokuBlock(int row, int column)
        {
            return CellList.First(a => a.Row == row && a.Column == column);
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var r in Enumerable.Range(0, RowCount))
            {
                var row = this[(byte) r];
                var line = string.Join("\t", row.Select(r => r.ToString()));
                str.AppendLine(line);
            }

            return str.ToString();
        }
    }
}