using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD3443Part3
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _partsGo;
        private Transform _pagesTra;
		private GameObject _sBD;
      
        private List<string> _recordOnClickNames;
        private int _recordOnClickNums;  //记录点击的次数

		private Transform _switchsTra;
		private int _curPageIndex,_pageMinIndex,_pageMaxIndex;        
        private Vector2 _prePressPos;   		        		
		private float _moveValue;       		
		private bool _isSlide;
         

        private bool _isPlaying;
       

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _partsGo = curTrans.GetGameObject("Parts");
            _pagesTra = curTrans.Find("Parts/Mask/Pages");
			_sBD = curTrans.GetGameObject("sBD");
			 
			_switchsTra = curTrans.Find("Parts/Switchs");
                     
            GameInit();
            GameStart();
        }

        void InitData()
        {            
			 _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = _pagesTra.childCount - 1;_moveValue = 1920; _isSlide = false; 
           			
			_isPlaying = false;
            _recordOnClickNames = new List<string>();						
            _recordOnClickNums =3;
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.StopAudio();
			_mono.StopAllCoroutines();

			_pagesTra.GetRectTransform().anchoredPosition =Vector2.zero;
      

            for (int i = 0; i < _pagesTra.childCount; i++)
            {
                var child = _pagesTra.GetChild(i);
                InitSpines(child, false);
                AddEvents(child, OnClickCard);
            }

			 InitSpines(_switchsTra, false);           
			AddEvents(_switchsTra, OnClickArrows);           			
           SlideSwitchPage(_partsGo.gameObject,() => { LeftSwitchPage(_pagesTra.GetRectTransform(), _moveValue);},() => {RightSwitchPage(_pagesTra.GetRectTransform(),-_moveValue);});
           

            RemoveEvent(_mask);
            _mask.Show();
            _sBD.Show();
        }

        void GameStart()
        {
			IsBorder(_curPageIndex);          			
			PlayCommonBgm(6);
            BellSpeck(_sBD, 0, null, () => { _mask.Hide();_sBD.Hide(); });
        }


        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            switch (_talkIndex)
            {
                case 1:
                    _sBD.Show(); 
					_isPlaying = true; 	
					
					_isSlide = true;
					
                    BellSpeck(_sBD, 4);  //ToDo...改下index
                    break;
            }
            _talkIndex++;
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
            if (_isPlaying)
                return;

			
            if (_isSlide)
                return;
           

            _isPlaying = true;
			
            _mask.Show();		
			
            SoundManager.instance.ShowVoiceBtn(false);
			
            PlayCommonSound(1);

            var name = go.name;

            bool isContains = _recordOnClickNames.Contains(name);
            if (!isContains)
                _recordOnClickNames.Add(name);


            var parent = go.transform.parent;
            var spineGo = FindGo(parent, name + "3");

            spineGo.transform.SetAsLastSibling();

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            var spineName1 = name + "1";         //放大
            var spineName2 = name + "2";    //缩小

            var time = PlaySound(soundIndex);
            PlaySpine(spineGo, spineName1);

            Delay(time, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                _isPlaying = false;
                RemoveEvent(g);
                PlayCommonSound(2);
                PlaySpine(spineGo, spineName2, () => {
                    _mask.Hide();
                    if (_recordOnClickNames.Count == _recordOnClickNums)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }


		
        private void OnClickArrows(GameObject go)
        {
            if (_isSlide)
                return;

			if (_isPlaying)
                return;
			
            SoundManager.instance.PlayClip(9);
            var name = go.name;

            var spineGo = FindGo(_switchsTra, name + "2");
            PlaySpine(spineGo, name);

                    if (name == "L")
                        LeftSwitchPage(_pagesTra.GetRectTransform(), _moveValue);
                    else
                        RightSwitchPage(_pagesTra.GetRectTransform(), -_moveValue);
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
						
			SoundManager.instance.ShowVoiceBtn(false);
			
            value = rect.anchoredPosition.x + value;
			
            rect.DOAnchorPosX(value, 1.0f).OnComplete(() => { 
			   _isSlide = false; 
			   IsBorder(_curPageIndex);
               if (_recordOnClickNames.Count == _recordOnClickNums)
                     SoundManager.instance.ShowVoiceBtn(true);
			});			       
        }


		
        private void RightSwitchPage(RectTransform rect, float value)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;

            if (_isSlide)
                return;

            _isSlide = true;
			
            _curPageIndex++;
			
		    SoundManager.instance.ShowVoiceBtn(false);
			
            value = rect.anchoredPosition.x + value;
			
            rect.DOAnchorPosX(value, 1.0f).OnComplete(() => { 
			   _isSlide = false; 
			   IsBorder(_curPageIndex);
               if (_recordOnClickNames.Count == _recordOnClickNums)
                     SoundManager.instance.ShowVoiceBtn(true);
			});			
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

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }
           
        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }
 
        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
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
            SpineManager.instance.DoAnimation(go,  "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
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
