
using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
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

    public class TD8963Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;
        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject btnBack;


        // 游戏逻辑变量
        private Transform panel;

        private Transform moveBg;
        private Transform scenePanel;
        private Transform objPool;

        private Transform objPoolPos;
        private Vector3[] objPosArr;

        private Transform startPos;
        private Transform drager;
        private GameObject effectStar;
        private GameObject dragerPanel;
        private BoxCollider2D renCollider;
        private EventDispatcher renEventDispatcher;

        private GameObject xem;
        private GameObject dzy;
        private GameObject guang;
        private Transform pinTuPanel;
        private GameObject showStar;
        private Image totalImg;
        private Transform showImgs;
        private BellSprites pinTuNumImg;
        private Transform pinTuBg;

        private Transform pinTuBox;
        private Transform pinTuBoxPos;
        private Text timeText;
        private Text countText;
        private int timeCount = 0;
        private int countTotal = 0;
        private bool isSwaped = false;

        private Vector3 curBlankPos;
        private GameObject starObj;

        private Transform endSpine;
        private Text timeEndText;
        private Text countEndText;
        //胜利动画名字
        private string tz;


        private bool isPlaying = false;
        private int flag = 0;

        private float curDistance = 0;
        private float boundaryX = 0;
        private float boundaryY = 0;
        private float widthBoundary = 0;
        private float heightBoundaryY = 0;
        private bool isStart = false;

        private int tatolNum = 0;

        private Coroutine countTime;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
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

            panel = curTrans.Find("panel");
            moveBg = panel.Find("moveBg");
            scenePanel = moveBg.Find("scenePanel");
            objPool = moveBg.Find("objPool");

            objPoolPos = moveBg.Find("objPoolPos");
            objPosArr = new Vector3[objPoolPos.childCount];

            startPos = moveBg.Find("startPos");

            drager = moveBg.Find("drager");
            dragerPanel = drager.GetChild(1).gameObject;
            Move(dragerPanel);
            renCollider = drager.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>();
            renEventDispatcher = drager.GetChild(0).GetChild(0).GetComponent<EventDispatcher>();
            effectStar = moveBg.Find("effectStar").gameObject;

            xem = scenePanel.Find("xem").gameObject;
            dzy = Bg.transform.Find("Dzy").gameObject;
            guang = Bg.transform.Find("guang").gameObject;

            pinTuPanel = Bg.transform.Find("pinTuPanel");
            showStar = pinTuPanel.GetChild(1).gameObject;
            totalImg = pinTuPanel.GetChild(0).GetComponent<Image>();
            pinTuNumImg = pinTuPanel.GetChild(0).GetComponent<BellSprites>();
            pinTuBg = Bg.transform.Find("pinTuBg");

            timeText = pinTuBg.Find("di/tipImg/tipImgBg/timeText").GetComponent<Text>();
            countText = pinTuBg.Find("di/tipImg/tipImgBg/countText").GetComponent<Text>();

            pinTuBox = pinTuBg.Find("di/pinTuBox");
            pinTuBoxPos = pinTuBg.Find("di/pinTuBoxPos");
            starObj = pinTuBg.Find("di/star").gameObject;

            showImgs = Bg.transform.Find("showImg");

            endSpine = curTrans.Find("mask/end");
            timeEndText = endSpine.GetChild(0).GetComponent<Text>();
            countEndText = endSpine.GetChild(1).GetComponent<Text>();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        IEnumerator DelayFunc(float delayTime, Action callback)
        {
            yield return new WaitForSeconds(delayTime);
            callback?.Invoke();
        }
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

        private void OnClickAnyBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
                        mask.SetActive(false);
                        isStart = true;
                        GameInit();
                        btnBack.SetActive(false);
                        talkIndex = 2;
                    });
                }
                else
                {
                    AnyBtnNextStep(obj, () =>
                    {
                        switchBGM();
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3));
                    });
                }
            });
        }


        void AnyBtnNextStep(GameObject obj, Action callback)
        {
            SpineManager.instance.DoAnimation(obj, "kong", false, () =>
            {
                anyBtns.gameObject.SetActive(false);
                isPlaying = false;
                callback?.Invoke();
            });
        }

        Vector3 initMoveBgPos;
        Vector3 limitMoveBgPos;

        private void GameInit()
        {
            DOTween.KillAll();
            widthBoundary = curTrans.GetRectTransform().sizeDelta.x;
            heightBoundaryY = curTrans.GetRectTransform().sizeDelta.y;
            curDistance = heightBoundaryY - moveBg.GetRectTransform().sizeDelta.y;
            initMoveBgPos = new Vector3(moveBg.transform.localPosition.x, 0, 0);
            limitMoveBgPos = new Vector3(moveBg.transform.localPosition.x, curDistance, 0);
            talkIndex = 1;
            flag = 0;
            tatolNum = 0;
            isPlaying = false;
            isSwaped = false;
            isDrag = false;
            tweener = null;
            MousePos = Vector2.zero;
            moveBg.transform.localPosition = initMoveBgPos;
            BoxCollider2D[] boxCollider2Ds = objPool.GetComponentsInChildren<BoxCollider2D>(true);
            for (int i = 0; i < boxCollider2Ds.Length; i++)
            {

                boxCollider2Ds[i].isTrigger = true;
                boxCollider2Ds[i].size = objPool.GetChild(i).GetRectTransform().sizeDelta;
                if (boxCollider2Ds[i].name.Contains("Doom"))
                {
                    boxCollider2Ds[i].name = "zd";
                }
            }
            for (int i = 0; i < objPoolPos.childCount; i++)
            {
                objPool.GetChild(i).gameObject.SetActive(true);
                objPool.GetChild(i).localScale = Vector3.one * 0.5f;
                SpineManager.instance.DoAnimation(objPool.GetChild(i).gameObject, objPool.GetChild(i).name, true);
                objPosArr[i] = objPoolPos.GetChild(i).position;
            }
            GetRandomArray(objPosArr);

            for (int i = 0; i < objPosArr.Length; i++)
            {
                objPool.GetChild(i).position = objPosArr[i];
            }
            SpineManager.instance.DoAnimation(showStar, "kong", false);
            for (int i = 0; i < showImgs.childCount; i++)
            {
                showImgs.GetChild(i).localPosition = Vector3.zero;
                showImgs.GetChild(i).localScale = Vector3.one;
                showImgs.GetChild(i).gameObject.SetActive(false);

            }
            drager.gameObject.SetActive(true);
            drager.position = startPos.position;
            // drager.rotation = Quaternion.Euler(Vector3.zero);
            drager.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);
            boundaryX = drager.GetRectTransform().sizeDelta.x / 2;
            boundaryY = drager.GetRectTransform().sizeDelta.y - 100;
            renCollider.isTrigger = true;
            renCollider.size = renCollider.transform.GetRectTransform().sizeDelta;
            renEventDispatcher.TriggerEnter2D -= OnTriggerEnter2D;
            renEventDispatcher.TriggerEnter2D += OnTriggerEnter2D;
            SpineManager.instance.DoAnimation(drager.GetChild(0).gameObject, drager.GetChild(0).name, true);
            SpineManager.instance.DoAnimation(drager.GetChild(0).GetChild(1).gameObject, "kong", false);

            effectStar.transform.position = Vector3.zero;
            SpineManager.instance.DoAnimation(effectStar, "kong", false);
            SpineManager.instance.DoAnimation(xem, "kong", false, () => { SpineManager.instance.DoAnimation(xem, xem.name + 3, true); });
            SpineManager.instance.DoAnimation(dzy, "kong", false);
            SpineManager.instance.DoAnimation(guang, "kong", false);
            totalImg.sprite = pinTuNumImg.sprites[tatolNum];
            totalImg.SetNativeSize();
            pinTuBg.gameObject.SetActive(false);
            starObj.transform.localPosition = Vector3.zero;
            starObj.SetActive(true);
            SpineManager.instance.DoAnimation(starObj, "kong", false);
            pinTuPanel.gameObject.SetActive(true);

            curBlankPos = pinTuBox.GetChild(0).transform.localPosition;
            pinTuBox.GetChild(0).gameObject.SetActive(false);

            for (int i = 1; i < pinTuBoxPos.childCount; i++)
            {
                Move(pinTuBox.GetChild(i).gameObject, true);
                pinTuBox.GetChild(i).localPosition = pinTuBoxPos.GetChild(i).transform.localPosition;
                pinTuBox.GetChild(i).rotation = Quaternion.Euler(Vector3.zero);
            }
            timeCount = 0;
            timeText.text = string.Format("{0:D2}:{1:D2}", timeCount / 60, timeCount % 60);
            countTotal = 0;
            countText.text = string.Format("{0}", countTotal);
            endSpine.localScale = Vector3.one * 0.5f;
            SpineManager.instance.DoAnimation(endSpine.gameObject, "kong", false);
            timeEndText.text = "";
            countEndText.text = "";
            countTime = null;
            SwapPiece();
        }


        Tweener tweener;
        private void OnTriggerEnter2D(Collider2D other, int time)
        {
            if (other.name.Contains("b"))
            {
                other.gameObject.SetActive(false);
                int index = int.Parse(other.transform.GetChild(0).name);
                if ((flag & (1 << index)) == 0)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    effectStar.transform.position = objPool.InverseTransformVector(other.transform.position);
                    SpineManager.instance.DoAnimation(effectStar, "star2", false);
                    flag += 1 << index;
                    mono.StartCoroutine(PlayNumPlus(index));
                }
            }
            else if (other.name.Contains("ren2"))
            {
                SetDrager();
                SpineManager.instance.DoAnimation(drager.GetChild(0).gameObject, drager.GetChild(0).name + 3, false,
                    () =>
                    {
                        btnBack.SetActive(false);
                        SpineManager.instance.DoAnimation(drager.GetChild(0).gameObject, drager.GetChild(0).name, true);
                    });
                SpineManager.instance.DoAnimation(drager.GetChild(0).GetChild(1).gameObject, drager.GetChild(0).name + 4, false, () => { SpineManager.instance.DoAnimation(drager.GetChild(0).GetChild(1).gameObject, drager.GetChild(0).name + 2, true); });
            }
            else if (other.name.Contains("zd"))
            {
                SetDrager();
                other.name = "Doom";
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                SpineManager.instance.DoAnimation(other.gameObject, "kong", false);
                SpineManager.instance.DoAnimation(other.transform.GetChild(0).gameObject, other.transform.GetChild(0).name + 2, false, () =>
                {
                    other.gameObject.SetActive(false); btnBack.SetActive(false); moveBg.transform.localPosition = initMoveBgPos;
                    drager.position = startPos.position;
                });

            }
            else
            {
                if (tweener == null || !tweener.IsPlaying())
                {
                    SetDrager();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    tweener = other.transform.DOShakeScale(0.5f).SetEase(Ease.OutSine).SetLoops(1);
                    SpineManager.instance.DoAnimation(drager.GetChild(0).gameObject, drager.GetChild(0).name + 3, false,
                        () =>
                        {
                            btnBack.SetActive(false);
                            SpineManager.instance.DoAnimation(drager.GetChild(0).gameObject, drager.GetChild(0).name, true);
                        });
                }
            }

        }

        private void SetDrager()
        {
            isDrag = false;
            btnBack.SetActive(true);
        }
        IEnumerator PlayNumPlus(int index)
        {

            showImgs.GetChild(index).gameObject.SetActive(true);
            showImgs.GetChild(index).DOScale(0.5f, 1);
            showImgs.GetChild(index).DOLocalMove(WorldToUgui(showStar.transform.position), 1).SetEase(Ease.InOutBack).OnComplete(
                () =>
                {
                    SpineManager.instance.DoAnimation(showStar, "star2", false, () => { SpineManager.instance.DoAnimation(showStar, "kong", false); });
                    totalImg.sprite = pinTuNumImg.sprites[++tatolNum];
                    totalImg.SetNativeSize();
                    showImgs.GetChild(index).gameObject.SetActive(false);
                });
            yield return new WaitForSeconds(1.5f);
            if (flag >= (Mathf.Pow(2, showImgs.childCount) - 1) && isStart)
            {
                btnBack.SetActive(true);
                isDrag = false;
                isStart = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
                SpineManager.instance.DoAnimation(guang, guang.name, true);
                yield return new WaitForSeconds(3f);
                drager.gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(guang, "kong", false);
                pinTuBg.gameObject.SetActive(true);
                mask.SetActive(true);
                anyBtns.gameObject.SetActive(false);
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null,
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
            }

        }

        private IEnumerator Count_down()
        {//协程方法实现倒计时
            while (timeCount <= 360)
            {
                yield return new WaitForSeconds(1.0f);
                timeCount++;
                timeText.text = string.Format("{0:D2}:{1:D2}", timeCount / 60, timeCount % 60);
            }
            if (timeCount > 360)
            {
                timeText.text = ":080:";
                //自动完成
                SuccessEvent();
            }
        }
        void GameStart()
        {
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }


        void StartPlayGame()
        {
            isStart = true;
            btnBack.SetActive(false);
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
            if (talkIndex == 2)
            {
                bd.SetActive(false);
                mask.SetActive(false);
                btnBack.SetActive(false);
                flag = 0;
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                countTime = mono.StartCoroutine(Count_down());
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

        bool isDrag = false;
        private Vector2 curMousePos;
        private float temAngle = 0;
        private Vector2 MousePos = Vector2.zero;

        void Move(GameObject obj, bool isButton = false)
        {
            if (isButton)
            {
                UIEventListener.Get(obj).onClick = OnClickObj;
            }
            else
            {
                UIEventListener.Get(obj).onBeginDrag = beginDragData =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    SpineManager.instance.DoAnimation(drager.GetChild(0).gameObject, drager.GetChild(0).name, true);
                    SpineManager.instance.DoAnimation(drager.GetChild(0).GetChild(1).gameObject, "kong", false);
                    isDrag = true;
                };

                UIEventListener.Get(obj).onDrag = dragData =>
                {
                    if (isDrag)
                    {
                        curMousePos = RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);
                        BoundaryFunc();
                        drager.transform.position = curMousePos;
                    }

                };
                UIEventListener.Get(obj).onEndDrag = endDragData =>
                {
                    isDrag = false;
                };
            }


        }

        bool isPressed = false;
        private void OnClickObj(GameObject go)
        {
            isPressed = false;
            if (Vector3.Distance(go.transform.localPosition, curBlankPos) <= 330)
            {
                isPressed = true;
                if (isSwaped)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                    //starObj.transform.position = pinTuBox.InverseTransformVector(go.transform.position);
                    //starObj.GetComponent<SkeletonGraphic>().Initialize(true);
                    //SpineManager.instance.DoAnimation(starObj, starObj.name + 2, false);
                    countTotal++;
                    countText.text = string.Format("{0}", countTotal);
                }
                Vector3 temPos = go.transform.localPosition;
                go.transform.localPosition = curBlankPos;
                curBlankPos = temPos;

                if (isSwaped)
                    SwapEndEvent(temPos);

            }
        }


        /// <summary>
        /// 主要是判断游戏结果
        /// </summary>
        /// <param name="targer">空格的坐标</param>
        public void SwapEndEvent(Vector3 targer)
        {
            curBlankPos = targer;
            if (pinTuBox.GetChild(0).localPosition == curBlankPos)
            {
                bool isWin = true;
                for (int i = 1; i < pinTuBox.childCount; i++)
                {
                    if (pinTuBox.GetChild(i).localPosition != pinTuBoxPos.GetChild(i).localPosition)
                    {
                        isWin = false;
                        break;
                    }
                }
                if (isWin && isSwaped)
                {

                    btnBack.SetActive(true);
                    isSwaped = false;
                    if (countTime != null)
                        mono.StopCoroutine(countTime);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                    pinTuBox.GetChild(0).position = Vector3.zero;
                    pinTuBox.GetChild(0).gameObject.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);
                    pinTuBox.GetChild(0).DOMove(pinTuBoxPos.GetChild(0).position, 1f).SetEase(Ease.InBack).OnComplete(
                        () =>
                        {
                            starObj.transform.position = pinTuBox.InverseTransformVector(pinTuBoxPos.GetChild(0).transform.position);
                            starObj.GetComponent<SkeletonGraphic>().Initialize(true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                            SpineManager.instance.DoAnimation(starObj, starObj.name + 2, false);
                            mono.StartCoroutine(DelayFunc(3f, () =>
                            {
                                SuccessEvent();
                            }));
                        });


                }
            }
        }


        private void SuccessEvent()
        {
            mask.SetActive(true);
            SpineManager.instance.DoAnimation(endSpine.gameObject, "kong", false,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
                    endSpine.DOScale(1, 1).SetEase(Ease.OutBack);
                    SpineManager.instance.DoAnimation(endSpine.gameObject, endSpine.name, true);
                    timeEndText.text = timeText.text;
                    countEndText.text = countText.text;
                    mono.StartCoroutine(DelayFunc(4f,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(endSpine.gameObject, "kong", false);
                        mask.SetActive(false);
                        timeEndText.text = "";
                        countEndText.text = "";
                        pinTuPanel.gameObject.SetActive(false);
                        moveBg.transform.localPosition = initMoveBgPos;
                        pinTuBg.gameObject.SetActive(false);
                        SpineManager.instance.DoAnimation(xem, xem.name + 2, true);

                        SpineManager.instance.DoAnimation(dzy, dzy.name, false, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
                            mono.StartCoroutine(DelayFunc(0.2f, () => { SpineManager.instance.DoAnimation(xem, xem.name + "-y", true); }));
                            SpineManager.instance.DoAnimation(dzy, dzy.name + 2, false, () =>
                            {
                                for (int i = 0; i < objPoolPos.childCount; i++)
                                {
                                    SpineManager.instance.DoAnimation(objPool.GetChild(i).gameObject, "kong", false);
                                }
                                SpineManager.instance.DoAnimation(xem, xem.name + "-y2", true);

                                SpineManager.instance.DoAnimation(dzy, dzy.name, true);
                                mono.StartCoroutine(DelayFunc(3f, () => { playSuccessSpine(); }));
                            });
                        });
                    }));
                });
        }

        /// <summary>
        /// 打乱方块的方法
        /// </summary>
        public void SwapPiece()//打乱方块的方法
        {
            int[] step = { -1, 1, -3, 3 };
            int emptyIndex = 0;//空白方块的索引
            int i = 0;
            int index = 0; ;
            while (i < 50)//随机点击各个方块,每点击一次就交换了一次方块
            {
                int temIndex = emptyIndex + step[Random.Range(0, 4)];
                if (index != temIndex)
                {
                    index = temIndex;
                    if (index < 9 && index >= 1)
                    {
                        OnClickObj(pinTuBox.GetChild(index).gameObject);
                        i++;
                    }
                    if (isPressed)//有效点击
                    {
                        emptyIndex = index;
                    }
                }
            }
            bool isInit = true;
            for (int j = 1; j < pinTuBox.childCount; j++)
            {
                if (pinTuBox.GetChild(j).localPosition != pinTuBoxPos.GetChild(j).localPosition&& emptyIndex != 1 && emptyIndex != 3)
                {
                    isInit = false;
                    break;
                }
            }
            if (isInit)
            {
                SwapPiece();
            }
            else
            {
                isSwaped = true;
            }

        }

        /// <summary>
        /// 边界
        /// </summary>
        private void BoundaryFunc()
        {
            if (curMousePos.x <= boundaryX)
            {
                curMousePos.x = boundaryX;
            }
            else if (curMousePos.x >= (widthBoundary - boundaryX))
            {
                curMousePos.x = (widthBoundary - boundaryX);
            }
            if (curMousePos.y <= boundaryY)
            {
                curMousePos.y = boundaryY;
            }
            else if ((curMousePos.y >= (heightBoundaryY - boundaryY)))
            {
                curMousePos.y = (heightBoundaryY - boundaryY);
            }
        }

        void FixedUpdate()
        {
            if (isDrag)
            {
                curMousePos = RectTransformUtility.WorldToScreenPoint(null, Input.mousePosition);
                BoundaryFunc();
                drager.transform.position = curMousePos;
                if (MousePos == Vector2.zero)
                {
                    //空一帧
                }
                else
                {
                    if (MousePos.x > curMousePos.x)
                    {
                        drager.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
                    }
                    if (MousePos.x < curMousePos.x)
                    {
                        drager.transform.GetChild(0).rotation = Quaternion.Euler(0, 180, 0);
                    }
                }
                MousePos = curMousePos;
                if (isStart)
                {
                    if (WorldToUgui(Input.mousePosition).y < 0)
                    {
                        if (moveBg.transform.localPosition.y >= 0)
                        {
                            moveBg.transform.localPosition = initMoveBgPos;
                        }
                        else
                        {
                            moveBg.transform.Translate(Vector3.up * Time.fixedDeltaTime * 200f);
                        }

                    }
                    else
                    {
                        if (moveBg.transform.localPosition.y <= curDistance)
                        {
                            moveBg.transform.localPosition = limitMoveBgPos;
                        }
                        else
                        {
                            moveBg.transform.Translate(Vector3.down * Time.fixedDeltaTime * 200f);
                        }
                    }
                }
            }

        }


        public bool IsRectTransformOverlap(RectTransform rect1, RectTransform rect2)
        {
            float rect1MinX = rect1.position.x - rect1.rect.width / 2;
            float rect1MaxX = rect1.position.x + rect1.rect.width / 2;
            float rect1MinY = rect1.position.y - rect1.rect.height / 2;
            float rect1MaxY = rect1.position.y + rect1.rect.height / 2;

            float rect2MinX = rect2.position.x - rect2.rect.width / 2;
            float rect2MaxX = rect2.position.x + rect2.rect.width / 2;
            float rect2MinY = rect2.position.y - rect2.rect.height / 2;
            float rect2MaxY = rect2.position.y + rect2.rect.height / 2;

            bool xNotOverlap = rect1MaxX <= rect2MinX || rect2MaxX <= rect1MinX;
            bool yNotOverlap = rect1MaxY <= rect2MinY || rect2MaxY <= rect1MinY;

            bool notOverlap = xNotOverlap || yNotOverlap;

            return !notOverlap;
        }


        public bool IsIntersect(Transform tran1, Transform tran2)
        {
            bool isIntersect;
            //另一个矩形的位置大小信息;
            Vector3 moveOrthogonPos = tran2.position;
            Vector3 moveOrthogonScale = tran2.localScale;
            //自己矩形的位置信息
            Vector3 smallOrthogonPos = tran1.position;
            Vector3 smallOrthogonScale = tran1.localScale;
            //分别求出两个矩形X或Z轴的一半之和
            float halfSum_X = (moveOrthogonScale.x * 0.5f) + (smallOrthogonScale.x * 0.5f);
            float halfSum_Z = (moveOrthogonScale.z * 0.5f) + (smallOrthogonScale.z * 0.5f);
            //分别求出两个矩形X或Z轴的距离
            float distance_X = Mathf.Abs(moveOrthogonPos.x - smallOrthogonPos.x);
            float distance_Z = Mathf.Abs(moveOrthogonPos.z - smallOrthogonPos.z);
            //判断X和Z轴的是否小于他们各自的一半之和
            if (distance_X <= halfSum_X && distance_Z <= halfSum_Z)
            {
                isIntersect = true;
                Debug.Log("相交");
            }
            else
            {
                isIntersect = false;
                Debug.Log("不相交");
            }
            return isIntersect;
        }
        public Vector2 WorldToUgui(Vector3 position)
        {
            Vector2 pixlPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(curTrans.GetRectTransform(), position, null, out pixlPoint);
            return pixlPoint;
        }
    }
}
