using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course933Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject Box;
        private bool _canClick;
        private bool[] _jugle;
        private bool _canvoice;
        private GameObject btnBack;
        private string temp;
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

            Box = curTrans.Find("Box").gameObject;
            _jugle = new bool[3];
            btnBack = curTrans.Find("btnBack").gameObject;
            for (int i = 0; i < 3; i++)
            {
                Util.AddBtnClick(Box.transform.GetChild(i).gameObject, Click);
            }

            Util.AddBtnClick(btnBack,Back);
            GameInit();
            GameStart();
        }

        private void Click(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);
                if(obj.name=="a")
                {
                    SpineManager.instance.DoAnimation(Box,"dian1",false,
                        ()=>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                            SpineManager.instance.DoAnimation(Box, "a", false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                                () =>
                                {
                                    btnBack.SetActive(true);
                                    temp = "a";
                                }
                                ));
                        }
                        );
                }
                if (obj.name == "b")
                {
                    SpineManager.instance.DoAnimation(Box, "dian2", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                            SpineManager.instance.DoAnimation(Box, "b", false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                                () =>
                                {
                                    btnBack.SetActive(true);
                                    temp = "b";
                                }
                                ));
                        }
                        );
                }
                if (obj.name == "c")
                {
                    SpineManager.instance.DoAnimation(Box, "dian3", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
                            SpineManager.instance.DoAnimation(Box, "c", false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                                () =>
                                {
                                    temp = "c";
                                    btnBack.SetActive(true);
                                }
                                ));
                        }
                        );
                }
            }
        }

        private void Back(GameObject obj)
        {
            btnBack.SetActive(false);
            if(temp=="a")
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                SpineManager.instance.DoAnimation(Box, "jing", false);
                _canClick = true;
                _jugle[0] = true; Jugle();

            }
            if(temp=="b")
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                SpineManager.instance.DoAnimation(Box, "jing", false);
                _canClick = true;
                _jugle[1] = true; Jugle();

            }
            if(temp=="c")
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                SpineManager.instance.DoAnimation(Box, "jing", false);
                _canClick = true;
                _jugle[2] = true; Jugle();

                
            }
        }

        private void Jugle()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_jugle[i])
                    return;
            }
            if(!_canvoice)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }



        private void GameInit()
        {
            talkIndex = 1;
            Box.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            btnBack.SetActive(false);
            temp = string.Empty;
            _canClick = false;
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false;_canClick = true; }));
            SpineManager.instance.DoAnimation(Box,"jing",false);
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
                Max.SetActive(true);
                _canClick = false;
                _canvoice = true;
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,4,null,null));
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
