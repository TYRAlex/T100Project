using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9312Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private GameObject _mask;
        private GameObject _max;
        private GameObject _ani;
        private Empty4Raycast[] _click;
        private GameObject _back;

        private bool _canClick;
        private int _flag;
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

            _mask = curTrans.Find("mask").gameObject;
            _max = curTrans.Find("max").gameObject;
            _ani = curTrans.Find("ani").gameObject;
            _click = curTrans.Find("Click").GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _click.Length; i++)
            {
                Util.AddBtnClick(_click[i].gameObject, ClickEvent);
            }
            _back = curTrans.Find("back").gameObject;
            Util.AddBtnClick(_back, BackEvnet);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _flag = 0;
            _canClick = false;

            _mask.Hide();
            _ani.Hide();
            _back.Hide();

            _max.Show();
            _max.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_max, "daiji", true);
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                _mask.Show();
                _ani.Show();
                _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_ani, "animation", false);

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, 
                () => 
                {
                    bell.Hide();
                    _canClick = true;
                }));
            }
            if(talkIndex == 2)
            {
                bell.Show();
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, null));
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
                SoundManager.instance.ShowVoiceBtn(false);

                if ((_flag & 1 << (int.Parse(obj.name) - 1)) == 0)
                    _flag += 1 << (int.Parse(obj.name) - 1);

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
                SpineManager.instance.DoAnimation(_ani, "dianji" + obj.name, false, 
                ()=> 
                {
                    _mask.Hide();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(obj.name) - 1, false);
                    SpineManager.instance.DoAnimation(_max, "daiji" + obj.name, false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_max, "d" + obj.name, false, 
                        ()=> 
                        {
                            _back.Show();
                        });
                    });
                });
            }
        }

        private void BackEvnet(GameObject obj)
        {
            obj.Hide();
            _mask.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            SpineManager.instance.DoAnimation(_max, "daiji", false);
            SpineManager.instance.DoAnimation(_ani, "animation", false);
            mono.StartCoroutine(WaitCoroutine(
            () => 
            { 
                _canClick = true;
                if (_flag == Mathf.Pow(2, curTrans.Find("Click").childCount) - 1)
                    SoundManager.instance.ShowVoiceBtn(true);
            }, 0.3f));
        }
    }
}
