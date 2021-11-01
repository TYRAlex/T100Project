using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD6753Part6
    {     
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _partsGo;
        private Transform _pagesTra;
        private Transform _switchsTra;
      

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

       
      
        private bool _isSlide; //是否滑动中

        void Start(object o)
        {
            Input.multiTouchEnabled = false;
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _partsGo = curTrans.GetGameObject("Parts");
            _pagesTra = curTrans.Find("Parts/Mask/Pages");
            _switchsTra = curTrans.Find("Parts/Switchs");
         
            GameInit();
            GameStart();
        }

        void InitData()
        {          
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = _pagesTra.childCount - 1;          
            _isSlide = false;
        }

        void GameInit()
        {
            InitData();
          
            SoundManager.instance.ShowVoiceBtn(false);         
            SoundManager.instance.StopAudio();
            _mono.StopAllCoroutines(); 
			
			_pagesTra.GetRectTransform().anchoredPosition = Vector2.zero;
          

            for (int i = 0; i < _pagesTra.childCount; i++)
            {
                var child = _pagesTra.GetChild(i);
                InitSpines(child, false);
                AddEvents(child, OnClickCard);
            }

            InitSpines(_switchsTra, false);

            AddEvents(_switchsTra, OnClickArrows);

            var rect = _pagesTra.GetRectTransform();
            SlideSwitchPage(_partsGo.gameObject, () => { LeftSwitchPage(rect, 1920); }, () => { RightSwitchPage(rect, -1920);});
            RemoveEvent(_mask);
            _mask.Hide();
        }

        void GameStart()
        {
            IsBorder(_curPageIndex);
        }

    
        private void IsBorder(int curPageIndex)
        {
            var l = _switchsTra.Find("L").gameObject;
            var r = _switchsTra.Find("R").gameObject;
            var l2 = _switchsTra.Find("L2").gameObject;
            var r2 = _switchsTra.Find("R2").gameObject;

            if (curPageIndex == _pageMinIndex)
            {
                l.Hide(); r.Show();
                PlaySpine(l2, "L4");
                PlaySpine(r2, r2.name);
            }
            else if (curPageIndex == _pageMaxIndex)
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
    
        private void OnClickCard(GameObject go)
        {
            if (_isSlide)
                return;

            _mask.Show();		
			SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            var name = go.name;

            var parent = go.transform.parent;
            var spineGo = parent.Find(name+"3").gameObject;

            var name1 = name + "1";   //放大
            var name2 = name + "2";   //缩小

            PlaySpine(spineGo, name1, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
                PlaySpine(spineGo, name2, () => { _mask.Hide(); });
            }
        }

        private void OnClickArrows(GameObject go)
        {
            if (_isSlide)
                return;

            SoundManager.instance.PlayClip(9);
            var name = go.name;									
            var spineGo = _switchsTra.Find(name + "2").gameObject;			
            PlaySpine(spineGo, name);
            var rect = _pagesTra.GetRectTransform();
            switch (name)
            {
                case "L":
                    LeftSwitchPage(rect, 1920);
                    break;
                case "R":
                    RightSwitchPage(rect, -1920);
                    break;
            }
        }
        
        private void SlideSwitchPage(GameObject rayCastTarget, Action leftCallBack, Action rightCallBack)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>{ _prePressPos = downData.pressPosition;};

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                        leftCallBack?.Invoke();
                    else
                        rightCallBack?.Invoke();
                }
            };
        }

        private void LeftSwitchPage(RectTransform rect, float value)
        {
            if (_curPageIndex <= _pageMinIndex)
                return;

            if (_isSlide)
                return;

            _isSlide = true;

            _curPageIndex--;
									
            value = rect.anchoredPosition.x + value;
			
            rect.DOAnchorPosX(value, 1.0f).OnComplete(() => {   _isSlide = false; IsBorder(_curPageIndex);});
			          
        }

        private void RightSwitchPage(RectTransform rect, float value)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;

            if (_isSlide)
                return;

            _isSlide = true;

            _curPageIndex++;
			
			 value = rect.anchoredPosition.x + value;
             rect.DOAnchorPosX(value, 1.0f).OnComplete(() => { _isSlide = false; IsBorder(_curPageIndex); });
			          
        }
							
         
        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                child.GetComponent<SkeletonGraphic>().Initialize(true);
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }
                               
        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }           
      

         
    }
}
