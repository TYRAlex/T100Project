using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ILFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace OneID
{
    public delegate void TriggerReceive(GameObject go);
    
    public class OneIDVSGameManager : MonoBehaviour
    {
        enum ItemType
        {
            Bean,
            Square,
            Triangle
        }

        enum TigerColor
        {
            Blue=1,
            Green,
            Pink,
            Purple,
            Orange 
        }

        enum  TigerStatu
        {
            Nomal,
            Eat,
            Hurt,
            DisPear
        }
        
        private GameObject _mainTiger;
        private GameObject _mainTigerSpine;
        private Transform _dropOffItemPanel;
        private Transform _dropOffPosPanel;
        private Sprite[] _dropItemSprites;
        private Transform _scorePanel;
        private Dictionary<GameObject, Text> _scoreDic;
        private Transform _endPanel;
        private GameObject _stuInfoShowSpine;
        private Transform _tigerRunningPanel;
        private Transform _gameOverPanel;
        private GameObject _leftGameOverSpine;
        private GameObject _rightGameOverSpine;
        private GameObject _caipiaoSpine;
        private Transform _timePanel;
        private Transform _framePanel;
        private Image _frameColorSwitchImage;
        private Sprite[] _frameColorSprite;
        private GameObject _endYellowFrame;
        private GameObject _endSplitImage;

        private List<Transform> _itemDropStartPosList;
        private List<Transform> _itemDropItemList;

        private Dictionary<string, List<GameObject>> _allItemPool;

        private TigerColor _currentTiggerColor=TigerColor.Blue;
        private OneIDStudent _currentPlayingStudent;
        private OneIDStudent _leftStudent;
        private OneIDStudent _rightStudent;
        private bool _isleft = true;
        private bool _isPlaying = false;
        
        private void Awake()
        {
            _allItemPool=new Dictionary<string, List<GameObject>>();
            _mainTiger = this.transform.GetGameObject("MainTiger");
            _mainTigerSpine = _mainTiger.transform.GetGameObject("Spine");
            _isPlaying = false;
            _isleft = true;
            InializedAllDropProperty();
            InializedTimeCountProperty();
            InializedScoreProperty();
            InializedEndProperty();
            InializedFramePanelProperty();
            
            
        }

        private void OnEnable()
        {
            OneIDTrigger.CurrentTriggerReceive += GetTriggerItem;
            ResetAllGameProperty();
        }

        private void OnDisable()
        {
            OneIDTrigger.CurrentTriggerReceive -= GetTriggerItem;
            _mainTiger.Hide();
            StopAllCoroutines();
        }

        void ResetAllGameProperty()
        {
            
            StopAllCoroutines();
            if (_dropOffItemPanel.childCount > 0)
            {
                for (int i = 0; i < _dropOffItemPanel.childCount; i++)
                {
                    _dropOffItemPanel.GetChild(i).gameObject.Hide();
                }
            }
            Text ShowNumber = _leftGameOverSpine.transform.GetTargetComponent<Text>("Number");
            ShowNumber.text = "";
            ShowNumber.gameObject.Hide();
            ShowNumber = _rightGameOverSpine.transform.GetTargetComponent<Text>("Number");
            ShowNumber.text = "";
            ShowNumber.gameObject.Hide();
            _gameOverPanel.GetGameObject().Hide();
            _endYellowFrame.Hide();
            _endSplitImage.Hide();
            _frameColorSwitchImage.sprite = _frameColorSprite[0];
            _timePanel.GetGameObject("Left").Hide();
            _timePanel.GetGameObject("Right").Hide();
            _scorePanel.GetGameObject("Left").Hide();
            _scorePanel.GetGameObject("Right").Hide();
            ResetGamePropety(true);
        }

        private void Start()
        {

            
            
            
        }

        void ResetGamePropety(bool isLeft)
        {
            //todo...简单定义学生名字，后续需要修改
            _mainTiger.Hide();
            _stuInfoShowSpine.Show();
            //Debug.Log(" ssssssssssssssssssssss" +OneIDSceneManager.Instance.GetVSStudentDic().Count);
            OneIDSceneManager.Instance.PlayCommonSound(20);
            SpineManager.instance.DoAnimation(_stuInfoShowSpine,  isLeft==true?"sx1":"sx3", false, () =>
            {
                SpineManager.instance.DoAnimation(_stuInfoShowSpine, isLeft==true? "sx2":"sx4", false, () =>
                {
                   StartTheGame(isLeft);
                });
            });
        }

        void StartTheGame(bool isLeft)
        {
            _isPlaying = true;
            StopAllCoroutines();
            string studentName = isLeft == true ? "1" : "2";
            Debug.Log(studentName+" " +OneIDSceneManager.Instance.GetStudentByName("1"));
            _currentPlayingStudent = OneIDSceneManager.Instance.GetStudentByName(studentName);
            if (isLeft)
            {
                _leftStudent = _currentPlayingStudent;
            }
            else
            {
                _rightStudent = _currentPlayingStudent;
            }
            _timePanel.GetGameObject("Bush1").Show();
            _timePanel.GetGameObject("Bush2").Show();
            SetTargetScore(isLeft, "0");
            ChooseWhichSideStudentAndShowTheCountTime(isLeft);
            SwitchFrameColor(isLeft);
            _endYellowFrame.Hide();
            _stuInfoShowSpine.Hide();
            _mainTiger.Show();
            _currentTiggerColor = (TigerColor) Random.Range(1, 6);
            SwitchTigerStatu(TigerStatu.Nomal,_currentTiggerColor);
            StartCoroutine(StartDropOffItem());
           
        }

        void SwitchTigerStatu(TigerStatu statu,TigerColor tigerColor,Action callback=null)
        {
            string tigerSta = string.Empty;
            string colorIndex = string.Empty;
            switch (tigerColor)
            {
                case TigerColor.Purple:
                    colorIndex = "1";
                    break;
                case TigerColor.Pink:
                    colorIndex = "2";
                    break;
                case TigerColor.Blue:
                    colorIndex = "3";
                    break;
                case TigerColor.Orange:
                    colorIndex = "4";
                    break;
                case TigerColor.Green:
                    colorIndex = "5";
                    break;
            }
            switch (statu)
            {
                case TigerStatu.Nomal:
                    tigerSta = "h-a"+colorIndex;
                    break;
                case TigerStatu.Eat :
                    tigerSta = "h-b"+colorIndex;
                    break;
                case TigerStatu.Hurt:
                    tigerSta = "h-c"+colorIndex;
                    break;
                case TigerStatu.DisPear:
                    tigerSta = "h-l";
                    break;
            }
            
            SpineManager.instance.DoAnimation(_mainTigerSpine, tigerSta, statu == TigerStatu.Nomal ? true : false,
                callback);

        }

        void GetTriggerItem(GameObject go)
        {
            if(!_isPlaying)
                return;
            string targetInfo = go.GetComponent<Image>().sprite.name;
            go.Hide();
            string[] tar= targetInfo.Split('-');
            if (tar[0].Equals("M1")&& tar[1].Equals(((int)_currentTiggerColor).ToString()))
            {
                OneIDSceneManager.Instance.PlayCommonSound(18);
                SwitchTigerStatu(TigerStatu.Eat,_currentTiggerColor,()=>
                {
                    _currentTiggerColor = (TigerColor) Random.Range(1, 6);
                    SwitchTigerStatu(TigerStatu.Nomal, _currentTiggerColor);
                    OneIDSceneManager.Instance.PlayCommonSound(17);
                    ChangeScoreAndSwitchTigerColor(1);    
                });
                
            }
            else
            {
                OneIDSceneManager.Instance.PlayCommonSound(19);
                SwitchTigerStatu(TigerStatu.Hurt,_currentTiggerColor, () =>
                {
                    
                    _currentTiggerColor = (TigerColor) Random.Range(1, 6);
                    SwitchTigerStatu(TigerStatu.Nomal, _currentTiggerColor);
                    OneIDSceneManager.Instance.PlayCommonSound(17);
                    ChangeScoreAndSwitchTigerColor(-1);    
                });
            }

        }

        void ChangeScoreAndSwitchTigerColor(int number)
        {
            _currentPlayingStudent.AddScore(number);
            SetTargetScore(_isleft, _currentPlayingStudent.GetScore().ToString());
        }

        

        void InializedAllDropProperty()
        {
            _dropOffItemPanel=this.transform.GetTransform("DropOffItem");
            _dropOffPosPanel=this.transform.GetTransform("DropOffPos");
            _itemDropStartPosList=new List<Transform>();
            _itemDropItemList=new List<Transform>();
            for (int i = 0; i < _dropOffPosPanel.childCount; i++)
            {
                Transform target = _dropOffPosPanel.GetChild(i);
                _itemDropStartPosList.Add(target);
            }

            _dropItemSprites = _dropOffItemPanel.GetComponent<BellSprites>().sprites;
        }
        
        IEnumerator StartDropOffItem()
        {
            bool isTheTargetColor = false;
            while (_isPlaying)
            {
                RandomAndCreatItem(isTheTargetColor);
                isTheTargetColor = !isTheTargetColor;
                yield return new WaitForSeconds(1f);
            }
        }

        void RandomAndCreatItem(bool isTheTargetColor)
        {
            int randomType = Random.Range(0, 2);
            if (randomType > 0)
            {
                randomType = Random.Range(0, 2);
                if (randomType == 0)
                    randomType = 1;
                else if (randomType == 1)
                    randomType = 2;
            }

            ItemType itemType = (ItemType) randomType;
            int colorTypeIndex = Random.Range(0, 5);
            switch (itemType)
            {
                case ItemType.Bean:
                    if (isTheTargetColor)
                    {
                        switch (_currentTiggerColor)
                        {
                            case TigerColor.Blue:
                                colorTypeIndex = 0;
                                break;
                            case TigerColor.Green:
                                colorTypeIndex = 1;
                                break;
                            case TigerColor.Orange:
                                colorTypeIndex = 4;
                                break;
                            case TigerColor.Pink:
                                colorTypeIndex = 2;
                                break;
                            case TigerColor.Purple:
                                colorTypeIndex = 3;
                                break;
                        }
                    }

                    break;
                case ItemType.Square:
                    colorTypeIndex += 5;
                    break;
                case ItemType.Triangle:
                    colorTypeIndex += 10;
                    break;
            }
            
            Transform target = CreatItem(itemType, _dropItemSprites[colorTypeIndex]);
            target.position = _itemDropStartPosList[Random.Range(0, _itemDropStartPosList.Count)].position;
        }

        Transform CreatItem(ItemType itemType,Sprite sprite)
        {
            string itemName = itemType.ToString();
            GameObject target = null;
            if (!_allItemPool.ContainsKey(itemName))
            {
                _allItemPool[itemName]=new List<GameObject>();
                target= ResourceManager.instance.LoadCommonPrefab(itemType.ToString());
                target.name = itemName;
                _allItemPool[itemName].Add(target);
            }
            else
            {
                target = FindUsage(_allItemPool[itemName]);
                if (target == null)
                {
                    target= ResourceManager.instance.LoadCommonPrefab(itemType.ToString());
                    target.name = itemName;
                    _allItemPool[itemName].Add(target);
                }
            }
            target.Show();
            target.transform.SetParent(_dropOffItemPanel);
            target.transform.position=Vector3.zero;
            target.GetComponent<Image>().sprite = sprite;
            return target.transform;
        }

        GameObject FindUsage(List<GameObject> targetList)
        {
            GameObject target =targetList.Find(p => !p.activeSelf);
            //Debug.LogError("target:"+target);
            return target;
        }

       

        void InializedTimeCountProperty()
        {
            _timePanel=this.transform.GetTransform("TimePanel");
            for (int i = 0; i < _timePanel.childCount; i++)
            {
                Transform target = _timePanel.GetChild(i);
                if(target.name.Contains("Bush"))
                    continue;
                _timePanel.GetChild(i).gameObject.Hide();
            }
        }

        void ChooseWhichSideStudentAndShowTheCountTime(bool isLeft)
        {
            Transform target = null;
            if (isLeft)
            {
                target= _timePanel.GetTransform("Left");
                _timePanel.GetGameObject("Right").Hide();
            }
            else
            {
                target= _timePanel.GetTransform("Right");
                _timePanel.GetGameObject("Left").Hide();
            }
            target.gameObject.Show();
            Image targetImage = target.GetTargetComponent<Image>("Count");
            StartCountTime(60f,targetImage);
        }

        void StartCountTime(float timer,Image target)
        {
            StartCoroutine(CountTimeIE(timer, target));
        }

        IEnumerator CountTimeIE(float timer,Image target)
        {
            float leftTime = timer;
            
            while (true)
            {
                leftTime -= Time.fixedDeltaTime;
                target.fillAmount = leftTime / timer;
                if (leftTime<=0f)
                {
                    TimeOverAndExcuteNext();
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        void TimeOverAndExcuteNext()
        {
            _isPlaying = false;
            SwitchTigerStatu(TigerStatu.DisPear,_currentTiggerColor, () =>
            {
                OneIDSceneManager.Instance.PlayCommonSound(16);
                _tigerRunningPanel.transform.localPosition=Vector3.zero;
                _tigerRunningPanel.GetGameObject().Show();
                _tigerRunningPanel.transform.DOLocalMoveX(-2000f, 5f).OnComplete(() =>
                {
                    _tigerRunningPanel.GetGameObject().Hide();
                    if (!_isleft)
                        ShowWinnerAndLoseInfoAndExcuteNext();
                    else
                    {
                        _isleft = false;
                        ResetGamePropety(_isleft);
                    }
                });
            });
            //todo....
            
            
        }

        

        void ShowWinnerAndLoseInfoAndExcuteNext()
        {
            _endSplitImage.Show();
            _endYellowFrame.Show();
            _gameOverPanel.gameObject.Show();
            string leftStuAni=String.Empty;
            string rightStuAni = string.Empty;
            if (_leftStudent.GetScore() >= _rightStudent.GetScore())
            {
                SpineManager.instance.DoAnimation(_leftGameOverSpine, "sl-a1", false,
                    () =>
                    {
                        Text number= _leftGameOverSpine.transform.GetTargetComponent<Text>("Number");
                        number.gameObject.Show();
                        number.text = _leftStudent.GetScore().ToString();
                        SpineManager.instance.DoAnimation(_leftGameOverSpine, "sl-a2", false);
                    });
                SpineManager.instance.DoAnimation(_rightGameOverSpine, "sl-b3", false, () =>
                {
                    Text number= _rightGameOverSpine.transform.GetTargetComponent<Text>("Number");
                    number.gameObject.Show();
                    number.text = _rightStudent.GetScore().ToString();
                });
                _caipiaoSpine.Show();
                _caipiaoSpine.transform.localPosition=Vector3.zero;
                SpineManager.instance.DoAnimation(_caipiaoSpine, "sp", false);
            }
            else
            {
                SpineManager.instance.DoAnimation(_rightGameOverSpine, "sl-b1", false,
                    () =>
                    {
                        Text number= _rightGameOverSpine.transform.GetTargetComponent<Text>("Number");
                        number.gameObject.Show();
                        number.text = _rightStudent.GetScore().ToString();
                        SpineManager.instance.DoAnimation(_rightGameOverSpine, "sl-b2", false);
                    });
                SpineManager.instance.DoAnimation(_leftGameOverSpine, "sl-a3", false, () =>
                {
                    Text number= _leftGameOverSpine.transform.GetTargetComponent<Text>("Number");
                    number.gameObject.Show();
                    number.text = _leftStudent.GetScore().ToString();
                });
                _caipiaoSpine.Show();
                _caipiaoSpine.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920f / 2f, 0);
                SpineManager.instance.DoAnimation(_caipiaoSpine, "sp", false);
            }

            
        }

        void InializedScoreProperty()
        {
            _scorePanel=this.transform.GetTransform("ScorePanel");
            _scoreDic=new Dictionary<GameObject, Text>();
            for (int i = 0; i < _scorePanel.childCount; i++)
            {
                Transform target = _scorePanel.GetChild(i);
                _scoreDic.Add(target.gameObject, target.GetTargetComponent<Text>("Text"));
                target.gameObject.Hide();   
            }
        }

        void SetTargetScore(bool isLeft,string targetValue)
        {
            GameObject target = null;
            if (isLeft)
            {
                target = _scorePanel.GetGameObject("Left");
                _scorePanel.GetGameObject("Right").Hide();
                
            }
            else
            {
                target = _scorePanel.GetGameObject("Right");
                _scorePanel.GetGameObject("Left").Hide();
               
            }
            target.Show();
            _scoreDic[target].text = targetValue;
        }

        void InializedEndProperty()
        {
            _endPanel=this.transform.GetTransform("EndPanel");
            _stuInfoShowSpine = _endPanel.GetGameObject("InfoShow");
            
            _tigerRunningPanel = _endPanel.GetTransform("TigerRunningPanel");
            _stuInfoShowSpine.Hide();
            
            _tigerRunningPanel.GetGameObject().Hide();
        }

        void InializedFramePanelProperty()
        {
            _framePanel=this.transform.GetTransform("FramePanel");
            _frameColorSwitchImage = _framePanel.GetTargetComponent<Image>("SwitchFrameColor");
            _endYellowFrame = _framePanel.GetGameObject("EndFrame");
            _frameColorSprite = _framePanel.GetComponent<BellSprites>().sprites;
            _endSplitImage = _framePanel.GetGameObject("SplitImage");
            
            _gameOverPanel = _framePanel.GetTransform("GameOverPanel");
            _leftGameOverSpine = _gameOverPanel.GetGameObject("Left");
            _rightGameOverSpine = _gameOverPanel.GetGameObject("Right");
            _caipiaoSpine = _gameOverPanel.GetGameObject("SP");
            _gameOverPanel.GetGameObject().Hide();
        }
        
       

        void SwitchFrameColor(bool isLeft)
        {
            _frameColorSwitchImage.gameObject.Show();
            if (isLeft)
            {
                _frameColorSwitchImage.sprite=_frameColorSprite[0];
            }
            else
            {
                _frameColorSwitchImage.sprite = _frameColorSprite[1];
            }
        }

        void SetTheEndYellowFrameVisible(bool isShow)
        {
            if (_endYellowFrame.activeSelf != isShow)
                _endYellowFrame.SetActive(isShow);
        }

        void DelayEvent(float timer, Action callback)
        {
            StartCoroutine(DelayEventIE(timer, callback));
        }

        IEnumerator DelayEventIE(float timer, Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }

    }
}