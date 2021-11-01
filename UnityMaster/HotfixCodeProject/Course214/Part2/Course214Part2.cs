using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course214Part2
    {
        private GameObject max;

        private int talkIndex;

        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject btnTest;

        private Transform btnImg;
        private Transform imgSpineShow;

        private Transform btnSpineShow;
        private GameObject btnBack;
        private Transform sgPanel;
        private GameObject sgQyUp;
        private GameObject sgQyDown;
        private GameObject sgWdj;
        private GameObject sgSpine;
        private GameObject sgBtnBack;


        private Transform hlPanel;
        private GameObject hlQyUp;
        private GameObject hlQyDown;
        private GameObject hlWdj;
        private GameObject hlSpine;
        private GameObject hlBtnBack;

        private GameObject starSpine;

        private RawImage Bg;
        private BellSprites bellTextures;


        bool isPlaying = false;

        int selectIndex = 0;

        bool sgFirstPress = false;
        bool hlFirstPress = false;
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

        private void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("bg").GetComponent<RawImage>();
            bellTextures = Bg.transform.GetComponent<BellSprites>();
            max = curTrans.Find("Max").gameObject;
            max.SetActive(true);
            btnImg = curTrans.Find("btnImg");
            btnImg.transform.localScale = new Vector3(0, 1, 1);
            Util.AddBtnClick(btnImg.gameObject, OnClickBtnImg);
            btnImg.gameObject.SetActive(true);

            imgSpineShow = curTrans.Find("imgSpineShow");
            btnSpineShow = curTrans.Find("btnSpineShow");
            for (int i = 0; i < imgSpineShow.childCount; i++)
            {
                Util.AddBtnClick(imgSpineShow.GetChild(i).gameObject, OnClickImgSpineBtn);
                imgSpineShow.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < btnSpineShow.childCount - 1; i++)
            {
                Util.AddBtnClick(btnSpineShow.GetChild(i).gameObject, OnClickBtnSpine);
                btnSpineShow.GetChild(i).gameObject.SetActive(false);
            }
            imgSpineShow.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(imgSpineShow.gameObject, "kong", false);
            btnSpineShow.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(btnSpineShow.gameObject, "kong", false);

            btnBack = curTrans.Find("btnSpineShow/fanhui2").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            sgPanel = curTrans.Find("sgPanel");

            sgQyUp = curTrans.Find("sgPanel/qyUp").gameObject;
            sgQyDown = curTrans.Find("sgPanel/qyDown").gameObject;
            sgWdj = curTrans.Find("sgPanel/wdj").gameObject;
            sgSpine = curTrans.Find("sgPanel/Spine").gameObject;
            sgBtnBack = curTrans.Find("sgPanel/btnBack").gameObject;
            Util.AddBtnClick(sgBtnBack, OnClickSGBtnBack);

            for (int i = 0; i < sgPanel.childCount; i++)
            {
                sgPanel.GetChild(i).gameObject.SetActive(false);
            }


            hlPanel = curTrans.Find("hlPanel");

            hlQyUp = curTrans.Find("hlPanel/qyUp").gameObject;
            hlQyDown = curTrans.Find("hlPanel/qyDown").gameObject;
            hlWdj = curTrans.Find("hlPanel/wdj").gameObject;
            hlSpine = curTrans.Find("hlPanel/Spine").gameObject;
            hlBtnBack = curTrans.Find("hlPanel/btnBack").gameObject;
            Util.AddBtnClick(hlBtnBack, OnClickHLBtnBack);
            for (int i = 0; i < hlPanel.childCount; i++)
            {
                hlPanel.GetChild(i).gameObject.SetActive(false);
            }
            starSpine = curTrans.Find("starSpine").gameObject;
            starSpine.SetActive(true);
            SpineManager.instance.DoAnimation(starSpine, "r2", false);
                talkIndex = 1;
            isPlaying = false;

            selectIndex = 0;
            sgFirstPress = false;
            hlFirstPress = false;

            temSunAndMoon = "";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private void OnClickHLBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            SgAndHlBack();
        }

        private void OnClickSGBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            SgAndHlBack();
        }
        void SgAndHlBack()
        {

            SpineManager.instance.DoAnimation(starSpine, temSunAndMoon + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(sgSpine, "kong", false, () => { sgSpine.SetActive(false); });
                SpineManager.instance.DoAnimation(hlSpine, "kong", false, () => { hlSpine.SetActive(false); });

                sgBtnBack.SetActive(false);
                hlBtnBack.SetActive(false);
                Bg.texture = bellTextures.texture[selectIndex > 0 ? 3 : 1];
                btnBack.SetActive(true);
                max.SetActive(true);
                SpineManager.instance.DoAnimation(imgSpineShow.gameObject, "kong", false, () =>
                {
                    imgSpineShow.gameObject.SetActive(false);
                });              
                btnSpineShow.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(btnSpineShow.gameObject, "b1", false, () =>
                {
                    for (int i = 0; i < btnSpineShow.childCount - 1; i++)
                    {
                        btnSpineShow.GetChild(i).gameObject.SetActive(true);
                    }
                    isPlaying = false;
                });

            });

        }

        private void OnClickBtnImg(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            obj.SetActive(false);
            SpineManager.instance.DoAnimation(imgSpineShow.gameObject, "a1", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
            () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                SpineManager.instance.DoAnimation(imgSpineShow.gameObject, "a2", false);
            },
            () =>
            {
                for (int i = 0; i < imgSpineShow.childCount; i++)
                {
                    imgSpineShow.GetChild(i).gameObject.SetActive(true);
                }
                isPlaying = false;
            }, 1.2f));
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                Bg.texture = bellTextures.texture[0];
                for (int i = 0; i < btnSpineShow.childCount; i++)
                {
                    btnSpineShow.GetChild(i).gameObject.SetActive(false);
                }
                SpineManager.instance.DoAnimation(btnSpineShow.gameObject, "kong", false, () => {                   
                    btnSpineShow.gameObject.SetActive(false);
                    imgSpineShow.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(imgSpineShow.gameObject, "a1", false,
                        () =>
                        {
                            for (int i = 0; i < imgSpineShow.childCount; i++)
                            {
                                imgSpineShow.GetChild(i).gameObject.SetActive(true);
                            }
                            isPlaying = false;
                            if (sgFirstPress&&hlFirstPress)
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            }
                            
                        });
                });
               
            });
        }


        private void OnClickImgSpineBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            obj.SetActive(false);
            selectIndex = obj.transform.GetSiblingIndex();
            SpineManager.instance.DoAnimation(imgSpineShow.gameObject, obj.name, false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(imgSpineShow.gameObject, obj.transform.GetChild(0).name, false, () =>
                {
                    if (!sgFirstPress&& selectIndex==0)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, selectIndex > 0 ? 11 : 2, null, () => { isPlaying = false; sgFirstPress = true; }));
                    }
                    else if (!hlFirstPress && selectIndex == 1)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, selectIndex > 0 ? 11 : 2,
                          null, () => { isPlaying = false; hlFirstPress = true; }));
                    }
                    else
                    {
                        isPlaying = false;
                    }
                    Bg.texture = bellTextures.texture[selectIndex > 0 ? 3 : 1];
                    btnBack.SetActive(true);
                    SpineManager.instance.DoAnimation(imgSpineShow.gameObject, "kong", false, () =>
                    {
                        imgSpineShow.gameObject.SetActive(false);
                        btnSpineShow.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(btnSpineShow.gameObject, "b1", false, () =>
                        {
                            for (int i = 0; i < btnSpineShow.childCount - 1; i++)
                            {
                                btnSpineShow.GetChild(i).gameObject.SetActive(true);
                            }

                        });
                    });


                });
            });
        }
        string temSunAndMoon = "";
        private void OnClickBtnSpine(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            obj.SetActive(false);
            max.SetActive(false);
            //BtnPlaySound();
            int tem = 0;
            if (selectIndex <= 0)
            {
                if (obj.transform.GetSiblingIndex() > 0)
                {
                    tem = 1;
                    temSunAndMoon = "y";
                }
                else
                {
                    tem = 0;
                    temSunAndMoon = "r";
                }
            }
            else
            {
                if (obj.transform.GetSiblingIndex() > 0)
                {
                    tem = 3;
                    temSunAndMoon = "y";
                }
                else
                {
                    temSunAndMoon = "r";
                    tem = 2;
                }
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, tem, false); starSpine.SetActive(true);
            SpineManager.instance.DoAnimation(btnSpineShow.gameObject, obj.name, false, () =>
            {
                btnSpineShow.gameObject.SetActive(false);
                btnBack.SetActive(false);
                Bg.texture = bellTextures.texture[selectIndex > 0 ? obj.transform.GetSiblingIndex() + 3 : obj.transform.GetSiblingIndex() + 1];


                SpineManager.instance.DoAnimation(starSpine, temSunAndMoon + "1", false, () =>
                  {
                      if (selectIndex <= 0)
                      {
                          if (obj.transform.GetSiblingIndex() <= 0)
                          {
                              mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () =>
                              {
                                  sgWdj.SetActive(true);
                                  SpineManager.instance.DoAnimation(sgWdj, "wd+", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(sgWdj, "wd+2", false, () =>
                                      {

                                      });

                                  });

                              }, () =>
                              {

                                  SpineManager.instance.DoAnimation(sgWdj, "kong", false, () => { sgWdj.SetActive(false); });

                                  mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                                  {
                                      sgSpine.SetActive(true);
                                      SpineManager.instance.DoAnimation(sgSpine, "qiya", false, () =>
                                      {
                                          SpineManager.instance.DoAnimation(sgSpine, "qiya2", false, () =>
                                          {

                                          });

                                      });

                                  }, () =>
                                  {
                                      SpineManager.instance.DoAnimation(sgSpine, "kong", false, () => { sgSpine.SetActive(false); });
                                      mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () =>
                                      {
                                          sgQyUp.SetActive(true);
                                          SpineManager.instance.DoAnimation(sgQyUp, "dqy3", false, () =>
                                          {
                                              SpineManager.instance.DoAnimation(sgQyUp, "dqy4", false, () =>
                                              {

                                              });

                                          });

                                      }, () =>
                                      {
                                          mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () =>
                                          {
                                              sgQyDown.SetActive(true);
                                              SpineManager.instance.DoAnimation(sgQyDown, "gqy3", false, () =>
                                              {
                                                  SpineManager.instance.DoAnimation(sgQyDown, "gqy4", false, () =>
                                                  {

                                                  });

                                              });

                                          }, () =>
                                          {
                                              SpineManager.instance.DoAnimation(sgQyUp, "kong", false, () => { sgQyUp.SetActive(false); });
                                              SpineManager.instance.DoAnimation(sgQyDown, "kong", false, () => { sgQyDown.SetActive(false); });
                                              mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () =>
                                              {
                                                  sgSpine.SetActive(true);
                                                  SpineManager.instance.DoAnimation(sgSpine, "sga1", false, () =>
                                                  {
                                                      SpineManager.instance.DoAnimation(sgSpine, "sga2", true);
                                                      sgBtnBack.SetActive(true);
                                                  });

                                              }, () => { isPlaying = false; }));
                                          }));
                                      }));
                                  }));
                              }));
                          }
                          else
                          {

                              mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, () =>
                              {
                                  sgWdj.SetActive(true);
                                  SpineManager.instance.DoAnimation(sgWdj, "wd-", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(sgWdj, "wd-2", false, () =>
                                      {

                                      });

                                  });

                              }, () =>
                              {

                                  SpineManager.instance.DoAnimation(sgWdj, "kong", false, () => { sgWdj.SetActive(false); });
                                  mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, () =>
                                  {
                                      sgQyDown.SetActive(true);
                                      SpineManager.instance.DoAnimation(sgQyDown, "dqy", false, () =>
                                      {
                                          SpineManager.instance.DoAnimation(sgQyDown, "dqy2", false, () =>
                                          {

                                          });

                                      });

                                      sgQyUp.SetActive(true);
                                      SpineManager.instance.DoAnimation(sgQyUp, "gqy", false, () =>
                                      {
                                          SpineManager.instance.DoAnimation(sgQyUp, "gqy2", false, () =>
                                          {

                                          });

                                      });

                                  }, () =>
                                  {
                                      SpineManager.instance.DoAnimation(sgQyDown, "kong", false, () => { sgQyDown.SetActive(false); });
                                      SpineManager.instance.DoAnimation(sgQyUp, "kong", false, () => { sgQyUp.SetActive(false); });



                                      mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () =>
                                      {
                                          sgSpine.SetActive(true);
                                          SpineManager.instance.DoAnimation(sgSpine, "sgb1", false, () =>
                                          {
                                              SpineManager.instance.DoAnimation(sgSpine, "sgb2", true);
                                              sgBtnBack.SetActive(true);
                                          });

                                      }, () => { isPlaying = false; }));
                                  }));
                              }));
                          }

                      }
                      else
                      {
                          if (obj.transform.GetSiblingIndex() <= 0)
                          {

                              mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, () =>
                              {
                                  hlWdj.SetActive(true);

                                  SpineManager.instance.DoAnimation(hlWdj, "wd+", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(hlWdj, "wd+2", false, () =>
                                      {

                                      });

                                  });
                                  hlSpine.SetActive(true);
                                  SpineManager.instance.DoAnimation(hlSpine, "hbatyc", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(hlSpine, "hbatyd", false, () =>
                                      {

                                      });

                                  });

                              }, () =>
                              {
                                  SpineManager.instance.DoAnimation(hlWdj, "kong", false, () => { hlWdj.SetActive(false); }); SpineManager.instance.DoAnimation(hlSpine, "kong", false, () => { hlSpine.SetActive(false); });
                                  mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, () =>
                                  {
                                      hlQyUp.SetActive(true);
                                      hlQyDown.SetActive(true);
                                      SpineManager.instance.DoAnimation(hlQyUp, "wdg", false, () =>
                                      {
                                          SpineManager.instance.DoAnimation(hlQyUp, "wdg2", false, () =>
                                          {

                                          });

                                      });

                                      SpineManager.instance.DoAnimation(hlQyDown, "wdd", false, () =>
                                      {
                                          SpineManager.instance.DoAnimation(hlQyDown, "wdd2", false, () =>
                                          {

                                          });

                                      });



                                  }, () =>
                                  {
                                      SpineManager.instance.DoAnimation(hlQyUp, "kong", false, () => { hlQyUp.SetActive(false); });
                                      SpineManager.instance.DoAnimation(hlQyDown, "kong", false, () => { hlQyDown.SetActive(false); });
                                      mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 14, () =>
                                      {
                                          hlSpine.SetActive(true);
                                          SpineManager.instance.DoAnimation(hlSpine, "hba1", false, () =>
                                          {
                                              SpineManager.instance.DoAnimation(hlSpine, "hba2", true);
                                              hlBtnBack.SetActive(true);
                                          });

                                      }, () => { isPlaying = false; }));
                                  }));
                              }));
                          }
                          else
                          {

                              mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 15, () =>
                              {
                                  hlQyUp.SetActive(true);
                                  SpineManager.instance.DoAnimation(hlQyUp, "wdd3", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(hlQyUp, "wdd4", false, () =>
                                      {

                                      });

                                  });

                                  hlQyDown.SetActive(true);
                                  SpineManager.instance.DoAnimation(hlQyDown, "wdg3", false, () =>
                                  {
                                      SpineManager.instance.DoAnimation(hlQyDown, "wdg4", false, () =>
                                      {

                                      });

                                  });


                              }, () =>
                              {
                                  SpineManager.instance.DoAnimation(hlQyUp, "kong", false, () => { hlQyUp.SetActive(false); });
                                  SpineManager.instance.DoAnimation(hlQyDown, "kong", false, () => { hlQyDown.SetActive(false); });
                                  mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 16, () =>
                                  {

                                      hlSpine.SetActive(true);
                                      SpineManager.instance.DoAnimation(hlSpine, "hbb1", false, () =>
                                      {
                                          SpineManager.instance.DoAnimation(hlSpine, "hbb2", true);
                                          hlBtnBack.SetActive(true);
                                      });
                                  }, () => { isPlaying = false; }));
                              }));
                          }
                      }


                  });
            });
        }

        void GameStart()
        {
            isPlaying = true;
            Bg.texture = bellTextures.texture[0];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
            {
                btnImg.transform.DOScale(Vector3.one, 1);
            }, () => { isPlaying = false; }));

        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                isPlaying = true;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 17,null, () => { isPlaying = false; }));
            }
            talkIndex++;
        }







        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind - len);
            SpineManager.instance.DoAnimation(max, "daiji");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

    }
}
