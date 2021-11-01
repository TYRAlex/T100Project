using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDKP1L12Part1
    {
        GameObject curGo;
        Transform Dogs;
        Transform Btns;
        GameObject Npc;
        Transform Buttom;
        GameObject LightAnim;

        GameObject NormalDog;
        GameObject CoverMask;
        string[] DogAnims;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");

            Btns = Buttom.Find("Btns");
            Dogs = Buttom.Find("Dogs");
            Npc = curTrans.Find("Content/Npc").gameObject;
            CoverMask = curTrans.Find("Content/CoverMask").gameObject;
            LightAnim = Buttom.Find("LightAnim").gameObject;


            DogAnims = new string[] { "1walk", "2ear", "3lanyao", "4tongue", "5paotu", "6paxia", "7tail", "8shake head" };

            InitGame();
        }

        void InitGame()
        {
            InitDogs();
            InitBtns();
            CoverMask.SetActive(true);
            //播放BGM
            SoundManager.instance.BgSoundPart2();
            LightAnim.SetActive(false);

            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                Npc.SetActive(true);
                //npc idle
                SpineManager.instance.DoAnimation(Npc, "breath");
                CoverMask.SetActive(false);

                NormalDog = Dogs.GetChild(0).gameObject;
                NormalDog.SetActive(true);
                SpineManager.instance.DoAnimation(NormalDog, "idle");
            });
        }


        void InitDogs()
        {
            CloseAllDogs();

        }

        void CloseAllDogs()
        {
            for (int i = 0; i < Dogs.childCount; i++)
            {
                var dog = Dogs.GetChild(i).gameObject;
                dog.SetActive(false);
            }
        }

        void InitBtns()
        {
            ResetAllBtns();
            for (int i = 0; i < Btns.childCount; i++)
            {
                var btn = Btns.GetChild(i);
                //注册回调
                if (i < Btns.childCount - 1)
                {
                    //皮肤按钮
                    var action = btn.GetComponent<ILObject3DAction>();
                    action.OnPointDownLua = SkinCallback;
                    action.index = i + 1;

                }
                else
                {
                    //ok按钮
                    var action = btn.GetComponent<ILObject3DAction>();
                    action.OnPointDownLua = OKCallback;
                    action.index = i + 1;
                }

            }
        }

        void ResetAllBtns()
        {
            for (int i = 0; i < Btns.childCount; i++)
            {
                var btn = Btns.GetChild(i);
                var normal = btn.Find("Normal").gameObject;
                var anim = btn.Find("Anim").gameObject;
                normal.SetActive(true);
                anim.SetActive(false);
            }
        }

        private void OKCallback(int index)
        {
            Debug.Log("点击了OK按钮");
            //按钮点击音效
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 4, null, () =>
            {
                Npc.SetActive(true);
                SpineManager.instance.DoAnimation(Npc, "breath");
            });

            CoverMask.SetActive(true);
            CloseAllDogs();
            NormalDog.SetActive(true);
            SpineManager.instance.DoAnimation(NormalDog, "animation");

            LightAnim.SetActive(true);
            SpineManager.instance.DoAnimation(LightAnim,"animation",false,()=>{
                SpineManager.instance.DoAnimation(LightAnim,"idle");
            });
        }

        private void SkinCallback(int index)
        {
            //按钮点击音效
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            var SoundIndex = UnityEngine.Random.Range(1, 4);
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, SoundIndex, null, () =>
            {
                Npc.SetActive(true);
                SpineManager.instance.DoAnimation(Npc, "breath");
            });

            ResetAllBtns();

            var btn = Btns.GetChild(index - 1);
            var normal = btn.Find("Normal").gameObject;
            var anim = btn.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "ui_" + index, false);

            CloseAllDogs();
            var dog = Dogs.GetChild(index).gameObject;
            dog.SetActive(true);
            Debug.Log("SkinIndex:--" + index);
            SpineManager.instance.DoAnimation(dog, DogAnims[index - 1]);
        }
    }
}
