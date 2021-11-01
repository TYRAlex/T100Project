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

    public class TD3463Part1
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
        private GameObject _dDD;
        private GameObject _sDD;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


        private GameObject bj;
        private GameObject ban;
        private GameObject jfb;
        private GameObject grade;
        private GameObject yu;
        private GameObject xem;
        private GameObject ban2;
       


        private Transform buble;
        private Transform buble2;
        private Transform bublePos;
        private Transform buble2Pos;
        private Transform dragPanel;
        private Transform dragPanelPos;
        private Transform dropPic;
        private Transform jdt;
        private Transform dropPicEffect;
        private Transform dropXuXian;
        private Transform star;

        private mILDrager[] dragers;
        private mILDroper[] dropers;

        private GameObject[] xuXian;

        private Image _jdt2;   //进度条

        private bool _isPlaying;

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


            _dDD = curTrans.GetGameObject("dDD");



            _sDD = curTrans.GetGameObject("sDD");

            bj = curTrans.GetGameObject("bj");
            ban = curTrans.GetGameObject("ban");
            ban2 = curTrans.GetGameObject("ban2");
            jfb = curTrans.GetGameObject("jfb");
            grade = curTrans.GetGameObject("jfb/grade");
        
            xem = curTrans.GetGameObject("xem");


            buble = curTrans.Find("buble");
            star = curTrans.Find("star");
            buble2 = curTrans.Find("buble2");
            dropXuXian = curTrans.Find("dropXuXian");

            for (int i = 0; i < buble.childCount; i++)
            {
                Util.AddBtnClick(buble.GetChild(i).GetChild(0).gameObject, ClickCorrect);
            }
            for (int i = 0; i < buble2.childCount; i++)
            {
                Util.AddBtnClick(buble2.GetChild(i).GetChild(0).gameObject, ClickFalse);
            }

            bublePos = curTrans.Find("bublePos");
            buble2Pos = curTrans.Find("buble2Pos");
            dragPanel = curTrans.Find("dragPanel");
            dragPanelPos = curTrans.Find("dragPanelPos");
            dropPic = curTrans.Find("dropPic");
            jdt = curTrans.Find("jdt");
            dropPicEffect = curTrans.Find("dropPicEffect");

            yu = curTrans.GetGameObject("yu");

            dragers = dragPanel.GetComponentsInChildren<mILDrager>(true);
          
            dragers = SortDrager(dragers);

            dropers = dropPic.GetComponentsInChildren<mILDroper>(true);
            dropers = SortDroper(dropers);
            xuXian = new GameObject[6];
            _jdt2 = curTrans.GetImage("jdt/2");

            GameInit();
            GameStart();
        }
        int gradeNum;

        private void DelayFor(Transform parent, float delay, Action<RectTransform, int> callBack)
        {
            _mono.StartCoroutine(IEDelayFor(parent, delay, callBack));
        }
        IEnumerator IEDelayFor(Transform parent, float delay, Action<RectTransform, int> callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {

                var rect = parent.GetChild(i).GetRectTransform();
                callBack?.Invoke(rect, i);
                yield return new WaitForSeconds(delay);
            }
        }

        void InitData()
        {
            for (int i = 0; i < dropXuXian.childCount; i++)
            {
                xuXian[i] = dropXuXian.GetChild(i).gameObject;
            }
            yu.transform.GetRectTransform().anchoredPosition = new Vector2(0, -1080);
            xuXian[3] = dropPic.Find("x4").gameObject;
            xuXian[4] = dropPic.Find("x5").gameObject;
            xuXian[5] = dropPic.Find("x6").gameObject;

            dropers[6].enabled = true;

            dropers[2].enabled = true;

            yuNum = "";
            jdtNum = 0;
            _isPlaying = true;
            _canClickBuble = false;
            gradeNum = 0;
            SwitchGrade(0);
          
            InitSpine(bj, "animation", true);
            InitSpine(ban, "animation");
            InitSpine(xem, "");
            for (int i = 0; i < dropers.Length; i++)
            {
                dropers[i].gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
            ban2.SetActive(false);
            jfb.SetActive(true);
            dropXuXian.gameObject.SetActive(false);
            buble.gameObject.SetActive(true);
            buble2.gameObject.SetActive(true);
            ban.SetActive(true);

            for (int i = 0; i < buble.childCount; i++)
            {
                GameObject obj = buble.GetChild(i).gameObject;
                InitSpine(obj, obj.name + 1);               
                obj.transform.GetChild(0).GetComponent<Empty4Raycast>().enabled = true;
                Transform obj1 = buble.GetChild(i);
                Transform obj2 = bublePos.GetChild(i);
                InitRect(obj1.GetRectTransform(), obj2.GetRectTransform());
            }

            for (int i = 0; i < buble2.childCount; i++)
            {
                GameObject obj = buble2.GetChild(i).gameObject;
                InitSpine(obj, obj.name + 1, true);
                obj.transform.GetChild(0).GetComponent<Empty4Raycast>().enabled = true;
                Transform obj3 = buble2.GetChild(i);
                Transform obj4 = buble2Pos.GetChild(i);
                InitRect(obj3.GetRectTransform(), obj4.GetRectTransform());
            }

            DelayFor(buble, 0.3f, (rect, index) => {
                var go = rect.gameObject;
                var name = go.name + "1";
                PlaySpine(go, name, null, true);
            });

            DelayFor(buble2, 0.3f, (rect, index) => {

                var go = rect.gameObject;
                var name = go.name + "1";
                PlaySpine(go, name, null, true);
            });

            yu.SetActive(true);
            InitSpine(yu, "2");
            dragPanel.gameObject.SetActive(false);

            for (int i = 0; i <dragers.Length; i++)
            {
                InitRect(dragers[i].transform.GetRectTransform(), dragPanelPos.GetChild(i).GetRectTransform());
                dragers[i].gameObject.SetActive(false);
            }
            dropPic.gameObject.SetActive(false);
            InitSpine(dropPic.GetGameObject("body"), "yuxuxian7");
            jdt.gameObject.SetActive(false);


            for (int i = 0; i < dropPicEffect.childCount; i++)
            {
                if(i!=3&& i != 4 && i != 5 )
                    InitSpine(dropPicEffect.GetChild(i).gameObject, "2");
            }

            for (int i = 0; i < star.childCount; i++)
            {
                InitSpine(star.GetChild(i).gameObject, "");

            }

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
        }

        void GameInit()
        {
            DOTween.KillAll();
            Input.multiTouchEnabled = false;
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();

            _dDD.Hide();
            _sDD.Hide();

            _jdt2.fillAmount = 0;
            DelayFor(buble, 0.3f, (rect, index) => {


                var go = rect.gameObject;
                var name = go.name + "1";
                PlaySpine(go, name, null, true);

            });

            DelayFor(buble2, 0.3f, (rect, index) => {

                var go = rect.gameObject;
                var name = go.name + "1";
                PlaySpine(go, name, null, true);

            });

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
                        PlayBgm(0);                   
                        _startSpine.Hide();
                        _sDD.Show();
                        BellSpeck(_sDD, 0, null, ShowVoiceBtn);                                             
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
                    BellSpeck(_sDD, 1, null, () => { _sDD.Hide(); StartGame(); });
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑       
        #region spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }
        #endregion

        #region 大小和位置初始化
        void InitRect(RectTransform rect1, RectTransform initPos)
        {
            rect1.anchoredPosition = initPos.anchoredPosition;
            rect1.localScale = initPos.localScale;

        }

        #endregion

        #region 点击正确
        bool _canClickBuble;
        void ClickCorrect(GameObject obj)
        {
            Debug.Log(_canClickBuble);
            if (!_canClickBuble)
                return;
            _canClickBuble = false;

            obj.GetComponent<Empty4Raycast>().enabled = false;
            GameObject obj2 = obj.transform.parent.gameObject;
            PlayVoice(1);
            SpineManager.instance.DoAnimation(obj2, obj2.name + 2, false, () =>
            {
                SpineManager.instance.DoAnimation(obj2, "2", false);
                Delay(0f, () =>
                {
                    PlayVoice(2);
                    obj2.transform.GetRectTransform().DOScale(new Vector2(1, 1), 0);
                    obj2.transform.GetRectTransform().DOAnchorPos(new Vector2(0, 0), 0);
                });
                ++gradeNum;

                SwitchGrade(gradeNum);

                Delay(0f,()=>{ SpineManager.instance.DoAnimation(star.Find(obj2.name).gameObject, "star6", false); });
                    SpineManager.instance.DoAnimation(obj2, obj.name, false, () =>
                    {
                        
                        if (gradeNum == 5)
                        {
                            Delay(1.3f,()=> {
                                _mask.Show();_sDD.Show();
                                BellSpeck(_sDD, 2, null, () => { _mask.Hide(); _sDD.Hide(); });
                                SwitchLevel(); 
                            });

                            return;
                        }

                        _canClickBuble = true;
                    });
                

            });

        }
        #endregion

        #region 点击错误
        void ClickFalse(GameObject obj)
        {
            Debug.Log(_canClickBuble);
            if (!_canClickBuble)
                return;
            _canClickBuble = false;
            PlayCommonSound(5);
            GameObject obj2 = obj.transform.parent.gameObject;

            float time = PlaySpine(obj2, obj2.name + 2,()=> { PlaySpine(obj2, obj2.name + 1,null,true); });
            //float time = SpineManager.instance.DoAnimation(obj2, obj2.name + 2, false, () =>
            //{
            //  SpineManager.instance.DoAnimation(obj2, obj2.name + 1, true);              
            //});
            Delay(time, () => { _canClickBuble = true; });

        }

        #endregion

        #region 第一关成绩
        void SwitchGrade(int num)
        {
            grade.GetComponent<Image>().SetNativeSize();
            grade.GetComponent<Image>().sprite = grade.GetComponent<BellSprites>().sprites[num];
            grade.GetComponent<Image>().SetNativeSize();


        }
        #endregion

        #region 切换关卡
        void SwitchLevel()
        {
            
            dragers[2].enabled = true;
          //  dragers[5].enabled = true;
            dropXuXian.gameObject.SetActive(true);
            for (int i = 0; i <xuXian.Length; i++)
            {
                xuXian[i].SetActive(true);
            }
            SpineManager.instance.DoAnimation(ban, "2", false);        
            jfb.SetActive(false);
            ban2.SetActive(true);

            for (int i = 0; i < buble.childCount; i++)
            {
                GameObject obj = buble.GetChild(i).gameObject;
                InitSpine(obj, "2", false);
                obj.transform.GetChild(0).GetComponent<Empty4Raycast>().enabled = false;
            }
            for (int i = 0; i < buble2.childCount; i++)
            {
                GameObject obj = buble2.GetChild(i).gameObject;
                InitSpine(obj, "2", false);
                obj.transform.GetChild(0).GetComponent<Empty4Raycast>().enabled = false;
            }

            dragPanel.gameObject.SetActive(true);
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].transform.gameObject.SetActive(true);
                dragers[i].SetDragCallback(DragBegin, null, DragEnd);

            }

            dropPic.gameObject.SetActive(true);
            for (int i = 0; i < xuXian.Length; i++)
            {
                xuXian[i].SetActive(true);
            }
            jdt.gameObject.SetActive(true);
          
            for (int i = 0; i < dropers.Length; i++)
            {
                dropers[i].SetDropCallBack(DoAfter);
            }

            for (int i = 0; i < dropPicEffect.childCount; i++)
            {
                InitSpine(dropPicEffect.GetChild(i).gameObject,"");
               
            }
            InitSpine(xem, "xem1", true);

        }
        #endregion

        #region 加拖拽
        string yuNum;
        int jdtNum;
        void DragBegin(Vector3 position, int type, int index)
        {
            dragers[index].transform.SetAsLastSibling();




        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {

            if (isMatch)
            {
                PlayVoice(2);
                jdtNum++;
                dragers[index].gameObject.SetActive(false);
                GameObject obj = dropPicEffect.Find(index + 1+"").gameObject;
                SpineManager.instance.DoAnimation(obj,obj.transform.GetChild(0).name,false);
                var endValue = _jdt2.fillAmount + 0.2f;
                _jdt2.DOFillAmount(endValue, 0.2f);

                dropers[index].gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);


                if (index == 0)
                {
                    dropers[6].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    xuXian[0].SetActive(false);
                    xuXian[5].SetActive(false);

                }
                else if (index == 2)
                {
                  //  dragers[5].enabled = false;
                    xuXian[2].SetActive(false);
                }
                else if (index == 5)
                {
                    xuXian[2].SetActive(false);
                    dragers[2].enabled = false;

                }
            
                else
                {
                    xuXian[index].SetActive(false);
                }
                   

                if (index == 2)
                {
                    dropers[6].enabled = false;
                    yuNum = "";

                }
                else if (index == 5)
                {
                    dropers[2].enabled = false;
                    yuNum = "2";
                }
                if (jdtNum == 5) {
                    PlayVoice(3);
                    SpineManager.instance.DoAnimation(xem, "zhadan", false,()=> {
                        SpineManager.instance.DoAnimation(yu, "yu" + yuNum, true);

                        for (int i = 0; i < dropers.Length; i++)
                        {
                            dropers[i].gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                        }
                        InitSpine(dropPic.Find("body").gameObject, "");
                        dragPanel.gameObject.SetActive(false);
                       
                        ban2.SetActive(false);
                        jdt.gameObject.SetActive(false);

                        
                        Delay(0.2f,()=> {
                            yu.transform.GetRectTransform().DOAnchorPosX(-650,4f);
                        });
                      
                        Delay(4f,()=> { GameSuccess(); });
                    });
                }
            }
            else
            {
                /* for (int i = 0; i < dragPanel.childCount; i++)
                 {
                     InitRect(dragPanel.GetChild(i).GetRectTransform(), dragPanelPos.GetChild(i).GetRectTransform());
                 }*/
                PlayCommonSound(5);
                InitRect(dragers[index].transform.GetRectTransform(), dragPanelPos.GetChild(index).GetRectTransform());
            }
        }
        bool DoAfter(int dragType, int index, int dropType)
        {


            return dragers[index].drops[0].GetComponent<PolygonCollider2D>().OverlapPoint(Input.mousePosition);
        }
        mILDrager[] SortDrager(mILDrager[] list)
        {
            int n = list.Length;
            mILDrager[] ret = new mILDrager[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }
        mILDroper[] SortDroper(mILDroper[] list)
        {
            int n = list.Length;
            mILDroper[] ret = new mILDroper[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }
        #endregion

   
        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();

            Delay(0.5f, ()=>
            {
                _canClickBuble = true;
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
                        _okSpine.Hide();
                       
                        GameInit();                     			
                        StartGame();
                        PlayBgm(0);
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();                                   
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dDD.Show();
                        BellSpeck(_dDD, 3);                     

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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
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
