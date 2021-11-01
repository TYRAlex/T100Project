using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILFramework;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using OneID;

public class LoadCourse : MonoBehaviour
{
    public string course;
    public string coursead;
    public bool adMode = false;
    public GameObject preBtn, nextBtn;
    DirectoryInfo[] dirInfos = null;
    private void Start()
    {

#if UNITY_ANDROID 
        Screen.fullScreen = true;
#else
        Screen.fullScreen = false;
#endif

#if UNITY_EDITOR && UNITY_ANDROID || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX

        if (!adMode)
        {
            HotfixManager.instance.totalPackage = 1;
            preBtn.SetActive(false);
            nextBtn.SetActive(false);
            HotfixManager.instance.InstanceHotfixPackage(course, (p) =>
            {
                Debug.LogFormat(" Init {0} Success !!!! ", course);
                //HotfixManager.instance.ShowHotfixPackage(course);
                GameManager.instance.PlayUnity(course);
            });
        }
        else
        {
            preBtn.SetActive(true);
            nextBtn.SetActive(true);
            // 预加载测试
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/HotFixPackage" + "/" + coursead);
            dirInfos = dir.GetDirectories();
            string[] s = new string[dirInfos.Length];
            for (int i = 0,len = dirInfos.Length; i < len; i++)
            {
                s[i] = coursead +"/"+ dirInfos[i].Name;
            }
            HotfixManager.instance.totalPackage = dirInfos.Length+1;
            adPackage(s);
            int curIdx = 0;
            Util.AddBtnClick(nextBtn, (go) =>
            {
                if (curIdx == s.Length - 1)
                    return;
                if (curIdx >= 0)
                    GameManager.instance.StopUnity(s[curIdx]);
                curIdx++;
                GameManager.instance.PlayUnity(s[curIdx]);
            });

            Util.AddBtnClick(preBtn, (go) =>
            {
                if (curIdx == 0)
                    return;
                GameManager.instance.StopUnity(s[curIdx]);
                curIdx--;
                GameManager.instance.PlayUnity(s[curIdx]);
            });
        }
#endif
    }

    void adPackage(string[] str, int i = 0)
    {
        HotfixManager.instance.InstanceHotfixPackage(str[i], (p) =>
        {
            Debug.LogFormat(" Init {0} Success !!!! ", str[i]);
            if (i < str.Length - 1)
            {
                adPackage(str, ++i);
            }
            else
            {
                //if (ResourceManager.instance.LoadAllDynamicRes())
                //todo.... 暂时关闭20210427
                 
                 if (GameManager.instance.IsPlayingOneIDCourse&&str[0].Contains("TD5623"))
                 {
                     OneIDSceneManager.Instance.ShowTargetPanel(OneID_SceneType.CameraPanel);
                     OneIDSceneManager.Instance.LoadAllVideoPlayer();
                 }
                 else
                 {
                     GameManager.instance.PlayUnity(str[0]);
                 }

                
            }
        });
    }
}
