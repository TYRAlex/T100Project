using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course745Part5
    {

        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;
        private GameObject _caiwa;
        private GameObject _xiaohongtou;
        private GameObject _mask;
        private GameObject _a0Spine;
        private GameObject _b0Spine;
        private GameObject _maxyingzi;
        private GameObject _lnn;
        private Transform _spines;


        private RawImage _bgRImg;

        private BellSprites _bgSprites;

        private int _talkIndex;


        /// <summary>
        /// 角色类型
        /// </summary>
        public enum RoleType
        {
            /// <summary>
            /// 贝尔
            /// </summary>
            Bell,

            /// <summary>
            /// Max
            /// </summary>
            Max,

            /// <summary>
            /// Amy
            /// </summary>
            Amy,

            /// <summary>
            /// Max的爷爷
            /// </summary>
            MaxGrandpa,

            /// <summary>
            /// 伊恩
            /// </summary>
            YiEn,

            /// <summary>
            /// 扭蛋
            /// </summary>
            NiuDan,

            /// <summary>
            /// 飞轮小子
            /// </summary>
            FeiLunXiaoZi,

            /// <summary>
            /// 黑探长
            /// </summary>
            HeiTanZhang,

            /// <summary>
            /// 摇摇
            /// </summary>
            YaoYao,

            /// <summary>
            /// 闪电
            /// </summary>
            ShanDian,

            /// <summary>
            /// 小红头
            /// </summary>
            XiaoHongTou,

            /// <summary>
            /// 彩哇
            /// </summary>
            CaiWa
        }

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _max = curTrans.GetGameObject("Roles/max");
            _caiwa = curTrans.GetGameObject("Roles/caiwa");
            _xiaohongtou = curTrans.GetGameObject("Roles/xiaohongtou");
            _mask = curTrans.GetGameObject("mask");
            _a0Spine = curTrans.GetGameObject("Spines/a0");
            _b0Spine = curTrans.GetGameObject("Spines/b0");
            _maxyingzi = curTrans.GetGameObject("Roles/maxyingzi");
            _lnn = curTrans.GetGameObject("Spines/lnn");

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
                case 3:
                    Process3_1();
                    break;
                case 4:
                    Process3_2();
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
            _max.Show(); _maxyingzi.Show(); _caiwa.Hide(); _xiaohongtou.Hide();

            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(-185, -382);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.45f, 0.45f);

            _maxyingzi.GetComponent<RectTransform>().anchoredPosition = new Vector2(-229, -429);
            _maxyingzi.GetComponent<RectTransform>().localScale = new Vector2(0.8f, 0.5f);


            _caiwa.GetComponent<RectTransform>().anchoredPosition = new Vector2(-713, -466);
            _caiwa.GetComponent<RectTransform>().localScale = new Vector2(0.7f, 0.7f);

            _xiaohongtou.GetComponent<RectTransform>().anchoredPosition = new Vector2(-392, -450);
            _xiaohongtou.GetComponent<RectTransform>().localScale = new Vector2(0.6f, 0.6f);

            BgUpdate(0);
            PlaySpine(_lnn, "lnn", null, true);
            Speck(_max, 0, null, ShowVoice);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {

            CaiWa();
            void CaiWa()
            {
                _caiwa.Show();
                Speck(_caiwa, 0, null, XiaoHongTou, RoleType.CaiWa);
            }

            void XiaoHongTou()
            {
                _xiaohongtou.Show();
                Speck(_xiaohongtou, 0, null, MaxSpeak, RoleType.XiaoHongTou);
            }

            void MaxSpeak()
            {
                Speck(_max, 0, null, ShowVoice);
            }

        }

        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {
            _max.Hide(); _maxyingzi.Hide();
            BgUpdate(1);
            _lnn.GetComponent<SkeletonGraphic>().Initialize(true);


            PlaySpine(_a0Spine, "a0"); PlaySpine(_b0Spine, "b0");

            _caiwa.GetComponent<RectTransform>().anchoredPosition = new Vector2(-605, -393);
            _caiwa.GetComponent<RectTransform>().localScale = new Vector2(0.5f, 0.5f);

            _xiaohongtou.GetComponent<RectTransform>().anchoredPosition = new Vector2(222, -396);
            _xiaohongtou.GetComponent<RectTransform>().localScale = new Vector2(0.45f, 0.45f);

            ShowVoice();

        }


        private void Process3_1()
        {
            PlaySpines(_a0Spine, new string[3] {"a1","a2","a3"});
            Speck(_caiwa, 0, null, ShowVoice,RoleType.CaiWa);
        }

        private void Process3_2()
        {
            PlaySpines(_b0Spine, new string[3] { "b1", "b2", "b3" });
            Speck(_xiaohongtou, 0, null, ShowVoice, RoleType.XiaoHongTou);
        }

        private void Process4()
        {
            BgUpdate(0);
            _caiwa.Hide(); _xiaohongtou.Hide();
            _max.Show();_maxyingzi.Show();

            _a0Spine.GetComponent<SkeletonGraphic>().Initialize(true);
            _b0Spine.GetComponent<SkeletonGraphic>().Initialize(true);

            _max.GetComponent<RectTransform>().anchoredPosition = new Vector2(93, -321);
            _max.GetComponent<RectTransform>().localScale = new Vector2(0.52f, 0.52f);

            _maxyingzi.GetComponent<RectTransform>().anchoredPosition = new Vector2(40, -371);
            _maxyingzi.GetComponent<RectTransform>().localScale = new Vector2(1.2f, 0.5f);


            Speck(_max, 0);
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
        private void Speck(GameObject go, int index, Action specking = null, Action speckend = null,RoleType role = RoleType.Max, SoundManager.SoundType type = SoundManager.SoundType.SOUND) { _mono.StartCoroutine(SpeckerCoroutine(type, index, go,role, specking, speckend)); }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, RoleType role = RoleType.Max ,Action method_1 = null, Action method_2 = null, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;
         
            switch (role)
            {
              
                case RoleType.Max:
                    daiJi = "daiji"; speak = "daijishuohua";
                    break;
                case RoleType.XiaoHongTou:                 
                case RoleType.CaiWa:
                    daiJi = "daiji"; speak = "speak";
                    break;
                default:
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
