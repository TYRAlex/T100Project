using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course9211Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private Transform OnClick1;
        private Transform OnClick2;
        private Transform OnClick3;
        private Transform spine;
        private List<string> name1;
        private GameObject Max;
        private int index=0;
        private bool isfinish = false;
        bool isPlaying = false;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            name1 = new List<string>();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            OnClick1 = curTrans.Find("OnClick1");
            OnClick2 = curTrans.Find("OnClick2");
            OnClick3 = curTrans.Find("OnClick3");
            spine = curTrans.Find("spine");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            Util.AddBtnClick(OnClick1.gameObject, OnClick1Event);
            Util.AddBtnClick(OnClick2.gameObject, OnClick2Event);
            Util.AddBtnClick(OnClick3.gameObject, OnClick3Event);
            SpineManager.instance.DoAnimation(spine.GetChild(0).gameObject,"j",true);
            SpineManager.instance.DoAnimation(spine.GetChild(1).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(spine.GetChild(2).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(spine.GetChild(3).gameObject, "kong", true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = false;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = true; }));

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
                isPlaying = false;
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,4,()=> 
                {
                    SpineManager.instance.DoAnimation(spine.GetChild(0).gameObject,"j2",false);
                },()=> 
                {
                    isPlaying = true;
                    isfinish = true;
                    Max.SetActive(false);
                }));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private void OnClick1Event(GameObject obj) 
        {
            if (isPlaying)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                if (name1.Contains(obj.name) == false)
                {
                    name1.Add(obj.name);
                    index++;
                }
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,()=>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2,true);
                    SpineManager.instance.DoAnimation(spine.GetChild(1).gameObject, "1", true);
                    isPlaying = false;
                },()=> 
                {
                    
                    SpineManager.instance.DoAnimation(spine.GetChild(1).gameObject, "kong", true);
                    StopAudio(SoundManager.SoundType.SOUND);
                    isPlaying = true;
                    if (index == 3)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    if (isfinish) 
                    {
                        SoundManager.instance.ShowVoiceBtn(false);
                    }
                }));
              
            }
          
        }
        private void OnClick2Event(GameObject obj)
        {
            if (isPlaying)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                if (name1.Contains(obj.name) == false)
                {
                    name1.Add(obj.name);
                    index++;
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, () =>
                {
                    SpineManager.instance.DoAnimation(spine.GetChild(2).gameObject, "2", true);
                    isPlaying = false;
                }, () =>
                {
                    SpineManager.instance.DoAnimation(spine.GetChild(2).gameObject, "kong", true);
                    isPlaying = true;
                    StopAudio(SoundManager.SoundType.SOUND);
                    if (index == 3)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    if (isfinish)
                    {
                        SoundManager.instance.ShowVoiceBtn(false);
                    }
                }));
                
            }
              
        }
        private void OnClick3Event(GameObject obj)
        {
            if (isPlaying)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                if (name1.Contains(obj.name) == false)
                {
                    name1.Add(obj.name);
                    index++;
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, () =>
                {
                    SpineManager.instance.DoAnimation(spine.GetChild(3).gameObject, "3", true);
                    isPlaying = false;
                }, () =>
                {
                    
                    SpineManager.instance.DoAnimation(spine.GetChild(3).gameObject, "kong", true);
                    StopAudio(SoundManager.SoundType.SOUND);
                    // SoundManager.instance.ShowVoiceBtn(true);
                    isPlaying = true;
                    if (index == 3)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    if (isfinish)
                    {
                        SoundManager.instance.ShowVoiceBtn(false);
                    }
                }));
               
            }
                
           
           
        }
        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }
    }
}
