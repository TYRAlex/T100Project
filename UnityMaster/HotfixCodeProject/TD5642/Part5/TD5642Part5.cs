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
    public class TD5642Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;
        
          #region Mask
        private Transform anyBtns;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion
        
        #region 田丁对话

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
        private Transform SpineShow;

        #endregion
        
        #endregion

          #region 点击滑动图片

        private GameObject pageBar;
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;
        private GameObject rightBtn;
        private GameObject leftBtn;
        private GameObject btnBack;
        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        #endregion
        
       
        
        bool isPressBtn = false;
        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        #endregion

        private Transform HuaBa;
        private Transform Drag1;
        private List<mILDrager> mILDragers1;
        private Transform Drag2;
        private List<mILDrager> mILDragers2;
        private Transform Drag3;
        private List<mILDrager> mILDragers3;

        bool isPlaying = false;
        private Transform ts;
        private bool isLeft;
        private bool isRight;
        private bool _isPause;
        private bool Level1_1;
        private bool Level1_2;
        private bool Level1_3;

        private bool Level2_1;
        private bool Level2_2;
        private bool Level2_3;
        private bool Level2_4;

        private bool Level3_1;
        private bool Level3_2;
        private bool Level3_3;
        private bool Level3_4;
        private bool Level3_5;
        private Transform Level1; private Transform Level2; private Transform Level3;
        private int success_index;
        private Transform tuzi;private Transform huli;private Transform songshu;
        private Transform tuzi2;
        private Transform Drag1_1;
        private Transform Drag1_2;
        private Transform Drag1_3;
        private Transform Drag1_4;
        private Transform Drag1_5;
        private Transform Drag1_6;

        private Transform Drag2_1;
        private Transform Drag2_2;
        private Transform Drag2_3;
        private Transform Drag2_4;
        private Transform Drag2_5;
        private Transform Drag2_6;

        private Transform Drag3_1;
        private Transform Drag3_2;
        private Transform Drag3_3;
        private Transform Drag3_4;
        private Transform Drag3_5;
        private Transform Drag3_6;

        private Transform xem;
        private Transform xem2;
        private Transform xem3;
        private Transform _Mask;
        private bool IsPlaying;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            HuaBa = curTrans.Find("HuoBa");
            Drag1 = curTrans.Find("Drag");
            Drag1_1 = curTrans.Find("Drag/1");
            Drag1_2 = curTrans.Find("Drag/2");
            Drag1_3 = curTrans.Find("Drag/3");
            Drag1_4 = curTrans.Find("Drag/4");
            Drag1_5 = curTrans.Find("Drag/5");
            Drag1_6 = curTrans.Find("Drag/6");

            Drag2 = curTrans.Find("Drag2");
            Drag2_1 = curTrans.Find("Drag2/1");
            Drag2_2 = curTrans.Find("Drag2/2");
            Drag2_3 = curTrans.Find("Drag2/3");
            Drag2_4 = curTrans.Find("Drag2/4");
            Drag2_5 = curTrans.Find("Drag2/5");
            Drag2_6 = curTrans.Find("Drag2/6");

            Drag3 = curTrans.Find("Drag3");
            Drag3_1 = curTrans.Find("Drag3/1");
            Drag3_2 = curTrans.Find("Drag3/2");
            Drag3_3 = curTrans.Find("Drag3/3");
            Drag3_4 = curTrans.Find("Drag3/4");
            Drag3_5 = curTrans.Find("Drag3/5");
            Drag3_6 = curTrans.Find("Drag3/6");

            Level1 = curTrans.Find("Level");
            Level2 = curTrans.Find("Level2");
            Level3 = curTrans.Find("Level3");
            tuzi = curTrans.Find("tuzi"); 
            huli = curTrans.Find("huli");
            songshu = curTrans.Find("songshu");
            ts = curTrans.Find("mask/ts");

            xem = curTrans.Find("Bg/xem1");
            xem2 = curTrans.Find("Bg/xem2");
            xem3 = curTrans.Find("Bg/xem3");
            _Mask = curTrans.Find("_Mask");
            bellTextures = Bg.GetComponent<BellSprites>();
            isLeft = false; isRight = true;_isPause = false;
            Level1_1 = true; Level1_2 = false; Level1_3 =false;
            Level2_1 = true; Level2_2 = false; Level2_3 = false; Level2_4 = false;
            Level3_1 = true; Level3_2 = false; Level3_3 = false; Level3_4 = false; Level3_5= false;
            success_index = 0;
            IsPlaying = false;
            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            InitLayers();

            ADDDrager();
            ADDDragEvent();
            CW();
            //GameStart();
        }


        void IEMove(RectTransform rect,float startXPos,float endXPos,float speed) 
        {
            if (rect.anchoredPosition.x >= endXPos)
            {         
                isRight = false;
                isLeft = true;
              
            }
            if (rect.anchoredPosition.x <= startXPos) 
            {
                isRight = true;
                isLeft = false;
            }
            if (isLeft) 
            {
                rect.Translate(Vector2.left * speed);
            }
            if (isRight) 
            {
                rect.Translate(Vector2.right*speed);
            }
         
        }
        private void ADDDrager() 
        {
            mILDragers1 = new List<mILDrager>();
            for (int i = 0; i < Drag1.childCount; i++) 
            {
                var a = Drag1.GetChild(i).GetComponent<mILDrager>();
                mILDragers1.Add(a);
            }
            mILDragers2 = new List<mILDrager>();
            for (int i = 0; i < Drag2.childCount; i++)
            {
                var a = Drag2.GetChild(i).GetComponent<mILDrager>();
                mILDragers2.Add(a);
            }
            mILDragers3 = new List<mILDrager>();
            for (int i = 0; i < Drag3.childCount; i++)
            {
                var a = Drag3.GetChild(i).GetComponent<mILDrager>();
                mILDragers3.Add(a);
            }
        }
        private void ADDDragEvent() 
        {
            foreach (var drager in mILDragers1) 
            {
                drager.SetDragCallback(DragStart,Draging,DragEnd);
            }
            foreach (var drager in mILDragers2)
            {
                drager.SetDragCallback(DragStart2, Draging, DragEnd2);
            }
            foreach (var drager in mILDragers3)
            {
                drager.SetDragCallback(DragStart3, Draging, DragEnd3);
            }
        }
        private void DragStart(Vector3 position,int type,int index) 
        {
            mILDragers1[index].transform.SetAsLastSibling();
            mILDragers1[index].transform.position = Input.mousePosition;
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            mILDragers2[index].transform.SetAsLastSibling(); mILDragers2[index].transform.position = Input.mousePosition;
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            mILDragers3[index].transform.SetAsLastSibling(); mILDragers3[index].transform.position = Input.mousePosition;
        }
        private void Draging(Vector3 position, int type, int index)
        {

        }
        private void DragEnd(Vector3 position, int type, int index,bool isMatch)
        {
            if (isMatch)
            {
               

                if (Level1_1 && type == 6)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level1_2 = true;
                    mILDragers1[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaD", false, () =>
                       {
                           SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaD2", true);
                       });
                    SpineManager.instance.DoAnimation(Level1.GetChild(0).gameObject, "shuE2", false, () =>
                       {
                           SpineManager.instance.DoAnimation(Level1.GetChild(0).gameObject, "shuE3", true);
                       });
                }
                else if (Level1_2 && type == 2)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level1_3 = true;
                    mILDragers1[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaB", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaB2", true);
                    });
                    SpineManager.instance.DoAnimation(Level1.GetChild(1).gameObject, "shuB2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level1.GetChild(1).gameObject, "shuB3", true);
                    });
                }
                else if (Level1_3 && type == 4)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;


                    mILDragers1[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaC", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaC2", true);
                    });
                    SpineManager.instance.DoAnimation(Level1.GetChild(2).gameObject, "shuA2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level1.GetChild(2).gameObject, "shuA3", true);
                    });
                }
                else 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    mILDragers1[index].DoReset();
                }
               
                if (success_index == 3) 
                {
                    success_index = 0;
                    _Mask.gameObject.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                    SpineManager.instance.DoAnimation(tuzi.GetChild(0).gameObject,"tuizi5",false,()=>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                        SpineManager.instance.DoAnimation(xem.gameObject, "xiaoguo", false, () =>
                        {
                            SpineManager.instance.DoAnimation(xem.gameObject, "kong", false);
                        });

                        SpineManager.instance.DoAnimation(tuzi.GetChild(0).gameObject, "tuizi4", true);
                        tuzi.GetRectTransform().DOAnchorPosX(1096,3f);
                    });
                    MyWait(5f, () =>
                    {
                        mask.SetActive(true);
                        _Mask.gameObject.SetActive(false);
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject,"huoba00",false);
                        Drag1.gameObject.SetActive(false);
                        Drag2.gameObject.SetActive(true);
                        Level1.gameObject.SetActive(false);
                        Level2.gameObject.SetActive(true);
                        tuzi.gameObject.SetActive(false);
                        huli.gameObject.SetActive(true);

                        ts.GetComponent<RawImage>().texture = ts.GetComponent<BellSprites>().texture[1];
                        ts.GetComponent<RawImage>().SetNativeSize();
                        MyWait(4f,()=> 
                        {
                            mask.gameObject.SetActive(false);
                            ts.gameObject.SetActive(false);
                        });
                    });

                }

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                mILDragers1[index].DoReset();
            }


        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch) 
        {
            if (isMatch)
            {
               
                if (Level2_1 && type == 6)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level2_2 = true;

                    mILDragers2[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaA", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaA2", true);
                    });
                    SpineManager.instance.DoAnimation(Level2.GetChild(0).gameObject, "shuC2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level2.GetChild(0).gameObject, "shuC3", true);
                    });
                }
                else if (Level2_2 && type == 4)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level2_3 = true;
                    mILDragers2[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaE", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaE2", true);
                    });
                    SpineManager.instance.DoAnimation(Level2.GetChild(1).gameObject, "shuF2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level2.GetChild(1).gameObject, "shuF3", true);
                    });
                }
                else if (Level2_3 && type == 5)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;
                    Level2_4 = true;
                    mILDragers2[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaF", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaF2", true);
                    });
                    SpineManager.instance.DoAnimation(Level2.GetChild(2).gameObject, "shuH2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level2.GetChild(2).gameObject, "shuH3", true);
                    });
                }
                else if (Level2_4 && type == 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;
                    mILDragers2[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaG", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaG2", true);
                    });
                    SpineManager.instance.DoAnimation(Level2.GetChild(3).gameObject, "shuJ2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level2.GetChild(3).gameObject, "shuJ3", true);
                    });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    mILDragers2[index].DoReset();
                }

                if (success_index == 4)
                {
                    _Mask.gameObject.SetActive(true);
                    success_index = 0; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,3);
                    SpineManager.instance.DoAnimation(huli.GetChild(0).gameObject, "huli5", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                        SpineManager.instance.DoAnimation(xem2.gameObject, "xiaoguo", false, () =>
                        {
                            SpineManager.instance.DoAnimation(xem2.gameObject, "kong", false);
                        });

                        SpineManager.instance.DoAnimation(huli.GetChild(0).gameObject, "huli4", true);
                        huli.GetRectTransform().DOAnchorPosX(-1116,5f);
                    });

                    MyWait(5f, () =>
                    {
                        _Mask.gameObject.SetActive(false);
                        mask.SetActive(true);
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huoba00", false);
                        ts.gameObject.SetActive(true);
                        Drag2.gameObject.SetActive(false);
                        Drag3.gameObject.SetActive(true);
                        Level2.gameObject.SetActive(false);
                        Level3.gameObject.SetActive(true);
                        huli.gameObject.SetActive(false);
                        songshu.gameObject.SetActive(true);

                        ts.GetComponent<RawImage>().texture = ts.GetComponent<BellSprites>().texture[2];
                        ts.GetComponent<RawImage>().SetNativeSize();
                        MyWait(4f, () =>
                        {
                            mask.gameObject.SetActive(false);
                            ts.gameObject.SetActive(false);
                        });
                    });

                }
            }
            else 
            {
               
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                mILDragers2[index].DoReset();
            }
        }

        private void DragEnd3(Vector3 position, int type, int index, bool isMatch) 
        {
            
            if (isMatch) 
            {
               
                if (Level3_1 && type == 4)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level3_2 = true;

                    mILDragers3[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaH", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaH2", true);
                    });
                    SpineManager.instance.DoAnimation(Level3.GetChild(0).gameObject, "shuD2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level3.GetChild(0).gameObject, "shuD3", true);
                    });
                } 
                else if (Level3_2 && type ==6)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level3_3 = true;
                    Debug.Log("11");
                    mILDragers3[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaI", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaI2", true);
                    });
                    SpineManager.instance.DoAnimation(Level3.GetChild(1).gameObject, "shuG2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level3.GetChild(1).gameObject, "shuG3", true);
                    });
                }
                else if (Level3_3 && type ==3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level3_4 = true;

                    mILDragers3[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaJ", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaJ2", true);
                    });
                    SpineManager.instance.DoAnimation(Level3.GetChild(2).gameObject, "shuI2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level3.GetChild(2).gameObject, "shuI3", true);
                    });
                }
                else if (Level3_4 && type == 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    Level3_5 = true;

                    mILDragers3[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaK", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaK2", true);
                    });
                    SpineManager.instance.DoAnimation(Level3.GetChild(3).gameObject, "shuL2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level3.GetChild(3).gameObject, "shuL3", true);
                    });
                }
                else if (Level3_5 && type == 5)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                    success_index++;

                    //   Level3_5 = true;
                    _isPause = true;
                    mILDragers3[index].gameObject.SetActive(false);

                    SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaL", false, () =>
                    {
                        SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huobaL2", true);
                    });
                    SpineManager.instance.DoAnimation(Level3.GetChild(4).gameObject, "shuK2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Level3.GetChild(4).gameObject, "shuK3", true);
                    });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    mILDragers3[index].DoReset();
                }

                if (success_index == 5)
                {
                    _Mask.gameObject.SetActive(true);
                    success_index = 0; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                    SpineManager.instance.DoAnimation(songshu.GetChild(0).gameObject, "songshu5", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2);
                        SpineManager.instance.DoAnimation(xem3.gameObject, "xiaoguo", false, () =>
                        {
                            SpineManager.instance.DoAnimation(xem3.gameObject, "kong", false);
                        });

                        SpineManager.instance.DoAnimation(songshu.GetChild(0).gameObject, "songshu4", true);
                        songshu.GetRectTransform().DOAnchorPosX(-1093,5f);
                    });
                    MyWait(5f,()=>
                    {
                        _Mask.gameObject.SetActive(false);
                        playSuccessSpine();
                    });
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                mILDragers3[index].DoReset();
            }


        }
        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            devilText.text = "";
            bdText.text = "";

            //田丁初始化
            TDGameInit(); DOTween.KillAll();
            IEmove2(HuaBa.GetRectTransform(), -586, 821, 3.5f, IEMove);
        }

        private void InitLayers() 
        {
            Drag1_1.SetAsFirstSibling();
            Drag1_2.SetSiblingIndex(1);
            Drag1_3.SetSiblingIndex(2);
            Drag1_4.SetSiblingIndex(3);
            Drag1_5.SetSiblingIndex(4);
            Drag1_6.SetAsLastSibling();

            Drag2_1.SetAsFirstSibling();
            Drag2_2.SetSiblingIndex(1);
            Drag2_3.SetSiblingIndex(2);
            Drag2_4.SetSiblingIndex(3);
            Drag2_5.SetSiblingIndex(4);
            Drag2_6.SetAsLastSibling();

            Drag3_1.SetAsFirstSibling();
            Drag3_2.SetSiblingIndex(1);
            Drag3_3.SetSiblingIndex(2);
            Drag3_4.SetSiblingIndex(3);
            Drag3_5.SetSiblingIndex(4);
            Drag3_6.SetAsLastSibling();
        }
        
        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();
           
        }
        Coroutine IEmove2(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
            return mono.StartCoroutine(IEMove(rect, startXPos, endXPos, speed, moveCallBack));
        }
        IEnumerator IEMove(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
            while (true)
            {
                if (_isPause==false)
                {
                    yield return new WaitForSeconds(0.01f);
                
                    moveCallBack?.Invoke(rect, startXPos, endXPos, speed);
                }
                else
                {
                    yield return null;
                }

            }
        }
        #region 田丁

        void TDGameInit()
        {
            
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            LRBtnUpdate();
        }
        void TDGameStart()
        {
            buDing.SetActive(true);devil.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 0, () =>
                {
                    ShowDialogue("我讨厌一切，是不会让你们得逞的", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 1, () =>
                        {
                            ShowDialogue("不好!小恶魔布鲁鲁又来捣乱了，我们一起阻止它", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));
            });
        }

            #endregion

        #endregion
       

        #region 说话语音

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 语音键对应方法
        
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //田丁游戏开始方法
                    TDGameStartFunc();
                    
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            buDing.SetActive(false);
            devil.SetActive(false);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 2, null, () => {  bd.SetActive(false); ts.gameObject.SetActive(true); MyWait(4f, () => { mask.SetActive(false); }); }));
      
        }

        #endregion

        #region 通用方法
        
        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target,string name,bool isLoop=false,Action callback=null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }
        
        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index,Action goingEvent=null,Action finishEvent=null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target,SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }
        
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex,Action callback=null)
        {
            float voiceTimer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }
        
        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }
        
        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="callback"></param>
        void WaitTimeAndExcuteNext(float timer,Action callback)
        {      
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer,Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();
            
        }
        
        
        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }
        
        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }
        
        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
           
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete( () => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
        }
        private void LRBtnUpdate()
        {
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
            else if (curPageIndex == SpinePage.childCount - 1)
            {
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.name + "4", false);
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            }
            else
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
        }
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

        #endregion
        

        #endregion


        #region 田丁
        
          #region 田丁加载

          /// <summary>
          /// 田丁加载所有物体
          /// </summary>
          void TDLoadGameProperty()
          {
              mask = curTrans.Find("mask").gameObject;
              mask.SetActive(true);
              //任务对话方法加载
              TDLoadDialogue();
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();
              //加载点击滑动图片
              TDLoadPageBar();
              //加载材料环节
              LoadSpineShow();

          }

          /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            
            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
        }
        
        /// <summary>
        /// 加载对话环节
        /// </summary>
        void TDLoadDialogue()
        {
            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");
        }

        /// <summary>
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
        }
        /// <summary>
        /// 加载按钮
        /// </summary>
        void TDLoadButton()
        {
            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }
        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {
            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);
        }
        /// <summary>
        /// 加载点击材料环节
        /// </summary>
        void LoadSpineShow()
        {
            SpineShow = curTrans.Find("SpineShow");
            SpineShow.gameObject.SetActive(false);
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }
        }
        

        #endregion

          #region 鼠标滑动图片方法

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = Math.Abs(upData.position.x - _prePressPos.x);
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 300)
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
        

        #endregion

          #region 点击材料环节

        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
            {
                isPlaying = false;
                if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
                {
                    flag += 1 << obj.transform.GetSiblingIndex();
                }
                if (flag == (Mathf.Pow(2, SpineShow.childCount) - 1))
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }));
            SpineManager.instance.DoAnimation(SpineShow.gameObject, obj.name, false);
        }
        

        #endregion

          #region 点击移动图片环节

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
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false); isPlaying = false; isPressBtn = false; 
                    if (flag == (Mathf.Pow(2, SpinePage.childCount) - 1) && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }

        private void OnClickShow(GameObject obj)
        {
            if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
            {
                SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            }
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () =>
                {
                    isPressBtn = true;
                    btnBack.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name)+1, null, () =>
                     {
                        //用于标志是否点击过展示板
                        if ((flag & (1 << int.Parse(obj.transform.GetChild(0).name))) == 0)
                         {
                             flag += 1 << int.Parse(obj.transform.GetChild(0).name);
                         }
                         isPressBtn = false;
                     }));
                });
            });
        }

        #endregion

          #region 切换游戏按键方法

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            isPlaying = false;
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
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (isPlaying==false)
            {
                isPlaying = true;
                BtnPlaySound();
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    if (obj.name == "bf")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false); GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                        { 
                            anyBtns.gameObject.SetActive(false);
                          //  mask.SetActive(false);
                            CW();
                            mask.gameObject.SetActive(true);
                            ts.gameObject.SetActive(true);
                            MyWait(4f, () => { mask.SetActive(false); });
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.SOUND, 3)); });
                    }

                });
            }
         
           
        }
        

        #endregion

          #region 田丁对话方法

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
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

        #endregion
        
          #region 田丁成功动画

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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                        () =>
                        {
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            
               
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }
        IEnumerator Wait(float time,Action method_1=null)
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        private void MyWait(float time, Action method_1 = null) 
        {
            mono.StartCoroutine(Wait(time,method_1));
        }
        private void CW()
        {
          
            _Mask.gameObject.SetActive(false);
          
            for (int i = 0; i < Drag1.childCount; i++)
            {
                mILDragers1[i].gameObject.SetActive(true);
                mILDragers2[i].gameObject.SetActive(true);
                mILDragers3[i].gameObject.SetActive(true);
            }
            InitLayers();
            CWInitPos();
            talkIndex = 1;
            devilText.text = "";
            bdText.text = "";
            HuaBa.GetRectTransform().position = new Vector2(0,137);
            ts.GetComponent<RawImage>().texture = ts.GetComponent<BellSprites>().texture[0];
            ts.GetComponent<RawImage>().SetNativeSize();
            ts.gameObject.SetActive(false);
            
            tuzi.gameObject.SetActive(true);SpineManager.instance.DoAnimation(tuzi.GetChild(0).gameObject,"tuizi",false);tuzi.GetRectTransform().anchoredPosition = new Vector2(450f,-366);
            huli.gameObject.SetActive(false); SpineManager.instance.DoAnimation(huli.GetChild(0).gameObject, "huli", false); huli.GetRectTransform().anchoredPosition = new Vector2(515.81f, -335.5f);
            songshu.gameObject.SetActive(false); SpineManager.instance.DoAnimation(songshu.GetChild(0).gameObject, "songshu", false); songshu.GetRectTransform().anchoredPosition = new Vector2(400f, -365);
            _isPause = false;
            Level1_1 = true; Level1_2 = false; Level1_3 = false;
            Level2_1 = true; Level2_2 = false; Level2_3 = false; Level2_4 = false;
            Level3_1 = true; Level3_2 = false; Level3_3 = false; Level3_4 = false; Level3_5 = false;
     
            success_index = 0;
            SpineManager.instance.DoAnimation(HuaBa.GetChild(0).gameObject, "huoba00", false);
            SpineManager.instance.DoAnimation(Level1.GetChild(0).gameObject,"shuE",false);
            SpineManager.instance.DoAnimation(Level1.GetChild(1).gameObject, "shuB", false);
            SpineManager.instance.DoAnimation(Level1.GetChild(2).gameObject, "shuA", false);

            SpineManager.instance.DoAnimation(Level2.GetChild(0).gameObject, "shuC", false);
            SpineManager.instance.DoAnimation(Level2.GetChild(1).gameObject, "shuF", false);
            SpineManager.instance.DoAnimation(Level2.GetChild(2).gameObject, "shuH", false);
            SpineManager.instance.DoAnimation(Level2.GetChild(3).gameObject, "shuJ", false);

            SpineManager.instance.DoAnimation(Level3.GetChild(0).gameObject, "shuC", false);
            SpineManager.instance.DoAnimation(Level3.GetChild(1).gameObject, "shuF", false);
            SpineManager.instance.DoAnimation(Level3.GetChild(2).gameObject, "shuH", false);
            SpineManager.instance.DoAnimation(Level3.GetChild(3).gameObject, "shuL", false);
            SpineManager.instance.DoAnimation(Level3.GetChild(4).gameObject, "shuJ", false);

       
          
            xem.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
           xem2.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
           xem3.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(xem.gameObject, "xem", true);
            SpineManager.instance.DoAnimation(xem2.gameObject, "xem", true);
            SpineManager.instance.DoAnimation(xem3.gameObject, "xem", true);
            Level1.gameObject.SetActive(true);
            Drag1.gameObject.SetActive(true);

            Level2.gameObject.SetActive(false);
            Drag2.gameObject.SetActive(false);

            Level3.gameObject.SetActive(false);
            Drag3.gameObject.SetActive(false);
    
        }
        private void CWInitPos() 
        {
            Drag1_1.GetRectTransform().position = curTrans.Find("1").GetRectTransform().position;
            Drag1_2.GetRectTransform().position = curTrans.Find("2").GetRectTransform().position;
            Drag1_3.GetRectTransform().position = curTrans.Find("3").GetRectTransform().position;
            Drag1_4.GetRectTransform().position = curTrans.Find("4").GetRectTransform().position;
            Drag1_5.GetRectTransform().position = curTrans.Find("5").GetRectTransform().position;
            Drag1_6.GetRectTransform().position = curTrans.Find("6").GetRectTransform().position;

            Drag2_1.GetRectTransform().position = curTrans.Find("1").GetRectTransform().position;
            Drag2_2.GetRectTransform().position = curTrans.Find("2").GetRectTransform().position;
            Drag2_3.GetRectTransform().position = curTrans.Find("3").GetRectTransform().position;
            Drag2_4.GetRectTransform().position = curTrans.Find("4").GetRectTransform().position;
            Drag2_5.GetRectTransform().position = curTrans.Find("5").GetRectTransform().position;
            Drag2_6.GetRectTransform().position = curTrans.Find("6").GetRectTransform().position;

            Drag3_1.GetRectTransform().position = curTrans.Find("1").GetRectTransform().position;
            Drag3_2.GetRectTransform().position = curTrans.Find("2").GetRectTransform().position;
            Drag3_3.GetRectTransform().position = curTrans.Find("3").GetRectTransform().position;
            Drag3_4.GetRectTransform().position = curTrans.Find("4").GetRectTransform().position;
            Drag3_5.GetRectTransform().position = curTrans.Find("5").GetRectTransform().position;
            Drag3_6.GetRectTransform().position = curTrans.Find("6").GetRectTransform().position;
        }
        #endregion
       

        #endregion

        

        

    }
}
