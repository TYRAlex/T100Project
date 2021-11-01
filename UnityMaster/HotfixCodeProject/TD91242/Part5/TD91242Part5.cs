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

    public class TD91242Part5
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

        private Transform drag1;
        private Transform drag2;
        private Transform drag3;
        private Transform drag;
        private GameObject _dBD;

        private Transform BG;

        private GameObject _sBD;

        private int index;



        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
        private List<Vector3> RandomPosList;
        private List<GameObject> CanMoveGameObject;
        private Transform POS;
        private Transform Pos1;
        private Transform Pos2;
        private Transform Pos3;
        private Transform Pos4;
        private Transform Pos5;
        private bool Moving;
        private Transform shiban;
        private Transform kou;
        private Transform  xx;
        private GameObject _Mask;
        private bool isstart;
        private int checkindex;
        private Transform xiaoguo;
        private Transform xiaoguo1;
        private Transform allxiaoguo;
        private Transform xem;
        private bool _canClick;
        private Transform MovePos;



        private bool _isPlaying;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _Mask = curTrans.GetGameObject("_mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");

            _dBD = curTrans.GetGameObject("dBD");
            isstart = true;


            _sBD = curTrans.GetGameObject("sBD");
            BG = curTrans.Find("BG");
            checkindex = 0;

            drag1 = curTrans.Find("drag/drag1");
            drag2 = curTrans.Find("drag/drag2");
            drag3 = curTrans.Find("drag/drag3");
            drag = curTrans.Find("drag");
            CanMoveGameObject = new List<GameObject>();
            RandomPosList = new List<Vector3>();
            POS = curTrans.Find("POS");
            Pos1 = curTrans.Find("POS/1");
            Pos2 = curTrans.Find("POS/2");
            Pos3 = curTrans.Find("POS/3");
            Pos4 = curTrans.Find("POS/4");
            Pos5 = curTrans.Find("POS/5");

            shiban = curTrans.Find("shiban");
            kou = curTrans.Find("kou");
            xx = curTrans.Find("xx");
            Moving = false;
            xiaoguo = curTrans.Find("xiaoguo"); xiaoguo1 = curTrans.Find("xiaoguo1");
            index = 0;
            allxiaoguo = curTrans.Find("all");
            xem = curTrans.Find("xem");

            MovePos = curTrans.Find("MovePos");
            _canClick = true;
            GameInit();
            GameStart();
            AddOnClickEvent();
        }

        void InitData()
        {
            _isPlaying = true;

            _Mask.SetActive(false);
            isstart = false;
           xx.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            
            kou.Find("kou1").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            kou.Find("kou2").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            kou.Find("kou3").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            shiban.Find("shiban1").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            shiban.Find("shiban2").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            shiban.Find("shiban3").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            xiaoguo1.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            xiaoguo.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            xem.gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            for (int i = 0; i < drag1.childCount; i++) 
            {
                drag1.GetChild(i).GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            }
            for (int i = 0; i < drag2.childCount; i++)
            {
                drag2.GetChild(i).GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
               
            }
            for (int i = 0; i < drag3.childCount; i++)
            {
                drag3.GetChild(i).GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            }
            for (int i = 0; i < allxiaoguo.childCount; i++)
            {
                allxiaoguo.GetChild(i).GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
              
            }

            SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject,"DaxingxingA1",true);
          
            SpineManager.instance.DoAnimation(kou.Find("kou1").gameObject,"tou",true);
            SpineManager.instance.DoAnimation(kou.Find("kou2").gameObject, "tou", true);
            SpineManager.instance.DoAnimation(kou.Find("kou3").gameObject, "tou", true);
            SpineManager.instance.DoAnimation(shiban.Find("shiban1").gameObject,"shibanA",false);
            SpineManager.instance.DoAnimation(shiban.Find("shiban2").gameObject, "shibanB", false);
            SpineManager.instance.DoAnimation(shiban.Find("shiban3").gameObject, "shibanC", false);
            SpineManager.instance.DoAnimation(xiaoguo.GetChild(0).gameObject,"kong",false);
            SpineManager.instance.DoAnimation(xiaoguo1.GetChild(0).gameObject, "kong", false);
            SpineManager.instance.DoAnimation(xem.gameObject,"xem1",true);

            SpineManager.instance.DoAnimation(drag1.Find("1/a").gameObject, "SKa", false);
            SpineManager.instance.DoAnimation(drag1.Find("2/b").gameObject, "SKb", false);
            SpineManager.instance.DoAnimation(drag1.Find("3/c").gameObject, "SKc", false);
            SpineManager.instance.DoAnimation(drag1.Find("4/d").gameObject, "SKd", false);
            SpineManager.instance.DoAnimation(drag1.Find("5/e").gameObject, "SKe", false);


            SpineManager.instance.DoAnimation(drag2.Find("1/f").gameObject, "SKf", false);
            SpineManager.instance.DoAnimation(drag2.Find("2/g").gameObject, "SKg", false);
            SpineManager.instance.DoAnimation(drag2.Find("3/h").gameObject, "SKh", false);
            SpineManager.instance.DoAnimation(drag2.Find("4/i").gameObject, "SKi", false);
            SpineManager.instance.DoAnimation(drag2.Find("5/j").gameObject, "SKj", false);

            SpineManager.instance.DoAnimation(drag3.Find("1/k").gameObject, "SKk", false);
            SpineManager.instance.DoAnimation(drag3.Find("2/l").gameObject, "SKl", false);
            SpineManager.instance.DoAnimation(drag3.Find("3/m").gameObject, "SKm", false);
            SpineManager.instance.DoAnimation(drag3.Find("4/n").gameObject, "SKn", false);
            SpineManager.instance.DoAnimation(drag3.Find("5/o").gameObject, "SKo", false);
            SpineManager.instance.DoAnimation(BG.Find("dd").gameObject, "dingding", true);

            drag1.Find("1").GetRectTransform().anchoredPosition = new Vector2(15,140f);
            drag1.Find("2").GetRectTransform().anchoredPosition = new Vector2(-387, 140f);
            drag1.Find("3").GetRectTransform().anchoredPosition = new Vector2(-188, 140f);
            drag1.Find("4").GetRectTransform().anchoredPosition = new Vector2(415, 140f);
            drag1.Find("5").GetRectTransform().anchoredPosition = new Vector2(217, 140f);

            drag2.Find("1").GetRectTransform().anchoredPosition = new Vector2(217, 140f);
            drag2.Find("2").GetRectTransform().anchoredPosition = new Vector2(-188, 140f);
            drag2.Find("3").GetRectTransform().anchoredPosition = new Vector2(15, 140f);
            drag2.Find("4").GetRectTransform().anchoredPosition = new Vector2(-387, 140f);
            drag2.Find("5").GetRectTransform().anchoredPosition = new Vector2(415, 140f);

            drag3.Find("1").GetRectTransform().anchoredPosition = new Vector2(-387, 140f);
            drag3.Find("2").GetRectTransform().anchoredPosition = new Vector2(15, 140f);
            drag3.Find("3").GetRectTransform().anchoredPosition = new Vector2(217, 140f);
            drag3.Find("4").GetRectTransform().anchoredPosition = new Vector2(415, 140f);
            drag3.Find("5").GetRectTransform().anchoredPosition = new Vector2(-188, 140f);
            for (int i = 0; i < allxiaoguo.childCount; i++) 
            {
                SpineManager.instance.DoAnimation(allxiaoguo.GetChild(i).GetChild(0).gameObject,"kong",false);
            }
            xx.transform.GetRectTransform().anchoredPosition = new Vector2(104,611);
            drag1.gameObject.SetActive(true); drag2.gameObject.SetActive(false); drag3.gameObject.SetActive(false);
            //RandomPos(drag1);
            CanMoveGameObject.Clear();










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



            _sBD.Hide();








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
                     //   _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,_sBD,()=> { _sBD.SetActive(true); },null,RoleType.Bd));
                       _sBD.Show();
                        BellSpeck(_sBD, 0, null,ShowVoiceBtn);
                        //ToDo...
                      

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
                    BellSpeck(_sBD, 1, null, () => 
                    {
                        _sBD.SetActive(false);
                        StartGame();
                    });
                    break;
            }
            _talkIndex++;
        }

        private void AddOnClickEvent() 
        {
            //Util.AddBtnClick(drag1.Find("1").gameObject,OnClick1);
            //Util.AddBtnClick(drag1.Find("2").gameObject, OnClick1);
            //Util.AddBtnClick(drag1.Find("3").gameObject, OnClick1);
            //Util.AddBtnClick(drag1.Find("4").gameObject, OnClick1);
            //Util.AddBtnClick(drag1.Find("5").gameObject, OnClick1);
            for (int i = 1; i <= drag1.childCount; i++) 
            {
                string a = i.ToString();
                Util.AddBtnClick(drag1.Find(a).gameObject, OnClick1);
            }
            for (int i = 1; i <= drag2.childCount; i++)
            {
                string a = i.ToString();
                Util.AddBtnClick(drag2.Find(a).gameObject, OnClick1);
            }
            for (int i = 1; i <= drag3.childCount; i++)
            {
                string a = i.ToString();
                Util.AddBtnClick(drag3.Find(a).gameObject, OnClick1);
            }
        }
        private void OnClick1(GameObject obj) 
        {
            if (_canClick) { 
            if (index < 2) 
            {
              //  index++;
              //  drag.GetChild(obj.transform.parent.GetSiblingIndex()).GetChild(int.Parse(obj.name) - 1).gameObject.GetComponent<Empty4Raycast>().raycastTarget = false;
               // obj.GetComponent<Empty4Raycast>().raycastTarget = false;
                //if (index == 1)
                //{
                //    xiaoguo.transform.position = obj.transform.position;
                //    SpineManager.instance.DoAnimation(xiaoguo.GetChild(0).gameObject, "guangxiao", false);
                //}
                //else if (index == 2)
                //{
                //    xiaoguo1.transform.position = obj.transform.position;
                //    SpineManager.instance.DoAnimation(xiaoguo1.GetChild(0).gameObject, "guangxiao", false);
                //}

                PlayOnClickSound();
               // SpineManager.instance.DoAnimation(drag.GetChild(obj.transform.parent.GetSiblingIndex()).GetChild(int.Parse(obj.name) - 1).GetChild(0).gameObject, "SK" + (obj.transform.GetChild(0).name).ToString() + "2", false, () =>
            //    {
                    if ((CanMoveGameObject.Count < 2))
                    {
                        if (!CanMoveGameObject.Contains(obj))
                        {
                            SpineManager.instance.DoAnimation(drag.GetChild(obj.transform.parent.GetSiblingIndex()).GetChild(int.Parse(obj.name) - 1).GetChild(0).gameObject, "SK" + (obj.transform.GetChild(0).name).ToString() + "2", false);
                            CanMoveGameObject.Add(obj);
                        xiaoguo.transform.position = obj.transform.position;
                        _canClick = false;
                        SpineManager.instance.DoAnimation(xiaoguo.GetChild(0).gameObject, "guangxiao", false,()=> 
                        { 
                            _canClick = true;
                           
                         
                        });

                        index++;
                        }
                        else if(CanMoveGameObject.Contains(obj))
                        {
                        CanMoveGameObject.Remove(obj);
                        xiaoguo.transform.position = obj.transform.position;
                        _canClick = false;
                        SpineManager.instance.DoAnimation(xiaoguo.GetChild(0).gameObject, "guangxiao", false,()=> { _canClick = true; });
                        index--;
                     
                        SpineManager.instance.DoAnimation(drag.GetChild(obj.transform.parent.GetSiblingIndex()).GetChild(int.Parse(obj.name) - 1).GetChild(0).gameObject, "SK" + (obj.transform.GetChild(0).name).ToString(), false);
                        }
                    } ChangePos(obj.transform.parent);
                

             
            }
          
        }
        }
      
        #region 游戏逻辑

        private void ChangePos(Transform obj) 
        {
            Delay(1f, () =>
            {

                if (CanMoveGameObject.Count == 2)
                {
                    index = 0;
                    Vector3 a = CanMoveGameObject[0].transform.GetRectTransform().anchoredPosition;
                    Vector3 b = CanMoveGameObject[1].transform.GetRectTransform().anchoredPosition;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);


                    CanMoveGameObject[0].transform.GetRectTransform().anchoredPosition = b;
                    CanMoveGameObject[1].transform.GetRectTransform().anchoredPosition = a;

                    CanMoveGameObject[0].GetComponent<Empty4Raycast>().raycastTarget = true;
                    CanMoveGameObject[1].GetComponent<Empty4Raycast>().raycastTarget = true;
                 
                    SpineManager.instance.DoAnimation(CanMoveGameObject[0].transform.GetChild(0).gameObject, "SK" + CanMoveGameObject[0].transform.GetChild(0).name.ToString() + "4", false);
                    SpineManager.instance.DoAnimation(CanMoveGameObject[1].transform.GetChild(0).gameObject, "SK" + CanMoveGameObject[1].transform.GetChild(0).name.ToString() + "4", false, () =>
                    {

                    });

                    CanMoveGameObject.Clear();
                    if (checkPos(obj) && obj == drag1)
                    {
                        //下一关
                        ShowXiao();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);
                        _Mask.SetActive(true);
                        Delay(2.5f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                            SpineManager.instance.DoAnimation(shiban.GetChild(0).gameObject, "shibanA2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(kou.GetChild(0).gameObject,"tou2",true);
                              //  SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                                SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "Daxingxing2", false,()=> 
                                {
                                    SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "DaxingxingA2", true);
                                    Delay(2.2f, () =>
                                    {
                                        // SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "Daxingxing", true);
                                        drag1.gameObject.SetActive(false);
                                        drag2.gameObject.SetActive(true);
                                        _Mask.SetActive(false);
                                      //  RandomPos(drag2);
                                    });
                                });
                                
                             
                            });

                        });
                    }
                    else if (checkPos(obj) && obj == drag2)
                    {
                        ShowXiao(); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                        _Mask.SetActive(true);
                        Delay(2.5f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);
                            SpineManager.instance.DoAnimation(shiban.GetChild(1).gameObject, "shibanB2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(kou.GetChild(1).gameObject, "tou2", true);
                                //     SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                                SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "Daxingxing3", false,()=> 
                                {
                                    Delay(2.15f, () =>
                                    {
                                        //  SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "Daxingxing", true);
                                        drag2.gameObject.SetActive(false);
                                        drag3.gameObject.SetActive(true);
                                        _Mask.SetActive(false);
                                      //  RandomPos(drag3);
                                    });
                                    SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "DaxingxingA3",true);
                                });
                               
                            });
                        });


                    }
                    else if (checkPos(obj) && obj == drag3)
                    {
                        ShowXiao(); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);
                        _Mask.SetActive(true);
                        Delay(2.5f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                            SpineManager.instance.DoAnimation(shiban.GetChild(2).gameObject, "shibanC2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(kou.GetChild(2).gameObject, "tou2", true);
                                //  SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                                SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "Daxingxing4", false,()=>
                                {
                                    Delay(4f, () => { GameSuccess(); });
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                    SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "Daxingxing5", false, () =>
                                    {
                                        SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "DaxingxingA4",false,()=> 
                                        {

                                        });
                                   
                                        SpineManager.instance.DoAnimation(xem.gameObject, "xem-y", false);


                                    });
                                    //SpineManager.instance.DoAnimation(xx.GetChild(0).gameObject, "DaxingxingA4",false,()=> 
                                    //{
                                       
                                    //});
                                });
                             
                            });
                        });
                    }
                }
            });
           
         
       
            
         
        }
        private bool checkPos(Transform obj) 
        {
            if (obj == drag1)
            {
                if (obj.Find("1").GetRectTransform().anchoredPosition == Pos1.GetRectTransform().anchoredPosition
               && obj.Find("2").GetRectTransform().anchoredPosition == Pos2.GetRectTransform().anchoredPosition
               && obj.Find("3").GetRectTransform().anchoredPosition == Pos3.GetRectTransform().anchoredPosition
               && obj.Find("4").GetRectTransform().anchoredPosition == Pos4.GetRectTransform().anchoredPosition
               && obj.Find("5").GetRectTransform().anchoredPosition == Pos5.GetRectTransform().anchoredPosition)
                {
                   
                    return true;
                }
            }
            else if (obj == drag2)
            {
                if (obj.Find("1").transform.position == Pos1.transform.position
               && obj.Find("2").transform.position == Pos2.transform.position
               && obj.Find("3").transform.position == Pos3.transform.position
               && obj.Find("4").transform.position == Pos4.transform.position
               && obj.Find("5").transform.position == Pos5.transform.position)
                {
                 
                    return true;
                }
            }
            else if (obj == drag3) 
            {
                if (obj.Find("1").transform.position == Pos1.transform.position
              && obj.Find("2").transform.position == Pos2.transform.position
              && obj.Find("3").transform.position == Pos3.transform.position
              && obj.Find("4").transform.position == Pos4.transform.position
              && obj.Find("5").transform.position == Pos5.transform.position)
                {
             ;
                    return true;
                }
            }
            return false;
        }

        private void RandomPos(Transform obj) 
        {
            for (int i = 0; i < POS.childCount; i++)
            {
                RandomPosList.Add(POS.GetChild(i).transform.position);
            }

            for (int i=0;i<obj.childCount;i++) 
            {
                int randomindex = Random.Range(0,RandomPosList.Count);
                obj.GetChild(i).transform.position = RandomPosList[randomindex];
                RandomPosList.Remove(RandomPosList[randomindex]);
            }
        }
   

     
        
        










        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
          
           
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
                        InitData();
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
                        _dBD.Show(); BellSpeck(_dBD, 3);
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
        private void ShowXiao() 
        {
            for (int i = 0; i < allxiaoguo.childCount; i++) 
            {
                SpineManager.instance.DoAnimation(allxiaoguo.GetChild(i).GetChild(0).gameObject,"guangxiao",false);
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

        private void UpDate(bool isstart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isstart, delay, callBack));
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
                isStart = isstart;
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
                if (isStart == false) 
                {
                    break;
                }
             
            }
        }
        IEnumerator IEUpdate1(bool isStart, float delay, Action callBack)
        {


            while (isStart)
            {
                isStart = isstart;
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
                if (checkindex == 0)
                {
                    if (xx.transform.GetRectTransform().anchoredPosition == new Vector2(-608, 56))
                    {
                        checkindex++;
                        break;
                       
                    }
                }
                else if (checkindex==1)
                {
                    if (xx.transform.GetRectTransform().anchoredPosition == new Vector2(-258, 61))
                    {
                        checkindex++;
                        break;
                    
                    }
                }
              
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
