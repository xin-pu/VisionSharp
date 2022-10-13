using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace VisionSharp.Utils
{
    /// <summary>
    ///     Should use PointXd
    /// </summary>
    public class CvCvt
    {
        #region Convert Mat to float[,]

        public static float[,] CvtToArray(Mat mat)
        {
            if (mat.Type() != MatType.CV_32F)
            {
                throw new ArgumentException();
            }

            var final = new Mat<float>();
            mat.ConvertTo(final, MatType.CV_32F);
            return final.ToRectangularArray();
        }

        #endregion

        #region Cut Point

        public static Point2d CutZToPoint2d(Point3d point3ds)
        {
            return new Point2d(point3ds.X, point3ds.Y);
        }

        #endregion


        #region Convert bool[,]

        public static string CvtToStr(bool[,] mat)
        {
            var row = mat.GetLength(0);
            var column = mat.GetLength(1);
            var strBuild = new StringBuilder();
            Enumerable.Range(0, row)
                .ToList()
                .ForEach(r =>
                {
                    var line = Enumerable.Range(0, column)
                        .ToList()
                        .Select(c => mat[r, c] ? 1 : 0);
                    strBuild.AppendLine(string.Join(",", line));
                });
            return strBuild.ToString();
        }


        public static Mat CvtToMat(bool[,] input)
        {
            var row = input.GetLength(0);
            var column = input.GetLength(1);
            var array = new double[row, column];
            Enumerable.Range(0, row)
                .ToList()
                .ForEach(r => Enumerable.Range(0, column)
                    .ToList()
                    .ForEach(c => array[r, c] = input[r, c] ? 1 : 0));
            return Mat.FromArray(array);
        }

        #endregion

        #region Conver From PointXd to PointXf

        public static Point2f CvtToPoint2F(Point point)
        {
            return new Point2f(point.X, point.Y);
        }

        public static Point2f CvtToPoint2F(Point2d point2F)
        {
            return new Point2f((float) point2F.X, (float) point2F.Y);
        }

        public static Point3f CvtToPoint3F(Point3d point3F)
        {
            return new Point3f((float) point3F.X, (float) point3F.Y, (float) point3F.Z);
        }


        /// <summary>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point2f[] CvtToPoint2Fs(IEnumerable<Point> points)
        {
            return points.Select(CvtToPoint2F).ToArray();
        }


        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=3, X,Y,Z</param>
        /// <returns></returns>
        public static Point3f[] CvtToPoint3Fs(IEnumerable<Point3d> positions)
        {
            return positions.Select(CvtToPoint3F).ToArray();
        }

        #endregion

        #region Conver From PointXf to PointXd

        public static Point2d CvtToPoint2d(Point point)
        {
            return new Point2d(point.X, point.Y);
        }

        public static Point2d CvtToPoint2d(Point2f point2F)
        {
            return new Point2d(point2F.X, point2F.Y);
        }

        public static Point3d CvtToPoint3d(Point3f point3F)
        {
            return new Point3d(point3F.X, point3F.Y, point3F.Z);
        }


        /// <summary>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point2d[] CvtToPoint2ds(IEnumerable<Point> points)
        {
            return points.Select(CvtToPoint2d).ToArray();
        }


        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=3, X,Y,Z</param>
        /// <returns></returns>
        public static Point3d[] CvtToPoint3ds(IEnumerable<Point3f> positions)
        {
            return positions.Select(CvtToPoint3d).ToArray();
        }

        #endregion

        #region Convert From Mat to PointXd

        public static Point2d[] CvtToPoint2ds(Mat word)
        {
            var final = new Mat<double>();
            word.ConvertTo(final, MatType.CV_64F);
            return CvtToPoint2ds(final);
        }

        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=2, u,v,</param>
        /// <returns></returns>
        public static Point2d[] CvtToPoint2ds(Mat<double> word)
        {
            var size = word.Size();
            if (size.Width != 2)
            {
                throw new ArgumentException("Width is not 2 for Parse");
            }

            var array = word.ToRectangularArray();

            return Enumerable.Range(0, size.Height).ToList()
                .Select(row => new Point2d(array[row, 0], array[row, 1]))
                .ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="word">Mat with width=3, X,Y,Z</param>
        /// <returns></returns>
        public static Point3d[] CvtToPoint3ds(Mat<double> world)
        {
            var size = world.Size();
            if (size.Width != 3)
            {
                throw new ArgumentException("Width is not 3 for Parse");
            }

            var array = world.ToRectangularArray();

            return Enumerable.Range(0, size.Height).ToList()
                .Select(row => new Point3d(array[row, 0], array[row, 1], array[row, 2]))
                .ToArray();
        }

        #endregion

        #region Convert From Point to Mat

        public static Mat CvtToMat(double[,] points)
        {
            return Mat.FromArray(points);
        }

        public static Mat CvtToMat(Point3d[] point3ds)
        {
            var len = point3ds.Length;
            var size = new Size(3, len);
            var mat = new Mat(size, MatType.CV_64F);
            foreach (var i in Enumerable.Range(0, len))
            {
                var p = point3ds[i];
                mat.Set(i, 0, p.X);
                mat.Set(i, 1, p.Y);
                mat.Set(i, 2, p.Z);
            }

            return mat;
        }

        public static Mat CvtToMat(Point2d[] point2ds)
        {
            var len = point2ds.Length;
            var size = new Size(2, len);
            var mat = new Mat(size, MatType.CV_64F);
            foreach (var i in Enumerable.Range(0, len))
            {
                var p = point2ds[i];
                mat.Set(i, 0, p.X);
                mat.Set(i, 1, p.Y);
            }

            return mat;
        }


        public static Mat CvtToMat(Point[] point2Fs)
        {
            var point2ds = point2Fs.Select(CvtToPoint2d).ToArray();
            return CvtToMat(point2ds);
        }

        #endregion

        #region Covert for Rect / RotatedRect

        public static RotatedRect CvtToRotatedRect(Rect rect)
        {
            var topRight = new Point(rect.Right, rect.Top);
            var bottomLeft = new Point(rect.Left, rect.Bottom);
            var points = new[] {rect.TopLeft, rect.BottomRight, topRight, bottomLeft};
            return Cv2.MinAreaRect(points);
        }


        public static Rect CvtToRect(RotatedRect rotatedRect)
        {
            return rotatedRect.BoundingRect();
        }

        #endregion


        #region Convert Mat to Bitmap

        /// <summary>
        ///     Converts Mat to System.Drawing.Bitmap
        /// </summary>
        /// <param name="src">Mat</param>
        /// <returns></returns>
        //[SupportedOSPlatform("windows")]
        public static Bitmap CvtToBitmap(Mat src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            PixelFormat pf;
            switch (src.Channels())
            {
                case 1:
                    pf = PixelFormat.Format8bppIndexed;
                    break;
                case 3:
                    pf = PixelFormat.Format24bppRgb;
                    break;
                case 4:
                    pf = PixelFormat.Format32bppArgb;
                    break;
                default:
                    throw new ArgumentException("Number of channels must be 1, 3 or 4.", nameof(src));
            }

            return ToBitmap(src, pf);
        }


        /// <summary>
        ///     Converts Mat to System.Drawing.Bitmap
        /// </summary>
        /// <param name="src">Mat</param>
        /// <param name="pf">Pixel Depth</param>
        /// <returns></returns>
        //[SupportedOSPlatform("windows")]
        protected static Bitmap ToBitmap(Mat src, PixelFormat pf)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            src.ThrowIfDisposed();

            var bitmap = new Bitmap(src.Width, src.Height, pf);
            ToBitmap(src, bitmap);
            return bitmap;
        }

        /// <summary>
        ///     Converts Mat to System.Drawing.Bitmap
        /// </summary>
        /// <param name="src">Mat</param>
        /// <param name="dst">Mat</param>
        /// <example></example>
        //[SupportedOSPlatform("windows")]
        protected static unsafe void ToBitmap(Mat src, Bitmap dst)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (dst == null)
            {
                throw new ArgumentNullException(nameof(dst));
            }

            if (src.IsDisposed)
            {
                throw new ArgumentException("The image is disposed.", nameof(src));
            }

            if (src.Depth() != MatType.CV_8U)
            {
                throw new ArgumentException("Depth of the image must be CV_8U");
            }

            if (src.Width != dst.Width || src.Height != dst.Height)
            {
                throw new ArgumentException("");
            }

            var pf = dst.PixelFormat;


            if (pf == PixelFormat.Format8bppIndexed)
            {
                var plt = dst.Palette;
                for (var x = 0; x < 256; x++)
                {
                    plt.Entries[x] = Color.FromArgb(x, x, x);
                }

                dst.Palette = plt;
            }

            var w = src.Width;
            var h = src.Height;
            var rect = new Rectangle(0, 0, w, h);
            BitmapData bd = null;

            var submat = src.IsSubmatrix();
            var continuous = src.IsContinuous();

            try
            {
                bd = dst.LockBits(rect, ImageLockMode.WriteOnly, pf);

                var srcData = src.Data;
                var pSrc = (byte*) srcData.ToPointer();
                var pDst = (byte*) bd.Scan0.ToPointer();
                var ch = src.Channels();
                var srcStep = (int) src.Step();
                var dstStep = (src.Width * ch + 3) / 4 * 4; // 4の倍数に揃える
                var stride = bd.Stride;

                switch (pf)
                {
                    case PixelFormat.Format1bppIndexed:
                    {
                        if (submat)
                        {
                            throw new NotImplementedException("submatrix not supported");
                        }

                        // BitmapDataは4byte幅だが、IplImageは1byte幅
                        // 手作業で移し替える                 
                        //int offset = stride - (w / 8);
                        var x = 0;
                        byte b = 0;
                        for (var y = 0; y < h; y++)
                        {
                            for (var bytePos = 0; bytePos < stride; bytePos++)
                            {
                                if (x < w)
                                {
                                    for (var i = 0; i < 8; i++)
                                    {
                                        var mask = (byte) (0x80 >> i);
                                        if (x < w && pSrc[srcStep * y + x] == 0)
                                        {
                                            b &= (byte) (mask ^ 0xff);
                                        }
                                        else
                                        {
                                            b |= mask;
                                        }

                                        x++;
                                    }

                                    pDst[bytePos] = b;
                                }
                            }

                            x = 0;
                            pDst += stride;
                        }

                        break;
                    }

                    case PixelFormat.Format8bppIndexed:
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppArgb:
                        if (srcStep == dstStep && !submat && continuous)
                        {
                            var bytesToCopy = src.DataEnd.ToInt64() - src.Data.ToInt64();
                            Buffer.MemoryCopy(pSrc, pDst, bytesToCopy, bytesToCopy);
                        }
                        else
                        {
                            for (var y = 0; y < h; y++)
                            {
                                long offsetSrc = y * srcStep;
                                long offsetDst = y * dstStep;
                                long bytesToCopy = w * ch;
                                // 一列ごとにコピー
                                Buffer.MemoryCopy(pSrc + offsetSrc, pDst + offsetDst, bytesToCopy, bytesToCopy);
                            }
                        }

                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            finally
            {
                if (bd != null)
                {
                    dst.UnlockBits(bd);
                }
            }
        }

        #endregion

        #region 颜色控件转换

        /// <summary>
        ///     将h,s,v转RGB颜色
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Scalar CvtHsvToRgb(double h, double s, double v)
        {
            var hI = (int) (h * 6);
            var f = h * 6 - hI;
            var p = v * (1 - s);
            var q = v * (1 - f * s);
            var t = v * (1 - (1 - f) * s);
            double r, g, b;
            switch (hI)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
                default:
                    r = 1;
                    g = 1;
                    b = 1;
                    break;
            }

            return new Scalar(r * 255, g * 255, b * 255, 0);
        }

        /// <summary>
        ///     生成分类枚举的颜色字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<T, Scalar> GetColorDict<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            var pairSelect = values
                .Select((a, b) => (a, CvtHsvToRgb(1.0 * b / values.Count, 1, 1)))
                .ToList();
            var colorDict = pairSelect.ToDictionary(p => p.a, p => p.Item2);
            return colorDict;
        }

        #endregion
    }
}