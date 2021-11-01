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

    public class TD6762Part1
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
        private GameObject pzPos;



        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


        private Transform px;
        // private Transform py;
        private Transform pz;
        private GameObject DD;
        private GameObject xem;
        private GameObject jdt;
        private GameObject dpz;
        private GameObject dzxg;
        private Transform BG;
        private Transform bg2;
        private Transform dpzPos;
        private mILDrager[] milDragers;

        private mILDroper[] milPxDropers;
        private mILDroper[] milPyDropers;
        private mILDroper[] milTotalDropers;
        private Image mask_image;

        private int pxIndex;
        private int pyIndex;








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
            mask_image = _mask.GetComponent<Image>();
            px = curTrans.Find("px");
            // py = curTrans.Find("py");
            pz = curTrans.Find("pz");
            xem = curTrans.Find("xem").gameObject;
            jdt = curTrans.Find("jdt").gameObject;
            DD = curTrans.Find("DD").gameObject;
            dpz = curTrans.Find("dpz").gameObject;
            BG = curTrans.Find("BG");
            pzPos = curTrans.Find("pzPos").gameObject;
            dpzPos = curTrans.Find("dpzPos");
            dzxg = curTrans.Find("dzxg").gameObject;
            bg2 = curTrans.Find("bg2").transform;
            milTotalDropers = px.GetComponentsInChildren<mILDroper>();

            milDragers = pz.GetComponentsInChildren<mILDrager>();
            milDragers = Sort(milDragers);
            milPxDropers = new mILDroper[19];
            milPyDropers = new mILDroper[7];


            pxIndex = 0;
            pyIndex = 0;
            for (int i = 1; i < px.childCount; i++)
            {

                if (px.GetChild(i).GetChild(0).GetComponent<mILDroper>().dropType < 10)
                    milPyDropers[pyIndex++] = px.GetChild(i).GetChild(0).GetComponent<mILDroper>();

            }
            for (int i = 1; i < px.childCount; i++)
            {

                if (px.GetChild(i).GetChild(0).GetComponent<mILDroper>().dropType > 10)
                    milPxDropers[pxIndex++] = px.GetChild(i).GetChild(0).GetComponent<mILDroper>();

            }

            milPxDropers = SortDrop(milPxDropers);

            milPyDropers = SortPyDrop(milPyDropers);





            GameInit();
            GameStart();
        }
        #region 层级初始化
        void initSlibe(){
            //排层级
            milTotalDropers = px.GetComponentsInChildren<mILDroper>();
            milTotalDropers = SortTotalDrop(milTotalDropers);
            for (int i = 0; i < milTotalDropers.Length; i++)
            {
                milTotalDropers[i].transform.parent.SetAsLastSibling();
            }
        }
        #endregion

        void InitData()
        {

            //排层级
            milTotalDropers = px.GetComponentsInChildren<mILDroper>();
            milTotalDropers= SortTotalDrop(milTotalDropers);
            for (int i = 0; i < milTotalDropers.Length; i++)
            {
                milTotalDropers[i].transform.parent.SetAsLastSibling();
            }

            mask_image.DOColor(new Vector4(0, 0, 0, 0.78f), 0f);
            _isPlaying = true;
            mask_image.color = new Color(0, 0, 0, 0);
            for (int i = 0; i < milPxDropers.Length; i++)
            {
                milPxDropers[i].transform.parent.gameObject.GetComponent<SkeletonGraphic>().freeze = true;
                //px.GetChild(i).GetChild(0).gameObject.GetComponent<mILDroper>().SetDropCallBack(DoAfter);
              milPxDropers[i].SetDropCallBack(DoAfter);
            }

            for (int i = 0; i <milPyDropers.Length; i++)
            {
                milPyDropers[i].transform.parent.gameObject.GetComponent<SkeletonGraphic>().freeze = true;
                //py.GetChild(i).GetChild(0).gameObject.GetComponent<mILDroper>().SetDropCallBack(DoAfter);
                milPyDropers[i].SetDropCallBack(DoAfter);
            }
            for (int i = 0; i < pz.childCount; i++)
            {
                SpineManager.instance.DoAnimation(pz.GetChild(i).GetChild(0).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(pz.GetChild(i).GetChild(1).gameObject, "kong", false);
                pz.GetChild(i).GetComponent<mILDrager>().enabled = false;
                pz.GetChild(i).GetRectTransform().DOAnchorPos(pzPos.transform.GetRectTransform().anchoredPosition, 0f);
                pz.GetChild(i).gameObject.SetActive(false);
            }

            dpz.SetActive(false);
            dpz.transform.DOMove(dpzPos.position, 0f);
          //  dpz.transform.GetRectTransform().DOAnchorPos(new Vector2(-46,470), 0f);
            SpineManager.instance.DoAnimation(dpz, "kong", false);
            dpz.transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            dpz.transform.GetRectTransform().rotation = Quaternion.Euler(0,0,0);

            SpineManager.instance.DoAnimation(DD, "kong", false);
            SpineManager.instance.DoAnimation(dzxg, "kong", false);
           
            bg2.gameObject.SetActive(false);
            bg2.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            InitSpine(xem, "xem1", true);
            InitSpine(bg2.GetChild(0).gameObject,"");
            jdt.GetComponent<SkeletonGraphic>().Initialize(true);
            jdt.GetComponent<SkeletonGraphic>().freeze = true;
            isXemPlay = false;


            curPz = 1;
            hasBeginDrag = false;

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
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
        mILDrager[] Sort(mILDrager[] list)
        {
            int n = list.Length;
            mILDrager[] ret = new mILDrager[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }

        mILDroper[] SortTotalDrop(mILDroper[] list)
        {
            int n = list.Length;
            mILDroper[] ret = new mILDroper[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }
        mILDroper[] SortDrop(mILDroper[] list)
        {
            int n = list.Length;
            mILDroper[] ret = new mILDroper[n];

            for (int i = 0; i < n; ++i)
            {
                Debug.Log(list[i].dropType - 12);
                ret[list[i].dropType - 12] = list[i];
            }

            return ret;
        }
        mILDroper[] SortPyDrop(mILDroper[] list)
        {
            int n = list.Length;
            mILDroper[] ret = new mILDroper[n];

            for (int i = 0; i < n; ++i)
            {
                //Debug.Log(list[i].dropType - 10);
                ret[list[i].dropType - 1] = list[i];
            }

            return ret;
        }
        int curPz;
        /// <summary>
        /// 生成瓶子
        /// </summary>
        void PzInstance(int curPz)
        {
            milDragers[curPz - 1].gameObject.SetActive(true);

            hasBeginDrag = false;
            GameObject obj = milDragers[curPz - 1].transform.GetChild(0).gameObject;
            GameObject obj2 = milDragers[curPz - 1].transform.GetChild(1).gameObject;
            PlayVoice(0);
            SpineManager.instance.DoAnimation(xem, "xem2", false, () =>
            {
                SpineManager.instance.DoAnimation(xem, "xem1", true);
            });
            float time = SpineManager.instance.DoAnimation(obj2, obj2.name, true);
            Delay(time, () =>
            {
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {

                    SpineManager.instance.DoAnimation(obj, obj.name + 3, false, () =>
                    {
                        //加回调和拖动
                        milDragers[curPz - 1].transform.GetComponent<mILDrager>().enabled = true;
                        milDragers[curPz - 1].transform.gameObject.GetComponent<mILDrager>().SetDragCallback(DragBegin, null, DragEnd);
                        milDragers[curPz - 1].transform.SetAsLastSibling();
                        SpineManager.instance.DoAnimation(DD, "DD", false, () =>
                        {
                           /* Delay(time2, () =>
                            {

                                //加回调和拖动
                                milDragers[curPz - 1].transform.GetComponent<mILDrager>().enabled = true;
                                milDragers[curPz - 1].transform.gameObject.GetComponent<mILDrager>().SetDragCallback(DragBegin, null, DragEnd);
                                milDragers[curPz - 1].transform.SetAsLastSibling();
                            });*/
                        });
                    });

                });


            });
        }


        #region 进度条
        IEnumerator JDTMove()
        {
            jdt.GetComponent<SkeletonGraphic>().freeze = false;
            yield return new WaitForSeconds(1);
            jdt.GetComponent<SkeletonGraphic>().freeze = true;
        }


        #endregion

        #region 瓶子掉落
        bool isXemPlay;
        private void OnCollisionEnter2D(Collision2D c, int time)
        {
            if (!isXemPlay)
            {
                isXemPlay = true;
                PlayVoice(1);
            SpineManager.instance.DoAnimation(xem,"xem3",false,()=> {

                SpineManager.instance.DoAnimation(xem, "xem4", true);
                Delay(2,EndVideo);
            });
            }
            

        }

        #endregion


        #region  对错
        /// <summary>
        /// 判断正误
        /// </summary>
        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            if (isMatch)
            {
                PlaySuccessSound();
                PlayVoice(3);
                dzxg.transform.DOMove(milPyDropers[curPz - 1].transform.position, 0);
                milDragers[curPz - 1].gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(milDragers[curPz - 1].transform.GetChild(0).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(milDragers[curPz - 1].transform.GetChild(1).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(dzxg, "dzxg", false, () =>
                {
                    _mono.StartCoroutine(JDTMove());

                });

                if (curPz < 7)
                {
                    curPz++;
                    Delay(1,()=> { PzInstance(curPz); });
                    
                }
                else {
                    Delay(1,()=> {
                        dpz.SetActive(true);
                        dpz.GetComponent<EventDispatcher>().CollisionEnter2D += OnCollisionEnter2D;
                        SpineManager.instance.DoAnimation(dpz, "pz", false);
                        dpz.transform.GetComponent<Rigidbody2D>().gravityScale = 200;

                        PlaySpine(DD, "DD3", isLoop: false);
                    });
                    
                }

            }
            else
            {
                PlayVoice(5);
                milDragers[curPz - 1].transform.GetRectTransform().DOAnchorPos(pzPos.transform.GetRectTransform().anchoredPosition, 0f);
                hasBeginDrag = false;
                GameObject obj = milDragers[curPz - 1].transform.GetChild(1).gameObject;
                GameObject obj2 = milDragers[curPz - 1].transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(obj, obj.name, true);
                SpineManager.instance.DoAnimation(obj2, obj2.name + 3, false);

                if (falsePz)
                {
    
                    falsePz.SetAsLastSibling();
                    falsePz.gameObject.GetComponent<SkeletonGraphic>().freeze = false;
                    SpineManager.instance.DoAnimation(falsePz.gameObject, falsePz.gameObject.name, false,()=> {
                        initSlibe();
                        falsePz = null;
                    });
                    
                }
            }
        }
        bool hasBeginDrag;
        Transform falsePz;

        void DragBegin(Vector3 position, int type, int index)
        {
            if (!hasBeginDrag)
            {
                PlayVoice(4);
                hasBeginDrag = true;
                GameObject obj1 = milDragers[curPz - 1].transform.GetChild(0).gameObject;
                GameObject obj2 = milDragers[curPz - 1].transform.GetChild(1).gameObject;
                SpineManager.instance.DoAnimation(obj1,obj1.name+4,false);
                Debug.Log("obj1.parent:  " + obj1.transform.parent.name);
                Debug.Log("obj1.name + 4:  " + obj1.name + 4);

                SpineManager.instance.DoAnimation(obj2, "kong", false);
                milDragers[curPz - 1].transform.position = Input.mousePosition;
                return;
            }
        }

        bool DoAfter(int dragType, int index, int dropType)
        {
            if (dragType != dropType && (dropType >= 12 && dropType <= 30))
                falsePz = milPxDropers[dropType - 12].transform.parent;
            if (dragType != dropType && (dropType >= 1 && dropType <= 7))
                falsePz = milPyDropers[dropType - 1].transform.parent;
           
            return dragType == dropType;
        }

        #endregion

        #region 结束动画
        void EndVideo() {

            PlayVoice(2);
            ColorDisPlay(mask_image, true, ()=> { bg2.gameObject.SetActive(true); }, 0.3f);
          
            Delay(0.45f, () => { ColorDisPlay(mask_image, false, null, 0.3f); });
            Delay(1.05f,()=> {
                bg2.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(bg2.GetChild(0).gameObject, "01", false, () =>
                {
                    SpineManager.instance.DoAnimation(bg2.GetChild(0).gameObject, "02", true);
                   mask_image.DOColor(new Vector4(0, 0, 0, 0.78f),0f);
                    GameSuccess();
                });
            });
            
        
        }
        #endregion



        //物体渐变显示或者消失
        void ColorDisPlay(Image raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(0, 0, 0, 0);
                raw.gameObject.SetActive(true);

                raw.DOColor(new Color(0, 0, 0, 1f), _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = new Color(0, 0, 0, 1f);
                raw.DOColor(new Color(0, 0, 0, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }






        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();

            PzInstance(curPz);

            // EndVideo();
           /* dpz.SetActive(true);
            dpz.GetComponent<EventDispatcher>().CollisionEnter2D += OnCollisionEnter2D;
            SpineManager.instance.DoAnimation(dpz, "pz", false);
            dpz.transform.GetComponent<Rigidbody2D>().gravityScale = 250;*/

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
                        PlayBgm(0);
                        StartGame();                      
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                  
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();

                        _dDD.Show();
                        BellSpeck(_dDD, 2);

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
         //   PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
          //  PlayCommonSound(4);
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Adult, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Adult, float len = 0)
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
