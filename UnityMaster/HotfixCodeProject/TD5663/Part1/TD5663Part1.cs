using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,
       Alligator
	}
	
    public class TD5663Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;	
		private GameObject _nextSpine;	
        private GameObject _dTT;		
        private GameObject _sTT;	
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private RawImage _currentBG;
        private BellSprites _allBGArray;

        private bool _isPlaying;

        private bool _isToothGamePlaying;
        private int _toothGameLevel = 1;
        private int _toothGameRightItemNumber = 0;

        // private GameObject _eyRight;
        // private GameObject _eyLeft;
        private GameObject _alligatorSpine;
        private Transform _gamePanel;
        private Transform _equipmentPanel;
        private GameObject _equitmentSpineGameObject;
        private Transform _equipmentClickPanel;
        private List<string> _equipmentClickFinishedList;
        private Transform _toothGamePanel;
        private GameObject _toothGameMask;
        private GameObject _toothGameBGGameObject;
        private Transform _toothGameItemsPosPanel;
        private List<Image> _toothGameItemsList;
        private Transform _toothGameAllItemsPanel;
        private Slider _toothGameTimeSlider;
        private Image _toothGameEffect;
        private GameObject _toothGameLoseSpineGameObject;

        private List<GameObject> _allSpineGameObjectList;
        //private GameObject _diaLoguePanel;

        private Dictionary<string, Sprite[]> _toothGameItemsDic;
        private GameObject _toothWarrior;
        private GameObject _littleDevil;

        private GameObject _tweezerGameObject;
        private GameObject _oralIrrigatorGameObject;

        private bool _isPlayingTheVoice = false;
        private bool _isClearing = false;

        private string _candy = "Candy";
        private string _flower = "Flower";
        private string _vegetable = "Vegetables";
        private string _virus = "Virus";
        /// <summary>
        /// 拔牙钳
        /// </summary>
        private string _dentalForceps = "DentalForceps";
        /// <summary>
        /// 超声波
        /// </summary>
        private string _ultrasonic = "Ultrasonic";
        /// <summary>
        /// 冲牙器
        /// </summary>
        private string _oralIrrigator = "OralIrrigator";
        /// <summary>
        /// 镊子
        /// </summary>
        private string _tweezers = "Tweezers";
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");			
			_nextSpine = curTrans.GetGameObject("nextSpine");						
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");
            
			_dTT = curTrans.GetGameObject("dTT");           			
			
			_sTT = curTrans.GetGameObject("sTT");
            LoadGameProperty(curTrans);
                                    
            GameInit();
            GameStart();
        }
        
        void OnDisable()
        {
            Debug.LogError("执行Disable方法");
            if (_allSpineGameObjectList.Count > 0)
            {
                for (int i = 0; i < _allSpineGameObjectList.Count; i++)
                {
                    GameObject target = _allSpineGameObjectList[i];
                    SpineManager.instance.ClearTrack(target);
                    if (target.GetComponent<SkeletonGraphic>() != null)
                    {
                        target.GetComponent<SkeletonGraphic>().Initialize(true);
                    }
                }
            }
        }

        void LoadGameProperty(Transform curTrans)
        {
            _gamePanel = curTrans.GetTransform("GamePanel");
            Transform bgPanel = curTrans.GetTransform("BG");
            _allBGArray = bgPanel.GetComponent<BellSprites>();
            _currentBG = bgPanel.GetTargetComponent<RawImage>("RawImage");
            bgPanel = null;
            _currentBG.texture = _allBGArray.texture[0];
            // = curTrans.GetGameObject("BG/RightEy");
            //_eyLeft = curTrans.GetGameObject("BG/LeftEy");
            _equipmentPanel = _gamePanel.GetTransform("EquipMent");
            _equitmentSpineGameObject = _equipmentPanel.GetGameObject("Spine");
            _equipmentClickPanel = _equipmentPanel.GetTransform("Click");
            _alligatorSpine = _equipmentPanel.GetGameObject("Alligator");
            //_diaLoguePanel = _equipmentPanel.GetGameObject("DiaLoguePanel");
            _equipmentClickFinishedList=new List<string>();
            for (int i = 0; i < _equipmentClickPanel.childCount; i++)
            {
                GameObject clickItem = _equipmentClickPanel.GetChild(i).gameObject;
                PointerClickListener.Get(clickItem).clickDown = EquitmentClickEvent;
            }
            _equipmentClickPanel.gameObject.Hide();
            _toothGamePanel = _gamePanel.GetTransform("ToothGame");
            _toothGameBGGameObject = _toothGamePanel.GetGameObject("ToothBG");
            _toothGameItemsPosPanel = _toothGamePanel.GetTransform("ItemPos");
            _toothGameItemsList=new List<Image>();
            for (int i = 0; i < _toothGameItemsPosPanel.childCount; i++)
            {
                Transform posTrans = _toothGameItemsPosPanel.GetChild(i);
                Image target = posTrans.GetComponent<Image>();
                PointerClickListener.Get(posTrans.GetGameObject("click")).clickDown = ToothGameClickEvent; 
                _toothGameItemsList.Add(target);
                
            }

            _toothGameAllItemsPanel = _toothGamePanel.GetTransform("AllItem");
            _toothGameItemsDic=new Dictionary<string, Sprite[]>();
            for (int i = 0; i < _toothGameAllItemsPanel.childCount; i++)
            {
                Transform spriteItem = _toothGameAllItemsPanel.GetChild(i);
                Sprite[] target = spriteItem.GetComponent<BellSprites>().sprites;
                _toothGameItemsDic.Add(spriteItem.name, target);
            }
            _allSpineGameObjectList=new List<GameObject>();

            _toothGameMask = _toothGamePanel.GetGameObject("mask");
            _toothWarrior = _toothGamePanel.GetGameObject("ToothWarrior");
            _littleDevil = _toothGamePanel.GetGameObject("LittleDevil");
            _tweezerGameObject = _toothGamePanel.GetGameObject("Tweezer");
            _oralIrrigatorGameObject = _toothGamePanel.GetGameObject("OralIrrigator");
            _toothGameTimeSlider = _toothGamePanel.GetTargetComponent<Slider>("Time/Slider");
            _toothGameEffect = _toothGamePanel.GetTargetComponent<Image>("ItemEffect");
            _toothGameLoseSpineGameObject = _toothGamePanel.GetGameObject("LoseGame");
            _toothGameTimeSlider.value = 1;
            _candy = "Candy";
            _flower = "Flower";
            _vegetable = "Vegetables";
            _virus = "Virus";
        }

        private void InitToothGameLevelProperty(int gameLevel)
        {
            ChangeBG(1);
            _toothGamePanel.gameObject.Show();
            _isClearing = false;
            _isToothGamePlaying = true;
            _toothGameLevel = gameLevel;
            _toothGameRightItemNumber = 0;
            _toothGameMask.Hide();
            _littleDevil.Hide();
            _replaySpine.Hide(); 
            if (_tweezerGameObject.activeSelf)
            {
                PlaySpine(_tweezerGameObject.transform.GetGameObject("Spine"), "nie");
                _tweezerGameObject.Hide();
            }

            
            _oralIrrigatorGameObject.Hide();
            _toothWarrior.Hide();
            _mask.transform.SetParent(_curGo.transform);
            _mask.transform.SetSiblingIndex(1);
            _mask.Hide();
            _toothGameBGGameObject.Show();
            PlaySpine(_toothGameBGGameObject, "zui", null, true);
            InializedItemsOnTheTooth();
            CountTimeAndExcuteNext();
        }

        private void InializedItemsOnTheTooth()
        {
            
            if (_toothGameLevel == 1)
            {
                RandomAndSetRightAndWrongItemImage(_toothGameItemsDic[_candy],_toothGameItemsDic[_vegetable]);
                // Sprite[] candySprite = _toothGameItemsDic[_candy];
                // Sprite[] vegetableSprite = _toothGameItemsDic[_vegetable];
                // int rightItemNumber = 0;
                // for (int i = 0; i < _toothGameItemsList.Count; i++)
                // {
                //     Image target = _toothGameItemsList[i];
                //     int randomType = Random.Range(0, 2);
                //     if (randomType == 1&&rightItemNumber<=3)
                //     {
                //         int rightSpriteIndex = Random.Range(0, candySprite.Length);
                //         target.sprite = candySprite[rightSpriteIndex];
                //     }
                //     else
                //     {
                //         int wrongSpriteIndex = Random.Range(0, vegetableSprite.Length);
                //         target.sprite = vegetableSprite[wrongSpriteIndex];
                //     }
                // }
            }
            else if (_toothGameLevel == 2)
            {
                RandomAndSetRightAndWrongItemImage( _toothGameItemsDic[_flower],_toothGameItemsDic[_virus]);
            }
        }

        /// <summary>
        /// 所有物体随机生成
        /// </summary>
        /// <param name="rightSprite"></param>
        /// <param name="wrongSprite"></param>
        void RandomAndSetRightAndWrongItemImage(Sprite[] rightSprite,Sprite[] wrongSprite)
        {
            // rightSprite = _toothGameItemsDic[_candy];
            // wrongSprite = _toothGameItemsDic[_vegetable];
            Dictionary<int,Image> targetList=new Dictionary<int, Image>();
            for (int i = 0; i < _toothGameItemsList.Count; i++)
            {
                Image target = _toothGameItemsList[i];
                target.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                target.gameObject.Show();
                target.transform.GetGameObject("click").Show();
                target.transform.GetGameObject("Spine").Hide();
                target.color = Color.white;
                int randomType = Random.Range(0, 2);
                if (randomType == 1 && _toothGameRightItemNumber <= 3)
                {
                    int wrongSpriteIndex = Random.Range(0, wrongSprite.Length);
                    if (_toothGameLevel == 1)
                    {
                        
                        
                        target.sprite = wrongSprite[wrongSpriteIndex];
                        target.name = wrongSprite[wrongSpriteIndex].name;
                    }
                    else if(_toothGameLevel==2)
                    {
                        GameObject targetSpine = target.transform.GetGameObject("Spine");
                        //targetSpine.GetComponent<SkeletonGraphic>().Initialize(true);
                        targetSpine.Show();
                        
                        target.color = Color.clear;
                        string spineAni=String.Empty;
                        target.name = wrongSprite[wrongSpriteIndex].name;
                        spineAni = wrongSpriteIndex == 0 ? "xj" : "xj" + (wrongSpriteIndex+1);
                        if (spineAni.Contains("4"))
                        {
                            targetSpine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90f);
                        }
                        else
                        {
                            targetSpine.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }

                        PlaySpine(targetSpine, spineAni, null, true);
                    }

                    
                    _toothGameRightItemNumber++;
                    targetList.Add(i,target);
                }
                else
                {
                    int rightSpriteIndex = Random.Range(0, rightSprite.Length);
                    target.sprite = rightSprite[rightSpriteIndex];
                    target.name = rightSprite[rightSpriteIndex].name;
                    if (target.name.Equals("candy3") || target.name.Equals("candy1"))
                    {
                        target.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                    }
                    else
                    {
                        target.transform.localScale=Vector3.one;
                    }



                }
            }

            // foreach (var target in targetList)
            // {
            //
            //     Debug.LogError("换成需要点击的名称为" + target.Key + " :" + target.Value);
            //
            // }
            // Debug.LogError("需要点击的数量为："+_toothGameRightItemNumber);
        }

        private List<Image> listRandom(List<Image> myList)
        {

            System.Random ran=new System.Random();
            
            int index = 0;
            Image temp;
            for (int i = 0; i < myList.Count; i++)
            {
                index = ran.Next(0, myList.Count - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }

            return myList;
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="go"></param>
        private void ToothGameClickEvent(GameObject go)
        {
            Debug.LogError("开始点击");
            if (_isToothGamePlaying&&go.name.Equals("click"))
            {
                Debug.LogError("go的名字："+go.name+"这个时候其他的物体是否在清理："+_isClearing+"_isToothGamePlaying:"+_isToothGamePlaying);
                if(_isClearing)
                    return;
                _isClearing = true;
                ExcuteClearAnimation(go,()=>JudgeIfClickTheRightTargetAndExcuteNext(go));
            }
        }

        void ExcuteClearAnimation(GameObject go,Action callback)
        {
            
            GameObject target = _toothGameLevel == 1 ? _tweezerGameObject : _oralIrrigatorGameObject;
            target.Show();
            target.transform.position = go.transform.position;
            GameObject spineGameObject = target.transform.GetGameObject("Spine");
            spineGameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            // PlaySpine(spineGameObject, _toothGameLevel == 1 ? "nie2" : "qingjie2",
            //     () => PlaySpine(spineGameObject, _toothGameLevel == 1 ? "nie" : "qingjie", () =>
            //     {
            //         target.Hide();
            //         callback?.Invoke();
            //     }));
            string nieziAniName="nie";
            string clickName = go.transform.parent.name;
            switch (clickName)
            {
                case "candy1":
                    nieziAniName = nieziAniName + "3";
                    break;
                case "candy2":
                    nieziAniName = nieziAniName + "4";
                    break;
                case "candy3":
                    nieziAniName = nieziAniName + "5";
                    break;
                case "vegetable1":
                    nieziAniName = nieziAniName + "6";
                    break;
                case "vegetable2":
                    nieziAniName = nieziAniName + "10";
                    break;
                case "vegetable3":
                    nieziAniName = nieziAniName + "7";
                    break;
                case "vegetable4":
                    nieziAniName = nieziAniName + "8";
                    break;
                case "vegetable5":
                    nieziAniName = nieziAniName + "9";
                    break;
            }

            PlaySound(_toothGameLevel == 1 ? 3 : 4);
            float timer = PlaySpine(spineGameObject, _toothGameLevel == 1 ? "nie2" : "qingjie2", null,
                _toothGameLevel == 1 ? false : true);
            if (_toothGameLevel == 2&&go.transform.parent.name.Contains("virus"))
            {
                
                _toothGameEffect.gameObject.Show();
                _toothGameEffect.transform.localPosition = new Vector3(-2000f, 0, 0);
                GameObject targetSpine= _toothGameEffect.transform.GetGameObject("Spine");
                targetSpine.GetComponent<SkeletonGraphic>().Initialize(true);
                targetSpine.Show();
                _toothGameEffect.color=Color.clear;
                string index = go.transform.parent.name.Split('s')[1];
                string aniName = index.Equals("1") ? "xj" : "xj" + index;
                if (index.Equals("4"))
                    targetSpine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90f);
                else
                {
                    targetSpine.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
                PlaySpine(targetSpine,  aniName,null,true);
                
            }

            if (_toothGameLevel == 2)
            {
                Delay(1f, () =>
                {
                    _toothGameEffect.transform.position = go.transform.parent.position;
                    _toothGameEffect.transform.rotation = go.transform.parent.rotation;
                    _toothGameEffect.transform.localScale = go.transform.parent.localScale;
                    go.transform.parent.GetGameObject().Hide();
                });
            }

            Delay(_toothGameLevel==1? timer:2f, () =>
            {
                //go.transform.parent.gameObject.Hide();
                
                float timer2 = PlaySpine(spineGameObject, _toothGameLevel == 1 ? nieziAniName : "qingjie");
                Delay(timer2, () =>
                {
                    //target.Hide();
                    if (_toothGameLevel == 1)
                    {
                        go.transform.parent.gameObject.Hide();
                        // target.transform.DOLocalMove(newVector3, 0.5f).OnComplete(() =>
                        // {
                        //     target.Hide();
                        //     callback?.Invoke();
                        // });
                        callback?.Invoke();
                    }
                    else if (_toothGameLevel == 2)
                    {
                        callback?.Invoke();
                    }


                    //callback?.Invoke();
                });
            });
        }

        /// <summary>
        /// 判断是否结束游戏，如果条件不成立就结束游戏，如果是的话就进入下一个阶段
        /// </summary>
        /// <param name="go"></param>
        void JudgeIfClickTheRightTargetAndExcuteNext(GameObject go)
        {
            bool isRight = JudgeIfClickTheRight(go);
            Transform toolTransform =
                _toothGameLevel == 1 ? _tweezerGameObject.transform : _oralIrrigatorGameObject.transform;
            if (isRight)
            {
                PlaySound(2);
                ItemFallOut(toolTransform,go.transform.parent, () =>
                {
                    //Debug.LogError("点击之前还剩：" + _toothGameRightItemNumber);
                    _isClearing = false;
                    _toothGameRightItemNumber--;
                    if (_toothGameRightItemNumber <= 0)
                    {
                        if (_isToothGamePlaying)
                            ClearRightItemAndExcuteNext();
                    }
                });
                
            }
            else
            {
                if (_isToothGamePlaying)
                {
                    LoseGame();
                }
            }

            
        }

        void CountTimeAndExcuteNext()
        {
            float timer = 20f;
            _mono.StartCoroutine(CountTimeAndExcuteNextIE(timer));
        }

        IEnumerator CountTimeAndExcuteNextIE(float timer)
        {
            float countTimer = timer;
            float showValue = 0;
            while (_isToothGamePlaying)
            {
                countTimer -= Time.fixedDeltaTime;
                showValue = countTimer / timer;
                _toothGameTimeSlider.value = showValue;
                if (countTimer <= 0)
                {
                    if (_isToothGamePlaying&& _isClearing==false)
                    {
                        LoseGame();
                        break;
                    }
                    
                }
                if(!_isToothGamePlaying)
                    break;
                yield return new WaitForFixedUpdate();
            }
        }

        void ClearRightItemAndExcuteNext()
        {
            _isToothGamePlaying = false;
            foreach (Image item in _toothGameItemsList)
            {
                item.gameObject.Hide();
            }
            ChangeBG(2);
            _oralIrrigatorGameObject.Hide();
            PlaySound(6);
            PlaySpine(_toothGameBGGameObject, "xiao2", () =>
            {
                _toothGameMask.Show();
                _toothWarrior.Show();
                _littleDevil.Show();
                _littleDevil.GetComponent<SkeletonGraphic>().Initialize(true);
                PlaySpine(_littleDevil, "xem1");
                float timer = PlaySpine(_toothWarrior, "ychi",
                    () =>
                    {
                        
                        PlaySpine(_toothWarrior, "ychi2", () => { PlaySpine(_toothWarrior, "ychi"); });
                    });
                Delay(1.5f*timer, () =>
                {
                    PlaySound(5);
                    PlaySpine(_littleDevil, "xem-y",
                        () =>
                        {
                            
                            PlaySpine(_littleDevil, "xem-y2", () =>
                            {
                                _littleDevil.Hide();
                                _toothWarrior.Hide();
                                //_mask.transform.SetSiblingIndex(2);
                                if (_toothGameLevel == 1)
                                {
                                    Delay(PlayVoice(7), () => { InitToothGameLevelProperty(2); });
                                }
                                else if (_toothGameLevel == 2)
                                {
                                    WinTheGame();
                                }
                            });
                        });        
                });
                
                //todo...
            });

        }

        

        void WinTheGame()
        {
            GameSuccess();
        }

        void LoseGame()
        {
            //todo...
            //Debug.Log("输了重来一次");
            _isToothGamePlaying = false;
            _toothGameLoseSpineGameObject.Show();
            _toothGameMask.Show();
            _mask.Hide();
            PlaySound(1);
            PlaySpine(_toothGameLoseSpineGameObject, "paizi", () =>
            {
                Delay(1f, () =>
                {
                    //Debug.LogError("准备重新开始游戏");
                    _toothGameLoseSpineGameObject.Hide();
                    _toothGameMask.Hide();
                    InitToothGameLevelProperty(_toothGameLevel);
                });
            }, false);
            
        }

        void ItemFallOut(Transform toolTransform,Transform go,Action callback)
        {
            Debug.Log("开始掉落");
            if(!_isToothGamePlaying)
                return;
            //go.gameObject.Hide();
            if (_toothGameLevel == 2)
            {
                
                _toothGameEffect.gameObject.Show();
                _toothGameEffect.transform.position = toolTransform.position;
                _toothGameEffect.transform.rotation = go.rotation;
            }

            
            // if (_toothGameLevel == 1)
            // {
            //     Debug.Log("开始掉落1111");
            //     _toothGameEffect.transform.GetGameObject("Spine").Hide();
            //     _toothGameEffect.color=Color.white;
            //     _toothGameEffect.sprite = go.GetComponent<Image>().sprite;
            //     
            // }
            // if (_toothGameLevel == 2)
            // {
            //     GameObject targetSpine= _toothGameEffect.transform.GetGameObject("Spine");
            //     targetSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            //     targetSpine.Show();
            //     _toothGameEffect.color=Color.clear;
            //     string index = go.name.Split('s')[1];
            //     string aniName = index.Equals("1") ? "xj" : "xj" + index;
            //     if (index.Equals("4"))
            //         targetSpine.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 90f);
            //     else
            //     {
            //         targetSpine.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            //     }
            //     PlaySpine(targetSpine,  aniName);
            // }

            if (_toothGameLevel == 1)
            {
                _mono.StartCoroutine(ItemFallOutIE(toolTransform.gameObject, callback));
            }
            else if (_toothGameLevel == 2)
            {
                
                _toothGameEffect.transform.DOShakeRotation(1f,new Vector3(0f, 0f, 10f)).OnComplete(() =>
                {
                    _mono.StartCoroutine(ItemFallOutIE(_toothGameEffect.gameObject, callback));
                    // if(_toothGameLevel==2)
                    //     toolTransform.GetGameObject().Hide();
                    //_mono.StartCoroutine("ItemFallOutIE",new object[]{ _toothGameEffect.gameObject, callback });
                });
            }

            
        }

        IEnumerator ItemFallOutIE(GameObject target,Action callback)
        {
            if (!_isToothGamePlaying)
                yield break;
            float g = 980f;
            float v0 = g * Time.fixedDeltaTime;
            Vector3 direction = _toothGameLevel == 1 ? new Vector3(-1f,-1f,0) : Vector3.down;
            while (true)
            {
                target.transform.Translate(direction * v0 * Time.fixedDeltaTime,Space.World);
                yield return new WaitForFixedUpdate();
                v0 = v0 + g * Time.fixedDeltaTime;
                if (target.GetComponent<RectTransform>().anchoredPosition.y < -666f)
                {
                    callback?.Invoke();
                    target.Hide();
                    if(_toothGameLevel==2)
                        _oralIrrigatorGameObject.Hide();
                    else if (_toothGameLevel == 1)
                    {
                        GameObject targetSpine = target.transform.GetGameObject("Spine");
                        PlaySpine(targetSpine, "nie");
                    }

                    break;
                }

                if (!_isToothGamePlaying)
                {
                    target.Hide();
                    break;
                }
            }
        }

        bool JudgeIfClickTheRight(GameObject go)
        {
            Transform target = go.transform.parent;
            bool isRight = false;
            if (_toothGameLevel == 1&&target.name.Contains("vegetable"))
            {
                isRight = true;

            }
            else if (_toothGameLevel == 2&&target.name.Contains("virus"))
            {
                isRight = true;
            }
            else if(_toothGameLevel>2)
            {
                Debug.LogError("错误的关卡等级，请检查！GameLevel:" + _toothGameLevel);
            }

            return isRight;
        }

        private void EquitmentClickEvent(GameObject go)
        {
            if(_isPlayingTheVoice)
                return;
            _isPlayingTheVoice = true;
            string spineName=String.Empty;
            int voiceIndex = 2;
            switch (go.name)
            {
                case "DentalForceps":
                    //拔牙钳
                    spineName = "gj1";
                    voiceIndex = 4;
                    break;
                case "Ultrasonic":
                    //超声波
                    spineName = "gj2";
                    voiceIndex = 2;
                    break;
                case "OralIrrigator":
                    //冲牙器
                    spineName = "gj3";
                    voiceIndex = 5;
                    break;
                case "Tweezers":
                    //镊子
                    spineName = "gj4";
                    voiceIndex = 3;
                    break;
            }
            PlaySpine(_equitmentSpineGameObject, spineName);
            PlaySound(0);
            HideVoiceBtn();
            Delay(PlayVoice(voiceIndex,false), () =>
            {
                PlaySpine(go, "gj0");
                _isPlayingTheVoice = false;
                if (_equipmentClickFinishedList.Count >= 4)
                {
                    ShowVoiceBtn();
                }
            });
            if (spineName != String.Empty && !_equipmentClickFinishedList.Contains(spineName))
            {
                _equipmentClickFinishedList.Add(spineName);
            }

            
        }

        void ChangeBG(int stageIndex)
        {
            _currentBG.texture = _allBGArray.texture[stageIndex];
        }

        void InitData()
        {
            _isPlayingTheVoice = false;
            _isPlaying = true;
            _isToothGamePlaying = false;
            _toothGameLevel = 1;
            _toothGameRightItemNumber = 0;
            _equipmentClickFinishedList.Clear();
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
			           
        }

        void GameInit()
        {
            InitData();
            _isClearing = false;
            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); 
			StopAllCoroutines();
            _mask.Show();
            _mask.transform.SetParent(_curGo.transform);
            _mask.transform.SetSiblingIndex(1);
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); 
			_nextSpine.Hide();
			_dTT.Hide(); 
			_sTT.Hide(); 	
            //_eyRight.Show();
            _alligatorSpine.Hide();
            _toothGameMask.Hide();
            _toothGameEffect.gameObject.Hide();
            //PlaySpine(_eyRight, "ey", null, true);
            //_eyLeft.Hide();
			_toothGamePanel.gameObject.Hide();
            _toothGameLoseSpineGameObject.Hide();
            //_diaLoguePanel.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
			RemoveEvent(_nextSpine);			         
        }



        void GameStart()
        {
            
            _mask.Show(); _startSpine.Show();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);
                        _startSpine.Hide();
						
						//ToDo...
						StartGame();
                        
                    });
                });
            });
         //InitToothGameLevelProperty(2);
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:  
                    //todo.....田田只出现语音：我们认识了牙科器材，现在使用镊子把嘴巴里的蔬菜清理干净把
                     _mask.Show();
                     _mask.transform.SetSiblingIndex(2);
                    _mask.Hide();
                    _toothGameMask.Show();
                    PlaySpine(_tweezerGameObject.transform.GetGameObject("Spine"), "nie");
                    _tweezerGameObject.Hide();
                    _oralIrrigatorGameObject.Hide();
                    _equipmentPanel.gameObject.Hide();
                    _equipmentClickPanel.gameObject.Hide();
                    _toothGamePanel.gameObject.Show();
                    _toothGameItemsPosPanel.gameObject.Hide();
                    _toothGameBGGameObject.Show();
                    PlaySpine(_toothGameBGGameObject, "zui", null, true);
                   ChangeBG(1);
                    BellSpeck(_sTT,6,null, () =>
                    {
                         _mask.transform.SetSiblingIndex(1);
                         _mask.Hide();
                        _sTT.Hide();
                        _toothGameItemsPosPanel.gameObject.Show();
                        InitToothGameLevelProperty(1);
                    });
                    
                   
                   
                    break;                                
            }
            _talkIndex++;
        }

        #region 游戏逻辑

     
		
        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
           //todo... 甜甜出现说话
           
           _mask.Show();
           _sTT.Show();
           BellSpeck(_sTT,0,null, () =>
            {
                
                _mask.Hide();
                _equipmentPanel.gameObject.Show();
                // _diaLoguePanel.Show();
                // Text dialogueContent = _diaLoguePanel.transform.GetTargetComponent<Text>("Text");
                //ShowDialogue("小朋友好，我是鳄鱼牙医，接下来认识一下不同牙科器材的作用吧", dialogueContent);
                _equitmentSpineGameObject.Hide();
                //_eyRight.Hide();
                //_eyLeft.Show();
                //PlaySpine(_eyLeft, "ey", null, true);
                _sTT.Hide();
                _alligatorSpine.Show();
                _mask.Show();
                BellSpeck(_alligatorSpine,1,null, () =>
                {
                    // dialogueContent.text = "";
                    // _diaLoguePanel.Hide();
                    _alligatorSpine.Hide();
                    _mask.Show();
                    _equitmentSpineGameObject.Show();
                    _equipmentClickPanel.gameObject.Show();
                    PlaySpine(_equitmentSpineGameObject, "gj0");
                },RoleType.Alligator);
                
            }); 
            
            
            //测试代码记得删
            //Delay(4,GameSuccess);
        }

		
		
		/// <summary>
        /// 游戏下一步
        /// </summary>
		private void GameNext()
		{
			_nextSpine.Show();
            PlaySpine(_nextSpine, "next2",()=> {
                AddEvent(_nextSpine, nextGo => {
                    PlayOnClickSound();
                    RemoveEvent(_nextSpine);
                    PlaySpine(_nextSpine, "next", () => {
                        _nextSpine.Hide();
                        //ToDo....                      
                    });
                });
            });
        }


		
		
        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            //_mask.Show();
            _replaySpine.Show();
			_okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); 
					RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();
                        PlayBgm(0);
                        //_mask.transform.SetSiblingIndex(1);
                        //_mask.Hide();
                        _sTT.Hide();
                        InitToothGameLevelProperty(1);
                        // GameInit();    				
                        // StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
					//PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _toothGamePanel.gameObject.Hide();
                        ChangeBG(3);
                        _replaySpine.Hide();
						_dTT.Show();
                        _mask.Show();
                        BellSpeck(_dTT, 8);
                        //BellSpeck(_dTT,0);
                        //ToDo...
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);						

                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            ChangeBG(2);
            _mask.Hide();
            _successSpine.Show();
            PlayCommonSound(3);			
			 PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });         
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }


		

        #endregion

        #region 常用函数

        #region 语音按钮

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region 隐藏和显示

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent,int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
        }


        #endregion
                 
        #region Spine相关

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            if (!_allSpineGameObjectList.Contains(go))
                _allSpineGameObjectList.Add(go);
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            SoundManager.instance.Stop("bgm");
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			switch(roleType)
			{
				case RoleType.Bd:
				     daiJi = "bd-daiji"; speak = "bd-speak";
				break;
				case RoleType.Xem:
				     daiJi = "daiji"; speak = "speak";
				break;
				case RoleType.Child:
				     daiJi = "animation"; speak = "animation2";
				 break;
				case RoleType.Adult:
				     daiJi = "daiji"; speak = "speak";
				break;
                case RoleType.Alligator:
                    daiJi = "1";
                    speak = "2";
                    break;
			}				
						 
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

        #endregion

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

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }


        #endregion

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.2f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion

    }
}
