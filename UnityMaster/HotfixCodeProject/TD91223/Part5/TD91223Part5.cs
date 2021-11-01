using DG.Tweening;
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
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD91223Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
       

        #region 田丁
        private GameObject tt;
        private GameObject dtt;
        
          #region Mask
        private Transform anyBtns;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
       
        private string sz;
        #endregion
        
        #region 田丁对话

       

       

        #endregion
        
        #endregion

        #region 游戏物体

        private Transform _gamePanel;

        private GameObject _hitCat;
        private GameObject _finishedAni;

        private Transform _catShadowPanel;
        private Dictionary<int, GameObject> _catShaowDLevelDic;

        private Transform _catPanel;
        private Dictionary<string, GameObject> _catDic;
        

        private Transform _catStartPosPanel;
        private Dictionary<string, Vector3> _catStartPosDic;

        private List<GameObject> _catLeftList;
        private Dictionary<GameObject,int> _catClassDic;

        private Transform _devilHpPanel;
        private List<GameObject> _hpList;

        private GameObject _devil;

        private GameObject _correctEffect;

        private GameObject _boomEffect;

        private Transform _currentClickTarget;

        private int _currentGameLevel = 1;

        private bool _isClickTheAnimal = false;

        private bool _clickButtonSwitch = false;

        #endregion
        
       
        
        
        
        
        #endregion
        
        
       
        bool isPlaying = false;
       

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            Input.multiTouchEnabled = false;
                

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GamePrepared();
            //GameStart();
        }

        void GamePrepared()
        {
            mask.Show();
            successSpine.Hide();
            caidaiSpine.Hide();
            anyBtns.gameObject.Show();
            anyBtns.GetChild(1).gameObject.Hide();
            anyBtns.GetChild(0).gameObject.Show();
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }




        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            //田丁初始化
            TDGameInit();
        }

        

        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();
           
        }

        
        #region 田丁

        void TDGameInit()
        {
            _currentGameLevel = 1;
            _isClickTheAnimal = false;
            InitCatObject();
            SetCatStartPos();
            _finishedAni.Hide();
            _correctEffect.Hide();
            _boomEffect.Hide();
            _devil.Show();
            ReduceTheHp();
            ResetCatProperty();
            ShowTheCurrentLevelShaow();
            PlaySpineAni(_hitCat,"kong",false, () =>
            {
                PlaySpineAni(_hitCat, "m0",true);
            });
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Talk(tt, 0, null, () => SoundManager.instance.ShowVoiceBtn(true));
        }

            #endregion

        #endregion

        #region 游戏主体方法

        void ShowTheCurrentLevelShaow()
        {
            _catShaowDLevelDic[_currentGameLevel].Show();
            _catShaowDLevelDic[_currentGameLevel].GetComponent<RectTransform>().anchoredPosition=Vector2.zero;
            for (int i = 1; i <= 3; i++)
            {
                if (_currentGameLevel != i)
                {
                    _catShaowDLevelDic[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(1800f, 0);
                    _catShaowDLevelDic[i].Hide();
                }
                    
            }
        }

        void InitCatObject()
        {
            _catLeftList=new List<GameObject>();
            Transform catParent = _catShaowDLevelDic[_currentGameLevel].transform;
            for (int i = 0; i < catParent.childCount; i++)
            {
                Transform target = catParent.GetChild(i);
                GameObject leftCat = _catDic[target.name];
                _catLeftList.Add(leftCat);
            }
        }

        void SetCatStartPos()
        {
            for (int i = 0; i < _catPanel.childCount; i++)
            {
                Transform cat = _catPanel.GetChild(i);
                cat.position = _catStartPosDic[cat.name];
            }
        }

        void ReduceTheHp()
        {
            int hpNumber = 4 - _currentGameLevel;

            if (hpNumber > 0)
            {
                for (int i = 0; i < hpNumber; i++)
                {
               
                    _hpList[i].Show();
                }
            }

           

            if (hpNumber < 3&& hpNumber>=0)
            {
                for (int i = hpNumber; i < 3; i++)
                {
                    _hpList[i].Hide();
                }
            }
        }

        GameObject GetTargetGameObjectByDragType(int drageType)
        {
            switch (drageType)
            {
                case 1:
                    return _catDic["a"];
                   
                case 2:
                    return _catDic["b"];
                   
                case 3:
                    return _catDic["c"];
                   
                case 4:
                    return _catDic["d"];
                    
                case 5:
                    return _catDic["e"];
                   
                case 6:
                    return _catDic["f"];
                   
            }

            return null;
        }

        void DragStart(Vector3 position,int dragType,int index)
        {
            _currentClickTarget = GetTargetGameObjectByDragType(dragType).transform;
            _currentClickTarget.SetAsLastSibling();
          
            PlaySpineAni(_currentClickTarget.transform.GetGameObject("Ani"), "mao-" + _currentClickTarget.name + "2");
           
            
            // _currentClickTarget.GetComponent<ILDrager>().enabled = true;
            // _currentClickTarget.GetComponent<PointerClickListener>().enabled = false;
        }

        void DrageEnd(Vector3 position , int index,int type,bool isMatch)
        {
            //Debug.Log("index:" + index + " IsMatch" + isMatch);
            SoundManager.instance.Stop("voice");
            if (isMatch)
            {
                
                BtnPlaySoundSuccess();
                _correctEffect.Show();
                _correctEffect.transform.position = position;
                PlaySpineAni(_correctEffect, "xingxing", false, () => _correctEffect.Hide());
                Transform targetShowParent = _catShaowDLevelDic[_currentGameLevel].transform;
                //Debug.Log(targetShowParent+" "+_currentClickTarget.name);
                Transform clickObjectPos = targetShowParent.GetTransform(_currentClickTarget.name)
                    .GetTransform(_currentClickTarget.name);
                _currentClickTarget.position = clickObjectPos.position;
                _currentClickTarget.SetSiblingIndex(_catClassDic[_currentClickTarget.gameObject]);
                _currentClickTarget.GetComponent<ILDrager>().enabled = false;
                _catLeftList.Remove(_currentClickTarget.gameObject);
                _currentClickTarget = null;
                JudgeCatNumberLeftAndTurnToTheNext();
            }
            else
            {
                
                if (_currentClickTarget != null)
                {
                    _currentClickTarget.transform.position = _catStartPosDic[_currentClickTarget.name];
                    //_currentClickTarget.GetComponent<ILDrager>().DoReset();
                    BtnPlaySoundFail();
                    PlaySpineAni(_currentClickTarget.GetGameObject("Ani"), "mao-" + _currentClickTarget.name + "1",
                        true);
                    _currentClickTarget.GetComponent<ILDrager>().enabled = false;
                    // _currentClickTarget.GetComponent<ILDrager>().enabled = false;
                    // _currentClickTarget.GetComponent<PointerClickListener>().enabled = true;
                }

                
            }
        }

        void ResetCatProperty()
        {
            for (int i = 0; i < _catPanel.childCount; i++)
            {
                ILDrager target = _catPanel.GetChild(i).GetComponent<ILDrager>();
                PlaySpineAni(target.transform.GetGameObject("Ani"),"kong",false, () =>
                {
                    PlaySpineAni(target.transform.GetGameObject("Ani"), "mao-" + target.name + "1", true);
                });
                
                //target.enabled = false;
                target.enabled = false;
            }
        }
        
        void GetCurrentTargetGameObject(GameObject o)
        {
            //Debug.Log("点击之前的开关：" + _isClickTheAnimal + "  " + o.GetComponent<ILDrager>().enabled+" isplaying:"+isPlaying);
            if(_isClickTheAnimal||isPlaying==false)
                return;
            
            _isClickTheAnimal = true;
            //Debug.Log("点击之后的开关："+_isClickTheAnimal);
            //Debug.Log("o:"+o.name);
            _currentClickTarget = o.transform;
            
            if (SpineManager.instance.GetCurrentAnimationName(_currentClickTarget.transform.GetGameObject("Ani")) ==
                "mao-" + o.name + "2")
            {
                return;
            }

            _currentClickTarget.SetAsLastSibling();
            PlaySpineAni(_currentClickTarget.transform.GetGameObject("Ani"), "mao-" + _currentClickTarget.name + "2"); 
            _currentClickTarget.GetComponent<ILDrager>().enabled = true;
            // _currentClickTarget.GetComponent<PointerClickListener>().enabled = false;
        }

        void QuitClickAnimal(GameObject o)
        {
            if(isPlaying==false)
                return;
            if (_isClickTheAnimal)
            {
                if (_currentClickTarget != null)
                {

                    FindTargetClickObject(_currentClickTarget.name);
                }

                

                
                //Debug.Log("Quit click");
                _isClickTheAnimal = false;
            }
        }


        void FindTargetClickObject(string targetName)
        {
            Vector3 startPos = _catStartPosDic[targetName];
            if (Vector3.Distance(startPos, _currentClickTarget.position) < 5)
            {
                PlaySpineAni(_currentClickTarget.transform.GetGameObject("Ani"),
                    "mao-" + _currentClickTarget.name + "1", true);
            }

            

            
        }

        void JudgeCatNumberLeftAndTurnToTheNext()
        {
            if (_catLeftList.Count <= 0)
            {
                isPlaying = false;
                _catShaowDLevelDic[_currentGameLevel].gameObject.Hide();
                
                if (_currentGameLevel == 3)
                {
                    _finishedAni = _gamePanel.GetGameObject("FinishedAni2");
                }
                else
                {
                    _finishedAni = _gamePanel.GetGameObject("FinishedAni1");
                }

                _finishedAni.transform.SetAsFirstSibling();
                //PlaySpineAni(_finishedAni,"g"+_currentGameLevel,false, () =>
                _isClickTheAnimal = true;
                PlaySound(0);
                PlaySpineAni(_finishedAni,"light",false,()=>
                {
                    
                    PlaySpineAni( _hitCat, "m" + _currentGameLevel + "-boom", false, () =>
                    { 
                        _devil.Hide();
                        PlayVoice(2);
                        TurnToTheNextLevel();
                        // PlaySpineAni(_boomEffect, "boom", false, () =>
                        // {
                        //     
                        //     _boomEffect.Hide(); 
                        //     
                        // });
                        
                        
                    });
                  
                    
                });
                
            }
        }

        void TurnToTheNextLevel()
        {
            if (_currentGameLevel < 3)
            {
                isPlaying = true;
                _isClickTheAnimal = false;
                _currentGameLevel++;
                ReduceTheHp();
                _finishedAni.Hide();
                ShowTheCurrentLevelShaow();
                PlaySpineAni(_hitCat,"kong",false, () =>
                {
                    PlaySpineAni(_hitCat, "m0",true);
                });
               
                SetCatStartPos();
                InitCatObject();
                _devil.Show();
                ResetCatProperty();
            }
            else
            {
                _currentGameLevel = 4;
                ReduceTheHp();
                WinTheGame();
            }
            
        }

        void WinTheGame()
        {
            playSuccessSpine();
        }

        #endregion
       

        #region 说话语音

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
                speaker = tt;
            }
            SoundManager.instance.SetShield(false);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            

            
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            
            SpineManager.instance.DoAnimation(speaker, "speak");
            

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            
            SpineManager.instance.DoAnimation(speaker, "daiji");
            

            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 语音键对应方法
        
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //田丁游戏开始方法
                    //TDGameStartFunc();
                    Talk(tt,1,null, () =>
                    {
                        tt.Hide();
                        mask.Hide();
                        isPlaying = true;
                    });
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            
            tt.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); tt.SetActive(false); }));
        }

        #endregion

        #region 通用方法
        
        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target,string name,bool isLoop=false,Action callback=null)
        {
            target.Show();
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
            // SpineManager.instance.DoAnimation(target, "kong", false,
            //     () => SpineManager.instance.DoAnimation(target, name, isLoop, callback));
        }
        
        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index,Action goingEvent=null,Action finishEvent=null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target,SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }
        
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex,Action callback=null)
        {
            float voiceTimer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }
        
        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }
        
        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="callback"></param>
        void WaitTimeAndExcuteNext(float timer,Action callback)
        {
            
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer,Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();
            
        }
        
        
        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }
        
        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }
        
      
        
        #region 监听相关

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
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
        #endregion

        
        

        #endregion


        #region 田丁
        
          #region 田丁加载

          /// <summary>
          /// 田丁加载所有物体
          /// </summary>
          void TDLoadGameProperty()
          {
              mask = curTrans.Find("mask").gameObject;
              mask.SetActive(false);
            
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();
              //加载游戏物体
              TDLoadGameObject();
          }

          void TDLoadGameObject()
          {
              _gamePanel = curTrans.GetTransform("GamePanel");
              _catLeftList=new List<GameObject>();
              _catDic=new Dictionary<string, GameObject>();
              _catStartPosDic=new Dictionary<string, Vector3>();
              _catShaowDLevelDic=new Dictionary<int, GameObject>();
              _hpList=new List<GameObject>();
              _hitCat = _gamePanel.GetGameObject("HitCat");
              _catShadowPanel = _gamePanel.GetTransform("CatShadowPanel");
              _finishedAni = _gamePanel.GetGameObject("FinishedAni1");
              _correctEffect = _gamePanel.GetGameObject("CorrectEffect");
              _boomEffect = _gamePanel.GetGameObject("BoomEffect");
              _devil = _gamePanel.GetGameObject("Devil");
              _catClassDic=new Dictionary<GameObject, int>();
              for (int i = 0; i < _catShadowPanel.childCount; i++)
              {
                  GameObject target = _catShadowPanel.GetChild(i).gameObject;
                  int level = Convert.ToInt32(target.name.Replace("Level", ""));
                  _catShaowDLevelDic.Add(level, target);
                  // for (int j = 0; j < target.transform.childCount; j++)
                  // {
                  //     ILDroper targetDroper = target.transform.GetChild(j).GetComponent<ILDroper>();
                  //     targetDroper.SetDropCallBack(DropEnd);
                  // }
              }

              _catPanel = _gamePanel.GetTransform("CatPanel");
              for (int i = 0; i < _catPanel.childCount; i++)
              {
                  ILDrager target = _catPanel.GetChild(i).GetComponent<ILDrager>();
                  _catDic.Add(target.name, target.gameObject);
                  _catClassDic.Add(target.gameObject, target.transform.GetSiblingIndex());
                  target.SetDragCallback(null,null,DrageEnd);
                  PointerClickListener.Get(target.gameObject).clickDown = GetCurrentTargetGameObject;
                  PointerClickListener.Get(target.gameObject).clickUp = QuitClickAnimal;
              }

              _catStartPosPanel = _gamePanel.GetTransform("CatStartPosPanel");
              for (int i = 0; i < _catStartPosPanel.childCount; i++)
              {
                  Transform target = _catStartPosPanel.GetChild(i);
                  _catStartPosDic.Add(target.name, target.position);
              }

              _devilHpPanel = _gamePanel.GetTransform("DevilHpPanel/Hp");
              for (int i = 0; i < _devilHpPanel.childCount; i++)
              {
                  GameObject target = _devilHpPanel.GetChild(i).gameObject;
                  _hpList.Add(target);
              }
          }

        


          /// <summary>
          ///  加载人物
         /// </summary>
          void TDLoadCharacter()
          {
              tt = curTrans.Find("mask/TT").gameObject;
              tt.SetActive(false);
              dtt = curTrans.Find("mask/DTT").gameObject;
              dtt.SetActive(false);
          }
        
        

          /// <summary>
          /// 加载成功环节
          /// </summary>
          void TDLoadSuccessPanel()
          {
              successSpine = curTrans.Find("mask/successSpine").gameObject;
              successSpine.SetActive(false);
              caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
              caidaiSpine.SetActive(false);
              //替换胜利动画需要替换spine 
             
              sz = "6-12-z";
          }
          /// <summary>
          /// 加载按钮
          /// </summary>
          void TDLoadButton()
          {
              anyBtns = curTrans.Find("mask/Btns");
              for (int i = 0; i < anyBtns.childCount; i++)
              {
                  Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                  anyBtns.GetChild(i).gameObject.SetActive(false);
              }

              _clickButtonSwitch = false;
              anyBtns.gameObject.SetActive(true);
              anyBtns.GetChild(0).gameObject.SetActive(true);
              anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
          }
        
       
        

        #endregion

         

         

          

          #region 切换游戏按键方法

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            
            if(_clickButtonSwitch)
                return;
            
            BtnPlaySound();
            _clickButtonSwitch = true;
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                        _clickButtonSwitch = false;
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit();
                        isPlaying = true;
                        _clickButtonSwitch = false;
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dtt.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dtt, SoundManager.SoundType.VOICE, 3,null,()=>_clickButtonSwitch = false)); });
                }

            });
        }
        

        #endregion

         
        
          #region 田丁成功动画

        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                        () =>
                        {
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion
       

        #endregion

        

        

    }
}
