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
    }

    public class TD6754Part1
    {
        private enum ClickLeftEnum
        {
            One,
            Two,
            Three,
            Null
        }
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _dDD;
        private GameObject _sDD;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
        private bool _isPlaying;

        //rightPanel
        private RawImage _bg;
        private Transform _rightPanelStartPos;
        private Transform _rightPanelEndPos;
        private Transform _rightPanel;

        private GameObject _presonPanel;
        private Image _face;
        private GameObject _personPanelBg;

        private Transform _droperPanel;
        private Transform[] _droperPanels;
        private ILDroper[] _ilDroper0;
        private ILDroper[] _ilDroper1;
        private ILDroper[] _ilDroper2;

        private Transform _spinePanel0;
        private GameObject[] _spinePanels0;
        private Transform _spinePanel1;
        private GameObject[] _spinePanels1;

        private Transform _starPanel0;
        private Transform _starPanel1;
        private GameObject[] _starPanels0;
        private GameObject[] _starPanels1;

        private Transform _clickSpinePanel;
        private Empty4Raycast[] _clickSpinePanels;
        private ILDrager[] _clickSpineILDragers;
        private Transform _clickSpinePos;
        private Vector3[] _clickSpinePoss;

        private Transform clickDragerPanelPos0;
        private Transform clickDragerPanelPos1;
        private Transform clickDragerPanelPos2;

        private Vector3[] _clickDragerPanelPos0;
        private Vector3[] _clickDragerPanelPos1;
        private Vector3[] _clickDragerPanelPos2;


        private Transform clickDragerPanel0;
        private Transform clickDragerPanel1;
        private Transform clickDragerPanel2;
        private Empty4Raycast[] _clickDragerPanel0;
        private Empty4Raycast[] _clickDragerPanel1;
        private Empty4Raycast[] _clickDragerPanel2;

        private Vector3 _dragerStartPos;
        private GameObject _dragerStartPosObj;
        private ILDrager _ilDrager;

        private List<int> clickDragerList0;
        private List<int> clickDragerList1;
        private List<int> clickDragerList2;

        private ClickLeftEnum clickLeftEnum;

        private GameObject _maskTarget;

        private GameObject _pkPanel;
        private GameObject _pkPanel1;
        private Transform _huaK;

        private Transform _dragerPanelShadow;
        private GameObject[] _dragerPanelShadows;

        private Transform _faceStartPos;
        private Transform _faceEndPos;

        private int droperIndex;
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            Input.multiTouchEnabled = false;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");


            _dDD = curTrans.GetGameObject("dDD");



            _sDD = curTrans.GetGameObject("sDD");


            GameInit();
            FindInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();



            _dDD.Hide();



            _sDD.Hide();


            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

        }

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();

            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        //PlayCommonBgm(8);//ToDo...改BmgIndex
                        PlayBgm(0);
                        _startSpine.Hide();

                        //ToDo...
                        _sDD.Show();
                        BellSpeck(_sDD, 0, null, () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        }, RoleType.Adult);
                    });
                });
            });
        }
        private void FindInit()
        {
            _bg = _curGo.transform.Find("BG/bg").GetComponent<RawImage>();           
            _bg.texture = _bg.GetComponent<BellSprites>().texture[0];

            _rightPanelStartPos = _curGo.transform.Find("RightPanel/xzkPanelStartPos");
            _rightPanelEndPos = _curGo.transform.Find("RightPanel/xzkPanelEndPos");
            _rightPanel = _curGo.transform.Find("RightPanel/xzkPanel");

            _rightPanel.gameObject.Show();
            _rightPanel.position = _rightPanelStartPos.position;

            _presonPanel = _curGo.transform.Find("personPanel").gameObject;
            _presonPanel.Show();

            _personPanelBg = _curGo.transform.Find("personPanel/bg").gameObject;
            _personPanelBg.Show();

            _faceStartPos = _curGo.transform.Find("personPanel/faceStartPos");
            _faceEndPos= _curGo.transform.Find("personPanel/faceEndPos");

            _huaK = _curGo.transform.Find("personPanel/0");
            _huaK.position = _faceStartPos.position;
            _huaK.gameObject.Hide();

            _face = _curGo.transform.Find("personPanel/face").GetComponent<Image>();
            _face.transform.position= _faceStartPos.position;
            _face.gameObject.Show();
            _face.sprite = _face.GetComponent<BellSprites>().sprites[0];

            _droperPanel = _curGo.transform.Find("droperPanel");
            _droperPanels = new Transform[_droperPanel.childCount];
            for (int i = 0; i < _droperPanels.Length; i++)
            {
                _droperPanels[i] = _droperPanel.GetChild(i);
            }
            _ilDroper0 = _droperPanels[0].GetComponentsInChildren<ILDroper>(true);
            _ilDroper1 = _droperPanels[1].GetComponentsInChildren<ILDroper>(true);
            _ilDroper2 = _droperPanels[2].GetComponentsInChildren<ILDroper>(true);

            AddDroperEvent(_ilDroper0);
            AddDroperEvent(_ilDroper1);
            //AddDroperEvent(_ilDroper2);
            for (int i = 0; i < _ilDroper2.Length; i++)
            {
                _ilDroper2[i].index = 0;
                _ilDroper2[i].SetDropCallBack(OnAfter);
            }

            _spinePanel0 = _curGo.transform.Find("spinePanel/0");
            _spinePanel1 = _curGo.transform.Find("spinePanel/1");
            _spinePanels0 = new GameObject[_spinePanel0.childCount];
            _spinePanels1 = new GameObject[_spinePanel1.childCount];
            GetGameObject(_spinePanels0, _spinePanel0);
            GetGameObject(_spinePanels1, _spinePanel1);

            _starPanel0 = _curGo.transform.Find("spinePanel/0_0");
            _starPanel1 = _curGo.transform.Find("spinePanel/1_1");
            _starPanels0 = new GameObject[_starPanel0.childCount];
            _starPanels1 = new GameObject[_starPanel1.childCount];
            GetGameObject(_starPanels0, _starPanel0);
            GetGameObject(_starPanels1, _starPanel1);

            _clickSpinePanel = _curGo.transform.Find("clickSpinePanel/spine");
            _clickSpinePanel.position= _faceStartPos.position;           
            _clickSpinePanels = _clickSpinePanel.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clickSpinePanels.Length; i++)
            {
                Util.AddBtnClick(_clickSpinePanels[i].gameObject, ClickSpineEvent);
                SpineManager.instance.DoAnimation(_clickSpinePanels[i].transform.GetChild(0).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(_clickSpinePanels[i].transform.GetChild(1).gameObject, "kong", false);
                _clickSpinePanels[i].transform.GetChild(0).gameObject.Hide();
                _clickSpinePanels[i].transform.GetChild(1).gameObject.Hide();
            }
            _clickSpineILDragers= _clickSpinePanel.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < _clickSpineILDragers.Length; i++)
            {
                _clickSpineILDragers[i].index = i;
                _clickSpineILDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }
            _clickSpinePos = _curGo.transform.Find("clickSpinePanel/spinePos");
            _clickSpinePos.position = _faceStartPos.position;
            _clickSpinePoss = new Vector3[_clickSpinePos.childCount];
            for (int i = 0; i < _clickSpinePoss.Length; i++)
            {
                _clickSpinePoss[i] = _clickSpinePos.GetChild(i).position;
            }

            clickDragerPanelPos0 = _curGo.transform.Find("clickDragerPanelPos/mask_0");
            clickDragerPanelPos1 = _curGo.transform.Find("clickDragerPanelPos/mask_1");
            clickDragerPanelPos2 = _curGo.transform.Find("clickDragerPanelPos/mask_2");

            _clickDragerPanelPos0 = new Vector3[clickDragerPanelPos0.childCount];
            _clickDragerPanelPos1 = new Vector3[clickDragerPanelPos1.childCount];
            _clickDragerPanelPos2 = new Vector3[clickDragerPanelPos2.childCount];

            GetClickDragerPos(_clickDragerPanelPos0, clickDragerPanelPos0);
            GetClickDragerPos(_clickDragerPanelPos1, clickDragerPanelPos1);
            GetClickDragerPos(_clickDragerPanelPos2, clickDragerPanelPos2);


            clickDragerPanel0 = _curGo.transform.Find("clickDragerPanel/mask_0");
            clickDragerPanel1 = _curGo.transform.Find("clickDragerPanel/mask_1");
            clickDragerPanel2 = _curGo.transform.Find("clickDragerPanel/mask_2");

            _clickDragerPanel0 = clickDragerPanel0.GetComponentsInChildren<Empty4Raycast>(true);
            _clickDragerPanel1 = clickDragerPanel1.GetComponentsInChildren<Empty4Raycast>(true);
            _clickDragerPanel2 = clickDragerPanel2.GetComponentsInChildren<Empty4Raycast>(true);

            clickDragerList0 = new List<int>();
            clickDragerList1 = new List<int>();
            clickDragerList2 = new List<int>();
            temE4rs = new List<Empty4Raycast>(); ;
            SetList(clickDragerList0, _clickDragerPanel0);
            SetList(clickDragerList1, _clickDragerPanel1);
            SetList(clickDragerList2, _clickDragerPanel2);

            AddClickDragerEvent(_clickDragerPanel0, ClickDragerEvent);
            AddClickDragerEvent(_clickDragerPanel1, ClickDragerEvent);
            AddClickDragerEvent(_clickDragerPanel2, ClickDragerEvent);

            _dragerStartPosObj = _curGo.transform.Find("dragerPanel/0/startPos").gameObject;
            _dragerStartPos = _curGo.transform.Find("dragerPanel/0/startPos").position;
            _ilDrager = _curGo.transform.Find("dragerPanel/0/1/drager").GetComponent<ILDrager>();
            _ilDrager.index = -1;
            _ilDrager.SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            _ilDrager.transform.parent.gameObject.Hide();

            _maskTarget = _curGo.transform.Find("maskTarget").gameObject;
            _maskTarget.Show();

            _pkPanel = _curGo.transform.Find("pkPanel/0").gameObject;
            _pkPanel.Hide();

            _pkPanel1 = _curGo.transform.Find("pkPanel/1").gameObject;
            _pkPanel1.Hide();

            _dragerPanelShadow = _curGo.transform.Find("dragerPanelShadow/0");
            _dragerPanelShadows = new GameObject[_dragerPanelShadow.childCount];
            for (int i = 0; i < _dragerPanelShadows.Length; i++)
            {
                _dragerPanelShadows[i] = _dragerPanelShadow.GetChild(i).gameObject;
                _dragerPanelShadows[i].transform.position = _dragerStartPos;
            }

            clickDragerPanel0.gameObject.Hide();
            clickDragerPanel1.gameObject.Hide();
            clickDragerPanel2.gameObject.Hide();

        }
        private void AddDroperEvent(ILDroper[] ildropers)
        {
            for (int i = 0; i < ildropers.Length; i++)
            {
                ildropers[i].index = i;
                ildropers[i].SetDropCallBack(OnAfter);
            }
        }
        private int RandomLeftClickSprite(List<int> list)
        {
            int temp = -1;
            int tempNum = -1;
            temp = Random.Range(0, list.Count);
            tempNum = list[temp];
            list.Remove(tempNum);
            return tempNum;

        }
        private void AddClickDragerEvent(Empty4Raycast[] e4r, Action<GameObject> callBack = null)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                e4r[i].gameObject.Show();
                Util.AddBtnClick(e4r[i].gameObject, callBack);
            }
        }
        private void ShowClickDrager(Empty4Raycast[] e4r)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                e4r[i].gameObject.Show();                
            }
        }
        private void SetList(List<int> clickDragerList, Empty4Raycast[] e4r)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                clickDragerList.Add(i);
            }
        }
        private void SetLeftSprite(List<int> list, Empty4Raycast[] e4r)
        {
            int temp = -1;
            GameObject tempGo = null;
            for (int i = 0; i < e4r.Length; i++)
            {
                temp = RandomLeftClickSprite(list);
                tempGo = e4r[i].transform.GetChild(0).gameObject;
                tempGo.GetComponent<Image>().sprite = tempGo.GetComponent<BellSprites>().sprites[temp];
                tempGo.GetComponent<Image>().SetNativeSize();
            }
        }
        private void GetClickDragerPos(Vector3[] clickDragerPos, Transform clickDragerParent)
        {
            for (int i = 0; i < clickDragerPos.Length; i++)
            {
                clickDragerPos[i] = clickDragerParent.GetChild(i).position;
            }

        }
        private void GetGameObject(GameObject[] gos, Transform tran, Action callBack=null)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i] = tran.GetChild(i).gameObject;
                Delay(0, callBack);
                gos[i].Hide();
            }
        }
        private void ShowOrHideGameObject(GameObject[] gos, bool isShow)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i].SetActive(isShow);
            }
        }
        private void PlayKongAni(GameObject[] gos)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                SpineManager.instance.DoAnimation(gos[i], "kong", false);
            }
        }
        private void ResetGame()
        {
            droperIndex = -1;
            _rightPanel.gameObject.Show();

            _bg.texture = _bg.GetComponent<BellSprites>().texture[0];

            _personPanelBg.Show();
            _huaK.position = _faceStartPos.position;
            _huaK.gameObject.Hide();
            _face.gameObject.Show();
            _face.transform.position = _faceStartPos.position;
            _face.sprite = _face.GetComponent<BellSprites>().sprites[0];

            for (int i = 0; i < _droperPanels.Length; i++)
            {
                _droperPanels[i].gameObject.Hide();
            }
            _droperPanels[0].gameObject.Show();

            ShowOrHideGameObject(_spinePanels0, false);
            ShowOrHideGameObject(_spinePanels1, false);
            ShowOrHideGameObject(_starPanels0, false);
            ShowOrHideGameObject(_starPanels1, false);

            SetNormalClickDragerPos(_clickDragerPanelPos0, _clickDragerPanel0);
            SetNormalClickDragerPos(_clickDragerPanelPos1, _clickDragerPanel1);
            SetNormalClickDragerPos(_clickDragerPanelPos2, _clickDragerPanel2);

            SetNormalClickDragerPos(_clickSpinePoss, _clickSpinePanels);


            for (int i = 0; i < _droperPanels.Length; i++)
            {
                _droperPanels[i].gameObject.Hide();
            }
            _droperPanels[0].gameObject.Show();
            for (int i = 0; i < _clickSpinePanels.Length; i++)
            {
                
                SpineManager.instance.DoAnimation(_clickSpinePanels[i].transform.GetChild(0).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(_clickSpinePanels[i].transform.GetChild(1).gameObject, "kong", false);
                _clickSpinePanels[i].transform.GetChild(0).gameObject.Hide();
                _clickSpinePanels[i].transform.GetChild(1).gameObject.Hide();
            }

            for (int i = 0; i < _dragerPanelShadows.Length; i++)
            {                
                _dragerPanelShadows[i].transform.position = _dragerStartPos;
            }

            SetLeftSprite(clickDragerList0, _clickDragerPanel0);
            SetLeftSprite(clickDragerList1, _clickDragerPanel1);
            SetLeftSprite(clickDragerList2, _clickDragerPanel2);

            ShowClickDrager(_clickDragerPanel0);
            ShowClickDrager(_clickDragerPanel1);
            ShowClickDrager(_clickDragerPanel2);

            _clickSpinePanel.position = _faceStartPos.position;
            _clickSpinePanel.gameObject.Hide();

            clickDragerPanel1.gameObject.Hide();
            clickDragerPanel2.gameObject.Hide();

            _ilDrager.transform.parent.gameObject.Show();
            _ilDrager.transform.parent.position = _dragerStartPos;
            _ilDrager.transform.position = _dragerStartPosObj.transform.GetChild(0).position;

            _rightPanel.transform.DOMoveX(_rightPanelEndPos.position.x, 0.5f).SetEase(Ease.Flash).OnComplete(() => { clickDragerPanel0.gameObject.Show(); _maskTarget.Hide(); });

            clickLeftEnum = ClickLeftEnum.One;
            clickGo = null;

            IsActivedDroper(true, _ilDroper0);
            IsActivedDroper(false, _ilDroper1);
            IsActivedDroper(false, _ilDroper2);
            IsActivedClickILDrager(false, _clickSpineILDragers);
            IsCanRay(false, _clickSpinePanels);
            IsCanRay(true, _clickDragerPanel0);
            //_ilDroper2[0].isActived = false;
        }
        private void IsCanRay(bool isCanRay,Empty4Raycast[]e4r)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                e4r[i].raycastTarget = isCanRay;
            }
        }
        /// <summary>
        /// 激活或不激活ILDroper
        /// </summary>
        /// <param name="IsActived"></param>
        /// <param name="ilDroper"></param>
        private void IsActivedDroper(bool IsActived, ILDroper[] ilDroper) 
        {
            for (int i = 0; i < ilDroper.Length; i++)
            {
                ilDroper[i].isActived = IsActived;
            }
        }
        /// <summary>
        /// 激活或不激活ILDrager
        /// </summary>
        /// <param name="IsActived"></param>
        /// <param name="ilDroper"></param>
        private void IsActivedClickILDrager(bool IsActived, ILDrager[] ilDrager)
        {
            for (int i = 0; i < ilDrager.Length; i++)
            {
                ilDrager[i].isActived = IsActived;
            }
        }
        private void SetNormalClickDragerPos(Vector3[] pos, Empty4Raycast[] e4r)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                e4r[i].transform.position = pos[i];
            }
        }
        private void ShowDragerShadow(bool isShow)
        {
            for (int i = 0; i < _dragerPanelShadows.Length; i++)
            {
                _dragerPanelShadows[i].SetActive(isShow);
            }
        }
        /// <summary>
        /// 第三关卡点击事件
        /// </summary>
        /// <param name="go"></param>
        private void ClickSpineEvent(GameObject go)
        {
            //PlayBgm(4,false);
            //go.transform.SetAsLastSibling();
        }
        private Sprite tempSprite;
        private GameObject clickGo = null;
        /// <summary>
        /// 左侧点击事件
        /// </summary>
        /// <param name="go"></param>
        private void ClickDragerEvent(GameObject go)
        {
            if (clickGo != null && clickGo != go)
            {
                Debug.LogError("==============");
                clickGo.GetComponent<Empty4Raycast>().raycastTarget = true;
            }
            go.GetComponent<Empty4Raycast>().raycastTarget = false;
            clickGo = go;
            PlayBgm(4, false);           
            if (clickLeftEnum == ClickLeftEnum.One)
            {
                SetDragerSprite(go);
                _ilDrager.transform.GetChild(0).GetComponent<Image>().sprite = _ilDrager.transform.GetChild(0).GetComponent<BellSprites>().sprites[int.Parse(tempSprite.name.Substring(2)) - 1];
                _ilDrager.transform.parent.GetComponent<Image>().sprite = _ilDrager.transform.parent.GetComponent<BellSprites>().sprites[int.Parse(tempSprite.name.Substring(2)) - 1];                
            }
            else if (clickLeftEnum == ClickLeftEnum.Two)
            {
                SetDragerSprite(go);               
                _ilDrager.transform.GetChild(0).GetComponent<Image>().sprite = _ilDrager.transform.GetChild(0).GetComponent<BellSprites>().sprites[int.Parse(tempSprite.name.Substring(3)) + 5];
                _ilDrager.transform.parent.GetComponent<Image>().sprite = _ilDrager.transform.parent.GetComponent<BellSprites>().sprites[int.Parse(tempSprite.name.Substring(3)) + 11];
            }
            else if (clickLeftEnum == ClickLeftEnum.Three)
            {
                SetDragerSprite(go);               
                _ilDrager.transform.GetChild(0).GetComponent<Image>().sprite = _ilDrager.transform.GetChild(0).GetComponent<BellSprites>().sprites[int.Parse(tempSprite.name.Substring(3, 1)) + 9];
                _ilDrager.transform.parent.GetComponent<Image>().sprite = _ilDrager.transform.parent.GetComponent<BellSprites>().sprites[int.Parse(tempSprite.name.Substring(3, 1)) + 5];
            }
            _ilDrager.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            _ilDrager.transform.parent.GetComponent<Image>().SetNativeSize();
        }
        private void SetDragerSprite(GameObject go)
        {
            _ilDrager.transform.parent.gameObject.Show();
            tempSprite = go.transform.GetChild(0).GetComponent<Image>().sprite;
            _ilDrager.transform.parent.position = go.transform.position;          

        }
        private Vector3 _ilDragerParentPos;
        private Vector3 _ilDragerThreeFacePo;
        private void OnBeginDrag(Vector3 pos, int type, int index) 
        {
            _ilDragerParentPos = _ilDrager.transform.parent.position;
            if(clickLeftEnum == ClickLeftEnum.Three && index != -1)
            {
                _clickSpineILDragers[index].transform.SetAsLastSibling();
                _ilDragerThreeFacePo = _clickSpineILDragers[index].transform.position;
            }
            //_maskTarget.Show();
        }
        private void OnDrag(Vector3 pos, int type, int index) { }

        private float soundTime;
        private int dragerSum;
        private Vector3 _dragerThreePos;
        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            _maskTarget.Show();
            if (clickLeftEnum == ClickLeftEnum.Three)
            {
                _dragerThreePos = _ilDrager.transform.position;
            }
            _ilDrager.DoReset();
            Debug.LogError("droperIndex ====:" + droperIndex);
            if (isMatch)
            {
                if (clickLeftEnum == ClickLeftEnum.One)
                {
                    DragerFaction(1, _ilDroper0, _spinePanels0,_starPanels0, 3,4, _clickDragerPanel0, _clickDragerPanelPos0,() =>
                       {
                           _maskTarget.Hide();
                           if (dragerSum == 6)
                           {
                               temE4rs.Clear();
                               dragerSum = 0;
                               clickLeftEnum = ClickLeftEnum.Two;
                               clickGo = null;
                               Delay(1.0f, () =>
                               {                                  
                                   ShowDragerShadow(false);
                                   _face.sprite = _face.GetComponent<BellSprites>().sprites[1];
                                   _droperPanels[0].gameObject.Hide();
                                   _droperPanels[1].gameObject.Show();
                                   clickDragerPanel0.gameObject.Hide();
                                   clickDragerPanel1.gameObject.Show();
                                   PlayKongAni(_spinePanels0);
                                   PlayKongAni(_starPanels0);
                                   ShowOrHideGameObject(_spinePanels0, false);
                                   ShowOrHideGameObject(_starPanels0, false);

                                   IsActivedDroper(false, _ilDroper0);
                                   IsCanRay(true, _clickDragerPanel0);
                                   IsCanRay(true, _clickDragerPanel1);
                                   IsActivedDroper(true, _ilDroper1);
                               });
                           }
                       }                       
                     );
                }
                else if (clickLeftEnum == ClickLeftEnum.Two)
                {
                    DragerFaction(14, _ilDroper1, _spinePanels1,_starPanels1, 1, 2, _clickDragerPanel1, _clickDragerPanelPos1,() =>
                       {
                           _maskTarget.Hide();
                           if (dragerSum == 4)
                           {
                               _maskTarget.Show();
                               temE4rs.Clear();
                               dragerSum = 0;
                               clickLeftEnum = ClickLeftEnum.Three;
                               clickGo = null;
                               Delay(1.0f, () =>
                               {
                                   ShowDragerShadow(false);
                                   _face.sprite = _face.GetComponent<BellSprites>().sprites[2];
                                   _presonPanel.Hide();
                                   _droperPanels[1].gameObject.Hide();                                  
                                   clickDragerPanel1.gameObject.Hide();
                                   PlayKongAni(_spinePanels1);
                                   PlayKongAni(_starPanels1);
                                   ShowOrHideGameObject(_spinePanels1, false);
                                   ShowOrHideGameObject(_starPanels1, false);

                                   _rightPanel.position = _rightPanelStartPos.position;

                                   PlayBgm(3, false);
                                   _pkPanel.Show();
                                   _pkPanel.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                   SpineManager.instance.DoAnimation(_pkPanel, "pk", false, () => 
                                   {
                                       SpineManager.instance.DoAnimation(_pkPanel, "pk2", true);
                                       Delay(2.0f, () => 
                                       {                                          
                                           _pkPanel.Hide();
                                           _presonPanel.Show();
                                           _droperPanels[2].gameObject.Show();
                                           _clickSpinePanel.gameObject.Show();
                                           SetNormalClickDragerPos(_clickSpinePoss, _clickSpinePanels);
                                           _sDD.Show();
                                           BellSpeck(_sDD, 2, null, () =>
                                           {
                                               _sDD.Hide();
                                               IsActivedDroper(false, _ilDroper1);
                                               IsCanRay(false, _clickDragerPanel1);
                                               IsCanRay(true, _clickDragerPanel2);
                                               IsActivedDroper(true, _ilDroper2);
                                               _maskTarget.Hide();
                                           }, RoleType.Adult);
                                           //soundTime = PlaySound(2, false);
                                           _rightPanel.transform.DOMoveX(_rightPanelEndPos.position.x, 0.5f).SetEase(Ease.Flash).OnComplete(() => { clickDragerPanel2.gameObject.Show(); });
                                           //Delay(soundTime, () => 
                                           //{
                                           //    _sDD.Hide();
                                           //    IsActivedDroper(false, _ilDroper1);
                                           //    IsCanRay(false, _clickDragerPanel1);
                                           //    IsCanRay(true, _clickDragerPanel2);
                                           //    IsActivedDroper(true, _ilDroper2);
                                           //    _maskTarget.Hide();
                                           //});                                           
                                       });
                                   });
                               });
                           }
                      }                      
                      );
                }
                else if(clickLeftEnum == ClickLeftEnum.Three)
                {
                    dragerThreeTime = 0;
                    if (index < 0)
                    {
                        clickGo.Hide();
                        dragerSum++;
                        _ilDrager.transform.parent.gameObject.Hide();
                        DragerThreeNum = int.Parse(_ilDrager.transform.parent.GetComponent<Image>().sprite.name.Substring(5, 1));
                        //Debug.LogError("DragerThreeNum:" + DragerThreeNum);
                        _clickSpinePanels[DragerThreeNum - 1].gameObject.Show();
                        _clickSpinePanels[DragerThreeNum - 1].transform.SetAsLastSibling();
                        _clickSpinePanels[DragerThreeNum - 1].transform.position = _dragerThreePos;

                        PlayBgm(1, false);
                        _clickSpinePanels[DragerThreeNum - 1].transform.GetChild(0).gameObject.Show();
                        _clickSpinePanels[DragerThreeNum - 1].transform.GetChild(1).gameObject.Show();
                        _clickSpinePanels[DragerThreeNum - 1].transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        _clickSpinePanels[DragerThreeNum - 1].transform.GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);                       
                        SpineManager.instance.DoAnimation(_clickSpinePanels[DragerThreeNum - 1].transform.GetChild(1).gameObject, "star", false);
                        SpineManager.instance.DoAnimation(_clickSpinePanels[DragerThreeNum - 1].transform.GetChild(0).gameObject, "x" + (DragerThreeNum + 6), false);

                        _clickSpineILDragers[DragerThreeNum - 1].isActived = true;//激活选中
                        _clickSpinePanels[DragerThreeNum - 1].raycastTarget = true;
                        soundTime = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                        dragerThreeTime = soundTime;
                        Delay(soundTime, () =>
                        {
                            _maskTarget.Hide();
                        });

                        if (dragerSum <= 3)
                        {
                            SliderDrager(clickGo, _clickDragerPanel2, _clickDragerPanelPos2);

                        }
                        else
                        {                           
                            _dragerPanelShadows[dragerSum - 4].GetComponent<Image>().sprite = _ilDrager.transform.parent.GetComponent<Image>().sprite;
                            _dragerPanelShadows[dragerSum - 4].GetComponent<Image>().SetNativeSize();
                            _dragerPanelShadows[dragerSum - 4].SetActive(true);
                            _dragerPanelShadows[dragerSum - 4].transform.position = _ilDragerParentPos;
                        }

                        if (dragerSum == 6)
                        {
                            _maskTarget.Show();
                            ShowDragerShadow(false);
                            temE4rs.Clear();
                            dragerSum = 0;
                            clickLeftEnum = ClickLeftEnum.One;                            
                            Delay(soundTime, () =>
                            {
                                IsCanRay(false, _clickDragerPanel2);
                                IsActivedClickILDrager(false, _clickSpineILDragers);
                                IsActivedDroper(false, _ilDroper2);

                                _rightPanel.position = _rightPanelStartPos.position;
                                clickDragerPanel2.gameObject.Hide();
                                _pkPanel1.Show();
                                SpineManager.instance.DoAnimation(_pkPanel1, "light", false, () =>
                                {
                                    Delay(1.0f, () =>
                                    {
                                        _huaK.gameObject.Show();
                                        _pkPanel1.Hide();
                                        _personPanelBg.gameObject.Hide();
                                        _bg.texture = _bg.GetComponent<BellSprites>().texture[1];
                                        _huaK.transform.DOMove(_faceEndPos.position, 0.5f).SetEase(Ease.Linear);
                                        _face.transform.DOMove(_faceEndPos.position, 0.5f).SetEase(Ease.Linear);
                                        _clickSpinePanel.transform.DOMove(_faceEndPos.position, 0.5f).SetEase(Ease.Linear);
                                        Delay(2.0f, () => { GameSuccess(); });
                                    });
                                });
                            });
                        }
                    }
                    else
                    {
                       
                        if(soundTime!= dragerThreeTime)
                        {
                            _maskTarget.Hide();
                            dragerThreeTime = 0;
                        }
                        else
                        {
                            dragerThreeTime = 0;
                            Delay(soundTime, () =>
                            {
                                _maskTarget.Hide();                                
                            });
                        }
                    }

                }
            }
            else
            {
                PlayBgm(2, false);
                if (index < 0)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                    soundTime = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                    Delay(soundTime, () => { _maskTarget.Hide(); });
                }
                else
                {
                    _maskTarget.Hide();
                    _clickSpineILDragers[index].DoReset();
                }

                if (clickLeftEnum == ClickLeftEnum.Three && index != -1)
                {
                    _clickSpineILDragers[index].transform.position = _ilDragerThreeFacePo;
                }
            }
        }
        private float dragerThreeTime;
        private int DragerThreeNum;        
        private bool OnAfter(int dragType, int index, int dropType)
        {

            droperIndex = index;
            return true;
        }
        private void DragerFaction(int index, ILDroper[] ilDroper, GameObject[] spinePanel,GameObject[]starPanel, int dragerSumIndex,int dragerSumIndex0,Empty4Raycast[]e4r,Vector3[] pos, Action callBack = null)
        {            
            if (_ilDrager.transform.GetChild(0).GetComponent<Image>().sprite.name == ilDroper[droperIndex].gameObject.name)
            {
                dragerSum++;
                clickGo.Hide();
                //Delay(0, callBack2);                                 
                if (dragerSum <= dragerSumIndex)
                {
                    _ilDrager.transform.parent.gameObject.Hide();
                    SliderDrager(clickGo, e4r, pos);
                }
                else if (dragerSum > dragerSumIndex)
                {
                    _dragerPanelShadows[dragerSum - dragerSumIndex0].transform.position = _ilDragerParentPos;
                    _dragerPanelShadows[dragerSum - dragerSumIndex0].GetComponent<Image>().sprite = _ilDrager.transform.parent.GetComponent<Image>().sprite;                                       
                    _dragerPanelShadows[dragerSum - dragerSumIndex0].GetComponent<Image>().SetNativeSize();
                    _dragerPanelShadows[dragerSum - dragerSumIndex0].SetActive(true);
                    _ilDrager.transform.parent.gameObject.Hide();
                }               

                spinePanel[droperIndex].Show();
                spinePanel[droperIndex].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                starPanel[droperIndex].Show();
                starPanel[droperIndex].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                soundTime = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                PlayBgm(1, false);
                SpineManager.instance.DoAnimation(starPanel[droperIndex], "star", false);               
                SpineManager.instance.DoAnimation(spinePanel[droperIndex], "x" + (droperIndex + index).ToString(), false);
                Delay(soundTime, callBack);                
            }
            else
            {
                PlayBgm(2, false);
                soundTime = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                Delay(soundTime, () => { _maskTarget.Hide(); });
            }
        }
        List<Empty4Raycast> temE4rs = null;
        private void SliderDrager(GameObject go, Empty4Raycast[] e4r, Vector3[] pos)
        {
            int tem = go.transform.GetSiblingIndex();            
            Empty4Raycast temE4r = go.GetComponent<Empty4Raycast>();
            if (temE4rs.Count>0)
            {
                temE4rs.Remove(temE4r);
            }
            else
            {
                for (int i = 0; i < e4r.Length; i++)
                {
                    if (i != tem)
                    {
                        temE4rs.Add(e4r[i]);
                    }
                }
            }
           
            for (int i = 0; i < temE4rs.Count; i++)
            {
                temE4rs[i].transform.DOMove(pos[i], 0.5f);
            }

        }
        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sDD, 1, null, () =>
                    {
                        _sDD.SetActive(false);
                        _mask.SetActive(false);
                        StartGame();
                    }, RoleType.Adult);
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
            //_mask.Hide();
            //测试代码记得删
            //Delay(4,GameSuccess);
            ResetGame();
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
            PlaySpine(_replaySpine, "fh2", () =>
            {
                AddEvent(_replaySpine, (go) =>
                {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        PlayBgm(0);
                        _okSpine.Hide();
                        GameInit();
                        //ToDo...	
                        SetList(clickDragerList0, _clickDragerPanel0);
                        SetList(clickDragerList1, _clickDragerPanel1);
                        SetList(clickDragerList2, _clickDragerPanel2);
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                    StopAudio(SoundManager.SoundType.BGM);
                    StopAudio(SoundManager.SoundType.VOICE);
                    PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dDD.Show();
                        BellSpeck(_dDD, 3,null,null,RoleType.Adult);
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
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });
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

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
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
