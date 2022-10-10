using System;
using System.Collections.Generic;
using System.Linq;
using CVLib.Models;
using OpenCvSharp;

namespace CVLib.Processor.Unit.Painter
{
    /// <summary>
    ///     布局绘画器
    /// </summary>
    public class TrayLayoutPainter : Processor<Tuple<Mat, DetectLayout>, Mat>
    {
        internal Dictionary<LayoutStatus, Scalar> ColorDict = new()
        {
            [LayoutStatus.Positive] = new Scalar(0, 0, 255),
            [LayoutStatus.Negative] = new Scalar(106, 255, 0),
            [LayoutStatus.Unidentified] = new Scalar(0, 106, 255)
        };

        public TrayLayoutPainter()
            : base("TrayLayoutPainter")
        {
        }

        internal override Mat Process(Tuple<Mat, DetectLayout> input)
        {
            var (mat, result) = input;
            var PatternSize = new Size(result.Column, result.Row);
            var space = mat.Height / 100;
            var fontscale = 2 * mat.Height / 1000;
            var thickness = 5;
            var grid_width = mat.Size().Width / PatternSize.Width;
            var grid_height = mat.Size().Height / PatternSize.Height;
            var grid_size = new Size(grid_width - space, grid_height - space);

            var pointStart = new Point(0, 0);

            Enumerable.Range(0, PatternSize.Height).ToList().ForEach(r =>
                Enumerable.Range(0, PatternSize.Width).ToList().ForEach(c =>
                {
                    var status = result.getDetectPosition(r, c);
                    var color = ColorDict[status.LayoutStatus];
                    var info = $"{status.LayoutStatus}:{status.GetScore():F4}";
                    var currentPoint = pointStart +
                                       new Point(grid_width * c, grid_height * r) +
                                       new Point(space / 2, space / 2);

                    mat = DrawRect(mat, new Rect(currentPoint, grid_size), color, 1, thickness);

                    DrawText(mat, currentPoint, info, color, fontscale, thickness);
                }));
            return mat;
        }

        internal override Mat Draw(Mat mat, Mat result)
        {
            return result;
        }
    }
}