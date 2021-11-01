
using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public class TD6764Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private Transform Bg;
        private BellSprites bellTextures;
        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject btnBack;


        // 游戏逻辑变量
        private Transform ding;

        private Transform dizuo;
        private Transform panelStartPos;
        private Transform panelEndPos;
        private Transform mMaskPanel;

        private struct MMInfo
        {
            public GameObject mmObj;
            public ILDrager iLDrager;
            public Image img;
            public BellSprites bellSprites;
        }

        List<MMInfo> curList;

        private Transform mMaskPanelPos;
        private List<Vector3> curLevelV3s;
        private Vector3[] curLevelArr;
        private Transform scBoom;
        private GameObject paida;

        private GameObject yaogan;
        private GameObject shouzhi;
        private GameObject btn;

        private GameObject clock;

        private Text timeText;

        private Transform xemPos;
        private GameObject failXem;
        private Transform bloodXem;

        private Transform qtPanel;


        //胜利动画名字
        private string tz;


        private bool isPlaying = false;
        private int flag = 0;
        private int curLevel = -1;

        private ILDroper[] iLDropers;

        private int recordTime = 0;
        private int[] countDownTime;

        private RectMask2D mask2D;
        private Coroutine temCountTime;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg");
            //bellTextures = Bg.GetComponent<BellSprites>();
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = successSpine.transform.GetChild(0).gameObject;
            caidaiSpine.SetActive(false);
            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(0).gameObject.SetActive(true);

            btnBack = curTrans.Find("btnBack").gameObject;
            //Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            tz = "6-12-z";

            ding = Bg.Find("ding");
            dizuo = Bg.Find("dizuo");
            panelStartPos = dizuo.Find("startPos");
            panelEndPos = dizuo.Find("endPos");
            curList = new List<MMInfo>();
            curLevelV3s = new List<Vector3>();
            mMaskPanel = dizuo.Find("mask");
            mask2D = mMaskPanel.GetComponent<RectMask2D>();
            mMaskPanelPos = dizuo.Find("maskPos");

            scBoom = dizuo.Find("scBoom");
            paida = dizuo.Find("paida").gameObject;
            yaogan = Bg.Find("yaogan").gameObject;
            shouzhi = yaogan.transform.GetChild(0).gameObject;
            btn = yaogan.transform.GetChild(1).gameObject;
            btn.SetActive(true);
            Util.AddBtnClick(btn, OnClickBtnYaoGan);

            clock = Bg.Find("clock").gameObject;
            timeText = clock.transform.GetChild(0).GetComponent<Text>();

            xemPos = Bg.Find("xemPos");

            failXem = Bg.Find("failXem").gameObject;
            bloodXem = Bg.Find("countPanel/bloodXem");
            qtPanel = Bg.Find("qtPanel");


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }


        private void SetBloodPanel()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            qtPanel.GetChild(curLevel).GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(qtPanel.GetChild(curLevel).gameObject, curLevel == 0 ? "quantou" : "quantou2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(qtPanel.GetChild(curLevel).gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "xem-y", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(qtPanel.GetChild(curLevel).gameObject, "kong", false);
                        });
                    mono.StartCoroutine(DelayFunc(curLevel == 2 ? 0f : 0.5f,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "xem-y2", true);
                            xemPos.GetChild(0).DOMove(qtPanel.GetChild(curLevel).GetChild(0).position, curLevel == 0 ? 3f : 2f).SetEase(Ease.OutQuart).OnComplete(
                                () =>
                                {
                                    bloodXem.GetChild(curLevel).GetChild(0).gameObject.SetActive(false);
                                    //for (int i = 0; i < iLDropers.Length; i++)
                                    //{
                                    //    SpineManager.instance.DoAnimation(iLDropers[i].transform.parent.gameObject, ding.GetChild(curLevel).name + (i == 0 ? "" : i + 1 + ""), true);
                                    //}
                                    mono.StartCoroutine(DelayFunc(1f, () =>
                                    {
                                        btn.SetActive(true);
                                        btnBack.SetActive(false);
                                        if (curLevel >= 2)
                                        {
                                            playSuccessSpine();
                                        }
                                    }));

                                });
                        }));


                });


        }

        private void OnClickBtnYaoGan(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            btnBack.SetActive(true);
            curLevel++;
            obj.SetActive(false);
            StopCountTimeCoroutine();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(yaogan, yaogan.name + 2, false,
                () =>
                {
                    isPlaying = false;
                    //init
                    GameDataInit();
                });
        }

        private void StopCountTimeCoroutine()
        {
            if (temCountTime != null)
                mono.StopCoroutine(temCountTime);
            temCountTime = null;
        }
        private IEnumerator Count_down(int limitNum, Action callBack = null)
        {//协程方法实现倒计时
            clock.SetActive(true);
            timeText.text = string.Format("{0}", limitNum);
            while (limitNum >= 0)
            {
                if (limitNum>=0&&limitNum < 3)
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                yield return new WaitForSeconds(1.0f);
                limitNum--;
                timeText.text = string.Format("{0}", limitNum);
            }
            timeText.text = string.Format("{0}", limitNum >= 0 ? limitNum : 0);
            if (limitNum < 0)
            {
                callBack?.Invoke();
            }
        }

        /// <summary>
        /// 延迟回调
        /// </summary>
        /// <param name="delayTime"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator DelayFunc(float delayTime, Action callback)
        {
            yield return new WaitForSeconds(delayTime);
            callback?.Invoke();
        }
        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        private void GetRandomArray<T>(T[] arr)
        {
            System.Random r = new System.Random();
            for (int i = 0; i < arr.Length; i++)
            {
                int tem = r.Next(arr.Length);
                T temItem = arr[i];
                arr[i] = arr[tem];
                arr[tem] = temItem;
            }

        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
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
                case BtnEnum.next:
                    result = "next";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        /// <summary>
        /// 按钮方法分bf fh next ok
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickAnyBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            //SoundManager.instance.StopAudio(SoundManager.SoundType.BGM); 
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "next")
                {
                    AnyBtnNextStep(obj, () =>
                    {
                        bd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                        {
                            mask.SetActive(false);
                            bd.SetActive(false);
                        }));

                    });
                }
                else if (obj.name == "bf")
                {
                    AnyBtnNextStep(obj, GameStart);
                }
                else if (obj.name == "fh")
                {
                    AnyBtnNextStep(obj, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        mask.SetActive(false);

                        GameInit();
                        btnBack.SetActive(false);
                    });
                }
                else
                {
                    AnyBtnNextStep(obj, () =>
                    {
                        switchBGM();
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2));
                    });
                }
            });
        }

        /// <summary>
        ///  按钮方法分bf fh next ok的回调
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        void AnyBtnNextStep(GameObject obj, Action callback)
        {
            SpineManager.instance.DoAnimation(obj, "kong", false, () =>
            {
                anyBtns.gameObject.SetActive(false);
                isPlaying = false;
                callback?.Invoke();
            });
        }


        private void GameInit()
        {
            mono.StopAllCoroutines();
            DOTween.KillAll();
            countDownTime = new int[] { 10, 12, 14 };
            talkIndex = 1;
            flag = 0;
            clock.SetActive(false);
            curdragIndex = -1;
            curLevel = -1;
            recordTime = 8;
            StopCountTimeCoroutine();
            iLDropers = ding.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < iLDropers.Length; i++)
            {
                iLDropers[i].dropType = int.Parse(iLDropers[i].name);
                iLDropers[i].SetDropCallBack();
                iLDropers[i].SetDropCallBack(OnAfter);
                iLDropers[i].transform.parent.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(iLDropers[i].transform.parent.gameObject, "kong", false);
                SpineManager.instance.DoAnimation(iLDropers[i].transform.GetChild(0).gameObject, "kong", false);
            }
            for (int i = 0; i < mMaskPanel.childCount; i++)
            {
                ding.GetChild(i).gameObject.SetActive(false);
                mMaskPanel.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < scBoom.childCount; i++)
            {
                SpineManager.instance.DoAnimation(scBoom.GetChild(i).gameObject, "kong", false);
                scBoom.GetChild(i).gameObject.SetActive(false);
            }
            SpineManager.instance.DoAnimation(paida, "kong", false);
            for (int i = 0; i < bloodXem.childCount; i++)
            {
                bloodXem.GetChild(i).GetChild(0).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(qtPanel.GetChild(i).gameObject, "kong", false);
            }
            //InitXemPos();
            xemPos.GetChild(0).localPosition = Vector3.left * 800;
            SpineManager.instance.DoAnimation(failXem, "kong", false);
            SpineManager.instance.DoAnimation(shouzhi, "kong", false);
            SpineManager.instance.DoAnimation(yaogan, yaogan.name, false);

            SpineManager.instance.DoAnimation(ding.gameObject, ding.name, true);
            SpineManager.instance.DoAnimation(dizuo.gameObject, dizuo.name, true);
            //btnBack.SetActive(true);

            // GameDataInit();
        }

        private void InitXemPos(Action callBack = null)
        {
            xemPos.GetChild(0).localPosition = Vector3.left * 800;
            xemPos.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "xem1", true);
            xemPos.GetChild(0).DOLocalMoveX(0, 1).SetEase(Ease.Flash).OnComplete(() => { callBack?.Invoke(); });
        }

        private void GameDataInit(bool isInitXemPos = true)
        {
            flag = 0;
            mask2D.enabled = true;
            if (isInitXemPos)
                InitXemPos();
            for (int i = 0; i < mMaskPanel.childCount; i++)
            {
                ding.GetChild(i).gameObject.SetActive(false);
                //mMaskPanel.GetChild(i).gameObject.SetActive(false);
            }
            curLevelV3s.Clear();

            for (int i = 0; i < mMaskPanelPos.GetChild(curLevel).childCount; i++)
            {
                curLevelV3s.Add(mMaskPanelPos.GetChild(curLevel).GetChild(i).position);
                if (mMaskPanelPos.GetChild(curLevel).GetChild(i).childCount > 0)
                    mMaskPanelPos.GetChild(curLevel).GetChild(i).GetChild(0).SetParent(mMaskPanel.GetChild(curLevel));
            }

            for (int i = 0; i < mMaskPanelPos.GetChild(curLevel).childCount; i++)
            {
                int tem = int.Parse(mMaskPanel.GetChild(curLevel).GetChild(mMaskPanel.GetChild(curLevel).childCount - 1).name);
                mMaskPanel.GetChild(curLevel).GetChild(mMaskPanel.GetChild(curLevel).childCount - 1).SetParent(mMaskPanelPos.GetChild(curLevel).GetChild(tem));
            }

            for (int i = 0; i < mMaskPanelPos.GetChild(curLevel).childCount; i++)
            {
                if (mMaskPanelPos.GetChild(curLevel).GetChild(i).childCount > 0)
                    mMaskPanelPos.GetChild(curLevel).GetChild(i).GetChild(0).SetParent(mMaskPanel.GetChild(curLevel));
            }

            ding.GetChild(curLevel).gameObject.SetActive(true);
            iLDropers = ding.GetChild(curLevel).GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < iLDropers.Length; i++)
            {
                iLDropers[i].index = i;
                SpineManager.instance.DoAnimation(iLDropers[i].transform.parent.gameObject, "kong", false);
                SpineManager.instance.DoAnimation(iLDropers[i].transform.GetChild(0).gameObject, "kong", false);
                mMaskPanel.GetChild(curLevel).GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            curList.Clear();
            mMaskPanel.GetChild(curLevel).localPosition = Vector3.left * 1550;
            for (int i = 0; i < mMaskPanelPos.GetChild(curLevel).childCount; i++)
            {
                mMaskPanel.GetChild(curLevel).GetChild(i).localPosition = mMaskPanelPos.GetChild(curLevel).GetChild(i).localPosition;
                mMaskPanel.GetChild(curLevel).GetChild(i).GetChild(0).localPosition = Vector3.zero;
            }
            mMaskPanel.GetChild(curLevel).gameObject.SetActive(true);

            if (curLevel - 1 >= 0 && isInitXemPos)
            {
                mMaskPanel.GetChild(curLevel - 1).DOLocalMoveX(1550, 1).SetEase(Ease.OutBack);
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            mMaskPanel.GetChild(curLevel).DOLocalMoveX(0, 1).SetEase(Ease.OutBack).OnComplete(
                () =>
                {
                    mono.StartCoroutine(Count_down(recordTime,
                        () =>
                        {
                            //随机位置
                            RandomPostion(
                                () =>
                                {
                                    btnBack.SetActive(false);
                                    temCountTime = mono.StartCoroutine(Count_down(countDownTime[curLevel],
                                    () =>
                                    {
                                        if (curdragIndex >= 0)
                                        {
                                            curList[curdragIndex].iLDrager.canMove = false;
                                            curList[curdragIndex].mmObj.transform.localPosition = Vector3.zero;
                                            curList[curdragIndex].img.sprite = curList[curdragIndex].bellSprites.sprites[0];
                                            curList[curdragIndex].img.SetNativeSize();
                                            curdragIndex = -1;
                                        }
                                        btnBack.SetActive(true);
                                        SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "kong", false,
                                            () =>
                                            {
                                                SpineManager.instance.DoAnimation(failXem, "xem5", false,
                                            () =>
                                            {
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
                                                mono.StartCoroutine(DelayFunc(0.5f, () => { Bg.transform.DOShakePosition(2f, Vector3.one * 30); }));
                                                SpineManager.instance.DoAnimation(failXem, "xem4", false,
                                                    () =>
                                                    {
                                                        SpineManager.instance.DoAnimation(failXem, "kong", false);
                                                        InitXemPos(() =>
                                                        {
                                                            temCountTime = mono.StartCoroutine(RePlayGame());
                                                        });
                                                    });
                                            });
                                            });
                                    }));
                                });


                        }));
                });


        }

        IEnumerator RePlayGame()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            float timef = SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "xem-jx", false);
            yield return new WaitForSeconds(timef);
            SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "xem1", true);
            GameDataInit(false);
        }

        private void RandomPostion(Action ac = null)
        {
            curLevelArr = curLevelV3s.ToArray();
            GetRandomArray(curLevelArr);

            SpineManager.instance.DoAnimation(xemPos.GetChild(0).gameObject, "kong", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            SpineManager.instance.DoAnimation(paida, curLevel == 0 ? paida.name : paida.name + (curLevel + 1), false,
                () =>
                {
                    SpineManager.instance.DoAnimation(paida, "kong", false,
                    () =>
                    {

                        MMInfo temMMInfo = new MMInfo();
                        for (int i = 0; i < curLevelArr.Length; i++)
                        {
                            temMMInfo.mmObj = mMaskPanel.GetChild(curLevel).GetChild(i).GetChild(0).gameObject;
                            temMMInfo.img = mMaskPanel.GetChild(curLevel).GetChild(i).GetChild(0).GetComponent<Image>();
                            temMMInfo.bellSprites = mMaskPanel.GetChild(curLevel).GetChild(i).GetChild(0).GetComponent<BellSprites>();
                            temMMInfo.img.sprite = temMMInfo.bellSprites.sprites[0];
                            temMMInfo.img.SetNativeSize();
                            temMMInfo.iLDrager = mMaskPanel.GetChild(curLevel).GetChild(i).GetChild(0).GetComponent<ILDrager>();
                            temMMInfo.iLDrager.canMove = true;
                            temMMInfo.iLDrager.dragType = int.Parse(mMaskPanel.GetChild(curLevel).GetChild(i).name);
                            temMMInfo.iLDrager.SetDragCallback();
                            temMMInfo.iLDrager.SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                            temMMInfo.mmObj.transform.parent.position = curLevelArr[i];
                            temMMInfo.mmObj.transform.localPosition = Vector3.zero;
                            temMMInfo.mmObj.SetActive(true);
                            scBoom.GetChild(i).gameObject.SetActive(true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                            SpineManager.instance.DoAnimation(scBoom.GetChild(i).gameObject, "sc-boom-w", false);
                            curList.Add(temMMInfo);
                        }
                        InitXemPos(() => { ac?.Invoke(); });

                        //mono.StartCoroutine(DelayFunc(2f, () =>
                        //{

                        //}));


                    });
                });


        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }


        void StartPlayGame()
        {
            btnBack.SetActive(true);
            SpineManager.instance.DoAnimation(shouzhi, shouzhi.name, false, () => { SpineManager.instance.DoAnimation(shouzhi, "kong", false, () => { btnBack.SetActive(false); }); });

        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    bd.SetActive(false);
                    mask.SetActive(false);
                    StartPlayGame();
                }));
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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
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

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
                SpineManager.instance.DoAnimation(iLDropers[index].transform.GetChild(0).gameObject, "star2", false);
                SpineManager.instance.DoAnimation(iLDropers[index].transform.parent.gameObject, ding.GetChild(curLevel).name + (dropType == 0 ? "" : dropType + 1 + ""), true);
            }
            return dragType == dropType;
        }

        private int curdragIndex = 0;
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
            mask2D.enabled = false;
            curList[type].img.sprite = curList[type].bellSprites.sprites[1];
            curList[type].img.SetNativeSize();
            curList[type].mmObj.transform.parent.SetAsLastSibling();
            curdragIndex = type;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (curdragIndex < 0)
                return;
            curList[type].mmObj.transform.localPosition = Vector3.zero;
            curList[type].img.sprite = curList[type].bellSprites.sprites[0];
            curList[type].img.SetNativeSize();
            curdragIndex = -1;
            if (!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            }
            else
            {
                curList[type].mmObj.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);

                if ((flag & (1 << type)) == 0)
                {
                    flag += 1 << type;
                }
                if (flag >= Mathf.Pow(2, mMaskPanel.GetChild(curLevel).childCount) - 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
                    flag = 0;
                    StopCountTimeCoroutine();
                    int ranIndex = Random.Range(4, 10);
                    ranIndex = ranIndex == 8 ? 9 : ranIndex;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, ranIndex, false);
                    mono.StartCoroutine(DelayFunc(1.5f, () => { SetBloodPanel(); }));
                }
            }
        }

        public Vector2 WorldToScreen(Vector3 position)
        {
            return RectTransformUtility.WorldToScreenPoint(null, position);
        }

    }
}
