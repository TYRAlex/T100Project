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
    }
    public class TD5612Part4
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

        private Transform catShadows;
        private Transform catsPos;

        private Transform cats;
        private ILDrager[] catsILDragers;
        private ILDroper[] catsILDropers;
        private List<GameObject> catLists;

        private GameObject bossDi;
        private GameObject boss;
        private Transform bossPos;

        private Image blood0;
        private Image blood1;

        private Transform boom;

        //胜利动画名字
        private string tz;
        private string sz;
        private int totalTime = 0;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            bossDi = curTrans.Find("Bg/bossDi").gameObject;
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
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            catShadows = curTrans.Find("catShadows");
            cats = curTrans.Find("cats");
            catsPos = curTrans.Find("catsPos");
            catsILDragers = cats.GetComponentsInChildren<ILDrager>(true);
            catsILDropers = cats.GetComponentsInChildren<ILDroper>(true);

            catLists = new List<GameObject>();

            for (int i = 0; i < cats.childCount; i++)
            {
                for (int j = cats.childCount - 1; j >= 0; j--)
                {
                    if (j != i)
                    {
                        catLists.Add(cats.GetChild(j).gameObject);
                    }
                }
                if (catsILDragers[i].drops.Length <= 0)
                {
                    catsILDragers[i].AddDrops(catLists.ToArray());
                    catLists.Clear();
                }

                catsILDragers[i].GetComponent<SkeletonGraphic>().raycastTarget = true;
                catsILDropers[i].SetDropCallBack(OnDoAfter);
                catsILDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }
            boss = curTrans.Find("boss").gameObject;
            bossPos = curTrans.Find("boss/bossPos");

            boom = curTrans.Find("boom");
            boom.gameObject.SetActive(false);
            blood0 = curTrans.Find("blood/0").GetComponent<Image>();
            blood1 = curTrans.Find("blood/1").GetComponent<Image>();

            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        private bool OnDoAfter(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
                boom.position = cats.GetChild(index).position;
                boom.gameObject.SetActive(true);
                cats.GetChild(index).gameObject.SetActive(false);
                catShadows.GetChild(index).gameObject.SetActive(false);
            }
            else
            {
                SpineManager.instance.DoAnimation(cats.GetChild(index).gameObject, cats.GetChild(index).name + "2", false, () => { SpineManager.instance.DoAnimation(cats.GetChild(index).gameObject, cats.GetChild(index).name, true); });
            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 Pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            setCatsIsAction(index, false);
            catShadows.GetChild(index).gameObject.SetActive(false);
        }


        private void OnDrag(Vector3 Pos, int type, int index)
        {
            cats.GetChild(index).position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 Pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {
                float temBlood = 0.1f;
                catsILDragers[index].isActived = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), () =>
                {
                    cats.GetChild(index).DOMove(catsPos.GetChild(index).position, 1).OnComplete(() => { catShadows.GetChild(index).gameObject.SetActive(true); SpineManager.instance.DoAnimation(cats.GetChild(index).gameObject, cats.GetChild(index).name, true); });
                    blood0.fillAmount += temBlood;
                }, () => {
                    catsILDragers[index].isActived = true;
                    setCatsIsAction(index, true); }));

            }
            else
            {
                BtnPlaySoundSuccess();
                cats.GetChild(index).gameObject.SetActive(false);
                totalTime++;
                cats.GetChild(index).transform.DOMove(catsPos.GetChild(index).position, 2.5f).OnComplete(()=> { setCatsIsAction(index, true); });
                boom.DOMove(bossPos.position, 1).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    if (totalTime >= 3)
                    {
                        bossDi.SetActive(true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                        SpineManager.instance.DoAnimation(boss, "3", false, () =>
                        {
                            SpineManager.instance.DoAnimation(boss, "4", true);
                            playSuccessSpine(/*() => { setCatsIsAction(index, true); }*/);
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(boss, "2", false, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                            SpineManager.instance.DoAnimation(boss, "1", true);
                            //setCatsIsAction(index, true);
                        });
                    };

                    boom.gameObject.SetActive(false);
                    blood1.fillAmount = 1 - totalTime / 3f;
                    blood0.fillAmount = blood1.fillAmount;
                });
            }
            UpdatePanel();
        }

        void UpdatePanel()
        {
            for (int i = 0; i < cats.childCount; i++)
            {
                if (!cats.GetChild(i).gameObject.activeSelf)
                {
                    catShadows.GetChild(cats.GetChild(i).GetComponent<ILDrager>().index).gameObject.SetActive(false);
                }
            }
        }
        void setCatsIsAction(int index, bool isActive)
        {
            for (int i = 0; i < cats.childCount; i++)
            {
                if (i != index)
                {
                   catsILDragers[i].isActived = isActive;
                }
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
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3)); });

                }
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            blood0.fillAmount = 1;
            blood1.fillAmount = 1;
            totalTime = 0;
            bossDi.SetActive(false);
            SpineManager.instance.DoAnimation(boss, "1", true);

            for (int i = 0; i < catsPos.childCount; i++)
            {
                catShadows.GetChild(i).gameObject.SetActive(true);
                for (int j = 0; j < cats.childCount; j++)
                {
                    if (catsILDragers[j].index == i)
                    {
                        cats.GetChild(j).position = catsPos.GetChild(i).position;
                    }
                }

                cats.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(cats.GetChild(i).gameObject, cats.GetChild(i).name, true);
            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 8, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () =>
                 {
                     ShowDialogue("既然人类都不爱护你们，还去伤害你们，那我就彻底毁了你们吧，我不允许有任何人来帮助你们！", devilText);
                 }, () =>
                 {
                     buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                     {
                         mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                         {
                             ShowDialogue("不！我们会阻止你的!", bdText);
                         }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                     });
                 }));

            });
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
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
    }
}
