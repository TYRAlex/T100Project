using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course738Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        private Transform car1;
        private Transform car2;
        private Transform _mask;
        private Transform ban;
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
            _mask = curTrans.Find("mask");
            car1 = curTrans.Find("car1"); car2 = curTrans.Find("car2");
            ban = curTrans.Find("mask/1");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            car1.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            car2.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            //SpineManager.instance.DoAnimation(car1.gameObject,"kong",false); SpineManager.instance.DoAnimation(car2.gameObject, "kong", false); SpineManager.instance.DoAnimation(ban.gameObject, "kong", false);
            //car1.gameObject.SetActive(false); car2.gameObject.SetActive(false); 
            _mask.gameObject.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(true);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
           // mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { Max.SetActive(false); isPlaying = false; }));

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
                //车子倒车撞车
                car1.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(car1.gameObject, "c1", false);
            }
            else if (talkIndex == 2)
            {
                //车子倒车但有提示
                car1.gameObject.SetActive(false);
                car2.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(car2.gameObject, "c3", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND,1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                Delay(5.5f,()=> 
                {
                    SpineManager.instance.DoAnimation(car2.gameObject, "c2", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1);
                });
              
            }
            else if (talkIndex == 3) 
            {
                //拓展运用UI
                Max.SetActive(false);
                _mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(ban.gameObject,"ban",false);
                Delay(2f,()=> 
                {
                    SpineManager.instance.DoAnimation(ban.gameObject, "ban2", false);
                });
                Delay(4f, () =>
                {
                    SpineManager.instance.DoAnimation(ban.gameObject, "ban3", false);
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
        private void Delay(float time, Action action = null) 
        {
            mono.StartCoroutine(delay(time,action));
        }
        IEnumerator delay(float time,Action action=null)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
    }
}
