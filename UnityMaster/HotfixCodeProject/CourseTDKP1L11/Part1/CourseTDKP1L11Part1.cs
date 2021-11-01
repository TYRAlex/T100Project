using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

namespace ILFramework.HotClass
{
    //调料type
    public enum SeasoningType
    {
        Brown = 1, Blue = 2, Pink = 3, Fruit = 4, Candy = 5, Chocolate = 6
    }

    public class CourseTDKP1L11Part1
    {
        GameObject curGo;
        Transform Buttom;
        GameObject Hand;
        GameObject Hand2;
        GameObject CakeParfeb;
        GameObject Cake;
        Transform CakeParent;

        GameObject Seasoning;
        GameObject Npc;
        Transform SeasoningBtns;
        GameObject OkBtn;
        GameObject FrontMask;
        GameObject Mask;
        GameObject StarAnim;
        SeasoningType[] seasonings;
        Dictionary<SeasoningType,string> dict;


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");
            Npc = curTrans.Find("Content/Npc").gameObject;
            Mask = curTrans.Find("Content/Mask").gameObject;
            FrontMask = Buttom.Find("BG/mask").gameObject;
            StarAnim = Buttom.Find("StarAnim").gameObject;
            Hand = Buttom.Find("Hand").gameObject;
            Hand2 = Buttom.Find("Hand2").gameObject;

            CakeParfeb = Buttom.Find("Cake").gameObject;
            CakeParent = Buttom.Find("CakePartent");

            Seasoning = Buttom.Find("Seasoning").gameObject;
            SeasoningBtns = Buttom.Find("SeasoningBtns");
            OkBtn = Buttom.Find("OkBtn").gameObject;
            dict = new Dictionary<SeasoningType, string>();
            dict.Add(SeasoningType.Brown,"1");
            dict.Add(SeasoningType.Blue,"2");
            dict.Add(SeasoningType.Pink,"3");
            dict.Add(SeasoningType.Fruit,"4");
            dict.Add(SeasoningType.Candy,"5");
            dict.Add(SeasoningType.Chocolate,"6");

            InitGame();
        }

        void InitGame()
        {
            ResetHandAndCake();
            RegisterBtnsCallback();
            Mask.SetActive(true);
            FrontMask.SetActive(false);
            StarAnim.SetActive(false);
            SoundManager.instance.BgSoundPart2();

            if (CakeParent.childCount > 0)
            {
                GameObject.Destroy(CakeParent.GetChild(0).gameObject);
            }


            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                //Npc.SetActive(true);
                Mask.SetActive(false);
                //SpineManager.instance.DoAnimation(Npc, "breath");
                StartGame();
            });
        }

        void ResetGame()
        {
            ResetHandAndCake();
            RegisterBtnsCallback();
            Mask.SetActive(true);
            FrontMask.SetActive(false);
            StarAnim.SetActive(false);
            SoundManager.instance.BgSoundPart2();
            //Npc.SetActive(true);
            //SpineManager.instance.DoAnimation(Npc, "breath");
            StartGame();
        }

        void StartGame()
        {
            FrontMask.SetActive(true);
            Hand.SetActive(true);
            Hand2.SetActive(false);

            Mask.SetActive(true);
            SpineManager.instance.DoAnimation(Hand, "animation", false, () =>
            {
              
                Cake = GameObject.Instantiate(CakeParfeb, Vector3.zero, Quaternion.identity);
                Cake.transform.SetParent(CakeParent);
                Cake.transform.localPosition = CakeParfeb.transform.localPosition;

                Cake.SetActive(true);
                SpineManager.instance.DoAnimation(Cake, "cake", false);

                var skt = Hand.GetComponent<SkeletonGraphic>();
                skt.AnimationState.ClearTracks();
                skt.AnimationState.SetEmptyAnimations(0);
                SpineManager.instance.DoAnimation(Hand, "animation2", false, () =>
                {
                   
                   
                    Mask.SetActive(false);
                    Hand.SetActive(false);
                });
            });
        }

        void ResetHandAndCake()
        {
            seasonings = new SeasoningType[] { 0, 0 };
            Hand.SetActive(false);
            GameObject.Destroy(Cake);
            Cake = null;
            Seasoning.SetActive(false);
        }

        void RegisterBtnsCallback()
        {
            for (int i = 0; i < SeasoningBtns.childCount; i++)
            {
                var action = SeasoningBtns.GetChild(i).GetComponent<ILObject3DAction>();
                action.index = i;
                action.OnPointDownLua = SeasoningBtnDown;
                action.OnPointUpLua = SeasoningBtnUp;
            }
            var okAction = OkBtn.GetComponent<ILObject3DAction>();
            okAction.OnPointDownLua = OkBtnDown;
            okAction.OnPointUpLua = OkBtnUp;
        }

        //按钮回调部分
        private void SeasoningBtnUp(int index)
        {
            var btn = SeasoningBtns.GetChild(index);
            btn.GetComponent<Image>().enabled = true;
            var anim = btn.Find("Anim").gameObject;
            anim.SetActive(false);
        }

        private void SeasoningBtnDown(int index)
        {
            var btn = SeasoningBtns.GetChild(index);
            string btnName = btn.name.ToLower();
            btn.GetComponent<Image>().enabled = false;
            var anim = btn.Find("Anim").gameObject;
            anim.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(anim, btnName + "_s", false);

            Mask.SetActive(true);
            GetSeasoning(index);
        }

        private void OkBtnUp(int index)
        {
            var btn = Buttom.Find("OkBtn");
            btn.GetComponent<Image>().enabled = true;
            var anim = btn.Find("Anim").gameObject;
            anim.SetActive(false);
        }

        private void OkBtnDown(int index)
        {
            Debug.Log("按下OK");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            var btn = Buttom.Find("OkBtn");
            btn.GetComponent<Image>().enabled = false;
            var anim = btn.Find("Anim").gameObject;
            anim.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "button_ok", false);
            //取走cake
            TakeOutCake();
        }

        void GetSeasoning(int index)
        {

            var seasoningName = SeasoningBtns.GetChild(index).name;
            SeasoningType seasoning = (SeasoningType)(index + 1);
            Seasoning.SetActive(true);
            //固体调料
            if (index >= 3 && index <= 5)
            {
                seasonings[0] = seasoning;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                SpineManager.instance.DoAnimation(Seasoning, seasoningName.ToLower(), false, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                    SetCake(seasonings);
                    Mask.SetActive(false);
                });
            }
            else if (index >= 0 && index <= 2)
            {
                //液体调料
                seasonings[1] = seasoning;
                if (seasoningName.Equals("Brown"))
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    SpineManager.instance.DoAnimation(Seasoning, "bottle_chocolate", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        Seasoning.SetActive(true);
                        SetCake(seasonings);
                        Mask.SetActive(false);
                    });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    SpineManager.instance.DoAnimation(Seasoning, "bottle_" + seasoningName.ToLower(), false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        Seasoning.SetActive(true);
                        SetCake(seasonings);
                        Mask.SetActive(false);
                    });
                }
            }
        }

        void SetCake(SeasoningType[] seasoingArray)
        {
            //混合调料
            if (seasoingArray[0] != 0 && seasoingArray[1] != 0)
            {
                SpineManager.instance.DoAnimation(Cake, ((int)seasoingArray[1]) + "+" + ((int)seasoingArray[0]), false);
            }
            else if (seasoingArray[0] != 0 && seasoingArray[1] == 0)
            {
                //单独固体调料
                SpineManager.instance.DoAnimation(Cake, ((int)seasoingArray[0]).ToString(), false);
            }
            else if (seasoingArray[0] == 0 && seasoingArray[1] != 0)
            {
                //单独液体调料
                SpineManager.instance.DoAnimation(Cake, ((int)seasoingArray[1]).ToString(), false);
            }
        }

        void TakeOutCake()
        {

            if (seasonings[0] != 0 && seasonings[1] != 0)
            {
                Debug.Log("拿走cake");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
                StarAnim.SetActive(true);
                SpineManager.instance.DoAnimation(StarAnim, "animation", false, () =>
                {
                    Cake.SetActive(false);
                    Hand2.SetActive(true);
                    Hand2.GetComponent<SkeletonGraphic>().AnimationState.ClearTracks();
                    Hand2.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimations(0);
                    string animName = dict[seasonings[1]]+ "+" + dict[seasonings[0]];
                    SpineManager.instance.DoAnimation(Hand2, animName, false, () =>
                    {
                        Debug.Log(animName);
                        Hand2.SetActive(false);
                        ResetGame();
                    });
                });
            }
            else
            {
                Debug.Log("拿不走cake");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
            }

        }

    }
}
