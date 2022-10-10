using System.Text;
using OpenCvSharp;

namespace VisionSharp.Models
{
    public class AnnObject
    {
        public AnnObject(
            string name,
            Point topLeft,
            Point bottomRight)
        {
            Name = name;
            Rect = new Rect(
                topLeft.X,
                topLeft.Y,
                bottomRight.X - topLeft.X,
                bottomRight.Y - topLeft.Y);
        }

        public string Name { set; get; }

        public Rect Rect { set; get; }

        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine($"AnnObject:{Name}");
            strBuild.AppendLine($"\tTopLeft:\t({Rect.Left},{Rect.Top})");
            strBuild.AppendLine($"\tBottomRight:\t({Rect.Right},{Rect.Bottom})");
            return strBuild.ToString();
        }
    }
}