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
    public class TD91233Part6
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _partsGo;
        private Transform _pagesTra;
        private Transform _switchsTra;


        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


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
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = _pagesTra.childCount - 1;
            _moveValue = 1920; _duration = 1.0f;
            _isSlide = false;
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            SetPos(_pagesTra.GetRectTransform(), new Vector2(0, 0));

            for (int i = 0; i < _pagesTra.childCount; i++)
            {
                var child = _pagesTra.GetChild(i);
                InitSpines(child, false);
                AddEvents(child, OnClickCard);
            }

            InitSpines(_switchsTra, false);

            AddEvents(_switchsTra, OnClickArrows);

            var rect = _pagesTra.GetRectTransform();
            SlideSwitchPage(_partsGo.gameObject,
                () => { LeftSwitchPage(rect, _moveValue, _duration, null, () => { _isSlide = false; IsBorder(_curPageIndex); }); },
                () => { RightSwitchPage(rect, -_moveValue, _duration, null, () => { _isSlide = false; IsBorder(_curPageIndex); }); });

            RemoveEvent(_mask);
            _mask.Hide();
        }

        void GameStart()
        {
            IsBorder(_curPageIndex);
        }


        void TalkClick()
        {
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:

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
                r.Hide(); l.Show();
                PlaySpine(r2, "R4");
                PlaySpine(l2, l2.name);
            }
            else
            {
                l.Show(); r.Show();
                PlaySpine(l2, l2.name);
                PlaySpine(r2, r2.name);
            }
        }

        #region 游戏逻辑

        private void OnClickCard(GameObject go)
        {
            if (_isSlide)
                return;

            _mask.Show(); PlayCommonSound(1);
            var name = go.name;

            var parent = go.transform.parent;
            var spineGo = parent.GetChild(0).gameObject;

            var name1 = name ;        //放大
            var name2 = name + "2";   //缩小

            PlaySpine(spineGo, name1, () => { AddEvent(_mask, OnClickMask); });

            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                PlayCommonSound(2);
                PlaySpine(spineGo, name2, () => { _mask.Hide(); });
            }
        }


        private void OnClickArrows(GameObject go)
        {
            if (_isSlide)
                return;

            PlayOnClickSound();
            var name = go.name;

            var spineGo = FindSpineGo(_switchsTra, name + "2");
            PlaySpine(spineGo, name);
            var rect = _pagesTra.GetRectTransform();
            switch (name)
            {
                case "L":
                    LeftSwitchPage(rect, _moveValue, _duration, null, () => { _isSlide = false; IsBorder(_curPageIndex); });
                    break;
                case "R":
                    RightSwitchPage(rect, -_moveValue, _duration, null, () => { _isSlide = false; IsBorder(_curPageIndex); });
                    break;
            }
        }


        #endregion

        #region 常用函数

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
            SetMoveAncPosX(rect, value, duration, callBack1, callBack2);
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

        private GameObject FindSpineGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private void PlayFailSound()
        {
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        private void PlaySuccessSound()
        {
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
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

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
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

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBD = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBD));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, bool isBD = true, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            if (isBD)
            {
                daiJi = "bd-daiji"; speak = "bd-speak";
            }
            else
            {
                daiJi = "daiji"; speak = "speak";
            }

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
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

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
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

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion

    }
}
