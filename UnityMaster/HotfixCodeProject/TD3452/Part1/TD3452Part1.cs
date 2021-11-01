using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;
using Object = UnityEngine.Object;
namespace ILFramework.HotClass
{	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,		
	}
	
    public class TD3452Part1
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
	    					        
		private GameObject _dTT;							      
	    private GameObject _sTT;				
		              	   
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Transform _bgs;
        
        private Transform _spines;
        private Transform _treePrefabs;
        private Transform _des;

        private bool _isMove;
        private bool _isOnClickFruits;

        private GameObject _niao;

        private RectTransform _ban;

        private List<string> _gameCorrectSpineNames;         //游戏所有正确的Spine名
        private List<string> _gameErrorSpineNames;           //游戏所有错误的Spine名
        private List<string> _gameChooseCorrectSpineNames;   //游戏选择正确的Spine名
        private List<string> _gameAllSpineName;

        private GameObject _black;
       
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");			
									
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");					
			_dTT = curTrans.GetGameObject("dTT");           			         	
			_sTT = curTrans.GetGameObject("sTT");

            _bgs = curTrans.Find("BG/bg");
           
            _spines = curTrans.Find("spines");
            _treePrefabs = curTrans.Find("treePrefabs");
            _des = curTrans.Find("des");
            _niao = curTrans.GetGameObject("spines/game1/niao");
            _ban = curTrans.GetRectTransform("spines/game1/ban");


            _black = curTrans.GetGameObject("spines/black");

           


            GameInit();
            GameStart();
        }

        void InitData()
        {
            _talkIndex = 1;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _isMove = false;
            _isOnClickFruits = false;
            _gameCorrectSpineNames = new List<string> { "yza1", "yza2", "yza3", "yza4", "yza5" };
            _gameErrorSpineNames = new List<string>   { "yzc1", "yzc2", "yzc3", "yzc4", "yzc5" };
          
            _gameChooseCorrectSpineNames = new List<string>();
                              
            _gameAllSpineName = new List<string> { "yza1", "yza2", "yza3", "yza4", "yza5", "yzc1", "yzc2", "yzc3", "yzc4", "yzc5" };
        }

        void GameInit()
        {
            InitData();

            
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); 
			StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();_dTT.Hide(); _sTT.Hide(); 				
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine); _black.Hide();

            HideAllChilds(_bgs); ShowChilds(_bgs, 0);
                 
            HideAllChilds(_spines);

            //初始化背景1下子物体的位置
            _bgs.Find("1/1-1").GetRectTransform().anchoredPosition = new Vector2(0, 0);          
            _bgs.Find("1/1-2").GetRectTransform().anchoredPosition = new Vector2(1920, 0);
        

            //初始化开头显示孔雀Spine
            ShowChilds(_spines, 0);          
            var startshowkongque = _spines.Find("startshowkongque").GetComponentsInChildren<SkeletonGraphic>();
            foreach (var spine in startshowkongque)
            {
                spine.Initialize(true);

                var name = spine.gameObject.name;
                var rect = spine.transform.GetRectTransform();
                switch (name)
                {
                    case "kq-0":
                       // PlaySpine(spine.gameObject, "kong" );
                        break;
                    case "kq2":
                    case "kq3":
                        rect.anchoredPosition = new Vector2(-1920, 1080);
                        PlaySpine(spine.gameObject, name, null, true);                     
                        break;
                    case "kq4":
                    case "kq5":
                        rect.anchoredPosition = new Vector2(1920, 1080);
                        PlaySpine(spine.gameObject, name, null, true);                  
                        break;
                    case "niao1":
                        rect.anchoredPosition = new Vector2(-1130, 280);
                        PlaySpine(spine.gameObject, "niao",null,true);
                        break;
                    case "niao2":
                        rect.anchoredPosition = new Vector2(2984, 227);
                        PlaySpine(spine.gameObject, "niao", null, true);
                        break;
                    default:
                        PlaySpine(spine.gameObject, spine.name, null, true);
                        break;
                }              
            }

            //初始化游戏1左侧面板Spine
            ShowChilds(_spines, 1);          
            var bans = _spines.Find("game1/ban").GetComponentsInChildren<SkeletonGraphic>();
            foreach (var ban in bans)
            {
                ban.Initialize(true);
                PlaySpine(ban.gameObject, "kong");
            }
            PlaySpine(bans[0].gameObject, bans[0].gameObject.name);

           
            //初始实例化树2颗 除了第1颗，其他随机
            var treeParent = _spines.Find("game1/trees");

            if (treeParent.childCount!=0)
            {                                   
                for (int i = 0; i < treeParent.childCount; i++)                                 
                    Object.Destroy(treeParent.GetChild(i).gameObject);                                           
            }

            CreateTree(_treePrefabs.GetChild(0).gameObject, treeParent,new Vector2(-480,0)); //创建第一颗树
            CreateTree(_treePrefabs.GetChild(Random.Range(1, 6)).gameObject, treeParent,new Vector2(480,0),(newTree)=> {     //随机创建第二颗树
                var newTreeE4Rs = newTree.transform.GetComponentsInChildren<Empty4Raycast>();
                foreach (var treeE4R in newTreeE4Rs)
                    AddEvent(treeE4R.gameObject, OnClickTree);
                RandomFeather(newTreeE4Rs);
            });
     

            //初始化游戏1鸟的Spine和位置
            _niao.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(_niao, _niao.name, null, true);
            _niao.transform.GetRectTransform().anchoredPosition = new Vector2(-258, 27);
            _niao.transform.GetRectTransform().localEulerAngles = Vector3.zero;

           //初始化板的位置
           _ban.anchoredPosition = new Vector2(0, 1080);
             HideChilds(_spines, 1);


            //初始化游戏2Spine
            ShowChilds(_spines, 2);           
            var game2Spines = _spines.Find("game2").GetComponentsInChildren<SkeletonGraphic>();
        
            GetRandomList(ref _gameAllSpineName);   
            int indexTemp = 0;
            foreach (var spine in game2Spines)
            {
                spine.Initialize(true);
                var go = spine.gameObject;
                var name = go.name;
                
                switch (name)
                {
                    case "spine":
                        PlaySpine(go, _gameAllSpineName[indexTemp]);
                        go.transform.GetChild(0).name = _gameAllSpineName[indexTemp];
                        go.transform.localPosition = new Vector2(30, 60);
                        spine.color = new Color(1, 1, 1, 1);
                        indexTemp++;
                        break;
                    case "apple":
                        PlaySpine(go, "apple3");
                        go.transform.localPosition = Vector2.zero;
                        break;
                    case "orange":
                        PlaySpine(go, "apple4");
                        go.transform.localPosition = Vector2.zero;
                        break;
                    case "kq-a1":
                        PlaySpine(go, name,null,true);
                        break;
                    case "niao":
                        PlaySpine(go, name, null, true);
                        go.transform.localPosition = new Vector2(-800, -15);
                        go.transform.localEulerAngles = new Vector3(0, 0, 0);                          
                        break;
                    case "yun":
                    case "star":
                        break;
                    default:
                        PlaySpine(go, name, null, true);
                        spine.color = new Color(1, 1, 1, 0);
                        break;
                }               
            }
          
            //初始化游戏2点击事件
            var game2E4Rs = _spines.Find("game2").GetComponentsInChildren<Empty4Raycast>();
            foreach (var e4R in game2E4Rs)
            {
                e4R.raycastTarget = true;
                AddEvent(e4R.gameObject, OnClickFruits);
            }
            HideChilds(_spines, 2);

            //初始化拍照Spine
            ShowChilds(_spines, 3);

            _spines.Find("game3/1").gameObject.Show();
            _spines.Find("game3/2").gameObject.Show();
            _spines.Find("game3/3").gameObject.Show();
            var game3AllSpine = _spines.Find("game3").GetComponentsInChildren<SkeletonGraphic>();
            var xingNames = new List<string> { "x", "x2", "x3" };

            foreach (var spine in game3AllSpine)
            {
                spine.freeze = false;
                spine.Initialize(true);

                var name = spine.gameObject.name;
                switch (name)
                {
                    case "cam": //什么都不干                               
                      break;
                    case "x":
                       var index = Random.Range(0, xingNames.Count);
                        PlaySpine(spine.gameObject, xingNames[index],null,true);
                        break;
                    default:
                        PlaySpine(spine.gameObject, spine.name,null,true);
                        break;
                }                         
            }
       
            _spines.Find("game3/1").gameObject.Show();
            _spines.Find("game3/2").gameObject.Hide();
            _spines.Find("game3/3").gameObject.Hide();

            HideChilds(_spines, 3);

        }

        private bool IsExistTree(string name)
        {
            bool isExist = false;

            for (int i = 0; i < _des.childCount; i++)
            {
                var child = _des.GetChild(i);
                if (child.name==name)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();			
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        //PlayCommonBgm(8);
                        PlayBgm(0);

                        _startSpine.Hide();
                        _sTT.Show();
                        BellSpeck(_sTT, 0, null,()=> { ShowVoiceBtn(); });					                       
                    });
                });
            });
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sTT, 1, null, () => { _sTT.Hide(); StartGame(); });           
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
            _mask.Hide();

            Delay(3, () => {            
                Game1();
            });
        }
	
        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
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
                        GameInit();
                        StartGame();
                        PlayBgm(0);

                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
					PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();                     
                        _dTT.Show(); BellSpeck(_dTT, 2);
                    });
                });
            });
        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);			
			 PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });         
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }


        /// <summary>
        /// 游戏1
        /// </summary>
        private void Game1()
        {
           // PlayVoice(0, true);
            HideChilds(_bgs, 0);ShowChilds(_bgs, 1);
            HideChilds(_spines, 0);ShowChilds(_spines, 1);          
        }

        private void OnClickTree(GameObject go)
        {          
            if (_isMove)            
                return;

            PlayVoice(5);
            _isMove = true;

            var parent = go.transform.parent.parent;
            var pos = _niao.transform.parent.InverseTransformPoint(go.transform.position);  //把点击的世界坐标转成在游戏1鸟父物体下Transform下的点



            bool isRotationY = _niao.transform.GetRectTransform().anchoredPosition.x > pos.x;                             //是否需要旋转鸟的Y轴

            if (isRotationY)
            {
                _niao.transform.GetRectTransform().rotation = Quaternion.Euler(new Vector3(0, 180, 0));              
            }
            else
            {
                _niao.transform.GetRectTransform().rotation = Quaternion.Euler(Vector3.zero);              
            }

            //判断是否是正确的羽毛
            var name = go.name;
            var goChildName = go.transform.GetChild(0).name;
            var spineGo = go.transform.parent.Find("Spine" + name.Replace("OnClick", string.Empty)).gameObject;
            var star = go.transform.parent.Find("star").gameObject;
            bool isCorrect = _gameCorrectSpineNames.Contains(goChildName);
            bool isActiveSelf = spineGo.activeSelf;


            if (isActiveSelf)
            {
                if (isRotationY)
                {                 
                    _niao.transform.GetRectTransform().DOLocalMove(new Vector2(pos.x+129,pos.y+153), 1.5f);
                }
                else
                {
                    _niao.transform.GetRectTransform().DOLocalMove(new Vector2(pos.x - 100, pos.y + 130), 1.5f);
                }
                

                Delay(1.5f, () => {
             
                if (isCorrect)
                {
                        bool isContain = _gameChooseCorrectSpineNames.Contains(goChildName);

                        if (!isContain)
                             _gameChooseCorrectSpineNames.Add(goChildName);

                        PlaySpine(_niao, "niao2", () => {

                       
                        PlaySuccessSound();
                        star.transform.GetRectTransform().anchoredPosition = spineGo.transform.GetRectTransform().anchoredPosition;
                        star.transform.SetAsLastSibling();
                        spineGo.transform.SetAsLastSibling();
                        PlaySpine(star, "star6");
                        PlaySpine(spineGo, goChildName.Replace("a", "b"), () => { spineGo.Hide();

                            if (isContain)
                            {
                                Move(parent, pos);
                            }
                            else
                            {
                                //下滑
                                PlayVoice(3);
                                _ban.DOAnchorPosY(0, 1.5f).SetEase(Ease.OutBounce);
                                Delay(1.5f, () => {
                                    var banName = "banzi" + (6 - int.Parse(goChildName.Replace("yza", string.Empty)));
                                    var banGo = _ban.Find(banName).gameObject;
                                    PlaySpine(banGo, banName, () => {
                                        Delay(1f, () => {  //上滑
                                            if (_gameCorrectSpineNames.Count == _gameChooseCorrectSpineNames.Count)
                                            {
                                                Delay(2, Game1Over);
                                                return;
                                            }
                                            PlayVoice(8);
                                            _ban.DOAnchorPosY(1080, 0.5f).SetEase(Ease.InSine);
                                            Delay(0.5f, () => { Move(parent, pos); });
                                        });

                                    });
                                });
                            }                           
                        });
                        PlaySpine(_niao, "niao3", () => { PlaySpine(_niao, _niao.name, null, true); }); });
                                                     
                    }
                    else
                    {
                        PlayFailSound();
                        PlaySpine(_niao, "niao2", () => {
                            spineGo.transform.SetAsLastSibling();
                            PlaySpine(spineGo, goChildName.Replace("c", "d"), () => { PlaySpine(spineGo, "yun", () => { spineGo.Hide(); }); });
                            PlaySpine(_niao, "niao4", () => { PlaySpine(_niao, _niao.name, null, true); }); });
                        Move(parent, pos);

                    }

                });
            }
            else
            {
                _niao.transform.GetRectTransform().DOLocalMove(new Vector2(pos.x,pos.y+100), 1);
                Delay(1, () => { _isMove = false; });
            }     
        }


        private void Move(Transform parent, Vector2 pos)
        {
            Delay(0.8f, () => {

                if (pos.x > 100)
                {
                    CreateNextTree(parent);
                    BgMove(480, 1);
                    TreeMove(parent, 960, 1);
                    _niao.transform.GetRectTransform().DOLocalMoveX(pos.x - 960, 1);
                }
                else
                {
                    _isMove = false;
                }
            });
        }


        /// <summary>
        /// 背景移动
        /// </summary>
        /// <param name="endValue"></param>
        /// <param name="durantion"></param>
        private void BgMove(float endValue,float durantion )
        {
            //背景整体移动480个单位长度

            var bgParent = _bgs.Find("1");
            var rect11 = bgParent.Find("1-1").GetRectTransform();
            var rect12 = bgParent.Find("1-2").GetRectTransform();

            var x1 = Convert.ToInt32(rect11.anchoredPosition.x);
            var x2 = Convert.ToInt32(rect12.anchoredPosition.x);
         
            if (x1 <= -1920)
            { 
                rect11.anchoredPosition = new Vector2(1920, 0);
                x1 = Convert.ToInt32(rect11.anchoredPosition.x);
            }

            if (x2 <= -1920)
            {
                rect12.anchoredPosition = new Vector2(1920, 0);
                x2 = Convert.ToInt32(rect12.anchoredPosition.x);
            }
           
            rect11.DOAnchorPosX(x1 - endValue, durantion);
            rect12.DOAnchorPosX(x2 - endValue, durantion);
    
        }

        /// <summary>
        /// 树移动
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="endValue"></param>
        /// <param name="duration"></param>
        private void TreeMove(Transform parent,float endValue,float duration)
        {
            //树整体移动960个单位长度
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).GetRectTransform();
                var x = Convert.ToInt32(child.anchoredPosition.x);
                child.DOAnchorPosX(x - endValue, duration);
            }
           
            Delay(duration, () => {
                parent.GetChild(0).SetParent(_des);          
                _isMove = false;
            });
        }

        /// <summary>
        /// 生成树
        /// </summary>
        /// <param name="prefab">要创建树的预制体</param>
        /// <param name="parent">预制体生成的父物体</param>
        /// <param name="pos">坐标</param>
        /// <param name="callBack">创建完成的回调</param>
        private void CreateTree(GameObject prefab, Transform parent, Vector2 pos, Action<GameObject> callBack=null)
        {    
            var name = prefab.name;

            GameObject newTree = null;

            bool isExist = IsExistTree(name);

            if (isExist)
            {
                newTree= _des.Find(name).gameObject;
                newTree.transform.SetParent(parent);
            }
            else
            {
                newTree= Object.Instantiate(prefab, parent, false);
                newTree.name = name;
            }

            newTree.transform.GetRectTransform().anchoredPosition = pos;

            callBack?.Invoke(newTree);
        }

        /// <summary>
        /// 生成新的树
        /// </summary>
        /// <param name="parent"></param>
        private void CreateNextTree(Transform parent)
        {
            var firstPosx = Convert.ToInt32(parent.GetChild(0).GetRectTransform().anchoredPosition.x);
            var lastPosX =  Convert.ToInt32(parent.GetChild(parent.childCount - 1).GetRectTransform().anchoredPosition.x);

            bool isCreateNew = firstPosx - 960 <= -1440f;
            if (isCreateNew)
            {                
                var prefab = _treePrefabs.GetChild(Random.Range(1, 6)).gameObject;

                CreateTree(prefab, parent, new Vector2(lastPosX + 960, 0),(newtree) => {

                    var treeE4Rs = newtree.transform.GetComponentsInChildren<Empty4Raycast>();

                    foreach (var treeE4R in treeE4Rs)
                        AddEvent(treeE4R.gameObject, OnClickTree);

                    RandomFeather(treeE4Rs);
                });              
            }
        }
         
        /// <summary>
        /// 随机羽毛
        /// </summary>
        /// <param name="e4Rs"></param>
        private void RandomFeather(Empty4Raycast[] e4Rs)
        {                   
            var count = e4Rs.Length;
            string[] spineName = new string[count];

            //先随正确的
            while (true)
            {
                int index = Random.Range(0, _gameCorrectSpineNames.Count);
                string correctName = _gameCorrectSpineNames[index];
                bool isContain =  _gameChooseCorrectSpineNames.Contains(correctName);
                if (!isContain)
                {
                    spineName[0] = correctName;                   
                    break;
                }
            }
          
            //再随错误的
            for (int i = 0; i < count-1; i++)
            {
                int index = Random.Range(0, _gameErrorSpineNames.Count);
                string errorName = _gameErrorSpineNames[index];
                spineName[i+1] = errorName;               
            }
          
            GetRandomArray(ref spineName);
           
            for (int i = 0; i < e4Rs.Length; i++)
            {
                var onClick = e4Rs[i].transform;
                var parent = onClick.parent;
                var child = onClick.GetChild(0);
                child.name = spineName[i];

                var spineGoName = "Spine" + onClick.name.Replace("OnClick", string.Empty);

                var spineGo = parent.Find(spineGoName).gameObject;
                spineGo.transform.SetAsFirstSibling();
                spineGo.Show();
                PlaySpine(spineGo, spineName[i]);         
            }

            var tree =   e4Rs[0].transform.parent;
            tree.Find("star").GetComponent<SkeletonGraphic>().Initialize(true);
        }

        /// <summary>
        /// 游戏1结束
        /// </summary>
        private void Game1Over()
        {
            
            HideChilds(_bgs, 1);ShowChilds(_bgs, 0);
           
            HideChilds(_spines, 1);ShowChilds(_spines, 0);

            var parent = _spines.Find("startshowkongque");

            var kq2Rect = parent.GetRectTransform("kq2");
            var kq3Rect = parent.GetRectTransform("kq3");       
            var kq4Rect = parent.GetRectTransform("kq4");
            var kq5Rect = parent.GetRectTransform("kq5");
            
            var niao1Rect = parent.GetRectTransform("niao1");
            var niao2Rect = parent.GetRectTransform("niao2");

            kq2Rect.DOAnchorPos(Vector2.zero, 2);
            kq3Rect.DOAnchorPos(Vector2.zero, 2);       
            kq4Rect.DOAnchorPos(Vector2.zero, 2);
            kq5Rect.DOAnchorPos(Vector2.zero, 2);
            niao1Rect.DOAnchorPos(new Vector2(788, -800), 2);
            niao2Rect.DOAnchorPos(new Vector2(1062, -850), 2);

            Delay(2, () => {
                PlayVoice(4);
                niao1Rect.DOAnchorPos(new Vector2(307, -505), 1);             
                niao2Rect.DOAnchorPos(new Vector2(1650, -555), 1);
                Delay(1, () => {
                    var kq1 = parent.Find("kq1").gameObject;
                    PlaySpine(kq1, "kq6", null, true);
                    PlayVoice(6);
                    var kq0Spine = parent.Find("kq-0").GetComponent<SkeletonGraphic>();
                    PlaySpine(kq0Spine.gameObject, "kq-0+", () => {
                        var time = PlaySpine(kq0Spine.gameObject, kq0Spine.name, null, true);
                       
                        Delay(time, () => {

                            //孔雀开心
                           
                            var niao1 = niao1Rect.gameObject;
                            var niao2 = niao2Rect.gameObject;
                            PlaySpine(kq1, "kq7", () => { PlaySpine(kq1, kq1.name, Game2); });
                            PlaySpine(niao1, "niao3", () => { PlaySpine(niao1, "niao", null, true); });
                            PlaySpine(niao2, "niao3", () => { PlaySpine(niao2, "niao", null, true); });

                        });
                    });
                });
                                                      
            });
        }

        private void Game2()
        {
            _gameChooseCorrectSpineNames.Clear();
            ShowChilds(_bgs, 2); HideChilds(_bgs, 0);           
            ShowChilds(_spines, 2); HideChilds(_spines, 0);
        }

     

        private void OnClickFruits(GameObject go)
        {
            if (_isOnClickFruits)            
                return;
            
            _isOnClickFruits = true;
            PlayVoice(5);
            go.transform.GetComponent<Empty4Raycast>().raycastTarget = false;          //关闭点击检测

            var name = go.name;
            var star = _spines.Find("game2/star").gameObject;
            var niaoRect = _spines.Find("game2/niao").transform.GetRectTransform();        //游戏2蜂鸟
            var featherNameRect = go.transform.Find("spine").GetChild(0).GetRectTransform();   //游戏2羽毛名字Rect
            var featherGo = go.transform.Find("spine").gameObject;                         //游戏2羽毛GameObject
            GameObject fruitGo = null;                                                     //水果GameObject
            string fruitspineName = string.Empty;                                          //水果点击播放SpineName
            string featherName = featherNameRect.name;                                         //羽毛Spine名字    
            GameObject yun = go.transform.Find("yun").gameObject;                          //爆炸的云

            bool isApple = int.Parse(name.Replace("grid", string.Empty)) <= 5;
            if (isApple)
            { 
                fruitGo = go.transform.Find("apple").gameObject;
                fruitspineName = "apple";
            }
            else
            { 
                fruitGo = go.transform.Find("orange").gameObject; 
                fruitspineName = "apple2";
            }
          
            var goPos = go.transform.GetRectTransform().anchoredPosition;       //点击物体的锚点坐标   
            var niaoPos = niaoRect.anchoredPosition;                            //鸟的锚点坐标
            var featherPos = niaoRect.transform.parent.InverseTransformPoint(featherNameRect.transform.position);  //把羽毛的世界坐标转换到相对于鸟的父物体下的局部锚点坐标
        
            Vector2 niaoEndPos  =new Vector2();
            niaoEndPos.y = Convert.ToInt32(featherPos.y) - 73;
            bool isRotationY = niaoPos.x > goPos.x;                             //是否需要旋转鸟的Y轴

            if (isRotationY)
            {
                niaoRect.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                niaoEndPos.x = Convert.ToInt32(featherPos.x) +107;              
            }
            else
            {
                niaoRect.rotation = Quaternion.Euler(Vector3.zero);
                niaoEndPos.x = Convert.ToInt32(featherPos.x) - 117;              
            }
         
            niaoRect.DOLocalMove(niaoEndPos, 1f);

            Delay(0.5f, () => { PlayVoice(9); });
         
            PlaySpine(fruitGo, fruitspineName,()=> { fruitGo.transform.GetRectTransform().DOAnchorPosY(-1080, 0.5f).SetEase(Ease.Linear); });

            Delay(1, () => {
              
                bool isCorrect = _gameCorrectSpineNames.Contains(featherName);
                
                if (isCorrect)
                {
                    PlaySuccessSound();
                    star.transform.GetRectTransform().anchoredPosition = featherPos;
                    PlaySpine(star, "star6");
                    PlaySpine(niaoRect.gameObject, "niao2", () => { PlaySpine(niaoRect.gameObject,"niao3",()=> { PlaySpine(niaoRect.gameObject,"niao",null,true); }); });
                    PlaySpine(featherGo, featherName.Replace("a", "b"),()=> {
                        featherGo.GetComponent<SkeletonGraphic>().DOColor(new Color(1, 1, 1, 0), 0.5f);

                        string kongQueFeatherName = string.Empty;
                        switch (featherName)
                        {
                            case "yza2":
                                kongQueFeatherName = "kq-a2";
                                break;
                            case "yza5":
                                kongQueFeatherName = "kq-a5";
                                break;
                            default:
                                kongQueFeatherName = "kq-a" + (7 - int.Parse(featherName.Replace("yza", string.Empty)));
                                break;

                        }

                       
                        var kongQueFeatherSG = _spines.Find("game2/"+ kongQueFeatherName).GetComponent<SkeletonGraphic>();
                        kongQueFeatherSG.DOColor(new Color(1, 1, 1, 1), 0.5f);

                        _gameChooseCorrectSpineNames.Add(featherName);
                        if(_gameChooseCorrectSpineNames.Count== _gameCorrectSpineNames.Count)
                        {
                             _spines.Find("game2/kq-a7").GetComponent<SkeletonGraphic>().DOColor(new Color(1,1,1,1),0);
                           
                            Delay(2, Game2Over);
                           
                            return;
                        }
                        _isOnClickFruits = false;
                    });
                }                      
                else
                {
                    PlayFailSound();
                    PlaySpine(niaoRect.gameObject, "niao2", () => { PlaySpine(niaoRect.gameObject, "niao4", () => { PlaySpine(niaoRect.gameObject, "niao", null, true); }); });
                    PlaySpine(featherGo, featherName.Replace("c", "d"),()=> {
                        featherGo.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);                      
                        PlaySpine(yun, yun.name);
                        _isOnClickFruits = false;
                    });
                }                       
            });
          

        }


        private void Game2Over()
        {
          

            ShowChilds(_bgs, 0); HideChilds(_bgs, 2);
            HideAllChilds(_spines);
            ShowChilds(_spines, 3);
            var cam = _spines.Find("game3/cam").gameObject;
            var camSG = cam.GetComponent<SkeletonGraphic>();

            var spine1s =_spines.Find("game3/1").GetComponentsInChildren<SkeletonGraphic>();
            var spine2s = _spines.Find("game3/2").GetComponentsInChildren<SkeletonGraphic>();
            var niaos = _spines.Find("game3/allniao").GetComponentsInChildren<SkeletonGraphic>();
            var kqa8 = _spines.Find("game3/3/kongque/kq-a8").gameObject;
            var xing3 = _spines.Find("game3/3/xing").gameObject;
            var z000 = _spines.Find("game3/Z000").gameObject;
            xing3.Hide();
            z000.Show();
            Delay(2, Ani1); 
          
            void Ani1()
            {
                PlaySpine(cam, cam.name, () => {
                    _black.Show();
                    PlayVoice(7);
                    camSG.Initialize(true);
                    foreach (var spine1 in spine1s)
                        spine1.freeze = true;

                    foreach (var niao in niaos)
                        niao.freeze = true;

                    Delay(0.5f, () => { _black.Hide();
                        Delay(1,  Ani2);
                    });
                });
            }

            void Ani2()
            {
                foreach (var niao in niaos)
                    niao.freeze = false;
                z000.Hide();
                _spines.Find("game3/1").gameObject.Hide();
                _spines.Find("game3/2").gameObject.Show();
               



                Delay(2, () => {
                    PlaySpine(cam, cam.name, () => {
                        _black.Show();
                        PlayVoice(7);
                        camSG.Initialize(true);

                        foreach (var spine2 in spine2s)
                            spine2.freeze = true;
                        foreach (var niao in niaos)
                            niao.freeze = true;

                        Delay(0.5f, () => {
                            _black.Hide();
                            Delay(1, Ani3);
                        });
                    });

                });
              
            }

            void Ani3()
            {
                foreach (var niao in niaos)
                    niao.freeze = false;
                z000.Hide();
                _spines.Find("game3/2").gameObject.Hide();
                _spines.Find("game3/3").gameObject.Show();
                
                PlaySpine(kqa8, kqa8.name,()=> { xing3.Show();

                    Delay(2, GameSuccess);
                });
              
            }

        }


        private  void GetRandomList<T>(ref List<T> list)
        {
            int i, r = list.Count - 1;
            System.Random rand = new System.Random();
            for (i = 0; i < list.Count; i++)
            {
                int n = rand.Next(0, r);
                T tem = list[i];
                list[i] = list[n];
                list[n] = tem;
                r--;
            }
        }

        private void GetRandomArray<T>(ref T[] arr)
        {
            int i, r = arr.Length - 1;
            System.Random rand = new System.Random();
            for (i = 0; i < arr.Length; i++)
            {
                int n = rand.Next(0, r);
                T tem = arr[i];
                arr[i] = arr[n];
                arr[n] = tem;
                r--;
            }
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
           // PlayCommonSound(5);
            PlayVoice(2);
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            //PlayCommonSound(4);
            PlayVoice(1);
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
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
            RemoveEvent(go);
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
                yield return new WaitForSeconds(0.1f);
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
