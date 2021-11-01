using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7310Part5
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
        private GameObject show;
        private GameObject show1;
        private GameObject show2;
        private bool _canclick;
        private Coroutine temp;
        private bool[] _jugle;
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
            for (int i = 0; i < 3; i++)
            {
                Util.AddBtnClick(Box.transform.GetChild(i).gameObject, Click);
            }
            show = curTrans.Find("show").gameObject;
            show1 = show.transform.GetChild(0).gameObject;
            show2 = show.transform.GetChild(1).gameObject;
            
            _jugle = new bool[3];
            GameInit();
            GameStart();
        }

        private void Click(GameObject obj)
        {
            if (!_canclick)
                return;
            BtnPlaySound();
            _canclick = false;
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "bj" + obj.name + "1", false,
                () =>
                {
                    Box.SetActive(false);
                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                    show.SetActive(true);
                    show1.SetActive(true);
                    show2.SetActive(true);
                    if (obj.name == "a")
                    {
                        SpineManager.instance.DoAnimation(show1, "zhaopian2", false);
                        SpineManager.instance.DoAnimation(show2, "zhaopian1", false);
                        temp = mono.StartCoroutine(Change());
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                            () =>
                            {
                                mono.StopCoroutine(temp); show.SetActive(false);
                                show1.GetComponent<SkeletonGraphic>().Initialize(true); show2.GetComponent<SkeletonGraphic>().Initialize(true); _canclick = true; Box.SetActive(true);
                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0]; _jugle[0] = true; Jugle();
                            }
                            ));
                    }
                    if (obj.name == "b")
                    {
                        SpineManager.instance.DoAnimation(show1, "zhaopian4", false);
                        SpineManager.instance.DoAnimation(show2, "zhaopian3", false);
                        temp = mono.StartCoroutine(Change());
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                            () =>
                            {
                                mono.StopCoroutine(temp); show.SetActive(false);
                                show1.GetComponent<SkeletonGraphic>().Initialize(true); show2.GetComponent<SkeletonGraphic>().Initialize(true); _canclick = true; Box.SetActive(true);
                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0]; _jugle[1] = true; Jugle();

                            }
                            ));
                    }
                    if (obj.name == "c")
                    {
                        SpineManager.instance.DoAnimation(show1, "zhaopian6", false);
                        SpineManager.instance.DoAnimation(show2, "zhaopian5", false);
                        temp = mono.StartCoroutine(Change());
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                            () =>
                            {
                                mono.StopCoroutine(temp); show.SetActive(false);
                                show1.GetComponent<SkeletonGraphic>().Initialize(true); show2.GetComponent<SkeletonGraphic>().Initialize(true); _canclick = true; Box.SetActive(true);
                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0]; _jugle[2] = true; Jugle();

                            }
                            ));
                    }
                }
                );
        }

        IEnumerator Change()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                show2.SetActive(false);
                yield return new WaitForSeconds(2f);
                show2.SetActive(true);
            }
        }

        private void Jugle()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_jugle[i])
                    return;
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }


        private void GameInit()
        {
            talkIndex = 1;
            Box.SetActive(true);
            var sGs=  Box.GetComponentsInChildren<SkeletonGraphic>();

            foreach (var sG in sGs)
            {
                sG.Initialize(true);
                var go = sG.gameObject;
                var name = sG.gameObject.name;

                switch (name)
                {
                    case "a":
                        SpineManager.instance.DoAnimation(go, "bja2", false);
                        break;
                    case "b":
                        SpineManager.instance.DoAnimation(go, "bjb2", false);
                        break;
                    case "c":
                        SpineManager.instance.DoAnimation(go, "bjc2", false);
                        break;
                }
            }

            //for (int i = 0; i < 3; i++)
            //{
            //    Box.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);       
            //}


            _canclick = false;
            show.SetActive(false);
            show1.GetComponent<SkeletonGraphic>().Initialize(true); show2.GetComponent<SkeletonGraphic>().Initialize(true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => {  isPlaying = false; _canclick = true; }));

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
                show.SetActive(true); Box.SetActive(false);
                show2.SetActive(true); SpineManager.instance.DoAnimation(show2, "zhaopian7", false);
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
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
