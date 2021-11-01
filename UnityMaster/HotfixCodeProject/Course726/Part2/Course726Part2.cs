using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spine.Unity;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course726Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private bool _isPlaying = false;
        private Transform _mainPanel;
        private bool _isFinished = false;
        private bool _isDoingTheAni = false;
        private GameObject _currentClickTarget = null;
        private bool _isOnceGoingIn = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;
            _isFinished = false;
            talkIndex = 1;
            _isPlaying = false;
            _isDoingTheAni = false;
            _isOnceGoingIn = false;
            _currentClickTarget = null;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            InitProperty(curTrans);
            GameStart();
        }

        void InitProperty(Transform curTrans)
        {
            _mainPanel = curTrans.GetTransform("MainPanel");
            for (int i = 0; i < _mainPanel.childCount; i++)
            {
                GameObject targetAni = _mainPanel.GetChild(i).gameObject;
                targetAni.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(targetAni, targetAni.name + "0", false);
                GameObject target = targetAni.transform.GetChild(0).gameObject;
                PointerClickListener.Get(target).onClick = ClickEvent;
            }
        }

        void ClickEvent(GameObject obj)
        {
            if(_isPlaying==false)
                return;
           
            if (_isFinished == true)
                _isFinished = false;

            

            GameObject target = obj.transform.parent.gameObject;
            if (_isDoingTheAni == false)
            {
                _isDoingTheAni = true;
                _currentClickTarget = target;
               
            }
            else
            {
                if(_currentClickTarget!=target)
                    return;
            }
            switch (target.name)
            {
                case "a":
                   
                    
                    if (SpineManager.instance.GetCurrentAnimationName(target) == "a0")
                    {
                        
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                        SpineManager.instance.DoAnimation(target, "a1", false,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                                SpineManager.instance.DoAnimation(target, "a2", true);
                            });
                        if (_isOnceGoingIn==false)
                        {
                            SoundManager.instance.Stop("voice");
                            _isOnceGoingIn = true;
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                            {
                                SoundManager.instance.Stop("sound");
                                _isFinished = true;
                                SpineManager.instance.DoAnimation(target, "a0", false);
                                _isDoingTheAni = false;
                                _isOnceGoingIn = false;
                            }));
                        }

                        
                    }
                    else 
                    {
                        _isFinished = true;
                        SoundManager.instance.Stop("sound");
                        
                        //SpineManager.instance.DoAnimation(bell, "DAIJI");
                        SpineManager.instance.DoAnimation(target, "a0", false, () => _isFinished = false);
                        
                    }
                   

                    break;
                case "b":
                    
                    if (SpineManager.instance.GetCurrentAnimationName(target) == "b0")
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                        SpineManager.instance.DoAnimation(target, "b1", true);
                        if (_isOnceGoingIn == false)
                        {
                            _isOnceGoingIn = true;
                            SoundManager.instance.Stop("voice");
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, () =>
                            {
                                SoundManager.instance.Stop("sound");
                                _isFinished = true;
                                SpineManager.instance.DoAnimation(target, "b0", false);
                                _isDoingTheAni = false;
                                _isOnceGoingIn = false;
                            }));
                        }

                        
                    }
                    else 
                    {
                        _isFinished = true;
                        SoundManager.instance.Stop("sound");
                        
                        SpineManager.instance.DoAnimation(target, "b0", false,()=>_isFinished=false);
                       
                    }
                    break;
                case "c":
                   
                    if (SpineManager.instance.GetCurrentAnimationName(target) == "c0")
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                        SpineManager.instance.DoAnimation(target, "c1", false,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true);
                                SpineManager.instance.DoAnimation(target, "c2", true);
                            });
                        if (_isOnceGoingIn == false)
                        {
                            _isOnceGoingIn = true;
                            SoundManager.instance.Stop("voice");
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () =>
                            {
                                _isFinished = true;
                                SoundManager.instance.Stop("sound");

                                SpineManager.instance.DoAnimation(target, "c0", false);
                                _isDoingTheAni = false;
                                _isOnceGoingIn = false;
                            }));
                        }

                        
                    }
                    else
                    {
                        _isFinished = true;
                        SoundManager.instance.Stop("sound");
                        
                        SpineManager.instance.DoAnimation(target, "c0", false,()=>_isFinished=false);
                       
                    }
                    break;
                case "d":
                   
                    if (SpineManager.instance.GetCurrentAnimationName(target) == "d0")
                    {
                        //DoTheDAni(target);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, true);
                        SpineManager.instance.DoAnimation(target, "d1", true);
                        if (_isOnceGoingIn == false)
                        {
                            _isOnceGoingIn = true;
                            SoundManager.instance.Stop("voice");
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, () =>
                            {
                                _isFinished = true;
                                SoundManager.instance.Stop("sound");
                                SpineManager.instance.DoAnimation(target, "d0", false);
                                _isDoingTheAni = false;
                                _isOnceGoingIn = false;
                            }));

                        }

                        
                    }
                    else
                    {
                        _isFinished = true;
                        SoundManager.instance.Stop("sound");
                        SpineManager.instance.DoAnimation(target, "d0", false, () => _isFinished = false);

                    }
                    break;
            }
        }

       

        void DoTheDAni(GameObject target)
        {
            SpineManager.instance.DoAnimation(target, "d1", false,
                () =>
                {
                    if (_isFinished == false)
                    {
                        SpineManager.instance.DoAnimation(target, "d2", false,
                            () =>
                            {
                                if (_isFinished == false)
                                {
                                    SpineManager.instance.DoAnimation(target, "d3", false, () =>
                                    {
                                        if (_isFinished == false)
                                            DoTheDAni(target);
                                    });
                                }

                                
                            });
                    }

                   
                });
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => _isPlaying = true));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
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
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
