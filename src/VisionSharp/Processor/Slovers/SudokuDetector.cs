using OpenCvSharp;
using VisionSharp.Models.Sudoku;

namespace VisionSharp.Processor.Slovers
{
    public class SudokuDetector : FeatureExtractor<Sudoku>
    {
        /// <summary>
        ///     数独检测器
        /// </summary>
        public SudokuDetector() : base("SudokuDetector")
        {
        }

        internal override Sudoku Process(Mat input)
        {
            throw new NotImplementedException();
        }
    }
}