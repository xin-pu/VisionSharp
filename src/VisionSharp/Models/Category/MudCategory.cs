namespace VisionSharp.Models.Category
{
    /// <summary>
    ///     散热泥项目分类
    /// </summary>
    public enum MudCategory
    {
        /// <summary>
        ///     位置没有产品
        /// </summary>
        Empty = 0,

        /// <summary>
        ///     位置有产品，但丢失散热泥
        /// </summary>
        DutMissMud = 1,

        /// <summary>
        ///     位置有产品，并且有散热泥
        /// </summary>
        DutWithMud = 2
    }
}