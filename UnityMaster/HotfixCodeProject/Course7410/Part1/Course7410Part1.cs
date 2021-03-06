using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7410Part1
    {

        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;
        private GameObject _animationSpine;
        private GameObject _mask;

        private Transform _spines;
        private Transform _onClicks;
       
        private int _talkIndex;

        private List<string> _recordOnClicks;
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _max = curTrans.GetGameObject("Roles/Max");
            _animationSpine = curTrans.GetGameObject("Spines/animation");
            _mask = curTrans.GetGameObject("mask");


            _spines = curTrans.Find("Spines");
            _onClicks = curTrans.Find("OnClicks");
      
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

            _mask.Hide();

            InitSpines(_spines);

            AddEvents(_onClicks, OnClickEvent); 

        }



        void GameStart()
        {
            Process1();
        }


        /// <summary>
        /// ??????????
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
        /// ????1
        /// </summary>
        private void Process1()
        {
            _max.Show();
            Speck(_max, 0,null,ShowVoice);
            PlaySpines(_animationSpine, new string[3] { "animation", "animation2", "animation3"});
        }

        /// <summary>
        /// ????2
        /// </summary>
        private void Process2()
        {
            _max.Hide();
            PlaySpine(_animationSpine, "animation");
        }

        /// <summary>
        /// ????3
        /// </summary>
        private void Process3()
        {
            _max.Show();
            PlaySpines(_animationSpine, new string[3] { "animation", "animation2", "animation3" });
            PlaySpine(_spines.Find("g5").gameObject,"g5");
            PlaySpine(_spines.Find("zie1").gameObject, "zie1");
            Speck(_max, 0);
        }

        private void OnClickEvent(GameObject go)
        {
            _mask.Show();HideVoice();
            var name = go.name;
            var guang = _spines.Find(name).gameObject;
            var zi = _spines.Find(go.transform.GetChild(0).name).gameObject;
            var soundIndex = int.Parse(go.transform.GetChild(1).name);

            PlaySpine(guang, name);
            PlaySpine(zi, zi.name);

            if (!_recordOnClicks.Contains(name))
                _recordOnClicks.Add(name);

            bool isOver = _recordOnClicks.Count == 7;

            Speck(_max, soundIndex, null, () => {
                zi.GetComponent<SkeletonGraphic>().Initialize(true);
                _mask.Hide();

                if (isOver)
                   ShowVoice();
            });
        }

      

        //????Spines
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

        // ????Spine 
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }


        //??ʼ??Spines
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

        // ??ʾ??????
        private void ShowVoice() { SoundManager.instance.ShowVoiceBtn(true); }

        // ??????????
        private void HideVoice() { SoundManager.instance.ShowVoiceBtn(false); }

        // ֹͣ??????Ƶ
        private void StopAllAudio() { SoundManager.instance.StopAudio(); }

        //ֹͣ????Э??
        private void StopAllCor() { _mono.StopAllCoroutines(); }

        // ????
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

        //?ӳ?
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
            var e4Rs = Gets<CustomImage>(parent);

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
