using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course9112Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject _cardAni;
        private GameObject _bigCardAni;
        private GameObject _clickEvent;
        private GameObject[] _clickArray;
        private GameObject _return;
        private bool _isPlayed1;
        private bool _isPlayed2;
        private bool _isPlayed3;
        private bool _isSaying;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            _cardAni = curTrans.GetGameObject("CardPar/CardAni");
            _bigCardAni = curTrans.GetGameObject("BigCardAni");
            _clickEvent = curTrans.GetGameObject("ClickEvent");
            _return = curTrans.GetGameObject("ReturnEvent");
            Util.AddBtnClick(_return, ReturnAni);
            _clickArray = new GameObject[3];
            for (int i = 0; i < _clickEvent.transform.childCount; i++)
            {
                _clickArray[i] = _clickEvent.transform.GetChild(i).gameObject;
            }
            foreach (var child in _clickArray)
            {
                Util.AddBtnClick(child, clickEvent);
            }
            Util.AddBtnClick(btnTest, ReStart);

#if !UNITY_EDITOR
            btnTest.SetActive(false);
#endif
            ReStart(btnTest);
            btnTest.SetActive(false);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _isPlayed1 = false;
            _isPlayed2 = false;
            _isPlayed3 = false;
            _isSaying = false;

            bell.Show();
            _cardAni.Show();
            SpineManager.instance.DoAnimation(_cardAni, "animation");
            _bigCardAni.Hide();
            _clickEvent.Hide();
            _return.Hide();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            //提取青蒿素用的是低温萃取的方法。在化学实验中，还有非常多有趣的实验方法。
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                bell.Hide();
                _clickEvent.Show();
            }, 0.5f));
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //自定义动画协程
        IEnumerator AniCoroutine(Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            if (method_1 != null)
            {
                method_1();
            }

            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _isSaying = true;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                () =>
                {
                    bell.Show();
                    _clickEvent.Hide();
                    _return.Hide();
                }, 
                () =>
                {
                    bell.Hide();
                    _clickEvent.Show();
                }, 0));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        void clickEvent(GameObject obj)
        {
            _clickEvent.Hide();
            if (obj.name == "Click1")
            {
                _isPlayed1 = true;
                CardAni(1, "a", "a", "a2", "a3");
            }
            if (obj.name == "Click2")
            {
                _isPlayed2 = true;
                CardAni(2, "b", "b", "b2", "b3");
            }
            if (obj.name == "Click3")
            {
                _isPlayed3 = true;
                CardAni(3, "c", "c", "c2", "c3");
            }
        }

        //卡牌动画
        void CardAni(int clipIndex, string curAni, string beforeBigAni, string curBigAni, string afterBigAni)
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                SpineManager.instance.DoAnimation(_cardAni, curAni, false);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _cardAni.Hide();
                    _bigCardAni.Show();
                    SpineManager.instance.DoAnimation(_bigCardAni, beforeBigAni, false);
                },
                () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, clipIndex);
                        SpineManager.instance.DoAnimation(_bigCardAni, curBigAni, false);
                    },
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bigCardAni, afterBigAni, false);
                        _return.Show();
                    }, 0));
                }, 0.1f));
            }, 0.7f));
        }

        //返回动画
        void ReturnAni(GameObject obj)
        {
            _return.Hide();
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                SpineManager.instance.DoAnimation(_bigCardAni, "animation", false);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _cardAni.Show();
                    _bigCardAni.Hide();
                    SpineManager.instance.DoAnimation(_cardAni, "animation", false);
                },
                () =>
                {
                    _clickEvent.Show();
                    if (_isPlayed1 && _isPlayed2 && _isPlayed3 && !_isSaying)
                        SoundManager.instance.ShowVoiceBtn(true);
                    else
                        SoundManager.instance.ShowVoiceBtn(false);
                }, 0.2f));
            }, 0.2f));
        }
    }
}
