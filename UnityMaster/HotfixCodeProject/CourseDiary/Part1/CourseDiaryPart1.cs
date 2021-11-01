using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseDiaryPart1
    {
        GameObject curGo;
        List<Button> onClickList;
        GameObject parentOnClick;
        Transform target;
        GameObject targetAnimation;
        GameObject tiantian;
        Button mask;
        GameObject frontMask;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            parentOnClick = curTrans.Find("onclick").gameObject;
            onClickList = new List<Button>();
            for(int i = 0;i < parentOnClick.transform.childCount;i++)
            {
                Button btn = parentOnClick.transform.GetChild(i).GetComponent<Button>();
                int index = i;
                btn.onClick.AddListener(() =>  OnMouseDown(index));
                onClickList.Add(btn);
            };
            target = curTrans.Find("target");
            targetAnimation = target.Find("targetAnimation").gameObject;
            tiantian = curTrans.Find("tiantian").gameObject;
            mask = target.transform.Find("mask").GetComponent<Button>();
            mask.onClick.AddListener(OnMouseDownOnclick);
            frontMask = curTrans.Find("frontMask").gameObject;
            OnInit();
            SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, 0, () => frontMask.SetActive(true), Speaking);
            SoundManager.instance.BgSoundPart1();
        }
        public void Speaking()
        {
            SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, 1, null,() => frontMask.SetActive(false));
        }
        public void OnMouseDown(int index)
        {
            SpineManager.instance.PlayAnimationState(targetAnimation.GetComponent<SkeletonGraphic>(), onClickList[index].name);
            PlayAnimation(index);
            target.localScale = Vector3.one;
        }
        public void OnInit()
        {
            frontMask.SetActive(false);
            target.localScale = Vector3.zero;
        }
        public void OnReset()
        {
            target.localScale = Vector3.zero;
        }
        public void PlayAnimation(int index)
        {
            SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, index + 2, ()=>frontMask.SetActive(true), () => frontMask.SetActive(false));
            SpineManager.instance.DoAnimation(targetAnimation, onClickList[index].name, false);
        }
        public void OnMouseDownOnclick()
        {
            OnReset();
        }
    }
}
