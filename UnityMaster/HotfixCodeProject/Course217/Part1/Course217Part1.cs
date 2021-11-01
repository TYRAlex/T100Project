using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
namespace ILFramework.HotClass
{
    public class Course217Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private GameObject _mask;
        private Transform _trashParent;
        private Transform _trashBtnsParent;
        private Transform _trashPrincipleParent;
        private Transform _trashPrincipleBtns;

        private GameObject _principleSpine1;
        private GameObject _principleSpine2;

        private GameObject _downBtn;
        private GameObject _upBtn;
        private GameObject _mask1;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
        
            bell = curTrans.Find("bell").gameObject;
            mono = curGo.GetComponent<MonoBehaviour>();

            _mask = curTrans.Find("mask").gameObject;
            _trashBtnsParent = curTrans.Find("TrashBtns");
            _trashParent = curTrans.Find("TrashParent");
            _trashPrincipleParent = curTrans.Find("TrashPrincipleParent");
            _trashPrincipleBtns = curTrans.Find("TrashPrincipleBtns");

            _principleSpine1 = _trashPrincipleParent.Find("PrincipleSpine1").gameObject;
            _principleSpine2 = _trashPrincipleParent.Find("PrincipleSpine2").gameObject;

            _downBtn = _trashPrincipleBtns.Find("DownBtn").gameObject;
            _upBtn = _trashPrincipleBtns.Find("UpBtn").gameObject;
            _mask1 = curTrans.GetGameObject("mask1");
            GameInit();
          
        }
       
     

        void GameInit()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            talkIndex = 1;
            _mask1.Hide();
            bell.transform.GetRectTransform().anchoredPosition = new Vector2(300,-55);
            bell.Show();
            _trashParent.gameObject.Show();
            _trashPrincipleParent.gameObject.Hide();
            _trashBtnsParent.gameObject.Hide();
            _trashPrincipleBtns.gameObject.Hide();
            _mask.gameObject.Hide();
            _downBtn.Hide(); _upBtn.Hide();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            LeftTrashLongPressBtnEvent();
            RightTrashLongPressBtnEvent();

            GameStart();
        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0, null, 
                () =>{    SoundManager.instance.ShowVoiceBtn(true); }
            ));
        
        }


        IEnumerator TextSpeckerCoroutine(float len,Action action=null)
        {
            if (len > 0)            
                yield return new WaitForSeconds(len);

            if (action != null)
                action();
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)          
                yield return new WaitForSeconds(len);
            
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)           
                method_1();
            
            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)           
                method_2();           
        }
        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,1,
                    ()=> {
                        SoundManager.instance.ShowVoiceBtn(false);
                    },
                    ()=> { _trashBtnsParent.gameObject.SetActive(true);}));
                    break;
                case 2:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 2,()=> { _trashBtnsParent.gameObject.SetActive(false); },
                    () => {
                        _trashBtnsParent.gameObject.SetActive(true);
                        SoundManager.instance.ShowVoiceBtn(true);
                    } ));
                    break;
                case 3:
                    _trashBtnsParent.gameObject.SetActive(false);
                    _mask.SetActive(true);
                    _trashParent.gameObject.SetActive(false);
                    _trashPrincipleParent.gameObject.SetActive(true);
                    bell.SetActive(false);
                    //动画循环
                    SpineManager.instance.DoAnimation(_principleSpine1, "animation2");
                //    _principleSpine2.SetActive(false);

                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,3,null
                        ,()=>{ SoundManager.instance.ShowVoiceBtn(true); }));
                    break;
                case 4:

                    //结束动画                 
                    SpineManager.instance.DoAnimation(_principleSpine1, "animation", false);
                //    SpineManager.instance.ClearTrack(_principleSpine1);
                    bell.GetComponent<RectTransform>().anchoredPosition = new Vector3(300, -500,0);
                    bell.SetActive(true);

            
                    var tweener =  bell.GetComponent<RectTransform>().DOAnchorPos(new Vector3(300, -55, 0), 1.0f);
                    tweener.OnComplete(() => {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 4, null, () => {
                            _trashPrincipleBtns.gameObject.SetActive(true);
                            _downBtn.SetActive(true);
                            Util.AddBtnClick(_downBtn, DownBtnEvent);
                        }));
                    });                      
                    break;
                case 5:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,6,
                    ()=>{
                        SpineManager.instance.DoAnimation(_principleSpine2, "yy01",false);
                    },
                    ()=>{
                        SpineManager.instance.DoAnimation(_principleSpine1, "animation", false);
                      //  SpineManager.instance.ClearTrack(_principleSpine1);
                        SpineManager.instance.DoAnimation(_principleSpine2, "yy03", false, () => {
                            _upBtn.SetActive(true);
                            Util.AddBtnClick(_upBtn, UpBtnEvent);
                        });                                      
                    }));               
                    break;
                case 6:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,8,()=> 
                    {
                        SpineManager.instance.DoAnimation(_principleSpine2, "yy11",false);
                    },
                    ()=> {
                        SpineManager.instance.DoAnimation(_principleSpine2, "yy13", false,()=> {
                            SpineManager.instance.DoAnimation(_principleSpine1, "animation", false);
                           // SpineManager.instance.ClearTrack(_principleSpine1);
                            _upBtn.Show();
                            _downBtn.Show();
                            Util.AddBtnClick(_upBtn, RestartUpBtnEvent);
                            Util.AddBtnClick(_downBtn, RestartDownBtnEvent);
                        });
                      
                    }));                                   
                    break;
            }
            talkIndex++;
        }




        /// <summary>
        /// 长按按钮
        /// </summary>
        /// <param name="parent">父物体</param>
        /// <param name="longPressBtnName">按钮名字</param>
        /// <param name="delay">延迟</param>
        /// <param name="onDownCallBack">按下回调</param>
        /// <param name="onUpCallBack">抬起回调</param>
        private void LongPressBtn(Transform parent, string longPressBtnName,float delay,Action onDownCallBack,Action onUpCallBack)
        {
           var longBtn = parent.Find(longPressBtnName).GetComponent<LongPressButton>();
            if (longBtn==null)            
                parent.Find(longPressBtnName).gameObject.AddComponent<LongPressButton>();
            
            

           float time = 0;
            //按下
            longBtn.OnDown=()=> {
                
                time += 1f;            
                if (time== delay)                                 
                    onDownCallBack?.Invoke();                                                             
            };
            //抬起
            longBtn.OnUp = () => {

                if (time>=delay)                                 
                    onUpCallBack?.Invoke();               
                time = 0;
            };
        }
        
        /// <summary>
        /// 左边垃圾桶按钮事件
        /// </summary>
        private void LeftTrashLongPressBtnEvent()
        {
            var aniGo = _trashParent.Find("LeftTrash").gameObject;
            LongPressBtn(_trashBtnsParent,"LeftTrashBtn", 7f,
              () => {
                  SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                  SpineManager.instance.DoAnimation(aniGo, "a2",false);                
              },
              () => {               
                  SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                  SpineManager.instance.DoAnimation(aniGo, "a4", false,()=> {
                      SoundManager.instance.ShowVoiceBtn(true);
                  });                                                  
              });          
        }

        /// <summary>
        /// 右边垃圾桶按钮事件
        /// </summary>
        private void RightTrashLongPressBtnEvent()
        {
            var aniGo = _trashParent.Find("RigthTrash").gameObject;
            LongPressBtn(_trashBtnsParent,"RigthTrashBtn", 7f,
              () => {
                  SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                  SpineManager.instance.DoAnimation(aniGo, "b2", false);               
              },
              () => {                 
                  SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                  SpineManager.instance.DoAnimation(aniGo, "b4", false,()=> {
                     SoundManager.instance.ShowVoiceBtn(true);
                  });                                              
              });
        }

        /// <summary>
        /// 杠杆原理(下面杠杆按钮事件)
        /// </summary>
        /// <param name="obj"></param>
        private void DownBtnEvent(GameObject obj)
        {           
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,5,
            ()=> 
            {
                obj.SetActive(false);
            //    _principleSpine2.SetActive(true);
                SpineManager.instance.DoAnimation(_principleSpine2, "g1", false);
                SpineManager.instance.DoAnimation(_principleSpine1, "animation2", true);
            },
            ()=>{
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }


        /// <summary>
        /// 杠杆原理(上面杠杆按钮事件)
        /// </summary>
        /// <param name="obj"></param>
        private void UpBtnEvent(GameObject obj)
        {

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,7,()=>
            {
                obj.SetActive(false);
           //     _principleSpine2.SetActive(true);
                SpineManager.instance.DoAnimation(_principleSpine2, "g2", false);
                SpineManager.instance.DoAnimation(_principleSpine1, "animation2", true);
            },
            ()=> 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));      
        }


        private void RestartDownBtnEvent(GameObject obj)
        {
            _mask1.Show();
            SpineManager.instance.DoAnimation(_principleSpine1, "animation2", true);
         //   _principleSpine2.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 6,
                  () => {
                      SpineManager.instance.DoAnimation(_principleSpine2, "g1", false,()=> { SpineManager.instance.DoAnimation(_principleSpine2, "yy01", false); });                   
                  },
                  () => {
                      SpineManager.instance.DoAnimation(_principleSpine1, "animation", false);
                  //    SpineManager.instance.ClearTrack(_principleSpine1);
                      SpineManager.instance.DoAnimation(_principleSpine2, "yy03", false,()=> { _mask1.Hide(); });                    
                  }));


        }

        private void RestartUpBtnEvent(GameObject obj)
        {
            _mask1.Show();
            SpineManager.instance.DoAnimation(_principleSpine1, "animation2", true);
        //    _principleSpine2.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 8, () =>
            {
                SpineManager.instance.DoAnimation(_principleSpine2, "g2", false,()=> { SpineManager.instance.DoAnimation(_principleSpine2, "yy11", false); });             
            },
            () => {
                SpineManager.instance.DoAnimation(_principleSpine1, "animation", false);
             //   SpineManager.instance.ClearTrack(_principleSpine1);
                SpineManager.instance.DoAnimation(_principleSpine2, "yy13", false,()=> { _mask1.Hide(); });                          
            }));   
        }
    }
}
