using System;
using System.Collections;
using UnityEngine;
using System.Data;
using System.Data.SqlTypes;

namespace ILFramework.HotClass
{
    public class Course9212Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject xjyq;
        private bool _canClick;
        private GameObject clickBox;
        private GameObject a, b, c, d, e, f;
        private GameObject show;
        private bool[] _allJugle;
        private string sum;
        private string ad;
        private string _rname;
        private bool _canRenturn;
        private GameObject Return;
        

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
            _rname = string.Empty;
            xjyq = curTrans.Find("xjyq").gameObject;
            clickBox = curTrans.Find("clickBox").gameObject;
            show = curTrans.Find("show").gameObject;
            Return = curTrans.Find("Return").gameObject;
            a = clickBox.transform.GetChild(0).GetChild(0).gameObject;
            b = clickBox.transform.GetChild(0).GetChild(1).gameObject;
            c = clickBox.transform.GetChild(1).GetChild(0).gameObject;
            d = clickBox.transform.GetChild(1).GetChild(1).gameObject;
            e = clickBox.transform.GetChild(2).GetChild(0).gameObject;
            f = clickBox.transform.GetChild(2).GetChild(1).gameObject;
            _allJugle = new bool[3];

            Util.AddBtnClick(Return,ReturnEvent);
            for (int i = 0; i < 3; i++)
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
                BtnPlaySound();
                show.SetActive(false);
                _canClick = false;
                if (obj.name == "1")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
                    a.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    b.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    a.SetActive(true);
                    b.SetActive(true);
                    PlaySpine(a, "a1", "a2", "a3", "aj");
                    PlaySpine(b, "b1", "b2", "b3", "bj");
                    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,4,null,
                        ()=>
                        {
                            _rname = "1";
                            Return.SetActive(true);
                            _canRenturn = true;
                        }
                        ));
                }
                if(obj.name == "2")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    c.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    d.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    c.SetActive(true);
                    d.SetActive(true);
                    PlaySpine(c,"c1","c2","c3");
                    PlaySpine(d, "d1", "d2", "d3");
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                        ()=>
                        {
                            _rname = "2";
                            Return.SetActive(true);
                            _canRenturn = true;
                        }
                        ));
                }
                if (obj.name == "3")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    e.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    f.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    e.SetActive(true);
                    f.SetActive(true);
                    PlaySpine(e, "e1");
                    PlaySpine(f, "f1");
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                        () =>
                        {
                            _rname = "3";
                            Return.SetActive(true);
                            _canRenturn = true;
                        }
                        ));
                }
            }
        }
        

        private void ReturnEvent(GameObject obj)
        {
            if (_canRenturn)
            {
                BtnPlaySound();
                _canRenturn = false;
                if (_rname == "1")
                {
                    a.SetActive(false); b.SetActive(false); show.SetActive(true); _canClick = true;
                    _allJugle[0] = true; /*JugleClick();*/Return.SetActive(false);
                }
                if (_rname == "2")
                {
                    c.SetActive(false); d.SetActive(false); show.SetActive(true); _canClick = true;
                    _allJugle[1] = true; /*JugleClick();*/ Return.SetActive(false);
                }
                if (_rname == "3")
                {
                    e.SetActive(false); f.SetActive(false); show.SetActive(true); _canClick = true;
                    _allJugle[2] = true; /*JugleClick();*/ Return.SetActive(false);
                }
            }
        }
        //private void JugleClick()
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (!_allJugle[i])
        //            return;
        //    }
        //    _canClick = false;
        //    Max.SetActive(true);
        //    mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.COMMONVOICE,2,null,
        //        ()=>
        //        { Max.SetActive(false); }
        //        ));
        //}

        private void PlaySpine(GameObject obj, string name1, string name2 = null, string name3 = null, string name4 = null)
        {
            SpineManager.instance.DoAnimation(obj, name1, false,
                () =>
                {
                    if (name2 != null)
                    {
                        SpineManager.instance.DoAnimation(obj, name2, false,
                            () =>
                            {
                                if (name3 != null)
                                {
                                    SpineManager.instance.DoAnimation(obj, name3, false,
                                        () =>
                                        {
                                            if (name4 != null)
                                            {
                                                SpineManager.instance.DoAnimation(obj, name4, false);
                                            }
                                        }
                                        );
                                }
                            });
                    }
                }
                );
        }

        IEnumerator WaitTime(float time ,Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }



        private void GameInit()
        {
            _rname = string.Empty;
            Return.SetActive(false);
            _canRenturn = false;
            xjyq.SetActive(true);
            talkIndex = 1;
            _canClick = false;
            show.SetActive(false);
            clickBox.SetActive(false);
            a.SetActive(false);
            a.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            b.SetActive(false);
            b.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            c.SetActive(false);
            c.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            d.SetActive(false);
            d.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            e.SetActive(false);
            e.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            f.SetActive(false);
            f.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
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
                xjyq.SetActive(false);
                clickBox.SetActive(true);
                show.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                    ()=>
                    {
                        _canClick = true;
                        Max.SetActive(false);
                    }
                    ));

            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
