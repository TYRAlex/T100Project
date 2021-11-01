using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course941Part2
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
        int curNum = 0;
        bool[] isClickeds;

        Transform bottomTra;
        GameObject[] spines;

        GameObject backButton;
        GameObject back;

        RawImage maskRaw;
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
            bottomTra = curTrans.Find("bottom");
            spines = new GameObject[bottomTra.childCount];

            for (int i = 0; i < bottomTra.childCount; i++)
            {
                spines[i] = bottomTra.GetChild(i).gameObject;
            }

            back = curTrans.Find("back").gameObject;
            backButton = curTrans.Find("backButton").gameObject;

            maskRaw = curTrans.Find("mask").GetRawImage();
        }

        private void GameInit()
        {
            talkIndex = 1;

            HideAllAndShowOne(bottomTra, 0);

            InitSpine(spines[0], "heiban", false);

            back.SetActive(false);
            backButton.SetActive(false);
            maskRaw.gameObject.SetActive(false);

            back.transform.localScale = Vector2.one;

            Util.AddBtnClick(back, Back);
            Util.AddBtnClick(backButton, BackButton);

            //初始化点击
            for (int i = 0; i < spines[0].transform.childCount; i++)
            {
                GameObject obj = spines[0].transform.GetChild(i).gameObject;
                InitSpine(obj, "heiban" + obj.name + 4, false);

                GameObject objChild = obj.transform.GetChild(0).gameObject;
                Util.AddBtnClick(objChild, Click);
                objChild.name = "0";
            }

            //打开放大
            for (int i = 0; i < spines[1].transform.childCount; i++)
            {
                GameObject obj = spines[1].transform.GetChild(i).gameObject;
                obj.SetActive(true);
                Util.AddBtnClick(obj, Enlarge);
            }

            isClickeds = new bool[spines[1].transform.childCount];
            for (int i = 0; i < isClickeds.Length; i++)
            {
                isClickeds[i] = false;
            }
        }

        void GameStart()
        {
            bell.SetActive(true);
            isPlaying = true;

            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
            {
                isPlaying = false;
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //关闭全部点击
                HideAllAndShowOne(spines[1].transform, -1);

                InitSpine(spines[1], "jiangjieb6", false);

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    talkIndex = 2;
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    back.SetActive(true);
                }));
            }

            talkIndex++;
        }

        #region 游戏方法
        //返回讲解界面
        void Back(GameObject obj)
        {
            ClickFeedback(obj.transform);

            mono.StartCoroutine(WaitFor(0.5f, () =>
            {
                back.SetActive(false);

                HideAllAndShowOne(bottomTra, 0);
            }));
        }

        //点击
        void Click(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            GameObject objParent = obj.transform.parent.gameObject;
            int num = objParent.transform.GetSiblingIndex();

            if (obj.name == "0")
            {
                float time = InitSpine(objParent, "heiban" + objParent.name + 1, false);

                mono.StartCoroutine(WaitFor(time, ()=>
                {
                    InitSpine(objParent, "heiban" + objParent.name + 2, false);
                    isPlaying = false;
                }));

                obj.name = "1";
            }
            else
            {
                float time = InitSpine(objParent, "heiban" + objParent.name + 3, false);

                mono.StartCoroutine(WaitFor(time, () =>
                {
                    mono.StartCoroutine(WhiteCurtainTransition(maskRaw, () =>
                    {
                        switch (num)
                        {
                            case 0:
                                HideAllAndShowOne(bottomTra, 2);
                                PlayAnimation("b", 3);
                                isPlaying = false;
                                break;

                            case 1:
                                //放大缩小介绍框
                                HideAllAndShowOne(bottomTra, 1);

                                HideAllAndShowOne(spines[1].transform, -1, true);

                                InitSpine(spines[1], "jiangjieb1", false);

                                isPlaying = false;
                                break;

                            case 2:
                                HideAllAndShowOne(bottomTra, 2);
                                PlayAnimation("a", 5);
                                isPlaying = false;
                                break;
                        }
                    }));
                }));
            }
        }

        //放大
        void Enlarge(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            backButton.SetActive(true);

            int num = obj.transform.GetSiblingIndex() + 2;
            SpineManager.instance.DoAnimation(spines[1], "jiangjieb" + num, false, () =>
            {
                SpineManager.instance.DoAnimation(spines[1], "jiangjieb" + (num + 2), false, () =>
                {
                    curNum = num;
                    isPlaying = false;
                    isClickeds[num - 2] = true;
                });
            });
        }

        //缩小
        void BackButton(GameObject obj)
        {
            Debug.Log(isPlaying);

            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(spines[1], "jiangjieb" + (curNum + 5), false, () =>
            {
                SpineManager.instance.DoAnimation(spines[1], "jiangjieb1", false);
                backButton.SetActive(false);
                isPlaying = false;

                if (isAllTrue(isClickeds))
                {
                    talkIndex = 1;
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            });
        }

        //播放画画动画
        void PlayAnimation(string animation, int totalNum, Action method = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0));

            InitSpine(spines[2], "hua" + animation + "0", false);

            //画画动画
            string[] animations = new string[totalNum];
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i] = "hua" + animation + i;
            }

            DoAnimationContinuity(spines[2], animations, false, () =>
            {
                isPlaying = false;
                back.SetActive(true);

                method?.Invoke();
            });
        }
        #endregion

        #region 通用方法
        //点击反馈
        void ClickFeedback(Transform tar, float time = 0.5f, float strength = 0.1f)
        {
            SoundManager.instance.PlayClip(9);

            strength = (1 - strength);
            Vector2 curScale = tar.localScale;

            tar.DOScale(curScale * strength, time / 3).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tar.DOScale(curScale, 2 * time / 3).SetEase(Ease.InOutSine);
            });
        }

        //同一个物体连续播放Spine
        float DoAnimationContinuity(GameObject _obj, string[] animationNames, bool isEndLoop = true, Action _method = null)
        {
            float timeLine = 0f;
            if (_obj == null) return timeLine;

            //获得动画队列
            var skeletonGraphic = _obj.GetComponent<SkeletonGraphic>();
            Spine.AnimationState spineAnimationState = skeletonGraphic.AnimationState;

            if (spineAnimationState != null)
            {
                skeletonGraphic.startingAnimation = null;
                TrackEntry track = null;

                float time = 0f;

                for (int i = 0; i < animationNames.Length; i++)
                {
                    bool isLoop = isEndLoop && i == animationNames.Length - 1;
                    track = spineAnimationState.AddAnimation(0, animationNames[i], isLoop, time);
                    time = spineAnimationState.Data.SkeletonData.FindAnimation(animationNames[i]).Duration;
                    timeLine += time;
                }

                track.Complete += (TrackEntry trackEntry) =>
                {
                    _method?.Invoke();
                };
            }

            return timeLine;
        }

        //白幕转场
        IEnumerator WhiteCurtainTransition(RawImage _raw, Action method = null)
        {
            _raw.color = new Color(1, 1, 1, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.white, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.5f);

            _raw.DOColor(new Color(1, 1, 1, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

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
