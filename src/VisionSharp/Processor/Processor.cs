using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;
using VisionSharp.Utils;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     最上层抽象处理区
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class Processor<T1, T2> : ObservableObject
    {
        private bool _enableDrawInfo = true;
        private bool _enableSaveMat = true;
        private string _fileName;
        private string _name;

        protected Processor(string name)
        {
            Name = name;
        }


        public string OutPutDire => Path.Combine(Environment.CurrentDirectory, "Temp", Name);
        public Scalar PenColor => Scalar.OrangeRed;

        public string Name
        {
            internal set => SetProperty(ref _name, value);
            get => _name;
        }

        public string FileName
        {
            internal set => SetProperty(ref _fileName, value);
            get => _fileName;
        }

        public bool EnableSaveMat
        {
            set => SetProperty(ref _enableSaveMat, value);
            get => _enableSaveMat;
        }

        public bool EnableDrawInfo
        {
            set => SetProperty(ref _enableDrawInfo, value);
            get => _enableDrawInfo;
        }


        /// <summary>
        ///     If you want to keep input, you should insert a clone of input.
        ///     Is you want to save memory, you can insert the original input, but it will be change in the process.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public RichInfo<T2?> Call(T1 input, Mat? mat = null, string saveName = "")
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

                return new RichInfo<T2?>(result, confi, matOut);
            }
            catch (Exception ex)
            {
                return new RichInfo<T2?>(ex.Message);
            }
        }

        public T2? CallLight(T1 input)
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

        internal Mat? DrawMat(Mat? mat, T2? result, bool score, string savename)
        {
            if (!Directory.Exists(OutPutDire))
            {
                Directory.CreateDirectory(OutPutDire);
            }

            try
            {
                mat = Draw(mat.Clone(), result);

                if (!EnableSaveMat)
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
        internal virtual Mat? Draw(Mat? mat, T2 result)
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


        #region 内部绘画功能

        /// <summary>
        ///     绘制点
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="point"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        internal Mat DrawPoint(Mat mat, Point point, Scalar color, int size = 20, int thickness = 3)
        {
            return CvDraw.DrawPoint(mat, point, color, size, thickness);
        }

        /// <summary>
        ///     绘制线
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="pointStart"></param>
        /// <param name="pointEnd"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        internal Mat DrawLine(Mat mat, Point pointStart, Point pointEnd, Scalar color, int thickness = 3)
        {
            return CvDraw.DrawLine(mat, pointStart, pointEnd, color, thickness);
        }

        /// <summary>
        ///     绘制矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        internal Mat DrawRect(Mat mat, Rect rect, Scalar color, int thickness = 3)
        {
            return CvDraw.DrawRect(mat, rect, color, thickness);
        }

        /// <summary>
        ///     绘制旋转矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rotatedRect"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        internal Mat DrawRotatedRect(Mat mat, RotatedRect rotatedRect, Scalar color, int thickness = 3)
        {
            return CvDraw.DrawRotatedRect(mat, rotatedRect, color, thickness);
        }

        /// <summary>
        ///     绘制文字
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="point"></param>
        /// <param name="info"></param>
        /// <param name="color"></param>
        /// <param name="fontScale"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        internal Mat DrawText(Mat mat, Point point, string info, Scalar color, int fontScale = 1, int thickness = 3)
        {
            return CvDraw.DrawText(mat, point, info, color, fontScale, thickness);
        }

        #endregion
    }
}