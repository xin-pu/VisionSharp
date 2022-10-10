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
        public RichInfo(T result, bool conf, Mat outmat)
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
            Confidence = false;
            OutMat = null;
        }

        public T? Result { set; get; }

        public Mat? OutMat { set; get; }

        public bool Confidence { set; get; }

        public string ErrorMessage { set; get; }

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