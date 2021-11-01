using System;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace ILFramework.HotClass
{
    public class CourseTDG4P1L10Part1
    {
        GameObject curGo;
        Transform Buttom;
        GameObject BG;
        Transform Btns;
        GameObject BtnUpAnim;
        GameObject Npc;
        GameObject Finger;
        GameObject Covermask;
        int lastBtnIndex;
        int curbtnIndex;
        Transform HandAndPage;
        GameObject Touchmask;
        GameObject OpenAnim;
        GameObject ShowPages;
        GameObject OpenShowPageBtn;
        GameObject CloseShowPageBtn;
        string[] showpageAnims;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            showpageAnims = new string[] { "ag1", "ag2", "ag3", "ag4", "ag5" };

            Buttom = curTrans.Find("Content/Buttom");
            BG = Buttom.Find("BG").gameObject;
            Finger = Buttom.Find("Finger").gameObject;
            Btns = Buttom.Find("Btns");
            BtnUpAnim = Buttom.Find("BtnUpAnim").gameObject;
            Npc = curTrans.Find("Content/Npc").gameObject;
            Covermask = curTrans.Find("Content/Covermask").gameObject;
            Touchmask = curTrans.Find("Content/TouchMask").gameObject;
            HandAndPage = Buttom.Find("HandAndPage");
            OpenAnim = Buttom.Find("OpenAnim").gameObject;
            ShowPages = Buttom.Find("ShowPages").gameObject;
            OpenShowPageBtn = Buttom.Find("OpenShowPageBtn").gameObject;
            CloseShowPageBtn = Buttom.Find("CloseShowPageBtn").gameObject;

            InitGame();
        }

        void InitGame()
        {
            lastBtnIndex = 0;
            curbtnIndex = 0;
            Covermask.SetActive(false);
            Touchmask.SetActive(true);
            ResetHandAndPage();
            ResgisterBtns();
            BtnUpAnim.SetActive(false);
            Btns.gameObject.SetActive(false);
            OpenAnim.SetActive(false);
            ShowPages.SetActive(false);
            HandAndPage.gameObject.SetActive(true);

            OpenShowPageBtn.GetComponent<ILObject3DAction>().OnPointDownLua = OpenShowPage;
            OpenShowPageBtn.SetActive(false);

            CloseShowPageBtn.GetComponent<ILObject3DAction>().OnPointDownLua = CloseShowPage;
            CloseShowPageBtn.SetActive(false);

            SpineManager.instance.DoAnimation(BG, "animation");

            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM, 0.4f);
            StartGame();
        }

        private void CloseShowPage(int obj)
        {
            Covermask.SetActive(false);
            var normal = CloseShowPageBtn.transform.Find("Normal").gameObject;
            var anim = CloseShowPageBtn.transform.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);
            OpenShowPageBtn.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "ui_back", false, () =>
            {
                normal.SetActive(true);
                anim.SetActive(false);
                CloseShowPageBtn.SetActive(false);

                //reset Showpage
                ShowPages.transform.Find("Page").gameObject.SetActive(false);
                ShowPages.transform.Find("Yunran").gameObject.SetActive(false);
                ShowPages.SetActive(false);
                HandAndPage.gameObject.SetActive(true);
            });
        }

        private void OpenShowPage(int obj)
        {
            Covermask.SetActive(true);
            HandAndPage.gameObject.SetActive(false);
            PlayShowPageAnim(curbtnIndex);
        }

        void StartGame()
        {
            Npc.SetActive(false);
            OpenAnim.SetActive(true);
            SkeletonAnimation ske = OpenAnim.GetComponent<SkeletonAnimation>();
            if (ske != null)
            {
                ske.AnimationState.SetEmptyAnimation(0, 0);
            }
            SpineManager.instance.DoAnimation(OpenAnim, "animation", false, () =>
            {
                //npc talking
                SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    OpenAnim.SetActive(false);
                    Touchmask.SetActive(false);
                    // Npc.SetActive(true);
                    // SpineManager.instance.DoAnimation(Npc, "breath");

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    BtnUpAnim.SetActive(true);
                    SpineManager.instance.DoAnimation(BtnUpAnim, "ui_up", false, () =>
                    {
                        Btns.gameObject.SetActive(true);
                        BtnUpAnim.SetActive(false);
                    });
                });
            });
        }

        void ResgisterBtns()
        {
            for (int i = 0; i < Btns.childCount; i++)
            {
                var btn = Btns.GetChild(i).gameObject;
                var action = btn.GetComponent<ILObject3DAction>();
                action.index = i;
                action.OnPointDownLua = BtnCallBack;
                var normal = btn.transform.Find("Normal").gameObject;
                var anim = btn.transform.Find("Anim").gameObject;
                normal.SetActive(true);
                anim.SetActive(false);
            }
        }

        private void BtnCallBack(int index)
        {
            //old btn
            var lastBtn = Btns.GetChild(lastBtnIndex);
            var normal_o = lastBtn.Find("Normal").gameObject;
            var anim_o = lastBtn.Find("Anim").gameObject;
            normal_o.SetActive(true);
            anim_o.SetActive(false);
            Finger.SetActive(false);

            lastBtnIndex = index;
            curbtnIndex = index;
            //new btn
            var btn = Btns.GetChild(index);
            var normal = btn.Find("Normal").gameObject;
            var anim = btn.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);


            Covermask.SetActive(true);
            ResetHandAndPage();

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

            Covermask.SetActive(false);
            PlayHandAndPageAnim(index);
            SpineManager.instance.DoAnimation(anim, "ag" + (index + 1).ToString(), false, () =>
            {

                SpineManager.instance.DoAnimation(anim, "ag" + (index + 1).ToString() + "_idle", false, () =>
                {
                    SpineManager.instance.DoAnimation(anim, "ag" + (index + 1).ToString() + "_idle");

                });
            });
        }

        void ResetHandAndPage()
        {
            for (int i = 0; i < HandAndPage.childCount; i++)
            {
                var brushAnim = HandAndPage.GetChild(i).Find("BrushAnim").gameObject;
                var pageAnim = HandAndPage.GetChild(i).Find("PageAnim").gameObject;
                brushAnim.SetActive(false);
                pageAnim.SetActive(false);
            }
        }

        void PlayHandAndPageAnim(int index)
        {
            var Fobj = HandAndPage.GetChild(index);
            var BrushAnim = Fobj.Find("BrushAnim").gameObject;
            var PageAnim = Fobj.Find("PageAnim").gameObject;
            BrushAnim.SetActive(true);
            PageAnim.SetActive(true);

            try
            {
                var sk1 = BrushAnim.GetComponent<SkeletonAnimation>();
                var sk2 = PageAnim.GetComponent<SkeletonAnimation>();
                sk1.AnimationState.SetEmptyAnimation(0, 0);
                sk2.AnimationState.SetEmptyAnimation(0, 0);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }


            SpineManager.instance.DoAnimation(BrushAnim, "ag" + (index + 1).ToString());
            SpineManager.instance.DoAnimation(PageAnim, "ag" + (index + 1).ToString() + "_1", false, () =>
            {
                Finger.SetActive(true);
                SpineManager.instance.DoAnimation(Finger, "animation");
                SpineManager.instance.DoAnimation(PageAnim, "ag" + (index + 1).ToString() + "_1", true, () =>
                 {
                     Finger.SetActive(true);
                     SpineManager.instance.DoAnimation(Finger, "animation");
                 });
            });
            OpenShowPageBtn.SetActive(true);
        }

        void PlayShowPageAnim(int index)
        {
            OpenShowPageBtn.SetActive(false);
            Finger.SetActive(false);
            ShowPages.SetActive(true);

            var bg = ShowPages.transform.Find("BG").gameObject;
            var ZGJ = ShowPages.transform.Find("ZGJ").gameObject;
            var page = ShowPages.transform.Find("Page").gameObject;
            var yunran = ShowPages.transform.Find("Yunran").gameObject;

            SpineManager.instance.DoAnimation(ZGJ, "zhongguojie");

            SpineManager.instance.DoAnimation(bg, "diban", false, () =>
            {
                page.SetActive(true);

                //true show
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SkeletonAnimation ske = page.GetComponent<SkeletonAnimation>();
                ske.AnimationState.ClearTrack(0);
                var track = ske.AnimationState.SetAnimation(0, showpageAnims[index], false);
                track.TrackTime = (track.TrackEnd - 5f)/30f;
                yunran.SetActive(true);
                SpineManager.instance.DoAnimation(yunran, "yunran");
                yunran.GetComponent<SkeletonAnimation>().AnimationState.ClearTrack(0);
                var track2 =  yunran.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "yunran", true);;
                track2.TrackTime = 0f;
               

                Debug.Log("显示出关闭按钮");
                CloseShowPageBtn.SetActive(true);
                // track.Complete += (TrackEntry trackEntry) =>
                // {
                //     yunran.SetActive(true);
                //     SpineManager.instance.DoAnimation(yunran, "yunran");
                //     yunran.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0,"yunran",true);

                //     Debug.Log("显示出关闭按钮");
                //     CloseShowPageBtn.SetActive(true);
                // };

                // SpineManager.instance.DoAnimation(page, showpageAnims[index], false, () =>
                // {
                //     ske.AnimationState.ClearTracks();
                //     ske.AnimationState.SetEmptyAnimation(0, 0);
                //     yunran.SetActive(true);
                //     SpineManager.instance.DoAnimation(yunran, "yunran");

                //     Debug.Log("显示出关闭按钮");
                //     CloseShowPageBtn.SetActive(true);
                // });

            });

        }

    }
}
