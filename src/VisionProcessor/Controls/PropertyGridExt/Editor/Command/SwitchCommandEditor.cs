using System.Windows;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;

namespace VisionProcessor.Controls.PropertyGridExt.Editor.Command
{
    public class SwitchCommandEditor : ButtonBase
    {
        public static readonly DependencyProperty RelayCommandProperty = DependencyProperty.Register(
            "RelayCommand", typeof(RelayCommand<bool>), typeof(SwitchCommandEditor),
            new FrameworkPropertyMetadata(default(RelayCommand<bool>), OnRelayCommandChanged));

        static SwitchCommandEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchCommandEditor),
                new FrameworkPropertyMetadata(typeof(SwitchCommandEditor)));
        }

        public RelayCommand<bool> RelayCommand
        {
            get => (RelayCommand<bool>) GetValue(RelayCommandProperty);
            set => SetValue(RelayCommandProperty, value);
        }

        private static void OnRelayCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class SwitchCommandPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            return new SwitchCommandEditor();
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return SwitchCommandEditor.RelayCommandProperty;
        }
    }
}