using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public class TD6712Part6
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;


        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex; //当前页签索引
        private Vector2 _prePressPos;

        bool isPlaying = false;
        bool isPressBtn = false;

        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();


            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);


            GameInit();
            GameStart();
        }


        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                SetMoveAncPosX(-1);
                isPressBtn = false;
            });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                SetMoveAncPosX(1);
                isPressBtn = false;
            });
        }

        private GameObject tem;
        private GameObject child;


        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, child.name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false);
                    isPlaying = false;
                    isPressBtn = false;
                });
            });
        }

        //点击放大
        private void OnClickShow(GameObject obj)
        {
            
            if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
            {
                SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            }
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            child = obj;
            SpineManager.instance.DoAnimation(tem, obj.name + "1", false,
                () => { btnBack.SetActive(true); });
        }


        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name,
                false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name,
                false);
        }

        void GameStart()
        {
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform()
                .DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() =>
                {
                    callBack?.Invoke();
                    isPlaying = false;
                });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData => { _prePressPos = downData.pressPosition; };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }
    }
}