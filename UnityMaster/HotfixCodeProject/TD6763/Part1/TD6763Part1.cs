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

    public class TD6763Part1
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

        private Transform Bg;
        private Transform xemR;
        private Transform huDong;
        private GameObject y;
        private GameObject Num;
        private GameObject hailang;
        private GameObject yun;
        private GameObject clock;
        private GameObject tw;
        private GameObject zd2_water;
        private GameObject zd1_water;
        private Transform zuoTi;
        private GameObject star;
        private GameObject xempos2;
        private GameObject ypos;
        private GameObject xuetiao;
        private RectTransform rect1;
        private RectTransform rect2;
        private float startX;
        private float endX;
        private GameObject q;
        private GameObject tw2;
        private Transform zhaiai;
        private Transform zhaiai2;
        private Image mask_image;

        SkeletonGraphic obj_hai;
        SkeletonGraphic obj_mask;
        SkeletonGraphic obj_tw;
        SkeletonGraphic obj_tw2;
        private bool _isPlaying;
        private int HP;

        Coroutine coroutine;

        int yuNum;
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

            ypos = curTrans.Find("ypos").gameObject;
            Bg = curTrans.Find("BG");
            huDong = curTrans.Find("huDong");
            y = huDong.Find("yDragReact/y").gameObject;
            clock = curTrans.Find("clock").gameObject;
            Num = curTrans.Find("clock/num").gameObject;
            hailang = huDong.Find("hailang").gameObject;
            yun = curTrans.Find("yun").gameObject;
            zuoTi = curTrans.Find("zuoTi");
            tw = curTrans.Find("zuoTi/tw").gameObject;
            tw2 = curTrans.Find("zuoTi/tw2").gameObject;
            star = zuoTi.Find("star").gameObject;
            jfb = curTrans.Find("jfb").gameObject;
            grade = curTrans.Find("jfb/grade").gameObject;
            xem = curTrans.Find("xemR/xem").gameObject;
            xemR = curTrans.Find("xemR");
            xempos = curTrans.Find("xempos").gameObject;
            xempos2 = curTrans.Find("xempos2").gameObject;
            star2 = curTrans.Find("star2").gameObject;
            xuetiao = curTrans.Find("xuetiao").gameObject;
            q = zuoTi.Find("q").gameObject;
            zhaiai = huDong.Find("zhaiai");
            zhaiai2 = huDong.Find("zhaiai2");
            zd1_water = Bg.Find("Panel/water3").gameObject;
            zd2_water = Bg.Find("Panel2/Panel2/water7").gameObject;
            canRotate = false;

            obj_hai = hailang.transform.GetComponent<SkeletonGraphic>();
            obj_tw = tw.transform.GetComponent<SkeletonGraphic>();
            obj_tw2 = tw2.transform.GetComponent<SkeletonGraphic>();

            mask_image = _mask.GetComponent<Image>();

            Empty4Raycast[] e4rs = zuoTi.GetChild(2).GetComponentsInChildren<Empty4Raycast>();
            for (int i = 0; i < e4rs.Length; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, ClickJz);
            }

            // Util.AddBtnClick()

            StopAllAudio();
            StopAllCoroutines();

            GameInit();
            GameStart();
        }

        private RectTransform rect3;
        private RectTransform rect4;

        void InitData()
        {
            _isPlaying = true;
            rect1 = Bg.GetChild(0).GetRectTransform();
            rect2 = Bg.GetChild(1).GetRectTransform();

            rect3 = zhaiai.GetRectTransform();
            rect4 = zhaiai2.GetRectTransform();

            SpineManager.instance.DoAnimation(xem, "xem1", true);

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }
        void damage()
        {
            switch (HP)
            {
                case 3:
                    for (int i = 0; i < xuetiao.transform.childCount; i++)
                    {
                        xuetiao.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    xuetiao.transform.GetChild(2).gameObject.SetActive(false);
                    break;

                case 1:
                    xuetiao.transform.GetChild(1).gameObject.SetActive(false);
                    break;
                case 0:
                    xuetiao.transform.GetChild(0).gameObject.SetActive(false);


                    break;
                default:
                    break;
            }

        }

        void GameInit()
        {
            IsPass = true;
            startX = 5760;
            endX = -5760;
            SpineManager.instance.DoAnimation(zhaiai.GetChild(3).gameObject, "zd", true);
            SpineManager.instance.DoAnimation(zhaiai2.GetChild(4).gameObject, "zd", true);

            HP = 3;
            for (int i = 0; i < xuetiao.transform.childCount; i++)
            {
                xuetiao.transform.GetChild(i).gameObject.SetActive(true);
            }
            y.transform.parent.GetComponent<mILDrager>().enabled = true;

            xemR.transform.GetRectTransform().DOMove(xempos2.transform.position, 0f);
            xemR.transform.rotation = Quaternion.Euler(0, 0, 10);
            y.transform.parent.GetRectTransform().DOAnchorPos(new Vector2(331, -697), 0f);
            y.GetComponent<PolygonCollider2D>().enabled = true;
            //y.transform.parent.GetRectTransform().DOMove(ypos.transform.position, 0f);
            //y.transform.GetRectTransform().DOMove(ypos.transform.position, 0f);
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();

            _dDD.Hide();

            _sDD.Hide();

            huDong.gameObject.Hide();
            ToLoadZa();

            y.transform.localScale = Vector2.one;
            _curGo.transform.Find("ui").gameObject.SetActive(false);
            xuetiao.SetActive(false);
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            ChangeNum(clockNumber);
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

        #region 游戏加载
        int zaNum1;
        int zaNum2;

        void ToLoadTm()
        {
            for (int i = 0; i < zuoTi.GetChild(2).childCount; i++)
            {

                SpineManager.instance.DoAnimation(zuoTi.GetChild(2).GetChild(i).gameObject, "kong", true);
                SkeletonGraphic obj_sg = zuoTi.GetChild(2).GetChild(i).GetComponent<SkeletonGraphic>();
                SkeletonGraphic obj_sg2 = tw2.GetComponent<SkeletonGraphic>();
                _mono.StartCoroutine(ChangeSpineAlpha(obj_sg.material, 1, 0.1f, () => { }));
                _mono.StartCoroutine(ChangeSpineAlpha(obj_sg2.material, 0, 0.1f, () => { }));
                zuoTi.GetChild(2).GetChild(i).GetRectTransform().DOScale(new Vector2(1f, 1f), 0f);
                zuoTi.GetChild(2).GetChild(i).GetComponent<RectTransform>().DOAnchorPos(zuoTi.GetChild(5).GetChild(i).GetComponent<RectTransform>().anchoredPosition, 0f);
                // zuoTi.GetChild(2).GetChild(i).GetComponent<SkeletonGraphic>().DOColor(new Color(255, 255, 255, 255), 0f);
            }
        }

        void ToLoadZa()
        {

            zhaiai.GetChild(3).gameObject.SetActive(true);
            zhaiai2.GetChild(4).gameObject.SetActive(true);
            xem.transform.GetRectTransform().DORotate(new Vector3(0, 0, -11), 0);
            canRotate = false;
            //tw.GetComponent<SkeletonGraphic>().DOColor(new Color(255, 255, 255, 0), 0f);

            _canClick = false;
            //障碍
            zaNum1 = 3;
            zaNum2 = 4;
            //isMoveBackground = true;
            zhaiai.Find("zd1").GetComponent<PolygonCollider2D>().enabled = true;
            zhaiai2.Find("zd2").GetComponent<PolygonCollider2D>().enabled = true;
            jfb.SetActive(false);
            gradeNum = 0;
            //时间
            clockNumber = 30;
            clock.SetActive(false);
            //动画
            for (int i = 0; i < zaNum1; i++)
            {
                SpineManager.instance.DoAnimation(zhaiai.GetChild(i).gameObject, "za" + (i + 1), true);
            }
            for (int i = 0; i < zaNum2; i++)
            {
                SpineManager.instance.DoAnimation(zhaiai2.GetChild(i).gameObject, "za" + (i + 4), true);
            }

            //水波
            for (int i = 2; i < 6; i++)
            {
                SpineManager.instance.DoAnimation(Bg.GetChild(0).GetChild(i).gameObject, "sw", true);
                Bg.GetChild(0).GetChild(i).gameObject.SetActive(false);


            }
            for (int i = 0; i < 5; i++)
            {
                SpineManager.instance.DoAnimation(Bg.GetChild(1).GetChild(1).GetChild(i).gameObject, "sw", true);
                Bg.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
            SpineManager.instance.DoAnimation(hailang, "kong", true);
            ToLoadTm();

            SpineManager.instance.DoAnimation(zuoTi.GetChild(3).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(tw, "kong", false);
            SpineManager.instance.DoAnimation(tw2, "kong", false);
            SpineManager.instance.DoAnimation(yun, "kong", false);
            y.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(y, "y1", true);
            // SpineManager.instance.DoAnimation(tw2, "kong", true);

            y.GetComponent<EventDispatcher>().TriggerEnter2D += OnTriggerEnter2D;
            for (int i = 0; i < 2; i++)
            {
                Bg.GetChild(0).GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                Bg.GetChild(1).GetChild(i).GetComponent<BoxCollider2D>().enabled = true;

            }

            // zuoTi.gameObject.SetActive(false);


            //背景
            Bg.GetChild(0).GetComponent<RectTransform>().DOAnchorPosX(0, 0f);
            Bg.GetChild(1).GetComponent<RectTransform>().DOAnchorPosX(5760, 0f);

            zhaiai.GetComponent<RectTransform>().DOAnchorPosX(0, 0f);
            zhaiai2.GetComponent<RectTransform>().DOAnchorPosX(5760, 0f);

            clock.GetComponent<RectTransform>().DOAnchorPos(new Vector2(178, -225), 0f);

            // _mono.StartCoroutine(WaterDis());

            zuoTi.gameObject.SetActive(true);
        }
        #endregion

        //物体渐变显示或者消失
        void ColorDisPlay(Image raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(0, 0, 0, 0);
                raw.gameObject.SetActive(true);

                raw.DOColor(new Color(0, 0, 0, 0.8f), _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = new Color(0, 0, 0, 0.8f);
                raw.DOColor(new Color(0, 0, 0, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        #region 背景循环

        bool IsPass;
        void LeftMove(RectTransform rect, float startXPos, float endXPos, float speed = 8)
        {
            if (Convert.ToInt32(rect.anchoredPosition.x) <= endXPos)
                rect.anchoredPosition = new Vector2(startXPos, Convert.ToInt32(rect.anchoredPosition.y));
            rect.Translate(Vector2.left * speed * Screen.width / 1920f);
        }

        bool canRotate;

        void Update()
        {
            // xem.transform.Rotate((xemR.transform.position)* Time.deltaTime * 90);

            if (canRotate)
            {
                xem.transform.RotateAround(xemR.transform.position, Vector3.forward, 5);
            }
        }

        void FixedUpdate()
        {
            if (!IsPass)
            {
                LeftMove(rect1, startX, endX);
                LeftMove(rect2, startX, endX);
                LeftMove(rect3, startX, endX);
                LeftMove(rect4, startX, endX);
            }
        }
        #endregion

        #region 障碍层级
        void ZhaiaiFront()
        {
            zhaiai.SetAsFirstSibling();
            zhaiai2.SetAsFirstSibling();
        }
        void ZhaiaiBack()
        {
            zhaiai.SetAsLastSibling();
            zhaiai2.SetAsLastSibling();
        }

        #endregion

        #region 海豚碰撞
        private void OnTriggerEnter2D(Collider2D other, int time)
        {
            if (other.transform.position.y < y.transform.position.y) ZhaiaiBack();
            else ZhaiaiFront(); 

            if ((other.gameObject.name == "zd1") || (other.gameObject.name == "zd2"))
            {
                _mono.StartCoroutine(InjuredAnimaion(other.transform, 3));
            }
            else if (other.gameObject.name == "za1" || other.gameObject.name == "za2" || other.gameObject.name == "za3" || other.gameObject.name == "za4" ||
                other.gameObject.name == "za5" || other.gameObject.name == "za6" || other.gameObject.name == "za7")
            {
                _mono.StartCoroutine(InjuredAnimaion(other.transform, 2));
            }
            else if (other.gameObject.name == "Panel1" || other.gameObject.name == "Panel2")  //海浪出现
            {
                _mono.StopCoroutine(coroutine);

                _mono.StartCoroutine(ChangeSpineAlpha(obj_hai.material, 1, 0.1f));
                _mono.StartCoroutine(ChangeSpineAlpha(obj_tw.material, 0, 0.1f));
                y.transform.parent.GetComponent<mILDrager>().enabled = false;
                IsPass = true;
                Vector2 tmPos = new Vector2(y.transform.position.x + 400, y.transform.position.y);
                hailang.transform.position = tmPos;
                SpineManager.instance.DoAnimation(hailang, "tm", false, () =>
                {
                    _mono.StartCoroutine(WaitCoroutine(() =>
                    {
                        ZuoTi();
                    }, 0.5f));

                });
            }
        }

        //受伤动画
        IEnumerator InjuredAnimaion(Transform tra, int num = 2)
        {
            PlayVoice(num);

            //撞到炸弹
            if (num == 3)
            {
                SpineManager.instance.DoAnimation(tra.GetChild(0).gameObject, "sc-boom", false);
                SpineManager.instance.DoAnimation(tra.gameObject, "kong", false);
                tra.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                if (tra.gameObject.name == "zd1")
                {
                    zd1_water.SetActive(false);
                }
                else if (tra.gameObject.name == "zd2")
                {
                    zd2_water.SetActive(false);
                }
            }

            //受伤
            IsPass = true;
            y.transform.parent.GetComponent<mILDrager>().enabled = false;
            float time = SpineManager.instance.DoAnimation(y, "y" + num, true);

            yield return new WaitForSeconds(time + 1f);

            //血量归零
            if (--HP == 0)
            {
                Bg.GetChild(0).GetComponent<RectTransform>().DOAnchorPosX(0, 0f);
                Bg.GetChild(1).GetComponent<RectTransform>().DOAnchorPosX(5760, 0f);
                zhaiai.GetComponent<RectTransform>().DOAnchorPosX(0, 0f);
                zhaiai2.GetComponent<RectTransform>().DOAnchorPosX(5760, 0f);
                y.transform.parent.GetRectTransform().DOAnchorPos(new Vector2(331, -697), 0f);
                HP = 3;
                SpineManager.instance.DoAnimation(zhaiai.GetChild(3).gameObject, "zd", true);
                SpineManager.instance.DoAnimation(zhaiai2.GetChild(4).gameObject, "zd", true);
            }

            damage();

            //恢复状态
            IsPass = false;
            SpineManager.instance.DoAnimation(y, "y1", true);
            y.transform.parent.GetComponent<mILDrager>().enabled = true;
        }
        #endregion

        #region 倒计时
        int clockNumber;
        IEnumerator CountDown()
        {
            while (clockNumber >= 0)
            {
                yield return new WaitForSeconds(1);

                Image image = Num.GetComponent<Image>();
                image.sprite = Num.GetComponent<BellSprites>().sprites[clockNumber];
                image.SetNativeSize();
                clockNumber--;

                if (clockNumber < 0)
                {
                    // isMoveBackground = false;
                    // DOTween.Kill("Background");
                    IsPass = true;
                    y.transform.parent.GetComponent<mILDrager>().enabled = false;
                    Grade();
                    _mono.StartCoroutine(WaitCoroutine(() => { EndVedio(); }, 2));
                    y.GetComponent<PolygonCollider2D>().enabled = false;

                    break;
                }
            }
        }
        #endregion

        List<int> curList;
        List<int> totalList;
        IEnumerator WaitAnimation(int yuNum, List<int> curList, Action method = null)
        {

            for (int i = 0; i < curList.Count; i++)
            {

                SpineManager.instance.DoAnimation(zuoTi.GetChild(2).GetChild(i).gameObject, "t" + curList[i], false);
                yield return new WaitForSeconds(0.2f);
            }

            method?.Invoke();
        }

        #region 跳转做题
        void ZuoTi()
        {
            _mono.StartCoroutine(ChangeSpineAlpha(obj_tw2.material, 0, 0.1f));
            //zuoTi.gameObject.SetActive(true);
            ToLoadTm();

            //_mask.SetActive(true);
            ColorDisPlay(mask_image, true, null, 0.5f);
            clock.transform.position = zuoTi.GetChild(0).position;

            //随机题目和答案
            _mono.StartCoroutine(ChangeSpineAlpha(obj_hai.material, 0, 0.5f));

            totalList = new List<int>();
            curList = new List<int>();

            for (int i = 1; i <= 8; i++)
            {
                totalList.Add(i);
            }
            yuNum = Random.Range(1, 6);

            curList.Add(yuNum);

            GetRandomList(ref totalList);
            totalList.Remove(yuNum);

            for (int i = 0; i < 3; i++)
            {
                Debug.Log(totalList[i]);
                curList.Add(totalList[i]);
            }

            GetRandomList(ref curList);
            correctNum = curList.IndexOf(yuNum);

            SpineManager.instance.DoAnimation(tw, "tw0", true);
            SpineManager.instance.DoAnimation(tw2, "tw" + curList[correctNum], true);
            _mono.StartCoroutine(ChangeSpineAlpha(obj_tw.material, 1, 0.5f));

            _mono.StartCoroutine(QuestionAnimation(yuNum));

            //时钟
            ChangeNum(5);
        }

        //问题出现动画
        IEnumerator QuestionAnimation(int num)
        {
            yield return new WaitForSeconds(1f);

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8);
            //思考
            float time = SpineManager.instance.DoAnimation(zuoTi.GetChild(3).gameObject, "q" + num, false);

            yield return new WaitForSeconds(time - 0.2f);

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);

            //选择框
            _mono.StartCoroutine(WaitAnimation(num, curList));

            yield return new WaitForSeconds(0.8f);

            coroutine = _mono.StartCoroutine(QuestionCutDown(5));
            _canClick = true;
        }

        IEnumerator QuestionCutDown(int num)
        {
            ChangeNum(num);

            WaitForSeconds wait = new WaitForSeconds(1f);

            while(num > 0)
            {
                yield return wait;

                ChangeNum(--num);
            }

            yield return 0;

            _mono.StartCoroutine(Back(false));
        }

        void ChangeNum(int num)
        {
            Image image = Num.GetComponent<Image>();
            image.sprite = Num.GetComponent<BellSprites>().sprites[num];
            image.SetNativeSize();
        }

        #region 随机链表
        void GetRandomList<T>(ref List<T> list)
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

        void GetRandomArray<T>(T[] arr)
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

        #endregion

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

        #region 做题点击和完成恢复
        int correctNum;//正确答案下标
        bool _canClick;
        void ClickJz(GameObject obj)
        {
            if (!_canClick) return;
            _canClick = false;

            if (obj.name != correctNum.ToString())
            {
                //错误
                PlayCommonSound(5);
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "t-d" + curList[int.Parse(obj.name)], false);
                _canClick = true;
            }
            else
            {
                _mono.StopCoroutine(coroutine);
                //正确
                PlayVoice(5);
                SkeletonGraphic obj_sg = obj.transform.parent.GetComponent<SkeletonGraphic>();
                SkeletonGraphic obj_sg2 = tw2.transform.GetComponent<SkeletonGraphic>();
                obj.transform.parent.GetRectTransform().DOScale(new Vector2(0.7f, 0.7f), 1f);

                obj.transform.parent.DOMove(star.transform.position, 0.6f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _mono.StartCoroutine(ChangeSpineAlpha(obj_sg.material, 0, 1f, () => { }));


                    SpineManager.instance.DoAnimation(star, "star", false, () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "kong", false);

                        Delay(1f, () => { _mono.StartCoroutine(Back()); });
                    });
                    SpineManager.instance.DoAnimation(zuoTi.GetChild(3).gameObject, "kong", false);

                    _mono.StartCoroutine(ChangeSpineAlpha(obj_tw2.material, 1, 0.6f));
                });
            }
        }

        #region 转场
        IEnumerator Back(bool isTrue = true)
        {
            _canClick = false;

            yield return new WaitForSeconds(0.5f);

            yun.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            PlayVoice(0);
            float time = SpineManager.instance.DoAnimation(yun, "yun", false);

            yield return new WaitForSeconds(0.7f);

            //云聚拢时改界面
            //云2秒，1秒时聚拢，延迟0.5
            InitSpine(zuoTi.GetChild(3).gameObject, "", false);
            clock.GetComponent<RectTransform>().DOAnchorPos(new Vector2(178, -225), 0f);
            _mask.SetActive(false);
            _mono.StartCoroutine(ChangeSpineAlpha(obj_hai.material, 1, 0.1f));
            ToLoadTm();
            SpineManager.instance.DoAnimation(tw, "kong", false);
            SpineManager.instance.DoAnimation(tw2, "kong", false);
            ChangeNum(clockNumber);

            yield return new WaitForSeconds(time - 0.7f);

            SpineManager.instance.DoAnimation(yun, "kong", false);

            IsPass = false;
            int a = Random.Range(1, 5);

            //选择正确翻过海浪
            Delay(0.5f, () => { HLMove(); });

            if (isTrue)
            {
                time = SpineManager.instance.DoAnimation(y, "y-j" + a, false, () =>
                {
                    y.transform.parent.GetComponent<mILDrager>().enabled = true;
                    SpineManager.instance.DoAnimation(y, "y1", true);
                });

                gradeNum++;

                yield return new WaitForSeconds(time / 2f);
                PlayVoice(7);
                coroutine = _mono.StartCoroutine(CountDown());
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                _mono.StartCoroutine(InjuredAnimaion(null, 2));
                coroutine = _mono.StartCoroutine(CountDown());
            }
        }
        #endregion

        #endregion

        #region 海浪移动

        void HLMove()
        {
            hailang.transform.GetRectTransform().DOAnchorPosX(-300, 1f);
        }
        #endregion

        #region 成绩结算
        private GameObject jfb;
        private GameObject grade;
        private GameObject star2;
        int gradeNum;

        void Grade()
        {
            PlayVoice(1);
            jfb.SetActive(true);
            SpineManager.instance.DoAnimation(star2, "star", false);
            Image image = grade.GetComponent<Image>();
            image.sprite = jfb.GetComponent<BellSprites>().sprites[gradeNum];
            image.SetNativeSize();
            grade.transform.GetRectTransform().DOScale(new Vector2(2, 2), 0.5f).OnComplete(() =>
            {
                grade.transform.GetRectTransform().DOScale(new Vector2(1, 1), 0.5f);
            });
        }
        #endregion

        #region 结束动画
        private GameObject xem;
        private GameObject xempos;
        void EndVedio()
        {
            // isMoveBackground = false;
            //  DOTween.Kill("Background");
            IsPass = true;
            for (int i = 2; i < 6; i++)
            {
                Bg.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < 5; i++)
            {
                Bg.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
            }

            SpineManager.instance.DoAnimation(q, "kong", false);
            for (int i = 0; i < 2; i++)
            {
                Bg.GetChild(0).GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                Bg.GetChild(1).GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
            }

            _mono.StartCoroutine(WaitCoroutine(() =>
            {
                yun.transform.GetComponent<SkeletonGraphic>().Initialize(true);


                PlayVoice(0);
                float time = SpineManager.instance.DoAnimation(yun, "yun", false, () =>
                {
                    SpineManager.instance.DoAnimation(xem, "xem3", true);

                    SpineManager.instance.DoAnimation(yun, "kong", false);
                    // isMoveBackground = true;
                    // _mono.StartCoroutine(MoveBackground(Bg, 12));
                    IsPass = false;
                    y.transform.parent.GetRectTransform().DOAnchorPosX(xempos.transform.position.x + 420, 8f).SetEase(Ease.Linear);

                    xemR.transform.GetRectTransform().DOMove(xempos.transform.position, 1.5f).SetEase(Ease.OutSine);

                    _mono.StartCoroutine(FishAnimation(() =>
                    {
                        _mono.StartCoroutine(WaitCoroutine(() =>
                        {
                            canRotate = true;
                            SpineManager.instance.DoAnimation(xem, "xem-y", false);
                            PlayVoice(4);
                            xemR.transform.GetRectTransform().DOMove(xempos2.transform.position, 0.8f).SetEase(Ease.OutCubic).OnComplete(() =>
                            {

                                //isMoveBackground = false;
                                //  DOTween.Kill("Background");
                                IsPass = true;
                                GameSuccess();
                            });
                        }, 1.2f));

                        // xemR.transform.GetRectTransform().DORotateQuaternion( Quaternion.Euler(0, 0 , 360), 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                        Delay(0.5f, () =>
                        {

                            SpineManager.instance.DoAnimation(y, "y-j4", false, () =>
                            {
                                SpineManager.instance.DoAnimation(y, "y1", true);
                            });

                        });
                    }));
                });
            }, 3f));

            //云聚拢时改界面
            _mono.StartCoroutine(WaitCoroutine(() =>
            {
                clock.SetActive(false);
                _mask.SetActive(false);
                ToLoadTm();
                jfb.SetActive(false);
                zhaiai.gameObject.SetActive(false);
                zhaiai2.gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(tw, "kong", false);
                SpineManager.instance.DoAnimation(tw2, "kong", false);
                //SpineManager.instance.DoAnimation(tw2, "kong", true);
                SpineManager.instance.DoAnimation(hailang, "kong", true);
                y.transform.localScale = Vector2.one * 1.5f;
                y.transform.parent.GetRectTransform().DOAnchorPos(new Vector2(331, -797), 0f);
                xuetiao.SetActive(false);
                _curGo.transform.Find("ui").gameObject.SetActive(false);
            }, 3.67f
            ));
        }

        //小鱼跳跃动画
        IEnumerator FishAnimation(Action method = null)
        {
            for (int i = 1; i < 5; i++)
            {
                float time = SpineManager.instance.DoAnimation(y, "y-j" + i, false);

                yield return new WaitForSeconds(0.5f);

                PlayVoice(7);

                yield return new WaitForSeconds(time - 1f);
            }

            method?.Invoke();
        }

        #endregion

        //Spine初始化
        float InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);

            if (animation == "") return 0f;
            else return _ske.AnimationState.Data.SkeletonData.FindAnimation(animation).Duration;
        }

        IEnumerator WaitCoroutine(Action method_2 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_2?.Invoke();
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            clock.SetActive(true);

            zhaiai.gameObject.SetActive(true);
            zhaiai2.gameObject.SetActive(true);
            IsPass = false;
            for (int i = 2; i < 6; i++)
            {
                Bg.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < 5; i++)
            {
                Bg.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(true);
            }
            coroutine = _mono.StartCoroutine(CountDown());
            huDong.gameObject.Show();
            SpineManager.instance.DoAnimation(y, "y1", true);


            _curGo.transform.Find("ui").gameObject.SetActive(true);
            xuetiao.SetActive(true);
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
