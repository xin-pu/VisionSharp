using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using VisionSharp.Models.Category;

namespace VisionSharp.Models.Layout
{
    public class Layout<T> : ObservableObject, IEquatable<Layout<T>> where T : Enum
    {
        private int _column;
        private List<LayoutCell<T>> _layoutCells;
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
            LayoutCells = new List<LayoutCell<T>>();
            foreach (var r in Enumerable.Range(0, row))
            foreach (var c in Enumerable.Range(0, column))
            {
                LayoutCells.Add(new LayoutCell<T>(r, c));
            }
        }

        /// <summary>
        ///     获取分类数目
        /// </summary>
        public int CategoryCount => Enum.GetValues(typeof(T)).Length;

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
        internal List<LayoutCell<T>> LayoutCells
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
        public LayoutCell<T> this[int row, int column] =>
            LayoutCells.FirstOrDefault(a => a.Row == row && a.Column == column);

        /// <summary>
        ///     获取布局中一行单元
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public LayoutCell<T>[] this[int row] =>
            LayoutCells.Where(a => a.Row == row).OrderBy(a => a.Column).ToArray();

        /// <summary>
        ///     判断整个预测结果是否可靠
        ///     只要有一个位置分数存疑，则失败
        /// </summary>
        /// <returns></returns>
        public bool GetReliability()
        {
            return LayoutCells.All(d => d.Reliable == Reliable.Reliable);
        }

        /// <summary>
        ///     更新分数，必须紧跟更新状态才这个布局状态更新生效
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="score">sigmoid score</param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateScore(int row, int column, double[] scores, double threshold = 0.7)
        {
            var cell = this[row, column];
            cell.UpdateScore(scores, threshold);
        }


        /// <summary>
        ///     获取分类布局
        /// </summary>
        /// <returns></returns>
        public T[,] ToLayoutStatus()
        {
            var res = new T[Row, Column];
            Enumerable.Range(0, Row).ToList()
                .ForEach(r => Enumerable.Range(0, Column).ToList()
                    .ForEach(c => res[r, c] = this[r, c].Category));
            return res;
        }

        /// <summary>
        ///     获取可靠度布局
        /// </summary>
        /// <returns></returns>
        public Reliable[,] ToReliables()
        {
            var res = new Reliable[Row, Column];
            Enumerable.Range(0, Row).ToList()
                .ForEach(r => Enumerable.Range(0, Column).ToList()
                    .ForEach(c => res[r, c] = this[r, c].Reliable));
            return res;
        }

        /// <summary>
        ///     修改可靠度阈值
        /// </summary>
        /// <param name="scoreThreshold"></param>
        public void UpdateThreshold(double scoreThreshold)
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
                var status = string.Join("\t", this[row].Select(c => c.ToValueStatus()));
                var line = $"{row:D2}|\t{status}";
                str.AppendLine(line);
            }

            str.AppendLine("");
            foreach (var row in Enumerable.Range(0, Row))
            {
                var score = string.Join("\t", this[row].Select(c => c.ToScoreStatus()));
                var line = $"{row:D2}|\t{score}";
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
                var line = string.Join(",", this[row].Select(c => c.ToValueStatus()));
                str.AppendLine(line);
            }

            return str.ToString();
        }


        #region 比较器

        public bool Equals(Layout<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is null)
            {
                return false;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            if (Row == other.Row && Column == other.Column)
            {
                return ToAnnotationString().Equals(other.ToAnnotationString());
            }

            return false;
        }

        public int GetHashCode(Layout<T> obj)
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
        public static Layout<T> LoadFromAnnotation(string annfile, char split = ',')
        {
            var cateCount = Enum.GetValues(typeof(T)).Length;

            using var sr = new StreamReader(annfile);
            var lines = sr.ReadToEnd()
                .Split('\r', '\n')
                .Where(a => a != "")
                .ToList();

            var rowlines = lines
                .Select(r => r.Split(split).Where(a => a != "").ToList())
                .ToList();
            var rows = rowlines.Count;
            var columnsRows = rowlines.Select(a => a.Count).Distinct().ToList();
            if (columnsRows.Count != 1)
            {
                throw new FileLoadException();
            }

            var column = columnsRows[0];

            var res = new Layout<T>(rows, column);
            foreach (var r in Enumerable.Range(0, rows))
            foreach (var c in Enumerable.Range(0, column))
            {
                var resCell = int.Parse(rowlines[r][c]);
                var score = new double[cateCount];
                score[resCell] = 1;
                res.UpdateScore(r, c, score);
            }

            return res;
        }

        /// <summary>
        ///     保存布局
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="layout"></param>
        public static void SaveAnnotation(string filename, Layout<T> layout)
        {
            using var sw = new StreamWriter(filename, false);
            sw.Write(layout.ToAnnotationString());
        }

        #endregion
    }
}