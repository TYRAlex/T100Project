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
    public class TD3424Part5
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
        private Transform spineShows;
        private Transform posShows;
        private Transform dragers;
        private Transform drops;
        private GameObject endSpine;


        private Transform panel2;
        private GameObject XEM;
        private Transform LZ2;
        private GameObject LZ;

        private Transform foodPanel;
        private Transform foodPos;
        private Transform comparePanel;

        //胜利动画名字
        private string tz;
        private string sz;


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

        private string[] Names;
        private ILDrager[] iLDragers;
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
            panel.gameObject.SetActive(true);
            spineShows = curTrans.Find("panel/spineShows");
            posShows = curTrans.Find("panel/posShows");
            posShows.gameObject.SetActive(false);
            dragers = curTrans.Find("panel/dragers");
            drops = curTrans.Find("panel/drops");

            endSpine = curTrans.Find("panel/guang").gameObject;
            for (int i = 0; i < posShows.childCount; i++)
            {
                dragers.GetChild(i).GetComponent<ILDrager>().SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                drops.GetChild(i).GetComponent<ILDroper>().SetDropCallBack(OnAfter);
            }

            panel2 = curTrans.Find("panel2");
            panel2.GetComponent<RawImage>().enabled = true;
            panel2.gameObject.SetActive(false);
            LZ2 = curTrans.Find("panel2/LZ2");
            LZ = curTrans.Find("panel2/LZ").gameObject;
            XEM = curTrans.Find("panel2/XEM").gameObject;
            foodPanel = curTrans.Find("panel2/foodPanel");
            iLDragers = foodPanel.GetComponentsInChildren<ILDrager>(true);
            Names = new string[] { "S4", "S5", "S8", "S1", "S10", "S2", "S3", "S6", "S7", "S9" };
            LZ2.GetChild(0).GetComponent<ILDroper>().SetDropCallBack(OnAfter2);
            for (int i = 0; i < foodPanel.childCount; i++)
            {
                iLDragers[i].SetDragCallback(OnBeginDrag2, OnDrag2, OnEndDrag2);
            }

            foodPos = curTrans.Find("panel2/foodPos");
            foodPos.gameObject.SetActive(false);
            comparePanel = curTrans.Find("panel2/comparePanel");

            temList = new List<int>();
            tz = "3-5-z";
            sz = "6-12-z";
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
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });

                }
                else if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);

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
                                        ShowDialogue("不好！新鲜测量枪被小恶魔弄坏了！怎么办？丁丁要参加比赛了！", bdText);
                                    }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                                });
                            }));

                        });
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameReStart(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 11)); });

                }
            });
        }

        private void GameReStart()
        {
            temList.Clear();
            SpineManager.instance.DoAnimation(XEM, XEM.name, false);
            SpineManager.instance.DoAnimation(LZ, "kong", false);
            SpineManager.instance.DoAnimation(LZ2.gameObject, "kong", false);
            iLDragers = null;
            iLDragers = foodPanel.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < foodPanel.childCount; i++)
            {
                foodPanel.GetChild(i).name = Names[i];
                iLDragers[i].index = i;
                if (i < foodPanel.childCount / 2)
                {
                    iLDragers[i].dragType = 0;
                }
                else
                {
                    iLDragers[i].dragType = 1;
                }
                foodPanel.GetChild(i).position = foodPos.GetChild(i).position;
                foodPanel.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(foodPanel.GetChild(i).gameObject, Names[i], false);
            }

            LZ2.gameObject.SetActive(true);
            LZ2.GetChild(0).gameObject.SetActive(true);
            for (int i = 1; i < LZ2.childCount; i++)
            {
                LZ2.GetChild(i).gameObject.SetActive(false);
            }


        }

        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];

            SpineManager.instance.DoAnimation(panel.GetChild(0).gameObject, panel.GetChild(0).name, false);
            SpineManager.instance.DoAnimation(panel.GetChild(2).gameObject, panel.GetChild(2).name, false);

            for (int i = 0; i < spineShows.childCount; i++)
            {
                SpineManager.instance.DoAnimation(spineShows.GetChild(i).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(spineShows.GetChild(i).GetChild(0).gameObject, "kong", false);
                dragers.GetChild(i).position = posShows.GetChild(i).position;
                dragers.GetChild(i).gameObject.SetActive(true);
                dragers.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            SpineManager.instance.DoAnimation(endSpine, "kong", false);

            SpineManager.instance.DoAnimation(XEM, "kong", false);
            LZ2.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(LZ2.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(LZ, "kong", false);
            for (int i = 0; i < foodPanel.childCount; i++)
            {

                foodPanel.GetChild(i).name = Names[i];
                iLDragers[i].index = i;
                if (i < foodPanel.childCount / 2)
                {
                    iLDragers[i].dragType = 0;
                }
                else
                {
                    iLDragers[i].dragType = 1;
                }

                foodPanel.GetChild(i).transform.position = foodPos.GetChild(i).position;
                foodPanel.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(foodPanel.GetChild(i).gameObject, "kong", false);
            }

            LZ2.GetChild(0).gameObject.SetActive(false);
            for (int i = 1; i < LZ2.childCount; i++)
            {
                LZ2.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < comparePanel.childCount; i++)
            {
                SpineManager.instance.DoAnimation(comparePanel.GetChild(i).gameObject, "kong", false);
            }



        }

        void GameStart()
        {
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            panel.gameObject.SetActive(false);
            panel2.gameObject.SetActive(true);
            devilText.text = "";
            bdText.text = "";
            devil.transform.position = devilStartPos.position;
            buDing.transform.position = bdStartPos.position;
            devil.SetActive(true);
            buDing.SetActive(true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 3, () =>
                {
                    ShowDialogue("想修好没那么容易！", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 4, () =>
                        {
                            ShowDialogue("丁丁！你还是学习下辨别食材的方法吧！", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));

            });
            SpineManager.instance.DoAnimation(XEM, XEM.name, true);
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                buDing.SetActive(false);
                devil.SetActive(false);
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () => { mask.SetActive(false); bd.SetActive(false); }));
            }
            if (talkIndex == 2)
            {
                buDing.SetActive(false);
                devil.SetActive(false);
                mask.SetActive(false);
                //说话动效
                foodPos.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(comparePanel.GetChild(0).gameObject, "B4", false);
                SpineManager.instance.DoAnimation(comparePanel.GetChild(1).gameObject, "B3", false);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 5, null, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 6, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                        SpineManager.instance.DoAnimation(comparePanel.GetChild(0).gameObject, "B4a", false);
                    }, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 7, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                SpineManager.instance.DoAnimation(comparePanel.GetChild(1).gameObject, "B3a", false);
                            }, () =>
                            {
                                mono.StartCoroutine(WaitTime(2f, null, () =>
                                {
                                    SpineManager.instance.DoAnimation(comparePanel.GetChild(0).gameObject, "B2", false);
                                    SpineManager.instance.DoAnimation(comparePanel.GetChild(1).gameObject, "B1", false);

                                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 8, () =>
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                                        SpineManager.instance.DoAnimation(comparePanel.GetChild(0).gameObject, "B2a", false);
                                    }, () =>
                                    {
                                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 9, () =>
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                            SpineManager.instance.DoAnimation(comparePanel.GetChild(1).gameObject, "B1a", false);
                                        }, () =>
                                        {
                                            mono.StartCoroutine(WaitTime(2f, null, () =>
                                            {
                                                anyBtns.gameObject.SetActive(false);
                                                mask.SetActive(true);
                                                dbd.SetActive(true);
                                                mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 10, null, () =>
                                                {
                                                    SpineManager.instance.DoAnimation(comparePanel.GetChild(0).gameObject, "kong", false);
                                                    SpineManager.instance.DoAnimation(comparePanel.GetChild(1).gameObject, "kong", false);
                                                    mask.SetActive(false);
                                                    dbd.SetActive(false);
                                                    panel2.GetComponent<RawImage>().enabled = false;
                                                    for (int i = 0; i < foodPanel.childCount; i++)
                                                    {
                                                        foodPanel.GetChild(i).gameObject.SetActive(true);
                                                        SpineManager.instance.DoAnimation(foodPanel.GetChild(i).gameObject, Names[i], false);
                                                    }
                                                    LZ2.GetChild(0).gameObject.SetActive(true);
                                                    foodPos.gameObject.SetActive(false);

                                                }));

                                            }));
                                        }));
                                    }));
                                }));

                            }));
                        }));
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


        IEnumerator WaitTime(float time, Action fFunc, Action sFunc)
        {
            fFunc?.Invoke();
            yield return new WaitForSeconds(time);
            sFunc?.Invoke();
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
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                if (i == 30)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }


        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {
                SpineManager.instance.DoAnimation(spineShows.GetChild(index).gameObject, spineShows.GetChild(index).name, false, () => { SpineManager.instance.DoAnimation(spineShows.GetChild(index).GetChild(0).gameObject, spineShows.GetChild(index).GetChild(0).name, false); });
            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            dragers.GetChild(index).GetChild(0).gameObject.SetActive(true);
            dragers.GetChild(index).position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            dragers.GetChild(index).position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            posShows.gameObject.SetActive(true);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            if (!isMatch)
            {
                dragers.GetChild(index).GetChild(0).gameObject.SetActive(false);
                dragers.GetChild(index).gameObject.SetActive(true);
                dragers.GetChild(index).DOMove(posShows.GetChild(index).position, 1f).OnComplete(() => { posShows.gameObject.SetActive(false); });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                flag += 1 << index;
                dragers.GetChild(index).GetChild(0).gameObject.SetActive(false);
                dragers.GetChild(index).gameObject.SetActive(false);
                dragers.GetChild(index).DOMove(posShows.GetChild(index).position, 1f).OnComplete(() =>
                {
                    posShows.gameObject.SetActive(false);
                    if (flag >= (Mathf.Pow(2, dragers.childCount) - 1))
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONBGM);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                        SpineManager.instance.DoAnimation(endSpine, endSpine.name, false, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 5, null, () =>
                            {
                                mono.StartCoroutine(WaitTime(3f, null, () =>
                                {
                                    flag = 0;
                                    mask.SetActive(true);
                                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                    anyBtns.gameObject.SetActive(true);
                                    anyBtns.GetChild(0).gameObject.SetActive(true);
                                }));

                            }));
                        });
                    }
                });

            }
        }



        private bool OnAfter2(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
            }
            return dragType == dropType;
        }

        private void OnBeginDrag2(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.SetAsLastSibling();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnDrag2(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.position = Input.mousePosition;
        }
        List<int> temList;
        private void OnEndDrag2(Vector3 pos, int type, int index, bool isMatch)
        {
            foodPos.gameObject.SetActive(true);
            if (!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(XEM, XEM.name + ((flag >= (Mathf.Pow(2, LZ2.childCount - 3) - 1)) ? "3" : "2"), false, () => { SpineManager.instance.DoAnimation(XEM, XEM.name, true); });
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                iLDragers[index].transform.DOMove(foodPos.GetChild(index).position, 1f).OnComplete(() => { foodPos.gameObject.SetActive(false); });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                flag += 1 << index;
                temList.Add(index);
                iLDragers[index].gameObject.SetActive(false);

                LZ2.GetChild(index + 1).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(LZ2.gameObject, LZ2.name, false, () =>
                {
                    SpineManager.instance.DoAnimation(XEM, XEM.name + "4", false, () =>
                    {
                        if (flag == (Mathf.Pow(2, (LZ2.childCount - 1)) - 1))
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONBGM);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
                            LZ2.GetChild(0).gameObject.SetActive(false);
                            SpineManager.instance.DoAnimation(LZ2.gameObject, "kong", false, () => { LZ2.gameObject.SetActive(false); });
                            SpineManager.instance.DoAnimation(LZ, LZ.name, false, () =>
                            {
                                SpineManager.instance.DoAnimation(LZ, "kong", false);
                                SpineManager.instance.DoAnimation(XEM, XEM.name + "5", false, () =>
                                {
                                    foodPos.gameObject.SetActive(false);
                                    SpineManager.instance.DoAnimation(XEM, "kong", false);
                                    flag = 0;
                                    playSuccessSpine();
                                });
                            });

                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(XEM, XEM.name, true);
                            foodPos.gameObject.SetActive(false);
                        }
                    });

                });
            }
        }
    }
}
