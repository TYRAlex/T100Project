using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course211Part2
    {

        private GameObject specker;
        private GameObject speckerSpine;
        private GameObject startBtn;
        private GameObject leftBtn;
        private GameObject rightBtn;
        private GameObject bookBg;
        private GameObject leftPageObj;
        private GameObject rightPageObj;
        private GameObject bookIdle;
        private GameObject bookTurn;

        private bool isPageClip;//是否有页面语音
        private bool isPageAni;//是否有页面Spine动画
        private bool isEndClip;//是否有结束语音
        private bool isBookClip;//是否打开音效

        private int startClip;//开始语音下标
        private int endClip;//结束语音下标
        private int[] pageClip;//页面内语音

        private int startBookClip;//开书音效
        private int bookClip;//翻书音效
        private int endBookClip;//关书音效

        private string speckerShuosho;//角色说话动画
        private string speckerDaiji;//角色待机动画
        private string startAni;//开书动画
        private string daijiAni;//书本待机动画
        private string[] leftPage;//左边静态页面
        private string[] rightPage;//右边静态页面
        private string[] leftDownAni;//左边翻页动画
        private string[] rightDownAni;//右边翻页动画
        private string[] pageAni;//页面内动画

        private int talkIndex;
        private int currentPage;//当前页面
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            specker = curTrans.Find("Specker").gameObject;
            speckerSpine = curTrans.Find("Specker/SpeckerSpine").gameObject;

            startBtn = curTrans.Find("ImgBtn/startBtn").gameObject;
            leftBtn = curTrans.Find("ImgBtn/leftBtn").gameObject;
            rightBtn = curTrans.Find("ImgBtn/rightBtn").gameObject;

            bookBg = curTrans.Find("bookBg/bg").gameObject;
            leftPageObj = curTrans.Find("booksAni/leftPage").gameObject;
            rightPageObj = curTrans.Find("booksAni/rightPage").gameObject;
            bookIdle = curTrans.Find("booksAni/bookIdle").gameObject;
            bookTurn = curTrans.Find("booksAni/bookTurn").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            currentPage = 0;

            isPageClip = true;
            isPageAni = true;
            isEndClip = false;
            isBookClip = true;

            startBookClip = 0;//开书音效
            bookClip = 1;//翻书音效
            endBookClip = 2;//关书音效

            startClip = 0;
            endClip = -1;
            int[] pageClip_test = {1,2,3,4 };
            pageClip = pageClip_test;

            speckerShuosho = "DAIJIshuohua";
            speckerDaiji = "DAIJI";
            startAni = "dakai";
            daijiAni = "fengmian";
            string[] leftPage_test = { "1", "3", "5", "7" };
            leftPage = leftPage_test;
            string[] rightPage_test = { "2", "4", "6", "8" };
            rightPage = rightPage_test;
            string[] leftDownAni_test = { "guanbi", "b1", "b2", "b3" };
            leftDownAni = leftDownAni_test;
            string[] rightDownAni_test = { "a1", "a2", "a3", "guanbi2" };
            rightDownAni = rightDownAni_test;
            string[] pageAni_test = { "g1", "g2", "g3", "g4" };
            pageAni = pageAni_test;

            startBtn.SetActive(false);
            leftBtn.SetActive(false);
            rightBtn.SetActive(false);
            InitPage(false);
            bookTurn.SetActive(true);
            SpineManager.instance.DoAnimation(bookTurn, "fengmian", false);
            Util.AddBtnClick(startBtn, DoStartBtnClick);
            Util.AddBtnClick(leftBtn, DoLeftBtnClick);
            Util.AddBtnClick(rightBtn, DoRightBtnClick);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void InitPage(bool state)
        {            
            leftPageObj.SetActive(state);
            rightPageObj.SetActive(state);
            bookIdle.SetActive(state);
            bookBg.SetActive(state);
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, startClip, () => { }, () => {
                startBtn.SetActive(true);
            }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        //打开书本
        void DoStartBtnClick(GameObject obj)
        {
            currentPage = 0;
            startBtn.SetActive(false);
            mono.StartCoroutine(StartBtnCoroutine());
        }

        IEnumerator StartBtnCoroutine()
        {
            if (isBookClip) SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, startBookClip, false);
            float spiTime = SpineManager.instance.DoAnimation(bookTurn, startAni, false);
            yield return new WaitForSeconds(spiTime-0.05f);
            InitPage(true);
            SpineManager.instance.DoAnimation(leftPageObj, leftPage[currentPage], false);
            SpineManager.instance.DoAnimation(rightPageObj, rightPage[currentPage], false);
            yield return new WaitForSeconds(0.05f);
            bookTurn.SetActive(false);
            if (isPageAni) SpineManager.instance.DoAnimation(bookIdle, pageAni[0], false);
            if (isPageClip)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, pageClip[0], () => { }, () => {
                    leftBtn.SetActive(true);
                    rightBtn.SetActive(true);
                }));
            }else
            {
                leftBtn.SetActive(true);
                rightBtn.SetActive(true);
            }
        }

        //左边按钮点击事件
        void DoLeftBtnClick(GameObject obj)
        {
            leftBtn.SetActive(false);
            rightBtn.SetActive(false);
            BtnPlaySound();
            mono.StartCoroutine(LeftBtnCoroutine());
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        IEnumerator LeftBtnCoroutine()
        {
            bookTurn.SetActive(true);
            if (currentPage == 0 && isBookClip) SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, endBookClip, false);
            else SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, bookClip, false);
            float spiTime = SpineManager.instance.DoAnimation(bookTurn, leftDownAni[currentPage], false);
            yield return new WaitForSeconds(0.05f);
            if (currentPage == 0) InitPage(false);
            else SpineManager.instance.DoAnimation(leftPageObj, leftPage[currentPage - 1], false);
            yield return new WaitForSeconds(spiTime-0.1f);
            if (currentPage != 0)
            {
                SpineManager.instance.DoAnimation(rightPageObj, rightPage[currentPage - 1], false);
                yield return new WaitForSeconds(0.05f);
                bookTurn.SetActive(false);
                if (isPageAni) SpineManager.instance.DoAnimation(bookIdle, pageAni[currentPage - 1], false);
                if (isPageClip)
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, pageClip[currentPage - 1], () => { }, () =>
                    {
                        leftBtn.SetActive(true);
                        rightBtn.SetActive(true);
                    }));
                }
                else
                {
                    leftBtn.SetActive(true);
                    rightBtn.SetActive(true);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
                startBtn.SetActive(true);
            }
            currentPage--;
        }

        //右边按钮点击事件
        void DoRightBtnClick(GameObject obj)
        {
            leftBtn.SetActive(false);
            rightBtn.SetActive(false);
            BtnPlaySound();
            mono.StartCoroutine(RightBtnCoroutine());
        }

        IEnumerator RightBtnCoroutine()
        {
            bookTurn.SetActive(true);
            if (currentPage == 3 && isBookClip) SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, endBookClip, false);
            else SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, bookClip, false);
            float spiTime = SpineManager.instance.DoAnimation(bookTurn, rightDownAni[currentPage], false);
            yield return new WaitForSeconds(0.05f);
            if (currentPage == 3) InitPage(false);
            else SpineManager.instance.DoAnimation(rightPageObj, rightPage[currentPage + 1], false);
            yield return new WaitForSeconds(spiTime - 0.1f);
            if (currentPage != 3)
            {
                SpineManager.instance.DoAnimation(leftPageObj, leftPage[currentPage + 1], false);
                yield return new WaitForSeconds(0.05f);
                bookTurn.SetActive(false);
                if (isPageAni) SpineManager.instance.DoAnimation(bookIdle, pageAni[currentPage + 1], false);
                if (isPageClip)
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, pageClip[currentPage + 1], () => { }, () =>
                    {
                        leftBtn.SetActive(true);
                        rightBtn.SetActive(true);
                    }));
                }
                else
                {
                    leftBtn.SetActive(true);
                    rightBtn.SetActive(true);
                }
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
                startBtn.SetActive(true);
            }
            currentPage++;
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(speckerSpine, speckerShuosho);
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(speckerSpine, speckerDaiji);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            talkIndex++;
        }
    }
}
