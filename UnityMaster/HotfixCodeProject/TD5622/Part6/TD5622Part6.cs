using DG.Tweening;
using DG.Tweening.Core.Easing;
using ILRuntime.Runtime;
using Microsoft.SqlServer.Server;
using Microsoft.Win32.SafeHandles;
using Spine.Unity;
using Spine.Unity.Playables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Ximmerse.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD5622Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;

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
        private Transform SpineShow;

        #endregion

        #endregion

        #region 点击滑动图片

        private GameObject pageBar;
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;
        private GameObject rightBtn;
        private GameObject leftBtn;
        private GameObject btnBack;
        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        #endregion



        bool isPressBtn = false;
        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        #endregion


        GameObject _yezi;
        GameObject fishBox;
        GameObject wang;
        private RectTransform _fishRect;
        GameObject jd;
        private Vector2 _jdpos;
        private Vector2 _yezipos;
        EventDispatcher eventDispatcher;
        bool isPlaying = false;
        //private GameObject[] allfish;
        //bool[] jugleDown;
        mILDrager milDrager;
        mILDrager milDragerJD;
        private Vector3[] vector3s;
        private List<GameObject> _fishlist;
        private List<GameObject> _xemlist;
        private int _fishListIndex;
        private GameObject[] number;
        private Image Img;
        private int numberIndex;
        private Vector2[] alltra;
        private Vector2[] _xempos;
        private RectTransform[] returnBox;
        private int _fishcaught;
        private int _xemListIndex;
        private GameObject paopao;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //田丁加载游戏物体方法
            TDLoadGameProperty();



            paopao = curTrans.Find("paopao").gameObject;
            number = new GameObject[curTrans.Find("time/number").childCount];
            Img = curTrans.Find("time/timelimited").GetComponent<Image>();
            wang = curTrans.Find("wang").gameObject;
            fishBox = curTrans.Find("fishBox/fish").gameObject;
            _fishRect = fishBox.GetComponent<RectTransform>();
            //jugleDown = new bool[fishBox.transform.childCount];
            milDrager = curTrans.Find("yezi").GetComponent<mILDrager>();
            jd = curTrans.Find("jd").gameObject;
            milDragerJD = jd.GetComponent<mILDrager>();

            _jdpos = curTrans.Find("_jdpos").transform.localPosition;
            _yezipos = curTrans.Find("_yezipos").transform.localPosition;
            Random random = new Random();
            _fishlist = new List<GameObject>();
            _xemlist = new List<GameObject>();
            returnBox = new RectTransform[curTrans.Find("returnBox").childCount];
            for (int i = 0; i < returnBox.Length; i++)
            {
                returnBox[i] = curTrans.Find("returnBox").GetChild(i).GetRectTransform();
            }
            _xempos = new Vector2[curTrans.Find("fishBox/xem").childCount];
            for (int i = 0; i < _xempos.Length; i++)
            {
                _xempos[i] = curTrans.Find("fishBox/_xempos").GetChild(i).localPosition;
            }
            alltra = new Vector2[fishBox.transform.childCount];
            for (int i = 0; i < alltra.Length; i++)
            {
                alltra[i] = curTrans.Find("fishBox/_fishpos").GetChild(i).localPosition;
            }
            vector3s = GetVertexLocalPosition(_fishRect);


            _yezi = curTrans.GetGameObject("yezi");

            for (int i = 0; i < number.Length; i++)
            {
                number[i] = curTrans.Find("time/number").GetChild(i).gameObject;
            }


            milDrager.SetDragCallback(null, null, null, null);
            milDragerJD.SetDragCallback(null, null, DragEnd, null);


            GameInit();
            //GameStart();


        }

        /// <summary>
        /// 获取UI局部顶点坐标
        /// 顺序：左下、左上、右上、右下，
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        /// 

        private void DragEnd(Vector3 pos, int x, int y, bool a)
        {
            if (a)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(jd, "jd-1", false,
                        () =>
                        {
                            jd.SetActive(false);
                            _yezi.GetComponent<Collider2D>().enabled = true;
                            _yezi.GetComponent<mILDrager>().isActived = true;
                            _yezi.GetComponent<mILDrager>().canMove = true;
                            SpineManager.instance.DoAnimation(wang, "wang2", false,
                             () =>
                             {
                                 SpineManager.instance.DoAnimation(wang, "wang2", true);
                                 BeginMoveDown(1.76f);
                                 //BeginXemMoveDown(5);
                             }
                                );
                        }
                        );
            }
        }
        private Vector3[] GetVertexLocalPosition(RectTransform rectTransform)
        {
            Vector3[] vertexPos = new Vector3[4];

            var rect = rectTransform.rect;

            var localPosition = rectTransform.localPosition;
            var localPosX = localPosition.x;
            var localPosY = localPosition.y;

            var xMin = rect.xMin; var yMin = rect.yMin;
            var xMan = rect.xMax; var yMax = rect.yMax;

            Vector3 leftDown = new Vector3(localPosX + xMin, localPosY + yMin, 0);
            Vector3 leftUp = new Vector3(localPosX + xMin, localPosY + yMax, 0);
            Vector3 rightUp = new Vector3(localPosX + xMan, localPosY + yMax, 0);
            Vector3 rightDown = new Vector3(localPosX + xMan, localPosY + yMin, 0);

            vertexPos[0] = leftDown;
            vertexPos[1] = leftUp;
            vertexPos[2] = rightUp;
            vertexPos[3] = rightDown;

            return vertexPos;
        }





        private void Touch(Collider2D collider2D, int num)
        {
            if (collider2D != null)
            {

                //SpineManager.instance.DoAnimation(_yezi.transform.GetChild(0).gameObject, "yezi2", false,
                //    () =>
                //    {
                //        SpineManager.instance.DoAnimation(_yezi.transform.GetChild(0).gameObject, "yezi", true);
                //    }
                //    );
                //if (collider2D.name == "xem")
                //{
                //    //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,)
                //    Disappear(collider2D.gameObject, 0.03f,
                //        () =>
                //        { collider2D.gameObject.SetActive(false); }
                //        );
                //}
                if (collider2D.name == "nofish")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    collider2D.enabled = false;
                    XemMoveDown();
                    Disappear(collider2D.gameObject, 0.03f,
                        () =>
                        {
                            collider2D.gameObject.SetActive(false);
                            collider2D.enabled = true;
                        });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    collider2D.enabled = false;
                    _fishcaught++;
                    int i = Random.Range(0, 2);
                    if (i == 0)
                    {
                        collider2D.gameObject.transform.localEulerAngles = new Vector3(0, 180, 0);
                        SpineManager.instance.DoAnimation(collider2D.gameObject.transform.GetChild(0).gameObject, collider2D.gameObject.transform.GetChild(0).gameObject.name + "1", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(collider2D.gameObject.transform.GetChild(0).gameObject, collider2D.gameObject.transform.GetChild(0).gameObject.name + "1", false,
                               () =>
                               {
                                   collider2D.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                                   collider2D.enabled = true;

                                   SpineManager.instance.DoAnimation(collider2D.gameObject.transform.GetChild(0).gameObject, collider2D.gameObject.transform.GetChild(0).gameObject.name + "1", true);
                               });
                            }
                            );
                        collider2D.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1200, collider2D.gameObject.GetComponent<RectTransform>().rect.y - 350), 2f);
                    }
                    if (i == 1)
                    {
                        SpineManager.instance.DoAnimation(collider2D.gameObject.transform.GetChild(0).gameObject, collider2D.gameObject.transform.GetChild(0).gameObject.name + "1", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(collider2D.gameObject.transform.GetChild(0).gameObject, collider2D.gameObject.transform.GetChild(0).gameObject.name + "1", false,
                                  () =>
                                  {
                                      collider2D.gameObject.transform.localEulerAngles = new Vector3(0, 180, 0);
                                      collider2D.enabled = true;

                                      SpineManager.instance.DoAnimation(collider2D.gameObject.transform.GetChild(0).gameObject, collider2D.gameObject.transform.GetChild(0).gameObject.name + "1", true);
                                  });
                            }
                            );
                        collider2D.gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1200, collider2D.gameObject.GetComponent<RectTransform>().rect.y - 350), 2f);
                    }
                }
            }
        }

        //private void BeginXemMoveDown(float time, Action callback = null)
        //{
        //    mono.StartCoroutine(iexemtime(time, callback));
        //}

        //IEnumerator iexemtime(float time, Action callback = null)
        //{
        //    while (true)
        //    {
        //        if (_xemlist.Count > 0)
        //        {
        //            _xemListIndex = Random.Range(0, _xemlist.Count);
        //            _xemlist[_xemListIndex].SetActive(true);
        //            _xemlist[_xemListIndex].transform.GetRectTransform().DOAnchorPosY(-1200, 2f);
        //            _xemlist.RemoveAt(_xemListIndex);
        //        }
        //        else
        //        {
        //            yield break;
        //        }
        //        yield return new WaitForSeconds(time);
        //        callback?.Invoke();
        //    }
        //}

        IEnumerator iepaopao(float time)
        {
            SpineManager.instance.DoAnimation(paopao.transform.GetChild(0).gameObject, "2", true);
            yield return new WaitForSeconds(time);
            SpineManager.instance.DoAnimation(paopao.transform.GetChild(1).gameObject, "2", true);
        }
        private void Disappear(GameObject obj, float time, Action callback = null)
        {
            mono.StartCoroutine(iexemdisappear(obj, time, callback));
        }
        IEnumerator iexemdisappear(GameObject obj, float time, Action callback = null)
        {
            float a = 256;
            while (true)
            {
                a -= 256 / 4;
                obj.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, a / 256);
                yield return new WaitForSeconds(time);
                if (a == 0)
                { yield break; }

            }
        }

        private void BeginFishOut(float time, Action callback = null)
        {
            mono.StartCoroutine(iefishbegin(time, callback));
        }

        IEnumerator iefishbegin(float time, Action callback = null)
        {
            for (int i = 0; i < fishBox.transform.childCount; i++)
            {
                if (fishBox.transform.GetChild(i).name != "xem" && fishBox.transform.GetChild(i).name != "nofish")
                    SpineManager.instance.DoAnimation(fishBox.transform.GetChild(i).GetChild(0).gameObject, fishBox.transform.GetChild(i).GetChild(0).name + "0", true);
                yield return new WaitForSeconds(time);

            }
            callback?.Invoke();
            yield break;
        }

        private void BeginMoveDown(float time, Action callback = null)
        {
            mono.StartCoroutine(ietimedown(time, callback));
            BeginTimeLimited(1f);
        }

        IEnumerator ietimedown(float time, Action callback = null)
        {
            while (true)
            {
                MoveDown();
                if (_fishlist.Count == 0)
                {
                    yield break;
                }
                yield return new WaitForSeconds(time);
                callback?.Invoke();
            }
        }

        private void BeginTimeLimited(float time, Action callback = null)
        {
            mono.StartCoroutine(ietimelimited(time, callback));
        }
        IEnumerator ietimelimited(float time, Action callback = null)
        {
            while (true)
            {
                yield return new WaitForSeconds(time);
                if(Img.fillAmount<0.2&&Img.fillAmount>0.04)
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("time").GetChild(0).gameObject, "jdt3", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                }
                if (Img.fillAmount < 0.01)
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("time").GetChild(0).gameObject, "jdt2", false);
                    DOTween.Clear();
                    int t = 0;
                    _yezi.GetComponent<Collider2D>().enabled = false;
                    _yezi.GetComponent<mILDrager>().canMove = false;
                    SpineManager.instance.DoAnimation(wang, "wang3", false);
                    eventDispatcher.TriggerEnter2D -= Touch;
                    for (int i = 0; i < fishBox.transform.childCount; i++)
                    {
                        if (fishBox.transform.GetChild(i).name != "xem")
                        {
                            fishBox.transform.GetChild(i).GetRectTransform().DOAnchorPos(returnBox[t].localPosition, 3f);
                            t += 1;
                        }
                    }
                    yield return new WaitForSeconds(3f);
                    playSuccessSpine();
                    yield break;
                }
                Img.fillAmount -= 0.033f;
                if (numberIndex < 30)
                {
                    number[numberIndex].SetActive(false);
                    number[numberIndex + 1].SetActive(true);
                    numberIndex += 1;
                }
            }

        }
        private void MoveDown()
        {
            if (_fishlist.Count > 0)
            {
                _fishListIndex = Random.Range(0, _fishlist.Count);
                if (_fishlist[_fishListIndex].name == "xem")
                {
                    _fishlist[_fishListIndex].SetActive(true);

                }

                else if (_fishlist[_fishListIndex].name == "nofish")
                {

                }

                else
                {
                    SpineManager.instance.DoAnimation(_fishlist[_fishListIndex].transform.GetChild(0).gameObject, _fishlist[_fishListIndex].transform.GetChild(0).name + "2", true);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                _fishlist[_fishListIndex].transform.GetRectTransform().DOAnchorPosY(-1300, 2.5f);

                _fishlist.RemoveAt(_fishListIndex);
            }
            return;
        }

        private void XemMoveDown()
        {

            Debug.Log("777");
            if (_xemlist.Count > 0)
            {
                _xemListIndex = Random.Range(0, _xemlist.Count);
                _xemlist[_xemListIndex].transform.GetRectTransform().DOAnchorPosY(-1300, 3f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                _xemlist.RemoveAt(_xemListIndex);
            }
        }


        //private void mydate(float delay, Action callback = null)
        //{
        //    mono.StartCoroutine(iemydata(delay, callback));
        //}

        //IEnumerator iemydata(float delay, Action callback = null)
        //{
        //    while (true)
        //    {
        //        if (jd.transform.localPosition.x > vector3s[0].x && jd.transform.localPosition.x < vector3s[3].x && jd.transform.localPosition.y > vector3s[0].y)
        //        {

        //            SpineManager.instance.DoAnimation(jd, "jd-1", false,
        //                () =>
        //                {
        //                    jd.SetActive(false);
        //                    _yezi.GetComponent<Collider2D>().enabled = true;
        //                    _yezi.GetComponent<mILDrager>().isActived = true;
        //                    _yezi.GetComponent<mILDrager>().canMove = true;
        //                    SpineManager.instance.DoAnimation(wang, "wang2", false,
        //                     () =>
        //                     {

        //                         BeginMoveDown(1.56f);
        //                         //BeginXemMoveDown(5);
        //                     }
        //                        );
        //                }
        //                );
        //            yield break;
        //        }
        //        yield return new WaitForSeconds(delay);
        //        callback?.Invoke();
        //    }
        //}


        #region 初始化和游戏开始方法

        private void GameInit()
        {
            _xemListIndex = 0;
            talkIndex = 1;
            SpineManager.instance.DoAnimation(curTrans.Find("yuqun").gameObject, "1", true);
            _fishcaught = 0;
            DOTween.Clear();
            numberIndex = 0;

            for (int i = 0; i < curTrans.Find("fishBox/xem").childCount; i++)
            {
                curTrans.Find("fishBox/xem").GetChild(i).localPosition = _xempos[i];
            }
            for (int i = 0; i < fishBox.transform.childCount; i++)
            {
                fishBox.transform.GetChild(i).transform.localPosition = alltra[i];
                fishBox.transform.GetChild(i).transform.localEulerAngles = new Vector3(0, 0, 0);
                if (fishBox.transform.GetChild(i).name == "nofish")
                {
                    fishBox.transform.GetChild(i).gameObject.SetActive(true);
                    fishBox.transform.GetChild(i).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                }
                fishBox.transform.GetChild(i).GetComponent<Collider2D>().enabled = true;
            }


            number[30].SetActive(false);
            number[0].SetActive(true);
            jd.SetActive(true);
            jd.transform.localPosition = _jdpos;
            _yezi.transform.localPosition = _yezipos;
            _yezi.GetComponent<mILDrager>().canMove = false;
            SpineManager.instance.DoAnimation(jd, "jd-0", false);
            Img.fillAmount = 1;
            SpineManager.instance.DoAnimation(wang, "wang", true);
            BeginFishOut(0.1f);
            for (int i = 0; i < fishBox.transform.childCount; i++)
            {
                _fishlist.Add(fishBox.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < curTrans.Find("fishBox/xem").childCount; i++)
            {
                _xemlist.Add(curTrans.Find("fishBox/xem").GetChild(i).gameObject);
            }
            //mydate(0.02f);
            eventDispatcher = _yezi.GetComponent<EventDispatcher>();
            if (eventDispatcher != null)
            {
                Component.DestroyImmediate(_yezi.GetComponent<EventDispatcher>());
                _yezi.AddComponent<EventDispatcher>();
                eventDispatcher = _yezi.GetComponent<EventDispatcher>();
            }
            else
            {
                _yezi.AddComponent<EventDispatcher>();
                eventDispatcher = _yezi.GetComponent<EventDispatcher>();
            }
            eventDispatcher = _yezi.GetComponent<EventDispatcher>();

            eventDispatcher.TriggerEnter2D += Touch;

            mono.StartCoroutine(iepaopao(0.5f));
        }



        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();

        }


        #region 田丁

        void TDGameInit()
        {

            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }
        void TDGameStart()
        {
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null,
                () =>
                { SoundManager.instance.ShowVoiceBtn(true); }
                ));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            //{
            //    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
            //    {
            //        ShowDialogue("", devilText);
            //    }, () =>
            //    {
            //        buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            //        {
            //            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
            //            {
            //                ShowDialogue("", bdText);
            //            }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //        });
            //    }));
            //});
        }

        #endregion

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
                speaker = bd;
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
            //点击标志位
            flag = 0;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 3, null, () => { mask.SetActive(false); bd.SetActive(false);
                SpineManager.instance.DoAnimation(jd, "jd-1", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            SpineManager.instance.DoAnimation(jd, "jd-1", false,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    SpineManager.instance.DoAnimation(jd, "jd-1", false,
                                        () =>
                                        { SpineManager.instance.DoAnimation(jd, "jd-0", false); }
                                        );
                                }
                                );
                        }
                        ); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            }));
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
        private void PlaySpineAni(GameObject target, string name, bool isLoop = false, Action callback = null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }

        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index, Action goingEvent = null, Action finishEvent = null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex, Action callback = null)
        {
            float voiceTimer = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
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
        void WaitTimeAndExcuteNext(float timer, Action callback)
        {

            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer, Action callBack)
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

        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
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

        //private float _durY=10;
        //private RectTransform  _yuRect;

        //private Vector3[] pos = new Vector3[4];


        //private void Cheak()
        //{
        //    MyDate(0.02f, () => {

        //        if (_yuRect!=null)
        //        {
        //            // 更新鱼的坐标

        //            //前提是拖拽中
        //            //更新叶子的坐标
        //            if (true)
        //            {

        //            }

        //            //判断鱼是否在叶子的范围内




        //        }
        //    });
        //}



        #region 田丁

        #region 田丁加载

        /// <summary>
        /// 田丁加载所有物体
        /// </summary>
        void TDLoadGameProperty()
        {
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);
            //任务对话方法加载
            TDLoadDialogue();
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();
            //加载点击滑动图片
            //TDLoadPageBar();
            //加载材料环节
            //LoadSpineShow();

        }

        /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
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
            tz = "3-5";
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
        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {
            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);
        }
        /// <summary>
        /// 加载点击材料环节
        /// </summary>
        void LoadSpineShow()
        {
            SpineShow = curTrans.Find("SpineShow");
            SpineShow.gameObject.SetActive(true);
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }
        }


        #endregion

        #region 鼠标滑动图片方法

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = Math.Abs(upData.position.x - _prePressPos.x);
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 300)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }


        #endregion

        #region 点击材料环节

        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
             {
                 isPlaying = false;
                 if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
                 {
                     flag += 1 << obj.transform.GetSiblingIndex();
                 }
                 if (flag == (Mathf.Pow(2, SpineShow.childCount) - 1))
                 {
                     SoundManager.instance.ShowVoiceBtn(true);
                 }
             }));
            SpineManager.instance.DoAnimation(SpineShow.gameObject, obj.name, false);
        }


        #endregion

        #region 点击移动图片环节

        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        }

        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false); isPlaying = false; isPressBtn = false;
                    if (flag == (Mathf.Pow(2, SpinePage.childCount) - 1) && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }

        private void OnClickShow(GameObject obj)
        {
            if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
            {
                SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            }
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () =>
                {
                    isPressBtn = true;
                    btnBack.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name) + 1, null, () =>
                       {
                           //用于标志是否点击过展示板
                           if ((flag & (1 << int.Parse(obj.transform.GetChild(0).name))) == 0)
                           {
                               flag += 1 << int.Parse(obj.transform.GetChild(0).name);
                           }
                           isPressBtn = false;
                       }));
                });
            });
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
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit();
                        SpineManager.instance.DoAnimation(jd, "jd-1", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            SpineManager.instance.DoAnimation(jd,"jd-1",false,
                                ()=>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    SpineManager.instance.DoAnimation(jd,"jd-1",false,
                                        ()=>
                                        { SpineManager.instance.DoAnimation(jd, "jd-0", false); }
                                        );
                                }
                                );
                        }
                        ); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
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
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                if (i == 25)
                {
                    text.text = "";
                }
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
            successSpine.transform.Find("jishu").gameObject.SetActive(true);
            successSpine.transform.Find("jishu").gameObject.GetComponent<Text>().text = _fishcaught.ToString();
            successSpine.transform.Find("jishuyu").gameObject.SetActive(true);
            successSpine.transform.Find("chenghao").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "-2", false,
                        () =>
                        {
                            successSpine.transform.Find("jishu").gameObject.SetActive(false);
                            successSpine.transform.Find("jishuyu").gameObject.SetActive(false);
                            successSpine.transform.Find("chenghao").gameObject.SetActive(false);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            anyBtns.gameObject.SetActive(true);
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
