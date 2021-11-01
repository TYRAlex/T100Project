using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course723Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject bell;

        private GameObject _spineAni;
        private GameObject _block;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _spineAni = curTrans.GetGameObject("SpineAni");
            _block = curTrans.GetGameObject("Block");
            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            SpineManager.instance.DoAnimation(_spineAni, "1", false);
            _block.Hide();
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true);}));
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

        IEnumerator WaitAniCoroutine(float len, Action method_1 = null)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(_spineAni, "2", false);
                }, 
                () => 
                {
                    bell.Hide();
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2,
                () =>
                {
                    _block.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_spineAni, "3", false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_spineAni, "4", false);
                        mono.StartCoroutine(WaitAniCoroutine(8.0f, 
                        () => 
                        {
                            SpineManager.instance.DoAnimation(_spineAni, "5", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            mono.StartCoroutine(WaitAniCoroutine(1.0f,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_spineAni, "6", false);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                mono.StartCoroutine(WaitAniCoroutine(1.0f,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_spineAni, "7", false);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                }));
                            }));
                        }));
                        
                    });
                },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    mono.StopAllCoroutines();
                }));
            }
            if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3,
                () =>
                {
                    bell.Show();
                    _block.Hide();
                    SpineManager.instance.DoAnimation(_spineAni, "1", false);
                },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 4)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4,
                () =>
                {
                    SpineManager.instance.DoAnimation(_spineAni, "8", false);
                }, null));
            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
