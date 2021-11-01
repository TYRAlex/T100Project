using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ILFramework.HotClass
{
    public class Course721Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private VideoPlayer _videoPlayer;
        private Transform _videos;
        private RawImage _rtImg;
        private Transform _mask;
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

            _videoPlayer = curTrans.Find("VideoPlayer").GetComponent<VideoPlayer>();
            _rtImg = curTrans.GetRawImage("Videos/RTImg");
            _videos = curTrans.Find("Videos");
            _mask = curTrans.Find("mask");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            _rtImg.gameObject.SetActive(false);
            talkIndex = 1;

        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => {  isPlaying = false;SoundManager.instance.ShowVoiceBtn(true); }));

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
                OnClickEvent1();
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void OnClickEvent1()
        {
            Debug.Log("1");
            //var name = go.name;

            //var soundIndex = int.Parse(go.transform.GetChild(0).name);

            string url = string.Empty;
            int voiceIndex = -1;

            url = "1.mp4";
            voiceIndex = 0;
           


            _videoPlayer.url = GetVideoPath(url);
            mono.StartCoroutine(PlayMp4());
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            _mask.gameObject.SetActive(true);
            Delay(29f, () => { OnClickMask(); });

        }
        IEnumerator PlayMp4()
        {
            _videoPlayer.Prepare();

            while (true)
            {
                if (!_videoPlayer.isPrepared)   //监听是否准备完毕。没有完成一直等待，完成后跳出循环，进行img赋值，让后播放                             
                    yield return null;
                else
                    break;
            }

            _rtImg.texture = _videoPlayer.texture;
            _videoPlayer.Play();
            ShowChilds(_videos, 0);

            StopCoroutines("PlayMp4");
        }
        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void StopCoroutines(string methodName)
        {
            mono.StopCoroutine(methodName);
        }
        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
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
        void OnClickMask()
        {

            AddEvent(_mask.gameObject, g => {
                // PlayCommonSound(2);
                RemoveEvent(g);
                HideAllChilds(_videos);
                _rtImg.texture = null;
        

                _mask.gameObject.Hide();
            });
        }
        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }
        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }
    }
}
