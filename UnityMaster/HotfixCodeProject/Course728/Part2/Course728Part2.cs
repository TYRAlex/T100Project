using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILRuntime.Runtime;
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

    public class Course728Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject Bg;
        private BellSprites bellTextures;

        //--------------------------------------------------

        private GameObject _bell;
        private GameObject _ani;
        private GameObject _ani2;
        private Empty4Raycast[] e4rs;
        private bool isPlaying;
        private GameObject _btn;
        private int index;
        private GameObject _spines;

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
            _ani = curTrans.GetGameObject("ani");
            _ani2 = curTrans.GetGameObject("ani2");
            SoundManager.instance.ShowVoiceBtn(false);
            _btn = curTrans.GetGameObject("btn");
            _bell = curTrans.GetGameObject("bell");
            _spines = curTrans.GetGameObject("spines");
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            e4rs = _spines.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            for (int i = 0; i < _spines.transform.childCount; i++)
            {
                var child = _spines.transform.GetChild(i).gameObject;
                if (child.name == "a")
                {
                    child.transform.SetSiblingIndex(0);
                }

                if (child.name == "b")
                {
                    child.transform.SetSiblingIndex(1);
                }

                if (child.name == "c")
                {
                    child.transform.SetSiblingIndex(2);
                }

                if (child.name == "d")
                {
                    child.transform.SetSiblingIndex(3);
                }
            }

            Util.AddBtnClick(_btn, OnClickBtn);

            GameInit();
            GameStart();
        }

        private void OnClickBtn(GameObject obj)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                _btn.Hide();
                _bell.Show();
                SpineManager.instance.DoAnimation(_ani2, "kong", false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SpineManager.instance.DoAnimation(temp.transform.parent.gameObject, "h" + temp.name, false,
                () =>
                {
                    isPlaying = false;
                    temp.transform.parent.SetSiblingIndex(index);
                });
            }
        }

        private GameObject temp = null;

        private void OnClickShow(GameObject obj)
        {
            if (!isPlaying)
            {
                temp = obj;
                isPlaying = true;
                index = obj.transform.parent.transform.GetSiblingIndex();
                obj.transform.parent.transform.SetAsLastSibling();

                mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, int.Parse(obj.name),
                () =>
                {
                    SpineManager.instance.DoAnimation(temp.transform.parent.gameObject, "d" + temp.name, false, () =>
                    {
                        if (obj.name == "3")
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(obj.name), true);
                        }
                        else if (obj.name == "4")
                        {
                            Wait(0.4f,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(obj.name), true);
                            });
                        }
                        else
                        {
                            Wait(0.3f,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(obj.name), true);
                            });
                        }


                        SpineManager.instance.DoAnimation(_ani2, obj.transform.parent.name, true);
                    });
                }, () =>
                {
                    isPlaying = false;
                    _btn.Show();
                }));
            }
        }

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            _ani.Show();
            _ani2.Show();
            _btn.Hide();
            isPlaying = false;
            _bell.Show();
            talkIndex = 1;
            SpineManager.instance.DoAnimation(_ani2, "kong", false);
            SpineManager.instance.DoAnimation(_ani, "kong", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani, "1", false,
                        () => { });
                });

            for (int i = 0; i < _spines.transform.childCount; i++)
            {
                var child = _spines.transform.GetChild(i).gameObject;
                if (child.name == "a")
                {
                    child.transform.SetSiblingIndex(0);
                }

                if (child.name == "b")
                {
                    child.transform.SetSiblingIndex(1);
                }

                if (child.name == "c")
                {
                    child.transform.SetSiblingIndex(2);
                }

                if (child.name == "d")
                {
                    child.transform.SetSiblingIndex(3);
                }
            }

            for (int i = 0; i < _spines.transform.childCount; i++)
            {
                var child = _spines.transform.GetChild(i).gameObject;
                SpineManager.instance.DoAnimation(child, "kong", false);
            }
            for (int i = 0; i < e4rs.Length; i++)
            {
                e4rs[i].raycastTarget = false;
            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(_bell, SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    Wait(1.5f, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SpineManager.instance.DoAnimation(_ani, "2", false,
                            () => { });
                    });
                },
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitDoSomthing(time, method));
        }

        IEnumerator WaitDoSomthing(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    _ani.Hide();
                    for (int i = 0; i < _spines.transform.childCount; i++)
                    {
                        var child = _spines.transform.GetChild(i).gameObject;
                        SpineManager.instance.DoAnimation(child, "j" + child.transform.GetChild(0).name, false);
                    }
                    for (int i = 0; i < e4rs.Length; i++)
                    {
                        e4rs[i].raycastTarget = true;
                    }

                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:
                    break;
                case 5:
                    break;
            }

            talkIndex++;
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