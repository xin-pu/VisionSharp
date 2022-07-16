using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CVLib.Processor.Unit;
using CVLib.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using OpenCvSharp;

namespace VisionProcessor.Models
{
    public class ImageProcessor : ViewModelBase
    {
        private BitmapImage _bitmapImage;
        private BitmapSource _bitmapSource;
        private YoloV3Detector _vocObjectsDetector = new YoloV3Detector();

        public YoloV3Detector VocObjectsDetector
        {
            get => _vocObjectsDetector;
            set => Set(ref _vocObjectsDetector, value);
        }

        public BitmapImage BitmapDataImage
        {
            get => _bitmapImage;
            set => Set(ref _bitmapImage, value);
        }

        public BitmapSource BitmapSource
        {
            get => _bitmapSource;
            set => Set(ref _bitmapSource, value);
        }


        public RelayCommand ExecuteProcessorCommand => new RelayCommand(ExecuteProcessorCommand_Execute);

        public RelayCommand ExecuteVedioCommand => new RelayCommand(ExecuteProcessorCommand_Execute2);

        private async void ExecuteProcessorCommand_Execute2()
        {
            VocObjectsDetector.ModelWeights = @"E:\Code DeepLearning\darknet_release\weights\yolov3-voc.weights";
            VocObjectsDetector.ConfigFile = @"E:\Code DeepLearning\darknet_release\cfg\yolov3-voc.cfg";
            VocObjectsDetector.CONFIDENCE = 0.5F;
            VocObjectsDetector.IOUThreshold = 0.5F;

            using (var capture =
                   new VideoCapture())
            {
                //var isopen =
                //    capture.Open(@"E:\Code Web\OpenCVSharp\test\OpenCvSharp.Tests\_data\image\blob\shapes%d.png");
                var isopen =
                    capture.Open(@"E:\OneDrive - II-VI Incorporated\Pictures\demon.MP4");
                if (!isopen)
                    return;

                var mat = new Mat();
                var outmat = new Mat();

                while (true)
                {
                    capture.Read(mat);
                    if (mat.Empty())
                        break;

                    Cv2.CvtColor(mat, outmat, ColorConversionCodes.BGRA2BGR);
                    var res = VocObjectsDetector.Call(outmat, outmat);
                    RefreshView(res.OutMat);
                    await Task.Delay(TimeSpan.FromMilliseconds(0.5));
                }
            }
        }

        private void ExecuteProcessorCommand_Execute()
        {
            VocObjectsDetector.ModelWeights = @"E:\Code DeepLearning\darknet_release\weights\yolov3-voc.weights";
            VocObjectsDetector.ConfigFile = @"E:\Code DeepLearning\darknet_release\cfg\yolov3-voc.cfg";
            VocObjectsDetector.CONFIDENCE = 0.5F;
            VocObjectsDetector.IOUThreshold = 0.5F;

            var fileDialog = new OpenFileDialog
            {
                Filter = "(*.jpg)|*.jpg|(*.png)|*.png"
            };
            var dialogRes = fileDialog.ShowDialog();

            if (dialogRes != true)
                return;


            var mat = Cv2.ImRead(fileDialog.FileName);

            var res = VocObjectsDetector.Call(mat, mat);
            RefreshView(res.OutMat);
        }


        public void RefreshView(Mat proResult)
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                var temp = CvCvt.CvtToBitmap(proResult);
                BitmapDataImage = Bitmaptobitmapimage(temp);
                BitmapSource = BitmapFrame.Create(BitmapDataImage);
            });
        }

        private BitmapImage Bitmaptobitmapimage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}