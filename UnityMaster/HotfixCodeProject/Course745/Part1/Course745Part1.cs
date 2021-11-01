using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course745Part1
    {

        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;
        private GameObject _spine3;
        private GameObject _mask;      
        
        private Transform _spines;
        private Transform _onClicks;

        private int _talkIndex;
      

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _max = curTrans.GetGameObject("Max");       
            _spine3 = curTrans.GetGameObject("Spines/3");          
            _mask = curTrans.GetGameObject("mask");
      

            _spines = curTrans.Find("Spines");
            _onClicks = curTrans.Find("OnClicks");

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

            InitSpines(_spines); AddEvents(_onClicks,OnClickEvent);


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
                 
                    break;
            }
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            PlaySpines(_spine3, new string[3] { "3","1","2"});
            Speck(_max, 0, null, Process2);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            Speck(_max, 0, null, ()=> { _mask.Hide(); });
        }

        private void OnClickEvent(GameObject go)
        {
            _mask.Show();
            var name = go.name;
            var ziName = go.transform.GetChild(0).name;

            var soundIndex =int.Parse(go.transform.GetChild(1).name);

            var guangGo = _spines.Find(name).gameObject;
            var ziGo = _spines.Find(ziName).gameObject;

            PlaySpine(guangGo, guangGo.name);
            Delay(1, () => { PlaySpine(ziGo, ziGo.name); });

            Speck(_max, soundIndex, null,()=> { _mask.Hide(); });

        }

        //播放Spines
        private void PlaySpines(GameObject go,string [] names,Action callBack=null)
        {
            _mono.StartCoroutine(IEPlaySpines(go,names,callBack));
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

            daiJi = "daiji"; speak = "daijishuohua";

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
