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
    public class Course914Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _ani;
        private GameObject _clickEvent;
        private GameObject[] _clickArray;
        private GameObject _bigAni;
        private GameObject[] _bigAniArray;
        private GameObject _ani1;
        private GameObject _ani2_1;
        private GameObject _ani2_2;
        private GameObject _ani3_1;
        private GameObject _ani3_2;
        private Vector2[] _aniPosArray;
        private GameObject _return;
        private GameObject _returnBigObj1;
        private GameObject _returnBigObj2;
        private string _returnBigAni1;
        private string _returnBigAni2;
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
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            _ani = curTrans.GetGameObject("Card/Ani");
            _clickEvent = curTrans.GetGameObject("ClickEvent");
            _return = curTrans.GetGameObject("ReturnEvent");
            _bigAni = curTrans.GetGameObject("BigAni");
            _ani1 = _bigAni.transform.GetGameObject("Ani1");
            _ani2_1 = _bigAni.transform.GetGameObject("Ani2.1");
            _ani2_2 = _bigAni.transform.GetGameObject("Ani2.2");
            _ani3_1 = _bigAni.transform.GetGameObject("Ani3.1");
            _ani3_2 = _bigAni.transform.GetGameObject("Ani3.2");
            _aniPosArray = new Vector2[5];
            _aniPosArray[0] = _ani1.GetComponent<RectTransform>().anchoredPosition;
            _aniPosArray[1] = _ani2_1.GetComponent<RectTransform>().anchoredPosition;
            _aniPosArray[2] = _ani2_2.GetComponent<RectTransform>().anchoredPosition;
            _aniPosArray[3] = _ani3_1.GetComponent<RectTransform>().anchoredPosition;
            _aniPosArray[4] = _ani3_2.GetComponent<RectTransform>().anchoredPosition;

            Util.AddBtnClick(_return, ReturnAni);
            _clickArray = new GameObject[3];
            for (int i = 0; i < _clickEvent.transform.childCount; i++)
            {
                _clickArray[i] = _clickEvent.transform.GetChild(i).gameObject;
            }
            _bigAniArray = new GameObject[5];
            for (int i = 0; i < _bigAni.transform.childCount; i++)
            {
                _bigAniArray[i] = _bigAni.transform.GetChild(i).gameObject;
            }

            foreach (var child in _clickArray)
            {
                Util.AddBtnClick(child, clickEvent);
            }
            Util.AddBtnClick(btnTest, ReStart);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell.Show();
            _ani.Show();
            SpineManager.instance.DoAnimation(_ani, "animation");
            _clickEvent.Hide();
            _return.Hide();
            _bigAni.Show();

            foreach (var child in _bigAniArray)
            {
                child.Show();
                child.GetComponent<RectTransform>().anchoredPosition = new Vector2(5000, 5000);
            }

            SpineManager.instance.DoAnimation(_ani1, "ck", false);
            SpineManager.instance.DoAnimation(_ani2_1, "bzk", false);
            SpineManager.instance.DoAnimation(_ani2_2, "byk", false);
            SpineManager.instance.DoAnimation(_ani3_1, "azk", false);
            SpineManager.instance.DoAnimation(_ani3_2, "ayk", false);

            _isPlayed1 = false;
            _isPlayed2 = false;
            _isPlayed3 = false;
            _isSaying = false;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            //if (bellTextures.texture.Length <= 0)
            //{
            //    Debug.LogError("@愚蠢！！ 哈哈哈 Bg上的BellSprites 里没有东西----------添加完删掉这个打印");
            //}
            //else
            //{
            //    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            //}

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            //画面中有三种不同工作方式的安全实验助手，它们都能完成两种不同物质的混合实验，但其中一个接收实验材料的杯子的传递方式不一样，我们一起来看看它们的结构运动各有什么特点！
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
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
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
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                () =>
                {
                    _isSaying = true;
                    _clickEvent.Hide();
                    bell.Show();
                    SpineManager.instance.DoAnimation(_ani, "animation");
                },
                () =>
                {
                    _clickEvent.Show();
                    bell.Hide();
                }, 0.5f));
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
            SoundManager.instance.ShowVoiceBtn(false);
            if (obj.name == "Click1")
            {
                _isPlayed1 = true;
                CardAni(1, 0, "animation2", "c2", "c1", "c3");
            }
            if (obj.name == "Click2")
            {
                _isPlayed2 = true;
                CardAni(2, 1, "animation3", "b3", "b1", "b4");
                CardAni(2, "b5", "b2", "b6");
            }
            if (obj.name == "Click3")
            {
                _isPlayed3 = true;
                CardAni(3, 3, "animation4", "a3", "a1", "a6");
                CardAni(4, "a5", "a2", "a4");
            }
        }

        //卡牌动画（curAni放大动画，beforeBigAni大卡牌出现动画，curBigAni大卡牌流程动画，returnBigAni消失动画供返回事件使用）
        void CardAni(int clipIndex, int curObj1, string curAni, string beforeBigAni, string curBigAni, string returnBigAni)
        {
            _returnBigObj1 = _bigAniArray[curObj1];

            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SpineManager.instance.DoAnimation(_ani, curAni, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            },
            () =>
            {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        _ani.Hide();
                        _returnBigObj1.GetComponent<RectTransform>().anchoredPosition = _aniPosArray[curObj1];
                        SpineManager.instance.DoAnimation(_returnBigObj1, beforeBigAni, false);
                    },
                    () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_returnBigObj1, curBigAni, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, clipIndex - 1);
                        },
                        () =>
                        {
                            _returnBigAni1 = returnBigAni;
                            _return.Show();
                        }, 0.2f));
                    }, 1.0f));
            }, 0.667f));
        }

        //卡牌动画（无需语音版，curAni放大动画，beforeBigAni大卡牌出现动画，curBigAni大卡牌流程动画，returnBigAni消失动画供返回事件使用）
        void CardAni(int curObj2, string beforeBigAni, string curBigAni, string returnBigAni)
        {
            _returnBigObj2 = _bigAniArray[curObj2];
            mono.StartCoroutine(AniCoroutine(null,
            () =>
            {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        _returnBigObj2.GetComponent<RectTransform>().anchoredPosition = _aniPosArray[curObj2];
                        SpineManager.instance.DoAnimation(_returnBigObj2, beforeBigAni, false);
                    },
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_returnBigObj2, curBigAni, false);
                        },
                        () =>
                        {
                            _returnBigAni2 = returnBigAni;
                        }, 12.3f));
                    }, 1.0f));
            }, 0.667f));
        }

        //返回动画
        void ReturnAni(GameObject obj)
        {
            _return.Hide();
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                SpineManager.instance.DoAnimation(_returnBigObj1, _returnBigAni1, false);
                if (_returnBigObj2 != null)
                    SpineManager.instance.DoAnimation(_returnBigObj2, _returnBigAni2, false);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _returnBigObj1.GetComponent<RectTransform>().anchoredPosition = new Vector2(5000, 5000);
                    if (_returnBigObj2 != null)
                        _returnBigObj2.GetComponent<RectTransform>().anchoredPosition = new Vector2(5000, 5000);

                    SpineManager.instance.DoAnimation(_ani1, "ck", false);
                    SpineManager.instance.DoAnimation(_ani2_1, "bzk", false);
                    SpineManager.instance.DoAnimation(_ani2_2, "byk", false);
                    SpineManager.instance.DoAnimation(_ani3_1, "azk", false);
                    SpineManager.instance.DoAnimation(_ani3_2, "ayk", false);

                    _ani.Show();
                    SpineManager.instance.DoAnimation(_ani, "animation", false);
                },
                () =>
                {
                    _clickEvent.Show();
                    _returnBigObj2 = null;
                    _returnBigAni2 = null;
                    JudgeSoundBtn();
                }, 0.667f));
            }, 1.0f));
        }

        //判断语音键是否出现
        void JudgeSoundBtn()
        {
            if (_isPlayed1 && _isPlayed2 && _isPlayed3 && !_isSaying)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
    }
}
