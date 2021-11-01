
using DG.Tweening;
using Spine.Unity;
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
    public class DragerInfo
    {
        public ILDrager[] iLDragers;
        public BellSprites[] bellSprites;
        public Image[] imgs;
    }
    public class TD6741Part5
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

        private Transform panel;

        private Transform yuStartPos;
        private Transform yuEndPos;
        private Transform yuBg;
        private Transform yuBgPos;
        private GameObject img;
        private GameObject img2;

        private Transform startPos;
        private Transform endPos;


        private GameObject spineShow;
        private GameObject di;

        private Transform showPanels;
        private Transform showPanelEndPos;
        private Transform showPanelStartPos;
        private GameObject p;
        private GameObject dt;
        private GameObject xem;
        private GameObject star;
        private GameObject btnNext;

        private Transform endBg;
        private Transform endPanel;
        private GameObject line;
        private Transform jinhuaList;
        private Transform centerPos;

        private GameObject spineMask;



        //胜利动画名字
        private string tz;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;


        private int flag = 0;
        private bool isPlaying = false;
        private int curLevel = 0;

        private List<DragerInfo> listDragerInfo;
        private List<int> temInts;
        private List<ILDroper[]> listDroper;
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

            buDing = curTrans.Find("mask/buDing").gameObject;
            buDing.SetActive(true);
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdText.text = "";
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devil.SetActive(true);
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilText.text = "";
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
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


            panel = curTrans.Find("panel");
            yuStartPos = panel.Find("yuStartPos");
            yuEndPos = panel.Find("yuEndPos");
            yuBgPos = panel.Find("yuBgPos");
            yuBg = panel.Find("yuBg");
            yuBg.gameObject.SetActive(true);         

            listDragerInfo = new List<DragerInfo>();
            temInts = new List<int>();
            listDroper = new List<ILDroper[]>();
            for (int i = 0; i < yuBg.childCount; i++)
            {
                DragerInfo temDInfo = new DragerInfo();
                temDInfo.iLDragers = yuBg.GetChild(i).GetComponentsInChildren<ILDrager>(true);
                temDInfo.bellSprites = yuBg.GetChild(i).GetComponentsInChildren<BellSprites>(true);
                temDInfo.imgs = yuBg.GetChild(i).GetComponentsInChildren<Image>(true);
                temInts.Clear();
                for (int j = 0; j < temDInfo.iLDragers.Length; j++)
                {
                    temDInfo.iLDragers[j].dragType = GetRandomNum(temDInfo.iLDragers.Length);
                    temDInfo.imgs[j+1].sprite = temDInfo.bellSprites[j].sprites[temDInfo.iLDragers[j].dragType * 2];
                    temDInfo.iLDragers[j].index = j;
                    temDInfo.iLDragers[j].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                    temDInfo.iLDragers[j].transform.position = yuBgPos.GetChild(i).GetChild(j).position;
                }
                listDragerInfo.Add(temDInfo);
            }

            img = panel.Find("img").gameObject;
            img2 = panel.Find("img2").gameObject;

            startPos = panel.Find("startPos");
            endPos = panel.Find("endPos");

            spineShow = panel.Find("d").gameObject;
            di = panel.Find("di").gameObject;
            di.SetActive(true);
            showPanels = panel.Find("showPanels");
            showPanels.gameObject.SetActive(true);
            showPanelEndPos = panel.Find("showPanelEndPos");
            showPanelStartPos = panel.Find("showPanelStartPos");
            showPanels.position = showPanelStartPos.position;
            for (int i = 0; i < showPanels.childCount; i++)
            {
                ILDroper[] temdropers = showPanels.GetChild(i).GetComponentsInChildren<ILDroper>(true);
                for (int j = 0; j < temdropers.Length; j++)
                {
                    temdropers[j].dropType = j;
                    temdropers[j].index = j;
                    temdropers[j].SetDropCallBack(OnAfter);
                }
                listDroper.Add(temdropers);
            }
            p = panel.Find("p").gameObject;

            dt = panel.Find("dt").gameObject;
            xem = panel.Find("xem").gameObject;
            star = panel.Find("star").gameObject;
            btnNext = panel.Find("btnNext").gameObject;
            Util.AddBtnClick(btnNext, OnClickBtnNext);
            btnNext.SetActive(false);
            endBg = curTrans.Find("endBg");
            endPanel = curTrans.Find("endPanel");
            endPanel.gameObject.SetActive(false);
            line = endPanel.Find("x").gameObject;
            jinhuaList = endPanel.Find("jinhuaList");
            centerPos = endPanel.Find("centerPos");

            spineMask = curTrans.Find("spineMask").gameObject;
            spineMask.SetActive(false);
            tz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameInit();
            //GameStart();
        }




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
                        GameInit();
                        SwitchAnim();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        switchBGM(); anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3));
                    });
                }
            });
        }


        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            isPlaying = false;
            curLevel = 0;
            panel.gameObject.SetActive(true);
            img2.SetActive(false);
            img.SetActive(true);
            endBg.localScale = Vector3.one * 0.8f;
            endBg.GetRawImage().CrossFadeAlpha(0, 0, false);
            spineShow.transform.position = startPos.position;
            SpineManager.instance.DoAnimation(spineShow, spineShow.name + "1", true);
            for (int i = 0; i < spineShow.transform.GetChild(0).childCount; i++)
            {
                SpineManager.instance.DoAnimation(spineShow.transform.GetChild(0).GetChild(i).gameObject, spineShow.transform.GetChild(0).name, true);
            }
            yuBg.position = yuStartPos.position;
            yuBgPos.position = yuStartPos.position;
            for (int i = 0; i < yuBg.childCount; i++)
            {
                yuBg.GetChild(i).gameObject.SetActive(false);
                for (int j = 0; j < yuBg.GetChild(i).childCount; j++)
                {
                    yuBg.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < showPanels.childCount; i++)
            {
                showPanels.GetChild(i).gameObject.SetActive(false);
                for (int j = 0; j < showPanels.GetChild(i).childCount; j++)
                {
                    showPanels.GetChild(i).GetChild(j).gameObject.SetActive(false);
                }
            }

            SpineManager.instance.DoAnimation(p, "kong", false);

            SpineManager.instance.DoAnimation(dt, "kong", false);
            SpineManager.instance.DoAnimation(xem, "kong", false);
            star.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(line, "kong", false);
            for (int i = 0; i < jinhuaList.childCount; i++)
            {
                jinhuaList.GetChild(i).gameObject.SetActive(false);
                centerPos.GetChild(i).localScale = Vector3.one;
                centerPos.GetChild(i).transform.position = centerPos.position;
                centerPos.GetChild(i).gameObject.SetActive(false);
            }

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () =>
                {
                    ShowDialogue("哈哈，本大王来啦。", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            ShowDialogue("小朋友们，小恶魔又来了，我们一起来赶跑它吧~", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));

            });


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
                buDing.SetActive(false);
                devil.SetActive(false);
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null,
                    () =>
                    {
                        bd.SetActive(false);
                        mask.SetActive(false);
                        SwitchAnim();
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
                star.transform.position = listDroper[curLevel][index].transform.position;
                listDroper[curLevel][index].gameObject.SetActive(true);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            }

            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            listDragerInfo[curLevel].iLDragers[index].transform.SetAsLastSibling();
            listDragerInfo[curLevel].imgs[index+1].sprite = listDragerInfo[curLevel].bellSprites[index].sprites[listDragerInfo[curLevel].iLDragers[index].dragType * 2 + 1];
            listDragerInfo[curLevel].iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            listDragerInfo[curLevel].iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            spineMask.SetActive(true);
            listDragerInfo[curLevel].imgs[index+1].sprite = listDragerInfo[curLevel].bellSprites[index].sprites[listDragerInfo[curLevel].iLDragers[index].dragType * 2];
            listDragerInfo[curLevel].iLDragers[index].transform.position = yuBgPos.GetChild(curLevel).GetChild(index).position;
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { spineMask.SetActive(false); }));
                listDragerInfo[curLevel].iLDragers[index].gameObject.SetActive(false);
                if ((flag & (1 << index)) <= 0)
                {
                    flag += (1 << index);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                if (flag >= (Mathf.Pow(2, listDragerInfo[curLevel].iLDragers.Length) - 1))
                {
                    flag = 0;
                    curLevel++;
                    if (curLevel <= 1)
                    {
                        SpineManager.instance.DoAnimation(star, star.name + 6, false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            SpineManager.instance.DoAnimation(dt, dt.name, false,
                              () =>
                              {
                                  for (int i = 0; i < spineShow.transform.GetChild(0).childCount; i++)
                                  {
                                      SpineManager.instance.DoAnimation(spineShow.transform.GetChild(0).GetChild(i).gameObject, "kong", false);
                                  }
                                  img.SetActive(false);
                                  img2.SetActive(true);
                                  SwitchLevel();
                              });
                        });                 
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(star, star.name + 6, false,()=> { btnNext.SetActive(true); });         
                    }
                }
                else
                {
                    SpineManager.instance.DoAnimation(star, star.name + 6, false);
                }
            }
            else
            {
                spineMask.SetActive(false);
            }

        }

        private void OnClickBtnNext(GameObject obj)
        {
            obj.SetActive(false);
            if (curLevel >= 4)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(spineShow, "kong", false);
                SpineManager.instance.DoAnimation(xem, xem.name + 1, false, () =>
                {
                    mono.StartCoroutine(PlayEndAnimation());
                });

            }
            else
            {
                SwitchLevel();
            }
        }

        void SwitchAnim()
        {
            spineMask.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
            if (curLevel > 0)
            {
                yuBg.GetChild(curLevel - 1).gameObject.SetActive(false);
                showPanels.GetChild(curLevel - 1).gameObject.SetActive(false);
            }
            yuBg.GetChild(curLevel).gameObject.SetActive(true);
            showPanels.GetChild(curLevel).gameObject.SetActive(true);
            yuBg.position = yuStartPos.position;
            yuBgPos.position = yuStartPos.position;
            showPanels.position = showPanelStartPos.position;
            yuBg.DOMove(yuEndPos.position, 2).SetEase(Ease.OutBack).OnComplete(()=> { yuBgPos.position = yuEndPos.position; });
            showPanels.DOMove(showPanelEndPos.position, 2).SetEase(Ease.OutBack).OnComplete(()=> { spineMask.SetActive(false); });
        }
        void SwitchLevel()
        {
            SpineManager.instance.DoAnimation(spineShow, "kong", false,
              () =>
              {
                  SwitchAnim();
                  spineShow.transform.position = endPos.position;
                  SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                  SpineManager.instance.DoAnimation(spineShow, spineShow.name + (curLevel + 1) + "-c", false,
                  () =>
                  {
                      SpineManager.instance.DoAnimation(spineShow, spineShow.name + (curLevel + 1), true);
                  });
                  SpineManager.instance.DoAnimation(p, p.name, false);
              });
        }


        IEnumerator PlayEndAnimation()
        {
            float lineTime = 0;
            lineTime = SpineManager.instance.DoAnimation(xem, xem.name + 2, false,
                    () =>
                    {
                        panel.gameObject.SetActive(false);
                        endPanel.gameObject.SetActive(true);
                    });
            yield return new WaitForSeconds(lineTime / 2);
            endBg.GetRawImage().CrossFadeAlpha(1, 1.5f, false);
            endBg.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.InBack).OnComplete(() => { endBg.DOScale(Vector3.one, 1.5f).SetEase(Ease.InBack); });
            yield return new WaitForSeconds(3);
            for (int i = 0; i < centerPos.childCount; i++)
            {
                centerPos.GetChild(i).gameObject.SetActive(true);
                centerPos.GetChild(i).DOScale(Vector3.one * 2, 2).OnComplete(
                    () =>
                    {
                        centerPos.GetChild(i).gameObject.SetActive(false);
                        jinhuaList.GetChild(i).gameObject.SetActive(true);
                    });
                yield return new WaitForSeconds(3);
            }
            lineTime = SpineManager.instance.DoAnimation(line, "x", false);
            yield return new WaitForSeconds(lineTime + 2);
            playSuccessSpine();
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
                text.text += str[i];
                if (i == 30)
                {
                    text.text = "";
                }
                i++;
                yield return new WaitForSeconds(0.1f);
            }
            callBack?.Invoke();
            yield break;
        }


    }
}
