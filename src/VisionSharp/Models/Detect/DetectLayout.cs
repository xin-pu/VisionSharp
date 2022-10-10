using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Detect
{
    public class DetectLayout : ObservableObject, IEquatable<DetectLayout>
    {
        private int _column;
        private List<DetectPosition> _detectPositions;
        private int _row;
        private double _scoreThreshold;

        public DetectLayout(int row, int column, double scoreThreshold = 0.7)
        {
            Row = row;
            Column = column;
            ScoreThreshold = scoreThreshold;
            DetectPositions = new List<DetectPosition>();
            foreach (var r in Enumerable.Range(0, row))
            foreach (var c in Enumerable.Range(0, column))
            {
                DetectPositions.Add(new DetectPosition(r, c));
            }
        }

        public int Row
        {
            protected set => SetProperty(ref _row, value);
            get => _row;
        }

        public int Column
        {
            protected set => SetProperty(ref _column, value);
            get => _column;
        }

        public List<DetectPosition> DetectPositions
        {
            protected set => SetProperty(ref _detectPositions, value);
            get => _detectPositions;
        }


        public double ScoreThreshold
        {
            protected set => SetProperty(ref _scoreThreshold, value);
            get => _scoreThreshold;
        }

        public bool Equals(DetectLayout? other)
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
                var res = DetectPositions.All(d =>
                    d.LayoutStatus == other.GetDetectPosition(d.Row, d.Column).LayoutStatus);
                return res;
            }

            return false;
        }


        public int GetHashCode(DetectLayout obj)
        {
            return HashCode.Combine(obj._column, obj._detectPositions, obj._row, obj._scoreThreshold);
        }


        /// <summary>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="score">sigmoid score</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetStatus(int row, int column, double positiveScore, double negativeScore)
        {
            var detectPosition = GetDetectPosition(row, column);
            detectPosition.PositiveScore = positiveScore;
            detectPosition.NegativeScore = negativeScore;

            detectPosition.LayoutStatus = positiveScore < ScoreThreshold && negativeScore < ScoreThreshold
                ? LayoutStatus.Unidentified
                : positiveScore > negativeScore
                    ? LayoutStatus.Positive
                    : LayoutStatus.Negative;
        }

        /// <summary>
        ///     判断整个预测结果是否可靠，只要有一个位置分数存疑，则失败
        /// </summary>
        /// <returns></returns>
        public bool GetReliability()
        {
            return DetectPositions.All(d => d.LayoutStatus != LayoutStatus.Unidentified);
        }

        public bool[,] GetBoolLayout()
        {
            var res = new bool[Row, Column];
            Enumerable.Range(0, Row).ToList()
                .ForEach(r => Enumerable.Range(0, Column).ToList()
                    .ForEach(c => res[r, c] = GetDetectPosition(r, c).GetLayoutStatus()));
            return res;
        }

        public void ChangeThreshold(double scoreThreshold)
        {
            ScoreThreshold = scoreThreshold;
        }


        public DetectPosition GetDetectPosition(int row, int column)
        {
            return DetectPositions.FirstOrDefault(a => a.Row == row && a.Column == column)
                   ?? throw new InvalidOperationException();
        }

        public override string ToString()
        {
            var str = new StringBuilder();
            foreach (var r in Enumerable.Range(0, Row))
            {
                var res = Enumerable.Range(0, Column)
                    .Select(c => GetDetectPosition(r, c).LayoutStatus)
                    .Select(a => $"{(int) a}");
                str.AppendLine(string.Join(",", res));
            }

            if (GetReliability())
            {
                return str.ToString();
            }

            {
                var unidentifieds = DetectPositions
                    .Where(a => a.LayoutStatus == LayoutStatus.Unidentified)
                    .ToList();
                foreach (var u in unidentifieds)
                {
                    str.AppendLine(u.ToString());
                }
            }

            return str.ToString();
        }

        public string ToScoreString()
        {
            var str = new StringBuilder();

            var unidentifieds = DetectPositions
                .ToList();
            foreach (var u in unidentifieds)
            {
                str.AppendLine(u.ToString());
            }

            return str.ToString();
        }

        public static DetectLayout LoadFromAnnotation(string annfile)
        {
            using var sr = new StreamReader(annfile);
            var lines = sr.ReadToEnd().Split('\r', '\n').Where(a => a != "").ToList();
            var rowlines = lines.Select(r => r.Split(',').Where(a => a != "").ToList()).ToList();
            var rows = rowlines.Count();
            var columns_rows = rowlines.Select(a => a.Count()).Distinct().ToList();
            if (columns_rows.Count() != 1)
            {
                throw new FileLoadException();
            }

            var column = columns_rows[0];

            var res = new DetectLayout(rows, column);
            foreach (var r in Enumerable.Range(0, rows))
            foreach (var c in Enumerable.Range(0, column))
            {
                var res_cell = rowlines[r][c] == "1";
                res.SetStatus(r, c, res_cell ? 1 : 0, res_cell ? 0 : 1);
            }

            return res;
        }

        public static void SaveAnnotation(string filename, DetectLayout detectLayout)
        {
            using var sw = new StreamWriter(filename, false);
            sw.Write(detectLayout.ToString());
        }
    }

    /// <summary>
    ///     布局预测每个位置的信息结构类
    /// </summary>
    public class DetectPosition : ObservableObject
    {
        private int _column;
        private LayoutStatus _layoutStatus;
        private double _negativeScore;
        private double _positiveScore;
        private int _row;

        public DetectPosition(int row, int column)
        {
            Row = row;
            Column = column;
            LayoutStatus = LayoutStatus.Unidentified;
        }

        public int Row
        {
            protected set => SetProperty(ref _row, value);
            get => _row;
        }

        public int Column
        {
            protected set => SetProperty(ref _column, value);
            get => _column;
        }

        public LayoutStatus LayoutStatus
        {
            set => SetProperty(ref _layoutStatus, value);
            get => _layoutStatus;
        }

        public double PositiveScore
        {
            set => SetProperty(ref _positiveScore, value);
            get => _positiveScore;
        }

        public double NegativeScore
        {
            set => SetProperty(ref _negativeScore, value);
            get => _negativeScore;
        }

        public bool GetLayoutStatus()
        {
            return LayoutStatus == LayoutStatus.Positive;
        }

        public double GetScore()
        {
            return LayoutStatus switch
            {
                LayoutStatus.Positive => PositiveScore,
                LayoutStatus.Negative => NegativeScore,
                _ => double.NaN
            };
        }

        public override string ToString()
        {
            var score = $"[{PositiveScore:F4},{NegativeScore:F4}]";
            var str = new StringBuilder($"[{Row},{Column}]:{LayoutStatus}\t{score})");
            return str.ToString();
        }
    }

    /// <summary>
    ///     分类结果
    /// </summary>
    public enum LayoutStatus
    {
        /// <summary>
        ///     存疑
        /// </summary>
        Unidentified = -1,

        /// <summary>
        ///     有目标物
        /// </summary>
        Positive = 1,

        /// <summary>
        ///     无目标物
        /// </summary>
        Negative = 0
    }
}