using System;
using CVLib.Processor.Custom;
using CVLib.Processor.Unit;
using GalaSoft.MvvmLight;

namespace VisionProcessor.Models
{
    public class DistributionDetectorManager : ViewModelBase
    {
        private static readonly Lazy<DistributionDetectorManager> lazy = new(() => new DistributionDetectorManager());

        private DistributionDetector _distributionDetector = new CocTrayDistributionPredictor();


        public static DistributionDetectorManager Instance => lazy.Value;


        public DistributionDetector DistributionDetector
        {
            get => _distributionDetector;
            set => Set(ref _distributionDetector, value);
        }
    }
}