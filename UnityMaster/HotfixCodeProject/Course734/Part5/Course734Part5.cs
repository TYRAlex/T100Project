using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace ILFramework.HotClass
{
    public class Course734Part5
    {
        public enum WuPingEnum
        {
            
            Clock,
            Package,
            Box,
            Null
        }

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _spine0;
        private GameObject _succeed;
        private GameObject _mask;


        private GameObject _diZhuo;
        private GameObject _yaoGan;
        private GameObject _show0;
        private GameObject _show1;
        private GameObject _show2;

        private Transform _wuPing;
        private GameObject[] _wuPingObj;

        private GameObject yaoganCollder;
        private EventDispatcher mobotEventDispatcher;

        private Transform _ctrl;
        private Empty4Raycast[] _ctrlE4r;
        private GameObject _targetClock;
        private GameObject _targetPack;
        private GameObject _targetBox;

        private bool _canpress;
        private bool _cando;
        private float timesum;
        private int _LorR;//0中间，1表R，-1表L
        private WuPingEnum _wuPingEnum;

      
        private GameObject _cartBoxCollider;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            GameInit();
            GameStart();
        }
        private void GameInit()
        {
            talkIndex = 1;
            _canpress = false;
            _cando = false;
            timesum = 0;
            _LorR = 0;            
            _wuPingEnum = WuPingEnum.Null;

            _spine0 = curTrans.Find("spinePanel/0").gameObject;
            _succeed = curTrans.Find("spinePanel_1/succeed").gameObject;
            SpineManager.instance.DoAnimation(_succeed, "kong", false);

            _diZhuo = curTrans.Find("mobot/dizhuo").gameObject;
            _yaoGan = curTrans.Find("mobot/yaogan").gameObject;
            _show0 = curTrans.Find("mobot/show/0").gameObject;
            _show1 = curTrans.Find("mobot/show/1").gameObject;
            _show2 = curTrans.Find("mobot/show/2").gameObject;

            _targetClock = curTrans.Find("mobot/yaogan/0/0").gameObject;
            _targetPack = curTrans.Find("mobot/yaogan/0/1").gameObject;
            _targetBox = curTrans.Find("mobot/yaogan/0/2").gameObject;

            _wuPing = curTrans.Find("mobot/wuping");
            _wuPingObj = new GameObject[_wuPing.childCount];
            for (int i = 0; i < _wuPingObj.Length; i++)
            {
                _wuPingObj[i] = _wuPing.GetChild(i).gameObject;
                _wuPingObj[i].Show();
            }

            _cartBoxCollider = curTrans.Find("mobot/showCollider/box").gameObject;
            _cartBoxCollider.GetComponent<BoxCollider2D>().enabled = false;

            _mask = curTrans.Find("mask").gameObject;

            yaoganCollder = curTrans.Find("mobot/yaogan/0/collider").gameObject;
            _yaoGan.transform.rotation = Quaternion.Euler(Vector3.zero);

            mobotEventDispatcher = yaoganCollder.GetComponent<EventDispatcher>();
            yaoganCollder.Hide();

            _ctrl = curTrans.Find("mobot/ctrl");
            _ctrlE4r = _ctrl.GetComponentsInChildren<Empty4Raycast>(true);
            ShowCtrlE4r(true);

            UIEventListener.Get(_ctrlE4r[0].gameObject).onDown = BtnDownL;
            UIEventListener.Get(_ctrlE4r[0].gameObject).onUp = BtnUpL;
            UIEventListener.Get(_ctrlE4r[0].gameObject).onExit = BtnExitL;

            UIEventListener.Get(_ctrlE4r[1].gameObject).onDown = BtnDownR;
            UIEventListener.Get(_ctrlE4r[1].gameObject).onUp = BtnUpR;
            UIEventListener.Get(_ctrlE4r[1].gameObject).onExit = BtnExitR;
            for (int i = 0; i < _ctrlE4r.Length; i++)
            {                                            
                _ctrlE4r[i].transform.parent.gameObject.Hide();
            }
            _wuPingObj[0].GetComponent<BoxCollider2D>().enabled = true;
            _wuPingObj[1].GetComponent<BoxCollider2D>().enabled = true;
            _wuPingObj[2].GetComponent<BoxCollider2D>().enabled = true;

            _diZhuo.Hide();
            _yaoGan.Hide();
            _show0.Hide();
            _show1.Hide();
            _show2.Hide();
            
            _succeed.Hide();

            _mask.Hide();

            _targetClock.SetActive(false);
            _targetPack.SetActive(false);
            _targetBox.SetActive(false );
        }
        private void ShowOrHideCtrlE4r(bool isShow)
        {
            for (int i = 0; i < _ctrlE4r.Length; i++)
            {
                _ctrlE4r[i].transform.parent.gameObject.SetActive(isShow);
            }
        }
        private void ShowCtrlE4r(bool isShow)
        {
            for (int i = 0; i < _ctrlE4r.Length; i++)
            {
                _ctrlE4r[i].gameObject.SetActive(isShow);
            }
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            _spine0.Show();
            targetOneIndex = -1;
            targetThreeIndex = -1;
            targetTwoIndex = -1;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true); 
            }));
            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine0, "jing", false);

        }

        void FixedUpdate()
        {
            if (_canpress && _cando)
            {
                timesum += Time.deltaTime;
                if (_LorR == -1 && _wuPingEnum == WuPingEnum.Null)//按R
                {                  
                    _yaoGan.transform.Rotate(Vector3.forward, 0.6f);
                }
                else if (_LorR == 1 && _wuPingEnum == WuPingEnum.Null)//按L
                {                  
                    _yaoGan.transform.Rotate(Vector3.forward, -0.6f);
                }

            }                       
        }
        private void BtnDownL(PointerEventData eventData)
        {           
            _canpress = true;
            _cando = true;
            _LorR = 1;
        }
        private void BtnUpL (PointerEventData eventData)
        {
            _canpress = false;
            _cando = false;                
        }
        private void BtnExitL (GameObject obj)
        {
            _canpress = false;
        }
        private void BtnDownR(PointerEventData eventData)
        {
            _canpress = true;
            _cando = true;
            _LorR = -1;
        }
        private void BtnUpR(PointerEventData eventData)
        {
            _canpress = false;                       
        }
        private void BtnExitR(GameObject obj)
        {
            _canpress = false;
        }
        private int targetOneIndex;
        private int targetTwoIndex;
        private int targetThreeIndex;

        private void YaoGanTouch(Collider2D c, int time)
        {
            _wuPingObj[0].GetComponent<BoxCollider2D>().enabled = false;
            _wuPingObj[1].GetComponent<BoxCollider2D>().enabled = false;
            _wuPingObj[2].GetComponent<BoxCollider2D>().enabled = false;
            if (c.name == "box")//箱子
            {               
                _cartBoxCollider.GetComponent<BoxCollider2D>().enabled = false;
                if (targetOneIndex == 0)
                {                   
                    _show2.gameObject.SetActive(true);//闹钟                   
                    _targetClock.SetActive(false);
                    targetOneIndex = 1;
                    if (targetOneIndex != 1 || targetTwoIndex != 2 || targetThreeIndex != 3)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                        {
                           
                            _wuPingObj[1].GetComponent<BoxCollider2D>().enabled = true;
                            _wuPingObj[2].GetComponent<BoxCollider2D>().enabled = true;
                           
                        }));
                    }

                    mono.StartCoroutine(WaiteCoroutine(() => { PlaySucceedAni(); }, 1.5f));
                }
                else if (targetTwoIndex == 1)
                {                 
                    _show1.gameObject.SetActive(true);//包                   
                    _targetPack.SetActive(false);
                    targetTwoIndex = 2;
                    if (targetOneIndex != 1 || targetTwoIndex != 2 || targetThreeIndex != 3)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                        {                            
                            _wuPingObj[0].GetComponent<BoxCollider2D>().enabled = true;
                            _wuPingObj[2].GetComponent<BoxCollider2D>().enabled = true;                           
                        }));
                    }                   
                    mono.StartCoroutine(WaiteCoroutine(() => { PlaySucceedAni(); }, 1.5f));
                }
                else if (targetThreeIndex == 2)
                {                   

                    _show0.gameObject.SetActive(true);                    
                    _targetBox.SetActive(false);
                    targetThreeIndex = 3;                    

                    if (targetOneIndex != 1 || targetTwoIndex != 2 || targetThreeIndex != 3)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                        {
                           
                            _wuPingObj[0].GetComponent<BoxCollider2D>().enabled = true;
                            _wuPingObj[1].GetComponent<BoxCollider2D>().enabled = true;
                           
                        }));
                    }                   
                    mono.StartCoroutine(WaiteCoroutine(() => { PlaySucceedAni(); }, 1.5f));
                }                
            }
            else if (c.gameObject.name == "0")//闹钟
            {                                         
                _targetClock.SetActive(true);
                _wuPingObj[0].SetActive(false);
                targetOneIndex = 0;
                _cartBoxCollider.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (c.gameObject.name == "1")//包
            {                                        
                _targetPack.SetActive(true);
                _wuPingObj[1].SetActive(false);
                targetTwoIndex = 1;
                _cartBoxCollider.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (c.gameObject.name == "2")//最重
            {                              
                _targetBox.SetActive(true);
                _wuPingObj[2].Hide();
                targetThreeIndex = 2;
                _cartBoxCollider.GetComponent<BoxCollider2D>().enabled = true;
            }
        } 
        private void PlaySucceedAni()
        {
            if (targetOneIndex == 1 && targetTwoIndex == 2 && targetThreeIndex == 3)
            {
                _mask.Show();
                _succeed.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 5, false);
                SpineManager.instance.DoAnimation(_succeed, "animation", false, () =>
                {
                    SpineManager.instance.DoAnimation(_succeed, "animation2", false);
                });
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null));
                _wuPingObj[0].GetComponent<BoxCollider2D>().enabled = false;
                _wuPingObj[1].GetComponent<BoxCollider2D>().enabled = false;
                _wuPingObj[2].GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () => { }));
                mono.StartCoroutine(WaiteCoroutine(() => 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4, false);
                    SpineManager.instance.DoAnimation(_spine0, "animation", false, () =>
                    {
                        SpineManager.instance.DoAnimation(_spine0, "animation2", false, () =>
                        {
                            mono.StartCoroutine(WaiteCoroutine(() => 
                            {
                                SpineManager.instance.DoAnimation(_spine0, "animation3", false, () =>
                                {
                                    _spine0.Hide();
                                    _diZhuo.Show();
                                    _yaoGan.Show();
                                    ShowOrHideCtrlE4r(true);
                                    yaoganCollder.Show();
                                    mobotEventDispatcher.TriggerEnter2D += YaoGanTouch;
                                });

                            }, 1.0f));
                        });
                    });
                }, 1.5f));                
            }

            talkIndex++;
        }
        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator WaiteCoroutine( Action method_2 = null, float len = 0)
        {
           
            yield return new WaitForSeconds(len);            
            method_2?.Invoke();
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
