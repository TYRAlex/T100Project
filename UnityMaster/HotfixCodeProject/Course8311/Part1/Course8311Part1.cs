using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course8311Part1
    {
      
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
    
        private GameObject _bell;
        private GameObject _Bg;
        private GameObject _mask;

        private GameObject _spineAnimation;
        private GameObject _spineCs;
        private GameObject _spine1;
        private GameObject _spine2;

        private GameObject _onClickAnimation;
        private Transform _numParent;

    
        private bool _isShowJianPan;    //是否显示出键盘
        private Text _speedNum;
        private string _num;

        private float _startPos;
        private float _endPos;
        private float _moveSpeed;

        private RectTransform _treePartent;
   
        private Vector2 _tempPos;
        private bool _isStartMove;

        private GameObject _c2;
        private bool _isStop;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            Transform curTrans = _curGo.transform;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            _bell = curTrans.Find("bell").gameObject;
            _Bg = curTrans.Find("Bg").gameObject;
            _mask = curTrans.GetGameObject("mask");

            _spineAnimation = curTrans.GetGameObject("Spines/animation");
            _spineCs = curTrans.GetGameObject("Spines/cs");
            _spine1 = curTrans.GetGameObject("Spines/1");
            _spine2 = curTrans.GetGameObject("Spines/2");
            _onClickAnimation = curTrans.GetGameObject("OnClicks/animation");
            _speedNum = curTrans.GetText("OnClicks/animation/Text");
            _numParent = curTrans.GetTransform("OnClicks/numParent");

            _treePartent = curTrans.GetRectTransform("Bg/TreesParent");
         
            _c2 = curTrans.GetGameObject("Bg/C2");


            GameInit();
        }

    

        void GameInit()
        {
            _isStop = false;
            _c2.Show();
            _isStartMove = true;
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _speedNum.text = string.Empty;
            _num = string.Empty;
          
            _isShowJianPan = false;

            _bell.Show();
            _onClickAnimation.Hide();
            _numParent.gameObject.Hide();
            _spineAnimation.Hide();
            _spineCs.Hide();
            _spine1.Hide();
            _spine2.Hide();

            _startPos = 0;
            _endPos = -3200;

            SetMoveSpeed(60);


            InitBgPos();

            GameStart();
        }


        void InitBgPos()
        {
            _tempPos = new Vector2(0, _startPos);
            _treePartent.localPosition = _tempPos;        
        }

        void SetMoveSpeed(float speed)
        {

            _moveSpeed = speed*(Screen.height / 1080f);
            Debug.LogError("_moveSpeed:" + _moveSpeed);
        }



        /// <summary>
        /// 设置汽车不同速度音效
        /// </summary>
        /// <param name="speed"></param>
        void SetCarSpeedVoice(float speed)
        {
            if (speed==0)
            {            
                _isStop = true;
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                return;
            }

            if (_isStop)
            {
                PlayVoice(0,true);
                _isStop = false;
            }

            int voiceIndex = -1;
            if (0<speed && speed<60)            
                voiceIndex = 8;          
            else if(60<=speed && speed<=120)           
                voiceIndex = 5;
            else if(speed>120)
                voiceIndex = 2;
            PlayVoice(voiceIndex);
        }

        void PlayVoice(int index, bool isLoop = false)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
        }


        void Move(RectTransform rect)
        {
            if (rect.localPosition.y<=_endPos)           
                rect.localPosition = _tempPos;
            rect.Translate(Vector3.down * _moveSpeed);
        }



        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            PlayVoice(0,true);       

            MyUpdate(0.02f,() => {
                Move(_treePartent);
            
            });

            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,
                ()=> { _mask.Show(); },
                ()=> {
                     _bell.Hide();
                    _spineAnimation.Show();
                    SpineManager.instance.DoAnimation(_spineAnimation, _spineAnimation.name, false, AddOnClickEvent);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,1,null,()=> { _mask.Hide(); SoundManager.instance.ShowVoiceBtn(true); }));
                  
                    }));
        }



        #region 添加点击事件

        private void AddOnClickEvent()
        {
            OnClickSpineAnimation();
            OnClickNums();
        }

        private void OnClickSpineAnimation()
        {
            _onClickAnimation.Show();
            PointerClickListener.Get(_onClickAnimation).onClick = null;
            PointerClickListener.Get(_onClickAnimation).onClick = go => {

                PlayVoice(4);
                if (!_isShowJianPan)
                {
                    _spine1.Show();
                    _numParent.gameObject.Show();
                    SpineManager.instance.DoAnimation(_spine1, "jsqj", false);
                    _isShowJianPan = true;
                   
                }
                else
                {
                    _spine1.Hide();
                    _numParent.gameObject.Hide();
                    _isShowJianPan = false;
                }
            };
        }

        /// <summary>
        /// 点击数字
        /// </summary>
        private void OnClickNums()
        {
            for (int i = 0; i < _numParent.childCount; i++)
            {
                var child = _numParent.GetChild(i).gameObject;

                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = go => {

                    BtnPlaySound();
                    var name = go.name;
                    int result ;
                    bool isNum = int.TryParse(name, out result);

                    if (isNum)
                    {                       
                        string aniName = "jsq" + name;
                        if (name == "0")
                            aniName = "jsq11";
                        SpineManager.instance.DoAnimation(_spine1, aniName, false);
                        if (_num.Length >= 3)                                                
                            return;                        
                        _num += name;                                           
                        _speedNum.text = _num;                
                    }
                    else
                    {
                        bool isOk = name == "Ok";
                        string aniName1 = string.Empty;
                        if (isOk)
                            aniName1 = "jsq12";
                        else
                            aniName1 = "jsq10";
                        SpineManager.instance.DoAnimation(_spine1, aniName1, false);

                        switch (name)
                        {
                            case "Cancel":                              
                                if (_num.Length!=0)
                                {
                                   _num= _num.Remove(_num.Length-1,1);
                                    _speedNum.text = _num;                                 
                                }
                                break;
                            case "Ok":                             
                                Debug.LogError("Ok num :"+_num);
                                if (_num!=string.Empty)                              
                                    OnClickOk(_num);                                                              
                                break;

                        }
                    }                    
                };
            }
        }

        /// <summary>
        /// 点击Ok
        /// </summary>
        private void OnClickOk(string num)
        {
            int speed = int.Parse(num);

            if (speed>220)           
                return;

            SetCarSpeedVoice(speed);

            _mask.Show();
          
            SetMoveSpeed(speed);
            bool isLowVelocity = 0 <= speed && speed < 60;       //是否低速
            bool isNormalVelocity = 60 <= speed && speed <=120;  //是否正常
            bool isOverSpeed = speed >120;                     //是否超速

            string aniName = string.Empty;    //动画名
          

            if (isLowVelocity)
            { aniName = "cs3";  }

            if (isNormalVelocity)
            { aniName = "cs2"; }

            if (isOverSpeed)
            { aniName = "cs"; }

            BtnPlaySound();
         
            _spine1.Hide();
            _numParent.gameObject.Hide();
            _spineCs.Show();

            SpineManager.instance.DoAnimation(_spineCs, aniName, false,()=> {

                _mono.StartCoroutine(Delay(2.5f,()=> {
                    _num = string.Empty;
                    _speedNum.text = _num;
                  
                    _isShowJianPan = false;
                    _spineCs.Hide();
                    _mask.Hide();
                }));
            });
        }

        #endregion

        #region 点击语音键

        /// <summary>
        /// 第一次点击语音键
        /// </summary>
        private void FirstOnClickVoiceBtn()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);

            _c2.Hide();
            _isStartMove = false;
            _mono.StopCoroutine("IEUpdate");
            InitBgPos();

            _spineAnimation.Hide();
            _spineCs.Hide();
            _spine1.Hide();
            _onClickAnimation.Hide();
            _numParent.gameObject.Hide();

            _spine2.Show();
            _bell.Show();
            SpineManager.instance.DoAnimation(_spine2, "1", false);
            PlayVoice(6);
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,2,
                ()=> { _mask.Show(); },
                ()=> { _mask.Hide(); SoundManager.instance.ShowVoiceBtn(true); }));

        }

        /// <summary>
        /// 第二次点击语音键
        /// </summary>
        private void SecondOnClickVoiceBtn()
        {
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,3,
                ()=> { _mask.Show();

                       _mono.StartCoroutine(Delay(3.0f, () => {  SpineManager.instance.DoAnimation(_spine2, "3", false,()=> {

                           _mono.StartCoroutine(Delay(3.0f, () => { SpineManager.instance.DoAnimation(_spine2, "g1", false); }));
                           _mono.StartCoroutine(Delay(8.0f, () => { SpineManager.instance.DoAnimation(_spine2, "g2", false); }));
                       }); }));
                      
                },
                ()=> { _mask.Hide(); SoundManager.instance.ShowVoiceBtn(true); }));
        }

        /// <summary>
        /// 第三次点击语音键
        /// </summary>
        private void ThirdOnClickVoiceBtn()
        {
            PlayVoice(7);
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 4,
                () => {
                    _mask.Show();
                    SpineManager.instance.DoAnimation(_spine2, "lc1", false, One);                 
                    void One()
                    {
                        _mono.StartCoroutine(Delay(2.0f, () => {
                            SpineManager.instance.DoAnimation(_spine2, "lc2", false, Two);
                        }));
                       
                    }

                    void Two()
                    {
                        _mono.StartCoroutine(Delay(2.0f, () => {
                            SpineManager.instance.DoAnimation(_spine2, "lc3", false, Three);
                        }));                                                               
                    }

                    void Three()
                    {
                       
                        SpineManager.instance.DoAnimation(_spine2, "lc4", false, Four);
                    }

                    void Four()
                    {
                        SpineManager.instance.DoAnimation(_spine2, "lc5", false, Five);
                       _mono.StartCoroutine(Delay(3.0f,()=> { SpineManager.instance.DoAnimation(_spine2, "kuang", false); }));
                    }

                    void Five()
                    {
                        SpineManager.instance.DoAnimation(_spine2, "lc6", false);
                    }              
                },
                () => { _mask.Hide(); SoundManager.instance.ShowVoiceBtn(true); }));
        }

        /// <summary>
        /// 第四次点击语音键
        /// </summary>
        private void FourthlyOnClickVoiceBtn()
        {
            PlayVoice(1);
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 5,
               () => {
                   _mask.Show();
                 
                   _mono.StartCoroutine(Delay(0.3f, () => {                  
                       SpineManager.instance.DoAnimation(_spine2, "qt1", false); }));                 
               },
               () => { _mask.Hide(); SoundManager.instance.ShowVoiceBtn(true); }));
        }

        /// <summary>
        /// 第五次点击语音键
        /// </summary>
        private void FifthOnClickVoiceBtn()
        {
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,6,
               () => {
                   _mask.Show();
                   _mono.StartCoroutine(Delay(0.3f, () => { SpineManager.instance.DoAnimation(_spine2, "qt2", false); }));
               },
               () => { _mask.Hide();}));
        }

        #endregion


       void MyUpdate(float delay, Action callBack)
       {
            _mono.StartCoroutine(IEUpdate(delay,callBack));
       }

        IEnumerator IEUpdate(float delay, Action callBack)
        {
            while (_isStartMove)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        IEnumerator Delay(float delay,Action callBack)
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
                    FirstOnClickVoiceBtn();
                    break;
                case 2:
                    SecondOnClickVoiceBtn();
                    break;
                case 3:
                    ThirdOnClickVoiceBtn();
                    break;
                case 4:
                    FourthlyOnClickVoiceBtn();
                    break;
                case 5:
                    FifthOnClickVoiceBtn();
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
    }
}
