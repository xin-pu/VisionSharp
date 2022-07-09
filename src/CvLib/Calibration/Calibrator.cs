using System;
using System.Linq;
using System.Text;
using CVLib.Utils;
using OpenCvSharp;

namespace CVLib.Calibration
{
    /// <summary>
    ///     Abstract Calibrator
    /// </summary>
    public abstract class Calibrator
    {
        /// <summary>
        ///     You must define ths Size for your Model
        /// </summary>
        /// <param name="size"></param>
        /// <param name="name"></param>
        protected Calibrator(Size size, string name = "")
        {
            Name = name;
            Size = size;
        }


        public Mat Model { set; get; }

        public Evaluate Eva { set; get; }

        public string Name { get; }

        public Size Size { protected set; get; }

        public void Feed(Point3d[] worldCoords, Point2d[] imageCoords)
        {
            Model = Calibrate(worldCoords, imageCoords);
            var predXYZ = PredictWorldCoord(imageCoords.ToArray());
            Eva = Evaluate(worldCoords, predXYZ);
        }

        public void Feed(Mat model)
        {
            if (model.Size() != Size)
                throw new ArgumentException($"Size of model not match width:{Size.Width},height:{Size.Height}");

            Model = model;
        }

        public Point3d[] Predict(Point2d[] cameraPoint2ds)
        {
            if (Model == null) throw new ArgumentException("Please feed Model first");

            return PredictWorldCoord(cameraPoint2ds);
        }

        public Point3d Predict(Point2d cameraPoint2d)
        {
            if (Model == null) throw new ArgumentException("Please feed Model first");

            return PredictWorldCoord(new[] {cameraPoint2d})[0];
        }

        public Point2d[] Predict(Point3d[] wordPoint3ds)
        {
            if (Model == null) throw new ArgumentException("Please feed Model first");

            return PredictImageCoord(wordPoint3ds);
        }

        public Point2d Predict(Point3d wordPoint3d)
        {
            if (Model == null) throw new ArgumentException("Please feed Model first");

            return PredictImageCoord(new[] {wordPoint3d})[0];
        }


        internal abstract Point3d[] PredictWorldCoord(Point2d[] cameraPoint2ds);

        internal abstract Point2d[] PredictImageCoord(Point3d[] worldPoint3ds);

        internal abstract Mat Calibrate(Point3d[] wordPoint3ds, Point2d[] cameraPoint2ds);

        public virtual Evaluate Evaluate(Point3d[] trueWorld, Point3d[] predWorld)
        {
            if (trueWorld.Length != predWorld.Length) return new Evaluate {AverageError = double.MaxValue};

            var allDiss = trueWorld.Zip(predWorld, CvMath.GetDistance);
            return new Evaluate
            {
                AverageError = allDiss.Average()
            };
        }

        public override string ToString()
        {
            var msg = new StringBuilder($"{Name}\r");
            msg.AppendLine(new string('-', 30));
            msg.AppendLine($"Matrix Size:\t{Size.Height}*{Size.Width}");
            for (var r = 0; r < Size.Height; r++)
            {
                var row = Model.Row(r);
                row.GetArray(out double[] rowArray);
                var rowArrayStr = rowArray.Select(a => $"{a:F4}");
                msg.AppendLine(string.Join("\t", rowArrayStr));
            }

            msg.AppendLine(Eva.ToString());
            msg.AppendLine(new string('-', 30));
            return msg.ToString();
        }
    }
}