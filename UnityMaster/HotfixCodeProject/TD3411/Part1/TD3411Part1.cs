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
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public enum Animals
    {
        giraffe,
        panda,
        leopard,
        butterfly,
        zebra,
        fox
    }


    public class TD3411Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject DD;
        private GameObject Bg;
        private GameObject dd;
        private BellSprites bellTextures;

        private GameObject fhBtn;
        private GameObject okBtn;
        private GameObject bfBtn;
        private GameObject caidaiSpine;
        private GameObject successSpine;
        private GameObject mask;


        //--------------------
        GameObject _parents;
        GameObject _tails; //尾巴
        GameObject _animation; //特效
        List<GameObject> _icons; //图标
        int _tailsLevel; //尾巴序号
        GameObject _aniMask;
        private GameObject _elephant;

        int flag = 0;
        //--------------------
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            DD = curTrans.Find("mask/DD").gameObject;
            DD.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            _elephant = mask.transform.GetGameObject("elephant");

            //-------------------------------------
            dd = curTrans.Find("mask/dd").gameObject;
            _icons = new List<GameObject>();
            _parents = curTrans.GetGameObject("Parents");
            _tails = _parents.transform.GetGameObject("tails");
            _animation = _parents.transform.GetGameObject("animation");
            _aniMask = _parents.transform.GetGameObject("Mask");
            for (int i = 0; i < _parents.transform.childCount; i++)
            {
                if (2 < i)
                {
                    _icons.Add(_parents.transform.GetChild(i).gameObject);
                    Util.AddBtnClick(_parents.transform.GetChild(i).gameObject, IconsTouchEvent);
                }
            }


            fhBtn = curTrans.Find("mask/fh").gameObject;
            fhBtn.SetActive(false);
            okBtn = curTrans.Find("mask/ok").gameObject;
            bfBtn = curTrans.Find("mask/bf").gameObject;
            okBtn.SetActive(false);
            bfBtn.SetActive(false);
            Util.AddBtnClick(fhBtn, OnClickAnyBtn);
            Util.AddBtnClick(okBtn, OnClickAnyBtn);
            Util.AddBtnClick(bfBtn, OnClickAnyBtn);


            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
            IsStart();
        }

        void IsStart()
        {
            mask.Show();
            bfBtn.Show();


        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    SpineManager.instance.DoAnimation(fhBtn, result + "2", false);
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    SpineManager.instance.DoAnimation(okBtn, result + "2", false);
                    break;
                default:
                    break;
            }


            return result;
        }


        //结束按钮事件
        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                    bfBtn.Hide();
                }

                else if (obj.name == "ok")
                {
                    NextGame();
              
                    okBtn.Hide();
                }
                else
                {
                    fhBtn.Hide();
                    GameInit();
                }

                //mask.gameObject.SetActive(false);
            });
        }


        private void GameInit()
        {
            flag = 0;
            Input.multiTouchEnabled = false;
            _tailsLevel = 0;
            talkIndex = 1;
            mask.Hide();
            okBtn.Hide();
            bfBtn.Hide();
            fhBtn.Hide();
            dd.Hide();
            DD.Hide();
            SpineManager.instance.DoAnimation(_tails, "kong", false,
                () => { SpineManager.instance.DoAnimation(_tails, "e1", true, () => { }); });
            SpineManager.instance.DoAnimation(_animation, "kong", false);
            SpineManager.instance.DoAnimation(okBtn, "ok2", false);
            SpineManager.instance.DoAnimation(fhBtn, "fh2", false);
            SpineManager.instance.DoAnimation(bfBtn, "bf2", false);
            IconsInit();
            _elephant.Hide();
            _aniMask.Hide();
        }


        void GameStart()
        {
            mask.Show();
            DD.Show();
            okBtn.Hide();
            fhBtn.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);

            //在屏幕下方有很多小动物
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, DD, null,
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
               
                }));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject obj, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(obj, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(obj, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(obj, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();

        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, DD, null,
              () =>
              {               
                  DD.SetActive(false);
                  mask.SetActive(false);
              }));
            }

            talkIndex++;
        }


        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            Debug.Log("Success!!!!!!!!!!!!!!!!!!");
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "3-5-z", false, () =>
            {
                SpineManager.instance.DoAnimation(successSpine, "3-5-z2", true,
                    () =>
                    {
                        PlayChooseReturnOrOk();
                        /*anyBtn.Show();
                        anyBtn.name = getBtnName(BtnEnum.fh);
                        successSpine.SetActive(false);
                        okBtn.Show();
                        okBtn.name = getBtnName(BtnEnum.ok);*/
                        ac?.Invoke();
                    });
            });
        }


        //
        void PlayChooseReturnOrOk()
        {
            mask.SetActive(true);
            fhBtn.Show();
            fhBtn.name = getBtnName(BtnEnum.fh);
            successSpine.SetActive(false);
            okBtn.Show();
            //okBtn.name = getBtnName(BtnEnum.ok);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        //小图标初始化
        void IconsInit()
        {
            SpineManager.instance.DoAnimation(_icons[0].transform.GetChild(0).gameObject, "ya2", true);
            SpineManager.instance.DoAnimation(_icons[1].transform.GetChild(0).gameObject, "n2", true);
            SpineManager.instance.DoAnimation(_icons[2].transform.GetChild(0).gameObject, "yb2", true);
            SpineManager.instance.DoAnimation(_icons[3].transform.GetChild(0).gameObject, "yc2", true);
            SpineManager.instance.DoAnimation(_icons[4].transform.GetChild(0).gameObject, "yd2", true);
            SpineManager.instance.DoAnimation(_icons[5].transform.GetChild(0).gameObject, "n4", true);
          
        }


        //点击事件
        void IconsTouchEvent(GameObject go)
        {
            _aniMask.Show();
            //SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            //长颈鹿
            if (go == _icons[0])
            {
                if (_tailsLevel == 0&&(flag& (1 << _tailsLevel))< 1)
                {
                    flag += (1 << _tailsLevel);
                    //播放完全正确
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 8, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 8, DD,
                        () => { _aniMask.Show(); },()=> {  }));
                    IconsDoAnimation(GetChild(go), "ya1", "ya3",
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_animation, "fd1", false,()=> { _aniMask.Hide(); });

                            mono.StartCoroutine(WaitForSecondDoAction(2f,
                                () =>
                                {
                                    ChooseCorrectly();
                                   
                                }));
                        });
                }
                else
                {
                    SpineManager.instance.DoAnimation(GetChild(go), "ya4", false, () => { SpineManager.instance.DoAnimation(GetChild(go), "ya2", false); });

                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 0, false);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, DD,
                        () => { },
                        () => { _aniMask.Hide(); }));
                }
            }

            //熊猫
            if (go == _icons[1])
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 2, DD,
                       () => { },
                       () => { _aniMask.Hide(); }));
                IconsDoAnimation(GetChild(go), "n1", "n2", () => { });
            }

            //豹子
            if (go == _icons[2])
            {
                if (_tailsLevel == 1 && (flag & (1 << _tailsLevel)) < 1)
                {
                    flag += (1 << _tailsLevel);
                    //播放恭喜你答对了
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 11);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 5, DD,
                        () => { _aniMask.Show(); },
                        () => {  }));

                    IconsDoAnimation(GetChild(go), "yb1", "yb3",
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_animation, "fc1", false,()=> { _aniMask.Hide(); });
                            mono.StartCoroutine(WaitForSecondDoAction(2f, () =>
                            {
                                ChooseCorrectly();                
                            }));
                        });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    SpineManager.instance.DoAnimation(GetChild(go), "yb4", false, () => { SpineManager.instance.DoAnimation(GetChild(go), "yb2", false); });

                    //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 0, false);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, DD,
                        () => { _aniMask.Show(); },
                        () => { _aniMask.Hide(); }));

                    //_aniMask.Hide();
                }
            }

            //蝴蝶
            if (go == _icons[3])
            {
                if (_tailsLevel == 2 && (flag & (1 << _tailsLevel)) < 1)
                {
                    flag += (1 << _tailsLevel);
                    //播放小朋友们真是太厉害了
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 9);
                    _aniMask.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 9, DD,
                        () => { _aniMask.Show(); },
                        () => {}));

                    IconsDoAnimation(GetChild(go), "yc1", "yc3",
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_animation, "ff1", false,()=> { _aniMask.Hide(); });
                            mono.StartCoroutine(WaitForSecondDoAction(2f, () =>
                            {
                                ChooseCorrectly();
                               
                            }));
                        });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    SpineManager.instance.DoAnimation(GetChild(go), "yc4", false, () => { SpineManager.instance.DoAnimation(GetChild(go), "yc2", false); });

                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 2, DD,
                        () => { _aniMask.Show(); },
                        () => { _aniMask.Hide(); }));

                    //_aniMask.Hide();
                }
            }

            //斑马
            if (go == _icons[4])
            {
                if (_tailsLevel == 3 && (flag & (1 << _tailsLevel)) < 1)
                {
                    flag += (1 << _tailsLevel);
                    //播放哇你们简直太棒了

                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 4, DD,
                        () => { _aniMask.Show(); },
                        () => {  }));
                    IconsDoAnimation(GetChild(go), "yd1", "yd3",
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_animation, "fe1", false);
                            mono.StartCoroutine(WaitForSecondDoAction(2f, () =>
                            {
                                ChooseCorrectly();
                                _aniMask.Hide();
                            }));
                            Wait(2f, () => { playSuccessSpine(); });
                           
                        });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    SpineManager.instance.DoAnimation(GetChild(go), "yd4", false, () => { SpineManager.instance.DoAnimation(GetChild(go), "yd2", false); });
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 2, DD,
                        () => { },
                        () => { _aniMask.Hide(); }));
                }
            }

            //狐狸
            if (go == _icons[5])
            {
                //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 1, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 1, DD, () => { },
                    () => { _aniMask.Hide(); }));
                IconsDoAnimation(GetChild(go), "n3", "n4", () => { });
            }
        }


        //图标动画连续
        void IconsDoAnimation(GameObject go, string animation1, string animation2 = null, Action method1 = null)
        {
            SpineManager.instance.DoAnimation(go, animation1, false,
                () =>
                {
                    if (animation2 == null)
                    {
                        Debug.LogError("animation2 is null");
                    }

                    SpineManager.instance.DoAnimation(go, animation2, false,
                        () => { method1?.Invoke(); });
                });
        }


        //正确选择
        void ChooseCorrectly()
        {
         
            if (_tailsLevel < 3)
            {
        
                SpineManager.instance.DoAnimation(_animation, "kong", true, () => { });
            }

            if (_tailsLevel == 0)
            {
                SpineManager.instance.DoAnimation(_tails, "kong", true, () =>
                {
                    SpineManager.instance.DoAnimation(_tails, "e3", true);
                    IconsInit();                
     
                });
            }

            if (_tailsLevel == 1)
            {
                SpineManager.instance.DoAnimation(_tails, "kong", false, () =>
                {
                    SpineManager.instance.DoAnimation(_tails, "e7", true);
                 
                    IconsInit();
                });
            }

            if (_tailsLevel == 2)
            {
                SpineManager.instance.DoAnimation(_tails, "kong", false, () =>
                {
                    SpineManager.instance.DoAnimation(_tails, "e5", true);
                    IconsInit();
         
                });
            }

            _tailsLevel++;
        }


        GameObject GetChild(GameObject go)
        {
            return go.transform.GetChild(0).gameObject;
        }

        IEnumerator WaitForSecondDoAction(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSecondDoAction(time, method));
        }


        //下一环节
        private void NextGame()
        {
            mask.SetActive(true);
            dd.Show();
            fhBtn.Hide();
            okBtn.Hide();
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, dd,
                () => { Debug.Log("小朋友们的眼力超棒哦"); },
                () => { EndSence(); }));
        }

        void EndSence()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, dd,
                () => { },
                () =>
                {
                    dd.Hide();
                    _elephant.Show();
                    //Debug.Log("找到了尼古");

                    IconsDoAnimation(_elephant, "t", "t2",
                        () =>
                        {
                        });
                }));
        }
    }
}