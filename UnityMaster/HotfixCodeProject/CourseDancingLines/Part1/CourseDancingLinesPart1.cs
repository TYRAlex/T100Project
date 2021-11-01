using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseDancingLinesPart1
    {
        GameObject curGo;
        GameObject target1;
        GameObject target2;
        string[] names;
        GameObject dingding;
        int numIndex;
        int maxIndex;
        GameObject btnStart;
        GameObject btnEnd;
        GameObject curObj;
        GameObject nextObj;
        GameObject bg;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            numIndex = 0;
            maxIndex = 3;
            btnStart = curTrans.Find("Content/btnStart").gameObject;
            btnEnd = curTrans.Find("Content/btnEnd").gameObject;
            dingding = curTrans.Find("Content/dingding").gameObject;
            target1 = curTrans.Find("Content/target1").gameObject;
            target2 = curTrans.Find("Content/target2").gameObject;
            bg = curTrans.Find("Content/bg").gameObject;
            Util.AddBtnClick(btnStart, BtnStartOnClick);
            Util.AddBtnClick(btnEnd, BtnEndOnClick);
            GetNames();
            SpineManager.instance.PlayAnimationState(target1.GetComponent<SkeletonGraphic>(), names[numIndex]);
            SpineManager.instance.PlayAnimationState(target2.GetComponent<SkeletonGraphic>(), names[numIndex]);
            btnStart.SetActive(false);
            btnEnd.SetActive(false);
            curObj = target2;
            SpineManager.instance.DoAnimation(bg, "animation");
            SpeakStart();
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            for(int i = 0; i < 4; i++)
            {
                curTrans.Find("Descrip" + i).gameObject.SetActive(false);
            }
        }
        public void SpeakStart()
        {
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, 0, ()=>SoundManager.instance.skipBtn.SetActive(false),()=> btnStart.SetActive(true));
        }
        /*
        public void Speak()
        {
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, numIndex, () => SoundManager.instance.skipBtn.SetActive(false), () => btnEnd.SetActive(true));
        }
        */
        public void BtnStartOnClick(GameObject go)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            go.SetActive(false);
            if(curObj.name == target1.name)
            {
                curObj = target2;
                nextObj = target1;
            }
            else
            {
                curObj = target1;
                nextObj = target2;
            }
            nextObj.transform.localScale = new Vector3(0, 0, 1);
            curObj.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            if (numIndex  < maxIndex)
            {
                SpineManager.instance.PlayAnimationState(nextObj.GetComponent<SkeletonGraphic>(), names[numIndex + 1]);
            }
            SpineManager.instance.DoAnimation(curObj, names[numIndex], false, ()=>
            {
                btnEnd.SetActive(true);
            });
            for (int i = 0; i < 4; i++)
            {
                if (i != numIndex)
                {
                    dingding.transform.parent.parent.Find("Descrip" + i).gameObject.SetActive(false);
                }
                else
                {
                    dingding.transform.parent.parent.Find("Descrip" + i).gameObject.SetActive(true);
                }
            }
        }
        public void BtnEndOnClick(GameObject go)
        {
            go.SetActive(false);
            if (numIndex < maxIndex)
            {
              
                curObj.transform.localScale = new Vector3(0, 0, 1);
                nextObj.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                numIndex++;
                btnStart.SetActive(true);
                // BtnStartOnClick(btnStart);
                for (int i = 0; i < 4; i++)
                {
                    dingding.transform.parent.parent.Find("Descrip" + i).gameObject.SetActive(false);
                }
            }

        }
        public void GetNames()
        {
            SkeletonGraphic skeleton = target1.GetComponent<SkeletonGraphic>();
            ExposedList<Animation> list = skeleton.SkeletonData.Animations;
            Animation[] animations = list.ToArray();
            names = new string[list.Count];
            for (int i = 0;i < animations.Length;i++)
            {
                names[i] = animations[i].Name;
            }
        }
    }
}
