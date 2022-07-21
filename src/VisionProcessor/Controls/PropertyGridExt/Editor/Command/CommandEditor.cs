using System.Windows;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.CommandWpf;
using HandyControl.Controls;

namespace VisionProcessor.Controls.PropertyGridExt.Editor.Command
{
    public class RelayCommandEditor : ButtonBase
    {
        public static readonly DependencyProperty RelayCommandProperty = DependencyProperty.Register(
            "RelayCommand", typeof(RelayCommand), typeof(RelayCommandEditor),
            new FrameworkPropertyMetadata(default(RelayCommand), OnRelayCommandChanged));

        static RelayCommandEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RelayCommandEditor),
                new FrameworkPropertyMetadata(typeof(RelayCommandEditor)));
        }

        public RelayCommand RelayCommand
        {
            get => (RelayCommand) GetValue(RelayCommandProperty);
            set => SetValue(RelayCommandProperty, value);
        }

        private static void OnRelayCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class CommandPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            return new RelayCommandEditor();
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return RelayCommandEditor.RelayCommandProperty;
        }
    }
}