using OpenCvSharp;
using VisionSharp.Models.Sudoku;

namespace VisionSharp.Processor.Slovers
{
    public class SudokuDetector : FeatureExtractor<SudokuSubject>
    {
        /// <summary>
        ///     数独检测器
        /// </summary>
        public SudokuDetector() : base("SudokuDetector")
        {
        }

        internal override SudokuSubject Process(Mat input)
        {
            throw new NotImplementedException();
        }
    }
}