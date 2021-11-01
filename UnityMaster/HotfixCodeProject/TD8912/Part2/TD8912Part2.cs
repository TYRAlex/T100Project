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
    public class TD8912Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _bD;
        private GameObject _mask;
        private Transform _spines;
        private List<string> _recordOnClickNames;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            _bD = curTrans.GetGameObject("BD");
            _mask = curTrans.GetGameObject("mask");
            _spines = curTrans.Find("Spines");

            GameInit();
        }



        private void GameInit()
        {
            StopAllCoroutines();
            StopAllAudio();
            _recordOnClickNames = new List<string>();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            InitSpines(_spines);
            AddBtnsEvent(_spines, OnClick);
            RemoveEvent(_mask);

            _bD.Show();

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            BellSpeck(0, () => { _mask.Show(); }, () => { _mask.Hide(); _bD.Hide(); });
        }

        void OnClick(GameObject go)
        {
            SoundManager.instance.ShowVoiceBtn(false);

            _mask.Show(); PlayVoice(0);
            var name = go.name;

            bool isContains = _recordOnClickNames.Contains(name);
            if (!isContains)
                _recordOnClickNames.Add(name);

            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            var time = PlaySound(soundIndex);
            var parent = go.transform.parent;
            var spineGo = GetSpineGo(parent, name + "3");
            var spineName1 = name + "4"; //放大
            var spineName2 = name + "3";  //缩小
            spineGo.transform.SetAsLastSibling();
            PlaySpine(spineGo, spineName1);
            Delay(time, () => { AddEvent(_mask, OnClickMask); });
            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                PlayVoice(1);
                PlaySpine(spineGo, spineName2, () => {
                    _mask.Hide();
                    bool isFinish = 3 == _recordOnClickNames.Count;
                    if (isFinish)
                    {  SoundManager.instance.ShowVoiceBtn(true);  }

                });
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
                _bD.Show(); _mask.Show();
                BellSpeck(4);
            }     
            _talkIndex++;
        }


        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);         
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

        #region 播放Audio

        private float PlayBgm(int index, bool isLoop = true, SoundManager.SoundType type = SoundManager.SoundType.BGM)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);        
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);          
            return time;
        }

        private float PlaySound(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);        
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
        private void BellSpeck(int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {         
            _mono.StartCoroutine(SpeckerCoroutine(type, index, specking, speckend));
        }



        #endregion

        #region 添加Btn监听
        private void AddBtnsEvent(Transform parent, PointerClickListener.VoidDelegate callBack, Action callBack1 = null)
        {         
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
            PointerClickListener.Get(go).onClick = g => {         
                callBack?.Invoke(g);
            };
        }

        private void RemoveEvent(GameObject go)
        {        
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

        #region 修改Rect
        private void SetPos(RectTransform rect, Vector2 pos)
        {          
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack = null)
        {
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        #endregion

        #region 延时
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


    }
}
