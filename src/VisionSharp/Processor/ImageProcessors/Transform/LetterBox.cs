using OpenCvSharp;

namespace VisionSharp.Processor.Transform
{
    public class LetterBox : ImageProcessor
    {
        public LetterBox(string name = "LetterBox")
            : base(name)
        {
        }

        internal Size SrcSize { set; get; }

        internal override Mat Process(Mat input)
        {
            SrcSize = input.Size();
            var netInputImage = input.Clone();

            var col = SrcSize.Width;
            var row = SrcSize.Height;
            var maxLen = new[] {col, row}.Max();

            if (maxLen > 1.2 * col || maxLen > 1.2 * row)
            {
                var resizeImage = Mat.Zeros(maxLen, maxLen, input.Type()).ToMat();
                input.CopyTo(resizeImage[new Rect(0, 0, col, row)]);
                netInputImage = resizeImage;
            }

            return netInputImage;
        }
    }
}
