using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVLib.Processor.Unit;
using OpenCvSharp;

namespace CVLib.Processor.Module
{
    public class ModuleSplitter : Processor<Mat, List<Mat>>
    {
        public ModuleSplitter(
            NormalPretreatment normalPretreatment,
            ObjectsDetector objectsDetector)
            : base("ModuleSplitter")
        {
            NormalPretreatment = normalPretreatment;
            ModuleDetector = objectsDetector;
        }

        public ObjectsDetector ModuleDetector { set; get; }
        public NormalPretreatment NormalPretreatment { set; get; }

        internal override List<Mat> Process(Mat input)
        {
            var matAfterPre = NormalPretreatment.Call(input.Clone()).Result;

            var detectObj = ModuleDetector.Call(matAfterPre).Result;

            var result = new List<Mat>();
            detectObj
                .Where(a => a.IsConfidence)
                .ToList()
                .ForEach(obj =>
                {
                    var filename = Path.Combine(OutPutDire,
                        $"{DateTime.Now:dd_HH_mm_ss}_{DateTime.Now.Millisecond}.png");
                    var mat = input[obj.RotatedRect.BoundingRect()];
                    result.Add(mat);
                    mat.SaveImage(filename);
                });
            return result;
        }
    }
}