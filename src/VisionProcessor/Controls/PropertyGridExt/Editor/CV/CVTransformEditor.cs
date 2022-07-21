using System.Windows;
using System.Windows.Controls.Primitives;
using CVLib.Processor;
using HandyControl.Controls;

namespace VisionProcessor.Controls.PropertyGridExt.Editor.CV
{
    public class CVTransformEditor : ButtonBase
    {
        public static readonly DependencyProperty CVTransformProperty = DependencyProperty.Register(
            "CVTransform", typeof(CvTransform), typeof(CVTransformEditor),
            new FrameworkPropertyMetadata(default(CVTransformEditor), OnCVTransformChanged));

        static CVTransformEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CVTransformEditor),
                new FrameworkPropertyMetadata(typeof(CVTransformEditor)));
        }

        public CvTransform CVTransform
        {
            get => (CvTransform) GetValue(CVTransformProperty);
            set => SetValue(CVTransformProperty, value);
        }


        private static void OnCVTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class CVTransformPropetyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            return new CVTransformEditor();
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return CVTransformEditor.CVTransformProperty;
        }
    }
}