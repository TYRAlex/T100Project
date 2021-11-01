using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course736Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject pic;
        private bool[] _jugle;
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
            _jugle = new bool[5];
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            pic = curTrans.Find("Box").GetChild(0).GetChild(0).gameObject;
            for (int i = 0; i < 5; i++)
            {
                Util.AddBtnClick(curTrans.Find("Box").GetChild(1).GetChild(i).gameObject, Click);
            }
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            pic.SetActive(false);
            curTrans.Find("Box").GetChild(2).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false;_canClick = true;SoundManager.instance.ShowVoiceBtn(true); }));
            SpineManager.instance.DoAnimation(curTrans.Find("Box").GetChild(2).gameObject,"a1",true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0,true);
        }

        private void Click(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            pic.SetActive(true);
            pic.GetComponent<Image>().sprite = pic.GetComponent<BellSprites>().sprites[Convert.ToInt32(obj.name)];
            pic.GetComponent<Image>().SetNativeSize();
            _jugle[Convert.ToInt32(obj.name)] = true; 
        }

        private void Jugle()
        {
            for (int i = 0; i < 5; i++)
            {
                if (!_jugle[i])
                    return;
            }
            SoundManager.instance.ShowVoiceBtn(true);
            _canClick = false;
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
                _canClick = false;
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,()=> { Max.SetActive(false);_canClick = true; }));
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
