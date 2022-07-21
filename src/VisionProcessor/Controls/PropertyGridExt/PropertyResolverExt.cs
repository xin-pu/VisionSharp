using System;
using System.Collections.Generic;
using System.Windows.Input;
using CVLib.Processor;
using GalaSoft.MvvmLight.CommandWpf;
using HandyControl.Controls;
using VisionProcessor.Controls.PropertyGridExt.Editor.Command;
using VisionProcessor.Controls.PropertyGridExt.Editor.CV;

namespace VisionProcessor.Controls.PropertyGridExt
{
    public class PropertyResolverExt : PropertyResolver
    {
        private static readonly Dictionary<Type, EditorType> TypeCodeDic =
            new()
            {
                [typeof(RelayCommand)] = EditorType.Command,
                [typeof(RelayCommand<bool>)] = EditorType.SwitchCommand,
                [typeof(ICommand)] = EditorType.Command,


                [typeof(CvPoint)] = EditorType.CVPoint,
                [typeof(CvSize)] = EditorType.CVSize,
                [typeof(CvTransform)] = EditorType.CVTransform,
                [typeof(CvRect)] = EditorType.CVRect
            };


        public override PropertyEditorBase CreateDefaultEditor(Type type)
        {
            var getType = TypeCodeDic.TryGetValue(type, out var editorType);
            var editor = getType
                ? CreateCVEditor(editorType)
                : base.CreateDefaultEditor(type);
            return editor;
        }


        public PropertyEditorBase CreateCVEditor(EditorType type)
        {
            switch (type)
            {
                case EditorType.Command:
                    return new CommandPropertyEditor();
                case EditorType.SwitchCommand:
                    return new SwitchCommandPropertyEditor();

                case EditorType.CVRect:
                    return new CVRectPropetyEditor();
                case EditorType.CVPoint:
                    return new CVPointPropetyEditor();
                case EditorType.CVSize:
                    return new CVSizePropetyEditor();
                case EditorType.CVTransform:
                    return new CVTransformPropetyEditor();

                default:
                    return new PlainTextPropertyEditor();
            }
        }
    }

    public class PropertyGridExt : PropertyGrid
    {
        public override PropertyResolver PropertyResolver => new PropertyResolverExt();
    }
}