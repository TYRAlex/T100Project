using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7410Part5
    {

        public enum RoleType 
        {
            Max,
            FeiLunXiaoZi
        }


        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;
        private GameObject _feilunxiaozi;
        private GameObject _animationSpine;
        private GameObject _maxYingZi;
        private Transform _spines;
        private Transform _roles;

        private int _talkIndex;

        private RawImage _bgRImg;

        private BellSprites _bgSprites;


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _max = curTrans.GetGameObject("Roles/Max");
            _feilunxiaozi = curTrans.GetGameObject("Roles/FeiLunXiaoZi");
            _animationSpine = curTrans.GetGameObject("Spines/animation");
            _maxYingZi = curTrans.GetGameObject("Roles/MaxYingZi");

            _bgRImg = curTrans.GetRawImage("BG/bg");
            _bgSprites = curTrans.Find("BG/bg").GetComponent<BellSprites>();


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
            _roles.SetSiblingIndex(1);
            _spines.SetSiblingIndex(2);
            _feilunxiaozi.Hide(); _maxYingZi.Show();
            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(-221,-271);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.3f,0.3f);

            BgUpdate(0);
            Speck(_max, 0, null, ShowVoice);
            PlaySpines(_animationSpine, new string[3] { "che1", "che2","chedaiji"});
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            _feilunxiaozi.Show();
            Speck(_feilunxiaozi, 0, null, ()=> { 
                _feilunxiaozi.Hide();
                PlaySpines(_animationSpine, new string[2] { "chedzou", "chezou2"});
                Delay(3, () => {Delay(PlaySound(0),ShowVoice); });
            },RoleType.FeiLunXiaoZi);
        }

        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {
            _roles.SetSiblingIndex(2);
            _spines.SetSiblingIndex(1);
            _maxYingZi.Hide();
            BgUpdate(1);
            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(-666, -576);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);

            PlaySpines(_animationSpine, new string[5] { "animation", "animation2", "animation3", "animation4", "animation5" });
            Speck(_max, 0);
        }

        //更新背景
        private void BgUpdate(int index)
        {
            _bgRImg.texture = _bgSprites.texture[index];           
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


        private float PlaySound(int index)
        {
           return SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index);
        }

        // 讲话
        private void Speck(GameObject go, int index, Action specking = null, Action speckend = null,RoleType roleType = RoleType.Max,SoundManager.SoundType type = SoundManager.SoundType.SOUND) { _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend,roleType)); }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Max, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
            {
                case RoleType.Max:
                    daiJi = "daiji"; speak = "daijishuohua";
                    break;
                case RoleType.FeiLunXiaoZi:
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
