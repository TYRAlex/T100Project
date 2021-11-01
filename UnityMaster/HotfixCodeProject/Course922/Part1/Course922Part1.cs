using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course922Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject click1;
        private bool _canClick;
        private bool[] _jugleClick1;
        private GameObject mask;
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

            mask = curTrans.Find("mask").gameObject;
            click1 = curTrans.Find("click1").gameObject;
            _jugleClick1 = new bool[2];

            for (int i = 0; i < 2; i++)
            {
                Util.AddBtnClick(click1.transform.GetChild(i).gameObject, ClickEvent1);
            }

            GameInit();
            GameStart();
        }

        private void ClickEvent1(GameObject obj)
        {
            if (_canClick)
            {
                //obj.transform.GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Max.SetActive(false);
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                if (obj.name == "2")
                {
                    mono.StartCoroutine(WaitTime(5.2f,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        }
                        ));
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "animation" + obj.name, false,
                        () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2));
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                            mask.SetActive(false);
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "zxc2", false,
                            () =>
                            {
                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                obj.transform.GetChild(0).gameObject.SetActive(false);
                                mask.SetActive(true);
                                _jugleClick1[0] = true;
                                JugleClick1();
                                Max.SetActive(true);
                                _canClick = true;
                                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "animation", false);
                            }
                            );
                        }
                        );

                }
                if (obj.name == "3")
                {
                    mono.StartCoroutine(WaitTime(9f,
                        () =>
                        { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false); }
                        ));
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "animation" + obj.name, false,
                        () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3));
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                            mask.SetActive(false);
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "mr2", false,
                            () =>
                            {
                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                obj.transform.GetChild(0).gameObject.SetActive(false);
                                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "animation", false);
                                mask.SetActive(true);
                                _jugleClick1[1] = true;
                                JugleClick1();
                                _canClick = true;
                                Max.SetActive(true);
                            }
                            );
                        }
                        );
                }
            }
        }

        private void JugleClick1()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_jugleClick1[i])
                    return;
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }

        IEnumerator WaitTime(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        private void GameInit()
        {
            curTrans.Find("heiban").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            click1.transform.GetChild(0).GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            click1.transform.GetChild(1).GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            click1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            curTrans.Find("heiban").gameObject.SetActive(false);
            mask.SetActive(false);
            click1.SetActive(false);
            talkIndex = 1;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            curTrans.Find("mr").gameObject.SetActive(true);
            curTrans.Find("mr").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(curTrans.Find("mr").gameObject, "mr1", true);

            //SpineManager.instance.DoAnimation(click1, "kong", false,
            //    () =>
            //    {
            //        SpineManager.instance.DoAnimation(click1, "animation", false);
            //    }
            //    );
            //click1.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            //SpineManager.instance.DoAnimation(click1.transform.GetChild(0).GetChild(0).gameObject, "kong", false,
            //    () => { click1.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); }
            //    );
            //click1.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            //SpineManager.instance.DoAnimation(click1.transform.GetChild(1).GetChild(0).gameObject, "kong", false,
            //    () => { click1.transform.GetChild(1).GetChild(0).gameObject.SetActive(false); }
            //    );
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));

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
            if (talkIndex == 1)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                curTrans.Find("mr").gameObject.SetActive(false);
                curTrans.Find("mask").gameObject.SetActive(true);
                click1.SetActive(true);
                SpineManager.instance.DoAnimation(click1, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    { _canClick = true; }
                    ));
            }
            if (talkIndex == 2)
            {
                mask.SetActive(false);
                click1.SetActive(false);
                curTrans.Find("heiban").gameObject.SetActive(true);
                
                SpineManager.instance.DoAnimation(curTrans.Find("heiban").gameObject, "animation", false);
                mono.StartCoroutine(WaitTime(6f,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        SpineManager.instance.DoAnimation(curTrans.Find("heiban").gameObject, "animation2", false);
                    }
                    ));
                mono.StartCoroutine(WaitTime(6.5f,
                    () =>
                    {
                        
                        SpineManager.instance.DoAnimation(curTrans.Find("heiban").gameObject, "animation3", false);
                    }
                    ));
                mono.StartCoroutine(WaitTime(6.8f,
                    ()=>
                    { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); }
                    ));
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4));

            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
