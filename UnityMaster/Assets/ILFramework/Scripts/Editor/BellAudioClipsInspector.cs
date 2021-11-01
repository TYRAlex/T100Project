using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(BellAudiosClip))]
public class BellAudioClipsInspector : Editor
{
    BellAudiosClip mTarget;

    string bgmAudios = "/Bgm";
    string soundAudios = "/Sound";
    string voiceAudios = "/Voice";
    //char[] nums = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    string commonPath = "Assets/HotFixPackage/";
    string cmResPath = "Assets/ILFramework/CommonRes/Audios";
    string framePath = "Assets/ILFramework/";
    bool isReplace;
    List<AudioClip> SearchAudios(string directory)
    {
        string audiosPath;
        List<AudioClip> clips = new List<AudioClip>();
        try
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] fileInfo = dir.GetFiles("*.*");
            SortFileInfo(fileInfo);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                if (fileInfo[i].Name.EndsWith(".wav") || fileInfo[i].Name.EndsWith(".mp3") || fileInfo[i].Name.EndsWith(".ogg") || fileInfo[i].Name.EndsWith(".aiff"))
                {
                    audiosPath = fileInfo[i].FullName.Substring(fileInfo[i].FullName.IndexOf("Assets"));
                    //Debug.Log("******* SearchAudios  filesName: " + audiosPath);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audiosPath);
                    clips.Add(clip);
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("请检查文件路径是否存在： " + directory);
        }
        return clips;
    }

    // 因为 GetFiles 返回的数组顺序是按照ASCII码进行排序的 所以这里要自己重新排序
    private static void SortFileInfo(FileInfo[] files)
    {
        FileInfo temp = null;
        string mStr, nStr;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.IndexOf("-") == -1)
                return;
            for (int j = i + 1; j < files.Length - 1; j++)
            {
                if (files[j].Name.IndexOf("-") == -1) // 如果不存在 "-" 则默认该语音(以及之后的全部语音)是后面新加的 不需要排序
                    break;
                mStr = files[i].Name.Substring(0, files[i].Name.IndexOf("-"));
                nStr = files[j].Name.Substring(0, files[j].Name.IndexOf("-"));
                //Debug.LogFormat(" -=-=-=-=- Sort A: {0},  B: {1}", mStr, nStr);
                if (int.Parse(mStr) > int.Parse(nStr))
                {
                    temp = files[i];
                    files[i] = files[j];
                    files[j] = temp;
                }
            }
        }
    }

    private void SetClips(string path)
    {
        //Debug.Log(" ***** SetClips path: " + path);
        var audios = SearchAudios(path);
        mTarget.clips = new AudioClip[audios.Count];
        for (int i = 0; i < audios.Count; i++)
        {
            mTarget.clips[i] = audios[i];
        }
    }

    private void ReNameAudios(string path)
    {
        string fileName;
        string filePath;
        int idx = 0;
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fileInfo = dir.GetFiles("*.*");
            SortFileInfo(fileInfo);
            for (int i = 0; i < fileInfo.Length; i++)
            {
                if (fileInfo[i].Name.EndsWith(".wav") || fileInfo[i].Name.EndsWith(".mp3") || fileInfo[i].Name.EndsWith(".ogg") || fileInfo[i].Name.EndsWith(".aiff"))
                {
                    fileName = idx + "-" + fileInfo[i].Name.Substring(fileInfo[i].Name.IndexOf("-") + 1);
                    filePath = fileInfo[i].FullName.Substring(0, fileInfo[i].FullName.LastIndexOf(@"\") + 1);
                    filePath += fileName;
                    //Debug.LogFormat("ReName: {0},  Path: {1}", fileName, filePath);
                    fileInfo[i].MoveTo(filePath);
                    idx++;
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("请检查文件路径是否存在： " + path);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        mTarget = target as BellAudiosClip;
        GameObject selectObj = Selection.activeGameObject;
        string selectStr = selectObj.name;
        int idx = selectStr.IndexOf('_');
        if (idx < 0)
            return;
        string cstr = selectStr.Substring(0, idx);
        bool isCommon = Directory.Exists(framePath + cstr);
        if (!Directory.Exists(commonPath + cstr) && !isCommon)
            cstr = cstr.Substring(0, cstr.Length - 5) + "/" + cstr.Substring(cstr.Length - 5, 5);
        string audiosPath = commonPath + cstr + "/Audios";
        string audiosPrefabPath = isCommon ? framePath+cstr+"/AudiosPrefab/" : commonPath + cstr + "/AudiosPrefab/";
        GUILayout.Space(10);
        if (GUILayout.Button("检索音频"))
        {
            switch (mTarget.audioType)
            {
                case BellAudiosClip.AudioType.Bgm:
                    SetClips(audiosPath + bgmAudios);
                    break;
                case BellAudiosClip.AudioType.Sound:
                    SetClips(audiosPath + soundAudios);
                    break;
                case BellAudiosClip.AudioType.Voice:
                    SetClips(audiosPath + voiceAudios);
                    break;
                case BellAudiosClip.AudioType.CommonBgm:
                    SetClips(cmResPath + bgmAudios);
                    break;
                case BellAudiosClip.AudioType.CommonSound:
                    SetClips(cmResPath + soundAudios);
                    break;
                case BellAudiosClip.AudioType.CommonVoice:
                    SetClips(cmResPath + voiceAudios);
                    break;
            }
            PrefabUtility.SaveAsPrefabAsset(selectObj, audiosPrefabPath + selectObj.name + ".prefab", out isReplace);
            if (!isReplace)
                Debug.LogError("保存预制失败！");
            else
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        GUILayout.Space(10);
        GUILayout.Label("*选择Audio类型,检索对应文件夹下的音频文件,文件格式类型包括：.mp3 .wav .ogg .aiff");
        GUILayout.Space(20);
        if(GUILayout.Button("重命名音频资源"))
        {
            switch(mTarget.audioType)
            {
                case BellAudiosClip.AudioType.Bgm:
                    ReNameAudios(audiosPath + bgmAudios);
                    break;
                case BellAudiosClip.AudioType.Sound:
                    ReNameAudios(audiosPath + soundAudios);
                    break;
                case BellAudiosClip.AudioType.Voice:
                    ReNameAudios(audiosPath + voiceAudios);
                    break;
                case BellAudiosClip.AudioType.CommonBgm:
                    ReNameAudios(cmResPath + bgmAudios);
                    break;
                case BellAudiosClip.AudioType.CommonSound:
                    ReNameAudios(cmResPath + soundAudios);
                    break;
                case BellAudiosClip.AudioType.CommonVoice:
                    ReNameAudios(cmResPath + voiceAudios);
                    break;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.Space(10);
        GUILayout.Label("*重命名对应Audio类型文件夹下的音频资源，根据当前顺序按照 1-xxxx.mp3 依次命名");
    }
}
