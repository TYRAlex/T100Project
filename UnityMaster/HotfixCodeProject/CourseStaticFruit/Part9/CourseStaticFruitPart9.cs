using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class CourseStaticFruitPart9
    {
        GameObject curGo;
        private Transform Buttom;
        private GameObject Anim;
        private GameObject BtnUpAnim;
        private Transform Btns;
        private Transform Sprites;
        private GameObject CoverMask;
        private GameObject Npc;


        void Start(object o)
        {
            curGo = (GameObject) o;
            Transform curTrans = curGo.transform;
            Buttom = curTrans.Find("Content/Buttom");
            Anim = Buttom.Find("Anim").gameObject;
            BtnUpAnim = Buttom.Find("BtnUpAnim").gameObject;
            Btns = Buttom.Find("Btns");
            Sprites = Buttom.Find("Spirtes");
            CoverMask = Buttom.Find("CoverMask").gameObject;
            Npc = Buttom.Find("Npc").gameObject;
            InitGame();
        }

        void InitGame()
        {
            RegisterBtn();
            Anim.SetActive(false);
            CoverMask.SetActive(false);
            ActiveSprite(2);
            BtnUpAnim.SetActive(true);
            SoundManager.instance.BgSoundPart2();
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SpineManager.instance.DoAnimation(BtnUpAnim, "animation", false, () =>
                {
                    BtnUpAnim.gameObject.SetActive(false);
                    Btns.gameObject.SetActive(true);
                });
            });
        }

        void RegisterBtn()
        {
            BtnUpAnim.gameObject.SetActive(false);
            try
            {
               
                BtnUpAnim.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimations(0);
            }
            catch (Exception e)
            {
                
            }
            Btns.gameObject.SetActive(false);

            for (int i = 0; i < Btns.childCount; i++)
            {
                var btn = Btns.GetChild(i);
                var normal = btn.Find("Normal").gameObject;
                var anim = btn.Find("Anim").gameObject;

                normal.SetActive(true);
                anim.SetActive(false);

                var action = btn.GetComponent<ILObject3DAction>();
                action.index = i;
                action.OnPointDownLua = OnBtnClick;
            }
        }

        private void OnBtnClick(int index)
        {
            var btn = Btns.GetChild(index);
            var normal = btn.Find("Normal").gameObject;
            var anim = btn.Find("Anim").gameObject;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            
            normal.SetActive(false);
            anim.SetActive(true);
            CoverMask.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "ui_" + btn.name, false, () =>
            {
                CoverMask.SetActive(false);
                normal.SetActive(true);
                anim.SetActive(false);
            });
            ActiveSprite(index);
        }

        void ActiveSprite(int index)
        {
            //关闭所有sprite
            for (int i = 0; i < Sprites.childCount; i++)
            {
                var sprite = Sprites.GetChild(i).gameObject;
                sprite.SetActive(false);
            }

            Anim.SetActive(false);
            if (index < 3)
            {
                var activeSprite = Sprites.GetChild(index).gameObject;
                activeSprite.SetActive(true);
            }
            else
            {
                CoverMask.SetActive(true);
                Anim.SetActive(true);
                Anim.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                Anim.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimations(0);
                SpineManager.instance.DoAnimation(Anim, "animation2", false, () =>
                {
                    //Anim.SetActive(false);
                    CoverMask.SetActive(false);
                });
            }
        }
    }
}