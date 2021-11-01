using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7410Part2
    {

        public enum RoleType
        {
            Max,
            YiEN,
        }

        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;
        private GameObject _yien;
        private GameObject _animationSpine;
        private GameObject _zig1Spine;
      
        private GameObject _mask;

        private Transform _spines;
     

        private int _talkIndex;

    
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _max = curTrans.GetGameObject("Roles/Max");
            _yien = curTrans.GetGameObject("Roles/YiEn");
            _animationSpine = curTrans.GetGameObject("Spines/animation");
            _zig1Spine = curTrans.GetGameObject("Spines/zig1");
         
            _mask = curTrans.GetGameObject("mask");


            _spines = curTrans.Find("Spines");
            

            GameInit();
            GameStart();
        }

        void GameInit()
        {
            _talkIndex = 1;
           

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            HideVoice(); StopAllAudio(); StopAllCor();

            Input.multiTouchEnabled = false;
            DOTween.KillAll();

            _mask.Hide();

            InitSpines(_spines);

           

        }



        void GameStart()
        {
            Process1();
        }


        /// <summary>
        /// 点击语音健
        /// </summary>
        private void TalkClick()
        {
            HideVoice();
            SoundManager.instance.PlayClip(9);

            switch (_talkIndex)
            {
                case 1:
                    Speak(_max, 0, null, ShowVoice);
                    break;
                case 2:
                    Process2();
                    break;
                case 3:
                    Process3();
                    break;
            }

            _talkIndex++;
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            _max.Show();_yien.Hide();
            Speak(_max, 0, null, ShowVoice);
            PlaySpines(_animationSpine, new string[3] { "animation", "animation4", "animation5" });
            Delay(3, () => { PlaySpine(_zig1Spine, "zig1"); });
        }

       

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            _max.Hide(); _yien.Show();
            PlaySpines(_zig1Spine, new string[3] { "kuang", "kuang2", "kuang3" });
            Speak(_yien, 0, null, YiEnSpeck2, RoleType.YiEN);
          
            void YiEnSpeck2()
            {
                Speak(_yien, 0, null, ShowVoice, RoleType.YiEN);
                _zig1Spine.GetComponent<SkeletonGraphic>().Initialize(true);
                PlaySpines(_animationSpine, new string[2] { "animation6", "animation7"});
            }
        }

        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {
            _max.Show(); _yien.Hide();
            Speak(_max, 0);
            PlaySpines(_animationSpine, new string[2] { "animation6", "animation7" });
        }



        //播放Spines
        private void PlaySpines(GameObject go, string[] names, Action callBack = null)
        {
            _mono.StartCoroutine(IEPlaySpines(go, names, callBack));
        }

        private IEnumerator IEPlaySpines(GameObject go, string[] names, Action callBack = null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                var delay = PlaySpine(go, names[i]);
                yield return new WaitForSeconds(delay);
            }
            callBack?.Invoke();
        }

        // 播放Spine 
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }


        //初始化Spines
        private void InitSpines(Transform parent, Action<SkeletonGraphic> callBack = null)
        {
            var spines = Gets<SkeletonGraphic>(parent);
            for (int i = 0; i < spines.Length; i++)
            {
                var spine = spines[i];
                spine.Initialize(true);
                callBack?.Invoke(spine);
            }
        }

        private T[] Gets<T>(Transform parent) { return parent.GetComponentsInChildren<T>(true); }

        // 显示语音键
        private void ShowVoice() { SoundManager.instance.ShowVoiceBtn(true); }

        // 隐藏语音键
        private void HideVoice() { SoundManager.instance.ShowVoiceBtn(false); }

        // 停止所有音频
        private void StopAllAudio() { SoundManager.instance.StopAudio(); }

        //停止所有协程
        private void StopAllCor() { _mono.StopAllCoroutines(); }

        // 讲话
        private void Speak(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Max, SoundManager.SoundType type = SoundManager.SoundType.SOUND) { _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend,roleType)); }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Max, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
            {
                case RoleType.Max:
                    daiJi = "daiji"; speak = "daijishuohua";
                    break;
                case RoleType.YiEN:
                    daiJi = "daiji"; speak = "speak";
                    break;            
            }

        

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //延迟
        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }
        private IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();

        }

    }
}
