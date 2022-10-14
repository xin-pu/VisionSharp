using FluentAssertions;
using VisionSharp.Models.Sudoku;
using VisionSharp.Processor.Slovers;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class SudokuSolverTest : AbstractTest
    {
        private readonly byte[,] _demo =
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
            var sudokuSubject = new Sudoku(_demo);
            PrintObject(sudokuSubject);
        }

        [Fact]
        public void SolveSudoku()
        {
            byte[,] _demo =
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
            var sudokuSubject = new Sudoku(_demo);
            PrintObject(sudokuSubject);
            var solve = new SudokuSolver();
            var s = solve.Call(sudokuSubject);
            PrintObject(s);
        }

        [Fact]
        public void SolveSudoku2()
        {
            var s = "008317000004205109000040070327160904901450000045700800030001060872604000416070080";
            var a = "298317645764285139153946278327168954981453726645792813539821467872634591416579382";

            var sudokuSubject = new Sudoku(s);
            PrintObject(sudokuSubject);
            var solve = new SudokuSolver();
            var res = solve.Call(sudokuSubject);
            PrintObject(res);

            res.Answer.Should().BeEquivalentTo(Sudoku.CvtSubject(a));
        }

        [Fact]
        public void Verify()
        {
            var s = "008317000004205109000040070327160904901450000045700800030001060872604000416070080";
            var a = "298317645764285139153946278327168954981453726645792813539821467872634591416579382";
            var slove = new SudokuSolver();
            var res = slove.Verify(s, a);
            PrintObject(res);
        }
    }
}