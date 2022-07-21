using System.Windows;
using System.Windows.Controls.Primitives;
using CVLib.Processor;
using HandyControl.Controls;

namespace VisionProcessor.Controls.PropertyGridExt.Editor.CV
{
    public class CVSizeEditor : ButtonBase
    {
        public static readonly DependencyProperty CVSizeProperty = DependencyProperty.Register(
            "CVSize", typeof(CvSize), typeof(CVSizeEditor),
            new FrameworkPropertyMetadata(default(CVSizeEditor), OnCVSizeChanged));

        static CVSizeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CVSizeEditor),
                new FrameworkPropertyMetadata(typeof(CVSizeEditor)));
        }

        public CvSize CVSize
        {
            get => (CvSize) GetValue(CVSizeProperty);
            set => SetValue(CVSizeProperty, value);
        }


        private static void OnCVSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class CVSizePropetyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            return new CVSizeEditor();
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return CVSizeEditor.CVSizeProperty;
        }
    }
}