using DG.Tweening;
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
    public class Course836Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject conditionSpine;
        private GameObject conditionColorSpine;
        private GameObject conditionSpineShow;
        private GameObject conditionColorSpineShow;
        private GameObject carImg;
        private Transform btnGroup;
        private GameObject car;
        private GameObject carShow;

        private Transform ImgEffect;
        private GameObject successPanelBg;
        private GameObject successPanel;
        private int MaxSpines = 0;

        private List<string> cars;

        private string temCarName;

        bool isPlaying = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
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
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            conditionSpine = curTrans.Find("conditionSpine").gameObject;
            conditionSpine.SetActive(true);
            conditionSpineShow = curTrans.Find("conditionSpineShow").gameObject;
            for (int i = 0; i < conditionSpineShow.transform.childCount; i++)
            {
                conditionSpineShow.transform.GetChild(i).gameObject.SetActive(false);
            }
            conditionSpineShow.SetActive(false);
            conditionColorSpine = curTrans.Find("conditionColorSpine").gameObject;
            carImg = curTrans.Find("conditionColorSpine/carImg").gameObject;
            ImgEffect = curTrans.Find("conditionColorSpine/ImgEffect");
            ImgEffect.gameObject.SetActive(false);
            conditionColorSpine.SetActive(false);

            conditionColorSpineShow = curTrans.Find("conditionColorSpineShow").gameObject;
            conditionColorSpineShow.SetActive(false);

            btnGroup = curTrans.Find("btnGroup");
            car = curTrans.Find("car").gameObject;
            car.SetActive(false);
            carShow = curTrans.Find("conditionColorSpineShow/carShow").gameObject;
            carShow.SetActive(false);

            if (cars == null || cars.Count < 4)
            {
                cars = new List<string>() { "1", "2", "3", "4" };
            }

            successPanelBg = curTrans.Find("successPanelBg").gameObject;
            successPanel = curTrans.Find("successPanelBg/successPanel").gameObject;
            successPanelBg.SetActive(false);

            for (int i = 0; i < btnGroup.childCount; i++)
            {
                Util.AddBtnClick(btnGroup.GetChild(i).gameObject, OnClickBtn);
            }
            btnGroup.gameObject.SetActive(false);

            temCarName = "";
            MaxSpines = 10;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            if (temCarName == obj.name)
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 5, () => { SoundManager.instance.skipBtn.SetActive(false); }, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false); });

                SpineManager.instance.DoAnimation(car, String.Format("b{0}2", temCarName), false, () =>
                {
                    if (cars.Count <= 0)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () =>
                        {
                            isPlaying = true;
                            successPanelBg.SetActive(true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                            SpineManager.instance.DoAnimation(successPanelBg, "animation", false, () => { SpineManager.instance.DoAnimation(successPanelBg, "animation2", true); });
                            SpineManager.instance.DoAnimation(successPanel, "animation3", false, () => { SpineManager.instance.DoAnimation(successPanelBg, "animation4", true); });
                        }, () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);

                        }));

                    }
                    else
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,null,()=> { RandomCars(); }));
                       
                    }

                });

                
            }
            else
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
                    SpineManager.instance.DoAnimation(car, String.Format("b{0}1", temCarName), false);
                }, () => { isPlaying = false; }));
            }
        }

        void RandomCars()
        {
            car.SetActive(true);
            int tem = Random.Range(0, cars.Count);
            temCarName = cars[tem];
            SpineManager.instance.DoAnimation(car, String.Format("b{0}0", temCarName), false, () => { cars.RemoveAt(tem); isPlaying = false; });
        }
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
              mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
             { mono.StartCoroutine(PlaySpine()); }));  
        }

        IEnumerator PlaySpine()
        {
            float temSeconds = 0;
            for (int i = 1; i < MaxSpines; i++)
            {
                switch (i)
                {
                    case 2:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
                        break;
                    case 4:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        break;
                    case 6:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        break;
                    case 7:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        break;
                    case 8:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        break;
                    case 9:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        break;
                    default:
                        break;
                }
                temSeconds = SpineManager.instance.DoAnimation(conditionSpine, "a" + (i > 1 ? i + "" : ""), false);
                yield return new WaitForSeconds(temSeconds);
            }         
                SoundManager.instance.ShowVoiceBtn(true);
            
        }

        IEnumerator PlaySpineShow()
        {
            float temSeconds = 0;
            for (int i = 1; i < 8; i++)
            {
                switch (i)
                {
                    case 2:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        break;
                    case 4:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        break;
                    case 6:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        break;
                    case 7:
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        break;            
                    default:
                        break;
                }
                temSeconds = SpineManager.instance.DoAnimation(conditionSpineShow, "a" + (i > 1 ? i + "" : ""), false);
                yield return new WaitForSeconds(temSeconds);
            }
            SpineManager.instance.DoAnimation(conditionSpineShow, "a5", false, () =>
            {
                for (int i = 0; i < conditionSpineShow.transform.childCount; i++)
                {
                    conditionSpineShow.transform.GetChild(i).gameObject.SetActive(true);
                }
                SpineManager.instance.DoAnimation(conditionColorSpineShow, "bfg", false);

            });

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
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                {
                    conditionSpine.SetActive(false);
                    conditionColorSpine.SetActive(true);
                    carImg.SetActive(true);
                    SpineManager.instance.DoAnimation(conditionColorSpine, "b", false);
                }, () => { SoundManager.instance.ShowVoiceBtn(true); }));

            }
            if (talkIndex == 2)
            {

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                {
                    carImg.SetActive(false);
                    SpineManager.instance.DoAnimation(conditionColorSpine, "b2", false);
                }, () => { btnGroup.gameObject.SetActive(true); RandomCars(); }));


            }
            if (talkIndex == 3)
            {
                successPanelBg.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () =>
                {
                    mono.StartCoroutine(PlayCarSpine());
                }));

            }
            if (talkIndex == 4)
            {
                successPanelBg.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () =>
                {
                    SpineManager.instance.DoAnimation(conditionColorSpine, "bfg", false, () => { SoundManager.instance.ShowVoiceBtn(true);/* ImgEffect.gameObject.SetActive(true); ImgEffect.DOScale(Vector3.one*1.5f,1f).OnComplete(()=> { SoundManager.instance.ShowVoiceBtn(true); });*/ });
                }));
            }
            if (talkIndex == 5)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, () =>
                {
                    car.SetActive(false);
                    conditionColorSpine.SetActive(false);
                    conditionSpineShow.SetActive(true);
                    SpineManager.instance.DoAnimation(conditionSpineShow, "a", false);
                    conditionColorSpineShow.SetActive(true);
                    SpineManager.instance.DoAnimation(conditionColorSpineShow, "b2", false);
                    carShow.SetActive(true);
                    mono.StartCoroutine(PlaySpineShow());
                }));
            }
                talkIndex++;
        }
        IEnumerator PlayCarSpine()
        {
            float temSeconds = 0;
            for (int i = 1; i < 5; i++)
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 5, () => { SoundManager.instance.skipBtn.SetActive(false); }, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false); });
                temSeconds = SpineManager.instance.DoAnimation(car, String.Format("b{0}2", i), false);
                yield return new WaitForSeconds(temSeconds);
            }
            SoundManager.instance.ShowVoiceBtn(true);
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


