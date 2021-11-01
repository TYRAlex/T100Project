using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course8412Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject bell;
        private Transform robots;
        private Transform btns;

        bool isPlaying = false;


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
            bell.SetActive(false);
            robots = curTrans.Find("robots");
            btns = curTrans.Find("btns");

            for (int i = 0; i < btns.childCount; i++)
            {
                Util.AddBtnClick(btns.GetChild(i).gameObject, OnClickBtn);
            }
            Button[] buttons = btns.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].transition = Selectable.Transition.None;
            }
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            bool isRight = obj.transform.GetSiblingIndex() == 0;
            if (isRight)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1);
            }
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, isRight ? 2 : 1, () => { SpineManager.instance.DoAnimation(robots.GetChild(obj.transform.GetSiblingIndex()).gameObject, obj.name + 2, false, () => { SpineManager.instance.DoAnimation(robots.GetChild(obj.transform.GetSiblingIndex()).gameObject, obj.name + 1, true); }); }, () => { isPlaying = false; }));

        }

        private void GameInit()
        {
            talkIndex = 1;

            for (int i = 0; i < robots.childCount; i++)
            {
                SpineManager.instance.DoAnimation(robots.GetChild(i).gameObject, robots.GetChild(i).name, true);
            }
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM,7,true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { isPlaying = false; }));

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

            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
