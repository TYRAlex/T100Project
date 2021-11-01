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
    public class Course917Part1
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
        private GameObject _return;
        private string _returnBigAni;       //消失的大动画
        private string _returnSmallAni;     //缩小的小动画
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
            _ani = curTrans.GetGameObject("AniPar/Ani");
            _clickEvent = curTrans.GetGameObject("ClickEvent");
            _return = curTrans.GetGameObject("ReturnEvent");
            _bigAni = curTrans.GetGameObject("BigAniPar/BigAni");
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

            btnTest.SetActive(false);
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
            SpineManager.instance.DoAnimation(_bigAni, "animation");
            _bigAni.Hide();

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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            //画面中有三种不同工作方式的擦地机器人，我们一起来看看它们的擦地方式各有什么特点
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                bell.Hide();
                _clickEvent.Show();
            }, 0));
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
                _isSaying = true;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, 
                () =>
                {
                    _clickEvent.Hide();
                    bell.Show();
                    SpineManager.instance.DoAnimation(_ani, "animation");
                },
                () =>
                {
                    _clickEvent.Show();
                    bell.Hide();
                    SpineManager.instance.DoAnimation(_ani, "animation");
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
                CardAni(1, "animation4", "a3", "a2", "a4", "animation7");
            }
            if (obj.name == "Click2")
            {
                _isPlayed2 = true;
                CardAni(2, "animation2", "b3", "b2", "b4", "animation5");
            }
            if (obj.name == "Click3")
            {
                _isPlayed3 = true;
                CardAni(3, "animation3", "c3", "c2", "c4", "animation6");
            }
        }

        //卡牌动画（curAni放大动画，beforeBigAni大卡牌出现动画，curBigAni大卡牌流程动画，returnBigAni消失动画供返回事件使用）
        void CardAni(int clipIndex, string curAni, string beforeBigAni, string curBigAni, string returnBigAni, string returnSmallAni)
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SpineManager.instance.DoAnimation(_ani, curAni, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _ani.Hide();
                    _bigAni.Show();
                    SpineManager.instance.DoAnimation(_bigAni, beforeBigAni, false);
                },
                () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bigAni, curBigAni, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, clipIndex);
                    },
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, clipIndex);
                        SpineManager.instance.DoAnimation(_bigAni, curBigAni, false, 
                        ()=> 
                        {
                            _returnBigAni = returnBigAni;
                            _returnSmallAni = returnSmallAni;
                            _return.Show();
                        });
                    }, 0));
                }, 1.0f));
            }, 0.8f));
        }

        //返回动画
        void ReturnAni(GameObject obj)
        {
            _return.Hide();
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SpineManager.instance.DoAnimation(_bigAni, _returnBigAni, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _ani.Show();
                    _bigAni.Hide();
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_ani, _returnSmallAni, false);
                    },
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_ani, "animation", false);
                    }, 0.667f));
                },
                () =>
                {
                    _clickEvent.Show();
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
