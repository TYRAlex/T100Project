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
	
    public class TD91252Part1
    {

        public enum ClickEnum
        {
            First,
            Second
        }
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;


        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;		
	    
		private GameObject _nextSpine;		
		
          private GameObject _dDD;
          private GameObject _dDDL;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
      
        private bool _isPlaying;
        private int timeIndex;
        private bool isAgain;  //判断第二关花朵是否再生成，true（生成）false(不生成）


        //LevelOne
        private GameObject _levelOne;
        private Transform _startFlower;
        private GameObject[] _startFlowers;

        private Transform _endFlower;
        private GameObject[] _endFlowers;

        private Transform _dragerFlowr;
        private ILDrager[] _ilDragers;
        private List<int> _dragerList;
        private string[] _dragerAniName;
        private int dragerNum;
        private int dragerSum;

        private Transform _droperFlowr;
        private ILDroper[] _ilDropers;

        private Transform _succeedFlower;
        private GameObject[] _succeedFlowers;
        private bool isSucceed;
               

        //LevelTwo
        private GameObject _levelTwoBg;
        private GameObject _levelTwo;

        private Transform _xemPanel;
        private GameObject[] _xemPanels;
        private Transform _flowerPanel;
        private Empty4Raycast[] _e4rFlower;
        private List<int> _e4rList;
        private string[] _e4rFlowerAni;
        private int e4rNum;

        private Transform _startGame;
        private GameObject[] _startGames;
        private GameObject levelMask;
        private GameObject endMask;
        private ClickEnum clickEnum;
        private int clickFlowerSum;

        private Transform _dragerFlowerPosParent;
        private Transform[] _dragerFlowerPos;

        private PolygonCollider2D _flowerClick;       

        private Vector3 _dragerStartPos;

        private Coroutine timeCoroutine;
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            Input.multiTouchEnabled = false;

            endMask = curTrans.Find("endMask").gameObject;
            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");			
			_nextSpine = curTrans.GetGameObject("nextSpine");						
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");

			
			_dDD = curTrans.GetGameObject("dDD");
            _dDDL = _curGo.transform.Find("dDDL").gameObject;

            GameInit();
            LevelOneFindInit();
            LevelTwoFindInit();
            GameStart();
        }
        void Update()
        {
            Succeed();
        }
        void InitData()
        {
            _isPlaying = true;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _dragerAniName = new string[] { "maoA", "maoB", "maoC", "maoD", "maoE", "maoF", "maoG" };
            _e4rFlowerAni = new string[] {"huaA","huaB", "huaC", "huaD", "huaE","huaF", "huaG", "huaH", "huaI", "huaJ", "huaK", "huaL", "huaM","huaN", "huaO" };
        }

        void GameInit()
        {
            endMask.SetActive(false);
            InitData();

            _talkIndex = 1;
            timeIndex = -1;
            isAgain = true;
            timeCoroutine = null;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); 
			StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); 
			_nextSpine.Hide();	
            
		    _dDD.Hide();
            _dDDL.Hide();

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
			RemoveEvent(_nextSpine);           
        }
        private void LevelOneFindInit()
        {
            _levelOne = _curGo.transform.Find("LevelOne").gameObject;         
            levelMask = _curGo.transform.Find("levelMask").gameObject;

            _startFlower = _curGo.transform.Find("LevelOne/StartFlower");
            _startFlowers = new GameObject[_startFlower.childCount];
            for (int i = 0; i < _startFlowers.Length; i++)
            {
                _startFlowers[i] = _startFlower.GetChild(i).gameObject;                
            }
            _flowerClick = _curGo.transform.Find("LevelOne/StartFlower/flowerClick").GetComponent<PolygonCollider2D>();
            Util.AddBtnClick(_flowerClick.gameObject, FlowerClickEvent);

            _endFlower = _curGo.transform.Find("LevelOne/EndFlower");
            _endFlowers = new GameObject[_endFlower.childCount];
            for (int i = 0; i < _endFlowers.Length; i++)
            {
                _endFlowers[i] = _endFlower.GetChild(i).gameObject;
            }

            _dragerFlowerPosParent = _curGo.transform.Find("LevelOne/DragerFlowerPos");
            _dragerFlowerPos = new Transform[_dragerFlowerPosParent.childCount];
            for (int i = 0; i < _dragerFlowerPos.Length; i++)
            {
                _dragerFlowerPos[i] = _dragerFlowerPosParent.GetChild(i);
            }


            _dragerFlowr = _curGo.transform.Find("LevelOne/DragerFlower");
            _dragerList = new List<int>();
            _ilDragers = _dragerFlowr.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < _ilDragers.Length; i++)
            {                
                _ilDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                _ilDragers[i].isActived = true;
                _ilDragers[i].gameObject.Hide();
                _ilDragers[i].index = i;
                _dragerList.Add(i);
            }            

            _droperFlowr = _curGo.transform.Find("LevelOne/DroperFlower");
            _ilDropers = _droperFlowr.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < _ilDropers.Length; i++)
            {
                _ilDropers[i].index = i;
                _ilDropers[i].SetDropCallBack(OnAfter);
                _ilDropers[i].isActived = true;
            }

            _succeedFlower = _curGo.transform.Find("LevelOne/SucceedFlower");
            _succeedFlowers = new GameObject[_succeedFlower.childCount];
            for (int i = 0; i < _succeedFlowers.Length; i++)
            {
                _succeedFlowers[i] = _succeedFlower.GetChild(i).gameObject;
                _succeedFlowers[i].Hide();
            }
            _levelOne.Hide();
        }
        private void LevelTwoFindInit()
        {
            _levelTwo = _curGo.transform.Find("LevelTwo").gameObject;          
            _levelTwoBg = _curGo.transform.Find("BG/levelTwoBg").gameObject;            
            _xemPanel = _curGo.transform.Find("LevelTwo/xemPanel");
            _xemPanels = new GameObject[_xemPanel.childCount];
            for (int i = 0; i < _xemPanels.Length; i++)
            {
                _xemPanels[i] = _xemPanel.GetChild(i).gameObject;
            }
            //_xemPanels[1].Hide();
            _xemPanels[2].transform.GetChild(0).GetComponent<Text>().text = "90";            

            _flowerPanel = _curGo.transform.Find("LevelTwo/flowerPanel");
            _e4rList = new List<int>();
            _e4rList.Clear();
            _e4rFlower = _flowerPanel.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _e4rFlower.Length; i++)
            {
                Util.AddBtnClick(_e4rFlower[i].gameObject, LevelTwoClick);

                _e4rList.Add(i);
            }            
            _startGame = _curGo.transform.Find("LevelTwo/startGame");
            _startGames = new GameObject[_startGame.childCount];
            for (int i = 0; i < _startGames.Length; i++)
            {
                _startGames[i] = _startGame.GetChild(i).gameObject;
                _startGames[i].Hide();
            }           

            _levelTwo.Hide();
            _levelTwoBg.Hide();           
        }
        private string dragerName;
        private void LevelOneStart()
        {
            dragerNum = -1;
            dragerSum = 0;
            _levelTwoBg.Hide();
            _levelTwo.Hide();
            levelMask.Hide();
            _levelOne.Show();
            isSucceed = false;
            dragerName = string.Empty;

            PlayAniStart(_startFlowers, 0, "hua4",true);            
            PlayAniStart(_startFlowers, 2, "hua3",true);

            _succeedFlower.gameObject.Show();
            for (int i = 0; i < _succeedFlowers.Length; i++)
            {
                _succeedFlowers[i].Hide();
            }
            for (int i = 0; i < _endFlowers.Length; i++)
            {
                _endFlowers[i].Hide();
            }
            for (int i = 0; i < _ilDragers.Length; i++)
            {
                _ilDragers[i].transform.position = _dragerFlowerPos[i].position;
                _ilDragers[i].gameObject.Hide();              
            }
            _startFlowers[3].Hide();
            _startFlowers[1].Show();
            _startFlowers[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);           
            SpineManager.instance.DoAnimation(_startFlowers[1], "hua", false, () => 
            {
                _startFlowers[3].Show();               
            });                      
           
        }
        /// <summary>
        /// 游戏开始大花朵点击事件
        /// </summary>
        /// <param name="obj"></param>
        private void FlowerClickEvent(GameObject obj)
        {
            _startFlowers[3].Hide();
            _startFlowers[1].Hide();
            levelMask.Show();
           
            for (int i = 0; i < _ilDragers.Length; i++)
            {                
                dragerNum = RandomLevelOneDrager();                
                _ilDragers[dragerNum].gameObject.Show();
                _ilDragers[dragerNum].index = dragerNum;               
                _ilDragers[dragerNum].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);               
                SpineManager.instance.DoAnimation(_ilDragers[dragerNum].transform.GetChild(0).gameObject, _dragerAniName[i], false);
                _ilDragers[dragerNum].gameObject.name = _dragerAniName[i];               
               
            }
            BellSpeck(_dDDL, 2, null, () =>
            {               
                for (int i = 0; i < _ilDragers.Length; i++)
                {
                    _ilDragers[i].isActived = true;
                    _ilDragers[i].canMove = true;
                    _ilDropers[i].isActived = true;
                    levelMask.Hide();
                }

            }, RoleType.Adult);
        }
        private void PlayAniStart(GameObject[]goSpine,int index,string aniName,bool isLoor)
        {           
            goSpine[index].Show();
            SpineManager.instance.DoAnimation(goSpine[index], aniName, isLoor);
        } 
        /// <summary>
        /// 随机出花（毛线）
        /// </summary>
        /// <returns></returns>
        private int RandomLevelOneDrager()
        {
            int temp = -1;
            int tempDragerNum = -1;
            temp = Random.Range(0, _dragerList.Count);
            tempDragerNum = _dragerList[temp];
            _dragerList.Remove(_dragerList[temp]);
            return tempDragerNum;
        }
        private void ResetDataLevelOne()
        {
            if (_dragerList.Count == 0)
            {
                for (int i = 0; i < _ilDragers.Length; i++)
                {
                    _dragerList.Add(i);
                }
            }          
        }
        private void ResetDataLevelTwo()
        {
            _e4rList.Clear();
            if (_e4rList.Count == 0)
            {
                for (int i = 0; i < _e4rFlower.Length; i++)
                {
                    _e4rList.Add(i);
                }
            }            
        }
        private void LevelTwoStart()
        {
            _talkIndex = 2;
            e4rNum = -1;
            levelClickNum = 0;
            clickFlowerSum = 0;
            clickEnum = ClickEnum.First;
            _levelTwoBg.Show();
            _levelOne.Hide();
            _levelTwo.Show();
            levelMask.Show();

            levelFlowerAniNameFirst = string.Empty;
            levelFlowerAniNameSecond = string.Empty;
            goName = string.Empty;
             
            _xemPanels[0].Hide();
            _xemPanels[2].Show();
            _xemPanels[3].Show();
            _xemPanels[2].transform.localPosition = _xemPanels[3].transform.localPosition;
            _xemPanels[2].transform.GetChild(0).GetComponent<Text>().text = "90";

            _xemPanels[1].Show();
            SpineManager.instance.SetFreeze(_xemPanels[1], false);
            _xemPanels[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);                         
            SpineManager.instance.DoAnimation(_xemPanels[1], "time2", false);           

            for (int i = 0; i < _e4rFlower.Length; i++)
            {
                _e4rFlower[i].gameObject.Show();
            }                      
            E4rStart();
            E4rStart();                       
            _startGames[2].Hide();
            _startGames[2].transform.GetChild(0).gameObject.Hide();
        }
        private int RandomLevelTwoFlower()
        {
            int temp = -1;
            int tempE4r = -1;
            temp = Random.Range(0, _e4rList.Count);
            tempE4r = _e4rList[temp];
            _e4rList.Remove(_e4rList[temp]);
            return tempE4r;
        }
        /// <summary>
        /// 随机生成第二关花朵，每次生成15朵，共30朵，所以需要调用两次
        /// </summary>
        private void E4rStart()
        {
            for (int i = 0; i < _e4rFlower.Length / 2; i++)
            {
                e4rNum = RandomLevelTwoFlower();
                _e4rFlower[e4rNum].gameObject.Show();
                _e4rFlower[e4rNum].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_e4rFlower[e4rNum].transform.GetChild(0).gameObject, _e4rFlowerAni[i], false);
                SpineManager.instance.DoAnimation(_e4rFlower[e4rNum].transform.GetChild(1).gameObject, "kong", false);                
            }
        }
        /// <summary>
        /// 倒计时
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        IEnumerator CalcuGameTime(int temp)
        {            
            while (temp >0)
            {
                if (temp <= 10)
                {
                    PlayBgm(8, false);
                }
                yield return new WaitForSeconds(1.0f);
                temp--;
                timeIndex = temp;

                ResetLevelTwo();

                _xemPanels[2].transform.GetChild(0).GetComponent<Text>().text = temp.ToString();
                if (_xemPanels[2].transform.GetChild(0).GetComponent<Text>().text == "0")
                {
                    isSucceed = true;
                }                
            }           
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
                        _dDDL.Show();
                        BellSpeck(_dDDL, 0, null, () => 
                        {
                            SoundManager.instance.ShowVoiceBtn(true);                            
                        },RoleType.Adult);                        
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
                    BellSpeck(_dDDL, 1, null, () =>
                    {
                        StartGame(); 
                    }, RoleType.Adult);
                    break;
                case 2:
                    _mask.Hide();
                    _dDDL.Hide();
                    _startGames[0].Show();
                    PlayAniStart(_xemPanels, 0, "xem", true);
                    _mono.StartCoroutine(PlayCountDownAudios());
                    SpineManager.instance.DoAnimation(_startGames[0], "start", false, () =>
                    {
                        levelMask.Hide();
                        _startGames[0].Hide();                        
                        _xemPanels[1].GetComponent<SkeletonGraphic>().timeScale = 0.065f;
                        SpineManager.instance.DoAnimation(_xemPanels[1], "time", false);
                        timeCoroutine = _mono.StartCoroutine(CalcuGameTime(90));
                    });
                    break;
            }
            //_talkIndex++;
        }

       IEnumerator PlayCountDownAudios()
        {
            for (int i = 0; i < 3; i++)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                yield return new WaitForSeconds(1f);
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
        }

        #region 游戏逻辑
        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _dDDL.Hide();
             LevelOneStart();
        }
		/// <summary>
        /// 游戏下一步
        /// </summary>
		private void GameNext()
		{
            _mask.Show();
			_nextSpine.Show();
                    PlayOnClickSound();
            PlaySpine(_nextSpine, "next2");
            AddEvent(_nextSpine, nextGo => {                    
                RemoveEvent(_nextSpine);
                SpineManager.instance.DoAnimation(_nextSpine, "next", false, () =>
                {
                    _nextSpine.Hide();
                    _dDDL.Show();
                    NextGame();
                    BellSpeck(_dDDL, 3, null, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }, RoleType.Adult);
                });               
            });
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
                        PlayBgm(0); //ToDo...改BmgIndex
                        GameInit();       
                        //ToDo...						
                        StartGame();
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
                        //ToDo...
                        //显示Middle角色并且说话  					
                        _dDD.Show(); BellSpeck(_dDD, 4,null,null, RoleType.Adult);
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
        private int droperIndex;
        private bool OnAfter(int dragType, int index, int dropType)
        {

            droperIndex = index;
            return true;
        }       
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            _dragerStartPos = _ilDragers[index].transform.position;
            _ilDragers[index].transform.SetAsLastSibling();                                  
            SpineManager.instance.DoAnimation(_ilDragers[index].transform.GetChild(0).gameObject, _ilDragers[index].name + 2, false);            
        }

        private void OnDrag(Vector3 pos, int type, int index){}      
        private float soundTime;
        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            levelMask.Show();
            _ilDragers[index].transform.position = _dragerStartPos;                     
            SpineManager.instance.DoAnimation(_ilDragers[index].transform.GetChild(0).gameObject, _ilDragers[index].name, false);
            if (isMatch)
            {                
                if (_ilDragers[index].name == _ilDropers[droperIndex].gameObject.name)
                {                    
                    dragerSum++;
                    _ilDropers[droperIndex].isActived = false;                   
                    _ilDragers[index].gameObject.Hide();
                    _succeedFlowers[droperIndex].gameObject.Show();                   
                    SpineManager.instance.DoAnimation(_succeedFlowers[droperIndex], _ilDragers[index].name + 3, false);
                    PlayBgm(1,false);
                    soundTime = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                    SpineManager.instance.DoAnimation(_succeedFlowers[droperIndex].transform.GetChild(0).gameObject, "guangxiao", false);
                    Delay(soundTime, () => 
                    {
                        levelMask.Hide();                       
                        if (dragerSum == 7)
                        {
                            dragerName = string.Empty;
                            _endFlowers[0].Show();
                            _endFlowers[0].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            _endFlowers[0].GetComponent<SkeletonGraphic>().timeScale = 1;
                            PlayBgm(4,false);
                            SpineManager.instance.DoAnimation(_endFlowers[0], "miao", false, () =>
                            {
                                _endFlowers[0].GetComponent<SkeletonGraphic>().timeScale = 0.5f;
                                SpineManager.instance.DoAnimation(_endFlowers[0], "miao2", true);
                                Delay(2, () =>
                                {
                                    _talkIndex = 2;
                                    GameNext();
                                });
                            });
                        }
                    });
                }
                else
                {
                    PlayFialVoice();
                }
            }
            else
            {
                PlayFialVoice();
            }
        }
        /// <summary>
        /// 播放失败音效
        /// </summary>
        private void PlayFialVoice()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            PlayBgm(2,false);
            soundTime = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
            Delay(soundTime, () => 
            {
                levelMask.Hide();
            });
        }
        private void NextGame()
        {
            LevelTwoStart();            
        }

        /// <summary>
        /// 第二关花朵点击事件相关变量
        /// </summary>
        private string levelFlowerAniNameFirst;
        private string levelFlowerAniNameSecond;
        private int levelClickNum;        
        private GameObject clickGo;
        private string goName;

        private float bgmTime;
        /// <summary>
        /// 第二关花朵点击事件
        /// </summary>
        /// <param name="go"></param>
        private void LevelTwoClick(GameObject go)
        {
            levelMask.Show();           
            if (clickEnum == ClickEnum.First)
            {                                        
                clickEnum = ClickEnum.Second;                
            }
            if (levelClickNum == 0)
            {
                PlayBgm(3, false);
                goName = go.name;
                clickGo = go;
                levelFlowerAniNameFirst = SpineManager.instance.GetCurrentAnimationName(go.transform.GetChild(0).gameObject);
                SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameFirst + 2, false, () =>
                {
                    levelClickNum += 1;
                    levelMask.Hide();
                });
            }
            else if (levelClickNum == 1)
            {               
                if (goName == go.name)//选择错误
                {
                    PlayBgm(3, false);
                    SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameFirst + 3, false, () => 
                    {
                        SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameFirst, false, () => 
                        {
                            levelMask.Hide();
                            goName = string.Empty;
                            levelFlowerAniNameFirst = string.Empty;
                            levelClickNum = 0;
                        });                        
                    });                   
                }
                else
                {
                    levelFlowerAniNameSecond = SpineManager.instance.GetCurrentAnimationName(go.transform.GetChild(0).gameObject);
                    SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameSecond + 2, false, () =>
                    {
                        if (levelFlowerAniNameFirst == levelFlowerAniNameSecond)
                        {
                            //PlayBgm(3, false);

                            bgmTime = PlayBgm(7, false);
                            Delay(bgmTime, () => { levelMask.Hide(); });
                            SpineManager.instance.DoAnimation(clickGo.transform.GetChild(1).gameObject, "guangxiao", false);                           
                            SpineManager.instance.DoAnimation(go.transform.GetChild(1).gameObject, "guangxiao", false);
                            SpineManager.instance.DoAnimation(clickGo.transform.GetChild(0).gameObject, levelFlowerAniNameFirst + 4, false);
                            SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameFirst + 4, false, () =>
                            {                                
                                levelClickNum = 0;                               
                                clickFlowerSum++;
                                if (clickFlowerSum>=30)
                                {
                                    isSucceed = true;
                                    SpineManager.instance.SetFreeze(_xemPanels[1], true);
                                    if (timeCoroutine!=null)
                                    _mono.StopCoroutine(timeCoroutine);        
                                }
                                SpineManager.instance.DoAnimation(clickGo.transform.GetChild(0).gameObject, "kong", false);
                                SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, "kong", false);
                                clickGo.Hide();
                                go.Hide();
                                levelFlowerAniNameFirst = string.Empty;
                                levelFlowerAniNameSecond = string.Empty;
                            });

                        }
                        else
                        {
                            PlayBgm(2, false);
                            SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameSecond + 3, false, () =>
                            {
                                SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, levelFlowerAniNameSecond, false,()=> 
                                {
                                    levelClickNum = 1;
                                    levelMask.Hide();
                                });                                                               
                            });
                        }
                    });
                }
            }                                  
        }
        /// <summary>
        /// 判断是否成功，成功，出现分数面板
        /// </summary>
        private void Succeed()
        {
            if (isSucceed == true)
            {               
                levelMask.Show();               
                isSucceed = false;

                _startGames[2].Show();
                _startGames[2].transform.GetChild(1).GetComponent<Text>().text = string.Empty;
                Delay(0.3f, () => 
                {
                    _startGames[2].transform.GetChild(0).gameObject.Show();
                    _startGames[2].transform.GetChild(1).GetComponent<Text>().text = clickFlowerSum.ToString();
                });
                endMask.SetActive(true);
                _startGames[2].GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_startGames[2], "jsban", false, () =>
                {
                    PlayBgm(6,false);
                    _startGames[2].transform.GetChild(2).GetComponent<SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_startGames[2].transform.GetChild(2).gameObject, "guangxiao", false);
                    Delay(2.5f, () =>
                    {
                        _startGames[1].Hide();
                        _xemPanels[4].Show();
                        PlayBgm(5,false);
                        Tweener twe = _xemPanels[2].transform.DOLocalMoveY(_xemPanels[4].transform.localPosition.y, 1.0f);
                        twe.SetEase(Ease.InQuart);
                        Delay(1.0f, () =>
                        {
                            Tweener te = _xemPanels[2].transform.DOLocalMove(_xemPanels[5].transform.localPosition, 1.0f);
                            te.SetEase(Ease.InQuart);                            
                            float time = SpineManager.instance.DoAnimation(_xemPanels[0], "xem-y1", false, () =>
                            {
                                GameSuccess();
                                ResetDataLevelOne();
                                ResetDataLevelTwo();
                            });
                            Delay(time, () => 
                            {
                                SpineManager.instance.DoAnimation(_xemPanels[0], "xem-y2", true);
                            });
                        });
                    });
                });                               
            }
        }
        /// <summary>
        /// 时间没到，重新生成花朵
        /// </summary>
        private void ResetLevelTwo()
        {
            if(timeIndex!=0 && clickFlowerSum == 15 && isAgain)
            {
                isAgain = false;
                ResetDataLevelTwo();              
                E4rStart();
                E4rStart();
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

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
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
