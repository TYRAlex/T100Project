using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course745Part2
    {
		         		
        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;      
        private GameObject _di;
       
        private GameObject _mask;

     

        private Transform _spines;
        private Transform _drags;
        private Transform _dragsPos;
        private Transform _drops;
        private Transform _onClicks;

        private int _talkIndex;
        private int _dragNum;

        private mILDrager _curDrag;
        private Vector2 _curDragInitPos;

      

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
          
            _max = curTrans.GetGameObject("Max");
            _mask = curTrans.GetGameObject("mask");
            _di = curTrans.GetGameObject("di");

            _spines = curTrans.Find("Spines");
            _drags = curTrans.Find("drags");
            _dragsPos = curTrans.Find("dragspos");
            _drops = curTrans.Find("drops");
            _onClicks = curTrans.Find("OnClicks");





            GameInit();
            GameStart();
        }
    
        void GameInit()
        {
            _talkIndex = 1;
            _dragNum = 0;

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            HideVoice(); StopAllAudio(); StopAllCor();

            Input.multiTouchEnabled = false;
            DOTween.KillAll();

             _mask.Show(); _di.Show();

            InitSpines(_spines,spine=> {

                var name = spine.name;
                var go = spine.gameObject;
                switch (name)
                {
                    case "3":
                        PlaySpine(go, name);
                        break;
                }

            });

            InitDrags(_drags, drag => {  drag.isActived = false; drag.gameObject.Show();
                drag.SetDragCallback(DragStart, null, DragEnd);
            });

            InitDragsPos(_dragsPos, _drags);

            //隐藏拖拽最终位置图标
            var rmgs= Gets<Image>(_drops);
            for (int i = 0; i < rmgs.Length; i++)         
                rmgs[i].gameObject.Hide();

            AddEvents(_onClicks, OnClickEvent);
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
            }
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            _onClicks.gameObject.Hide(); _max.Show();
            Speck(_max, 0,null,()=> { 
                _mask.Hide(); 
                InitDrags(_drags, drag => { drag.isActived = true; });
            });
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {

            //隐藏拖拽最终位置图标
            var rmgs = Gets<Image>(_drops);
            for (int i = 0; i < rmgs.Length; i++)
                rmgs[i].gameObject.Hide();

            _mask.Show(); _onClicks.gameObject.Show();_di.Hide();
            Speck(_max, 0, null, () => { _mask.Hide();_max.Hide(); });


            InitSpines(_spines, spine => {

                var name = spine.name;
                var go = spine.gameObject;
             
                switch (name)
                {
                    case"guang":
                        break;
                    default:
                        PlaySpine(go, name);
                        break;
                }

            });
        }



        private void OnClickEvent(GameObject go)
        {
            _mask.Show();
            var name = go.name;
            bool isCorrect = name == "qbhk2";
            var spineGo = _spines.Find(name).gameObject;
            var guangGo = spineGo.transform.Find("guang").gameObject;

            PlaySpine(guangGo, "guang");

            PlaySpine(spineGo, name.Replace("2", "1"),()=> { PlaySpine(guangGo, "guang3"); });

            int soundIndex = isCorrect ? 0 : 0;
            Speck(_max, soundIndex,null,()=> { _mask.Hide(); });

           
        }

        //拖拽开始
        private void DragStart(Vector3 pos, int dragType, int index)
        {
            StartDrag(_drags, index);
        }

        //拖拽结束
        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            _mask.Show();

            var box2d = _curDrag.drops[0].GetComponent<BoxCollider2D>();

            isMatch = box2d.OverlapPoint(Input.mousePosition);

            if (isMatch)
            {
                _curDrag.gameObject.Hide();
                var name = _curDrag.name+"/Icon";
                _drops.Find(name).gameObject.Show();

                _mask.Hide();
                _dragNum++;

                if (_dragNum==4)               
                    ShowVoice();
                
            }
            else
            {
                _curDrag.transform.localPosition = _curDragInitPos;
                _mask.Hide();
            }
        }

      

    

     
        //拖拽开始
        private void StartDrag(Transform parent, int index)
        {
           var mILDragers = Gets<mILDrager>(parent);

            for (int i = 0; i < mILDragers.Length; i++)
            {
                var drag = mILDragers[i];
                if (drag.index==index)
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
        private void InitSpines(Transform parent,Action<SkeletonGraphic> callBack=null)
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
        private void InitDrags(Transform parent,Action<mILDrager> callBack=null)
        {
            var mILDragers = Gets<mILDrager>(parent);
            for (int i = 0; i < mILDragers.Length; i++)
            {
                var drag = mILDragers[i];              
                callBack?.Invoke(drag);
            }
        }
      
        //初始化Drag位置层级顺序
        private void InitDragsPos(Transform pos,Transform drags)
        {
            for (int i = 0; i < pos.childCount; i++)
            {
                var posChild = pos.GetChild(i);
                var dragChild =  drags.Find(posChild.name);
                dragChild.localPosition = posChild.localPosition;
                dragChild.SetSiblingIndex(i);
            }
        }


        private T[] Gets<T>(Transform parent){ return parent.GetComponentsInChildren<T>(true);}
     
        // 显示语音键
        private void ShowVoice(){ SoundManager.instance.ShowVoiceBtn(true); }
      
        // 隐藏语音键
        private void HideVoice(){SoundManager.instance.ShowVoiceBtn(false); }
      
        // 停止所有音频
        private void StopAllAudio() {SoundManager.instance.StopAudio(); }
     
        //停止所有协程
        private void StopAllCor() { _mono.StopAllCoroutines(); }
     
        // 讲话
        private void Speck(GameObject go, int index, Action specking = null, Action speckend = null,SoundManager.SoundType type = SoundManager.SoundType.SOUND) { _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend));}

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null,float len = 0)
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
        private void Delay(float delay,Action callBack)
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
