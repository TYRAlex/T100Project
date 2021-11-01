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

    public enum RoleType
    {
        Bd,
        Xem,
        Child,
        Adult,
    }

    public class Px
    {

        public string PxDaiJiSpineName;
        public string PxErrorSpineName;
        public string PxAttackSpineName;
        public string OkSpineName;
        public string CorrectSpineName;
        public string CorrectSpineErrorFeedbackName;   //颜色对了，但是位置错了

        public Dictionary<string, string> LevelShowSpineName = new Dictionary<string, string>();
        public Dictionary<string, string> LevelShowFeedbackSpineName = new Dictionary<string, string>();
        public Dictionary<string, string> LevelOtherBoomSpineName = new Dictionary<string, string>();



        /// <summary>
        /// Px构造函数
        /// </summary>
        /// <param name="pxDaiJiSpineName">螃蟹待机动画名</param>
        /// <param name="pxErrorSpineName">螃蟹伤心动画名</param>
        /// <param name="pxAttackSpineName">螃蟹攻击动画名</param>
        /// <param name="okSpineName">oK动画名</param>
        /// <param name="correctSpineName">正确动画名</param>
        /// <param name="levelShowSpineName">关卡显示动画名;key:按钮名-value对应spine名</param>
        public Px( string pxDaiJiSpineName,string pxErrorSpineName,string pxAttackSpineName,
                   string okSpineName,string correctSpineName,string correctSpineErrorFeedbackName,
                   Dictionary<string, string> levelShowSpineName,
                   Dictionary<string, string> levelShowFeedbackSpineName,
                   Dictionary<string, string> levelOtherBoomSpineName)
        {
            PxDaiJiSpineName = pxDaiJiSpineName;
            PxErrorSpineName = pxErrorSpineName;
            PxAttackSpineName = pxAttackSpineName;
            OkSpineName = okSpineName;
            CorrectSpineName = correctSpineName;

            CorrectSpineErrorFeedbackName = correctSpineErrorFeedbackName;

            LevelShowSpineName = levelShowSpineName;
            LevelShowFeedbackSpineName = levelShowFeedbackSpineName;
            LevelOtherBoomSpineName = levelOtherBoomSpineName;
        }


        /// <summary>
        /// 是否正确
        /// </summary>
        /// <param name="btnName">点击的按钮名</param>
        /// <returns></returns>
        public bool IsCorrect(string btnName)
        {         
            var value = LevelShowSpineName[btnName];
            return CorrectSpineName == value;            
        }

        public string GetShowSpineName(string btnName)
        {
            return LevelShowSpineName[btnName];
        }

        /// <summary>
        /// 获取反馈SpineName
        /// </summary>
        /// <param name="btnName">点击按钮名</param>
        /// <returns></returns>
        public string GetFeedbackSpineName(string btnName)
        {
            return LevelShowFeedbackSpineName[btnName];
        }
    }


    public class TD3441Part5
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


        private GameObject _dTT;
        private GameObject _xem;
        private GameObject _sTT;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private bool _isPlaying;

        private GameObject _stGo;
        private Transform _oksTra;
        private Transform _onClicksTra;
        private Transform _dragendTra;
        
        private GameObject _pxGo;
        private GameObject _finishGo;

        private List<Vector2> _levelPos;
        private Dictionary<int, Px> _levelDatas;
        private List<int> _levelIds;
        private int _curLevelIndex;
        private Px _curPx;
        private Transform _hearsTra;
        private Transform _incosTra;
        private Transform _hpsTra;

        private List<mILDrager> _mILDragers;
        private mILDrager _curDrager;
        private Vector3 _curDragerStartPos;
        private List<Vector2> _randomPos;
        private int _tierIndex;


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

            _dTT = curTrans.GetGameObject("dTT");
            _xem = curTrans.GetGameObject("xem");

            _sTT = curTrans.GetGameObject("sTT");

            _stGo = curTrans.GetGameObject("BG/ST");
            _oksTra = curTrans.Find("Spines/oks");
            _onClicksTra = curTrans.Find("Spines/onClicks");

            _dragendTra = curTrans.Find("Spines/dragend");

            _pxGo = curTrans.GetGameObject("Spines/px");
            _finishGo = curTrans.GetGameObject("Spines/finish");
            _hearsTra = curTrans.Find("Spines/hps/hears");
            _incosTra = curTrans.Find("Spines/hps/Icons");
            _hpsTra = curTrans.Find("Spines/hps");
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _levelIds = new List<int>();
            _levelDatas = new Dictionary<int, Px>{
                { 1,new Px("px-1","px-c1","px-d1","1-ok","1-a2","1-a",
                 new Dictionary<string, string>{{"a","1-a2" },{"b","1-b2" },{"c","1-c2" } },
                 new Dictionary<string, string>{{"a","1-a1" },{"b","1-b1" },{"c","1-c1" } },
                 new Dictionary<string, string>{ { "b", "1-b3" },{ "c", "1-c3" } })},

                { 2,new Px("px-2","px-c2","px-d2","3-ok","2-a2","2-a",
                 new Dictionary<string, string>{{"a","2-a2" },{"b","2-b2" },{"c","2-c2" } },
                 new Dictionary<string, string>{{"a","2-a1" },{"b","2-b1" },{"c","2-c1" } },
                 new Dictionary<string, string>{ { "b", "2-b3" },{ "c", "2-c3" }})},

                { 3,new Px("px-3","px-c3","px-d3","2-ok","3-a2","3-a",
                 new Dictionary<string, string>{{"a","3-a2" },{"b","3-b2" },{"c","3-c2" } },
                 new Dictionary<string, string>{{"a","3-a1" },{"b","3-b1" },{"c","3-c1" } },
                 new Dictionary<string, string>{ { "b", "3-b3" },{ "c", "3-c3" }})},
            };

            _levelPos = new List<Vector2> { new Vector2(480,-420),new Vector2(900,-420),new Vector2(1320,-420)};
            
            RandomLevelIds();
            _curLevelIndex = 0;
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();
            _dTT.Hide(); _xem.Hide();  _sTT.Hide(); _finishGo.Hide();
            _stGo.Show(); _oksTra.gameObject.Show(); _pxGo.Show(); _onClicksTra.gameObject.Show();
            ShowAllChilds(_hearsTra);
            ShowChilds(_incosTra, 0);
            HideChilds(_incosTra, 1);
            _hpsTra.gameObject.Show();

            InitSpines(_oksTra, false);
           // AddEvents(_onClicksTra, OnClicks);

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

        }

      

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
            ShowLevel();
            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        PlayCommonBgm(8);//ToDo...改BmgIndex
                        _startSpine.Hide();

                        _sTT.Show();
                        BellSpeck(_sTT, 0, null, ()=> { ShowVoiceBtn(); }, RoleType.Child);
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
                    BellSpeck(_sTT, 1, null, () => { StartGame(); _sTT.Hide(); }, RoleType.Child);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        private void GameOver()
        {
            _stGo.Hide(); _oksTra.gameObject.Hide(); _pxGo.Hide(); _onClicksTra.gameObject.Hide(); _hpsTra.gameObject.Hide();
            _finishGo.Show();
            PlayBgm(0);
            PlaySpine(_finishGo, "animation", null, true);
            Delay(4, GameSuccess);
        }


        private void ShowLevel()
        {

            _mILDragers = new List<mILDrager>();

            var curId = _levelIds[_curLevelIndex];
           
            var curPx = _levelDatas[curId];
            _curPx = curPx;
            _pxGo.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(_pxGo, "kong", () => {
                PlaySpine(_pxGo, curPx.PxDaiJiSpineName, null, true);
            });

            SetE4Rays(_onClicksTra, false);
            _randomPos = RandomLevelDragPos();
                           
            for (int i = 0; i < _onClicksTra.childCount; i++)
            {
                var child = _onClicksTra.GetChild(i);
                var childRect = child.GetRectTransform();

                var drager = child.GetComponent<mILDrager>();
                drager.SetDragCallback(StartDrag, null, EndDrag);
                drager.dragType = curId;

              

                var correctSpineName = curPx.LevelShowSpineName[child.name];
                var isCorrect = curPx.CorrectSpineName == correctSpineName;
                if (isCorrect)
                {
                    drager.drops = new mILDroper[1];
                    drager.drops[0] = FindGo(_dragendTra,curPx.OkSpineName).GetComponent<mILDroper>();
                }
                _mILDragers.Add(drager);

                var endPos = _randomPos[i];
                var startPos = new Vector2(endPos.x + 1920, endPos.y);

                childRect.anchoredPosition = startPos;
                           
                var spine = child.GetChild(0).gameObject;
                spine.GetComponent<SkeletonGraphic>().Initialize(true);
             
                PlaySpine(spine, "kong", () =>{PlaySpine(spine, curPx.LevelShowSpineName[child.name], null, true); });

                childRect.DOAnchorPos(endPos, 1.5f).SetEase(Ease.InOutCubic);

            }

            Delay(1.5f, () => { SetE4Rays(_onClicksTra, true); });
          
        }

        private void SetE4Rays(Transform parent,bool isRay)
        {
            for (int i = 0; i < parent.childCount; i++)            
                 parent.GetChild(i).GetEmpty4Raycast().raycastTarget = isRay;                           
        }

        private void EndDrag(Vector3 pos, int dragType, int index, bool isMatch)
        {

            _onClicksTra.SetSiblingIndex(3);
            _curDrager.transform.SetSiblingIndex(_tierIndex);

            SetE4Rays(_onClicksTra, false);

            var name = _curDrager.name;
            var spineGo = _curDrager.transform.GetChild(0).gameObject;
            var isCorrect = _curPx.IsCorrect(name);
            var feedbackSpineName = _curPx.GetFeedbackSpineName(name);
            var showSpineName = _curPx.GetShowSpineName(name);
         
            if (isMatch)
            {
                var otherDic = _curPx.LevelOtherBoomSpineName;
                var okSpineName = _curPx.OkSpineName;
                var okSpineGo = FindGo(_oksTra, okSpineName + "2");

                PlaySpine(spineGo, feedbackSpineName,()=> {    //泡泡爆炸Spine
                    PlaySpine(okSpineGo, okSpineName);  //泡泡显示Spine
                }); 
                              
                var successTime = PlaySuccessSound();
                Delay(0.8f, () => { PlayVoice(0); });
                var attackTime = PlaySpine(_pxGo, _curPx.PxAttackSpineName);//螃蟹攻击Spine       

                Delay(attackTime, () => {
                    PlaySpine(_pxGo, _curPx.PxDaiJiSpineName, null, true);
                    foreach (var other in otherDic)
                    {
                        var otherGo = FindGo(_onClicksTra, other.Key).transform.transform.GetChild(0).gameObject;
                        PlaySpine(otherGo, other.Value);
                    }
                });

                var time = successTime > attackTime ? successTime : attackTime;
                float offsetTime = 0;
                if (successTime > attackTime)
                {
                    if (successTime < 2.8f)
                        offsetTime = 2.8f - successTime;
                }
                else if (successTime <= attackTime)
                    offsetTime = 0.867f;

                Delay(time + offsetTime, () => {

                    HideChilds(_hearsTra, _curLevelIndex.ToString());
                    _curLevelIndex++;
                    bool isOver = _curLevelIndex > _levelIds.Count - 1;
                    if (isOver)
                    {
                        ShowChilds(_incosTra, 1); HideChilds(_incosTra, 0);
                        Delay(2, GameOver);
                        return;
                    }
                    ShowLevel();

                });
            }
            else
            {
                _curDrager.transform.GetRectTransform().anchoredPosition = _randomPos[_tierIndex];

                if (_curPx.IsCorrect(name))                
                    feedbackSpineName = _curPx.CorrectSpineErrorFeedbackName;
                
                PlaySpine(spineGo, feedbackSpineName, () => { PlaySpine(spineGo, showSpineName, null, true); });
                var failTime = PlayFailSound();
                var erroTime = PlaySpine(_pxGo, _curPx.PxErrorSpineName);
                var time = failTime > erroTime ? failTime : erroTime;
                Delay(time, () => { PlaySpine(_pxGo, _curPx.PxDaiJiSpineName, null, true); SetE4Rays(_onClicksTra, true); });
            }
        }

        private void StartDrag(Vector3 pos, int dragType, int index)
        {
            _curDrager = GetCurDrager(dragType, index);
            _tierIndex= _curDrager.transform.GetSiblingIndex();

            _curDrager.transform.SetAsLastSibling();

            _onClicksTra.SetSiblingIndex(4);
        }


        private mILDrager GetCurDrager(int dragType, int index)
        {
            mILDrager mILDrager = null;
            foreach (var item in _mILDragers)
            {
                if (item.dragType==dragType && item.index ==index)
                {
                    mILDrager = item;
                    break;
                }
            }
            return mILDrager;
        }

        private List<Vector2> RandomLevelDragPos()
        {
            List<Vector2> tempPos = new List<Vector2>();
            while (true)
            {
                var posIndex = Random.Range(0, 3);
                var isContains = tempPos.Contains(_levelPos[posIndex]);

                if (!isContains)
                    tempPos.Add(_levelPos[posIndex]);

                if (tempPos.Count == 3)
                    break;
            }


            return tempPos;
        }

        private void RandomLevelIds()
        {
            while (true)
            {
                int id = Random.Range(1, 4);
                bool isContain = _levelIds.Contains(id);
                if (!isContain)
                    _levelIds.Add(id);

                if (_levelIds.Count == 3)
                    break;
            }          
        }


        private void OnClicks(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            var name = go.name;
            var spineGo = go.transform.GetChild(0).gameObject;
            var isCorrect =  _curPx.IsCorrect(name);
            var feedbackSpineName = _curPx.GetFeedbackSpineName(name);
            var showSpineName = _curPx.GetShowSpineName(name);
          
            if (isCorrect)
            {
                var otherDic = _curPx.LevelOtherBoomSpineName;
                PlaySpine(spineGo, feedbackSpineName); //泡泡爆炸Spine
                var okSpineName = _curPx.OkSpineName;
                var okSpineGo = FindGo(_oksTra, okSpineName + "2");
              
                PlaySpine(okSpineGo, okSpineName);  //泡泡显示Spine

            

                var successTime=  PlaySuccessSound();
                Delay(0.8f, () => { PlayVoice(0); });             
                var attackTime = PlaySpine(_pxGo, _curPx.PxAttackSpineName);//螃蟹攻击Spine       

                Delay(attackTime, () => {
                    PlaySpine(_pxGo, _curPx.PxDaiJiSpineName, null, true);
                    foreach (var other in otherDic)
                    {
                        var otherGo = FindGo(_onClicksTra, other.Key).transform.transform.GetChild(0).gameObject;
                        PlaySpine(otherGo, other.Value);
                    }
                });

                var time = successTime > attackTime ? successTime : attackTime;
                float offsetTime = 0;
                if (successTime > attackTime)
                {
                    if (successTime<2.8f)                    
                        offsetTime = 2.8f - successTime;                                        
                }
                else if (successTime <= attackTime)
                    offsetTime = 0.867f;
               
                Delay(time+ offsetTime, () => { 
                    
                    HideChilds(_hearsTra, _curLevelIndex.ToString());
                    _curLevelIndex++;
                    bool isOver = _curLevelIndex > _levelIds.Count - 1;
                    if (isOver)
                    {                      
                        ShowChilds(_incosTra, 1); HideChilds(_incosTra, 0);                     
                        Delay(2, GameOver);
                        return;
                    }
                    ShowLevel();
                   
                });
                        
            }
            else
            {             
                PlaySpine(spineGo, feedbackSpineName, () => { PlaySpine(spineGo, showSpineName, null, true);});
                var failTime =  PlayFailSound();
                var erroTime = PlaySpine(_pxGo, _curPx.PxErrorSpineName);                
                var time = failTime > erroTime ? failTime : erroTime;             
                Delay(time, () => { PlaySpine(_pxGo, _curPx.PxDaiJiSpineName, null, true); _isPlaying = false; });
            }
        }


        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();                   
        }

        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show();
            _okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () =>
            {
                AddEvent(_replaySpine, (go) =>
                {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        _okSpine.Hide();
                        PlayCommonBgm(8); //ToDo...改BmgIndex
                        GameInit();
                        ShowLevel();
                       
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                    PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dTT.Show();
                        BellSpeck(_dTT,2,null,null,RoleType.Child);                     		
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
            Delay(4.0f, GameReplayAndOk);
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

        #region Spine相关

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                child.GetComponent<SkeletonGraphic>().Initialize(true);
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

        private GameObject FindGo(Transform parent, string goName)
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
            {
                case RoleType.Bd:
                    daiJi = "bd-daiji"; speak = "bd-speak";
                    break;
                case RoleType.Xem:
                    daiJi = "daiji"; speak = "speak";
                    break;
                case RoleType.Child:
                    daiJi = "animation"; speak = "animation2";
                    break;
                case RoleType.Adult:
                    daiJi = "daiji"; speak = "speak";
                    break;
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
