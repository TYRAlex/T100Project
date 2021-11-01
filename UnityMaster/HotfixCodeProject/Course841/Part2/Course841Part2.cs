using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course841Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject Max;

        private Transform _level1;
        private GameObject _muscle1;
        private Transform _click1;
        private GameObject[] _clickArray1;
        private int[] _flag;

        private Transform _level2;
        private GameObject _muscle2;
        private GameObject _human2;
        private GameObject _false;
        private GameObject _maskLevel2;
        private Transform _click2;
        private GameObject[] _clickArray2;
        private int[] _clickObjIndex;
        private Coroutine _canStopCoroutine;

        private Transform _last;
        private GameObject _maskLast;
        private GameObject _humanLast;
        private Transform _countPng;    //单个数字
        private Transform _countGPng;   //个位
        private Transform _countSPng;   //十位

        private GameObject _startBtn;
        private GameObject _restartBtn;
        private bool _canClick;
        private bool _canEnd;
        private int _clickCount;
        private int _allCount;

        private int _number;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg2").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("MAX").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _level1 = curTrans.Find("Level1");
            _muscle1 = curTrans.GetGameObject("Level1/muscle");
            _click1 = curTrans.Find("Level1/Click");
            _clickArray1 = new GameObject[_click1.childCount];
            for (int i = 0; i < _clickArray1.Length; i++)
            {
                _clickArray1[i] = _click1.GetChild(i).gameObject;
                Util.AddBtnClick(_clickArray1[i], ClickLevel1);
            }
            _flag = new int[3] { 1, 2, 4 };

            _level2 = curTrans.Find("Level2");
            _human2 = curTrans.GetGameObject("Level2/human1");
            _muscle2 = curTrans.GetGameObject("Level2/muscle");
            _false = curTrans.GetGameObject("Level2/false");
            _maskLevel2 = curTrans.GetGameObject("Level2/mask");
            _click2 = curTrans.Find("Level2/Click");
            _clickObjIndex = new int[6];
            _clickArray2 = new GameObject[_click2.childCount];
            for (int i = 0; i < _clickArray2.Length; i++)
            {
                _clickArray2[i] = _click2.GetChild(i).gameObject;
                Util.AddBtnClick(_clickArray2[i], ClickLevel2);
            }

            _last = curTrans.Find("Last");
            _maskLast = curTrans.GetGameObject("Last/mask");
            _humanLast = curTrans.GetGameObject("Last/humanEnd");
            _countPng = curTrans.Find("Last/Count");
            _countGPng = curTrans.Find("Last/CountG");
            _countSPng = curTrans.Find("Last/CountS");

            _startBtn = curTrans.GetGameObject("Level2/start");
            _restartBtn = curTrans.GetGameObject("Last/restart");

            Util.AddBtnClick(_startBtn, GameStart);
            Util.AddBtnClick(_restartBtn, GameRestart);
            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _number = 0;
            _canClick = false;

            SpineManager.instance.DoAnimation(_muscle1, "kong", false);
            SpineManager.instance.DoAnimation(_muscle2, "kong", false);

            _level1.gameObject.Show();
            _level2.gameObject.Hide();
            _last.gameObject.Hide();
        }

        void GameStart()
        {
            Max.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true;}));
        }

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
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }
        Coroutine ReturnCoroutine(Action method_1 = null, float len = 0)
        {
            return mono.StartCoroutine(FinishOneCoroutine(method_1, len));
        }
        IEnumerator FinishOneCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StopAllCoroutines();
                _level1.gameObject.Hide();
                _level2.gameObject.Show();
                _startBtn.Hide();
                _maskLevel2.Show();
                _false.Hide();

                _human2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                _startBtn.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                _muscle2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_human2, "ydy", true);
                SpineManager.instance.DoAnimation(_startBtn.transform.GetChild(0).gameObject, "ks3", true);
                SpineManager.instance.DoAnimation(_muscle2, "kong", false);

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () => { Max.SetActive(false); _startBtn.Show(); _canClick = true; }));
            }

            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        #region 方法
        private void ClickLevel1(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);

                int index = int.Parse(obj.name);
                for (int i = 0; i < _flag.Length; i++)
                {
                    if (_flag[i] == index)
                        _flag[i] = 0;
                }
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index == 4 ? index - 1 : index, 
                () => 
                {
                    SpineManager.instance.DoAnimation(_muscle1, "jrc" + index.ToString(), false, 
                    () => 
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SpineManager.instance.DoAnimation(_muscle1, "jrg" + index.ToString(), false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_muscle1, "jr" + index.ToString(), false);
                        });
                    });
                }, 
                () => 
                {
                    SpineManager.instance.DoAnimation(_muscle1, "jrx" + index.ToString(), false, 
                    ()=> 
                    { 
                        _canClick = true;
                        FlagCheck();
                    });
                }));
            }
        }

        void FlagCheck()
        {
            bool allClick = true;
            for (int i = 0; i < _flag.Length; i++)
            {
                if (_flag[i] != 0)
                {
                    allClick = false;
                    return;
                }
            }

            if (allClick)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }

        private void ClickLevel2(GameObject obj)
        {
            if(_canClick)
            {
                if(obj.name != "3")
                {
                    _clickCount += 1;
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_muscle2, "jrg" + obj.name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_muscle2, "jr" + obj.name, false);
                    });
                }
                else
                {
                    _canClick = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    mono.StopCoroutine(_canStopCoroutine);
                    _false.Show();
                    SpineManager.instance.DoAnimation(_muscle2, "kong", false);
                    SpineManager.instance.DoAnimation(_false, "1", false, () => { _false.Hide(); RandomAni(); });
                }
            }
        }

        private void GameStart(GameObject obj)
        {
            if (_canClick)
            {
                BtnPlaySound();
                _canClick = false;
                SpineManager.instance.DoAnimation(_startBtn.transform.GetChild(0).gameObject, "ks1", false, 
                () => 
                {
                    StartLevel2();
                });
            }
        }

        void StartLevel2()
        {
            mono.StopAllCoroutines();
            _canClick = true;
            _number = 0;
            _clickCount = 0;
            _allCount = 0;
            _clickObjIndex = new int[6] { 1, 3, 2, 3, 1, 2 };
            int addLen = Random.Range(1, 4);
            for (int a = 0; a < addLen; a++)
            {
                for (int i = 0; i < _clickObjIndex.Length - 1; i++)
                {
                    int temp = _clickObjIndex[i];
                    _clickObjIndex[i] = _clickObjIndex[i + 1];
                    _clickObjIndex[i + 1] = temp;
                }
            }
            _canEnd = false;

            _maskLevel2.Hide();
            _startBtn.Hide();
            RandomAni();
        }

        void RandomAni()
        {
            _clickCount = 0;
            _canClick = true;
            if (_number < 6)
            {
                _muscle2.Show();
                _muscle2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_muscle2, "jr" + _clickObjIndex[_number].ToString(), false);
                for (int i = 0; i < _clickArray2.Length; i++)
                {
                    if (_clickArray2[i].name != _clickObjIndex[_number].ToString())
                        _clickArray2[i].Hide();
                    else
                        _clickArray2[i].Show();
                }
                _number++;

                _canStopCoroutine = ReturnCoroutine(
                () => 
                {
                    _muscle2.Hide();
                    _canClick = false;
                    int i = _clickCount / 5;
                    Debug.Log("点击次数：" + _clickCount);
                    Debug.Log("运动员要做的引体向上数：" + i);
                    _allCount += i;

                    SportCount(i);
                }, _clickObjIndex[_number - 1] == 3 ? 1.5f : 4.0f);
            }
            else
            {
                _muscle2.Hide();
                _canClick = false;
                mono.StartCoroutine(WaitCoroutine(
                () =>
                {
                    _level2.gameObject.Hide();
                    _last.gameObject.Show();
                    _maskLast.Hide();
                    _restartBtn.Hide();

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

                    SpineManager.instance.DoAnimation(_restartBtn.transform.GetChild(0).gameObject, "cx3", true);
                    SpineManager.instance.DoAnimation(_last.GetGameObject("caidai"), "animation", false);
                    if (_allCount >= 10)
                    {
                        _countGPng.gameObject.Show();
                        _countSPng.gameObject.Show();
                        _countPng.gameObject.Hide();
                        _countSPng.GetComponent<RawImage>().texture = _countPng.GetComponent<BellSprites>().texture[_allCount / 10];
                        _countGPng.GetComponent<RawImage>().texture = _countPng.GetComponent<BellSprites>().texture[_allCount % 10];
                    }
                    else
                    {
                        _countGPng.gameObject.Hide();
                        _countSPng.gameObject.Hide();
                        _countPng.gameObject.Show();
                        _countPng.GetComponent<RawImage>().texture = _countPng.GetComponent<BellSprites>().texture[_allCount];
                    }

                    mono.StartCoroutine(WaitCoroutine(
                    () =>
                    {
                        _canClick = true;
                        _maskLast.Show();
                        _restartBtn.Show();
                    }, 3.0f));
                }, 0.5f));
            }
        }

        void SportCount(int i)
        {
            if(i != 0)
            {
                i--;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(_human2, "ydy2", false, () =>
                {
                    SpineManager.instance.DoAnimation(_human2, "ydy", false);
                    if (i == 0)
                        RandomAni();
                    else
                        SportCount(i);
                });
            }
            else
                RandomAni();
        }

        private void GameRestart(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                BtnPlaySound();
                SpineManager.instance.DoAnimation(_restartBtn.transform.GetChild(0).gameObject, "cx1", false,
                () =>
                {
                    _level2.gameObject.Show();
                    _last.gameObject.Hide();
                    StartLevel2();
                });
            }
        }
        #endregion
    }
}
