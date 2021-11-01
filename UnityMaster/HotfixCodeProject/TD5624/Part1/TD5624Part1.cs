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
    public class TD5624Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;


        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _nextSpine;
        private GameObject _spSpine;
        private GameObject _smallTT;
        private GameObject _bigTT;
        private GameObject _jySpine;


        private CanvasGroup _a2CanvasGroup;
        private Transform _unknowns;

        private GameObject _zzz5Go;
        private Transform _drapsTra;
        private Transform _dropsTra;
        private Transform _sleeps;


        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
       
        private List<int> _randomIndexData;
        private List<Vector2> _unknownsInitPos;

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private GameObject _curAnimal;
        private int _curLevelId;

        private float _moveValue;
        private float _duration;    //切换时长

        private bool _isPlaying;

        private List<GameObject> _zzz6Spines;

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
            _smallTT = curTrans.GetGameObject("SmallTT");
            _bigTT = curTrans.GetGameObject("BigTT");

            _a2CanvasGroup = curTrans.Find("Bg/A2").GetComponent<CanvasGroup>();
            _unknowns = curTrans.Find("Bg/unknowns");

            _zzz5Go = curTrans.GetGameObject("Bg/A7/zzz5");
            _drapsTra = curTrans.Find("Spines/zoologys/Draps");
            _dropsTra = curTrans.Find("Spines/zoologys/Drops");
            _sleeps = curTrans.Find("Spines/zoologys/sleeps");
            _jySpine=curTrans.GetGameObject("Bg/A7/PAI/jy");

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
            _a2CanvasGroup.alpha = 0;
            _unknownsInitPos = new List<Vector2> {
                new Vector2(-6,0),
                new Vector2(0,14),
                new Vector2(0,-4),
                new Vector2(-4,-2),
                new Vector2(9,-17)};

            _zzz6Spines = new List<GameObject>();
            _zzz6Spines.Add(_curGo.transform.GetGameObject("Bg/A7/DONG/zzz6"));

            for (int i = 0; i < _sleeps.childCount; i++)
            {
                var zzz6 = _sleeps.GetChild(i).Find("zzz6").gameObject;
                _zzz6Spines.Add(zzz6);
            }


            _curLevelId = 1;
            InitRandowData();
        }

        void GameInit()
        {
            InitData();
         
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _nextSpine.Hide();
            _smallTT.Hide(); _bigTT.Hide();
            _zzz5Go.Hide();

            SetAllE4RayTargrt(_drapsTra, false);

            HideAllChilds(_sleeps);
            HideAllChilds(_drapsTra);

            HideAllChilds(_unknowns);           
            UnknownsAni();
          
            AddEvents(_unknowns, OnClickUnknowns);
            SetDragsCallBack(_drapsTra, StartDrag, null, DragEnd);

            for (int i = 0; i < _zzz6Spines.Count; i++)            
                _zzz6Spines[i].Hide();            
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

        private void StartDrag(Vector3 pos, int dragType, int index)
        {
            _curAnimal.transform.GetChild(0).gameObject.Hide();
            var curSpineGo = _curAnimal.transform.GetChild(1).gameObject;
            PlaySpine(curSpineGo, _curAnimal.name + "1", null, true);
        }


        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            if (isMatch)
            {
                PlayCommonSound(4);
                _curAnimal.Hide();
                var name = _curAnimal.name;

                switch (name)
                {
                    case "a":
                        ShowChilds(_sleeps, 0, (g) => { _curLevelId++; ShowLevel();  });
                        break;
                    case "b":
                        ShowChilds(_sleeps, 1, (g) => { _curLevelId++; ShowLevel(); });
                        break;
                    case "c":
                        ShowChilds(_sleeps, 2, (g) => { _curLevelId++; ShowLevel(); });
                        break;
                    case "d":
                        ShowChilds(_sleeps, 3, (g) => { _curLevelId++; ShowLevel(); });
                        break;
                    case "e":
                        _zzz5Go.Show(); _curLevelId++; ShowLevel();
                        break;
                }
            }
        }

     

        private void InitRandowData()
        {
            _randomIndexData = new List<int>();
            while (true)
            {
                var index = Random.Range(0, 5);
                var isContain= _randomIndexData.Contains(index);

                if (!isContain)               
                    _randomIndexData.Add(index);

                if (_randomIndexData.Count == 5)
                    break;
            }            
        }  

        private void OnClickUnknowns(GameObject go)
        {
            if (_isPlaying)           
                return;
            
            _isPlaying = true;

            PlayOnClickSound();
            var name = go.name;
            bool isSame = name == _curAnimal.name;
            var curSpineGo = _curAnimal.transform.GetChild(1).gameObject;

            if (isSame)
            {
                FindSpineGo(_unknowns, name).Hide();
                PlaySpine(curSpineGo, name + "3", null, true);

                Delay( PlaySuccessSound(), () => { SetE4RayTargrt(_curAnimal, true); });               
            }
            else
            {
                PlayFailSound();
                Delay(PlaySpine(curSpineGo, _curAnimal.name + "2"),()=> { PlaySpine(curSpineGo, _curAnimal.name + "1", null, true); });               
                Delay(2.3f, () => { _isPlaying = false; });
            }
        }

        private void ShowCurAnimal(int index)
        {
            ShowChilds(_drapsTra, _randomIndexData[index], go => {
                _curAnimal = go;
                _curAnimal.transform.GetChild(0).gameObject.Show();
                var spineGo = go.transform.GetChild(1).gameObject;             
                PlaySpine(spineGo, spineGo.name, null, true);
                _jySpine.Show();
                PlaySpine(_jySpine, _jySpine.name + (_randomIndexData[index] + 1), null, true);
            });
        }

        private void ShowLevel()
        {
            _isPlaying = false;
            switch (_curLevelId)
            {
                case 1:                 
                    ShowCurAnimal(0);
                    break;
                case 2:              
                    ShowCurAnimal(1);
                    break;
                case 3:             
                    ShowCurAnimal(2);
                    break;
                case 4:                 
                    ShowCurAnimal(3);
                    break;
                case 5:                
                    ShowCurAnimal(4);
                    break;
                case 6:
                    _isPlaying = true;                
                    _jySpine.Hide();
                    _a2CanvasGroup.DOFade(1, 2);
                    Delay(2, () => {
                        PlayVoice(0, true);

                        DelayFor(_zzz6Spines, 0.2f, (rect, index) =>
                        {
                            var spine = _zzz6Spines[index];
                            spine.Show();
                            PlaySpine(spine, spine.name, null, true);
                        });
                        
                        Delay(3, GameSuccess);
                    });                    
                    break;
            }
        }

        private void UnknownsAni()
        {
            DOTween.KillAll();
            DelayFor(_unknowns, 0.1f, (rect,index)=>{
                rect.gameObject.Show();
                var childRect = rect.GetChild(0).GetRectTransform();
                childRect.anchoredPosition = _unknownsInitPos[index];
                var offset = -1;
                if (index%2==0)                
                    offset = 20;                
                else               
                    offset = -20;
               
                childRect.DOAnchorPosY(childRect.anchoredPosition.y + offset, 1.5f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear);              
            });
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
                    PlayOnClickSound(); PlayCommonBgm(4);
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

        #region 拖拽相关

        private List<mILDrager> SetDragsCallBack(Transform parent,Action<Vector3,int,int> startDrag=null, Action<Vector3, int, int> draging = null,Action<Vector3,int,int,bool> dragEnd=null,Action<int> onClick=null)
        {
            List<mILDrager> mILDragers = new List<mILDrager>();
            for (int i = 0; i < parent.childCount; i++)
            {
                var drag = parent.GetChild(i).GetComponent<mILDrager>();
                mILDragers.Add(drag);
                drag.SetDragCallback(startDrag, draging, dragEnd, onClick);
                drag.DoReset();
            }
            return mILDragers;
        }

        private List<mILDroper> SetDropsCallBack(Transform parent,Action<int> failCallBack=null)
        {
            List<mILDroper> mILDropers = new List<mILDroper>();
            for (int i = 0; i < parent.childCount; i++)
            {
                var drop = parent.GetChild(i).GetComponent<mILDroper>();
                mILDropers.Add(drop);
                drop.SetDropCallBack(null, null, failCallBack);
            }
            return mILDropers;
        }

        #endregion

        #region Empty4Raycast相关
        private void SetAllE4RayTargrt(Transform parent,bool isEnable)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var e4R = parent.GetChild(i).GetEmpty4Raycast();
                e4R.raycastTarget = isEnable;
            }
        }

        private void SetE4RayTargrt(GameObject go, bool isEnable)
        {
            go.transform.GetEmpty4Raycast().raycastTarget = isEnable;
        }

        private void SetE4RayTargrt(Transform parent, string name, bool isEnable)
        {
            parent.Find(name).GetEmpty4Raycast().raycastTarget = isEnable;
        }
        #endregion

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

        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
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

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
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

        private void DelayFor(Transform parent, float delay, Action<RectTransform,int> callBack)
        {
            _mono.StartCoroutine(IEDelayFor(parent, delay, callBack));
        }

        private void DelayFor(List<GameObject> list, float delay, Action<RectTransform, int> callBack)
        {
            _mono.StartCoroutine(IEDelayFor(list, delay, callBack));
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

        IEnumerator IEDelayFor(List<GameObject> list, float delay, Action<RectTransform, int> callBack)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var rect = list[i].transform.GetRectTransform();
                callBack?.Invoke(rect, i);
                yield return new WaitForSeconds(delay);
            }
        }

        IEnumerator IEDelayFor(Transform parent, float delay, Action<RectTransform,int> callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var rect = parent.GetChild(i).GetRectTransform();
                callBack?.Invoke(rect, i);
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
