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
	
    public class TD8951Part1
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
		 private GameObject _dialogue;

        private Transform Card;
      
	   
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

		 private Vector2 _roleStartPos,_roleEndPos,_enemyStartPos,_enemyEndPos;
       
           
        private RectTransform _devilRect;
        
        private RectTransform _tianTianRect;
        
		
		   
        private Text _devilRectTxt;
        
		private Text _tianTianRectTxt;
       

		private List<string> _dialogues;

        

        private List<Vector2> CardInitPosList;
        private List<Vector2> YhcInitPosList;
        private List<Transform> StorageList;
        private Transform lightPos;
        private Transform xemLeavePos;
        private int ClickCount;
        private Transform light;
        private bool _canClick;
        private Transform xem;
        private Transform yhc;
        private Transform yhcEndingPos;
        
        


        private bool _isPlaying;

        void Start(object o)
        {
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
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
			
			
            _devilRect = curTrans.GetRectTransform("dialogue/devil");
            
            _tianTianRect = curTrans.GetRectTransform("dialogue/tianTian");
          
			
			
            _devilRectTxt = curTrans.GetText("dialogue/devil/Text");
            
            _tianTianRectTxt = curTrans.GetText("dialogue/tianTian/Text");
			
			
			 _dialogue = curTrans.GetGameObject("dialogue");
            lightPos = curTrans.Find("lightpos");
            xemLeavePos = curTrans.Find("xemLeavePos");
            yhcEndingPos = curTrans.Find("yhcEndPos");
            light = curTrans.Find("light");
            Card = curTrans.Find("Card");
            xem = curTrans.Find("BG/xem");
            yhc = curTrans.Find("yhc");
            ClickCount = 0;
            _canClick = true;              
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;

            StorageList = new List<Transform>();
            _devilRectTxt.text = string.Empty;            
			 _tianTianRectTxt.text =string.Empty;          
		    		   
		   
		     _dialogues = new List<string> {
             "哈哈哈，我来捣蛋喽",
             "小朋友们，小恶魔又来了，快和我们一起打跑它吧"
             };
            _roleStartPos = new Vector2(-2170, 539);  _roleEndPos = new Vector2(-960, 539);
            _enemyStartPos = new Vector2(200, 540); _enemyEndPos = new Vector2(-994, 540);
          

            SetPos(_devilRect, _enemyStartPos);            
            SetPos(_tianTianRect, _roleStartPos);
            AddBtnOnClick();
            AddYhcInitPosList();
            AddCardInitPosList();
            RandomPos();

            _canClick = true;
            StorageList.Clear();
            ClickCount = 0;
            xem.GetRectTransform().anchoredPosition = new Vector2(-710, 0);
            SpineManager.instance.DoAnimation(xem.gameObject, "xem1", true);
            light.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            for (int i = 0; i < 12; i++) 
            {
                yhc.GetChild(i).GetRectTransform().anchoredPosition = YhcInitPosList[i];
            }

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
			           
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
			
			          
			
		    
			_dTT.Hide(); 
			
			
			
			_sTT.Hide(); 		
			
			_dialogue.Hide();
			
			
			

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
						         
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
			
			_dialogue.Show();
			

            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);//ToDo...改BmgIndex
                        _startSpine.Hide();

                        //ToDo...
                        XemDialogue(0,0,()=> 
                        {
                            TianTianDialogue(1,1,()=> 
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            });
                        });
						//StartGame();
                        
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
                    _dialogue.SetActive(false);
                    _sTT.SetActive(true);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,2,_sTT,null,()=> 
                    {
                        _sTT.SetActive(false);
                        StartGame();
                    },RoleType.Adult));
                    break;
                case 2:
                   
                    break;
            }
            _talkIndex++;
        }
        private void AddCardInitPosList() 
        {
            CardInitPosList = new List<Vector2>();
            CardInitPosList.Add(new Vector2(-340,857));
            CardInitPosList.Add(new Vector2(-40, 857));
            CardInitPosList.Add(new Vector2(260, 857));
            CardInitPosList.Add(new Vector2(560, 857));
            CardInitPosList.Add(new Vector2(-340,538));
            CardInitPosList.Add(new Vector2(-40, 538));
            CardInitPosList.Add(new Vector2(260, 538));
            CardInitPosList.Add(new Vector2(560, 538));
            CardInitPosList.Add(new Vector2(-340, 219));
            CardInitPosList.Add(new Vector2(-40, 219));
            CardInitPosList.Add(new Vector2(260, 219));
            CardInitPosList.Add(new Vector2(560, 219));
        }
        private void AddYhcInitPosList() 
        {
            YhcInitPosList = new List<Vector2>();
            YhcInitPosList.Add(new Vector2(-883,590));
            YhcInitPosList.Add(new Vector2(-518, 838));
            YhcInitPosList.Add(new Vector2(167, 962));
            YhcInitPosList.Add(new Vector2(496, 819));
            YhcInitPosList.Add(new Vector2(734, 1039));
            YhcInitPosList.Add(new Vector2(-79, 76));
            YhcInitPosList.Add(new Vector2(840, 210));
            YhcInitPosList.Add(new Vector2(-289, 247));
            YhcInitPosList.Add(new Vector2(-569, 450));
            YhcInitPosList.Add(new Vector2(-116, 719));
            YhcInitPosList.Add(new Vector2(426, 477));
            YhcInitPosList.Add(new Vector2(-772, 477));

        }
        private void AddBtnOnClick() 
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Util.AddBtnClick(Card.GetChild(i).GetChild(j).gameObject, CardOnClickEvent);
                }
            }
        }
       
        private void RandomCardPoS(Transform obj) 
        {
            int a = Random.Range(0, CardInitPosList.Count);
            obj.GetRectTransform().anchoredPosition = CardInitPosList[a];
            CardInitPosList.Remove(CardInitPosList[a]);
        }
        private void RandomPos() 
        {
            for (int i = 0; i < 6; i++) 
            { 
                for (int j = 0; j <2; j++)
                {
                    Card.GetChild(i).GetChild(j).GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    Card.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(Card.GetChild(i).GetChild(j).GetChild(0).gameObject,"paiA",false);
                    //Card.GetChild(i).GetChild(j).GetRectTransform().anchoredPosition = RandomCardPos();
                    RandomCardPoS(Card.GetChild(i).GetChild(j));
                }
            }
        }
        private void CardOnClickEvent(GameObject obj)
        {
            if (_canClick)
            {

                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,3);
                StorageList.Add(obj.transform);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).GetChild(0).gameObject,"paiH2",false);
                if (StorageList.Count == 1)
                {

                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name + "2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name, false,()=> 
                        {
                            Delay(0.3f,()=> 
                            {
                                _canClick = true;
                            });
                            
                        });
                    });
                }
                else if (StorageList.Count == 2)
                {

                    if (StorageList[0].parent.name == StorageList[1].parent.name)
                    {
                       
                        if (StorageList[0].name == StorageList[1].name)
                        {
                            //翻到自己了 翻回去

                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name + "2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "paiA", false, () =>
                                {
                                    Delay(0.3f, () =>
                                    {
                                        _canClick = true;
                                    });
                                    //注意
                                    StorageList.Clear();
                                });
                            });
                        }
                        else
                        {
                            //翻到正确的选项了 
                         
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1);
                            ClickCount++;
                            StorageList[0].parent.SetAsLastSibling();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name + "2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name, false, () =>
                                {
                                    Delay(1.5f,()=> 
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                                        StorageList[0].GetRectTransform().DOAnchorPos(lightPos.GetRectTransform().anchoredPosition, 1.2f);
                                        StorageList[1].GetRectTransform().DOAnchorPos(lightPos.GetRectTransform().anchoredPosition, 1.2f).OnComplete(() =>
                                        {
                                            SpineManager.instance.DoAnimation(light.gameObject, "light", false, () =>
                                            {
                                                SpineManager.instance.DoAnimation(light.gameObject, "ren" + StorageList[0].parent.name, false, () =>
                                                {
                                                    light.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                                    
                                                    if (ClickCount == 6)
                                                    {
                                                        //要出现成功特效
                                                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);
                                                        XemFlyEffect();
                                                        Delay(4f, () =>
                                                        {
                                                            yhcEnding();
                                                            Delay(4f, () =>
                                                            {
                                                                xemAndYhcLeave();
                                                                Delay(6f,()=> { GameSuccess(); });
                                                            });
                                                        });
                                                    }
                                                    else
                                                    {
                                                        Delay(0.3f, () =>
                                                        {
                                                            _canClick = true;
                                                        });
                                                    }

                                                });

                                                StorageList[0].gameObject.SetActive(false); StorageList[1].gameObject.SetActive(false);
                                                StorageList.Clear();
                                            });
                                        });
                                    });
                                 
                                });
                            });

                        }

                    }
                    else
                    {
                     
                        //翻到的不是自己 是别人 
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name + "2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "pai" + obj.transform.parent.name, false, () =>
                            {
                                //注意
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
                                SpineManager.instance.DoAnimation(xem.gameObject,"xem2",false,()=> 
                                {
                                    SpineManager.instance.DoAnimation(xem.gameObject,"xem1",true);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                                    SpineManager.instance.DoAnimation(StorageList[0].GetChild(0).gameObject, "pai" + StorageList[0].parent.name + "2", false, () =>
                                    {
                                        SpineManager.instance.DoAnimation(StorageList[0].GetChild(0).gameObject, "paiA", false, () =>
                                        {
                                            //注意

                                        });
                                    });
                                    SpineManager.instance.DoAnimation(StorageList[1].GetChild(0).gameObject, "pai" + StorageList[1].parent.name + "2", false, () =>
                                    {
                                        SpineManager.instance.DoAnimation(StorageList[1].GetChild(0).gameObject, "paiA", false, () =>
                                        {
                                            //注意
                                            Delay(0.3f, () =>
                                            {
                                                _canClick = true;
                                            });
                                            StorageList.Clear();
                                        });
                                    });
                                });
                               
                            });
                        });
                    }
                }
            }
           
          
   

        }

        private void XemFlyEffect() 
        {
            xem.GetRectTransform().DOAnchorPos(lightPos.GetRectTransform().anchoredPosition,4f);
        }
        IEnumerator DoAnimation(float time,Action a=null)
        {
            for (int i=0;i<yhc.childCount;i++)
            {
                yield return new WaitForSeconds(time);
                SpineManager.instance.DoAnimation(yhc.GetChild(i).gameObject,"yhc",true);
            }
            a?.Invoke();

           // yield break;
        }
       
        private void yhcEnding() 
        {
            for (int i=0;i<yhc.childCount;i++)
            {
                yhc.GetChild(i).GetRectTransform().DOAnchorPos(yhcEndingPos.GetChild(i).GetRectTransform().anchoredPosition,4);
            }
        }
        private void xemAndYhcLeave() 
        {
            SpineManager.instance.DoAnimation(xem.gameObject,"xem4",true);
            xem.GetRectTransform().DOAnchorPos(xemLeavePos.GetRectTransform().anchoredPosition,5f);
            for (int i = 0; i < yhc.childCount; i++)
            {
                yhc.GetChild(i).GetRectTransform().DOAnchorPos(xemLeavePos.GetRectTransform().anchoredPosition, 5.5f);
            }
        }
        //游戏开始所有卡牌先展示几秒
        private void GameStartAllCardShow() 
        {
            _canClick = false;

            Delay(2f,()=> 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2);
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                      
                        OpenCard(i,j);
                   }
                }  
            });
          
            Delay(6f,()=> 
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        CloseCard(i,j);
                    }
                }            
            });
        }
        private void OpenCard(int i,int j) 
        {
            SpineManager.instance.DoAnimation(Card.GetChild(i).GetChild(j).GetChild(0).gameObject, "pai" + Card.GetChild(i).name + "2", false,()=> 
            {
                SpineManager.instance.DoAnimation(Card.GetChild(i).GetChild(j).GetChild(0).gameObject, "pai" + Card.GetChild(i).name, false);
            });
        }
        private void CloseCard(int i, int j)
        {
            SpineManager.instance.DoAnimation(Card.GetChild(i).GetChild(j).GetChild(0).gameObject, "pai" + Card.GetChild(i).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(Card.GetChild(i).GetChild(j).GetChild(0).gameObject, "paiA", false, () =>
                {
                    _canClick = true;
                });
            });
        }
        #region 游戏逻辑



        /// <summary>
        /// 开始对话
        /// </summary>
        private void StartDialogue()
        {
			 //ToDo...  
			 
            //测试代码记得删
			//Delay(4,GameSuccess);			 
        }




		
        /// <summary>
        /// 小恶魔对话
        /// </summary>		
		private void XemDialogue(int dialogueIndex,int soundIndex,Action callBack)
		{			 
			 SetMove(_devilRect, _enemyEndPos, 1.0f, () => {
                ShowDialogue(_dialogues[dialogueIndex], _devilRectTxt);
                Delay(PlaySound(soundIndex), callBack);
            });
		}


		
		

		
		/// <summary>
        /// 田田对话
        /// </summary>	
	    private void TianTianDialogue(int dialogueIndex,int soundIndex,Action callBack)
		{			
            SetMove(_tianTianRect, _roleEndPos, 1.0f, () => {
                ShowDialogue(_dialogues[dialogueIndex], _tianTianRectTxt);                  
                Delay(PlaySound(soundIndex), callBack);
            });
		}


			
	    
     
		
        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _dialogue.SetActive(false);
            _mono.StartCoroutine(DoAnimation(0.1f, null));
            GameStartAllCardShow();
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
                        _dTT.Show();
                        BellSpeck(_dTT,3,null,null,RoleType.Adult);						
                       
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

    }
}
