using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course927Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private GameObject hxSpine;
        private GameObject fhlSpine;
        private GameObject cqqSpine;
        private GameObject jtSpine;
        private Transform showImg;

        private Transform showSpine;
        private Transform showSpineLeft;
        private Transform showSpineRight;


        private GameObject btnBack;




        bool isPress = false;

        int totalTime = 0;
        bool isEnd = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("max").gameObject;

            hxSpine = curTrans.Find("hxSpine").gameObject;
            hxSpine.SetActive(true);
            fhlSpine = curTrans.Find("fhlSpine").gameObject;
            fhlSpine.SetActive(true);
            cqqSpine = curTrans.Find("cqqSpine").gameObject;
            cqqSpine.SetActive(false);
            jtSpine = curTrans.Find("jtSpine").gameObject;
            jtSpine.SetActive(false);
            showImg = curTrans.Find("showImg");
            showImg.GetImage().sprite = showImg.GetComponent<BellSprites>().sprites[0];
            showImg.GetImage().SetNativeSize();
            showImg.gameObject.SetActive(false);
            showSpine = curTrans.Find("0");
            for (int i = 0; i < showSpine.childCount; i++)
            {
                Util.AddBtnClick(showSpine.GetChild(i).gameObject, OnClickShowSpine);
            }
            showSpineLeft = curTrans.Find("7");
            Util.AddBtnClick(showSpineLeft.GetChild(0).gameObject, OnClickShowSpine);
            showSpineRight = curTrans.Find("8");
            Util.AddBtnClick(showSpineRight.GetChild(0).gameObject, OnClickShowSpine);
            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        Transform temTran;
        private void OnClickShowSpine(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            btnBack.SetActive(true);
            isPress = true;
            temTran = obj.transform;
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(showSpine.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(temTran.parent.gameObject, "t" + obj.name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(showSpineLeft.gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(showSpineRight.gameObject, "kong", false);
                    bool isLeft = (int.Parse(obj.name) % 2 == 0);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, (isLeft ? 6 : 7), () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, (isLeft ? 4 : 5));
                        //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, (isLeft ? 6 : 7));
                        SpineManager.instance.DoAnimation(showSpine.gameObject, (isLeft ? "b" : "a") + 0, false, () => { SpineManager.instance.DoAnimation(showSpine.gameObject, (isLeft ? "b" : "a") + 1, false); });
                    }, () => { isPress = false; }));
                });


        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPress)
                return;
            isPress = true;

            if (temTran.parent.gameObject.activeSelf && int.Parse(temTran.parent.name) > 5)
            {
                totalTime++;
                temTran.parent.gameObject.SetActive(false);
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(showSpine.gameObject, "t" + temTran.GetChild(0).name, false, () =>
            {
                SpineManager.instance.DoAnimation(showSpine.gameObject, "t0", false, () =>
                {
                    SpineManager.instance.DoAnimation(showSpineLeft.gameObject, "t" + showSpineLeft.name, false);
                    SpineManager.instance.DoAnimation(showSpineRight.gameObject, "t" + showSpineRight.name, false);
                    obj.SetActive(false);
                    isPress = false;
                    if (totalTime >= 2 && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            totalTime = 0;
            isEnd = false;
            SpineManager.instance.DoAnimation(fhlSpine, "kong", false);
            SpineManager.instance.DoAnimation(showSpine.gameObject, "kong", false);
            showSpineLeft.gameObject.SetActive(true);
            showSpineRight.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(showSpineLeft.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(showSpineRight.gameObject, "kong", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPress = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                SpineManager.instance.DoAnimation(hxSpine, "animation", true);
            }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

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
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                hxSpine.SetActive(false);
                showImg.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, () => { SpineManager.instance.DoAnimation(fhlSpine, "animation2", false); }, () =>
                {

                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2,
                        () =>
                        {
                            Max.SetActive(false);
                            showImg.gameObject.SetActive(false);
                            showImg.GetImage().sprite = showImg.GetComponent<BellSprites>().sprites[1];
                            showImg.GetImage().SetNativeSize();
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                            SpineManager.instance.DoAnimation(fhlSpine, "animation", false);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));

                }));
            }
            if (talkIndex == 2)
            {
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, () => { showImg.gameObject.SetActive(true); SpineManager.instance.DoAnimation(fhlSpine, "animation2", false); }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, () =>
                {
                    showImg.gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(fhlSpine, "kong", false);
                    cqqSpine.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    SpineManager.instance.DoAnimation(cqqSpine, "animation", false);
                }, () =>
                {
                    jtSpine.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    SpineManager.instance.DoAnimation(jtSpine, "wenhao", false, () => { SpineManager.instance.DoAnimation(jtSpine, "wenhao2", false, () => { SoundManager.instance.ShowVoiceBtn(true); }); });
                }));
            }
            if (talkIndex == 4)
            {
                cqqSpine.SetActive(false);
                jtSpine.SetActive(false);

                btnBack.SetActive(true);
                isPress = true;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5,
                    () =>
                        {
                            SpineManager.instance.DoAnimation(showSpineLeft.gameObject, "t" + showSpineLeft.name, false);
                            SpineManager.instance.DoAnimation(showSpineRight.gameObject, "t" + showSpineRight.name, false);
                        },
                    () =>
                    {
                        btnBack.SetActive(false);
                        isPress = false;
                        Max.SetActive(false);
                    }));
            }

            if (talkIndex == 5)
            {
                isEnd = true;
                btnBack.SetActive(true);
                isPress = true;
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 8, () => { }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 6)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 9, () => { }, () =>
                {
                    btnBack.SetActive(false);
                    isPress = false;
                    Max.SetActive(false);
                }));
            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
