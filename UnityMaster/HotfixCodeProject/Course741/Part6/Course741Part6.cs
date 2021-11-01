using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course741Part6
    {

        private MonoBehaviour _mono;
        private GameObject _curGo;
        private GameObject _max;
        private GameObject _bell;
        private GameObject _yien;
        private GameObject _maxyingzi;
        private GameObject _bellyingzi;
        private GameObject _yeye;
        private GameObject _animationSpine;
        private GameObject _spine1;

        private RawImage _bgRImg;

        private BellSprites _bgSprites;

        private Transform _spines;
        private Transform _roles;

        private int _talkIndex;


        private enum RoleType
        {           
            Max,
            YiEn,
            HeiTanZhang,
            YaoYao,
            Bell,
            Grandpa
        }


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            _max = curTrans.GetGameObject("Roles/Max");
            _bell = curTrans.GetGameObject("Roles/Bell");
            _yien = curTrans.GetGameObject("Roles/YiEn");
            _maxyingzi = curTrans.GetGameObject("Roles/MaxYingZi");
            _bellyingzi = curTrans.GetGameObject("Roles/BellYingZi");
            _yeye = curTrans.GetGameObject("Roles/YeYe");
            _animationSpine = curTrans.GetGameObject("Spines/animation");
            _spine1 = curTrans.GetGameObject("Spines/1");

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
                    Process2();
                    break;
                case 2:
                    Process3();
                    break;
                case 3:
                    Process4();
                    break;
                case 4:
                    Process5();
                    break;
            }
            _talkIndex++;
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {

            _maxyingzi.Show(); _bellyingzi.Show(); _bell.Show(); _yien.Show(); _max.Show(); _yeye.Show();

            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(496, -204);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.3f, 0.3f);

            _bell.GetComponent<RectTransform>().anchoredPosition = new Vector2(-664, -215);
            _bell.GetComponent<RectTransform>().localScale = new Vector2(0.35f, 0.35f);

            _yien.GetComponent<RectTransform>().anchoredPosition = new Vector2(551, -215);
            _yien.GetComponent<RectTransform>().localScale = new Vector2(0.65f, 0.65f);

            _yeye.GetComponent<RectTransform>().anchoredPosition = new Vector2(-616, -248);
            _yeye.GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);

            //博士说话
            Speck(_yeye, 0, null, ShowVoice,RoleType.Grandpa);
            PlaySpine(_animationSpine, "animation", null,true);
            PlaySpine(_bell, "DAIJI",null,true);
            PlaySpine(_max, "daiji", null, true);
            PlaySpine(_yien, "jqr", null, true);         
            BgUpdate(0);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            _maxyingzi.Hide(); _bellyingzi.Hide();_bell.Hide(); _yien.Hide();_yeye.Hide(); _max.Show();

            BgUpdate(1);

            PlaySpine(_spine1, "1");
            _animationSpine.GetComponent<SkeletonGraphic>().Initialize(true);

            Speck(_max, 0, null, ShowVoice,RoleType.Max);

            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(-667, -580);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);
        }


        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {

           _bell.Hide(); _yien.Show(); _max.Hide();_yeye.Hide();
            PlaySpine(_spine1, "2");
            Speck(_yien, 0, null, ShowVoice,RoleType.YiEn);

            _yien.GetComponent<RectTransform>().anchoredPosition = new Vector2(-917, -565);
            _yien.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        }

        /// <summary>
        /// 流程4
        /// </summary>
        private void Process4()
        {

            _bell.Show(); _yien.Hide(); _max.Hide();_yeye.Hide();
            PlaySpine(_spine1, "3");
            Speck(_bell, 0, null, ShowVoice, RoleType.Bell);

            _bell.GetComponent<RectTransform>().anchoredPosition = new Vector2(-675, -575);
            _bell.GetComponent<RectTransform>().localScale = new Vector2(0.6f, 0.6f);

        }

        /// <summary>
        /// 流程5
        /// </summary>
        private void Process5()
        {

            _spine1.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(_animationSpine, "animation",null,true);
            _maxyingzi.Show(); _bellyingzi.Show(); _bell.Show(); _yien.Show(); _max.Show();_yeye.Show();

            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(496, -204);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.3f, 0.3f);

            _bell.GetComponent<RectTransform>().anchoredPosition = new Vector2(-664, -215);
            _bell.GetComponent<RectTransform>().localScale = new Vector2(0.35f, 0.35f);

            _yien.GetComponent<RectTransform>().anchoredPosition = new Vector2(551, -215);
            _yien.GetComponent<RectTransform>().localScale = new Vector2(0.65f, 0.65f);

            _yeye.GetComponent<RectTransform>().anchoredPosition = new Vector2(-616, -248);
            _yeye.GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);



            //博士说话
            Speck(_yeye, 0, null, null,RoleType.Grandpa);

            PlaySpine(_animationSpine, "animation", null, true);
            PlaySpine(_bell, "DAIJI", null, true);
            PlaySpine(_max, "daiji", null, true);
            PlaySpine(_yien, "jqr", null, true);
           
            BgUpdate(0);
        }

    


        //更新背景
        private void BgUpdate(int index, Action callBack = null)
        {
            _bgRImg.texture = _bgSprites.texture[index];
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
                case RoleType.Bell:
                    daiJi = "DAIJI"; speak = "DAIJIshuohua";
                    break;
                case RoleType.Grandpa:
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
