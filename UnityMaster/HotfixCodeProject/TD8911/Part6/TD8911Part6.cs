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
    }
    public class TD8911Part6
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


        private GameObject xiong;
        private GameObject tuZi;
        private Image devilImg;
        private Image scoreImg;
        private Transform diBox;

        private Transform devils;


        List<string> mistakeBBalloons;
        List<string> rightBBalloons;
        List<string> mistakeSBalloons;
        List<string> rightSBalloons;
        List<string> bBalloons;
        List<string> sBalloons;

        string xiongSpine;
        string tuZiSpine;

        //胜利动画名字
        private string sz;
        bool isPlaying = false;

        int totalTime = 0;
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

            xiong = curTrans.Find("xiong").gameObject;
            tuZi = curTrans.Find("tuZi").gameObject;
            devilImg = curTrans.Find("scoreCard/devilImg").GetImage();
            scoreImg = curTrans.Find("scoreCard/scoreImg").GetImage();
            diBox = curTrans.Find("diBox");
            for (int i = 0; i < diBox.childCount; i++)
            {
                Util.AddBtnClick(diBox.GetChild(i).GetChild(0).gameObject, OnClickSelect);
            }
            devils = curTrans.Find("devils");

            //替换胜利动画需要替换spine 
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        private void OnClickSelect(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            obj.transform.DOScale(0.8f, 0.4f).OnComplete(() => { obj.transform.DOScale(1f, 0.4f); });
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            bool isRight = false;
            GameObject temObj = null;
            for (int i = 0; i < rightBBalloons.Count; i++)
            {
                if (rightBBalloons.Contains(obj.transform.parent.name))
                {
                    isRight = true;
                    temObj = xiong;
                }
            }
            for (int i = 0; i < rightSBalloons.Count; i++)
            {
                if (rightSBalloons.Contains(obj.transform.parent.name))
                {
                    isRight = true;
                    temObj = tuZi;
                }
            }
            if (isRight)
            {
                totalTime++;

                BtnPlaySoundSuccess();
                SpineManager.instance.DoAnimation(temObj, obj.transform.parent.name, false, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    SpineManager.instance.DoAnimation(devils.GetChild(totalTime - 1).gameObject, "zd", false, () =>
                  {
                      SpineManager.instance.DoAnimation(devils.GetChild(totalTime - 1).GetChild(0).gameObject, devils.GetChild(totalTime - 1).GetChild(0).name + "-", false,
                          () =>
                              {

                                  SpineManager.instance.DoAnimation(devils.GetChild(totalTime - 1).GetChild(0).gameObject, "kong", false,
                                       () =>
                                           {

                                               scoreImg.sprite = scoreImg.GetComponent<BellSprites>().sprites[totalTime];
                                               scoreImg.SetNativeSize();
                                               if (totalTime >= devils.childCount)
                                               {
                                                   devilImg.sprite = devilImg.GetComponent<BellSprites>().sprites[1];
                                                   devilImg.SetNativeSize();
                                                   playSuccessSpine();
                                               }
                                               else
                                               {
                                                   RandowBox();
                                               }
                                           });

                              });
                      SpineManager.instance.DoAnimation(devils.GetChild(totalTime - 1).gameObject, "kong", false, () => { });
                  });
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                obj.SetActive(false);
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.transform.parent.name, false,
                   () =>
                   {
                       BtnPlaySoundFail();
                       isPlaying = false;
                   });

            }

        }



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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 1)); });
                }

            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            totalTime = 0;
            xiongSpine = "";
            tuZiSpine = "";
            bBalloons = new List<string>();
            sBalloons = new List<string>();
            mistakeBBalloons = new List<string>() { "z1", "z3" };
            rightBBalloons = new List<string>() { "a", "b", "c", "d" };
            mistakeSBalloons = new List<string>() { "z2", "z4", "z5", "z4" };
            rightSBalloons = new List<string>() { "e", "f" };

            bBalloons.AddRange(rightBBalloons);
            bBalloons.AddRange(mistakeBBalloons);

            sBalloons.AddRange(rightSBalloons);
            sBalloons.AddRange(mistakeSBalloons);

            devilImg.sprite = devilImg.GetComponent<BellSprites>().sprites[0];
            devilImg.SetNativeSize();
            scoreImg.sprite = scoreImg.GetComponent<BellSprites>().sprites[0];
            scoreImg.SetNativeSize();
            for (int i = 0; i < devils.childCount; i++)
            {
                devils.GetChild(i).name = "zd";
                devils.GetChild(i).GetChild(0).name = "xem";
                SpineManager.instance.DoAnimation(devils.GetChild(i).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(devils.GetChild(i).GetChild(0).gameObject, "kong", false);
            }

            for (int i = 0; i < devils.childCount; i++)
            {
                mono.StartCoroutine(PlayDevilSpine(i));
            }
            RandowBox();
        }


        IEnumerator PlayDevilSpine(int index)
        {
            yield return new WaitForSeconds(0.1f * index);
            SpineManager.instance.DoAnimation(devils.GetChild(index).GetChild(0).gameObject, devils.GetChild(index).GetChild(0).name, true);

        }
        void RandowBox()
        {
            if (bBalloons.Count <= 0 && sBalloons.Count <= 0)
                return;

            for (int i = 0; i < diBox.childCount; i++)
            {
                diBox.GetChild(i).gameObject.SetActive(false);
            }
            int index = Random.Range(0, 2);
            int index2 = Random.Range(2, 4);
            if (bBalloons.Count <= 0)
            {
                index = 1;
                index2 = 3;
            }
            if (sBalloons.Count <= 0)
            {
                index = 0;
                index2 = 2;
            }
            int imgIndex = 0;
            if (diBox.GetChild(index).GetChild(0).name == "0")
            {
                imgIndex = Random.Range(0, bBalloons.Count);
                RandowScend(index, index2, imgIndex, bBalloons, true);
            }
            else
            {
                imgIndex = Random.Range(0, sBalloons.Count);
                RandowScend(index, index2, imgIndex, sBalloons, false);
            }


        }

        void RandowScend(int index, int index2, int imgIndex, List<string> balloons, bool isbig)
        {
            string tem = balloons[imgIndex];
            balloons.RemoveAt(imgIndex);
            diBox.GetChild(index).name = tem;
            for (int i = 0; i < bellTextures.sprites.Length; i++)
            {
                if (tem == bellTextures.sprites[i].name)
                {
                    diBox.GetChild(index).GetChild(0).GetImage().sprite = bellTextures.sprites[i];
                    diBox.GetChild(index).GetChild(0).gameObject.SetActive(true);
                    diBox.GetChild(index).gameObject.SetActive(true);
                    if (i >= bellTextures.sprites.Length / 2)
                    {
                        if (diBox.GetChild(index2).GetChild(0).name == "0")
                        {
                            xiongSpine = "daiji";
                            tuZiSpine = "daiji4";
                            int imgIndex2 = Random.Range(0, rightBBalloons.Count);
                            diBox.GetChild(index2).name = rightBBalloons[imgIndex2];

                            for (int j = 0; j < bellTextures.sprites.Length; j++)
                            {
                                if (rightBBalloons[imgIndex2] == bellTextures.sprites[j].name)
                                {
                                    diBox.GetChild(index2).GetChild(0).GetImage().sprite = bellTextures.sprites[j];
                                    diBox.GetChild(index2).GetChild(0).gameObject.SetActive(true);
                                    diBox.GetChild(index2).gameObject.SetActive(true);

                                    for (int k = 0; k < bBalloons.Count; k++)
                                    {
                                        if (bBalloons[k] == rightBBalloons[imgIndex2])
                                        {
                                            bBalloons.RemoveAt(k);
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {

                            xiongSpine = "daiji2";
                            tuZiSpine = "daiji3";
                            int imgIndex2 = Random.Range(0, rightSBalloons.Count);
                            diBox.GetChild(index2).name = rightSBalloons[imgIndex2];
                            for (int j = 0; j < bellTextures.sprites.Length; j++)
                            {
                                if (rightSBalloons[imgIndex2] == bellTextures.sprites[j].name)
                                {
                                    diBox.GetChild(index2).GetChild(0).GetImage().sprite = bellTextures.sprites[j];
                                    diBox.GetChild(index2).GetChild(0).gameObject.SetActive(true);
                                    diBox.GetChild(index2).gameObject.SetActive(true);
                                    for (int k = 0; k < sBalloons.Count; k++)
                                    {
                                        if (sBalloons[k] == rightSBalloons[imgIndex2])
                                        {
                                            sBalloons.RemoveAt(k);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (isbig)
                        {
                            xiongSpine = "daiji";
                            tuZiSpine = "daiji4";
                        }
                        else
                        {
                            xiongSpine = "daiji2";
                            tuZiSpine = "daiji3";
                        }
                        if (diBox.GetChild(index2).GetChild(0).name == "0")
                        {
                            int imgIndex2 = Random.Range(0, mistakeBBalloons.Count);
                            diBox.GetChild(index2).name = mistakeBBalloons[imgIndex2];
                            for (int j = 0; j < bellTextures.sprites.Length; j++)
                            {
                                if (mistakeBBalloons[imgIndex2] == bellTextures.sprites[j].name)
                                {
                                    diBox.GetChild(index2).GetChild(0).GetImage().sprite = bellTextures.sprites[j];
                                    diBox.GetChild(index2).GetChild(0).gameObject.SetActive(true);
                                    diBox.GetChild(index2).gameObject.SetActive(true);
                                    for (int k = 0; k < bBalloons.Count; k++)
                                    {
                                        if (bBalloons[k] == mistakeBBalloons[imgIndex2])
                                        {
                                            bBalloons.RemoveAt(k);
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            int imgIndex2 = Random.Range(0, mistakeSBalloons.Count);
                            diBox.GetChild(index2).name = mistakeSBalloons[imgIndex2];
                            for (int j = 0; j < bellTextures.sprites.Length; j++)
                            {
                                if (mistakeSBalloons[imgIndex2] == bellTextures.sprites[j].name)
                                {
                                    diBox.GetChild(index2).GetChild(0).GetImage().sprite = bellTextures.sprites[j];
                                    diBox.GetChild(index2).GetChild(0).gameObject.SetActive(true);
                                    diBox.GetChild(index2).gameObject.SetActive(true);
                                    for (int k = 0; k < sBalloons.Count; k++)
                                    {
                                        if (sBalloons[k] == mistakeSBalloons[imgIndex2])
                                        {
                                            sBalloons.RemoveAt(k);
                                        }
                                    }
                                }
                            }

                        }
                    }

                }

            }
            SpineManager.instance.DoAnimation(xiong, xiongSpine, true);
            SpineManager.instance.DoAnimation(tuZi, tuZiSpine, true);
            isPlaying = false;
        }
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 8, true);

            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));

        }
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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


    }
}
