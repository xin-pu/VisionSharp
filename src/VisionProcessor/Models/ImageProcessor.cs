using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private VocObejctsDetector _vocObejctsDetector = new VocObejctsDetector();

        public VocObejctsDetector VocObejctsDetector
        {
            get => _vocObejctsDetector;
            set => Set(ref _vocObejctsDetector, value);
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


        private void ExecuteProcessorCommand_Execute()
        {
            VocObejctsDetector.ModelWeights = @"E:\Code DeepLearning\darknet_release\weights\yolov3-voc.weights";
            VocObejctsDetector.ConfigFile = @"E:\Code DeepLearning\darknet_release\cfg\yolov3-voc.cfg";
            VocObejctsDetector.CONFIDENCE = 0.5F;
            VocObejctsDetector.THRESHOLD = 0.5F;

            var fileDialog = new OpenFileDialog
            {
                Filter = "(*.jpg)|*.jpg|(*.png)|*.png"
            };
            var dialogRes = fileDialog.ShowDialog();

            if (dialogRes != true)
                return;


            var mat = Cv2.ImRead(fileDialog.FileName);

            var res = VocObejctsDetector.Call(mat, mat);
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