using OpenCvSharp;
using VisionSharp.Utils;
using ZXing;

namespace VisionSharp.Processor.TextDetectors
{
    public class BarcodeDetector : TextDetector
    {
        public BarcodeDetector() : base("BarcodeDetector")
        {
        }

        internal override string Process(Mat input)
        {
            var bitmap = CvCvt.CvtToBitmap(input);
            var reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            return result != null ? result.Text : string.Empty;
        }
    }
}