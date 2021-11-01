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
    public class Course838Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject _car;
        private GameObject _carAni;
        private Vector2 _carPos;
        private GameObject _fourCar;
        private GameObject[] _carArray;
        private GameObject[] _carAniArray;
        private bool _canClick;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);

            _car = curTrans.GetGameObject("Car");
            _carAni = _car.transform.GetGameObject("carAni");
            _carPos = new Vector2(0, 0);
            _fourCar = curTrans.GetGameObject("FourCar");
            _carArray = new GameObject[4];
            _carAniArray = new GameObject[4];
            for (int i = 0; i < _fourCar.transform.childCount; i++)
            {
                _carArray[i] = _fourCar.transform.GetChild(i).gameObject;
                Util.AddBtnClick(_carArray[i], ClickCar);
                _carAniArray[i] = _carArray[i].transform.GetChild(0).gameObject;
            }
            ReStart(btnTest);
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

            for (int i = 0; i < _carAniArray.Length; i++)
            {
                _carAniArray[i].SetActive(true);
            }
            SpineManager.instance.DoAnimation(_carAniArray[0], "a");
            SpineManager.instance.DoAnimation(_carAniArray[1], "b");
            SpineManager.instance.DoAnimation(_carAniArray[2], "c");
            SpineManager.instance.DoAnimation(_carAniArray[3], "d");
            _car.SetActive(true);
            _car.GetComponent<RectTransform>().anchoredPosition = _carPos;
            SpineManager.instance.DoAnimation(_carAni, "animation7d", false);
            _fourCar.SetActive(false);
            _canClick = false;

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
            //画面中是一辆拥有两个颜色传感器的小车，它是怎样巡线的呢，我们一起来看看！
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                SpineManager.instance.DoAnimation(_carAni, "animation7d");
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(_carAni, "animation7", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    bell.SetActive(false);
                },
                () =>
                {
                    bell.SetActive(true);
                    //你们发现了吗，小车是以夹住黑线的方式行进的，它是如何做到的呢，一起来分析看看！
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }, 0));
                }, 10.0f));
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
                _car.GetComponent<RectTransform>().anchoredPosition = new Vector2(5000, 5000);
                SpineManager.instance.DoAnimation(_carAni, "animation7d");
                _fourCar.SetActive(true);
                //画面中的四种情况是小车在巡线过程中会出现的，请说出每种情况小车的状态分别是什么？”
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null,
                () =>
                {
                    bell.SetActive(false);
                    _canClick = true;
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

        //小车点击事件
        void ClickCar(GameObject obj)
        {
            if(_canClick)
            {
                if (obj.name == _carArray[0].name)
                {
                    //当小车的两个颜色传感器分别位于黑线的两边，小车直行
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            CarPlayAni(_carAniArray[0], "a3", true, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                        },
                        () =>
                        {
                            CarPlayAni(_carAniArray[0], "a2", false, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        }, 4.0f));
                    },
                    () =>
                    {
                        CarPlayAni(_carAniArray[0], "a", true, true);
                    }, 0));
                }
                if (obj.name == _carArray[1].name)
                {
                    //当小车左边的传感器不在黑线上，右边的传感器在黑线上，小车右转
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            CarPlayAni(_carAniArray[1], "b3", true, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                        },
                        () =>
                        {
                            CarPlayAni(_carAniArray[1], "b2", false, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        }, 5.0f));
                    },
                    () =>
                    {
                        CarPlayAni(_carAniArray[1], "b", true, true);
                    }, 0));
                }
                if (obj.name == _carArray[2].name)
                {
                    //当小车左边的传感器在黑线上，右边的传感器不在黑线上，小车左转
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            CarPlayAni(_carAniArray[2], "c3", true, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                        },
                        () =>
                        {
                            CarPlayAni(_carAniArray[2], "c2", false, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        }, 5.0f));
                    },
                    () =>
                    {
                        CarPlayAni(_carAniArray[2], "c", true, true);
                    }, 0));
                }
                if (obj.name == _carArray[3].name)
                {
                    //当小车的两个传感器都在黑线上，小车停止
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            CarPlayAni(_carAniArray[3], "d3", true, false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                        },
                        () =>
                        {
                            CarPlayAni(_carAniArray[3], "d2", false, false);
                        }, 3.0f));
                    },
                    () =>
                    {
                        CarPlayAni(_carAniArray[3], "d", true, true);
                    }, 0));
                }
            }
        }

        //四小车播放动画
        void CarPlayAni(GameObject obj, string aniName, bool aniLoop, bool canClick)
        {

            SpineManager.instance.DoAnimation(obj, aniName, aniLoop);
            _canClick = canClick;
        }
    }
}
