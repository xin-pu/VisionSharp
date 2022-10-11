using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VisionSharp.Models.Layout
{
    public class LayoutCell : ObservableObject
    {
        private int _column;
        private LayoutStatus _layoutStatus;
        private double _negativeScore;
        private double _positiveScore;
        private int _row;

        /// <summary>
        ///     布局预测每个位置的信息结构类
        /// </summary>
        public LayoutCell(int row, int column)
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

        /// <summary>
        ///     状态
        /// </summary>
        public LayoutStatus LayoutStatus
        {
            set => SetProperty(ref _layoutStatus, value);
            get => _layoutStatus;
        }

        /// <summary>
        ///     置物的分数
        /// </summary>
        public double PositiveScore
        {
            internal set => SetProperty(ref _positiveScore, value);
            get => _positiveScore;
        }

        /// <summary>
        ///     置空的分数
        /// </summary>
        public double NegativeScore
        {
            internal set => SetProperty(ref _negativeScore, value);
            get => _negativeScore;
        }


        public void UpdateScore(float[] score)
        {
            PositiveScore = score[0];
            NegativeScore = score[1];
        }

        /// <summary>
        ///     获取单元状态，
        /// </summary>
        /// <returns>置物为True,置空为False,存疑的为空</returns>
        public bool? GetLayoutStatus()
        {
            switch (LayoutStatus)
            {
                case LayoutStatus.Positive:
                    return true;
                case LayoutStatus.Negative:
                    return false;
                default:
                    return null;
            }
        }

        /// <summary>
        ///     返回激活后的分数
        /// </summary>
        /// <returns></returns>
        /// <remarks>如果没有激活，返回空</remarks>
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
            var str = new StringBuilder(
                $"[{Row},{Column}]:{AsStrStatus()}\t{AsScoreStatus()}");
            return str.ToString();
        }

        public string AsScoreStatus()
        {
            return $"[{PositiveScore:F4},{NegativeScore:F4}]";
        }

        public string AsStrStatus()
        {
            return LayoutStatus switch
            {
                LayoutStatus.Positive => "+",
                LayoutStatus.Negative => "-",
                _ => "?"
            };
        }

        public string AsAnnStatus()
        {
            return LayoutStatus switch
            {
                LayoutStatus.Positive => "1",
                LayoutStatus.Negative => "0",
                _ => "?"
            };
        }
    }
}