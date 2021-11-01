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

    public enum E_Color
    {
        None=0,
        Green,
        Blue,
        Purple,
        Red,
        Yellow,
        DeepBlue,
        
    }

    public class TD3423Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject dd;
        private GameObject XDD;
        
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
        
       
        
        #endregion

        #region 游戏物体

        private Transform _gamePanel;

        private Transform _colorPanel;

        private Transform _colorContent;

        private ILDrager _pen;
        private GameObject _penAni;
        private Transform _pensTip;

        private Image _potImage;

        private Transform _potAreaParent;
        private Dictionary<int, Transform> _gameLevelAreaDic;

        private Dictionary<int, Sprite> _bgSpriteDic;

        private Transform _potColorParent;
        private Dictionary<int, Dictionary<string, Image>> _imageDic;

        private Dictionary<int, Dictionary<string, Sprite>> _spriteDic;

        private Dictionary<int, Dictionary<GameObject, bool>> _allColorCoverStatuDic; 

        private int _currentGameLevel = 1;
        private E_Color _currentColor = E_Color.Green;
        private bool _isFinishedPaiting = false;

        // private GameObject _bee1;
        // private GameObject _bee2;
        // private GameObject _bee3;
        private List<GameObject> _allBeeList;
        private Dictionary<string, Transform> _allBeePosDic;

        private Transform _beeParentItem;
        private GameObject _devil;
        private GameObject _honey;

        private Transform _deviHpPanel;
        private List<GameObject> _devilHpList;

        private GameObject _starEffect;

        private bool _clickSwitch = false;
        
        #endregion
   
      
        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        #endregion

        private bool _isCoverTheColor = false;
        bool isPlaying = false;
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

        #region 初始化和游戏开始方法

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

        private void GameInit()
        {
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
            _clickSwitch = false;
            dd.Hide();
            XDD.Hide();
            talkIndex = 1;
            _isCoverTheColor = false;
            _currentGameLevel = 1;
            UpdateDevilHp();
            flag = 0;
            _currentColor = E_Color.None;
            _isFinishedPaiting = true;
            _potImage.sprite = _bgSpriteDic[_currentGameLevel];
            PlaySpineAni(_devil, "xem1", true);
            _honey.Hide();
            ResetBeeStatu();
            TDLoadAllGameImage();
            SetTargetPotColorImageVisible(true);
            _colorPanel.GetTargetComponent<Scrollbar>("ColorItem/Scrollbar Vertical").value = 1;
        }
        void TDGameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Talk(dd, 0, null, () => SoundManager.instance.ShowVoiceBtn(true));
                // PlayVoice(0, () => SoundManager.instance.ShowVoiceBtn(true));
                // ShowDialogue("小朋友们，我们一起来装饰送给维尼的生日礼物吧", bdText);
          

            //AllBeeMoveTogether();
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

        void StartMainGame()
        {
            _pen.transform.SetAsLastSibling();
            isPlaying = true;
            
            _colorPanel.DOMoveX(300f, 0.8f).SetEase(Ease.OutQuart);
            
            mask.Hide();
        }

        #endregion

        #endregion

        #region 游戏主体方法

        

        void ResetEndStatu()
        {
            _honey.Hide();
            ResetBeeStatu();
            PlaySpineAni(_devil, "xem1", true);
        }

        void WinTheGame()
        {
            isPlaying = false;
            playSuccessSpine();
        }

        void ClickColorEvent(GameObject obj)
        {
            _isCoverTheColor = true;
            _pen.gameObject.Show();
            _pen.transform.position = obj.transform.position + new Vector3(230f, 0, 0);
            Debug.Log(obj.name);
            _currentColor = (E_Color) Enum.Parse(typeof(E_Color), obj.name);
            PlaySound(3, () => _isCoverTheColor = false);
            PlaySpineAni(_penAni, GetTargetPenName(_currentColor), false);

        }

        string GetTargetPenName(E_Color color)
        {
            StringBuilder penName = new StringBuilder("hb-");
            int colorIndex = (int) color;
            int colorName = colorIndex + 3;
            penName.Append(colorName);
            return penName.ToString();
        }

        // void DragStart(Vector3 position, int type, int index)
        // {
        //     RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //     Debug.Log(hit.collider.name);
        // }

        void DragGoing(Vector3 position, int type, int index)
        {
            if (isPlaying)
            {
                GetTargetPartNameAndPaiting(_pensTip.position);


                string getColorName = GetColor(_pensTip.position);
                if (getColorName != string.Empty&&_isCoverTheColor==false)
                {
                    _isCoverTheColor = true;
                    _currentColor = (E_Color) Enum.Parse(typeof(E_Color), getColorName);
                    PlaySound(3, () => _isCoverTheColor = false);
                    PlaySpineAni(_penAni, GetTargetPenName(_currentColor), false);
                }
                // if (_isCoverTheColor)
                // {
                //     Debug.Log("填色状态");
                //     GetTargetPartNameAndPaiting(_pensTip.position);
                // }
                // else
                // {
                //     Debug.Log("提取颜料状态");
                //     string getColorName = GetColor(_pensTip.position);
                //     if (getColorName != string.Empty)
                //     {
                //         _isCoverTheColor = true;
                //         _currentColor = (E_Color) Enum.Parse(typeof(E_Color), getColorName, false);
                //         PlaySpineAni(_penAni, GetTargetPenName(_currentColor));
                //     }
                // }
            }
        }

        string GetColor(Vector3 position)
        {
            for (int i = 0; i < _colorContent.childCount; i++)
            {
                RectTransform targetColor = _colorContent.GetChild(i).GetRectTransform();
                if(IsInTheReactArea(targetColor,position))
                {
                    return targetColor.name;
                }
            }

            return string.Empty;
        }

        void DrageEnd(Vector3 position, int type, int index,bool isMatch)
        {
            _pen.gameObject.Hide();
            _isCoverTheColor = false;
        }

        void UpdateDevilHp()
        {
            int hpNumber = 4 - _currentGameLevel;

            if (hpNumber > 0)
            {
                for (int i = 0; i < hpNumber; i++)
                {
               
                    _devilHpList[i].Show();
                }
            }

           

            if (hpNumber < 3&& hpNumber>=0)
            {
                for (int i = hpNumber; i < 3; i++)
                {
                    _devilHpList[i].Hide();
                }
            }

            
        }

        void GetTargetPartNameAndPaiting(Vector3 currentPos)
        {
            Transform targetParent = _gameLevelAreaDic[_currentGameLevel];
            string name = SearchIfIsInTheArea(currentPos, targetParent);
            // Debug.Log(name+"   "+_isFinishedPaiting);
            if (name != string.Empty)
            {
                if (_currentColor == E_Color.None)
                {
                    Debug.LogError("环节有错误，这个时候不应该还是没有颜色的状态，请检查！");
                    return;
                }

                string colorName = name + ((int) _currentColor).ToString();
                if (_isFinishedPaiting == true)
                { 
                    ShowTheTargetImageColor(name, colorName);
                }

                
            }

            
        }

        void ShowTheTargetImageColor(string partPosName,string colorName)
        {
            Image targetImage = null;
            if (_imageDic[_currentGameLevel].TryGetValue(partPosName, out targetImage))
            {
                _isFinishedPaiting = false;
                targetImage.color = new Color(1, 1, 1, 0);
                targetImage.gameObject.Show();
                targetImage.sprite = _spriteDic[_currentGameLevel][colorName];
                _allColorCoverStatuDic[_currentGameLevel][targetImage.gameObject] = true;
                PlaySound(1);
                targetImage.DOFade(1, 2f).OnComplete(() =>
                    {
                        //PlayVoice(2,()=>_isFinishedPaiting = true);
                       
                        _isFinishedPaiting = true;
                        _imageDic[_currentGameLevel].Remove(partPosName);
                        JudgeColorPaintIfFinished();
                    }
                );
                // if (_isFinishedPaiting == true)
                // {
                //     _isFinishedPaiting = false;
                //     targetImage.color = new Color(1, 1, 1, 0);
                //     targetImage.gameObject.Show();
                //     targetImage.sprite = _spriteDic[_currentGameLevel][colorName];
                //     _allColorCoverStatuDic[_currentGameLevel][targetImage.gameObject] = true;
                //     targetImage.DOFade(1, 2f).OnComplete(JudgeColorPaintIfFinished);
                // }

            }

            
            
        }

        void JudgeColorPaintIfFinished()
        {
            //_isFinishedPaiting = true;
            // foreach (GameObject targetGameObject in _allColorCoverStatuDic[_currentGameLevel].Keys)
            // {
            //     if (_allColorCoverStatuDic[_currentGameLevel][targetGameObject] == false)
            //     {
            //         return;
            //     }
            // }
            
            if (_imageDic[_currentGameLevel].Keys.Count <= 0&& isPlaying)
                TurnToTheNextPot();
        }

        void TurnToTheNextPot()
        {
            isPlaying = false;
            if (_currentGameLevel < 3)
            {
                _currentGameLevel++;
            }
            else
            {
                _currentGameLevel = 4;
            }

            AllBeeMoveTogether(() =>
            {
                if (_currentGameLevel <= 3)
                {
                    //_currentGameLevel++;
                    _isCoverTheColor = false;
                    
                    _isFinishedPaiting = true;
                    _potImage.sprite = _bgSpriteDic[_currentGameLevel];
                    TDLoadAllGameImage();
                    SetTargetPotColorImageVisible(true);
                    ResetEndStatu();
                    _colorPanel.GetTargetComponent<Scrollbar>("ColorItem/Scrollbar Vertical").value = 1;
                    isPlaying = true;
                }
                else
                {
                    //_currentGameLevel = 4;
                    //DevilHp();
                    WinTheGame();
                }
            });
            
           
            
        }

        string SearchIfIsInTheArea(Vector3 positon,Transform targetParent)
        {
            for (int i = 0; i < targetParent.childCount; i++)
            {
                RectTransform target = targetParent.GetChild(i).GetRectTransform();
                if (IsInTheReactArea(target, positon))
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

        void SetTargetPotColorImageVisible(bool isShow)
        {
            if (isShow)
            {
                for (int i = 1; i <= _allColorCoverStatuDic[_currentGameLevel].Keys.Count; i++)
                {
                    Image targetImage = _imageDic[_currentGameLevel][i.ToString()];
                    targetImage.color = new Color(1, 1, 1, 0);
                    _allColorCoverStatuDic[_currentGameLevel][targetImage.gameObject] = false;
                    targetImage.gameObject.Show();
                }
                for (int i = 1; i <= 3; i++)
                {
                    if (i == _currentGameLevel)
                        continue;

                    for (int j = 1; j <= _allColorCoverStatuDic[i].Keys.Count; j++)
                    {
                        GameObject targetGameObject = _imageDic[i][j.ToString()].gameObject;
                        _allColorCoverStatuDic[i][targetGameObject] = false;
                        targetGameObject.Hide();
                    }

                   
                }
            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= _allColorCoverStatuDic[i].Keys.Count; j++)
                    {
                        GameObject targetGameObject = _imageDic[i][j.ToString()].gameObject;
                        _allColorCoverStatuDic[i][targetGameObject] = false;
                        targetGameObject.Hide();
                    }
                }
            }
        }

        void AllBeeMoveTogether(Action callback)
        {
            _honey.Show();
            BtnPlaySoundSuccess();
            PlaySpineAni(_starEffect, "guang", false,()=>_starEffect.Hide());
            PlaySpineAni(_honey, "kong", false, () => PlaySpineAni(_honey, "fm", false, () =>
            {
                PlaySound(2);
                for (int i = 0; i < _allBeeList.Count; i++)
                {
                    GameObject target = _allBeeList[i];
                    //Debug.Log(target.name);
                    
                    PlaySound(0);
                    _honey.Show();
                    BeeMoveAndStickTheDevil(target, callback);
                }
            }));
            
           
        }

        void BeeMoveAndStickTheDevil(GameObject target,Action callback=null)
        {
            //Vector3 startPos = _allBeePosDic[target.name + "StartPos"].position;
           
            
            PlaySpineAni(target, "mf1", true);
            Vector3 endPos = _allBeePosDic[target.name + "EndPos"].position;
            Debug.Log(endPos);
            Tweener tw = target.transform.DOMove(endPos, 2f);
            if (target.name.Equals("mf1"))
                tw.OnComplete(() => { PlaySpineAni(target, "mf3", false, () => { PlaySpineAni(target, "mf1", true); });
                    UpdateDevilHp();
                    PlaySpineAni(_devil, "xem2", false,
                        () => PlaySpineAni(_devil, "xem3", false, () => PlayVoice(3, callback)));
                });
            else
                tw.OnComplete(() => PlaySpineAni(target, "mf2", true));
        }

        void ResetBeeStatu()
        {
            for (int i = 0; i < _allBeeList.Count; i++)
            {
                GameObject target = _allBeeList[i];
                if (target.name.Equals("mf1"))
                {
                    PlaySpineAni(target, "mf1", true);
                }
                else
                {
                    PlaySpineAni(target, "mf2", true);
                }

                Vector3 startPos = _allBeePosDic[target.name + "StartPos"].position;
                target.transform.position = startPos;
            }
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
                speaker = dd;
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
                    //田丁游戏开始方法
                    //TDGameStartFunc();
                    Talk(dd,1,null, () =>
                    {
                        dd.Hide();
                        
                        StartMainGame();
                    });
                    // ShowDialogue("点击喜欢的颜色，为蜂蜜罐涂色并且进行装饰吧", bdText);
                    // PlayVoice(1, () =>
                    // {
                    //     
                    // });
                    
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
          
           
            dd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); dd.SetActive(false); }));
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
        private void PlaySound(int targetIndex,Action callback=null)
        {
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
            WaitTimeAndExcuteNext(timer,callback);
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 5, false);
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
              
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();
              //加载游戏物体
              TDLoadSceneGameObject();

          }

          private void TDLoadSceneGameObject()
          {
              _gamePanel = curTrans.GetTransform("GamePanel");
              _colorPanel = _gamePanel.GetTransform("ColorPanel");
              _colorContent = _colorPanel.GetTransform("ColorItem/Viewport/Content");
              _devil = _gamePanel.GetGameObject("Devil");
              _deviHpPanel = _gamePanel.GetTransform("DevilHpPanel/Hp");
              _starEffect = _gamePanel.GetGameObject("Effect");
              _starEffect.Hide();
              _devilHpList=new List<GameObject>();
              for (int i = 0; i < _deviHpPanel.childCount; i++)
              {
                  _devilHpList.Add(_deviHpPanel.GetChild(i).gameObject);
              }
              _honey = _gamePanel.GetGameObject("Honey");
              for (int i = 0; i < _colorContent.childCount; i++)
              {
                  GameObject targetColor = _colorContent.GetChild(i).GetChild(0).gameObject;
                  PointerClickListener.Get(targetColor).onClick = ClickColorEvent;
              }

              _pen = _gamePanel.GetTargetComponent<ILDrager>("Pen");
              _penAni = _pen.transform.GetGameObject("PenAni");
              _pensTip = _pen.transform.GetTransform("Draw");
              _pen.SetDragCallback(null, DragGoing, DrageEnd);
              _pen.gameObject.Hide();
              _potAreaParent = _gamePanel.GetTransform("PotArea");
              _potImage = _gamePanel.GetTargetComponent<Image>("Pot");
              _gameLevelAreaDic=new Dictionary<int, Transform>();
              _bgSpriteDic= new Dictionary<int, Sprite>();
              Sprite[] allBGSprite = _potAreaParent.GetComponent<BellSprites>().sprites;
              
              for (int i = 0; i < allBGSprite.Length; i++)
              {
                 
                  _bgSpriteDic.Add(i + 1, allBGSprite[i]);
              }
              for (int i = 0; i < _potAreaParent.childCount; i++)
              {
                  Transform target = _potAreaParent.GetChild(i);
                  _gameLevelAreaDic.Add(Convert.ToInt32(target.name), target);
              }

              _potColorParent = _gamePanel.GetTransform("PotColor");
              _spriteDic=new Dictionary<int, Dictionary<string, Sprite>>();
              _imageDic=new Dictionary<int, Dictionary<string, Image>>();
              _allColorCoverStatuDic=new Dictionary<int, Dictionary<GameObject, bool>>();
              for (int i = 0; i < _potColorParent.childCount; i++)
              {
                  Transform target = _potColorParent.GetChild(i);
                  int gameLevel = Convert.ToInt32(target.name);
                  _spriteDic.Add(gameLevel, new Dictionary<string, Sprite>());
                  _allColorCoverStatuDic.Add(gameLevel, new Dictionary<GameObject, bool>());
                  Sprite[] sprites = target.GetComponent<BellSprites>().sprites;
                  for (int j = 0; j < sprites.Length; j++)
                  {
                      _spriteDic[gameLevel].Add(sprites[j].name, sprites[j]);
                  }

                  _imageDic.Add(gameLevel, new Dictionary<string, Image>());
                  for (int j = 0; j < target.childCount; j++)
                  {
                      Image targetImage = target.GetChild(j).GetComponent<Image>();
                      _imageDic[gameLevel].Add(targetImage.name, targetImage);
                      _allColorCoverStatuDic[gameLevel].Add(targetImage.gameObject, false);
                      targetImage.gameObject.Hide();
                  }
              }

              _beeParentItem = _gamePanel.GetTransform("Bee");
              _allBeePosDic=new Dictionary<string, Transform>();
              _allBeeList=new List<GameObject>();
              for (int i = 0; i < _beeParentItem.childCount; i++)
              {
                  Transform target = _beeParentItem.GetChild(i);
                  if (target.name.Equals("mf1")||target.name.Equals("mf2")||target.name.Equals("mf3"))
                  {
                      _allBeeList.Add(target.gameObject);
                      
                  }
                  
                  else if (target.name.Contains("Pos"))
                  {
                      _allBeePosDic.Add(target.name, target);
                  }
              }
              Debug.Log(_allBeeList.Count);

          }

          void TDLoadAllGameImage()
          {
             _imageDic.Clear();
              for (int i = 0; i < _potColorParent.childCount; i++)
              {
                  Transform target = _potColorParent.GetChild(i);
                  int gameLevel = Convert.ToInt32(target.name);
                  _imageDic.Add(gameLevel, new Dictionary<string, Image>());
                  for (int j = 0; j < target.childCount; j++)
                  {
                      Image targetImage = target.GetChild(j).GetComponent<Image>();
                      _imageDic[gameLevel].Add(targetImage.name, targetImage);
                      targetImage.gameObject.Hide();
                  }
              }
        }





          /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            
            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(false);
            XDD = curTrans.Find("mask/XDD").gameObject;
            XDD.SetActive(false);
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
            if(_clickSwitch)
                return;
            _clickSwitch = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                        _clickSwitch = false;
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); isPlaying = true; _clickSwitch = false; });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); _clickSwitch = false; XDD.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(XDD, SoundManager.SoundType.VOICE, 4)); });
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
