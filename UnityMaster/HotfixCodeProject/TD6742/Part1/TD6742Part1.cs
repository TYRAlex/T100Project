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
    public class TD6742Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

		
        private GameObject _bg;
        private GameObject _sBD;
        private GameObject _mask;

        private Transform _part1Tra;
        private Transform _part2Tra;
        private Transform _part2PagesTra;

		private Transform _switchsTra;        
		private bool _isSlide;              
        private int _curPageIndex,_pageMinIndex,_pageMaxIndex;          
        private Vector2 _prePressPos;   
            				    		
  	    private bool _isPlaying;		
		private int _recordOnClickNums;  
        private List<string> _recordOnClickNames;  
        
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _bg = curTrans.GetGameObject("BG");
            _sBD = curTrans.GetGameObject("sBD");
            _mask = curTrans.GetGameObject("mask");

            _part1Tra = curTrans.Find("Parts/1");
            _part2Tra = curTrans.Find("Parts/2");
            _part2PagesTra = curTrans.Find("Parts/2/Mask/Pages");

			_switchsTra = curTrans.Find("Parts/Switchs");
            

            GameInit();
            GameStart();
        }

        void InitData()
        {           
            _recordOnClickNames = new List<string>();
			_recordOnClickNums = 6;
           		
			_curPageIndex = 0; _pageMinIndex = 0;_pageMaxIndex=_part2PagesTra.childCount-1; _isSlide = false;		               			
            _isPlaying = false;           
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
			StopAllCoroutines();

            InitSpines(_part1Tra, false);


            for (int i = 0; i < _part2PagesTra.childCount; i++)
            {
                InitSpines(_part2PagesTra.GetChild(i), false);
                AddEvents(_part2PagesTra.GetChild(i), OnClickCard);
            }

            SetPos(_part1Tra.GetRectTransform(), new Vector2(0, 0));
            SetPos(_part2PagesTra.GetRectTransform(), new Vector2(1920, 0));
			
            RemoveEvent(_mask);

            _sBD.Show(); 
			_part2Tra.gameObject.Hide();
			_switchsTra.gameObject.Hide(); AddEvents(_switchsTra, OnClickArrows);                       
            AddEvents(_part1Tra, OnClickMaterials);
            //RemoveEvent(_bg);
            RemoveSildeEvent(_bg);
        }

        void GameStart()
        {
            PlayCommonBgm(6);
            BellSpeck(_sBD, 0, () => { _mask.Show(); }, () => { ShowVoiceBtn(); });
        }

        private void RemoveSildeEvent(GameObject go)
        {
            var uiL = go.GetComponent<UIEventListener>();
            if (uiL != null)
            {
                go.transform.RemoveComponent<UIEventListener>();
            }

        }
        void TalkClick()
        {
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sBD, 1, null, () => { _mask.Hide(); });
                    break;
                case 2:
                    _isPlaying = true;                   
                    _recordOnClickNames.Clear();
					_recordOnClickNums = 5;					
                    _sBD.Show();
					_part2Tra.gameObject.Show();
					
					_isSlide = true;
					
                    BellSpeck(_sBD, 8, null,
                        () => {
							_sBD.Hide();  
                            SetMoveAncPosX(_part1Tra.GetRectTransform(), -1920, 1.0f);
                            SetMoveAncPosX(_part2PagesTra.GetRectTransform(),-1920, 1.0f);
							Delay(1.0f,()=>{ _isPlaying = false;});							
							
							Delay(1.0f,()=>{_isSlide = false; _switchsTra.gameObject.Show(); IsBorder(_curPageIndex);});                                                     
                            SlideSwitchPage(_bg,() => {LeftSwitchPage(_part2PagesTra.GetRectTransform(),1920);},() => {RightSwitchPage(_part2PagesTra.GetRectTransform(),-1920);});

                        });
                    break;
                case 3:
                    _sBD.Show(); _isPlaying = true;  BellSpeck(_sBD, 14);
					_isSlide = true;
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


		
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
                


		
        private void OnClickArrows(GameObject go)
        {
            if (_isSlide)
                return;

            PlayOnClickSound();
            var name = go.name;

            var spineGo = FindGo(_switchsTra, name + "2");
            PlaySpine(spineGo, name);

            switch (name)
            {
                 case "L":
                 LeftSwitchPage(_part2PagesTra.GetRectTransform(), 1920);
                 break;
                 case "R":
                  RightSwitchPage(_part2PagesTra.GetRectTransform(), -1920);
                 break;
            }
         }
                
  


        /// <summary>
        /// 点击材料
        /// </summary>
        /// <param name="go"></param>
        void OnClickMaterials(GameObject go)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            PlayCommonSound(6);
            HideVoiceBtn();

            var name = go.name;
            bool isContains = _recordOnClickNames.Contains(name);
            if (!isContains)
                _recordOnClickNames.Add(name);

            var spineGo = FindGo(_part1Tra, "0");
            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            PlaySpine(spineGo, name);
            BellSpeck(_sBD, soundIndex, null,
                () => {
                    _isPlaying = false;
                    bool isFinish = _recordOnClickNums == _recordOnClickNames.Count;
                    if (isFinish)
                        ShowVoiceBtn();
                });
        }

        /// <summary>
        /// 点击卡牌
        /// </summary>
        /// <param name="go"></param>
        private void OnClickCard(GameObject go)
        {
            if (_isPlaying)
                return;
			
			
            if (_isSlide)
                return;
       
            _isPlaying = true;

            _mask.Show();
            HideVoiceBtn();
            PlayCommonSound(1);

            var name = go.name;

            bool isContains = _recordOnClickNames.Contains(name);
            if (!isContains)
                _recordOnClickNames.Add(name);



            var parent = go.transform.parent;
            var spineGo = FindGo(parent, name + "3");

            spineGo.transform.SetAsLastSibling();

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            var spineName1 = name + "1";    //放大
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
                        ShowVoiceBtn();
                });
            }
        }
        #endregion

        #region 常用函数

        #region 语音按钮

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

		
        #region 左右切换
        
        private void SlideSwitchPage(GameObject rayCastTarget, Action leftCallBack, Action rightCallBack)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>{_prePressPos = downData.pressPosition;};

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
			
			HideVoiceBtn();
			
			value = rect.anchoredPosition.x + value;
			
            rect.DOAnchorPosX(value, 1.0f).OnComplete(() => { 
			   _isSlide = false; 
			   IsBorder(_curPageIndex);
               if (_recordOnClickNames.Count == _recordOnClickNums)
                     ShowVoiceBtn();
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
			
			HideVoiceBtn();
			
			value = rect.anchoredPosition.x + value;
			
			rect.DOAnchorPosX(value, 1.0f).OnComplete(() => { 
			_isSlide = false; 
			IsBorder(_curPageIndex);
               if (_recordOnClickNames.Count == _recordOnClickNums)
                     ShowVoiceBtn();
			});		
			
            
        }
		
        #endregion

  
        #region Spine相关

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

        #endregion

        #region 音频相关

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
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

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }
     
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
  

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }
   
        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null,  float len = 0)
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

        #endregion

        #region 监听相关

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
        #endregion

        #region 修改Rect相关

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

      
        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }

        #endregion



        #endregion

    }
}
