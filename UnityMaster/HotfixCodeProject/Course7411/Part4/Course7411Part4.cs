using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7411Part4
    {

        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _bell;
        private GameObject _spine0;   
        private GameObject _mask;
        private Transform _spines;

    
        private int _talkIndex;

        private RawImage _bgRImg;

        private BellSprites _bgSprites;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _bell = curTrans.GetGameObject("Roles/Bell");
            _spine0 = curTrans.GetGameObject("Spines/0");
          
            _mask = curTrans.GetGameObject("mask");

            _bgRImg = curTrans.GetRawImage("BG/bg");
            _bgSprites = curTrans.Find("BG/bg").GetComponent<BellSprites>();


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

            _mask.Show();

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
                    Process2();
                    break;
                case 2:
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
            BgUpdate(0);
            Speck(_bell, 0,null,ShowVoice);
            PlaySpine(_spine0, "0");
            Delay(2, () => { PlaySpines(_spine0, new string[2] { "1", "2" }); });
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {    
            BgUpdate(1);
            Speck(_bell, 1, null, ShowVoice);
            PlaySpine(_spine0, "3");

            Delay(3, () => {  PlaySpine(_spine0, "4"); });

            Delay(12,()=> { BgUpdate(0);  _spine0.GetComponent<SkeletonGraphic>().Initialize(true);  PlaySpine(_spine0, "8"); });

            Delay(22, () => { PlaySpine(_spine0, "9"); });

            Delay(25f, () => { PlaySpine(_spine0, "5"); });

            Delay(32f, () => { PlaySpine(_spine0, "6"); });
        }

        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {           
            Speck(_bell, 2);
        }

        //更新背景
        private void BgUpdate(int index, Action callBack = null)
        {
            _bgRImg.texture = _bgSprites.texture[index];
            callBack?.Invoke();
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
        private void Speck(GameObject go, int index, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND) { _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend)); }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            daiJi = "DAIJI"; speak = "DAIJIshuohua";

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

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            var e4Rs = Gets<Empty4Raycast>(parent);

            for (int i = 0; i < e4Rs.Length; i++)
            {
                var go = e4Rs[i].gameObject;
                RemoveEvent(go);
                AddEvent(go, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
    }
}
