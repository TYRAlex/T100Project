using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;
using System.Linq;

namespace ILFramework.HotClass
{
	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,		
	}
	
    public class TD8952Part5
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

        private bool _isPlaying;

        private Transform xem;
        private Transform panda1;
        private Transform panda2;
        private Transform panda3;
        private Transform panda4;
        private Transform panda5;
        private Transform panda6;
        private Transform panda7;
        private Transform panda8;
        private Transform panda9;
        private Transform panda10;
        private Transform panda11;
        private Transform panda12;
        private Transform bg3;
        private Transform _noClickMask;
        private Transform _Mask;
        private int  Level1FIndex;
        private bool isChangeVoice;

        private List<Vector2> L1PandaPosList;
        private List<Vector2> L2PandaPosList;
        private List<Vector2> L3PandaPosList;

        private int FaultIndex;

        private int currentLevelIndex;

        // private Transform Level1;

        private float XemMoveSpeed;

        private bool XemMovePause;
        private bool isLeft;
        private bool isRight;
        private bool isEnd; private bool isEnd2; private bool isEnd3; private bool isEnd4; private bool isEnd5;

        void Start(object o)
        {
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _Mask = curTrans.Find("_mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");			
									
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");

			
			_dDD = curTrans.GetGameObject("dDD");
			           			
			
			
         	_sDD = curTrans.GetGameObject("sDD");
            


            xem = curTrans.Find("xem");
            panda1 = curTrans.Find("1");
            panda2 = curTrans.Find("2");
            panda3 = curTrans.Find("3");
            panda4 = curTrans.Find("4");
            panda5 = curTrans.Find("5");
            panda6 = curTrans.Find("6");
            panda7 = curTrans.Find("7");
            panda8 = curTrans.Find("8");
            panda9 = curTrans.Find("9");
            panda10 = curTrans.Find("10");
            panda11 = curTrans.Find("11");
            panda12 = curTrans.Find("12");
         
            FaultIndex = 0;
            currentLevelIndex = 1;
            Level1FIndex = 0;
            bg3 = curTrans.Find("bg3");
            //  Level1 = curTrans.Find("L1");
            XemMoveSpeed = 5f * (Screen.width / 1920f);


            XemMovePause = true;
            isLeft = false;
            isRight = false;
            isEnd = false; isEnd2 = false; isEnd3 = false; isEnd4 = false; isEnd5 = false;
            isChangeVoice = false;




            GameInit();
            GameStart();
            AddOnClickEvent();
        }

        void InitData()
        {
            _isPlaying = true;
            isChangeVoice = false;
            XemMovePause = false;
            FaultIndex = 0;
            currentLevelIndex = 1;
            Level1FIndex = 0;
            L1PandaPosList = null; L2PandaPosList = null; L3PandaPosList = null;
            L1PandaPosList = new List<Vector2>();
            L2PandaPosList = new List<Vector2>();
            L3PandaPosList = new List<Vector2>();
            _Mask.gameObject.SetActive(false);
            xem.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject,"xem1",true);
            panda2.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
            panda5.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            panda11.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            xem.GetRectTransform().anchoredPosition = new Vector2(-770,420f);
            panda1.gameObject.SetActive(false); panda2.gameObject.SetActive(false); panda3.gameObject.SetActive(false);
            panda4.gameObject.SetActive(false); panda5.gameObject.SetActive(false); panda6.gameObject.SetActive(false); panda7.gameObject.SetActive(false);
            panda8.gameObject.SetActive(false); panda9.gameObject.SetActive(false); panda10.gameObject.SetActive(false); panda11.gameObject.SetActive(false); panda12.gameObject.SetActive(false);



            panda1.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda2.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda3.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda4.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda5.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda6.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda7.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda8.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda9.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda10.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda11.GetComponent<Empty4Raycast>().raycastTarget = false;
            panda12.GetComponent<Empty4Raycast>().raycastTarget = false;

            panda1.SetSiblingIndex(1);
            panda2.SetSiblingIndex(2);
            panda3.SetSiblingIndex(3);
            panda4.SetSiblingIndex(4);
            panda5.SetSiblingIndex(5);
            panda6.SetSiblingIndex(6);
            panda7.SetSiblingIndex(7);
            panda8.SetSiblingIndex(8);
            panda9.SetSiblingIndex(9);
            panda10.SetSiblingIndex(10);
            panda11.SetSiblingIndex(11);
            panda12.SetSiblingIndex(12);
            panda1.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda1.GetChild(0).gameObject,"xm1",true);
            panda2.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda2.GetChild(0).gameObject, "xm2", true);
            panda3.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda3.GetChild(0).gameObject, "xm3", true);
            panda4.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda4.GetChild(0).gameObject, "xm4", true);
            panda5.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda5.GetChild(0).gameObject, "xm5", true);
            panda6.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda6.GetChild(0).gameObject, "xm6", true);

            panda7.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda7.GetChild(0).gameObject, "xm7", true);
            panda8.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda8.GetChild(0).gameObject, "xm8", true);
            panda9.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda9.GetChild(0).gameObject, "xm9", true);
            panda10.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda10.GetChild(0).gameObject, "xm10", true);
            panda11.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda11.GetChild(0).gameObject, "xm11", true);
            panda12.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(panda12.GetChild(0).gameObject, "xm12", true);

            panda1.transform.GetRectTransform().anchoredPosition = new Vector2(-421,390);
            panda2.transform.GetRectTransform().anchoredPosition = new Vector2(137, 390);
            panda3.transform.GetRectTransform().anchoredPosition = new Vector2(595, 390);
            panda4.transform.GetRectTransform().anchoredPosition = new Vector2(-754, 390);
            panda5.transform.GetRectTransform().anchoredPosition = new Vector2(-229, 390);
            panda6.transform.GetRectTransform().anchoredPosition = new Vector2(243, 390);
            panda7.transform.GetRectTransform().anchoredPosition = new Vector2(820, 390);
            panda8.transform.GetRectTransform().anchoredPosition = new Vector2(-671, 390);
            panda9.transform.GetRectTransform().anchoredPosition = new Vector2(-234, 390);
            panda10.transform.GetRectTransform().anchoredPosition = new Vector2(192, 390);
            panda11.transform.GetRectTransform().anchoredPosition = new Vector2(503, 390);
            panda12.transform.GetRectTransform().anchoredPosition = new Vector2(789, 390);

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
			           
        }

        void GameInit()
        {
            InitData();
            Input.multiTouchEnabled = false;
            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); 
			StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); 
			
			          
			
		     _dDD.Hide(); 
			
			
			
			 _sDD.Hide(); 
					
			
			
			
			
			

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
						         
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
			
			
			

            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);//ToDo...改BmgIndex
                        _startSpine.Hide();

                        //ToDo...
                        _sDD.SetActive(true);
                        BellSpeck(_sDD,0,null,ShowVoiceBtn,RoleType.Adult);
                      
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
            
                    BellSpeck(_sDD, 1, null, ShowVoiceBtn, RoleType.Adult);
                    break;
                case 2:
                    BellSpeck(_sDD, 2, null, ()=> 
                    {
                        StartGame();
                    }, RoleType.Adult);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑
        Coroutine IEXemMoveEvent(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
            return _mono.StartCoroutine(IEXemMove(rect,startXPos,endXPos,speed,moveCallBack));
        }
        IEnumerator IEXemMove(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack) 
        {
            
            while (true) 
            {
                if (XemMovePause == false)
                {
                    yield return new WaitForSeconds(0.01f);
                    moveCallBack?.Invoke(rect, startXPos, endXPos, speed) ;
               
                }
                else 
                {
                    yield return null;
                }
            }
        }
        private void XemMoveCallBack(RectTransform rect, float startXPos, float endXPos, float speed) 
        {
          
          
            if (rect.anchoredPosition.x >= endXPos) 
            {
                //向左
                isRight = false;
                isLeft = true;

            }
            if (rect.anchoredPosition.x <= startXPos) 
            {
                //向右
                isRight = true;
                isLeft = false;
            }
            if (isLeft)
                rect.Translate(Vector2.left*XemMoveSpeed);
            if(isRight)
                rect.Translate(Vector2.right * XemMoveSpeed);
        }

        private void PandaMove(RectTransform panda,float startPosX,float endPosX,float duration,float WaitSecond) 
        {
           
               panda.DOAnchorPosX(endPosX, duration).SetEase(Ease.Linear).OnComplete(() =>
                 {
                     //停留n秒
                 
                   
                  
                     Delay(WaitSecond, () =>
                     {
                         if (isEnd == false)
                         {
                             panda.DOAnchorPosX(startPosX, duration).SetEase(Ease.Linear).OnComplete(() =>
                             {
                              

                             });

                         }

                     });
                 });
           
        }
       
        IEnumerator IEPandaMove(RectTransform panda, float startPosX, float endPosX, float duration, float WaitSecond,float n) 
        {
          
            
                    PandaMove(panda, startPosX, endPosX, duration, WaitSecond);

                    yield return new WaitForSeconds(2 * duration + WaitSecond + n);
              
             
              
           
           
        }
        private  void addList() 
        {
            L1PandaPosList.Add( new Vector2(-421,472));
            L1PandaPosList.Add( new Vector2(137, 472));
            L1PandaPosList.Add( new Vector2(595, 472));
        }
        private void addList2()
        {
            L2PandaPosList.Add(new Vector2(-754, 472));
            L2PandaPosList.Add(new Vector2(-229, 472));
            L2PandaPosList.Add(new Vector2(243, 472));
            L2PandaPosList.Add(new Vector2(820, 472));
        }
        private void addList3()
        {
            L3PandaPosList.Add(new Vector2(-671, 472));
            L3PandaPosList.Add(new Vector2(-234, 472));
            L3PandaPosList.Add(new Vector2(192, 472));
            L3PandaPosList.Add(new Vector2(503, 472));
            L3PandaPosList.Add(new Vector2(789, 472));
        }

        private void AddOnClickEvent() 
        {
            Util.AddBtnClick(panda1.gameObject, OnClickEventFault);
            Util.AddBtnClick(panda2.gameObject, OnClickEventRight);
            Util.AddBtnClick(panda3.gameObject, OnClickEventFault);

            Util.AddBtnClick(panda4.gameObject, OnClickEventFault);
            Util.AddBtnClick(panda5.gameObject, OnClickEventRight);
            Util.AddBtnClick(panda6.gameObject, OnClickEventFault);
            Util.AddBtnClick(panda7.gameObject, OnClickEventFault);

            Util.AddBtnClick(panda8.gameObject, OnClickEventFault);
            Util.AddBtnClick(panda9.gameObject, OnClickEventFault);
            Util.AddBtnClick(panda10.gameObject, OnClickEventFault);
            Util.AddBtnClick(panda11.gameObject, OnClickEventRight);
            Util.AddBtnClick(panda12.gameObject, OnClickEventFault);
            Util.AddBtnClick(xem.gameObject,PlayFaultVoice);
        }
        private void PlayFaultVoice(GameObject obj) 
        {

            _Mask.gameObject.SetActive(true);
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1, false),()=> 
            {
                _Mask.gameObject.SetActive(false);
            });
        }
     
            public int SingleNumber(int[] nums)
            {
            Array.Sort(nums);
            HashSet<int> b = new HashSet<int>();
         //   List<int> a = new List<int>();
            for (int i = 0; i < nums.Length; i++) 
            {
                //if (a.Contains(nums[i]))
                //{
                //    a.Remove(nums[i]);
                //}
                //else 
                //{
                //    a.Add(nums[i]);
                //}
                if (b.Add(nums[i]))
                {
                    b.Remove(nums[i]);
                }
            }

             return b.ToList().Count;

        }
     
        private void OnClickEventRight (GameObject obj)
        {
            
            XemMovePause = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
            _Mask.gameObject.SetActive(true);
            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false), () =>
            {
               
            });
            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "xm-g" + obj.name, false, () =>
            {
                Delay(0.5f,()=> { obj.transform.SetSiblingIndex(bg3.GetSiblingIndex() + 1); });
      
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                //开始替换
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "xm-j" + obj.name, false, () =>
                {
                    if (obj.name == "2")
                    {
                        CheckPos(obj, "xm-z1", "xm-tt1");
                    }

                    if (obj.name == "5")
                    {
                        CheckPos(obj, "xm-z5", "xm-tt5");
                    }
                    if (obj.name == "11")
                    {
                        CheckPos(obj, "xm-z11", "xm-tt11");
                    }
                    Delay(7f,()=> 
                    { 
                        if (obj.name == "2")
                        {
                      
                            L1NextL2();
                            panda4.gameObject.SetActive(true);
                            _mono.StartCoroutine(IEDelay(0.2f,()=> { panda5.gameObject.SetActive(true); }));
                            _mono.StartCoroutine(IEDelay(0.4f, () => { panda6.gameObject.SetActive(true); }));
                            _mono.StartCoroutine(IEDelay(0.6f, () => { panda7.gameObject.SetActive(true); Level2StartGame(); }));
                        
                        }
                        if (obj.name == "5")
                        {
                     
                            L2NextL3();
                            panda8.gameObject.SetActive(true);
                            _mono.StartCoroutine(IEDelay(0.2f, () => { panda9.gameObject.SetActive(true); }));
                            _mono.StartCoroutine(IEDelay(0.4f, () => { panda10.gameObject.SetActive(true); }));
                            _mono.StartCoroutine(IEDelay(0.6f, () => { panda11.gameObject.SetActive(true); }));
                            _mono.StartCoroutine(IEDelay(0.6f, () => { panda12.gameObject.SetActive(true); Level3StartGame(); }));
                           
                        }
                        if (obj.name == "11")
                        {
                            GameSuccess();
                        }
                    });
                });
            });
        }

        private void OnClickEventFault(GameObject obj)
        {
            _Mask.gameObject.SetActive(true);
            FaultIndex++;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        
            // obj.transform.SetSiblingIndex(bg3.GetSiblingIndex() + 1);
       
            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject,"xm-g"+obj.name,false,()=> 
            {
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "xm-j" + obj.name, false, () =>
                {
                
                    CheckLevelIndex();

                    _Mask.gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "xm" + obj.name, true, () =>
                    {
                       
                    });
                });
            });
        }





        private void CheckLevelIndex() 
        {
            if (currentLevelIndex == 1)
            {
                if (FaultIndex == 1)
                {
                    Level1FIndex++;
                    FaultIndex = 0;
                    panda1.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda2.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda3.GetComponent<Empty4Raycast>().raycastTarget = false;
                    
                    Level1ChangePos2();
                    Delay(2f, () => 
                    {
                        if (Level1FIndex == 1)
                        {
                            Level1ChangePos();
                        }
                        else if (Level1FIndex == 2) 
                        {
                            Level1FIndex = 0;
                            Level1ChangePos3();
                        }
                       
                        Delay(2f, () =>
                        {

                            panda1.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda2.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda3.GetComponent<Empty4Raycast>().raycastTarget = true;
                        });
                    });
                }
            } else if (currentLevelIndex == 2)
            {
                if (FaultIndex == 2)
                {
                    FaultIndex = 0;
                    panda4.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda5.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda6.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda7.GetComponent<Empty4Raycast>().raycastTarget = false;
                    ChangePos2();
                    Delay(2f, () => { 
                        ChangePos2();
                        Delay(2f, () =>
                        {

                            panda4.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda5.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda6.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda7.GetComponent<Empty4Raycast>().raycastTarget = true;
                        });
                    });
                }
            }
            else if (currentLevelIndex == 3) 
            {
                if (FaultIndex == 3)
                {
                    FaultIndex = 0;
                    panda8.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda9.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda10.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda11.GetComponent<Empty4Raycast>().raycastTarget = false;
                    panda12.GetComponent<Empty4Raycast>().raycastTarget = false;
                    ChangePos3();
                    Delay(2f, () => 
                    { 
                        ChangePos3();
                        Delay(2f, () =>
                        {

                            panda8.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda9.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda10.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda11.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda12.GetComponent<Empty4Raycast>().raycastTarget = true;
                        });
                    });
                }
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _sDD.SetActive(false);
            XemMovePause = false;
            IEXemMoveEvent(xem.GetRectTransform(), -760, 760, XemMoveSpeed, XemMoveCallBack);

            panda1.gameObject.SetActive(true);
            _mono.StartCoroutine(IEDelay(0.3f,()=> { panda2.gameObject.SetActive(true); }));
            _mono.StartCoroutine(IEDelay(0.7f, () => { panda3.gameObject.SetActive(true); Level1StartGame(); }));
      



            //测试代码记得删
            //Delay(4,GameSuccess);
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
					PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();

                        //ToDo...
                        //显示Middle角色并且说话 		
                        _sDD.SetActive(false);
                        _dDD.SetActive(true); BellSpeck(_dDD, 3,null,null,RoleType.Adult);
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
        private void RandomChangePos(Transform panda) 
        {
     
            int i = Random.Range(0,L1PandaPosList.Count);  
  
            panda.GetRectTransform().DOAnchorPosX(L1PandaPosList[i].x,2).SetEase(Ease.Linear);
            L1PandaPosList.Remove(L1PandaPosList[i]);
        }
        private void RandomChangePos2(Transform panda)
        {

            int i = Random.Range(0, L2PandaPosList.Count);
            panda.GetRectTransform().DOAnchorPosX(L2PandaPosList[i].x, 2).SetEase(Ease.Linear);
            L2PandaPosList.Remove(L2PandaPosList[i]);
        }
        private void RandomChangePos3(Transform panda)
        {

            int i = Random.Range(0, L3PandaPosList.Count);
            panda.GetRectTransform().DOAnchorPosX(L3PandaPosList[i].x, 2).SetEase(Ease.Linear);
            L3PandaPosList.Remove(L3PandaPosList[i]);
        }
        private void Level1ChangePos() 
        {
           // (-421,385.5)(137,385.5)(595,385.5)
           panda1.GetRectTransform().DOAnchorPosX(595,2).SetEase(Ease.Linear);
           panda2.GetRectTransform().DOAnchorPosX(137, 2f).SetEase(Ease.Linear);
           panda3.GetRectTransform().DOAnchorPosX(-421, 2f).SetEase(Ease.Linear);
            //addList();
           // RandomChangePos(panda1);
           // RandomChangePos(panda2);
           // RandomChangePos(panda3);
        }
        private void Level1ChangePos2()
        {
            // (-421,385.5)(137,385.5)(595,385.5)
           panda1.GetRectTransform().DOAnchorPosX(-421, 2).SetEase(Ease.Linear);
           panda2.GetRectTransform().DOAnchorPosX(595, 2f).SetEase(Ease.Linear);
           panda3.GetRectTransform().DOAnchorPosX(137, 2f).SetEase(Ease.Linear);
            //addList();
            // RandomChangePos(panda1);
            // RandomChangePos(panda2);
            // RandomChangePos(panda3);
        }
        private void Level1ChangePos3()
        {
            panda1.GetRectTransform().DOAnchorPosX(137, 2).SetEase(Ease.Linear);
            panda2.GetRectTransform().DOAnchorPosX(-421, 2f).SetEase(Ease.Linear);
            panda3.GetRectTransform().DOAnchorPosX(595, 2f).SetEase(Ease.Linear);
        }
        private void ChangePos2()
        {
            //(-421,385.5)(137,385.5)(595,385.5)
            //panda1.GetRectTransform().DOAnchorPosX(-421, 2).SetEase(Ease.Linear);
            //panda2.GetRectTransform().DOAnchorPosX(595, 2f).SetEase(Ease.Linear);
            //panda3.GetRectTransform().DOAnchorPosX(137, 2f).SetEase(Ease.Linear);
            addList2();
            RandomChangePos2(panda4);
            RandomChangePos2(panda5);
            RandomChangePos2(panda6);
            RandomChangePos2(panda7);
        }
        private void ChangePos3()
        {
            addList3();
            //(-421,385.5)(137,385.5)(595,385.5)
            //panda1.GetRectTransform().DOAnchorPosX(137, 2).SetEase(Ease.Linear);
            //panda2.GetRectTransform().DOAnchorPosX(-421, 2f).SetEase(Ease.Linear);
            //panda3.GetRectTransform().DOAnchorPosX(595, 2f).SetEase(Ease.Linear);
            RandomChangePos3(panda8);
            RandomChangePos3(panda9);
            RandomChangePos3(panda10);
            RandomChangePos3(panda11);
            RandomChangePos3(panda12);
        }

        private void ChangePos4() 
        {
            //-754 -229 243 820
            panda4.GetRectTransform().DOAnchorPosX(-754, 2).SetEase(Ease.Linear);
            panda5.GetRectTransform().DOAnchorPosX(820, 2f).SetEase(Ease.Linear);
            panda6.GetRectTransform().DOAnchorPosX(243, 2f).SetEase(Ease.Linear);
            panda7.GetRectTransform().DOAnchorPosX(-229, 2f).SetEase(Ease.Linear);
        }
        private void ChangePos5()
        {
            //-754 -229 243 820
            panda4.GetRectTransform().DOAnchorPosX(820, 2).SetEase(Ease.Linear);
            panda5.GetRectTransform().DOAnchorPosX(-229, 2f).SetEase(Ease.Linear);
            panda6.GetRectTransform().DOAnchorPosX(-754, 2f).SetEase(Ease.Linear);
            panda7.GetRectTransform().DOAnchorPosX(243, 2f).SetEase(Ease.Linear);
        }
        private void ChangePos6()
        {
            //-754 -229 243 820
            panda4.GetRectTransform().DOAnchorPosX(-754, 2f).SetEase(Ease.Linear);
            panda5.GetRectTransform().DOAnchorPosX(243, 2f).SetEase(Ease.Linear);
            panda6.GetRectTransform().DOAnchorPosX(-229, 2f).SetEase(Ease.Linear);
            panda7.GetRectTransform().DOAnchorPosX(820, 2f).SetEase(Ease.Linear);
        }
        private void ChangePos7()
        {
            //-671 -234 192 503 789
            panda8.GetRectTransform().DOAnchorPosX(789, 2f).SetEase(Ease.Linear);
            panda9.GetRectTransform().DOAnchorPosX(-234, 2f).SetEase(Ease.Linear);
            panda10.GetRectTransform().DOAnchorPosX(671, 2f).SetEase(Ease.Linear);
            panda11.GetRectTransform().DOAnchorPosX(192, 2f).SetEase(Ease.Linear);
            panda12.GetRectTransform().DOAnchorPosX(503, 2f).SetEase(Ease.Linear);
        }
        private void ChangePos8()
        {
            //-671 -234 192 503 789
            panda8.GetRectTransform().DOAnchorPosX(192, 2f).SetEase(Ease.Linear);
            panda9.GetRectTransform().DOAnchorPosX(789, 2f).SetEase(Ease.Linear);
            panda10.GetRectTransform().DOAnchorPosX(-234, 2f).SetEase(Ease.Linear);
            panda11.GetRectTransform().DOAnchorPosX(503, 2f).SetEase(Ease.Linear);
            panda12.GetRectTransform().DOAnchorPosX(-671, 2f).SetEase(Ease.Linear);
        }
        private void ChangePos9()
        {
            //-671 -234 192 503 789
            panda8.GetRectTransform().DOAnchorPosX(503, 2f).SetEase(Ease.Linear);
            panda9.GetRectTransform().DOAnchorPosX(-671, 2f).SetEase(Ease.Linear);
            panda10.GetRectTransform().DOAnchorPosX(192, 2f).SetEase(Ease.Linear);
            panda11.GetRectTransform().DOAnchorPosX(789, 2f).SetEase(Ease.Linear);
            panda12.GetRectTransform().DOAnchorPosX(-234, 2f).SetEase(Ease.Linear);
        }
        private  void Level1StartGame() 
        {

            _mono.StartCoroutine(IEPandaMove(panda1.GetRectTransform(), -421, -542, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda2.GetRectTransform(), 137, -33, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda3.GetRectTransform(), 595, 429, 1f, 3f, 3));
            Delay(7f, () =>
            {
                Level1ChangePos();
                Delay(2f, () =>
                {
                    Level1ChangePos2();
                    Delay(2f, () =>
                    {
                        Level1ChangePos3();
                        Delay(2f, () => 
                        {
                        
                            panda1.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda2.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda3.GetComponent<Empty4Raycast>().raycastTarget = true;
                        });
                    });
                });
            });
        }
        private void Level2StartGame() 
        {
            _mono.StartCoroutine(IEPandaMove(panda4.GetRectTransform(), -754, -547, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda5.GetRectTransform(), -229, -32, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda6.GetRectTransform(), 243,418, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda7.GetRectTransform(), 820, 740, 1f, 3f, 3));
            Delay(7f,()=> 
            {
                ChangePos2();
                Delay(2f,()=> 
                {
                    ChangePos2();
                    Delay(2f,()=> 
                    {
                        ChangePos2();
                        Delay(2f, () =>
                        {
                            panda4.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda5.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda6.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda7.GetComponent<Empty4Raycast>().raycastTarget = true;
                        });
                    });
                });
            });
        }
        private void Level3StartGame()
        {
            _mono.StartCoroutine(IEPandaMove(panda8.GetRectTransform(), -671, -821, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda9.GetRectTransform(), -234, -300, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda10.GetRectTransform(), 192,37, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda11.GetRectTransform(), 503,408, 1f, 3f, 3));
            _mono.StartCoroutine(IEPandaMove(panda12.GetRectTransform(), 789, 685, 1f, 3f, 3));
            Delay(7f, () =>
            {
                ChangePos3();
                Delay(2f, () =>
                {
                    ChangePos3();
                    Delay(2f, () =>
                    {
                        ChangePos3();
                        Delay(2f, () =>
                        {
                            panda8.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda9.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda10.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda11.GetComponent<Empty4Raycast>().raycastTarget = true;
                            panda12.GetComponent<Empty4Raycast>().raycastTarget = true;
                        });
                    });
                });
            });
        }
        private void L1NextL2() 
        {
            currentLevelIndex++;
            isChangeVoice = false;
            _Mask.gameObject.SetActive(false);
            panda1.gameObject.SetActive(false); panda2.gameObject.SetActive(false); panda3.gameObject.SetActive(false);
            panda4.gameObject.SetActive(false); panda5.gameObject.SetActive(false); panda6.gameObject.SetActive(false); panda7.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject,"xem1",true);
            XemMovePause = false;
        }

        private void L2NextL3()
        {
            currentLevelIndex++; isChangeVoice = false;
            _Mask.gameObject.SetActive(false);
            panda4.gameObject.SetActive(false); panda5.gameObject.SetActive(false); panda6.gameObject.SetActive(false); panda7.gameObject.SetActive(false);
            panda8.gameObject.SetActive(false); panda9.gameObject.SetActive(false); panda10.gameObject.SetActive(false); panda11.gameObject.SetActive(false); panda12.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem1", true);
            XemMovePause = false;
        }

        private void CheckPos(GameObject obj,string spinename1, string spinename2) 
        {
            if (obj.transform.GetRectTransform().anchoredPosition.x >= xem.GetRectTransform().anchoredPosition.x)
            {
              
                //恶魔在左                                     
                float speed = 3f;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject,spinename1, true, () =>
                {

                });
                if (obj.transform.GetRectTransform().anchoredPosition.x - xem.GetRectTransform().anchoredPosition.x < 300) 
                {
                    isChangeVoice = true;
                    speed = 1.5f;
                }

                if (isChangeVoice == true)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                }
                else 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                }
               
                obj.transform.GetRectTransform().DOAnchorPos(new Vector2(xem.GetRectTransform().anchoredPosition.x + xem.GetRectTransform().rect.width / 2 + obj.transform.GetRectTransform().rect.width / 4, 375f), speed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Delay(0.5f,()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4); });
                   
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, spinename2, false, () =>
                    {
                     
                        SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "sc-boom", false);
                    });

                });
            }
            else if (obj.transform.GetRectTransform().anchoredPosition.x < xem.GetRectTransform().anchoredPosition.x)
            {
                //恶魔在右
             
                float speed = 3f;
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, spinename1, true, () =>
                {

                });
               
                if (xem.GetRectTransform().anchoredPosition.x - obj.transform.GetRectTransform().anchoredPosition.x < 300)
                {
                    
                    speed = 1.5f;
                    isChangeVoice = true;
                }
                if (isChangeVoice == true)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                }
                obj.transform.GetRectTransform().DOAnchorPos(new Vector2(xem.GetRectTransform().anchoredPosition.x - xem.GetRectTransform().rect.width / 2 - obj.transform.GetRectTransform().rect.width / 4 ,375f), speed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Delay(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4); });
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, spinename2, false, () =>
                    {
                        SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "sc-boom", false);
                    });
                });

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
