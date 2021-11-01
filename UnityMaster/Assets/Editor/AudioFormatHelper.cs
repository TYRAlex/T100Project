using UnityEditor;
using UnityEngine;


public class AudioFormatHelper : FormatHelper
{
    protected override bool IsAttachToPath()
    {
        return assetPath.Contains("Audios")&& assetPath.Contains("HotFixPackage");
    }

    protected override bool IsAttachToFile()
    {
        return assetPath.EndsWith(".mp3");
    }


    protected override void SetImporterPlatform(params object[] paramsObjects)
    {
        return;
        if (paramsObjects.Length == 0)
            return;

        var platform = (string)paramsObjects[0];
        var isLongAudio = (bool)paramsObjects[1];
        var importer = (AudioImporter)paramsObjects[2];

        var settings = importer.GetOverrideSampleSettings(platform);
        importer.loadInBackground = true;
        settings.loadType = isLongAudio ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
        settings.compressionFormat = isLongAudio ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.PCM;
        importer.SetOverrideSampleSettings(platform, settings);

        Debug.Log($"处理Audio资源的平台：{platform}；Path：{assetPath}");
    }

    public void OnPostprocessAudio(AudioClip clip)
    {
        return;
        AudioImporter importer = assetImporter as AudioImporter;

        if (importer != null && IsAttachToPath() && IsAttachToFile())
        {
            var isLongAudio = IsLongAudio(clip.length);

            SetImporterPlatform(Android, isLongAudio, importer);
            // SetImporterPlatform(iPhone, isLongAudio, importer);
            SetImporterPlatform(PC, isLongAudio, importer);
        }
    }

    /// <summary>
    /// 是否是长音频
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private bool IsLongAudio(float seconds)
    {
        return seconds >= 60.0f;
    }
}
