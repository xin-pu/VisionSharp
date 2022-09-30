using System;
using CVLib.Processor.Unit.ObjDetectors;
using GalaSoft.MvvmLight;

namespace VisionProcessor.Models
{
    public class ObjDetectorManager : ViewModelBase
    {
        private static readonly Lazy<ObjDetectorManager> lazy = new(() => new ObjDetectorManager());

        private ObjDetector _objDetector = new YoloV3Detector();


        public static ObjDetectorManager Instance => lazy.Value;


        public ObjDetector ObjDetector
        {
            get => _objDetector;
            set => Set(ref _objDetector, value);
        }
    }
}