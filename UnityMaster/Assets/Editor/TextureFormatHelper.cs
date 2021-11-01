using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TextureFormatHelper : FormatHelper
{
    protected override bool IsAttachToPath()
    {
        return assetPath.Contains("Textures") && assetPath.Contains("HotFixPackage");
    }

    protected override bool IsAttachToFile()
    {
        return assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg");
    }


    protected override void SetImporterPlatform(params object[] paramsObjects)
    {
        return;
        if (paramsObjects.Length == 0)
            return;

        var platform = (string)paramsObjects[0];
        var isAlpha = (bool)paramsObjects[1];
        var importer = (TextureImporter)paramsObjects[2];

        var settings = importer.GetPlatformTextureSettings(platform);
        settings.overridden = true;
        settings.format = GetPlatformFormat(platform, isAlpha);
        importer.SetPlatformTextureSettings(settings);

        Debug.Log($"处理Texture资源的平台：{platform}；Path：{assetPath}");
    }

    private void OnPreprocessTexture()
    {
        return;
        TextureImporter importer = assetImporter as TextureImporter;

        if (importer != null && IsAttachToPath() && IsAttachToFile())
        {
            var isAlpha = IsAlpha(importer);
            var size = GetTextureImporterSize(importer);
            var isBigTexture = IsBigTexture(size.Item1, size.Item2);
            var isPowerOfTwo = IsPowerOfTwo(size.Item1, size.Item2);

            importer.textureType = isBigTexture ? TextureImporterType.GUI : TextureImporterType.Sprite;
            importer.alphaIsTransparency = isAlpha;

            if (importer.textureType == TextureImporterType.GUI)
                importer.npotScale =
                    isPowerOfTwo ? TextureImporterNPOTScale.None : TextureImporterNPOTScale.ToNearest;

            importer.alphaSource = isAlpha ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;

            SetImporterPlatform(Android, isAlpha, importer);
          //  SetImporterPlatform(iPhone, isAlpha, importer);
            SetImporterPlatform(PC, isAlpha, importer);
        }
    }


    /// <summary>
    /// 获取平台格式
    /// </summary>
    /// <param name="platform">平台名</param>
    ///  /// <param name="isAlpha">是否有Alpha</param>
    /// <returns></returns>
    private TextureImporterFormat GetPlatformFormat(string platform, bool isAlpha)
    {
        switch (platform)
        {
            case Android:
                return isAlpha ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4;
            case iPhone:
                return isAlpha ? TextureImporterFormat.ASTC_RGBA_8x8 : TextureImporterFormat.ASTC_RGB_8x8;
            case PC:
                return isAlpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1;
        }

        return TextureImporterFormat.Alpha8;
    }


    /// <summary>
    /// 获取Size
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    private static (int, int) GetTextureImporterSize(TextureImporter importer)
    {
        object[] args = new object[2];
        MethodInfo mi =
            typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi?.Invoke(importer, args);
        return ((int)args[0], (int)args[1]);
    }

    /// <summary>
    /// 是否是大Texture
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    private static bool IsBigTexture(int width, int height)
    {
        return width >= 200 && height >= 200;
    }


    /// <summary>
    /// 是否被4整除
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    private static bool IsDivisibleOf4(int width, int height)
    {
        return (width % 4 == 0 && height % 4 == 0);
    }


    /// <summary>
    /// 是否是2的整数次幂
    /// </summary>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    private static bool IsPowerOfTwo(int width, int height)
    {
        return (width == height) && (width > 0) && ((width & (width - 1)) == 0);
    }


    /// <summary>
    /// 是否有Alpha
    /// </summary>
    /// <param name="importer"></param>
    /// <returns></returns>
    private static bool IsAlpha(TextureImporter importer)
    {
        return importer.DoesSourceTextureHaveAlpha();
    }
}
