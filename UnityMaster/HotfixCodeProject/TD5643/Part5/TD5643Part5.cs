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


    public class BirdLevelData
    {
        public int LevelId;
        public int BirdImgIndex;
        public int LineImgIndex;
       
        /// <summary>
        /// 正确小恶魔Sp名
        /// </summary>
        public string CorrectXemSpName;

        public int ShowXemNums;
        private List<string> _errorXemSpNames = new List<string> { "xem2", "xem3", "xem4" };

        /// <summary>
        /// 当前关卡显示XemSp名
        /// </summary>
        public List<string> ShowXemSpNames;
                   
        public BirdLevelData(int levelId,int birdImgIndex)
        {
            LevelId = levelId;
            BirdImgIndex = birdImgIndex;
            LineImgIndex = birdImgIndex;
            CorrectXemSpName = SetCorrectXemSpName(birdImgIndex);
            ShowXemNums = SetShowXemNums(levelId);
            ShowXemSpNames = SetShowXemSpNames();

        }
        private string SetCorrectXemSpName(int birdImgIndex)
        {
            string name = string.Empty;
            switch (birdImgIndex)
            {
                case 0:
                    name = "xem7";
                    break;
                case 1:
                    name = "xem5";
                    break;
                case 2:
                    name = "xem6";
                    break;
            }
            return name;
        }
        private int SetShowXemNums(int levelId)
        {
            int num = 0;
            if (levelId == 1)
                num = 2;
            else if (levelId == 2)
                num = 3;
            else
                num = 4;            
            return num;
        }

        private List<string> SetShowXemSpNames()
        {
            List<string> list = new List<string>();
            list.Add(CorrectXemSpName);
        
            switch (ShowXemNums)
            {
                case 2:              
                    list.Add(_errorXemSpNames[Random.Range(0, _errorXemSpNames.Count)]);
                    break;
                case 3:
                    while (true)
                    {                   
                        var  name = _errorXemSpNames[Random.Range(0, _errorXemSpNames.Count)];
                        var isContain = list.Contains(name);
                        if (!isContain)
                            list.Add(name);
                        if (list.Count == 3)
                            break;
                    }
                    break;
                case 4:
                    foreach (var spName in _errorXemSpNames)                    
                        list.Add(spName);                    
                    break;
            }


            //打乱顺序
            //int index = 0;
            //string temp = string.Empty;
            //System.Random random = new System.Random();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    index = random.Next(0, list.Count - 1);
            //    if (index != i)
            //    {
            //        temp = list[i];
            //        list[i] = list[index];
            //        list[index] = temp;
            //    }
            //}

            return list;
        }
      





    }


    public class TD5643Part5
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

        private bool _isPlaying;
        private bool _isDownBrid;  //是否按下鸟

        private EdgeCollider2D _floorEC;  //地面边界

        private Transform _birdTra;
        private RectTransform _birdRect;
        private Rigidbody2D _birdRB2D;
        private BellSprites _birdBellSp;
        private Image _birdImg;

        private LongPressButton _birdLPBtn;
        private EventDispatcher _birdED;

        private Transform _originTra;
        private Vector3 _originPos;

        private Transform _tracingPointsTra;

        private float _radius;      //半径
        private Vector3 _tempPos;
        private double _angleOff;   //夹角
        private Vector3 _lastPos;   //上一帧鸟的位置
        private float _throwSpeed;  //飞的速度

        
        private bool _isFlying;     //是否飞
        private bool _isCrash;      //是否击中
        private Transform _xemContentTra;
        private Transform _stoneContentTra;



        private Dictionary<string, Vector2> _stonesPos;
        private Dictionary<string, Vector2> _xemsPos;


        private bool _isPlayingBirdDragVoice;


        private List<BirdLevelData> _birdLevelDatas;
        private BirdLevelData _curBirdLevelData;


        private int _curLevelId;
    
        private bool _isHitCorrectXem;
   
        private Transform _scoreContentTra;

        private GameObject _paiSp;

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
            _floorEC = curTrans.Find("floor").GetComponent<EdgeCollider2D>();

            _birdTra = curTrans.Find("GameContent/BridContent/brid");
            _birdRect = _birdTra.GetRectTransform();
            _birdRB2D = _birdTra.GetComponent<Rigidbody2D>();
            _birdLPBtn = _birdTra.GetComponent<LongPressButton>();
            _birdED = _birdTra.GetComponent<EventDispatcher>();
            _birdBellSp = _birdTra.GetComponent<BellSprites>();
            _birdImg = _birdTra.GetImage();

            _originTra = curTrans.Find("GameContent/Points/origin");
            _tracingPointsTra = curTrans.Find("GameContent/Points/PointList");

            _xemContentTra = curTrans.Find("GameContent/XemContent");
            _stoneContentTra = curTrans.Find("GameContent/StoneContent");
            _scoreContentTra = curTrans.Find("GameContent/ScoreContent");

            _paiSp = curTrans.Find("pai").gameObject;

            SetFloorEc(curTrans);
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _throwSpeed = 12f;
            _radius = 1f;
            _isPlaying = true;
            _isDownBrid = false;
            _isFlying = false;
            _isHitCorrectXem = false;
            _isCrash = false;
            _isPlayingBirdDragVoice = false;
              _curLevelId = 1;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

            InitLevelData();

            _stonesPos = new Dictionary<string, Vector2> {
                {"st7",new Vector2(565f,-203f) },           
                {"st9",new Vector2(393f,-286f) },
                {"st10",new Vector2(569f,-286f)},
                {"st6",new Vector2(180f,-247f) },
                {"st13",new Vector2(258f,-367f) },
                {"st11",new Vector2(182f,-330f)  },
                {"st8",new Vector2(407f,-203f) },
                {"st66",new Vector2(485f,-155f) }
            };

            _xemsPos = new Dictionary<string, Vector2>
            {
                {"xem0",new Vector2(478f,-287f)},
                {"xem1",new Vector2(282,-287f)},
                {"xem2",new Vector2(390f,-122f)},
                {"xem3",new Vector2(574f,-122f)}
            };
 
            _originPos = _originTra.transform.position;
            _birdTra.position = _originPos;

            _lastPos = _birdTra.position;
            _isDownBrid = false;

            InitScoreSps();
            ResetBird();
            ShowLevel();
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
            _dTT.Hide(); _sTT.Hide();
            _paiSp.Hide();
            InitializeSpine(_paiSp);

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            _birdLPBtn.OnDown = OnDownBird;
            _birdLPBtn.OnUp = OnUpBird;

            _birdED.CollisionEnter2D += BirdCollisionEnter2D;

            ResetTrajectoryLine();
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();

            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        PlayBgm(0);
                        //PlayCommonBgm(8);//ToDo...改BmgIndex
                        _startSpine.Hide();
                      
                        BellSpeck(_sTT, 0, ()=> { _sTT.Show(); }, ShowVoiceBtn, RoleType.Child);
                      
                     

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
                    BellSpeck(_sTT, 1, null, () => { _sTT.Hide(); StartGame(); }, RoleType.Child);
                    break;
            }
            _talkIndex++;
        }

        void Update()
        {
            if (_isDownBrid)
            {
                LimitBirdDragRange();
                LimitBirdDragAngle();

                if (_angleOff > -120)
                    _birdTra.position = _lastPos;

                _lastPos = _birdTra.position;

                DrawTrajectoryLine();
            }

            if (_isFlying)
            {
                var birdRBVelocityIsZero = _birdRB2D.velocity == Vector2.zero;
                var allStoneVelocityIsZero = AllStoneVelocityIsZero();
                var allXemVelocaityIsZero= AllXemVelocaityIsZero();

                if (birdRBVelocityIsZero && allStoneVelocityIsZero && allXemVelocaityIsZero)
                {
                    _isFlying = false;


                    Delay(0.4f, () =>
                    {
                        if (_isHitCorrectXem)
                        {
                            _curLevelId++;

                           

                            if (_curLevelId > _birdLevelDatas.Count)
                            {
                                _mask.Show();
                                _paiSp.Show();
                                PlayVoice(4);
                                PlaySpine(_paiSp, _paiSp.name, ()=> {

                                    Delay(2, () => { _paiSp.Hide(); GameSuccess(); });
                                    
                                });
                               
                                return;
                            }

                            ShowLevel();
                        }
                        ResetBird();
                    });

                    

                              
                }
                
            }
        }

        #region 游戏逻辑

        private bool AllStoneVelocityIsZero()
        {
            bool isZero = true;

            for (int i = 0; i < _stoneContentTra.childCount; i++)
            {
                var stoneRB = _stoneContentTra.GetChild(i).GetComponent<Rigidbody2D>();
                if (stoneRB.velocity!=Vector2.zero)                
                    isZero = false;               
            }

            return isZero;
        }

        private bool AllXemVelocaityIsZero()
        {
            bool isZero = true;

            for (int i = 0; i < _xemContentTra.childCount; i++)
            {
                var xemRB = _xemContentTra.GetChild(i).GetComponent<Rigidbody2D>();
                if (xemRB.velocity != Vector2.zero)
                    isZero = false;
            }
            return isZero;
        }

        private void InitScoreSps()
        {
            for (int i = 0; i < _scoreContentTra.childCount; i++)
            {
                var child = _scoreContentTra.GetChild(i);
                var xem = child.Find("xem").gameObject;
                var baozha = child.Find("baozha").gameObject;
                xem.Show();
                PlaySpine(xem, xem.name,null, true);
                baozha.Hide();
            }
        }
   

        private void ShowLevel()
        {
            

            InitGamePos();

            foreach (var data in _birdLevelDatas)
            {
                if (data.LevelId==_curLevelId)
                {
                    _curBirdLevelData = data;
                    break;
                }
            }

            

         
            _isHitCorrectXem = false;
            _birdImg.sprite = _birdBellSp.sprites[_curBirdLevelData.BirdImgIndex];


            for (int i = 0; i < _xemContentTra.childCount; i++)
            {
                var child = _xemContentTra.GetChild(i);
                var spine = child.Find("spine").gameObject;
                var isBounds = i > _curBirdLevelData.ShowXemSpNames.Count - 1;
                if (isBounds)
                {
                    child.gameObject.Hide();
                }
                else
                {
                    var spineName = _curBirdLevelData.ShowXemSpNames[i];                 
                    PlaySpine(spine, spineName, null, true);
                    spine.Show();
                    child.gameObject.Show();
                }               
            }

            for (int i = 0; i < _tracingPointsTra.childCount; i++)
            {
                var child = _tracingPointsTra.GetChild(i);

                var childImg = child.GetImage();
                var bellSprites = child.GetComponent<BellSprites>();
                childImg.sprite = bellSprites.sprites[_curBirdLevelData.LineImgIndex];
            }
        }


        private void InitLevelData()
        {
            List<int> indexs = new List<int>();
            while (true)
            {
                var index = Random.Range(0, 3);
                var isContain = indexs.Contains(index);
                if (!isContain)
                    indexs.Add(index);
                if (indexs.Count == 3)
                    break;
            }

            _birdLevelDatas = new List<BirdLevelData>();

            for (int i = 0; i < indexs.Count; i++)
            {
                var levelId = i + 1;
                var birImgIndex = indexs[i];
                var birdLevelData = new BirdLevelData(levelId, birImgIndex);
                _birdLevelDatas.Add(birdLevelData);
            }

        }




        /// <summary>
        /// 初始化坐标
        /// </summary>
        private void InitGamePos()
        {
            foreach (var data in _stonesPos)
            {
                var name = data.Key;
                var pos = data.Value;              
                var stone = _stoneContentTra.Find(name);
                var stoneRect = stone.GetRectTransform();
                var img = stone.GetImage();
                stoneRect.localPosition = pos;
                stoneRect.localEulerAngles = Vector3.zero;
                img.DOFade(1, 0);
                stone.gameObject.Show();
            }

            foreach (var data in _xemsPos)
            {
                var name = data.Key;
                var pos = data.Value;
                var xem = _xemContentTra.Find(name);
                var xemRect = xem.GetRectTransform();
                xemRect.localPosition = pos;
                xemRect.localEulerAngles = Vector3.zero;
                var xemSp = xem.Find("spine").gameObject;
                xemSp.Show();
                xem.gameObject.Hide();
              
            }
        }


        /*
         标签(Tag)：xem.stone,soildstone(不会消失)
         */

        private void BirdCollisionEnter2D(Collision2D other, int time)
        {

           if (!_isCrash)
            {
                _isCrash = true;
                StopAudio(SoundManager.SoundType.VOICE);

                var parentName = other.gameObject.transform.parent.name;
                var name = other.gameObject.name;

                  if(name=="floor"|| parentName== "SolidStoneContent")
                  {
                    PlayVoice(5);
                  }

               //if (_birdRB2D.velocity.magnitude > 5.0f)
                //{
                    if (parentName == "StoneContent")
                    {
                    PlayVoice(2);
                        var go = _stoneContentTra.Find(name).gameObject;
                        var img = go.GetComponent<Image>();

                        var t1 = img.DOFade(0.5f, 0.2f);
                        var t2 = img.DOFade(1.0f, 0.2f);
                        var t3 = img.DOFade(0.5f, 0.2f);
                        var t4 = img.DOFade(1.0f, 0.2f);
                        var t5 = img.DOFade(0.0f, 0.2f);
                        DOTween.Sequence().Append(t1).Append(t2).Append(t3).Append(t4).Append(t5).OnComplete(() => { go.Hide(); });
                    }
               // }


                //if (_birdRB2D.velocity.magnitude > 3.0f)
               // {
                    if (parentName == "XemContent")
                    {
                         PlayVoice(3);

                        if (name == "xem0")
                        {
                            _isHitCorrectXem = true;


                         
                            var scoreSpGo = FindGo(_scoreContentTra, (_curLevelId).ToString());
                            var xem = scoreSpGo.transform.Find("xem").gameObject;
                            var baozha = scoreSpGo.transform.Find("baozha").gameObject;
                            baozha.Show();
                            xem.Hide();
                            PlaySpine(baozha, baozha.name, () => { baozha.Hide(); });

                            PlaySuccessSound();
                        }
                        else
                        {
                            PlayFailSound();
                        }

                        var go = _xemContentTra.Find(name).gameObject;

                        var xemSp = go.transform.Find("spine").gameObject;
                        var baozhaSp = go.transform.Find("baozha").gameObject;
                        var baozhaTime = PlaySpine(baozhaSp, baozhaSp.name);
                        xemSp.Hide();
                        Delay(baozhaTime, () => { go.Hide(); });
                    }
               // }
            }

         
        }



        /// <summary>
        /// 重置鸟的相关信息
        /// </summary>
        void ResetBird()
        {
           
            _birdRB2D.velocity = Vector2.zero;
            _birdRB2D.isKinematic = true;
            _birdRB2D.constraints = RigidbodyConstraints2D.FreezeAll;
            _birdTra.position = _originPos;
            _birdRect.localEulerAngles = Vector3.zero;
            _lastPos = _originPos;
            _birdImg.raycastTarget = true;
            _isCrash = false;

        }

        /// <summary>
        /// 抬起鸟
        /// </summary>
        private void OnUpBird()
        {
            StopAudio(SoundManager.SoundType.VOICE);
            _isPlayingBirdDragVoice = false;

            _isDownBrid = false;
            ResetTrajectoryLine();
            BirdFly();
            _birdImg.raycastTarget = false;
           
        }

        /// <summary>
        /// 按下鸟
        /// </summary>
        private void OnDownBird()
        {
            _isDownBrid = true;
            _tracingPointsTra.gameObject.Show();

            if (!_isPlayingBirdDragVoice)
            {
                _isPlayingBirdDragVoice = true;
                PlayVoice(0);
            }

        }

        private void ResetTrajectoryLine()
        {
            _tracingPointsTra.gameObject.Hide();
            for (int i = 0; i < _tracingPointsTra.childCount; i++)
            {
                var go = _tracingPointsTra.GetChild(i).gameObject;

                go.transform.position = _originPos;
            }
        }

        /// <summary>
        /// 画出轨迹线
        /// </summary>
        private void DrawTrajectoryLine()
        {
            // X轴 距离=速度*时间；
            // Y轴 距离=速度*时间+1/2*加速度*时间的平方

            var distance = Vector3.Distance(_birdTra.position, _originPos);
            Vector2 v2 = _originPos - _birdTra.position;
            Vector2 segVelocity = new Vector2(v2.x, v2.y) * _throwSpeed * distance;
            Vector2[] segments = new Vector2[_tracingPointsTra.childCount];
            segments[0] = _birdTra.position;

            for (int i = 1; i < segments.Length; i++)
            {
                float time2 = i * Time.fixedDeltaTime * 2;
                segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics2D.gravity * Mathf.Pow(time2, 2);
            }

            for (int i = 0; i < _tracingPointsTra.childCount; i++)
            {
                var go = _tracingPointsTra.GetChild(i).gameObject;
                var pos = segments[i];
                go.transform.position = pos;
            }
        }

        /// <summary>
        /// 限制鸟的拖拽范围
        /// </summary>
        private void LimitBirdDragRange()
        {
            _tempPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _tempPos.z = 0;
            _birdTra.position = _tempPos;

            if (Vector3.Distance(_birdTra.position, _originPos) > _radius)
            {
                var pos = (_birdTra.position - _originPos).normalized;
                pos *= _radius;
                _birdTra.position = pos + _originPos;
            }

        }

        /// <summary>
        /// 限制鸟的拖拽角度
        /// </summary>
        private void LimitBirdDragAngle()
        {
            var birdPos = _birdTra.position;
            var bX = birdPos.x;
            var bY = birdPos.y;

            var oPos = _originPos;
            var oX = oPos.x;
            var oY = oPos.y;

            _angleOff = Math.Atan2((bY - oY), (bX - oX)) * 180 / Math.PI;
            _angleOff = Math.Round(_angleOff, 0);
        }

        /// <summary>
        /// 鸟开始飞
        /// </summary>
        private void BirdFly()
        {
            PlayVoice(1);

            _birdRB2D.isKinematic = false;
            _birdRB2D.constraints = RigidbodyConstraints2D.None;

            var distance = Vector3.Distance(_originPos, _birdTra.position);
            Vector3 velocity = _originPos - _birdTra.position;
            _birdRB2D.velocity = new Vector2(velocity.x, velocity.y) * _throwSpeed * distance;
            _isFlying = true;
        }

        /// <summary>
        /// 设置墙壁边界
        /// </summary>
        /// <param name="tra"></param>
        void SetFloorEc(Transform tra)
        {
            var sD = tra.GetRectTransform().sizeDelta;
            var w = sD.x / 2;
            var h = sD.y / 2;
            float offset = 151;

            Vector2[] tempPoints = new Vector2[5];
            tempPoints[0] = new Vector2(w, h);
            tempPoints[1] = new Vector2(w, -h + offset);
            tempPoints[2] = new Vector2(-w, -h + offset);
            tempPoints[3] = new Vector2(-w, h);
            tempPoints[4] = new Vector2(w, h);


            _floorEC.points = tempPoints;
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
                        PlayBgm(0);
                      //  PlayCommonBgm(8); //ToDo...改BmgIndex
                        GameInit();
                       					
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                    PlayBgm(0);
                   // PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();

                        _dTT.Show();
                        BellSpeck(_dTT, 2, null, null, RoleType.Child);
                        //ToDo...
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);						

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

        private void InitializeSpine(GameObject go)
        {
            go.GetComponent<SkeletonGraphic>().Initialize(true);
        }

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
