using FluentAssertions;
using OpenCvSharp;
using VisionSharp.Processor.TextDetectors;
using Xunit.Abstractions;

namespace UnitTest.ProcessorTest
{
    public class TextProcessor : AbstractTest
    {
        public TextProcessor(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        [Fact]
        public void TextDetectorTest()
        {
            var barcodeDetector = new BarcodeDetector();
            var mat = Cv2.ImRead(@"..\..\..\..\testimages\barcode.png");
            mat.Should().NotBeNull();
            var res = barcodeDetector.Call(mat, mat);
            PrintObject(res.Result);
            res.Confidence.Should().BeTrue();
        }

        [Fact]
        public void BarCodeTest()
        {
            var barcodeDetector = new BarcodeDetector();
            var mat = Cv2.ImRead(
                @"F:\QR\JPEGImages\2127267576.jpg");
            mat.Should().NotBeNull();
            var res = barcodeDetector.Call(mat);
            PrintObject(res);
            //res.Should().Be("ABC-abc-1234");
        }

        [Fact]
        public void QrCodeCodeTest()
        {
            var barcodeDetector = new BarcodeDetector();
            var mat = Cv2.ImRead(@"..\..\..\..\testimages\qrcode.png");
            mat.Should().NotBeNull();
            var res = barcodeDetector.Call(mat);
            PrintObject(res);
        }

        [Fact]
        public void LRPDetectotTest()
        {
        }
    }
}