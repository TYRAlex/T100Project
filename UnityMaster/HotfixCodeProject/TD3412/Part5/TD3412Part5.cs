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
        next,
    }

    public enum BugTag
    {
        BlueBug,
        RedBug,
        YellowBug,
    }
    public class TD3412Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject dd;
        private GameObject ddd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;
        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        //胜利动画名字
        private string tz;
        private string sz;

        private Dictionary<int, string> _bugIndexDic;
        private GameObject[] _bug;
        private mILDrager[] _bugDrager;
        private mILDroper[] _bugDroper;
        private int _curDragType;
        private bool _canDrag;
        private bool _isDraging;
        private BugTag[] _bugTagArray;
        private int _blueBugCount;
        private int _redBugCount;
        private int _yellowBugCount;
        private int _bluePoint;
        private int _redPoint;
        private int _yellowPoint;
        private int _curBugSibling;

        private Transform _redBugPoint;
        private Transform _blueBugPoint;
        private Transform _yellowBugPoint;
        private GameObject _redBottle;
        private GameObject _blueBottle;
        private GameObject _yellowBottle;
        private GameObject _bomb;
        private GameObject _devil1;
        private GameObject _devil2;
        private GameObject _devil3;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(false);
            ddd = curTrans.Find("mask/DDD").gameObject;
            ddd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            talkIndex = 1;
            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _blueBugCount = 0;
            _redBugCount = 0;
            _yellowBugCount = 0;
            _bluePoint = 0;
            _redPoint = 0;
            _yellowPoint = 0;
            _curDragType = 15;
            _curBugSibling = 0;
            _isDraging = false;

            _redBugPoint = curTrans.Find("RedPoint/Point");
            _blueBugPoint = curTrans.Find("BluePoint/Point");
            _yellowBugPoint = curTrans.Find("YellowPoint/Point");

            _redBottle = curTrans.GetGameObject("Bottle/RedBottle");
            _blueBottle = curTrans.GetGameObject("Bottle/BlueBottle");
            _yellowBottle = curTrans.GetGameObject("Bottle/YellowBottle");

            _bomb = curTrans.GetGameObject("Bomb");

            _devil1 = curTrans.GetGameObject("Devil/Devil1");
            _devil2 = curTrans.GetGameObject("Devil/Devil2");
            _devil3 = curTrans.GetGameObject("Devil/Devil3");
            _bugTagArray = new BugTag[12];
            _bluePoint = 0;
            _redPoint = 0;
            _yellowPoint = 0;
            //数组初始化
            for (int i = 0; i < 12; i++)
            {
                RandomArray(i);
            }
            _bugIndexDic = new Dictionary<int, string>(12);
            _bugIndexDic.Add(1, "CD3");
            _bugIndexDic.Add(2, "CD7");
            _bugIndexDic.Add(3, "CD7");
            _bugIndexDic.Add(4, "CD3");
            _bugIndexDic.Add(5, "Flower4");
            _bugIndexDic.Add(6, "CD7");
            _bugIndexDic.Add(7, "Flower3");
            _bugIndexDic.Add(8, "CD7");
            _bugIndexDic.Add(9, "CD7");
            _bugIndexDic.Add(10, "Flower4");
            _bugIndexDic.Add(11, "Flower4");
            _bugIndexDic.Add(12, "CD3");

            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            GameInit();
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
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
                case BtnEnum.next:
                    result = "next";
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
                        anyBtns.gameObject.SetActive(false);
                        dd.Show();
                        GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false,
                    () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                    });
                }
                else if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); ddd.SetActive(true); mono.StartCoroutine(DBDSpeckerCoroutine(SoundManager.SoundType.VOICE, 2)); });
                }
            });
        }

        private void GameInit()
        {
            _bluePoint = 0;
            _redPoint = 0;
            _yellowPoint = 0;

            //随机颜色
            for (int i = 0; i < _bugTagArray.Length; i += 2)
            {
                BugTag tag = _bugTagArray[i];
                _bugTagArray[i] = _bugTagArray[i + 1];
                _bugTagArray[i + 1] = tag;
            }

            //寻找虫子炸弹
            _bug = new GameObject[12];
            for (int i = 1; i <= 12; i++)
            {
                string num = i.ToString();
                _bug[i - 1] = Bg.transform.GetGameObject("Bug" + num);
            }

            for (int i = 1; i <= 12; i++)
            {
                int index = Bg.transform.Find(_bugIndexDic[i]).GetSiblingIndex();
                _bug[i - 1].transform.SetSiblingIndex(index + 1);
            }

            //初始化drager数组，droper数组
            _bugDrager = new mILDrager[_bug.Length];
            for (int i = 0; i < _bug.Length; i++)
            {
                _bugDrager[i] = _bug[i].GetComponent<mILDrager>();
            }
            _bugDroper = new mILDroper[3];
            for (int i = 0; i < 3; i++)
            {
                _bugDroper[i] = curTrans.GetTransform("DropEnd").GetChild(i).GetComponent<mILDroper>();
            }

            for (int i = 0; i < 3; i++)
            {
                UIEventListener.Get(curTrans.GetTransform("DropEnd").GetChild(i).gameObject).onEnter = OnEnterEvent;
                UIEventListener.Get(curTrans.GetTransform("DropEnd").GetChild(i).gameObject).onExit = OnExitEvent;
            }

            //炸弹隐藏
            SpineManager.instance.DoAnimation(_bomb, "kong", false);

            for (int i = 0; i < 5; i++)
            {
                _redBugPoint.GetChild(i).gameObject.Hide();
                _blueBugPoint.GetChild(i).gameObject.Hide();
                _yellowBugPoint.GetChild(i).gameObject.Hide();
            }
            _redBugPoint.GetChild(_redPoint).gameObject.Show();
            _blueBugPoint.GetChild(_bluePoint).gameObject.Show();
            _yellowBugPoint.GetChild(_yellowPoint).gameObject.Show();

            InitAni();

            InitDrag();
            StartDrag();

            talkIndex = 1;
            JudgePoint();
        }

        void GameStart()
        {
            dd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            _canDrag = false;
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);

            }));
        }

        //等待协程
        IEnumerator WaitCoroutine(float len = 0, Action method = null)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
                yield return new WaitForSeconds(len);
            method?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(dd, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dd, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dd, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //大布丁说话
        IEnumerator DBDSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(ddd, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(ddd, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(ddd, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    StartDrag();
                    dd.SetActive(false);
                    mask.SetActive(false);
                }));
            }
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            dd.Hide();
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, tz, false,
            () =>
            {
                SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                () =>
                {
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
            });
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        void SwitchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        //所有初始化动画
        void InitAni()
        {
            //虫虫炸弹动画
            ChangeDragProperty();

            //小恶魔动画
            _devil1.Show();
            _devil2.Show();
            _devil3.Show();
            SpineManager.instance.DoAnimation(_devil1, "xem", true);
            SpineManager.instance.DoAnimation(_devil2, "xem2", true);
            SpineManager.instance.DoAnimation(_devil3, "xem3", true);

            //瓶子动画
            SpineManager.instance.DoAnimation(_redBottle, "r3", true);
            SpineManager.instance.DoAnimation(_blueBottle, "b3", true);
            SpineManager.instance.DoAnimation(_yellowBottle, "y3", true);
        }

        //数组随机初始化
        void RandomArray(int i)
        {
            int random = Random.Range(0, 3);
            if (random == 0)
            {
                if (_redBugCount == 4)
                    RandomArray(i);
                else
                {
                    _bugTagArray[i] = BugTag.RedBug;
                    _redBugCount += 1;
                }
            }
            if (random == 1)
            {
                if (_blueBugCount == 4)
                    RandomArray(i);
                else
                {
                    _bugTagArray[i] = BugTag.BlueBug;
                    _blueBugCount += 1;
                }
            }
            if (random == 2)
            {
                if (_yellowBugCount == 4)
                    RandomArray(i);
                else
                {
                    _bugTagArray[i] = BugTag.YellowBug;
                    _yellowBugCount += 1;
                }
            }
        }

        #region 拖拽相关

        void ChangeDragProperty()
        {
            //赋予虫虫炸弹随机标签，并根据标签初始化动画和drager属性
            for (int i = 0; i < _bug.Length; i++)
            {
                if (_bugTagArray[i] == BugTag.RedBug)
                {
                    SpineManager.instance.DoAnimation(_bug[i].transform.GetGameObject("Bug"), "rrr", true);
                    _bugDrager[i].dragType = 0;
                    _bugDrager[i].drops = new mILDroper[1] { _bugDroper[0] };
                    _bugDrager[i].failDrops = new mILDroper[2] { _bugDroper[1], _bugDroper[2] };
                }
                if (_bugTagArray[i] == BugTag.BlueBug)
                {
                    SpineManager.instance.DoAnimation(_bug[i].transform.GetGameObject("Bug"), "bbb", true);
                    _bugDrager[i].dragType = 1;
                    _bugDrager[i].drops = new mILDroper[1] { _bugDroper[1] };
                    _bugDrager[i].failDrops = new mILDroper[2] { _bugDroper[0], _bugDroper[2] };
                }
                if (_bugTagArray[i] == BugTag.YellowBug)
                {
                    SpineManager.instance.DoAnimation(_bug[i].transform.GetGameObject("Bug"), "yyy", true);
                    _bugDrager[i].dragType = 2;
                    _bugDrager[i].drops = new mILDroper[1] { _bugDroper[2] };
                    _bugDrager[i].failDrops = new mILDroper[2] { _bugDroper[0], _bugDroper[1] };
                }
            }
        }

        void InitDrag()
        {
            for (int i = 0; i < _bugDrager.Length; i++)
            {
                _bugDrager[i].DoReset();
                _bugDrager[i].SetDragCallback(null, null, null, null);
                _bugDrager[i].canMove = false;
            }
        }

        void StartDrag()
        {
            for (int i = 0; i < _bugDrager.Length; i++)
            {
                _bugDrager[i].GetComponent<Empty4Raycast>().raycastTarget = true;
                _bugDrager[i].SetDragCallback(BeginDragEvent, DragingEvent, EndDragEvent, ClickEvent);
                _bugDrager[i].canMove = true;
            }
            _isDraging = false;
            _canDrag = true;
        }

        void StopDrag()
        {
            for (int i = 0; i < _bugDrager.Length; i++)
            {
                _bugDrager[i].GetComponent<Empty4Raycast>().raycastTarget = false;
                _bugDrager[i].canMove = false;
                _bugDrager[i].SetDragCallback(null, null, null, null);
            }
            _isDraging = false;
            _canDrag = false;
        }

        //开始拖拽
        private void BeginDragEvent(Vector3 dragPos, int dragType, int dragIndex)
        {
            _curDragType = dragType;
            _curBugSibling = _bugDrager[dragIndex].transform.GetSiblingIndex();
            _bugDrager[dragIndex].transform.SetParent(curTrans.GetTransform("Bottle"));
            _bugDrager[dragIndex].transform.SetAsFirstSibling();
            // 虫虫炸弹方便且边缘发光动画
            if (dragType == 0)
                SpineManager.instance.DoAnimation(_bugDrager[dragIndex].transform.GetGameObject("Bug"), "rrr2", true);
            if (dragType == 1)
                SpineManager.instance.DoAnimation(_bugDrager[dragIndex].transform.GetGameObject("Bug"), "bbb2", true);
            if (dragType == 2)
                SpineManager.instance.DoAnimation(_bugDrager[dragIndex].transform.GetGameObject("Bug"), "yyy2", true);
        }

        //拖拽中
        private void DragingEvent(Vector3 dragPos, int dragType, int dragIndex)
        {
            _isDraging = true;
        }

        //拖拽结束
        private void EndDragEvent(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if (_canDrag)
            {
                StopDrag();
                _bugDrager[dragIndex].transform.SetParent(Bg.transform);
                _bugDrager[dragIndex].transform.SetSiblingIndex(_curBugSibling);
                if (dragBool)
                {
                    if (dragType == 0)
                    {
                        SuccessAni(dragIndex, _redBottle, "r2", "rrr3", "rrr", dragType);
                    }
                    if (dragType == 1)
                    {
                        SuccessAni(dragIndex, _blueBottle, "b2", "bbb3", "bbb", dragType);
                    }
                    if (dragType == 2)
                    {
                        SuccessAni(dragIndex, _yellowBottle, "y2", "yyy3", "yyy", dragType);
                    }
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    //虫子回归初始状态
                    if (dragType == 0)
                    {
                        SpineManager.instance.DoAnimation(_bugDrager[dragIndex].transform.GetGameObject("Bug"), "rrr", true);
                    }
                    if (dragType == 1)
                    {
                        SpineManager.instance.DoAnimation(_bugDrager[dragIndex].transform.GetGameObject("Bug"), "bbb", true);
                    }
                    if (dragType == 2)
                    {
                        SpineManager.instance.DoAnimation(_bugDrager[dragIndex].transform.GetGameObject("Bug"), "yyy", true);
                    }

                    //瓶子回归初始状态
                    SpineManager.instance.DoAnimation(_redBottle, "r3", false);
                    SpineManager.instance.DoAnimation(_blueBottle, "b3", false);
                    SpineManager.instance.DoAnimation(_yellowBottle, "y3", false);
                    _bugDrager[dragIndex].DoReset();
                    StartDrag();
                }
            }
        }

        //正确的拖拽动画
        void SuccessAni(int index, GameObject bottle, string bottleName, string bugAniName, string bugAfterAniName, int endIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            _bugDrager[index].transform.DOLocalMove(_bugDroper[endIndex].transform.localPosition, 0.5f);
            _bugDrager[index].transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(_bugDrager[index].transform.GetGameObject("Bug"), bugAniName, false,
            () =>
            {
                SpineManager.instance.DoAnimation(_bugDrager[index].transform.GetGameObject("Bug"), bugAfterAniName);
            });
            float len = SpineManager.instance.DoAnimation(bottle, bottleName, false);
            mono.StartCoroutine(WaitCoroutine(len,
            () =>
            {
                if (bottle.name == "RedBottle")
                    _redPoint += 1;
                if (bottle.name == "BlueBottle")
                    _bluePoint += 1;
                if (bottle.name == "YellowBottle")
                    _yellowPoint += 1;

                //防止分数超过4
                if (_redPoint >= 4)
                    _redPoint = 4;
                if (_bluePoint >= 4)
                    _bluePoint = 4;
                if (_yellowPoint >= 4)
                    _yellowPoint = 4;
                JudgePoint();
            }));
        }

        //点击事件
        private void ClickEvent(int obj)
        {

        }

        //虫虫炸弹进入瓶子区域事件
        void OnEnterEvent(GameObject o)
        {
            if (_isDraging && _canDrag)
            {
                if (o.name.Equals("RedEnd"))
                {
                    if (_curDragType == 0)
                    {
                        //瓶子放大，边缘发白光，瓶盖自动打开
                        SpineManager.instance.DoAnimation(_redBottle, "r", false, () => { SpineManager.instance.DoAnimation(_redBottle, "r4", true); });
                    }
                    else
                    {
                        //瓶子放大，边缘发白光，瓶子抖动
                        SpineManager.instance.DoAnimation(_redBottle, "r6", false, () => { SpineManager.instance.DoAnimation(_redBottle, "r3", true); });
                    }
                }
                if (o.name.Equals("BlueEnd"))
                {
                    if (_curDragType == 1)
                    {
                        //瓶子放大，边缘发白光，瓶盖自动打开
                        SpineManager.instance.DoAnimation(_blueBottle, "b", false, () => { SpineManager.instance.DoAnimation(_blueBottle, "b4", true); });
                    }
                    else
                    {
                        //瓶子放大，边缘发白光，瓶子抖动
                        SpineManager.instance.DoAnimation(_blueBottle, "b6", false, () => { SpineManager.instance.DoAnimation(_blueBottle, "b3", true); });
                    }
                }
                if (o.name.Equals("YellowEnd"))
                {
                    if (_curDragType == 2)
                    {
                        //瓶子放大，边缘发白光，瓶盖自动打开
                        SpineManager.instance.DoAnimation(_yellowBottle, "y", false, () => { SpineManager.instance.DoAnimation(_yellowBottle, "y4", true); });
                    }
                    else
                    {
                        //瓶子放大，边缘发白光，瓶子抖动
                        SpineManager.instance.DoAnimation(_yellowBottle, "y6", false, () => { SpineManager.instance.DoAnimation(_yellowBottle, "y3", true); });
                    }
                }
            }
        }

        //虫虫炸弹退出瓶子区域事件
        void OnExitEvent(GameObject o)
        {
            //瓶子回归初始状态
            SpineManager.instance.DoAnimation(_redBottle, "r3", false);
            SpineManager.instance.DoAnimation(_blueBottle, "b3", false);
            SpineManager.instance.DoAnimation(_yellowBottle, "y3", false);
        }

        //判断分数数值
        void JudgePoint()
        {
            for (int i = 0; i < 5; i++)
            {
                _redBugPoint.GetChild(i).gameObject.Hide();
                _blueBugPoint.GetChild(i).gameObject.Hide();
                _yellowBugPoint.GetChild(i).gameObject.Hide();
            }
            _redBugPoint.GetChild(_redPoint).gameObject.Show();
            _blueBugPoint.GetChild(_bluePoint).gameObject.Show();
            _yellowBugPoint.GetChild(_yellowPoint).gameObject.Show();
            mono.StartCoroutine(WaitCoroutine(0.5f, ()=> { JudgeBomb(); }));
        }

        //判断炸弹是否出现
        void JudgeBomb()
        {
            int runIndex = 4;  //判断执行哪个语句

            //防止炸弹炸恶魔时进行拖拽出现的拖拽bug
            if ((_redPoint == 4 && _devil1.activeSelf) || (_bluePoint == 4 && _devil2.activeSelf) || (_yellowPoint == 4 && _devil3.activeSelf))
            {
                mono.StartCoroutine(WaitCoroutine(2.2f, () => { StartDrag(); }));
            }

            if (_redPoint == 4 && _devil1.activeSelf)
                runIndex = 1;
            else if (_bluePoint == 4 && _devil2.activeSelf)
                runIndex = 2;
            else if (_yellowPoint == 4 && _devil3.activeSelf)
                runIndex = 3;
            else
                runIndex = 4;

            if (runIndex == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                mono.StartCoroutine(WaitCoroutine(0.1f, () => { SpineManager.instance.DoAnimation(_redBottle, "r5", false); }));
                mono.StartCoroutine(WaitCoroutine(0.667f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_bomb, "zd", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bomb, "kong", false);
                        SpineManager.instance.DoAnimation(_devil1, "xem-", false,
                        () =>
                        {
                            _devil1.Hide();
                            mono.StartCoroutine(WaitCoroutine(0.5f, () => { JudgeSuccess(); }));
                        });
                    });
                }));
            }
            if (runIndex == 2)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                mono.StartCoroutine(WaitCoroutine(0.1f, ()=> { SpineManager.instance.DoAnimation(_blueBottle, "b5", false); }));
                mono.StartCoroutine(WaitCoroutine(0.667f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_bomb, "zd2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bomb, "kong", false);
                        SpineManager.instance.DoAnimation(_devil2, "xem-2", false,
                        () =>
                        {
                            _devil2.Hide();
                            mono.StartCoroutine(WaitCoroutine(0.5f, () => { JudgeSuccess(); }));
                        });
                    });
                }));
            }
            if (runIndex == 3)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                mono.StartCoroutine(WaitCoroutine(0.1f, () => { SpineManager.instance.DoAnimation(_yellowBottle, "y5", false); }));
                mono.StartCoroutine(WaitCoroutine(0.667f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_bomb, "zd3", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_bomb, "kong", false);
                        SpineManager.instance.DoAnimation(_devil3, "xem-3", false, () =>
                        {
                            _devil3.Hide();
                            mono.StartCoroutine(WaitCoroutine(0.5f, () => { JudgeSuccess(); }));
                        });
                    });
                }));
            }
            if (runIndex == 4)
                StartDrag();
        }

        //胜利判定
        void JudgeSuccess()
        {
            if (_redPoint == 4 && _bluePoint == 4 && _yellowPoint == 4)
            {
                StopDrag();
                mono.StartCoroutine(WaitCoroutine(1.0f, () => { playSuccessSpine(); }));
            }
        }
        #endregion
    }
}
