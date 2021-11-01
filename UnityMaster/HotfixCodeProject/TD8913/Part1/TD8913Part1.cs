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
    
    public class TD8913Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        
        //private GameObject mask;

        private Transform SpinePage;

        private Transform _clickArea;

        private List<GameObject> _clickObjectLeftList;
        private GameObject _mainTarget;

        private GameObject _pageBar;
        private List<GameObject> _pageObjectLeftList;
        
        private Vector2 _prePressPos;

        private Empty4Raycast[] e4rs;
        bool isPressBtn = false;
      
        bool isPlaying = false;
        private bool isPressBackBtn = false;
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
         

            // mask = curTrans.Find("mask").gameObject;
            // mask.SetActive(true);
            btnBack = curTrans.GetGameObject("btnBack");
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
            _clickArea = curTrans.GetTransform("CickArea");
            _mainTarget = curTrans.GetGameObject("Main");
            _mainTarget.Show();
            _pageBar = curTrans.GetGameObject("PageBar");
            SpinePage = _pageBar.transform.GetTransform("MaskImg/SpinePage");
            e4rs = SpinePage.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }



            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            //SlideSwitchPage(_pageBar);
            GameInit();
            GameStart();
        }

        private GameObject tem;
        void OnClickShow(GameObject obj)
        {
            
            if (isPressBtn||!isPlaying)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => { btnBack.SetActive(true); });
            isPressBackBtn = true;
            PlayPageImageVoice(obj.name, () => isPressBackBtn = false);
            _pageObjectLeftList.Remove(obj);
            if (_pageObjectLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(false);
            }
        }

        void PlayPageImageVoice(string name,Action callback=null)
        {
            int targetIndex = 0;
            switch (name)
            {
                case "a":
                    targetIndex = 7;
                    break;
                case "b":
                    targetIndex = 8;
                    break;
                case "c":
                    targetIndex = 9;
                    break;
                
            }

            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            mono.StartCoroutine(WaitTimeAndExcuteNext(timer, callback));

        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBackBtn)
                return;
            isPressBackBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false);
                isPressBackBtn = false; isPressBtn = false; }); });
            JudgePageImageFinished();
        }

        void JudgePageImageFinished()
        {
            //Debug.Log(_pageObjectLeftList.Count);
            if (_pageObjectLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(true);
                
            }
        }


        void ShowThePagePanel()
        {
            isPlaying = true;
            _pageBar.Show();
            curPageIndex = -1;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(1920f, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SetMoveAncPosX(-1);
        }




        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            isPlaying = false;
            isPressBackBtn = false;
            _pageBar.Hide();
            _clickObjectLeftList=new List<GameObject>();
            for (int i = 0; i < _clickArea.childCount; i++)
            {
                GameObject target = _clickArea.GetChild(i).gameObject;
                _clickObjectLeftList.Add(target);
                PointerClickListener.Get(target).onClick = OnclickEvent;
            }
            _pageObjectLeftList=new List<GameObject>();
            Transform pageObjectParent = _pageBar.transform.GetTransform("MaskImg/SpinePage");
            for (int i = 0; i < pageObjectParent.childCount; i++)
            {
                GameObject target = pageObjectParent.GetChild(i).GetChild(0).gameObject;
                _pageObjectLeftList.Add(target);
            }
        }

        void OnclickEvent(GameObject o)
        {
            if(!isPlaying||isPressBtn)
                return;
            isPressBtn = true;
            SpineManager.instance.DoAnimation(_mainTarget, o.name, false);
            _clickObjectLeftList.Remove(o);
            if (_clickObjectLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(false);
            }

            PlayMaterialVoice(o.name, () =>
            {
                JudgeIfFInished();
                isPressBtn = false;
            });
            
           
           
        }
        
        void JudgeIfFInished()
        {
            if (_clickObjectLeftList.Count <= 0)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        void PlayMaterialVoice(string name,Action callback=null)
        {
            int targetIndex = 0;
            switch (name)
            {
                case "1":
                    targetIndex = 1;
                    break;
                case "2":
                    targetIndex = 3;
                    break;
                case "3":
                    targetIndex = 4;
                    break;
                case "4":
                    targetIndex = 2;
                    break;
                case "5":
                    targetIndex = 5;
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

        

        void GameStart()
        {
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                bd.Hide();
                isPlaying = true;
                
            }));
            

         
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
               
                //mask.Show();
                //mask.transform.SetAsLastSibling();
                bd.Show();
                bd.transform.SetAsLastSibling();
                isPlaying = false;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, null, () =>
                {
                    bd.Hide();
                    _mainTarget.Hide();
                    ShowThePagePanel();
                }));
            }
            else if(talkIndex==2)
            {
                isPlaying = false;
                bd.Show();
                bd.transform.SetAsLastSibling();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10));
            }
            

            talkIndex++;
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
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        isPressBtn = true;
                        SetMoveAncPosX(1,1,()=>isPressBtn=false);
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
