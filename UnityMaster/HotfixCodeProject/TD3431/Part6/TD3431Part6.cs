
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
    public class TD3431Part6
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

        private Transform droperTran;
        private ILDroper droper;
        private Transform panel2;
        private Transform shiYePos;
        private Transform shiYe;

        private Transform panel3;
        private Transform imgPos;
        private Transform imgPanel;


        private Transform cwStartPos;
        private Transform cwTemPos;
        private Transform cwEndPos;

        private GameObject cw;
        private GameObject xem;
        private Transform lastPanel;
        private Transform lastPanel2;
        private Transform panel;
        private Transform jianDao;
        private Transform jianDaoPos;


        private GameObject showSpine;
        private GameObject spineMask;

        List<Transform> ilDragerTrans;
        List<ILDrager> ilDragerList;

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

        ILDroper[] iLDropersImg;

        private int flag = 0;
        private bool isPlaying = false;

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

            droperTran = curTrans.Find("droperTran");
            droper = curTrans.Find("droper").GetComponent<ILDroper>();
            droper.SetDropCallBack(OnAfter);

            panel2 = curTrans.Find("panel2");
            shiYePos = panel2.Find("shuYePos");
            shiYe = panel2.Find("shuYe");
            ilDragerList = new List<ILDrager>();
            ilDragerTrans = new List<Transform>();

            ILDrager[] iLDragers = shiYe.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < iLDragers.Length; i++)
            {
                ilDragerList.Add(iLDragers[i]);
                ilDragerTrans.Add(shiYePos.GetChild(i));
            }
            panel3 = curTrans.Find("panel3");

            imgPos = panel3.Find("imgPos/imgPos");
            imgPanel = panel3.Find("imgPanel");
            iLDragers = imgPanel.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < iLDragers.Length; i++)
            {
                ilDragerList.Add(iLDragers[i]);
                ilDragerTrans.Add(imgPos.GetChild(i));
            }

            for (int i = 0; i < ilDragerList.Count; i++)
            {
                ilDragerList[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }
            lastPanel = curTrans.Find("lastPanel");
            lastPanel2 = curTrans.Find("lastPanel2");

            cwStartPos = curTrans.Find("fixedPanel/startPos");
            cwTemPos = curTrans.Find("fixedPanel/temPos");
            cwEndPos = curTrans.Find("fixedPanel/endPos");
            cw = curTrans.Find("fixedPanel/cw").gameObject;
            xem = curTrans.Find("fixedPanel/xem").gameObject;

            panel = curTrans.Find("panel");
            iLDropersImg = panel.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < iLDropersImg.Length; i++)
            {
                iLDropersImg[i].SetDropCallBack(OnAfterOne);
            }
            jianDaoPos = panel.Find("jianDaoPos");
            jianDao = panel.Find("jianDao");
            jianDao.position = jianDaoPos.position;
            jianDao.GetComponent<ILDrager>().SetDragCallback(OnBeginDragOne, OnDragOne, OnEndDragOne);

            showSpine = curTrans.Find("panel/showSpine").gameObject;
            spineMask = curTrans.Find("spineMask").gameObject;

            tz = "3-5-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
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
                        panel.gameObject.SetActive(true);
                        xem.SetActive(true);
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
            spineMask.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];

            ILDrager[] temObjs = ilDragerList.ToArray();
            for (int i = 0; i < temObjs.Length; i++)
            {
                int tem = int.Parse(temObjs[i].name);
                ILDrager temILDrager;
                temILDrager = ilDragerList[tem];
                ilDragerList[tem] = temObjs[i];
            }

            for (int i = 0; i < droperTran.childCount; i++)
            {
                droperTran.GetChild(i).gameObject.SetActive(false);
            }
            droperTran.gameObject.SetActive(false);
            for (int i = 0; i < ilDragerTrans.Count; i++)
            {
                ilDragerList[i].transform.position = ilDragerTrans[i].position;
                ilDragerList[i].gameObject.SetActive(true);
            }
            showSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(showSpine, "kong", false);
            panel.gameObject.SetActive(false);
            jianDao.gameObject.SetActive(true);
            for (int i = 0; i < panel.childCount - 3; i++)
            {
                panel.GetChild(i).gameObject.SetActive(false);
                iLDropersImg[i].isActived = false;
            }
            panel.GetChild(0).gameObject.SetActive(true);
            iLDropersImg[0].isActived = true;
            panel2.gameObject.SetActive(false);
            for (int i = 0; i < shiYe.childCount; i++)
            {
                shiYe.GetChild(i).gameObject.SetActive(false);
                shiYePos.GetChild(i).gameObject.SetActive(false);
            }
            panel3.gameObject.SetActive(false);
            lastPanel.gameObject.SetActive(false);
            lastPanel2.gameObject.SetActive(false);
            
            cw.transform.position = cwStartPos.position;
            SpineManager.instance.DoAnimation(cw, "kong", false);


            xem.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(xem, xem.name, true);
            xem.SetActive(false);
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () =>
                {
                    ShowDialogue("我讨厌一切，是不会让你们得逞的！", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            ShowDialogue("加油！我们一定会战胜小恶魔！", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));

            });


        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
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
                        xem.SetActive(true);
                        panel.gameObject.SetActive(true);
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            droperTran.GetChild(dragType).gameObject.SetActive(true);

            return true;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            ilDragerList[index].transform.SetAsLastSibling();
            ilDragerList[index].transform.position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            ilDragerList[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {

            if (isMatch)
            {
                if ((flag & (1 << index)) <= 0)
                {
                    flag += (1 << index);
                }

                if (flag == Mathf.Pow(2, shiYe.childCount) - 1)
                {
                    spineMask.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { spineMask.SetActive(false); }));
                    panel2.gameObject.SetActive(false);
                    panel3.gameObject.SetActive(true);

                }
                if (flag >= (Mathf.Pow(2, droperTran.childCount - 1) - 1))
                {
                    spineMask.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { spineMask.SetActive(false); }));
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                    panel3.gameObject.SetActive(false);
                    lastPanel.gameObject.SetActive(true);
                    lastPanel2.gameObject.SetActive(true);

                    SpineManager.instance.DoAnimation(cw, cw.name, true);
                    cw.transform.DOMove(cwTemPos.position, 1.5f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(cw, cw.name + 2, true);
                            mono.StartCoroutine(WaitTime(
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                                    SpineManager.instance.DoAnimation(cw, cw.name + 3, true);
                                    cw.transform.DOMove(cwEndPos.position, 1.5f).SetEase(Ease.InQuart).OnComplete(
                                        ()=> 
                                        {
                                            SpineManager.instance.DoAnimation(cw, "kong", false);
                                            SpineManager.instance.DoAnimation(xem, xem.name + "-boom", false,
                                                   () =>
                                                   {
                                                       xem.transform.DOMove(xem.transform.position, 2f).OnComplete(() => { playSuccessSpine(); });
                                                   });
                                        });

                                   
                                }));

                        });


                }
                ilDragerList[index].gameObject.SetActive(false);
            }
            ilDragerList[index].transform.DOMove(ilDragerTrans[index].position, 1);
        }

        int temIndex = 0;
        private bool OnAfterOne(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
                temIndex = index;
                jianDao.gameObject.SetActive(false);
                if ((flag & (1 << index)) <= 0)
                {
                    flag += (1 << index);
                }

            }
            return dragType == dropType;
        }

        IEnumerator WaitTime(Action ac)
        {
            yield return new WaitForSeconds(1.5f);
            ac?.Invoke();
        }

        private void OnBeginDragOne(Vector3 pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

            jianDao.position = Input.mousePosition;
        }

        private void OnDragOne(Vector3 pos, int type, int index)
        {
            jianDao.position = Input.mousePosition;
        }

        private void OnEndDragOne(Vector3 pos, int type, int index, bool isMatch)
        {

            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                panel.GetChild(temIndex).gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SpineManager.instance.DoAnimation(showSpine, iLDropersImg[temIndex].name, false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(showSpine, iLDropersImg[temIndex].name + 2, false,
                                () =>
                                {
                                    droperTran.GetChild(0).DOMove(droperTran.GetChild(0).position, 0.8f).OnComplete(
                                         () =>
                                         {
                                             SpineManager.instance.DoAnimation(showSpine, "kong", false,
                                                         () =>
                                                         {
                                                             droperTran.gameObject.SetActive(true);
                                                             if (temIndex == 0)
                                                             {
                                                                 droperTran.GetChild(0).gameObject.SetActive(true);
                                                             }
                                                             else
                                                             {
                                                                 panel2.gameObject.SetActive(true);
                                                                 shiYePos.GetChild(temIndex - 1).gameObject.SetActive(true);
                                                                 ilDragerList[temIndex - 1].gameObject.SetActive(true);
                                                             }

                                                             if (flag >= Mathf.Pow(2, shiYe.childCount + 1) - 1)
                                                             {
                                                                 spineMask.SetActive(true);
                                                                 mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { spineMask.SetActive(false); }));
                                                                 jianDao.gameObject.SetActive(false);
                                                                 flag = 0;
                                                                 jianDao.gameObject.SetActive(false);
                                                                 panel.gameObject.SetActive(false);
                                                             }
                                                             else
                                                             {
                                                                 mono.StartCoroutine(WaitTime(() =>
                                                                         {
                                                                             panel2.gameObject.SetActive(false);
                                                                             droperTran.gameObject.SetActive(false);
                                                                             jianDao.gameObject.SetActive(true);
                                                                             iLDropersImg[temIndex + 1].isActived = true;
                                                                             iLDropersImg[temIndex + 1].gameObject.SetActive(true);
                                                                         }));
                                                             }

                                                         });
                                         });
                                });
                        });
            }
            jianDao.DOMove(jianDaoPos.position, 1);
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
