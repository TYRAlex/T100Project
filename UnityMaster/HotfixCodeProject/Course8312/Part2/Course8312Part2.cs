using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course8312Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private GameObject Bg2;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _cardAni;
        private GameObject _clickEvent;
        private GameObject[] _clickEventArray;
        private GameObject _bigAni;
        private GameObject[] _bigAniArray;
        private int _curAniIndex;
        private GameObject _returnClick;
        private GameObject _3DObj;
        private GameObject _mask;
        private bool _videoPlayed;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            _3DObj = curTrans.GetGameObject("3DObj");
            _mask = curTrans.GetGameObject("mask");
            _cardAni = curTrans.GetGameObject("Card/CardAni");
            _clickEvent = curTrans.GetGameObject("ClickEvent");
            _clickEventArray = new GameObject[3];
            for (int i = 0; i < _clickEvent.transform.childCount; i++)
            {
                _clickEventArray[i] = _clickEvent.transform.GetChild(i).gameObject;
            }
            _bigAni = curTrans.GetGameObject("BigAni");
            _bigAniArray = new GameObject[3];
            for (int i = 0; i < _bigAni.transform.childCount; i++)
            {
                _bigAniArray[i] = _bigAni.transform.GetChild(i).gameObject;
            }
            _returnClick = curTrans.GetGameObject("ReturnClick");
            //_videoPlayer.url = "Assets/HotFixPackage/" + HotfixManager.instance.curShowPackage.Name + "/Videos/video.mp4";
            Util.AddBtnClick(_returnClick, ReturnEvent);
            Util.AddBtnClick(btnTest, ReStart);
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
            Bg2 = curTrans.Find("Bg2").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Bg2.Hide();
            bell.Show();
            _cardAni.Show();
            _bigAni.Show();
            _clickEvent.Hide();
            _returnClick.Hide();
            _3DObj.Hide();
            _mask.Hide();
            _videoPlayed = false;
            for (int i = 0; i < _bigAniArray.Length; i++)
            {
                _bigAniArray[i].Hide();
            }

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Debug.LogError("Restart执行");
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
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                SpineManager.instance.DoAnimation(_cardAni, "j");
            },
            () =>
            {
                bell.Hide();
                _clickEvent.Show();
                foreach (var card in _clickEventArray)
                {
                    Util.AddBtnClick(card, ClickEvent);
                }
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
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                () =>
                {
                    _3DObj.Show();
                    _mask.Show();
                    _videoPlayed = true;
                },
                () =>
                {
                    _returnClick.Show();
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

        //卡牌点击事件
        void ClickEvent(GameObject obj)
        {
            int objIndex = Convert.ToInt32(obj.name);
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(false);
                _clickEvent.Hide();
                if (objIndex == 0)
                    SpineManager.instance.DoAnimation(_cardAni, "a1", false);
                if (objIndex == 1)
                    SpineManager.instance.DoAnimation(_cardAni, "b1", false);
                if (objIndex == 2)
                    SpineManager.instance.DoAnimation(_cardAni, "c1", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _cardAni.Hide();
                    Bg2.Show();
                    _bigAniArray[objIndex].Show();
                    _curAniIndex = objIndex;
                    Debug.Log(_bigAniArray[objIndex]);
                    SpineManager.instance.DoAnimation(_bigAniArray[objIndex], "animation2", false);
                },
                () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, objIndex + 1,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bigAniArray[objIndex], "animation");
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, objIndex, true);
                    },
                    () =>
                    {
                        _returnClick.Show();
                    }, 0));
                },0.667f));
            },
            0.667f));
        }

        //返回事件
        void ReturnEvent(GameObject obj)
        {
            if (_3DObj.activeSelf)
            {
                _3DObj.Hide();
                _mask.Hide();
                _returnClick.Hide();
            }
            else
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    _returnClick.Hide();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SpineManager.instance.DoAnimation(_bigAniArray[_curAniIndex], "animation3", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                },
                () =>
                {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        _cardAni.Show();
                        Bg2.Hide();
                        if (_curAniIndex == 0)
                            SpineManager.instance.DoAnimation(_cardAni, "a2", false);
                        if (_curAniIndex == 1)
                            SpineManager.instance.DoAnimation(_cardAni, "b2", false);
                        if (_curAniIndex == 2)
                            SpineManager.instance.DoAnimation(_cardAni, "c2", false);
                    },
                    () =>
                    {
                        if (!_videoPlayed)
                            SoundManager.instance.ShowVoiceBtn(true);
                        SpineManager.instance.DoAnimation(_cardAni, "j", true);
                        _bigAniArray[_curAniIndex].Hide();
                        _clickEvent.Show();
                    }, 0.667f));
                }, 0.667f));
            }
        }
    }
}
