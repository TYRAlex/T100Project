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

    public class TD3413Part7
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
       
        private GameObject Bg;



        private Transform SpinePage;

        private GameObject rightBtn;
        private GameObject leftBtn;

     

        
        

        private GameObject _pageBar;
       

        private Vector2 _prePressPos;

        private Empty4Raycast[] e4rs;
        bool isPressBtn = false;

        bool isPlaying = false;
        private bool ispressBack = false;
        private int curPageIndex; //当前页签索引

        
        private GameObject btnBack;
        
        //胜利动画名字


        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;

            btnBack = curTrans.GetGameObject("btnBack");
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            _pageBar = curTrans.GetGameObject("PageBar");
            _pageBar.Show();
            SpinePage = _pageBar.transform.GetTransform("MaskImg/SpinePage");
            e4rs = SpinePage.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;
            
            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);
            leftBtn = leftBtn.transform.parent.gameObject;
            rightBtn = rightBtn.transform.parent.gameObject;
            leftBtn.Show();
            rightBtn.Show();
            SpineManager.instance.DoAnimation(leftBtn, "L2", false);
            SpineManager.instance.DoAnimation(rightBtn, "R2", false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            
            GameInit();
            GameStart();
            SlideSwitchPage(_pageBar);
        }

        private GameObject tem;

        void OnClickShow(GameObject obj)
        {

            if (isPressBtn || !isPlaying)
                return;
            isPressBtn = true;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => { btnBack.SetActive(true); });
            
         
        }


        private void OnClickBtnBack(GameObject obj)
        {
            if (ispressBack)
                return;
            ispressBack = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false);
                    isPressBtn = false;
                    ispressBack = false;
                });
            });
          
        }

        private void OnClickBtnRight(GameObject obj)
        {
           
            if (curPageIndex >= 5 || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                SetMoveAncPosX(-1,1,()=>isPressBtn = false);
                
            });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () =>
            {
                SetMoveAncPosX(1,1,()=>isPressBtn = false);
                
            });
        }



        private void GameInit()
        {

            
            isPressBtn = false;
            ispressBack = false;
            _pageBar.Show();
            curPageIndex = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }
           
        }

        


        

        void GameStart()
        {

            isPlaying = true;



            
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));
        }

       

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
           
            talkIndex++;
        }

        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (!isPlaying)
                return;

            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform()
                .DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration)
                .OnComplete(() => { callBack?.Invoke(); });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            Debug.Log("111");
            if(!isPlaying)
                return;
            
            Debug.Log("222");
            UIEventListener.Get(rayCastTarget).onDown = downData => { Debug.Log("333"); _prePressPos = downData.pressPosition; };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0||isPressBtn)
                            return;
                        isPressBtn = true;
                        SetMoveAncPosX(1, 1f,() =>  isPressBtn = false );
                    }
                    else
                    {
                        if (curPageIndex >= 5||isPressBtn)
                            return;
                        isPressBtn = true;
                        SetMoveAncPosX(-1, 1f, () => isPressBtn = false);
                    }
                }
            };
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

      

    }
}
