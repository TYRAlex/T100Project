using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace ILFramework.HotClass
{
    public class Course738Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        private Transform spine;

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

            Max = curTrans.Find("bell").gameObject;
            spine = curTrans.Find("1");
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }





        public int[] Intersect(int[] nums1, int[] nums2)
        {
            List<int> result = new List<int>();
            for (int i=0;i<nums1.Length;i++) 
            {
                for (int j=0;i<nums2.Length;j++) 
                {
                    if (nums1[i] == nums2[j]) 
                    {
                        if (nums2.Length > nums1.Length)
                        {
                            result.Add(nums2[i]);
                        }
                        else 
                        {
                            result.Add(nums1[i]);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private void GameInit()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            talkIndex = 1; SoundManager.instance.ShowVoiceBtn(true);
            SpineManager.instance.DoAnimation(spine.gameObject,"1",false);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
        
           // mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; }));

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
           
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                Delay(2f, () =>
                 {
                     //mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                     SpineManager.instance.DoAnimation(spine.gameObject, "2", false);
                 });

            }
            else if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                SpineManager.instance.DoAnimation(spine.gameObject, "3", false);
                Delay(2f,()=> 
                {
                    SpineManager.instance.DoAnimation(spine.gameObject, "4", false);
                });
            }
            else if (talkIndex==3)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () => {}));
                Delay(0.5f,()=> 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                });
          
                SpineManager.instance.DoAnimation(spine.gameObject, "5", false,()=> 
                {
                    SpineManager.instance.DoAnimation(spine.gameObject, "6", false);
                });
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }
        private void Delay(float time, Action action = null)
        {
            mono.StartCoroutine(delay(time, action));
        }
        IEnumerator delay(float time, Action action = null)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
