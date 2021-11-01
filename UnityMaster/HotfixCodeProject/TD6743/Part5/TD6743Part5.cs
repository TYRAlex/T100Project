using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD6743Part5
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



        bool isPlaying = false;

        private GameObject Next;
        private bool _canNext;
        private int nextnumber;
        private GameObject _mask;

        private GameObject Level1;
        private GameObject Level2;
        private GameObject Level3;
        private GameObject Level4;
        private GameObject pao1;
        private GameObject pao2;
        private GameObject pao3;
        private GameObject Box1;
        private GameObject Box2;
        private GameObject Box3;
        private bool[] bool1;
        private bool[] bool2;
        private bool[] bool3;
        private bool _canClick;
        private bool _canOK;
        private bool _canwin;
        private GameObject Drag1;
        private GameObject Drag2;
        private GameObject Drag3;
        private GameObject jy;
        private Vector2 startPos;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _mask = curTrans.Find("mymask").gameObject;
            Next = curTrans.Find("next").gameObject;

            Level1 = curTrans.Find("Level1").gameObject;
            Level2 = curTrans.Find("Level2").gameObject;
            Level3 = curTrans.Find("Level3").gameObject;
            Level4 = curTrans.Find("Level4").gameObject;
            Box1 = Level1.transform.Find("Box").gameObject;
            Box2 = Level2.transform.Find("Box").gameObject;
            Box3 = Level3.transform.Find("Box").gameObject;
            bool1 = new bool[Box1.transform.childCount];
            bool2 = new bool[Box2.transform.childCount];
            bool3 = new bool[Box3.transform.childCount];
            Drag1 = Level1.transform.Find("Drag").gameObject;
            Drag2 = Level2.transform.Find("Drag").gameObject;
            Drag3 = Level3.transform.Find("Drag").gameObject;

            pao1 = Level1.transform.Find("pao").gameObject;
            pao2 = Level2.transform.Find("pao").gameObject;
            pao3 = Level3.transform.Find("pao").gameObject;
            jy = curTrans.Find("JY").gameObject;

            Drag1.GetComponent<mILDrager>().SetDragCallback(BeginDragMoveEvent, null, EndDrag, null);
            Drag2.GetComponent<mILDrager>().SetDragCallback(BeginDragMoveEvent, null, EndDrag2, null);
            Drag3.GetComponent<mILDrager>().SetDragCallback(BeginDragMoveEvent, null, EndDrag3, null);

            Util.AddBtnClick(Level1.transform.Find("OK1").gameObject, OK1);
            Util.AddBtnClick(Level1.transform.Find("Replay1").gameObject, Replay1);
            Util.AddBtnClick(Level2.transform.Find("OK2").gameObject, OK2);
            Util.AddBtnClick(Level2.transform.Find("Replay2").gameObject, Replay2);
            Util.AddBtnClick(Level3.transform.Find("OK3").gameObject, OK3);
            Util.AddBtnClick(Level3.transform.Find("Replay3").gameObject, Replay3);
            Util.AddBtnClick(Next, NextPart);

            GameInit();
            //GameStart();
        }
        #region 初始化和游戏开始方法

        private void GameInit()
        {
            nextnumber = 0;
            pao1.SetActive(false);
            pao2.SetActive(false);
            pao3.SetActive(false);
            talkIndex = 1;
            Box1.SetActive(false);
            Box2.SetActive(false);
            Box3.SetActive(false);
            Level1.SetActive(true);
            Level2.SetActive(false);
            Level3.SetActive(false);
            Level4.SetActive(false);
            Level1.transform.Find("show1").gameObject.SetActive(false);
            Level1.transform.Find("guang").gameObject.SetActive(false);
            Level2.transform.Find("show2").gameObject.SetActive(false);
            Level2.transform.Find("guang").gameObject.SetActive(false);
            Level3.transform.Find("show3").gameObject.SetActive(false);
            Level3.transform.Find("guang").gameObject.SetActive(false);
            Level1.transform.Find("xem").gameObject.SetActive(false);
            Level1.transform.Find("xem").rotation = Quaternion.Euler(0,0,-30);
            Level1.transform.Find("xem").localPosition = Level1.transform.Find("xemPos").localPosition;
            Level2.transform.Find("xem").gameObject.SetActive(false);
            Level2.transform.Find("xem").rotation = Quaternion.Euler(0, 0, -30);
            Level2.transform.Find("xem").localPosition = Level2.transform.Find("xemPos").localPosition;
            Level3.transform.Find("xem").gameObject.SetActive(false);
            Level3.transform.Find("xem").localPosition = Level3.transform.Find("xemPos").localPosition;
            Level1.transform.Find("OK1").gameObject.SetActive(false);
            Level1.transform.Find("Replay1").gameObject.SetActive(false);
            Level2.transform.Find("OK2").gameObject.SetActive(false);
            Level2.transform.Find("Replay2").gameObject.SetActive(false);
            Level3.transform.Find("OK3").gameObject.SetActive(false);
            Level3.transform.Find("Replay3").gameObject.SetActive(false);
            Drag1.GetComponent<mILDrager>().canMove = false;
            Drag1.SetActive(false);
            Drag1.transform.localPosition = Level1.transform.Find("DragPos").localPosition;
            for (int i = 0; i < Drag1.transform.childCount; i++)
            {
                Drag1.transform.GetChild(i).gameObject.SetActive(false);
            }
            Drag2.GetComponent<mILDrager>().canMove = false;
            Drag2.SetActive(false);
            Drag2.transform.localPosition = Level2.transform.Find("DragPos").localPosition;
            for (int i = 0; i < Drag2.transform.childCount; i++)
            {
                Drag2.transform.GetChild(i).gameObject.SetActive(false);
            }
            Drag3.GetComponent<mILDrager>().canMove = false;
            Drag3.SetActive(false);
            Drag3.transform.localPosition = Level3.transform.Find("DragPos").localPosition;
            for (int i = 0; i < Drag3.transform.childCount; i++)
            {
                Drag3.transform.GetChild(i).gameObject.SetActive(false);
            }
            jy.transform.rotation = new Quaternion(0, 0, 0, 0);
            jy.SetActive(true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            _mask.SetActive(false);
            Next.SetActive(false);
            Level4.transform.Find("jy").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level4.transform.Find("paopao").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level3.transform.Find("pao").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level2.transform.Find("pao").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level1.transform.Find("pao").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            for (int i = 0; i < bool1.Length; i++)
            {
                bool1[i] = false;
            }
            for (int i = 0; i < bool2.Length; i++)
            {
                bool2[i] = false;
            }
            for (int i = 0; i < bool3.Length; i++)
            {
                bool3[i] = false;
            }
            for (int i = 0; i < Box1.transform.childCount; i++)
            {
                Box1.transform.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
            for (int i = 0; i < Box2.transform.childCount; i++)
            {
                Box2.transform.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
            for (int i = 0; i < Box3.transform.childCount; i++)
            {
                Box3.transform.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }

        }



        void GameStart()
        {
            for (int i = 0; i < 4; i++)
            {
                mono.StartCoroutine(Wait(0.4f,
                () => { SpineManager.instance.DoAnimation(Level1.transform.Find("paobj").GetChild(i).gameObject, "pao-bj", true); }
                ));
            }
            SpineManager.instance.DoAnimation(jy, "JY2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(jy, "JY", true);
                    pao1.transform.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(pao1.transform.gameObject, "pao", false,
                        () =>
                        {
                            Level1.transform.Find("xem").gameObject.SetActive(true);
                            Level1.transform.Find("xem").localPosition = Level1.transform.Find("xemPos").localPosition;
                            SpineManager.instance.DoAnimation(Level1.transform.Find("xem").gameObject, "xem1", false,
                                () => { SpineManager.instance.DoAnimation(Level1.transform.Find("xem").gameObject, "xem2", true); }
                                );
                            SpineManager.instance.DoAnimation(pao1.transform.gameObject, "pao2", false);
                            Box1.SetActive(true);
                            for (int i = 0; i < Box1.transform.childCount; i++)
                            {
                                Util.AddBtnClick(Box1.transform.GetChild(i).gameObject, Click1);
                                SpineManager.instance.DoAnimation(Box1.transform.GetChild(i).gameObject, "qiu3", false);
                            }
                            _canClick = true;
                            Level1.transform.Find("OK1").gameObject.SetActive(true);
                            Level1.transform.Find("Replay1").gameObject.SetActive(true);
                        }
                        );
                }
                );
            isPlaying = false;
            //田丁开始游戏
            //TDGameStart();

        }



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

        private void Click1(GameObject obj)
        {
            if (_canClick)
            {
                if (!bool1[Convert.ToInt32(obj.name) - 1])
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    bool1[Convert.ToInt32(obj.name) - 1] = true;
                    SpineManager.instance.DoAnimation(obj, "qiu2", false);
                }
                else
                {
                    bool1[Convert.ToInt32(obj.name) - 1] = false;
                    SpineManager.instance.DoAnimation(obj, "qiu3", false);
                }
            }
        }
        private void Click2(GameObject obj)
        {
            if (_canClick)
            {
                if (!bool2[Convert.ToInt32(obj.name) - 1])
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    bool2[Convert.ToInt32(obj.name) - 1] = true;
                    SpineManager.instance.DoAnimation(obj, "qiu2", false);
                }
                else
                {
                    bool2[Convert.ToInt32(obj.name) - 1] = false;
                    SpineManager.instance.DoAnimation(obj, "qiu3", false);
                }
            }
        }
        private void Click3(GameObject obj)
        {
            if (_canClick)
            {
                if (!bool3[Convert.ToInt32(obj.name) - 1])
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    bool3[Convert.ToInt32(obj.name) - 1] = true;
                    SpineManager.instance.DoAnimation(obj, "qiu2", false);
                }
                else
                {
                    bool3[Convert.ToInt32(obj.name) - 1] = false;
                    SpineManager.instance.DoAnimation(obj, "qiu3", false);
                }
            }
        }
        private void OK1(GameObject obj)
        {
            for (int i = 0; i < bool1.Length; i++)
            {
                if (bool1[i])
                {
                    _canOK = true;
                    break;
                }
            }

            if (_canOK)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2,false);
                SpineManager.instance.DoAnimation(Level1.transform.Find("OK1").GetChild(0).gameObject, "ok3", false);
                _canOK = false;
                _canClick = false;
                Drag1.SetActive(true);
                Drag1.GetComponent<mILDrager>().canMove = true;
                int number = Box1.transform.childCount;
                Box1.SetActive(false);
                for (int i = 0; i < number; i++)
                {
                    if (bool1[Convert.ToInt32(Box1.transform.GetChild(i).name) - 1])
                    {
                        Drag1.transform.Find(Box1.transform.GetChild(i).name).gameObject.SetActive(true);
                    }
                }
                for (int i = 0; i < 9; i++)
                {
                    if (!bool1[i])
                        return;
                }
                for (int i = 9; i < 16; i++)
                {
                    if (bool1[i])
                        return;
                }
                _canwin = true;
            }
        }
        private void OK2(GameObject obj)
        {
            for (int i = 0; i < bool2.Length; i++)
            {
                if (bool2[i])
                {
                    _canOK = true;
                    break;
                }
            }
            if (_canOK)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(Level2.transform.Find("OK2").GetChild(0).gameObject, "ok3", false);
                _canOK = false;
                _canClick = false;
                Drag2.SetActive(true);
                Drag2.GetComponent<mILDrager>().canMove = true;
                int number = Box2.transform.childCount;
                Box2.SetActive(false);
                for (int i = 0; i < number; i++)
                {
                    if (bool2[Convert.ToInt32(Box2.transform.GetChild(i).name) - 1])
                    {
                        Drag2.transform.Find(Box2.transform.GetChild(i).name).gameObject.SetActive(true);
                    }
                }
                for (int i = 0; i < 11; i++)
                {
                    if (!bool2[i])
                        return;
                }
                for (int i = 11; i < 19; i++)
                {
                    if (bool2[i])
                        return;
                }
                _canwin = true;
            }
        }
        private void OK3(GameObject obj)
        {
            for (int i = 0; i < bool3.Length; i++)
            {
                if (bool3[i])
                {
                    _canOK = true;
                    break;
                }
            }
            if (_canOK)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(Level3.transform.Find("OK3").GetChild(0).gameObject, "ok3", false);
                _canOK = false;
                _canClick = false;
                Drag3.SetActive(true);
                Drag3.GetComponent<mILDrager>().canMove = true;
                int number = Box3.transform.childCount;
                Box3.SetActive(false);
                for (int i = 0; i < number; i++)
                {
                    if (bool3[Convert.ToInt32(Box3.transform.GetChild(i).name) - 1])
                    {
                        Drag3.transform.Find(Box3.transform.GetChild(i).name).gameObject.SetActive(true);
                    }
                }
                for (int i = 0; i < 13; i++)
                {
                    if (!bool3[i])
                        return;
                }
                for (int i = 13; i < 28; i++)
                {
                    if (bool3[i])
                        return;
                }
                _canwin = true;
            }
        }
        private void Replay1(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SpineManager.instance.DoAnimation(Level1.transform.Find("Replay1").GetChild(0).gameObject, "ok4", false);
            Box1.SetActive(true);
            _canClick = true;
            _canwin = false;
            Drag1.GetComponent<mILDrager>().canMove = false;
            Drag1.transform.localPosition = Level1.transform.Find("DragPos").localPosition;
            for (int i = 0; i < bool1.Length; i++)
            {
                bool1[i] = false;
            }
            for (int i = 0; i < Drag1.transform.childCount; i++)
            {
                Box1.transform.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Box1.transform.GetChild(i).gameObject, "qiu3", false);
                Drag1.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void Replay2(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SpineManager.instance.DoAnimation(Level2.transform.Find("Replay2").GetChild(0).gameObject, "ok4", false);
            Box2.SetActive(true);
            _canClick = true;
            _canwin = false;
            Drag2.GetComponent<mILDrager>().canMove = false;
            Drag2.transform.localPosition = Level2.transform.Find("DragPos").localPosition;
            for (int i = 0; i < bool2.Length; i++)
            {
                bool2[i] = false;
            }
            for (int i = 0; i < Drag2.transform.childCount; i++)
            {
                Box2.transform.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Box2.transform.GetChild(i).gameObject, "qiu3", false);
                Drag2.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void Replay3(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SpineManager.instance.DoAnimation(Level3.transform.Find("Replay3").GetChild(0).gameObject, "ok4", false);
            Box3.SetActive(true);
            _canClick = true;
            _canwin = false;
            Drag3.GetComponent<mILDrager>().canMove = false;
            Drag3.transform.localPosition = Level3.transform.Find("DragPos").localPosition;
            for (int i = 0; i < bool3.Length; i++)
            {
                bool3[i] = false;
            }
            for (int i = 0; i < Drag3.transform.childCount; i++)
            {
                Box3.transform.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Box3.transform.GetChild(i).gameObject, "qiu3", false);
                Drag3.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void BeginDragMoveEvent(Vector3 dragPosition, int dragType, int dragIndex)
        {
            startPos = dragPosition;
        }
        private void EndDrag(Vector3 dragPosition, int dragType, int dragIndex, bool isMatch)
        {
            for (int i = 1; i < 3; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(Level1.transform.GetChild(1).GetChild(i).GetComponent<RectTransform>(), Input.mousePosition))
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(Level1.transform.GetChild(1).GetChild(i).GetChild(0).gameObject, "1-yun" + Level1.transform.GetChild(1).GetChild(i).GetChild(0).gameObject.name, false);
                    SpineManager.instance.DoAnimation(Level1.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level1.transform.GetChild(0).gameObject, "xem2", true); }
                        );
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(Level1.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>(), Input.mousePosition))
            {
                SpineManager.instance.DoAnimation(Level1.transform.GetChild(1).GetChild(0).GetChild(0).gameObject, "1-yun" + Level1.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.name, false);
                if(!_canwin)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(Level1.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level1.transform.GetChild(0).gameObject, "xem2", true); }
                        );
                }
            }

            if (isMatch && _canwin)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Drag1.SetActive(false);
                Level1.transform.Find("show1").gameObject.SetActive(true);
                for (int i = 0; i < Level1.transform.Find("show1").childCount; i++)
                {
                   SpineManager.instance.DoAnimation(Level1.transform.Find("show1").GetChild(i).gameObject, "qiu2", false);
                }
                Level1.transform.Find("guang").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level1.transform.Find("guang").gameObject, "ok5", false,
                    () => {
                        Next.SetActive(true);
                        SpineManager.instance.DoAnimation(Next, "next2", false);
                        _canNext = true;
                    }
                    );
                _mask.SetActive(true);
                Level1.transform.Find("OK1").gameObject.SetActive(false);
                Level1.transform.Find("Replay1").gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(pao1, "pao3", false);
                Level1.transform.Find("xem").rotation = new Quaternion(0, 0, 0, 0);
                SpineManager.instance.DoAnimation(Level1.transform.Find("xem").gameObject, "xem-y", true);
                Level1.transform.Find("xem").GetComponent<RectTransform>().DOAnchorPosY(-700f, 2f).OnComplete(() =>
                {
                    Level1.transform.Find("xem").gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(Level1.transform.GetChild(1).GetChild(2).GetChild(0).gameObject, "1-yunb", false,
                        () => { SpineManager.instance.DoAnimation(Level1.transform.GetChild(1).GetChild(2).GetChild(0).gameObject, "1-yunb2", true); }
                        );
                });
            }
            else
            {
                Drag1.transform.localPosition = startPos;
            }
        }
        private void EndDrag2(Vector3 dragPosition, int dragType, int dragIndex, bool isMatch)
        {
            for (int i = 0; i < 2; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(Level2.transform.GetChild(1).GetChild(i).GetComponent<RectTransform>(), Input.mousePosition))
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(Level2.transform.GetChild(1).GetChild(i).GetChild(0).gameObject, "2-yun" + Level2.transform.GetChild(1).GetChild(i).GetChild(0).gameObject.name, false);
                    SpineManager.instance.DoAnimation(Level2.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level2.transform.GetChild(0).gameObject, "xem2", true); }
                        );
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(Level2.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>(), Input.mousePosition))
            {
                SpineManager.instance.DoAnimation(Level2.transform.GetChild(1).GetChild(2).GetChild(0).gameObject, "2-yun" + Level2.transform.GetChild(1).GetChild(2).GetChild(0).gameObject.name, false);
                if (!_canwin)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(Level2.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level2.transform.GetChild(0).gameObject, "xem2", true); }
                        );
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(Level2.transform.GetChild(1).GetChild(3).GetComponent<RectTransform>(), Input.mousePosition))
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(Level2.transform.GetChild(1).GetChild(3).GetChild(0).gameObject, "2-yun" + Level2.transform.GetChild(1).GetChild(3).GetChild(0).gameObject.name, false);
                SpineManager.instance.DoAnimation(Level2.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level2.transform.GetChild(0).gameObject, "xem2", true); }
                        );
            }
            if (isMatch && _canwin)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Drag2.SetActive(false);
                Level2.transform.Find("show2").gameObject.SetActive(true);
                for (int i = 0; i < Level2.transform.Find("show2").childCount; i++)
                {
                    SpineManager.instance.DoAnimation(Level2.transform.Find("show2").GetChild(i).gameObject, "qiu2", false);
                }
                Level2.transform.Find("guang").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level2.transform.Find("guang").gameObject, "ok5", false,
                    () => {
                        Next.SetActive(true);
                        SpineManager.instance.DoAnimation(Next, "next2", false);
                        _canNext = true;
                    }
                    );
                _mask.SetActive(true);
                Level2.transform.Find("OK2").gameObject.SetActive(false);
                Level2.transform.Find("Replay2").gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(pao2, "pao3", false);
                Level2.transform.Find("xem").rotation = new Quaternion(0, 0, 0, 0);
                SpineManager.instance.DoAnimation(Level2.transform.Find("xem").gameObject, "xem-y", true);
                Level2.transform.Find("xem").GetComponent<RectTransform>().DOAnchorPosY(-700f, 2f).OnComplete(() =>
                {
                    Level2.transform.Find("xem").gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(Level2.transform.GetChild(1).GetChild(3).GetChild(0).gameObject, "2-yunc", false,
                        () => { SpineManager.instance.DoAnimation(Level2.transform.GetChild(1).GetChild(3).GetChild(0).gameObject, "2-yunc2", true); }
                        );
                    
                });
            }
            else
            {
                Drag2.transform.localPosition = startPos;
            }
        }
        private void EndDrag3(Vector3 dragPosition, int dragType, int dragIndex, bool isMatch)
        {
            for (int i = 0; i < 2; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(Level3.transform.GetChild(1).GetChild(i).GetComponent<RectTransform>(), Input.mousePosition))
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(Level3.transform.GetChild(1).GetChild(i).GetChild(0).gameObject, "3-yun" + Level3.transform.GetChild(1).GetChild(i).GetChild(0).gameObject.name, false);
                    SpineManager.instance.DoAnimation(Level3.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level3.transform.GetChild(0).gameObject, "xem2", true); }
                        );
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(Level3.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>(), Input.mousePosition))
            {
                SpineManager.instance.DoAnimation(Level3.transform.GetChild(1).GetChild(2).GetChild(0).gameObject, "3-yun" + Level3.transform.GetChild(1).GetChild(2).GetChild(0).gameObject.name, false);
                if (!_canwin)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,4,false);
                    SpineManager.instance.DoAnimation(Level3.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level3.transform.GetChild(0).gameObject, "xem2", true); }
                        );
                }
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(Level3.transform.GetChild(1).GetChild(3).GetComponent<RectTransform>(), Input.mousePosition))
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(Level3.transform.GetChild(1).GetChild(3).GetChild(0).gameObject, "3-yun" + Level3.transform.GetChild(1).GetChild(3).GetChild(0).gameObject.name, false);
                SpineManager.instance.DoAnimation(Level3.transform.GetChild(0).gameObject, "xem-jx", false,
                        () => { SpineManager.instance.DoAnimation(Level3.transform.GetChild(0).gameObject, "xem2", true); }
                        );
            }
            if (isMatch && _canwin)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Drag3.SetActive(false);
                Level3.transform.Find("show3").gameObject.SetActive(true);
                for (int i = 0; i < Level3.transform.Find("show3").childCount; i++)
                {
                    SpineManager.instance.DoAnimation(Level3.transform.Find("show3").GetChild(i).gameObject, "qiu2", false);
                }
                Level3.transform.Find("guang").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level3.transform.Find("guang").gameObject, "ok5", false,
                    () => {
                        Next.SetActive(true);
                        SpineManager.instance.DoAnimation(Next, "next2", false);
                        _canNext = true;
                    }
                    );
                _mask.SetActive(true);
                Level3.transform.Find("OK3").gameObject.SetActive(false);
                Level3.transform.Find("Replay3").gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(pao3, "pao3", false);
                Level3.transform.Find("xem").rotation = new Quaternion(0, 0, 0, 0);
                SpineManager.instance.DoAnimation(Level3.transform.Find("xem").gameObject, "xem-y", true);
                Level3.transform.Find("xem").GetComponent<RectTransform>().DOAnchorPosY(-700f, 2f).OnComplete(() =>
                {
                    Level3.transform.Find("xem").gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(Level3.transform.GetChild(1).GetChild(1).GetChild(0).gameObject, "3-yund", false,
                        () => { SpineManager.instance.DoAnimation(Level3.transform.GetChild(1).GetChild(1).GetChild(0).gameObject, "3-yund2", true); }
                        );
                });
            }
            else
            {
                Drag3.transform.localPosition = startPos;
            }
        }



        private void NextPart(GameObject obj)
        {
            if (_canNext)
            {
                BtnPlaySound();
                nextnumber++;
                _canNext = false;
                _canwin = false;
                _canClick = false;
                SpineManager.instance.DoAnimation(Next, "next", false,
                    () =>
                    {
                        Next.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        if (nextnumber == 1)
                        {
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                            Level1.SetActive(false);
                            Level2.SetActive(true);
                            Next.SetActive(false);
                            _mask.SetActive(false);
                            for (int i = 0; i < 3; i++)
                            {
                                mono.StartCoroutine(Wait(0.4f,
                                () => { SpineManager.instance.DoAnimation(Level2.transform.Find("paobj").GetChild(i).gameObject, "pao-bj", true); }
                                ));
                            }

                            SpineManager.instance.DoAnimation(jy, "JY2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(jy, "JY", true);
                    pao2.transform.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(pao2.transform.gameObject, "pao", false,
                        () =>
                        {
                            Level2.transform.Find("xem").gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(Level2.transform.Find("xem").gameObject, "xem1", false,
                                () => { SpineManager.instance.DoAnimation(Level2.transform.Find("xem").gameObject, "xem2", true); }
                                );
                            SpineManager.instance.DoAnimation(pao2.transform.gameObject, "pao2", false);
                            Box2.SetActive(true);
                            for (int i = 0; i < Box2.transform.childCount; i++)
                            {
                                Util.AddBtnClick(Box2.transform.GetChild(i).gameObject, Click2);
                                SpineManager.instance.DoAnimation(Box2.transform.GetChild(i).gameObject, "qiu3", false);
                            }
                            _canClick = true;
                            Level2.transform.Find("OK2").gameObject.SetActive(true);
                            Level2.transform.Find("Replay2").gameObject.SetActive(true);
                        }
                        );
                }
                );
                        }
                        if (nextnumber == 2)
                        {
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                            jy.transform.Rotate(new Vector3(0, 0, -20));
                            Level2.SetActive(false);
                            Level3.SetActive(true);
                            Next.SetActive(false);
                            _mask.SetActive(false);
                            for (int i = 0; i < 1; i++)
                            {
                                mono.StartCoroutine(Wait(0.4f,
                                () => { SpineManager.instance.DoAnimation(Level3.transform.Find("paobj").GetChild(i).gameObject, "paopao", true); }
                                ));
                            }
                            SpineManager.instance.DoAnimation(jy, "JY2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(jy, "JY", true);
                    pao3.transform.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(pao3.transform.gameObject, "pao", false,
                        () =>
                        {
                            Level3.transform.Find("xem").gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(Level3.transform.Find("xem").gameObject, "xem1", false,
                                () => { SpineManager.instance.DoAnimation(Level3.transform.Find("xem").gameObject, "xem2", true); }
                                );
                            SpineManager.instance.DoAnimation(pao3.transform.gameObject, "pao2", false);
                            Box3.SetActive(true);
                            for (int i = 0; i < Box3.transform.childCount; i++)
                            {
                                Util.AddBtnClick(Box3.transform.GetChild(i).gameObject, Click3);
                                SpineManager.instance.DoAnimation(Box3.transform.GetChild(i).gameObject, "qiu3", false);
                            }
                            _canClick = true;
                            Level3.transform.Find("OK3").gameObject.SetActive(true);
                            Level3.transform.Find("Replay3").gameObject.SetActive(true);
                        }
                        );
                }
                );

                        }
                        if (nextnumber == 3)
                        {
                            jy.SetActive(false);
                            Level3.SetActive(false);
                            Level4.SetActive(true);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                            SpineManager.instance.DoAnimation(Level4.transform.GetChild(2).gameObject, "4-JY", false);
                            SpineManager.instance.DoAnimation(Level4.transform.GetChild(1).gameObject, "4-paopao", false);
                            mono.StartCoroutine(Wait(5f,
                                () => { playSuccessSpine(); }
                                ));
                        }

                    }
                    );


            }
        }

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator WaitFor(float time, Action callback = null)
        {
            callback?.Invoke();
            yield return new WaitForSeconds(time);
        }
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
                    mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE,1,null,
                        () => {
                            mask.SetActive(false);
                            bd.SetActive(false);
                            GameStart();
                        }
                        ));
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

            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
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
            ////加载点击滑动图片
            TDLoadPageBar();
            ////加载材料环节
            //LoadSpineShow();

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
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(true);
                        bd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE,0,null,
                            () => {
                                SoundManager.instance.ShowVoiceBtn(true); }
                            ));
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); GameStart(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
                }

            });
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





    }
}
#endregion