using System.Drawing;
using ZXing;

namespace CVLib.Processor.Unit
{
    public class BarcodeDetector : Processor<Bitmap, string>
    {
        public BarcodeDetector() : base("BarcodeDetector")
        {
        }

        internal override string Process(Bitmap input)
        {
            IBarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(input);
            // do something with the result
            return result != null ? result.Text : string.Empty;
        }
    }
}