using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course729Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject max;
        private Transform cards;

        private Transform btns;
        private GameObject btnBack;

        private int isFirstPress = 0;
        private int isScaling = -1;

        private bool isPlaying = false;

        private int CardNumMax = 3;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            max = curTrans.Find("max").gameObject;
            max.SetActive(true);
            cards = curTrans.Find("cards");
            btns = curTrans.Find("btns");
            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            CardNumMax = btns.childCount;
            for (int i = 0; i < CardNumMax; i++)
            {
                cards.GetChild(i).gameObject.SetActive(true);
                Util.AddBtnClick(btns.GetChild(i).gameObject, onClickBtnCard);
            }

            Util.AddBtnClick(btnBack, onClickBtnBack);

            isFirstPress = 0;
            isScaling = -1;

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private void onClickBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;         
            if (isScaling < 0)          
                return;           
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            btnBack.SetActive(false);
            GameObject temCard = cards.GetChild(isScaling).gameObject;
            SpineManager.instance.DoAnimation(temCard, temCard.name + "4", false, () =>
            {
                for (int i = 0; i < CardNumMax; i++)
                {
                    cards.GetChild(i).gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(temCard, temCard.name, false);
                }
                isPlaying = false;
            });
        }

        private void onClickBtnCard(GameObject obj)
        {

            if (isPlaying)
                return;
            isPlaying = true;

            int index = int.Parse(obj.name);
            isScaling = index;

            bool flag = false;
            flag = (isFirstPress & (1 << index)) > 0;
            if (!flag)
            {
                isFirstPress += (1 << index);
            }

            string spineName = "";
            if (flag)
            {
                spineName = "1";
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, obj.transform.GetSiblingIndex());
                for (int i = 0; i < CardNumMax; i++)
                {
                    if (i != index)
                    {
                        cards.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                spineName = "5";
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            }

            GameObject temCard = cards.GetChild(index).gameObject;
            SpineManager.instance.DoAnimation(temCard, temCard.name + spineName, false, () =>
            {
                if (flag)
                {
                    mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex()+1, () => { SpineManager.instance.DoAnimation(temCard, temCard.name + "3", false); }, () => { isPlaying = false; btnBack.SetActive(true); }));                   
                }
                else
                {
                    isPlaying = false;
                }
            });
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            for (int i = 0; i < CardNumMax; i++)
            {
                SpineManager.instance.DoAnimation(cards.GetChild(i).gameObject, cards.GetChild(i).name + "6", false);
            }
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(max,SoundManager.SoundType.VOICE, 0, null, () => { max.SetActive(false); isPlaying = false; }));
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
