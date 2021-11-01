using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course839Part1
    {
     
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;   
        private GameObject _bell;


        private GameObject _bg1;
        private GameObject _bg2;

        private Transform _spines;
        private GameObject _langan;
        private GameObject _xian;
        private GameObject _xuxian;
        private GameObject _hongxian;

        private RectTransform _fazhiRectTra;
        private GameObject _fazhi;

        private RectTransform _spine1RectTra;
        private GameObject _spine1;
        private GameObject _spine2;
        private GameObject _spine3;

        private Transform _onClicks;
        private GameObject _onClick1;
        private GameObject _onClick2;
        private GameObject _timeBg;
        private Text _timeTxt;
        private GameObject _mask;
   
        private mILDrager _hongXinaDrager;
        private Empty4Raycast _empty4Raycast;

        private Vector3 _lastPos;
        private bool _isPlayingXuXianVoice;



        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;

            _bell = curTrans.Find("bell").gameObject;

            _bg1 = curTrans.GetGameObject("Bg/1");
            _bg2 = curTrans.GetGameObject("Bg/2");

            _spines = curTrans.GetTransform("Spines");
            _langan =curTrans.GetGameObject("Spines/langan");
            _xian = curTrans.GetGameObject("Spines/xian");
            _xuxian = curTrans.GetGameObject("Spines/xian/xuxian");
            _hongxian = curTrans.GetGameObject("Spines/xian/hongxianDrag/hongxian");
            _fazhi = curTrans.GetGameObject("Spines/xian/hongxianDrag/hongxian/fazhi");
            _fazhiRectTra = _fazhi.transform.GetRectTransform();
            _spine1 = curTrans.GetGameObject("Spines/1");

            _spine1RectTra = _spine1.transform.GetRectTransform();
            _spine2 = curTrans.GetGameObject("Spines/2");
            _spine3 = curTrans.GetGameObject("Spines/3");
            _onClicks = curTrans.GetTransform("OnClicks");
            _onClick1 = curTrans.GetGameObject("OnClicks/1");
            _onClick2 = curTrans.GetGameObject("OnClicks/2");
            _timeBg = curTrans.GetGameObject("OnClicks/TimeBg");
            _timeTxt = curTrans.GetText("OnClicks/Text");
            _mask = curTrans.GetGameObject("mask");
            _hongXinaDrager = curTrans.Find("Spines/xian/hongxianDrag").GetComponent<mILDrager>();
            _empty4Raycast = curTrans.GetEmpty4Raycast("Spines/xian/hongxianDrag");
           
           
            GameInit();
        }

        void GameInit()
        {
            DOTween.KillAll();
            _bell.Show();
            _isPlayingXuXianVoice = false;
            _spine1RectTra.anchoredPosition = new Vector2(0, 0);
            _hongXinaDrager.transform.localPosition = new Vector2(0, _hongXinaDrager.transform.localPosition.y);
            _lastPos = _hongXinaDrager.transform.localPosition;
            Debug.LogError("_lastPos:"+ _lastPos);
            _empty4Raycast.raycastTarget = false;
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            PointerClickListener.Get(_mask).onClick = null;          
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _spine1.Show(); _spine2.Hide(); _spine3.Hide(); _bell.Show(); _bg1.Show(); _bg2.Hide();
            _langan.Hide(); _xian.Hide(); _xuxian.Hide(); _hongxian.Hide(); _fazhi.Hide(); _onClick1.Hide(); _onClick2.Hide(); _timeBg.Hide();

            PointerClickListener.Get(_onClick1).onClick = null;
            PointerClickListener.Get(_onClick2).onClick = null;
           
            _timeTxt.text = string.Empty;

          
            _hongXinaDrager.SetDragCallback(DragStart, Draging, DragEnd);

            GameStart();          
        }


   

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);

            SpineManager.instance.DoAnimation(_spine1, "1", false,()=> {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                SoundManager.instance.ShowVoiceBtn(true);
            });
                
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0, 
                () => {_mask.Show(); },
                () => {_mask.Hide(); }));
        }

        IEnumerator TxtUpdate(float delay,float time,float addValue,Action<string> callBack)
        {
            float temp = 0;

            while (temp<= time)
            {
                yield return new WaitForSeconds(delay);
                temp += addValue;
                callBack?.Invoke(temp.ToString("#0.0"));               
            }

            _mono.StopCoroutine("TxtUpdate");

        }

        IEnumerator Delay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(_bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (_talkIndex)
            {
                case 1:
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,1,
                        ()=> {
                            _mask.Show();
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4,false);
                            SpineManager.instance.DoAnimation(_spine1, "2", false);
                            AddOnClickCarEvent();
                        },
                        ()=> {
                            _mask.Hide();
                        }));
                    break;
                case 2:
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,3,
                    ()=> {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        _mask.Show();
                        SpineManager.instance.DoAnimation(_spine1, "6", false);
                        AddOnClickSensorEvent();
                    },
                    ()=> {
                        _mask.Hide();
                    }));
                    break;
                case 3:
                    _onClick2.Hide();
                    PointerClickListener.Get(_onClick2).onClick = null;
                  
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,5,
                    ()=> {
                        _mask.Show();_langan.Show();_xian.Show();_hongxian.Show();
                       var time=  SpineManager.instance.DoAnimation(_spine1, "7", false);                      
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);                       
                        var spine1Move = _spine1RectTra.DOAnchorPosX(250, time).SetEase(Ease.OutSine).OnComplete(()=> { _fazhi.Show(); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false); });
                        var fazhiScale1 = _fazhiRectTra.DOScale(new Vector3(2, 2, 2), 0.5f);
                        var fazhiScale2 = _fazhiRectTra.DOScale(new Vector3(1, 1, 1), 0.3f);

                        _mono.StartCoroutine(Delay(13.5f,()=> {   _fazhi.Hide();}));

                            DOTween.Sequence().Append(spine1Move)
                                          .Append(fazhiScale1)
                                          .Append(fazhiScale2)
                                          .AppendInterval(1)
                                          .OnComplete(()=> {
                                              _spine1RectTra.anchoredPosition = new Vector2(0, 0);                                          
                                              _empty4Raycast.raycastTarget = true;
                                          });    
                        
                    },
                    ()=> { _mask.Hide(); _bell.Hide(); }));
                    break;
            }
            _talkIndex++;
        }

        


      

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        /// <summary>
        /// 添加点击车子事件
        /// </summary>
        void AddOnClickCarEvent()
        {
          
            _onClick1.Show();
            PointerClickListener.Get(_onClick1).onClick = go => {
                BtnPlaySound();
                _onClick1.Hide();
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,2,
                    ()=> {
                        _mask.Show();
                        SpineManager.instance.DoAnimation(_spine1, "4",false);
                    },
                    ()=> {
                        _mask.Hide();
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
            };
        }
        
        /// <summary>
        /// 添加点击传感器起事件
        /// </summary>
        void AddOnClickSensorEvent()
        {
            _onClick2.Show();       
          
            PointerClickListener.Get(_onClick2).onClick = go => {
                _spine3.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                SpineManager.instance.DoAnimation(_spine3, "bj2", false);
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,4,
                ()=> {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                    SoundManager.instance.ShowVoiceBtn(false);                
               
                    _mask.Show(); _bg2.Show(); _spine1.Hide(); _spine2.Show(); _spine3.Show(); _timeBg.Show();
                  
                    _mono.StartCoroutine(Delay(13.5f,()=> { _spine3.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                        SpineManager.instance.DoAnimation(_spine3, "b2", false); }));
                    SpineManager.instance.DoAnimation(_spine2, "b1", false);
                    _mono.StartCoroutine(TxtUpdate(0.1f,5.0f,0.1f,s => {
                        _timeTxt.text = s;
                    }));                   
               },
                ()=> {

                    PointerClickListener.Get(_mask).onClick = o => {
                        PointerClickListener.Get(_mask).onClick = null;
                        _mask.Hide(); _spine1.Show(); _bg2.Hide(); _spine2.Hide(); _spine3.Hide(); _timeBg.Hide();
                        _timeTxt.text = string.Empty;
                        SoundManager.instance.ShowVoiceBtn(true);
                    };
                }));
            };
        }


        void DragStart(Vector3 pos, int type, int index)
        {

        }


        void Draging(Vector3 pos,int type, int index)
        {
            var isShowXuXian = IsShowXuXian(pos);

            if (isShowXuXian)
            {   _xuxian.Show();
                PlayXuXianVoice();
            }
            else
            {
                _xuxian.Hide();
            }
        }

       void DragEnd(Vector3 pos, int type, int index,bool isMatch)
       {
            var isShowXuXian = IsShowXuXian(pos);

            if (isShowXuXian)
            {
                _hongXinaDrager.transform.localPosition = _lastPos;
                _xuxian.Hide();
             
            }
            else
            {
                _lastPos = pos;
                //计算车的移动距离
                CarMoveAni();
            }
       }

       /// <summary>
       /// 是否显示虚线
       /// </summary>
       /// <param name="localPos"></param>
       /// <returns></returns>
       bool IsShowXuXian(Vector3 localPos)
       {
            return localPos.x>= 210;
       }

        void PlayXuXianVoice()
        {
            if (!_isPlayingXuXianVoice)
            {
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,6,
                ()=> {
                    _isPlayingXuXianVoice = true;
                },
                ()=> {
                    _isPlayingXuXianVoice = false;
                }));
            }  
        }

        float SetMoveValua()
        {
            var curLocalX = _hongXinaDrager.transform.localPosition.x;
            Debug.LogError("curLocalX:"+ curLocalX);
            return 250 + curLocalX;
        }

        void CarMoveAni()
        {
            _mask.Show();
            var time = SpineManager.instance.DoAnimation(_spine1, "7", false);
            Debug.LogError("time:"+time);
            var spine1Move = _spine1RectTra.DOAnchorPosX(SetMoveValua(), time).SetEase(Ease.OutSine).OnComplete(() => { _fazhi.Show(); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false); });
            //var fazhiScale1 = _fazhiRectTra.DOScale(new Vector3(2, 2, 2), 0.5f);
            //var fazhiScale2 = _fazhiRectTra.DOScale(new Vector3(1, 1, 1), 0.3f);

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
            DOTween.Sequence().Append(spine1Move)
                                      //  .Append(fazhiScale1)
                                      //  .Append(fazhiScale2)
                                       
                                        .OnComplete(() => {                                                                         
                                            _fazhi.Hide();                                      
                                            _mono.StartCoroutine(Delay(0.5f,()=> {
                                                _mask.Hide();
                                                _spine1RectTra.anchoredPosition = new Vector2(0, 0);
                                            }));
                                            
                                        });
        }
       
    }
}
