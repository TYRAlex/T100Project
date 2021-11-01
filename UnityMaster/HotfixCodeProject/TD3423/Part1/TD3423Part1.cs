using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Spine.Unity;
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

    public enum E_CurrentColor
    {
        None=0,
        Green,
        Orange,
        Purple
    }

    public class TD3423Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject tt;
        private GameObject xtt;
        
          #region Mask
        private Transform anyBtns;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion
        
        #region 田丁对话

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
       

        #endregion
        
        #endregion

       
        private int flag = 0;
       
        #endregion

        #region 游戏物体

        private Transform _gamePanel;
        
        private GameObject _mainHumanAni;

        private GameObject _drawPanelAni;

        private ILDrager _pen;
        private GameObject _pensAni;
        private Transform _pensTip;

        private Transform _bodyPanel;
        private Transform _level1Body;
        private Transform _level2Body;
        private Transform _level3Body;

        private Transform _bodyTrigger;

        private Dictionary<E_CurrentColor, Dictionary<string, GameObject>> _bodyDic;
        private Dictionary<E_CurrentColor, Dictionary<string, GameObject>> _bodyOriginalDic;

        private E_CurrentColor _currentcolor = E_CurrentColor.None;
        private E_CurrentColor _penColor = E_CurrentColor.None;
        private bool _isCoverTheColor=false;
        private Transform _colorChoosePanel;
        private int _currentGameLevel = 1;

        private GameObject _drawPanelPurple;
        private GameObject _drawPanelOrange;
        private GameObject _drawPanelGreen;
        private GameObject _bee;
        private GameObject _beeTrail;

        private bool _isclickButton = false;
        #endregion
        
       
        bool isPlaying = false;
        private bool _judgeIfFinished = false;
        private bool _isPainting = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            

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
            _pen.gameObject.Hide();
            mask.Show();
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
            isPlaying = false;
            _judgeIfFinished = false;
            _isPainting = false;
            _isclickButton = false;
            ResetBG();
            _isCoverTheColor = false;
            _bodyDic.Clear();
            _bodyOriginalDic.Clear();
            for (int i = 0; i < _bodyPanel.childCount; i++)
            {
                Transform currentLevelBody = _bodyPanel.GetChild(i);
                E_CurrentColor targetColorEnum = (E_CurrentColor) (i + 1);
                _bodyDic.Add(targetColorEnum, new Dictionary<string, GameObject>());
                _bodyOriginalDic.Add(targetColorEnum, new Dictionary<string, GameObject>());
                for (int j = 0; j < currentLevelBody.childCount; j++)
                {
                    Transform target = currentLevelBody.GetChild(j);
                    _bodyDic[targetColorEnum].Add(target.name, target.gameObject);
                    _bodyOriginalDic[targetColorEnum].Add(target.name, target.gameObject);
                    target.GetComponent<SkeletonGraphic>().color = new Color(1f, 1f, 1f, 0f);
                }
            }
            _currentcolor = E_CurrentColor.Green;
            _currentGameLevel = 1;
            textSpeed = 0.1f;
            flag = 0;
            _pen.transform.SetAsLastSibling();
            _pen.DoReset();
            _penColor = E_CurrentColor.None;
            PlaySpineAni(_mainHumanAni, "r");
            ResetAllLevelBodyColor();
            ShowTheTargetBody();
        }

        void ResetBG()
        {
            ChangeBG(E_CurrentColor.Green);
            ResetPenAni();
        }

        void ResetPenAni()
        {
            PlaySpineAni(_pensAni, "b-0", false);
        }



        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            // Talk(bd,0,null, () =>
            // {
            //     SoundManager.instance.ShowVoiceBtn(true);
            // });
           
            
            isPlaying = true;
            Talk(tt,0,null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            });
            
            // buDing.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            // {
            //     ShowDialogue("小朋友们，让我们帮助丁丁躲避蜜蜂的追捕吧", bdText);
            //     PlayVoice(0, () =>
            //     {
            //         SoundManager.instance.ShowVoiceBtn(true);
            //     });
            // });
            // devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            // {
            //     mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
            //     {
            //         ShowDialogue("", devilText);
            //     }, () =>
            //     {
            //         buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            //         {
            //             mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
            //             {
            //                 ShowDialogue("", bdText);
            //             }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //         });
            //     }));
            // });
        }

        void StartMaintGame()
        {
            PlaySpineAni(_drawPanelAni,"kong",false, () =>
            {
                PlaySound(2);
                PlaySpineAni(_drawPanelAni,"hb-0",false, () =>
                {
                    PlaySpineAni(_drawPanelAni, "hb-1", false);
                    PlaySpineAni(_drawPanelPurple,"hb-c1",false, () =>
                    {
                        PlaySpineAni(_drawPanelOrange,"hb-c2",false, () =>
                        {
                            PlaySpineAni(_drawPanelGreen,"hb-c3",false, () =>
                            {
                                _pen.gameObject.Show();
                            });
                        });
                    });
                });
            });
        }

        #endregion

        #endregion

        #region 游戏主体方法
        
        void ChangeBG(E_CurrentColor currentColor)
        {
            RawImage targetImage = Bg.GetComponent<RawImage>();
            if (currentColor == E_CurrentColor.Green)
            {
                targetImage.texture = bellTextures.texture[0];
            }
            else if (currentColor == E_CurrentColor.Orange)
            {
                targetImage.texture = bellTextures.texture[1];
            }
            else if (currentColor == E_CurrentColor.Purple)
            {
                targetImage.texture = bellTextures.texture[2];
            }
        }

      

        void ShowTheTargetBody()
        {
            if (_currentGameLevel == 1)
            {
                _level1Body.gameObject.Show();
                _level2Body.gameObject.Hide();
                _level3Body.gameObject.Hide();
            }
            else if (_currentGameLevel == 2)
            {
                _level1Body.gameObject.Hide();
                _level2Body.gameObject.Show();
                _level3Body.gameObject.Hide();
            }
            else if (_currentGameLevel == 3)
            {
                _level1Body.gameObject.Hide();
                _level2Body.gameObject.Hide();
                _level3Body.gameObject.Show();
            }
        }

        void HideTheCurrentBody()
        {
            if (_currentGameLevel == 1)
            {
                _level1Body.gameObject.Hide();
                
            }
            else if (_currentGameLevel == 2)
            {
              
                _level2Body.gameObject.Hide();
               
            }
            else if (_currentGameLevel == 3)
            {
                
                _level3Body.gameObject.Hide();
            }
        }

        void ResetAllLevelBodyColor()
        {
            _level1Body.gameObject.Show();
            _level2Body.gameObject.Show();
            _level3Body.gameObject.Show();
            foreach (E_CurrentColor color in Enum.GetValues(typeof(E_CurrentColor)))
            {
                if(color==E_CurrentColor.None)
                    continue;
                ResetLevelBodyColor(color);
            }
        }
        
        void ResetLevelBodyColor(E_CurrentColor currentColor)
        {
            //Debug.Log("初始化的颜色为："+currentColor);
            for (int i = 1; i <= 8; i++)
            {
               
                //GameObject target = _bodyDic[currentColor][i.ToString()];
                //Debug.Log("i:" + i+" "+target.name);
                _bodyOriginalDic[currentColor][i.ToString()].GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
            }
        }

        bool DropCallBack(int dragType,int index,int drogType)
          {
              //Debug.Log("dragType:" + dragType + " index:" + index + "DrogType:" + drogType);
              if (isPlaying)
              {
                  if (dragType == drogType)
                  {
                      return true;
                  }
              }

              return false;
          }


          void DragGoing(Vector3 position,int type,int index)
          {
              
              //Debug.Log("正在不断的接触当中"+position+" Type:"+type+" Index:"+index);
              if (isPlaying)
              {
                  if (_isCoverTheColor == false)
                  {
                      string getColorName = GetColor(_pensTip.position);
                      if (getColorName != string.Empty)
                      {
                          _isCoverTheColor = true;
                          if (getColorName.Equals(_currentcolor.ToString()))
                          {
                              //笔触变成相应颜色
                              //Debug.Log("正确的颜色");
                              _penColor = _currentcolor;
                              string penName = JudgeColorAndGetName(getColorName);
                              DoTheDrawPanelAni(_currentcolor);
                              PlaySpineAni(_pensAni, penName);
                              PlaySound(0);
                              BtnPlaySoundSuccess();
                          }
                          else
                          {
                              //todo... 提示错误
                              _penColor = E_CurrentColor.None;
                              Debug.Log("错误的颜色，请更换~ ");
                              BtnPlaySoundFail(()=>_isCoverTheColor = false);
                              PlaySpineAni(_pensAni, "b-6", false);
                          }
                      }
                  }
                  else
                  {
                      if(_penColor==E_CurrentColor.None)
                          return;
                      string targetName = SearchIfIsInTheArea(_pensTip.position);
                      if (targetName != string.Empty)
                      {
                          GameObject targetAni = null;
                          if (_bodyDic[_currentcolor].TryGetValue(targetName, out targetAni))
                          {
                              Debug.Log("碰到的物体的名字是："+targetName);
                              _bodyDic[_currentcolor].Remove(targetName);
                              if (_isPainting == false)
                                  ShowTheBodySlowly(targetAni);
                          }
                          // else
                          // {
                          //     Debug.Log("没找到相应的对象，请检查！ " + targetName);
                          // }
                  
                      }
                  }
              }

             


              
              

              
              //Debug.Log("目标内容"+ targetName);
          }

          void DoTheDrawPanelAni(E_CurrentColor color)
          {
              _drawPanelGreen.Hide();
              _drawPanelOrange.Hide();
              _drawPanelPurple.Hide();
              if (color == E_CurrentColor.Purple)
              {
                  PlaySpineAni(_drawPanelAni, "hb-2");
              }
              else if (color == E_CurrentColor.Orange)
              {
                  PlaySpineAni(_drawPanelAni, "hb-3");
              }
              else if (color == E_CurrentColor.Green)
              {
                  PlaySpineAni(_drawPanelGreen, "hb-4");
              }
          }

          string JudgeColorAndGetName(string colorName)
          {
              string targetName=String.Empty;
              if (colorName == "Green")
              {
                  targetName = "b-3";
              }
              else if (colorName == "Orange")
              {
                  targetName = "b-4";
              }
              else if (colorName == "Purple")
              {
                  targetName = "b-5";
              }

              return targetName;
          }

          void ShowTheBodySlowly(GameObject target)
          {
              isPlaying = true;
              target.GetComponent<SkeletonGraphic>().DOFade(2, 1f).OnComplete(() =>JudgeIfFinished());
          }

          void JudgeIfFinished()
          {
              
              if (_bodyDic[_currentcolor].Count <= 0)
              {
                  if (_judgeIfFinished == false)
                  {
                      _judgeIfFinished = true;
                      HumanHideTheBee(TurnToTheNextStep);

                  }
              }
          }

          void HumanHideTheBee(Action callback)
          {
              PlayVoice(2);
              ResetLevelBodyColor(_currentcolor);
              HideTheCurrentBody();
              PlaySpineAni(_bee,"kong",false, () =>
              {
                  PlaySound(1);
                  PlaySpineAni(_bee,"mf",false, () =>
                  {
                      PlaySpineAni(_beeTrail,"mf-x",false, () =>
                      {
                          PlaySpineAni(_beeTrail, "mf-x2", false, () =>
                          {
                              callback?.Invoke();
                          });
                      });
                  });
              });
              
              PlaySpineAni(_mainHumanAni,GetCurrentHideHumanName(),false, () =>
              {
                 
              });
          }

          string GetCurrentHideHumanName()
          {
              string targetName = null;
              if (_currentcolor == E_CurrentColor.Green)
              {
                  targetName = "rb-9";
              }
              else if (_currentcolor == E_CurrentColor.Orange)
              {
                  targetName = "ra-9";
              }
              else if (_currentcolor == E_CurrentColor.Purple)
              {
                  targetName = "rc-9";
              }

              return targetName;
          }


          void TurnToTheNextStep()
          {
              _pen.DoReset();
              PlaySpineAni(_pensAni, "b-0", false);
              _isCoverTheColor = false;
              isPlaying = false;
              _isPainting = false;
              PlaySpineAni(_mainHumanAni, "r", false);

              if (_currentGameLevel < 3)
              {
                  //Debug.Log("11111"+_currentcolor);

                  //Debug.Log("333333");
                  _currentGameLevel++;
                  _currentcolor = (E_CurrentColor) _currentGameLevel;
                  //Debug.Log("22222"+_currentcolor);
                  ShowTheTargetBody();
                  _judgeIfFinished = false;
                  ChangeBG(_currentcolor);
                  isPlaying = true;
                  _penColor = E_CurrentColor.None;
              }
              else
              {
                  _currentGameLevel = 1;
                  WinTheGame();
              }


          }

          void ExcuteHideAniAndTurnTotheNext(Action callback=null)
          {
              //todo...执行小孩子捂脸，伴随语音：田田：快藏好，小蜜蜂来了 ，蜜蜂没发现丁丁，然后丁丁胜利捂住嘴窃喜
              callback?.Invoke();
          }

          void WinTheGame()
          {
              playSuccessSpine();
          }

          string SearchIfIsInTheArea(Vector3 positon)
          {
              for (int i = 0; i < _bodyTrigger.childCount; i++)
              {
                  RectTransform target = _bodyTrigger.GetChild(i).GetRectTransform();
                  if (IsInTheReactArea(target, positon))
                  {
                      return target.name;
                  }
              }

              return string.Empty;
          }

          string GetColor(Vector3 position)
          {
              for (int i = 0; i < _colorChoosePanel.childCount; i++)
              {
                  RectTransform target = _colorChoosePanel.GetChild(i).GetRectTransform();
                  if (IsInTheReactArea(target, position))
                  {
                      return target.name;
                  }
              }

              return string.Empty;
          }

          bool IsInTheReactArea(RectTransform originalPos, Vector3 targetPosition)
          {
              float minX = originalPos.position.x - originalPos.sizeDelta.x/2f;
              float maxX = originalPos.position.x + originalPos.sizeDelta.x/2f;
              float minY = originalPos.position.y - originalPos.sizeDelta.y/2f;
              float maxY = originalPos.position.y + originalPos.sizeDelta.y/2f;
              bool isXInTheArea = targetPosition.x >= minX && targetPosition.x <= maxX;
              bool isYInTheArea = targetPosition.y >= minY && targetPosition.y <= maxY;
              if (isXInTheArea && isYInTheArea)
                  return true;
              else
                  return false;
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
            
            SpineManager.instance.DoAnimation(speaker, "animation");
            
           

            
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            
            SpineManager.instance.DoAnimation(speaker, "animation2");
            

            

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
           
            SpineManager.instance.DoAnimation(speaker, "animation");
            
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
                    Talk(tt,1,null, () =>
                    {
                        buDing.SetActive(false);
                        devil.SetActive(false);
                        tt.Hide();
                        mask.Hide();
                        StartMaintGame();
                    });
                    //ShowDialogue("选择和背景一样的颜色，涂在丁丁的身体上，帮助他隐身吧", bdText);
                    //田丁游戏开始方法
                    // PlayVoice(1, () =>
                    // {
                    //     buDing.SetActive(false);
                    //     devil.SetActive(false);
                    //     mask.Hide();
                    //     StartMaintGame();
                    // });

                    break;
                case 2:
                    TDGameStartFunc();
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            buDing.SetActive(false);
            devil.SetActive(false);
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
        private void BtnPlaySoundFail(Action callback)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
            WaitTimeAndExcuteNext(timer, callback);
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

        #region 修改Rect相关

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
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
              //任务对话方法加载
              TDLoadDialogue();
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();
              //加载游戏物体
              LoadGameProperty();
          }

          void LoadGameProperty()
          {
              _gamePanel = curTrans.GetTransform("GamePanel");
              _mainHumanAni = _gamePanel.GetGameObject("Human");
              _pen = _gamePanel.GetTargetComponent<ILDrager>("Panel/Pen");
              _pen.SetDragCallback(null,DragGoing);
              _pensAni = _pen.transform.GetGameObject("PenAni");
              _pensTip = _pen.transform.GetTransform("Draw");
              _bodyPanel = _gamePanel.GetTransform("BodyPanel");
              _level1Body = _bodyPanel.GetTransform("Level1Body");
              _level2Body = _bodyPanel.GetTransform("Level2Body");
              _level3Body = _bodyPanel.GetTransform("Level3Body");
              _bodyTrigger = _gamePanel.GetTransform("BodyTrigger");
              _bodyDic=new Dictionary<E_CurrentColor, Dictionary<string, GameObject>>();
              _bodyOriginalDic=new Dictionary<E_CurrentColor, Dictionary<string, GameObject>>();
              
              for (int i = 0; i < _bodyTrigger.childCount; i++)
              {
                  ILDroper targetDroper = _bodyTrigger.GetChild(i).GetComponent<ILDroper>();
                  targetDroper.SetDropCallBack(DropCallBack);
                  RectTransform target = targetDroper.transform.GetRectTransform();
                  Debug.Log("1"+target.pivot+"2"+target.anchoredPosition+"3"+target.anchorMax+"4"+target.anchorMin+"5"+target.sizeDelta+"6"+target.position);
              }
              
              _colorChoosePanel = _gamePanel.GetTransform("ColorChoosePanel");
              _drawPanelGreen = _gamePanel.GetGameObject("DrawPanelGreen");
              _drawPanelOrange = _gamePanel.GetGameObject("DrawPanelOrange");
              _drawPanelPurple = _gamePanel.GetGameObject("DrawPanelPurple");
              _drawPanelAni = _gamePanel.GetGameObject("DrawPanel");
              _bee = _gamePanel.GetGameObject("Bee");
              _beeTrail = _gamePanel.GetGameObject("BeeTrail");
              _mainHumanAni.Show();
              _drawPanelGreen.Hide();
              _drawPanelOrange.Hide();
              _drawPanelPurple.Hide();
              _drawPanelAni.Hide();
              _bee.Hide();
              _beeTrail.Hide();
          }
          
          

        

          /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            
            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            xtt = curTrans.Find("mask/XTT").gameObject;
            xtt.SetActive(false);
        }
        
        
        /// <summary>
        /// 加载对话环节
        /// </summary>
        void TDLoadDialogue()
        {
            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");
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
            tz = "3-5-z";
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
            if(_isclickButton)
                return;
            _isclickButton = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                        _isclickButton = false;
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                        isPlaying = true;
                        _isclickButton = false;
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); xtt.SetActive(true); _mainHumanAni.Hide();_isclickButton = false;  mono.StartCoroutine(SpeckerCoroutine(xtt, SoundManager.SoundType.VOICE, 3)); });
                }

            });
        }
        

        #endregion

          #region 田丁对话方法

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            text.text = "";
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                // if (i == 25)
                // {
                //     text.text = "";
                // }
                i++;
            }
            callBack?.Invoke();
            yield break;
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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
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
