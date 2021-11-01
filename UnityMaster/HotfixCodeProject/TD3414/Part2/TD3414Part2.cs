using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using Spine.Unity;

namespace ILFramework.HotClass
{
   
    public class TD3414Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;   
        private GameObject _bD;          
        private GameObject _mask;

        private Transform _part1Tra;
       
        private Transform _part3Tra;
        private Transform _part3PagesTra;

       
        private List<string> _spine1Names;
        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长

        void Start(object o)
        {
           
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
             Transform  curTrans = _curGo.transform;                   
            _bD = curTrans.Find("BD").gameObject;
            _mask = curTrans.GetGameObject("mask");

            _part1Tra = curTrans.Find("Parts/1");
          
            _part3Tra = curTrans.Find("Parts/3");
            _part3PagesTra = curTrans.Find("Parts/3/Pages");

        
            GameInit();
        }

 


        private void GameInit()
        {
            _spine1Names = new List<string>();

            StopAllAudio();
            StopAllCoroutines();

           

            _curPageIndex = 0; _pageMinIndex = 0;
            _pageMaxIndex = _part3PagesTra.childCount-1;
            _duration =1f; _moveValue = 1920f;

             _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            InitSpines(_part1Tra); AddBtnsEvent(_part1Tra, OnClick1Child);

          
            for (int i = 0; i < _part3PagesTra.childCount; i++)
            {
                InitSpines(_part3PagesTra.GetChild(i));
                AddBtnsEvent(_part3PagesTra.GetChild(i), OnClick3Child);
            }
            _part3Tra.gameObject.Hide();

            SetPos(_part1Tra.GetRectTransform(), new Vector2(0, 0));
            SetPos(_part3PagesTra.GetRectTransform(), new Vector2(_moveValue, 0));
            RemoveEvent(_mask);
        
            _bD.Show();
            GameStart();

        }

        void GameStart()
        {
            PlayBgm(6, true, SoundManager.SoundType.COMMONBGM);
            BellSpeck(0, () => { _mask.Show(); }, () => { _mask.Hide(); _bD.Hide(); });
        }

        void OnClick1Child(GameObject go)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            var name = go.name;
            
            bool isContains= _spine1Names.Contains(name);
            if (!isContains)
                _spine1Names.Add(name);

            PlayVoice(0);
            var spineGo = GetSpineGo(_part1Tra,"0");
            var soundIndex = int.Parse(go.transform.GetChild(0).name);         
            BellSpeck(soundIndex, () => { _mask.Show(); PlaySpine(spineGo, name); },
                ()=> {
                    _mask.Hide();
                    bool isFinish = 10 == _spine1Names.Count;
                    if (isFinish)
                    {    //_mask.Show();
                        SoundManager.instance.ShowVoiceBtn(true);  }
                });
        }
   

        void OnClick3Child(GameObject go)
        {
            _mask.Show();  PlayVoice(1);
            var name = go.name;
            SoundManager.instance.ShowVoiceBtn(false);
            var parent = go.transform.parent;
            var spineGo = GetSpineGo(parent, name + "3");
            var spineName1 = name+"4"; //放大
            var spineName2 = name + "3";  //缩小
            spineGo.transform.SetAsLastSibling();
            PlaySpine(spineGo, spineName1);

            var soundIndex = int.Parse(go.transform.GetChild(0).name);
            var time = PlaySound(soundIndex);
            Delay(time, () => { AddEvent(_mask, OnClickMask); });

         
            void OnClickMask(GameObject g)
            {
                RemoveEvent(g);
                PlayVoice(2);
                PlaySpine(spineGo, spineName2,()=> { _mask.Hide(); SoundManager.instance.ShowVoiceBtn(true); });
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
            switch (_talkIndex)            {
                case 1:
                    FirstOnClickVoice();
                    break;
                case 2:
                    TwoOnClickVoice();
                    break;
            }
            _talkIndex++;
        }


        #region 点击语音键

        void FirstOnClickVoice()
        {
            _mask.Show();
            _bD.Show(); _part3Tra.gameObject.Show();
            BellSpeck(11, null, () => {
                _bD.Hide();
                SetMoveAncPosX(_part1Tra.GetRectTransform(), -_moveValue, _duration);
                SetMoveAncPosX(_part3PagesTra.GetRectTransform(), -_moveValue, _duration,()=> {
                    _mask.Hide(); 
                 
                });
            });   
        }

        void TwoOnClickVoice()
        {
            _bD.Show(); _mask.Show();
            BellSpeck(12);
        }

        #endregion

        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
           // string log = string.Format("SpineGoName:{0}---SpineAniName:{1}---time:{2}---isLoop:{3}", go.name, name, time, isLoop);
           // Debug.Log(log);
            return time;
        }

        private void InitSpines(Transform parent,bool isKong=true, Action callBack=null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;

                var isNullSpine = child.GetComponent<SkeletonGraphic>()==null;
                if (isNullSpine)
                    continue;

                if (isKong)                
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            callBack?.Invoke();
        }

        private GameObject GetSpineGo(Transform parent,string goName)
        {         
            return parent.Find(goName).gameObject;    
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
           // string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
           // Debug.Log(log);
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
           // string log = "StopAllAudio";
           // Debug.Log(log);
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
           // string log = "StopAllCoroutines";
           // Debug.Log(log);
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
            //string log = string.Format("index:{0}---type:{1}", index, type);
            //Debug.Log(log);
            _mono.StartCoroutine(SpeckerCoroutine(type, index, specking, speckend));
        }

    

        #endregion
            
        #region 添加Btn监听
        private void AddBtnsEvent(Transform parent, PointerClickListener.VoidDelegate callBack,Action callBack1=null)
        {
           // string log = string.Format("parentName:{0}---parentChildCount:{1}", parent.name, parent.childCount);
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
           // Debug.Log(log);
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

        #region 修改Rect
        private void SetPos(RectTransform rect, Vector2 pos)
        {
           // string log = string.Format("rect:{0}---pos:{1}", rect.name, pos);
          //  Debug.Log(log);
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
   
        private void LeftSwitchPage(RectTransform rect, float value, float duration,Action callBack1 = null,Action callBack2 =null)
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
    }
}
