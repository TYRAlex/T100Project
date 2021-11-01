using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course841Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg2;
        private BellSprites bellTextures;

        private GameObject Max;

        private GameObject _ani;
        private GameObject _bigAni;
        private GameObject _finalAni;
        private Transform _click;
        private GameObject[] _clickArray;
        private GameObject _back;

        private bool _canClick;
        private bool _played1;
        private bool _played2;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg2 = curTrans.Find("Bg2").gameObject;

            Max = curTrans.Find("MAX").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _ani = curTrans.GetGameObject("Ani");
            _bigAni = curTrans.GetGameObject("BigAni");
            _finalAni = curTrans.GetGameObject("FinalAni");
            _click = curTrans.Find("Click");
            _clickArray = new GameObject[_click.childCount];
            for (int i = 0; i < _click.childCount; i++)
            {
                _clickArray[i] = _click.GetChild(i).gameObject;
            }

            _back = curTrans.GetGameObject("Back");
            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _canClick = false;
            _played1 = false;
            _played2 = false;

            SpineManager.instance.DoAnimation(_finalAni.transform.GetChild(0).gameObject, "b1", true);
            SpineManager.instance.DoAnimation(_finalAni.transform.GetChild(1).gameObject, "c1", true);
            SpineManager.instance.DoAnimation(_bigAni, "kong", false);
            _ani.Show();
            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_ani, "j3", false);

            Bg2.Hide();
            _finalAni.Hide();
            _back.Hide();

            Util.AddBtnClick(_back, BackEvent);
            for (int i = 0; i < _clickArray.Length; i++)
            {
                Util.AddBtnClick(_clickArray[i], ClickEvent);
            }
        }

        private GameObject tem;
        private int aniIndex;
        private string aniStr;
        private void BackEvent(GameObject obj)
        {
            //_back.Hide();
            //SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            //SpineManager.instance.DoAnimation(_bigAni, aniStr + "d2", false, () => { SpineManager.instance.DoAnimation(_bigAni, "kong", false); });
            //SpineManager.instance.DoAnimation(_ani, "at" + aniIndex.ToString(), false,
            //() =>
            //{
            //    SpineManager.instance.DoAnimation(_ani, "j3", false,
            //    () =>
            //    {
            //        mono.StartCoroutine(WaitCoroutine(() => { _canClick = true; }, 0.3f));
            //        if (_played1 && _played2)
            //            SoundManager.instance.ShowVoiceBtn(true);
            //        else
            //            SoundManager.instance.ShowVoiceBtn(false);
            //    });
            //});
            _back.Hide();
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            SpineManager.instance.DoAnimation(_ani, aniStr + "d2", false, 
            () => 
            {
                    SpineManager.instance.DoAnimation(_ani, "j3", false,
                    () =>
                    {
                        mono.StartCoroutine(WaitCoroutine(() => { _canClick = true; }, 0.3f));
                        if (_played1 && _played2)
                            SoundManager.instance.ShowVoiceBtn(true);
                        else
                            SoundManager.instance.ShowVoiceBtn(false);
                    });
            });
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
                SoundManager.instance.ShowVoiceBtn(false);

                tem = obj;
                aniIndex = int.Parse(obj.name);
                if (obj.name == "1")
                { 
                    aniStr = "c";
                    _played1 = true;
                }
                else
                {
                    aniStr = "b";
                    _played2 = true;
                }

                PlayAni(aniIndex);
            }
        }

        void PlayAni(int index)
        {
            //SpineManager.instance.DoAnimation(_ani, "ad" + index.ToString(), false, ()=> { SpineManager.instance.DoAnimation(_ani, "kong", false); });
            //SpineManager.instance.DoAnimation(_bigAni, aniStr + "d1", false,
            //() =>
            //{
            //    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, aniIndex,
            //    () =>
            //    {
            //        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            //        SpineManager.instance.DoAnimation(_bigAni, aniStr + "1", true);
            //    },
            //    () =>
            //    {
            //        _back.Show();
            //    }));
            //});

            SpineManager.instance.DoAnimation(_ani, "ad" + index.ToString(), false, 
            () => 
            {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, aniIndex,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                        SpineManager.instance.DoAnimation(_ani, aniStr + "1", true);
                    },
                    () =>
                    {
                        _back.Show();
                    }));
            });
        }

        void GameStart()
        {
            Max.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); _canClick = true; }));

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

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _canClick = false;
                Bg2.Show();
                _ani.Hide();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3,
                () =>
                {
                    _finalAni.Show();
                    _finalAni.transform.GetGameObject("0").GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    _finalAni.transform.GetGameObject("1").GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_finalAni.transform.GetGameObject("0"), "c1", true);
                    SpineManager.instance.DoAnimation(_finalAni.transform.GetGameObject("1"), "b1", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                }, null));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
