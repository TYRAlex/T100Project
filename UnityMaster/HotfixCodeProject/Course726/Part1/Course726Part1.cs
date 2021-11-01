using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spine.Unity;
using unitycoder_MobilePaint;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course726Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private GameObject _worm1;
        private GameObject _worm2;
        private GameObject _goBackButton;

        private int _worm1TexIndex = 0;
        private int _worm2TexIndex = 0;
        
        private bool _isPressButton=false;
        private bool _isPressBackButton = false;

        private List<GameObject> _wormClickLeftList;

        private float _timer = 0;

       

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            SoundManager.instance.StopAllSoundCoroutine();
            SoundManager.instance.Stop();
            bell = curTrans.Find("bell").gameObject;

            talkIndex = 1;

            _isPressButton = true;
            _isPressBackButton = false;
            _worm1 = curTrans.Find("GamePanel/Worm1").gameObject;
            _worm2 = curTrans.Find("GamePanel/Worm2").gameObject;
            _wormClickLeftList=new List<GameObject>();
            _wormClickLeftList.Add(_worm1);
            _wormClickLeftList.Add(_worm2);
            _goBackButton = curTrans.Find("GamePanel/GoBackButton").gameObject;
            _timer = 0;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
          
            PointerClickListener.Get(_worm1.transform.GetGameObject("Worm1")).onClick = WormMove;
            PointerClickListener.Get(_worm2.transform.GetGameObject("Worm2")).onClick = WormMove;
          
            PointerClickListener.Get(_goBackButton).onClick = UIGoBackMove;
          
            GameStart();
            
            //Debug.LogError("Test");
        }

        void WormMove(GameObject obj)
        {
            if (obj.name == "Worm1" && _isPressButton == false)
            {
                Debug.Log("Worm1");
                _isPressButton = true;
                SoundManager.instance.ShowVoiceBtn(false);
                _wormClickLeftList.Remove(_worm1);
                if (SpineManager.instance.GetCurrentAnimationName(_worm2) == "b1")
                {
                    SpineManager.instance.DoAnimation(_worm2, "b5", false);
                }
                _worm1.transform.SetAsLastSibling();
                _goBackButton.transform.SetAsLastSibling();
               
                _isPressBackButton = true;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, (() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    SpineManager.instance.DoAnimation(_worm1, "a0", false,
                        (() =>
                        {
                            
                            SpineManager.instance.DoAnimation(_worm1, "a1", false);
                        }));
                }), () =>
                {
                    _isPressBackButton = false;
                }));
            }
            else if(obj.name == "Worm2"&&_isPressButton==false)
            {
                Debug.Log("Worm2");
                _isPressButton = true;
                _isPressBackButton = true;
                SoundManager.instance.ShowVoiceBtn(false);
                _wormClickLeftList.Remove(_worm2);
                if (SpineManager.instance.GetCurrentAnimationName(_worm1) == "a1")
                {
                    SpineManager.instance.DoAnimation(_worm1, "a5", false);
                }
                _worm2.transform.SetAsLastSibling();
                _goBackButton.transform.SetAsLastSibling();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, (() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    SpineManager.instance.DoAnimation(_worm2, "b0", false,
                        (() => SpineManager.instance.DoAnimation(_worm2, "b1", false)));
                }), () =>
                {
                    _isPressBackButton = false;
                }));
            }
        }

       

        void UIGoBackMove(GameObject o)
        {
            Debug.Log("返回" + o.name);
            if (_isPressBackButton == false)
            {
                _isPressBackButton = true;
                if (SpineManager.instance.GetCurrentAnimationName(_worm1) == "a1")
                {
                    SpineManager.instance.DoAnimation(_worm1, "a5", false,()=>
                    {
                        _isPressButton = false;
                        _isPressBackButton = false;
                        _goBackButton.transform.SetAsFirstSibling();
                        JudgeWormListIfFinish();
                    });
                }
                if (SpineManager.instance.GetCurrentAnimationName(_worm2) == "b1")
                {
                    SpineManager.instance.DoAnimation(_worm2, "b5", false,()=>
                    {
                        _isPressButton = false;
                        _isPressBackButton = false;
                        _goBackButton.transform.SetAsFirstSibling();
                        JudgeWormListIfFinish();
                    });
                }
            }

            
        }

        void JudgeWormListIfFinish()
        {
            if (_wormClickLeftList.Count <= 0)
            {
                //_isPressButton = true;
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        void GameStart()
        {
            // mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,null, () =>
            // {
            //     SpineManager.instance.DoAnimation(_skeletonGraphic, "a0", false, (() =>
            //     {
            //         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
            //         Debug.LogError("11");
            //     }));
            // }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            _worm1.GetComponent<SkeletonGraphic>().Initialize(true);
            _worm2.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_worm1, "a7", false);
            SpineManager.instance.DoAnimation(_worm2, "b7", false, (() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, (() =>
                {
                    SpineManager.instance.DoAnimation(bell, "DAIJI");
                    SoundManager.instance.ShowVoiceBtn(true);
                })));
            }));

        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            _timer = ind;
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //Debug.LogError("执行下一个步骤");
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                    {
                        _worm1.transform.SetAsLastSibling();

                        SpineManager.instance.DoAnimation(_worm1, "a0", false,
                            () =>
                            {
                                mono.StartCoroutine(DelayTimeAndExcuteAction(0.4f,
                                    () => SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1)));
                            });
                        mono.StartCoroutine(DelayTimeAndExcuteAction(_timer * (10f / 50f), () =>
                        {
                            
                            SpineManager.instance.DoAnimation(_worm1, "a8", false, () =>
                            {
                                SpineManager.instance.DoAnimation(_worm1, "a5", false);
                            });
                        }));
                        
                        mono.StartCoroutine(DelayTimeAndExcuteAction(_timer * (20f / 50f),
                            () =>
                            {
                                
                                SpineManager.instance.DoAnimation(_worm2, "b0", false,()=> SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3));
                                _worm2.transform.SetAsLastSibling();
                                mono.StartCoroutine(DelayTimeAndExcuteAction(_timer * (12f / 50f),
                                    () =>
                                    {
                                        
                                        SpineManager.instance.DoAnimation(_worm2, "b8", false);
                                    }));

                            }));
                        
                    }, () =>
                    {
                       
                        SpineManager.instance.DoAnimation(_worm2, "b5", false);
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                    
                    break;
                case 2:
                    Debug.LogError("执行下两个步骤");
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,4,null, () =>
                    {
                        _isPressButton = false;
                    }));
                    
                    break;
                case 3:
                    Debug.LogError("执行下三个步骤");
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6));

                    _isPressButton = true;
                    break;
                case 4:
                    Debug.LogError("执行下四个步骤");
                   
                    break;

            }
            talkIndex++;
        }

        // void AddBtn()
        // {
        //     Util.AddBtnClick();
        // }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
        
        

        IEnumerator DelayTimeAndExcuteAction(float timer,Action e)
        {
            yield return new WaitForSeconds(timer);
            if (e != null)
                e();
        }
    }
}
