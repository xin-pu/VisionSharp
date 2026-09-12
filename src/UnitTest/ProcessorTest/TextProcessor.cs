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
            using var mat = Cv2.ImRead(TestImage("barcode.png"));
            mat.Should().NotBeNull();
            using var res = barcodeDetector.Call(mat, mat);
            PrintObject(res.Result);
            res.Confidence.Should().BeTrue();
        }

        [Trait("Category", "RequiresLocalAssets")]
        [Fact]
        public void BarCodeTest()
        {
            var barcodeDetector = new BarcodeDetector();
            using var mat = Cv2.ImRead(
                @"E:\OneDriver Core\OneDrive\Documents\ShareX\Screenshots\2023-12\OUTLOOK_c0OEMa3p2K.png");
            mat.Should().NotBeNull();
            var res = barcodeDetector.Call(mat);
            PrintObject(res);
            res.Should().Be("ABC-abc-1234");
        }

        [Fact]
        public void QrCodeTest()
        {
            var barcodeDetector = new BarcodeDetector();
            using var mat = Cv2.ImRead(TestImage("qrcode.png"));
            mat.Should().NotBeNull();
            var res = barcodeDetector.Call(mat);
            PrintObject(res);
        }

        [Trait("Category", "RequiresLocalAssets")]
        [Fact]
        public void DiameterDetectorTest()
        {
            using var image = Cv2.ImRead(@"D:\Download\MicrosoftTeams-image (2).png", ImreadModes.Unchanged);
            using var outMat = new DiameterDetector().Call(image);
            PrintMatrix(outMat);
        }
    }
}
