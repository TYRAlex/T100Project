using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course748Part5
    {
        #region 通用变量
        int talkIndex;

        bool isPlaying = false;
        bool[] isClickeds;

        GameObject curGo;
        GameObject bg;

        Transform curTrans;

        MonoBehaviour mono;
        #endregion

        #region 游戏变量
        Transform downTra;
        Transform maskTra;

        GameObject bell;
        GameObject amy;
        GameObject box;
        GameObject spineObj;
        GameObject back;

        string curName;
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

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            downTra = curTrans.Find("down");
            maskTra = curTrans.Find("mask");

            box = downTra.Find("box").gameObject;
            amy = downTra.Find("amy").gameObject;
            bell = downTra.Find("bell").gameObject;
            spineObj = maskTra.Find("bottom/Spine").gameObject;
            back = maskTra.Find("back").gameObject;
        }

        private void GameInit()
        {
            talkIndex = 1;
            ChangeLevel(0);

            InitSpine(box, "animation", false);

            for (int i = 0; i < spineObj.transform.childCount; i++)
            {
                GameObject obj = spineObj.transform.GetChild(i).gameObject;
                Util.AddBtnClick(obj, Click);
            }

            Util.AddBtnClick(back, BackButton);

            int n = spineObj.transform.childCount;
            isClickeds = new bool[n];
            for (int i = 0; i < n; i++)
            {
                isClickeds[i] = false;
            }
        }

        void GameStart()
        {
            downTra.gameObject.SetActive(true);

            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
            {
                string[] animtionNames1 = new string[2] { "animation", "animation2" };
                string[] animtionNames2 = new string[1] { "animation3" };

                //盒子打开
                DoAnimationContinuity(box, animtionNames1, false, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(amy, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                    {
                        //人物旋转
                        float time = DoAnimationContinuity(box, animtionNames2, true);

                        mono.StartCoroutine(WaitFor(time - 2f, () =>
                        {

                            mono.StartCoroutine(SpeckerCoroutine(amy, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                            {
                                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                }));
                            }));
                        }));
                    }));
                });
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                ChangeLevel(1);

                InitSpine(spineObj, "jing", false);

                isPlaying = false;
                back.SetActive(false);
            }
            else if (talkIndex == 2)
            {
                ChangeLevel(0);
                SpineManager.instance.DoAnimation(amy, "daiji2", true);

                mono.StartCoroutine(SpeakAndAnimation(spineObj, cur: "animation4", index: 0, method: () =>
                 {
                    //amy拿笔
                    SpineManager.instance.DoAnimation(amy, "speak3", false, () =>
                     {
                         SpineManager.instance.DoAnimation(amy, "speak2", true);
                     });

                     float time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 0);

                     mono.StartCoroutine(WaitFor(time, () =>
                     {
                         SpineManager.instance.DoAnimation(amy, "daiji2", true);

                         SpineManager.instance.DoAnimation(bell, "speak2", false, () =>
                         {
                             SpineManager.instance.DoAnimation(bell, "daiji", true);
                         });

                         SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 0);
                     }));
                 }));
            }

            talkIndex++;
        }

        #region 游戏方法
        void ChangeLevel(int num)
        {
            downTra.gameObject.SetActive(num == 0);
            maskTra.gameObject.SetActive(num == 1);
        }

        void Click(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.ShowVoiceBtn(false);

            SpineManager.instance.DoAnimation(spineObj, "d" + obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(spineObj, "yp" + obj.name, false, () =>
                {
                    back.SetActive(true);
                    curName = obj.name;
                    isPlaying = false;

                    isClickeds[obj.transform.GetSiblingIndex()] = true;
                });
            });
        }

        void BackButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            float time = InitSpine(spineObj, "t" + curName, false);

            mono.StartCoroutine(WaitFor(time, ()=>
            {
                SpineManager.instance.DoAnimation(spineObj, "jing", false, () =>
                {
                    isPlaying = false;
                    back.SetActive(false);

                    if (isAllTrue(isClickeds))
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
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

        //是否全部都是对的
        bool isAllTrue(bool[] bools)
        {
            for (int i = 0; i < bools.Length; i++)
            {
                if (!bools[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// 同一个物体连续播放Spine
        /// </summary>
        /// <param 播放动画的物体="_obj"></param>
        /// <param 连续播放队列的动画名="animationNames"></param>
        /// <param 最后一个动画是否循环="isEndLoop"></param>
        /// <param 回调="_method"></param>
        /// <returns></returns>
        float DoAnimationContinuity(GameObject _obj, string[] animationNames, bool isEndLoop = true, Action _method = null)
        {
            float timeLine = 0f;//总时间
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

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bell;
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

        //Spine初始化
        float InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);

            return _ske.AnimationState.Data.SkeletonData.FindAnimation(animation).Duration;
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
