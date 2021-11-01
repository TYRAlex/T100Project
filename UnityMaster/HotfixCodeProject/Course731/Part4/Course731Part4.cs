using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course731Part4
    {
        #region 通用变量
        private int talkIndex;

        bool isPlaying = false;

        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        #endregion

        #region 游戏变量
        GameObject background;

        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();

            GameInit();

            GameStart();
        }

        void LoadGame()
        {
            background = curTrans.Find("Background").gameObject;
            background.SetActive(true);

        }

        void GameInit()
        {
            talkIndex = 1;

            SkeletonGraphic graphic = background.GetComponent<SkeletonGraphic>();
            graphic.startingAnimation = "";
            graphic.Initialize(true);
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeakAndAnimation(background, null, "0", "1", 0, () =>
            {
                isPlaying = false;
                
                SoundManager.instance.ShowVoiceBtn(true);
            }));


        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);

            if (talkIndex == 1)
            {
                Max.SetActive(false);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                SpineManager.instance.DoAnimation(background, "2", false, () =>
                {
                    SpineManager.instance.DoAnimation(background, "3", false, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                        {
                            Max.SetActive(true);

                            mono.StartCoroutine(SpeakAndAnimation(background, b: "5", index: 2, method: () =>
                            {

                                mono.StartCoroutine(SpeakAndAnimation(background, "6", "7", "8", 3, () =>
                                {
                                    Max.SetActive(false);

                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                                    SkeletonGraphic graphic = background.GetComponent<SkeletonGraphic>();
                                    graphic.startingAnimation = "19";
                                    graphic.Initialize(true);

                                    mono.StartCoroutine(SpeakAndAnimation(background, a: "3", b: "9", index: 4, method: () =>
                                    {
                                        Max.SetActive(true);

                                        mono.StartCoroutine(SpeakAndAnimation(background, "10", "11", "12", 5, () =>
                                        {
                                            mono.StartCoroutine(SpeakAndAnimation(background, "13", "14", "15", 6, () =>
                                            {
                                                mono.StartCoroutine(SpeakAndAnimation(background, "16", "17", "18", 7, () =>
                                                {
                                                    mono.StartCoroutine(SpeakAndAnimation(background, null, "0", "1", 8));
                                                }));
                                            }));
                                        }));
                                    }));
                                }));
                            }));
                        }));

                        mono.StartCoroutine(WaitFor(3.5f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                            SpineManager.instance.DoAnimation(background, "4", false);
                        }));
                    });
                });

            }

            talkIndex++;
        }

        //播放动画和语音
        IEnumerator SpeakAndAnimation(GameObject obj, string a = null, string b = null, string c = null, int index = -1, Action method = null)
        {
            float _time = 0f;
            if (a != null) _time = SpineManager.instance.DoAnimation(obj, a, false);
            yield return new WaitForSeconds(_time);

            float speakTime = 0f;
            if (index != -1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index));
                speakTime = SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, index);      
            }

            float animationTime = 0f;
            if (b != null) animationTime = SpineManager.instance.DoAnimation(obj, b, false);

            _time = speakTime > animationTime ? speakTime : animationTime;
            yield return new WaitForSeconds(_time);

            _time = 0f;
            if (c != null) _time = SpineManager.instance.DoAnimation(obj, c, false);
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

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

    }
}
