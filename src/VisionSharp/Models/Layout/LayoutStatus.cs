namespace VisionSharp.Models.Layout
{
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