using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;

namespace VisionProcessor.Models
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            /// 否则 design time 模式下，会重复注册Instance.
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register(() => new ImageProcessor());
        }

        public static ViewModelLocator Instance => new Lazy<ViewModelLocator>(() =>
            Application.Current.TryFindResource("Locator") as ViewModelLocator).Value;


        public ImageProcessor ImageProcessor => SimpleIoc.Default.GetInstance<ImageProcessor>();
    }
}