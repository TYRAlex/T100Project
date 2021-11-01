using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course8412Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject shadow;
        private GameObject bell;
        private GameObject robotSpine;

        private Transform spinePanel;
        private Transform btns;

        private GameObject btnBack;

        bool isPlaying = false;
        bool isPress = false;
        int flag = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            shadow = curTrans.Find("shadow").gameObject;
            bell = curTrans.Find("shadow/bell").gameObject;
            shadow.SetActive(false);
            robotSpine = curTrans.Find("robotSpine").gameObject;
            spinePanel = curTrans.Find("spinePanel");
            btns = curTrans.Find("btns");

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);
            for (int i = 0; i < btns.childCount; i++)
            {
                Util.AddBtnClick(btns.GetChild(i).gameObject, OnClickBtn);
                for (int j = 0; j < btns.GetChild(i).childCount; j++)
                {
                    Util.AddBtnClick(btns.GetChild(i).GetChild(j).gameObject, OnClickBtn);
                }
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

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPress)
                return;
            isPress = true;
            SpineManager.instance.DoAnimation(spinePanel.GetChild(tem.GetSiblingIndex()).gameObject, tem.name + 3, false, () =>
            {
                obj.SetActive(false);
                isPress = false;
                isPlaying = false;
                if (flag >= (Mathf.Pow(2, spinePanel.childCount) - 1))
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }

            });
        }
        Transform tem;
        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            if (obj.transform.parent.name != "btns")
            {
                tem = obj.transform.parent;
            }
            else
            {
                tem = obj.transform;
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, tem.GetSiblingIndex());
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, tem.GetSiblingIndex() + 1, () =>
            {
                SpineManager.instance.DoAnimation(spinePanel.GetChild(tem.GetSiblingIndex()).gameObject, tem.name + 1, false,
                () => { SpineManager.instance.DoAnimation(spinePanel.GetChild(tem.GetSiblingIndex()).gameObject, tem.name + 2, false); });
            }, () =>
            {
                btnBack.SetActive(true);

                if ((flag & (1 << tem.GetSiblingIndex())) < 1)
                {
                    flag += (1 << tem.GetSiblingIndex());
                }

            }));
        }

        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            isPress = false;
            SpineManager.instance.DoAnimation(robotSpine, "kong", false, () => { SpineManager.instance.DoAnimation(robotSpine, "animation2", true); });
            for (int i = 0; i < spinePanel.childCount; i++)
            {
                SpineManager.instance.DoAnimation(spinePanel.GetChild(i).gameObject, "kong", false);
            }
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 7, true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
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
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                shadow.SetActive(true);
                SpineManager.instance.DoAnimation(robotSpine, "kong", false);
                for (int i = 0; i < spinePanel.childCount; i++)
                {
                    SpineManager.instance.DoAnimation(spinePanel.GetChild(i).gameObject, "kong", false);
                }
                isPlaying = true;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 6));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
