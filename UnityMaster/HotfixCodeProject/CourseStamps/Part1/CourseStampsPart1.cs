using CourseStampsPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseStampsPart1
    {
        public static GameObject curGo;
        int index;
        Transform Content;
        GameObject dingding;
        tampChild tampChild;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Content = curTrans.Find("Content");
            curGo.AddComponent<MesManager>();
            MesManager.instance.Register("CourseStampsPart",1, FinishOnClick);
            index = 0;
            tampChild = new tampChild(index);
            dingding = curTrans.Find("dingding").gameObject;
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, 0, null, () => {
                SoundManager.instance.BgSoundPart2();
                tampChild.AddOnClick(); });
            //SoundManager.instance.skipBtn.gameObject.SetActive(false);
        }
        public void FinishOnClick(params object[] ps)
        {
            index++;
            Debug.Log(index);
            if (index >= 4)
            {
                tampChild.ShowFinish();
                return;
            }
            for(int i = 0;i<Content.childCount;i++)
            {
                Content.GetChild(i).gameObject.SetActive(false);
            }
            tampChild = new tampChild(index);
            tampChild.AddOnClick();
        }
    }
}
