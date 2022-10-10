using System.Text;
using OpenCvSharp;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     最上层抽象处理区
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class Processor<T1, T2>
    {
        protected Processor(string name)
        {
            Name = name;
        }


        public string OutPutDire => Path.Combine(Environment.CurrentDirectory, "Temp", Name);

        public string Name { internal set; get; }

        public string FileName { set; get; }

        public bool SaveOutMat { set; get; } = true;
        public bool DrawInfo { set; get; } = true;

        public Scalar PenColor => Scalar.OrangeRed;

        /// <summary>
        ///     If you want to keep input, you should insert a clone of input.
        ///     Is you want to save memory, you can insert the original input, but it will be change in the process.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public RichInfo<T2> Call(T1 input, Mat mat = null, string saveName = "")
        {
            try
            {
                var matOut = new Mat().Clone();

                /// Step 1 Process and get Result
                var result = Process(input);

                /// Step 2 Give a Score
                var confi = CalScore(result);

                /// Draw 3 Draw Result and Score to Mat
                if (mat != null)
                {
                    var color = mat.Type() == MatType.CV_8UC3
                        ? mat
                        : mat.CvtColor(ColorConversionCodes.GRAY2BGR);
                    matOut = DrawMat(color, result, confi, saveName);
                }

                return new RichInfo<T2>(result, confi, matOut);
            }
            catch (Exception ex)
            {
                return new RichInfo<T2>(ex.Message);
            }
        }

        public T2 CallLight(T1 input)
        {
            try
            {
                /// Step 1 Process and get Result
                return Process(input);
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        internal Mat DrawMat(Mat mat, T2 result, bool score, string savename)
        {
            if (!Directory.Exists(OutPutDire))
            {
                Directory.CreateDirectory(OutPutDire);
            }

            try
            {
                mat = Draw(mat.Clone(), result);

                if (!SaveOutMat)
                {
                    return mat;
                }

                FileName = savename == ""
                    ? Path.Combine(OutPutDire, $"{DateTime.Now:MM_dd_HH_mm_ss}_{DateTime.Now.Ticks}.png")
                    : Path.Combine(OutPutDire, $"{savename}.png");
                mat.SaveImage(FileName);
                return mat;
            }
            catch (Exception ex)
            {
                /// Todo Return Error Mat
                return mat;
            }
        }


        /// <summary>
        ///     The main method for your process
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal abstract T2 Process(T1 input);


        /// <summary>
        ///     When you input a mat, we will draw a result mat.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal virtual Mat Draw(Mat mat, T2 result)
        {
            return mat;
        }

        /// <summary>
        ///     You need give a score between 0 and 1 for your process
        ///     0 means bad, 1 means good
        /// </summary>
        /// <returns></returns>
        internal virtual bool CalScore(T2 result)
        {
            return true;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"Processor:{Name}");
            return str.ToString();
        }


        #region Normal DrawFunction

        public Mat DrawPoint(Mat mat, Point point, Scalar color, int size = 20, int thickness = 3)
        {
            mat.Line(point + new Point(-size, 0), point + new Point(size, 0), color, thickness);
            mat.Line(point + new Point(0, size), point + new Point(0, -size), color, thickness);
            return mat;
        }

        public Mat DrawLine(Mat mat, Point pointStart, Point pointEnd, Scalar color, int thickness = 3)
        {
            mat.Line(pointStart, pointEnd, color, thickness);
            return mat;
        }

        public Mat DrawRotatedRect(Mat mat, RotatedRect rect, Scalar color, int size = 10, int thickness = 3)
        {
            var points = Cv2.BoxPoints(rect).ToList();
            var cons = points.Select(a => a.ToPoint());
            Cv2.DrawContours(mat, new[] {cons}, -1, color, thickness);

            points.Add(rect.Center.ToPoint());
            return mat;
        }

        public Mat DrawRect(Mat mat, Rect rect, Scalar color, int size = 10, int thickness = 3)
        {
            var topRight = new Point(rect.Right, rect.Top);
            var bottomLeft = new Point(rect.Left, rect.Bottom);
            var points = new[] {rect.TopLeft, rect.BottomRight, topRight, bottomLeft};
            var rotatcedRect = Cv2.MinAreaRect(points);
            return DrawRotatedRect(mat, rotatcedRect, color, size, thickness);
        }

        public Mat DrawText(Mat mat, Point point, string info, Scalar color, int fontScale = 1, int thickness = 3)
        {
            var size = Cv2.GetTextSize(info, HersheyFonts.HersheyPlain, fontScale, thickness, out var base_line);

            var newpoint = point + new Point(0, size.Height + 10);

            var rectSize = new Size(size.Width, size.Height + 10);
            Cv2.Rectangle(mat, new Rect(point, rectSize), color, -1);
            Cv2.PutText(mat, info, newpoint, HersheyFonts.HersheyPlain,
                fontScale,
                new Scalar(255, 255, 255),
                thickness);
            return mat;
        }

        #endregion
    }
}