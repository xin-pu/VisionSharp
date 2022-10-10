using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVLib.Models;
using CVLib.Processor.Unit.Painter;
using CVLib.Utils;
using OpenCvSharp;
using Xunit;
using Xunit.Abstractions;

namespace CvExperiment
{
    public class ModuleObjDetectExpe
        : AbstractTest
    {
        public List<Size> Size = new List<Size>
        {
            new Size(137.8866, 226.34021),
            new Size(229.0, 234.05463),
            new Size(561.5787, 570.9663),
            new Size(418.17334, 249.576),
            new Size(394.39008, 399.95035),
            new Size(246.06462, 416.47693),
            new Size(358.14908, 602.3091),
            new Size(224.6105, 139.56018),
            new Size(603.7755, 366.2857)
        };

        public ModuleObjDetectExpe(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            Creator = new ObjAnnotationCreator(ObjFolder, BackGroubFolder,
                @"F:\Module Object Detection\Train");
            Painter = new ObjAnnotationPainter();
        }

        private string BackGroubFolder => @"F:\Module Object Detection\BackGround";
        private string ObjFolder => @"F:\Module Object Detection\ModuleTemplate";


        public ObjAnnotationCreator Creator { get; }
        public ObjAnnotationPainter Painter { get; }


        [Fact]
        public void GenegrateObjectImage()
        {
            Creator.MatRation = 4;
            Creator.Call(1000);
        }

        [Fact]
        public void TestWriteAnn()
        {
            var a = new Annotation("test.xml");
            PrintObject(a);
            a.SaveXml("test.xml");
        }

        [Fact]
        public void TestIou()
        {
            var rect1 = new Rect(50, 50, 100, 100);
            var rect2 = new Rect(100, 100, 100, 100);
            var iou = CvIOU.getIoU(rect1, rect2);
            PrintObject(iou);
        }

        [Fact]
        public void SaveAnchor()
        {
            var files = new DirectoryInfo(@"F:\Module Object Detection\Train\annotations").GetFiles("*.xml");
            using var txtwrite = new StreamWriter("anchor.txt");
            foreach (var fileInfo in files)
            {
                var a = new Annotation(fileInfo.FullName);
                a.ObjectInfos.ForEach(ann => { txtwrite.WriteLine($"{ann.Rect.Width},{ann.Rect.Height}"); });
            }
        }

        [Fact]
        public void SortSize()
        {
            var res = Size.Select(a => (a, a.Height * a.Width)).OrderBy(a => a.a.Width);
            foreach (var valueTuple in res) PrintObject($"{valueTuple.a.Width:D},{valueTuple.a.Height:D}");
        }
    }
}