using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD3434Part5
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
        private bool hasbeenOnClick;
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

        private List<mILDrager> _ILDragers1;//获取游戏中MILDrager组件的集合
        private List<mILDrager> _ILDragers2;
        private List<mILDrager> _ILDragers3;

        private Transform Drag1;
        private Transform Drag2;
        private Transform Drag3;

        private Transform Drop1;
        private Transform Drop2;
        private Transform Drop3;

        private Transform _Level1;
        private Transform _Level2;
        private Transform _Level3;

        private Transform boom;
        private Transform xem;
        private Transform UI;

        private int FGindex1;
        private int FGindex2;
        private int FGindex3;
        private int FGindex4;

        private Transform drag1_se1;
        private Transform drag1_se2;
        private Transform drag1_se3;
        private Transform drag1_se4;
        private Transform drag1_se5;
        private Transform drag1_se6;
        private Transform drag1_se7;
        private Transform drag1_se8;

        private Transform drag2_se1;
        private Transform drag2_se2;
        private Transform drag2_se3;
        private Transform drag2_se4;
        private Transform drag2_se5;
        private Transform drag2_se6;
        private Transform drag2_se7;
        private Transform drag2_se8;

        private Transform drag3_se1;
        private Transform drag3_se2;
        private Transform drag3_se3;
        private Transform drag3_se4;
        private Transform drag3_se5;
        private Transform drag3_se6;
        private Transform drag3_se7;
        private Transform drag3_se8;

        private Transform _mask;
        private int OnDragRightNum;
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
        
        
       
        bool isPlaying = false;
       

        void Start(object o)
        {

            
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            hasbeenOnClick = false;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            _mask = curTrans.Find("_mask");//语音播放时候的遮罩

            Drag1 = curTrans.Find("drag1");
            Drag2 = curTrans.Find("drag2");
            Drag3 = curTrans.Find("drag3");

            Drop1 = curTrans.Find("drop1");
            Drop2 = curTrans.Find("drop2");
            Drop3 = curTrans.Find("drop3");

            _Level1 = curTrans.Find("Level1");
            _Level2 = curTrans.Find("Level2");
            _Level3 = curTrans.Find("Level3");

            boom = curTrans.Find("boom");
            xem = curTrans.Find("UI/xem");
            UI = curTrans.Find("UI");
            FGindex1 = 0;
            FGindex2 = 0;
            FGindex3 = 0;
            FGindex4 = 0;

            OnDragRightNum = 0;

            drag1_se1 = curTrans.Find("drag1/se1");
            drag1_se2 = curTrans.Find("drag1/se2");
            drag1_se3 = curTrans.Find("drag1/se3");
            drag1_se4 = curTrans.Find("drag1/se4");
            drag1_se5 = curTrans.Find("drag1/se5");
            drag1_se6 = curTrans.Find("drag1/se6");
            drag1_se7 = curTrans.Find("drag1/se7");
            drag1_se8 = curTrans.Find("drag1/se8");

            drag2_se1 = curTrans.Find("drag2/se1");
            drag2_se2 = curTrans.Find("drag2/se2");
            drag2_se3 = curTrans.Find("drag2/se3");
            drag2_se4 = curTrans.Find("drag2/se4");
            drag2_se5 = curTrans.Find("drag2/se5");
            drag2_se6 = curTrans.Find("drag2/se6");
            drag2_se7 = curTrans.Find("drag2/se7");
            drag2_se8 = curTrans.Find("drag2/se8");

            drag3_se1 = curTrans.Find("drag3/se1");
            drag3_se2 = curTrans.Find("drag3/se2");
            drag3_se3 = curTrans.Find("drag3/se3");
            drag3_se4 = curTrans.Find("drag3/se4");
            drag3_se5 = curTrans.Find("drag3/se5");
            drag3_se6 = curTrans.Find("drag3/se6");
            drag3_se7 = curTrans.Find("drag3/se7");
            drag3_se8 = curTrans.Find("drag3/se8");
            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            InitLayers();
            InitmILDrager();
            AddDragersEvent();

            GameInit();
            CZPos();
            CZPosition();
            CW();
            //GameStart();
        }

        
        

      

        #region 初始化和游戏开始方法

        private void GameInit()
        {
         
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            
            //田丁初始化
            TDGameInit();
           
        }

        

        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();
           
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bd.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutinett(bd.gameObject,SoundManager.SoundType.VOICE,0,null,()=> { SoundManager.instance.ShowVoiceBtn(true);  }));
            //devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            //{
            //    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
            //    {
            //        ShowDialogue("", devilText);
            //    }, () =>
            //    {
            //        buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            //        {
            //            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
            //            {
            //                ShowDialogue("", bdText);
            //            }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //        });
            //    }));
            //});
        }
        void InitmILDrager() 
        {
            _ILDragers1 = null;
            _ILDragers2 = null;
            _ILDragers3 = null;



            _ILDragers1 = new List<mILDrager>();

            for (int i = 0; i < Drag1.childCount; i++) 
            {
                var iLDrager1 = Drag1.GetChild(i).GetComponent<mILDrager>();
                _ILDragers1.Add(iLDrager1);
            }
            _ILDragers2 = new List<mILDrager>();
            for (int i = 0; i < Drag2.childCount; i++)
            {
                var iLDrager2 = Drag2.GetChild(i).GetComponent<mILDrager>();
                _ILDragers2.Add(iLDrager2);
            }
            _ILDragers3 = new List<mILDrager>();
            for (int i = 0; i < Drag3.childCount; i++)
            {
                var iLDrager3 = Drag3.GetChild(i).GetComponent<mILDrager>();
                _ILDragers3.Add(iLDrager3);
            }
        }
        private void AddDragersEvent()
        {

            foreach (var drager in _ILDragers1)
                drager.SetDragCallback(DragStart, Draging, DragEnd);

            foreach (var drager in _ILDragers2)
                drager.SetDragCallback(DragStart2, Draging, DragEnd2);
            foreach (var drager in _ILDragers3)
                drager.SetDragCallback(DragStart3, Draging, DragEnd3);
        }
        private void DragStart(Vector3 position, int type, int index)
        {
            _ILDragers1[index].transform.SetAsLastSibling();
            Debug.Log("DragStart1执行成功");
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            _ILDragers2[index].transform.SetAsLastSibling();
            Debug.Log("DragStart1执行成功");
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            _ILDragers3[index].transform.SetAsLastSibling();
            Debug.Log("DragStart1执行成功");
        }
        private void Draging(Vector3 position, int type, int index)
        {
            Debug.Log("Draging执行成功");
        }
      
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(" DragEnd执行成功");
            if (isMatch)
            {
                if (index == 0)
                {
                    FGindex1++;
                    _Level1.GetChild(4).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex1 == 2)
                    {

                        _Level1.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 1)
                {
                    FGindex1++;
                    _Level1.GetChild(4).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex1 == 2)
                    {
                        _Level1.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 2)
                {
                    FGindex2++;
                    _Level1.GetChild(5).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex2 == 2)
                    {
                        _Level1.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 3)
                {
                    FGindex2++;
                    _Level1.GetChild(5).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex2 == 2)
                    {
                        _Level1.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 4)
                {
                    FGindex3++;
                    _Level1.GetChild(6).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex3 == 2)
                    {
                        _Level1.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 5)
                {
                    FGindex3++;
                    _Level1.GetChild(6).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex3 == 2)
                    {
                        _Level1.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 6)
                {
                    FGindex4++;
                    _Level1.GetChild(7).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex4 == 2)
                    {
                        _Level1.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 7)
                {
                    FGindex4++;
                    _Level1.GetChild(7).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex4 == 2)
                    {
                        _Level1.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                CheckFG();
                _ILDragers1[index].gameObject.SetActive(false);
                OnDragRightNum++;
                if (OnDragRightNum == 8)
                {
                    //成功效果 吸水 变大
                    _Level1SpineCZ();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2,false);
                    PlaySpine(_Level1.GetChild(9).gameObject, "A10", () =>
                    {
                        PlaySpine(_Level1.GetChild(9).gameObject, "A1", null, true);
                        _Level1SpineCZ();
                    }, false);
                    _Level1.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).OnComplete(()=> { AttackXEM(4); });
                    //延迟后出现下一关
                    Delay(3f, () =>
                    {
                        FGindex1 = 0;FGindex2 = 0;FGindex3 = 0;FGindex4 = 0;
                        _Level1.gameObject.SetActive(false);
                        _Level2.gameObject.SetActive(true);
                        Drag1.gameObject.SetActive(false);
                        Drag2.gameObject.SetActive(true);
                        Drop1.gameObject.SetActive(false);
                        Drop2.gameObject.SetActive(true);
                        OnDragRightNum = 0;
                    });
                }
            }
            else
            {
                _ILDragers1[index].DoReset();
                mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 1,()=> { _mask.gameObject.SetActive(true); },()=> { _mask.gameObject.SetActive(false); }));
                
            }

        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(" DragEnd执行成功");
            if (isMatch)
            {
                if (index == 0)
                {
                    FGindex1++;
                    _Level2.GetChild(4).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex1 == 2)
                    {
                        _Level2.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 1)
                {
                    FGindex1++;
                    _Level2.GetChild(4).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex1 == 2)
                    {
                        _Level2.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 2)
                {
                    FGindex2++;
                    _Level2.GetChild(5).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex2 == 2)
                    {
                        _Level2.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 3)
                {
                    FGindex2++;
                    _Level2.GetChild(5).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex2 == 2)
                    {
                        _Level2.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 4)
                {
                    FGindex3++;
                    _Level2.GetChild(6).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex3 == 2)
                    {
                        _Level2.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 5)
                {
                    FGindex3++;
                    _Level2.GetChild(6).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex3 == 2)
                    {
                        _Level2.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 6)
                {
                    FGindex4++;
                    _Level2.GetChild(7).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex4 == 2)
                    {
                        _Level2.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 7)
                {
                    FGindex4++;
                    _Level2.GetChild(7).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex4 == 2)
                    {
                        _Level2.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                CheckFG2();
                _ILDragers2[index].gameObject.SetActive(false);
                OnDragRightNum++;
                if (OnDragRightNum == 8)
                {
                    //成功效果 吸水 变大
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    _Level2SpineCZ();
                    PlaySpine(_Level2.GetChild(9).gameObject, "B10", () =>
                    {
                        PlaySpine(_Level2.GetChild(9).gameObject, "B1", null, true);
                        _Level2SpineCZ();
                    }, false);
                    _Level2.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).OnComplete(() => { AttackXEM(3); }); ;
                    //延迟后出现下一关
                    Delay(3f, () =>
                    {
                        FGindex1 = 0; FGindex2 = 0; FGindex3 = 0; FGindex4 = 0;
                        _Level2.gameObject.SetActive(false);
                        _Level3.gameObject.SetActive(true);
                        Drag2.gameObject.SetActive(false);
                        Drag3.gameObject.SetActive(true);
                        Drop2.gameObject.SetActive(false);
                        Drop3.gameObject.SetActive(true);
                        OnDragRightNum = 0;
                    });
                }
            }
            else
            {
                _ILDragers2[index].DoReset();
                mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 1, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
            }

        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(" DragEnd执行成功");
            if (isMatch)
            {
                if (index == 0)
                {
                    FGindex1++;
                    _Level3.GetChild(4).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex1 == 2)
                    {
                        _Level3.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 1)
                {
                    FGindex1++;
                    _Level3.GetChild(4).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex1 == 2)
                    {
                        _Level3.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 2)
                {
                    FGindex2++;
                    _Level3.GetChild(5).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex2 == 2)
                    {
                        _Level3.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 3)
                {
                    FGindex2++;
                    _Level3.GetChild(5).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex2 == 2)
                    {
                        _Level3.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 4)
                {
                    FGindex3++;
                    _Level3.GetChild(6).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex3 == 2)
                    {
                        _Level3.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 5)
                {
                    FGindex3++;
                    _Level3.GetChild(6).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex3 == 2)
                    {
                        _Level3.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 6)
                {
                    FGindex4++;
                    _Level3.GetChild(7).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex4 == 2)
                    {
                        _Level1.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                if (index == 7)
                {
                    FGindex4++;
                    _Level3.GetChild(7).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    if (FGindex4 == 2)
                    {
                        _Level1.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 0, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
                    }
                }
                CheckFG3();
                _ILDragers3[index].gameObject.SetActive(false);
                OnDragRightNum++;
                if (OnDragRightNum == 8)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,3, false);
                    //成功效果 吸水 变大
                    _Level3SpineCZ();
                    PlaySpine(_Level3.GetChild(9).gameObject, "C10", () =>
                    {
                      
                        PlaySpine(_Level3.GetChild(9).gameObject, "C1", null, true);
                        _Level3SpineCZ();
                    }, false);
                    _Level3.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).OnComplete(() =>
                    {
                        _Level3.DOScale(new Vector3(1,1,1),1f);
                        PlaySpine(_Level3.GetChild(9).gameObject, "C11", ()=> 
                        {
                            AttackXEM(2);
                  
                            PlaySpine(_Level3.GetChild(9).gameObject, "C1",null, true);
                            _Level3SpineCZ();
                        }, false);
                        _Level3SpineCZ();
                    });
                    //延迟后出现下一关
                    Delay(5f, () =>
                    {
                        FGindex1 = 0; FGindex2 = 0; FGindex3 = 0; FGindex4 = 0;

                        playSuccessSpine();
                        OnDragRightNum = 0;
                    });
                }
            }
            else
            {
                _ILDragers3[index].DoReset();
                mono.StartCoroutine(SpeckerCoroutinett(bd, SoundManager.SoundType.SOUND, 1, () => { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); }));
            }

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
        IEnumerator SpeckerCoroutinett(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
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
                    mono.StartCoroutine(SpeckerCoroutinett(bd,SoundManager.SoundType.VOICE,1,null,()=> 
                    {
                        bd.gameObject.SetActive(false);
                        mask.SetActive(false);
                    }));
                   
                 //   TDGameStartFunc();
                    
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
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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
            string result = string.Empty;
            hasbeenOnClick = false;
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
            if (hasbeenOnClick == false) 
            {
                hasbeenOnClick = true;
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); CW();  });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutinett(dbd, SoundManager.SoundType.VOICE, 4)); });
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

        #endregion


        #endregion

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }
        private void CW() 
        {
           
            PlaySpine(boom.gameObject, "kong", null, false);
            UI.GetChild(1).gameObject.SetActive(true); UI.GetChild(2).gameObject.SetActive(true); UI.GetChild(3).gameObject.SetActive(true); UI.GetChild(4).gameObject.SetActive(true);

            _Level1.DOScale(new Vector3(1, 1, 1), 0.1f); _Level2.DOScale(new Vector3(1, 1, 1), 0.1f); _Level3.DOScale(new Vector3(1, 1, 1), 0.1f);
            _Level1.gameObject.SetActive(true); _Level2.gameObject.SetActive(false); _Level3.gameObject.SetActive(false);
            Drag1.gameObject.SetActive(true); Drag2.gameObject.SetActive(false); Drag3.gameObject.SetActive(false); 
            Drop1.gameObject.SetActive(true); Drop2.gameObject.SetActive(false); Drop3.gameObject.SetActive(false);
            PlaySpine(xem.gameObject,"xem",null,true);
            _mask.gameObject.SetActive(false);
            FGindex1 = 0; FGindex2 = 0; FGindex3 = 0; FGindex4 = 0;
           
          
            //外表光
            _Level1.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1 ,1, 1, 0);
            _Level1.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            //色块
            _Level1.GetChild(4).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(4).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(5).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(5).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(6).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(6).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(7).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level1.GetChild(7).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            //外表光
            _Level2.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            //色块
            _Level2.GetChild(4).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(4).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(5).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(5).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(6).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(6).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(7).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level2.GetChild(7).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            //外表光
            _Level3.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            //色块
            _Level3.GetChild(4).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(4).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(5).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(5).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(6).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(6).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(7).GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            _Level3.GetChild(7).GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);

            CZPosition();
            InitLayers();
            CZPos();
          
        }
        private void _Level1SpineCZ() 
        {
            PlaySpine(_Level1.GetChild(1).gameObject, "G1", null, true);
            PlaySpine(_Level1.GetChild(2).gameObject, "G2", null, true);
            PlaySpine(_Level1.GetChild(3).gameObject, "G3", null, true);
            PlaySpine(_Level1.GetChild(8).gameObject, "G4", null, true);



            PlaySpine(_Level1.GetChild(4).GetChild(0).gameObject,"A2",null,true);
            PlaySpine(_Level1.GetChild(4).GetChild(1).gameObject, "A6", null, true);
            PlaySpine(_Level1.GetChild(4).gameObject, "X1", null, true);

            PlaySpine(_Level1.GetChild(5).GetChild(0).gameObject, "A3", null, true);
            PlaySpine(_Level1.GetChild(5).GetChild(1).gameObject, "A7", null, true);
            PlaySpine(_Level1.GetChild(5).gameObject, "X2", null, true);

            PlaySpine(_Level1.GetChild(6).GetChild(0).gameObject, "A4", null, true);
            PlaySpine(_Level1.GetChild(6).GetChild(1).gameObject, "A8", null, true);
            PlaySpine(_Level1.GetChild(6).gameObject, "X3", null, true);

            PlaySpine(_Level1.GetChild(7).GetChild(0).gameObject, "A5", null, true);
            PlaySpine(_Level1.GetChild(7).GetChild(1).gameObject, "A9", null, true);
            PlaySpine(_Level1.GetChild(7).gameObject, "X4", null, true);

        }
        private void _Level2SpineCZ()
        {

            PlaySpine(_Level2.GetChild(1).gameObject, "G1", null, true);
            PlaySpine(_Level2.GetChild(2).gameObject, "G2", null, true);
            PlaySpine(_Level2.GetChild(3).gameObject, "G3", null, true);
            PlaySpine(_Level2.GetChild(8).gameObject, "G4", null, true);

            PlaySpine(_Level2.GetChild(4).GetChild(0).gameObject, "B2", null, true);
            PlaySpine(_Level2.GetChild(4).GetChild(1).gameObject, "B6", null, true);
            PlaySpine(_Level2.GetChild(4).gameObject, "X1", null, true);

            PlaySpine(_Level2.GetChild(5).GetChild(0).gameObject, "B3", null, true);
            PlaySpine(_Level2.GetChild(5).GetChild(1).gameObject, "B7", null, true);
            PlaySpine(_Level2.GetChild(5).gameObject, "X2", null, true);

            PlaySpine(_Level2.GetChild(6).GetChild(0).gameObject, "B4", null, true);
            PlaySpine(_Level2.GetChild(6).GetChild(1).gameObject, "B8", null, true);
            PlaySpine(_Level2.GetChild(6).gameObject, "X3", null, true);

            PlaySpine(_Level2.GetChild(7).GetChild(0).gameObject, "B5", null, true);
            PlaySpine(_Level2.GetChild(7).GetChild(1).gameObject, "B9", null, true);
            PlaySpine(_Level2.GetChild(7).gameObject, "X4", null, true);

        }
        private void _Level3SpineCZ()
        {

            PlaySpine(_Level3.GetChild(1).gameObject, "G1", null, true);
            PlaySpine(_Level3.GetChild(2).gameObject, "G2", null, true);
            PlaySpine(_Level3.GetChild(3).gameObject, "G3", null, true);
            PlaySpine(_Level3.GetChild(8).gameObject, "G4", null, true);

            PlaySpine(_Level3.GetChild(4).GetChild(0).gameObject, "C2", null, true);
            PlaySpine(_Level3.GetChild(4).GetChild(1).gameObject, "C6", null, true);
            PlaySpine(_Level3.GetChild(4).gameObject, "X1", null, true);

            PlaySpine(_Level3.GetChild(5).GetChild(0).gameObject, "C3", null, true);
            PlaySpine(_Level3.GetChild(5).GetChild(1).gameObject, "C7", null, true);
            PlaySpine(_Level3.GetChild(5).gameObject, "X2", null, true);

            PlaySpine(_Level3.GetChild(6).GetChild(0).gameObject, "C4", null, true);
            PlaySpine(_Level3.GetChild(6).GetChild(1).gameObject, "C8", null, true);
            PlaySpine(_Level3.GetChild(6).gameObject, "X3", null, true);

            PlaySpine(_Level3.GetChild(7).GetChild(0).gameObject, "C5", null, true);
            PlaySpine(_Level3.GetChild(7).GetChild(1).gameObject, "C9", null, true);
            PlaySpine(_Level3.GetChild(7).gameObject, "X4", null, true);

        }

        private void CheckFG() 
        {
            if (FGindex1 == 2) 
            {
               
                _Level1.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex2 == 2) 
            {
                _Level1.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex3 == 2)
            {
                _Level1.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex4 == 2)
            {
                _Level1.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
        }
        private void CheckFG2()
        {
            if (FGindex1 == 2)
            {
                _Level2.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex2 == 2)
            {
                _Level2.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex3 == 2)
            {
                _Level2.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex4 == 2)
            {
                _Level2.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
        }
        private void CheckFG3()
        {
            if (FGindex1 == 2)
            {
                _Level3.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex2 == 2)
            {
                _Level3.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex3 == 2)
            {
                _Level3.GetChild(3).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
            if (FGindex4 == 2)
            {
                _Level3.GetChild(8).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            }
        }

        private void AttackXEM(int i)
        {
            PlaySpine(boom.gameObject,"xem-boom",()=> 
            {
                UI.GetChild(i).gameObject.SetActive(false);
                PlaySpine(boom.gameObject, "kong", null, false);
                if (i == 2) 
                {
                    PlaySpine(xem.gameObject,"xem-y",null,false);
                }
            },false);
        }
        private void InitLayers() 
        {
            drag1_se1.transform.SetAsFirstSibling();
            drag1_se2.transform.SetSiblingIndex(1);
            drag1_se3.transform.SetSiblingIndex(2);
            drag1_se4.transform.SetSiblingIndex(3);
            drag1_se5.transform.SetSiblingIndex(4);
            drag1_se6.transform.SetSiblingIndex(5);
            drag1_se7.transform.SetSiblingIndex(6);
            drag1_se8.transform.SetAsLastSibling();

            drag2_se1.transform.SetAsFirstSibling();
            drag2_se2.transform.SetSiblingIndex(1);
            drag2_se3.transform.SetSiblingIndex(2);
            drag2_se4.transform.SetSiblingIndex(3);
            drag2_se5.transform.SetSiblingIndex(4);
            drag2_se6.transform.SetSiblingIndex(5);
            drag2_se7.transform.SetSiblingIndex(6);
            drag2_se8.transform.SetAsLastSibling();

            drag3_se1.transform.SetAsFirstSibling();
            drag3_se2.transform.SetSiblingIndex(1);
            drag3_se3.transform.SetSiblingIndex(2);
            drag3_se4.transform.SetSiblingIndex(3);
            drag3_se5.transform.SetSiblingIndex(4);
            drag3_se6.transform.SetSiblingIndex(5);
            drag3_se7.transform.SetSiblingIndex(6);
            drag3_se8.transform.SetAsLastSibling();
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }
        private void CZPosition() 
        {
            InitLayers();
            for (int i = 0; i < Drag1.childCount; i++)
            {
                //_ILDragers1[i].DoReset();
                _ILDragers1[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < Drag2.childCount; i++)
            {
               // _ILDragers2[i].DoReset();
                _ILDragers2[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < Drag3.childCount; i++)
            {
                //_ILDragers3[i].DoReset();
                _ILDragers3[i].gameObject.SetActive(true);
            }
        }
        private void CZPos() 
        {
            Drag1.GetChild(0).GetRectTransform().position = curTrans.Find("se8Pos").position;
            Drag1.GetChild(1).GetRectTransform().position = curTrans.Find("se7Pos").position;
            Drag1.GetChild(2).GetRectTransform().position = curTrans.Find("se3Pos").position;
            Drag1.GetChild(3).GetRectTransform().position = curTrans.Find("se6Pos").position;
            Drag1.GetChild(4).GetRectTransform().position = curTrans.Find("se5Pos").position;
            Drag1.GetChild(5).GetRectTransform().position = curTrans.Find("se4Pos").position;
            Drag1.GetChild(6).GetRectTransform().position = curTrans.Find("se2Pos").position;
            Drag1.GetChild(7).GetRectTransform().position = curTrans.Find("se1Pos").position;

            Drag2.GetChild(0).GetRectTransform().position = curTrans.Find("se5Pos").position;
            Drag2.GetChild(1).GetRectTransform().position = curTrans.Find("se8Pos").position;
            Drag2.GetChild(2).GetRectTransform().position = curTrans.Find("se6Pos").position;
            Drag2.GetChild(3).GetRectTransform().position = curTrans.Find("se4Pos").position;
            Drag2.GetChild(4).GetRectTransform().position = curTrans.Find("se1Pos").position;
            Drag2.GetChild(5).GetRectTransform().position = curTrans.Find("se3Pos").position;
            Drag2.GetChild(6).GetRectTransform().position = curTrans.Find("se7Pos").position;
            Drag2.GetChild(7).GetRectTransform().position = curTrans.Find("se2Pos").position;


            Drag3.GetChild(0).GetRectTransform().position = curTrans.Find("se6Pos").position;
            Drag3.GetChild(1).GetRectTransform().position = curTrans.Find("se4Pos").position;
            Drag3.GetChild(2).GetRectTransform().position = curTrans.Find("se8Pos").position;
            Drag3.GetChild(3).GetRectTransform().position = curTrans.Find("se2Pos").position;
            Drag3.GetChild(4).GetRectTransform().position = curTrans.Find("se5Pos").position;
            Drag3.GetChild(5).GetRectTransform().position = curTrans.Find("se1Pos").position;
            Drag3.GetChild(6).GetRectTransform().position = curTrans.Find("se7Pos").position;
            Drag3.GetChild(7).GetRectTransform().position = curTrans.Find("se3Pos").position;
        }
    }
}
