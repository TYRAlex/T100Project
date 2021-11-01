using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course8410Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject bell;

        private Transform _ani;
        private GameObject[] _aniArray;
        private Transform _click;
        private GameObject[] _clickArray;

        private bool _canClick;
        private int _count;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            _ani = curTrans.Find("Ani");
            _click = curTrans.Find("Click");

            _aniArray = new GameObject[_ani.childCount];
            _clickArray = new GameObject[_ani.childCount];
            for (int i = 0; i < _ani.childCount; i++)
            {
                _aniArray[i] = _ani.GetChild(i).gameObject;
                _clickArray[i] = _click.GetChild(i).gameObject;

                Util.AddBtnClick(_clickArray[i], ClickEvent);
            }

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _count = 0;
            _canClick = false;

            for (int i = 0; i < _aniArray.Length; i++)
            {
                _clickArray[i].Show();

                GameObject o = _aniArray[i];
                SpineManager.instance.DoAnimation(o, "kong", false, () => { SpineManager.instance.DoAnimation(o, "dj1", false); });
                SpineManager.instance.DoAnimation(o.transform.GetChild(0).gameObject, "kong", false);
            }
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); _canClick = true; }));

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
                speaker = bell;
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 2)
            {
                bell.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, 
                () => 
                {
                    for (int i = 0; i < _aniArray.Length; i++)
                    {
                        GameObject o = _aniArray[i];
                        SpineManager.instance.DoAnimation(o, "kong", false, ()=> { SpineManager.instance.DoAnimation(o, "dj1", false); });
                    }

                    mono.StartCoroutine(WaitCoroutine(
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SpineManager.instance.DoAnimation(_aniArray[0], _aniArray[0].name, false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            SpineManager.instance.DoAnimation(_aniArray[1], _aniArray[1].name, false,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                                SpineManager.instance.DoAnimation(_aniArray[2], _aniArray[2].name, false);
                            });
                        });
                    }, 0.5f));

                    mono.StartCoroutine(WaitCoroutine(
                    () => 
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        SpineManager.instance.DoAnimation(_aniArray[0].transform.GetChild(0).gameObject, _aniArray[0].name, false, 
                        () => 
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                            SpineManager.instance.DoAnimation(_aniArray[1].transform.GetChild(0).gameObject, _aniArray[1].name, false, 
                            () => 
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                                SpineManager.instance.DoAnimation(_aniArray[2].transform.GetChild(0).gameObject, _aniArray[2].name, false);
                            });
                        });
                    }, 7.5f));
                }));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                obj.Hide();
                _canClick = false;
                int index = int.Parse(obj.name);

                if ((_count & 1 << index) == 0)
                    _count += 1 << index;

                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, false);
                SpineManager.instance.DoAnimation(_aniArray[index], _aniArray[index].name, false, 
                ()=> 
                { 
                    _canClick = true;
                    if (_count == Mathf.Pow(2, _aniArray.Length) - 1)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }
    }
}
