using VisionSharp.Models.Sudoku;
using VisionSharp.Processor.Slovers;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class SudokuSolverTest : AbstractTest
    {
        private readonly byte[,] demo =
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };

        public SudokuSolverTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void PrintSudoku()
        {
            var sudokuSubject = new SudokuSubject(demo);
            PrintObject(sudokuSubject);
        }

        [Fact]
        public void SolveSudoku()
        {
            var sudokuSubject = new SudokuSubject(demo);
            var solve = new SudokuSolver();
            var s = solve.Call(sudokuSubject);
            PrintObject(s);
        }
    }
}