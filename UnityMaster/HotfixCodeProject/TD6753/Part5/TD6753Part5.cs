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

    public class TD6753Part5
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

        private Transform drag1;
        private Transform drag2;
        private Transform drag3;
        private Transform spineL1;
        private Transform spineL2;
        private Transform spineL3;
        private int Index;
        private List<mILDrager> DragerList1;
        private List<mILDrager> DragerList2;
        private List<mILDrager> DragerList3;
        private Transform xx; private Transform xx2; private Transform xx3;


        private Transform za1; private Transform za2; private Transform za3;
        private Transform xem;
        private Transform ui;
        private Transform mask1;




        private bool _isPlaying;

        void Start(object o)
        {
            Input.multiTouchEnabled = false;
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            mask1 = curTrans.Find("_mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");


            _dDD = curTrans.GetGameObject("dDD");



            _sDD = curTrans.GetGameObject("sDD");


            drag1 = curTrans.Find("drag1");
            drag2= curTrans.Find("drag2");
            drag3 = curTrans.Find("drag3");
            spineL1 = curTrans.Find("spineL1");
            spineL2 = curTrans.Find("spineL2");
            spineL3 = curTrans.Find("spineL3");
            za1 = curTrans.Find("za1");
            za2 = curTrans.Find("za2");
            za3 = curTrans.Find("za3");
            xem = curTrans.Find("xem");
            xx = curTrans.Find("xx"); xx2 = curTrans.Find("xx2"); xx3 = curTrans.Find("xx3");
            ui = curTrans.Find("ui");









            GameInit();
            InitDrager();
            AddDragEvent();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;
            Index = 0;
            mask1.gameObject.SetActive(false);
            za1.gameObject.SetActive(true);   za2.gameObject.SetActive(true); za3.gameObject.SetActive(true);
            za1.GetRectTransform().anchoredPosition = new Vector2(668,69);
            za2.GetRectTransform().anchoredPosition = new Vector2(156, 69);
            za3.GetRectTransform().anchoredPosition = new Vector2(156, 69);

            xx2.gameObject.SetActive(false);
            spineL2.gameObject.SetActive(false);
            drag2.gameObject.SetActive(false);
            ui.GetChild(0).gameObject.SetActive(true);
            ui.GetChild(1).gameObject.SetActive(true);
            ui.GetChild(2).gameObject.SetActive(true);

            xx.gameObject.SetActive(true);
            spineL1.gameObject.SetActive(true);
            drag1.gameObject.SetActive(true);

            xx2.gameObject.SetActive(false);
            spineL2.gameObject.SetActive(false);
            drag2.gameObject.SetActive(false);

            xx3.gameObject.SetActive(false);
            spineL3.gameObject.SetActive(false);
            drag3.gameObject.SetActive(false);
            spineL1.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL1.GetChild(1).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL1.GetChild(2).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL1.GetChild(3).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);

            spineL2.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL2.GetChild(1).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL2.GetChild(2).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL2.GetChild(3).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL2.GetChild(4).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);

            spineL3.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL3.GetChild(1).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL3.GetChild(2).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL3.GetChild(3).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL3.GetChild(4).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            spineL3.GetChild(5).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(spineL1.GetChild(0).gameObject,"1-a",false);
            SpineManager.instance.DoAnimation(spineL1.GetChild(1).gameObject, "1-b", false);
            SpineManager.instance.DoAnimation(spineL1.GetChild(2).gameObject, "1-c", false);
            SpineManager.instance.DoAnimation(spineL1.GetChild(3).gameObject, "1-d", false);

            SpineManager.instance.DoAnimation(spineL2.GetChild(0).gameObject, "2-a", false);
            SpineManager.instance.DoAnimation(spineL2.GetChild(1).gameObject, "2-b", false);
            SpineManager.instance.DoAnimation(spineL2.GetChild(2).gameObject, "2-c", false);
            SpineManager.instance.DoAnimation(spineL2.GetChild(3).gameObject, "2-d", false);
            SpineManager.instance.DoAnimation(spineL2.GetChild(4).gameObject, "2-e", false);

            SpineManager.instance.DoAnimation(spineL3.GetChild(0).gameObject, "3-a", false);
            SpineManager.instance.DoAnimation(spineL3.GetChild(1).gameObject, "3-b", false);
            SpineManager.instance.DoAnimation(spineL3.GetChild(2).gameObject, "3-c", false);
            SpineManager.instance.DoAnimation(spineL3.GetChild(3).gameObject, "3-d", false);
            SpineManager.instance.DoAnimation(spineL3.GetChild(4).gameObject, "3-e", false);
            SpineManager.instance.DoAnimation(spineL3.GetChild(5).gameObject, "3-f", false);

            drag1.Find("1").GetRectTransform().anchoredPosition = new Vector2(-799,345);
            drag1.Find("1").gameObject.SetActive(true);
            drag1.Find("1").GetComponent<RawImage>().texture = drag1.Find("1").GetComponent<BellSprites>().texture[0];
            drag1.Find("2").GetRectTransform().anchoredPosition = new Vector2(-800, 97);
            drag1.Find("2").gameObject.SetActive(true);
            drag1.Find("2").GetComponent<RawImage>().texture = drag1.Find("2").GetComponent<BellSprites>().texture[0];
            drag1.Find("3").GetRectTransform().anchoredPosition = new Vector2(-800, -143.4f);
            drag1.Find("3").gameObject.SetActive(true);
            drag1.Find("3").GetComponent<RawImage>().texture = drag1.Find("3").GetComponent<BellSprites>().texture[0];
            drag1.Find("4").GetRectTransform().anchoredPosition = new Vector2(-799, -386);
            drag1.Find("4").gameObject.SetActive(true);
            drag1.Find("4").GetComponent<RawImage>().texture = drag1.Find("4").GetComponent<BellSprites>().texture[0];

            drag2.Find("1").GetRectTransform().anchoredPosition = new Vector2(-800, 355f);
            drag2.Find("1").gameObject.SetActive(true);
            drag2.Find("1").GetComponent<RawImage>().texture = drag2.Find("1").GetComponent<BellSprites>().texture[0];
            drag2.Find("2").GetRectTransform().anchoredPosition = new Vector2(-800, 153);
            drag2.Find("2").gameObject.SetActive(true);
            drag2.Find("2").GetComponent<RawImage>().texture = drag2.Find("2").GetComponent<BellSprites>().texture[0];
            drag2.Find("3").GetRectTransform().anchoredPosition = new Vector2(-799, -48.7f);
            drag2.Find("3").gameObject.SetActive(true);
            drag2.Find("3").GetComponent<RawImage>().texture = drag2.Find("3").GetComponent<BellSprites>().texture[0];
            drag2.Find("4").GetRectTransform().anchoredPosition = new Vector2(-799, -255);
            drag2.Find("4").gameObject.SetActive(true);
            drag2.Find("4").GetComponent<RawImage>().texture = drag2.Find("4").GetComponent<BellSprites>().texture[0];
            drag2.Find("5").GetRectTransform().anchoredPosition = new Vector2(-799, -456);
            drag2.Find("5").gameObject.SetActive(true);
            drag2.Find("5").GetComponent<RawImage>().texture = drag2.Find("5").GetComponent<BellSprites>().texture[0];

            drag3.Find("1").GetRectTransform().anchoredPosition = new Vector2(-792, 370.3f);
            drag3.Find("1").gameObject.SetActive(true);
            drag3.Find("1").GetComponent<RawImage>().texture = drag3.Find("1").GetComponent<BellSprites>().texture[0];
            drag3.Find("2").GetRectTransform().anchoredPosition = new Vector2(-793, 202);
            drag3.Find("2").gameObject.SetActive(true);
            drag3.Find("2").GetComponent<RawImage>().texture = drag3.Find("2").GetComponent<BellSprites>().texture[0];
            drag3.Find("3").GetRectTransform().anchoredPosition = new Vector2(-792, 38);
            drag3.Find("3").gameObject.SetActive(true);
            drag3.Find("3").GetComponent<RawImage>().texture = drag3.Find("3").GetComponent<BellSprites>().texture[0];
            drag3.Find("4").GetRectTransform().anchoredPosition = new Vector2(-793.6f, -133);
            drag3.Find("4").gameObject.SetActive(true);
            drag3.Find("4").GetComponent<RawImage>().texture = drag3.Find("4").GetComponent<BellSprites>().texture[0];
            drag3.Find("5").GetRectTransform().anchoredPosition = new Vector2(-793.6f, -299.5f);
            drag3.Find("5").gameObject.SetActive(true);
            drag3.Find("5").GetComponent<RawImage>().texture = drag3.Find("5").GetComponent<BellSprites>().texture[0];
            drag3.Find("6").GetRectTransform().anchoredPosition = new Vector2(-793, -466.3f);
            drag3.Find("6").gameObject.SetActive(true);
            drag3.Find("6").GetComponent<RawImage>().texture = drag3.Find("6").GetComponent<BellSprites>().texture[0];

            xem.GetChild(0).gameObject.SetActive(true);
            xem.GetChild(1).gameObject.SetActive(true);
            xem.GetChild(2).gameObject.SetActive(true);


            InitSpine(xem.GetChild(0).gameObject, "xem1");
            InitSpine(xem.GetChild(1).gameObject, "", false);
            InitSpine(xem.GetChild(2).gameObject, "", false);
    ;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }
        void InitDrager()
        {
            DragerList1 = new List<mILDrager>();
           
            DragerList1.Add(drag1.Find("1").GetComponent<mILDrager>());
            DragerList1.Add(drag1.Find("2").GetComponent<mILDrager>());
            DragerList1.Add(drag1.Find("3").GetComponent<mILDrager>());
            DragerList1.Add(drag1.Find("4").GetComponent<mILDrager>());

            DragerList2 = new List<mILDrager>();
           
            DragerList2.Add(drag2.Find("1").GetComponent<mILDrager>());
            DragerList2.Add(drag2.Find("2").GetComponent<mILDrager>());
            DragerList2.Add(drag2.Find("3").GetComponent<mILDrager>());
            DragerList2.Add(drag2.Find("4").GetComponent<mILDrager>());
            DragerList2.Add(drag2.Find("5").GetComponent<mILDrager>());
            DragerList3 = new List<mILDrager>();
          
                DragerList3.Add(drag3.Find("1").GetComponent<mILDrager>());
                DragerList3.Add(drag3.Find("2").GetComponent<mILDrager>());
                DragerList3.Add(drag3.Find("3").GetComponent<mILDrager>());
                DragerList3.Add(drag3.Find("4").GetComponent<mILDrager>());
            DragerList3.Add(drag3.Find("5").GetComponent<mILDrager>());
            DragerList3.Add(drag3.Find("6").GetComponent<mILDrager>());
        }
        void AddDragEvent() 
        {
            foreach (var drager in DragerList1) 
            {
                drager.SetDragCallback(DragStart,null,DragEnd);
            }
            foreach (var drager in DragerList2)
            {
                drager.SetDragCallback(DragStart2, null, DragEnd2);
            }
            foreach (var drager in DragerList3)
            {
                drager.SetDragCallback(DragStart3, null, DragEnd3);
            }
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
                        _sDD.Show();
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
                    BellSpeck(_sDD, 1, null,()=> { StartGame(); }, RoleType.Adult);
                    break;                                
            }
            _talkIndex++;
        }

        private void DragStart(Vector3 position,int type,int index) 
        {
            DragerList1[index].transform.SetAsLastSibling();
            DragerList1[index].GetComponent<RawImage>().texture = DragerList1[index].GetComponent<BellSprites>().texture[1];
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            DragerList2[index].transform.SetAsLastSibling();
            DragerList2[index].GetComponent<RawImage>().texture = DragerList2[index].GetComponent<BellSprites>().texture[1];
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            DragerList3[index].transform.SetAsLastSibling();
            DragerList3[index].GetComponent<RawImage>().texture = DragerList3[index].GetComponent<BellSprites>().texture[1];
        }
        private void DragEnd(Vector3 position, int type, int index,bool isMatch)
        {
            mask1.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0, false);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false), () =>
                {
                    mask1.gameObject.SetActive(false);
                });
               
                
                
                DragerList1[index].gameObject.SetActive(false);
                switch (index) 
                {
                    case 0:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL1.Find("2").gameObject,"1-b2",false);
                        break;
                    case 1:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL1.Find("3").gameObject, "1-c2", false);
                        break;
                    case 2:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL1.Find("1").gameObject, "1-a2", false);
                        break;
                    case 3:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL1.Find("4").gameObject, "1-d2", false);
                        break;
                   
                }
                if (Index==4)
                {
                    //小恶魔挂
                    Delay(2f,()=> 
                    {
                        za1.GetRectTransform().DOAnchorPos(new Vector2(668,-603), 0.4f).OnComplete(() =>
                        {
                            za1.gameObject.SetActive(false); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem-y", false, () =>
                            {
                                //ui
                               
                                ui.GetChild(0).gameObject.SetActive(false);
                                SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem-y2", false, () =>
                                {
                                    xem.GetChild(0).gameObject.SetActive(false);
                                    Delay(1.5f, () => 
                                    {
                                        //下一关
                                        Index = 0;
                                        xem.GetChild(1).gameObject.SetActive(true);
                                        SpineManager.instance.DoAnimation(xem.GetChild(1).gameObject,"xem1",true);
                                        xx.gameObject.SetActive(false);
                                        spineL1.gameObject.SetActive(false);
                                        drag1.gameObject.SetActive(false);
                                        xx2.gameObject.SetActive(true);
                                        spineL2.gameObject.SetActive(true);
                                        drag2.gameObject.SetActive(true);
                                    });
                                });
                            });
                        });
                    });
                
                } 
            }
            else 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false), () =>
                {
                    mask1.gameObject.SetActive(false);
                });

                DragerList1[index].DoReset();
                DragerList1[index].GetComponent<RawImage>().texture = DragerList1[index].GetComponent<BellSprites>().texture[0];
            }
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            mask1.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false), () =>
                {
                    mask1.gameObject.SetActive(false);
                });

                DragerList2[index].gameObject.SetActive(false);
                switch (index)
                {
                    case 0:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL2.Find("3").gameObject, "2-c2", false);
                        break;
                    case 1:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL2.Find("4").gameObject, "2-d2", false);
                        break;
                    case 2:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL2.Find("5").gameObject, "2-e2", false);
                        break;
                    case 3:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL2.Find("1").gameObject, "2-a2", false);
                        break;
                    case 4:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL2.Find("2").gameObject, "2-b2", false);
                        break;

                }
                if (Index == 5)
                {
                    //小恶魔挂
                    Delay(2f, () =>
                    {
                        za2.GetRectTransform().DOAnchorPos(new Vector2(156, -156), 0.3f).OnComplete(() =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false); ;
                            za2.gameObject.SetActive(false);
                            SpineManager.instance.DoAnimation(xem.GetChild(1).gameObject, "xem-y", false, () =>
                            {
                                ui.GetChild(1).gameObject.SetActive(false);
                                SpineManager.instance.DoAnimation(xem.GetChild(1).gameObject, "xem-y2", false, () =>
                                {
                                    xem.GetChild(1).gameObject.SetActive(false);
                                    Delay(1.5f, () =>
                                    {
                                        //下一关
                                        Index = 0;
                                        xem.GetChild(2).gameObject.SetActive(true);
                                        SpineManager.instance.DoAnimation(xem.GetChild(2).gameObject, "xem1", true);
                                        xx2.gameObject.SetActive(false);
                                        spineL2.gameObject.SetActive(false);
                                        drag2.gameObject.SetActive(false);
                                        xx3.gameObject.SetActive(true);
                                        spineL3.gameObject.SetActive(true);
                                        drag3.gameObject.SetActive(true);
                                    });
                                });
                            });
                        });
                    });

                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false), () =>
                {
                    mask1.gameObject.SetActive(false);
                });
               
                DragerList2[index].DoReset();
                DragerList2[index].GetComponent<RawImage>().texture = DragerList2[index].GetComponent<BellSprites>().texture[0];
            }
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
            mask1.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false), () =>
                {
                    mask1.gameObject.SetActive(false);
                });

                DragerList3[index].gameObject.SetActive(false);
                switch (index)
                {
                    case 0:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL3.Find("6").gameObject, "3-f2", false);
                        break;
                    case 1:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL3.Find("2").gameObject, "3-b2", false);
                        break;
                    case 2:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL3.Find("5").gameObject, "3-e2", false);
                        break;
                    case 3:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL3.Find("3").gameObject, "3-c2", false);
                        break;
                    case 4:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL3.Find("1").gameObject, "3-a2", false);
                        break;
                    case 5:
                        Index++;
                        SpineManager.instance.DoAnimation(spineL3.Find("4").gameObject, "3-d2", false);
                        break;
                }
                if (Index == 6)
                {
                    //小恶魔挂
                    Delay(2f, () =>
                    {
                        za3.GetRectTransform().DOAnchorPos(new Vector2(156, -156), 0.3f).OnComplete(() =>
                        {
                            za3.gameObject.SetActive(false); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                            SpineManager.instance.DoAnimation(xem.GetChild(2).gameObject, "xem-y", false, () =>
                            {
                                ui.GetChild(2).gameObject.SetActive(false);
                                SpineManager.instance.DoAnimation(xem.GetChild(2).gameObject, "xem-y2", false, () =>
                                {
                                    xem.GetChild(2).gameObject.SetActive(false);
                                    Delay(1.5f, () =>
                                    {
                                       GameSuccess();
                                    });
                                });
                            });
                        });
                    });

                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false), () =>
                {
                    mask1.gameObject.SetActive(false);
                });
               
                DragerList3[index].DoReset();
                DragerList3[index].GetComponent<RawImage>().texture = DragerList3[index].GetComponent<BellSprites>().texture[0];
            }
        }
        #region 游戏逻辑




        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _sDD.SetActive(false);
            _dDD.SetActive(false);
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
                        _sDD.Hide();
                        _dDD.Show(); BellSpeck(_dDD, 2,null,null,RoleType.Adult);
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

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }
    }
}
