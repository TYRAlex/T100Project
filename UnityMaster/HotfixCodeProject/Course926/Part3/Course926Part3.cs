using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course926Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private GameObject Bg2;
        private BellSprites bellTextures;

        private GameObject bell;

        private GameObject _ani;
        private GameObject _bigAni;
        private Transform _click;
        private GameObject[] _clickObj;

        private GameObject btnBack;

        bool isPlaying = false;
        bool isEnd = false;
        int flag;

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

            _ani = curTrans.Find("Ani").gameObject;
            _bigAni = curTrans.Find("BigAni/BigAni").gameObject;
            _click = curTrans.Find("Click");
            _clickObj = new GameObject[_click.childCount];
            for (int i = 0; i < _click.childCount; i++)
            {
                _clickObj[i] = _click.GetChild(i).gameObject;
                Util.AddBtnClick(_clickObj[i], OnClickShow);
            }

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            isEnd = false;

            Bg.Show();
            Bg2.Hide();
            _ani.Show();
            _bigAni.Show();

            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_ani, "xuanze", false);
            SpineManager.instance.DoAnimation(_bigAni, "kong", false);
        }

        void GameStart()
        {
            bell.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { isPlaying = false; }));
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
                isPlaying = true;
                isEnd = true;
                bell.Show();
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, null, ()=> { isPlaying = false; }));
            }
            talkIndex++;
        }

        int clickIndex;
        bool _canStop;

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            SpineManager.instance.DoAnimation(_bigAni, "t" + clickIndex.ToString(), false, () =>
            {
                SpineManager.instance.DoAnimation(_bigAni, "kong", false);
            });
            SpineManager.instance.DoAnimation(_ani, "t" + clickIndex.ToString(), false, () =>
            {
                Bg2.Hide();
                bell.Show();
                obj.SetActive(false); 
                isPlaying = false;
                SpineManager.instance.DoAnimation(_ani, "xuanze", false);
                if (flag == (Mathf.Pow(2, _click.childCount) - 1) && !isEnd)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            });
        }

        private void OnClickShow(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            _canStop = false;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            clickIndex = int.Parse(obj.transform.GetChild(0).name) + 1;

            bell.Hide();
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(_ani, "d" + clickIndex.ToString(), false);

            _bigAni.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_bigAni, "d" + clickIndex.ToString(), false, () =>
            {
                Bg2.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, clickIndex - 1, true);
                SpineManager.instance.DoAnimation(_bigAni, obj.name, true,
                () =>
                {
                    if (_canStop)
                    {
                        SpineManager.instance.DoAnimation(_bigAni, obj.name, false, ()=> { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); });
                    }
                });

                float len = SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, clickIndex) - SpineManager.instance.GetAnimationLength(_bigAni, obj.name) + 2.0f;
                mono.StartCoroutine(WaitCoroutine(() => { _canStop = true; }, len));

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, clickIndex, null, () =>
                {
                    //用于标志是否点击过展示板
                    if ((flag & (1 << int.Parse(obj.transform.GetChild(0).name))) == 0)
                    {
                        flag += 1 << int.Parse(obj.transform.GetChild(0).name);
                    }
                    isPlaying = false;
                    btnBack.SetActive(true);
                }));
            });
        }
    }
}
