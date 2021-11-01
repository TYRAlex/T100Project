using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD3464Part1
    {
        #region 常用变量
        GameObject curCanvas;
        GameObject mask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;
        GameObject nextButton;

        Transform ddTra;
        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        int flag;
        int preNum;
        //int[] demonsIndex;

        bool isEnter;
        bool isStop;
        bool isMove;
        bool isCreate;
        bool isEnd;
        bool[][] isDraweds;

        List<RawImage> lines;

        mILDrager[][] dragers;

        Transform starsTra;
        Transform game1Tra;
        Transform game2Tra;
        Transform lightTra;
        Transform ballsTra;

        RectTransform bgRect;
        RectTransform jugglesRect;
        RectTransform lineRect;
        RectTransform starRect;
        RectTransform earthRect;
        RectTransform uiRect;
        RectTransform bg1Rect;
        RectTransform bg2Rect;

        RawImage blackRaw;

        Image processImage;

        GameObject fail;
        GameObject unDragableMask;
        GameObject end;

        Vector2 staticPosition;

        List<GameObject> demonList;
        List<GameObject> celestialList;
        List<GameObject> ballsList;

        mILDrager drager;
        #endregion

        void Start(object o)
        {
            #region 通用初始化
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            #endregion

            LoadMask();

            LoadGame();//加载

            MaskInit();

            GameInit();

            MaskStart();
        }

        void LoadMask()
        {
            //unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;

            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;

            ddTra = maskTra.Find("DD");

            nextButton = maskTra.Find("NextButton").gameObject;

            successSpine = maskTra.Find("successSpine").gameObject;

            caidaiSpine = maskTra.Find("caidaiSpine").gameObject;

            btn01 = maskTra.Find("Btns/0").gameObject;
            btn02 = maskTra.Find("Btns/1").gameObject;
            btn03 = maskTra.Find("Btns/2").gameObject;

            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn02, Win);
            Util.AddBtnClick(btn03, GamePlay);
            Util.AddBtnClick(nextButton, NextButton);
        }

        void MaskInit()
        {
            //unDragableMask.SetActive(false);
            mask.SetActive(true);

            ddTra.GetChild(0).gameObject.SetActive(false);
            ddTra.GetChild(1).gameObject.SetActive(false);

            nextButton.SetActive(false);
            successSpine.SetActive(false);
            caidaiSpine.SetActive(false);

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);
        }

        void LoadGame()
        {
            bgRect = curCanvasTra.Find("Background").GetRectTransform();

            game1Tra = curCanvasTra.Find("game1");
            game2Tra = curCanvasTra.Find("game2");
            starsTra = game1Tra.Find("stars");
            ballsTra = game2Tra.Find("balls");

            lineRect = starsTra.Find("line").GetRectTransform();
            jugglesRect = game2Tra.Find("juggles").GetRectTransform();
            starRect = game2Tra.Find("UI/process/5").GetRectTransform();
            earthRect = game2Tra.Find("earthDrag").GetRectTransform();
            uiRect = game2Tra.Find("UI").GetRectTransform();
            bg1Rect = bgRect.GetChild(0).GetRectTransform();
            bg2Rect = bgRect.GetChild(1).GetRectTransform();

            blackRaw = curCanvasTra.Find("BlackMask").GetRawImage();

            processImage = game2Tra.Find("UI/process/3").GetImage();

            fail = curCanvasTra.Find("failFrame/fail").gameObject;
            unDragableMask = curCanvasTra.Find("unDragableMask").gameObject;
            end = curCanvasTra.Find("end").gameObject;
            lightTra = curCanvasTra.Find("endlight");
        }

        void GameInit()
        {
            isStop = false;
            isMove = false;

            game1Tra.gameObject.SetActive(true);
            game2Tra.gameObject.SetActive(false);

            InitSpine(lightTra.GetChild(0).gameObject, "", false);
            InitSpine(lightTra.GetChild(1).gameObject, "", false);
            InitSpine(end, "", false);

            //背景
            BgInit(bg1Rect.GetChild(0));
            BgInit(bg2Rect.GetChild(0));

            dragers = new mILDrager[3][];
            isDraweds = new bool[3][];

            //星星
            for (int i = 0; i < starsTra.childCount - 1; i++)
            {
                RectTransform rect = starsTra.Find("star" + (i + 1)).GetRectTransform();
                rect.anchoredPosition = Vector2.zero;
                rect.rotation = Quaternion.Euler(0, 0, 0);
                rect.localScale = Vector2.one;
                rect.gameObject.SetActive(false);

                //拖拽
                dragers[i] = rect.GetComponentsInChildren<mILDrager>();

                //判断数组
                isDraweds[i] = new bool[rect.GetChild(1).childCount];
                for (int j = 0; j < isDraweds[i].Length; j++)
                {
                    isDraweds[i][j] = false;
                }
            }

            //拖拽数组
            for (int i = 0; i < dragers.Length; i++)
            {
                for (int j = 0; j < dragers[i].Length; j++)
                {
                    dragers[i][j].SetDragCallback(DragStart, Drag, DragEnd);
                    dragers[i][j].GetComponent<mILDroper>().SetDropCallBack(DoAfter);
                    dragers[i][j].GetComponent<SkeletonGraphic>().raycastTarget = false;
                }
            }

            lineRect.gameObject.SetActive(false);
            blackRaw.gameObject.SetActive(false);
            end.SetActive(false);

            //复位
            bgRect.anchoredPosition = Vector2.zero;
            jugglesRect.anchoredPosition = Vector2.zero;

            //位置
            bg1Rect.anchoredPosition = Vector2.zero;
            bg2Rect.anchoredPosition = Vector2.up * 1740;

            lines = new List<RawImage>();

            //失败
            fail.SetActive(false);
            unDragableMask.SetActive(false);
            unDragableMask.transform.GetChild(0).gameObject.SetActive(false);

            Util.AddBtnClick(unDragableMask.transform.GetChild(0).gameObject, RePlay);
        }

        //第二关初始化
        void Game2Init()
        {
            DOTween.Kill("process");

            for (int i = 1; i < 7; i++)
            {
                DOTween.Kill("" + i);
            }

            DOTween.Kill("xem");

            mono.StopAllCoroutines();

            isMove = true;
            isEnter = false;
            isPlaying = false;
            isEnd = false;
            flag = 0;

            game1Tra.gameObject.SetActive(false);
            game2Tra.gameObject.SetActive(true);

            //UI
            processImage.fillAmount = 1f;
            starRect.anchoredPosition = new Vector2(60, -7.5f);
            uiRect.gameObject.SetActive(true);

            InitSpine(starRect.gameObject, "star");

            //失败
            fail.SetActive(false);
            unDragableMask.SetActive(false);
            unDragableMask.transform.GetChild(0).gameObject.SetActive(false);

            celestialList = new List<GameObject>();
            demonList = new List<GameObject>();

            InitTrueAndError(celestialList, jugglesRect.GetChild(0));
            InitTrueAndError(demonList, jugglesRect.GetChild(1));

            //地球
            InitSpine(earthRect.GetChild(0).gameObject, "DQ4");
            InitSpine(earthRect.GetChild(1).gameObject, "DQ5");
            earthRect.anchoredPosition = Vector2.up * 100;

            //位置
            bg1Rect.anchoredPosition = Vector2.zero;
            bg2Rect.anchoredPosition = Vector2.up * 1740;

            //倒计时
            ProcessCutDown();

            //碰撞
            EventDispatcher eventDispatcher = earthRect.GetChild(0).GetComponent<EventDispatcher>();
            eventDispatcher.TriggerEnter2D += EarthTriggerEnter2D;
            eventDispatcher.CollisionEnter2D += EarthCollisionEnter2D;
            Physics2D.IgnoreCollision(earthRect.GetComponent<PolygonCollider2D>(), earthRect.GetChild(0).GetComponent<CircleCollider2D>());

            //拖拽
            drager = earthRect.GetComponent<mILDrager>();
            drager.SetDragCallback(EarthDragStart, EarthDrag, EarthDragEnd, EarthOnClick);
            drager.GetComponent<CustomImage>().raycastTarget = true;
            drager.canMove = false;

            //ui球
            ballsList = new List<GameObject>();
            for (int i = 0; i < ballsTra.childCount; i++)
            {
                GameObject obj = ballsTra.GetChild(i).gameObject;
                RectTransform rect = obj.transform.GetRectTransform();

                ballsList.Add(obj);
                obj.SetActive(true);
                rect.GetRawImage().color = Color.white;
                rect.anchoredPosition = new Vector2(125 + i * 200, -100);
            }

            //地球外发光
            Material material = earthRect.GetChild(1).GetComponent<SkeletonGraphic>().material;
            material.SetColor("_Color", new Color(1, 1, 1, 0));

            
        }

        //开始游戏
        void StartGame()
        {
            mask.SetActive(false);

            isPlaying = false;

            //星星连线
            ShowStar(starsTra, 0);
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            InitSpine(btn03, "bf2", false);
        }

        void FixedUpdate()
        {
            BackgroundMove(bg1Rect);
            BackgroundMove(bg2Rect);
        }

        #region 游戏方法
        //显示星星
        void ShowStar(Transform _tra, int _num)
        {
            Transform traChild = _tra.Find("star" + (_num + 1));
            Transform tra1 = traChild.GetChild(0);
            Transform tra2 = traChild.GetChild(1);

            //关闭线
            for (int i = 0; i < tra1.childCount; i++)
            {
                tra1.GetChild(i).gameObject.SetActive(false);
            }

            traChild.gameObject.SetActive(true);

            //星星动画
            for (int i = 0; i < tra2.childCount; i++)
            {
                GameObject obj = tra2.GetChild(i).gameObject;
                InitSpine(obj, "", false);
            }

            mono.StartCoroutine(WaitForAnimation(tra2, 0.2f, () =>
            {
                //闪烁连线一会
                for (int i = 0; i < tra1.childCount; i++)
                {
                    RawImage raw = tra1.GetChild(i).GetRawImage();
                    Twinkle(raw);
                }

                mono.StartCoroutine(WaitFor(3.6f, () =>
                {
                    //能点击
                    for (int i = 0; i < dragers[_num].Length; i++)
                    {
                        dragers[_num][i].GetComponent<SkeletonGraphic>().raycastTarget = true;
                    }

                    //前两关为红线
                    Image image = lineRect.GetImage();
                    BellSprites bellSprites = lineRect.GetComponent<BellSprites>();
                    image.sprite = bellSprites.sprites[_num == 2 ? 1 : 0];

                    lineRect.SetSiblingIndex(_num);
                }));

                //倒计时
                //linesCor = mono.StartCoroutine(TipsCutDown());
            }));

            //List存储线
            /*if (lines.Count != 0) lines.Clear();
            for (int i = 0; i < tra1.childCount; i++)
            {
                lines.Add(tra1.GetChild(i).GetRawImage());
            }*/
        }

        //连完一关
        IEnumerator StarSuccess(int num)
        {
            //记录位置和角度
            float[] angles = new float[3] { 105, -60, 60 };
            Vector2[] poses = new Vector2[3]
            {
                new Vector2(500, 200),
                new Vector2(-510, -240),
                new Vector2(-425, 200)
            };

            //取消点击
            for (int i = 0; i < dragers[num].Length; i++)
            {
                dragers[num][i].GetComponent<SkeletonGraphic>().raycastTarget = false;
            }

            //移动星星动画
            RectTransform rect = starsTra.Find("star" + (num + 1)).GetRectTransform();

            rect.DOAnchorPos(poses[num], 1f).SetEase(Ease.InOutSine);
            rect.DORotate(new Vector3(0, 0, angles[num]), 1f).SetEase(Ease.InOutSine);
            rect.DOScale(Vector2.one * 0.5f, 1.1f).SetEase(Ease.OutBack);

            //如果是最后一组星星追加一个动画
            if (num == 2)
            {
                yield return new WaitForSeconds(0.3f);

                RectTransform rect1 = starsTra.Find("star" + 2).GetRectTransform();

                rect1.DOAnchorPos(new Vector2(25, -180), 1f).SetEase(Ease.InOutSine);
                rect1.DORotate(new Vector3(0, 0, -30), 1f).SetEase(Ease.InOutSine);
            }

            yield return new WaitForSeconds(1f);

            if (++num == 3)
            {
                mono.StartCoroutine(WaitFor(3f, () =>
                {
                    mask.SetActive(true);
                    nextButton.SetActive(true);
                    InitSpine(nextButton, "next2", false);
                }));
            }
            else
            {
                //显示下个星星
                ShowStar(starsTra, num);
            }
        }

        //地球吃到其它星球
        void EarthTriggerEnter2D(Collider2D other, int time)
        {
            if (isEnter) return;
            isEnter = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);

            int num = other.transform.GetSiblingIndex();
            other.gameObject.SetActive(false);
            DOTween.Kill(other.gameObject.name);

            //ui消失与移动
            UIAnimation(ballsList, ballsTra.GetChild(num).gameObject);

            //停止拖拽
            drager.canMove = false;

            //外发光闪烁(懒得写函数了)
            Material material = earthRect.GetChild(1).GetComponent<SkeletonGraphic>().material;

            mono.StartCoroutine(ChangeSpineAlpha(material, 1, 0.25f, () =>
            mono.StartCoroutine(ChangeSpineAlpha(material, 0, 0.25f, () =>
            mono.StartCoroutine(ChangeSpineAlpha(material, 1, 0.25f, () =>
            mono.StartCoroutine(ChangeSpineAlpha(material, 0, 0.25f, () =>
            isEnter = false))))))));

            if (++flag == 4)
            {
                if (isEnd) return;
                isEnd = true;

                mono.StartCoroutine(Success());
            }
        }

        //地球撞到小恶魔
        void EarthCollisionEnter2D(Collision2D c, int time)
        {
            if (isEnter) return;
            isEnter = true;

            float _time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);

            //地球抖动
            c.gameObject.SetActive(false);
            earthRect.DOShakePosition(1f, 30);
            drager.canMove = false;

            mono.StartCoroutine(WaitFor(_time, () => isEnter = false));
        }

        //进度条倒计时
        void ProcessCutDown(float time = 45f)
        {
            processImage.DOFillAmount(0, time).SetEase(Ease.Linear).SetId<Tween>("process");
            starRect.DOAnchorPosX(1000, time).SetEase(Ease.Linear).SetId<Tween>("process").OnComplete(() =>
            {
                drager.canMove = false;
                if (isEnd) return;
                isEnd = true;

                mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
                {
                    game2Tra.gameObject.SetActive(false);
                    fail.SetActive(true);
                    InitSpine(fail, "", false);
                    unDragableMask.SetActive(true);

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                    SpineManager.instance.DoAnimation(fail, "p", false, () =>
                    {
                        unDragableMask.transform.GetChild(0).gameObject.SetActive(true);
                    });
                }));
            });
        }

        //背景滑动
        void BackgroundMove(RectTransform rect)
        {
            if (isStop || !isMove) return;

            float speed = -1740 / 30f;

            float x = rect.anchoredPosition.x;
            float y = rect.anchoredPosition.y + Time.fixedDeltaTime * speed;

            if (y < -1800)
            {
                rect.anchoredPosition = new Vector2(x, 1775);
            }
            else
            {
                rect.anchoredPosition = new Vector2(x, y);
            }
        }

        //重玩
        void RePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
            {
                StartLevel2();
            }));
        }

        //闪烁
        void Twinkle(RawImage _raw, float _time = 3.6f, int _times = 6)
        {
            Color transparent = new Color(1, 1, 1, 0);
            Color translucent = new Color(1, 1, 1, 0.5f);

            _raw.gameObject.SetActive(true);
            _raw.color = transparent;
            _raw.DOColor(translucent, _time / _times).
                SetEase(Ease.InOutSine).
                SetLoops(_times, LoopType.Yoyo).
                SetId<Tween>("" + _raw.name).
                OnComplete(() => _raw.gameObject.SetActive(false));
        }

        //背景初始化
        void BgInit(Transform bgTra)
        {
            Transform bgSpineTra = bgTra.Find("BgSpine");
            //背景
            for (int i = 0; i < bgSpineTra.childCount; i++)
            {
                GameObject obj = bgSpineTra.GetChild(i).gameObject;
                InitSpine(obj, "bj" + (i + 1));
            }

            InitSpine(bgTra.Find("star").gameObject, "bj0");
        }

        //间断生成星球和小恶魔
        IEnumerator CreateObj(float deltaTime = 2f)
        {
            isCreate = true;
            WaitForSeconds wait = new WaitForSeconds(deltaTime);

            while (isCreate)
            {
                //随机决定生成小恶魔还是星球
                int temp = Random.Range(0, 2);
                if (demonList.Count == 0) temp = 0;
                List<GameObject> objList = temp == 0 ? celestialList : demonList;

                int n = objList.Count;

                if (n != 0)
                {
                    int num = Random.Range(0, n);
                    ObjMove(objList, num);
                }

                yield return wait;
            }
        }

        //小恶魔或星球移动
        void ObjMove(List<GameObject> objList, int num)
        {
            GameObject obj = objList[num];
            RectTransform rect = obj.transform.GetRectTransform();

            objList.RemoveAt(num);
            obj.SetActive(true);

            if (obj.name == "xem")
            {
                InitSpine(obj, "xem");
            }
            else
            {
                InitSpine(obj, "dxq" + obj.name);
            }

            //球或小恶魔移动
            int x = Random.Range(-800, 800);
            rect.anchoredPosition = new Vector2(x, 100);
            rect.DOAnchorPosY(-1300, 6f).
                SetEase(Ease.Linear).
                SetId<Tween>(obj.name).
                OnComplete(() => objList.Add(obj));
        }

        //成功
        IEnumerator Success()
        {
            drager.GetComponent<CustomImage>().raycastTarget = false;
            drager.canMove = false;
            DOTween.Kill("process");

            yield return new WaitForSeconds(1.5f);

            isCreate = false;
            DOTween.KillAll();
            unDragableMask.SetActive(true);

            //结束动画
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

            mono.StartCoroutine(BlackCurtainTransition(blackRaw));

            yield return new WaitForSeconds(0.3f);
            game2Tra.gameObject.SetActive(false);
            end.SetActive(true);
            InitSpine(end, "DQ0", false);

            yield return new WaitForSeconds(1f);

            //星球变大
            float time = SpineManager.instance.DoAnimation(end, "DQ2", false);

            yield return new WaitForSeconds(time);

            SpineManager.instance.DoAnimation(end, "DQ3", true);

            yield return new WaitForSeconds(2f);

            mono.StartCoroutine(BlackCurtainTransition(blackRaw));

            yield return new WaitForSeconds(0.3f);

            //砸中小恶魔
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

            end.SetActive(false);
            time = SpineManager.instance.DoAnimation(lightTra.GetChild(0).gameObject, "end", false);

            yield return new WaitForSeconds(time);

            InitSpine(lightTra.GetChild(1).gameObject, "sc-boom3", false);

            GameEnd();
        }

        //ui动画逻辑
        void UIAnimation(List<GameObject> _objList, GameObject _obj)
        {
            float initPosX = 125;
            float offsetX = 200;
            float time = 0.75f;

            ColorDisPlay(_obj.transform.GetRawImage(), false);
            _objList.Remove(_obj);

            if (_objList.Count == 0) return;

            for (int i = 0; i < _objList.Count; i++)
            {
                RectTransform rect = _objList[i].transform.GetRectTransform();
                rect.DOAnchorPosX(initPosX + i * offsetX, time).SetEase(Ease.OutBack);
            }
        }

        void StartLevel2()
        {
            Game2Init();
            mask.SetActive(false);
            isPlaying = false;

            mono.StartCoroutine(CreateObj());
        }

        //正确错误星球初始化
        void InitTrueAndError(List<GameObject> list, Transform tra)
        {
            list.Clear();

            for (int i = 0; i < tra.childCount; i++)
            {
                GameObject obj = tra.GetChild(i).gameObject;
                RectTransform rect = tra.GetChild(i).GetRectTransform();

                list.Add(obj);
                obj.SetActive(false);
                InitSpine(obj, "", false);
                rect.anchoredPosition = Vector2.up * 100;
            }
        }
        #endregion

        #region 星星拖拽
        void DragStart(Vector3 position, int type, int index)
        {
            SoundManager.instance.PlayClip(9);

            lineRect.position = dragers[type][index].transform.position;
            lineRect.gameObject.SetActive(true);
            preNum = index;

            //正确动画
            GameObject obj = dragers[type][index].gameObject;
            SpineManager.instance.DoAnimation(obj, "xxb" + obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(obj, "xxa" + obj.name);
            });
        }

        void Drag(Vector3 position, int type, int index)
        {
            position = lineRect.position;
            dragers[type][index].transform.position = position;
            lineRect.rotation = Quaternion.Euler(0, 0, GetAngle(position, Input.mousePosition));
            lineRect.sizeDelta = new Vector2(21, GetDistance(position, Input.mousePosition));
        }

        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            lineRect.gameObject.SetActive(false);

            if (isMatch)
            {
                if (isAllTrue(isDraweds[type]))
                {
                    mono.StartCoroutine(StarSuccess(type));
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            }
        }

        bool DoAfter(int dragType, int index, int drogType)
        {
            #region 判断是否是连的

            //大于队尾选队头
            int num = index + 1 == isDraweds[dragType].Length ? 0 : index + 1;
            int end = -1;

            //是否是拖拽的下一个数和连过了
            if (num == preNum && !isDraweds[dragType][index])
            {
                end = index;
            }

            //大于队头选队尾
            num = index - 1 == -1 ? isDraweds[dragType].Length - 1 : index - 1;

            //是否是拖拽的上一个数和连过了
            if (num == preNum && !isDraweds[dragType][preNum])
            {
                end = preNum;
            }

            #endregion

            //如果匹配上
            if (end != -1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                isDraweds[dragType][end] = true;

                //显示连线
                GameObject line = starsTra.Find("star" + (dragType + 1)).GetChild(0).GetChild(end).gameObject;
                lines.Remove(line.transform.GetRawImage());
                DOTween.Kill("" + line.name);
                line.SetActive(true);
                line.transform.GetRawImage().color = Color.white;

                //正确动画
                GameObject obj = dragers[dragType][index].gameObject;
                SpineManager.instance.DoAnimation(obj, "xxb" + obj.name, false, () =>
                {
                    SpineManager.instance.DoAnimation(obj, "xxa" + obj.name);
                });

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 地球拖拽
        void EarthDragStart(Vector3 position, int type, int index)
        {
            if (isEnter) return;

            drager.canMove = true;

            SoundManager.instance.PlayClip(9);
        }

        void EarthDrag(Vector3 position, int type, int index)
        {
            earthRect.anchoredPosition = new Vector2(earthRect.anchoredPosition.x, 100);

            if (isStop)
            {
                earthRect.anchoredPosition = Vector2.up * 100;
            }
        }

        void EarthDragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            if (isStop)
            {
                isStop = false;
            }
        }

        void EarthOnClick(int index)
        {
            Debug.Log("OnClick");
        }
        #endregion

        #region 游戏通用环节
        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                ddTra.GetChild(0).gameObject.SetActive(true);

                btn03.SetActive(false);

                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 3, null, () =>
                    {
                        ddTra.GetChild(0).gameObject.SetActive(false);

                        StartGame();
                    }));
                }));
            });

        }

        void GameEnd()
        {
            isPlaying = true;

            mono.StartCoroutine(WaitFor(2f, () =>
            {
                mask.SetActive(true);
                btn03.SetActive(false);
                successSpine.SetActive(true);
                caidaiSpine.SetActive(true);

                InitSpine(caidaiSpine, "sp", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);

                SpineManager.instance.DoAnimation(successSpine, "3-5-z", false, () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "3-5-z2", false, () =>
                    {
                        successSpine.SetActive(false);
                        caidaiSpine.SetActive(false);

                        btn01.SetActive(true);
                        btn02.SetActive(true);

                        InitSpine(btn01, "fh2", false);
                        InitSpine(btn02, "ok2", false);

                        isPlaying = false;
                    });
                });
            }));
        }

        //重玩
        void Replay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                MaskInit();
                GameInit();
                StartGame();
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(1).gameObject, SoundManager.SoundType.VOICE, 1));

                ddTra.GetChild(1).gameObject.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }

        //下一关
        void NextButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            ddTra.GetChild(0).gameObject.SetActive(true);

            float time = SpineManager.instance.DoAnimation(nextButton, "next", false, () =>
            {
                nextButton.SetActive(false);
            });

            mono.StartCoroutine(WaitFor(time, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    ddTra.GetChild(0).gameObject.SetActive(false);

                    //黑幕转场
                    mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
                    {
                        StartLevel2();
                    }));
                }));
            }));
        }
        #endregion

        #region 通用方法
        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(1, 1, 1, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(1, 1, 1, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        //洗牌算法
        void Shuffle<T>(ref T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = (Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
        }

        //黑幕转场
        IEnumerator BlackCurtainTransition(RawImage _raw, Action method = null)
        {
            _raw.color = new Color(0, 0, 0, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.black, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.4f);

            _raw.DOColor(new Color(0, 0, 0, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

            method?.Invoke();
        }

        //协程:改变Spine透明度
        IEnumerator ChangeSpineAlpha(Material material, float aimAlpha, float time, Action method = null)
        {
            float i = 0;
            float curAlpha = material.GetColor("_Color").a;
            float deltaAlpha = aimAlpha - curAlpha;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                material.SetColor("_Color", new Color(1, 1, 1, curAlpha + deltaAlpha * i / time));

                yield return wait;
                i += Time.fixedDeltaTime;
            }

            method?.Invoke();
        }

        //是否全部都是对的
        bool isAllTrue(bool[] bools)
        {
            for (int i = 0; i < bools.Length; i++)
            {
                if (!bools[i]) return false;
            }

            return true;
        }

        //错开播放
        IEnumerator WaitForAnimation(Transform _tra, float _time = 0.2f, Action method = null)
        {
            WaitForSeconds wait = new WaitForSeconds(_time);

            for (int i = 0; i < _tra.childCount; i++)
            {
                GameObject obj = _tra.GetChild(i).gameObject;
                Material material = obj.GetComponent<SkeletonGraphic>().material;

                //渐变显示
                material.SetColor("_Color", new Color(1, 1, 1, 0));

                InitSpine(obj, "xxa" + obj.name);

                mono.StartCoroutine(ChangeSpineAlpha(material, 1, _time * 1.5f));

                yield return wait;
            }

            yield return wait;

            method?.Invoke();
        }

        //判断角度
        private float GetAngle(Vector3 startPos, Vector3 endPos)
        {
            Vector3 dir = endPos - startPos;
            float angle = Vector3.Angle(Vector3.up, dir);
            Vector3 cross = Vector3.Cross(Vector3.up, dir);
            float dirF = cross.z > 0 ? 1 : -1;
            angle = angle * dirF;
            return angle;
        }

        //计算两点之间的距离并将长度自适应
        private float GetDistance(Vector3 startPos, Vector3 endPos)
        {
            float distance = Vector3.Distance(endPos, startPos);
            return distance * 1 / curCanvasTra.localScale.x;
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = ddTra.GetChild(0).gameObject;

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");


            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }
        #endregion
    }
}
