using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course924Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject clickBox;
        private GameObject show;
        private bool _canClick;
        private bool _canRenturn;
        private GameObject Return;
        private string _rname;
        private GameObject _guang;
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

            clickBox = curTrans.Find("clickBox").gameObject;
            show = curTrans.Find("show").gameObject;
            _guang = curTrans.Find("RawImage").gameObject;
            Return = curTrans.Find("Return").gameObject;

            Util.AddBtnClick(Return,ReturnEvent);
            for (int i = 0; i < 3; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject, ClickEvent);
            }
            GameInit();
            GameStart();
        }

        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                BtnPlaySound();
                _canClick = false;
                clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                if (obj.name == "1")
                {
                    
                    SpineManager.instance.DoAnimation(clickBox, "d1", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,3,null,
                                ()=>
                                {
                                    _rname = "1";
                                    Return.SetActive(true);
                                    _canRenturn = true;
                                }
                                ));
                            _guang.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1]; 
                            SpineManager.instance.DoAnimation(clickBox, "titui", false); }
                        );
                    mono.StartCoroutine(WaitTime(3,
                        () =>
                        {
                            SpineManager.instance.SetFreeze(clickBox, true);
                            show.SetActive(true);
                            SpineManager.instance.DoAnimation(show, "jing2", false,
                                ()=>
                                { SpineManager.instance.DoAnimation(show,"jing3",false,
                                    ()=>
                                    { SpineManager.instance.DoAnimation(show,"jing4",false); }
                                    ); }
                                );
                        }
                        ));
                    mono.StartCoroutine(WaitTime(9f,
                        () =>
                        {
                            //SpineManager.instance.DoAnimation(show,"kong",false,
                            //    () => { show.SetActive(false); }
                            //    );
                            show.SetActive(false);
                            SpineManager.instance.SetFreeze(clickBox, false);
                        }
                        ));
                }

                if (obj.name == "2")
                {
                    
                    SpineManager.instance.DoAnimation(clickBox,"d2",false,
                        ()=>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                            _guang.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                            
                            SpineManager.instance.DoAnimation(clickBox,"shafa",false,
                            ()=>
                            {
                                _rname = "2";
                                Return.SetActive(true);
                                _canRenturn = true;
                            }
                            ); }
                        );
                }

                if (obj.name == "3")
                {
                    
                    SpineManager.instance.DoAnimation(clickBox, "d3", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                                ()=>
                                {
                                    _rname = "3";
                                    Return.SetActive(true);
                                    _canRenturn = true;
                                }
                                ));
                            _guang.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
                            curTrans.Find("di").gameObject.SetActive(false);
                            SpineManager.instance.DoAnimation(clickBox, "xiangzi", false);
                        }
                        );
                }
            }
        }

        private void ReturnEvent(GameObject obj)
        {
            if (_canRenturn)
            {
                clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                BtnPlaySound();
                _canRenturn = false;
                if (_rname == "1")
                {
                    _guang.SetActive(true);
                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                    SpineManager.instance.DoAnimation(clickBox, "t1", false,
                  () =>
                  {
                      SpineManager.instance.DoAnimation(clickBox, "jing", false);
                      _canClick = true; Return.SetActive(false);
                  }
                  );
                }
                if (_rname == "2")
                {
                    _guang.SetActive(true);
                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                    SpineManager.instance.DoAnimation(clickBox, "t2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(clickBox, "jing", false);
                        _canClick = true; Return.SetActive(false);

                    }
                    );
                }
                if (_rname == "3")
                {
                    _guang.SetActive(true);
                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                    SpineManager.instance.DoAnimation(clickBox, "t3", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(clickBox, "jing", false);
                        _canClick = true; Return.SetActive(false);
                    }
                    );
                }
            }
        }

        IEnumerator WaitTime(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        private void GameInit()
        {
            Return.SetActive(false);
            _rname = string.Empty;
            _canRenturn = false;

            _guang.SetActive(true);
            talkIndex = 1;
            _canClick = false;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            curTrans.Find("di").gameObject.SetActive(false);
            show.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(clickBox,"jing",false);
            clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().freeze = false;
            show.SetActive(false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; _canClick = true; }));

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

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
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


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
