using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course741Part2
    {

        private MonoBehaviour _mono;
        private GameObject _curGo;
        private GameObject _max;      
        private GameObject _mask;
        private GameObject _animationSpine;
        private GameObject _jqrSpine;

        private Transform _spines;
        private Transform _drags;
        private Transform _dragspos;


        private int _talkIndex;

        private mILDrager _curDrag;

        private Vector2 _curDragInitPos;

        private enum RoleType
        {
            Max,
            YiEn,
        }


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            _max = curTrans.GetGameObject("Max");      
            _mask = curTrans.GetGameObject("mask");
            _animationSpine = curTrans.GetGameObject("Spines/animation");
            _jqrSpine = curTrans.GetGameObject("Spines/jqr");

            _spines = curTrans.Find("Spines");
            _drags = curTrans.Find("drags");
            _dragspos = curTrans.Find("dragspos");


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

            InitSpines(_spines,null); InitSpines(_drags,null);
            InitDragsPos(_dragspos,_drags);
            InitDrags(_drags, drag => { 
                drag.isActived = false;
                drag.SetDragCallback(DragStart, null, DragEnd);
            });
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
                    Process4();
                    break;
            }
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            _max.Hide();
            PlaySpine(_animationSpine, "animation", null, true);
            Speck(_jqrSpine, 0, null, Process2, RoleType.YiEn);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            PlaySpine(_animationSpine, "animation2");
            Speck(_jqrSpine, 0, null, Process3, RoleType.YiEn);
        }


        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {
            PlaySpine(_animationSpine, "animation3");
            Speck(_jqrSpine, 0, null, ShowVoice, RoleType.YiEn);

        }


        /// <summary>
        /// 流程4
        /// </summary>
        private void Process4()
        {
            _jqrSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            _max.Show();
            PlaySpine(_animationSpine, "animation2",()=> { PlaySpine(_animationSpine, "animation3", Process5); });
            Speck(_max, 0 );
        }

        /// <summary>
        /// 流程5
        /// </summary>
        private void Process5()
        {
            PlaySpine(_animationSpine, "1");

            //初始化左边拖拽Spine

            InitSpines(_drags, spine => {
                var name = spine.name;
                var go = spine.gameObject;
                PlaySpine(go, name);           
            });
            InitDrags(_drags, drag => { drag.isActived = true; });
            Speck(_max, 0, null, ()=> { _max.Hide(); _mask.Hide(); });
        }


        private void DragStart(Vector3 pos, int dragType, int index)
        {
            StartDrag(_drags, index);
        }

        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            isMatch = dragType == 1; //dragType为1说明是对的
            Debug.LogError("dragType:" + dragType);
            if (isMatch)
            {
                var box2d = _curDrag.drops[0].GetComponent<BoxCollider2D>();

                isMatch = box2d.OverlapPoint(Input.mousePosition);

                if (isMatch)
                {
                    _mask.Show();

                    bool isCaiWa = index == 7;   //彩哇的人物拖拽index 是 7

                    int soundIndex = isCaiWa ? 0 : 0;

                    Speck(_max, soundIndex, null, () => { _mask.Hide(); _curDrag.isActived = false; });
                }
                else
                {
                    //错了
                    _curDrag.transform.localPosition = _curDragInitPos;
                }
            }
            else
            {
                _curDrag.transform.localPosition = _curDragInitPos;
            }

        }


        //拖拽开始
        private void StartDrag(Transform parent, int index)
        {
            var mILDragers = Gets<mILDrager>(parent);

            for (int i = 0; i < mILDragers.Length; i++)
            {
                var drag = mILDragers[i];
                if (drag.index == index)
                {
                    _curDrag = drag;
                    _curDragInitPos = drag.transform.localPosition;
                    break;
                }
            }
            _curDrag.transform.SetAsLastSibling();
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

        //初始化Drags
        private void InitDrags(Transform parent, Action<mILDrager> callBack = null)
        {
            var mILDragers = Gets<mILDrager>(parent);
            for (int i = 0; i < mILDragers.Length; i++)
            {
                var drag = mILDragers[i];
                callBack?.Invoke(drag);
            }
        }

        //初始化Drag位置层级顺序
        private void InitDragsPos(Transform pos, Transform drags)
        {
            for (int i = 0; i < pos.childCount; i++)
            {
                var posChild = pos.GetChild(i);
                var dragChild = drags.Find(posChild.name);
                dragChild.localPosition = posChild.localPosition;
                dragChild.SetSiblingIndex(i);
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

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Max,  float len = 0)
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
