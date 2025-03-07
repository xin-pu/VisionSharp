using FluentAssertions;
using OpenCvSharp;
using VisionSharp.Processor.FeatureExtractors;
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
                @"E:\OneDriver Core\OneDrive - Coherent Corporation\Documents\ShareX\Screenshots\2023-12\OUTLOOK_c0OEMa3p2K.png");
            mat.Should().NotBeNull();
            var res = barcodeDetector.Call(mat);
            PrintObject(res);
            res.Should().Be("ABC-abc-1234");
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
            var image = Cv2.ImRead(@"D:\Download\MicrosoftTeams-image (2).png", ImreadModes.Unchanged);
            var outMat = new DiameterDetector().Call(image);

            Cv2.ImShow("d", outMat);
            Cv2.WaitKey();
        }
    }
}