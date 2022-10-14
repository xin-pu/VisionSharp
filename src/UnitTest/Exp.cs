using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace UnitTest
{
    public class Exp : AbstractTest
    {
        public Exp(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void TestRead()
        {
            using var stringReader = new StreamReader(@"F:\QR\Annotations\0050027974.json");
            var str = stringReader.ReadToEnd();


            var obj = JsonConvert.DeserializeObject(str) as JObject;

            var width = obj["imageHeight"];
            var height = obj["imageWidth"];
            var shapes = obj["shapes"];
            foreach (var jToken in shapes)
            {
                var a = jToken["points"];
                var dd = a?.ToArray();
                var x1 = dd?[0][0]?.Value<double>();
                var y1 = dd?[0][1]?.Value<double>();
                var x2 = dd?[1][0]?.Value<double>();
                var y2 = dd?[1][1]?.Value<double>();
            }

            PrintObject(width);
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
    }
}