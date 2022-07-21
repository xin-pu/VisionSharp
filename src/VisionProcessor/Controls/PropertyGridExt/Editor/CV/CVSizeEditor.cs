using System.Windows;
using System.Windows.Controls.Primitives;
using CVLib.Processor;
using HandyControl.Controls;

namespace VisionProcessor.Controls.PropertyGridExt.Editor.CV
{
    public class CVPointEditor : ButtonBase
    {
        public static readonly DependencyProperty CVPointProperty = DependencyProperty.Register(
            "CVPoint", typeof(CvPoint), typeof(CVPointEditor),
            new FrameworkPropertyMetadata(default(CVPointEditor), OnCVPointChanged));

        static CVPointEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CVPointEditor),
                new FrameworkPropertyMetadata(typeof(CVPointEditor)));
        }

        public CvPoint CVPoint
        {
            get => (CvPoint) GetValue(CVPointProperty);
            set => SetValue(CVPointProperty, value);
        }


        private static void OnCVPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class CVPointPropetyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            return new CVPointEditor();
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return CVPointEditor.CVPointProperty;
        }
    }
}