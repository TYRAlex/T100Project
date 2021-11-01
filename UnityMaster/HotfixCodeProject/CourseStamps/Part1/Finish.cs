using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CourseStampsPart
{
    class Finish
    {
        Transform parent;
        SkeletonGraphic lizi;
        SkeletonGraphic lizi1;
        SkeletonGraphic light;
        SkeletonGraphic popup;
        Button btn;
        GameObject image;
        Transform mask;

        string[] finishSpine;
        GameObject finishScene;
        GameObject[]  winSpine;

        //enum FINISH_SPINE
        //{
        //    Text1 = 0, Text2 = 1, Text3 = 2, Light = 3, Star = 4
        //}

        public Finish()
        {
            this.parent = CourseStampsPart1.curGo.transform.Find("finish");
            this.mask = CourseStampsPart1.curGo.transform.Find("finish/mask");
            lizi = parent.Find("lizi").GetComponent<SkeletonGraphic>();
            //lizi1 = parent.Find("mask/lizi").GetComponent<SkeletonGraphic>();
            //light = parent.Find("mask/light").GetComponent<SkeletonGraphic>();
            //popup = parent.Find("mask/popup").GetComponent<SkeletonGraphic>();
            SpineManager.instance.PlayAnimationState(lizi, lizi.name);
            //SpineManager.instance.PlayAnimationState(light, light.name);
            //SpineManager.instance.PlayAnimationState(popup, popup.name);
            parent.localScale = Vector3.zero;
            //image = parent.Find("mask/Image").gameObject;
            //btn = parent.Find("mask/btn").GetComponent<Button>();
            //btn.onClick.AddListener(BtnOnClick);
            //btn.gameObject.SetActive(false);
            //finishScene = CourseStampsPart1.curGo.transform.Find("finshScene").gameObject;
           
            winSpine = mask.transform.GetChildren(mask.gameObject);
            finishSpine = new string[] { "1", "2", "3", "4", "2_1", "2_2", "3_1" };
            for (int i = 0; i < winSpine.Length; i++)
            {
                SpineManager.instance.PlayAnimationState(winSpine[i].GetComponent<SkeletonGraphic>(), finishSpine[5]);
            }
            //image.SetActive(false);
        }
        public void BtnOnClick()
        {
            //btn.onClick.RemoveListener(BtnOnClick);
            //MesManager.instance.Dispatch("CourseStampsPart", 1);//发送下一步指令
        }
        public void ShowLizi()
        {
            parent.localScale = Vector3.one;
            mask.localScale = Vector3.zero;
            lizi.transform.localScale = Vector3.one;
            SpineManager.instance.DoAnimation(lizi.gameObject, lizi.name, false,() =>
            {
                SpineManager.instance.PlayAnimationState(lizi, lizi.name);
                MesManager.instance.Dispatch("CourseStampsPart", 1);//发送下一步指令
            });
        }
        public void ShowFinish()
        {
            parent.localScale = Vector3.one;
            mask.localScale = Vector3.one;
            lizi.transform.localScale = Vector3.zero;
            //SpineManager.instance.DoAnimation(lizi1.gameObject, "lizi", false);
            //SpineManager.instance.DoAnimation(light.gameObject, light.name);
            //SpineManager.instance.DoAnimation(popup.gameObject, popup.name, false, () =>
            //       {
            //           image.SetActive(true);
            //           SpineManager.instance.DoAnimation(popup.gameObject, "star_5", false);
            //       });

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE , 0, false);
            SpineManager.instance.DoAnimation(winSpine[1], finishSpine[0], false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                SpineManager.instance.DoAnimation(winSpine[1], finishSpine[1], false, () =>
                {
                    SpineManager.instance.DoAnimation(winSpine[2], finishSpine[2], true);
                    SpineManager.instance.DoAnimation(winSpine[1], finishSpine[3], true);
                });
                SpineManager.instance.DoAnimation(winSpine[0], finishSpine[4], false, () =>
                {
                    SpineManager.instance.DoAnimation(winSpine[0], finishSpine[5], true);
                });                
            });
        }
        public void HideFinish()
        {

        }
    }
}
