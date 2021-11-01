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
	
    public class TD6744Part5
    {
        public enum ColorEnum
        {
            orange,
            yellow,
            blue,
            green,
            perple,
            Null
        }
        public enum DragerEnum
        {
            Bkb,
            Lock,
            Null
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
	    
				
		  private GameObject _dBD;
        
				
		  private GameObject _xem;		
		  private GameObject _sBD;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
        private bool _isPlaying;

        private Transform xem;
        private GameObject[] _xems;

        private Transform bkbPanel;
        private GameObject[] _bkbPanel;
        private Empty4Raycast[] bkbE4r;

        private Transform spinePanel;
        private GameObject[] _spinePanel;

        private Transform bG;
        private Transform bG_0;
        private Vector3 bGStartPos;
        private Vector3 bearStartPos;
        private Vector3 bearMonStartPos;
        private Vector3 bearMonEndPo;
        private Transform bGPos;
        private Vector3[] bGEndPos;
        private Transform xfbkBg;
        private GameObject[] _xfbkBg;
        private Transform bg_6;
        private Vector3 bg_6StartPos;
        private Vector3 bg_6EndPos;

        private GameObject lockPanel_0;
        private ILDroper lockDroper;
        private ILDrager keyDrager;
        private GameObject bearMom;
        private GameObject lockPanel_2;
        private GameObject lockPanel_3;

        private string[] xfbkName;
        private GameObject clickMask;

        private ColorEnum colorEnum;
        private int bkbSum;
        private DragerEnum dragerEnum;

        private ILDroper _ilDroper;
        private ILDrager[] _ilDrager;
        private int droperIndex;
        private int drageSucceedIndex;
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

			_dBD = curTrans.GetGameObject("dBD");
			
			           			
			_xem = curTrans.GetGameObject("xem");
			_sBD = curTrans.GetGameObject("sBD");
         	
                                           
            GameInit();
            FindInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;
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
			
			          
			 _dBD.Hide(); 
		    
			
			_xem.Hide(); 
			_sBD.Hide(); 
			
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            xfbkName = new string[] { "a", "b", "c", "d", "e" };
        }
        private void FindInit()
        {
            xem = _curGo.transform.Find("Xem");           
            _xems = new GameObject[xem.childCount];
            for (int i = 0; i < _xems.Length; i++)
            {
                _xems[i] = xem.GetChild(i).gameObject;
            }

            bkbPanel = _curGo.transform.Find("bkbPanel");
            _bkbPanel = new GameObject[bkbPanel.childCount];
            for (int i = 0; i < _bkbPanel.Length; i++)
            {
                _bkbPanel[i] = bkbPanel.GetChild(i).gameObject;              
            }
            bkbE4r = new Empty4Raycast[bkbPanel.childCount];            
            _ilDrager = bkbPanel.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < bkbE4r.Length; i++)
            {
                bkbE4r[i] = bkbPanel.GetChild(i).GetChild(2).GetComponent<Empty4Raycast>();
                Util.AddBtnClick(bkbE4r[i].gameObject, BkbClickEvent);

                _ilDrager[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                _ilDrager[i].index = i;
            }

            bG_0 = _curGo.transform.Find("BG/bg_0");

            bG = _curGo.transform.Find("BG/bg");
            bg_6 = _curGo.transform.Find("BG/bg/bg6");

            bGStartPos = _curGo.transform.Find("BG/BGStartPos").localPosition;
            bearStartPos = _curGo.transform.Find("BG/bearStartPos").localPosition;
            bearMonStartPos= _curGo.transform.Find("BG/bg/bearMomStartPos").localPosition;
            bearMonEndPo= _curGo.transform.Find("BG/bg/bearMomEndPos").localPosition;
            bg_6StartPos = _curGo.transform.Find("BG/bg/bg6_StartPos").localPosition;
            bg_6EndPos= _curGo.transform.Find("BG/bg/bg6_EndPos").localPosition;

            bGPos = _curGo.transform.Find("BgPos");
            bGEndPos = new Vector3[bGPos.childCount];
            for (int i = 0; i < bGEndPos.Length; i++)
            {
                bGEndPos[i] = bGPos.GetChild(i).transform.localPosition;
            }            
            xfbkBg = _curGo.transform.Find("BG/bg/xfbkBg");
            _xfbkBg = new GameObject[xfbkBg.childCount];
            for (int i = 0; i < _xfbkBg.Length; i++)
            {
                _xfbkBg[i] = xfbkBg.GetChild(i).gameObject;               
            }

            spinePanel = _curGo.transform.Find("spinePanel");
            _spinePanel = new GameObject[spinePanel.childCount];
            for (int i = 0; i < _spinePanel.Length; i++)
            {               
                _spinePanel[i] = spinePanel.GetChild(i).gameObject;
                _spinePanel[i].Hide();
            }

            clickMask = _curGo.transform.Find("clickMask").gameObject;

            _ilDroper = _curGo.transform.Find("Droper/0").GetComponent<ILDroper>();
            _ilDroper.SetDropCallBack(OnAfter);
            _ilDroper.index = 0;

            lockPanel_0 = _curGo.transform.Find("BG/bg/lockPanel/0").gameObject;
            lockDroper = _curGo.transform.Find("BG/bg/lockPanel/1").GetComponent<ILDroper>();
            lockDroper.SetDropCallBack(OnAfter);
            lockDroper.index = 10;
            keyDrager = _spinePanel[5].transform.GetChild(0).GetComponent<ILDrager>();
            keyDrager.SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            keyDrager.index = 10;
            keyDrager.isActived = false;
            bearMom = _curGo.transform.Find("BG/bg/bearMom").gameObject;

            lockPanel_2 = _curGo.transform.Find("BG/bg/lockPanel/2").gameObject;
            lockPanel_3 = _curGo.transform.Find("BG/bg/lockPanel/3").gameObject;

            bG.localPosition = bGStartPos;
            bG_0.localPosition = bGStartPos;
            xem.gameObject.Hide();
            bkbPanel.gameObject.Hide();
            xfbkBg.gameObject.Hide();
            clickMask.Show();
        }
        private void ShowOrHideStart()
        {
            xem.gameObject.Show();
            bG.localPosition = bGStartPos;
            bG_0.localPosition = bGStartPos;
            drageSucceedIndex = -1;
            failIndex = -1;
            bkbSum = 0;
            colorEnum = ColorEnum.Null;
            dragerEnum = DragerEnum.Bkb;

            for (int i = 0; i < _xems.Length; i++)
            {
                _xems[i].Show();
                _xems[i].transform.GetChild(0).gameObject.Hide();
                SpineManager.instance.DoAnimation(_xems[i].transform.GetChild(1).gameObject, "kong", false);
                _xems[i].transform.GetChild(1).gameObject.Hide();
            }

            bkbPanel.gameObject.Show();
            for (int i = 0; i < _bkbPanel.Length; i++)
            {
                _bkbPanel[i].Show();
                SpineManager.instance.DoAnimation(_bkbPanel[i], "kong", false);
                _bkbPanel[i].transform.GetChild(0).gameObject.Show();                
                PlayBkbAni(i);
                _bkbPanel[i].transform.GetChild(1).transform.localPosition = _bkbPanel[i].transform.GetChild(2).transform.localPosition;
                SpineManager.instance.DoAnimation(_bkbPanel[i].transform.GetChild(1).gameObject, "kong", false);
                _bkbPanel[i].transform.GetChild(1).gameObject.Hide();

                bkbE4r[i].gameObject.Show();
                _ilDrager[i].isActived = false;
            }            

            bG.gameObject.Show();
            xfbkBg.gameObject.Show();
            for (int i = 0; i < _xfbkBg.Length; i++)
            {
                _xfbkBg[i].Show();
                _xfbkBg[i].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_xfbkBg[i], "xfbk-" + xfbkName[i], true);                
            }

            spinePanel.gameObject.Show();           
            for (int i = 0; i < _spinePanel.Length; i++)
            {
                _spinePanel[i].Hide();
            }
            _spinePanel[1].Show();
            _spinePanel[1].transform.localPosition = bearStartPos;
            _spinePanel[1].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);

            lockPanel_0.Show();
            SpineManager.instance.DoAnimation(lockPanel_0, "xz", true);

            lockPanel_2.Show();
            SpineManager.instance.DoAnimation(lockPanel_2, "kong", false);
            lockPanel_3.Show();
            SpineManager.instance.DoAnimation(lockPanel_3, "kong", false);

            bearMom.Show();
            bearMom.transform.localPosition = bearMonStartPos;
            SpineManager.instance.DoAnimation(bearMom, "xiong-mon", true);

            lockDroper.transform.GetChild(0).gameObject.Show();
            SpineManager.instance.DoAnimation(lockDroper.transform.GetChild(0).gameObject, "suo", true);
            bg_6.transform.localPosition = bg_6StartPos;

            clickMask.Hide();
                       
        }       
        private void PlayBkbAni(int index)
        {
            if (_bkbPanel[index].name == "0")
            {                
                SpineManager.instance.DoAnimation(_bkbPanel[index].transform.GetChild(0).gameObject, "bk-a", true);
            }
            else if (_bkbPanel[index].name == "1")
            {
                SpineManager.instance.DoAnimation(_bkbPanel[index].transform.GetChild(0).gameObject, "bk-a2", true);
            }
            else if (_bkbPanel[index].name == "2")
            {
                SpineManager.instance.DoAnimation(_bkbPanel[index].transform.GetChild(0).gameObject, "bk-a3", true);
            }
            else if (_bkbPanel[index].name == "3")
            {
                SpineManager.instance.DoAnimation(_bkbPanel[index].transform.GetChild(0).gameObject, "bk-a4", true);
            }
            else if (_bkbPanel[index].name == "4")
            {
                SpineManager.instance.DoAnimation(_bkbPanel[index].transform.GetChild(0).gameObject, "bk-a5", true);
            }
        }
        private void ShowOrHide(bool isShow)
        {
            for (int i = 0; i < bkbE4r.Length; i++)
            {
                bkbE4r[i].gameObject.SetActive(isShow);
            }
        }
        private void ShowXFB(bool isShow)
        {
            for (int i = 0; i < _bkbPanel.Length; i++)
            {
                SpineManager.instance.DoAnimation(_bkbPanel[i], "kong", false);
                _bkbPanel[i].transform.GetChild(0).gameObject.SetActive(isShow);
            }
        }
        private void BkbClickEvent(GameObject go)
        {            
            clickMask.Show();
            ShowOrHide(false);
            go.transform.parent.GetChild(0).gameObject.Hide();           
            go.transform.parent.GetChild(1).gameObject.Show();           
            PlayBgm(4,false);
            if (go.transform.parent.name == "0")
            {                            
                colorEnum = ColorEnum.orange;
                PlayBkbAni(go, "bk-b", 1, "bk-d");

            }
            else if(go.transform.parent.name == "1")
            {
                colorEnum = ColorEnum.blue;
                PlayBkbAni(go, "bk-b2", 1, "bk-d2");               
            }
            else if (go.transform.parent.name == "2")
            {
                colorEnum = ColorEnum.green;
                PlayBkbAni(go, "bk-b3", 1, "bk-d3");
            }
            else if (go.transform.parent.name == "3")
            {
                colorEnum = ColorEnum.yellow;
                PlayBkbAni(go, "bk-b4", 1,"bk-d4");
            }
            else if (go.transform.parent.name == "4")
            {
                colorEnum = ColorEnum.perple;
                PlayBkbAni(go, "bk-b5", 1, "bk-d5");
            }                   
        }
        private void PlayBkbAni(GameObject go,string aniName,int index,string aniName1)
        {              
            SpineManager.instance.DoAnimation(go.transform.parent.GetChild(index).GetChild(0).gameObject, aniName, false, () =>
            {
                clickMask.Hide();
                //parent
                SpineManager.instance.DoAnimation(go.transform.parent.gameObject, aniName1, true);
                //drager
                go.transform.parent.GetChild(index).GetComponent<ILDrager>().isActived = true;
            });
        }       
        private bool OnAfter(int dragType, int index, int dropType)
        {
            droperIndex = index;
            return true;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {           
            if (dragerEnum == DragerEnum.Bkb)
            {
                _ilDrager[index].transform.parent.SetAsLastSibling();
            }           
        }

        private void OnDrag(Vector3 pos, int type, int index){ }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            clickMask.Show();           
            if (isMatch)
            {
                if (dragerEnum == DragerEnum.Bkb)
                {
                    if (droperIndex == 0)
                    {
                        Succeed(ColorEnum.orange, ColorEnum.yellow, index, 1, 0, "xfbk-a2", 4, 0, () => {
                            ShowXFB(true);
                            ShowOrHide(true);
                            clickMask.Hide();
                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);
                            failIndex = -1;
                        });
                    }
                    else if (droperIndex == 1)
                    {
                        Succeed(ColorEnum.yellow, ColorEnum.green, index, 2, 1, "xfbk-b2", 3, 1, () => {
                            ShowXFB(true);
                            ShowOrHide(true);
                            clickMask.Hide();
                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);
                            failIndex = -1;
                        });
                    }
                    else if (droperIndex == 2)
                    {
                        Succeed(ColorEnum.green, ColorEnum.blue, index, 3, 2, "xfbk-c2", 2, 2, () => {
                            ShowXFB(true);
                            ShowOrHide(true);
                            clickMask.Hide();
                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);
                            failIndex = -1;
                        });
                    }
                    else if (droperIndex == 3)
                    {
                        Succeed(ColorEnum.blue, ColorEnum.perple, index, 4, 3, "xfbk-d2", 1, 3, () => {
                            ShowXFB(true);
                            ShowOrHide(true);
                            clickMask.Hide();
                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);
                            failIndex = -1;
                        });
                    }
                    else if (droperIndex == 4)
                    {
                        Succeed(ColorEnum.perple, ColorEnum.orange, index, 0, 4, "xfbk-e2", 0, 4, () =>
                        {
                            ShowXFB(false);
                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);
                            Delay(0.5f, () =>
                            {
                                Delay(0.5f, () => 
                                { 
                                    Tween tw = bG.DOLocalMoveX(bGEndPos[5].x, 0.5f); tw.SetEase(Ease.Linear);
                                    Tween twe = bG_0.DOLocalMoveX(bGEndPos[11].x, 0.5f);twe.SetEase(Ease.Linear);
                                });
                                PlayBgm(3,false);
                                SpineManager.instance.DoAnimation(lockPanel_0, "xz2", false,()=> { SpineManager.instance.DoAnimation(lockPanel_0, "kong", false); });
                                SpineManager.instance.DoAnimation(_spinePanel[1], "xiong2", false, () =>
                                {                                   
                                    _spinePanel[4].Show();
                                 SpineManager.instance.DoAnimation(_spinePanel[4], "ys", false, () =>{_spinePanel[4].Hide();});
                                    Delay(1.0f, () => {
                                        _spinePanel[5].Show();
                                        SpineManager.instance.DoAnimation(keyDrager.transform.GetChild(0).gameObject, "ys3", false);
                                        SpineManager.instance.DoAnimation(_spinePanel[5], "ys2", false, () => {
                                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong3", true);
                                            _spinePanel[0].Show();
                                            Tween tw = _spinePanel[1].transform.DOLocalMove(_spinePanel[0].transform.localPosition, 2); tw.SetEase(Ease.Linear);
                                            Delay(2, () =>
                                            {
                                                dragerEnum = DragerEnum.Lock;
                                                SpineManager.instance.DoAnimation(_spinePanel[1], "xiong", true);
                                                keyDrager.isActived = true;                                               
                                                clickMask.Hide();
                                            });
                                        });
                                    });
                                });
                            });
                        });

                    }
                }
                else if (dragerEnum == DragerEnum.Lock)
                {
                    keyDrager.DoReset();
                    if (index == droperIndex)
                    {
                       
                        SpineManager.instance.DoAnimation(keyDrager.transform.GetChild(0).gameObject, "kong", false);
                        lockPanel_2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        lockPanel_3.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        lockDroper.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(lockPanel_3, "suo3", false);
                        SpineManager.instance.DoAnimation(lockPanel_2, "ys4", false);
                        PlayBgm(7,false);
                        SpineManager.instance.DoAnimation(lockDroper.transform.GetChild(0).gameObject, "suo", false, () => 
                        {
                            Delay(0.5f, () => 
                            {
                                lockPanel_2.Hide();
                                lockPanel_3.Hide();
                                lockDroper.transform.GetChild(0).gameObject.Hide();                               
                                Tween tw = bg_6.transform.DOLocalMoveX(bg_6EndPos.x, 0.5f); tw.SetEase(Ease.Linear);
                                Delay(0.5f, () =>
                                {
                                    SpineManager.instance.DoAnimation(bearMom, "xiong-mon2", true);
                                    Tween twe = bearMom.transform.DOLocalMove(bearMonEndPo, 1f); twe.SetEase(Ease.Linear);
                                    Delay(1, () =>
                                    {
                                        SpineManager.instance.DoAnimation(bearMom, "xiong-mon", true);
                                        Delay(2f, () => { GameSuccess(); });
                                    });
                                });
                            });
                        });
                        
                    }
                    else
                    {                        
                        time_Sound = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                        Delay(time_Sound, () => { clickMask.Hide(); });
                    }
                }
            }
            else
            {
                PlayBgm(5,false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                if (dragerEnum == DragerEnum.Bkb)
                {
                    _ilDrager[index].DoReset();
                    _ilDrager[index].gameObject.Hide();
                    SpineManager.instance.DoAnimation(_ilDrager[index].transform.parent.gameObject, "kong", false);
                    _bkbPanel[index].transform.GetChild(0).gameObject.Show();
                    PlayBkbAni(index);                   
                    PlayFailVoice();
                    time_Sound = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                    Delay(time_Sound, () => { clickMask.Hide(); });
                }
                else if (dragerEnum == DragerEnum.Lock)
                {
                    keyDrager.DoReset();                   
                    time_Sound = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                    Delay(time_Sound, () => { clickMask.Hide(); });
                }

            }
        }
        private float time_Sound;
        private int failIndex;
        private void PlayFailVoice()
        {            
            ShowOrHide(true);
            if (failIndex != -1)
            {
                _ilDrager[failIndex].transform.parent.GetChild(2).gameObject.SetActive(false);                
            }           
        }
        private void Succeed(ColorEnum colorEnum1,ColorEnum colorEnum2,int index,int droperIndex,int xfbkIndex,string xfName,int xemIndx,int endPosIndex,Action callBack = null)
        {
            if (colorEnum == colorEnum1 || colorEnum == colorEnum2)
            {
                bkbSum++;
                ShowOrHide(true);                 
                if (bkbSum == 1)
                {
                    drageSucceedIndex = index;
                    failIndex = index;
                    _ilDrager[drageSucceedIndex].transform.parent.GetChild(2).gameObject.SetActive(false);
                    _ilDrager[drageSucceedIndex].isActived = false;
                    PlayBgm(1,false);
                    time_Sound = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                    Delay(time_Sound, () =>
                    {
                        clickMask.Hide();
                    });
                }                
                //clickMask.Hide();                
                if (bkbSum == 2)
                {                   
                    clickMask.Show();
                    bkbSum = 0;
                    _ilDroper.index = droperIndex;
                    _ilDrager[index].transform.parent.GetChild(2).gameObject.SetActive(false);
                    _spinePanel[3].Show(); //星星
                    SpineManager.instance.DoAnimation(_spinePanel[3], "star", false);
                    _spinePanel[2].Show(); //yes
                    SpineManager.instance.DoAnimation(_spinePanel[2], "yes", false);
                    PlayBgm(2,false);
                    time_Sound = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                    Delay(time_Sound, () =>
                    {
                        _ilDrager[index].DoReset();
                        _ilDrager[index].gameObject.Hide();
                        _ilDrager[drageSucceedIndex].DoReset();
                        _ilDrager[drageSucceedIndex].isActived = true;
                        _ilDrager[drageSucceedIndex].gameObject.Hide();
                        PlayBgm(6,false);
                        _xems[xemIndx].transform.GetChild(0).gameObject.Show();
                        _xems[xemIndx].transform.GetChild(1).gameObject.Show();
                        SpineManager.instance.DoAnimation(_xems[xemIndx].transform.GetChild(1).gameObject, "boom", false, () => { _xems[xemIndx].Hide(); });
                        _spinePanel[2].Hide(); //熊   
                        SpineManager.instance.DoAnimation(_xfbkBg[xfbkIndex], "kong", false);                       
                        SpineManager.instance.DoAnimation(_xfbkBg[xfbkIndex], xfName, false, () =>
                        {
                            Delay(0.5f, () =>
                            {
                                Tween tw = bG.DOLocalMoveX(bGEndPos[endPosIndex].x, 0.5f);
                                tw.SetEase(Ease.Linear);
                                Tween twe = bG_0.DOLocalMoveX(bGEndPos[endPosIndex + 6].x, 0.5f);
                                twe.SetEase(Ease.Linear);
                            });
                            PlayBgm(8,false);
                            SpineManager.instance.DoAnimation(_spinePanel[1], "xiong2", false, callBack);
                        });
                    });                   
                }
            }
            else
            {
                _spinePanel[2].Show(); //yes
                PlayBgm(5, false);
                SpineManager.instance.DoAnimation(_spinePanel[2], "no", false, () => 
                { 
                    _ilDrager[index].DoReset(); _ilDrager[index].gameObject.Hide();
                    SpineManager.instance.DoAnimation(_ilDrager[index].transform.parent.gameObject, "kong", false);
                    _bkbPanel[index].transform.GetChild(0).gameObject.Show();
                    PlayBkbAni(index);
                    Delay(0.2f, () => { clickMask.Hide(); });
                });               
                PlayFailVoice();
                time_Sound = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);                
            }
        }
        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
			
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        //PlayCommonBgm(0);//ToDo...改BmgIndex
                        PlayBgm(0);
                        _startSpine.Hide();

                        //ToDo...
                        _xem.Show();
                        BellSpeck(_xem, 0, null, () =>
                        {
                            _xem.Hide();
                            _sBD.Show();
                            BellSpeck(_sBD, 1, null, () =>{ _sBD.Hide(); StartGame();}, RoleType.Bd);
                        }, RoleType.Xem);
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
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            ShowOrHideStart();
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
                        //PlayCommonBgm(8); //ToDo...改BmgIndex   
                        PlayBgm(0);
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
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);	
                        _dBD.Show(); BellSpeck(_dBD, 2);
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
