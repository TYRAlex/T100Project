using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course941Part4
    {
        #region 通用变量
        int talkIndex;

        bool isPlaying = false;

        GameObject curGo;
        GameObject bg;
        GameObject bell;

        Transform curTrans;

        MonoBehaviour mono;
        #endregion

        #region 游戏变量
        Transform booksTra;
        Transform clicksTra;

        GameObject[] books;

        int page = 0;
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

            bg = curTrans.Find("Bg").gameObject;
            bell = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            booksTra = curTrans.Find("book");
            clicksTra = curTrans.Find("Clicks");

            int n = booksTra.childCount;
            books = new GameObject[n];
            for (int i = 0; i < n; i++)
            {
                books[i] = booksTra.GetChild(i).gameObject;
            }
        }

        void GameInit()
        {
            talkIndex = 1;

            HideAllAndShowOne(booksTra, 0);
            HideAllAndShowOne(clicksTra, 0);

            InitSpine(books[0], "fengmian", false);

            Util.AddBtnClick(clicksTra.GetChild(0).gameObject, OpenBook);
            Util.AddBtnClick(clicksTra.GetChild(1).gameObject, Click);
            Util.AddBtnClick(clicksTra.GetChild(2).gameObject, Click);
        }

        void GameStart()
        {
            bell.SetActive(true);
            isPlaying = true;

            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
            {
                bell.SetActive(false);
                isPlaying = false;
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }

        #region 游戏方法
        private void Click(GameObject obj)
        {
            Debug.Log(isPlaying);

            if (isPlaying) return;
            isPlaying = true;

            int end = 6;
            int num = obj.transform.GetSiblingIndex();
            int delta = num == 1 ? -1 : 1;
            page += delta;
            float time = 0f;

            Debug.Log("curPage: " + page);

            //合上书
            if (page == 0 || page == end)
            {
                time = InitSpine(books[0], page == 0 ? "guanbi" : "guanbi2", false);

                HideAllAndShowOne(booksTra, 0);
                HideAllAndShowOne(clicksTra, 0);
                page = 0;
            }
            else
            {
                //翻页
                time = ChangePage(page, num == 2);
            }

            mono.StartCoroutine(WaitFor(time, () =>
            {
                //翻页之后的动画和语音
                switch (page)
                {
                    case 2:
                        mono.StartCoroutine(SpeakAndAnimation(books[3], "dha1", "dha2", "dha3", 0, () => isPlaying = false));
                        break;
                    case 4:
                        mono.StartCoroutine(SpeakAndAnimation(books[3], null, "dhb1", "dhb2", 0, () =>
                        {
                            isPlaying = false;
                            InitSpine(books[1], "7b", false);
                            InitSpine(books[2], "8b", false);
                        }));
                        break;
                    case 5:
                        mono.StartCoroutine(SpeakAndAnimation(books[3], null, "dhc1", "dhc2", 0, () =>
                        {
                            isPlaying = false;
                            InitSpine(books[1], "9a", false);
                            InitSpine(books[2], "10a", false);
                        }));
                        break;
                    default:
                        isPlaying = false;
                        break;
                }
            }));
        }

        //翻页
        float ChangePage(int page, bool isRight = true)
        {
            int pre, cur;
            string animation = isRight ? "a" : "b";

            if (isRight)
            {
                page -= 1;
                pre = 2;
                cur = 1;
            }
            else
            {
                pre = 1;
                cur = 2;
            }

            //翻页动画
            float time = InitSpine(books[3], animation + page, false);

            //初始化要翻的那页
            int index = isRight ? page * 2 : (page - 1) * 2;
            InitSpine(books[pre], "" + (index + pre), false);

            mono.StartCoroutine(WaitFor(time - 0.1f, () =>
              {
                //初始化翻过去的那页
                InitSpine(books[cur], "" + (index + cur), false);
              }));

            return time;
        }

        //打开书
        void OpenBook(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            obj.SetActive(false);
            float time = InitSpine(books[0], "dakai", false);
            InitSpine(books[1], "1", false);
            InitSpine(books[2], "2", false);
            InitSpine(books[3], "", false);

            mono.StartCoroutine(WaitFor(time, () =>
            {
                ++page;
                isPlaying = false;

                HideAllAndShowOne(booksTra, -1, true);
                HideAllAndShowOne(clicksTra, 0, true);
            }));
        }
        #endregion

        #region 通用方法
        //播放动画和语音
        IEnumerator SpeakAndAnimation(GameObject obj, string pre = null, string cur = null, string aft = null, int index = -1, Action method = null)
        {
            GameObject speaker = bell;

            //说话前动画
            float _time = 0.01f;
            if (pre != null) _time = SpineManager.instance.DoAnimation(obj, pre, false);
            yield return new WaitForSeconds(_time);

            //说话语音
            float speakTime = 0.01f;
            if (index != -1)
            {
                mono.StartCoroutine(SpeckerCoroutine(speaker, SoundManager.SoundType.COMMONVOICE, index));
                speakTime = SoundManager.instance.GetLength(SoundManager.SoundType.COMMONVOICE, index);
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

        //隐藏或显示所有物体 再显示或者隐藏一个物件
        void HideAllAndShowOne(Transform _tra, int _num = -1, bool isShow = false)
        {
            for (int i = 0; i < _tra.childCount; i++)
            {
                _tra.GetChild(i).gameObject.SetActive(isShow);
            }

            if (_num >= 0)
            {
                _tra.GetChild(_num)?.gameObject.SetActive(!isShow);
            }
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bell;
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
        float InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);

            if (animation == "")
            {
                return 0f;
            }
            else
            {
                return _ske.AnimationState.Data.SkeletonData.FindAnimation(animation).Duration;
            }
        }

        //获取Spine动画列表中第n个动画的名字
        string GetSpineAnimationName(GameObject _obj, int _index = 0)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();
            Spine.Animation _animation = _ske.AnimationState.Data.SkeletonData.Animations.Items[_index];
            return _animation.Name;
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }
        #endregion
    }
}
