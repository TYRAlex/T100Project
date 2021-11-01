using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course748Part3
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
        RawImage raw;
        BellSprites sprites;
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
            Transform tra = curTrans.Find("images");
            raw = tra.GetRawImage();
            sprites = tra.GetComponent<BellSprites>();
        }

        private void GameInit()
        {
            talkIndex = 1;
            raw.texture = sprites.texture[0];
        }

        void GameStart()
        {
            bell.SetActive(true);
            isPlaying = true;

            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () => 
            {
                SoundManager.instance.ShowVoiceBtn(true);
                isPlaying = false; 
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    ChangeImage(1);

                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                }));
            }
            else if(talkIndex == 2)
            {
                ChangeImage(2);

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if(talkIndex == 3)
            {
                ChangeImage(3);

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.COMMONVOICE, 0));
            }

            talkIndex++;
        }

        void ChangeImage(int num)
        {
            raw.texture = sprites.texture[num];
        }
        #region 游戏方法

        #endregion

        #region 通用方法

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

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }
        #endregion
    }
}
