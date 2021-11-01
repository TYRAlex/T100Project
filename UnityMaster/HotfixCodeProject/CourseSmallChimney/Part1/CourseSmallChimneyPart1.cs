using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseSmallChimneyPart1
    {
        GameObject curGo;
        GameObject mask;
        GameObject bg;
        GameObject btnParent;
        List<ILObject3DAction> btns;
        GameObject chimneyParent;
        GameObject target1;
        GameObject target2;
        ILObject3DAction exitBtn;
        GameObject dingding;
        GameObject speakMask;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Transform ui3d = curTrans.Find("UI3D");
            mask = ui3d.Find("mask").gameObject;
            speakMask = ui3d.Find("speakMask").gameObject;
            bg = ui3d.Find("bg").gameObject;
            btnParent = ui3d.Find("btn").gameObject;
            btns = new List<ILObject3DAction>();
            for (int i = 0;i < btnParent.transform.childCount;i++)
            {
                btns.Add(btnParent.transform.GetChild(i).GetComponent<ILObject3DAction>());
                btns[i].index = i + 1;
                btns[i].OnMouseDownLua = OnMouseDown;
            }
            ClearBtns();
            chimneyParent = ui3d.Find("chimney").gameObject;
            target1 = chimneyParent.transform.Find("target1").gameObject;
            target2 = chimneyParent.transform.Find("target2").gameObject;
            exitBtn = chimneyParent.transform.Find("exitBtn").GetComponent<ILObject3DAction>();
            exitBtn.OnMouseDownLua = ExitBtn;
            SpineManager.instance.PlayAnimationState(exitBtn.GetComponent<SkeletonAnimation>(),"x_animation");
            dingding = curTrans.Find("dingding").gameObject;
            SpineManager.instance.DoAnimation(bg, "animation");
            mask.SetActive(false);
            Speaking(0);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }
        public void ClearBtns()
        {
            for (int i = 0; i < btns.Count; i++)
            {
                SpineManager.instance.PlayAnimationState(btns[i].GetComponent<SkeletonAnimation>(), (i + 1) + "_animation");
            }
        }
        public void Speaking(int index)
        {
            SoundManager.instance.Speaking(dingding,"talk",SoundManager.SoundType.SOUND,index,() => speakMask.SetActive(true)
            ,() => speakMask.SetActive(false));
        }
        public void OnMouseDown(int index)
        {
            PlayAnimation(index);
        }
        public void ExitBtn(int index)
        {
            exitBtn.gameObject.SetActive(false);
            chimneyParent.transform.localScale = Vector3.zero;
            btnParent.SetActive(true);
        }
        public void PlayAnimation(int index)
        {
            ClearBtns();
            exitBtn.gameObject.SetActive(false);
            btnParent.SetActive(false);
            chimneyParent.transform.localScale = Vector3.one;
            Speaking(index);
            string name1 = "bg" + index + "_idle";
            string name2 = "decorate" + index + "_idle";
            string name3 = "decorate" + index + "_animation";
            mask.SetActive(true);
            target1.transform.localScale = Vector3.one;
            target2.transform.localScale = Vector3.zero;
            SpineManager.instance.PlayAnimationState(target2.GetComponent<SkeletonAnimation>(), name2);
            SpineManager.instance.PlayAnimationState(target1.GetComponent<SkeletonAnimation>(), name1);
            SpineManager.instance.DoAnimation(target1, name1, false, () =>
               {
                   target1.transform.localScale = Vector3.zero;
                   target2.transform.localScale = Vector3.one;
                   SpineManager.instance.DoAnimation(target2, name2, false, () =>
                   {
                       SpineManager.instance.DoAnimation(target2, name3, true);
                       exitBtn.gameObject.SetActive(true);
                       mask.SetActive(false);
                       
                   });
               });


        }
    }
}
