using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ILFramework.HotClass
{
    public class Course848Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform _btns;
        private Transform _spineGo;
        private RawImage _rtImg;
        private RawImage _rtImg2;
        bool isPlaying = false;
        private Transform _videos;
        private Transform _mask;
        private VideoPlayer _videoPlayer;
        private VideoPlayer _videoPlayer2;


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

            _btns = curTrans.Find("Btns");
            _spineGo = curTrans.Find("Spines");
            _videoPlayer = curTrans.Find("VideoPlayer").GetComponent<VideoPlayer>(); 
            _rtImg = curTrans.GetRawImage("Videos/RTImg"); 
            _videos = curTrans.Find("Videos");
            _mask = curTrans.Find("mask");


            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            PlaySpine(_spineGo.GetChild(0).gameObject,"animation",null,true); PlaySpine(_spineGo.GetChild(1).gameObject, "animation", null, true);

            _rtImg.gameObject.Hide();
          
        
            Util.AddBtnClick(_btns.GetChild(0).gameObject,OnClickEvent1);
            Util.AddBtnClick(_btns.GetChild(1).gameObject, OnClickEvent2);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, ()=> { _mask.gameObject.SetActive(true); }, () => { _mask.gameObject.SetActive(false); Max.SetActive(false); isPlaying = false; }));

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

            }

            talkIndex++;
        }

        private void OnClickEvent1(GameObject go)
        {
            Debug.Log("1");
            var name = go.name;

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            string url = string.Empty;
            int voiceIndex = -1;


            switch (name)
            {
                case "a":
                    url = "1.mp4";
                    voiceIndex = 0;
                    break;
                case "b":
                    url = "2.mp4";
                    voiceIndex = 1;
                    break;

            }

            _videoPlayer.targetTexture.Release();
            _videoPlayer.targetTexture.DiscardContents();
            _videoPlayer.url = GetVideoPath(url);
            PlaySpine(_spineGo.GetChild(0).gameObject, "animation2",()=> 
            {
                mono.StartCoroutine(PlayMp4());
                //PlaySpine(_spineGo.GetChild(0).gameObject,"animation",null,true);
            });
            _mask.gameObject.SetActive(true);
            Delay(24f,()=> { OnClickMask(go); });
   
      }
        private void OnClickEvent2(GameObject go)
        {
            Debug.Log("1");
            var name = go.name;

            var soundIndex = int.Parse(go.transform.GetChild(0).name);

            string url = string.Empty;
            int voiceIndex = -1;


            switch (name)
            {
                case "a":
                    url = "1.mp4";
                    voiceIndex = 0;
                    break;
                case "b":
                    url = "2.mp4";
                    voiceIndex = 1;
                    break;

            }

            _videoPlayer.targetTexture.Release();
            _videoPlayer.targetTexture.DiscardContents();
            _videoPlayer.url = GetVideoPath(url);
            PlaySpine(_spineGo.GetChild(1).gameObject, "animation2", () =>
            {
                mono.StartCoroutine(PlayMp4());
               // PlaySpine(_spineGo.GetChild(1).gameObject, "animation", null, true);
            });
            _mask.gameObject.SetActive(true);
            Delay(37f, () => { OnClickMask(go); });

        }
        void OnClickMask(GameObject obj)
        {
          
            AddEvent(_mask.gameObject, g => {
               // PlayCommonSound(2);
                RemoveEvent(g);
                HideAllChilds(_videos);
                Max.SetActive(true);
                _rtImg.texture = null;
                //_rtImg2.texture = null;
                PlaySpine(_spineGo.Find(obj.name).gameObject,"animation3",()=> 
                {
                    PlaySpine(_spineGo.Find(obj.name).gameObject,"animation",null,true);
                },false);
                
                _mask.gameObject.Hide();
            });
        }
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

   

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
        }
        IEnumerator PlayMp4()
        {
           // _rtImg.color = new Color(1,1,1,0);
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
        //IEnumerator PlayMp42()
        //{
        //    _rtImg2.color = new Color(1, 1, 1, 0);
        //    _videoPlayer2.Prepare();

        //    while (true)
        //    {
        //        if (!_videoPlayer2.isPrepared)   //监听是否准备完毕。没有完成一直等待，完成后跳出循环，进行img赋值，让后播放                             
        //            yield return null;
        //        else
        //            break;
        //    }

        //    // _rtImg.texture = _videoPlayer.texture;
        //    _videoPlayer2.Play();
        //    Delay(0.2f, () => { _rtImg2.color = new Color(1, 1, 1, 1); });
        //    ShowChilds(_videos, 1);

        //    StopCoroutines("PlayMp42");
        //}
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
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
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
