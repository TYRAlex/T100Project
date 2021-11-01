using UnityEditor;


public class FormatHelper : AssetPostprocessor
{
    /// <summary>
    /// 安卓平台
    /// </summary>
    protected const string Android = "Android";

    /// <summary>
    /// iPhone平台
    /// </summary>
    protected const string iPhone = "iPhone";

    /// <summary>
    /// PC或Mac平台
    /// </summary>
    protected const string PC = "PC";

    /// <summary>
    /// 是否在指定路径
    /// </summary>
    protected virtual bool IsAttachToPath()
    {
        return assetPath.Contains(string.Empty);
    }


    /// <summary>
    /// 是否是指定类型文件
    /// </summary>
    protected virtual bool IsAttachToFile()
    {
        return assetPath.EndsWith(string.Empty);
    }

    /// <summary>
    /// 设置导入平台格式
    /// </summary>
    /// <param name="paramsObjects"></param>
    protected virtual void SetImporterPlatform(params object[] paramsObjects)
    {
    }
}
