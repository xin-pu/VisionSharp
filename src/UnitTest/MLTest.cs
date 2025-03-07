using OpenCvSharp;
using Xunit.Abstractions;

namespace UnitTest
{
    public class MlTest : AbstractTest
    {
        public MlTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) { }

        public string DataPath =>
            @"E:\OneDriver Core\OneDrive - Coherent Corporation\Core Documents\my-projects\Train\data_singlevar.txt";


        public Mat CreateMat(double[] xarray)
        {
            var mat = Mat.Ones(new Size(2, 50), MatType.CV_64F).ToMat();

            foreach (var r in Enumerable.Range(0, mat.Rows))
            {
                mat.Set(r, 0, xarray[r]);
            }

            return mat;
        }

        [Fact]
        public void ReadData()
        {
            var allText = File.ReadAllText(DataPath);
            var lines = allText.Split('\n')
                               .Where(a => !string.IsNullOrEmpty(a));

            var x = new List<double>();
            var y = new List<double>();

            foreach (var line in lines)
            {
                var d = line.Split(',');
                x.Add(double.Parse(d[0]));
                y.Add(double.Parse(d[1]));
            }

            PrintObject(x.Count);
            PrintObject(y.Count);
            var X = CreateMat(x.ToArray());
            var Y = Mat.FromArray(y);
            PrintMatrix(X);
            var res = new Mat();
            Cv2.Solve(X, Y, res, DecompTypes.SVD);
            PrintMatrix(res);

            var k = res.Get<double>(0);
            var b = res.Get<double>(1);
        }


        [Fact]
        public void Test()
        {
            var N = 500;
            var x = Enumerable.Range(0, N).Select(i => i * 0.01).ToArray();
            var y = Enumerable.Range(0, N).Select(i => 0.5 * Math.Sin(2 * x[i] + 1.5) + 0.3).ToArray();
            PrintObject(string.Join(",", y));
            var coefficients = FitSineFunction(x, y);
            PrintMatrix(coefficients[0]);
            PrintMatrix(coefficients[1]);
        }

        private static Mat[] FitSineFunction(double[] x, double[] y)
        {
            var curveFitting = new Mat(x.Length, 3, MatType.CV_64FC1);
            var coefficients = new double[3];
            var Y = Mat.FromArray(y);
            for (var i = 0; i < x.Length; i++)
            {
                var xi = x[i];
                curveFitting.Set(i, 0, Math.Cos(2 * Math.PI * xi));
                curveFitting.Set(i, 1, -Math.Sin(2 * Math.PI * xi));
                curveFitting.Set(i, 2, 1);
            }

            var outd = Mat.FromArray(coefficients);
            Cv2.Solve(curveFitting, Y, outd, DecompTypes.SVD);

            var d = (curveFitting * outd).ToMat();
            return new[] {outd, d};
        }
    }
}