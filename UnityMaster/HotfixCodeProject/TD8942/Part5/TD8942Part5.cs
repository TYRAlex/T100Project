
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
    public class PPInfo
    {
        public int index = 0;
        public Image img;
        public BellSprites bellSprites;
        private int next = 0;
        public int Next
        {
            get => next;
            set
            {
                if (value >= 7)
                {
                    next = value % 7;
                }
                else
                {
                    next = value;
                }
            }
        }
    }
    public class TD8942Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject xemTalker;
        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject btnBack;

        private Transform skyBgTran;
        private Image skyBg;
        private Image shuBg;

        private GameObject xem;
        private Transform xemPos;
        private Transform dingDing;
        private Transform dingDingPos;
        private Transform ren;
        private Transform qiPao;
        private Transform lh;
        private GameObject pp;
        private GameObject paoPao;

        private GameObject zwyc;

        private Transform ppImgs;
        private Transform collider;
        private Transform niao;
        private Transform niaoStartPos;
        private Transform ppList;

        private Transform tipPanel;
        private Transform tipPanelStartPos;
        private Transform tipPanelEndPos;

        //胜利动画名字
        private string tz;

        private int flag = 0;
        private bool isPlaying = false;
        private List<Tweener> tweenerList;
        private List<PPInfo> ppInfoList;

        private EventDispatcher eventDispatcher;
        private CapsuleCollider2D capsuleCollider;
        private float animationTime = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            xemTalker = curTrans.Find("mask/xemTalker").gameObject;
            xemTalker.SetActive(false);
            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(0).gameObject.SetActive(true);

            skyBgTran = curTrans.Find("panel/skyBg");
            skyBg = skyBgTran.GetImage();
            shuBg = skyBgTran.Find("shuBg").GetImage();

            skyBg.material = new Material(Shader.Find("Unlit/Texture"));
            shuBg.material = new Material(Shader.Find("Unlit/Transparent"));
            skyBg.material.SetFloat("_ScrollY", 0f);
            shuBg.material.SetFloat("_ScrollY", 0f);

            xemPos = curTrans.Find("panel/xemPos");
            xem = curTrans.Find("panel/xem").gameObject;
            dingDing = curTrans.Find("panel/dingDing");
            dingDingPos = curTrans.Find("panel/dingDingPos");
            ren = dingDing.Find("ren");
            pp = ren.Find("p").gameObject;
            paoPao = dingDing.Find("paopao").gameObject;

            ppImgs = ren.Find("ppImgs");
            qiPao = ren.Find("qiPao");
            lh = ren.Find("lh");

            collider = dingDing.Find("collider");
            capsuleCollider = collider.GetComponent<CapsuleCollider2D>();
            eventDispatcher = collider.GetComponent<EventDispatcher>();
            eventDispatcher.CollisionEnter2D += OnCollisionEnter2D;
            MoveRen(dingDing.gameObject);
            niao = curTrans.Find("panel/niao");
            niaoStartPos = curTrans.Find("panel/niaoStartPos");
            niao.position = niaoStartPos.position;
            ppList = niao.Find("ppList");

            tipPanel = curTrans.Find("panel/tipPanel");
            tipPanelEndPos = curTrans.Find("panel/tipPanelEndPos");
            tipPanelStartPos = curTrans.Find("panel/tipPanelStartPos");
            zwyc = curTrans.Find("panel/zwyc").gameObject;

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);



            tz = "6-12-z";
            tweenerList = new List<Tweener>();
            ppInfoList = new List<PPInfo>();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameInit();
            //GameStart();
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            BtnPlaySound();
            obj.SetActive(false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SpineManager.instance.DoAnimation(zwyc, "kong", false);
            GameInit(false);
        }

        void TipPanelUpdate(int index, int curNameIndex)
        {
            SpineManager.instance.DoAnimation(tipPanel.GetChild(index).GetChild(0).gameObject, "star6", false);
            SpineManager.instance.DoAnimation(tipPanel.GetChild(index).gameObject, "qqq" + (curNameIndex + 1), false);
        }


        private void OnCollisionEnter2D(Collision2D c, int time)
        {
            curPPNameIndex = int.Parse(c.transform.name) - 1;
            if (curPPNameIndex < 7)
            {
                if (curPPIndex == 0 || (curPPIndex > 0 && ppInfoList[curPPIndex - 1].Next == curPPNameIndex))
                {
                    for (int k = 0; k < tweenerList.Count; k++)
                    {
                        tweenerList[k].Pause();
                    }
                    isPlaying = true;
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                    SpineManager.instance.DoAnimation(collider.GetChild(0).gameObject, collider.GetChild(0).name, false, () => { SpineManager.instance.DoAnimation(collider.GetChild(0).gameObject, "kong", false); });
                    SpineManager.instance.DoAnimation(c.gameObject, "kong", false);
                    c.gameObject.SetActive(false);
                    int tem = curPPNameIndex;
                    qiPao.DOScale(1.2f, 0.2f).OnComplete
                     (
                       () =>
                       {
                           qiPao.DOScale(1.25f, 0.2f).OnComplete
                            (
                                () =>
                                {
                                    if (curPPIndex == 0)
                                    {
                                        tipPanel.DOMove(tipPanelEndPos.position, 1f).SetEase(Ease.OutBack).OnComplete(
                                         () =>
                                         {
                                             TipPanelUpdate(0, tem);
                                         });
                                    }
                                    else
                                    {
                                        TipPanelUpdate(curPPIndex, tem);
                                    }
                                    for (int k = 0; k < tweenerList.Count; k++)
                                    {
                                        tweenerList[k].Play();
                                    }
                                    isPlaying = false;

                                    ppInfoList[curPPIndex].index = tem;
                                    ppInfoList[curPPIndex].img.sprite = ppInfoList[curPPIndex].bellSprites.sprites[tem];

                                    ppInfoList[curPPIndex].Next = tem + 1;
                                    ppImgs.GetChild(curPPIndex).gameObject.SetActive(true);
                                    curPPIndex++;
                                    if (curPPIndex == 2)
                                    {
                                        for (int k = 0; k < tweenerList.Count; k++)
                                        {
                                            tweenerList[k].timeScale = 1.5f;
                                        }
                                        change = 7;
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(5).gameObject, qiPao.GetChild(5).name + 2, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(5).gameObject, qiPao.GetChild(5).name, true); });
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(6).gameObject, qiPao.GetChild(6).name + 2, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(6).gameObject, qiPao.GetChild(6).name, true); });
                                    }
                                    else if (curPPIndex == 4)
                                    {
                                        for (int k = 0; k < tweenerList.Count; k++)
                                        {
                                            tweenerList[k].timeScale = 2f;
                                        }
                                        change = 6;
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(2).gameObject, qiPao.GetChild(2).name, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(2).gameObject, qiPao.GetChild(2).GetChild(0).name, true); });
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(7).gameObject, qiPao.GetChild(7).name, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(7).gameObject, qiPao.GetChild(7).GetChild(0).name, true); });
                                    }
                                    else if (curPPIndex == 6)
                                    {
                                        for (int k = 0; k < tweenerList.Count; k++)
                                        {
                                            tweenerList[k].timeScale = 2.5f;
                                        }
                                        change = 5;
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(1).gameObject, qiPao.GetChild(1).name, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(1).gameObject, ren.GetChild(1).GetChild(0).name, true); });
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(8).gameObject, qiPao.GetChild(8).name, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(8).gameObject, qiPao.GetChild(8).GetChild(0).name, true); });
                                    }
                                    else if (curPPIndex == 7)
                                    {
                                        for (int k = 0; k < tweenerList.Count; k++)
                                        {
                                            tweenerList[k].timeScale = 3f;
                                        }
                                        change = 4;
                                        SpineManager.instance.DoAnimation(qiPao.GetChild(0).gameObject, qiPao.GetChild(0).name + 2, false, () => { SpineManager.instance.DoAnimation(qiPao.GetChild(0).gameObject, qiPao.GetChild(0).name, true); });
                                    }


                                    if (curPPIndex >= 7)
                                    {
                                        for (int k = 0; k < ppList.childCount; k++)
                                        {
                                            ppList.GetChild(k).gameObject.SetActive(false);
                                        }
                                        for (int k = 0; k < tweenerList.Count; k++)
                                        {
                                            tweenerList[k].Pause();
                                        }
                                        isPlaying = true;
                                        SpineManager.instance.DoAnimation(xem, xem.name + 1, true);
                                        ppImgs.gameObject.SetActive(false);
                                        qiPao.gameObject.SetActive(false);
                                        SpineManager.instance.DoAnimation(lh.gameObject, lh.name, false,
                                         () =>
                                         {
                                             SpineManager.instance.DoAnimation(lh.gameObject, lh.name + 2, false,
                                              () =>
                                              {
                                                  lh.DOMove(xemPos.position, 0.5f).SetEase(Ease.InBack).OnComplete(
                                                      () =>
                                                      {
                                                          SpineManager.instance.DoAnimation(lh.gameObject, "kong", false);
                                                          SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                                                          SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                                                          SpineManager.instance.DoAnimation(xem, "sc-boom", false);
                                                          xem.transform.DOMove(xem.transform.position, 3f).OnComplete(
                                                                   () => { playSuccessSpine(); isPlaying = false; });

                                                      });
                                              });
                                         });
                                    }
                                });
                       });


                }
                else
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                }
            }
            else
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                SpineManager.instance.DoAnimation(collider.GetChild(0).gameObject, "kong", false);
                capsuleCollider.isTrigger = true;
                for (int k = 0; k < tweenerList.Count; k++)
                {
                    tweenerList[k].Pause();
                }
                isPlaying = true;
                ppImgs.gameObject.SetActive(false);
                qiPao.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(pp, pp.name, false);
                SpineManager.instance.DoAnimation(paoPao, paoPao.name + 2, false,
                  () =>
                  {
                      SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                      SpineManager.instance.DoAnimation(ren.gameObject, ren.name + 6, true);

                      dingDing.DOMoveY(-300, 1f).OnComplete(
                          () =>
                          {
                              btnBack.SetActive(true);
                              SpineManager.instance.DoAnimation(zwyc.gameObject, zwyc.name, false, () => { isPlaying = false; });
                          });
                  });



            }
        }

        List<int> temInts;
        private int GetRandomNum(int len)
        {
            int tem = -1;
            while (tem < 0)
            {
                tem = Random.Range(0, len);
                if (temInts != null && temInts.Contains(tem))
                {
                    tem = -1;
                }
                else
                {
                    temInts.Add(tem);
                }
            }
            return tem;
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
            //SoundManager.instance.StopAudio(SoundManager.SoundType.BGM); 
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                    });
                }
                else if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        GameStart();

                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        mask.SetActive(false);
                        GameInit(false);
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        switchBGM(); anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2));
                    });
                }
            });
        }

        private int posX = 0;
        private int posY = 0;
        private int ppIndex = 0;
        private int ppType = 0;
        private string ppName = "";

        private int change = 0;
        void RandomPP()
        {
            for (int i = 0; i < ppList.childCount; i++)
            {

                ppList.GetChild(i).gameObject.SetActive(true);
                posX = Random.Range(150, 1770);
                posY = Random.Range(300, 4421 - Screen.height);
                ppType = i - change;
                ppIndex = Random.Range(1, 8);
                if (ppIndex == 1)
                {
                    ppName = "";
                }
                else
                {
                    ppName = ppIndex + "";
                }
                ppList.GetChild(i).localPosition = new Vector2(posX, posY);

                if (ppType > 0)
                {
                    ppList.GetChild(i).name = (ppIndex + 8) + "";
                }
                else
                {
                    ppList.GetChild(i).name = ppIndex + "";
                }
                SpineManager.instance.DoAnimation(ppList.GetChild(i).gameObject, ppType > 0 ? "xem-p" + ppName : "pp" + ppName, true);
            }
        }

        private void GameInit(bool init = true)
        {
            DOTween.KillAll();
            talkIndex = 1;
            flag = 0;
            curPPIndex = 0;
            curPPNameIndex = 0;
            change = 8;
            animationTime = 15.5f;
            isPlaying = false;
            skyBg.material.SetTextureOffset("_MainTex", Vector2.zero);
            shuBg.material.SetTextureOffset("_MainTex", Vector2.zero);
            tweenerList.Clear();
            ppInfoList.Clear();

            tipPanel.position = tipPanelStartPos.position;
            SkeletonGraphic[] tipPanelSG = tipPanel.GetComponentsInChildren<SkeletonGraphic>(true);
            for (int i = 0; i < tipPanelSG.Length; i++)
            {
                tipPanelSG[i].Initialize(true);
            }

            skyBgTran.GetChild(2).position = skyBgTran.GetChild(0).position;
            skyBgTran.GetChild(4).position = skyBgTran.GetChild(1).position;
            lh.position = qiPao.position;
            dingDing.position = dingDingPos.position;

            capsuleCollider.isTrigger = false;
            ppImgs.gameObject.SetActive(true);
            for (int i = 0; i < ppImgs.childCount; i++)
            {
                PPInfo temPPInfo = new PPInfo();
                temPPInfo.index = 0;
                temPPInfo.img = ppImgs.GetChild(i).GetImage();
                temPPInfo.bellSprites = ppImgs.GetChild(i).GetComponent<BellSprites>();
                temPPInfo.Next = temPPInfo.index + 1;
                ppInfoList.Add(temPPInfo);
                ppImgs.GetChild(i).gameObject.SetActive(false);
            }
            niao.position = niaoStartPos.position;
            SkeletonGraphic[] skeletonGraphics = skyBgTran.GetComponentsInChildren<SkeletonGraphic>(true);
            for (int i = 0; i < skeletonGraphics.Length; i++)
            {
                SpineManager.instance.DoAnimation(skeletonGraphics[i].gameObject, skeletonGraphics[i].name, true);
            }

            for (int i = 0; i < ppList.childCount; i++)
            {
                ppList.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(ppList.GetChild(i).gameObject, "kong", false);
            }
            SpineManager.instance.DoAnimation(niao.GetChild(0).gameObject, niao.name, true);
            SpineManager.instance.DoAnimation(niao.GetChild(1).gameObject, niao.name + 2, true);
            SpineManager.instance.DoAnimation(xem, "kong", false);
            SpineManager.instance.DoAnimation(zwyc, "kong", false);
            qiPao.gameObject.SetActive(true);
            SkeletonGraphic[] dingDingSG = dingDing.GetComponentsInChildren<SkeletonGraphic>(true);
            for (int i = 0; i < dingDingSG.Length; i++)
            {
                dingDingSG[i].Initialize(true);
                SpineManager.instance.DoAnimation(dingDingSG[i].gameObject, "kong", false);
            }
            SpineManager.instance.DoAnimation(ren.gameObject, ren.name + 0, true);
            SpineManager.instance.DoAnimation(paoPao, paoPao.name, true);
            RandomPP();
            if (!init)
            {
                StartPlayGame();
            }

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
            xemTalker.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(xemTalker, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }

        void StartPlayGame()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            SpineManager.instance.DoAnimation(ren.gameObject, ren.name + 1, false, () => { SpineManager.instance.DoAnimation(ren.gameObject, ren.name + 0, true); });
            SpineManager.instance.DoAnimation(qiPao.GetChild(3).gameObject, qiPao.GetChild(3).name, false);
            SpineManager.instance.DoAnimation(qiPao.GetChild(4).gameObject, qiPao.GetChild(4).name, false,
                         () =>
                         {
                             SpineManager.instance.DoAnimation(qiPao.GetChild(3).GetChild(0).gameObject, qiPao.GetChild(3).GetChild(0).name, true);
                             SpineManager.instance.DoAnimation(qiPao.GetChild(4).GetChild(0).gameObject, qiPao.GetChild(4).GetChild(0).name, true);
                             tweenerList.Add(skyBg.material.DOOffset(Vector2.up, animationTime + 2).SetEase(Ease.Flash).SetLoops(-1));
                             tweenerList.Add(shuBg.material.DOOffset(Vector2.up, animationTime).SetEase(Ease.Flash).SetLoops(-1).OnStepComplete(() => { RandomPP(); }));
                             tweenerList.Add(niao.DOLocalMoveY(-shuBg.transform.GetRectTransform().rect.height, animationTime).SetEase(Ease.Flash).SetLoops(-1));
                             skyBgTran.GetChild(2).DOLocalMoveY(-shuBg.transform.GetRectTransform().rect.height, animationTime).SetEase(Ease.Flash);
                             skyBgTran.GetChild(4).DOLocalMoveY(-shuBg.transform.GetRectTransform().rect.height, animationTime).SetEase(Ease.Flash);
                         });

            for (int k = 0; k < tweenerList.Count; k++)
            {
                tweenerList[k].timeScale = 1f;
            }
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
                xemTalker.SetActive(false);
                bd.SetActive(true);
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
        private int curPPIndex = 0;

        private int curPPNameIndex = 0;

        void MoveRen(GameObject obj)
        {
            UIEventListener.Get(obj).onDrag = dragData =>
            {
                if (isPlaying)
                    return;
                if (Input.mousePosition.x > dingDing.position.x)
                {
                    if (dingDing.position.x >= (Screen.width - dingDing.GetRectTransform().rect.width / 2))
                        return;
                    dingDing.position = new Vector2(Input.mousePosition.x, dingDing.position.y);
                }
                if (Input.mousePosition.x < dingDing.position.x)
                {
                    if (dingDing.position.x <= dingDing.GetRectTransform().rect.width / 2)
                        return;
                    dingDing.position = new Vector2(Input.mousePosition.x, dingDing.position.y);
                }
            };

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

    }
}
