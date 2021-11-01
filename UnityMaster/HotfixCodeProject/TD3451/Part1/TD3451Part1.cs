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
	
    public class TD3451Part1
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
		private GameObject _sTT;				
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Transform _bgsTra;
        private Transform _levelTra;

        private int _curDragNum;
        private mILDrager[] _curMILDrags;
        private mILDrager _curMILDrag;
        private Vector3 _curDragInitPos;
     
        private GameObject _level1Hwx;
        private bool _level1DragEffect;

        private Coroutine _level1MoveCor;

        private GameObject _mymask;
        private RectTransform _moveBg0;
        private RectTransform _moveBg1;
        private RectTransform _moveBg2;
        private RectTransform _moveBg3;
        private RectTransform _moveBg4;
        private RectTransform _moveBg5;

        private RectTransform _cao1;
        private RectTransform _cao2;

        private Vector2 _startPos;
        private Vector2 _startPos2;

        private float _endPosX;
        private GameObject _cao;
        private Transform _show;
        private Transform _showPos;

        private Image _wImg;
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
			_sTT = curTrans.GetGameObject("sTT");

            _bgsTra = curTrans.Find("BG");
            _levelTra = curTrans.Find("levels");
            _level1Hwx = curTrans.Find("levels/level1/hwx").gameObject;

            _wImg = curTrans.GetImage("wMask");
             _mymask = curTrans.Find("mymask").gameObject;

            _moveBg0 = curTrans.GetRectTransform("BG/cc/0");
            _moveBg1 = curTrans.GetRectTransform("BG/cc/1");
            _moveBg2 = curTrans.GetRectTransform("BG/cc/2");
            _moveBg3 = curTrans.GetRectTransform("BG/cc/3");
            _moveBg4 = curTrans.GetRectTransform("BG/cc/4");
            _moveBg5 = curTrans.GetRectTransform("BG/cc/5");

            _cao = curTrans.GetGameObject("cao");
            _cao1 = curTrans.GetRectTransform("cao/Cqc0");
            _cao2 = curTrans.GetRectTransform("cao/Cqc1");

            _show = curTrans.Find("show");
            _showPos = curTrans.Find("showpos");

            GameInit();
            GameStart();
        }

        void InitData()
        {
            _curDragNum = 0;
            _talkIndex = 1;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _level1DragEffect = false;
            Input.multiTouchEnabled = false;
            _startPos = new Vector2(3840, 0);
            _startPos2 = new Vector2(3840, 300);
            _endPosX = -3840f;
        }

        void GameInit()
        {
            InitData();

         
            DOTween.Clear();         
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); 
			StopAllCoroutines();
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _dTT.Hide(); _sTT.Hide(); _mymask.Hide(); _cao.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine); _show.gameObject.Hide(); _wImg.gameObject.Hide();
            _wImg.color = new Color(1, 1, 1, 0);
            InitLevelPos(_showPos,_show);

            ShowAllChilds(_levelTra);

            //初始化游戏Level1
            var level1 = _levelTra.Find("level1");
            //Init-Level1-Spine
            var level1AllSpines = level1.GetComponentsInChildren<SkeletonGraphic>(true);
            InitLevelSpine(level1AllSpines, spine => {

                var name = spine.name;
                switch (name)
                {
                    case "kla3":
                    case "klb3":
                    case "klc3":    //什么都不做
                        break;
                    default:
                        PlaySpine(spine.gameObject, spine.name);
                        break;
                }              
            });        
            //Init-Level1-Pos
            var levle1DragBoxPos = level1.Find("DragBoxpos"); var level1DragBox = level1.Find("DragBox");
            var hwxPos = level1.Find("hwxpos"); var hwx = level1.Find("hwx");
            InitLevelPos(levle1DragBoxPos, level1DragBox);
            hwx.localPosition = new Vector2(hwxPos.localPosition.x, hwxPos.localPosition.y);        
           
            //初始化游戏Level2
            var level2 = _levelTra.Find("level2");
            //Init-Level2-Spine
            var level2AllSpines = level2.GetComponentsInChildren<SkeletonGraphic>(true);
            InitLevelSpine(level2AllSpines, spine => {

                var name = spine.name;
                switch (name)
                {
                    case "Ba6":
                    case "Ba6a":
                    case "Ba7":
                    case "Ba7a":
                    case "Ba8":
                    case "Ba8a":
                    case "Ba9":
                    case "Ba9a":
                        spine.gameObject.Hide();
                        break;
                    case "Ba10":
                        PlaySpine(spine.gameObject, spine.name);
                        spine.color = new Color(1, 1, 1, 1);
                        break;
                    case "klCd2":                     
                        spine.gameObject.Hide();
                        break;
                    case "xem2":
                        PlaySpine(spine.gameObject, spine.name,null,true);
                        break;
                    default:
                        PlaySpine(spine.gameObject, spine.name);
                        break;
                }
            });
            //Init-Level2-Pos
            var level2DragBoxPos = level2.Find("DragBoxpos"); var level2DragBox = level2.Find("DragBox");
            InitLevelPos(level2DragBoxPos, level2DragBox);
            level2.Find("yingzi").GetImage().color = new Color(1, 1, 1, 0);

            //初始化游戏Level3
            var level3 = _levelTra.Find("level3");
            //Init-Level3-Spine
            var level3AllSpines = level3.GetComponentsInChildren<SkeletonGraphic>(true);
            InitLevelSpine(level3AllSpines, spine => {

                var name = spine.name;
                switch (name)
                {

                    case "Bb6":
                    case "Bb6a":
                    case "Bb7":
                    case "Bb7a":
                    case "Bb8":
                    case "Bb8a":
                    case "Bb9":
                    case "Bb9a":
                        spine.gameObject.Hide();
                        break;
                    case "Bb10":
                        PlaySpine(spine.gameObject, spine.name);
                        spine.color = new Color(1, 1, 1, 1);
                        break;
                    case "klCa2":                       
                        spine.gameObject.Hide();
                        break;
                    case "xem2":
                        PlaySpine(spine.gameObject, spine.name, null, true);
                        break;
                    default:
                        PlaySpine(spine.gameObject, spine.name);
                        break;
                }
            });
            //Init-Level3-Pos
            var level3DragBoxPos = level3.Find("DragBoxpos"); var level3DragBox = level3.Find("DragBox");
            InitLevelPos(level3DragBoxPos, level3DragBox);
            level3.Find("yingzi").GetImage().color = new Color(1, 1, 1, 0);

            //初始化游戏Level4
            var level4 = _levelTra.Find("level4");
            //Init-Level4-Spine
            var level4AllSpines = level4.GetComponentsInChildren<SkeletonGraphic>(true);
            InitLevelSpine(level4AllSpines, spine => {

                var name = spine.name;
                switch (name)
                {
                    case "klBc1":
                    case "klBb1":
                    case "klBd1":
                    case "klBa1":
                        PlaySpine(spine.gameObject, spine.name);
                        spine.color = new Color(1, 1, 1, 1);
                        break;
                    case "klBc2":
                    case "klBb2":
                    case "klBd2":
                    case "klBa2":
                    case "klCc1":
                    case "klCb1":
                    case "klCd1":
                    case "klCa1":
                        spine.gameObject.Hide();
                        break;
                    case "xem2":
                        PlaySpine(spine.gameObject, spine.name, null, true);
                        break;
                    default:
                        PlaySpine(spine.gameObject, spine.name);
                        break;
                }
            });
            //Init-Level4-Pos
            var level4DragBoxPos = level4.Find("DragBoxpos"); var level4DragBox = level4.Find("DragBox");
            InitLevelPos(level4DragBoxPos, level4DragBox);
            var level4XemPos = level4.Find("xempos"); var level4Xem = level4.Find("xem2");
            level4Xem.localPosition = new Vector2(level4XemPos.localPosition.x, level4XemPos.localPosition.y);
            var level4DropBox = level4.Find("DropBox");

            for (int i = 0; i < 4; i++)           
                level4DropBox.Find("yingzi"+(i+1)).GetImage().color = new Color(1, 1, 1, 0);

            HideAllChilds(_levelTra);                  
            HideAllChilds(_bgsTra);

            EnterLevel(-1,1,0,-1,0,"level1",(pos,type,index)=>{ if (_level1DragEffect)
                {
                    _level1DragEffect = false;
                    _curMILDrag.drops[0].transform.DOShakePosition(2f, 3f).OnComplete(() => { _level1DragEffect = true; });
                }  }, Level1DragEnd,-1);

            _moveBg0.anchoredPosition = new Vector2(0, 0);
            _moveBg1.anchoredPosition = new Vector2(3840, 0);
            _moveBg2.anchoredPosition = new Vector2(0,300);
            _moveBg3.anchoredPosition = new Vector2(3840,300);
            _moveBg4.anchoredPosition = new Vector2(0, 0);
            _moveBg5.anchoredPosition = new Vector2(3840, 0);
            _cao1.anchoredPosition = new Vector2(0, 0);
            _cao2.anchoredPosition = new Vector2(3840, 0);

        }

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                   PlaySpine(_startSpine, "bf", () => {                    
                        PlayBgm(0);
                        _startSpine.Hide();                   
                        _sTT.Show();
                       BellSpeck(_sTT, 0, null, () => { ShowVoiceBtn(); },RoleType.Child, SoundManager.SoundType.VOICE); 
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
                    BellSpeck(_sTT, 1, null, () => { StartGame(); }, RoleType.Child, SoundManager.SoundType.VOICE);         
                    break;                                
            }
            _talkIndex++;
        }

        #region 游戏逻辑
      
        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {          
            _mask.Hide();
            _sTT.Hide();
            _level1MoveCor = _mono.StartCoroutine(MovekLevel1Hwx());           
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
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); 
					RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();                      
                        PlayBgm(0);
                        GameInit();
                        StartGame();            
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
					PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();
                        _dTT.Show();
                        BellSpeck(_dTT, 4, null, null, RoleType.Child, SoundManager.SoundType.VOICE);
                      					
                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }


        /// <summary>
        /// Init关卡Spine
        /// </summary>
        /// <param name="spines">所有Spines</param>
        /// <param name="callBack">回调</param>
        void InitLevelSpine(SkeletonGraphic[] spines, Action<SkeletonGraphic> callBack)
        {
            foreach (var spine in spines)
            {
                spine.Initialize(true);
                callBack?.Invoke(spine);
            }
        }

        /// <summary>
        /// Init关卡Pos
        /// </summary>
        /// <param name="posTra">Bass位置Tra</param>
        /// <param name="tra">Set位置Tra</param>
        void InitLevelPos(Transform posTra, Transform tra)
        {

            for (int i = 0; i < posTra.childCount; i++)
            {
                var posChild = posTra.GetChild(i);
                var dragChild = tra.Find(posChild.name);
                dragChild.localPosition = posChild.localPosition;
                dragChild.gameObject.Show();
            }
        }

        /// <summary>
        /// 初始化关卡Drag
        /// </summary>
        /// <param name="drags"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        void InitLevelDrag(mILDrager[] drags, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null)
        {
            _curMILDrags = drags;
            foreach (var drag in _curMILDrags)
            {
                drag.isActived = true;
                drag.SetDragCallback(dragStart, draging, dragEnd);
            }
        }

        /// <summary>
        /// 获取当前Drag
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private mILDrager GetCurDrag(int index)
        {
            _curMILDrag = null;
            foreach (var drag in _curMILDrags)
            {
                if (drag.index == index)
                {
                    _curMILDrag = drag;
                    break;
                }
            }
            return _curMILDrag;
        }

        /// <summary>
        /// 拖拽成功
        /// </summary>
        private void DragEndSuccess(int soundIndex,string levelName,Action<SkeletonGraphic[],SkeletonGraphic[]> callBack)
        {
            PlaySound(soundIndex);
            _curMILDrag.gameObject.Hide();
            _curMILDrag.isActived = false;
            var name = _curMILDrag.name;
            var dropBox = _levelTra.Find(levelName+"/DropBox");
            var dropChild = dropBox.Find(name);         
            var dropBoxSpines = dropBox.GetComponentsInChildren<SkeletonGraphic>(true);
            var dropBoxChildSpines = dropChild.GetComponentsInChildren<SkeletonGraphic>(true);
            callBack?.Invoke(dropBoxSpines, dropBoxChildSpines);

        }

        private void StartNextLevel(float time,Action callBack)
        {
            Delay(time, ()=> {
                _curDragNum++;
                if (_curDragNum == _curMILDrags.Length)
                    callBack?.Invoke();
            });
        }

        /// <summary>
        /// 拖拽失败
        /// </summary>
        private void DragEndFail()
        {
            PlaySound(2);
            _curMILDrag.transform.localPosition = _curDragInitPos;
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <returns></returns>
        private bool IsMatch()
        {
            var curDrop = _curMILDrag.GetComponent<mILDrager>().drops[0];
            var pC2D = curDrop.GetComponent<PolygonCollider2D>();
            return pC2D.OverlapPoint(Input.mousePosition);
        }


        /// <summary>
        /// 进入关卡
        /// </summary>
        /// <param name="hideBgIndex">Hide背景Index</param>
        /// <param name="showBgIndex">Show背景Index</param>
        /// <param name="bellSpriteIndex">BellSprite组件图片Index</param>
        /// <param name="hideLevelIndex">Hide关卡Index</param>
        /// <param name="showLevelIndex">Show关卡Index</param>
        /// <param name="levelName">关卡Name</param>
        /// <param name="dragEnd">拖拽End回调</param>
        /// <param name="voiceIndex">Bell说话VoiceIndex</param>
        private void EnterLevel(int hideBgIndex, int showBgIndex, int bellSpriteIndex, int hideLevelIndex, int showLevelIndex, string levelName, Action<Vector3, int, int> darging, Action<Vector3, int, int, bool> dragEnd, int voiceIndex)
        {
           
            _curDragNum = 0; _mymask.Show();

            if (hideBgIndex != -1)
                HideChilds(_bgsTra, hideBgIndex);


            ShowChilds(_bgsTra, showBgIndex);
            ShowChilds(_bgsTra, 0, go => {
                var raw = go.GetComponent<RawImage>();
                var bellSprite = go.GetComponent<BellSprites>();
                raw.texture = bellSprite.texture[bellSpriteIndex];
            });

            if (hideLevelIndex != -1)
                HideChilds(_levelTra, hideLevelIndex);

            ShowChilds(_levelTra, showLevelIndex);

            var levelAllMILDrager = _levelTra.Find(levelName).GetComponentsInChildren<mILDrager>();
            InitLevelDrag(levelAllMILDrager, (pos, type, index) => {
                GetCurDrag(index);
                _curMILDrag.transform.SetAsLastSibling();
                _curDragInitPos = _curMILDrag.transform.localPosition;
                _level1DragEffect = true;

            }, darging, dragEnd);

            if (voiceIndex == -1)
            {
                _mymask.Hide();
            }
            else
            {
                BellSpeck(_sTT, voiceIndex, null, () => { _mymask.Hide(); }, RoleType.Child, SoundManager.SoundType.VOICE);
            }


        }

        private void Level1DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            isMatch = IsMatch();

            if (isMatch)
            {
                DragEndSuccess(0, "level1", (dropBoxSpines, dropBoxChildSpines) => {
                    PlaySpine(dropBoxChildSpines[0].gameObject, dropBoxChildSpines[0].name + "2");
                    var time = PlaySpine(dropBoxChildSpines[1].gameObject, dropBoxChildSpines[1].name);
                    StartNextLevel(time, () => {
                        StopCoroutines(_level1MoveCor);
                        _mono.StartCoroutine(BlackCurtainTransition(_wImg, () => {
                            EnterLevel(1, 2, 1, 0, 1, "level2", null, Level2DragEnd, 2);
                        }));
                      
                    });
                });

            }
            else
            {
                DragEndFail();
                PlaySpine(_level1Hwx, _level1Hwx.name + "2", () => { PlaySpine(_level1Hwx, _level1Hwx.name + "2", () => { PlaySpine(_level1Hwx, _level1Hwx.name); }); });
            }
        }
        private void Level2DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            isMatch = IsMatch();

            if (isMatch)
            {
                DragEndSuccess(1, "level2", (dropBoxSpines, dropBoxChildSpines) => {
                    dropBoxChildSpines[0].gameObject.Show();
                    dropBoxChildSpines[1].gameObject.Show();
                    PlaySpine(dropBoxChildSpines[0].gameObject, dropBoxChildSpines[0].name);
                    var time = PlaySpine(dropBoxChildSpines[1].gameObject, dropBoxChildSpines[1].name,()=> { PlaySpine(dropBoxChildSpines[1].gameObject, "kong"); });
                    StartNextLevel(time,()=> {
                        var klCd2 = _levelTra.Find("level2/klCd2").gameObject;
                        var ba10 = _levelTra.Find("level2/Ba10").gameObject;
                        var xem2 = _levelTra.Find("level2/xem2").gameObject;
                        ba10.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 0.5f);
                        Delay(0.5f, () => {
                            klCd2.Show();
                            _levelTra.Find("level2/yingzi").GetImage().DOColor(new Color(1,1,1,1), 0.5f);
                            PlaySpine(klCd2, klCd2.name, () => {

                                _mono.StartCoroutine(BlackCurtainTransition(_wImg, () => {
                                    EnterLevel(1, 2, 2, 1, 2, "level3", null, Level3DragEnd, -1);
                                }));

                              
                            });
                            foreach (var spine in dropBoxSpines)
                                spine.gameObject.Hide();
                            PlaySpine(xem2, "xem4");
                            PlaySound(4);
                        });

                    });
                   
                });
            }
            else
            {
                DragEndFail();
            }
        }
        private void Level3DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            isMatch = IsMatch();

            if (isMatch)
            {
                DragEndSuccess(1, "level3", (dropBoxSpines, dropBoxChildSpines) => {
                    dropBoxChildSpines[0].gameObject.Show();
                    dropBoxChildSpines[1].gameObject.Show();
                    PlaySpine(dropBoxChildSpines[0].gameObject, dropBoxChildSpines[0].name);
                    var time = PlaySpine(dropBoxChildSpines[1].gameObject, dropBoxChildSpines[1].name, () => { PlaySpine(dropBoxChildSpines[1].gameObject, "kong"); });
                    StartNextLevel(time, () => {
                        var klCa2 = _levelTra.Find("level3/klCa2").gameObject;
                        var ba10 = _levelTra.Find("level3/Bb10").gameObject;
                        var xem2 = _levelTra.Find("level3/xem2").gameObject;
                        ba10.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 0.5f);
                        Delay(0.5f, () => {
                            klCa2.Show();
                            _levelTra.Find("level3/yingzi").GetImage().DOColor(new Color(1, 1, 1, 1), 0.5f);
                            PlaySpine(klCa2, klCa2.name, () => {
                                _mono.StartCoroutine(BlackCurtainTransition(_wImg, () => {
                                    EnterLevel(1, 2, 2, 2, 3, "level4", null, Level4DragEnd, 3);
                                }));                           
                            });                     
                            foreach (var spine in dropBoxSpines)
                                spine.gameObject.Hide();
                            PlaySpine(xem2, "xem4");
                            PlaySound(4);
                        }); 
                    });

                });
            }
            else
            {
                DragEndFail();
            }
        }
        private void Level4DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            isMatch = IsMatch();

            if (isMatch)
            {
                DragEndSuccess(0, "level4", (dropBoxSpines, dropBoxChildSpines) => {
                    dropBoxChildSpines[0].DOColor(new Color(1, 1, 1, 0), 0.5f);
                    Delay(0.5f, () => {

                        var dropBox = _levelTra.Find("level4/DropBox");
                        var name = "yingzi"+ _curMILDrag.name;
                        var yingzi = dropBox.Find(name);
                        yingzi.GetImage().DOColor(new Color(1, 1, 1, 1), 0.5f);
                    });                 
                    dropBoxChildSpines[1].gameObject.Show();   //发光
                    dropBoxChildSpines[2].gameObject.Show();   //图形
                    PlaySpine(dropBoxChildSpines[1].gameObject, dropBoxChildSpines[1].name, () => { PlaySpine(dropBoxChildSpines[1].gameObject, "kong"); });
                    var time = PlaySpine(dropBoxChildSpines[2].gameObject, dropBoxChildSpines[2].gameObject.name);
                    StartNextLevel(time, () => {
                        foreach (var spine in dropBoxSpines)
                        {
                            var spineName = spine.name;

                            switch (spineName)
                            {
                                case "klCc1":
                                    PlaySpine(spine.gameObject, "klCc2", null, true);
                                    break;
                                case "klCb1":
                                    PlaySpine(spine.gameObject, "klCb2", null, true);
                                    break;
                                case "klCd1":
                                    PlaySpine(spine.gameObject, "klCd2", null, true);
                                    break;
                                case "klCa1":
                                    PlaySpine(spine.gameObject, "klCa2", null, true);
                                    break;
                            }
                        }
                        var xem2 = _levelTra.Find("level4/xem2").gameObject;
                        PlaySound(4);
                        PlaySpine(xem2, "xem4", () => {PlaySpine(xem2, "xem1", null, true);
                        var xem2Rect = xem2.transform.GetRectTransform();
                        xem2Rect.DOAnchorPosX(1920, 3);
                          
                         Delay(0.7f, ()=> {
                             _mono.StartCoroutine(BlackCurtainTransition(_wImg, () => {
                                 OverAnimation();
                             }));

                         });               
                        });
                    });

                });
            }
            else
            {
                DragEndFail();
            }
        }


        private void OverAnimation()
        {
           
            PlaySound(3);
            HideAllChilds(_levelTra);
            ShowChilds(_bgsTra, 3);
            _show.gameObject.Show();
            _cao.gameObject.Show();
            _mono.StartCoroutine(IEUpdate());
            For(_show, 0.1f, (index, tra) => {
                var go = tra.gameObject;
                var rect = tra.GetRectTransform();

                var sPos = rect.anchoredPosition;
                var ePos = new Vector2(sPos.x + 160, sPos.y);
                var name = go.name;
                PlaySpine(go, go.name, null, true);

                var to = rect.DOAnchorPos(ePos, index + 1);
                var from = rect.DOAnchorPos(sPos, index + 2);


                DOTween.Sequence().Append(to)
                                  .AppendInterval(index + 1)
                                  .Append(from)
                                  .SetLoops(-1);
            });

            Delay(4, GameSuccess);
        }

        private IEnumerator IEUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.02f);
                MoveBg(_moveBg0,_startPos,3,_endPosX);
                MoveBg(_moveBg1, _startPos, 3, _endPosX);
                MoveBg(_moveBg2, _startPos2, 10, _endPosX);
                MoveBg(_moveBg3, _startPos2, 10, _endPosX);
                MoveBg(_moveBg4, _startPos, 10, _endPosX);
                MoveBg(_moveBg5, _startPos, 10, _endPosX);
                MoveBg(_cao1, _startPos, 20, _endPosX);
                MoveBg(_cao2, _startPos, 20, _endPosX);

            }
        }

        private void MoveBg(RectTransform rect,Vector2 startPos,float speed,float endXPox)
        {
            if (rect.anchoredPosition.x <= endXPox)
                rect.anchoredPosition = startPos;
            rect.Translate(Vector2.left * speed*(Screen.width/1920f));
        }

        IEnumerator MovekLevel1Hwx()
        {
            var rect = _level1Hwx.GetComponent<RectTransform>();
            while (true)
            {
                rect.DOAnchorPosY(-500f, 2f).OnComplete(() => { rect.DOAnchorPosY(400f, 2f); });
                yield return new WaitForSeconds(4f);
            }
        }

        private void For(Transform parent, float delay, Action<int, Transform> callBack)
        {
            _mono.StartCoroutine(IEFor(parent, delay, callBack));
        }

        IEnumerator IEFor(Transform parent, float delay, Action<int, Transform> callBack)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(delay);
                var child = parent.GetChild(i);
                callBack?.Invoke(i, child);
            }
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

        private void HideChilds(Transform parent,int index, Action<GameObject> callBack = null)
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



        //白幕转场
        IEnumerator BlackCurtainTransition(Image _raw, Action method = null)
        {
            _raw.color = new Color(255, 255, 255, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.white, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.2f);

            _raw.DOColor(new Color(255, 255, 255, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

            method?.Invoke();
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

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			switch(roleType)
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
