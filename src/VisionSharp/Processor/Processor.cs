#nullable enable
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
    public abstract class Processor<T1, T2> : ObservableObject, IDisposable
    {
        private bool _enableSaveMat = false;
        private string _fileName;
        private string _name;
        private Scalar _penColor = Scalar.OrangeRed;

        protected Processor(string name)
        {
            Name = name;
        }

        public string OutputDirectory => Path.Combine(Environment.CurrentDirectory, "Temp", Name);

        public Scalar PenColor
        {
            set => SetProperty(ref _penColor, value);
            get => _penColor;
        }

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


        /// <summary>
        ///     执行处理器,并在传入MAT上绘制相关处理结果
        ///     绘制结果包含在返回的RichInfo中
        ///     异常直接抛出，不再吞入RichInfo
        /// </summary>
        /// <param name="input">输入对象</param>
        /// <param name="mat">传入图像</param>
        /// <param name="saveName">保存文件名（EnableSaveMat 为 true 时生效）</param>
        /// <returns></returns>
        public virtual RichInfo<T2> Call(T1 input, Mat mat, string saveName = "")
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (mat == null) throw new ArgumentNullException(nameof(mat));

            // Step 1 执行处理
            var result = Process(input);

            // Step 2 分析结果
            var confi = GetReliability(result);

            // Step 3 绘制输出图像（灰度图先转 BGR，转换产生的中间 Mat 用完即释放）
            var colorMat = mat.Type() == MatType.CV_8UC3
                ? null
                : mat.CvtColor(ColorConversionCodes.GRAY2BGR);
            try
            {
                var matOut = DrawMat(colorMat ?? mat, result, confi, saveName);
                return new RichInfo<T2>(result, confi, matOut);
            }
            finally
            {
                colorMat?.Dispose();
            }
        }

        /// <summary>
        ///     快速处理，直接返回处理结果，
        ///     若有异常，跑异常
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public T2 Call(T1 input)
        {
            return Process(input);
        }

        internal Mat DrawMat(Mat mat, T2 result, bool reliability, string savename)
        {
            var canvas = mat.Clone();
            var drawn = Draw(canvas, result, reliability);
            if (!ReferenceEquals(drawn, canvas))
            {
                // Draw 返回了新 Mat（如 ImageProcessor 直接返回处理结果），释放画布
                canvas.Dispose();
            }

            if (EnableSaveMat)
            {
                Directory.CreateDirectory(OutputDirectory);
                FileName = savename == ""
                    ? Path.Combine(OutputDirectory, $"{DateTime.Now:MMdd HH-mm-ss} {DateTime.Now.Millisecond:D3}.png")
                    : Path.Combine(OutputDirectory, $"{savename}.png");
                drawn.SaveImage(FileName);
            }

            return drawn;
        }


        /// <summary>
        ///     实际需要重载的处理器过程
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal abstract T2 Process(T1 input);

        /// <summary>
        ///     绘制带结果信息的图像
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="reliability"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal virtual Mat Draw(Mat mat, T2 result, bool reliability)
        {
            return mat;
        }

        /// <summary>
        ///     获取可靠度
        /// </summary>
        /// <returns></returns>
        internal virtual bool GetReliability(T2 result)
        {
            return true;
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine($"Processor:{Name}");
            return str.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     释放处理器持有的非托管资源（如 DNN 网络）
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
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

        internal Mat DrawCircle(Mat mat, Point center, int size, Scalar color, int thickness = 1)
        {
            return CvDraw.DrawCircle(mat, center, color, size, thickness);
        }

        /// <summary>
        ///     绘制矩形框
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="rect"></param>
        /// <param name="color"></param>
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
        internal Mat DrawText(Mat mat, Point point, string info, Scalar color, double fontScale = 1, int thickness = 1)
        {
            return CvDraw.DrawText(mat, point, info, color, fontScale, thickness);
        }

        #endregion
    }


    /// <summary>
    ///     Process 返回的富信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RichInfo<T> : ObservableObject, IDisposable
    {
        private bool _confidence;
        private string _errorMessage;
        private Mat _outMat;
        private T _result;

        /// <summary>
        ///     正常时的富信息
        /// </summary>
        /// <param name="result"></param>
        /// <param name="conf"></param>
        /// <param name="outmat"></param>
        public RichInfo(T result, bool conf, Mat outmat)
        {
            Confidence = conf;
            Result = result;
            OutMat = outmat;
            ErrorMessage = string.Empty;
        }

        /// <summary>
        ///     异常时的富信息
        /// </summary>
        /// <param name="errorMessage"></param>
        public RichInfo(string errorMessage)
        {
            Confidence = false;
            ErrorMessage = errorMessage;
            Result = default!;
            OutMat = null!;
        }

        public T Result
        {
            internal set => SetProperty(ref _result, value);
            get => _result;
        }

        public Mat OutMat
        {
            internal set => SetProperty(ref _outMat, value);
            get => _outMat;
        }

        public bool Confidence
        {
            internal set => SetProperty(ref _confidence, value);
            get => _confidence;
        }

        public string ErrorMessage
        {
            internal set => SetProperty(ref _errorMessage, value);
            get => _errorMessage;
        }

        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine(new string('-', 30));
            strBuild.AppendLine($"RichInfo:\t{Result?.GetType()}");
            strBuild.AppendLine($"\tConfidence:\t{Confidence}");
            strBuild.AppendLine(ErrorMessage == string.Empty
                ? $"\tResult:\t{Result}"
                : $"\tError:\t{ErrorMessage}");

            strBuild.AppendLine(new string('-', 30));
            return strBuild.ToString();
        }

        /// <summary>
        ///     OutMat 由 RichInfo 持有，用完调用 Dispose 释放
        /// </summary>
        public void Dispose()
        {
            OutMat?.Dispose();
            OutMat = null!;
            GC.SuppressFinalize(this);
        }
    }
}