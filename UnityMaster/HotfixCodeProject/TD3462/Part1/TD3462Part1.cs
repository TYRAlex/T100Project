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


    public class CaiAni
    {
        public string SpineName;
        public bool IsLoop;
        public float WaitTime;
        public GameObject Cai;
        public GameObject Tu;
        public int Voice;
       


        public CaiAni( string spineName,bool isLoop,float waitTime,GameObject cai,GameObject tu,int voice=-1)
        {
            SpineName = spineName;
            IsLoop = isLoop;
            WaitTime = waitTime;
            Cai = cai;
            Tu = tu;
            Voice = voice;
        }
    }

    public class TD3462Part1
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
        private GameObject _dDD;
        private GameObject _sDD;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

     
        private Transform _game1Tra;
        private Transform _game2Tra;
        private Transform _chanziTra;
        private Transform _chanziPosTra;
        private Transform _zhongziTra;
        private Transform _zhongziPosTra;
        private Transform _yunTra;
        private Transform _yunPosTra;
        private Transform _drogsTra;  
        private Transform _guangsTra;
        private Transform _game2DragsTra;
        private Transform _game2DragsPosTra;
        private Transform _game2DropTra;
        private mILDrager _curDrag;
        private Vector2 _curDragInitPos;

        private RectTransform _banziRect;
        private RectTransform _banzi2Rect;
        private RectTransform _baheAniRect;

        private GameObject _line;
        private GameObject _shengzi;
        private GameObject _xem2;
        private GameObject _xem;
        private GameObject _xemshou;
        private GameObject _cai1;
        private GameObject _cai1shou;
        private GameObject _cai2;
        private GameObject _cai2shou;
        private GameObject _cai3;
        private GameObject _cai3shou;
        private GameObject _game2Mask;
        private Image _progressImg;


        private float _time;
        private bool _yunIsMatch;
        private bool _isOverGame1;
        private mILDroper _yunmILDroper;

        private Queue<CaiAni> _caiAnis1;
        private Queue<CaiAni> _caiAnis2;
        private Queue<CaiAni> _caiAnis3;

        private int _game2DragSuccessNum;     
     


        private GameObject _vs;
        private RectTransform _failAni;

        private GameObject _yz;
        private GameObject _yz0;
        private GameObject _yz1;
        private GameObject _yz2;

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

            _dDD = curTrans.GetGameObject("dDD");
            _sDD = curTrans.GetGameObject("sDD");


            _game1Tra = curTrans.Find("game1");
            _game2Tra = curTrans.Find("game2");

            _chanziTra = curTrans.Find("game1/drags/chanzi");
            _chanziPosTra = curTrans.Find("game1/drags/chanziPos");

            _yunTra = curTrans.Find("game1/drags/yun");
            _yunPosTra = curTrans.Find("game1/drags/yunPos");

            _zhongziTra = curTrans.Find("game1/drags/zhongzi");
            _zhongziPosTra = curTrans.Find("game1/drags/zhongziPos");

            _drogsTra = curTrans.Find("game1/drogs");
         

            _guangsTra = curTrans.Find("game1/guangs");

            _banziRect = curTrans.GetRectTransform("game1/drags/pz6");
            _banzi2Rect = curTrans.GetRectTransform("game2/yx4");
            _baheAniRect = curTrans.GetRectTransform("game2/baheAni");

            _shengzi = curTrans.GetGameObject("game2/baheAni/shengzi");
            _xem2 = curTrans.GetGameObject("game2/baheAni/xem/xem-y2");
            _xem = curTrans.GetGameObject("game2/baheAni/xem/bh-xem");
            _xemshou = curTrans.GetGameObject("game2/baheAni/xemshou/bh-xem-s");

            _cai1 = curTrans.GetGameObject("game2/baheAni/cai/bh-c0");
            _cai1shou = curTrans.GetGameObject("game2/baheAni/caishou/bh-c-s0");
            _cai2 = curTrans.GetGameObject("game2/baheAni/cai/bh-c1");
            _cai2shou = curTrans.GetGameObject("game2/baheAni/caishou/bh-c-s1");
            _cai3 = curTrans.GetGameObject("game2/baheAni/cai/bh-c2");
            _cai3shou = curTrans.GetGameObject("game2/baheAni/caishou/bh-c-s2");
            _game2Mask = curTrans.GetGameObject("game2/game2Mask");

            _progressImg = curTrans.GetImage("game2/yx4/yxa4/yxa3");
            _game2DragsTra = curTrans.Find("game2/drags");
            _game2DragsPosTra = curTrans.Find("game2/dragsPos");

            _vs = curTrans.GetGameObject("game2/vs");
            _failAni = curTrans.GetRectTransform("game2/failAni");
            _line = curTrans.GetGameObject("game2/lines");

            _game2DropTra = curTrans.Find("game2/yx4/Drops");

            _yz = curTrans.GetGameObject("game2/baheAni/xem/yz");
            _yz0 = curTrans.GetGameObject("game2/baheAni/cai/yz0");
            _yz1 = curTrans.GetGameObject("game2/baheAni/cai/yz1");
            _yz2 = curTrans.GetGameObject("game2/baheAni/cai/yz2");
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _time = 0;

            
            _isOverGame1 = false;
            _yunIsMatch = false;
            _yunmILDroper = null;
            _talkIndex = 1;
            _progressImg.fillAmount = 0;
            _game2DragSuccessNum = 0;
       
          
            HideVoiceBtn();
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

            var cai1Go = _drogsTra.Find("1/cai-").gameObject;
            var tu1Go = _drogsTra.Find("1/t1").gameObject;

            var cai2Go = _drogsTra.Find("2/cai-").gameObject;
            var tu2Go = _drogsTra.Find("2/t1").gameObject;

            var cai3Go = _drogsTra.Find("3/cai-").gameObject;
            var tu3Go = _drogsTra.Find("3/t1").gameObject;

       
            _caiAnis1 = new Queue<CaiAni>();
            _caiAnis1.Enqueue(new CaiAni("cai-a", false,2f, cai1Go, tu1Go));
            _caiAnis1.Enqueue(new CaiAni("cai-a2",true, 0f, cai1Go, tu1Go,3));
            _caiAnis1.Enqueue(new CaiAni("cai-b", false,2f, cai1Go, tu1Go));
            _caiAnis1.Enqueue(new CaiAni("cai-b2",true, 0f, cai1Go, tu1Go,4));
            _caiAnis1.Enqueue(new CaiAni("cai-c", false, 2f, cai1Go, tu1Go));
            _caiAnis1.Enqueue(new CaiAni("cai-c2", true, 0f, cai1Go, tu1Go,5));       
            _caiAnis1.Enqueue(new CaiAni("d-cai", false, 2f, cai1Go, tu1Go));
            _caiAnis1.Enqueue(new CaiAni("d-cai2", true, 0f, cai1Go, tu1Go,6));
            

            _caiAnis2 = new Queue<CaiAni>();
            _caiAnis2.Enqueue(new CaiAni("cai-a", false, 2f, cai2Go, tu2Go));
            _caiAnis2.Enqueue(new CaiAni("cai-a2", true, 0f, cai2Go, tu2Go,3));
            _caiAnis2.Enqueue(new CaiAni("cai-b", false, 2f, cai2Go, tu2Go));
            _caiAnis2.Enqueue(new CaiAni("cai-b2", true, 0f, cai2Go, tu2Go,4));
            _caiAnis2.Enqueue(new CaiAni("cai-c", false, 2f, cai2Go, tu2Go));
            _caiAnis2.Enqueue(new CaiAni("cai-c2", true, 0f, cai2Go, tu2Go,5));
            _caiAnis2.Enqueue(new CaiAni("d-cai3", false, 2f, cai2Go, tu2Go));
            _caiAnis2.Enqueue(new CaiAni("d-cai4", true, 0f, cai2Go, tu2Go,6));


            _caiAnis3 = new Queue<CaiAni>();
            _caiAnis3.Enqueue(new CaiAni("cai-a", false, 2f, cai3Go, tu3Go));
            _caiAnis3.Enqueue(new CaiAni("cai-a2", true, 0f, cai3Go, tu3Go,3));
            _caiAnis3.Enqueue(new CaiAni("cai-b", false, 2f, cai3Go, tu3Go));
            _caiAnis3.Enqueue(new CaiAni("cai-b2", true, 0f, cai3Go, tu3Go,4));
            _caiAnis3.Enqueue(new CaiAni("cai-c", false, 2f, cai3Go, tu3Go));
            _caiAnis3.Enqueue(new CaiAni("cai-c2", true, 0f, cai3Go, tu3Go,5));
            _caiAnis3.Enqueue(new CaiAni("d-cai5", false, 2f, cai3Go, tu3Go));
            _caiAnis3.Enqueue(new CaiAni("d-cai6", true, 0f, cai3Go, tu3Go,6));
        }

        void GameInit()
        {
            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            InitData();
          
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _dDD.Hide();_sDD.Hide(); _game2Mask.Hide(); _shengzi.Hide();_line.Hide();
            _yz.Hide(); _yz0.Hide(); _yz1.Hide(); _yz2.Hide();

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);


            _banziRect.anchoredPosition = new Vector2(539, -217);
            _banzi2Rect.anchoredPosition = new Vector2(0, -150);
            _baheAniRect.anchoredPosition = new Vector2(0, 0);
            _failAni.anchoredPosition = new Vector2(0, 1080);
            _yz.transform.GetRectTransform().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _game1Tra.gameObject.Show();
            _game2Tra.gameObject.Show();

            //初始化Game1Spines
            InitGameSpines(_game1Tra, spine => {
             
                var name = spine.gameObject.name;
                switch (name)
                {
                    case "t1":
                    case "cai-":
                        spine.gameObject.Hide();
                        spine.color = new Color(1, 1, 1, 1);
                        break;                 
                }
            });

            //初始化Game1Pos相关         
            SetDragPos(_chanziPosTra, _chanziTra);
            SetDragPos(_zhongziPosTra, _zhongziTra);
            SetDragPos(_yunPosTra, _yunTra);

            //初始化Game1拖拽相关   
            InitGameDrags(_game1Tra, drag => {

                drag.isActived = false;

                var name = drag.transform.parent.name;
                drag.gameObject.Show();
                switch (name)
                {
                    case "chanzi":
                        
                        drag.SetDragCallback(ChanZiDragStart, null,ChanZiDragEnd);
                        break;
                    case "zhongzi":
                        drag.SetDragCallback(ZhongZiDragStart, null, ZhongZiDragEnd);
                        break;
                    case "yun":
                        drag.SetDragCallback(YunDragStart, YunDraging, YunDragEnd);                  
                        break;
                }              
            });

            InitGameSpines(_game2Tra, null);
            _game2Tra.gameObject.Hide();
            //初始化Game2Pos
            SetDragPos(_game2DragsPosTra, _game2DragsTra);

            //初始化Game2拖拽相关
            InitGameDrags(_game2Tra, drag => {
                drag.isActived = false;
                drag.gameObject.Show();
                drag.SetDragCallback(Game2DragStart,null,Game2DragEnd);
            });
           

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
                        _startSpine.Hide();

                        _sDD.Show(); 
                        BellSpeck(_sDD, 0, null, ShowVoiceBtn);
                       
                       

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
                    BellSpeck(_sDD, 1, null, () => { _sDD.Hide();  StartGame(); });                   
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

            var chanziDrags = _chanziTra.GetComponentsInChildren<mILDrager>(true);
            for (int i = 0; i < chanziDrags.Length; i++)
                chanziDrags[i].isActived = true;
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
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dDD.Show(); BellSpeck(_dDD, 3);                      						
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

        #region 云拖拽

        private void YunDragStart(Vector3 pos, int dragType, int index)
        {
           bool isDoMove =  _banziRect.anchoredPosition.y==-217f;

            if (isDoMove)
            {
                _banziRect.DOAnchorPosY(217, 0.5f).SetEase(Ease.Linear);
                GetCurDrag(_yunTra, index);                            
                var yun= _curDrag.transform.Find("yun").gameObject;
                PlaySpine(yun, "yun2", null, true);
                PlayVoice(2, true);
              
            }
            
        }
     
        private void YunDraging(Vector3 pos, int dragType, int index)
        {       
           _yunIsMatch= IsMatch<BoxCollider2D>(ref _yunmILDroper);                                           
        }

        private void YunDragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            _yunIsMatch = IsMatch<BoxCollider2D>(ref _yunmILDroper);                           
        }

        void Update()
        {
            if (_yunIsMatch)
            {
                var dropType = _yunmILDroper.dropType;                
                switch (dropType)
                {
                    case 1:
                        if (_caiAnis1.Count!=0)                       
                            QueueDequeue(_caiAnis1);                                                                                                                       
                        break;
                    case 2:
                        if (_caiAnis2.Count != 0)                       
                            QueueDequeue(_caiAnis2);                          
                        break;
                    case 3:
                        if (_caiAnis3.Count != 0)                        
                            QueueDequeue(_caiAnis3);                                            
                        break;
                }
            }
            else
            {
                _time = 0;
               
            }

          

            if (!_isOverGame1)
            {
                bool isFinish = _caiAnis1.Count == 0 && _caiAnis2.Count == 0 && _caiAnis3.Count == 0;
                if (isFinish)
                {
                    _isOverGame1 = true;
                    _curDrag.isActived = false;
                    var yunGo = _yunTra.Find("1/yun").gameObject;
                    PlaySpine(yunGo, yunGo.name, null, true);
                    StopAudio(SoundManager.SoundType.VOICE);
                    _yunTra.Find("1").GetRectTransform().DOAnchorPosX(-1600f, 2);
                    Delay(2, ()=> {
                        _sDD.Show();_mask.Show();
                        BellSpeck(_sDD, 2, 
                        ()=> {
                            _game1Tra.gameObject.Hide();
                            _game2Tra.gameObject.Show();
                            _vs.Show();

                        },
                        () => { 
                            _sDD.Hide(); _mask.Hide(); _vs.Hide(); _shengzi.Show();_line.Show();
                            _yz.Show();_yz0.Show();_yz1.Show();_yz2.Show();
                            StartGame2();
                        });
                       
                    });
                }
            }
           
        }


        private void QueueDequeue(Queue<CaiAni> caiAnis)
        {
            var curCaiAniData = caiAnis.Peek();
            _time += Time.deltaTime;
            if (_time >= curCaiAniData.WaitTime)
            {
                _time = 0;
                caiAnis.Dequeue();

                if (curCaiAniData.Voice!=-1)                
                    PlayVoice(curCaiAniData.Voice);
                
                if (caiAnis.Count==0)               
                    curCaiAniData.Tu.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 0.5f);
                
                PlaySpine(curCaiAniData.Cai, curCaiAniData.SpineName, null, curCaiAniData.IsLoop);
            }
        }

        #endregion


        #region 拔河
        private void StartGame2()
        {
            Debug.LogError("开始拔河");
         

            //初始化Game2Spines
            InitGameSpines(_game2Tra, spine => {
                var name = spine.gameObject.name;
                var go = spine.gameObject;
               
                switch (name)
                {
                    case "yxb1":
                    case "yxb2":
                    case "yxb3":
                    case "yxb4":
                    case "yxb5":
                        go.Show();
                        PlaySpine(go, name);                      
                        break;
                    case "bh-xem":
                    case "bh-xem-s":                      
                        PlaySpine(go, name, null, true);
                        break;
                    case "bh-c0":
                    case "bh-c2":
                        PlaySpine(go, "bh-c", null, true);
                        break;
                    case "bh-c1":
                        PlaySpine(go, "bh-cc", null, true);
                        break;                                
                    case "bh-c-s0":
                    case "bh-c-s2":
                        PlaySpine(go, "bh-c-s", null, true);
                        break;
                    case "bh-c-s1":
                        PlaySpine(go, "bh-cc-s", null, true);
                        break;                                       
                }
            });


            _banzi2Rect.DOAnchorPosY(92.4f, 0.5f);
            //激活Game2拖拽
            InitGameDrags(_game2Tra, drag => { drag.isActived = true; });
        }

        private void Game2DragStart(Vector3 pos, int dragType, int index)
        {
            PlayVoice(8);
            GetCurDrag(_game2DragsTra, index);
            _curDragInitPos = _curDrag.transform.localPosition;
            _curDrag.transform.SetAsLastSibling();
            var go = _curDrag.transform.GetChild(0).gameObject;
            var name = go.name;
            switch (name)
            {
                case "yxb1":
                    PlaySpine(go, "yxb6", null, false);                   
                    break;
                case "yxb2":
                    PlaySpine(go, "yxb7", null, false);                
                    break;
                case "yxb3":
                     PlaySpine(go, "yxb8", null, false);
                   
                    break;
                case "yxb4":
                    PlaySpine(go, "yxb9", null, false); 
                  
                    break;
                case "yxb5":
                     PlaySpine(go, "yxb10", null, false);                 
                    break;
            }
        }

        private void Game2DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            _game2Mask.Show();
            mILDroper curDrop = null;
            isMatch = IsMatch<PolygonCollider2D>(ref curDrop);

            if (isMatch)
            {
                PlayVoice(7);
                var go = curDrop.transform.GetChild(0).gameObject;
                MatchSuccess(go, new string[1] { go.name }, new bool[1] { false },()=> {

                    _game2DragSuccessNum++;

                  

                    switch (_game2DragSuccessNum)
                    {
                        case 1:
                            _progressImg.DOFillAmount(0.3f, 0.5f);
                            CaiBaHeSuccessAni();
                            break;
                        case 2:
                            _progressImg.DOFillAmount(0.6f, 0.5f);
                            CaiBaHeSuccessAni();
                            break;
                        case 3:
                            _progressImg.DOFillAmount(1f, 0.5f);
                            CaiBaHeSuccessAni();
                            Delay(1.3f, BaHeOverAni);
                            break;
                    }
                });
            }
            else
            {
                MatchFail();

               

               
                
               XemBaHeSuccessAni();
                
               
                  
                

              
            }
        }

       private void BaHeOverAni()
       {
            _game2Mask.Show();
            PlaySpine(_xem, "kong"); PlaySpine(_xemshou, "kong"); 
            PlaySpine(_cai1, "d-cai2",null,true); PlaySpine(_cai1shou, "kong");
            PlaySpine(_cai2, "d-cai4",null,true); PlaySpine(_cai2shou, "kong");
            PlaySpine(_cai3, "d-cai6",null,true); PlaySpine(_cai3shou, "kong");
            _shengzi.Hide();
            _yz.transform.GetRectTransform().localScale = new Vector3(1, 1, 1);
            PlaySpine(_xem2, _xem2.gameObject.name, null, true);
            Delay(2, GameSuccess);
        }

       private void CaiBaHeSuccessAni()
        {
            PlaySpine(_cai1, "bh-c2",  ()=>  { PlaySpine(_cai1, "bh-c",null,true); }); PlaySpine(_cai1shou, "bh-c2-s", () => { PlaySpine(_cai1shou, "bh-c-s", null, true); });
            PlaySpine(_cai2, "bh-cc2", () => { PlaySpine(_cai2, "bh-cc", null, true);}); PlaySpine(_cai2shou, "bh-cc2-s",() => { PlaySpine(_cai2shou, "bh-cc-s", null, true);});
            PlaySpine(_cai3, "bh-c2",  () => { PlaySpine(_cai3, "bh-c", null, true); }); PlaySpine(_cai3shou, "bh-c2-s", () => { PlaySpine(_cai3shou, "bh-c-s", null, true); });
                  
            Delay(0.33f, () =>
            {
                PlaySpine(_xem, "bh-xem3", () => { PlaySpine(_xem, "bh-xem", null, true); }); PlaySpine(_xemshou, "bh-xem3-s", () => { PlaySpine(_xemshou, "bh-xem-s", null, true); });
            });


            Delay(0.33f,()=> {
                bool isLast = _game2DragSuccessNum == 3;
                var endvalue = _baheAniRect.anchoredPosition.x + 50;
                if (isLast)
                {
                    endvalue = 150;
                }
                _baheAniRect.DOAnchorPosX(endvalue, 0.5f);
                Delay(0.5f, () => { _game2Mask.Hide(); });

            });
           
        }

        private void XemBaHeSuccessAni()
        {
            Delay(0.33f, () => {

                PlaySpine(_cai1, "bh-c3", () => { PlaySpine(_cai1, "bh-c", null, true); }); PlaySpine(_cai1shou, "bh-c3-s", () => { PlaySpine(_cai1shou, "bh-c-s", null, true); });
                PlaySpine(_cai2, "bh-cc3", () => { PlaySpine(_cai2, "bh-cc", null, true); }); PlaySpine(_cai2shou, "bh-cc3-s", () => { PlaySpine(_cai2shou, "bh-cc-s", null, true); });
                PlaySpine(_cai3, "bh-c3", () => { PlaySpine(_cai3, "bh-c", null, true); }); PlaySpine(_cai3shou, "bh-c3-s", () => { PlaySpine(_cai3shou, "bh-c-s", null, true); });
            });
          

            PlaySpine(_xem, "bh-xem2", () => { PlaySpine(_xem, "bh-xem", null, true); }); PlaySpine(_xemshou, "bh-xem2-s", () => { PlaySpine(_xemshou, "bh-xem-s", null, true); });

            Delay(0.33f, () => {
                var endvalue = _baheAniRect.anchoredPosition.x - 50;
                _baheAniRect.DOAnchorPosX(endvalue, 0.5f);
                Delay(0.5f, () => {

                   bool isReset  = _baheAniRect.anchoredPosition.x <= -150;
                    if (isReset)                    
                        BaHeResetAni();
                    else                    
                    _game2Mask.Hide(); 

                });
            });
           
        }

        private void BaHeResetAni()
        {
            PlaySpine(_cai1, "bh-c2", () => { PlaySpine(_cai1, "bh-c", null, true); }); PlaySpine(_cai1shou, "bh-c2-s", () => { PlaySpine(_cai1shou, "bh-c-s", null, true); });
            PlaySpine(_cai2, "bh-cc2", () => { PlaySpine(_cai2, "bh-cc", null, true); }); PlaySpine(_cai2shou, "bh-cc2-s", () => { PlaySpine(_cai2shou, "bh-cc-s", null, true); });
            PlaySpine(_cai3, "bh-c2", () => { PlaySpine(_cai3, "bh-c", null, true); }); PlaySpine(_cai3shou, "bh-c2-s", () => { PlaySpine(_cai3shou, "bh-c-s", null, true); });

            Delay(0.33f, () => {
                PlaySpine(_xem, "bh-xem3", () => { PlaySpine(_xem, "bh-xem", null, true); }); PlaySpine(_xemshou, "bh-xem3-s", () => { PlaySpine(_xemshou, "bh-xem-s", null, true); });
            });
          
       
            _game2DragSuccessNum = 0;

            Delay(0.33f,()=> { _baheAniRect.DOAnchorPosX(0, 1); });         
            SetDragPos(_game2DragsPosTra, _game2DragsTra);
            _progressImg.fillAmount = 0;

            InitGameSpines(_game2DropTra, null);

            
            InitGameDrags(_game2Tra, drag => { drag.isActived = true; drag.gameObject.Show(); });
      
            _failAni.DOAnchorPosY(0, 1.5f).SetEase(Ease.OutBounce);  //下滑
            Delay(2, () => {
                _failAni.DOAnchorPosY(1080, 0.5f).SetEase(Ease.InSine).OnComplete(()=> {

                    _game2Mask.Hide();
                }); ;//上划
            }); 
            

          
        }

        #endregion

        #region 铲子拖拽

        private void ChanZiDragStart(Vector3 pos, int dragType, int index)
        {         
            DragStart(_chanziTra, index, "t1");
        }

        private void ChanZiDragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            StopHintGuang();
                   
            mILDroper curDrop = null;
            isMatch = IsMatch<BoxCollider2D>(ref curDrop);
        
            if (isMatch)
            {
                var t1 = curDrop.transform.Find("t1").gameObject;
                var isActive = t1.activeSelf;
                var t1ParentName = t1.transform.parent.name;
                if (isActive)
                    MatchFail();
                else
                { 
                    PlayVoice(0);
                    string[] spineNames = null;
                    switch (t1ParentName)
                    {
                        case "1":
                            spineNames = new string[3] { "t1", "t2", "t3" };
                            break;
                        case "2":
                            spineNames = new string[3] { "t1", "t4", "t5" };
                            break;
                        case "3":
                            spineNames = new string[3] { "t1", "t6", "t7" };
                            break;
                    }

                    MatchSuccess(t1, spineNames, new bool[3] { false, false, false }, () => { ActiveNextDrag("t1", _zhongziTra); });
                }
            }
            else
            {
                MatchFail();
            }

        }

        #endregion

        #region 种子拖拽
        private void ZhongZiDragStart(Vector3 pos, int dragType, int index)
        {         
            DragStart(_zhongziTra, index, "cai-");
        }

        private void ZhongZiDragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            StopHintGuang();
                    
            mILDroper curDrop = null;
            isMatch = IsMatch<BoxCollider2D>(ref curDrop);
         
            if (isMatch)
            {
                var cai = curDrop.transform.Find("cai-").gameObject;
                var isActive = cai.activeSelf;
                if (isActive)
                    MatchFail();
                else
                { PlayVoice(1); MatchSuccess(cai, new string[2] { "cai-", "cai-2" }, new bool[2] { false, true }, () => { ActiveNextDrag("cai-", _yunTra); }); }
            }
            else
            {
                MatchFail();
            }

        }

        #endregion

        private void DragStart(Transform tra, int index, string name = "", Action callBack = null)
        {
            GetCurDrag(tra, index);
            _curDragInitPos = _curDrag.transform.localPosition;

            if (name!="")            
                HintGuang(name);

            callBack?.Invoke();

        }

        private void MatchSuccess(GameObject go,string [] spines,bool[] isLoops,Action callBack=null)
        {            
            _curDrag.gameObject.Hide();
            go.Show();
            ForSpine(go, spines, isLoops);
            callBack.Invoke();
        }

        private void MatchFail()
        {
            PlayVoice(9);
            _curDrag.transform.localPosition = _curDragInitPos;
        }

        private bool IsMatch<T>(ref mILDroper curDrop) where T : Collider2D
        {
            bool isMatch = false;
            var drops = _curDrag.drops;
            for (int i = 0; i < drops.Length; i++)
            {
                var collider = drops[i].transform.GetComponent<T>();
                bool isOverlap = collider.OverlapPoint(Input.mousePosition);
                if (isOverlap)
                {
                    curDrop = drops[i];
                    isMatch = true;
                    break;
                }
            }

            return isMatch;
        }        

        private  void SetDragPos(Transform posTra,Transform tra)
        {
            for (int i = 0; i < posTra.childCount; i++)
            {
                var child = posTra.GetChild(i);
                var name = child.name;

                var drag = tra.Find(name);
                drag.localPosition = child.localPosition;
                drag.SetSiblingIndex(i);
            }
        }

        private void GetCurDrag(Transform parent,int index)
        {        
            var mILDragers = parent.GetComponentsInChildren<mILDrager>(true);

            for (int i = 0; i < mILDragers.Length; i++)
            {
                if (mILDragers[i].index==index)
                {
                    _curDrag = mILDragers[i];
                    break;
                }
            }              
        }

        private void HintGuang(string name)
        {
            for (int i = 0; i < _drogsTra.childCount; i++)
            {
                var child = _drogsTra.GetChild(i);
                bool isActive = child.Find(name).gameObject.activeSelf;
                if (!isActive)
                {
                  var guangGo=  _guangsTra.Find("k" + child.name);
                    PlaySpine(guangGo.gameObject, "k", null, true);
                }
            }
        }

        private void StopHintGuang()
        {
            InitGameSpines(_guangsTra,null);
        }

        private void ActiveNextDrag(string name, Transform parent)
        {
            bool isActive = true;

            for (int i = 0; i < _drogsTra.childCount; i++)
            {
                var child = _drogsTra.GetChild(i);
                var go = child.Find(name).gameObject;

                if (!go.activeSelf)
                {
                    isActive = false;
                    break;
                }
            }

            if (isActive)
            {
                InitGameDrags(parent, drag => { drag.isActived = true; });
            }
        }

        private void InitGameSpines(Transform parent, Action<SkeletonGraphic> callBack)
        {
            Gets<SkeletonGraphic>(parent, spine => {
                spine.gameObject.Show();
                spine.Initialize(true);
                callBack?.Invoke(spine);
            });
        }

        private void InitGameDrags(Transform parent, Action<mILDrager> callBack)
        {
            Gets<mILDrager>(parent, drag => { callBack?.Invoke(drag); });
        }

        private void Gets<T>(Transform parent, Action<T> callBack, bool includeInactive = true)
        {
            var components = parent.GetComponentsInChildren<T>(includeInactive);

            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                callBack?.Invoke(component);
            }
        }


        private void ForSpine(GameObject go, string[] spines, bool[] isLoops, Action callBack = null)
        {
            _mono.StartCoroutine(IEForSpine(go,spines,isLoops, callBack));
        }


        IEnumerator IEForSpine(GameObject go, string[] spines,bool[] isLoops,Action callBack=null)
        {
            for (int i = 0; i < spines.Length; i++)
            {
                var time = PlaySpine(go, spines[i], null, isLoops[i]);
                yield return new WaitForSeconds(time);
            }
            callBack?.Invoke();
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
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
