using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace VisionSharp.Processor
{
    /// <summary>
    ///     Process 返回的富信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RichInfo<T> : ObservableObject
    {
        private bool _confidence;
        private string _errorMessage;
        private Mat? _outMat;
        private T? _result;

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
            Result = default;
            OutMat = null;
        }

        public T? Result
        {
            internal set => SetProperty(ref _result, value);
            get => _result;
        }

        public Mat? OutMat
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
    }
}