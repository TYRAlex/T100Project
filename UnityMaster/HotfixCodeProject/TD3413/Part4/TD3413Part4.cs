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

    public class TD3413Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        
        private Transform SpinePage;

        private GameObject _pageBar;
        private List<GameObject> _pageObjectLeftList;
        
        private Vector2 _prePressPos;

        private Empty4Raycast[] e4rs;
        bool isPressBtn = false;
      
        bool isPlaying = false;
        private bool ispressBack = false;
        private int curPageIndex;  //当前页签索引
        private GameObject btnBack;
        //胜利动画名字
       
       
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;

            btnBack = curTrans.GetGameObject("btnBack");
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
           
            _pageBar = curTrans.GetGameObject("PageBar");
            _pageBar.Hide();
            SpinePage = _pageBar.transform.GetTransform("MaskImg/SpinePage");
            e4rs = SpinePage.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

          
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            //SlideSwitchPage(_pageBar);
        }

        private GameObject tem;
        void OnClickShow(GameObject obj)
        {
           
            if (isPressBtn||!isPlaying)
                return;
            isPressBtn = true;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => { btnBack.SetActive(true);
                ispressBack = true;
            });
            if (_pageObjectLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(false);
            }

            GetAndPlaySpinePageVoice(obj.name,()=>ispressBack=false);
            _pageObjectLeftList.Remove(obj);
        }

        void GetAndPlaySpinePageVoice(string name,Action callback=null)
        {
            int targetVoiceIndex = 1;
            switch (name)
            {
                case "a":
                    targetVoiceIndex=1;
                    break;
                case "b":
                    targetVoiceIndex = 2;
                    break;
                case "c":
                    targetVoiceIndex=3;
                    break;
              
            }

            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetVoiceIndex);
            mono.StartCoroutine(WaitTimeAndExcuteNext(timer, callback));
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (ispressBack)
                return;
            ispressBack = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false);  isPressBtn = false;
                ispressBack = false;
                JudgePageImageFinished();
            }); });
            
        }
        
        void JudgePageImageFinished()
        {
          
            if (_pageObjectLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        void FinishedAllTheImage()
        {
            bd.Show();
            bd.transform.SetAsLastSibling();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13));
        }




        void ShowThePagePanel()
        {
            //isPlaying = false;
            _pageBar.Show();
            curPageIndex = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
          
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            //SetMoveAncPosX(-1);
        }




        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            ispressBack = false;
            _pageBar.Hide();
           
            _pageObjectLeftList=new List<GameObject>();
            Transform pageObjectParent = _pageBar.transform.GetTransform("MaskImg/SpinePage");
            for (int i = 0; i < pageObjectParent.childCount; i++)
            {
                GameObject target = pageObjectParent.GetChild(i).GetChild(0).gameObject;
                _pageObjectLeftList.Add(target);
            }
        }

        IEnumerator WaitTimeAndExcuteNext(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }

        

        void GameStart()
        {
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                bd.SetActive(false);
               
                isPlaying = true;
                //isPlaying = true;
                
            }));
            
            ShowThePagePanel();
         
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));
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
                // mask.Show();
                // mask.transform.SetAsLastSibling();
                isPlaying = false;
                bd.Show();
                bd.transform.SetAsLastSibling();

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4));
            }
           
            

            talkIndex++;
        }
        
        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (!isPlaying)
                return;
          
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke();  });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
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
                        if (curPageIndex >= 2 || isPressBtn)
                            return;
                        isPressBtn = true;
                        SetMoveAncPosX(-1, 1, () => isPressBtn = false);
                    }
                }
            };
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


        

       

    }
}
