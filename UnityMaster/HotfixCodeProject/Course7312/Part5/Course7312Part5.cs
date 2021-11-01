using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course7312Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _spine0;
        private GameObject _spine1;

        private GameObject _bg0;
        private GameObject _bg1;

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
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, true);
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;

            _spine0 = curTrans.Find("spineManager/0").gameObject;
            _spine0.Show();

            _spine1 = curTrans.Find("spineManager/1").gameObject;
            _spine1.Show();
            SpineManager.instance.DoAnimation(_spine1, "kong", false);

            _bg0 = curTrans.Find("Bg/0").gameObject;
            _bg0.Hide();
            _bg1 = curTrans.Find("Bg/1").gameObject;
            _bg1.Hide();


        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            
            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine0, "a", false);                           //流程1
            mono.StartCoroutine(WariteCoroutine(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, false);
            }, 0.5f));            
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            { 
               isPlaying = false;
               _bg0.Show();
                SpineManager.instance.SetTimeScale(_spine0, 1);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);               
                SpineManager.instance.DoAnimation(_spine0, "b", false);     //流程2
               SpineManager.instance.DoAnimation(_spine1, "changdonghua", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE,1, null, () =>
                {
                    SpineManager.instance.SetTimeScale(_spine0, 0.5f);
                    SpineManager.instance.DoAnimation(_spine1, "kong", false);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                    {                       
                        SpineManager.instance.SetTimeScale(_spine0, 1);
                        _bg1.Show();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
                        SpineManager.instance.DoAnimation(_spine0, "c", true);     //流程3
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, null));
                    }));
                }));
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

        IEnumerator WariteCoroutine(Action method_2 = null, float len = 0)
        {
            
            yield return new WaitForSeconds(len);           
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
    }
}
