using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course7211Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bs;
        private GameObject sdSpine;
        private Transform btns;
        bool isPlaying = false;
        void Start(object o)
        {

            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            Bg = curTrans.Find("Bg").gameObject;
            bs = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);

            sdSpine = curTrans.Find("cmjImg/sdSpine").gameObject;
            btns = curTrans.Find("btns");
            btns.gameObject.SetActive(true);         

            for (int i = 0; i < btns.childCount; i++)
            {
                Util.AddBtnClick(btns.GetChild(i).gameObject, onClickBtn);

                if (btns.GetChild(i).childCount > 1)
                {
                    for (int j = 0; j < btns.GetChild(i).childCount; j++)
                    {
                        Util.AddBtnClick(btns.GetChild(i).GetChild(j).gameObject, onClickBtn);
                    }
                }
            }

            talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);    
            GameStart();
        }
   
        private void onClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            if (obj.name == "3" || obj.name == "5")
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            }
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, int.Parse(obj.name), () =>
             {
                 SpineManager.instance.DoAnimation(sdSpine, obj.name, false);
             }, () => { isPlaying = false;}));
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2,true);
            SpineManager.instance.DoAnimation(sdSpine, "jing", false);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, () =>
             {

             }, () => { bell.SetActive(false); isPlaying = false; }));
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
            isPlaying = true;
            if (talkIndex == 1)
            {
                                       
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
    }
}
