using ILFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ILFramework.HotClass;
using Spine.Unity;
using DG.Tweening;

namespace CourseBirchesPart
{
   public class TargetChild
    {
        private ILObject3DAction btn;
        private GameObject sprite, npc, mask;
        string name;
        int voiceCount;

        public ILObject3DAction Btn { get => btn;}

        public TargetChild(GameObject go,int index,string aniName)
        {
            Transform curTran = go.transform.Find("btns");
            this.btn = curTran.GetChild(index).GetComponent<ILObject3DAction>();
            curTran = go.transform.Find("showSprits");
            mask = go.transform.Find("mask").gameObject;
            this.sprite = curTran.GetChild(index).gameObject;
            this.name = aniName;
            voiceCount = index + 1;
            npc = go.transform.parent.transform.Find("dingding").gameObject;
            SpineManager.instance.PlayAnimationState(btn.GetComponent<SkeletonAnimation>(), name);
            btn.transform.localScale = Vector3.zero;
            btn.GetComponent<SkeletonAnimation>().timeScale = 1.0f;
            sprite.SetActive(false);
        }
        public void BtnClick(Action act)
        {
            playDoAnimation();
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            SoundManager.instance.Speaking(npc,"talk", SoundManager.SoundType.SOUND, voiceCount, null, () =>
            {
                mask.gameObject.SetActive(true);
            });
            ShowSprite(act);
        }

        public float playDoAnimation(Action act = null)
        {
            if (act == null) act = () => { };
            float time = SpineManager.instance.DoAnimation(btn.gameObject, name, false, ()=> 
            {
                act();
            });
            btn.transform.localScale = Vector3.one;
            return time;
        }
        public void ShowSprite(Action act)
        {
            sprite.SetActive(true);
            sprite.transform.localScale = Vector3.zero;
            sprite.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart).OnComplete(()=>act());
        }
        public void HideSprite(Action act)
        {
            sprite.SetActive(false);
            act();
        }
    }
}
