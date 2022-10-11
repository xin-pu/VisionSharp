using FluentAssertions;
using OpenCvSharp;
using VisionSharp.Utils;
using Xunit.Abstractions;

namespace UnitTest.UtilsTest
{
    public class CvCvtTests : AbstractTest
    {
        public CvCvtTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Fact]
        public void CvtToPoint2FTest()
        {
            var res = CvCvt.CvtToPoint2F(new Point2d(0, 0));
            res.X.Should().Be(0);
            res.Y.Should().Be(0);

            res = CvCvt.CvtToPoint2F(new Point(0, 0));
            res.X.Should().Be(0);
            res.Y.Should().Be(0);
        }

        [Fact]
        public void CvtToPoint2FsTest()
        {
            var res = CvCvt.CvtToPoint2Fs(new[]
            {
                new Point(0, 0), new Point(2, 2)
            });
            res.Length.Should().Be(2);
        }


        [Fact]
        public void CvtToPoint3FTest()
        {
            var res = CvCvt.CvtToPoint3F(new Point3d(0, 0, 1));
            res.X.Should().Be(0);
            res.Y.Should().Be(0);
            res.Z.Should().Be(1);
        }


        [Fact]
        public void CvtToPoint3FsTest()
        {
            var res = CvCvt.CvtToPoint3Fs(new[]
            {
                new Point3d(0, 0, 0),
                new Point3d(2, 2, 2)
            });
            res.Length.Should().Be(2);
        }


        [Fact]
        public void CvtToPoint2dTest()
        {
            var res = CvCvt.CvtToPoint2d(new Point(0, 0));
            res.X.Should().Be(0);
            res.Y.Should().Be(0);

            res = CvCvt.CvtToPoint2d(new Point2f(0, 0));
            res.X.Should().Be(0);
            res.Y.Should().Be(0);
        }


        [Fact]
        public void CvtToPoint3dTest()
        {
            var res = CvCvt.CvtToPoint3d(new Point3f(0, 0, 1));
            res.X.Should().Be(0);
            res.Y.Should().Be(0);
            res.Z.Should().Be(1);
        }

        [Fact]
        public void CvtToPoint2dsTest()
        {
            var res = CvCvt.CvtToPoint2ds(new[] {new Point(0, 0), new Point(2, 2)});
            res.Length.Should().Be(2);

            var mat = Mat.FromArray(new[,] {{0d, 0d}, {0d, 2d}});
            PrintMatrix(mat);
            res = CvCvt.CvtToPoint2ds(mat);
            res.Length.Should().Be(2);

            res = CvCvt.CvtToPoint2ds(new Mat<double>(new[] {2, 2}));
            res.Length.Should().Be(2);
        }

        [Fact]
        public void CvtToPoint3dsTest()
        {
            var res = CvCvt.CvtToPoint3ds(new[]
            {
                new Point3f(0, 0, 0),
                new Point3f(0, 0, 3)
            });
            res.Length.Should().Be(2);

            res = CvCvt.CvtToPoint3ds(new Mat<double>(new[] {2, 3}));
            res.Length.Should().Be(2);
        }


        [Fact]
        public void CvtToMatTest()
        {
            var points = new[] {new Point(1F, 2F), new Point(3D, 4D)};
            var mat = CvCvt.CvtToMat(points);
            PrintMatrix(mat);

            points = new[] {new Point(1, 2), new Point(3, 4)};
            mat = CvCvt.CvtToMat(points);
            PrintMatrix(mat);

            var point3ds = new[] {new Point3d(1, 2, 3), new Point3d(3, 4, 5)};
            mat = CvCvt.CvtToMat(point3ds);
            PrintMatrix(mat);
        }

        [Fact]
        public void CutZToPoint2dTest()
        {
            var point = CvCvt.CutZToPoint2d(new Point3d(0, 0, 1));
            point.X.Should().Be(0);
            point.Y.Should().Be(0);
        }


        [Fact]
        public void CvtToRotatedRectTest()
        {
            var rotatedRect = CvCvt.CvtToRotatedRect(new Rect(0, 0, 10, 10));
            PrintObject(rotatedRect);
        }

        [Fact]
        public void CvtToRectTest()
        {
            var rect = CvCvt.CvtToRect(new RotatedRect(new Point2f(5, 5), new Size2f(10, 10), 90));
            PrintObject(rect);
        }
    }
}