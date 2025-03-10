﻿using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace VisionSharp.Models.Base
{
    public class CvTransform : ObservableObject
    {
        private List<CvMatCell> _arrayCells;

        /// <summary>
        ///     可观测的矩阵
        /// </summary>
        public CvTransform()
        {
        }

        /// <summary>
        ///     可观测的矩阵
        /// </summary>
        /// <param name="array"></param>
        public CvTransform(double[,] array)
        {
            ArrayList = ConvertFrom(array);
        }

        public List<CvMatCell> ArrayList
        {
            set => SetProperty(ref _arrayCells, value);
            get => _arrayCells;
        }

        public Mat Mat => ConvertToMat(ArrayList);


        public override string ToString()
        {
            var size = Mat.Size();
            var strBuild = new StringBuilder();
            strBuild.AppendLine("CvTransform");
            strBuild.AppendLine(new string('-', 30));
            strBuild.AppendLine($"\tMatrix Size:\t{size.Height:F4}*{size.Width:F4}");

            for (var r = 0; r < size.Height; r++)
            {
                var row = Mat.Row(r);
                row.GetArray(out double[] rowArray);
                var rowArrayStr = rowArray.Select(a => $"{a:F4}");
                strBuild.AppendLine(string.Join("\t", rowArrayStr));
            }

            strBuild.AppendLine(new string('-', 30));
            return strBuild.ToString();
        }

        #region 静态方法

        public static List<CvMatCell> ConvertFrom(double[,] array)
        {
            var res = new List<CvMatCell>();
            var row = array.GetLength(0);
            var column = array.GetLength(1);
            foreach (var r in Enumerable.Range(0, row))
            foreach (var c in Enumerable.Range(0, column))
            {
                res.Add(new CvMatCell(r, c, array[r, c]));
            }

            return res;
        }

        public static double[,] ConvertTo(List<CvMatCell> cells)
        {
            var row = cells.Max(a => a.Row) + 1;
            var column = cells.Max(a => a.Column) + 1;
            var array = new double[row, column];
            cells.ForEach(a => array[a.Row, a.Column] = a.Value);
            return array;
        }

        public static Mat ConvertToMat(List<CvMatCell> cells)
        {
            var array = ConvertTo(cells);
            return Mat.FromArray(array);
        }

        #endregion
    }
}