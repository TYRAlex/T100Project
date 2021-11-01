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
    public class TD5623Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        
        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _smallDD;
        private GameObject _bigDD;
        private GameObject _nextSpine;

        private Transform _spines;
        private GameObject _spine1;
        private Transform _btn1s;

        private Transform _spine3Parent;
        private GameObject _spine0;
        private GameObject _spineZ;


        private RectTransform _spine2ButterflyRect;
        private RectTransform _spine2TigerRect;
        private GameObject _spine2Butterfly;
        private GameObject _spine2Tiger;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长

        private bool _isPlaying;

        private CanvasGroup _b2GanvasGroup;
        private CanvasGroup  _uIs1GanvasGroup;
        private CanvasGroup _uIs2GanvasGroup;
        private int _levelId;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");
            _nextSpine = curTrans.GetGameObject("nextSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");
            _smallDD = curTrans.GetGameObject("SmallDD");
            _bigDD = curTrans.GetGameObject("BigDD");
        

            _spines = curTrans.Find("Spines");
            _spine1 = curTrans.GetGameObject("Spines/Spine1Parent/Spine1");
            _btn1s = curTrans.Find("Spines/Spine1Parent/Btn1s");

            _spine2ButterflyRect = curTrans.GetRectTransform("Spines/Spine2Parent/h");
            _spine2TigerRect = curTrans.GetRectTransform("Spines/Spine2Parent/lh");

            _spine2Butterfly = curTrans.GetGameObject("Spines/Spine2Parent/h");
            _spine2Tiger = curTrans.GetGameObject("Spines/Spine2Parent/lh");

            _spine3Parent = curTrans.Find("Spines/Spine3Parent");
            _spine0 = curTrans.GetGameObject("Spines/Spine3Parent/0");
            _spineZ = curTrans.GetGameObject("Spines/Spine3Parent/z");

            _b2GanvasGroup = curTrans.Find("Spines/Spine1Parent/B2").GetComponent<CanvasGroup>();
            _uIs1GanvasGroup = curTrans.Find("Spines/Spine2Parent/UIs1").GetComponent<CanvasGroup>();
            _uIs2GanvasGroup = curTrans.Find("Spines/Spine2Parent/UIs2").GetComponent<CanvasGroup>();
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = 0;
            _moveValue = 1920; _duration = 1.0f;
            _isPlaying = false;
            _b2GanvasGroup.alpha = 1;
            _uIs1GanvasGroup.alpha = 0;
            _uIs2GanvasGroup.alpha = 1;
            _levelId = 1;
        }

        void GameInit()
        {
            InitData();

            SetPos(_spine2ButterflyRect, new Vector2(1700, 195));
            SetPos(_spine2TigerRect, new Vector2(2000, -269));

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _nextSpine.Hide();
            _smallDD.Hide(); _bigDD.Hide();
            _spine0.Show();
            _spineZ.Show();
            _spineZ.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _spine0.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            HideAllChilds(_spines);

            ShowChilds(_spines, 0, (go) => { PlaySpine(_spine1, "kong", () => {PlaySpine(_spine1,"x0",null,true); });});
                  
            AddEvents(_btn1s, OnClickIcon);  AddEvents(_spine3Parent,OnClickEvent);
        }

     

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);
                        _smallDD.Show(); _startSpine.Hide();
                        BellSpeck(_smallDD, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); },false);
                    });
                });
            });
        }


        void TalkClick()
        {
            PlayOnClickSound();        
            switch (_talkIndex)
            {             
                case 1:
                    BellSpeck(_smallDD, 1, null, StartGame,false);
                    break;
                case 2:
                    BellSpeck(_smallDD, 3, null, ()=> {                     
                        _mask.Hide(); _smallDD.Hide(); 
                    }, false);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


        private void OnClickEvent(GameObject go)
        {
            if (_isPlaying)
                return;

            PlayOnClickSound();
            _isPlaying = true;

            var name = go.name;

            switch (_levelId)
            {
                case 1:
                    switch (name)
                    {
                        case "1":
                            PlaySpine(_spine0,"h-a1");
                            Delay(PlayFailSound(),()=> { _isPlaying = false; });
                            break;
                        case "2":
                            PlaySpine(_spine0, "h-a2");
                            Delay(PlayFailSound(), () => { _isPlaying = false; });
                            break;
                        case "3":
                            PlayVoice(1);
                            PlaySuccessSound();
                            Delay(PlaySpine(_spine0, "h-a3"), () => {  _levelId++; PlaySpine(_spine0, "h-b0", () => { _isPlaying = false; }); });
                            break;
                    }
                    break;
                case 2:
                    switch (name)
                    {
                        case "1":
                            PlaySpine(_spine0, "h-b1");
                            Delay(PlayFailSound(), () => { _isPlaying = false; });
                            break;
                        case "2":
                            PlayVoice(1);
                            PlaySuccessSound();
                            Delay(PlaySpine(_spine0, "h-b3"), () => { _levelId++; PlaySpine(_spine0, "h-c0", () => { _isPlaying = false; }); });
                            break;
                        case "3":
                            PlaySpine(_spine0, "h-b2");
                            Delay(PlayFailSound(), () => { _isPlaying = false; });
                            break;
                    }
                    break;
                case 3:
                    switch (name)
                    {
                        case "1":
                            PlaySpine(_spine0, "h-c1");
                            Delay(PlayFailSound(), () => { _isPlaying = false; });
                            break;
                        case "2":
                            PlaySpine(_spine0, "h-c2");
                            Delay(PlayFailSound(), () => { _isPlaying = false; });
                            break;
                        case "3":
                            PlayVoice(1);
                            PlaySuccessSound();
                            Delay(PlaySpine(_spine0, "h-c3"), () => {

                             
                               _spine0.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(() => { _spine0.Hide(); });

                                Delay(0.8f, () => {
                                   
                                    PlaySpine(_spineZ, "kong", () => {
                                        _spineZ.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                                        PlayBgm(1);
                                        PlayVoice(4); PlaySpine(_spineZ, _spineZ.name, () => {                                        
                                            Delay(2, GameSuccess); });
                                    });
                                });
                            

                              
                            
                                 
                                //_uIs1GanvasGroup.DOFade(0, 0.5f);
                                //_uIs2GanvasGroup.DOFade(0, 0.5f);
                               
                           
                               
                                                                                                        
                            });
                            break;
                    }
                    break;
            }
        }

        private void OnClickIcon(GameObject go)
        {
            if (_isPlaying)            
                return;
            
            PlayOnClickSound();
            _isPlaying = true;

            var name = go.name;

            switch (name)
            {
                case "x1":
                case "x2":
                    var failSoundTime = PlayFailSound();
                    PlaySpine(_spine1, name, () => { PlaySpine(_spine1, "x0", null, true); }, false);
                    Delay(failSoundTime, () => { _isPlaying = false; });
                    break;               
                case "x3":
                    PlayVoice(2);
                    PlaySuccessSound();                
                    PlaySpine(_spine1, name,()=> {
                        PlayVoice(3);
                        PlaySpine(_spine1, "x4", ButterflyAndTigerAni);
                    });
                    break;
                case "tigerIcon":
                    var tigerVoiceTime = PlayVoice(0);
                    PlaySpine(_spine1, "x0-hou", () => { PlaySpine(_spine1, "x0", null, true); });
                    Delay(tigerVoiceTime, () => { _isPlaying = false; });
                    break;
            }
        }

        private void ButterflyAndTigerAni()
        {
            PlayBgm(1);
            PlayVoice(5);
            ShowChilds(_spines, 1);
           
            _b2GanvasGroup.DOFade(0, 0.5f);
            _uIs1GanvasGroup.DOFade(1, 0.5f);

            Delay(0.5f, () => { HideChilds(_spines,0); });

            PlaySpine(_spine2Butterfly, "kong", () => { PlaySpine(_spine2Butterfly,_spine2Butterfly.name,null,true); });
            PlaySpine(_spine2Tiger, "kong", () => { PlaySpine(_spine2Tiger, _spine2Tiger.name, null, true); });

          
            SetMove(_spine2ButterflyRect, new Vector2(-1700, 195),6);

            Tweener TigerMove = _spine2TigerRect.DOAnchorPos(new Vector2(-2000, -269),8F).SetEase(Ease.Linear);

            DOTween.Sequence().Append(TigerMove)
                               .InsertCallback(3F, () => {                                
                                   PlaySpine(_spine2Tiger, _spine2Tiger.name + "2",()=> {PlaySpine(_spine2Tiger, _spine2Tiger.name, null, true); });
                               });

            Delay(7.2f, NextLevel);
        }

        private void NextLevel()
        {
            _mask.Show();
            _nextSpine.Show();
            PlaySpine(_nextSpine, "next2",()=> {
                AddEvent(_nextSpine, go => {
                    PlayOnClickSound();
                    PlayBgm(0);
                    RemoveEvent(_nextSpine);
                    PlaySpine(_nextSpine, "next", () => {
                        _nextSpine.Hide();
                        _smallDD.Show();
                        _spine3Parent.gameObject.Show();
                        _spine0.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                        PlaySpine(_spine0, "kong", () => {
                            _spine0.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                            PlaySpine(_spine0, "h-a0", () => {
                            
                            _isPlaying = false; }); });
                    
                        BellSpeck(_smallDD, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); }, false);
                    });                                   
                });
               
            });         
        }
       


        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _smallDD.Hide(); _mask.Hide();          
        }

        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show(); _okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();
                        PlayBgm(0);
                        GameInit();
                        StartGame();
                        _talkIndex = 2;
                    });
                });
            });
           

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
                    StopAudio(SoundManager.SoundType.BGM);
                    PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();
                        _bigDD.Show();
                        BellSpeck(_bigDD, 4, null, null, false);
                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4, GameReplayAndOk);
        }

        #endregion

        #region 常用函数

        #region 隐藏和显示

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null,bool isAlpha=false,float duration=0)
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
            SetMoveAncPosX(rect, value, duration, callBack1, callBack2);
        }

        private void RightSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;
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

        private float PlayFailSound()
        {
           PlayCommonSound(5);
          //  PlayVoice(3);
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
           // PlayCommonSound(4);
          //  PlayVoice(2);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time =  SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
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
                daiJi = "animation"; speak = "animation2";
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

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null,Ease type = Ease.Linear)
        {
            rect.DOAnchorPos(v2, duration).SetEase(type)
                .OnComplete(() => { callBack?.Invoke(); });
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