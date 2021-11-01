using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course929Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private GameObject _ani;
        private GameObject _bigAni;
        private Transform _click;
        private GameObject[] _clickArray;
        private GameObject _back;
        private bool _canClick;
        private bool _clickTalk;
        private int _count;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            _ani = curTrans.GetGameObject("ani");
            _bigAni = curTrans.GetGameObject("bigAni");
            _back = curTrans.GetGameObject("Back");
            Util.AddBtnClick(_back, BackEvent);

            _click = curTrans.Find("Click");
            _clickArray = new GameObject[_click.childCount];
            for (int i = 0; i < _click.childCount; i++)
            {
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
            _clickTalk = false;
            _click.gameObject.Show();
            for (int i = 0; i < _click.childCount; i++)
            {
                _clickArray[i].Show();
            }
            _back.Hide();
        }

        void GameStart()
        {
            bell.SetActive(true);
            _ani.Show();
            _bigAni.Show();
            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_ani, "kuai", false);
            SpineManager.instance.DoAnimation(_bigAni, "kong", false);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.Hide(); _canClick = true; }));
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
                _canClick = false;
                _clickTalk = true;
                _back.Hide();
                bell.Show();
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, null, null));
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
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
                SoundManager.instance.ShowVoiceBtn(false);
                int index = obj.transform.GetSiblingIndex() + 1;

                if ((_count & (1 << index - 1)) == 0)
                    _count += 1 << index - 1;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, index, null, null));
                SpineManager.instance.DoAnimation(_ani, "dj" + index.ToString(), false,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index - 1, false);
                    SpineManager.instance.DoAnimation(_bigAni, obj.name, false);
                });

                float clipLen = SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, index);
                float aniLen = SpineManager.instance.GetAnimationLength(_bigAni, obj.name);

                mono.StartCoroutine(WaitCoroutine(() => { _back.Show(); }, clipLen > aniLen ? clipLen : aniLen));
            }
        }

        private void BackEvent(GameObject obj)
        {
            obj.Hide();
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            SpineManager.instance.DoAnimation(_bigAni, "kong", false);
            SpineManager.instance.DoAnimation(_ani, "kuai", false, 
            () => 
            { 
                _canClick = true;
                if (_count == Mathf.Pow(2, _click.childCount) - 1 && !_clickTalk)
                    SoundManager.instance.ShowVoiceBtn(true);
                else
                    SoundManager.instance.ShowVoiceBtn(false);
            });
        }
    }
}
