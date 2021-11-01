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

    public class TD3454Part5
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
        private GameObject _dialogue;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Vector2 _roleStartPos, _roleEndPos, _enemyStartPos, _enemyEndPos;

        private RectTransform _devilRect;

        private RectTransform _tianTianRect;

        private Text _devilRectTxt;

        private Text _tianTianRectTxt;

        private List<string> _dialogues;

        private bool _isPlaying;

        private List<Vector2> xemposList;

        private Transform levelPos;

        private List<mILDrager> _MILDragers;
        private List<mILDrager> _MILDragers2;

        private List<mILDrager> _MILDragers_2;
        private List<mILDrager> _MILDragers2_2;

        private List<mILDrager> _MILDragers_3;
        private List<mILDrager> _MILDragers2_3;
        private Transform drag1;
        private Transform drop1;
        private Transform drag2;

        private Transform drag1_2;
        private Transform drop1_2;
        private Transform drag2_2;

        private Transform drag1_3;
        private Transform drop1_3;
        private Transform drag2_3;

        private Transform xem;
        private Transform bird;
        private Transform jindutiao;

        private Transform xempos;

        private Transform Level;
        private Transform Bg;
        private Transform mud;
        private Transform star;

        private int levelIndex;
        private float count;
        private float fillAmount;
        private int GetCount;
        private int randomLevel;

        private int temp;

        private int switch_case_index;

        private bool isEnd;
        private bool canClickXem;

        private Coroutine ab;
        private Coroutine ac;

        float flag;
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

            _devilRect = curTrans.GetRectTransform("dialogue/devil");

            _tianTianRect = curTrans.GetRectTransform("dialogue/tianTian");

            _devilRectTxt = curTrans.GetText("dialogue/devil/Text");

            _tianTianRectTxt = curTrans.GetText("dialogue/tianTian/Text");

            _dialogue = curTrans.GetGameObject("dialogue");

            levelPos = curTrans.Find("LevelPos");
            drag1 = curTrans.Find("Level/0/right/level1");
            drag2 = curTrans.Find("Level/0/right/level2");
            drop1 = curTrans.Find("Level/0/left/level1");

            drag1_2 = curTrans.Find("Level/1/right/level1");
            drag2_2 = curTrans.Find("Level/1/right/level2");
            drop1_2 = curTrans.Find("Level/1/left/level1");

            drag1_3 = curTrans.Find("Level/2/right/level1");
            drag2_3 = curTrans.Find("Level/2/right/level2");
            drop1_3 = curTrans.Find("Level/2/left/level1");

            xempos = curTrans.Find("xempos");
            mud = curTrans.Find("mud");
            xem = curTrans.Find("xem");
            bird = curTrans.Find("Bird");
            jindutiao = curTrans.Find("jindutiao");
            Level = curTrans.Find("Level");
            Bg = curTrans.Find("BG");
            star = curTrans.Find("star");
            ab = null; ac = null;
            randomLevel = 0;
            fillAmount = 1;

            levelIndex = 0; count = 0; switch_case_index = 0;
            GetCount = 0;
            temp = 50;

            isEnd = false;
            canClickXem = true;

            GameInit();

            AddDragEvent();
            AddXemPosList();
            AddBtnClick();
            GameStart();
        }

        void InitData()
        {
            flag = 0;
            DOTween.KillAll();
            _isPlaying = true;
            isEnd = false;

            _devilRectTxt.text = string.Empty;
            _tianTianRectTxt.text = string.Empty;


            _dialogues = new List<string> {
             "我讨厌一切，是不会让你们得逞的",
             "加油，我们一定会战胜小恶魔布鲁鲁的"
             };
            _roleStartPos = new Vector2(-2170, 539);
            _roleEndPos = new Vector2(-960, 539);
            _enemyStartPos = new Vector2(200, 540);
            _enemyEndPos = new Vector2(-994, 540);
            SetPos(_devilRect, _enemyStartPos);
            SetPos(_tianTianRect, _roleStartPos);
            Bg.Find("bg").gameObject.SetActive(true);
            Bg.Find("bg2").gameObject.SetActive(false);
            Level.gameObject.SetActive(true);
            bird.gameObject.SetActive(false);
            mud.gameObject.SetActive(false);
            xem.gameObject.SetActive(false);
            jindutiao.gameObject.SetActive(false);
            jindutiao.Find("gray/blue/logo").GetRectTransform().anchoredPosition = new Vector2(490, -16);
            jindutiao.Find("gray/blue").GetComponent<Image>().fillAmount = 1;
            for (int i = 0; i < 3; i++)
            {
                Level.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 1; i <= 10; i++)
            {
                //位置初始化
                Level.Find("0/left/level1").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("0/left/level1").Find(i.ToString()).GetRectTransform().anchoredPosition;
                Level.Find("1/left/level1").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("1/left/level1").Find(i.ToString()).GetRectTransform().anchoredPosition;
                Level.Find("2/left/level1").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("2/left/level1").Find(i.ToString()).GetRectTransform().anchoredPosition;
                //图片初始化
                Level.Find("0/left/level1").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("0/left/level1").Find(i.ToString()).GetComponent<BellSprites>().sprites[1];
                Level.Find("1/left/level1").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("1/left/level1").Find(i.ToString()).GetComponent<BellSprites>().sprites[1];
                Level.Find("2/left/level1").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("2/left/level1").Find(i.ToString()).GetComponent<BellSprites>().sprites[1];
                //子物体初始化
                Level.Find("0/left/level1").Find(i.ToString()).GetChild(0).gameObject.SetActive(false);
                Level.Find("1/left/level1").Find(i.ToString()).GetChild(0).gameObject.SetActive(false);
                Level.Find("2/left/level1").Find(i.ToString()).GetChild(0).gameObject.SetActive(false);
            }
            for (int i = 1; i <= 5; i++)
            {//位置初始化
                Level.Find("0/right/level1").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("0/right/level1").Find(i.ToString()).GetRectTransform().anchoredPosition;
                Level.Find("0/right/level2").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("0/right/level2").Find(i.ToString()).GetRectTransform().anchoredPosition;

                Level.Find("1/right/level1").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("1/right/level1").Find(i.ToString()).GetRectTransform().anchoredPosition;
                Level.Find("1/right/level2").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("1/right/level2").Find(i.ToString()).GetRectTransform().anchoredPosition;

                Level.Find("2/right/level1").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("2/right/level1").Find(i.ToString()).GetRectTransform().anchoredPosition;
                Level.Find("2/right/level2").Find(i.ToString()).GetRectTransform().anchoredPosition = levelPos.Find("2/right/level2").Find(i.ToString()).GetRectTransform().anchoredPosition;
                //图片初始化
                Level.Find("0/right/level1").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("0/right/level1").Find(i.ToString()).GetComponent<BellSprites>().sprites[0];
                Level.Find("0/right/level2").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("0/right/level2").Find(i.ToString()).GetComponent<BellSprites>().sprites[0];

                Level.Find("1/right/level1").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("1/right/level1").Find(i.ToString()).GetComponent<BellSprites>().sprites[0];
                Level.Find("1/right/level2").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("1/right/level2").Find(i.ToString()).GetComponent<BellSprites>().sprites[0];

                Level.Find("2/right/level1").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("2/right/level1").Find(i.ToString()).GetComponent<BellSprites>().sprites[0];
                Level.Find("2/right/level2").Find(i.ToString()).GetComponent<CustomImage>().sprite = Level.Find("2/right/level2").Find(i.ToString()).GetComponent<BellSprites>().sprites[0];
                //自身需要显示
                Level.Find("0/right/level1").Find(i.ToString()).gameObject.SetActive(true);
                Level.Find("0/right/level2").Find(i.ToString()).gameObject.SetActive(true);
                Level.Find("1/right/level1").Find(i.ToString()).gameObject.SetActive(true);
                Level.Find("1/right/level2").Find(i.ToString()).gameObject.SetActive(true);
                Level.Find("2/right/level1").Find(i.ToString()).gameObject.SetActive(true);
                Level.Find("2/right/level2").Find(i.ToString()).gameObject.SetActive(true);
            }
            for (int i = 0; i < 3; i++)
            {
                Level.Find(i.ToString()).Find("right/level2").gameObject.SetActive(false);
                Level.Find(i.ToString()).Find("right/level1").gameObject.SetActive(true);
            }
            for (int i = 0; i < 3; i++)
            {
                bird.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 6; i <= 10; i++)
            {
                Level.Find("0/left/level1").Find(i.ToString()).gameObject.SetActive(false);
                Level.Find("1/left/level1").Find(i.ToString()).gameObject.SetActive(false);
                Level.Find("2/left/level1").Find(i.ToString()).gameObject.SetActive(false);
            }
            count = 0; GetCount = 0; levelIndex = 0; temp = 50;

            Bg.Find("bg/frame").GetRectTransform().localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Bg.Find("bg/frame").gameObject.SetActive(false);
            Bg.Find("bg/yuan").GetRectTransform().anchoredPosition = new Vector2(442, 0);
            star.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }
        void AddXemPosList()
        {
            xemposList = new List<Vector2>();
            for (int i = 0; i < xempos.childCount; i++)
            {
                xemposList.Add(xempos.Find((i + 1).ToString()).GetRectTransform().anchoredPosition);
            }
        }
        private void AddDragEvent()
        {
            //level1
            _MILDragers = new List<mILDrager>();
            for (int i = 1; i <= drag1.childCount; i++)
            {
                _MILDragers.Add(drag1.Find(i.ToString()).GetComponent<mILDrager>());
            }
            _MILDragers2 = new List<mILDrager>();
            for (int i = 1; i <= drag2.childCount; i++)
            {
                _MILDragers2.Add(drag2.Find(i.ToString()).GetComponent<mILDrager>());
            }
            //level2
            _MILDragers_2 = new List<mILDrager>();
            for (int i = 1; i <= drag1_2.childCount; i++)
            {
                _MILDragers_2.Add(drag1_2.Find(i.ToString()).GetComponent<mILDrager>());
            }
            _MILDragers2_2 = new List<mILDrager>();
            for (int i = 1; i <= drag2_2.childCount; i++)
            {
                _MILDragers2_2.Add(drag2_2.Find(i.ToString()).GetComponent<mILDrager>());
            }
            //level3
            _MILDragers_3 = new List<mILDrager>();
            for (int i = 1; i <= drag1_3.childCount; i++)
            {
                _MILDragers_3.Add(drag1_3.Find(i.ToString()).GetComponent<mILDrager>());
            }
            _MILDragers2_3 = new List<mILDrager>();
            for (int i = 1; i <= drag2_3.childCount; i++)
            {
                _MILDragers2_3.Add(drag2_3.Find(i.ToString()).GetComponent<mILDrager>());
            }


            foreach (var a in _MILDragers)
            {
                a.SetDragCallback(DragStart, null, DragEnd);
            }
            foreach (var a in _MILDragers2)
            {
                a.SetDragCallback(DragStart, null, DragEnd);
            }
            foreach (var a in _MILDragers_2)
            {
                a.SetDragCallback(DragStart, null, DragEnd);
            }
            foreach (var a in _MILDragers2_2)
            {
                a.SetDragCallback(DragStart, null, DragEnd);
            }
            foreach (var a in _MILDragers_3)
            {
                a.SetDragCallback(DragStart, null, DragEnd);
            }
            foreach (var a in _MILDragers2_3)
            {
                a.SetDragCallback(DragStart, null, DragEnd);
            }
        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            Input.multiTouchEnabled = false;
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();

            _dTT.Hide();

            _sTT.Hide();

            _dialogue.Hide();

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
            //   _mono.StartCoroutine(changeTime());

        }
        //点击特效--星星的出现
        private void ShowStar()
        {
            star.position = Input.mousePosition;
            star.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(star.GetChild(0).gameObject, "guang2", false, () =>
            {
                SpineManager.instance.DoAnimation(star.GetChild(0).gameObject, "kong", false);
            });
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
                        PlayBgm(0);//ToDo...改BmgIndex
                        _startSpine.Hide();

                        //ToDo...
                        _dialogue.Show();
                        XemDialogue(0, 1, () =>
                        {
                            TianTianDialogue(1, 4, () =>
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            });
                        });


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
                    _dialogue.SetActive(false);
                    _sTT.SetActive(true);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 2, _sTT, null, () =>
                    {
                        _sTT.SetActive(false);
                        StartGame();
                    }, RoleType.Child));

                    break;
            }
            _talkIndex++;
        }

        void AddBtnClick()
        {
            Util.AddBtnClick(xem.gameObject, OnClickXem);
        }

        private void RandomLevel()
        {
            randomLevel = Random.Range(0, 3);
            Level.Find(randomLevel.ToString()).gameObject.SetActive(true);
        }

        //这是游戏环节，顶上的计时条，通过调节temp来改变时间
        IEnumerator changeTime()
        {
            while (temp > 0)
            {
                yield return new WaitForSeconds(1);
                temp--;
                JinDuTiaoMove();

                if (temp == 0)
                {
                    isEnd = true;
                    xem.gameObject.SetActive(false);
                    GameSuccess();
                }

            }
        }
        #region 游戏逻辑

        //小恶魔随机出现方法
        private void RandomAppearXem()
        {
            int random = Random.Range(0, xempos.childCount);
            xem.GetRectTransform().anchoredPosition = xemposList[random];

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);

            Delay(0.2f, ()=>
            {
                SpineManager.instance.DoAnimation(mud.GetChild(random).gameObject, "tuA" + (random + 1), false); 
            });

            canClickXem = true;

            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem1", false, () =>
            {
                   xem.GetComponent<Empty4Raycast>().raycastTarget = true;
                   SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem2", true);
                   //int randomTime = Random.Range(1,3);
            });

            ac = _mono.StartCoroutine(IEDelay(3.5f, () =>
              {
                  canClickXem = false;
                  SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem3", false, () =>
                  {
                      SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);

                      if (isEnd == false) RandomAppearXem();
                  });
              }));

            //Delay(3.5f, () =>
            //{



            //});

        }

        //点击小恶魔事件
        private void OnClickXem(GameObject obj)
        {
            if (canClickXem == true)
            {
                GetCount++;
                if (ab != null)
                {
                    StopCoroutines(ab);
                }
                if (ac != null)
                {
                    StopCoroutines(ac);
                }

                string aniName = "";
                switch (switch_case_index)
                {
                    case 0:
                        aniName = "C";
                        break;
                    case 1:
                        aniName = "B";
                        break;
                    case 2:
                        aniName = "A";
                        break;
                }

                GetCount++;
                isEnd = true;

                obj.GetComponent<Empty4Raycast>().raycastTarget = false;

                Delay(0.3f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);

                    SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem4", false, () =>
                     {
                         //JinDuTiaoMove();
                         xem.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                         xem.GetComponent<Empty4Raycast>().raycastTarget = false;
                         ab = _mono.StartCoroutine(IEDelay(2.5f, () =>
                         {
                            isEnd = false;
                            RandomAppearXem();
                         }));

                         //Delay(2.5f,()=>
                         //{


                         //});
                     });
                });

                bird.Find(randomLevel + "").GetRectTransform().anchoredPosition = new Vector2(obj.transform.GetRectTransform().anchoredPosition.x + 230, obj.transform.GetRectTransform().anchoredPosition.y + 120);

                SpineManager.instance.DoAnimation(bird.Find(randomLevel.ToString()).GetChild(0).gameObject, "tuniao" + aniName + Random.Range(3, 5), false, () =>
                {
                    Delay(1f, () =>
                    {
                        bird.Find(randomLevel + "").GetRectTransform().anchoredPosition = new Vector2(1649, 77);

                        SpineManager.instance.DoAnimation(bird.Find(randomLevel.ToString()).GetChild(0).gameObject, "tuniao" + aniName + "2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(bird.Find(randomLevel.ToString()).GetChild(0).gameObject, "tuniao" + aniName, true);
                        });
                    });
                });
            }
        }

        /// <summary>
        /// 开始对话
        /// </summary>
        private void StartDialogue()
        {
            //ToDo...  

            //测试代码记得删
            //Delay(4,GameSuccess);			 
        }

        //游戏环节顶上的进度条扣除的方法
        private void JinDuTiaoMove()
        {
            count++;
            fillAmount = jindutiao.Find("gray").Find("blue").gameObject.GetComponent<Image>().fillAmount - 0.02f;
            jindutiao.Find("gray").Find("blue").gameObject.GetComponent<Image>().fillAmount = fillAmount;
            UpdataLogo();
        }

        //游戏环节顶上的进度条扣除的方法
        private void UpdataLogo()
        {
            jindutiao.Find("gray").Find("blue").Find("logo").GetRectTransform().anchoredPosition = new Vector2(-(1 - fillAmount) * 980 + 490, -16);
        }

        //第一个拖拽环节
        private void DragStart(Vector3 position, int type, int index)
        {
            List<mILDrager> a = null;
            List<mILDrager> b = null;
            Transform Drop1 = null;
            Transform Drag1 = null;
            Transform Drag2 = null;
            switch (switch_case_index)
            {
                case 0:
                    a = _MILDragers;
                    b = _MILDragers2;
                    Drop1 = drop1;
                    Drag1 = drag1;
                    Drag2 = drag2;
                    break;
                case 1:
                    a = _MILDragers_2;
                    b = _MILDragers2_2;
                    Drop1 = drop1_2;
                    Drag1 = drag1_2;
                    Drag2 = drag2_2;
                    break;
                case 2:
                    a = _MILDragers_3;
                    b = _MILDragers2_3;
                    Drop1 = drop1_3;
                    Drag1 = drag1_3;
                    Drag2 = drag2_3;
                    break;
            }
            Debug.Log(levelIndex);
            if (levelIndex == 0)
            {
                a[index - 1].transform.SetAsLastSibling();
                Drop1.Find(index.ToString()).GetComponent<CustomImage>().raycastTarget = true;
                Drag1.Find(index.ToString()).GetComponent<CustomImage>().sprite = Drag1.Find(index.ToString()).GetComponent<BellSprites>().sprites[1];
                Drag1.Find(index.ToString()).GetComponent<CustomImage>().SetNativeSize();
            }
            else if (levelIndex == 1)
            {
                b[index - 6].transform.SetAsLastSibling();
                Drop1.Find((index - 5).ToString()).GetComponent<CustomImage>().raycastTarget = true;
                Drag2.Find((index - 5).ToString()).GetComponent<CustomImage>().sprite = Drag2.Find((index - 5).ToString()).GetComponent<BellSprites>().sprites[1];
                Drag2.Find((index - 5).ToString()).GetComponent<CustomImage>().SetNativeSize();
            }
            //  _MILDragers[index-1].transform.position = Input.mousePosition;


        }

        //第一个拖拽环节
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {

            List<mILDrager> a = null;
            List<mILDrager> b = null;
            Transform Drop1 = null;
            Transform Drag1 = null;
            Transform Drag2 = null;

            switch (switch_case_index)
            {
                case 0:
                    a = _MILDragers;
                    b = _MILDragers2;
                    Drop1 = drop1;
                    Drag1 = drag1;
                    Drag2 = drag2;
                    break;
                case 1:
                    a = _MILDragers_2;
                    b = _MILDragers2_2;
                    Drop1 = drop1_2;
                    Drag1 = drag1_2;
                    Drag2 = drag2_2;
                    break;
                case 2:
                    a = _MILDragers_3;
                    b = _MILDragers2_3;
                    Drop1 = drop1_3;
                    Drag1 = drag1_3;
                    Drag2 = drag2_3;
                    break;
            }

            bool isTrue = Drop1.Find("" + index).GetComponent<PolygonCollider2D>().OverlapPoint(Input.mousePosition);

            if (isTrue)
            {
                count++;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                ShowStar();
                if (levelIndex == 0)
                {
                    a[index - 1].gameObject.SetActive(false);
                    //这一段 需要改 思路：在drop下增加一个子物体 rawimage 图片是发光的图片 
                    //  Drop1.Find(index.ToString()).GetComponent<CustomImage>().sprite = Drop1.Find(index.ToString()).GetComponent<BellSprites>().sprites[2];
                    // Drop1.Find(index.ToString()).GetComponent<CustomImage>().SetNativeSize();
                    //
                    Drop1.Find(index.ToString()).GetChild(0).gameObject.SetActive(true);
                    Delay(0.5f, () =>
                    {
                        Drop1.Find(index.ToString()).GetChild(0).gameObject.SetActive(false);
                        Drop1.Find(index.ToString()).GetComponent<CustomImage>().sprite = Drop1.Find(index.ToString()).GetComponent<BellSprites>().sprites[0];
                        Drop1.Find(index.ToString()).GetComponent<CustomImage>().SetNativeSize();

                    });

                }
                else if (levelIndex == 1)
                {
                    b[index - 6].gameObject.SetActive(false);
                    Drop1.Find((index).ToString()).GetComponent<CustomImage>().sprite = Drop1.Find((index).ToString()).GetComponent<BellSprites>().sprites[2];
                    Drop1.Find((index).ToString()).GetComponent<CustomImage>().SetNativeSize();
                    Delay(0.5f, () =>
                    {

                        Drop1.Find((index).ToString()).GetComponent<CustomImage>().sprite = Drop1.Find((index).ToString()).GetComponent<BellSprites>().sprites[0];
                        Drop1.Find((index).ToString()).GetComponent<CustomImage>().SetNativeSize();

                    });
                }
            }
            else
            {
                //失败闪烁
                if(++flag == 3)
                {
                    flag = 0;

                    CustomImage dropCustom = Drop1.Find(index + "").GetComponent<CustomImage>();
                    BellSprites bellSprites = Drop1.Find(index + "").GetComponent<BellSprites>();

                    dropCustom.sprite = bellSprites.sprites[3];
                    dropCustom.SetNativeSize();

                    Delay(1f, () =>
                    {
                        dropCustom.sprite = bellSprites.sprites[1];
                        dropCustom.SetNativeSize();
                    });
                }

                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                if (levelIndex == 0)
                {
                    a[index - 1].DoReset();

                    Drag1.Find((index).ToString()).GetComponent<CustomImage>().sprite = Drag1.Find((index).ToString()).GetComponent<BellSprites>().sprites[0];
                }
                else if (levelIndex == 1)
                {
                    b[index - 6].DoReset();
                    Drag2.Find((index - 5).ToString()).GetComponent<CustomImage>().sprite = Drag2.Find((index - 5).ToString()).GetComponent<BellSprites>().sprites[0];
                }


            }
            if (count == 5)
            {
                flag = 0;

                levelIndex++;
                if (levelIndex == 1)
                {
                    count = 0;
                    Delay(1.5f, () =>
                     {

                         Drop1.Find("6").gameObject.SetActive(true);
                         Drop1.Find("7").gameObject.SetActive(true);
                         Drop1.Find("8").gameObject.SetActive(true);
                         Drop1.Find("9").gameObject.SetActive(true);
                         Drop1.Find("10").gameObject.SetActive(true);
                         Drag1.gameObject.SetActive(false); Drag2.gameObject.SetActive(true);

                     });

                }
                //从拖拽环节到游戏环节
                if (levelIndex == 2)
                {
                    //下一关
                    Debug.Log("NexT");
                    count = 0;
                    Delay(2.5f, () =>
                     {
                         _mask.SetActive(true);
                         _sTT.SetActive(true);
                         _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 3, _sTT, null, () =>
                         {
                             _mask.SetActive(false);
                             _sTT.SetActive(false);

                             PlayBgm(1);
                             xem.gameObject.SetActive(true);
                             RandomAppearXem();
                             jindutiao.Find("gray/blue/logo").GetComponent<RawImage>().texture = jindutiao.Find("gray/blue/logo").GetComponent<BellSprites>().texture[randomLevel];
                             _mono.StartCoroutine(changeTime());
                             Bg.Find("bg").gameObject.SetActive(false);
                             Level.gameObject.SetActive(false);

                             Bg.Find("bg2").gameObject.SetActive(true);
                             mud.gameObject.SetActive(true);

                             for(int i = 0; i < mud.childCount; ++i)
                             {
                                 mud.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
                                 SpineManager.instance.DoAnimation(mud.GetChild(i).gameObject, "tuA" + (i + 1) + (i + 1), false);
                             }
                             
                             jindutiao.gameObject.SetActive(true);
                             bird.gameObject.SetActive(true);
                             bird.Find(randomLevel.ToString()).gameObject.SetActive(true);

                         }, RoleType.Child));


                     });


                }
            }

        }

        //开始的DOTWEEN效果
        private void StartDoTween()
        {
            Bg.Find("bg/frame").gameObject.SetActive(true);
            Bg.Find("bg/yuan").GetRectTransform().DOAnchorPosX(-433, 1.5f);
            Bg.Find("bg/frame").GetRectTransform().DOScale(1, 1.5f).OnComplete(() =>
            {
                RandomLevel();
                switch_case_index = randomLevel;
            });
        }

        /// <summary>
        /// 小恶魔对话
        /// </summary>		
        private void XemDialogue(int dialogueIndex, int soundIndex, Action callBack)
        {
            SetMove(_devilRect, _enemyEndPos, 1.0f, () =>
            {
                ShowDialogue(_dialogues[dialogueIndex], _devilRectTxt);
                Delay(PlaySound(soundIndex), callBack);
            });
        }

        /// <summary>
        /// 田田对话
        /// </summary>	
        private void TianTianDialogue(int dialogueIndex, int soundIndex, Action callBack)
        {
            SetMove(_tianTianRect, _roleEndPos, 1.0f, () =>
            {
                ShowDialogue(_dialogues[dialogueIndex], _tianTianRectTxt);
                Delay(PlaySound(soundIndex), callBack);
            });
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            StartDoTween();


            //测试代码记得删
            //Delay(4,GameSuccess);
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
                        PlayBgm(0); //ToDo...改BmgIndex
                        GameInit();
                        //ToDo...						
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
                        _dTT.Show();
                        BellSpeck(_dTT, 0, null, null, RoleType.Child);

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
            _successSpine.transform.Find("Text").gameObject.SetActive(true);
            _successSpine.transform.Find("Text").GetComponent<Text>().text = GetCount.ToString();
            PlaySpine(_successSpine, "3-5", () => { PlaySpine(_successSpine, "3-5-2"); });
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
