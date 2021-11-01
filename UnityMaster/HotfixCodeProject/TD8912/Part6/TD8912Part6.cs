using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;
namespace ILFramework.HotClass
{
    public class TD8912Part6
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _bD;
        private GameObject _partsGo;
        private Transform _pagesTra;
        private Transform _switchsTra;
        private GameObject _mask;

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长
        private bool _isSlide; //是否滑动中
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;
            _bD = curTrans.GetGameObject("BD");


            _partsGo = curTrans.GetGameObject("Parts");
            _pagesTra = curTrans.GetTransform("Parts/Mask/Pages");
            _switchsTra = curTrans.GetTransform("Parts/Switchs");
            _mask = curTrans.GetGameObject("mask");



            GameInit();
        }



        private void GameInit()
        {
            StopAllCoroutines();
            StopAllAudio();
            _bD.Hide();
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = _pagesTra.childCount - 1;
            _moveValue = 1920; _duration =1.0f;
            _isSlide = false;
            SetPos(_pagesTra.GetRectTransform(), new Vector2(0, 0));

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            for (int i = 0; i < _pagesTra.childCount; i++)
            {
                var child = _pagesTra.GetChild(i);
                InitSpines(child);
                AddBtnsEvent(child, OnClickItem);
            }


            InitSpines(_switchsTra,false);
            AddBtnsEvent(_switchsTra, OnClickArrows);

            var rect = _pagesTra.GetRectTransform();
            SlideSwitchPage(_partsGo.gameObject,
                () => { LeftSwitchPage(rect, _moveValue, _duration, () => { _mask.Show(); }, () => { _mask.Hide(); _isSlide = false; }); },
                () => { RightSwitchPage(rect, -_moveValue, _duration, () => { _mask.Show(); }, () => { _mask.Hide(); _isSlide = false; }); });

            RemoveEvent(_mask);
            _mask.Hide();

        }





        private void OnClickItem(GameObject go)
        {
            _mask.Show(); PlayVoice(0);
            var name = go.name;

            var parent = go.transform.parent;
            var spineGo = GetSpineGo(parent, name + "3");
            var spineName1 = name + "4"; //放大
            var spineName2 = name + "3";  //缩小
            spineGo.transform.GetRectTransform().SetAsLastSibling();
            PlaySpine(spineGo, spineName1, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                PlayVoice(1);
                PlaySpine(spineGo, spineName2, () => { _mask.Hide(); });
            }
        }

        private void OnClickArrows(GameObject go)
        {
            if (_isSlide)
                return;

            PlayOnClickSound();
            var name = go.name;
          
            var spineGo = GetSpineGo(_switchsTra, name + "2");
            PlaySpine(spineGo, name);
            var rect = _pagesTra.GetRectTransform();
            switch (name)
            {
                case "L":
                    LeftSwitchPage(rect, _moveValue, _duration, () => { _mask.Show(); }, () => { _mask.Hide(); _isSlide = false; });
                    break;
                case "R":
                    RightSwitchPage(rect, -_moveValue, _duration, () => { _mask.Show(); }, () => { _mask.Hide(); _isSlide = false; });
                    break;
            }
        }


        

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(_bD, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_bD, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bD, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            PlayOnClickSound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (_talkIndex == 1)
            {

            }
            _talkIndex++;
        }


        #region 修改Rect
        private void SetMoveAncPosX(RectTransform rect, float value, float duration,Action callBack1=null,  Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;       
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }

        private void SetPos(RectTransform rect, Vector2 pos)
        {        
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }
        #endregion

        #region 左右切换

        private void SlideSwitchPage(GameObject rayCastTarget, Action leftCallBack, Action rightCallBack)
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
                        leftCallBack?.Invoke();
                    else
                        rightCallBack?.Invoke();
                }
            };
        }

        private void LeftSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex <= _pageMinIndex)
                return;

            if (_isSlide)
                return;

            _isSlide = true;

            _curPageIndex--;          
            SetMoveAncPosX(rect, value, duration, callBack1,callBack2);
        }

        private void RightSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;

            if (_isSlide)
                return;

            _isSlide = true;

            _curPageIndex++;         
            SetMoveAncPosX(rect, value, duration, callBack1, callBack2);
        }
        #endregion

        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            //    string log = string.Format("SpineGoName:{0}---SpineAniName:{1}---time:{2}---isLoop:{3}", go.name, name, time, isLoop);
            //    Debug.Log(log);
            return time;
        }

        private void InitSpines(Transform parent, bool isKong = true, Action callBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;

                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;

                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            callBack?.Invoke();
        }

        private GameObject GetSpineGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 添加Btn监听
        private void AddBtnsEvent(Transform parent, PointerClickListener.VoidDelegate callBack, Action callBack1 = null)
        {
            //   string log = string.Format("parentName:{0}---parentChildCount:{1}", parent.name, parent.childCount);
            // Debug.Log(log);
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;

                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (!isNullSpine)
                    continue;

                RemoveEvent(child);
                AddEvent(child, callBack);
            }
            callBack1?.Invoke();
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            // string log = string.Format("AddEvent GoName:{0}", go.name);
            // Debug.Log(log);
            PointerClickListener.Get(go).onClick = g => {
                //   Debug.Log("OnClick GoName:" + g.name);
                callBack?.Invoke(g);
            };
        }

        private void RemoveEvent(GameObject go)
        {
            // string log = string.Format("RemoveEvent GoName:{0}", go.name);
            //  Debug.Log(log);
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

        #region 播放Audio

        private float PlayBgm(int index, bool isLoop = true, SoundManager.SoundType type = SoundManager.SoundType.BGM)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            // string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            // Debug.Log(log);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            //  string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            //  Debug.Log(log);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            //  string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            //  Debug.Log(log);
            return time;
        }

        /// <summary>
        /// 播放点击声音
        /// </summary>
        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        /// <summary>
        /// 播放失败声音
        /// </summary>
        private void PlayFailSound()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        /// <summary>
        /// 播放成功声音
        /// </summary>
        private void PlaySuccessSound()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        #endregion

        #region 停止Audio
        private void StopAllAudio()
        {
            //  string log = "StopAllAudio";
            //   Debug.Log(log);
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            // string log = string.Format("StopAudio Type:{0}", type);
            // Debug.Log(log);
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            // string log = string.Format("StopAudio Name:{0}", audioName);
            // Debug.Log(log);
            SoundManager.instance.Stop(audioName);
        }
        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            //  string log = "StopAllCoroutines";
            //  Debug.Log(log);
            _mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话
        private void BellSpeck(int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            // string log = string.Format("index:{0}---type:{1}", index, type);
            //   Debug.Log(log);
            _mono.StartCoroutine(SpeckerCoroutine(type, index, specking, speckend));
        }



        #endregion
    }
}
