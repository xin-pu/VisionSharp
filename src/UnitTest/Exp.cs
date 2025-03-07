using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using VisionSharp.Processor.FeatureExtractors;
using Xunit.Abstractions;

namespace UnitTest
{
    public class Exp : AbstractTest
    {
        public Exp(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }


        public string GetFormat(string jsonFile)
        {
            var imageFolder = "F:\\QR\\JPEGImages\\";
            using var stringReader = new StreamReader(jsonFile);
            var obj = JsonConvert.DeserializeObject(stringReader.ReadToEnd()) as JObject;

            var str = new StringBuilder();
            str.Append(obj?["imagePath"].Value<string>().Replace("./", imageFolder));
            str.Append(" ");
            var shapes = obj["shapes"];
            foreach (var jToken in shapes)
            {
                var a = jToken["points"];
                var dd = a?.ToArray();
                var x1 = dd[0][0].Value<double>();
                var y1 = dd[0][1].Value<double>();
                var x2 = dd[1][0].Value<double>();
                var y2 = dd[1][1].Value<double>();
                var aray = new[]
                {
                    (int) Math.Round(x1, MidpointRounding.AwayFromZero),
                    (int) Math.Round(y1, MidpointRounding.AwayFromZero),
                    (int) Math.Round(x2, MidpointRounding.AwayFromZero),
                    (int) Math.Round(y2, MidpointRounding.AwayFromZero),
                    0
                };
                var shape = string.Join(",", aray);
                str.Append($"{shape} ");
            }

            return str.ToString();
        }

        [Fact]
        public void Formmat()
        {
            var stream = "F:\\QR\\train.txt";


            using var sw = new StreamWriter(stream);
            {
                var list = new List<string>();
                var work = "F:\\QR\\Annotations";
                foreach (var fileInfo in new DirectoryInfo(work).GetFiles("*.json"))
                {
                    var a = GetFormat(fileInfo.FullName);
                    PrintObject(a);
                    list.Add(a);
                }

                sw.Write(string.Join("\r\n", list));
            }
        }

        [Fact]
        public void TestGetCircle()
        {
            var image = Cv2.ImRead(@"D:\Download\MicrosoftTeams-image.png");

            var hsv = new Mat();
            Cv2.CvtColor(image, hsv, ColorConversionCodes.BGR2HSV);

            var outmat1 = new Mat();
            Cv2.InRange(hsv, new Scalar(156, 43, 46), new Scalar(180, 255, 255), outmat1);


            Cv2.ImWrite("test.jpg", outmat1);
            var element = Cv2.GetStructuringElement(
                MorphShapes.Ellipse,
                new Size(3, 3),
                new Point(-1, -1));

            Cv2.MorphologyEx(outmat1, outmat1, MorphTypes.Close, element, new Point(-1, -1));
            Cv2.MorphologyEx(outmat1, outmat1, MorphTypes.Open, element, new Point(-1, -1));


            Cv2.ImShow("ori", outmat1);
            Cv2.WaitKey();

            var circleFinder = new CircleDetector
            {
                BlobColor = 255,
                FilterByCircularity = true,
                MinCircularity = 0.3,
                MaxCircularity = 1,
                FilterByArea = true,
                MinArea = 10,
                MaxArea = 200,
                PenColor = new Scalar(255, 255, 255)
            };
            var final = circleFinder.Call(outmat1, image);

            Cv2.ImShow("ori", final.OutMat);
            Cv2.WaitKey();
        }


        [Fact]
        public void ImRead()
        {
            var res = Cv2.ImRead(@"F:\DogsCats\train\cat.36.jpg");
            var mat2 = new Mat();
            Cv2.CvtColor(res, mat2, ColorConversionCodes.RGB2GRAY);


            Cv2.Threshold(mat2, mat2, 127, 255, ThresholdTypes.Binary);

            var a = new Mat();
            Cv2.FindContours(mat2,
                out var cors,
                a,
                RetrievalModes.List,
                ContourApproximationModes.ApproxSimple);
            Cv2.ImShow("Hello", mat2);
            Cv2.PutText(mat2, "123", new Point(3, 4), HersheyFonts.HersheySimplex, 1, Scalar.Red, 2);
            Cv2.DrawContours(mat2, cors, -1, Scalar.Red);


            var rect = cors[190].MinAreaRect();
            PrintRotatedRects(new[] {rect});

            Cv2.WaitKey();
        }
    }
}