using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Video;

namespace ILFramework.HotClass
{
    public class Course847Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject Video;
        private GameObject lyj;
        private GameObject clickBox;
        private GameObject _mask;
        private bool[] jugleClick;
        private bool a, b, c;
        private VideoPlayer _videoPlayer;
        private Transform _videos;
        private RawImage _rtImg;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _mask = curTrans.Find("_mask").gameObject;
            Max = curTrans.Find("bell").gameObject;
            jugleClick = new bool[4];

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            lyj = curTrans.Find("lyj").gameObject;
            clickBox = curTrans.Find("ClickBox").gameObject;

            for (int i = 0; i < 4; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject, ClickEvent);
            }

            _videos = curTrans.Find("Videos");

            if (_videoPlayer==null)
            {
                Debug.LogError("111");
                _videoPlayer = curTrans.Find("VideoPlayer").gameObject.GetComponent<VideoPlayer>();
                _videoPlayer.url = GetVideoPath("1.mp4");
                mono.StartCoroutine(PlayMp4());
                _videoPlayer.isLooping = true;

            }
          


            
            
            _rtImg = curTrans.GetRawImage("Videos/RTImg");


            GameInit();
            GameStart();

          
        }


        private void  ClickEvent(GameObject obj)
        {
            _mask.SetActive(true);
            switch(obj.name)
            {
                case "a":
                    BtnPlaySound();
                    if (!a)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    }
                    clickBox.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    clickBox.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(clickBox.transform.GetChild(0).GetChild(0).gameObject,"animation",true);
                    mono.StartCoroutine(ieWaitTime(8,
                        ()=>
                        {
                            _mask.SetActive(false);
                            clickBox.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                            clickBox.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                            if (!a)
                            {
                                a = true;
                                SpineManager.instance.DoAnimation(clickBox.transform.GetChild(0).GetChild(1).gameObject, "m1", false,
                              () =>
                              { SpineManager.instance.DoAnimation(clickBox.transform.GetChild(0).GetChild(1).gameObject, "m12", false); }
                              );
                            }
                        }
                        )); ;

                    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,2,null,
                        ()=>
                        { 
                            jugleClick[0] = true;
                            JugleClick();
                        }
                        )); 
                    break;
                case "b":
                    BtnPlaySound();
                    if (!b)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    }
                    clickBox.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(clickBox.transform.GetChild(1).GetChild(0).gameObject, "animation2", true);
                    mono.StartCoroutine(ieWaitTime(4,
                       () =>
                       {
                           clickBox.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                           clickBox.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                           if (!b)
                           {
                               b = true;
                               SpineManager.instance.DoAnimation(clickBox.transform.GetChild(1).GetChild(1).gameObject, "m2", false,
                             () =>
                             { SpineManager.instance.DoAnimation(clickBox.transform.GetChild(1).GetChild(1).gameObject, "m22", false); }
                             );
                           }
                       }
                       )); ;
                   
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3,null,
                        () =>
                        { _mask.SetActive(false);
                            jugleClick[1] = true;
                            JugleClick();
                        }
                        ));
                    break;
                case "c":
                    BtnPlaySound();
                    if (!c)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    }
                    clickBox.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
                    clickBox.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(clickBox.transform.GetChild(2).GetChild(0).gameObject, "animation3", true);
                    mono.StartCoroutine(ieWaitTime(1,
                       () =>
                       {
                           clickBox.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                           clickBox.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                           if (!c)
                           {
                               c = true;
                               SpineManager.instance.DoAnimation(clickBox.transform.GetChild(2).GetChild(1).gameObject, "m6", false,
                             () =>
                             { SpineManager.instance.DoAnimation(clickBox.transform.GetChild(2).GetChild(1).gameObject, "m32", false); }
                             );
                           }
                       }
                       )); ;
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4,null,
                        () =>
                        { _mask.SetActive(false);
                            jugleClick[2] = true;
                            JugleClick();
                        }
                        ));
                    break;
                case "d":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    clickBox.transform.GetChild(4).gameObject.SetActive(true);
                    clickBox.transform.GetChild(4).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(clickBox.transform.GetChild(4).gameObject, "x", false,
                        ()=>
                        { SpineManager.instance.DoAnimation(clickBox.transform.GetChild(4).gameObject, "x2", false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5,null,
                            () =>
                            { _mask.SetActive(false);
                                jugleClick[3] = true;
                                JugleClick();
                            }
                            ));
                        }
                        );
                    break;
            }
        }


        private void JugleClick()
        {
            for (int i = 0; i < 4; i++)
            {
                if(!jugleClick[i])
                { return; }
            }
            //SoundManager.instance.ShowVoiceBtn(true);
        }

        IEnumerator ieWaitTime(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        private void GameInit()
        {
            _rtImg.texture = null;
            _rtImg.GetComponent<RawImage>().color = new Vector4(255, 255, 255, 0);

            a = false;
            b = false;
            c = false;

            curTrans.Find("1").gameObject.SetActive(false);
            curTrans.Find("2").gameObject.SetActive(false);
            curTrans.Find("3").gameObject.SetActive(false);
            curTrans.Find("4").gameObject.SetActive(false);

            _mask.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                clickBox.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                clickBox.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
            clickBox.transform.GetChild(4).gameObject.SetActive(false);
            clickBox.SetActive(false);
            talkIndex = 1;

        }

        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
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
            _rtImg.GetComponent<RawImage>().color = new Vector4(255, 255, 255, 255);
            _rtImg.gameObject.SetActive(true);
            _rtImg.texture = _videoPlayer.texture;
            _videoPlayer.Play();

            //StopCoroutines("PlayMp4");
        }

        private void StopCoroutines(string methodName)
        {
            mono.StopCoroutine(methodName);
        }

        void GameStart()
        {
            
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            lyj.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(lyj,"a",true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));

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
                _mask.SetActive(true);
                _rtImg.gameObject.SetActive(false);
                //SpineManager.instance.DoAnimation(lyj,"jingzhi",false);
                clickBox.SetActive(true);
                curTrans.Find("1").gameObject.SetActive(true);
                curTrans.Find("2").gameObject.SetActive(true);
                curTrans.Find("3").gameObject.SetActive(true);
                curTrans.Find("4").gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                    ()=>
                    { _mask.SetActive(false); }
                    ));
            }
            //if(talkIndex ==2)
            //{
            //    clickBox.SetActive(false);
            //    Video.SetActive(true);
            //    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.COMMONVOICE,4,null,null));
            //}
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
