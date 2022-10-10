using System;
using CVLib.Processor.Module;
using GalaSoft.MvvmLight;
using OpenCvSharp;

namespace VisionProcessor.Models
{
    public class DistributionDetectorManager : ViewModelBase
    {
        private static readonly Lazy<DistributionDetectorManager> lazy = new(() => new DistributionDetectorManager());

        private TrayDistributionPredictor _distributionDetector = new("load.onnx", new Size(2, 8));


        public static DistributionDetectorManager Instance => lazy.Value;


        public TrayDistributionPredictor DistributionDetector
        {
            get => _distributionDetector;
            set => Set(ref _distributionDetector, value);
        }
    }
}