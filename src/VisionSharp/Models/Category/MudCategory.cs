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
        Other = 1,

        /// <summary>
        ///     位置有产品，但丢失散热泥
        /// </summary>
        MissMud = 0
    }

    /// <summary>
    ///     二维码检测分类
    /// </summary>
    public enum QrCategory
    {
        QrCode
    }

    /// <summary>
    ///     浣熊检测分类（模型测试用）
    /// </summary>
    public enum Raccoon
    {
        Raccoon
    }
}