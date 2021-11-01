using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
namespace ILFramework.HotClass
{
    public class Course722Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bell;      
        private GameObject _spines;      
        private GameObject _spine0;
        private Transform _onClicks;
        private GameObject _mask;

        private bool _isPlaying;
        private List<string> _recordOnClickName;
        private int _recordOnClickNum;

        void Start(object o)
        {
         
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();

            _bell = curTrans.GetGameObject("bell");          
            _spines = curTrans.GetGameObject("Spines");
           
            _spine0 = curTrans.GetGameObject("Spines/0");
            _onClicks = curTrans.Find("OnClicks");

            _mask = curTrans.GetGameObject("mask");


            GameInit();
            GameStart();
        }
    
     
        void GameInit()
        {
            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _spines.Hide(); _mask.Show(); _bell.Show();
            RemoveEvent(_mask); 
            AddEvents(_onClicks, OnClicks);
            _isPlaying = false;

            _recordOnClickName = new List<string>();
            _recordOnClickNum = _onClicks.childCount;

            _spine0.GetComponent<SkeletonGraphic>().Initialize(true);
        }

        

        void GameStart()
        {
            PlayBgm(0);
            BellSpeck(_bell, 0, null, () => {ShowVoiceBtn(); });

        }


        #region 游戏逻辑

        private void OnClicks(GameObject go)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            PlayCommonSound(6);
            HideVoiceBtn();
         
            var name = go.name;

            var isContain = _recordOnClickName.Contains(name);

            if (!isContain)            
                _recordOnClickName.Add(name);

            var isFinish = _recordOnClickName.Count == _recordOnClickNum;

            switch (name)
            {
                case "EngineBtn":   //发动机舱
                    Delay(1.0f, () => { PlayCommonSound(1); });
                    Delay(1.667f, () => { PlayVoice(2); });
                    PlaySequenceSpine(_spine0, new List<string> { "g3", "ka1", "ka2" });
                    BellSpeck(_bell, 1, null, () => {
                        _mask.Show();
                        AddEvent(_mask, m => {
                            PlayCommonSound(2);
                            RemoveEvent(_mask);
                            PlaySpine(_spine0, "ka3", () => {
                                if (isFinish) { ShowVoiceBtn(); }
                                _isPlaying = false;
                                _mask.Hide();
                            });
                        });
                    });
                    break;
                case "MainScrewBtn": //主螺旋桨
                    Delay(1, () => { PlayVoice(0); });                  
                    PlaySequenceSpine(_spine0, new List<string> { "g4", "xz1" });
                    BellSpeck(_bell, 2, null, () => {
                        if (isFinish) { ShowVoiceBtn(); }
                        _isPlaying = false;
                    });
                    break;
                case "TailrotorBtn": //尾桨
                    Delay(1, () => { PlayVoice(1); });                
                    PlaySequenceSpine(_spine0, new List<string> { "g7", "xz2" });
                    BellSpeck(_bell, 3, null, () => {
                        if (isFinish) { ShowVoiceBtn(); }
                        _isPlaying = false;
                    });
                    break;
                case "Cockpit1Btn":  //驾驶舱
                    Delay(1.0f, () => { PlayCommonSound(1); });
                    PlaySequenceSpine(_spine0, new List<string> { "g2", "kb1" });
                    BellSpeck(_bell, 4, null, () => {
                        _mask.Show();
                        AddEvent(_mask, m => {
                            PlayCommonSound(2);
                            RemoveEvent(_mask);
                            PlaySpine(_spine0, "kb3", () => {
                                if (isFinish) { ShowVoiceBtn(); }
                                _isPlaying = false;
                                _mask.Hide();
                            });
                        });
                    });
                    break;
                case "Cockpit2Btn":  //座舱
                    Delay(1.0f, () => { PlayCommonSound(1); });
                    PlaySequenceSpine(_spine0, new List<string> { "g1", "kc1" });
                    BellSpeck(_bell, 5, null, () => {
                        _mask.Show();
                        AddEvent(_mask, m => {
                            PlayCommonSound(2);
                            RemoveEvent(_mask);
                            PlaySpine(_spine0, "kc3", () => {
                                if (isFinish) { ShowVoiceBtn(); }
                                _isPlaying = false;
                                _mask.Hide();
                            });
                        });
                    });
                    break;
                case "UndercarriageBtn": //起落架       
                    Delay(1.0f, () => { PlayCommonSound(1); });
                    PlaySequenceSpine(_spine0, new List<string> { "g5", "kd1"});
                    BellSpeck(_bell,6,null,()=> { _mask.Show();
                        AddEvent(_mask, m => {RemoveEvent(_mask);
                            PlayCommonSound(2);
                            PlaySpine(_spine0, "kd3", ()=> {
                                if (isFinish) { ShowVoiceBtn(); }
                                _isPlaying = false;
                                _mask.Hide();
                            });
                        });                       
                    });
                    break;
                case "TailBeamBtn":  //尾梁
                    PlaySpine(_spine0, "g6");
                    BellSpeck(_bell, 7,null,()=> {
                        if (isFinish) { ShowVoiceBtn(); }                         
                        _isPlaying = false;                      
                    });
                    break;
            }
        }

        #endregion


        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();
        
            switch (_talkIndex)
            {
                case 1:
                    _bell.Hide();
                    _spines.Show(); _spine0.Show();
                    PlaySpine(_spine0, _spine0.name);
                    BellSpeck(_bell, 9,null,()=> { _mask.Hide();  });
                    break;
                case 2:                
                    _bell.Show(); _mask.Show();             
                    BellSpeck(_bell, 8);                 
                    break;

            }         
            _talkIndex++;
        }

     

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

        private void PlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames));
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

        IEnumerator IEPlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
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
