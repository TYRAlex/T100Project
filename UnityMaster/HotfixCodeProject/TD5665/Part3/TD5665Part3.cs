using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{      
    public class TD5665Part3
    {

        private int _talkIndex;               //点击语音健Index
        private int _cardPageCurIndex;        //卡牌页当前Index    
        private int _cardPageMinIndex;        //卡牌页最小Index 
        private int _cardPageMaxIndex;        //卡牌页最大Index
        private int _recordOnClickCardNums;   //记录卡牌点击次数
       
        private MonoBehaviour _mono;

        private GameObject _curGo;            //当前环节GameObject
        private GameObject _silde;            //绑定滑动事件GameObject
        private GameObject _sBD;              //小布丁
        private GameObject _mask;             //Mask
          
        private Transform _cardParent;        //卡牌父物体
        private Transform _arrowsParent;      //箭头父物体
     
        private RectTransform _cardParentRect; //卡牌父物体Rect

        private Vector2 _prePressPos;

        private List<string> _recordOnClickCardNames;   //记录点击卡牌名List   

        private bool _isFinish;                //是否完成一遍


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _silde = curTrans.GetGameObject("BG");
            _sBD = curTrans.GetGameObject("sBD");
            _mask = curTrans.GetGameObject("mask");
            
            _cardParent = curTrans.Find("Parts/Mask/Pages");
            _arrowsParent = curTrans.Find("Parts/Switchs");
      
            _cardParentRect = _cardParent.GetRectTransform();

			Input.multiTouchEnabled = false;
			DOTween.KillAll();
			
            GameInit();
            GameStart();
        }


        #region 游戏逻辑

        /// <summary>
        /// 游戏初始化
        /// </summary>
        void GameInit()
        {
            _talkIndex = 1;
            _cardPageCurIndex = 0;
            _cardPageMinIndex = 0;
            _cardPageMaxIndex = _cardParent.childCount - 1;      
             _recordOnClickCardNums =2;
           
            _recordOnClickCardNames = new List<string>();

            _isFinish = false;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.StopAudio();
            _mono.StopAllCoroutines();
     

            //初始化卡牌Spine
            var cardSGs = _cardParent.GetComponentsInChildren<SkeletonGraphic>();
            foreach (var sG in cardSGs)
            {
                sG.Initialize(true);
                PlaySpine(sG.gameObject, sG.gameObject.name);
            }
          

            //绑定卡牌点击事件
            var cardE4Rs = _cardParent.GetComponentsInChildren<Empty4Raycast>();
            foreach (var cardE4R in cardE4Rs)
                AddEvent(cardE4R.gameObject, OnClickCard);

            //绑定卡牌箭头切换事件
            var arrowsE4Rs = _arrowsParent.GetComponentsInChildren<Empty4Raycast>(true);
            foreach (var arrowsE4R in arrowsE4Rs)
                AddEvent(arrowsE4R.gameObject, OnClickArrows);

            //绑定滑动监听事件
             AddSlideSwitchPage();
          

            //移除Mask监听
            RemoveEvent(_mask);


            //重置卡牌父物体位置
            _cardParentRect.anchoredPosition = new Vector2(0, 0);

            //控制箭头边界显示
            IsBorder();

        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        void GameStart()
        {
		    _mask.Show(); _sBD.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            BellSpeck(_sBD, 0, null, () => { _mask.Hide(); _sBD.Hide(); });
           
        }

        /// <summary>
        /// 点击语音健
        /// </summary>
        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(9);
            _mask.Show(); _sBD.Show();

            switch (_talkIndex)
            {
                case 1:
                    _isFinish = true;
                    BellSpeck(_sBD, 3, null, () => { _mask.Hide(); _sBD.Hide(); });
                    break;                
            }
            _talkIndex++;
        }
     
        /// <summary>
        /// 是否边界
        /// </summary>
        /// <param name="curPageIndex">当前卡牌页Index</param>
        private void IsBorder()
        {

            var l = _arrowsParent.Find("L").gameObject;
            var r = _arrowsParent.Find("R").gameObject;
            var l2 = _arrowsParent.Find("L2").gameObject;
            var r2 = _arrowsParent.Find("R2").gameObject;


            if (_cardPageCurIndex == _cardPageMinIndex)
            {
                l.Hide(); r.Show();
                PlaySpine(l2, "L4");
                PlaySpine(r2, r2.name);
            }
            else if (_cardPageCurIndex == _cardPageMaxIndex)
            {
                l.Show(); r.Show();
                PlaySpine(l2, l2.name);
                r.Hide();
                PlaySpine(r2, "R4");
            }
            else
            {
                l.Show(); r.Show();
                PlaySpine(l2, l2.name);
                PlaySpine(r2, r2.name);
            }
        }

        /// <summary>
        /// 点击切换卡牌箭头
        /// </summary>
        /// <param name="go"></param>
        private void OnClickArrows(GameObject go)
        {
            SoundManager.instance.PlayClip(9);
            var name = go.name;
            var spineGo = _arrowsParent.Find(name + "2").gameObject;
            PlaySpine(spineGo, name);
            SwitchPage(name == "L");
        }
 
        /// <summary>
        /// 点击卡牌
        /// </summary>
        /// <param name="go"></param>
        private void OnClickCard(GameObject go)
        {

            _mask.Show();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            var name = go.name;

            bool isContains = _recordOnClickCardNames.Contains(name);
            if (!isContains)
                _recordOnClickCardNames.Add(name);

            var spineGo = go.transform.parent.Find(name + "3").gameObject;

            spineGo.transform.SetAsLastSibling();

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            var spineName1 = name + "1";    //放大
            var spineName2 = name + "2";    //缩小

            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, soundIndex);

            PlaySpine(spineGo, spineName1);

            Delay(time, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
                PlaySpine(spineGo, spineName2, () =>
                {
                    _mask.Hide();
                    if (!_isFinish && _recordOnClickCardNames.Count == _recordOnClickCardNums)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }

        /// <summary>
        /// Add滑动切换监听
        /// </summary>
        private void AddSlideSwitchPage()
        {
            var uiL = _silde.GetComponent<UIEventListener>();
            if (uiL != null)
                _silde.transform.RemoveComponent<UIEventListener>();

            UIEventListener.Get(_silde).onDown = downData => { _prePressPos = downData.pressPosition; };

            UIEventListener.Get(_silde).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                        SwitchPage(true);
                    else
                        SwitchPage(false);
                }
            };
        }

        /// <summary>
        /// 切换卡牌页
        /// </summary>
        /// <param name="isLeftSilde">是否左滑</param>
        private void SwitchPage(bool isLeftSilde)
        {       
            if (isLeftSilde)
            {
                if (_cardPageCurIndex <= _cardPageMinIndex)
                    return;

                _cardPageCurIndex--;

            }
            else
            {
                if (_cardPageCurIndex >= _cardPageMaxIndex)
                    return;
                _cardPageCurIndex++;

            }

            _mask.Show();

            SoundManager.instance.ShowVoiceBtn(false);

            float value = isLeftSilde ? 1920f : -1920f;
            value = _cardParentRect.anchoredPosition.x + value;

            _cardParentRect.DOAnchorPosX(value, 1.0f).OnComplete(() =>
            {
                IsBorder();
                _mask.Hide();
                if (!_isFinish && _recordOnClickCardNames.Count == _recordOnClickCardNums)
                    SoundManager.instance.ShowVoiceBtn(true);
            });
        }

        #endregion

        #region 常用函数

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, float len = 0)
        {

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            RemoveEvent(go);
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

    }
}