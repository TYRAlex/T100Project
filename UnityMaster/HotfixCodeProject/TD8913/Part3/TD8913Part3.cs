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
    public class TD8913Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
       

       

       
        private GameObject mask;

        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        private List<GameObject> _imageLeftList;
       

        
      
        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;
        private bool ispressBackBtn = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);

            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
           

            

            pageBar = curTrans.Find("PageBar").gameObject;
            pageBar.Show();
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
           
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

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }


        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount/2 - 1 || !isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || !isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        }

        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (ispressBackBtn)
                return;
            ispressBackBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false);
                ispressBackBtn = false; isPressBtn = false; }); });
            FinishedImageClick();
        }


        void FinishedImageClick()
        {
           
            if (_imageLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(true);
               
            }
        }

        private void OnClickShow(GameObject obj)
        {
            if (!isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            _imageLeftList.Remove(obj);
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => {  });
            PlayPageVoice(obj.name, () => {btnBack.SetActive(true); });
            if (_imageLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(false);
            }
        }

        void PlayPageVoice(string name,Action callback=null)
        {
            int targetIndex = 0;
            switch (name)
            {
                case "d":
                    targetIndex = 5;
                    break;
                case "e":
                    targetIndex = 1;
                    break;
                case "f":
                    targetIndex = 2;
                    break;
                case "g":
                    targetIndex = 3;
                    break;
                
            }

            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            mono.StartCoroutine(WaitTimeAndExcuteNext(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNext(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }

        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            ispressBackBtn = false;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            _imageLeftList=new List<GameObject>();
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                GameObject target = SpinePage.GetChild(i).GetChild(0).GetGameObject();
                _imageLeftList.Add(target);
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
                
            }
            

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }

        void GameStart()
        {
 

            
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => { bd.Hide(); isPlaying = true; }));
        }
        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "bd-speak");
            method_1?.Invoke();
            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4));
                isPlaying = false;
                isPressBtn = false;
            }
            else if (talkIndex == 2)
            {
               
            }

            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);

        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            // if (isPlaying)
            //     return;
            // isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke();});
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                if (isPlaying)
                {
                    float dis = (upData.position - _prePressPos).magnitude;
                    bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                    if (dis > 100)
                    {
                        if (!isRight)
                        {
                            if (curPageIndex <= 0 || isPressBtn)
                                return;
                            isPressBtn = true;
                            SetMoveAncPosX(1, 1, () => isPressBtn = false);
                        }
                        else
                        {
                            if (curPageIndex >= SpinePage.childCount/2 - 1 || isPressBtn)
                                return;
                            isPressBtn = true;
                            SetMoveAncPosX(-1, 1, () => isPressBtn = false);
                        }
                    }
                }

                
            };
        }
    }
}
