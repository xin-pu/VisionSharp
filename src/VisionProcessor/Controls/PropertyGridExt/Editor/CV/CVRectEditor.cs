using System.Windows;
using System.Windows.Controls.Primitives;
using CVLib.Processor;
using HandyControl.Controls;

namespace VisionProcessor.Controls.PropertyGridExt.Editor.CV
{
    public class CVRectEditor : ButtonBase
    {
        public static readonly DependencyProperty CVRectProperty = DependencyProperty.Register(
            "CvRect", typeof(CvRect), typeof(CVRectEditor),
            new FrameworkPropertyMetadata(default(CVRectEditor), OnCVRectChanged));

        static CVRectEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CVRectEditor),
                new FrameworkPropertyMetadata(typeof(CVRectEditor)));
        }

        public CvRect CvRect
        {
            get => (CvRect) GetValue(CVRectProperty);
            set => SetValue(CVRectProperty, value);
        }


        private static void OnCVRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class CVRectPropetyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            return new CVRectEditor();
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return CVRectEditor.CVRectProperty;
        }
    }
}