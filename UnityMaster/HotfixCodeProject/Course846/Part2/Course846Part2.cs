using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course846Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private  bool a, b, c, d;
        private bool _canClick;
        private GameObject clickBox;
        private bool[] allClick;
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

            allClick = new bool[4];
            clickBox = curTrans.Find("clickBox").gameObject;

            for (int i = 0; i < 4; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject,ClickEvent);
            }

            GameInit();
            GameStart();
        }

        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                obj.transform.SetAsLastSibling();
                switch (obj.name)
                {
                    case "a":
                        if (!a)
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name, false,
                                () =>
                                { SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                    _canClick = true;
                                    a = true;
                                }
                                );
                        }
                        else
                        {
                            BtnPlaySound();
                                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "3", false,
                                    ()=>
                                    {
                                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                                            ()=>
                                            {
                                                SpineManager.instance.DoAnimation(curTrans.Find("showa").gameObject, "kong", false);
                                                _canClick = true;
                                                allClick[0] = true;
                                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                                Bg.transform.GetChild(0).gameObject.SetActive(true);
                                                curTrans.Find("mask").gameObject.SetActive(true);
                                                clickBox.SetActive(true);
                                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                                JuglieAllClick();
                                            }
                                            ));
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,3,true);
                                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                        curTrans.Find("showa").SetAsLastSibling();
                                        SpineManager.instance.DoAnimation(curTrans.Find("showa").gameObject,"animation",true);
                                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                                        curTrans.Find("mask").gameObject.SetActive(false);
                                        clickBox.SetActive(false);
                                        Bg.transform.GetChild(0).gameObject.SetActive(false);
                                    }
                                    );                    
                        }
                        break;
                    case "b":
                        if (!b)
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                    _canClick = true;
                                    b = true;
                                }
                                );
                        }
                        else
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "3", false,
                                    () =>
                                    {
                                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,
                                            () =>
                                            {
                                                SpineManager.instance.DoAnimation(curTrans.Find("showb").gameObject, "kong", false);
                                                _canClick = true;
                                                allClick[1] = true;
                                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                                Bg.transform.GetChild(0).gameObject.SetActive(true);
                                                curTrans.Find("mask").gameObject.SetActive(true);
                                                clickBox.SetActive(true);
                                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                                JuglieAllClick();
                                            }
                                            ));
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                        curTrans.Find("showb").SetAsLastSibling();
                                        SpineManager.instance.DoAnimation(curTrans.Find("showb").gameObject, "b", true);
                                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                                        curTrans.Find("mask").gameObject.SetActive(false);
                                        clickBox.SetActive(false);
                                        Bg.transform.GetChild(0).gameObject.SetActive(false);
                                    }
                                    );       
                        }
                        break;
                    case "c":
                        if (!c)
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                    _canClick = true;
                                    c = true;
                                }
                                );
                        }
                        else
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "3", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                    curTrans.Find("showc").SetAsLastSibling();
                                    SpineManager.instance.DoAnimation(curTrans.Find("showc").gameObject, "d", false);
                                    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                                        ()=>
                                        {
                                            SpineManager.instance.DoAnimation(curTrans.Find("showc").gameObject, "kong", false);
                                            SpineManager.instance.DoAnimation(curTrans.Find("showc").GetChild(0).gameObject, "kong", false);
                                            _canClick = true;
                                            allClick[2] = true;
                                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                            Bg.transform.GetChild(0).gameObject.SetActive(true);
                                            curTrans.Find("mask").gameObject.SetActive(true);
                                            clickBox.SetActive(true);
                                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                            JuglieAllClick();
                                        }
                                        ));
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                                    SpineManager.instance.DoAnimation(curTrans.Find("showc").GetChild(0).gameObject, "d", true);
                                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
                                    curTrans.Find("mask").gameObject.SetActive(false);
                                    clickBox.SetActive(false);
                                    Bg.transform.GetChild(0).gameObject.SetActive(false);
                                }
                                );
                        }
                        break;
                    case "d":
                        if (!d)
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                    _canClick = true;
                                    d = true;
                                }
                                );
                        }
                        else
                        {
                            BtnPlaySound();
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "3", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", false);
                                    SpineManager.instance.DoAnimation(curTrans.Find("showd").gameObject,"c",false);
                                    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,2,null,
                                        ()=>
                                        {
                                            SpineManager.instance.DoAnimation(curTrans.Find("showd").gameObject, "kong", false);
                                            SpineManager.instance.DoAnimation(curTrans.Find("showd").GetChild(0).gameObject, "kong", false);
                                            _canClick = true;
                                            allClick[3] = true;
                                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                            Bg.transform.GetChild(0).gameObject.SetActive(true);
                                            curTrans.Find("mask").gameObject.SetActive(true);
                                            clickBox.SetActive(true);
                                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                            JuglieAllClick();
                                        }
                                        ));
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                                    SpineManager.instance.DoAnimation(curTrans.Find("showd").GetChild(0).gameObject, "c", true );
                                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[4];
                                    curTrans.Find("mask").gameObject.SetActive(false);
                                    clickBox.SetActive(false);
                                    Bg.transform.GetChild(0).gameObject.SetActive(false);
                                }
                                );
                        }
                        break;
                }
            }
        }

        private void JuglieAllClick()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!allClick[i])
                    return;
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }



        private void GameInit()
        {
            Bg.transform.GetChild(0).gameObject.SetActive(true);
            curTrans.Find("mask").gameObject.SetActive(true);
            curTrans.Find("Dbell").gameObject.SetActive(false);
            clickBox.SetActive(true);
            SpineManager.instance.DoAnimation(clickBox.transform.Find("a").GetChild(0).gameObject, "01", false);
            SpineManager.instance.DoAnimation(clickBox.transform.Find("b").GetChild(0).gameObject, "02", false);
            SpineManager.instance.DoAnimation(clickBox.transform.Find("c").GetChild(0).gameObject, "03", false);
            SpineManager.instance.DoAnimation(clickBox.transform.Find("d").GetChild(0).gameObject, "04", false);
            talkIndex = 1;
            SpineManager.instance.DoAnimation(curTrans.Find("showa").gameObject,"kong",false);
            SpineManager.instance.DoAnimation(curTrans.Find("showb").gameObject, "kong", false);
            SpineManager.instance.DoAnimation(curTrans.Find("showc").GetChild(0).gameObject, "kong", false);
            SpineManager.instance.DoAnimation(curTrans.Find("showd").GetChild(0).gameObject, "kong", false);
            SpineManager.instance.DoAnimation(curTrans.Find("showc").gameObject, "kong", false);
            SpineManager.instance.DoAnimation(curTrans.Find("showd").gameObject, "kong", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false;_canClick = true; }));

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
                curTrans.Find("mask").gameObject.SetActive(false);
                clickBox.SetActive(false);
                curTrans.Find("Dbell").gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(curTrans.Find("Dbell").gameObject,SoundManager.SoundType.VOICE,5));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
