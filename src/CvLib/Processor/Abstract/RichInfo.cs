using System.Text;
using GalaSoft.MvvmLight;
using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     Process 返回的富信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RichInfo<T> : ViewModelBase
    {
        private double _confidence;
        private string _errorMessage;
        private Mat _outMat;
        private T _result;

        public RichInfo(T result, double conf, Mat outmat)
        {
            ErrorMessage = string.Empty;
            Result = result;
            Confidence = conf;
            OutMat = outmat;
        }

        public RichInfo(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Result = default;
            Confidence = 0;
            OutMat = null;
        }

        public T Result
        {
            set => Set(ref _result, value);
            get => _result;
        }

        public Mat OutMat
        {
            set => Set(ref _outMat, value);
            get => _outMat;
        }

        public double Confidence
        {
            set => Set(ref _confidence, value);
            get => _confidence;
        }

        public string ErrorMessage
        {
            set => Set(ref _errorMessage, value);
            get => _errorMessage;
        }

        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine(new string('-', 30));
            strBuild.AppendLine($"RichInfo:\t{Result.GetType()}");
            strBuild.AppendLine($"\tConfidence:\t{Confidence:P2}");
            strBuild.AppendLine(ErrorMessage == string.Empty
                ? $"\tResult:\t{Result}"
                : $"\tError:\t{ErrorMessage}");

            strBuild.AppendLine(new string('-', 30));
            return strBuild.ToString();
        }
    }
}