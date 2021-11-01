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

    public class CCPosInfo
    {
        public Vector2 Pos;
        public float RotationZ;

        public CCPosInfo(Vector2 pos, float rotationZ)
        {
            Pos = pos;
            RotationZ = rotationZ;
        }
    }

    public class TD6733Part5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;


        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _nextSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;

        private GameObject _dDD;

        private GameObject _dialogue;

        private Transform _spines0;
        private Transform _spines1;
        private Transform _spines2;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长


        private Vector2 _roleStartPos;
        private Vector2 _roleEndPos;

        private Vector2 _enemyStartPos;
        private Vector2 _enemyEndPos;

        private RectTransform _dingDing;
        private RectTransform _devil;

        private Text _dingDingTxt;
        private Text _devilTxt;

        private List<string> _dialogues;

        private bool _isPlaying;


        private CanvasGroup _bg1Can;
        private GameObject _kp;
        private GameObject _light;
        private GameObject _lightStart;

        private GameObject _curLunKuo;
        private GameObject _curLunKuoGuang;
        private GameObject _curXem;
       
        private GameObject _curXing;
        private GameObject _curFeng;
        

        private GameObject _onBtn;

        private List<CCPosInfo> _levelPosInfos;
       
       

        private int _curLevelId;

        private Transform _levelOneStart;
        private Transform _levelTwoStart;
        private Transform _levelThreeStart;


        private List<mILDrager> _curDrags;
        private List<mILDroper> _curDrops;
        private Vector2 _curDaggInitAncPos;
        private mILDrager _curDrag;
        private int _nums;

        private GameObject _biaoQing1;
        private GameObject _biaoQing2;

        private Transform _scoresTra;

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

            _dDD = curTrans.GetGameObject("DDD");

            _dingDing = curTrans.GetRectTransform("dialogue/dingDing");
            _devil = curTrans.GetRectTransform("dialogue/devil");

            _dingDingTxt = curTrans.GetText("dialogue/dingDing/Text");
            _devilTxt = curTrans.GetText("dialogue/devil/Text");

            _dialogue = curTrans.GetGameObject("dialogue");

            _spines0 = curTrans.Find("Spines0");
            _spines1 = curTrans.Find("Spines1");
            _spines2 = curTrans.Find("Spines2");

            _bg1Can = curTrans.Find("bg1").GetComponent<CanvasGroup>();

            _kp = curTrans.GetGameObject("Spines2/kp");
            _light = curTrans.GetGameObject("Spines2/light");
            _lightStart = curTrans.GetGameObject("Spines2/light-star");
            _onBtn = curTrans.GetGameObject("OnBtn");

            _levelOneStart = curTrans.Find("Spines0/0/star");
            _levelTwoStart = curTrans.Find("Spines0/1/star");
            _levelThreeStart = curTrans.Find("Spines0/2/star");

            _biaoQing1 = curTrans.GetGameObject("Icons/biaoqings/1");
            _biaoQing2 = curTrans.GetGameObject("Icons/biaoqings/2");

            _scoresTra = curTrans.Find("Icons/Scores");

            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;
            _dingDingTxt.text = string.Empty;
            _devilTxt.text = string.Empty;
            

            _dialogues = new List<string> {
                "我讨厌一切，是不会让你们得逞的！",
                "可恶的小恶魔，大家加油哦！",
            };

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = 0;
            _moveValue = 1920; _duration = 1.0f;

            _roleStartPos = new Vector2(-2170, 539); _roleEndPos = new Vector2(-960, 539);
            _enemyStartPos = new Vector2(200, 540); _enemyEndPos = new Vector2(-994, 540);
           
            SetPos(_devil, _enemyStartPos); SetPos(_dingDing, _roleStartPos);

            _curLevelId = 0;
           
            _bg1Can.alpha = 1;

           
            _levelPosInfos = new List<CCPosInfo>();
          
            _levelPosInfos.Add(new CCPosInfo(new Vector2(1383, -265), 65));
            _levelPosInfos.Add(new CCPosInfo(new Vector2(594, -204), 180));
            _levelPosInfos.Add(new CCPosInfo(new Vector2(1390, -702), 0));
            _levelPosInfos.Add(new CCPosInfo(new Vector2(745, -795), -70));
            _levelPosInfos.Add(new CCPosInfo(new Vector2(517, -561), -130));
            _levelPosInfos.Add(new CCPosInfo(new Vector2(1450, -461), 34));
            _levelPosInfos.Add(new CCPosInfo(new Vector2(408, -365), -147));
           
            

        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _nextSpine.Hide();
            _dDD.Hide(); _dialogue.Hide(); _onBtn.Show();


            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine); RemoveEvent(_nextSpine);
            AddEvent(_onBtn, OnBtn);

            InitSpineState();
            RodomPos();

            HideAllChilds(_scoresTra);
            ShowChilds(_scoresTra, _curLevelId);
            _biaoQing1.Show(); _biaoQing2.Hide();
        }

    

        void GameStart()
        {
            _mask.Show(); _startSpine.Show(); _dialogue.Show();
            ShowLevel(_curLevelId);

            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);
                        _startSpine.Hide();
                        StartDialogue();
                    });
                });
            });
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:

                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


        /// <summary>
        /// 开始对话
        /// </summary>
        private void StartDialogue()
        {
            PlayVoice(5);
            SetMove(_devil, _enemyEndPos, 1.0f, () => {

                ShowDialogue(_dialogues[0], _devilTxt);
                Delay(PlaySound(0), DingDing);

            });

            void DingDing()
            {
                PlayVoice(5);
                SetMove(_dingDing, _roleEndPos, 1.0f, () => {
                    ShowDialogue(_dialogues[1], _dingDingTxt);
                    Delay(PlaySound(1), () => {
                        _dialogue.Hide();
                        StartGame();
                    });

                });
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();

            BellSpeck(_dDD, 2, () => { _isPlaying = true; }, () => { _isPlaying = false; },false);
           
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
                        ShowLevel(_curLevelId);
                      
                        StartGame();
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
                        _dDD.Show();
                        BellSpeck(_dDD, 4,null,null,false);
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
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }

      
        private void RodomPos()
        {
            SetRandomPosAndAngle(_levelOneStart, Indexs(5));
            SetRandomPosAndAngle(_levelTwoStart, Indexs(7),new List<string> { "0","2"});
            SetRandomPosAndAngle(_levelThreeStart, Indexs(7),new List<string> { "3","5"});
        }


        private List<int> Indexs(int listCount)
        {
            List<int> indexs = new List<int>();

            while (true)
            {
                var tempIndex = Random.Range(0, listCount);
                var isContain = indexs.Contains(tempIndex);

                if (!isContain)
                    indexs.Add(tempIndex);

                if (indexs.Count == listCount)
                    break;
            }
            return indexs;
        }

        private void SetRandomPosAndAngle(Transform parent,List<int> indexs,List<string> butterflyNames=null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var childRect = parent.GetChild(i).GetRectTransform();

                bool isButterfly = false;

                if (butterflyNames!=null)               
                    isButterfly = butterflyNames.Contains(childRect.name);                
            
                var radomIndex = indexs[i];
                var posData = _levelPosInfos[radomIndex];
                childRect.anchoredPosition = new Vector2(posData.Pos.x, posData.Pos.y);

                if (isButterfly)
                    childRect.localEulerAngles = new Vector3(0, 0, 0);
                else
                childRect.localEulerAngles = new Vector3(0, 0, posData.RotationZ);
            }
        }


        private void InitSpineState()
        {
            for (int i = 0; i < _spines0.childCount; i++)
            {
                var child = _spines0.GetChild(i);
            
                var xemGo  = child.Find("xem").GetChild(0).gameObject;
                var shuGo  = child.Find("shu").GetChild(0).gameObject;
                var xingGo = child.Find("xing").GetChild(0).gameObject;
                var fengGo = child.Find("feng").GetChild(0).gameObject;

                var end = child.Find("end");
                var star = child.Find("star");
            
                ShowAllChilds(star);
                SetEndState(end);
                SpineInitialize(xemGo);
                SpineInitialize(shuGo);
                SpineInitialize(xingGo);
                SpineInitialize(fengGo);
                child.gameObject.Hide();
            }

            for (int i = 0; i < _spines1.childCount; i++)
            {
                var child = _spines1.GetChild(i);
             
                var lunkuoGo = child.Find("lunkuo").GetChild(0).gameObject;
                var lunkuoguangGo = child.Find("lunkuoguang").GetChild(0).gameObject;

                lunkuoGo.Show();
                SpineInitialize(lunkuoGo);
                SpineInitialize(lunkuoguangGo);

                child.gameObject.Hide();
            }

            for (int i = 0; i < _spines2.childCount; i++)
            {
                var child = _spines2.GetChild(i).gameObject;
                SpineInitialize(child);
            }

        }

        private void SetEndState(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                var icon = child.Find("icon");
                icon.GetChild(0).gameObject.Hide();
                icon.GetChild(1).gameObject.Show();                           
            }
        }

        private void SpineInitialize(GameObject go,Action callBack=null)
        {
            go.GetComponent<SkeletonGraphic>().Initialize(true);
            callBack?.Invoke();
        }


        private void GameOver()
        {
            _mask.Show();
            PlayVoice(4);
            PlaySpine(_light, _light.name+"1",()=> {PlaySpine(_light, _light.name + "2", null, true);});
            PlaySpine(_lightStart, _lightStart.name, null, true);
            PlaySpine(_kp, _kp.name);
           
            Delay(6,()=> {
             
                PlaySpine(_light,"kong");
                PlaySpine(_lightStart, "kong");
                PlaySpine(_kp, "kong");
                GameSuccess();
            } );
        }

        private void ShowLevel(int curLevelId)
        {
            if (_curLevelId>2)
            {              
                GameOver();              
                return;
            }

            _bg1Can.alpha = 1;
            _nums = 0;
            _onBtn.gameObject.Show();
            //树轮廓
            ShowChilds(_spines1, curLevelId, go => {
                var tra = go.transform;
                var lunkuoGo = tra.Find("lunkuo").GetChild(0).gameObject;
                var lunKuoGuang =tra.Find("lunkuoguang").GetChild(0).gameObject;
                _curLunKuo = lunkuoGo;
                _curLunKuoGuang = lunKuoGuang;

                PlaySpine(lunkuoGo, lunkuoGo.name,null,true);
            });

            //背后的树

            ShowChilds(_spines0, curLevelId, go => {
                var tra = go.transform;
                var xemGo = tra.Find("xem").GetChild(0).gameObject;
                var shuGo = tra.Find("shu").GetChild(0).gameObject;
                var xingGo = tra.Find("xing").GetChild(0).gameObject;
                var fengGo = tra.Find("feng").GetChild(0).gameObject;
                var star = tra.Find("star");
                var end = tra.Find("end");

                _curXem = xemGo;
                _curXing = xingGo;
                _curFeng = fengGo;


                xingGo.transform.parent.SetSiblingIndex(2);

                _curDrags = SetDragerCallBack(star, StarDrag, null, DragEnd);
                _curDrops = SetDroperCallBack(end);
                InitDragE4Ray(_curDrags);

                PlaySpine(shuGo, shuGo.name); PlaySpine(xingGo, xingGo.name+"0");
            });

        }

       

       

        private void OnBtn(GameObject go)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;
            PlayOnClickSound();

            var voiceIndex = _curLevelId;
            PlayVoice(voiceIndex);

            var name = _curLunKuo.name.Remove(_curLunKuo.name.Length - 1);
            PlaySpine(_curLunKuo, name, () => {
                _curLunKuo.Hide();
                 PlaySpine(_curLunKuoGuang, _curLunKuoGuang.name, () => {                   
                    _bg1Can.DOFade(0, 1.0f).OnComplete(()=> { go.Hide(); _isPlaying = false; });                                 
                 });               
            });
        }

        private void StarDrag(Vector3 pos, int dragType, int index)
        {
            _curDrag= GetCurDrag(dragType, index);
            _curDaggInitAncPos = _curDrag.transform.GetRectTransform().anchoredPosition;
            _curDrag.transform.SetAsLastSibling();
            SetOtherDragE4Ray(_curDrags, _curDrag);
        }

        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            InitDragE4Ray(_curDrags, false);

            if (isMatch)
            {
                _nums++;
                bool isFinish = false;

                switch (_curLevelId)
                {
                    case 0:
                        isFinish = _nums == 4;
                        break;
                    case 1:
                        isFinish = _nums == 4;
                        break;
                    case 2:
                        isFinish = _nums == 6;
                        break;
                }

                _curDrag.gameObject.Hide();
               

                var curDrop = GetCurDrop(dragType);
                curDrop.transform.Find("icon").GetChild(0).gameObject.Show();
                curDrop.transform.Find("icon").GetChild(1).gameObject.Hide();
               var starSpine = curDrop.transform.Find("icon").GetChild(2).gameObject;
                PlaySpine(starSpine, starSpine.name);
                PlayCommonSound(4);
                Delay(PlaySuccessSound(), () => {

                    if (!isFinish)                   
                        InitDragE4Ray(_curDrags);                    
                    else
                    {                    
                        _curXing.transform.parent.SetSiblingIndex(4);
                        PlaySpine(_curXem, _curXem.name+"3",()=> { PlayVoice(3); PlaySpine(_curXem, _curXem.name); });
                      
                        Delay(1.5f, () => { PlaySpine(_curFeng, _curFeng.name); });  
                        Delay(2, () => {                  
                          PlaySpine(_curXem, _curXem.name + "2",()=> {
                              PlaySpine(_curXing, _curXing.name+"1", () => { PlaySpine(_curXing,_curXing.name+"2",null,true); });

                              var socreIndex = _curLevelId + 1;

                              HideAllChilds(_scoresTra);ShowChilds(_scoresTra, socreIndex);

                              if (socreIndex==3)
                              {
                                  _biaoQing1.Hide(); _biaoQing2.Show();
                              }

                              Delay(2, () => {

                                  if (_curLevelId == 2)
                                  {

                                      _curLevelId++;
                                    
                                      ShowLevel(_curLevelId);
                                      return;
                                  }

                                  _nextSpine.Show();
                                  PlaySpine(_nextSpine, "next2",()=> {
                                      AddEvent(_nextSpine, nextGo => {
                                          PlayOnClickSound();
                                          RemoveEvent(_nextSpine);
                                          PlaySpine(_nextSpine, "next",()=> {
                                              _nextSpine.Hide();
                                              HideChilds(_spines0, _curLevelId);HideChilds(_spines1, _curLevelId);
                                              _curLevelId++;
                                              ShowLevel(_curLevelId);
                                          });
                                      });
                                  });
                              });
                          });

                        });

                    }
                   
                });


            }
            else
            {
                _curDrag.transform.GetRectTransform().anchoredPosition = _curDaggInitAncPos;
                PlayCommonSound(5);
                Delay(PlayFailSound(), () => { InitDragE4Ray(_curDrags); });
            }
        }

        private void SetOtherDragE4Ray(List<mILDrager> dragers,mILDrager curDrag)
        {
            foreach (var item in dragers)
            {
                if (item == curDrag)
                    continue;
                item.transform.GetEmpty4Raycast().raycastTarget=false;
            }
        }

        private void InitDragE4Ray(List<mILDrager> dragers,bool isRay=true)
        {
            for (int i = 0; i < dragers.Count; i++)
            {
                var e4Ray = dragers[i].transform.GetEmpty4Raycast();
                e4Ray.raycastTarget = isRay;
            }
        }

        private mILDrager GetCurDrag(int dragType, int index)
        {
            mILDrager drager = null;
            foreach (var item in _curDrags)
            {
                if (item.dragType== dragType && item.index == index)
                {
                    drager = item;
                    break;
                }
            }
            return drager;
        }

        private mILDroper GetCurDrop(int dragType)
        {
            mILDroper droper = null;
            foreach (var item in _curDrops)
            {
                if (item.dropType == dragType)
                {
                    droper = item;
                    break;
                }
            }
            return droper;
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

        #region 隐藏和显示



        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
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

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
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
