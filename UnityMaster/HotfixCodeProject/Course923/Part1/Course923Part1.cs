using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course923Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        bool isPlaying = false;
        bool isPress = false;
        private Transform spineShow;
        private GameObject btnBack;

        private int flag = 0;
        private bool isEnd = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            spineShow = curTrans.Find("spineShow");
            for (int i = 0; i < spineShow.childCount; i++)
            {
                Util.AddBtnClick(spineShow.GetChild(i).gameObject, OnClickBtn);
            }
            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPress)
                return;
            isPress = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(spineShow.gameObject, "kong", false, () =>
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                SpineManager.instance.DoAnimation(spineShow.gameObject, "animation", false, () =>
                {
                    btnBack.SetActive(false);
                    isPlaying = false;
                    isPress = false;
                    if (flag >= (Mathf.Pow(2, spineShow.childCount) - 1) && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }

        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            if (bell.activeSelf)
            {
                bell.SetActive(false);
            }
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
            {
                flag += (1 << obj.transform.GetSiblingIndex());
            }
            int tem = obj.transform.GetSiblingIndex() + 1;
            SpineManager.instance.DoAnimation(spineShow.gameObject, obj.name, false, () =>
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[tem];
                if (tem == 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1,
                        () =>
                        { SpineManager.instance.DoAnimation(spineShow.gameObject, "ly" + tem, false); },
                        () =>
                            {
                                bell.SetActive(true);
                                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, tem * 2, null, () => { bell.SetActive(false); btnBack.SetActive(true); }));
                            }));


                }
                else
                {
                    mono.StartCoroutine(PlaySpine(tem));
                }
            });
        }

        IEnumerator PlaySpine(int tem)
        {
            float temTime = 0;
            for (int i = 1; i < 7; i++)
            {
                temTime = SpineManager.instance.DoAnimation(spineShow.gameObject, "ly" + tem + i, false, () =>
                {
                    if (i == 2)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[tem + 1];
                    }
                    else if (i == 5)
                    {
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[tem];
                    }

                });
                yield return new WaitForSeconds(temTime);
            }
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, tem * 2, null, () => { bell.SetActive(false); btnBack.SetActive(true); }));
        }
        private void GameInit()
        {
            talkIndex = 1;
            isPress = false;
            flag = 0;
            isEnd = false;
            SpineManager.instance.DoAnimation(spineShow.gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(spineShow.gameObject, "animation", false, () => { }); });

        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            bell.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); isPlaying = false; }));

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
                speaker = bell;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    {
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[4];
                        isEnd = true;
                        bell.SetActive(true);
                        isPlaying = true;
                        SpineManager.instance.DoAnimation(spineShow.gameObject, "kong", false);
                        mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    }
                    break;
                case 2:
                    {
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                        SpineManager.instance.DoAnimation(spineShow.gameObject, "animation", false, () => { });
                        mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 6, null, () => { bell.SetActive(false); isPlaying = false; }));
                    }
                    break;
                default:
                    break;
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
