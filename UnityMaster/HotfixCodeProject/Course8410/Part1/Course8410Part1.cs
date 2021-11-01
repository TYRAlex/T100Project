using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course8410Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private GameObject Bg2;
        private BellSprites bellTextures;

        private GameObject bell;

        private GameObject _max;
        private Transform _ani;
        private GameObject[] _aniArray;
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
            Bg2 = curTrans.Find("Bg2").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;
            _max = curTrans.Find("Max").gameObject;
            _ani = curTrans.Find("Ani");

            _aniArray = new GameObject[_ani.childCount];
            _clickArray = new GameObject[_ani.childCount];
            for (int i = 0; i < _ani.childCount; i++)
            {
                _aniArray[i] = _ani.GetChild(i).gameObject;
                _clickArray[i] = _ani.GetChild(i).GetChild(0).gameObject;

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

            Bg.Show();
            Bg2.Hide();
            _max.Show();

            _ani.transform.GetComponent<CanvasGroup>().alpha = 0;
            for (int i = 0; i < _aniArray.Length; i++)
            {
                GameObject o = _aniArray[i];
                SpineManager.instance.DoAnimation(o, "kong", false, 
                () => 
                {
                    SpineManager.instance.DoAnimation(o, "dj" + o.name, false);
                });
                _clickArray[i].Hide();
            }
        }

        void GameStart()
        {
            bell.SetActive(true);
            SpineManager.instance.DoAnimation(_max, "animation", true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true);}));
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, 
                ()=> 
                {
                    _max.Hide();
                    _ani.transform.GetComponent<CanvasGroup>().alpha = 1;
                    for (int i = 0; i < _clickArray.Length; i++)
                    {
                        _clickArray[i].Show();
                    }
                    Bg2.Show();
                }, 
                () => 
                {
                    bell.Hide();
                    _canClick = true;
                }));
            }
            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2,
                () =>
                {
                    bell.Show();
                }, null));
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
                int index = int.Parse(obj.transform.parent.name);
                if ((_count & (1 << index - 1)) == 0)
                    _count += 1 << index - 1;

                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(_aniArray[index - 1], index.ToString(), false, 
                () => 
                {
                    SpineManager.instance.DoAnimation(_aniArray[index - 1], (index + 4).ToString(), false);
                    _canClick = true;
                    if (_count == Mathf.Pow(2, _clickArray.Length) - 1)
                        SoundManager.instance.ShowVoiceBtn(true);
                });
            }
        }
    }
}
