using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course941Part1
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

        GameObject spineObj;
        GameObject back;
        GameObject mask;
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
            spineObj = curTrans.Find("bottom/spine").gameObject;
            back = curTrans.Find("back").gameObject;
            mask = curTrans.Find("mask").gameObject;
        }

        void GameInit()
        {
            talkIndex = 1;

            InitSpine(spineObj, "a0", false);

            //打开点击
            for (int i = 0; i < spineObj.transform.childCount; i++)
            {
                GameObject obj = spineObj.transform.GetChild(i).gameObject;
                obj.SetActive(true);
                Util.AddBtnClick(obj, Click);
            }

            Util.AddBtnClick(back, BackButton);

            back.SetActive(false);
            mask.SetActive(false);

            isClickeds = new bool[spineObj.transform.childCount];
            for (int i = 0; i < isClickeds.Length; i++)
            {
                isClickeds[i] = false;
            }

            Util.AddBtnClick(mask.transform.GetChild(0).gameObject, Replay);
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
                isPlaying = true;

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, ()=>
                {
                    isPlaying = false;
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if(talkIndex == 2)
            {
                isPlaying = true;
                InitSpine(spineObj, "huaa0", false);

                //关闭点击
                for (int i = 0; i < spineObj.transform.childCount; i++)
                {
                    GameObject obj = spineObj.transform.GetChild(i).gameObject;
                    obj.SetActive(false);
                }

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if(talkIndex == 3)
            {
                PlayAnimation("b", 3, ()=>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                });
                
            }
            else if(talkIndex == 4)
            {
                PlayAnimation("a", 5);
            }

            talkIndex++;
        }

        #region 游戏方法
        //播放画画动画
        void PlayAnimation(string animation, int totalNum, Action method = null)
        {
            mask.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0));

            InitSpine(spineObj, "hua" + animation + "0", false);

            //画画动画
            string[] animations = new string[totalNum];
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i] = "hua" + animation + i;
            }

            DoAnimationContinuity(spineObj, animations, false, () =>
            {
                isPlaying = false;
                mask.SetActive(true);

                method?.Invoke();
            });
        }

        void Click(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);

            int num = obj.transform.GetSiblingIndex() + 1;
            SpineManager.instance.DoAnimation(spineObj, "a" + num, false, ()=>
            {
                SpineManager.instance.DoAnimation(spineObj, "a" + (num + 3), false, () =>
                {
                    curNum = num;
                    back.SetActive(true);
                    isPlaying = false;

                    isClickeds[num - 1] = true;
                });
            });
        }

        //缩小
        void BackButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);
            

            SpineManager.instance.DoAnimation(spineObj, "a" + (curNum + 6), false, () =>
            {
                SpineManager.instance.DoAnimation(spineObj, "a0", false);

                back.SetActive(false);
                isPlaying = false;

                if (isAllTrue(isClickeds))
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            });
        }

        //重玩
        void Replay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            --talkIndex;
            TalkClick();
        }
        #endregion

        #region 通用方法
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

        //是否全部都是对的
        bool isAllTrue(bool[] bools)
        {
            for (int i = 0; i < bools.Length; i++)
            {
                if (!bools[i]) return false;
            }

            return true;
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
