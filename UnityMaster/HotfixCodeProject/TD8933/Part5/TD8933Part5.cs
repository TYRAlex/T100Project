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
    public class TD8933Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private bool _canClick;
        private GameObject dz;
        private Vector2 _tempvector;
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



        bool isPlaying = false;

        private int number;
        private GameObject Box;
        private GameObject xem;
        private Vector3 clickPos;
        private EventDispatcher[] eventDispatcher;
        private EventDispatcher xemeventDispatcher;
        private bool _isMove;
        private GameObject _wuyun;
        private GameObject _wuyun2;
        private GameObject _wuyun3;
        private GameObject _xem;

        private bool _canbf;
        private bool _canfh;
        private bool _canok;
        private bool _cantouch;
        private bool _cando;
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

            dz = curTrans.Find("Bg/dz").gameObject;
            Box = curTrans.Find("Bg/Box").gameObject;

            xem = curTrans.Find("xem").gameObject;
            _xem = xem.transform.Find("xem").gameObject;
            _wuyun = curTrans.Find("wuyun").gameObject;
            _wuyun2 = xem.transform.Find("wuyun2").gameObject;
            _wuyun3 = curTrans.Find("wuyun3").gameObject;




            GameInit();
            //GameStart();
        }

        private void ClickEvent(GameObject obj)
        {
            bool a;
            bool b;
            if (number == 8)
            {
                a = obj.transform.position.x < (dz.transform.position.x - (0.5f * (0.65f * dz.GetComponent<RectTransform>().rect.width - 0.8f * obj.GetComponent<RectTransform>().rect.width)));
                b = obj.transform.position.x > (dz.transform.position.x + (0.5f * (0.65f * dz.GetComponent<RectTransform>().rect.width - 0.8f * obj.GetComponent<RectTransform>().rect.width)));

            }
            else
            {
                a = obj.transform.position.x > (Box.transform.GetChild(number + 1).position.x - (0.5f * 0.8f * (Box.transform.GetChild(number + 1).GetComponent<RectTransform>().rect.width - obj.GetComponent<RectTransform>().rect.width)));
                b = obj.transform.position.x < (Box.transform.GetChild(number + 1).position.x + (0.5f * 0.8f * (Box.transform.GetChild(number + 1).GetComponent<RectTransform>().rect.width - obj.GetComponent<RectTransform>().rect.width)));

            }
            if (_canClick && a && b)
            {
                //_tempvector = obj.GetComponent<Rigidbody2D>().velocity;
                
                clickPos = obj.transform.position;
                _canClick = false;
                _isMove = true;
                _cando = true;
                obj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -800f);
            }
        }
        private void Touch(Collision2D c, int time)
        {
            if (c.collider.gameObject.name == "qiang")
            {
                c.otherRigidbody.velocity = new Vector2(300 + (9 - number) * 40, 0);
            }
            else if (c.collider.gameObject.name == "qiang2")
            {
                c.otherRigidbody.velocity = new Vector2(-300 - (9 - number) * 40, 0);
            }
            else if (c.collider.gameObject.name == "xem")
            {
                //Debug.Log("aa");
                ////_isMove = false;
                ////c.otherRigidbody.velocity = new Vector3(0, 0, 0);
                ////mono.StartCoroutine(Move2(0.3f,c.otherRigidbody.gameObject, c.otherRigidbody.gameObject.transform.position, clickPos,
                ////    () => { _canClick = true; }
                ////    ));
                //////c.otherRigidbody.gameObject.transform.position = clickPos;
                //int temp;
                //if (number % 2 == 1)
                //{
                //    temp = 300 + number * 100;
                //}
                //else
                //{
                //    temp = -300 + number * -100;
                //}
                //Box.transform.GetChild(number).GetComponent<Rigidbody2D>().velocity = new Vector2(temp, 0);

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                _isMove = false;
                _canClick = true;
                c.otherRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                eventDispatcher[number].enabled = false;
                number--;
                if (number == 6 || number == 3)
                {
                    mono.StartCoroutine(Move(0.3f, Bg, Bg.transform.localPosition, new Vector3(0, Bg.transform.localPosition.y - ((0.5f * 0.8f * Box.transform.GetChild(number + 1).GetComponent<RectTransform>().rect.height) + (0.5f * 0.8f * Box.transform.GetChild(number).GetComponent<RectTransform>().rect.height)), 0),
                        () =>
                        {
                            _wuyun.SetActive(true);
                            _wuyun.transform.position = new Vector3(Box.transform.GetChild(number + 1).gameObject.transform.position.x, Box.transform.GetChild(number + 1).gameObject.transform.position.y + 40, 0);
                            for (int i = 8; i > number; i--)
                            {

                                SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name + "2", false);

                            }

                            mono.StartCoroutine(Time(0.5f,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                                    for (int i = 8; i > number; i--)
                                    {

                                        SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name + "2", false);
                                    }
                                }
                                ));
                            mono.StartCoroutine(Time(2.6f,
                                () =>
                                {
                                    for (int i = 8; i > number; i--)
                                    {
                                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                        SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name, false);

                                    }
                                }
                                ));
                            SpineManager.instance.DoAnimation(_wuyun, "Nengliang1", false,
                                () =>
                                {
                                    //SpineManager.instance.DoAnimation(_wuyun, "Nengliang2", false,
                                    //    () =>
                                    //    {
                                            _wuyun.SetActive(false);
                                            _wuyun2.SetActive(true);
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                                            SpineManager.instance.DoAnimation(_wuyun2, "Nengliang3", false,
                                                () =>
                                                {
                                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                                                    SpineManager.instance.DoAnimation(_wuyun2, "Nengliang3", false,
                                                        () => { SpineManager.instance.DoAnimation(_wuyun2, "Nengliang3", false); });
                                                    Vector2 vector2 = xem.GetComponent<Rigidbody2D>().velocity;
                                                    xem.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                                                    SpineManager.instance.DoAnimation(_xem, "XEM4", false,
                                                        () =>
                                                        {
                                                            if (number == 6)
                                                            { curTrans.Find("sz").GetChild(2).gameObject.SetActive(false); }
                                                            if (number == 3)
                                                            { curTrans.Find("sz").GetChild(1).gameObject.SetActive(false); }

                                                            SpineManager.instance.DoAnimation(_xem, "XEM5", false,
                                                        () =>
                                                        {
                                                            SpineManager.instance.DoAnimation(_xem, "XEM", true);
                                                            xem.GetComponent<Rigidbody2D>().velocity = vector2;
                                                            _wuyun2.SetActive(false);
                                                            CreatrAndMove();
                                                        }
                                                        );
                                                        }
                                                        );
                                                }
                                                );
                                        //}
                                        //);
                                }
                                );
                        }
                        ));
                }
                else if (number == -1)
                {
                    _wuyun3.SetActive(true);
                    _wuyun3.transform.position = new Vector3(Box.transform.GetChild(0).gameObject.transform.position.x, Box.transform.GetChild(0).gameObject.transform.position.y + 25, 0);
                    for (int i = 8; i > number; i--)
                    {

                        SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name + "2", false);

                    }

                    mono.StartCoroutine(Time(0.5f,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                            for (int i = 8; i > number; i--)
                            {

                                SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name + "2", false);

                            }
                        }
                        ));
                    mono.StartCoroutine(Time(2.6f,
                        () =>
                        {
                            for (int i = 8; i > number; i--)
                            {
                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name, false);

                            }
                        }
                        ));
                    for (int i = 8; i > number; i--)
                    {
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(0).gameObject, "ta" + Box.transform.GetChild(i).GetChild(0).gameObject.name + "2");
                    }
                    SpineManager.instance.DoAnimation(_wuyun3, "Nengliang1", false,
                        () =>
                        {
                            //SpineManager.instance.DoAnimation(_wuyun3, "Nengliang2", false,
                            //    () =>
                            //    {
                                    _wuyun3.SetActive(false);
                                    _wuyun2.SetActive(true);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                                    SpineManager.instance.DoAnimation(_wuyun2, "Nengliang3", false,
                                        () =>
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                                            SpineManager.instance.DoAnimation(_wuyun2, "Nengliang3", false,
                                                        () => { SpineManager.instance.DoAnimation(_wuyun2, "Nengliang3", false); });
                                            Vector2 vector2 = xem.GetComponent<Rigidbody2D>().velocity;
                                            xem.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                                            SpineManager.instance.DoAnimation(_xem, "XEM4", false,
                                                () =>
                                                {
                                                    curTrans.Find("sz").GetChild(0).gameObject.SetActive(false);
                                                    SpineManager.instance.DoAnimation(_xem, "XEM5", false,
                                                () =>
                                                {
                                                    SpineManager.instance.DoAnimation(_xem, "XEM", true);
                                                    xem.GetComponent<Rigidbody2D>().velocity = vector2;
                                                    _wuyun2.SetActive(false);
                                                    Win();
                                                }
                                                );
                                                }
                                                );
                                        }
                                        );
                                //}
                                //);
                        }
                        );
                }
                else
                {
                    mono.StartCoroutine(Move(0.3f, Bg, Bg.transform.localPosition, new Vector3(0, Bg.transform.localPosition.y - ((0.5f * 0.8f * Box.transform.GetChild(number + 1).GetComponent<RectTransform>().rect.height) + (0.5f * 0.8f * Box.transform.GetChild(number).GetComponent<RectTransform>().rect.height)), 0),
                        () =>
                        { CreatrAndMove(); }
                        ));
                }

            }
        }

        private void XemTouch(Collision2D c, int time)
        {
            if (c.collider.gameObject.name == "qiang")
            {
                c.otherRigidbody.velocity = new Vector2(300, 0);
            }
            else if (c.collider.gameObject.name == "qiang2")
            {
                c.otherRigidbody.velocity = new Vector2(-300, 0);
            }
        }

        private void XemMove()
        {
            xem.GetComponent<Rigidbody2D>().velocity = new Vector2(300, 0);
        }
        IEnumerator Move(float time, GameObject obj, Vector3 vector1, Vector3 vector2, Action callback = null)
        {
            float temp = 0;
            while (true)
            {
                temp += 0.01f;
                obj.transform.localPosition = new Vector3(vector1.x + (vector2.x - vector1.x) * (temp / time), vector1.y + (vector2.y - vector1.y) * (temp / time), 0);
                if (temp > time)
                {
                    break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            callback?.Invoke();
            yield break;
        }

        IEnumerator Move2(float time, GameObject obj, Vector3 vector1, Vector3 vector2, Action callback = null)
        {
            //if (obj.GetComponent<Rigidbody2D>() != null)
            //{ obj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); }
            float temp = 0;
            while (true)
            {
                temp += 0.01f;
                obj.transform.position = new Vector3(vector1.x + (vector2.x - vector1.x) * (temp / time), vector1.y + (vector2.y - vector1.y) * (temp / time), 0);
                if (obj.transform.position == vector2)
                {
                    break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            callback?.Invoke();
            yield break;
        }

        IEnumerator XemTime()
        {
            while (true)
            {
                if (_isMove && xem.transform.localPosition.x < 700f && xem.transform.localPosition.x > -700f)
                { xem.GetComponent<Collider2D>().isTrigger = true; }
                else
                { xem.GetComponent<Collider2D>().isTrigger = false; }
                yield return new WaitForSeconds(0.02f);
            }
        }

        IEnumerator Time(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator Scale(float time, GameObject obj, Vector3 vector, Action callback = null)
        {
            float temp = 0;
            while (true)
            {
                temp += 0.01f;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x + (vector.x - obj.transform.localScale.x) * (temp / time), obj.transform.localScale.y + (vector.y - obj.transform.localScale.y) * (temp / time), 1);
                if (obj.transform.localScale == new Vector3(1, 1, 1))
                {
                    break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            callback?.Invoke();
            yield break;
        }
        private void XemTouch2(Collider2D other, int time)
        {
            if (_cando)
            {
                _cando = false;
                other.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(_xem, "XEM2", false,
                    () => { SpineManager.instance.DoAnimation(_xem, "XEM", true); }
                    );
                mono.StartCoroutine(Move2(0.1f, other.gameObject, other.gameObject.transform.position, clickPos,
                    () =>
                    {
                        other.GetComponent<Rigidbody2D>().velocity = _tempvector;
                        _isMove = false;
                        _canClick = true;
                    }
                    ));
                //other.gameObject.transform.position = clickPos;
                //other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }
        private void CreatrAndMove()
        {
            if (number < 9)
            {
                Box.transform.GetChild(number).gameObject.SetActive(true);
                eventDispatcher[number].enabled = true;
                eventDispatcher[number].CollisionEnter2D += Touch;
                SpineManager.instance.DoAnimation(Box.transform.GetChild(number).GetChild(0).gameObject, "ta" + Box.transform.GetChild(number).GetChild(0).gameObject.name, false);
                int temp;
                if (number % 2 == 1)
                {
                    temp = 300 + (9-number) * 40;
                }
                else
                {
                    temp = -300 + (9-number) * -40;
                }
                Box.transform.GetChild(number).GetComponent<Rigidbody2D>().velocity = new Vector2(temp, 0);
                _tempvector = new Vector2(temp, 0);
            }
        }

        private void Win()
        {
            xem.SetActive(false);
            curTrans.Find("sz").gameObject.SetActive(false);
            mono.StartCoroutine(Move(2f, Bg, Bg.transform.localPosition, new Vector3(0, -544, 0),
               () => {
                   mono.StartCoroutine(Scale(2f, Bg, new Vector3(1, 1, 1),
               () =>
               {
                   SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                   mono.StartCoroutine(Time(5f,
                       () => { playSuccessSpine(); }
                       ));
                   curTrans.Find("guang").gameObject.SetActive(true);
                   SpineManager.instance.DoAnimation(curTrans.Find("guang").gameObject, "0guang", false,
                       () => { SpineManager.instance.DoAnimation(curTrans.Find("guang").gameObject, "0guang2", true); }
                       );
               }
               ));
               }
               ));
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            _cantouch = true;
            _canbf = true;
            _canfh = true;
            _canok = true;
            _cando = true;
            xemeventDispatcher = xem.GetComponent<EventDispatcher>();
            eventDispatcher = new EventDispatcher[Box.transform.childCount];
            curTrans.Find("xem").GetChild(1).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _wuyun.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _wuyun2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _wuyun3.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            mono.StartCoroutine(XemTime());
            Bg.GetComponent<RectTransform>().offsetMax = new Vector2(0, -160);
            Bg.GetComponent<RectTransform>().offsetMin = new Vector2(0, -160);
            Bg.GetComponent<RectTransform>().localScale = new Vector2(2, 2);
            isPlaying = false;
            xem.SetActive(false); 
            _tempvector = new Vector2();
            clickPos = new Vector3();
            _canClick = true;
            number = 8;
            for (int i = 8; i > -1; i--)
            {
                Box.transform.GetChild(i).GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                if (Box.transform.GetChild(i).name == "A")
                {
                    Box.transform.GetChild(i).localPosition = new Vector2(0, 640);
                }
                else
                {
                    Box.transform.GetChild(i).localPosition = new Vector2(0, Box.transform.GetChild(i + 1).localPosition.y + (0.5f * 0.8f * Box.transform.GetChild(i + 1).GetComponent<RectTransform>().rect.height) + (0.5f * 0.8f * Box.transform.GetChild(i).GetComponent<RectTransform>().rect.height));
                }
            }
            for (int i = 0; i < 9; i++)
            {
                Box.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                Box.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < 9; i++)
            {
                eventDispatcher[i] = Box.transform.GetChild(i).GetComponent<EventDispatcher>();
                Util.AddBtnClick(Box.transform.GetChild(i).gameObject, ClickEvent);
            }
            curTrans.Find("sz").gameObject.SetActive(false);
            curTrans.Find("guang").gameObject.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                curTrans.Find("sz").GetChild(i).gameObject.SetActive(true);
            }
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
            _cantouch = true;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            LRBtnUpdate();
        }
        void TDGameStart()
        {
            _cantouch = true;
            mask.SetActive(true);
            bd.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null,
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                ));
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

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
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
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null,
                        () =>
                        {
                            bd.SetActive(false);
                            mask.SetActive(false);
                            xemeventDispatcher.CollisionEnter2D += XemTouch;
                            xemeventDispatcher.TriggerEnter2D += XemTouch2;
                            xem.SetActive(true);
                            SpineManager.instance.DoAnimation(_xem, "XEM", true);
                            CreatrAndMove();
                            XemMove();
                            curTrans.Find("sz").gameObject.SetActive(true);
                        }
                        ));

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
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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

            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
        }
        private void LRBtnUpdate()
        {
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
            else if (curPageIndex == SpinePage.childCount - 1)
            {
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.name + "4", false);
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            }
            else
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
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
            TDLoadPageBar();
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
            if (!_cantouch)
                return;
            _cantouch = false;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart(); isPlaying = false;
                    });
                }
                else if (obj.name == "fh")
                {
                    
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => {
                        anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); xem.SetActive(true);
                        xemeventDispatcher.CollisionEnter2D += XemTouch;
                        xemeventDispatcher.TriggerEnter2D += XemTouch2;
                        SpineManager.instance.DoAnimation(_xem, "XEM", true);
                        CreatrAndMove();
                        XemMove();
                        curTrans.Find("sz").gameObject.SetActive(true);
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
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
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
