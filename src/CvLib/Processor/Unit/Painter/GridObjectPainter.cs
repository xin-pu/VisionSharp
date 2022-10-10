using System;
using System.Linq;
using MathNet.Numerics.Random;
using OpenCvSharp;

namespace CVLib.Processor.Unit.Painter
{
    public class GridObjectPainter : Processor<Tuple<Size, Size>, Tuple<Mat, bool[,]>>
    {
        public GridObjectPainter(string name = "GridObjectPainter")
            : base(name)
        {
        }

        internal Scalar getRandomScalar(bool isObject = true)
        {
            var level = SystemRandomSource.Default.NextDouble();
            var intLevel = isObject
                ? 255 - (int) (level * 256 / 2)
                : (int) (level * 256 / 5);
            return Scalar.FromRgb(intLevel, intLevel, intLevel);
        }

        internal override Tuple<Mat, bool[,]> Process(Tuple<Size, Size> input)
        {
            var imageSize = input.Item1;
            var gridSize = input.Item2;

            var bgdcolor = getRandomScalar(false);
            var mat = new Mat(imageSize, MatType.CV_8UC1, bgdcolor);
            var array = new bool[gridSize.Height, gridSize.Width];


            Enumerable.Range(0, gridSize.Height).ToList().ForEach(r =>
            {
                Enumerable.Range(0, gridSize.Width).ToList().ForEach(c =>
                {
                    var isObject = SystemRandomSource.Default.NextBoolean();
                    array[r, c] = isObject;
                    if (isObject)
                    {
                        var x = (c + 1) * 80;
                        var y = (r + 1) * 35;
                        DrawObject(mat, new Point(x, y));
                    }
                });
            });

            return new Tuple<Mat, bool[,]>(mat, array);
        }

        internal void DrawObject(Mat mat, Point pos)
        {
            var ratio = (int) (15 * (0.9 + SystemRandomSource.Default.NextDouble() / 5));
            mat.Circle(pos, ratio, getRandomScalar(), -1);
        }
    }
}