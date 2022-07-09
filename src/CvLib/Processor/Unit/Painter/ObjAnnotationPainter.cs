using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace CVLib.Processor.Unit.Painter
{
    public class ObjAnnotationPainter
        : Processor<Annotation, Mat>
    {
        public Scalar[] CloesList =
        {
            Scalar.Blue,
            Scalar.Red,
            Scalar.Green,
            Scalar.Yellow,
            Scalar.Orange,
            Scalar.Aqua,
            Scalar.Magenta
        };

        public ObjAnnotationPainter()
            : base("ObjAnnotationPainter")
        {
        }

        internal override Mat Process(Annotation objDetectiion)
        {
            var objs = objDetectiion.ObjectInfos;
            var mat = Cv2.ImRead(objDetectiion.FullPath).Clone();
            var objsNames = objs.Select(o => o.Name).Distinct().ToArray();
            var colorDict = CreateColorsDict(objsNames);

            objs.ToList().ForEach(o =>
            {
                var name = o.Name;
                DrawRect(mat, o.Rect, colorDict[name], 5);
            });
            return mat;
        }

        public Dictionary<string, Scalar> CreateColorsDict(string[] objTypes)
        {
            var orderedEnumerable = objTypes
                .OrderBy(a => a)
                .ToList();
            var colorLength = CloesList.Length;
            var resDict = orderedEnumerable
                .Select((a, b) => (a, b % colorLength))
                .ToDictionary(p => p.a, p => CloesList[p.Item2]);
            return resDict;
        }
    }
}