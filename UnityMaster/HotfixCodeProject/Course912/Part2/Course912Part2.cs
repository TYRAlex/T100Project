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
    public class Course912Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject max;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _cardAni;
        private GameObject _bigCardAni;
        private GameObject _clickEvent;
        private GameObject[] _clickArray;
        private GameObject _return;
        private string _returnBigAni;

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
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            max = curTrans.Find("max").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            max.Hide();
            _cardAni.Show();
            SpineManager.instance.DoAnimation(_cardAni, "animation");
            _bigCardAni.Hide();
            _clickEvent.Hide();
            _return.Hide();
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
            //提取青蒿素用的是低温萃取的方法。在化学实验中，还有非常多有趣的实验方法。
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                max.Hide();
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
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
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
            if(obj.name == "Click1")
            {
                CardAni(1, "a1", "a3", "a1", "a2", "a4");
            }
            if (obj.name == "Click2")
            {
                CardAni(2, "a2", "b3", "b1", "b2", "b4");
            }
            if (obj.name == "Click3")
            {
                CardAni(3, "a3", "c3", "c1", "c2", "c4");
            }
        }

        //卡牌动画
        void CardAni(int clipIndex, string curAni, string beforeBigAni, string curBigAni, string afterBigAni, string returnBigAni)
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SpineManager.instance.DoAnimation(_cardAni, curAni, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
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
                        SpineManager.instance.DoAnimation(_bigCardAni, curBigAni, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, clipIndex);
                    },
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bigCardAni, afterBigAni, false);
                        _returnBigAni = returnBigAni;
                        _return.Show();
                    }, 0));
                }, 0.667f));
            }, 0.8f));
        }

        //返回动画
        void ReturnAni(GameObject obj)
        {
            _return.Hide();
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SpineManager.instance.DoAnimation(_bigCardAni, _returnBigAni, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
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
                }, 0.5f));
            },  0.667f));
        }
    }
}
