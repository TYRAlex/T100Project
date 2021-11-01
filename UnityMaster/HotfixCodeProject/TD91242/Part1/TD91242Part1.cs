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
    public class TD91242Part1
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

			
            

            GameInit();
            GameStart();
        }

        void InitData()
        {           
            _recordOnClickNames = new List<string>();
			_recordOnClickNums = 10;
           		
					               			
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
			                       
            AddEvents(_part1Tra, OnClickMaterials);
            RemoveEvent(_bg);
            RemoveSildeEvent(_bg);
        }

        void GameStart()
        {
            PlayCommonBgm(6);
            BellSpeck(_sBD, 10, () => { _mask.Show(); }, () => { ShowVoiceBtn(); });
        }


        void TalkClick()
        {
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sBD,11, null, () => { _mask.Hide(); _sBD.Hide(); });
                    break;
                case 2:
                    _isPlaying = true;                   
                    _recordOnClickNames.Clear();
					_recordOnClickNums = 3;					
                    _sBD.Show();
					_part2Tra.gameObject.Show();
					
					
					
                    BellSpeck(_sBD, 12, null,
                        () => {
							_sBD.Hide();  
                            SetMoveAncPosX(_part1Tra.GetRectTransform(), -1920, 1.0f);
                            SetMoveAncPosX(_part2PagesTra.GetRectTransform(),-1920, 1.0f);
							Delay(1.0f,()=>{ _isPlaying = false;});							
							
                        });
                    break;
                case 3:
                    _sBD.Show(); _isPlaying = true;  BellSpeck(_sBD, 16);
					
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


		


		
  


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
        private void RemoveSildeEvent(GameObject go)
        {
            var uiL = go.GetComponent<UIEventListener>();
            if (uiL != null)
            {
                go.transform.RemoveComponent<UIEventListener>();
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
