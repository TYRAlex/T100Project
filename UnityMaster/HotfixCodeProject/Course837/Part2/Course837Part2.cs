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
    public class Course837Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        GameObject _water;
        GameObject _shelf;
        GameObject _thermometer;
        GameObject _shaoshui;
        GameObject _rectangle;
        mILDrager _ilDraw;


        GameObject _ice;
        GameObject _icetem;
        GameObject _hot;
        GameObject _hottem;

        GameObject _wintersweet;
        GameObject _tem;

        GameObject _tem15;
        GameObject _hz;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            Debug.Log("start_______");
            Bg = curTrans.Find("Bg").gameObject;
            btnTest = curTrans.GetGameObject("btnTest");
            bell = curTrans.GetGameObject("bell");
            bellTextures = Bg.GetComponent<BellSprites>();


            _water = curTrans.GetGameObject("water");
            _shelf = curTrans.GetGameObject("shelf");

            _thermometer = curTrans.GetGameObject("thermo/thermometer");

            _shaoshui = curTrans.GetGameObject("shaoshui");
            _rectangle = curTrans.GetGameObject("thermo/thermometer/rectangle");
            _ilDraw = _rectangle.GetComponent<mILDrager>();
            _ice = curTrans.GetGameObject("ice");
            _icetem = curTrans.GetGameObject("icetem");
            _hot = curTrans.GetGameObject("hot");
            _hottem = curTrans.GetGameObject("hottem");
            _wintersweet = curTrans.GetGameObject("wintersweet");
            _tem = curTrans.GetGameObject("tem");
            _tem15 = curTrans.GetGameObject("-15");
            _hz = curTrans.GetGameObject("20000");





            Util.AddBtnClick(btnTest, ReStart);

            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {


            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();


            btnTest.Hide();
            bell.Show();
            _water.Show();
            _shelf.Hide();
            _thermometer.Hide();
            _shaoshui.Hide();
            _ice.Hide();
            _icetem.Hide();
            _hot.Hide();
            _hottem.Hide();
            _wintersweet.Hide();
            _tem.Hide();
            _tem15.Hide();
            _hz.Hide();
            _rectangle.transform.localPosition = new Vector3(_rectangle.transform.localPosition.x, -14);


            SpineManager.instance.DoAnimation(_water, "1", true);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            #region 背景图片更换

            if (bellTextures.texture.Length <= 0)
            {
                //Debug.LogError("@愚蠢！！ 哈哈哈 Bg上的BellSprites 里没有东西----------添加完删掉这个打印");
            }
            else
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            }
            #endregion

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //伴随语音，播放水结冰动画：    
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    Debug.LogError("结冰语音");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    SpineManager.instance.DoAnimation(_water, "2", false, () => { SpineManager.instance.DoAnimation(_water, "3", true); });
                },
                () =>
                {
                    //伴随语音“那么”，页面从结冰状态恢复初始状态
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                        () =>
                        {
                            //沸腾
                            Debug.LogError("那么");

                            SpineManager.instance.DoAnimation(_water, "1", false, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SpineManager.instance.DoAnimation(_water, "6", true);
                            });
                        },
                        () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        }
                        ));
                }
                ));

        }


        IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
        }
        void NextSence()
        {
            _water.Hide();

            _shelf.Show();

            _thermometer.Show();

            _shaoshui.Show();
            SpineManager.instance.DoAnimation(_shelf, "shaoshui4", false);
            SpineManager.instance.DoAnimation(_shaoshui, "shaoshui5", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { Debug.LogError("一起探究水完全沸腾时的温度吧！"); }));
            _ilDraw.SetDragCallback(null, null, EndDraw, null);
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void SecondSence()
        {
            _shelf.Hide();
            _thermometer.Hide();
            _shaoshui.Hide();



            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                () =>
                {
                    Debug.LogError("水在0度时会结冰，");

                    _ice.Show();
                    _icetem.Show();
                    mono.StartCoroutine(Wait(2));
                    SpineManager.instance.DoAnimation(_ice, "bs", false);
                    SpineManager.instance.DoAnimation(_icetem, "zb2", false);

                    mono.StartCoroutine(Wait(2));
                    _hot.Show();
                    _hottem.Show();
                    SpineManager.instance.DoAnimation(_hot, "ks", true);
                    SpineManager.instance.DoAnimation(_hottem, "za2", true);
                },
                () =>
                {

                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                             () =>
                             {
                                 _hottem.transform.position = new Vector3(Screen.width / 4, _hottem.transform.position.y);

                                 Debug.LogError("它们都能被称为阈值。阈值是指一个效应能够产生的最低值或最高值");
                                 SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                                 _ice.Hide();
                                 SpineManager.instance.DoAnimation(_icetem, "zb3", false);
                                 _icetem.Hide();
                                 SpineManager.instance.DoAnimation(_hottem, "za3", false);
                                 _hot.Hide();
                                 SpineManager.instance.DoAnimation(_hottem, "yz", false, () => { SpineManager.instance.DoAnimation(_hottem, "yz2", false); });
                             },
                             () =>
                             {
                                 SpineManager.instance.DoAnimation(_hottem, "yz3", false, () => { _hottem.SetActive(false); });

                                 mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                                     () =>
                                     {
                                         Debug.LogError("梅花能承受的最低温度为-15℃");
                                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                         _tem.Show();
                                         _wintersweet.Show();
                                         SpineManager.instance.DoAnimation(_wintersweet, "animation", true);
                                     },
                                     () =>
                                     {
                                         _tem.Hide();
                                         _wintersweet.Hide();
                                         _hottem.Show();

                                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                                             () =>
                                             {
                                                 Debug.LogError("人耳能听到的最高声波频率为20000赫兹");
                                                 SpineManager.instance.DoAnimation(_hottem, "sy", false, () => { SpineManager.instance.DoAnimation(_hottem, "sy2", false, () => { SpineManager.instance.DoAnimation(_hottem, "sy3", false); }); });
                                             },
                                             () =>
                                             {
                                                 SpineManager.instance.DoAnimation(_hottem, "sy4", false,
                                                 () =>
                                                 {
                                                     _hottem.Hide();
                                                     mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7,
                                                         () =>
                                                         {
                                                             Debug.LogError("这些数值也是阈值。你们还知道生活中有哪些阈值呢？");
                                                             _hz.Show();
                                                             _tem15.Show();
                                                         }, null));
                                                 }
                                                 );

                                             }
                                             ));


                                     }
                                     ));
                             }
                             ));

                }));

        }



        private void EndDraw(Vector3 arg1, int arg2, int arg3, bool arg4)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

            //30-70
            if (_rectangle.GetComponent<RectTransform>().anchoredPosition.y > -14 && _rectangle.GetComponent<RectTransform>().anchoredPosition.y <= -2.5f)
            {
                SpineManager.instance.DoAnimation(_shaoshui, "shaoshui5", true);
            }

            //70-80
            if (_rectangle.GetComponent<RectTransform>().anchoredPosition.y > -2.5f && _rectangle.GetComponent<RectTransform>().anchoredPosition.y <= 0)
            {
                SpineManager.instance.DoAnimation(_shaoshui, "shaoshui4", true);
            }
            //80-90
            if (_rectangle.GetComponent<RectTransform>().anchoredPosition.y > 0 && _rectangle.GetComponent<RectTransform>().anchoredPosition.y <= 2.5f)
            {
                SpineManager.instance.DoAnimation(_shaoshui, "shaoshui3", true);
            }
            //90-99
            if (_rectangle.GetComponent<RectTransform>().anchoredPosition.y > 2.5f && _rectangle.GetComponent<RectTransform>().anchoredPosition.y < 5)
            {
                SpineManager.instance.DoAnimation(_shaoshui, "shaoshui2", true);
            }
            //100]
            if (_rectangle.GetComponent<RectTransform>().anchoredPosition.y >= 5)
            {
                SpineManager.instance.DoAnimation(_shaoshui, "shaoshui", true);
            }

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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                NextSence();
            }
            if (talkIndex == 2)
            {
                SecondSence();
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
    }
}
