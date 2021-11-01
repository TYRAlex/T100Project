using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course741Part5
    {

        private MonoBehaviour _mono;
        private GameObject _curGo;
        private GameObject _max;
        private GameObject _heitanzhang;
        private GameObject _yaoyao;
        private GameObject _animationSpine;
     
        private Transform _spines;
        private Transform _roles;

        private int _talkIndex;

      
        private enum RoleType
        {
            Max,
            YiEn,
            HeiTanZhang,
            YaoYao,
        }


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            _max = curTrans.GetGameObject("Roles/Max");
            _heitanzhang = curTrans.GetGameObject("Roles/HeiTanZhang");
            _yaoyao = curTrans.GetGameObject("Roles/YaoYao");
            _animationSpine = curTrans.GetGameObject("Spines/animation");
         

            _spines = curTrans.Find("Spines");
            _roles = curTrans.Find("Roles");

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
       
            InitSpines(_spines, null); InitSpines(_roles, null);

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
                    Process3();
                    break;
            }
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            PlaySpine(_animationSpine, "animation2");
            Speck(_max, 0,null, Process2);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            PlaySpine(_animationSpine, "animation3");
            Speck(_max, 0, null, ShowVoice);

          
        }


        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {

            PlaySpine(_yaoyao, "cx"); PlaySpine(_heitanzhang, "cx");

            TogetherSpeck();

            //黑探长和摇摇同时说
            void TogetherSpeck()
            {
                Speck(_yaoyao, 0, null, null, RoleType.YaoYao);
                Speck(_heitanzhang, 0, null, HeiTanZhangSpeck, RoleType.HeiTanZhang);
            }

            //黑探长说
            void HeiTanZhangSpeck()
            {
                Speck(_heitanzhang, 0, null, YaoYaoSpeck, RoleType.YaoYao);
            }

            //摇摇说
            void YaoYaoSpeck()
            {
                Speck(_yaoyao, 0, null, MaxSpeck, RoleType.YaoYao);
            }

            //Max说
            void MaxSpeck()
            {
                Speck(_max, 0);
            }

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
        private void Speck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Max, SoundManager.SoundType type = SoundManager.SoundType.SOUND) { _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType)); }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Max, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
            {
                case RoleType.Max:
                    daiJi = "daiji"; speak = "daijishuohua";
                    break;
                case RoleType.YiEn:
                    daiJi = "jqr"; speak = "jqr2";
                    break;
                case RoleType.HeiTanZhang:
                case RoleType.YaoYao:
                    daiJi = "dj"; speak = "sh";
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
