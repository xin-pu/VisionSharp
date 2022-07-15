using System;
using OpenCvSharp;

namespace CVLib.Processor
{
    /// <summary>
    ///     抽象分类器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Classifier<T> : Processor<Mat, T>
        where T : Enum
    {
        protected Classifier(string name)
            : base(name)
        {
        }
    }
}