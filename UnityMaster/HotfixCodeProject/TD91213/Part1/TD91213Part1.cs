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
    public class TD91213Part1
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _bD;
        private GameObject _mask;

        private Transform _part1Tra;
        private Transform _part3Tra;
        private Transform _part3PagesTra;
      
        private List<string> _recordOnClickNames;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private float _moveValue;
        private float _duration;    //切换时长
        private int _recordOnClickNums;  //记录点击的次数

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _bD = curTrans.Find("BD").gameObject;
            _mask = curTrans.GetGameObject("mask");

            _part1Tra = curTrans.Find("Parts/1");
            _part3Tra = curTrans.Find("Parts/3");
            _part3PagesTra = curTrans.Find("Parts/3/Mask/Pages");


            GameInit();
        }

        void GameInit()
        {
            InitData();
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            InitSpines(_part1Tra,false); AddEvents(_part1Tra, OnClick1Child);

            for (int i = 0; i < _part3PagesTra.childCount; i++)
            {
                InitSpines(_part3PagesTra.GetChild(i),false);
                AddEvents(_part3PagesTra.GetChild(i), OnClick3Child);
            }

            SetPos(_part1Tra.GetRectTransform(), new Vector2(0, 0));
            SetPos(_part3PagesTra.GetRectTransform(), new Vector2(_moveValue, 0));
            RemoveEvent(_mask);

            _bD.Show(); _part3Tra.gameObject.Hide();

            GameStart();
        }

        void InitData()
        {
            _succeedSoundIds = new List<int> { 4, 5, 8, 10, 12 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _recordOnClickNames = new List<string>();
            _recordOnClickNums = 5;
            _curPageIndex = 0;
            _pageMinIndex = 0;
            _pageMaxIndex = _part3PagesTra.childCount - 1;
            _duration = 1f;
            _moveValue = 1920f;
        }


        void GameStart()
        {
            PlayCommonBgm(6);
            BellSpeck(0, _bD, () => { _mask.Show(); },()=> { _mask.Hide();_bD.Hide(); });
        }

        #region 游戏逻辑

        /// <summary>
        /// 点击认识材料
        /// </summary>
        /// <param name="go"></param>
        void OnClick1Child(GameObject go)
        {
            var name = go.name;
            SoundManager.instance.ShowVoiceBtn(false);
            bool isContains = _recordOnClickNames.Contains(name);
            if (!isContains)
                _recordOnClickNames.Add(name);

            PlayCommonSound(6);
            var spineGo = FindSpineGo(_part1Tra, "TLGN_LC4_02_ABCDE");
            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            BellSpeck(soundIndex, _bD,() => { _mask.Show(); PlaySpine(spineGo, name); },
                () => {
                    _mask.Hide();
                    bool isFinish = _recordOnClickNums == _recordOnClickNames.Count;
                    if (isFinish)
                    {
                      //  _mask.Show();
                        SoundManager.instance.ShowVoiceBtn(true);
                      //  _recordOnClickNames.Clear();
                      //  _recordOnClickNums = 1;
                    }
                });
        }

        /// <summary>
        /// 点击图片
        /// </summary>
        /// <param name="go"></param>
        void OnClick3Child(GameObject go)
        {

            SoundManager.instance.ShowVoiceBtn(false);
            _mask.Show(); PlayCommonSound(1);
            var name = go.name;

            bool isContains = _recordOnClickNames.Contains(name);
            if (!isContains)
                _recordOnClickNames.Add(name);

            var parent = go.transform.parent;
            var spineGo = FindSpineGo(parent, "TLGN_LC4_3-4");

            var soundIndex = int.Parse( go.transform.GetChild(0).name);

            var spineName1 = string.Empty; //放大
            var spineName2 = string.Empty; //缩小
            switch (name)
            {
                case "a":
                    spineName1 = "TLGN_LC4_3-4_3a";
                    spineName2 = "TLGN_LC4_3-4_3c";
                    break;
                case "b":
                    spineName1 = "TLGN_LC4_3-4_4a";
                    spineName2 = "TLGN_LC4_3-4_4c";
                    break;
            }
        
            spineGo.transform.SetAsLastSibling();
            var time =  PlaySound(soundIndex);

            PlaySpine(spineGo, spineName1);

            Delay(time, () => { AddEvent(_mask, OnClickMask); });
            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                PlayCommonSound(2);
                PlaySpine(spineGo, spineName2, () => {
                    _mask.Hide();
                    if (_recordOnClickNames.Count== _recordOnClickNums)                    
                        SoundManager.instance.ShowVoiceBtn(true);                    
                });
            }

        }

        #endregion


        #region 常用函数

        #region Spine相关

        private void InitSpines(Transform parent,bool isKong =true,Action initCallBack=null)
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
            var time = SoundManager.instance.PlayClip( SoundManager.SoundType.COMMONSOUND, index, isLoop);
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

        private void BellSpeck(int index, GameObject go, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend));
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

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack = null)
        {
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack?.Invoke(); });
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
            _curPageIndex--;
            callBack1?.Invoke();
            SetMoveAncPosX(rect, value, duration, callBack2);
        }

        private void RightSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;
            _curPageIndex++;
            callBack1?.Invoke();
            SetMoveAncPosX(rect, value, duration, callBack2);
        }
        #endregion

        #endregion


        //bell说话协程
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

        void TalkClick()
        {
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:

                     _mask.Show();                
                      _recordOnClickNames.Clear();
                     _recordOnClickNums = 1;

                    _bD.Show(); _part3Tra.gameObject.Show();
                    BellSpeck(6, _bD,null,() => {
                        _bD.Hide();
                        SetMoveAncPosX(_part1Tra.GetRectTransform(), -_moveValue, _duration);
                        SetMoveAncPosX(_part3PagesTra.GetRectTransform(), -_moveValue, _duration, () => {

                            _mask.Hide();
                            //_bD.Show();
                            //BellSpeck(7, _bD,null, () => {
                            //   _bD.Hide();
                            //});                         
                        });
                    });
                    break;
                case 2:
                    _bD.Show(); _mask.Show();
                    BellSpeck(8, _bD);
                    break;
            }
            _talkIndex++;
        }



    }
}
