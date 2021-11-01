using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course9311Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject max;
        private GameObject _ani;
        private GameObject _wenzi;
        private GameObject _blue;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            max = curTrans.Find("Max").gameObject;
            _ani = curTrans.GetGameObject("ani");
            _wenzi = curTrans.GetGameObject("wenzi");
            _blue = curTrans.GetGameObject("blue");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _wenzi.Show();
            _blue.Hide();

            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_ani, "001", false);
        }

        void GameStart()
        {
            max.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                speaker = max;
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
                mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 1, 
                ()=> 
                {
                    _wenzi.Hide();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_ani, "002", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_ani, "003", false);
                    });
                },
                () => 
                {
                    mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 2,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_ani, "004", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani, "005", false);
                        });
                    },
                    () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 3,
                        () =>
                        {
                            _blue.Show();
                            SpineManager.instance.DoAnimation(_ani, "006", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_ani, "007", false);
                            });
                        },
                        () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 4,
                            () =>
                            {
                                _blue.Hide();
                                SpineManager.instance.DoAnimation(_ani, "008", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_ani, "009", false);
                                });
                            },
                            () =>
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            }));
                        }));
                    }));
                }));
            }
            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 5, null, null));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

    }
}
