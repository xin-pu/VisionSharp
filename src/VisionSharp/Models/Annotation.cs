using System.Text;
using System.Xml;
using OpenCvSharp;

namespace VisionSharp.Models
{
    /// <summary>
    ///     This is  Annotation for PascalVOA xml format
    /// </summary>
    public class Annotation : IDisposable
    {
        public Annotation(string xmlPath)
        {
            ObjectInfos = new List<AnnObject>();
            Init(xmlPath);
        }

        public Annotation()
        {
        }


        public string Folder { set; get; }

        public string FileName { set; get; }

        public string FullPath => Path.Combine(Folder, FileName);

        public Size Size { set; get; }

        public List<AnnObject> ObjectInfos { set; get; }

        public void Dispose()
        {
            ObjectInfos.Clear();
        }


        private void Init(string xmlPath)
        {
            var doc = new XmlDocument();
            doc.Load(xmlPath);


            var root = doc.DocumentElement;
            if (root == null) return;

            foreach (var rootChildNode in root.ChildNodes)
            {
                if (!(rootChildNode is XmlNode d)) return;

                switch (d.Name)
                {
                    case @"folder":
                        Folder = d.InnerText;
                        break;
                    case @"filename":
                        FileName = d.InnerText;
                        break;
                    case @"size":
                    {
                        var width = int.Parse(d.SelectSingleNode("width")?.InnerText!);
                        var height = int.Parse(d.SelectSingleNode("height")?.InnerText!);
                        Size = new Size(width, height);
                        break;
                    }
                    case @"object":
                    {
                        var name = d.SelectSingleNode("name")?.InnerText;
                        var xmin = d.SelectSingleNode(@"bndbox/xmin")?.InnerText;
                        var ymin = d.SelectSingleNode(@"bndbox/ymin")?.InnerText;
                        var xmax = d.SelectSingleNode(@"bndbox/xmax")?.InnerText;
                        var ymax = d.SelectSingleNode(@"bndbox/ymax")?.InnerText;

                        var ptopleft = new Point(int.Parse(xmin!), int.Parse(ymin!));
                        var pbottomright = new Point(int.Parse(xmax!), int.Parse(ymax!));
                        var obj = new AnnObject(name, ptopleft, pbottomright);
                        ObjectInfos.Add(obj);
                        break;
                    }
                }
            }
        }

        public static Annotation CreateAnnotation(string xmlPath)
        {
            return new Annotation(xmlPath);
        }

        public override string ToString()
        {
            var strBuild = new StringBuilder();
            strBuild.AppendLine("Annotation");
            strBuild.AppendLine($"\tSize:\t({Size.Width},{Size.Height})");
            ObjectInfos.ForEach(obj => { strBuild.AppendLine(obj.ToString()); });
            return strBuild.ToString();
        }

        public void SaveXml(string path)
        {
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true
            };


            using var writer = XmlWriter.Create(path, xmlWriterSettings);
            writer.WriteStartElement("annotation");
            writer.WriteElementString("folder", Folder);
            writer.WriteElementString("filename", FileName);

            writer.WriteStartElement("size");
            writer.WriteElementString("width", Size.Width.ToString());
            writer.WriteElementString("height", Size.Height.ToString());
            writer.WriteElementString("depth", "3");
            writer.WriteEndElement();

            ObjectInfos.ForEach(obj =>
            {
                writer.WriteStartElement("object");
                writer.WriteElementString("name", obj.Name);
                writer.WriteElementString("pose", "");
                writer.WriteElementString("truncated", "0");
                writer.WriteElementString("difficult", "0");

                writer.WriteStartElement("bndbox");
                writer.WriteElementString("xmin", obj.Rect.Left.ToString());
                writer.WriteElementString("ymin", obj.Rect.Top.ToString());
                writer.WriteElementString("xmax", obj.Rect.Right.ToString());
                writer.WriteElementString("ymax", obj.Rect.Bottom.ToString());
                writer.WriteEndElement();


                writer.WriteEndElement();
            });

            writer.WriteEndElement();
        }
    }
}