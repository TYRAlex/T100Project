using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;


namespace ILFramework.HotClass
{
    public class CourseTDKP1L13Part1
    {
        GameObject curGo;
        Transform Buttom;
        Transform RoleBtns;
        Transform lastRoleBtn;
        GameObject CoverMask;
        GameObject Npc;

        //GamePart
        GameObject GamePart;
        GameObject Bgs;
        GameObject Animals;
        GameObject curUseBody;

        Transform UIBarAnims;
        Transform DressBtns;
        Transform lastDressBtn;
        Transform backBtn;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            CoverMask = curTrans.Find("Content/CoverMask").gameObject;
            Buttom = curTrans.Find("Content/Buttom");
            RoleBtns = Buttom.Find("RoleBtns");
            GamePart = Buttom.Find("GamePart").gameObject;
            Npc = Buttom.Find("Npc").gameObject;

            Bgs = GamePart.transform.Find("Bgs").gameObject;
            Animals = GamePart.transform.Find("Animals").gameObject;

            UIBarAnims = GamePart.transform.Find("UIBarAnims");
            DressBtns = GamePart.transform.Find("DressBtns");
            backBtn = GamePart.transform.Find("BackBtn");
            InitGame();

        }

        void InitGame()
        {
            SoundManager.instance.BgSoundPart2();

            RegisterRoleBtn();
            RegisterDressBtn();
            
            CoverMask.SetActive(true);
            SoundManager.instance.Speaking(Npc,"talk",SoundManager.SoundType.VOICE,0,null,()=>{
                CoverMask.SetActive(false);
                Npc.SetActive(true);
                SpineManager.instance.DoAnimation(Npc,"breath");
            });
        }

        private void RegisterDressBtn()
        {
            GamePart.SetActive(false);
            for (int i = 1; i < DressBtns.childCount; i++)
            {
                var btn = DressBtns.GetChild(i);
                btn.Find("Anim").gameObject.SetActive(false);
                btn.Find("Normal").gameObject.SetActive(true);
                var action = btn.GetComponent<ILObject3DAction>();
                action.index = i;
                action.OnPointDownLua = ClickDressBtn;
            }

            backBtn.GetComponent<ILObject3DAction>().OnPointDownLua = (int index) =>
            {
                GamePart.SetActive(false);
                RoleBtns.gameObject.SetActive(true);
            };
        }

        //换装回调
        private void ClickDressBtn(int index)
        {
            //click btn sound
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);

            lastDressBtn = null;
            CommonClickBtnCallBack(DressBtns, lastDressBtn, index, () =>
            {
                int voiceIndex = UnityEngine.Random.Range(1,4);
                SoundManager.instance.Speaking(Npc,"talk",SoundManager.SoundType.VOICE,voiceIndex,null,()=>{
                    Npc.SetActive(true);
                    SpineManager.instance.DoAnimation(Npc,"breath");
                });

                string dressName = DressBtns.GetChild(index).name;
                PlayDressAnim(dressName);
            }, true);
        }

        private void PlayDressAnim(string dressName)
        {
            var curAnimal = curUseBody.transform.parent.parent;
            if (dressName == "magicwand" || dressName == "sword")
            {
                //body anim
                SpineManager.instance.DoAnimation(curUseBody, dressName, false, () =>
                {
                    //idle 
                    SpineManager.instance.DoAnimation(curUseBody, dressName + "_idle");
                });
            }
            else if (dressName == "crown" || dressName == "magichat")
            {
                //hat anim
                var hat = curAnimal.Find("Hat/Default").gameObject;
                hat.SetActive(true);
                SpineManager.instance.DoAnimation(hat, dressName, false);
            }
            else if (dressName == "wing")
            {
                //fly anim
                var fly = curAnimal.Find("Fly/Default").gameObject;
                fly.SetActive(true);
                SpineManager.instance.DoAnimation(fly, dressName, false, () =>
                {
                    SpineManager.instance.DoAnimation(fly, dressName + "_idle");
                });
            }
            else if (dressName == "knight" || dressName == "Princessdress")
            {   
                //clothes anim
                curUseBody.transform.parent.Find("Default").gameObject.SetActive(false);
                curUseBody.transform.parent.Find("Dress").gameObject.SetActive(true);

                curUseBody = curUseBody.transform.parent.Find("Dress").gameObject;
                var skt = curUseBody.GetComponent<SkeletonAnimation>();
                skt.AnimationState.SetEmptyAnimation(0, 0);
                SpineManager.instance.DoAnimation(curUseBody, "idle");
            }
            else if (dressName == "cover")
            {
                //cover anim
                var cover = curAnimal.Find("Cover/Default").gameObject;
                cover.SetActive(true);
                SpineManager.instance.DoAnimation(cover, dressName, false, () =>
                {
                    SpineManager.instance.DoAnimation(cover, dressName + "_idle");
                });
            }
        }

        private void RegisterRoleBtn()
        {
            RoleBtns.gameObject.SetActive(true);
            for (int i = 1; i < RoleBtns.childCount; i++)
            {
                var btn = RoleBtns.GetChild(i);
                btn.Find("Anim").gameObject.SetActive(false);
                btn.Find("Normal").gameObject.SetActive(true);
                var action = btn.GetComponent<ILObject3DAction>();
                action.index = i;
                action.OnPointDownLua = ClickRoleBtn;
            }
        }

        private void ClickRoleBtn(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);
            lastRoleBtn = null;
            CommonClickBtnCallBack(RoleBtns, lastRoleBtn, index, () =>
            {
                RoleBtns.gameObject.SetActive(false);
                OpenGamePart1(RoleBtns.GetChild(index).name);
            });
        }

        void CommonClickBtnCallBack(Transform btnParent, Transform lastClickBtn, int curClickIndex, Action animEndCallBack, bool isDress = false)
        {
            if (lastClickBtn != null)
            {
                lastClickBtn.Find("Anim").gameObject.SetActive(false);
                lastClickBtn.Find("Normal").gameObject.SetActive(true);
            }

            var btn = btnParent.GetChild(curClickIndex).gameObject;
            var animObj = btnParent.GetChild(curClickIndex).Find("Anim").gameObject;
            var normalObj = btnParent.GetChild(curClickIndex).Find("Normal").gameObject;
            lastClickBtn = btn.transform;
            animObj.SetActive(true);
            normalObj.SetActive(false);
            CoverMask.SetActive(true);

            string animName = isDress ? "ui_" + btn.name : btn.name;

            SpineManager.instance.DoAnimation(animObj, animName, false, () =>
            {
                animObj.SetActive(false);
                normalObj.SetActive(true);
                CoverMask.SetActive(false);

                animEndCallBack.Invoke();
            });
        }



        void OpenGamePart1(string animalName)
        {
            ResetGamePart1(animalName);
            var lightAnim = Bgs.transform.Find("Light").gameObject;
            var starAnim = Bgs.transform.Find("Star").gameObject;
            SpineManager.instance.DoAnimation(lightAnim, "light", false, () =>
            {
                starAnim.SetActive(true);
                SpineManager.instance.DoAnimation(starAnim, "star");
                AnimalDefaultIdle(animalName);
            });
            PlayUIBarAnim(animalName);
        }


        void ResetGamePart1(string animalName)
        {
            GamePart.SetActive(true);
            Bgs.SetActive(true);
            Animals.SetActive(true);

            for (int i = 0; i < Animals.transform.childCount; i++)
            {
                var curAnimal = Animals.transform.GetChild(i);
                curAnimal.gameObject.SetActive(false);
            }

            Bgs.transform.Find("Light").gameObject.SetActive(true);
            Bgs.transform.Find("Star").gameObject.SetActive(false);

            UIBarAnims.Find("bunnyAnim").gameObject.SetActive(false);
            UIBarAnims.Find("dinosaurAnim").gameObject.SetActive(false);

            InitAnimal("dinosaur");
            InitAnimal("bunny");

            ResetDressBtns(animalName);
        }

        void ResetDressBtns(string animalName)
        {
            DressBtns.gameObject.SetActive(false);
            for (int i = 1; i < DressBtns.childCount; i++)
            {
                var btn = DressBtns.GetChild(i);
                btn.Find("Anim").gameObject.SetActive(false);
                btn.Find("Normal").gameObject.SetActive(true);
                btn.gameObject.SetActive(false);
            }

            bool isKinght = animalName == "dinosaur" ? true : false;
            for (int i = 1; i < DressBtns.childCount; i++)
            {
                var btn = DressBtns.GetChild(i);
                if (btn.name != "knight" && btn.name != "Princessdress")
                {
                    btn.gameObject.SetActive(true);
                }
                else
                {
                    if (isKinght && btn.name == "knight")
                    {
                        btn.gameObject.SetActive(true);
                    }
                    else if (!isKinght && btn.name == "Princessdress")
                    {
                        btn.gameObject.SetActive(true);
                    }
                }
            }
        }

        void InitAnimal(string animalName)
        {
            var animal = Animals.transform.Find(animalName).gameObject;
            animal.transform.Find("Body/Default").gameObject.SetActive(true);
            animal.transform.Find("Body/Dress").gameObject.SetActive(false);
            animal.transform.Find("Fly/Default").gameObject.SetActive(false);
            animal.transform.Find("Hat/Default").gameObject.SetActive(false);
            animal.transform.Find("Cover/Default").gameObject.SetActive(false);
        }


        void AnimalDefaultIdle(string animalName)
        {
            //active animal
            var animalObj = Animals.transform.Find(animalName).gameObject;
            animalObj.SetActive(true);
            //play idle animation 
            curUseBody = animalObj.transform.Find("Body/Default").gameObject;
            var skt = curUseBody.GetComponent<SkeletonAnimation>();
            skt.AnimationState.SetEmptyAnimation(0, 0);
            SpineManager.instance.DoAnimation(curUseBody, "idle");
        }

        void PlayUIBarAnim(string animalName)
        {
            var animObj = UIBarAnims.Find(animalName + "Anim").gameObject;
            animObj.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1);
            SpineManager.instance.DoAnimation(animObj, "UI_" + animalName, false, () =>
            {
                animObj.SetActive(false);
                DressBtns.gameObject.SetActive(true);
            });
        }

    }
}
