using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD91241Part5
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



        bool isPlaying = false;

        private bool _canJump;
        private bool _canClick;

        private int number;
        private GameObject DBG;
        private GameObject DBG2;
        private GameObject DBG3;
        private GameObject yang;
        private GameObject Box1;
        private GameObject Box2;
        private GameObject Bg1;
        private GameObject Bg2;
        private GameObject Bg11;
        private GameObject Bg12;
        private int _newbg1;
        private GameObject Bg21;
        private GameObject Bg22;
        private int _newbg2;
        private Vector2 Bg1Pos;
        private Vector2 Bg2Pos;
        private List<int> _list;
        private List<string> yangname;
        private string _yang;
        private string[] name;
        private bool _start;
        private int xemsmz;
        private int mysmz;
        private bool[] rbool;
        private bool _canchange;
        private bool _canwrong;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            //Bg = curTrans.Find("Bg").gameObject;
            //bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            number = 0;
            name = new string[15];
            rbool = new bool[4];
            yang = curTrans.Find("yang").gameObject;
            DBG = curTrans.Find("DBG").gameObject;
            DBG2 = curTrans.Find("DBG2").gameObject;
            DBG3 = curTrans.Find("DBG3").gameObject;
            Bg1 = DBG.transform.Find("Bg").gameObject;
            Bg2 = DBG.transform.Find("Bg2").gameObject;
            Bg11 = DBG2.transform.Find("Bg").gameObject;
            Bg12 = DBG2.transform.Find("Bg2").gameObject;
            Bg21 = DBG3.transform.Find("Bg").gameObject;
            Bg22 = DBG3.transform.Find("Bg2").gameObject;
            Box1 = DBG.transform.GetChild(0).GetChild(0).gameObject;
            Box2 = DBG.transform.GetChild(1).GetChild(0).gameObject;
            _list = new List<int>();
            Util.AddBtnClick(curTrans.Find("diban").GetChild(0).gameObject, btn);
            Bg1Pos = curTrans.Find("BgPos").position;
            Bg2Pos = curTrans.Find("Bg2Pos").position;
            GameInit();
            //GameStart();
        }

        private void CreateYang()
        {
            if (yangname.Count == 0)
            {
                _canJump = false;
                DBG.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                DBG2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                DBG3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                SpineManager.instance.DoAnimation(yang, "yang" + _yang, true);
                playSuccessSpine();
                return;
            }
            else
            {
                _yang = yangname[Random.Range(0, yangname.Count)];
                yangname.Remove(_yang);
                if (!_start)
                {
                    _start = true;
                    SpineManager.instance.DoAnimation(yang, "yang" + _yang, true);
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                }
                else
                {
                    SpineManager.instance.DoAnimation(yang, "yang" + _yang + "2", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                }
            }
        }
        private void btn(GameObject obj)
        {
            
            if (!_canJump && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2,false);
                _canClick = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "play", false,
                    () =>
                    {
                        _canJump = true;
                        _canClick = true;
                        DBG.GetComponent<Rigidbody2D>().velocity = new Vector2(-600f, 0);
                        DBG2.GetComponent<Rigidbody2D>().velocity = new Vector2(-200f, 0);
                        DBG3.GetComponent<Rigidbody2D>().velocity = new Vector2(-50f, 0);
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "jump", false);
                        SpineManager.instance.DoAnimation(yang, "yang" + _yang + "2", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                        
                    }
                    );
            }
            if (_canJump && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                _canClick = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "jump", false);
                yang.transform.GetChild(0).gameObject.SetActive(false);
                yang.transform.GetChild(0).gameObject.SetActive(true);
                yang.transform.GetChild(1).gameObject.SetActive(false);
                yang.transform.GetChild(0).GetComponent<EventDispatcher>().TriggerEnter2D += touch;
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,5,false);
                SpineManager.instance.DoAnimation(yang, "yang" + _yang + "3", false,
                    () =>
                    {
                         SpineManager.instance.DoAnimation(yang, "yang" + _yang + "2", true);
                        if (mysmz >= 3)
                        {
                            _canClick = false;
                            DBG.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                            DBG2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                            DBG3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                            SpineManager.instance.DoAnimation(yang, "yang" + _yang + "5", false,
                                () =>
                                {
                                    replay();
                                }
                                );
                        }
                        else
                        {
                            _canClick = true;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                            if (_canchange)
                            {
                                _canchange = false;
                                CreateYang();
                            }
                            yang.transform.GetChild(0).gameObject.SetActive(false); yang.transform.GetChild(1).gameObject.SetActive(false); yang.transform.GetChild(1).gameObject.SetActive(true);
                            yang.transform.GetChild(1).GetComponent<EventDispatcher>().TriggerEnter2D += touch; _canwrong = true;
                        }
                    }
                    );
            }
        }
        private void touch(Collider2D other, int time)
        {
            string temp = _yang;
            int tempxem = xemsmz;
            if (temp == "A" && name[Convert.ToInt32(other.transform.parent.name)] == "dj8")
            {
                if (rbool[0])
                {
                    rbool[0] = false;
                    _canchange = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(curTrans.Find("xem").GetChild(tempxem).GetChild(0).gameObject, "gongji2", false,
                        () =>
                        {
                            curTrans.Find("xem").GetChild(tempxem).GetComponent<RawImage>().texture = Bg1.GetComponent<BellSprites>().texture[1];
                            mono.StartCoroutine(Wait(0.2f, () => { curTrans.Find("xem").GetChild(tempxem).gameObject.SetActive(false); }));
                            xemsmz++;
                        }
                        );
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(3).gameObject, "light", false);
                }
            }
            else if (temp == "B" && name[Convert.ToInt32(other.transform.parent.name)] == "dj2")
            {
                if (rbool[1])
                {
                    rbool[1] = false;
                    _canchange = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(curTrans.Find("xem").GetChild(tempxem).GetChild(0).gameObject, "gongji2", false,
                        () =>
                        {
                            curTrans.Find("xem").GetChild(tempxem).GetComponent<RawImage>().texture = Bg1.GetComponent<BellSprites>().texture[1];
                            mono.StartCoroutine(Wait(0.2f, () => { curTrans.Find("xem").GetChild(tempxem).gameObject.SetActive(false); }));
                            xemsmz++;
                        }
                        );
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(3).gameObject, "light", false);
                }
            }
            else if (temp == "C" && name[Convert.ToInt32(other.transform.parent.name)] == "dj6")
            {
                if (rbool[2])
                {
                    rbool[2] = false;
                    _canchange = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(curTrans.Find("xem").GetChild(tempxem).GetChild(0).gameObject, "gongji2", false,
                        () =>
                        {
                            curTrans.Find("xem").GetChild(tempxem).GetComponent<RawImage>().texture = Bg1.GetComponent<BellSprites>().texture[1];
                            mono.StartCoroutine(Wait(0.2f, () => { curTrans.Find("xem").GetChild(tempxem).gameObject.SetActive(false); }));
                            xemsmz++;
                        }
                        );
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(3).gameObject, "light", false);
                }
            }
            else if (temp == "D" && name[Convert.ToInt32(other.transform.parent.name)] == "dj4")
            {
                if (rbool[3])
                {
                    rbool[3] = false;
                    _canchange = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(curTrans.Find("xem").GetChild(tempxem).GetChild(0).gameObject, "gongji2", false,
                        () =>
                        {
                            curTrans.Find("xem").GetChild(tempxem).GetComponent<RawImage>().texture = Bg1.GetComponent<BellSprites>().texture[1];
                            mono.StartCoroutine(Wait(0.2f, () => { curTrans.Find("xem").GetChild(tempxem).gameObject.SetActive(false); }));
                            xemsmz++;
                        }
                        );
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(3).gameObject, "light", false);
                }
            }
            else
            {
                if (name[Convert.ToInt32(other.transform.parent.name)] == "lang")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(0).gameObject, "lang2", false);
                }
                else if (name[Convert.ToInt32(other.transform.parent.name)] == "xema")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(0).gameObject, "xema2", false,
                        () => { SpineManager.instance.DoAnimation(other.transform.parent.GetChild(0).gameObject, "xema", true); }
                        );
                }
                else if (name[Convert.ToInt32(other.transform.parent.name)] == "xemb")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(0).gameObject, "xemb2", false,
                        () => { SpineManager.instance.DoAnimation(other.transform.parent.GetChild(0).gameObject, "xemb", true); }
                        );
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    SpineManager.instance.DoAnimation(other.transform.parent.GetChild(0).gameObject, name[Convert.ToInt32(other.transform.parent.name)], false);
                }
                _canClick = false;
                if(_canwrong)
                {
                    _canwrong = false;
                    if (mysmz < 3)
                    {
                        curTrans.Find("smz").GetChild(mysmz).gameObject.SetActive(false);
                        mysmz++;
                    }
                    SpineManager.instance.DoAnimation(yang, "yang" + _yang + "4", false,
                    () =>
                    {

                        if (mysmz == 3)
                        {
                            DBG.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                            DBG2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                            DBG3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                            SpineManager.instance.DoAnimation(yang, "yang" + _yang + "5", false,
                                () =>
                                {
                                    replay();
                                }
                                );
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(yang, "yang" + _yang + "2", true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                            yang.transform.GetChild(0).gameObject.SetActive(false); yang.transform.GetChild(1).gameObject.SetActive(false); yang.transform.GetChild(1).gameObject.SetActive(true);
                            yang.transform.GetChild(1).GetComponent<EventDispatcher>().TriggerEnter2D += touch; _canClick = true; _canwrong = true;
                        }
                    }
                    );
                }
                
            }
        }


        private void InitBox(GameObject obj)
        {
            mono.StopAllCoroutines();
            string temp = string.Empty;
            int inttemp;
            if (obj.name == "Box")
            {
                _list.Clear();
                for (int i = 0; i < 15; i++)
                {
                    _list.Add(i);
                }
                for (int i = 0; i < Box1.transform.childCount; i++)
                {
                    inttemp = _list[Random.Range(0, _list.Count)];
                    _list.Remove(inttemp);
                    temp = JugleList(inttemp);
                    name[i] = temp;
                    if (temp == "xemb" || temp == "lang")
                    { SpineManager.instance.DoAnimation(Box1.transform.GetChild(i).GetChild(0).gameObject, temp, true);
                        Box1.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                        Box1.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    }
                    else if (temp == "xema")
                    {
                        SpineManager.instance.DoAnimation(Box1.transform.GetChild(i).GetChild(0).gameObject, temp, true);
                        Box1.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        Box1.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    { SpineManager.instance.DoAnimation(Box1.transform.GetChild(i).GetChild(0).gameObject, temp, false);
                        Box1.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        Box1.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    }
                }
                mono.StartCoroutine(BoxRun(obj));
                mono.StartCoroutine(BoxRun2());
                mono.StartCoroutine(BoxRun3());
            }
            if (obj.name == "Box2")
            {
                for (int i = 0; i < Box2.transform.childCount; i++)
                {
                    inttemp = _list[Random.Range(0, _list.Count)];
                    _list.Remove(inttemp);
                    temp = JugleList(inttemp);
                    name[7 + i] = temp;
                    if (temp == "xemb" || temp == "lang")
                    { SpineManager.instance.DoAnimation(Box2.transform.GetChild(i).GetChild(0).gameObject, temp, true);
                        Box2.transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
                        Box2.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    }
                    else if (temp == "xema")
                    {
                        SpineManager.instance.DoAnimation(Box2.transform.GetChild(i).GetChild(0).gameObject, temp, true);
                        Box2.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        Box2.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    { SpineManager.instance.DoAnimation(Box2.transform.GetChild(i).GetChild(0).gameObject, temp, false);
                        Box2.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                        Box2.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    }
                }
                mono.StartCoroutine(BoxRun(obj));
                mono.StartCoroutine(BoxRun2());
                mono.StartCoroutine(BoxRun3());
            }
        }
        private string JugleList(int number)
        {
            string temp = string.Empty;
            switch (number)
            {
                case 0:
                    temp = "xema";
                    break;
                case 1:
                    temp = "dj1";
                    break;
                case 2:
                    temp = "dj2";
                    break;
                case 3:
                    temp = "dj3";
                    break;
                case 4:
                    temp = "dj4";
                    break;
                case 5:
                    temp = "dj5";
                    break;
                case 6:
                    temp = "dj6";
                    break;
                case 7:
                    temp = "dj7";
                    break;
                case 8:
                    temp = "dj8";
                    break;
                case 9:
                    temp = "dj" + Random.Range(1, 9);
                    break;
                case 10:
                    temp = "dj" + Random.Range(1, 9);
                    break;
                case 11:
                    temp = "lang";
                    break;
                case 12:
                    temp = "lang";
                    break;
                case 13:
                    temp = "xema";
                    break;
                case 14:
                    temp = "xemb";
                    break;

            }
            return temp;
        }


        #region 初始化和游戏开始方法

        private void GameInit()
        {
            
            replay();

            talkIndex = 1;
            //田丁初始化
            //TDGameInit();
        }

        void GameStart()
        {
            
            _canClick = true;
            isPlaying = false;
            //田丁开始游戏
            //TDGameStart();

        }

        private void replay()
        {
            _canwrong = true;
            for (int i = 0; i < 4; i++)
            {
                rbool[i] = true;
            }
            for (int i = 0; i < Box1.transform.childCount; i++)
            {
                Box1.transform.GetChild(i).GetChild(3).GetComponent<SkeletonGraphic>().Initialize(true);
                Box1.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            }
            for (int i = 0; i < Box2.transform.childCount; i++)
            {
                Box2.transform.GetChild(i).GetChild(3).GetComponent<SkeletonGraphic>().Initialize(true);
                Box2.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            }
            isPlaying = false;
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            yang.GetComponent<SkeletonGraphic>().Initialize(true);
            DBG.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            DBG2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            DBG3.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Bg1.transform.position = Bg1Pos;
            Bg11.transform.position = Bg1Pos;
            Bg21.transform.position = Bg1Pos;
            Bg2.transform.position = Bg2Pos;
            Bg12.transform.position = Bg2Pos;
            Bg22.transform.position = Bg2Pos;
            SpineManager.instance.DoAnimation(curTrans.Find("diban").GetChild(0).GetChild(0).gameObject, "play", false);

            _canchange = false;
            _start = false;
            xemsmz = 0;
            mysmz = 0;
            _newbg1 = 0;
            _newbg2 = 0;
            yangname = new List<string>() { "A", "B", "C", "D" };
            yang.transform.GetChild(0).gameObject.SetActive(false);
            yang.transform.GetChild(1).gameObject.SetActive(true);
            yang.transform.GetChild(1).GetComponent<EventDispatcher>().TriggerEnter2D += touch;

            for (int i = 0; i < curTrans.Find("smz").childCount ; i++)
            {
                curTrans.Find("smz").GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < curTrans.Find("xem").childCount; i++)
            {
                curTrans.Find("xem").GetChild(i).gameObject.SetActive(true);
                curTrans.Find("xem").GetChild(i).GetComponent<RawImage>().texture = Bg1.GetComponent<BellSprites>().texture[0];
                curTrans.Find("xem").GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            }

            CreateYang();
            InitBox(Box1);
            InitBox(Box2);

            _canClick = true;
            _canJump = false;
        }

        IEnumerator BoxRun(GameObject obj)
        {
            while (true)
            {
                //Debug.LogError(Bg2.transform.position.x);
                yield return new WaitForSeconds(0.01f);
                if (obj.name == "Box2" && Bg2.transform.position.x < Bg1Pos.x)
                {
                    Bg1.transform.position = new Vector2((Bg2Pos.x - 17f), Bg2Pos.y); ;
                    InitBox(Box1);
                    break;
                }
                if (obj.name == "Box" && Bg1.transform.position.x < Bg1Pos.x)
                {
                    Bg2.transform.position = new Vector2((Bg2Pos.x - 17f), Bg2Pos.y); ;
                    InitBox(Box2);
                    break;
                }

            }
            yield break;
        }

        IEnumerator BoxRun2()
        {
            while (true)
            {
                //Debug.LogError(Bg2.transform.position.x);
                yield return new WaitForSeconds(0.01f);
                if (_newbg1 == 0 && Bg12.transform.position.x < Bg1Pos.x)
                {
                    Bg11.transform.position = new Vector2((Bg2Pos.x - 11f), Bg2Pos.y);
                    _newbg1 = 1;
                    break;
                }
                if (_newbg1 == 1 && Bg11.transform.position.x < Bg1Pos.x)
                {
                    Bg12.transform.position = new Vector2((Bg2Pos.x - 11f), Bg2Pos.y);
                    _newbg1 = 0;
                    break;
                }
            }
            yield break;
        }

        IEnumerator BoxRun3()
        {
            while (true)
            {
                //Debug.LogError(Bg2.transform.position.x);
                yield return new WaitForSeconds(0.01f);
                if (_newbg2 == 0 && Bg22.transform.position.x < Bg1Pos.x)
                {
                    Bg21.transform.position = new Vector2((Bg2Pos.x - 12f), Bg2Pos.y);
                    _newbg2 = 1;
                    break;
                }
                if (_newbg2 == 1 && Bg21.transform.position.x < Bg1Pos.x)
                {
                    Bg22.transform.position = new Vector2((Bg2Pos.x - 12f), Bg2Pos.y);
                    _newbg2 = 0;
                    break;
                }
            }
            yield break;
        }

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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

            LRBtnUpdate();
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
                {
                    ShowDialogue("", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
                        {
                            ShowDialogue("", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));
            });
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
                    mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE,1,null,
                        () => { mask.SetActive(false);bd.SetActive(false); GameStart(); }
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
            //TDLoadPageBar();
            ////加载材料环节
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
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        anyBtns.gameObject.SetActive(false);
                        bd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE,0,null,
                            () => { SoundManager.instance.ShowVoiceBtn(true); }
                            ));
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); replay();talkIndex = 1; //SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            mono.StartCoroutine(Wait(3.3f,()=> { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); }));
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
