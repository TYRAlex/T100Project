using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course736Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject btn;
        private GameObject show;
        private GameObject ttj;
        bool _canClick;

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

            btn = curTrans.Find("btn").gameObject;
            ttj = curTrans.Find("ttj").gameObject;
            Util.AddBtnClick(btn,Click);
            GameInit();
            GameStart();
        }

        private void Click(GameObject obj)
        {
            if (!_canClick)
                return;
            _canClick = false;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(ttj,"b3",false,()=> { SpineManager.instance.DoAnimation(ttj,"b4",true); });
            //SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject,"a2",false);
            //SpineManager.instance.DoAnimation(show.transform.GetChild(1).gameObject, "a3", false);
            Speak(1,()=> { SoundManager.instance.ShowVoiceBtn(true); });
        }





        private void GameInit()
        {
            talkIndex = 1;
            ttj.GetComponent<SkeletonGraphic>().Initialize(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true; }));
            SpineManager.instance.DoAnimation(ttj,"b",false,()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1, true); SpineManager.instance.DoAnimation(ttj,"b1",false,()=> { SpineManager.instance.DoAnimation(ttj,"b2",true); }); });
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
        }

        private void Speak(int index, Action callback = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, null, callback));
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
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SpineManager.instance.DoAnimation(ttj,"b5",false);
                Speak(2,()=> { Max.SetActive(false); });
            }

            talkIndex++;
        }

        IEnumerator Wait(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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
