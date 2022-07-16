using System;
using System.IO;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     最上层抽象处理区
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class Processor<T1, T2> : ViewModelBase
    {
        protected Processor(string name)
        {
            Name = name;
        }


        public string OutPutDire => Path.Combine(Environment.CurrentDirectory, "Temp", Name);

        public string Name { internal set; get; }

        public string FileName { internal set; get; }

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
        public RichInfo<T2> Call(T1 input, Mat mat = null)
        {
            try
            {
                var matOut = new Mat().Clone();

                /// Step 1 Process and get Result
                var result = Process(input);

                /// Step 2 Give a Score
                var score = CalScore(result);

                /// Draw 3 Draw Result and Score to Mat
                if (mat != null)
                {
                    var color = mat.Type() == MatType.CV_8UC3
                        ? mat
                        : mat.CvtColor(ColorConversionCodes.GRAY2BGR);
                    matOut = DrawMat(color, result, score);
                }

                return new RichInfo<T2>(result, score, matOut);
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
            catch (Exception)
            {
                return default;
            }
        }

        internal Mat DrawMat(Mat mat, T2 result, double score)
        {
            if (!Directory.Exists(OutPutDire)) Directory.CreateDirectory(OutPutDire);

            try
            {
                var fontScale = mat.Width / 500;
                fontScale = fontScale > 1 ? fontScale : 1;
                var distance = mat.Height / 1000 + 1;
                distance = distance > 1 ? distance : 1;


                mat = Draw(mat.Clone(), result);
                if (DrawInfo)
                {
                    mat.PutText($"Time:   {DateTime.Now:yy/MM/dd hh:mm:ss}", new Point(100, 120 * (distance - 1) - 50),
                        HersheyFonts.HersheyPlain,
                        fontScale,
                        PenColor, 2);
                    mat.PutText($"Result: {result}", new Point(100, 120 * distance - 50), HersheyFonts.HersheyPlain,
                        fontScale, PenColor, 2);
                    mat.PutText($"Score:  {score:P2}", new Point(100, 120 * (distance + 1) - 50),
                        HersheyFonts.HersheyPlain,
                        fontScale, PenColor, 2);
                }


                if (!SaveOutMat) return mat;

                FileName = Path.Combine(OutPutDire, $"{DateTime.Now:MM_dd_HH_mm_ss}_{DateTime.Now.Ticks}.png");
                mat.SaveImage(FileName);
                return mat;
            }
            catch (Exception)
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
        internal virtual double CalScore(T2 result)
        {
            return 1;
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
            points.ForEach(p => Cv2.Circle(mat, p.ToPoint(), size, color, -1));
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

        #endregion
    }
}