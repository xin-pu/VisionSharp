using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using VisionSharp.Models.Category;

namespace VisionSharp.Models.Layout
{
    public class LayoutCell<T> : ObservableObject where T : Enum
    {
        private int _column;
        private Reliable _reliable;
        private int _row;
        private double _score;
        private double[] _scoreCategory;
        private T _t;

        /// <summary>
        ///     布局预测每个位置的信息结构类
        ///     初始非可靠
        /// </summary>
        public LayoutCell(int row, int column)
        {
            Row = row;
            Column = column;
            Reliable = Reliable.Unreliable;
            Category = default;
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

        /// <summary>
        ///     结果是否可靠
        /// </summary>
        public Reliable Reliable
        {
            protected set => SetProperty(ref _reliable, value);
            get => _reliable;
        }


        /// <summary>
        ///     分类
        /// </summary>
        public T Category
        {
            internal set => SetProperty(ref _t, value);
            get => _t;
        }


        /// <summary>
        ///     激活后的分类分数
        /// </summary>
        public double[] ScoreCategory
        {
            internal set => SetProperty(ref _scoreCategory, value);
            get => _scoreCategory;
        }

        /// <summary>
        ///     激活后的分类分数
        /// </summary>
        public double Score
        {
            internal set => SetProperty(ref _score, value);
            get => _score;
        }

        /// <summary>
        ///     更新状态，唯一入口可更新分数
        /// </summary>
        /// <param name="score"></param>
        /// <param name="threshold"></param>
        public void UpdateScore(double[] score, double threshold)
        {
            ScoreCategory = score.ToArray();
            Score = ScoreCategory.Max();
            Reliable = Score >= threshold ? Reliable.Reliable : Reliable.Unreliable;
            Category = (T) Enum.ToObject(typeof(T), ScoreCategory.ToList().IndexOf(Score));
        }

        /// <summary>
        ///     获取单元状态，
        /// </summary>
        /// <returns>置物为True,置空为False,存疑的为空</returns>
        public T GetLayoutStatus()
        {
            return Reliable == Reliable.Unreliable
                ? default
                : Category;
        }

        /// <summary>
        ///     返回激活后的分数中最大项
        /// </summary>
        /// <returns></returns>
        /// <remarks>如果没有激活，返回空</remarks>
        public double GetScore()
        {
            return ScoreCategory.Max();
        }

        public override string ToString()
        {
            var str = new StringBuilder(
                $"[{Row},{Column}]:{ToCategoryStatus()})");
            return str.ToString();
        }

        /// <summary>
        ///     转换成分数字符串
        /// </summary>
        /// <returns></returns>
        public string ToScoreStatus()
        {
            if (ScoreCategory != null)
            {
                var scoreStr = ScoreCategory.Select(s => $"[{s:F4}]");
                return string.Join(",", scoreStr);
            }

            return string.Empty;
        }

        /// <summary>
        ///     转换成枚举数值字符
        /// </summary>
        /// <returns></returns>
        public string ToValueStatus()
        {
            return Reliable == Reliable.Unreliable
                ? "?"
                : Convert.ToInt32(Category).ToString("D");
        }

        /// <summary>
        ///     转换成枚举分类
        /// </summary>
        /// <returns></returns>
        public string ToCategoryStatus()
        {
            return Reliable == Reliable.Unreliable
                ? "Unreliable"
                : Category.ToString();
        }
    }
}