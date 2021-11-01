using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course741Part1
    {
		         		
        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _max;      
        private GameObject _bans;
        private GameObject _aSpine;     
        private GameObject _tonga2Spine;
        private GameObject _tonga1Spine;
        private GameObject _mask;

        private RawImage _bgRImg;

        private BellSprites _bgSprites;

        private Transform _spines;
        private Transform _leftDrags;
        private Transform _rightDrags;
        private Transform _leftDragsPos;
        private Transform _rightDragsPos;
    
        private int _talkIndex;
        private int _leftNum;
        private int _rightNum;

        private mILDrager _curDrag;
        private Vector2 _curDragInitPos;

      

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
          
            _max = curTrans.GetGameObject("Max");         
            _bans = curTrans.GetGameObject("bans");
            _aSpine = curTrans.GetGameObject("Spines/a");     
            _tonga2Spine = curTrans.GetGameObject("Spines/tonga2");
            _tonga1Spine = curTrans.GetGameObject("Spines/tonga1");
            _mask = curTrans.GetGameObject("mask");

            _bgRImg = curTrans.GetRawImage("BG/bg");
            _bgSprites = curTrans.Find("BG/bg").GetComponent<BellSprites>();

            _spines = curTrans.Find("Spines");
            _leftDrags = curTrans.Find("leftdrags");
            _rightDrags = curTrans.Find("rightdrags");
            _leftDragsPos = curTrans.Find("leftdrogpos");
            _rightDragsPos = curTrans.Find("rightdrogpos");

            GameInit();
            GameStart();
        }
    
        void GameInit()
        {
            _talkIndex = 1;
            _leftNum = 0; _rightNum = 0;

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            HideVoice(); StopAllAudio(); StopAllCor();

            Input.multiTouchEnabled = false;
            DOTween.KillAll();


            _bans.Hide(); _mask.Show();


            InitSpines(_spines);InitSpines(_leftDrags);InitSpines(_rightDrags);
            InitDragsPos(_leftDragsPos, _leftDrags);InitDragsPos(_rightDragsPos, _rightDrags);

            InitDrags(_leftDrags, drag => {  drag.isActived = false;
                drag.SetDragCallback(LeftDragStart, null, LeftDragEnd);
            }); 
            InitDrags(_rightDrags, drag => { drag.isActived = false;
                drag.SetDragCallback(RightDragStart, null, RightDragEnd);
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
                    BgUpdate(2, Process3);
                    break;
            }
        }


        /// <summary>
        /// 流程1
        /// </summary>
        private void Process1()
        {
            BgUpdate(0); 
            PlaySpine(_aSpine, "a");
            Speck(_max, 0, null, Process2);
        }

        /// <summary>
        /// 流程2
        /// </summary>
        private void Process2()
        {
            BgUpdate(1);
            PlaySpine(_aSpine, "b"); 
            Speck(_max, 0, null, ShowVoice);
        }


        /// <summary>
        /// 流程3
        /// </summary>
        private void Process3()
        {
            _max.Hide(); _bans.Show();_mask.Hide();
                     
            InitSpines(_spines, spine => {
                var name = spine.name;
                var go = spine.gameObject;
                switch (name)
                {
                    case "tonga1":
                    case "a":                     
                        break;
                    default:
                        PlaySpine(go, name);
                        break;
                }
            });

          
            InitDrags(_leftDrags, drag => { drag.isActived = true; });
            InitDrags(_rightDrags, drag => { drag.isActived = true; });


            InitSpines(_leftDrags, spine => {
                var name = spine.name;
                var go = spine.gameObject;
                PlaySpine(go, name);
            });

            InitSpines(_rightDrags, spine => {
                var name = spine.name;
                var go = spine.gameObject;
                PlaySpine(go, name);
            });
        
        }

        //游戏结束
        private void GameOver(int leftNum, int rightNum)
        {
            if (leftNum == 9 && rightNum == 9)
            {
                _max.Show();
                Speck(_max, 0);
            }
        }

        //左边拖拽开始
        private void LeftDragStart(Vector3 pos, int dragType, int index)
        {
            StartDrag(_leftDrags, index);
        }

        //左边拖拽结束
        private void LeftDragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
          
            var box2d = _curDrag.drops[0].GetComponent<BoxCollider2D>();

            isMatch = box2d.OverlapPoint(Input.mousePosition);

            bool isRecoverable = dragType == 1;  //类型为1 是可回收

            string tongSpineName = isRecoverable ? "tonga3" : "tonga4";

            if (isMatch)
            {
                _mask.Show();

                var spineGo = _curDrag.transform.GetChild(0).gameObject;
                var spineName = spineGo.name.Replace("1", "3");

                 PlaySpine(_tonga1Spine, "tonga1",()=> {      //光线扫描
                     PlaySpine(_tonga2Spine, tongSpineName);  // 垃圾桶打开
                     Delay(1f, () => { PlaySpine(spineGo, spineName,()=> { _curDrag.gameObject.Hide();_mask.Hide(); _leftNum++; GameOver(_leftNum, _rightNum); }); }); //延迟1秒播放垃圾消失动画
                 });                                  
            }
            else
            {
                _curDrag.transform.localPosition = _curDragInitPos;
               
            }
        }

        //右边拖拽开始
        private void RightDragStart(Vector3 pos, int dragType, int index)
        {
            StartDrag(_rightDrags, index);
        }

        //右边拖拽结束
        private void RightDragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
           
            var box2d = _curDrag.drops[0].GetComponent<BoxCollider2D>();

            isMatch = box2d.OverlapPoint(Input.mousePosition);
            
            if (isMatch)
            {
                _mask.Show();
                var spineGo = _curDrag.transform.GetChild(0).gameObject;
                var spineName = spineGo.name.Replace("1", "3");
               
                PlaySpine(spineGo, spineName, () => { 
                    _curDrag.gameObject.Hide();
                    _mask.Hide();
                    _rightNum++;
                    GameOver(_leftNum, _rightNum);
                });
            }
            else
            {              
                var failBox2d = _curDrag.failDrops[0].GetComponent<BoxCollider2D>();

                bool isFailMatch = failBox2d.OverlapPoint(Input.mousePosition);

                if (isFailMatch)
                {
                    _mask.Show();
                    var spineGo = _curDrag.transform.GetChild(0).gameObject;
                    var spineName = spineGo.name.Replace("1", "2");

                    PlaySpine(spineGo, spineName, () =>
                    {
                        _curDrag.transform.localPosition = _curDragInitPos;
                        _mask.Hide();
                    });
                }
                else
                {
                    _curDrag.transform.localPosition = _curDragInitPos;
                }

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


        //更新背景
        private void BgUpdate(int index,Action callBack=null)
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
    }
}
