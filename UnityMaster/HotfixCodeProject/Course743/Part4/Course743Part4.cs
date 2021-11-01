using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course743Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private RawImage _dirtyImage;
        private GameObject _mainTarget;
        
        private GameObject Max;

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
            _dirtyImage = curTrans.GetTargetComponent<RawImage>("Dirty");
            _mainTarget = curTrans.GetGameObject("Main");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            _dirtyImage.color=Color.white;
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
                isPlaying = false;
            }));

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
            switch (talkIndex)
            {
                case 1:
                    SpineManager.instance.DoAnimation(_mainTarget, "animation2",false);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null,
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;
                case 2:
                    _dirtyImage.color = Color.white;
                    float timer= SpineManager.instance.DoAnimation(_mainTarget, "animation3",true);
                    DelayTimeAndExcuteNext(1.5f * timer / 3f, () => _dirtyImage.DOColor(Color.clear, 3f));
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null,
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;
                case 3:
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0));
                    break;
                case 4:
                    break;
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

        void DelayTimeAndExcuteNext(float timer,Action callback)
        {
            mono.StartCoroutine(DelayIE(timer, callback));
        }

        IEnumerator DelayIE(float timer, Action callback)
        {
            yield return new WaitForSeconds(timer);
            
            callback?.Invoke();
        }
    }
}
