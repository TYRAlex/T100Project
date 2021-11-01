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
    public class Course838Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject _carAni;
        private GameObject _arrow;
        private GameObject _black;
        private GameObject _white;
        private GameObject[] _blackArrow;
        private GridLayoutGroup _blackGrid;
        private RectTransform _blackRect;
        private GameObject[] _whiteArrow;
        private GridLayoutGroup _whiteGrid;
        private RectTransform _whiteRect;
        private Dictionary<string, Vector2> _blackDatas;
        private Dictionary<string, Vector2> _whiteDatas;
        private bool _canClick;
        private int _blackIndex;
        private int _whiteIndex;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);

            _carAni = curTrans.Find("Car/carAni").gameObject;
            _arrow = curTrans.GetGameObject("Arrow");
            _black = curTrans.GetGameObject("Arrow/Black");
            _blackGrid = _black.GetComponent<GridLayoutGroup>();
            _blackRect = _black.GetComponent<RectTransform>();
            _white = curTrans.GetGameObject("Arrow/White");
            _whiteGrid = _white.GetComponent<GridLayoutGroup>();
            _whiteRect = _white.GetComponent<RectTransform>();

            InitAndAddDictionary();

            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _black.SetActive(true);
            _white.SetActive(true);
            _arrow.SetActive(true);
            _canClick = false;
            SpineManager.instance.DoAnimation(_carAni, "3t");
            _blackArrow = new GameObject[3];
            for (int i = 0; i < _black.transform.childCount - 1; i++)
            {
                _blackArrow[i] = _black.transform.GetChild(i + 1).gameObject;
                Util.AddBtnClick(_blackArrow[i], BlackClick);
            }
            _whiteArrow = new GameObject[3];
            for (int i = 0; i < _white.transform.childCount - 1; i++)
            {
                _whiteArrow[i] = _white.transform.GetChild(i + 1).gameObject;
                Util.AddBtnClick(_whiteArrow[i], WhiteClick);
            }

            _blackIndex = -1;
            _whiteIndex = -1;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            //if (bellTextures.texture.Length <= 0)
            //{
            //    Debug.LogError("@愚蠢！！ 哈哈哈 Bg上的BellSprites 里没有东西----------添加完删掉这个打印");
            //}
            //else
            //{
            //    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            //}

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            //画面中的小车只有一个颜色传感器，请同学们选择一种正确的线路方式，帮助小车完成走线任务
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                _blackRect.anchoredPosition = _blackDatas["_blackFirstPos"];
                _blackRect.sizeDelta = _blackDatas["_blackFirstSize"];
                _blackGrid.spacing = _blackDatas["_blackFirstSpacing"];
                _whiteRect.anchoredPosition = _whiteDatas["_whiteFirstPos"];
                _whiteRect.sizeDelta = _whiteDatas["_whiteFirstSize"];
                _whiteGrid.spacing = _whiteDatas["_whiteFirstSpacing"];
                for (int i = 0; i < _blackArrow.Length; i++)
                {
                    _blackArrow[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                }
                for (int i = 0; i < _whiteArrow.Length; i++)
                {
                    _whiteArrow[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                }
            },
            () =>
            {
                _blackRect.anchoredPosition = _blackDatas["_blackSecondPos"];
                _blackRect.sizeDelta = _blackDatas["_blackSecondSize"];
                _blackGrid.spacing = _blackDatas["_blackSecondSpacing"];
                _whiteRect.anchoredPosition = _whiteDatas["_whiteSecondPos"];
                _whiteRect.sizeDelta = _whiteDatas["_whiteSecondSize"];
                _whiteGrid.spacing = _whiteDatas["_whiteSecondSpacing"];
                _canClick = true;
                bell.SetActive(false);
            }, 0));

        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        //自定义动画协程
        IEnumerator AniCoroutine(Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            
            if (method_1 != null)
            {
                method_1();
            }

            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        //字典初始化与添加键值对
        void InitAndAddDictionary()
        {
            //黑色传感器字典添加键值对
            _blackDatas = new Dictionary<string, Vector2>();
            _blackDatas.Add("_blackFirstPos", new Vector2(192, -172));
            _blackDatas.Add("_blackSecondPos", new Vector2(42, -172));
            _blackDatas.Add("_blackFirstSize", new Vector2(1300, 250));
            _blackDatas.Add("_blackSecondSize", new Vector2(1600, 250));
            _blackDatas.Add("_blackFirstSpacing", new Vector2(60, 0));
            _blackDatas.Add("_blackSecondSpacing", new Vector2(150, 0));

            //白色传感器字典添加键值对
            _whiteDatas = new Dictionary<string, Vector2>();
            _whiteDatas.Add("_whiteFirstPos", new Vector2(192, -422));
            _whiteDatas.Add("_whiteSecondPos", new Vector2(42, -422));
            _whiteDatas.Add("_whiteFirstSize", new Vector2(1300, 250));
            _whiteDatas.Add("_whiteSecondSize", new Vector2(1600, 250));
            _whiteDatas.Add("_whiteFirstSpacing", new Vector2(60, 0));
            _whiteDatas.Add("_whiteSecondSpacing", new Vector2(150, 0));
        }

        //黑色传感器点击事件
        void BlackClick(GameObject obj)
        {
            if(_canClick)
            {
                JudgeChoose(obj, _blackArrow, _whiteArrow, ref _blackIndex);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            }
        }

        //白色传感器点击事件
        void WhiteClick(GameObject obj)
        {
            if (_canClick)
            {
                JudgeChoose(obj, _whiteArrow, _blackArrow, ref _whiteIndex);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            }
        }

        //判断能否正常点亮方向箭头
        void JudgeChoose(GameObject obj, GameObject[] currentArrows, GameObject[] anotherArrows, ref int currentIndex)
        {
            bool needChange = false;    //点击的方向箭头和已点亮的方向箭头是否是同一个，同一个则不做修改，不同再做修改
            int arrowCount = 0;     //点亮的箭头数量

            //点击白色直线时，黑色直线已亮
            if (obj.name == "Arrow01" && anotherArrows[0].GetComponent<RawImage>().color == new Color(0, 255, 255, 255))
            {
                return;
            }
            //点击白色左转弯时，黑色左转弯已亮
            else if (obj.name == "Arrow02" && anotherArrows[1].GetComponent<RawImage>().color == new Color(0, 255, 255, 255))
            {
                return;
            }
            //点击白色右转弯时，黑色右转弯已亮
            else if (obj.name == "Arrow03" && anotherArrows[2].GetComponent<RawImage>().color == new Color(0, 255, 255, 255))
            {
                return;
            }
            //除以上情况外可正常点亮
            else
            {
                for (int i = 0; i < currentArrows.Length; i++)
                {
                    if (currentArrows[i].GetComponent<RawImage>().color == new Color(0, 255, 255, 255))
                    {
                        arrowCount++;
                        //有已点亮，但与点击的不同
                        if (currentArrows[i].name != obj.name)
                        {
                            currentArrows[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                            needChange = true;
                            break;
                        }
                    }
                }

                //有已点亮，但与点击的不同的情况或者是新点击的情况，都需要点亮
                if (needChange || arrowCount == 0)
                {
                    obj.GetComponent<RawImage>().color = new Color(0, 255, 255, 255);
                    for (int i = 0; i < currentArrows.Length; i++)
                    {
                        if (currentArrows[i].GetComponent<RawImage>().color == new Color(0, 255, 255, 255))
                        {
                            currentIndex = i;
                        }
                    }
                    //新点亮方向箭头时再判断
                    JudgePlayAni();
                }
            }
        }

        //点击结束判断是否黑白传感器各选了一个，以此来播放动画
        void JudgePlayAni()
        {
            //检测到黑线直行，检测到白线左转弯时（不能完成任务）
            if (_blackIndex == 0 && _whiteIndex == 1)
            {
                CarPlayAni("animation", 2.333f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            }
            //检测到黑线直行，检测到白线右转弯时（不能完成任务）
            if (_blackIndex == 0 && _whiteIndex == 2)
            {
                CarPlayAni("animation2", 2.667f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            }
            //检测到黑线左转弯，检测到白线直行时（不能完成任务）
            if (_blackIndex == 1 && _whiteIndex == 0)
            {
                CarPlayAni("animation3", 2.0f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            }
            //检测到黑线左转弯，检测到白线右转弯时（可以完成任务）
            if (_blackIndex == 1 && _whiteIndex == 2)
            {
                CarPlayAni("animation5", 6.0f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
            }
            //检测到黑线右转弯，检测到白线直行时（不能完成任务）
            if (_blackIndex == 2 && _whiteIndex == 0)
            {
                CarPlayAni("animation4", 2.0f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            }
            //检测到黑线右转弯，检测到白线左转弯时（可以完成任务）
            if (_blackIndex == 2 && _whiteIndex == 1)
            {
                CarPlayAni("animation6", 6.0f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            }
        }

        //动画播放完所有箭头恢复初始状态
        void ResetArrow()
        {
            for (int i = 0; i < _blackArrow.Length; i++)
            {
                _blackArrow[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
            }
            for (int i = 0; i < _whiteArrow.Length; i++)
            {
                _whiteArrow[i].GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
            }
            _blackIndex = -1;
            _whiteIndex = -1;
        }

        //小车动画播放
        void CarPlayAni(string aniName, float aniTime)
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                _canClick = false;
                _black.SetActive(false);
                _white.SetActive(false);
                SpineManager.instance.DoAnimation(_carAni, aniName, false);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(_carAni, "3t", false);
                },
                () =>
                {
                    _black.SetActive(true);
                    _white.SetActive(true);
                    ResetArrow();
                    _canClick = true;
                }, 0.3f));
            }, aniTime + 0.5f));
        }
    }
}
