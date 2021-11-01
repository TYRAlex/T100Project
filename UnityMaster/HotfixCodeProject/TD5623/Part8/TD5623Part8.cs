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
    public class TD5623Part8
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
        private GameObject _smallTT;
        private GameObject _bigTT;


        private Transform _enemyTra;
        private Transform _scoresTra;
        private Transform _spines;


        private Transform _tigersTra;
        private RectTransform _0A2;
        private RectTransform _1A2;

        private RectTransform _0A3;
        private RectTransform _1A3;


        private RectTransform _0A5;
        private RectTransform _1A5;

        private RectTransform _0c;
        private RectTransform _1c;

        private GameObject _tigerAni;
        private CanvasGroup _tigerAniCanGroup;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Dictionary<int, bool> _levelState;
        private Dictionary<int, Vector2> _tigersInitPos;

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长

        private bool _isPlaying;
        private int _curIndex;

       

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");


            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");
            _smallTT = curTrans.GetGameObject("SmallTT");
            _bigTT = curTrans.GetGameObject("BigTT");


            _enemyTra = curTrans.Find("Bg/Score/C9/Enemy");
            _scoresTra = curTrans.Find("Bg/Score/C9/Scores");
            _spines = curTrans.Find("Spines");
            _curIndex = -1;



            _tigersTra = curTrans.Find("TigerAni/Ani/Tigers");
            _0A2 = curTrans.GetRectTransform("TigerAni/UIs0/0A2");
            _1A2 = curTrans.GetRectTransform("TigerAni/UIs0/1A2");


            _0A3 = curTrans.GetRectTransform("TigerAni/Ani/UIs1/0A3");
            _1A3 = curTrans.GetRectTransform("TigerAni/Ani/UIs1/1A3");

            _0A5 = curTrans.GetRectTransform("TigerAni/Ani/UIs2/0A5");
            _1A5 = curTrans.GetRectTransform("TigerAni/Ani/UIs2/1A5");

            _0c = curTrans.GetRectTransform("TigerAni/Ani/UIs3/0c");
            _1c = curTrans.GetRectTransform("TigerAni/Ani/UIs3/1c");

            _tigerAni = curTrans.GetGameObject("TigerAni");
            _tigerAniCanGroup = curTrans.Find("TigerAni").GetComponent<CanvasGroup>();


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

            _levelState = new Dictionary<int, bool>();
            _levelState.Add(0, false);
            _levelState.Add(1, false);
            _levelState.Add(2, false);
            _levelState.Add(3, false);
            _levelState.Add(4, false);

            _tigersInitPos = new Dictionary<int, Vector2>();
            _tigersInitPos.Add(0, new Vector2(596, 108));
            _tigersInitPos.Add(1, new Vector2(43, 39));
            _tigersInitPos.Add(2, new Vector2(-350, -98));
            _tigersInitPos.Add(3, new Vector2(486, -259));
            _tigersInitPos.Add(4, new Vector2(-125, -265));
            _tigersInitPos.Add(5, new Vector2(-729, 123));

        }

        void GameInit()
        {
            InitData();

            DOTween.KillAll();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();
            _smallTT.Hide(); _bigTT.Hide(); _tigerAni.Hide();
            _tigerAniCanGroup.alpha = 0;

            HideAllChilds(_spines); HideAllChilds(_enemyTra); HideAllChilds(_scoresTra);

            for (int i = 0; i < _spines.childCount; i++)
            {
                var btns = _spines.GetChild(i).Find("Btns");
                AddEvents(btns, OnClickEvent);
            }

            InitAnimatotPos();

            for (int i = 0; i < _tigersTra.childCount; i++)
            {
                var child = _tigersTra.GetChild(i);
                var rect = child.GetRectTransform();
                var pos = _tigersInitPos[i];
                rect.anchoredPosition = pos;
            }
          
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show(); ShowLevel();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                         PlayBgm(0);
                        _smallTT.Show(); _startSpine.Hide();
                        BellSpeck(_smallTT, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }, false);
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
                    BellSpeck(_smallTT, 1, null, StartGame, false);
                    break;
                case 2:

                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


        private void OnClickEvent(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            PlayOnClickSound();

            var name = go.name;
            var spineGp = go.transform.parent.parent.Find("Spine").GetChild(0).gameObject;

            switch (name)
            {                                                    
                case "tm-x-a4":    //正确                  
                case "tm-x-b4":    
                case "tm-x-c4":   
                case "tm-x-d4":   
                case "tm-x-e4":
                    PlaySuccessSound();
                    _levelState[_curIndex] = true;                                  
                    PlaySpine(spineGp, name,()=> {
                        HideChilds(_spines, _curIndex);
                        ShowLevel();
                    });
                    break;
                default:           //错误   
                    PlayFailSound();
                    PlaySpine(spineGp, name,()=> {
                        HideChilds(_spines, _curIndex);
                        ShowLevel(); } );                                 
                    break;
            }

        }

        private void ShowSocreAndEnmeyIcon()
        {
            HideAllChilds(_enemyTra); HideAllChilds(_scoresTra);
            var num = CurPassNums();
            ShowChilds(_scoresTra, num);

            if (num==5)            
                ShowChilds(_enemyTra, 1);
            else
                ShowChilds(_enemyTra, 0);      
        }

        private void ShowLevel()
        {

            ShowSocreAndEnmeyIcon();

            bool isFinish = IsAllPass();

            if (isFinish)
            {             
                _tigerAni.Show();
                Delay(1f, ()=> {
                    TigerAnimator();
                    _tigerAniCanGroup.DOFade(1, 0.5f).OnComplete(()=> { PlayVoice(2,true); }); ;

                    Delay(10f, GameSuccess);
                });
                return;
            }
           
            while (true)
            {               
                _curIndex = Random.Range(0, 5);
                var isPass = _levelState[_curIndex];

                if (!isPass)
                    break;
            }
            _isPlaying = true;
            ShowChilds(_spines, _curIndex, go=> {
                var spineGo = go.transform.Find("Spine").GetChild(0).gameObject;

                string kName = string.Empty;

                switch (spineGo.name)
                {
                    case "tm-x-a":
                        kName = "kong-a";
                        break;
                    case "tm-x-b":
                        kName = "kong-b";
                        break;
                    case "tm-x-e":
                        kName = "kong-e";
                        break;
                    default:
                        kName = "kong";
                        break;
                }

                Delay(PlaySpine(spineGo, kName),()=> {
                    Delay(1.33f, () => { _isPlaying = false; });
                    PlaySpine(spineGo,spineGo.name,null,true);
                });
            });
        }


        private bool IsAllPass()
        {
            bool isAllPass = true;
            foreach (var item in _levelState)
            {
                if (!item.Value)
                {
                    isAllPass = false;
                    break;
                }
            }
            return isAllPass;
        }

        private int CurPassNums()
        {
            int num = 0;

            foreach (var item in _levelState)
            {
                if (item.Value)               
                    num++;                
            }
            return num;
        }

        private void InitAnimatotPos()
        {
            var startPos1 = new Vector2(-1920, 0);

            _0A2.anchoredPosition = startPos1;
            _1A2.anchoredPosition = Vector2.zero;
          
            _0A5.anchoredPosition = startPos1;
            _1A5.anchoredPosition = Vector2.zero;

            _0c.anchoredPosition = new Vector2(-1920,0);
            _1c.anchoredPosition = Vector2.zero;

            _0A3.anchoredPosition = new Vector2(-1920, 641);
            _1A3.anchoredPosition = new Vector2(0, 641);

        }

        

        private void TigerAnimator()
        {
          
            PlayBgm(1);
            For(_tigersTra, 0.1f, (index,tra)=> {
                var go = tra.gameObject;
                var rect = tra.GetRectTransform();

                var sPos = rect.localPosition;            
                var ePos = new Vector2(sPos.x- 160, sPos.y);
                var name = go.name;
                PlaySpine(go, go.name, null, true);

                var to = rect.DOAnchorPos(ePos, index + 1);
                var from = rect.DOAnchorPos(sPos, index + 2);
                

                DOTween.Sequence().Append(to)
                                  .AppendInterval(index + 1)
                                  .Append(from)
                                  .SetLoops(-1);
            });

          
            InitAnimatotPos();

            var bgSpeed = GetSpeed(3);
            var grass1Speed = GetSpeed(5);
            var groundSpeed = GetSpeed(10);
            var grass2Speed = GetSpeed(15);

          
            var uIs0StartPos = new Vector2(-1920,0);
            var uIs1StartPos = new Vector2(-1920,641);
            var uIs2StartPos = new Vector2(-1920, 0);
            var uIs3StartPos = new Vector2(-1920, 0);

            UpDate(true, 0.02f, () =>
            {
                Move(_0A2, uIs0StartPos, bgSpeed); Move(_1A2, uIs0StartPos, bgSpeed); //远处的背景

                Move(_0A3, uIs1StartPos, grass1Speed); Move(_1A3, uIs1StartPos, grass1Speed);     //远处的草

                Move(_0A5, uIs2StartPos, groundSpeed); Move(_1A5, uIs2StartPos, groundSpeed);  //地面

                Move(_0c, uIs3StartPos, grass2Speed); Move(_1c, uIs3StartPos, grass2Speed); //近处的草
            });

        }



        private float GetSpeed(float speed)
        {
            var tempSpeed = 0f;
            tempSpeed = speed *(Screen.width/1920f);
            return tempSpeed;
        }

        private void  Move(RectTransform rect,Vector2 startPos,float speed,float endXPos=1900f)
        {
            if (rect.anchoredPosition.x >= endXPos)
                rect.anchoredPosition = startPos;
            rect.Translate(Vector3.right * speed);
        }


        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _smallTT.Hide(); _mask.Hide();

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
                        ShowLevel();
                    });
                });
            });
     
            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
                    StopAudio(SoundManager.SoundType.BGM);
                    StopAudio(SoundManager.SoundType.VOICE);
                    PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();
                        _bigTT.Show();
                        BellSpeck(_bigTT, 2, null, null, false);
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

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null, bool isAlpha = false, float duration = 0)
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
          //  PlayCommonSound(5);
            PlayVoice(1);
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayVoice(0);
            //PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
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
        
        private void For(Transform parent, float delay, Action<int,Transform> callBack)
        {
            _mono.StartCoroutine(IEFor(parent,delay,callBack));
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

        IEnumerator IEFor(Transform parent, float delay, Action<int,Transform> callBack)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(delay);
                var child = parent.GetChild(i);
                callBack?.Invoke(i, child);
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

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null, Ease type = Ease.Linear)
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
