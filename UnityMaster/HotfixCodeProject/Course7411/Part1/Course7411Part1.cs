using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7411Part1
    {

        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _bell;
        private GameObject _aSpine;
        private GameObject _b3Spine;

        private GameObject _mask;      
        
        private Transform _spines;
        private Transform _onClicks;
        private Transform _onClicks1;

        private int _talkIndex;
        private List<string> _recordOnClicks;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _bell = curTrans.GetGameObject("Roles/Bell");
            _aSpine = curTrans.GetGameObject("Spines/a");
            _b3Spine = curTrans.GetGameObject("Spines/b3");
            _mask = curTrans.GetGameObject("mask");
      

            _spines = curTrans.Find("Spines");
            _onClicks = curTrans.Find("OnClicks");
            _onClicks1 = curTrans.Find("OnClicks1");
            GameInit();
            GameStart();
        }

        void GameInit()
        {
            _talkIndex = 1;
            _recordOnClicks = new List<string>();

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            HideVoice(); StopAllAudio(); StopAllCor();

            Input.multiTouchEnabled = false;
            DOTween.KillAll();

            _mask.Show();

            InitSpines(_spines); 

            AddEvents(_onClicks,OnClickEvent); AddEvents(_onClicks1, OnClickEvent1); RemoveEvent(_mask);

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
                    Process3_1();
                    break;
                case 3:
                    Process3_2();
                    break;
                case 4:
                    Process3_3();
                    break;
                case 5:
                    Process4();
                    break;
            }

            _talkIndex++;
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            _bell.Show();_onClicks.gameObject.Hide(); _onClicks1.gameObject.Hide();
            PlaySpine(_aSpine, "c2");
            Speck(_bell, 0,null,ShowVoice);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            PlaySpine(_aSpine, "b1", () => { PlaySpine(_aSpine, "b2", null, true); });
            PlaySpine(_b3Spine, "b3");
            Speck(_bell, 1, null, ()=> {  _bell.Hide();_mask.Hide(); _onClicks.gameObject.Show(); });
        }

        /// <summary>
        /// 流程3-1
        /// </summary>
        private void Process3_1()
        {
            _bell.Show(); _onClicks.gameObject.Hide();
            _b3Spine.GetComponent<SkeletonGraphic>().Initialize(true);
            //PlaySpine(_aSpine, "a");
            PlaySpine(_aSpine, "c1", () => { PlaySpine(_aSpine, "c2"); });
            Speck(_bell, 7, null, ShowVoice);
        }

        /// <summary>
        /// 流程3-2
        /// </summary>
        private void Process3_2()
        {
            PlaySpines(_aSpine,new string[3] { "e1","e3","e4"});
            Speck(_bell, 8, null, ShowVoice);
        }
        /// <summary>
        /// 流程3-2
        /// </summary>
        private void Process3_3()
        {
            _bell.Hide();_onClicks1.gameObject.Show();
            PlaySpine(_aSpine, "d1");
            _recordOnClicks.Clear();
        }

        /// <summary>
        ///  流程4
        /// </summary>
        private void Process4()
        {
            _bell.Show(); _onClicks1.gameObject.Hide();
            PlaySpine(_aSpine, "ding");
            Speck(_bell, 9);
        }

        private void OnClickEvent(GameObject go)
        {
            _mask.Show();HideVoice();
            var name = go.name;

            PlaySpine(_b3Spine, name);

            bool isCorrect = name == "ba" || name == "bc";

            if (isCorrect && !_recordOnClicks.Contains(name))
                _recordOnClicks.Add(name);

            int soundIndex = isCorrect ? ((name=="ba")? 5:6) : (UnityEngine.Random.Range(2,5));

            Speck(_bell, soundIndex,null,()=> {

                _mask.Hide();
                bool isOver =  _recordOnClicks.Count == 2;
                if (isOver)
                    ShowVoice();
            });
        }

        private void OnClickEvent1(GameObject go)
        {
           
            _mask.Show(); HideVoice();
            var name = go.name;

            if (!_recordOnClicks.Contains(name))
                _recordOnClicks.Add(name);

            PlaySpine(_aSpine, name,()=> { AddEvent(_mask, OnClickMask); });
         
            void OnClickMask(GameObject m)
            {
                RemoveEvent(m);
                PlaySpine(_aSpine, "d1",()=> { 
                    m.Hide();
                    bool isOver = _recordOnClicks.Count == 2;
                    if (isOver)
                        ShowVoice();
                });
            }
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
