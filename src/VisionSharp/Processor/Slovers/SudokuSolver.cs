using VisionSharp.Models.Sudoku;

namespace VisionSharp.Processor.Slovers
{
    public class SudokuSolver : Processor<Sudoku, Sudoku>
    {
        /// <summary>
        ///     数独解题器
        /// </summary>
        public SudokuSolver() : base("SudokuSolver")
        {
        }

        internal override Sudoku Process(Sudoku input)
        {
            var row = new bool[9, 9]; // 行
            var col = new bool[9, 9]; // 列
            var block = new bool[9, 9]; // 块

            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    var cell = input[i, j];
                    if (cell.Number != 0)
                    {
                        var num = cell.Number - 1; // 数字 1-9 转为下标 0-8
                        row[i, num] = true;
                        col[j, num] = true;
                        block[i / 3 * 3 + j / 3, num] = true;
                    }
                }
            }

            DepthFirstAlgorithm(input, row, col, block, 0, 0);
            return input;
        }


        /// <summary>
        ///     这里采用深度优先算法来解决
        ///     这里的返回值是boolean，其实就是一个钩子，这是起到一个判断是否合适的作用
        /// </summary>
        /// <param name="board"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="block"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool DepthFirstAlgorithm(Sudoku board, bool[,] row, bool[,] col, bool[,] block, int i, int j)
        {
            // 结束条件
            while (board[i, j].Number != 0)
            {
                // 9个一轮回
                if (++j >= 9)
                {
                    ++i;
                    j = 0;
                }

                // 说明全部搜索完成了
                if (i >= 9)
                {
                    return true;
                }
            }

            // 下面是算法的精髓之处，利用回溯来解决
            for (var num = 0; num < 9; num++)
            {
                // 定位所在块的位置
                var blockIndex = i / 3 * 3 + j / 3;
                if (!row[i, num] && !col[j, num] && !block[blockIndex, num])
                {
                    board[i, j].Number = (byte) (1 + num);
                    row[i, num] = true;
                    col[j, num] = true;
                    block[blockIndex, num] = true;
                    if (DepthFirstAlgorithm(board, row, col, block, i, j))
                    {
                        return true;
                    }

                    // 这里失败了就需要进行回溯
                    board[i, j].Number = 0;
                    row[i, num] = false;
                    col[j, num] = false;
                    block[blockIndex, num] = false;
                }
            }

            return false;
        }

        #region Expand

        /// <summary>
        ///     校验解题结果：用题目求解后与给定答案逐格比对
        /// </summary>
        /// <param name="subject">数独题目字符串（81 位，0 代表未知）</param>
        /// <param name="answer">标准答案字符串（81 位）</param>
        /// <returns>解题结果与答案完全一致时为 true</returns>
        public bool Verify(string subject, string answer)
        {
            var sudoku = new Sudoku(subject);
            var answerPred = Call(sudoku).Answer;
            var answerTrue = Sudoku.CvtSubject(answer);
            return answerPred.Cast<byte>().SequenceEqual(answerTrue.Cast<byte>());
        }

        #endregion
    }
}