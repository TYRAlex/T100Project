using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course933Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        private bool _canClick;
        private bool _canClick1;
        private bool _canClick2;
        private bool _canClick3;
        private bool _canvoice;
        private bool[] _jugle;

        private GameObject xsp;
        private GameObject shu;
        private GameObject show;
        private GameObject show2;
        private GameObject btn;
        private GameObject bj;
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

            shu = curTrans.Find("shu").gameObject;
            xsp = curTrans.Find("xsp").gameObject;
            show = curTrans.Find("show").gameObject;
            show2 = curTrans.Find("show2").gameObject;
            btn = curTrans.Find("btn").gameObject;
            _jugle = new bool[2];
            bj = Bg.transform.GetChild(0).gameObject;

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            btn.SetActive(false);
            btn.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Util.AddBtnClick(xsp.transform.GetChild(0).gameObject, Click);
            Util.AddBtnClick(xsp.transform.GetChild(1).gameObject, Click1);
            Util.AddBtnClick(xsp.transform.GetChild(2).gameObject, Click2);
            Util.AddBtnClick(btn.transform.GetChild(0).gameObject, Click3);
            Util.AddBtnClick(btn.transform.GetChild(1).gameObject, Click3);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            shu.SetActive(false);
            show2.SetActive(false);
            show2.transform.GetChild(0).gameObject.SetActive(false);
            _canvoice = true;
            xsp.SetActive(true);
            bj.SetActive(false);
            bj.transform.position = Bg.transform.GetChild(1).position;
            xsp.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            for (int i = 0; i < 2; i++)
            {
                show.transform.GetChild(i).gameObject.SetActive(false);
                show.transform.GetChild(i).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true; isPlaying = false; }));
            for (int i = 0; i < 2; i++)
            {
                show.transform.GetChild(i).gameObject.SetActive(false);
            }
            SpineManager.instance.DoAnimation(xsp,"jing",false);
        }

        private void Click(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                Max.SetActive(false);
                SoundManager.instance.ShowVoiceBtn(false);
                SpineManager.instance.DoAnimation(xsp, "dianji", false,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                        bj.SetActive(true);
                        bj.transform.GetComponent<RectTransform>().DOAnchorPosX(-941, 21f).SetEase(Ease.Linear).SetId("1");
                        mono.StartCoroutine(Wait(12f,
                            () =>
                            {
                                DOTween.Pause(1);
                                bj.transform.GetComponent<RectTransform>().DOAnchorPosX(bj.GetComponent<RectTransform>().anchoredPosition.x+941f, 12f).SetEase(Ease.Linear).SetId("2");
                            }
                            ));
                        SpineManager.instance.DoAnimation(shu, "1", true);
                        SpineManager.instance.DoAnimation(xsp, "dh0", false,
                            () =>
                            {
                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                DOTween.Clear();
                                SpineManager.instance.DoAnimation(xsp, "jing", false);
                                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                bj.SetActive(false);
                                bj.transform.position = Bg.transform.GetChild(1).position;
                                SoundManager.instance.ShowVoiceBtn(true);
                                _canClick = true;
                                Max.SetActive(true);
                            }
                            );
                    }
                    );
            }
        }

        private void Click1(GameObject obj)
        {
            if (_canClick1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                _canClick1 = false;
                show.transform.GetChild(0).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject, "dianji1", false);
                SpineManager.instance.DoAnimation(xsp,"xian1",false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                    () => { _canClick2 = true; show2.transform.GetChild(0).gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(show2.transform.GetChild(0).gameObject, "dianji4", false); }
                    ));
            }
        }

        private void Click2(GameObject obj)
        {
            if (_canClick2)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                show.transform.GetChild(1).gameObject.SetActive(true);
                _canClick2 = false;
                SpineManager.instance.DoAnimation(show.transform.GetChild(1).gameObject, "dianji2", false);
                SpineManager.instance.DoAnimation(xsp, "xian2", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(xsp, "jing3", false);
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    ));
            }
        }

        private void Click3(GameObject obj)
        {
            if (_canClick3)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick3 = false;
                if (obj.name == "1")
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3));
                    SpineManager.instance.DoAnimation(btn, "anniu2", false);
                    SpineManager.instance.DoAnimation(xsp, "donghua1", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(btn, "anniu1", false);
                            _canClick3 = true;
                            _jugle[0] = true;
                            Julge();
                        }
                        );
                }
                else
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4,null,
                        () => {
                            SpineManager.instance.DoAnimation(btn, "anniu1", false);
                            _canClick3 = true;
                            _jugle[1] = true;
                            Julge();
                        }
                        ));
                    SpineManager.instance.DoAnimation(btn, "anniu3", false);
                    SpineManager.instance.DoAnimation(xsp, "donghua2", false );
                }
            }
        }

        IEnumerator Wait(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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

        private void Julge()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_jugle[i])
                    return;
            }
            if (_canvoice)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _canClick = false;
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                shu.SetActive(true);
                SpineManager.instance.DoAnimation(shu, "1", true);
                SpineManager.instance.DoAnimation(xsp, "jing2", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                    () => { _canClick1 = true; }
                    ));
                show2.SetActive(true);
                SpineManager.instance.DoAnimation(show2.transform.GetChild(1).gameObject, "dianji3", false);
                
            }
            if (talkIndex == 2)
            {
                btn.SetActive(true);
                SpineManager.instance.DoAnimation(btn, "anniu1", false);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                    () => { _canClick3 = true; }
                    ));
            }
            if (talkIndex == 3)
            {
                _canvoice = false;
                _canClick3 = false;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null,null));
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
