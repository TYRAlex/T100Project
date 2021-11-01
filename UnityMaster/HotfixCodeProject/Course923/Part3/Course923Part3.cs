using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course923Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private Transform panel;
        private Transform btns;
        private GameObject spineShow;
        private GameObject btnBack;

        private List<GameObject> spineObjs;

        bool isPlaying = false;
        bool isPress = false;
        int flag = 0;
        bool isEnd = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            panel = curTrans.Find("panel");

            spineObjs = new List<GameObject>();

            btns = curTrans.Find("btns");
            spineShow = curTrans.Find("spineShow").gameObject;
            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            for (int i = 0; i < btns.childCount; i++)
            {
                spineObjs.Add(panel.GetChild(i).gameObject);
                Util.AddBtnClick(btns.GetChild(i).gameObject, OnClickBtn);
            }
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPress)
                return;
            isPress = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(spineShow, "kong", false,
                              () =>
                              {
                                  panel.gameObject.SetActive(true);
                                  Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                  SpineManager.instance.DoAnimation(spineObjs[temIndex], temIndex + 1 + "t", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(spineObjs[temIndex], temIndex + 1 + "j2", false, () =>
                                      {
                                          isPress = false;
                                          isPlaying = false;
                                          obj.SetActive(false);
                                          if (flag >= (Mathf.Pow(2, panel.childCount) - 1) && !isEnd)
                                          {
                                              SoundManager.instance.ShowVoiceBtn(true);
                                          }

                                      });
                                  });

                              });

        }
        int temIndex = 0;
        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            temIndex = int.Parse(obj.name) - 1;
            spineObjs[temIndex].transform.SetAsLastSibling();
            mono.StartCoroutine(PlaySpine(obj, ((flag & (1 << temIndex)) == 0)));

        }

        IEnumerator PlaySpine(GameObject obj, bool isFirst)
        {
            float temTime = 0;
            if ((flag & (1 << temIndex)) == 0)
            {
                flag += 1 << temIndex;
            }
            if (isFirst)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                temTime = SpineManager.instance.DoAnimation(spineObjs[temIndex], obj.name + "d", false);
            }
            yield return new WaitForSeconds(temTime);
            if (!isFirst)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            }
            SpineManager.instance.DoAnimation(spineObjs[temIndex], obj.name + "d2", false,
                   () =>
                   {
                       panel.gameObject.SetActive(false);
                       Bg.GetComponent<RawImage>().texture = bellTextures.texture[temIndex + 1];

                       if (temIndex == 0)
                       {
                           mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1,
                              () =>
                              {
                                  SpineManager.instance.DoAnimation(spineShow, obj.transform.GetChild(0).name + "cx", false);
                              },
                               () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, temIndex);
                                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, () =>
                                    {

                                        SpineManager.instance.DoAnimation(spineShow, obj.transform.GetChild(0).name, false);
                                    },
                                    () =>
                                    {
                                        btnBack.SetActive(true);
                                    }));
                                }));
                       }
                       else if (temIndex == 1)
                       {
                           mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, () =>
                           {
                               SpineManager.instance.DoAnimation(spineShow, obj.transform.GetChild(0).name + "cx", false);
                           }, () =>
                           {

                               mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, () =>
                               {
                                   SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, temIndex);
                                   SpineManager.instance.DoAnimation(spineShow, obj.transform.GetChild(0).name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(spineShow, "kong", false, () => { Bg.GetComponent<RawImage>().texture = bellTextures.texture[5]; });
                                });

                               }, () => { btnBack.SetActive(true); }));
                           }));

                       }
                       else if (temIndex == 2)
                       {
                           mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5,
                               () =>
                               {
                                   SpineManager.instance.SetFreeze(spineShow, true);
                               },
                               () =>
                               {
                                   SpineManager.instance.SetFreeze(spineShow, false);

                               }, 2));

                           SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, temIndex);
                           SpineManager.instance.DoAnimation(spineShow, obj.transform.GetChild(0).name, false,
                              () =>
                              {
                                  btnBack.SetActive(true);

                              });
                       }
                       else
                       {
                           mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 6, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, temIndex);
                                SpineManager.instance.DoAnimation(spineShow, obj.transform.GetChild(0).name, false,
                               () =>
                               {

                               });
                            }, () => { btnBack.SetActive(true); }));

                       }



                   });

        }

        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            isPress = false;
            isEnd = false;
            panel.gameObject.SetActive(true);

            GameObject[] temObjs = spineObjs.ToArray();
            for (int i = 0; i < temObjs.Length; i++)
            {
                int tem = int.Parse(temObjs[i].name)-1;
                GameObject temILDrager;
                temILDrager = spineObjs[tem];
                spineObjs[tem] = temObjs[i];
            }
            for (int i = 0; i < panel.childCount; i++)
            {
                spineObjs[i].GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(spineObjs[i].gameObject, spineObjs[i].name + "j", false);
            }

            SpineManager.instance.SetFreeze(spineShow, false);
            SpineManager.instance.DoAnimation(spineShow, "kong", false);


        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            bell.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); isPlaying = false; }));

        }



        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bell;
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



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                isEnd = true;
                bell.SetActive(true);
                isPlaying = true;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 7, null, () =>
                {
                    bell.SetActive(false);
                    isPlaying = false;
                }));
            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
