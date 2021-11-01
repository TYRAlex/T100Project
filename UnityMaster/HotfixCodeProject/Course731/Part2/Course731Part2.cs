using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course731Part2
    {
        #region 通用变量
        int talkIndex;

        MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject Bg;
        BellSprites bellTextures;

        GameObject Max;
        #endregion

        #region 游戏变量
        GameObject dialogBox;
        GameObject program;
        GameObject juggle;
        GameObject frame;
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
            dialogBox = curTrans.Find("dialogBox").gameObject;
            dialogBox.SetActive(true);

            program = curTrans.Find("Program").gameObject;
            program.SetActive(false);

            juggle = curTrans.Find("juggle").gameObject;
            juggle.SetActive(false);

            frame = curTrans.Find("frame").gameObject;
            frame.SetActive(false);
        }

        void GameInit()
        {
            talkIndex = 1;

            dialogBox.GetComponent<SkeletonGraphic>().Initialize(true);
            program.GetComponent<SkeletonGraphic>().Initialize(true);
            juggle.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(dialogBox, "1", false);
        }

        void GameStart()
        {
            Max.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);

            SoundManager.instance.ShowVoiceBtn(false);

            switch (talkIndex)
            {
                //流程一
                case 1:
                    program.SetActive(true);

                    SpineManager.instance.DoAnimation(program, "zi", false);

                    mono.StartCoroutine(SpeakAndAnimation(dialogBox, b: "2", index: 2, method: () =>
                    {
                        SpineManager.instance.DoAnimation(program, "zi2", false);

                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                        {
                            mono.StartCoroutine(SpeakAndAnimation(dialogBox, b: "3", index: 5, method: () =>
                            {
                                SpineManager.instance.DoAnimation(program, "zi3", false);

                                mono.StartCoroutine(SpeakAndAnimation(dialogBox, null, "4", null, 6, () =>
                                {
                                    SpineManager.instance.DoAnimation(program, "zi4", false);

                                    mono.StartCoroutine(SpeakAndAnimation(dialogBox, null, "6", null, 3, () =>
                                    {
                                        SpineManager.instance.DoAnimation(program, "zi5", false);

                                        mono.StartCoroutine(SpeakAndAnimation(dialogBox, b: "2", index: 1, method: () =>
                                        {
                                            SoundManager.instance.ShowVoiceBtn(true);
                                        }));
                                    })); ;
                                }));
                            }));
                        }));


                    }));

                    break;

                //流程二
                case 2:
                    dialogBox.SetActive(false);
                    program.SetActive(false);
                    juggle.SetActive(true);

                    mono.StartCoroutine(SpeakAndAnimation(juggle, b: "1", index: 7, method: () =>
                    {
                        mono.StartCoroutine(WaitFor(1f, () =>
                        {
                            mono.StartCoroutine(WaitAnimation(juggle, 3, 4, () =>
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            }));
                        }));
                    }));

                    break;

                case 3:
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 12));
                    break;
            }

            talkIndex++;
        }

        //协程:间隔播放动画
        IEnumerator WaitAnimation(GameObject obj, int num, int total, Action method = null)
        {
            SpineManager.instance.DoAnimation(Max, "DAIJIshuohua");

            for (int i = 0; i < total; ++i)
            {
                if (i == total - 2) frame.SetActive(true);
                if (i == total - 1) frame.SetActive(false);

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, num + 5));

                float _time = SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, num + 5);

                SpineManager.instance.DoAnimation(obj, "" + num, false);

                ++num;

                yield return new WaitForSeconds(_time + 1f);
            }

            SpineManager.instance.DoAnimation(Max, "DAIJI");

            method?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
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
                if (Max.activeSelf) mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index));

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

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
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
    }
}
