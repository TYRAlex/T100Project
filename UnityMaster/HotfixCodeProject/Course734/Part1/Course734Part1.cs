using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course734Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _spineOption;
        private GameObject _spineMobot;

        private GameObject _spineOptionUI_0;
        private GameObject _spineOptionUI_1;
        private GameObject _spineOptionUI_2;

        private GameObject _spineMobot_hg1;
        private GameObject _spineMobot_hg2;
        private GameObject _spineMobot_hg3;
        private GameObject _spineMobot_hg4;

        private Transform _optionBtn;
        private Transform _mobotPosBtn;

        private Empty4Raycast[] _optionBtnE4rs;
        private PolygonCollider2D[] _mobotPosBtnPCosllider;

        private GameObject _mask;        


        private int flag;
        private int flag_1;
        private int voiceBtn;
        private int voiceBtn_1;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            flag_1 = 0;
            voiceBtn = 0;
            voiceBtn_1 = 0;
            voiceIndex = -1;
            FindInit();
        }
        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);

            isPlaying = true;

            SpineManager.instance.DoAnimation(_spineOption, "kong", false);
            _spineOption.Hide();

            _spineMobot.Show();
            _spineMobot.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            _spineMobot.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spineMobot, "hg5", false);


            _spineOptionUI_0.Hide();
            _spineOptionUI_1.Hide();
            _spineOptionUI_2.Hide();

            SpineManager.instance.DoAnimation(_spineMobot_hg1, "kong", false);
            SpineManager.instance.DoAnimation(_spineMobot_hg2, "kong", false);
            SpineManager.instance.DoAnimation(_spineMobot_hg3, "kong", false);
            SpineManager.instance.DoAnimation(_spineMobot_hg4, "kong", false);
            _spineMobot_hg1.Hide();
            _spineMobot_hg2.Hide();
            _spineMobot_hg3.Hide();
            _spineMobot_hg4.Hide();



            for (int i = 0; i < _optionBtnE4rs.Length; i++)
            {
                Util.AddBtnClick(_optionBtnE4rs[i].gameObject, OptionBtnClickEvent);
                _optionBtnE4rs[i].gameObject.Hide();
            }
            for (int i = 0; i < _mobotPosBtnPCosllider.Length; i++)
            {
                Util.AddBtnClick(_mobotPosBtnPCosllider[i].gameObject, MobotPosBtnClickEvent);
                _mobotPosBtnPCosllider[i].gameObject.Hide();
            }


            _mask.Show();
            Max.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                isPlaying = false;

                SpineManager.instance.DoAnimation(_spineOption, "kong", false);
                _spineOption.Hide();                              
                StartGame();
            }));
            mono.StartCoroutine(WariteTimeCoroutine(() =>
            {
                _spineOption.Show();
                _spineOption.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.SetTimeScale(_spineOption, 0.8f);
                SpineManager.instance.DoAnimation(_spineOption, "z", false, () =>
                {
                    SpineManager.instance.DoAnimation(_spineOption, "z2", false, () =>
                    {                        
                        SpineManager.instance.SetTimeScale(_spineOption, 1);                       

                        _spineOptionUI_0.Show();
                        _spineOptionUI_1.Show();
                        _spineOptionUI_2.Show();
                        _spineOptionUI_0.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        _spineOptionUI_1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        _spineOptionUI_2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(_spineOptionUI_0, "a", false);
                        SpineManager.instance.DoAnimation(_spineOptionUI_1, "b", false);
                        SpineManager.instance.DoAnimation(_spineOptionUI_2, "c", false,()=> 
                        {
                            SpineManager.instance.DoAnimation(_spineOption, "kong", false);
                            _spineOption.Hide();
                        });                       
                    });
                });
            }, 2.0f));

            mono.StartCoroutine(WaitePlayVoiceCoroutine(() => 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4);
            }, 2.0f,()=> 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4);
            },1.5f,()=> 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4);
            },1.5f));

        }
        private void FindInit()
        {
            _spineOption = curTrans.Find("spinePanel/option").gameObject;
            _spineMobot = curTrans.Find("spinePanel/mobot").gameObject;

            _spineOptionUI_0 = curTrans.Find("spinePanel/optionUI_0").gameObject;
            _spineOptionUI_1 = curTrans.Find("spinePanel/optionUI_1").gameObject;
            _spineOptionUI_2 = curTrans.Find("spinePanel/optionUI_2").gameObject;

            _optionBtn = curTrans.Find("optionBtn");
            _mobotPosBtn = curTrans.Find("mobotPosBtn");

            _spineMobot_hg1 = curTrans.Find("spinePanel/mobot_hg1").gameObject;
            _spineMobot_hg2 = curTrans.Find("spinePanel/mobot_hg2").gameObject;
            _spineMobot_hg3 = curTrans.Find("spinePanel/mobot_hg3").gameObject;
            _spineMobot_hg4 = curTrans.Find("spinePanel/mobot_hg4").gameObject;


            _optionBtnE4rs = _optionBtn.GetComponentsInChildren<Empty4Raycast>(true);
            _mobotPosBtnPCosllider = _mobotPosBtn.GetComponentsInChildren<PolygonCollider2D>(true);

            _mask = curTrans.Find("mask").gameObject;
           
        }
        private void StartGame()
        {
            _mask.Hide();
            for (int i = 0; i < _optionBtnE4rs.Length; i++)
            {
                _optionBtnE4rs[i].gameObject.Show();
            }
        }
        private void ShowOrHideOptionClickBtn(Empty4Raycast[] e4r, bool isShow)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                e4r[i].gameObject.SetActive(isShow);
            }
        }
        private void ShowOrHideMobotPolClickBtn(PolygonCollider2D[] pol, bool isShow)
        {
            for (int i = 0; i < pol.Length; i++)
            {
                pol[i].gameObject.SetActive(isShow);
            }
        }
        private void OptionBtnClickEvent(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1);
            SoundManager.instance.ShowVoiceBtn(false);

            if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
            {
                flag += 1 << obj.transform.GetSiblingIndex();
            }
            if (flag == (Mathf.Pow(2, _optionBtn.transform.childCount) - 1))
            {
                voiceBtn = 1;
                flag_1 = 0;
                voiceBtn_1 = 0;               
            }           

            ShowOrHideOptionClickBtn(_optionBtnE4rs, false);
            if (obj.name == "0")//执行机构
            {

                flag_1 = 0;

                SpineManager.instance.DoAnimation(_spineOptionUI_1, "b", false);
                SpineManager.instance.DoAnimation(_spineOptionUI_2, "c", false);
               
                SpineManager.instance.DoAnimation(_spineOptionUI_0, "a2", false, () =>
                {
                    SpineManager.instance.DoAnimation(_spineOptionUI_0, "a3", false, () =>
                    {
                        _spineMobot.Show();                       
                        SpineManager.instance.DoAnimation(_spineMobot, "hg5", false, () =>
                        {
                            mono.StartCoroutine(WariteTimeCoroutine(() => 
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 6, false);
                                _spineMobot_hg1.Show();
                                _spineMobot_hg2.Show();
                                _spineMobot_hg3.Show();
                                _spineMobot_hg4.Show();
                                _spineMobot_hg1.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
                                _spineMobot_hg2.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
                                _spineMobot_hg3.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
                                _spineMobot_hg4.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
                                SpineManager.instance.DoAnimation(_spineMobot_hg1, "hg1", false, () => { _spineMobot_hg1.Hide();});
                                SpineManager.instance.DoAnimation(_spineMobot_hg2, "hg2", false, () => {_spineMobot_hg2.Hide(); });
                                SpineManager.instance.DoAnimation(_spineMobot_hg3, "hg3", false, () => {_spineMobot_hg3.Hide(); });
                                SpineManager.instance.DoAnimation(_spineMobot_hg4, "hg4", false, () => {_spineMobot_hg4.Hide(); });
                            }, 9.0f));
                        });
                        mono.StartCoroutine(WariteTimeCoroutine(() => 
                        { 
                            _spineMobot_hg1.Show();
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 6, false);
                            SpineManager.instance.DoAnimation(_spineMobot_hg1, "hg1", false, () => { _spineMobot_hg1.Hide(); });
                            mono.StartCoroutine(WariteTimeCoroutine(() =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 6, false);
                                SpineManager.instance.DoAnimation(_spineMobot_hg1, "hg4", false, () => { _spineMobot_hg1.Hide(); });                              
                                _spineMobot_hg2.Show();
                                SpineManager.instance.DoAnimation(_spineMobot_hg2, "hg2", false, () => { _spineMobot_hg2.Hide(); });
                                mono.StartCoroutine(WariteTimeCoroutine(() =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 6, false);
                                    _spineMobot_hg2.Hide();
                                    SpineManager.instance.DoAnimation(_spineMobot_hg1, "hg3", false, () => { _spineMobot_hg1.Hide(); });
                                }, 1.0f));
                            }, 1.0f));
                        }, 1.0f));
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, ()=> 
                        { 
                            ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, true); 
                        }));
                    });
                });
            }
            else if (obj.name == "1")//驱动机构
            {               
                SpineManager.instance.DoAnimation(_spineOptionUI_0, "a", false);
                SpineManager.instance.DoAnimation(_spineOptionUI_2, "c", false);

                ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, false);                
                OptionBtnAni(_spineOptionUI_1, _spineMobot, "b", 2,3, 5, () =>
                {
                    mono.StartCoroutine(WariteTimeCoroutine(() => 
                    {
                        _spineMobot.Show();
                        SpineManager.instance.DoAnimation(_spineMobot, "xian", false, () =>
                        {
                            SpineManager.instance.DoAnimation(_spineMobot, "hg5", false);
                        });
                    }, 2.8f));
                },()=> 
                {
                    ShowOrHideOptionClickBtn(_optionBtnE4rs, true);
                    _optionBtnE4rs[1].gameObject.Hide();
                    if (voiceBtn == 1)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            }
            else if (obj.name == "2")//控制机构
            {               
                SpineManager.instance.DoAnimation(_spineOptionUI_0, "a", false);
                SpineManager.instance.DoAnimation(_spineOptionUI_1, "b", false);

                ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, false);                             
                OptionBtnAni(_spineOptionUI_2, _spineMobot, "c", 2,3, 6, () =>
                {
                    mono.StartCoroutine(WariteTimeCoroutine(() => 
                    {
                        _spineMobot.Show();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3);
                        SpineManager.instance.DoAnimation(_spineMobot, "jq4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(_spineMobot, "jq5", false, () =>
                            {
                                SpineManager.instance.DoAnimation(_spineMobot, "hg5", false);
                            });
                        });
                    }, 2.5f));                   
                },()=> 
                {
                    ShowOrHideOptionClickBtn(_optionBtnE4rs, true);
                    _optionBtnE4rs[2].gameObject.Hide();
                    if (voiceBtn == 1)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            }
        }        
        private void OptionBtnAni(GameObject obj, GameObject obj_1, string aniName, int aniNameIndex,int aniNameIndex1, int voiceIndex, Action callBack = null, Action callBack2 = null)
        {
            //obj.Show();
            SpineManager.instance.DoAnimation(obj, aniName + aniNameIndex, false, () =>
            {
                SpineManager.instance.DoAnimation(obj, aniName + aniNameIndex1, false, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, voiceIndex, callBack, callBack2));
                });
            });
        }
        private int voiceIndex;
        private void MobotPosBtnClickEvent(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2);
            ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, false);
            ShowOrHideOptionClickBtn(_optionBtnE4rs, false);
            SoundManager.instance.ShowVoiceBtn(false);

            if (obj.name == "0")//基座
            {
                SpineManager.instance.DoAnimation(_spineMobot, "bs3", false,()=> { MobotPosBtnAni(obj, "tx2", 2.0f); });
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                {
                    isPlaying = false;
                    ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, true);

                    if ((flag_1 & (1 << obj.transform.GetSiblingIndex())) == 0)
                    {
                        flag_1 += 1 << obj.transform.GetSiblingIndex();
                    }
                    if (flag_1 == (Mathf.Pow(2, _mobotPosBtn.transform.childCount) - 1))
                    {
                        //flag_1 = 0;
                        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);

                        _optionBtnE4rs[0].gameObject.Hide();

                        if (flag == (Mathf.Pow(2, _optionBtn.transform.childCount) - 1))
                        {
                            voiceBtn_1 = 1;
                        }
                    }

                    if (voiceBtn_1 == 1)
                    {
                        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);
                        _optionBtnE4rs[0].gameObject.Hide();
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));                
            }
            else if (obj.name == "1")//手臂
            {
                SpineManager.instance.DoAnimation(_spineMobot, "bs2", false, () => 
                {
                    mono.StartCoroutine(WariteTimeCoroutine(() => { MobotPosBtnAni(obj, "tx3", 0); }, 6.2f));
                });
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    isPlaying = false;
                    ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, true);

                    if ((flag_1 & (1 << obj.transform.GetSiblingIndex())) == 0)
                    {
                        flag_1 += 1 << obj.transform.GetSiblingIndex();
                    }
                    if (flag_1 == (Mathf.Pow(2, _mobotPosBtn.transform.childCount) - 1))
                    {                       
                        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);

                        _optionBtnE4rs[0].gameObject.Hide();

                        if (flag == (Mathf.Pow(2, _optionBtn.transform.childCount) - 1))
                        {
                            voiceBtn_1 = 1;
                        }
                    }


                    if (voiceBtn_1 == 1)
                    {
                        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);
                        _optionBtnE4rs[0].gameObject.Hide();
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));                
            }
            else if (obj.name == "2")//手部
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    isPlaying = false;
                    ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, true);

                    if ((flag_1 & (1 << obj.transform.GetSiblingIndex())) == 0)
                    {
                        flag_1 += 1 << obj.transform.GetSiblingIndex();
                    }
                    if (flag_1 == (Mathf.Pow(2, _mobotPosBtn.transform.childCount) - 1))
                    {
                        //flag_1 = 0;
                        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);

                        _optionBtnE4rs[0].gameObject.Hide();

                        if (flag == (Mathf.Pow(2, _optionBtn.transform.childCount) - 1))
                        {
                            voiceBtn_1 = 1;
                        }
                    }

                    if (voiceBtn_1 == 1)
                    {
                        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);
                        _optionBtnE4rs[0].gameObject.Hide();
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
                SpineManager.instance.DoAnimation(_spineMobot, "bs1", false, () =>
                {
                    mono.StartCoroutine(WariteTimeCoroutine(() => { MobotPosBtnAni(obj, "tx1", 0); }, 4.5f));
                });               
            }
            //if ((flag_1 & (1 << obj.transform.GetSiblingIndex())) == 0)
            //{
            //    flag_1 += 1 << obj.transform.GetSiblingIndex();
            //}
            //if (flag_1 == (Mathf.Pow(2, _mobotPosBtn.transform.childCount) - 1))
            //{
            //    ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, false);
            //    if (voiceIndex == -1)
            //    {
            //        voiceIndex = 1;
            //        ShowOrHideOptionClickBtn(_optionBtnE4rs, true);
            //    }                              

            //    _optionBtnE4rs[0].gameObject.Hide();

            //    if (flag == (Mathf.Pow(2, _optionBtn.transform.childCount) - 1))
            //    {
            //        voiceBtn_1 = 1;
            //    }
            //}
        }
        private void MobotPosBtnAni(GameObject obj, string aniName, float time)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 5);
            SpineManager.instance.DoAnimation(_spineMobot, aniName, false, () =>
            {
                mono.StartCoroutine(WariteTimeCoroutine(() =>
                {
                    SpineManager.instance.DoAnimation(_spineMobot, "hg5", false, () =>
                    {
                        //ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, true);
                        //if (voiceBtn_1 == 1)
                        //{
                        //    SoundManager.instance.ShowVoiceBtn(true);
                        //}
                    });
                }, time));
            });
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
                speaker = Max;
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
        IEnumerator WariteTimeCoroutine(Action method_2 = null, float len = 0)
        {

            yield return new WaitForSeconds(len);
            method_2?.Invoke();
        }
        IEnumerator WaitePlayVoiceCoroutine(Action method_2 = null, float len = 0, Action method_3 = null, float len1 = 0, Action method_4 = null, float len2 = 0)
        {

            yield return new WaitForSeconds(len);
            method_2?.Invoke();
            yield return new WaitForSeconds(len1);
            method_3?.Invoke();
            yield return new WaitForSeconds(len2);
            method_4?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);

            _spineOptionUI_0.Hide();
            _spineOptionUI_1.Hide();
            _spineOptionUI_2.Hide();
            ShowOrHideMobotPolClickBtn(_mobotPosBtnPCosllider, false);
            ShowOrHideOptionClickBtn(_optionBtnE4rs, false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7, null, null));


            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
