using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    
    public class TD5663Part2
    {

        private int _talkIndex;               //点击语音健Index      
        private int _recordOnClickCardNums;   //记录卡牌点击次数
        private int _smallPartCurIndex;       //小环节当前Index

        private MonoBehaviour _mono;

        private GameObject _curGo;            //当前环节GameObject
       
        private GameObject _sBD;              //小布丁
        private GameObject _mask;             //Mask
        private GameObject _last;             //绑定去上一小环节事件GameObject
        private GameObject _next;             //绑定去下一小环节事件GameObject

        private Transform _materialsParent;   //材料父物体    
        private Transform _cardParent;        //卡牌父物体
        
        private RectTransform _materialsParentRect;
        private RectTransform _cardParentRect;

        private List<string> _recordOnClickCardNames;   //记录点击卡牌名List   
        private List<bool> _smallPartIsFinishs;         //记录小环节是否完成List

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
        
            _sBD = curTrans.GetGameObject("sBD");
            _mask = curTrans.GetGameObject("mask");
            _last = curTrans.GetGameObject("LastBtn");
            _next = curTrans.GetGameObject("NextBtn");

            _materialsParent = curTrans.Find("Parts/1");
            _cardParent = curTrans.Find("Parts/2/Mask/Pages");
          
            _materialsParentRect = _materialsParent.GetRectTransform();
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
           
            _recordOnClickCardNums =1;
            _smallPartCurIndex = 0;

            _recordOnClickCardNames = new List<string>();
            _smallPartIsFinishs = new List<bool> { false, false };


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.StopAudio();
            _mono.StopAllCoroutines();


            //初始化材料Spine
            var materialsSpineGo = _materialsParent.Find("0").gameObject;
            materialsSpineGo.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(materialsSpineGo, materialsSpineGo.name);

            //初始化卡牌Spine
            var cardSGs = _cardParent.GetComponentsInChildren<SkeletonGraphic>();
            foreach (var sG in cardSGs)
            {
                sG.Initialize(true);
                PlaySpine(sG.gameObject, sG.gameObject.name);
            }

            //绑定材料点击事件
            var materialsE4Rs = _materialsParent.GetComponentsInChildren<Empty4Raycast>();
            foreach (var materialsE4R in materialsE4Rs)
                AddEvent(materialsE4R.gameObject, OnClickMaterials);


            //绑定卡牌点击事件
            var cardE4Rs = _cardParent.GetComponentsInChildren<Empty4Raycast>();
            foreach (var cardE4R in cardE4Rs)
                AddEvent(cardE4R.gameObject, OnClickCard);               

            //绑定小环节切换事件
            AddEvent(_last, OnClickLast);
            AddEvent(_next, OnClickNext);

            //移除Mask监听
            RemoveEvent(_mask);

            //重置材料和卡牌父物体位置
            _materialsParentRect.anchoredPosition = new Vector2(0, 0);
            _cardParentRect.anchoredPosition = new Vector2(1920, 0);

            ControllePartInteriorSwitchsShow();
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        void GameStart()
        {
		    _mask.Show(); _sBD.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            BellSpeck(_sBD, 0, null, () =>
            {
                _sBD.Hide();
                _mask.Hide();
                SoundManager.instance.ShowVoiceBtn(true);
            });
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
                    _smallPartIsFinishs[_smallPartCurIndex] = true;
                    BellSpeck(_sBD, 12, null, () => { _mask.Hide(); _sBD.Hide(); ControllePartInteriorSwitchsShow(); });
                    break;
                case 2:
				    _smallPartIsFinishs[_smallPartCurIndex] = true;
                    BellSpeck(_sBD, 14, null, () => { _mask.Hide(); _sBD.Hide(); ControllePartInteriorSwitchsShow(); });
                    break;
        //         case 3:
				    // _smallPartIsFinishs[_smallPartCurIndex] = true;
        //             BellSpeck(_sBD, 0, null, () => { _mask.Hide(); _sBD.Hide(); ControllePartInteriorSwitchsShow(); });
        //             break;
            }
            _talkIndex++;
        }

        /// <summary>
        /// 点击去下一小环节
        /// </summary>
        /// <param name="go"></param>
        private void OnClickNext(GameObject go)
        {
            _mask.Show();
			ClickFeedback(go.transform,()=>{
			
			_smallPartCurIndex++;
         
            switch (_smallPartCurIndex)
            {
                case 1:       //材料去卡牌1小环节
                    _materialsParentRect.DOAnchorPosX(_materialsParentRect.anchoredPosition.x - 1920, 1);
                    _cardParentRect.DOAnchorPosX(_cardParentRect.anchoredPosition.x - 1920, 1);
                    break;                      
            }

            Delay(1.0f, () => { _mask.Hide(); });
            ControllePartInteriorSwitchsShow();
			
			});

        }

        /// <summary>
        /// 点击去上一小环节
        /// </summary>
        /// <param name="go"></param>
        private void OnClickLast(GameObject go)
        {
            _mask.Show();
			ClickFeedback(go.transform,()=>{
			
			_smallPartCurIndex--;
     
            switch (_smallPartCurIndex)
            {
                case 0:     //卡牌1小环节去材料    
                    _materialsParentRect.DOAnchorPosX(_materialsParentRect.anchoredPosition.x + 1920, 1);
                    _cardParentRect.DOAnchorPosX(_cardParentRect.anchoredPosition.x + 1920, 1);
                    break;
            }

            Delay(1.0f, () => { _mask.Hide(); });
            ControllePartInteriorSwitchsShow();
			
			});

        }

        /// <summary>
        /// 控制小环节内切换Btn显示
        /// </summary>
        /// <param name="curSmallPartIndex">当前小环节Index</param>
        /// <param name="smallPartIsFinishs">小环节是否完成List</param>
        private void ControllePartInteriorSwitchsShow()
        {
            bool isFirstIndex = _smallPartCurIndex == 0;
            bool isEndIndex = _smallPartCurIndex == _smallPartIsFinishs.Count - 1;

            if (isFirstIndex)
            {
                _last.Hide();
                if (_smallPartIsFinishs[_smallPartCurIndex])
                    _next.Show();
                else
                    _next.Hide();
            }
            else if (isEndIndex)
            {
                _next.Hide();
                if (_smallPartIsFinishs[_smallPartCurIndex])
                    _last.Show();
                else
                    _last.Hide();
            }
            else
            {
                if (_smallPartIsFinishs[_smallPartCurIndex])
                {
                    _last.Show();
                    _next.Show();
                }
                else
                {
                    _last.Hide();
                    _next.Hide();
                }
            }
        }

    

        /// <summary>
        /// 点击材料
        /// </summary>
        /// <param name="go"></param>
        private void OnClickMaterials(GameObject go)
        {
            _mask.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            SoundManager.instance.ShowVoiceBtn(false);

            var name = go.name;
            var spineGo = _materialsParent.Find("0").gameObject;
            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            PlaySpine(spineGo, name);
            BellSpeck(_sBD, soundIndex, null, () =>
            {
                _mask.Hide();
                if (!_smallPartIsFinishs[_smallPartCurIndex])
                    SoundManager.instance.ShowVoiceBtn(true);
            });
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

            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, soundIndex);

            PlaySpine(spineGo, spineName1);

            Delay(time, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
                PlaySpine(spineGo, spineName2, () =>
                {
                    _mask.Hide();
                    if (!_smallPartIsFinishs[_smallPartCurIndex] && _recordOnClickCardNames.Count == _recordOnClickCardNums)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }

        #endregion

        #region 常用函数

        void ClickFeedback(Transform tar, Action callBack)
        {
            SoundManager.instance.PlayClip(9);

            Vector2 curScale = tar.localScale;

            tar.DOScale(curScale * 0.9f, 1 / 6f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tar.DOScale(curScale, 1 / 3f).SetEase(Ease.InOutSine);
            });
            Delay(0.5f, callBack);
        }
		
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
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
