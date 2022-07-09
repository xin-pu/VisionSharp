using System.Text;
using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     Process 返回的富信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RichInfo<T> : CvViewModelBase
    {
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

        public T Result { set; get; }

        public Mat OutMat { set; get; }

        public double Confidence { set; get; }

        public string ErrorMessage { set; get; }

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