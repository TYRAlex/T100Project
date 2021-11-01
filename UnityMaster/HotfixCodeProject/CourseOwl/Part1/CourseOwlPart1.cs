using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseOwlPart1
    {
        GameObject curGo;
        public SkeletonGraphic [] skeletonGraphics;
        public string[] data;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp1", "tamp2", "tamp3" },Speak);
            skeletonGraphics = new SkeletonGraphic[5];
            data = new string[] { "yj", "de", "mj", "ce", "xx" };
            GameObject go = curGo.transform.Find("tamp_Parent/tamp1").gameObject;
            for(int i = 0;i<go.transform.childCount;i++)
            {
                skeletonGraphics[i] = go.transform.GetChild(i).GetComponent<SkeletonGraphic>();
                SpineManager.instance.PlayAnimationState(skeletonGraphics[i], data[i]);
                go.transform.GetChild(i).GetChild(0).GetComponent<ILObject3DAction>().index = i;
                go.transform.GetChild(i).GetChild(0).GetComponent<ILObject3DAction>().OnPointUpLua = OnPointerUp;
                if(i == 1)
                {
                    SpineManager.instance.HideSpineTexture(skeletonGraphics[i], "ui_L");
                }
                SpineManager.instance.HideSpineTexture(skeletonGraphics[i], "bg");
            }
            SoundManager.instance.BgSoundPart1();
        }
        
        public void Speak()
        {
            int index = PowerpointManager.instance.curIndex;
            if (index != 0)
            {
                PowerpointManager.instance.MaskHide();
                float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index + 4);
                curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time));
            }
        }
        public void OwlState(bool isOn)
        {
            int scale = isOn == true ? 1 : 0;
            foreach(var v in skeletonGraphics)
            {
                v.transform.GetChild(0).transform.localScale = new Vector3(scale, scale, scale);
            }
            if (isOn)
            {
                PowerpointManager.instance.MaskShow();
            }
            else
            {
                PowerpointManager.instance.MaskHide();
            }
        }
        IEnumerator wait(float time, Action<bool> act = null, bool isOn = true)
        {
            yield return new WaitForSeconds(time);
            if (act != null)
            {
                act(isOn);
               
            }
            else
            {
                PowerpointManager.instance.MaskShow();
            }
        }
        public void OnPointerUp(int index)
        {
            if(PowerpointManager.instance.IsMove == false)
            {
                //µã»÷°´Å¥
                OwlState(false);
                float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index);
                SpineManager.instance.DoAnimation(skeletonGraphics[index].gameObject, data[index], false);
                curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time, OwlState,true));
            }
        }
    }
}
