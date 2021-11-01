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
    public class Course7211Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform spinePage;
        private GameObject spineShow;
        private GameObject btnBack;

        bool isPlaying = false;
        bool isPressed = false;
        int flag = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            spinePage = curTrans.Find("spinePage");
            spineShow = curTrans.Find("spineShow").gameObject;
            btnBack = curTrans.Find("btnBack").gameObject;

            for (int i = 0; i < spinePage.childCount; i++)
            {
                Util.AddBtnClick(spinePage.GetChild(i).GetChild(0).gameObject, OnClickShow);
            }

            Util.AddBtnClick(btnBack, OnClickBtnBack);

            btnBack.SetActive(false);
            flag = 0;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressed)
                return;
            isPressed = true;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(spineShow,  "kong", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name + "3", false, () =>
                {
                    for (int i = 0; i < spinePage.childCount; i++)
                    {
                        if (i != tem.transform.GetSiblingIndex())
                        {
                            SpineManager.instance.DoAnimation(spinePage.GetChild(i).gameObject, spinePage.GetChild(i).name + 1, false);
                        }
                    }

                    obj.SetActive(false); isPlaying = false; isPressed = false;
                    if (flag == (Mathf.Pow(2, spinePage.childCount) - 1))
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });

        }

        private void OnClickShow(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            for (int i = 0; i < spinePage.childCount; i++)
            {
                if (i != tem.transform.GetSiblingIndex())
                {
                    SpineManager.instance.DoAnimation(spinePage.GetChild(i).gameObject, "kong", false);
                }
            }
        
            SpineManager.instance.DoAnimation(tem, obj.name, false,()=> {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[tem.transform.GetSiblingIndex() + 1];
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, tem.transform.GetSiblingIndex() + 1,
                  () =>
                  {
                      if ((flag & (1 << tem.transform.GetSiblingIndex())) < 1)
                      {
                          flag += 1 << tem.transform.GetSiblingIndex();
                      }
                    
                      if (tem.name != "b")
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, tem.transform.GetSiblingIndex(), true);
                          SpineManager.instance.DoAnimation(spineShow, tem.name + 2,true);
                      }
                      else
                      {
                          SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, tem.transform.GetSiblingIndex(),()=> { SoundManager.instance.skipBtn.SetActive(false); },()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true); });
                          SpineManager.instance.DoAnimation(spineShow, tem.name + 2, false,()=> { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); });
                      }
                   
                  },
                  () =>
                  {
                      btnBack.SetActive(true);
                  }));

            });      
        }

     
        void GameStart()
        {

            SpineManager.instance.DoAnimation(spinePage.GetChild(0).gameObject, "kong", false,()=> { SpineManager.instance.DoAnimation(spinePage.GetChild(0).gameObject, spinePage.GetChild(0).name + "1", false); });
            SpineManager.instance.DoAnimation(spinePage.GetChild(1).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(spinePage.GetChild(1).gameObject, spinePage.GetChild(1).name + "1", false); });
            SpineManager.instance.DoAnimation(spinePage.GetChild(2).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(spinePage.GetChild(2).gameObject, spinePage.GetChild(2).name + "1", false); });

            SpineManager.instance.DoAnimation(spineShow, "kong", false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            isPlaying = true;
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); isPlaying = false; }));
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
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[4];
                for (int i = 0; i < spinePage.childCount; i++)
                {
                    SpineManager.instance.DoAnimation(spinePage.GetChild(i).gameObject, "kong", false);
                }

                SpineManager.instance.DoAnimation(spineShow, "kong", false);
                isPlaying = true;
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
