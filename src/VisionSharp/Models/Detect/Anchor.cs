using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Detect
{
    public class Anchor : ObservableObject
    {
        private float _width;
        private float _height;


        public Anchor()
        {
        }

        public Anchor(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width
        {
            internal set => SetProperty(ref _width, value);
            get => _width;
        }

        public float Height
        {
            internal set => SetProperty(ref _height, value);
            get => _height;
        }
    }
}