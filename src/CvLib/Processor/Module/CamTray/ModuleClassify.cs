using OpenCvSharp;

namespace CVLib.Processor.Module
{
    /// <summary>
    ///     产品分类器，为后续产品分类做准备
    /// </summary>
    public class ModuleClassify : Processor<Mat, ModuleType>
    {
        public ModuleClassify(string name = "ModuleClassify")
            : base(name)
        {
        }

        internal override ModuleType Process(Mat input)
        {
            return ModuleType.Default;
        }
    }
}