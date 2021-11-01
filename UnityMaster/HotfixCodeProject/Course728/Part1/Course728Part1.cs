using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public class Course728Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject Bg;
        private BellSprites bellTextures;

        //--------------------------------------------------

        private GameObject _bell;
        private Empty4Raycast[] e4rs;
        private GameObject _ani;
        private bool isPlaying;
        private List<string> _nameList;
        private GameObject _sence1;
        private GameObject _ani2;
        private string z = "z";
        private string zj = "zj";

        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //-------------------------------------------------------
            _sence1 = curTrans.GetGameObject("sence1");
            _bell = curTrans.GetGameObject("bell");
            _ani2 = curTrans.GetGameObject("ani2");
            _ani = _sence1.transform.GetGameObject("ani");
            _nameList = new List<string>();
            e4rs = _sence1.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            z = "z";
            zj = "zj";
            Input.multiTouchEnabled = false;
            _sence1.Show();
            _ani2.Hide();
            isPlaying = true;
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                _nameList.Add(e4rs[i].name);
            }

            for (int i = 0; i < e4rs.Length; i++)
            {
                var child = e4rs[i].transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(child, "kong", false);
                child.Hide();
            }

            _ani2.GetComponent<SkeletonGraphic>().freeze = false;
            SpineManager.instance.DoAnimation(_ani, "j", true);

            SpineManager.instance.DoAnimation(_ani2, "1", true);
        }

        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, 0,
                () => { },
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }

        private void OnClickShow(GameObject obj)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                obj.transform.GetChild(0).gameObject.Show();

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject,
                    z + obj.transform.GetChild(0).name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject,
                            zj + obj.transform.GetChild(0).name, false,
                            () => { });
                    });

                mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, int.Parse(obj.name),
                    () => { SpineManager.instance.DoAnimation(_ani, obj.name, false); },
                    () =>
                    {
                        ReMoveName(obj.name);
                        isPlaying = false;
                    }));
            }
        }

        void ReMoveName(string name)
        {
            for (int i = 0; i < _nameList.Count; i++)
            {
                if (name == _nameList[i])
                {
                    _nameList.RemoveAt(i);
                }

                if (_nameList.Count == 0)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    isPlaying = false;
                    break;
                case 2:
                    _sence1.Hide();
                    _ani2.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0,false);
                    mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, 7,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani2, "2", false,
                                () => { SpineManager.instance.DoAnimation(_ani2, "1", false, () => { }); });
                        },
                        () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    break;
                case 3:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, 8,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani2, "4", false, () =>
                            {
                                SpineManager.instance.DoAnimation(_ani2, "2", false,
                                    () => { SpineManager.instance.DoAnimation(_ani2, "1", false, () => { }); });
                            });
                        },
                        () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    break;
                case 4:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, 9,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani2, "3", false,
                                () =>
                                {
                                    Wait(2f, () => { _ani2.GetComponent<SkeletonGraphic>().freeze = true; });
                                    Wait(4f, () => { _ani2.GetComponent<SkeletonGraphic>().freeze = false; });
                                    SpineManager.instance.DoAnimation(_ani2, "2", false,
                                        () => { SpineManager.instance.DoAnimation(_ani2, "1", false, () => { }); });
                                });
                        },
                        () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    break;
                case 5:
                    _sence1.Show();
                    _ani2.Hide();
                    isPlaying = true;
                    mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, 10,
                        () => { },
                        () => { }));
                    break;
            }

            talkIndex++;
        }


        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForDo(time, method));
        }

        IEnumerator WaitForDo(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }

        #region 说话语音

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex,
            Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = _bell;
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

        #endregion
    }
}