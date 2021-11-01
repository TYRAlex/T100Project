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
    public class TD91212Part3
    {

         private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        
        //private GameObject mask;

        private Transform SpinePage;

      

       

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
        private GameObject _clickItem;
        
        void OnClickShow(GameObject obj)
        {
            
            if (isPressBtn||!isPlaying)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            _clickItem = obj;
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => { btnBack.SetActive(true); });
            isPressBackBtn = true;
            PlayPageImageVoice(obj.name,()=>isPressBackBtn=false);
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
                case "4_1":
                    targetIndex = 1;
                    break;
                case "5_1":
                    targetIndex = 2;
                    break;
                case "6_1":
                    targetIndex = 3;
                    break;
                
            }

            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            mono.StartCoroutine(WaitTimeAndExcuteNext(timer, callback));
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBackBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, _clickItem.name.Remove(2) + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false);
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


        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            isPlaying = false;
            isPressBackBtn = false;
            _pageBar.Show();
            curPageIndex = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }
            _pageObjectLeftList=new List<GameObject>();
            Transform pageObjectParent = _pageBar.transform.GetTransform("MaskImg/SpinePage");
            for (int i = 0; i < pageObjectParent.childCount; i++)
            {
                GameObject target = pageObjectParent.GetChild(i).GetChild(0).gameObject;
                _pageObjectLeftList.Add(target);
            }
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

       
        

        IEnumerator WaitTimeAndExcuteNext(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
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
               
                isPlaying = false;
                bd.Show();
                bd.transform.SetAsLastSibling();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4));
            }
            else if(talkIndex==2)
            {
               
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

        
        

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

    }
}
