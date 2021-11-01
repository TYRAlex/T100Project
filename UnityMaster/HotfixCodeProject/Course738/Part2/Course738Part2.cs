using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course738Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        private Transform spine;
        private Transform mask;
        private Transform car;
        private Transform zi; private Transform zi2;

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
            spine = curTrans.Find("mask/spine");
            mask = curTrans.Find("mask");
            car = curTrans.Find("car");
            zi = curTrans.Find("mask/zi");
            zi2 = curTrans.Find("mask/zi2");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            zi.gameObject.SetActive(false);
            zi2.gameObject.SetActive(false);
            mask.gameObject.SetActive(true);
            spine.gameObject.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(true);
            SpineManager.instance.DoAnimation(Max.gameObject,"DAIJI",true);
            car.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            spine.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(car.gameObject, "a3", false);
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
        
            if (talkIndex == 1)
            {
                //出现UI
                spine.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                SpineManager.instance.DoAnimation(spine.gameObject, "c2", false);
                Delay(2f, () =>
                {
                    SpineManager.instance.DoAnimation(spine.gameObject, "c1", false);
                });
                Delay(5f, () =>
                {
                    SpineManager.instance.DoAnimation(spine.gameObject, "c3", false);
                });
            }
            else if (talkIndex == 2)
            {
                //车子倒车
                mask.gameObject.SetActive(false);

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                SpineManager.instance.DoAnimation(car.gameObject, "a3", false);
                //倒车提示音
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(car.gameObject, "daoche", false);
            }
            else if (talkIndex == 3)
            {
                //出现橙色条件判断UI块
                mask.gameObject.SetActive(true);
                mask.Find("spine").gameObject.SetActive(false);
                zi.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

            }
            else if (talkIndex == 4)
            {
                //放大并移动积木
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 3, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                zi.GetChild(1).gameObject.SetActive(true); zi.GetChild(2).gameObject.SetActive(true);
                zi.GetRectTransform().DOScale(2, 1.5f);
                zi.GetRectTransform().DOAnchorPosY(-3, 1.5f);
            }
            else if (talkIndex == 5)
            {
                //车子再次倒车
                mask.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 4, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                car.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(car.gameObject, "a3", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(car.gameObject, "daoche", false);
                zi.gameObject.SetActive(false);
                Delay(12f,()=> 
                {
                    mask.gameObject.SetActive(true);
                    zi2.gameObject.SetActive(true);
                });
         
             
            }
            else if (talkIndex==6)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 5, null, () => {  }));
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
        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }


        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
    }
}
