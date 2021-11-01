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
        next,
    }
    public class TD6721Part5
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
        private GameObject DaTian;
        private Text DaTianText;

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
        private Transform CGSpine;
        private Transform BD2;
        private Transform XEM;
        private List<mILDrager> _iLDragers;
        private List<mILDrager> _iLDragers2;
        private List<mILDrager> _iLDragers3;
        private List<mILDrager> _iLDragers4;
        private Transform Drag;
        private Transform Drag2;
        private Transform Drag3;
        private Transform Drag4;
        private Transform Droper;
        private Transform Droper2;
        private Transform Droper3;
        private Transform Droper4;
        private Transform Panel;
        private Transform flowerSpines;
        private Transform flowerSpine;
        private bool _canClick;
        private int GainCount;
        bool isPlaying = false;
        //第二关
        private Transform L1_color;
        private Transform L1_color2;
        private Transform L1_color3;
        private Transform L1_color4;
        private Transform L2_color;
        private Transform L2_color2;
        private Transform L2_color3;
        private Transform L2_color4;
        private Transform L2_color5;
        private Transform L3_color;
        private Transform L3_color2;
        private Transform L3_color3;
        private Transform L3_color4;
        private Transform L3_color5;
        private Transform L3_color6;
        private Transform bg2;
        private int GainCount2;
        private Transform yunlight;
        private Transform yunlight2;
        private Transform yunlight3;
        private Transform yunface;
        private Transform yunface2;
        private Transform yunface3;
        private Transform yun;
        private Transform yun2;
        private Transform yun3;
        private Transform rain;
        private Transform grass;
        private Transform _mask;
        private int Level_index;
        bool hasbeenOnClick = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            XEM = curTrans.Find("mask/xem");
            BD2 = curTrans.Find("mask/BD2");
            Drag = curTrans.Find("Drag");
            Drag2 = curTrans.Find("Drag2");
            Drag3 = curTrans.Find("Drag3");
            Drag4 = curTrans.Find("Drag4");
            Droper = curTrans.Find("Droper");
            Droper2 = curTrans.Find("Droper2");
            Droper3 = curTrans.Find("Droper3");
            Droper4 = curTrans.Find("Droper4");
            Panel = curTrans.Find("Panel");
            flowerSpine = curTrans.Find("flowerSpine");
            flowerSpines = curTrans.Find("flowerSpines");
            _canClick = false;
            GainCount = 0;
            //第二关游戏
            L1_color = curTrans.Find("yun/color");
            L1_color2 = curTrans.Find("yun/color2");
            L1_color3 = curTrans.Find("yun/color3");
            L1_color4 = curTrans.Find("yun/color4");
            L2_color = curTrans.Find("yun2/color");
            L2_color2 = curTrans.Find("yun2/color2");
            L2_color3 = curTrans.Find("yun2/color3");
            L2_color4 = curTrans.Find("yun2/color4");
            L2_color5 = curTrans.Find("yun2/color5");
            L3_color = curTrans.Find("yun3/color");
            L3_color2 = curTrans.Find("yun3/color2");
            L3_color3 = curTrans.Find("yun3/color3");
            L3_color4 = curTrans.Find("yun3/color4");
            L3_color5 = curTrans.Find("yun3/color5");
            L3_color6 = curTrans.Find("yun3/color6");
            CGSpine = curTrans.Find("CGSpine");
            bg2 = curTrans.Find("Bg2");
            yun = curTrans.Find("yun");
            yun2 = curTrans.Find("yun2");
            yun3 = curTrans.Find("yun3");
            rain = curTrans.Find("Bg2/rain");
            GainCount2 = 0;
            yunlight = curTrans.Find("yun/yunlight");
            yunlight2 = curTrans.Find("yun2/yunlight");
            yunlight3 = curTrans.Find("yun3/yunlight");
            yunface = curTrans.Find("yun/yunface");
            yunface2 = curTrans.Find("yun2/yunface");
            yunface3 = curTrans.Find("yun3/yunface");
            rain = curTrans.Find("Bg2/rain");
            Level_index = 0;
            grass = curTrans.Find("Bg2/grass1");
            _mask = curTrans.Find("_mask");
            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);


            GameInit();
            //GameStart();
            InitILDragers();
            AddDragersEvent();
        }






        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            Input.multiTouchEnabled = false;
            bg2.GetChild(0).gameObject.SetActive(true); bg2.GetChild(1).gameObject.SetActive(true); bg2.GetChild(2).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(bg2.GetChild(3).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(bg2.GetChild(4).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(bg2.GetChild(5).gameObject, "kong", true);
            Level_index = 0;
            DaTianText.text = "";
            devilText.text = "";
            _mask.gameObject.SetActive(false);
            Drag.gameObject.SetActive(false);
            Droper.gameObject.SetActive(false);
            Drag2.gameObject.SetActive(false);
            Droper2.gameObject.SetActive(false);
            Drag3.gameObject.SetActive(false);
            Droper3.gameObject.SetActive(false);
            Drag4.gameObject.SetActive(false);
            Droper4.gameObject.SetActive(false);

            flowerSpine.gameObject.SetActive(true);
            flowerSpines.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(CGSpine.gameObject, "kong", true);
            yun.gameObject.SetActive(false);
            yun2.gameObject.SetActive(false);
            yun3.gameObject.SetActive(false);


            DaTian.transform.position = bdStartPos.position;
            devil.transform.position = devilStartPos.position;

            bg2.Find("CountGain/count2").gameObject.SetActive(false);
            bg2.Find("CountGain/count1").gameObject.SetActive(false);
            bg2.Find("CountGain/count0").gameObject.SetActive(false);
            //田丁初始化
            TDGameInit();
        }



        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();
            // mask.SetActive(false);
            _canClick = true;
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

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            //重玩

            flowerSpine.gameObject.SetActive(true);
            flowerSpines.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(CGSpine.gameObject, "kong", true);
            bg2.gameObject.SetActive(false);
            yun3.gameObject.SetActive(false);
            Drag4.gameObject.SetActive(false);
            Droper4.gameObject.SetActive(false);

            //Drag拖拽的物体归位
            Drag.gameObject.SetActive(true);
            Droper.gameObject.SetActive(true);
            for (int i = 0; i < Drag.childCount; i++)
            {
                Drag.GetChild(i).gameObject.SetActive(true);
            }
            Drag.GetChild(0).position = curTrans.Find("a").position;
            Drag.GetChild(1).position = curTrans.Find("b").position;
            Drag.GetChild(2).position = curTrans.Find("c").position;
            Drag.GetChild(3).position = curTrans.Find("d").position;
            Drag.GetChild(4).position = curTrans.Find("e").position;
            Drag.GetChild(5).position = curTrans.Find("f").position;

            SpineManager.instance.DoAnimation(flowerSpines.transform.GetChild(0).gameObject, "p-a2", true);
            SpineManager.instance.DoAnimation(flowerSpines.transform.GetChild(1).gameObject, "p-b2", true);
            SpineManager.instance.DoAnimation(flowerSpines.transform.GetChild(2).gameObject, "p-c2", true);
            SpineManager.instance.DoAnimation(flowerSpines.transform.GetChild(3).gameObject, "p-d2", true);
            SpineManager.instance.DoAnimation(flowerSpines.transform.GetChild(4).gameObject, "p-e2", true);
            SpineManager.instance.DoAnimation(flowerSpines.transform.GetChild(5).gameObject, "p-f2", true);

            //云朵通用
            SpineManager.instance.DoAnimation(bg2.GetChild(0).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(bg2.GetChild(1).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(bg2.GetChild(2).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(rain.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(grass.gameObject, "kong", true);
            //云朵关卡1
            SpineManager.instance.DoAnimation(L1_color.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L1_color2.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L1_color3.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L1_color4.gameObject, "kong", true);

            Drag2.GetChild(0).position = curTrans.Find("g1").position;
            Drag2.GetChild(1).position = curTrans.Find("g2").position;
            Drag2.GetChild(2).position = curTrans.Find("g3").position;
            Drag2.GetChild(3).position = curTrans.Find("g4").position;

            for (int i = 0; i < Drag2.childCount; i++)
            {
                Drag2.GetChild(i).gameObject.SetActive(true);
            }
            //云朵关卡2
            SpineManager.instance.DoAnimation(L2_color.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L2_color2.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L2_color3.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L2_color4.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L2_color5.gameObject, "kong", true);
            Drag3.GetChild(0).position = curTrans.Find("z1").position;
            Drag3.GetChild(1).position = curTrans.Find("z2").position;
            Drag3.GetChild(2).position = curTrans.Find("z3").position;
            Drag3.GetChild(3).position = curTrans.Find("z4").position;
            Drag3.GetChild(4).position = curTrans.Find("z5").position;
            for (int i = 0; i < Drag3.childCount; i++)
            {
                Drag3.GetChild(i).gameObject.SetActive(true);
            }
            //云朵关卡3
            SpineManager.instance.DoAnimation(L3_color.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L3_color2.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L3_color3.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L3_color4.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L3_color5.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(L3_color6.gameObject, "kong", true);
            Drag4.GetChild(0).position = curTrans.Find("b1").position;
            Drag4.GetChild(1).position = curTrans.Find("b2").position;
            Drag4.GetChild(2).position = curTrans.Find("b3").position;
            Drag4.GetChild(3).position = curTrans.Find("b4").position;
            Drag4.GetChild(4).position = curTrans.Find("b5").position;
            Drag4.GetChild(5).position = curTrans.Find("b6").position;

            for (int i = 0; i < Drag4.childCount; i++)
            {
                Drag4.GetChild(i).gameObject.SetActive(true);
            }
        }
        void TDGameStart()
        {
            DaTian.gameObject.SetActive(true);
            devil.gameObject.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            //mono.StartCoroutine(SpeckerCoroutine(XEM.gameObject, SoundManager.SoundType.SOUND, 0, ()=> 
            //{ 
            //    XEM.gameObject.SetActive(true); SpineManager.instance.DoAnimation(XEM.gameObject,"speak",true);
            //}, ()=> 
            //{
            //   SpineManager.instance.DoAnimation(XEM.gameObject, "daiji", true);
            //    SoundManager.instance.ShowVoiceBtn(true);
            //})); ;
            DaTian.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 6, () =>
                {
                    ShowDialogue("终于到清水湖了！仙子快去吧", DaTianText);
                }, () =>
                {
                    devil.gameObject.SetActive(true);
                    devil.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 0, () =>
                        {
                            ShowDialogue("想恢复美貌？不可能的！哈哈哈！", devilText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));
            });
        }
        void InitILDragers()
        {
            _iLDragers = new List<mILDrager>();
            for (int i = 0; i < Drag.childCount; i++)
            {

                var iLDrager = Drag.GetChild(i).GetComponent<mILDrager>();
                _iLDragers.Add(iLDrager);
            }

            _iLDragers2 = new List<mILDrager>();
            for (int i = 0; i < Drag2.childCount; i++)
            {

                var iLDrager = Drag2.GetChild(i).GetComponent<mILDrager>();
                _iLDragers2.Add(iLDrager);
            }
            _iLDragers3 = new List<mILDrager>();
            for (int i = 0; i < Drag3.childCount; i++)
            {

                var iLDrager = Drag3.GetChild(i).GetComponent<mILDrager>();
                _iLDragers3.Add(iLDrager);
            }
            _iLDragers4 = new List<mILDrager>();
            for (int i = 0; i < Drag4.childCount; i++)
            {

                var iLDrager = Drag4.GetChild(i).GetComponent<mILDrager>();
                _iLDragers4.Add(iLDrager);
            }
        }
        private void AddDragersEvent()
        {

            foreach (var drager in _iLDragers)
                drager.SetDragCallback(DragStart, Draging, DragEnd);
            foreach (var drager in _iLDragers2)
                drager.SetDragCallback(DragStart2, Draging2, DragEnd2);
            foreach (var drager in _iLDragers3)
                drager.SetDragCallback(DragStart2, Draging2, DragEnd3);
            foreach (var drager in _iLDragers4)
                drager.SetDragCallback(DragStart2, Draging2, DragEnd4);
        }
        private void DragStart(Vector3 position, int type, int index)
        {
            BtnPlaySound();
            Debug.Log("DragStart1执行成功");
            if (index == 0)
            {
                SpineManager.instance.DoAnimation(Panel.GetChild(0).gameObject, "a", false);

            }
            else if (index == 1)
            {
                SpineManager.instance.DoAnimation(Panel.GetChild(1).gameObject, "b", false);
            }
            else if (index == 2)
            {
                SpineManager.instance.DoAnimation(Panel.GetChild(2).gameObject, "c", false);
            }
            else if (index == 3)
            {
                SpineManager.instance.DoAnimation(Panel.GetChild(3).gameObject, "d", false);
            }
            else if (index == 4)
            {
                SpineManager.instance.DoAnimation(Panel.GetChild(4).gameObject, "e", false);
            }
            else if (index == 5)
            {
                SpineManager.instance.DoAnimation(Panel.GetChild(5).gameObject, "f", false);
            }
        }
        private void Draging(Vector3 position, int type, int index)
        {

            Debug.Log("Draging执行成功");
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {


            Debug.Log(" DragEnd执行成功");


            if (type == 1 && isMatch && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                _canClick = false;
                _mask.gameObject.SetActive(true);
                Drag.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpines.GetChild(index).gameObject, "p-a", false, () => { _canClick = true; _mask.gameObject.SetActive(false); });
                GainCount++;
            }
            else if (type == 2 && isMatch && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                _canClick = false;
                _mask.gameObject.SetActive(true);
                Drag.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpines.GetChild(index).gameObject, "p-b", false, () => { _canClick = true; _mask.gameObject.SetActive(false); });
                GainCount++;
            }
            else if (type == 3 && isMatch && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                _canClick = false;
                _mask.gameObject.SetActive(true);
                Drag.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpines.GetChild(index).gameObject, "p-c", false, () => { _canClick = true; _mask.gameObject.SetActive(false); });
                GainCount++;
            }
            else if (type == 4 && isMatch && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                _canClick = false;
                _mask.gameObject.SetActive(true);
                Drag.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpines.GetChild(index).gameObject, "p-d", false, () => { _canClick = true; _mask.gameObject.SetActive(false); });
                GainCount++;
            }
            else if (type == 5 && isMatch && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                _canClick = false;
                _mask.gameObject.SetActive(true);
                Drag.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpines.GetChild(index).gameObject, "p-e", false, () => { _canClick = true; _mask.gameObject.SetActive(false); });
                GainCount++;
            }
            else if (type == 6 && isMatch && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                _canClick = false;
                _mask.gameObject.SetActive(true);
                Drag.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpines.GetChild(index).gameObject, "p-f", false, () => { _canClick = true; _mask.gameObject.SetActive(false); });
                GainCount++;
            }
            else if (isMatch == false || _canClick == false)
            {
                _iLDragers[index].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
            }

            if (GainCount == 6)
            {
                //做一个iewait协程
                //nextLevel
                GainCount = 0;
                mono.StartCoroutine(waitsecond(0.5f, () =>
                {
                    flowerSpine.gameObject.SetActive(false);
                    flowerSpines.gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    SpineManager.instance.DoAnimation(CGSpine.gameObject, "p-g", false, () => { SpineManager.instance.DoAnimation(CGSpine.gameObject, "p-g2", true); });

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
                }));

                mono.StartCoroutine(waitsecond(4f, () =>
                {
                    mask.SetActive(true);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                    anyBtns.GetChild(1).gameObject.SetActive(false);
                }));

            }
        }

        private void DragStart2(Vector3 position, int type, int index)
        {

        }
        private void Draging2(Vector3 position, int type, int index)
        {

        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {

            Debug.Log(isMatch);
            if (type == 1 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag2.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L1_color.gameObject, "H4", false);
                GainCount2++;
            }
            else if (type == 2 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag2.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L1_color2.gameObject, "H3", false);
                GainCount2++;
            }
            else if (type == 3 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag2.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L1_color3.gameObject, "H2", false);
                GainCount2++;
            }
            else if (type == 4 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag2.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L1_color4.gameObject, "H1", false);
                GainCount2++;
            }
            else
            {
                // _canClick = false;
                _iLDragers2[index].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                // SpineManager.instance.DoAnimation(yunface.gameObject,"yun-2",false,()=> { _canClick = true; });
            }
            if (GainCount2 == 4)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                SpineManager.instance.DoAnimation(rain.gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(rain.gameObject, "R-G", true); });

                SpineManager.instance.DoAnimation(grass.gameObject, "c1", true);
                SpineManager.instance.DoAnimation(yunface.gameObject, "yun-3", true);
                SpineManager.instance.DoAnimation(yunlight.gameObject, "yun-light", false, () =>
                   {
                       mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 7, null, () =>
                        {
                           SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);//云朵和闪电分开
                        mono.StartCoroutine(waitsecond(0.5f, () => { bg2.GetChild(2).gameObject.SetActive(false); }));
                           SpineManager.instance.DoAnimation(bg2.GetChild(5).gameObject, "xem2", false, () =>
                           {
                               mono.StartCoroutine(waitsecond(1f, () =>
                             {
                                   GainCount2 = 0;
                                   bg2.Find("CountGain/count2").gameObject.SetActive(true);
                                   bg2.Find("CountGain/count3").gameObject.SetActive(false);
                                   mask.SetActive(true);
                                //下一关
                                anyBtns.GetChild(0).gameObject.Show();
                                   anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                   anyBtns.GetChild(1).gameObject.SetActive(false);

                               }));


                           });

                       }));

                    // SpineManager.instance.DoAnimation(yunlight.gameObject,"kong",false);

                });
            }
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {


            if (type == 1 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag3.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L2_color.gameObject, "I5", false);
                GainCount2++;
            }
            else if (type == 2 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag3.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L2_color2.gameObject, "I4", false);
                GainCount2++;
            }
            else if (type == 3 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag3.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L2_color3.gameObject, "I3", false);
                GainCount2++;
            }
            else if (type == 4 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag3.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L2_color4.gameObject, "I2", false);
                GainCount2++;

            }
            else if (type == 5 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag3.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L2_color5.gameObject, "I1", false);
                GainCount2++;
            }
            else
            {
                _iLDragers3[index].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
            }
            Debug.Log(GainCount2);
            if (GainCount2 == 5)
            {
                GainCount2 = 0;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                SpineManager.instance.DoAnimation(rain.gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(rain.gameObject, "R-Z", true); });
                SpineManager.instance.DoAnimation(grass.gameObject, "c2", true);
                SpineManager.instance.DoAnimation(yunface2.gameObject, "yun-3", true);
                SpineManager.instance.DoAnimation(yunlight2.gameObject, "yun-light", false, () =>
                {

                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 8, null, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                            mono.StartCoroutine(waitsecond(1f, () => { bg2.GetChild(1).gameObject.SetActive(false); }));

                            SpineManager.instance.DoAnimation(bg2.GetChild(4).gameObject, "xem2", false, () =>
                            {
                                mono.StartCoroutine(waitsecond(1f, () =>
                             {
                                    GainCount2 = 0;
                                    bg2.Find("CountGain/count1").gameObject.SetActive(true);
                                    bg2.Find("CountGain/count2").gameObject.SetActive(false);
                                    mask.SetActive(true);
                                //下一关
                                anyBtns.GetChild(0).gameObject.Show();
                                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                }));


                            });

                        }));

                });


            }
        }
        private void DragEnd4(Vector3 position, int type, int index, bool isMatch)
        {
            if (type == 1 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag4.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L3_color.gameObject, "J6", false);
                GainCount2++;
            }
            else if (type == 2 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag4.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L3_color2.gameObject, "J5", false);
                GainCount2++;
            }
            else if (type == 3 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag4.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L3_color3.gameObject, "J4", false);
                GainCount2++;
            }
            else if (type == 4 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag4.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L3_color4.gameObject, "J3", false);
                GainCount2++;
            }
            else if (type == 5 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag4.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L3_color5.gameObject, "J2", false);
                GainCount2++;
            }
            else if (type == 6 && isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Drag4.GetChild(index).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(L3_color6.gameObject, "J1", false);
                GainCount2++;
            }
            else
            {
                _iLDragers4[index].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
            }
            if (GainCount2 == 6)
            {
                //
                GainCount2 = 0;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);
                SpineManager.instance.DoAnimation(rain.gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(rain.gameObject, "R-B", true); });
                SpineManager.instance.DoAnimation(grass.gameObject, "c3", true);
                SpineManager.instance.DoAnimation(yunface3.gameObject, "yun-3", true);
                SpineManager.instance.DoAnimation(yunlight3.gameObject, "yun-light", false, () =>
                {

                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 9, null, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7);
                            mono.StartCoroutine(waitsecond(1f, () => { bg2.GetChild(0).gameObject.SetActive(false); }));
                            SpineManager.instance.DoAnimation(bg2.GetChild(3).gameObject, "xem2", false, () =>
                            {
                                GainCount2 = 0;
                                bg2.Find("CountGain/count0").gameObject.SetActive(true);
                                bg2.Find("CountGain/count1").gameObject.SetActive(false);
                                bg2.Find("CountGain/count2").gameObject.SetActive(false);
                                bg2.Find("CountGain/XEM2").gameObject.SetActive(true);
                                bg2.Find("CountGain/XEM").gameObject.SetActive(false);

                            //下一关
                            mono.StartCoroutine(waitsecond(3f, () =>
                                {
                                    mask.SetActive(true);
                                    playSuccessSpine();
                                }));

                            });

                        }));


                });
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

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator waitsecond(float second, Action method_1 = null)
        {
            yield return new WaitForSeconds(second);
            method_1?.Invoke();
        }

        #endregion

        #region 语音键对应方法

        void TalkClick()
        {
            Debug.Log(talkIndex);
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //田丁游戏开始方法
                    TDGameStartFunc();
                    break;
                case 2:
                    mask.SetActive(true);
                    dbd.gameObject.SetActive(false);
                    BD2.gameObject.SetActive(true);

                    mono.StartCoroutine(SpeckerCoroutine(BD2.gameObject, SoundManager.SoundType.SOUND, 4, () =>
                    {
                        SpineManager.instance.DoAnimation(BD2.gameObject, "speak", true);
                        _canClick = false;
                    }, () =>
                    {
                        BD2.gameObject.SetActive(false);
                        mask.SetActive(false);
                        _canClick = true;
                    }));
                    SpineManager.instance.DoAnimation(bd.gameObject, "daiji", true);
                    bd.gameObject.SetActive(false);

                    bg2.gameObject.SetActive(true);
                    yun.gameObject.SetActive(true);
                    Drag2.gameObject.SetActive(true);
                    Droper2.gameObject.SetActive(true);
                    Drag.gameObject.SetActive(false);
                    Droper.gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(grass.gameObject, "c0", true);
                    SpineManager.instance.DoAnimation(bg2.GetChild(0).gameObject, "xem", true);
                    SpineManager.instance.DoAnimation(bg2.GetChild(1).gameObject, "xem", true);
                    SpineManager.instance.DoAnimation(bg2.GetChild(2).gameObject, "xem", true);
                    bg2.Find("CountGain").GetChild(0).gameObject.SetActive(true);
                    bg2.Find("CountGain").GetChild(1).gameObject.SetActive(true);
                    bg2.Find("CountGain").GetChild(4).gameObject.SetActive(false);
                    bg2.Find("CountGain").GetChild(5).gameObject.SetActive(false);



                    break;
            }

            talkIndex++;
        }

        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            buDing.SetActive(false);
            DaTian.SetActive(false);
            devil.SetActive(false);
            bd.SetActive(true);
            XEM.gameObject.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 1, null, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 2, null, () =>
                    {
                        mask.SetActive(false); bd.SetActive(false);
                    }));

            }));
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
        private void PlaySpineAni(GameObject target, string name, bool isLoop = false, Action callback = null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }

        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index, Action goingEvent = null, Action finishEvent = null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex, Action callback = null)
        {
            float voiceTimer = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
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
        void WaitTimeAndExcuteNext(float timer, Action callback)
        {

            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer, Action callBack)
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
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
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
            //    LoadSpineShow();

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
            DaTian = curTrans.Find("mask/DaTian").gameObject;//大田对话框
            DaTianText = DaTian.transform.GetChild(0).GetComponent<Text>();//大田文本
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
            SpineShow.gameObject.SetActive(true);
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
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
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
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name) + 1, null, () =>
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
                case BtnEnum.next:
                    result = "next";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            
            if (!hasbeenOnClick)
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            mask.SetActive(false);

                            GameInit();
                            talkIndex = 2;
                        });
                    }
                    else if (obj.name == "next")
                    {
                        if (Level_index == 0)
                        {
                        //下一关方法
                        anyBtns.GetChild(0).gameObject.Hide();
                            mask.SetActive(true);
                            dbd.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(dbd.gameObject, SoundManager.SoundType.SOUND, 3, () =>
                            {
                                SpineManager.instance.DoAnimation(dbd.gameObject, "speak", true);
                            }, () =>
                            {
                                SpineManager.instance.DoAnimation(dbd.gameObject, "daiji", true);
                                SoundManager.instance.ShowVoiceBtn(true);
                                Level_index++;
                            }));
                        }
                        else if (Level_index == 1)
                        {
                            anyBtns.GetChild(0).gameObject.Hide();
                            yun.gameObject.SetActive(false);
                            Drag2.gameObject.SetActive(false);
                            Droper2.gameObject.SetActive(false);
                            yun2.gameObject.SetActive(true);
                            Drag3.gameObject.SetActive(true);
                            Droper3.gameObject.SetActive(true);
                            mask.SetActive(false);
                            SpineManager.instance.DoAnimation(grass.gameObject, "c0", true);
                            SpineManager.instance.DoAnimation(rain.gameObject, "kong", true);
                            Level_index++;
                        }
                        else if (Level_index == 2)
                        {
                            anyBtns.GetChild(0).gameObject.Hide();
                            yun2.gameObject.SetActive(false);
                            Drag3.gameObject.SetActive(false);
                            Droper3.gameObject.SetActive(false);
                            yun3.gameObject.SetActive(true);
                            Drag4.gameObject.SetActive(true);
                            Droper4.gameObject.SetActive(true);
                            mask.SetActive(false);
                            SpineManager.instance.DoAnimation(grass.gameObject, "c0", true);
                            SpineManager.instance.DoAnimation(rain.gameObject, "kong", true);
                            Level_index++;
                        }

                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.SOUND, 5)); });
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
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }


        #endregion


        #endregion





    }
}
