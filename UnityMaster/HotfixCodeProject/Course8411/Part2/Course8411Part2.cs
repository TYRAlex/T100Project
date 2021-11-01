using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course8411Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;

        private GameObject _bell;
        private GameObject _mask;
        private GameObject _animationSpineGo;
        private GameObject _ui1SpineGo;
        private GameObject _ui2SpineGo;

        private Text _timeTxt;

        private Transform _onClicks;

      
        private int _time;
        private Coroutine _upDataCor;
        private bool _isOver;
       

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var  curTrans = _curGo.transform;

            _bell = curTrans.GetGameObject("bell");
            _mask = curTrans.GetGameObject("mask");
            _animationSpineGo = curTrans.GetGameObject("Spines/animation");
            _ui1SpineGo = curTrans.GetGameObject("Spines/ui1");
            _ui2SpineGo = curTrans.GetGameObject("Spines/ui2");

            _timeTxt = curTrans.GetText("UIs/s-2/time");

            _onClicks = curTrans.Find("OnClicks");

            GameInit();
            GameStart();
        }

        #region 固定
        void GameInit()
        {
            _talkIndex = 1;         
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllCoroutines(); StopAllAudio(); HideVoiceBtn();
            _upDataCor = null;
        
            _time = 180;
            _timeTxt.text = SecToHMS(_time);


            _ui1SpineGo.Show(); _ui2SpineGo.Hide();
         
            HideAllChilds(_onClicks);
            ShowChilds(_onClicks,0);
            AddEvents(_onClicks, OnClickBtns);
            
            _mask.Show();
          
            PlaySpine(_ui1SpineGo, _ui1SpineGo.name);
            PlaySpine(_animationSpineGo, _animationSpineGo.name);
        }

        
        void GameStart()
        {

            PlayCommonBgm(1);
            BellSpeck(_bell, 0, ()=> { _bell.Show(); }, () => { _mask.Hide(); _bell.Hide(); });

        }

        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
            if (_talkIndex == 1)
            {

            }

            _talkIndex++;
        }
        #endregion

        #region 游戏逻辑

        //将秒数转化为时分秒
        private string SecToHMS(int time)
        {

            TimeSpan ts = new TimeSpan(0, 0, time);
            string str = "";
            if (ts.Hours > 0)
            {
                str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = "00:00:" + String.Format("{0:00}", ts.Seconds);
            }
          
            return str;
        }



        private void OnClickBtns(GameObject go)
        {
            
            PlayOnClickSound();
            var name = go.name;
            HideChilds(_onClicks, name);

            switch (name)
            {
                case "ui1":
                    OnClickStartBtn();
                    break;
                case "ui2":
                    OnClickAgainBtn();
                    break;
            }

        }


        private void OnClickStartBtn()
        {
            BellSpeck(_bell, 1, () => { _bell.Show(); PlaySpine(_ui1SpineGo, "ui11"); },()=> { _bell.Hide(); _ui1SpineGo.Hide(); _isOver = false; StartCountDown(); });
            
        }

        private void OnClickAgainBtn()
        {
            BellSpeck(_bell, 1, () => { _bell.Show(); PlaySpine(_ui2SpineGo, "ui22"); }, () => { _bell.Hide(); _ui2SpineGo.Hide(); _isOver = false; _time = 180; StartCountDown(); });         
        }

        private void StartCountDown()
        {

            if (_upDataCor != null)
                return;

            _upDataCor = UpDate(true, 1, () => {

                if (!_isOver)
                {
                    if (_time > 0)
                    {

                        _timeTxt.text = SecToHMS(_time);
                        _time--;
                    }
                    else
                    {
                        if (!_isOver)
                        {
                            _timeTxt.text = SecToHMS(_time);                           
                            _isOver = true;
                            BellSpeck(_bell, 2, ()=> { _bell.Show(); }, () => { _ui2SpineGo.Show(); PlaySpine(_ui2SpineGo, _ui2SpineGo.name, () => { ShowChilds(_onClicks, 1); }); });
                                                                                                      
                        }
                    }
                }
               
            });
        }

     


        #endregion

        #region 常用函数

        #region 语音键显示隐藏
        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion

        #region 隐藏和显示

        private void HideChilds(Transform parent, string name, Action<GameObject> callBack = null)
        {
            var go = parent.Find(name).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }
        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
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

        private void PlaySequenceSpine(GameObject go, List<string> spineNames, Action callBack = null)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames, callBack));
        }

        #endregion

        #region 音频相关

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

        private Coroutine UpDate(bool isStart, float delay, Action callBack=null)
        {
           return  _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
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
                callBack?.Invoke();
                yield return new WaitForSeconds(delay);
               
            }
            
        }

        IEnumerator IEPlaySequenceSpine(GameObject go, List<string> spineNames, Action callBack = null)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
            }
            callBack?.Invoke();
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBell = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBell));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, bool isBell = true, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            if (isBell)
            {
                daiJi = "DAIJI"; speak = "DAIJIshuohua";
            }
            else
            {
                Debug.LogError("Role Spine Name...");
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

        #endregion



    }
}
