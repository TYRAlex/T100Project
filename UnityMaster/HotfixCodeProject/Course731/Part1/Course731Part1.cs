using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course731Part1
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
        string animation;

        bool isClickA = false;
        bool isClickB = false;

        GameObject trafficLights;
        GameObject btnBack;
        GameObject unClickMask;
        GameObject trafficA;
        GameObject trafficB;
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
            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);

            unClickMask = curTrans.Find("UnClickMask").gameObject;
            unClickMask.SetActive(false);

            trafficLights = curTrans.Find("trafficLights").gameObject;
            trafficLights.SetActive(true);

            trafficA = curTrans.Find("trafficA").gameObject;
            trafficA.SetActive(false);

            trafficB = curTrans.Find("trafficB").gameObject;
            trafficB.SetActive(false);

            Util.AddBtnClick(trafficLights.transform.GetChild(0).gameObject, TrafficLightsClick);
            Util.AddBtnClick(trafficLights.transform.GetChild(1).gameObject, TrafficLightsClick);

            Util.AddBtnClick(btnBack, BtnBackClick);
        }

        void GameInit()
        {
            talkIndex = 1;

            isClickA = false;
            isClickB = false;

            trafficLights.GetComponent<SkeletonGraphic>().Initialize(true);
            trafficA.GetComponent<SkeletonGraphic>().Initialize(true);
            trafficB.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(trafficLights, "animation", false);
        }

        void GameStart()
        {
            unClickMask.SetActive(true);
            Max.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 9, null, () =>
            {
                unClickMask.SetActive(false);
            }));
        }

        #region 点击相关
        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);

            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6));

                unClickMask.SetActive(true);
            }

            talkIndex++;
        }

        //点击红绿灯
        void TrafficLightsClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(9);
            unClickMask.SetActive(true);
            SoundManager.instance.ShowVoiceBtn(false);

            switch (obj.name)
            {
                //点击行人红绿灯
                case "a":
                    mono.StartCoroutine(SpeakAndAnimation(trafficLights, obj.name + "1", obj.name + "2", index: 0, method: () =>
                    {
                        trafficLights.SetActive(false);
                        trafficA.SetActive(true);
                        trafficA.GetComponent<SkeletonGraphic>().Initialize(true);

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                        mono.StartCoroutine(SpeakAndAnimation(trafficA, b: "1", index: 10, method: () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7, null, () =>
                             {
                                 trafficLights.SetActive(true);
                                 trafficA.SetActive(false);

                                 isClickA = true;
                                 animation = "a";

                                 unClickMask.SetActive(false);
                                 btnBack.SetActive(true);

                                 trafficA.GetComponent<SkeletonGraphic>().Initialize(true);
                             }));

                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

                            SpineManager.instance.DoAnimation(trafficA, "2", false, () =>
                            {
                                mono.StartCoroutine(WaitFor(4.5f, () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);

                                    SpineManager.instance.DoAnimation(trafficA, "3", false);
                                }));
                            });
                        }));
                    }));

                    break;

                case "b":
                    mono.StartCoroutine(SpeakAndAnimation(trafficLights, obj.name + "1", obj.name + "2", index: 5, method: () =>
                    {
                        trafficLights.SetActive(false);
                        trafficB.SetActive(true);

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                        mono.StartCoroutine(SpeakAndAnimation(trafficB, b: "1", index: 11, method: () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                            trafficB.GetComponent<SkeletonGraphic>().Initialize(true);

                            mono.StartCoroutine(SpeakAndAnimation(trafficB, b: "2", index: 12, method: () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                trafficB.GetComponent<SkeletonGraphic>().Initialize(true);

                                mono.StartCoroutine(SpeakAndAnimation(trafficB, b: "3", index: 8, method: () =>
                                {
                                    trafficB.SetActive(false);
                                    trafficLights.SetActive(true);

                                    isClickB = true;
                                    animation = "b";

                                    unClickMask.SetActive(false);
                                    btnBack.SetActive(true);

                                    trafficB.GetComponent<SkeletonGraphic>().Initialize(true);
                                }));
                            }));
                        }));

                    }));

                    break;
            }
        }

        //缩小
        void BtnBackClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(9);
            unClickMask.SetActive(true);

            SpineManager.instance.DoAnimation(trafficLights, animation + "3", false, () =>
            {
                unClickMask.SetActive(false);
                btnBack.SetActive(false);

                if (isClickA && isClickB) SoundManager.instance.ShowVoiceBtn(true);
            });
        }
        #endregion

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
