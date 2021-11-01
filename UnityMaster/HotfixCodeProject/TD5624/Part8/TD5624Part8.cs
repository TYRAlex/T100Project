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
    public class TD5624Part8
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
        private GameObject _smallDD;
        private GameObject _bigDD;

        private Transform _spines;
        private Transform _rights;
        private Transform _drops;
        private Transform _lTra;
        private Transform _randomTra;

        private GameObject _earSpine;
        private GameObject _faceSpine;
        private GameObject _eyeSpine;
        private GameObject _mouthSpine;

        private GameObject _xuEarSpine;
        private GameObject _xuFaceSpine;
        private GameObject _xuEyeSpine;
        private GameObject _xuMouthSpine;

        private GameObject _resultEarSpine;
        private GameObject _resultFaceSpine;
        private GameObject _resultEyeSpine;
        private GameObject _resultMouthSpine;

        private CanvasGroup _game1SucessCanGroup;

        private GameObject _guangSpine;
      

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;




        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长

        private bool _isPlaying;

        private List<string> _randomSpineNames;
        private int _curStep;
        private List<mILDrager>  _mILDragers;
        private List<mILDroper> _mILDropers;
        private mILDrager _curDrag;
        

        //游戏2
        private Transform  _icons;
        private Transform _enemys;
        private Transform _scores;

        private Transform _game2Spines;
        private GameObject _tuZiSpine;
        private GameObject _02Spine;
        private GameObject _game2Sucess;
        private CanvasGroup _game2SuessCanGroup;

        private List<int> _game2RandomIndex;
        private int _curGame2Step;
       

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

            _earSpine = curTrans.GetGameObject("Spines/Game1/Lefts/L/ear");
            _faceSpine = curTrans.GetGameObject("Spines/Game1/Lefts/L/face");
            _eyeSpine = curTrans.GetGameObject("Spines/Game1/Lefts/L/eye");
            _mouthSpine = curTrans.GetGameObject("Spines/Game1/Lefts/L/mouth");

            _xuEarSpine = curTrans.GetGameObject("Spines/Game1/Lefts/Random/ear");
            _xuFaceSpine = curTrans.GetGameObject("Spines/Game1/Lefts/Random/face");
            _xuEyeSpine = curTrans.GetGameObject("Spines/Game1/Lefts/Random/eye");
            _xuMouthSpine = curTrans.GetGameObject("Spines/Game1/Lefts/Random/mouth");


            _resultEarSpine = curTrans.GetGameObject("Spines/Game1/Game1Sucess/resultParent/ear");
            _resultFaceSpine = curTrans.GetGameObject("Spines/Game1/Game1Sucess/resultParent/face");
            _resultEyeSpine = curTrans.GetGameObject("Spines/Game1/Game1Sucess/resultParent/eye");
            _resultMouthSpine = curTrans.GetGameObject("Spines/Game1/Game1Sucess/resultParent/mouth");

            _guangSpine = curTrans.GetGameObject("Spines/Game1/Game1Sucess/guangParent/guang");
          
            _game1SucessCanGroup = curTrans.Find("Spines/Game1/Game1Sucess").GetComponent<CanvasGroup>();

            _spines = curTrans.Find("Spines");
            _rights = curTrans.Find("Spines/Game1/Rights");
            _drops = curTrans.Find("Spines/Game1/Drops");

            _lTra = curTrans.Find("Spines/Game1/Lefts/L");
            _randomTra = curTrans.Find("Spines/Game1/Lefts/Random");


            //游戏2
            _icons = curTrans.Find("Spines/Game2/z3/z5/Icons");
            _enemys = curTrans.Find("Spines/Game2/z3/z6/Enemys");
            _scores = curTrans.Find("Spines/Game2/z3/z6/Scores");

            _game2Spines = curTrans.Find("Spines/Game2/Spines");
            _tuZiSpine = curTrans.GetGameObject("Spines/Game2/TZ/tuzi");

           
            _game2SuessCanGroup = curTrans.Find("Spines/Game2/Game2Sucess").GetComponent<CanvasGroup>();
            _02Spine = curTrans.GetGameObject("Spines/Game2/Game2Sucess/02");

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
            _randomSpineNames = new List<string>();
            _curStep = 1;
            _game2RandomIndex = new List<int>();
        
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            HideAllChilds(_spines);
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _nextSpine.Hide();
            _smallDD.Hide(); _bigDD.Hide(); 

            _game1SucessCanGroup.alpha = 0;
            _game2SuessCanGroup.alpha = 0;
            ShowChilds(_spines,0);

           

            RandomSpineName();

            _mILDropers = SetDropsCallBack(_drops, FailCallBack);


            for (int i = 0; i < _randomSpineNames.Count; i++)
            {
                var mILDroper = GetDroperToIndex(_mILDropers, i);
                mILDroper.dropType = int.Parse( _randomSpineNames[i]);
            }

            for (int i = 0; i < _lTra.childCount; i++)
            {
                var child = _lTra.GetChild(i).gameObject;
                PlaySpine(child, "kong");
            }

            PlaySpine(_guangSpine, "kong");
            for (int i = 0; i < _randomTra.childCount; i++)
            {
                var child = _randomTra.GetChild(i).gameObject;
                PlaySpine(child, "kong");
            }


           

        }

       

        void GameStart()
        {
            _mask.Show(); _startSpine.Show(); ShowNextStep();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                         PlayBgm(0);
                        _smallDD.Show(); _startSpine.Hide();
                        BellSpeck(_smallDD, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }, false);
                    });
                });
            });
        }


        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_smallDD, 1, null, StartGame, false);
                    break;
                case 2:
                    BellSpeck(_smallDD, 3, null, () => { _mask.Hide(); _smallDD.Hide(); }, false);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


        #region 游戏1

        private void RandomSpineName()
        {
            var faceName = Random.Range(1, 4) + "";
            var earName = Random.Range(4, 7)+"";         
            var eyeName = Random.Range(7, 10) + "";
            var mouthName = Random.Range(10, 13) + "";

            _randomSpineNames.Add(faceName);
            _randomSpineNames.Add(earName);
            _randomSpineNames.Add(eyeName);
            _randomSpineNames.Add(mouthName);

        }
        
        private void ShowNextStep()
        {

            HideAllChilds(_rights);
          

            switch (_curStep)
            {
                case 1:
                    SetNextStepDrag();
                    _xuFaceSpine.Show();
                    PlaySpine(_xuFaceSpine, _randomSpineNames[_curStep - 1]);
                    break;
                case 2:
                    SetNextStepDrag();
                    _xuEarSpine.Show();
                    PlaySpine(_xuEarSpine, _randomSpineNames[_curStep - 1]);
                    break;
                case 3:
                    SetNextStepDrag();
                    _xuEyeSpine.Show();
                    PlaySpine(_xuEyeSpine, _randomSpineNames[_curStep - 1]);
                    break;
                case 4:
                    SetNextStepDrag();
                    _xuMouthSpine.Show();
                    PlaySpine(_xuMouthSpine, _randomSpineNames[_curStep - 1]);
                    break;              
                case 5:
                   
                    Delay(2, ()=> {
                        _game1SucessCanGroup.alpha = 1;
                     
                        PlayVoice(1);
                        PlayBgm(1);
                  
                        PlaySpine(_guangSpine, _guangSpine.name, () => { PlaySpine(_guangSpine, _guangSpine.name + "2"); });
                        PlaySpine(_resultFaceSpine, "A" + _randomSpineNames[0]);                     
                        PlaySpine(_resultEarSpine, "A" + _randomSpineNames[1]);                    
                        PlaySpine(_resultEyeSpine, "A" + _randomSpineNames[2]);                      
                        PlaySpine(_resultMouthSpine, "A" + _randomSpineNames[3]);
                        Delay(4, NextGame);
                    });
                    break;
            }
        }
      
        private void SetNextStepDrag()
        {

            ShowChilds(_rights, _curStep - 1, g => {
                _mILDragers = SetDragsCallBack(g.transform, StartDrag, null, DragEnd);
                for (int i = 0; i < _mILDragers.Count; i++)
                {

                   
                    var childRect = _mILDragers[i].transform.GetChild(0).GetRectTransform();
                    SetScale(childRect, Vector3.one);
                    List<mILDroper> drops = new List<mILDroper>();
                    List<mILDroper> failDrops = new List<mILDroper>();
                    
                    var mILDrager = _mILDragers[i];
                    mILDrager.transform.GetEmpty4Raycast().raycastTarget = true;
                    mILDrager.DoReset();
                    mILDrager.gameObject.Show();
                    for (int j = 0; j < _mILDropers.Count; j++)
                    {
                        var mILDroper = _mILDropers[j];
                        if (mILDrager.dragType == mILDroper.dropType)
                            drops.Add(mILDroper);
                        else
                            failDrops.Add(mILDroper);
                    }
                    mILDrager.drops = drops.ToArray();
                    mILDrager.failDrops = failDrops.ToArray();
                }
            });
            PlaySpine(_xuFaceSpine, _randomSpineNames[_curStep - 1]);
        }

        private void FailCallBack(int index)
        {
          
        }

        private void StartDrag(Vector3 pos, int dragType, int index)
        {
           
           _curDrag =  GetDragToIndexAndType(_mILDragers,index, dragType);
           _curDrag.transform.SetAsLastSibling();
       
            if (_curStep != 4)
            {
                var childRect = _curDrag.transform.GetChild(0).GetRectTransform();
                SetScale(childRect, new Vector3(1.5f, 1.5f, 0));
            }
           

            foreach (var item in _mILDragers)
            {
                if (item.name!= _curDrag.name)                
                    item.transform.GetEmpty4Raycast().raycastTarget = false;                
            }
        }
     
        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            if (isMatch)
            {
                PlaySuccessSound();
                _curDrag.gameObject.Hide();
                var name = "A" + _curDrag.name;
                switch (_curStep)
                {
                    case 1:
                        _faceSpine.Show();
                        PlaySpine(_faceSpine, name,()=> {
                            _xuFaceSpine.Hide();
                            _curStep++;
                            ShowNextStep();
                        });
                        break;
                    case 2:
                        _earSpine.Show();
                        PlaySpine(_earSpine, name, () => {
                            _xuEarSpine.Hide();
                            _curStep++;
                            ShowNextStep();
                        });
                        break;
                    case 3:
                        _eyeSpine.Show();
                        PlaySpine(_eyeSpine, name, () => {
                            _xuEyeSpine.Hide();
                            _curStep++;
                            ShowNextStep();
                        });
                        break;
                    case 4:
                        _mouthSpine.Show();
                        PlaySpine(_mouthSpine, name, () => {
                            _xuMouthSpine.Hide();
                            _curStep++;
                            ShowNextStep();
                        });
                        break;                 
                }
            }
            else
            {
                _curDrag.DoReset();
                _curDrag.transform.GetEmpty4Raycast().raycastTarget = false;
                var childRect = _curDrag.transform.GetChild(0).GetRectTransform();
                SetScale(childRect, Vector3.one);
                Delay(PlayFailSound(), () => {
                    foreach (var item in _mILDragers)
                        item.transform.GetEmpty4Raycast().raycastTarget = true;
                });
             
            }
        }

        #endregion

        #region 游戏2

        private void NextGame()
        {


            //游戏2
            _curGame2Step = 1; _isPlaying = true;
            _game1SucessCanGroup.alpha = 0;
         

            _mask.Show();
            _nextSpine.Show();

            PlaySpine(_nextSpine, "next2", () => {
                AddEvent(_nextSpine, nextGo => {
                    PlayBgm(0);
                    PlayOnClickSound();
                    RemoveEvent(_nextSpine);
                    PlaySpine(_nextSpine, "next",()=> { _nextSpine.Hide();
                        HideChilds(_spines, 0); ShowChilds(_spines, 1);
                        _smallDD.Show();
                        BellSpeck(_smallDD, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); }, false);
                        PlaySpine(_tuZiSpine, "kong");

                        RandomGame2Index();

                        ShowGame2Level();
                    });

                    
                   
                });
            });                   
        }

        private void RandomGame2Index()
        {
            while (true)
            {
                var index = Random.Range(0, 3);
                var isContain = _game2RandomIndex.Contains(index);

                if (!isContain)                
                    _game2RandomIndex.Add(index);

                if (_game2RandomIndex.Count==3)                
                    break;                
            }

             
        }

        private void ShowGame2Level()
        {

            PlaySpine(_tuZiSpine, _tuZiSpine.name, null, true);
            HideAllChilds(_icons); HideAllChilds(_scores); HideAllChilds(_enemys); HideAllChilds(_game2Spines);

            switch (_curGame2Step)
            {
                case 1:
                
                    var index1 = _game2RandomIndex[0];
                    ShowChilds(_icons, index1); ShowChilds(_enemys, 0);ShowChilds(_scores, 0);
                    ShowChilds(_game2Spines, index1, go=> { InitGame2Spine(go); SetOnClickEvent(go); });
                    break;
                case 2:
                    var index2 = _game2RandomIndex[1];
                    ShowChilds(_icons, index2); ShowChilds(_enemys, 0); ShowChilds(_scores, 1);
                    ShowChilds(_game2Spines, index2, go => { InitGame2Spine(go); SetOnClickEvent(go);  });
                    break;
                case 3:
                    var index3 = _game2RandomIndex[2];
                    ShowChilds(_icons, index3); ShowChilds(_enemys, 0); ShowChilds(_scores, 2);
                    ShowChilds(_game2Spines, index3, go => { InitGame2Spine(go); SetOnClickEvent(go);  });
                    PlaySpine(_02Spine, "kong");
                    break;
                case 4:
                  
                    _isPlaying = true;
                    ShowChilds(_enemys, 1); ShowChilds(_scores, 3);
                    Delay(1.0f, () => {
                        _game2SuessCanGroup.alpha = 1;
                        PlayBgm(1);
                        PlayVoice(2);
                        PlaySpine(_02Spine, _02Spine.name, () => { PlaySpine(_02Spine, _02Spine.name + "a"); });
                      
                        Delay(7.2f, GameSuccess);

                    });
                  
                    break;
            }
        }

        private void InitGame2Spine(GameObject go)
        {
            var spine = go.transform.Find("Spine");

            for (int i = 0; i < spine.childCount; i++)
            {
                var child = spine.GetChild(i).gameObject;
                child.Show();
                PlaySpine(child, "kong", () => { PlaySpine(child,child.name); });
            }
           
        }

        private void SetOnClickEvent(GameObject go)
        {
            var onClick = go.transform.Find("OnClick");
            AddEvents(onClick, OnClick);
            _isPlaying = false;
        }

        private void OnClick(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            var name = go.name;

            switch (name)
            {
                case "a11":                  
                case "a12":
                case "b11":
                case "b13":
                case "c11":
                case "c13":   //失败
                    var failSpineName = name + "x";
                    var curSpineGo = go.transform.parent.parent.Find("Spine").Find(go.name).gameObject;
                    PlaySpine(curSpineGo, failSpineName);
                    PlayFailSound();
                    var tuZiSpine2Time = PlaySpine(_tuZiSpine, _tuZiSpine.name + "2");
                    Delay(tuZiSpine2Time, () => { PlaySpine(_tuZiSpine, _tuZiSpine.name, null, true); _isPlaying = false; });                                                                                        
                    break;
                case "a13":   //成功
                    PlayVoice(0);
                    PlaySuccessSound();
                    var a13yTime = PlaySpine(_tuZiSpine, "a13y");                
                    go.transform.parent.parent.Find("Spine").Find(go.name).gameObject.Hide();
                    Delay(a13yTime, () => { _curGame2Step++; ShowGame2Level(); });           
                    break;                                
                case "b12":
                    PlayVoice(0);
                    PlaySuccessSound();
                    var b12yTime = PlaySpine(_tuZiSpine, "b12y");                
                    go.transform.parent.parent.Find("Spine").Find(go.name).gameObject.Hide();
                    Delay(b12yTime, () => { _curGame2Step++; ShowGame2Level(); });                  
                    break;                                                               
                case "c12":
                    PlayVoice(0);
                    PlaySuccessSound();
                    var c12yTime = PlaySpine(_tuZiSpine, "c12y");               
                    go.transform.parent.parent.Find("Spine").Find(go.name).gameObject.Hide();
                    Delay(c12yTime, () => { _curGame2Step++; ShowGame2Level(); });                 
                    break;                              
            }
        }

        #endregion



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
                        ShowNextStep();
                        _talkIndex = 2;
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

        #region 拖拽相关

        private List<mILDrager> SetDragsCallBack(Transform parent, Action<Vector3, int, int> startDrag = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
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

        private List<mILDroper> SetDropsCallBack(Transform parent, Action<int> failCallBack = null)
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

        private mILDrager GetDragToIndexAndType(List<mILDrager> mILDragers, int index,int type)
        {
            mILDrager temp = null;
            for (int i = 0; i < mILDragers.Count; i++)
            {
                var mILDrager = mILDragers[i];

                if (mILDrager.index == index && mILDrager.dragType== type)
                {
                    temp = mILDrager;
                    break;
                }
            }

            return temp;
        }

        private mILDroper GetDroperToIndex(List<mILDroper> mILDropers,int index)
        {
            mILDroper temp = null;
            for (int i = 0; i < mILDropers.Count; i++)
            {
                var mILDroper = mILDropers[i];

                if (mILDroper.index==index)
                {
                    temp = mILDroper;
                    break;
                }
            }

            return temp;
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
