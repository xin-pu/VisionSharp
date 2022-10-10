using FluentAssertions;
using OpenCvSharp;
using VisionSharp.Models.Ext;
using VisionSharp.Utils;
using Xunit.Abstractions;

namespace UnitTest.UtilsTest
{
    public class CvMathTests : AbstractTest
    {
        private static readonly Point2d BottomRight = new(2, 2);
        private static readonly Point2d TopLeft = new(0, 0);
        private static readonly Size Size = new(BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y);
        private static readonly Rect Rect = new(TopLeft.ToPoint(), Size);

        private static readonly RotatedRect RotatedRect = new(new Point2f(1, 1),
            new Size2f(Size.Width, Size.Height), 90);

        public CvMathTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void GetMeanPoint2FTest()
        {
            var points = new[] {TopLeft, BottomRight};

            var res = CvMath.GetMeanPoint2F(points);
            PrintPoint2d(res);
            res.X.Should().Be(1f);
            res.Y.Should().Be(1f);

            var point2Fs = points.Select(p => new Point2f((float) p.X, (float) p.Y)).ToList();
            res = CvMath.GetMeanPoint2F(point2Fs);
            PrintPoint2d(res);

            res.X.Should().Be(1f);
            res.Y.Should().Be(1f);

            res = CvMath.GetMeanPoint2F(point2Fs.Select(a => a.ToPoint()).ToList());
            PrintPoint2d(res);
            res.X.Should().Be(1f);
            res.Y.Should().Be(1f);

            var point2F = CvMath.GetMeanPoint2F(Rect);
            point2F.X.Should().Be(1f);
            point2F.Y.Should().Be(1f);

            point2F = CvMath.GetMeanPoint2F(RotatedRect);
            point2F.X.Should().Be(1f);
            point2F.Y.Should().Be(1f);

            PrintObject(new CvRotatedRect(RotatedRect));
        }


        [Fact]
        public void LinefitTest()
        {
            var points = new[] {TopLeft, BottomRight};
            var point2Fs = points.Select(CvCvt.CvtToPoint2f);
            var res = CvMath.Linefit(point2Fs);
            res.Item1.Should().Be(0);
            res.Item2.Should().Be(1);
        }


        [Fact]
        public void GetPointTest()
        {
            var point = CvMath.GetPoint(1, new Tuple<double, double>(0, 1));
            PrintPoint2d(point);
            point.X.Should().Be(1);
            point.Y.Should().Be(1);
        }

        [Fact]
        public void GetCenterTest()
        {
            var points = new[] {TopLeft, BottomRight};
            var center = CvMath.GetCenter(points);
            center.X.Should().Be(1);
            center.Y.Should().Be(1);

            center = CvMath.GetCenter(points.Select(a => a.ToPoint()));
            center.X.Should().Be(1);
            center.Y.Should().Be(1);
        }


        [Fact]
        public void GetDistanceTest()
        {
            var dis = CvMath.GetDistance(TopLeft, BottomRight);
            PrintObject(dis);
            dis.Should().BeInRange(2.80, 2.83);

            dis = CvMath.GetDistance(TopLeft.ToPoint(), BottomRight.ToPoint());
            dis.Should().BeInRange(2.80, 2.83);

            dis = CvMath.GetDistance(CvCvt.CvtToPoint2f(TopLeft), CvCvt.CvtToPoint2f(BottomRight));
            dis.Should().BeInRange(2.80, 2.83);

            dis = CvMath.GetDistance(new Point3d(0, 0, 0), new Point3d(0, 0, 1));
            dis.Should().Be(1);
        }


        [Theory]
        [InlineData(5, 10)]
        public void GetPatternPointTest(int patternWidth, int patternHeight)
        {
            var topleft = new Point2d(-40, 50);
            var bottomright = new Point2d(175, -145);


            var point2ds = CvMath.GetPatternPoint(topleft, bottomright, new Size(patternWidth, patternHeight));
            var point3ds = point2ds.Select(a => new Point3d(a.X, a.Y, 1));
            PrintPoint3ds(point3ds);
        }

        [Theory]
        [InlineData(8, 6)]
        public void GetPatternPointTest1(int patternWidth, int patternHeight)
        {
            var topleft = new Point2d(-38, 41.8);
            var topright = new Point2d(184.9, 42.897);
            var bottomleft = new Point2d(-37.5, -117.1);


            var position = CvMath.GetPatternPoint(topleft, topright, bottomleft, new Size(patternWidth, patternHeight));
            var positions = position.Select(a => new Point3d(a.X, a.Y, 1));
            PrintPoint3ds(positions);
        }

        [Fact]
        public void GetRatioTest()
        {
            var ratio = CvMath.GetRatio(RotatedRect);
            ratio.Should().Be(1);
        }


        [Fact]
        public void GetAngleTest()
        {
            var start = new Point2f(0, 0);

            var angel = CvMath.GetAngle(start, new Point2f(1, (float) Math.Sqrt(3)));
            angel.Should()
                .BeInRange(60 - 0.1, 60 + 0.1);

            angel = CvMath.GetAngle(start, new Point2f(-1, -(float) Math.Sqrt(3)));
            angel.Should()
                .BeInRange(240 - 0.1, 240 + 0.1);
        }
    }
}