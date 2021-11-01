using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseCavePart2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            SoundManager.instance.BgSoundPart2();
            if (curGo.transform.Find("ScrollView").gameObject.GetComponent<ScrollRectMoveManager>() != null)
            {
                GameObject.DestroyImmediate(curGo.transform.Find("ScrollView").gameObject.GetComponent<ScrollRectMoveManager>());
            }
            curGo.transform.Find("ScrollView").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curGo.transform.Find("ScrollView"));

            SoundManager.instance.sheildGo.SetActive(false);
            SoundManager.instance.skipBtn.SetActive(false);
        }
    }
}
