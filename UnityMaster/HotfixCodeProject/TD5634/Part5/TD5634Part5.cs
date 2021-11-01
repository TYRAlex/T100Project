using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public enum E_MainFlowers
    {
        lan=1,hong,zi
    }

    public enum E_OtherFlowersColor
    {
        bai=1,
        fen,
        huang
    }

    public enum E_OtherFlowerShape
    {
        A=1,B,C,D
    }
    

    public class TD5634Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        

        #region 田丁
        private GameObject tt;
        private GameObject DTT;
        
          #region Mask
        private Transform anyBtns;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
       
        #endregion
        
       
        
        #endregion

        #region 游戏物体

        private Transform _gamePanel;

        private Dictionary<int, Sprite> _signSpriteDic;

        private Transform _devilHpPanel;
        private List<Image> _devilList;
        private Dictionary<int, Sprite> _devilStatuImageDic;

        private Transform _flowersPanel;
        private List<Transform> _flowerDragList;
        private Dictionary<int, Transform> _flowerDragDic;

        private Dictionary<Transform,int> _flowerLeftDic;

        private Transform _flowerOriginalPanel;
        private Dictionary<int, Transform> _flowerOriginalPosDic;

        private Transform _flowerEndPosPanel;
        private Dictionary<int, Transform> _flowerEndPosDic;
        private int _flowerEndPosIndex;

        private int _currentGameLevel = 1;

        private int _currentDevilHp = 3;

        private string _currentGamelevelColorName;

        private Image _signImage;

        private Sprite[] _allSignContentImage;

        private Sprite[] _devilStatuSprites;

        private Transform _choosePanel;
        private Transform _choosePanelFlowerParent;

        private GameObject _choosePanelMask;
        
        private Transform _shineEffectPanel;
        private GameObject _flowerFinalEffect;
        private GameObject _shineObject;
        private Transform _basketObject;

        private GameObject _devilEffect;

        private int _leftCount=0;
        
        #endregion
        //创作指引完全结束
        bool isEnd = false;
        #endregion
        bool isPlaying = false;
       
        void Start(object o)
        {
            
            
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            Bg = curTrans.Find("Bg").gameObject;
            
       
            //田丁加载游戏物体方法
            TDLoadGameProperty();
            

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
           
            //GameStart();
        }
        
        #region 田丁加载
        /// <summary>
        /// 田丁加载所有物体
        /// </summary>
        void TDLoadGameProperty()
        {
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();
            //加载游戏场景物体
            LoadAllGameObject();
           
        }
        void LoadAllGameObject()
        {
            _gamePanel = curTrans.GetTransform("GamePanel");
            _choosePanel = _gamePanel.GetTransform("ChoosePanel");
            _choosePanelMask = _choosePanel.GetGameObject("mask");
            
            _choosePanelFlowerParent = _choosePanel.GetTransform("2");
            
            _shineEffectPanel = _gamePanel.GetTransform("ShineEffect");
            _shineObject = _shineEffectPanel.GetGameObject("Shine");
            _basketObject = _choosePanel.GetTransform("Basket");
            _flowerFinalEffect = _shineEffectPanel.GetGameObject("FlowerFinal");
            _devilEffect = _shineEffectPanel.GetGameObject("DevilEffect");
            // _signSpriteDic=new Dictionary<int, Sprite>();
            // BellSprites signSprites = _gamePanel.GetComponent<BellSprites>();
            // for (int i = 0; i < signSprites.sprites.Length; i++)
            // {
            //     Sprite targetSprite = signSprites.sprites[i];
            //     _signSpriteDic.Add(i + 1, targetSprite);
            // }
            _devilHpPanel = _gamePanel.GetTransform("DevilHp");
            _devilStatuImageDic=new Dictionary<int, Sprite>();
            BellSprites devilStatuSprite = _devilHpPanel.GetComponent<BellSprites>();
            for (int i = 0; i < devilStatuSprite.sprites.Length; i++)
            {
                Sprite statu = devilStatuSprite.sprites[i];
                _devilStatuImageDic.Add(i, statu);
            }
            _devilList=new List<Image>();
            for (int i = 0; i < _devilHpPanel.childCount; i++)
            {
                Image targetDevilImage = _devilHpPanel.GetChild(i).GetComponent<Image>();
                targetDevilImage.sprite = _devilStatuImageDic[0];
                _devilList.Add(targetDevilImage);
            }

            _flowersPanel = _gamePanel.GetTransform("Flowers");
            _flowerDragList=new List<Transform>();
            if (_flowerLeftDic == null)
            {
                //Debug.Log("空的");
                _flowerLeftDic = new Dictionary<Transform, int>();
            }
            else
            {
                //Debug.Log("非空的");
                _flowerLeftDic.Clear();
            }
            _flowerDragDic=new Dictionary<int, Transform>();
            for (int i = 0; i < _flowersPanel.childCount; i++)
            {
                GameObject flowerObject = _flowersPanel.GetChild(i).gameObject;
                if (flowerObject.GetComponent<ILDrager>() != null)
                    Component.DestroyImmediate(flowerObject.GetComponent<ILDrager>());

                ILDrager targetFlower = flowerObject.AddComponent<ILDrager>();
                //Debug.Log("-------------"+targetFlower);
                targetFlower.index = int.Parse(flowerObject.name);
                targetFlower.DragRect = flowerObject.transform.parent.GetComponent<RectTransform>();
                targetFlower.drops = new[] {_choosePanel.GetTransform("Basket/Image").GetComponent<ILDroper>()};
                
                targetFlower.SetDragCallback(DragStart, null, DragEnd);
                // _flowerDragList.Add(targetFlower.transform);
                // _flowerDragDic.Add(i+1,targetFlower.transform);
                // _flowerLeftDic.Add(targetFlower.transform, i + 1);
            }

            _flowerOriginalPanel = _gamePanel.GetTransform("FlowerOriginalPos");
            _flowerOriginalPosDic=new Dictionary<int, Transform>();
            for (int i = 0; i < _flowerOriginalPanel.childCount; i++)
            {
                Transform target = _flowerOriginalPanel.GetChild(i);
                _flowerOriginalPosDic.Add(i + 1, target);
            }

            _flowerEndPosPanel = _gamePanel.GetTransform("FlowerEndPos");
            _flowerEndPosDic=new Dictionary<int, Transform>();
            for (int i = 0; i < _flowerEndPosPanel.childCount; i++)
            {
                Transform target = _flowerEndPosPanel.GetChild(i);
                _flowerEndPosDic.Add(i + 1, target);
                //Debug.Log("i:" + (i + 1) + " target:" + target);
            }

            _signImage = _gamePanel.GetTargetComponent<Image>("Sign");
            _allSignContentImage = _gamePanel.GetComponent<BellSprites>().sprites;
            MonoScripts monoObj = null;
            if (curTrans.Find("Mono") == null)
            {
                monoObj = new GameObject("Mono").AddComponent<MonoScripts>();
                monoObj.transform.SetParent(curTrans);
            }
            else
            {
                monoObj = curTrans.Find("Mono").GetComponent<MonoScripts>();
            }

            monoObj.OnDisableCallBack = TDOnDisableCallback;



        }

        void TDOnDisableCallback()
        {
            if (_choosePanelFlowerParent.childCount > 0)
            {
                
                int count = _choosePanelFlowerParent.childCount;
                for (int i = 0; i < count; i++)
                {
            
                    _choosePanelFlowerParent.GetChild(0).SetParent(_flowersPanel);
                }
            }
            
        }

        /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            DTT = curTrans.Find("mask/DTT").gameObject;
            DTT.SetActive(false);
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
        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _flowerEndPosIndex = 1;
            _currentGameLevel = 1;
            _currentGamelevelColorName = ((E_MainFlowers) _currentGameLevel).ToString();
            _currentDevilHp = 3;
            //Debug.Log("LeftCount初始化");
            _leftCount = 4;
            _flowerEndPosIndex = 1;
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
            
            _basketObject.gameObject.Show();
            _basketObject.SetAsLastSibling();
            ChangeSignImage();
            for (int i = 0; i < _devilList.Count; i++)
            {
                _devilList[i].sprite = _devilStatuImageDic[0];
            }
            ShowTheDevilHp();
            ResetFlowerStatuAndPos();
            ResetResultEffect();
            SetAllTheFlowersVisible(true);
            for (int i = 0; i < _shineEffectPanel.childCount; i++)
            {
                _shineEffectPanel.GetChild(i).gameObject.Hide();
            }
        }
        void TDGameStart()
        {
            tt.Show();
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Talk(tt,0,null,()=>SoundManager.instance.ShowVoiceBtn(true));
        }

            #endregion

        #endregion

        #region 游戏主体方法

        void ChangeSignImage()
        {
            _signImage.sprite = _allSignContentImage[_currentGameLevel - 1];
        }

        void ShowTheDevilHp()
        {
            
            for (int i = 0; i < _devilList.Count; i++)
            {
                if (_currentDevilHp >= _devilList.Count)
                {
                    _devilList[i].gameObject.Show();
                }
                else if (_currentDevilHp<=0)
                {
                    _devilList[i].gameObject.Hide();
                }
                else
                {
                    if (i < _currentDevilHp)
                    {
                        _devilList[i].gameObject.Show();
                    }
                    else
                    {
                        _devilList[i].gameObject.Hide();
                    }
                }
            }
        }

        void ResetFlowerStatuAndPos()
        {
            ResetFlowerParent();
           
            
            List<string> alReadyExistFlower=new List<string>();
            string exsitColor = "";
            for (int i = 1; i < 7; i++)
            {
                int randomNumber = Random.Range(0, _flowerDragList.Count);
                Transform target = _flowerDragList[randomNumber];
                //Debug.Log(target.name);
                if (i <= 4)
                {
                    
                    _flowerLeftDic.Add(target, randomNumber+1);
                    target.tag = "Respawn";
                    string targetCorlor = _currentGamelevelColorName + (E_OtherFlowerShape) i;
                    //Debug.Log("当前关卡的花的颜色：" + targetCorlor);
                   // Debug.Log("当前字典的长度为："+_flowerLeftDic.Count);
                    PlaySpineAni(target.GetChild(0).gameObject, targetCorlor,true);
                }
                else
                {
                    // int index1 = Random.Range(1, 4);
                    // int index2 = Random.Range(1, 4);
                    // string targetCorlor = ((E_OtherFlowersColor) index1).ToString() +
                    //                       ((E_otherFlowerShape) index2).ToString();
                    //string targetCorlor= GetNewTargetColor(ref exsitColor);
                    string targetCorlor = TransferToEnum(GetNewTargetColor(alReadyExistFlower));
                    //Debug.Log("当前关卡的其他的花的颜色：" + targetCorlor);
                    PlaySpineAni(target.GetChild(0).gameObject, targetCorlor,true);
                }
                _flowerDragList.Remove(target);
            }
        }

        string GetNewTargetColor(List<string> alreadyExist)
        {
            string target = null;
            int index1 = Random.Range(1, 4);
            int index2 = Random.Range(1, 4);
            target = index1.ToString() + index2;
            if (alreadyExist.Contains(target.Substring(0, 1)))
            {
                //Debug.Log("存在，现在重新随机"+target);
                target= GetNewTargetColor(alreadyExist);
            }
            else
            {
                //Debug.Log("不存在，现在添加"+target);
                alreadyExist.Add(target.Substring(0,1));
            }
            //("随机的第一个数"+target);
            return target;
        }

        string TransferToEnum(string colorNumber)
        {
            E_OtherFlowersColor front =
                (E_OtherFlowersColor) System.Enum.Parse(typeof(E_OtherFlowersColor), colorNumber.Substring(0, 1));
            E_OtherFlowerShape back =
                (E_OtherFlowerShape) Enum.Parse(typeof(E_OtherFlowerShape), colorNumber.Substring(1, 1));
            return front.ToString() + back;
        }

        void DragStart(Vector3 pos,int dragType,int index)
        {
            Transform flower = _flowerDragDic[index];
            flower.SetParent(_choosePanelFlowerParent);
        }

        void DragEnd(Vector3 pos ,int dragType,int index,bool isMatch)
        {
            Transform flower = _flowerDragDic[index];
           
            SoundManager.instance.Stop("voice");
            if (isMatch)
            {
                PutFLowerIntheBasket(flower,index);
            }
            else
            {
                BtnPlaySoundFail();
                flower.position = _flowerOriginalPosDic[index].position;
                flower.SetParent(_flowersPanel);
            }
        }

        string GetAllFlowerLeftItem()
        {
            string target = "";
            foreach (Transform flower in _flowerLeftDic.Keys)
            {
                target += flower.name;
            }

            return target;
        }
        
        void PutFLowerIntheBasket(Transform flower,int index)
        {
            //Debug.Log(_flowerLeftDic.GetType().+_flowerLeftDic.Count+"    :"+GetAllFlowerLeftItem() + " -----" + flower.name);
            //if (_flowerLeftDic.ContainsKey(flower))
            if(flower.CompareTag("Respawn"))
            {
                BtnPlaySoundSuccess();
                flower.GetComponent<ILDrager>().enabled = false;
                
                flower.position = _flowerEndPosDic[_flowerEndPosIndex].position;
                _flowerEndPosIndex++;
               
                //_leftCount--;
                _flowerLeftDic.Remove(flower);
                //if (_flowerLeftDic.Count <= 0)
                if(_flowerEndPosIndex>=5)
                {
                    //Debug.Log("判断还剩的鲜花为0");
                   HideTheDevil(JudgeIfFinishedAndGoToNext);
                }
            }
            else
            {
                //Debug.Log("回去");
                BtnPlaySoundFail();
                flower.position = _flowerOriginalPosDic[index].position;
                flower.SetParent(_flowersPanel);
            }

            
        }

        void SetAllTheFlowersVisible(bool isShow)
        {
            for (int i = 1; i < 7; i++)
            {
                GameObject target=_flowerDragDic[i].gameObject;
                if (target.activeSelf != isShow)
                    target.SetActive(isShow);
            }
        }

        void HideTheDevil(Action callback)
        {
           
            _choosePanelMask.Show();
            
            _choosePanelFlowerParent.transform.SetAsLastSibling();
           _basketObject.SetAsLastSibling();
           
            _choosePanel.DOLocalMoveY(TransferHeightValue(350f), 1f).OnComplete(() =>
            {
                _shineObject.Show();
                PlaySound(0);
                PlaySpineAni(_shineObject, "guang2",false, () =>
                {
                    PlaySpineAni(_shineObject, "guang3", true);
                    _flowerFinalEffect.Show();
                    _basketObject.gameObject.Hide();
                    SetAllTheFlowersVisible(false);
                    
                    PlaySpineAni(_flowerFinalEffect,"hua"+(E_OtherFlowerShape)_currentGameLevel,false, () =>
                    {
                        GameObject attactObject = _shineEffectPanel.GetGameObject("Attack" + _currentGameLevel);
                        attactObject.GetComponent<SkeletonGraphic>().Initialize(true);
                        PlaySpineAni(attactObject,"gongjixiaoguo",false,
                            () =>
                            {
                                Image devilImage = _devilList[3 - _currentGameLevel];
                                devilImage.sprite = _devilStatuImageDic[1];
                                _devilEffect.Show();
                                _devilEffect.transform.position = devilImage.transform.position;
                                PlaySound(1);
                                PlaySpineAni(_devilEffect,"xiaoguo",false, () =>
                                {
                                    
                                    _shineObject.Hide();
                                    _currentDevilHp--; 
                                    ShowTheDevilHp();
                                    WaitTimeAndExcuteNext(1f, () => callback?.Invoke());

                                });
                               
                            });
                        
                    });
                    
                });
               
            });
        }

        float TransferHeightValue(float originValue)
        {
            return originValue * Screen.height / 1080f;
        }


        void JudgeIfFinishedAndGoToNext()
        {
            if (_currentGameLevel < 3)
            {
               //Debug.Log("进入下一关");
                _currentGameLevel++;
                ResetGameProperty();
            }
            else
            {
                WinTheGame();
            }
        }

        void ResetGameProperty()
        {
            
            _flowerEndPosIndex = 1;
            _currentGamelevelColorName = ((E_MainFlowers) _currentGameLevel).ToString();
            _basketObject.gameObject.Show();
            _basketObject.transform.SetAsLastSibling();
            SetAllTheFlowersVisible(true);
            ChangeSignImage();
            ResetFlowerStatuAndPos();
            ResetResultEffect();
        }

        void ResetResultEffect()
        {
            _shineObject.Hide();
            _choosePanel.localPosition=Vector3.zero;
            _choosePanelMask.Hide();
            _flowerFinalEffect.Hide();
        }

        void ResetFlowerParent()
        {
           
            if (_choosePanelFlowerParent.childCount > 0)
            {
                
                int count = _choosePanelFlowerParent.childCount;
                for (int i = 0; i < count; i++)
                {

                    _choosePanelFlowerParent.GetChild(0).SetParent(_flowersPanel);
                }
            }
            _flowerDragDic.Clear();
            for (int i = 0; i < _flowersPanel.childCount; i++)
            {
                Transform target = _flowersPanel.GetChild(i);
                _flowerDragDic.Add(int.Parse(target.name), target);
                //Debug.Log("重置："+target.name+"-----"+_flowerDragDic[int.Parse(target.name)]);
            }
            _flowerDragList.Clear();
            for (int i = 1; i < 7; i++)
            {
                Transform targetFlower = _flowerDragDic[i];
                targetFlower.tag = "Untagged";
                targetFlower.GetComponent<ILDrager>().enabled = true;
                _flowerDragList.Add(targetFlower);
                targetFlower.position = _flowerOriginalPosDic[i].position;
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
                    TDGameStartFunc();
                    
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
           
            //tt.SetActive(true);
            Talk(tt,1,null,() => { mask.SetActive(false); tt.SetActive(false); });
            //mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.COMMONVOICE, 0, null, ));
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
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }
        
        
       
        #region 监听相关

       

        
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
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                        isPlaying = false;
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); isPlaying = false;});
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        switchBGM();
                        anyBtns.gameObject.SetActive(false);
                        DTT.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(DTT, SoundManager.SoundType.VOICE, 2, null,
                            () => isPlaying = false));
                    });
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

        

        // string GetNewTargetColor(ref string existColor)
        // {
        //     string target = null;
        //     int index1 = Random.Range(1, 4);
        //     int index2 = Random.Range(1, 4);
        //     target = index1.ToString() + index2;
        //     Debug.Log("target："+target+" 后："+existColor);
        //     if (existColor == "")
        //     {
        //         existColor = target;
        //         Debug.Log("1:"+existColor);
        //         return existColor;
        //     }
        //     else if (existColor != "" && existColor != target)
        //     {
        //         Debug.Log("2:" + existColor);
        //         existColor = target;
        //         return existColor;
        //     }
        //     else
        //     {
        //         Debug.Log("3"+existColor);
        //         GetNewTargetColor(ref existColor);
        //     }
        //
        //     return existColor;
        // }

    }
}
