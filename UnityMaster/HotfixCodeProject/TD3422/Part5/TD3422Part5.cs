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
    public class TD3422Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject caidaiSpine;
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

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(true);
            
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(true);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }


        private void GameInit()
        {
            talkIndex = 1;
            
        }

        void GameStart()
        {
            switchBGM();
            mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 0, 
            () => 
            {
                SpineManager.instance.DoAnimation(caidaiSpine, "kong", false, 
                () => 
                {
                    SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                });
            }, null));
        }
        /// <summary>
        /// bell????????????
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
                speaker = dbd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                
            }
            talkIndex++;
        }

        //??????????????????bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //??????????????????
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //??????????????????
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }

    }
}
