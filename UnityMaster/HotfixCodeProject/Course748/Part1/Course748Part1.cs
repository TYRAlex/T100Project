using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course748Part1
    {
        #region 通用变量
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        Transform frameTra;
        Transform robotsTra;
        Transform numsTra;
        Transform linesTra;
        Transform dropersTra;

        GameObject box;
        GameObject smallBox;
        GameObject unDragableMask;

        RawImage bgMaskRaw;

        mILDrager[] dragers;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            frameTra = curTrans.Find("frame");
            robotsTra = frameTra.Find("robots");
            numsTra = frameTra.Find("nums");
            linesTra = frameTra.Find("lines");
            dropersTra = frameTra.Find("dropers");

            smallBox = frameTra.Find("smallBox").gameObject;
            unDragableMask = frameTra.Find("unDragableMask").gameObject;
            box = curTrans.Find("box").gameObject;

            bgMaskRaw = curTrans.Find("BgMask").GetRawImage();
        }

        private void GameInit()
        {
            talkIndex = 1;

            InitSpine(box, "yyh1", false);

            box.SetActive(true);
            bgMaskRaw.gameObject.SetActive(false);
            frameTra.gameObject.SetActive(false);
            unDragableMask.gameObject.SetActive(false);

            //拖拽
            int n = robotsTra.childCount;
            dragers = new mILDrager[n];

            for (int i = 0; i < n; i++)
            {
                Transform tra = robotsTra.GetChild(i);
                dragers[i] = tra.GetComponent<mILDrager>();
                dragers[i].SetDragCallback(DragStart, null, DragEnd);
                dragers[i].isActived = true;

                GameObject childObj = tra.GetChild(0).gameObject;
                InitSpine(childObj, "jqr" + childObj.name);
            }

            //打乱位置
            Shuffle(dragers);

            for (int i = 0; i < dragers.Length; i++)
            {
                RectTransform rect = dragers[i].transform.GetRectTransform();
                rect.anchoredPosition = Vector2.right * (i - 2) * (287.5f);
                dragers[i].dragType = i;
            }
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
            {
                //靠近音乐盒
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0));

                mono.StartCoroutine(WaitAnimation(box, "yyh", 2, 3, () =>
                {
                    //离开音乐盒
                    mono.StartCoroutine(SpeakAndAnimation(box, cur: "yyh4", method: () =>
                     {
                         SoundManager.instance.ShowVoiceBtn(true);
                     }));
                }));
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //进入下个环节
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    mono.StartCoroutine(WhiteCurtainTransition(bgMaskRaw, () =>
                    {
                        box.SetActive(false);
                        frameTra.gameObject.SetActive(true);
                        GameInit2();

                        mono.StartCoroutine(WaitFor(0.5f, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                            {
                                Max.SetActive(false);
                            }));
                        }));
                    }));
                }));
            }
            else if (talkIndex == 2)
            {
                //进入下个环节
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0));
            }

            talkIndex++;
        }

        //第二环节初始化
        void GameInit2()
        {
            isPlaying = false;
            unDragableMask.SetActive(true);

            //放置
            for (int i = 0; i < dropersTra.childCount; i++)
            {
                Transform tra = dropersTra.GetChild(i);
                tra.GetComponent<mILDroper>().isActived = false;
            }

            //音乐盒
            InitSpine(smallBox, "xyyh1", false);

            //数字
            for (int i = 0; i < numsTra.childCount; i++)
            {
                Transform tra = numsTra.GetChild(i);
                tra.GetRawImage().color = Color.white;
                tra.localScale = Vector2.one;
                Util.AddBtnClick(tra.gameObject, Click);
            }

            //线条
            for (int i = 0; i < linesTra.childCount; i++)
            {
                Transform tra = linesTra.GetChild(i);
                tra.gameObject.SetActive(false);
            }

            ShowOneOnly(numsTra, 0);

            //排序
            dragers = DragSort(dragers);
        }

        #region 游戏方法
        //点击
        void Click(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;
            SoundManager.instance.PlayClip(9);

            //点击反馈
            ClickFeedback(obj.transform);
            ColorDisPlay(obj.transform.GetRawImage(), false, () => isPlaying = false);

            //可以拖拽放置
            int num = obj.transform.GetSiblingIndex();
            dropersTra.GetChild(num).GetComponent<mILDroper>().isActived = true;
            unDragableMask.SetActive(false);
        }

        //拖拽正确动画
        void TrueAnimation(int num)
        {
            Transform dragTra = dragers[num].transform;
            dragTra.DOScale(Vector2.one * 0.75f, 0.5f).SetEase(Ease.OutBack);

            ColorDisPlay(linesTra.GetChild(num).GetRawImage());

            SpineManager.instance.DoAnimation(smallBox, "xyyh" + (num + 1), false, () =>
            {
                if (num == 4)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                else
                {
                    ColorDisPlay(numsTra.GetChild(num + 1).GetRawImage());
                }
            });
        }
        #endregion
        #region 拖拽
        void DragStart(Vector3 position, int type, int index)
        {
            dragers[index].transform.position = Input.mousePosition;
            SoundManager.instance.PlayClip(9);
            dragers[index].transform.SetAsLastSibling();
        }

        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            if (isMatch)
            {
                dragers[index].isActived = false;
                Transform tra = dragers[index].drops[0].transform;
                dragers[index].transform.position = tra.position;

                unDragableMask.SetActive(true);

                TrueAnimation(index);
            }
            else
            {
                dragers[index].transform.GetRectTransform().anchoredPosition = Vector2.right * (type - 2) * (287.5f); ;
            }
        }
        #endregion

        #region 通用方法
        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

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

        //点击反馈
        void ClickFeedback(Transform tar)
        {
            SoundManager.instance.PlayClip(9);

            Vector2 curScale = tar.localScale;

            tar.DOScale(curScale * 0.9f, 1 / 6f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tar.DOScale(curScale, 1 / 3f).SetEase(Ease.InOutSine);
            });
        }

        //只显示一个子物件
        void ShowOneOnly(Transform _tra, int _num)
        {
            for (int i = 0; i < _tra.childCount; i++)
            {
                _tra.GetChild(i).gameObject.SetActive(false);
            }

            Transform tra = _tra.GetChild(_num);
            tra.gameObject.SetActive(true);
        }

        void Shuffle<T>(T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = (UnityEngine.Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
        }

        mILDrager[] DragSort(mILDrager[] dragers)
        {
            int n = dragers.Length;
            if (n == 0) return null;

            mILDrager[] ret = new mILDrager[n];

            for (int i = 0; i < n; ++i)
            {
                ret[i] = dragers[i];
            }

            for (int i = 0; i < n; ++i)
            {
                ret[dragers[i].index] = dragers[i];
            }

            return ret;
        }

        //白幕转场
        IEnumerator WhiteCurtainTransition(RawImage _raw, Action method = null)
        {
            _raw.color = new Color(1, 1, 1, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.white, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.2f);

            _raw.DOColor(new Color(1, 1, 1, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

            method?.Invoke();
        }

        //连续播放动画(首字母一样)
        IEnumerator WaitAnimation(GameObject _obj, string _initAnimation, int _start, int _end, Action _method = null)
        {
            for (int i = _start; i <= _end; i++)
            {
                float time = SpineManager.instance.DoAnimation(_obj, _initAnimation + i, false);

                yield return new WaitForSeconds(time);
            }

            _method?.Invoke();
        }

        //播放动画和语音
        IEnumerator SpeakAndAnimation(GameObject obj, string pre = null, string cur = null, string aft = null, int index = -1, Action method = null)
        {
            GameObject speaker = Max;

            //说话前动画
            float _time = 0.01f;
            if (pre != null) _time = SpineManager.instance.DoAnimation(obj, pre, false);
            yield return new WaitForSeconds(_time);

            //说话语音
            float speakTime = 0.01f;
            if (index != -1)
            {
                mono.StartCoroutine(SpeckerCoroutine(speaker, SoundManager.SoundType.COMMONVOICE, 0));
                speakTime = SoundManager.instance.GetLength(SoundManager.SoundType.COMMONVOICE, 0);
            }

            //说话的时候播放动画
            float animationTime = 0.01f;
            if (cur != null) animationTime = SpineManager.instance.DoAnimation(obj, cur, false);

            _time = speakTime > animationTime ? speakTime : animationTime;
            yield return new WaitForSeconds(_time);

            _time = 0.01f;
            if (aft != null) _time = SpineManager.instance.DoAnimation(obj, aft, false);
            yield return new WaitForSeconds(_time);


            method?.Invoke();
        }


        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, GetSpineAnimationName(speaker, 0));
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, GetSpineAnimationName(speaker, 1));

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, GetSpineAnimationName(speaker, 0));
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }

        //获取Spine动画列表中第n个动画的名字
        string GetSpineAnimationName(GameObject _obj, int _index = 0)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();
            Spine.Animation _animation = _ske.AnimationState.Data.SkeletonData.Animations.Items[_index];
            return _animation.Name;
        }

        #endregion
    }
}
