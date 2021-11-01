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
    public class TD6733Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _bg;
        private GameObject _bD;
        private GameObject _mask;

        private Transform _part1Tra;
        private Transform _part2Tra;
        private Transform _part2PagesTra;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
        private List<string> _recordOnClickNames;

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private int _recordOnClickNums;  //记录点击的次数

        private float _moveValue;
        private float _duration;    //切换时长
        private bool _isPlaying;

        private Transform _switchsTra;
        private bool _isSlide; //是否滑动中
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _bg = curTrans.GetGameObject("Bg");
            _bD = curTrans.GetGameObject("BD");
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
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _recordOnClickNames = new List<string>();

            _curPageIndex = 0; _pageMinIndex = 0;
            _pageMaxIndex = _part2PagesTra.childCount-1; 
            _moveValue = 1920; _duration = 1.0f;

            _recordOnClickNums = 6;
            _isPlaying = false;
            _isSlide = false;
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            InitSpines(_part1Tra, false);


            for (int i = 0; i < _part2PagesTra.childCount; i++)
            {
                InitSpines(_part2PagesTra.GetChild(i), false);
                AddEvents(_part2PagesTra.GetChild(i), OnClickCard);
            }

            SetPos(_part1Tra.GetRectTransform(), new Vector2(0, 0));
            SetPos(_part2PagesTra.GetRectTransform(), new Vector2(_moveValue, 0));
            RemoveEvent(_mask);

            _bD.Show(); _part2Tra.gameObject.Hide();

            _switchsTra.gameObject.Hide();
            AddEvents(_switchsTra, OnClickArrows);
            AddEvents(_part1Tra, OnClickMaterials);
            RemoveEvent(_bg);
        }

        void GameStart()
        {
            PlayCommonBgm(6);
            BellSpeck(_bD, 0, () => { _mask.Show(); }, () => { ShowVoiceBtn(); });
        }


        void TalkClick()
        {
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_bD, 1, null, () => { _mask.Hide(); _bD.Show(); });
                    break;         
                case 2:
                    _isPlaying = true;
                    _isSlide = true;
                    _recordOnClickNames.Clear();
                    _recordOnClickNums = 3;
                    _bD.Show(); _part2Tra.gameObject.Show();
                    BellSpeck(_bD, 8, null,
                        () => {
                            SetMoveAncPosX(_part1Tra.GetRectTransform(), -_moveValue, _duration);
                            SetMoveAncPosX(_part2PagesTra.GetRectTransform(), -_moveValue, _duration,null, () => { _isPlaying = false; _isSlide = false; _switchsTra.gameObject.Show(); });
                            _bD.Hide();
                           
                            IsBorder(_curPageIndex);

                            var rect = _part2PagesTra.GetRectTransform();
                            SlideSwitchPage(_bg,
                                () => {
                                    LeftSwitchPage(rect, _moveValue, _duration, () => { HideVoiceBtn(); }, () => {
                                        _isSlide = false; IsBorder(_curPageIndex);
                                        if (_recordOnClickNames.Count == _recordOnClickNums)
                                            ShowVoiceBtn();

                                    });
                                },
                                () => {
                                    RightSwitchPage(rect, -_moveValue, _duration, () => { HideVoiceBtn(); }, () => {
                                        _isSlide = false; IsBorder(_curPageIndex);
                                        if (_recordOnClickNames.Count == _recordOnClickNums)
                                            ShowVoiceBtn();
                                    });
                                });
                        });

                    break;
                case 3:
                    _bD.Show(); _isPlaying = true; _isSlide = true;
                    BellSpeck(_bD, 12);
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

            var spineGo = FindSpineGo(_switchsTra, name + "2");
            PlaySpine(spineGo, name);
            var rect = _part2PagesTra.GetRectTransform();
            switch (name)
            {
                case "L":
                    LeftSwitchPage(rect, _moveValue, _duration, () => { HideVoiceBtn(); }, () => {
                        _isSlide = false; IsBorder(_curPageIndex);
                        if (_recordOnClickNames.Count == _recordOnClickNums)
                            ShowVoiceBtn();
                    });
                    break;
                case "R":
                    RightSwitchPage(rect, -_moveValue, _duration, () => { HideVoiceBtn(); }, () => {
                        _isSlide = false; IsBorder(_curPageIndex);
                        if (_recordOnClickNames.Count == _recordOnClickNums)
                            ShowVoiceBtn();
                    });
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

            var spineGo = FindSpineGo(_part1Tra, "0");
            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            PlaySpine(spineGo, name);
            BellSpeck(_bD, soundIndex, null,
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
            var spineGo = FindSpineGo(parent, name + "3");

            spineGo.transform.SetAsLastSibling();

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            var spineName1 = name;          //放大
            var spineName2 = name + "2";    //缩小

            var time = PlaySound(soundIndex);
            PlaySpine(spineGo, spineName1);

            Delay(time, () => {  AddEvent(_mask, OnClickMask); });

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
