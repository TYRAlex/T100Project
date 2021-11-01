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
    public class Course9110Part2
    {
        
        public enum E_Process
        {
            Part1,
            Part2,
            Part3
        }
        
        public enum E_ButtonType
        {
            Next,
            Last,
            Both
        }

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        // private GameObject Bg;
        // private BellSprites bellTextures;
        // private GameObject btnTest;

        private GameObject _mainTarget;

        private E_Process _currentProcess;

        private GameObject _next;
        private GameObject _last;
        

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            // btnTest = curTrans.Find("btnTest").gameObject;
            // Util.AddBtnClick(btnTest, ReStart);
            // btnTest.SetActive(false);
            GameObjectInialized(curTrans);
            ReStart();
        }
        
        void GameObjectInialized(Transform curTrans)
        {
            ResetBGM();
            _currentProcess = E_Process.Part1;
            _mainTarget = curTrans.GetGameObject("MainTarget");
            _next = curTrans.GetGameObject("Next");
            _last = curTrans.GetGameObject("Last");
            Util.AddBtnClick(_next,GotoNext);
            Util.AddBtnClick(_last, ReturnToLast);
            // _next.onClick.AddListener(GotoNext);
            // _last.onClick.AddListener(ReturnToLast);
            SetLastAndNextButtonVisble(E_ButtonType.Both, false);
        }

        void ResetBGM()
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        void ReStart()
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            
            bell.transform.SetAsFirstSibling();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
           
        }

        void GameStart()
        {
            Part1Start();
           
        }

        void SetLastAndNextButtonVisble(E_ButtonType type,bool isShow)
        {
            switch (type)
            {
                case E_ButtonType.Last:
                    if (_last.gameObject.activeSelf != isShow)
                        _last.gameObject.SetActive(isShow);
                    break;
                case E_ButtonType.Next:
                    if (_next.gameObject.activeSelf != isShow)
                        _next.gameObject.SetActive(isShow);
                    break;
                case E_ButtonType.Both:
                    if (_last.gameObject.activeSelf != isShow)
                        _last.gameObject.SetActive(isShow);
                    if (_next.gameObject.activeSelf != isShow)
                        _next.gameObject.SetActive(isShow);
                    break;
            }
        }



        void GotoNext(GameObject go)
        {
            if (_currentProcess == E_Process.Part1)
            {
                Part2Start();
            }
            else if (_currentProcess == E_Process.Part2)
            {
                Part3Start();
            }
            else if (_currentProcess == E_Process.Part3)
            {
                Part1Start();
            }
        }

        void ReturnToLast(GameObject go)
        {
            if (_currentProcess == E_Process.Part1)
            {
                Part3Start();
            }
            else if (_currentProcess == E_Process.Part2)
            {
                Part1Start();
            }
            else if (_currentProcess == E_Process.Part3)
            {
                Part2Start();
            }
        }

        void Part1Start()
        {
            _currentProcess = E_Process.Part1;
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    SetLastAndNextButtonVisble(E_ButtonType.Both, false);
                    SpineManager.instance.DoAnimation(_mainTarget, "jing", false);
                },
                () => SetLastAndNextButtonVisble(E_ButtonType.Next, true)));
        }

        void Part2Start()
        {
            _currentProcess = E_Process.Part2;
            talkIndex = 1;
            SetLastAndNextButtonVisble(E_ButtonType.Both, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () => SpineManager.instance.DoAnimation(_mainTarget, "r1",false),
                () => SoundManager.instance.ShowVoiceBtn(true)));
        }

        void Part3Start()
        {
            _currentProcess = E_Process.Part3;
            SetLastAndNextButtonVisble(E_ButtonType.Both, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () => SpineManager.instance.DoAnimation(_mainTarget, "y1", false),
                () => SoundManager.instance.ShowVoiceBtn(true)));
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
            switch (talkIndex)
            {
                case 1:
                    //Part2:第二步
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                        () => SpineManager.instance.DoAnimation(_mainTarget, "r3",false),
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;
                case 2:
                    //Part2:第三步
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            SpineManager.instance.DoAnimation(_mainTarget, "ys1", false);
                        },
                        () =>
                        {
                            //SoundManager.instance.ShowVoiceBtn(true);
                            SetLastAndNextButtonVisble(E_ButtonType.Both, true);
                        }));
                    break;
                case 3:
                    //Part3:第二步
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                        () => SpineManager.instance.DoAnimation(_mainTarget, "y4",false),
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;
                case 4:
                    //Part3:第三步
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            SpineManager.instance.DoAnimation(_mainTarget, "rs1", false);
                        }, () =>
                        {
                            SetLastAndNextButtonVisble(E_ButtonType.Last, true);
                        }));
                    break;
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
