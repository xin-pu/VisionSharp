using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CVLib.Utils;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using OpenCvSharp;

namespace CVLib.Processor
{
    public abstract class MatProcessor<T> : Processor<Mat, T>
    {
        private BitmapImage _bitmapImage;
        private BitmapSource _bitmapSource;

        protected MatProcessor(string name)
            : base(name)
        {
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
            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        #region Commmands

        public RelayCommand CallProcessorMatCommand => new(CallProcessorMatCommand_Execute);
        public RelayCommand CallProcessorVideoCommand => new(CallProcessorVideoCommand_Execute);

        public RelayCommand SelectConfigCommand => new(SelectConfigCommand_Execute);

        internal virtual void SelectConfigCommand_Execute()
        {
        }

        internal virtual void CallProcessorMatCommand_Execute()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "(*.jpg)|*.jpg|(*.png)|*.png",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() != true) return;


            var file = dialog.FileName;
            var mat = Cv2.ImRead(file);
            var res = Call(mat, mat);
            RefreshView(res.OutMat);
        }

        private async void CallProcessorVideoCommand_Execute()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "(*.mp4)|*.mp4|(*.avi)|*.avi",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() != true) return;

            using var capture = new VideoCapture();
            var isopen = capture.Open(dialog.FileName);
            if (!isopen) return;

            var mat = new Mat();
            var outmat = new Mat();

            while (true)
            {
                capture.Read(mat);
                if (mat.Empty())
                    break;

                Cv2.CvtColor(mat, outmat, ColorConversionCodes.BGRA2BGR);
                var res = Call(outmat, outmat);
                RefreshView(res.OutMat);
                await Task.Delay(TimeSpan.FromMilliseconds(0.5));
            }
        }

        #endregion
    }
}