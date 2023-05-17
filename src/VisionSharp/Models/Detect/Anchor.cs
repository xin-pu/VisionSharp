using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Detect
{
    public class Anchor : ObservableObject
    {
        private float _wdith;
        private float _height;


        public Anchor()
        {
        }

        public Anchor(float wdith, float height)
        {
            Width = wdith;
            Height = height;
        }

        public float Width
        {
            internal set => SetProperty(ref _wdith, value);
            get => _wdith;
        }

        public float Height
        {
            internal set => SetProperty(ref _height, value);
            get => _height;
        }
    }
}