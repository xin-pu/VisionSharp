using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Sudoku
{
    public class Sudoku : ObservableObject
    {
        private List<SudokuCell> _cellList;

        /// <summary>
        ///     九宫数独题
        /// </summary>
        /// <param name="subject">0,9, 0代表未知</param>
        public Sudoku(byte[,] subject)
        {
            Subject = subject;
            CellList = new List<SudokuCell>();

            if (RowCount != 9 && ColumnCount != 9)
            {
                throw new ArgumentException();
            }

            CellList = CvtToSudokuList(subject);
        }

        /// <summary>
        ///     单行字符串,0代表位置
        /// </summary>
        /// <param name="subject"></param>
        public Sudoku(string subject)
            : this(CvtSubject(subject))
        {
        }

        public int RowCount => 9;
        public int ColumnCount => 9;
        public byte[,] Subject { internal set; get; }

        public byte[,] Answer => GetAnswer();

        public List<SudokuCell> CellList
        {
            protected set => SetProperty(ref _cellList, value);
            get => _cellList;
        }

        public SudokuCell[] this[int row] => GetRow(row);
        public SudokuCell this[int row, int column] => GetSudokuBlock(row, column);

        private byte[,] GetAnswer()
        {
            var answer = new byte[9, 9];
            foreach (var r in Enumerable.Range(0, 9))
            {
                foreach (var c in Enumerable.Range(0, 9))
                {
                    answer[r, c] = this[r, c].Number;
                }
            }

            return answer;
        }


        /// <summary>
        ///     获取一行
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public SudokuCell[] GetRow(int row)
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
        public SudokuCell[] GetColumn(int column)
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
        public SudokuCell GetSudokuBlock(int row, int column)
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


        #region 静态方法

        private static List<SudokuCell> CvtToSudokuList(byte[,] input)
        {
            var res = new List<SudokuCell>();
            foreach (var r in Enumerable.Range(0, 9))
            {
                foreach (var c in Enumerable.Range(0, 9))
                {
                    var value = input[r, c];
                    res.Add(new SudokuCell(value != 0, value, (byte) c, (byte) r));
                }
            }

            return res;
        }

        public static byte[,] CvtSubject(string subject)
        {
            if (subject.Length != 81)
            {
                throw new ArgumentException();
            }

            var res = new byte[9, 9];
            foreach (var r in Enumerable.Range(0, 9))
            {
                foreach (var c in Enumerable.Range(0, 9))
                {
                    res[r, c] = byte.Parse(subject.Substring(r * 9 + c, 1));
                }
            }

            return res;
        }

        #endregion
    }
}