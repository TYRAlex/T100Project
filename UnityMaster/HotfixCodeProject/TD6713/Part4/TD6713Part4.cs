using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD6713Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject anyBtn;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private Transform QiPaoTotalOne;
        private Transform QiPaoTotalTwo;
        private Transform QiPaoTotalThree;
        private Transform QiPaoTotalFour;
        private Transform Fire_moutain1;
        private Transform Fire_moutain2;
        private Transform Fire_moutain3;
        private Transform Fire_moutain4;
        private Transform Cold_moutain1;
        private Transform Cold_moutain2;
        private Transform Cold_moutain3;
        private Transform Cold_moutain4;
        private Transform BgLevel1;
        private Transform BgLevel2;
        private Transform BgLevel3;
        private Transform BgLevel4;
        private Transform MainScene;
        private Transform spin;
        private Transform Main;
        private int index1;
        private Transform Next;
        private int NextBtnEventindex;
        private bool _canClick;
        //结束相关
        private Transform sijifaguang;
        private Transform cw;
        private Transform ok;
        private Transform endingSpine;
        private Transform play;
        //用于情景对话
        private GameObject buDing;
        private GameObject buDing2;
        private Text bdText;
        private Text bdText2;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
        /// <summary>
        /// 拖拽数组s
        /// </summary>
        private List<ILDrager> _ILDrager1;
        private List<ILDrager> _ILDrager2;
        private List<ILDrager> _ILDrager3;
        private List<ILDrager> _ILDrager4;


        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
       // private GameObject buDing;
        //private Text bdText;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            //mask.SetActive(false);
            //用于情景对话
            buDing = curTrans.Find("mask/buDing").gameObject;
            buDing2 = curTrans.Find("mask/buDing2").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
           // bdText2 = buDing2.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");


            bd = curTrans.Find("mask/BD").gameObject;
           // bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtn = curTrans.Find("mask/Btn").gameObject;
            anyBtn.SetActive(false);
            //赋值
            textSpeed = 0.1f;
            Main = curTrans.Find("Main");
            QiPaoTotalOne = curTrans.Find("BgLevel1/QiPaoTotal");
            QiPaoTotalTwo = curTrans.Find("BgLevel2/QiPaoTotal");
            QiPaoTotalThree = curTrans.Find("BgLevel3/QiPaoTotal");
            QiPaoTotalFour= curTrans.Find("BgLevel4/QiPaoTotal");
            Fire_moutain1 = curTrans.Find("BgLevel1/Fire");
            Fire_moutain2 = curTrans.Find("BgLevel2/Fire");
            Fire_moutain3 = curTrans.Find("BgLevel3/Fire");
            Fire_moutain4 = curTrans.Find("BgLevel4/Fire");
            Cold_moutain1 = curTrans.Find("BgLevel1/Cold");
            Cold_moutain2 = curTrans.Find("BgLevel2/Cold");
            Cold_moutain3 = curTrans.Find("BgLevel3/Cold");
            Cold_moutain4 = curTrans.Find("BgLevel4/Cold");
            BgLevel1 = curTrans.Find("BgLevel1");
            BgLevel2 = curTrans.Find("BgLevel2");
            BgLevel3 = curTrans.Find("BgLevel3");
            BgLevel4 = curTrans.Find("BgLevel4");
            MainScene = curTrans.Find("Main");
            spin = curTrans.Find("Main/spin");
            index1 = 0;
            Next = curTrans.Find("mask/Next");
            NextBtnEventindex = 0;
            sijifaguang = curTrans.Find("mask/sijifaguang");
            endingSpine = curTrans.Find("mask/endingspine");
            cw = curTrans.Find("mask/cw");
            Util.AddBtnClick(cw.gameObject, OnClickRePlayBtn);
            ok = curTrans.Find("mask/ok");
            Util.AddBtnClick(ok.gameObject, OnClickOkBtn);
            play = curTrans.Find("mask/Play");
            Util.AddBtnClick(play.gameObject, PlayOnClickToBf);
            _canClick = true;


            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Util.AddBtnClick(Next.gameObject, NextBtnClick);//下一关按钮 

            InitDrager();
            GameInit();
            GameStart();
            AddIDragersEvent();
        }
        private void NextBtnClick(GameObject obj) 
        {
            SpineManager.instance.DoAnimation(Next.GetChild(0).gameObject, "next", false,()=> 
            {
                 if (NextBtnEventindex == 1) 
            {
                mask.SetActive(false);
                BgLevel1.gameObject.SetActive(false);
                MainScene.gameObject.SetActive(true);
                //"YLFG_LC5-1_wujiaoxin_guangxiao2",
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(spin.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
              
               
                // SpineManager.instance.DoAnimation(spin.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                SpineManager.instance.DoAnimation(spin.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false,() =>
                {
                    SpineManager.instance.DoAnimation(spin.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                    //  SpineManager.instance.DoAnimation(spin.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                    SpineManager.instance.DoAnimation(spin.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(spin.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                        //  SpineManager.instance.DoAnimation(spin.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                        SpineManager.instance.DoAnimation(spin.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false, () =>
                        {
                      
                           // SpineManager.instance.DoAnimation(spin.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                            SpineManager.instance.DoAnimation(spin.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_xia", false, () =>
                            {
                                //对Main进行移动
                                Main.transform.DOLocalMove(new Vector3(-30f, -760.83f, 0f), 1.0f);
                                mono.StartCoroutine(IEDelay(1.5f,()=> 
                                {
                                    BgLevel2.gameObject.SetActive(true);
                                    SpineManager.instance.DoAnimation(Next.GetChild(0).gameObject, "next2");
                                    Main.gameObject.SetActive(false);
                                }));
                            });
                        });
                    });
                });
            }
            if (NextBtnEventindex == 2) 
            {
                mask.SetActive(false);
                BgLevel2.gameObject.SetActive(false);
                MainScene.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

               
            
         
                // SpineManager.instance.DoAnimation(spin.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                SpineManager.instance.DoAnimation(spin.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                SpineManager.instance.DoAnimation(spin.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false,()=> 
                {
                    // SpineManager.instance.DoAnimation(spin.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                    SpineManager.instance.DoAnimation(spin.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                    SpineManager.instance.DoAnimation(spin.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false,()=> 
                    {
                        //   SpineManager.instance.DoAnimation(spin.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                        SpineManager.instance.DoAnimation(spin.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                        SpineManager.instance.DoAnimation(spin.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false,()=> 
                        {
                            //SpineManager.instance.DoAnimation(spin.GetChild(8).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                            SpineManager.instance.DoAnimation(spin.GetChild(8).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_qiu", false,()=> 
                            {
                                //对Main进行移动
                                Main.transform.DOLocalMove(new Vector3(-30f, -890f, 0f), 1.0f);
                                mono.StartCoroutine(IEDelay(1.5f, () =>
                                {
                                    BgLevel3.gameObject.SetActive(true);
                                    SpineManager.instance.DoAnimation(Next.GetChild(0).gameObject, "next2");
                                    Main.gameObject.SetActive(false);
                                }));
                            });
                        });
                    });
                });
            }
            if (NextBtnEventindex == 3) 
            {

                mask.SetActive(false);
                BgLevel3.gameObject.SetActive(false);
                MainScene.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                //  SpineManager.instance.DoAnimation(spin.GetChild(9).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                SpineManager.instance.DoAnimation(spin.GetChild(9).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                SpineManager.instance.DoAnimation(spin.GetChild(9).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false, () =>
                {
                    //  SpineManager.instance.DoAnimation(spin.GetChild(10).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                    SpineManager.instance.DoAnimation(spin.GetChild(10).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                    SpineManager.instance.DoAnimation(spin.GetChild(10).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false, () =>
                    {
                        //   SpineManager.instance.DoAnimation(spin.GetChild(11).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                        SpineManager.instance.DoAnimation(spin.GetChild(11).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
                        SpineManager.instance.DoAnimation(spin.GetChild(11).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao2", false, () =>
                        {
                          //  SpineManager.instance.DoAnimation(spin.GetChild(12).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", true);
                            SpineManager.instance.DoAnimation(spin.GetChild(12).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_dong", false, () =>
                            {
                                //对Main进行移动
                                //Main.transform.DOLocalMove(new Vector3(-75.6f, -1062.72f, 0f), 1.0f);
                                mono.StartCoroutine(IEDelay(1.5f, () =>
                                {
                                    BgLevel4.gameObject.SetActive(true);
                                    SpineManager.instance.DoAnimation(Next.GetChild(0).gameObject, "next2");
                                    Main.gameObject.SetActive(false);
                                }));
                               
                            });
                        });
                    });
                });
            }
            if (NextBtnEventindex == 4) 
            {
               //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                BgLevel4.gameObject.SetActive(false);
                mask.SetActive(false);
                Main.gameObject.SetActive(true);
                NextBtnEventindex = 0;
                mono.StartCoroutine(IEDelay(1.5f, () =>
                 {
                     mask.SetActive(true);
                     Next.gameObject.SetActive(false);
                     sijifaguang.gameObject.SetActive(true);


                     //    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                     SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 11, false);
                     SpineManager.instance.DoAnimation(sijifaguang.transform.gameObject, "YLFG_LC5-1_4JiTongGuang", false,()=> 
                     {
                        // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                         sijifaguang.gameObject.SetActive(false);
                         //SpineManager.instance.DoAnimation(sijifaguang.transform.gameObject, "YLFG_LC5-1_4JiTongGuang", false);
                         playSuccessSpine(()=> 
                         {
                             cw.gameObject.SetActive(true); ok.gameObject.SetActive(true);
                             SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh2", true); 
                             SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok2", true);

                         });
                     });

                 }));
            }
            });
           
           
           
        }
        void InitDrager() 
        {
            _ILDrager1 = new List<ILDrager>();

            for (int i =0;i<QiPaoTotalOne.childCount;i++) 
            {
                var iLDrager = QiPaoTotalOne.GetChild(i).GetComponent<ILDrager>();
                _ILDrager1.Add(iLDrager);
            }
            _ILDrager2 = new List<ILDrager>();

            for (int i = 0; i < QiPaoTotalTwo.childCount; i++)
            {
                var iLDrager = QiPaoTotalTwo.GetChild(i).GetComponent<ILDrager>();
                _ILDrager2.Add(iLDrager);
            }
            _ILDrager3 = new List<ILDrager>();

            for (int i = 0; i < QiPaoTotalThree.childCount; i++)
            {
                var iLDrager = QiPaoTotalThree.GetChild(i).GetComponent<ILDrager>();
                _ILDrager3.Add(iLDrager);
            }
            _ILDrager4 = new List<ILDrager>();

            for (int i = 0; i < QiPaoTotalFour.childCount; i++)
            {
                var iLDrager = QiPaoTotalFour.GetChild(i).GetComponent<ILDrager>();
                _ILDrager4.Add(iLDrager);
            }
        }
        private void AddIDragersEvent() 
        {
            foreach (var drager in _ILDrager1) 
            {
                drager.SetDragCallback(DragStart, Draging, DragEnd);
            }
            foreach (var drager in _ILDrager2)
            {
                drager.SetDragCallback(DragStart, Draging, DragEnd2);
            }
            foreach (var drager in _ILDrager3)
            {
                drager.SetDragCallback(DragStart, Draging, DragEnd3);
            }
            foreach (var drager in _ILDrager4)
            {
                drager.SetDragCallback(DragStart, Draging, DragEnd4);
            }
        }
        private void DragStart(Vector3 position, int type, int index)
        {

        }
        private void Draging(Vector3 position, int type, int index)
        {

        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            // mono.StartCoroutine((SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, ()=>{}, () => { _canClick=true;})));

            {
                if (_canClick&&isMatch && type == 1)
                {
                    _canClick = false;
                    //正确语音
                    mono.StartCoroutine((randomPlayVoiceRight(() =>
                    {
                        _canClick = false;
                        InitActive1();
                        SpineManager.instance.DoAnimation(Fire_moutain1.GetChild(0).gameObject, "YLFG_LC5-1_shan_nuan se2", false);
                        if (index == 0)
                        {
                            QiPaoTotalOne.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = false;
                            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                            index1++;
                        }
                        if (index == 3)
                        {
                            QiPaoTotalOne.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = false;
                            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            index1++;
                        }
                        if (index1 == 5)
                        {
                            mono.StartCoroutine(IEDelay(2, () =>
                            {
                                mask.SetActive(true);
                                NextBtnEventindex++;
                                Next.gameObject.SetActive(true);
                                index1 = 0;
                                //初始化
                                //for (int i = 0; i < QiPaoTotalOne.childCount; i++)
                                //{

                                //    _ILDrager1[i].DoReset();
                                //    SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                                //    QiPaoTotalOne.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                                //}


                            }));
                        }
                    }, () =>
                    {
                        InitActive2();
                        _canClick = true;

                    })));
                    
                   
                }
                else if (_canClick&&isMatch && type == 2)
                {
                    _canClick = false;
                    //正确语音
                    mono.StartCoroutine((randomPlayVoiceRight(()=> 
                    {
                        InitActive1();
                        SpineManager.instance.DoAnimation(Cold_moutain1.GetChild(0).gameObject, "YLFG_LC5-1_shan_leng se2", false);
                        if (index == 1)
                        {
                            QiPaoTotalOne.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = false;
                            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            index1++;
                        }
                        if (index == 2)
                        {
                            QiPaoTotalOne.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = false;
                            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            index1++;
                        }
                        if (index == 4)
                        {
                            QiPaoTotalOne.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = false;
                            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            index1++;
                        }
                        if (index1 == 5)
                        {
                            mono.StartCoroutine(IEDelay(2, () =>
                            {
                                mask.SetActive(true);
                                NextBtnEventindex++;
                                Next.gameObject.SetActive(true);
                                index1 = 0;
                                //初始化
                                //for (int i = 0; i < QiPaoTotalOne.childCount; i++)
                                //{

                                //    _ILDrager1[i].DoReset();
                                //    SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                                //    QiPaoTotalOne.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                                //}


                            }));
                        }
                    }
                    , () =>
                    {
                        InitActive2();
                        _canClick = true;
                      
                    })));

                }
                else
                {
                    //错误语音
                    mono.StartCoroutine((randomPlayVoiceFalse(()=> 
                    {
                        InitActive1();
                        _canClick = false;
                        _ILDrager1[index].DoReset();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    }, () =>
                    {
                        InitActive2();
                        _canClick = true;
                    })));

                }

             



            }
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch) 
        {
            Debug.Log("2");
            if (_canClick&&isMatch && type == 1)
            {
                _canClick = false;
                mono.StartCoroutine((randomPlayVoiceRight(()=>
                {
                    InitActive1();
                    SpineManager.instance.DoAnimation(Fire_moutain2.GetChild(0).gameObject, "YLFG_LC5-1_shan_nuan se2", false);
                    if (index == 4)
                    {
                        QiPaoTotalTwo.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 5)
                    {
                        QiPaoTotalTwo.GetChild(5).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index1 == 6)
                    {
                        mono.StartCoroutine(IEDelay(2, () =>
                        {
                            index1 = 0;
                            NextBtnEventindex++;
                            mask.SetActive(true);
                            Next.gameObject.SetActive(true);
                            //初始化
                            //for (int i = 0; i < QiPaoTotalTwo.childCount; i++)
                            //{

                            //    _ILDrager2[i].DoReset();
                            //    SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                            //    QiPaoTotalTwo.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                            //}
                        }));
                    }
                }, () => 
                {
                    InitActive2();
                    _canClick = true;
                })));
                
             

            }
            else if (_canClick&&isMatch && type == 2)
            {
                _canClick = false;
                mono.StartCoroutine((randomPlayVoiceRight(()=> 
                {
                    InitActive1();
                    SpineManager.instance.DoAnimation(Cold_moutain2.GetChild(0).gameObject, "YLFG_LC5-1_shan_leng se2", false);
                    if (index == 0)
                    {
                        QiPaoTotalTwo.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 1)
                    {
                        QiPaoTotalTwo.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 2)
                    {
                        QiPaoTotalTwo.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 3)
                    {
                        QiPaoTotalTwo.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index1 == 6)
                    {
                        mono.StartCoroutine(IEDelay(2, () =>
                        {
                            index1 = 0;
                            NextBtnEventindex++;
                            mask.SetActive(true);
                            Next.gameObject.SetActive(true);
                            //初始化
                            //for (int i = 0; i < QiPaoTotalTwo.childCount; i++)
                            //{

                            //    _ILDrager2[i].DoReset();
                            //    SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                            //    QiPaoTotalTwo.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                            //}
                        }));
                    }
                }, () => 
                {
                    InitActive2();
                    _canClick = true;
                })));


              
            }
            else
            {
                mono.StartCoroutine((randomPlayVoiceFalse(()=> 
                {
                    _canClick = false;
                    InitActive1();
                    _ILDrager2[index].DoReset();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                }, () =>
                {
                    InitActive2();
                    _canClick = true;
                })));
               
            }
          
            
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log("3");
            if (_canClick&&isMatch && type == 1)
            {
                _canClick = false;
                mono.StartCoroutine((randomPlayVoiceRight(() => 
                {
                    InitActive1();
                    SpineManager.instance.DoAnimation(Fire_moutain3.GetChild(0).gameObject, "YLFG_LC5-1_shan_nuan se2", false);
                    if (index == 0)
                    {
                        QiPaoTotalThree.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 1)
                    {
                        QiPaoTotalThree.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 2)
                    {
                        QiPaoTotalThree.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 5)
                    {
                        QiPaoTotalThree.GetChild(5).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index1 == 7)
                    {
                        mono.StartCoroutine(IEDelay(2, () =>
                        {
                            index1 = 0;
                            NextBtnEventindex++;
                            mask.SetActive(true);
                            Next.gameObject.SetActive(true);
                            //for (int i = 0; i < QiPaoTotalThree.childCount; i++)
                            //{

                            //    _ILDrager3[i].DoReset();
                            //    SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                            //    QiPaoTotalThree.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                            //}
                        }));
                    }
                }, () => 
                {
                    InitActive2();
                    _canClick = true;
                })));
              

            }
            else if (_canClick&&isMatch && type == 2)
            {
                _canClick = false;
                mono.StartCoroutine((randomPlayVoiceRight(() => 
                {
                    InitActive1();
                    SpineManager.instance.DoAnimation(Cold_moutain3.GetChild(0).gameObject, "YLFG_LC5-1_shan_leng se2", false);
                    if (index == 3)
                    {
                        QiPaoTotalThree.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 4)
                    {
                        QiPaoTotalThree.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 6)
                    {
                        QiPaoTotalThree.GetChild(6).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index1 == 7)
                    {
                        mono.StartCoroutine(IEDelay(2, () =>
                        {
                            index1 = 0;
                            NextBtnEventindex++;
                            mask.SetActive(true);
                            Next.gameObject.SetActive(true);
                            //for (int i = 0; i < QiPaoTotalThree.childCount; i++)
                            //{

                            //    _ILDrager3[i].DoReset();
                            //    SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                            //    QiPaoTotalThree.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                            //}
                        }));
                    }
                }, () => 
                {
                    InitActive2();
                    _canClick = true;
                    })));
              
            }
            else
            {
                // mono.StartCoroutine((SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, ()=>{}, () => { _canClick=true;})));
                mono.StartCoroutine((randomPlayVoiceFalse(() => 
                {
                    InitActive1();
                    _canClick = false;
                    _ILDrager3[index].DoReset();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                }, () =>
                { 
                    _canClick = true;
                    InitActive2();
                })));
             
            }
        

        }
        private void DragEnd4(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log("4");

            if (_canClick&&isMatch && type == 1)
            {
                _canClick = false;
                mono.StartCoroutine((randomPlayVoiceRight(() => 
                {
                    InitActive1();
                    SpineManager.instance.DoAnimation(Fire_moutain4.GetChild(0).gameObject, "YLFG_LC5-1_shan_nuan se2", false);
                    if (index == 1)
                    {
                        QiPaoTotalFour.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 3)
                    {
                        QiPaoTotalFour.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 6)
                    {
                        QiPaoTotalFour.GetChild(6).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 7)
                    {
                        QiPaoTotalFour.GetChild(7).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index1 == 8)
                    {
                        mono.StartCoroutine(IEDelay(2, () =>
                        {
                            index1 = 0;
                            NextBtnEventindex++;
                            mask.SetActive(true);
                            Next.gameObject.SetActive(true);
                            //for (int i = 0; i < QiPaoTotalFour.childCount; i++)
                            //{

                            //    _ILDrager4[i].DoReset();
                            //    SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                            //    QiPaoTotalFour.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                            //}
                        }));
                    }
                }, () => { _canClick = true; InitActive2(); })));
               

            }
            else if (_canClick&&isMatch && type == 2)
            {
                _canClick = false;
                mono.StartCoroutine((randomPlayVoiceRight(() => 
                {
                    InitActive1();
                    SpineManager.instance.DoAnimation(Cold_moutain4.GetChild(0).gameObject, "YLFG_LC5-1_shan_leng se2", false);
                    if (index == 0)
                    {
                        QiPaoTotalFour.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 2)
                    {
                        QiPaoTotalFour.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 4)
                    {
                        QiPaoTotalFour.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index == 5)
                    {
                        QiPaoTotalFour.GetChild(5).gameObject.GetComponent<ILDrager>().isActived = false;
                        SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_qi pao bao zha", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        index1++;
                    }
                    if (index1 == 8)
                    {
                        mono.StartCoroutine(IEDelay(2, () =>
                        {
                            index1 = 0;
                            NextBtnEventindex++;
                            mask.SetActive(true);
                            Next.gameObject.SetActive(true);
                            //for (int i = 0; i < QiPaoTotalFour.childCount; i++)
                            //{

                            //    _ILDrager4[i].DoReset();
                            //    SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(i).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
                            //    QiPaoTotalFour.GetChild(i).gameObject.GetComponent<ILDrager>().isActived = true;
                            //}
                        }));
                    }
                }, () => { _canClick = true;
                    InitActive2();
                })));
               

            }
            else
            {
                mono.StartCoroutine((randomPlayVoiceFalse(() => 
                {
                    _canClick = false;
                    InitActive1();
                    _ILDrager4[index].DoReset();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                }, () => { _canClick = true;
                    InitActive2();
                })));
               
            }
           
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
        
        private void OnClickRePlayBtn(GameObject obj)//重玩按钮
        {
           

           SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh", false, () =>
            {
                obj.SetActive(false);
                
                SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok", false);
                ok.gameObject.SetActive(false);
               // SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh2");
               // SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok2");
                mask.SetActive(false);
                RePlayInit();
                InitSpine();
               // GameInit();
            });
        }
        private void OnClickOkBtn(GameObject obj)//Ok按钮
        {
           

            SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok", false, () =>
            {
                ok.gameObject.SetActive(false);
               
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4));
                SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh");
                cw.gameObject.SetActive(false);
                endingSpine.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(endingSpine.gameObject, "speak", false, () =>
                {
                    SpineManager.instance.DoAnimation(endingSpine.gameObject, "daiji", true);
                });

            });
        }

        private void PlayOnClickToBf(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(play.GetChild(0).gameObject, "bf", false, () =>
            {
               // mask.SetActive(false);
                play.gameObject.SetActive(false);
                //这里要做小恶魔和布丁的对话框
                //bd.SetActive(true);
                //这里要做小恶魔和布丁的对话框
               // SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                // SpineManager.instance.DoAnimation(Play, "bf",false,()=> { Startbg.Show(); });
               // mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => {
                    bd.SetActive(false);
                    buDing.SetActive(true);
               // buDing2.SetActive(true);
                devil.SetActive(true);
                //SpineManager.instance.DoAnimation(buDing.gameObject, "xc", false,()=> 
                //{
                devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,1,()=> 
                        {
                            ShowDialogue("我讨厌季节冷暖颜色的变化，我要把这些颜色变成黑白的！", devilText);
                        },()=> 
                        {
                            buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                            {
                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () =>
                                 {
                                     ShowDialogue("小朋友们，小恶魔又来了，你们愿意和我们一起阻止小恶魔吗？", bdText);
                                 },()=> 
                                 {
                                     devil.transform.DOMove(devilStartPos.position, 1f);
                                     buDing.transform.DOMove(bdStartPos.position, 1f).OnComplete(()=> 
                                     {
                                         
                                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                                         {
                                             buDing2.SetActive(true);
                                             SpineManager.instance.DoAnimation(buDing2, "speak", true);

                                         }, ()=> 
                                         {
                                             SpineManager.instance.DoAnimation(buDing2, "daiji", true);
                                             SoundManager.instance.ShowVoiceBtn(true);
                                         }));
                                      

                                     });
                                     //buDing2.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                                     //{
                                     //    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,2,()=> 
                                     //    {
                                     //        ShowDialogue("我们一起进入四季线路图，雪山代表冷色，火山是暖色，拖拽物体进行冷暖色分类！", bdText2);
                                     //    },()=> 
                                     //    {
                                     //        //语音放完之后 需要开始的东西
                                     //        SoundManager.instance.ShowVoiceBtn(true);
                                     //    }));
                                     //});
                                 }));
                            });
                        }));
                    });
               //});
               
                    

                    // mask.SetActive(false);
                    //  Levels.SetActive(true);
                    // bd.SetActive(false);
                    // ChangeColorPant1.transform.DOLocalMoveX(1f, 0.5f);

                //}));
            });


        }
        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        }

        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false); isPlaying = false; isPressBtn = false; }); });
        }

        private void OnClickShow(GameObject obj)
        {
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => { SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () => { btnBack.SetActive(true); }); });
        }


        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtn, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(anyBtn, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                }
                else if(obj.name == "fh")
                {
                    GameInit();
                }
                else
                {

                }
                mask.gameObject.SetActive(false);
                anyBtn.name = "Btn";
            });
        }

        private void GameInit()
        {
            talkIndex = 1;

          
        }
        private void RePlayInit()//重玩按钮
        {
            talkIndex = 1;
            //星星特效
            SpineManager.instance.DoAnimation(Next.GetChild(0).gameObject, "next2", true);
            SpineManager.instance.DoAnimation(spin.GetChild(0).GetChild(0).gameObject, "4jchun", false);
            SpineManager.instance.DoAnimation(spin.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(4).GetChild(0).gameObject, "4jxia", false);
            SpineManager.instance.DoAnimation(spin.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(8).GetChild(0).gameObject, "4jqiu", false);
            SpineManager.instance.DoAnimation(spin.GetChild(9).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(10).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(11).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(12).GetChild(0).gameObject, "4jdong", false);
            //小星星和四季特效
            SpineManager.instance.DoAnimation(spin.GetChild(0).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(1).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(2).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(3).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(4).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(5).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(6).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(7).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(8).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(9).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(10).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(11).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(12).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            devilText.text= "";
            bdText.text = "";
           // bdText2.text = "";
            Main.transform.DOLocalMove(new Vector3(-30f, 0, 0), 2.0f);
            mono.StartCoroutine(IEDelay(1.5f,()=> 
            {
                SpineManager.instance.DoAnimation(spin.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_chun", false, () =>
                {
                    mono.StartCoroutine(IEDelay(1.0f, () =>
                    {
                        BgLevel1.gameObject.SetActive(true);
                    }));

                });
            }));
         
        }

        void GameStart()
        {
            SpineManager.instance.DoAnimation(Next.GetChild(0).gameObject, "next2", true);
            isPlaying = true;
            // SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            // mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));
            play.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(play.GetChild(0).gameObject, "bf2", true);
            buDing.gameObject.SetActive(false);
            buDing2.SetActive(false);
            buDing2.transform.GetRectTransform().anchoredPosition = new Vector3(292,-204);
           // buDing2.gameObject.SetActive(false);
            devil.gameObject.SetActive(false);
            Main.transform.DOLocalMove(new Vector3(-30f, 0, 0), 2.0f);
            BgLevel1.gameObject.SetActive(false);
            BgLevel2.gameObject.SetActive(false);
            BgLevel3.gameObject.SetActive(false);
            BgLevel4.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(spin.GetChild(0).GetChild(0).gameObject, "4jchun", false);
            SpineManager.instance.DoAnimation(spin.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(4).GetChild(0).gameObject, "4jxia", false);
            SpineManager.instance.DoAnimation(spin.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(8).GetChild(0).gameObject, "4jqiu", false);
            SpineManager.instance.DoAnimation(spin.GetChild(9).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(10).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(11).GetChild(0).gameObject, "YLFG_LC5-1_wujiaoxin_guangxiao", false);
            SpineManager.instance.DoAnimation(spin.GetChild(12).GetChild(0).gameObject, "4jdong", false);
            //小星星和四季特效
            SpineManager.instance.DoAnimation(spin.GetChild(0).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(1).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);//星星
            SpineManager.instance.DoAnimation(spin.GetChild(2).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);//星星
            SpineManager.instance.DoAnimation(spin.GetChild(3).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);//星星
            SpineManager.instance.DoAnimation(spin.GetChild(4).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(5).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(6).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(7).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(8).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(9).gameObject, "YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(10).gameObject,"YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(11).gameObject,"YLFG_LC5-1_ipJiangJiaYouXi", false);
            SpineManager.instance.DoAnimation(spin.GetChild(12).gameObject,"YLFG_LC5-1_ipJiangJiaYouXi", false);
            cw.gameObject.SetActive(false);
            ok.gameObject.SetActive(false);
            sijifaguang.gameObject.SetActive(false);
            devilText.text = "";
            bdText.text = "";
            //bdText2.text = "";
            Next.gameObject.SetActive(false);
            mask.SetActive(true);
            Main.gameObject.SetActive(true);
            endingSpine.gameObject.SetActive(false);
            //初始化
            // _ILDrager1[0].DoReset();
            InitSpine();
        }
        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

                bd.SetActive(false);
                mask.SetActive(false);
               // SpineManager.instance.DoAnimation(spin.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_XGX", false);
                SpineManager.instance.DoAnimation(spin.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_4JiTG_chun", false, () =>
                {
                    mono.StartCoroutine(IEDelay(1.5f, () =>
                    {
                        BgLevel1.gameObject.SetActive(true);
                    }));

                });
                buDing2.transform.DOMove(bdStartPos.position, 1f);
                devil.transform.DOMove(devilStartPos.position, 1f);
            }
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "6-12", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine,"6-12-2", false,
                () =>
                {  /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
                });
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        private void InitSpine() 
        {
            QiPaoTotalOne.GetChild(0).GetRectTransform().anchoredPosition = new Vector3(400.2f, 889.6f);
            QiPaoTotalOne.GetChild(1).GetRectTransform().anchoredPosition = new Vector3(723.6f, 584.6f);
            QiPaoTotalOne.GetChild(2).GetRectTransform().anchoredPosition = new Vector3(1010.9f, 839.1f);
            QiPaoTotalOne.GetChild(3).GetRectTransform().anchoredPosition = new Vector3(1560.2f, 841.6f);
            QiPaoTotalOne.GetChild(4).GetRectTransform().anchoredPosition = new Vector3(1249.5f, 573.3f);

            QiPaoTotalTwo.GetChild(0).GetRectTransform().anchoredPosition = new Vector3(222f, 743f);
            QiPaoTotalTwo.GetChild(1).GetRectTransform().anchoredPosition = new Vector3(588f, 922f);
            QiPaoTotalTwo.GetChild(2).GetRectTransform().anchoredPosition = new Vector3(779f, 608f);
            QiPaoTotalTwo.GetChild(3).GetRectTransform().anchoredPosition = new Vector3(1118f, 923f);
            QiPaoTotalTwo.GetChild(4).GetRectTransform().anchoredPosition = new Vector3(1249.5f, 573.3f);
            QiPaoTotalTwo.GetChild(5).GetRectTransform().anchoredPosition = new Vector3(1569f, 797f);

            QiPaoTotalThree.GetChild(0).GetRectTransform().anchoredPosition = new Vector3(283f, 828f);
            QiPaoTotalThree.GetChild(1).GetRectTransform().anchoredPosition = new Vector3(588f, 922f);
            QiPaoTotalThree.GetChild(2).GetRectTransform().anchoredPosition = new Vector3(779f, 608f);
            QiPaoTotalThree.GetChild(3).GetRectTransform().anchoredPosition = new Vector3(1032f, 931f);
            QiPaoTotalThree.GetChild(4).GetRectTransform().anchoredPosition = new Vector3(916f, 314f);
            QiPaoTotalThree.GetChild(5).GetRectTransform().anchoredPosition = new Vector3(1475f, 870f);
            QiPaoTotalThree.GetChild(6).GetRectTransform().anchoredPosition = new Vector3(1180f, 622f);

            QiPaoTotalFour.GetChild(0).GetRectTransform().anchoredPosition = new Vector3(314f, 769f);
            QiPaoTotalFour.GetChild(1).GetRectTransform().anchoredPosition = new Vector3(588f, 922f);
            QiPaoTotalFour.GetChild(2).GetRectTransform().anchoredPosition = new Vector3(779f, 608f);
            QiPaoTotalFour.GetChild(3).GetRectTransform().anchoredPosition = new Vector3(1032f, 931f);
            QiPaoTotalFour.GetChild(4).GetRectTransform().anchoredPosition = new Vector3(853f, 272f);
            QiPaoTotalFour.GetChild(5).GetRectTransform().anchoredPosition = new Vector3(1475f, 870f);
            QiPaoTotalFour.GetChild(6).GetRectTransform().anchoredPosition = new Vector3(1126f, 562f);
            QiPaoTotalFour.GetChild(7).GetRectTransform().anchoredPosition = new Vector3(1591f, 599f);

            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalOne.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);

            SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalTwo.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);

            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalThree.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);

            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(0).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(1).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(2).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(3).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(4).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(5).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(6).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);
            SpineManager.instance.DoAnimation(QiPaoTotalFour.GetChild(7).GetChild(0).gameObject, "YLFG_LC5-1_qi pao", false);

            QiPaoTotalOne.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = true;

            QiPaoTotalTwo.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(5).gameObject.GetComponent<ILDrager>().isActived = true;

            QiPaoTotalThree.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(5).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(6).gameObject.GetComponent<ILDrager>().isActived = true;

            QiPaoTotalFour.GetChild(0).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(1).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(2).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(3).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(4).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(5).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(6).gameObject.GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(7).gameObject.GetComponent<ILDrager>().isActived = true;
        }
        private void InitActive1() 
        {
            QiPaoTotalOne.GetChild(0).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalOne.GetChild(1).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalOne.GetChild(2).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalOne.GetChild(3).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalOne.GetChild(4).GetComponent<ILDrager>().isActived = false;

            QiPaoTotalTwo.GetChild(0).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalTwo.GetChild(1).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalTwo.GetChild(2).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalTwo.GetChild(3).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalTwo.GetChild(4).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalTwo.GetChild(5).GetComponent<ILDrager>().isActived = false;

            QiPaoTotalThree.GetChild(0).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalThree.GetChild(1).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalThree.GetChild(2).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalThree.GetChild(3).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalThree.GetChild(4).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalThree.GetChild(5).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalThree.GetChild(6).GetComponent<ILDrager>().isActived = false;

            QiPaoTotalFour.GetChild(0).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(1).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(2).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(3).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(4).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(5).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(6).GetComponent<ILDrager>().isActived = false;
            QiPaoTotalFour.GetChild(7).GetComponent<ILDrager>().isActived = false;
        }
        private void InitActive2()
        {
            QiPaoTotalOne.GetChild(0).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(1).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(2).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(3).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalOne.GetChild(4).GetComponent<ILDrager>().isActived = true;

            QiPaoTotalTwo.GetChild(0).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(1).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(2).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(3).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(4).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalTwo.GetChild(5).GetComponent<ILDrager>().isActived = true;

            QiPaoTotalThree.GetChild(0).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(1).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(2).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(3).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(4).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(5).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalThree.GetChild(6).GetComponent<ILDrager>().isActived = true;

            QiPaoTotalFour.GetChild(0).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(1).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(2).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(3).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(4).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(5).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(6).GetComponent<ILDrager>().isActived = true;
            QiPaoTotalFour.GetChild(7).GetComponent<ILDrager>().isActived = true;
        }
        IEnumerator randomPlayVoiceRight(Action callBack1, Action callBack2)
        {
            callBack1?.Invoke();
            int i = Random.Range(5, 8);
            float ind = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false);
            yield return new WaitForSeconds(ind);
            callBack2?.Invoke();
        }
        IEnumerator randomPlayVoiceFalse(Action callBack1, Action callBack2)
        {
            callBack1?.Invoke();
            int i = Random.Range(8, 11);
            float ind = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false);
            yield return new WaitForSeconds(ind);
            callBack2?.Invoke();
        }
    }
}
