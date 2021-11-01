using ILRuntime.Runtime;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course842Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private GameObject clickBox;
        bool isPlaying = false;
        bool _canClick;
        bool[] _allclick;
        private GameObject speedctr;
        private GameObject pbj;
        private float _speedIndex;
        private bool _canslow;
        private bool _canfast;
        private GameObject _mask;
        private float[] speed;
        private float _lastSpeedIndex;


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
            _mask = curTrans.Find("_mask").gameObject;
            speedctr = curTrans.Find("speed").gameObject;
            pbj = curTrans.Find("pbj").gameObject;
            speed = new float[11] {0f,0.25f,0.5f,0.68f,0.85f,1,1.3f,1.6f,1.9f,2.2f,2.5f };

            for (int i = 0; i < curTrans.Find("clickBox").childCount; i++)
            {
                Util.AddBtnClick(curTrans.Find("clickBox").GetChild(i).gameObject, ClickEvent);
            }
            _allclick = new bool[5];

            for (int i = 0; i < speedctr.transform.childCount; i++)
            {
                Util.AddBtnClick(speedctr.transform.GetChild(i).gameObject, SpeedClick);
            }

            GameInit();
            GameStart();
        }


        private void SpeedClick(GameObject obj)
        {
            speedctr.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            BtnPlaySound();
            if (obj.name == "slow" && _canslow)
            {
                SpineManager.instance.DoAnimation(speedctr,"kz-1",false);
                _lastSpeedIndex = _speedIndex;
                _speedIndex -= 1;
                pbj.GetComponent<SkeletonGraphic>().timeScale = speed[(int)_speedIndex];
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,(int)_speedIndex,true);
                _canslow = false;
                JugleZero();
                JugleSpeed();

            }
            if (obj.name == "fast" && _canfast)
            {
                SpineManager.instance.DoAnimation(speedctr, "kz+1", false);
                _lastSpeedIndex = _speedIndex;
                _speedIndex += 1;
                pbj.GetComponent<SkeletonGraphic>().timeScale = speed[(int)_speedIndex];
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, (int)_speedIndex, true);
                _canfast = false;
                JugleSpeed();
                JugleZero();
            }

        }

        private void JugleSpeed()
        {
            if (_speedIndex < 10)
            { _canfast = true;
                speedctr.transform.GetChild(1).gameObject.SetActive(true);
            }
            if(_speedIndex == 10)
            {
                SpineManager.instance.DoAnimation(speedctr, "kz+2", false);
                speedctr.transform.GetChild(1).gameObject.SetActive(false);
                    }
            if (_speedIndex > 0)
            { _canslow = true;
                speedctr.transform.GetChild(0).gameObject.SetActive(true);
            }
            if(_speedIndex == 0)
            {
                SpineManager.instance.DoAnimation(speedctr, "kz-2", false);
                speedctr.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        private void JugleZero()
        {
            if(_speedIndex==0)
            {
                SpineManager.instance.DoAnimation(pbj, "animation3", true);
            }
            if (_speedIndex == 1 &&_lastSpeedIndex==0)
            {
                SpineManager.instance.DoAnimation(pbj, "animation2", true);
            }
        }

        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,14,false);
                _canClick = false;
                string name = JugleClick(obj);
                if (name == "g1")
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("show/kb").gameObject, "kb1", false);
                    SpineManager.instance.DoAnimation(curTrans.Find("show/ybp").gameObject, "z1", false);
                    SpineManager.instance.DoAnimation(pbj, name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(pbj,"animation",false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(curTrans.Find("show/kb").gameObject, "kb3", false,
                                    () =>
                                    {
                                        SpineManager.instance.DoAnimation(curTrans.Find("show/ybp").gameObject, "z1j", false);
                                        JugleAllClick();
                                        _canClick = true;
                                    }
                                    );
                            })); ;
                    }
                    );
                }
                else if (name == "g3")
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("show/ka").gameObject, "ka1", false);
                    SpineManager.instance.DoAnimation(curTrans.Find("show/dj").gameObject,"z4",false);
                    SpineManager.instance.DoAnimation(pbj, name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(pbj, "animation", false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(curTrans.Find("show/ka").gameObject, "ka3", false,
                                    () =>
                                    {
                                        SpineManager.instance.DoAnimation(curTrans.Find("show/dj").gameObject, "z4j", false);
                                        JugleAllClick();
                                        _canClick = true;
                                    }
                                    );
                            })); ;
                    }
                    );
                }
                else if (name == "g2")
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("show/fs").gameObject, "z2", false);
                    SpineManager.instance.DoAnimation(pbj, name, false,
                        ()=>
                        { SpineManager.instance.DoAnimation(pbj, "animation", false); }
                        );
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(curTrans.Find("show/fs").gameObject, "z2j", false);
                            _canClick = true;
                            JugleAllClick();
                        }

                        ));
                }

                else if (name == "g4")
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("show/pd").gameObject, "z5", false);
                    SpineManager.instance.DoAnimation(pbj, name, false,
                        () =>
                        { SpineManager.instance.DoAnimation(pbj, "animation", false); });
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(curTrans.Find("show/pd").gameObject, "z5j", false);
                            _canClick = true;
                            JugleAllClick();
                        }

                        ));
                }
                else if (name == "g5")
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("show/jj").gameObject, "z3", false);
                    SpineManager.instance.DoAnimation(pbj, name, false,
                        () =>
                        { SpineManager.instance.DoAnimation(pbj, "animation", false); });
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(curTrans.Find("show/jj").gameObject, "z3j", false);
                            _canClick = true;
                            JugleAllClick();
                        }

                        ));
                }
            }
        }

        private string JugleClick(GameObject obj)
        {
            string name = null ;
            switch (obj.name)
            {
                case "ybp":
                    name = "g1";
                    _allclick[0] = true;
                    break;
                case "fs":
                    name = "g2";
                    _allclick[1] = true;
                    break;
                case "dj":
                    name = "g3";
                    _allclick[2] = true;
                    break;
                case "pd":
                    name = "g4";
                    _allclick[3] = true;
                    break;
                case "jj":
                    name = "g5";
                    _allclick[4] = true;
                    break;
            }
            return name;
        }

        private void JugleAllClick()
        {
            for (int i = 0; i < _allclick.Length; i++)
            {
                if (!_allclick[i])
                { return; }
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void GameInit()
        {
            pbj.SetActive(true);
            _mask.SetActive(false);
            talkIndex = 1;
            speedctr.transform.Find("fast").gameObject.SetActive(true);
            speedctr.transform.Find("slow").gameObject.SetActive(true);
            speedctr.SetActive(false);
            _speedIndex = 5;
            pbj.GetComponent<SkeletonGraphic>().timeScale = speed[(int)_speedIndex];
            pbj.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _canslow = true;
            _canfast = true;
            for (int i = 0; i < curTrans.Find("show").childCount; i++)
            {
                curTrans.Find("show").GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
            SpineManager.instance.DoAnimation(pbj, "animation", false);
            clickBox.SetActive(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () => { isPlaying = false; _canClick = true; }));

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
                for (int i = 0; i < curTrans.Find("show").childCount; i++)
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("show").GetChild(i).gameObject, "kong", false);
                }
                _mask.SetActive(true);
                Max.SetActive(false);
                curTrans.Find("clickBox").gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(curTrans.Find("pbj").gameObject, "animation2", true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,7,null,
                    ()=>
                    { _mask.SetActive(false); }
                    ));
                speedctr.SetActive(true);
                speedctr.transform.Find("fast").gameObject.SetActive(true);
                speedctr.transform.Find("slow").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(speedctr, "kz1", false);
                SoundManager.instance.ShowVoiceBtn(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,5,true);
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
