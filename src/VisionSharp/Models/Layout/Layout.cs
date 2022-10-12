using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using VisionSharp.Models.Category;

namespace VisionSharp.Models.Layout
{
    public class Layout : ObservableObject, IEquatable<Layout>
    {
        private int _column;
        private List<LayoutCell> _layoutCells;
        private int _row;
        private double _scoreThreshold;


        /// <summary>
        ///     平面布局 用于平面布局预测
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="scoreThreshold"></param>
        public Layout(int row, int column, double scoreThreshold = 0.7)
        {
            Row = row;
            Column = column;
            ScoreThreshold = scoreThreshold;
            LayoutCells = new List<LayoutCell>();
            foreach (var r in Enumerable.Range(0, row))
            foreach (var c in Enumerable.Range(0, column))
            {
                LayoutCells.Add(new LayoutCell(r, c));
            }
        }

        /// <summary>
        ///     行
        /// </summary>
        public int Row
        {
            protected set => SetProperty(ref _row, value);
            get => _row;
        }

        /// <summary>
        ///     宽
        /// </summary>
        public int Column
        {
            protected set => SetProperty(ref _column, value);
            get => _column;
        }

        /// <summary>
        ///     用于评判的阈值
        /// </summary>
        public double ScoreThreshold
        {
            protected set => SetProperty(ref _scoreThreshold, value);
            get => _scoreThreshold;
        }

        /// <summary>
        ///     布局中所有单元,不公开给外部
        /// </summary>
        internal List<LayoutCell> LayoutCells
        {
            set => SetProperty(ref _layoutCells, value);
            get => _layoutCells;
        }

        /// <summary>
        ///     获取布局中单个单元
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public LayoutCell this[int row, int column] =>
            LayoutCells.FirstOrDefault(a => a.Row == row && a.Column == column);

        /// <summary>
        ///     获取布局中一行单元
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public LayoutCell[] this[int row] =>
            LayoutCells.Where(a => a.Row == row).OrderBy(a => a.Column).ToArray();

        /// <summary>
        ///     判断整个预测结果是否可靠
        ///     只要有一个位置分数存疑，则失败
        /// </summary>
        /// <returns></returns>
        public bool GetReliability()
        {
            return LayoutCells.All(d => d.LayoutStatus != LayoutStatus.Unidentified);
        }

        /// <summary>
        ///     更新分数，必须紧跟更新状态才这个布局状态更新生效
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="score">sigmoid score</param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateScore(int row, int column, double positiveScore, double negativeScore)
        {
            this[row, column].PositiveScore = positiveScore;
            this[row, column].NegativeScore = negativeScore;
        }

        /// <summary>
        ///     更新单元状态
        /// </summary>
        public void UpdateStatus()
        {
            LayoutCells.ForEach(cell =>
            {
                var positiveScore = cell.PositiveScore;
                var negativeScore = cell.NegativeScore;
                cell.LayoutStatus = positiveScore < ScoreThreshold && negativeScore < ScoreThreshold
                    ? LayoutStatus.Unidentified
                    : positiveScore > negativeScore
                        ? LayoutStatus.Positive
                        : LayoutStatus.Negative;
            });
        }


        public LayoutStatus[,] ToLayoutStatus()
        {
            var res = new LayoutStatus[Row, Column];
            Enumerable.Range(0, Row).ToList()
                .ForEach(r => Enumerable.Range(0, Column).ToList()
                    .ForEach(c => res[r, c] = this[r, c].LayoutStatus));
            return res;
        }

        public void ChangeThreshold(double scoreThreshold)
        {
            ScoreThreshold = scoreThreshold;
        }


        /// <summary>
        ///     保存本布局
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            SaveAnnotation(filePath, this);
        }

        /// <summary>
        ///     返回布局转的文本信息
        ///     +,-,?标记
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        public override string ToString()
        {
            var str = new StringBuilder();
            var header = string.Join("\t", Enumerable.Range(0, Column).Select(c => $"{c:D2}"));
            str.AppendLine($"  \t{header}");
            foreach (var row in Enumerable.Range(0, Row))
            {
                var status = string.Join("\t", this[row].Select(c => c.AsStrStatus()));
                var score = string.Join("\t", this[row].Select(c => c.AsScoreStatus()));
                var line = $"{row:D2}|\t{status}\t{score}";
                str.AppendLine(line);
            }

            return str.ToString();
        }

        /// <summary>
        ///     返回用于训练的注释文件字符串
        ///     0,1,?标记
        /// </summary>
        /// <returns></returns>
        public string ToAnnotationString()
        {
            var str = new StringBuilder();
            foreach (var row in Enumerable.Range(0, Row))
            {
                var line = string.Join(",", this[row].Select(c => c.AsAnnStatus()));
                str.AppendLine(line);
            }

            return str.ToString();
        }


        #region 比较器

        public bool Equals(Layout other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(this, null))
            {
                return false;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            if (Row == other.Row && Column == other.Column)
            {
                var res = LayoutCells.All(d =>
                    d.LayoutStatus == other[d.Row, d.Column].LayoutStatus);
                return res;
            }

            return false;
        }

        public int GetHashCode(Layout obj)
        {
            return HashCode.Combine(obj._column, obj._layoutCells, obj._row, obj._scoreThreshold);
        }

        #endregion

        #region 静态方法

        /// <summary>
        ///     从注释文件获取布局
        /// </summary>
        /// <param name="annfile">注释文件路径</param>
        /// <returns>平面</returns>
        /// <exception cref="FileLoadException"></exception>
        public static Layout LoadFromAnnotation(string annfile)
        {
            using var sr = new StreamReader(annfile);
            var lines = sr.ReadToEnd().Split('\r', '\n').Where(a => a != "").ToList();
            var rowlines = lines.Select(r => r.Split(',').Where(a => a != "").ToList()).ToList();
            var rows = rowlines.Count;
            var columnsRows = rowlines.Select(a => a.Count).Distinct().ToList();
            if (columnsRows.Count != 1)
            {
                throw new FileLoadException();
            }

            var column = columnsRows[0];

            var res = new Layout(rows, column);
            foreach (var r in Enumerable.Range(0, rows))
            foreach (var c in Enumerable.Range(0, column))
            {
                var resCell = rowlines[r][c] == "1";
                res.UpdateScore(r, c, resCell ? 1 : 0, resCell ? 0 : 1);
            }

            res.UpdateStatus();
            return res;
        }

        /// <summary>
        ///     保存布局
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="layout"></param>
        public static void SaveAnnotation(string filename, Layout layout)
        {
            using var sw = new StreamWriter(filename, false);
            sw.Write(layout.ToAnnotationString());
        }

        #endregion
    }
}