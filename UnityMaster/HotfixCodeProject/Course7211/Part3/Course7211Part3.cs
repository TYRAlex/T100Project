using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course7211Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bs;

        private GameObject tlj;
        private GameObject zi;
        private GameObject tljUBtn;
        private GameObject tljQBtn;
        bool isPlaying = false;
        bool isEnd = false;

        private GameObject mask;
        void Start(object o)
        {

            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            Bg = curTrans.Find("Bg").gameObject;
            bs = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);


            tlj = curTrans.Find("tlj").gameObject;
            tlj.SetActive(true);
            zi = curTrans.Find("zi").gameObject;
            zi.SetActive(true);
            tljUBtn = curTrans.Find("yd").gameObject;
            tljUBtn.SetActive(false);
            tljQBtn = curTrans.Find("qd").gameObject;
            tljQBtn.SetActive(false);

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);
            Util.AddBtnClick(tljUBtn, onClickBtnU);
            Util.AddBtnClick(tljQBtn, onClickBtnQ);
          
            talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        
            GameStart();
        }

        private void onClickBtnU(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            mask.SetActive(true);
            SpineManager.instance.DoAnimation(obj, obj.name + "2", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0, true);
                    SpineManager.instance.DoAnimation(tlj, "1", false, () => { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); /*isPlaying = false;*/    mask.SetActive(false); });

                }, () => {
                        isPlaying = false;
                    if (!isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            });
        }

        private void onClickBtnQ(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            BtnPlaySound();
            mask.SetActive(true);
            SpineManager.instance.DoAnimation(obj, obj.name + "2", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                    SpineManager.instance.DoAnimation(tlj, "2", false, () => { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); mask.SetActive(false); /*isPlaying = false; */});

                }, () =>
                {
                    isPlaying = false;
                    if (!isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    }));
            });


        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);        
            isPlaying = true;
            isEnd = false;
            mask.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, () => { mono.StartCoroutine(WaitExe(() => { SpineManager.instance.DoAnimation(tlj, "1", false, () => { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); }); }, () => { mono.StartCoroutine(playSpineShow(zi)); }, 2)); }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
            if (talkIndex == 1)
            {
            isPlaying = true;
                mask.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    tljUBtn.SetActive(true);
                    tljQBtn.SetActive(true);
                    SpineManager.instance.DoAnimation(tljUBtn, tljUBtn.name, false);
                    SpineManager.instance.DoAnimation(tljQBtn, tljQBtn.name, false);
                    mask.SetActive(false);
                    isPlaying = false;
                }));

            }
            if (talkIndex == 2)
            {              
                isPlaying = true;
                mask.SetActive(true);
                isEnd = true;
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4,null,()=> { mask.SetActive(false); isPlaying = false; }));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


        IEnumerator WaitExe(Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(len);

            if (method_2 != null)
            {
                method_2();
            }
        }
        private IEnumerator playSpineShow(GameObject playGo)
        {
            float playDur = 0;

            for (int i = 1; i < 5; i++)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                playDur = SpineManager.instance.DoAnimation(playGo, playGo.name + i, false);
                yield return new WaitForSeconds(playDur);
            }
            mask.SetActive(false);
            isPlaying = false;
        }
    }
}
