using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course739Part1
    {
        #region 常用变量
        int talkIndex;
        bool isPlaying = false;

        MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject Bg;
        BellSprites bellTextures;

        GameObject Max;
        #endregion

        #region 游戏变量
        bool isSun = false;
        bool isSunClicked = false;
        bool isMoonClicked = false;


        float maxDistance;
        float speaktime;

        GameObject ball;
        GameObject mask;

        mILDrager[] drags;
        RawImage[] lightRaw;

        RawImage bgRaw;
        RawImage hourseRaw;

        Text text;

        Vector2 sunPos;
        Vector2 moonPos;
        Vector2 aimPos;

        SkeletonGraphic dropske;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
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
            Max.SetActive(true);

            ball = curTrans.Find("Ball").gameObject;
            ball.SetActive(true);

            Bg.SetActive(true);

            curTrans.Find("Game").gameObject.SetActive(false);

            drags = curTrans.Find("Game/drags").GetComponentsInChildren<mILDrager>(true);

            drags[0].SetDragCallback(DragBegin, Drag, DragEnd);
            drags[1].SetDragCallback(DragBegin, Drag, DragEnd);

            dropske = curTrans.Find("Game/drop").GetComponent<SkeletonGraphic>();

            text = curTrans.Find("Game/Number/Text(number)").GetText();

            bgRaw = curTrans.Find("Game/Night/Bg").GetRawImage();
            hourseRaw = curTrans.Find("Game/Night/Hourse").GetRawImage();

            lightRaw = new RawImage[3];
            for (int i = 0; i < 3; ++i)
            {
                lightRaw[i] = curTrans.Find("Game/lamp" + (i + 1) + "/light").GetRawImage();
            }

            sunPos = new Vector2(-840, -190);
            moonPos = new Vector2(900, -190);

            aimPos = Vector2.up * 120;
        }

        void GameInit()
        {
            talkIndex = 1;

            isPlaying = false;

            isSunClicked = false;
            isMoonClicked = false;

            ball.GetComponent<SkeletonGraphic>().Initialize(true);
            drags[0].GetComponent<SkeletonGraphic>().Initialize(true);
            drags[1].GetComponent<SkeletonGraphic>().Initialize(true);

            dropske.Initialize(true);
            dropske.color = Color.white;
            dropske.gameObject.SetActive(false);

            drags[0].transform.GetRectTransform().anchoredPosition = sunPos;
            drags[1].transform.GetRectTransform().anchoredPosition = aimPos;

            drags[0].transform.localScale = Vector2.one;
            drags[1].transform.localScale = Vector2.one;

            SpineManager.instance.DoAnimation(drags[0].gameObject, "animation2", true);
            SpineManager.instance.DoAnimation(drags[1].gameObject, "animation3", true);
            SpineManager.instance.DoAnimation(dropske.gameObject, "animation", true);

            drags[0].GetComponent<SkeletonGraphic>().raycastTarget = true;
            drags[1].GetComponent<SkeletonGraphic>().raycastTarget = false;
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

            mono.StartCoroutine(SpeakAndAnimation(ball, null, "a1", "a2", 0, () =>
            {
                isPlaying = false;

                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            if (talkIndex == 1)
            {

                SpineManager.instance.DoAnimation(ball, "a3", false, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1));

                    mono.StartCoroutine(WaitAnimation(ball, "a", 4, 6, () =>
                    {
                        mono.StartCoroutine(WaitFor(2f, () =>
                        {
                            mono.StartCoroutine(SpeakAndAnimation(ball, b: "a10", index: 2, method: () =>
                            {
                                mono.StartCoroutine(WaitFor(2f, () =>
                                {
                                    NextGame();
                                }));
                            }));
                        }));
                    }));
                });
            }

            if (talkIndex == 2)
            {
                isPlaying = true;

                drags[0].GetComponent<SkeletonGraphic>().raycastTarget = false;
                drags[1].GetComponent<SkeletonGraphic>().raycastTarget = false;

                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5));
            }

            talkIndex++;
        }

        void NextGame()
        {
            isSun = true;

            curTrans.Find("Game").gameObject.SetActive(true);

            ball.SetActive(false);
            Max.SetActive(false);
            Bg.SetActive(false);

            text.text = "0";
            ChangeAlpha(0);

            drags[0].gameObject.SetActive(true);
            drags[1].gameObject.SetActive(true);

            maxDistance = Vector2.Distance(drags[0].transform.position, dropske.transform.position);
        }

        #region 拖拽
        void DragBegin(Vector3 pos, int type, int index)
        {
            if (isPlaying || !drags[index].GetComponent<SkeletonGraphic>().raycastTarget) return;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.ShowVoiceBtn(false);

            drags[index].transform.position = Input.mousePosition;

            float _time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3 + index);
            mono.StartCoroutine(TimeLapse(_time));

            dropske.gameObject.SetActive(true);
        }

        void Drag(Vector3 pos, int type, int index)
        {
            if (isPlaying || !drags[index].GetComponent<SkeletonGraphic>().raycastTarget) return;

            float percentage;

            percentage = Mathf.Clamp01(Vector2.Distance(drags[index].transform.position, dropske.transform.position) / maxDistance);

            if (percentage < 0.03f) percentage = 0f;

            Vector2 deltaPos = isSun ? moonPos - aimPos : sunPos - aimPos;

            drags[1 - index].transform.GetRectTransform().anchoredPosition =
                aimPos + new Vector2(deltaPos.x * (1 - percentage), deltaPos.y * (float)Math.Pow(Mathf.Sin((1 - percentage) * (float)Math.PI / 2), 6));

            if (isSun) percentage = 1 - percentage;

            ChangeAlpha(percentage);

            text.text = Math.Round(100 * percentage) + "";
        }

        void DragEnd(Vector3 pos, int type, int index, bool isMatch)
        {
            if (isPlaying || !drags[index].GetComponent<SkeletonGraphic>().raycastTarget) return;

            dropske.gameObject.SetActive(false);

            isPlaying = true;
            drags[0].GetComponent<SkeletonGraphic>().raycastTarget = false;
            drags[1].GetComponent<SkeletonGraphic>().raycastTarget = false;

            mono.StartCoroutine(ChangeTextAndAlpha(1 - index));

            EnlargeShrink(drags[index].transform, ()=>
            {
                drags[index].transform.GetRectTransform().anchoredPosition = aimPos;
            });

            EnlargeShrink(drags[1 - index].transform, ()=>
            {
                drags[1 - index].transform.GetRectTransform().anchoredPosition = isSun ? moonPos : sunPos;

                maxDistance = Vector2.Distance(isSun ? moonPos : sunPos, aimPos);

                if (isSun) isSun = false;
                else isSun = true;
            });
        }
        #endregion

        #region 通用方法
        void EnlargeShrink(Transform tra, Action method = null)
        {
            tra.DOScale(Vector2.one * 1.1f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                tra.DOScale(Vector2.zero, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                    method?.Invoke();

                    tra.transform.DOScale(Vector2.one * 1.1f, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        tra.transform.DOScale(Vector2.one, 0.1f).SetEase(Ease.InOutSine);
                    });
                });
            });
        }

        void ChangeAlpha(float percentage)
        {
            Color color = new Color(255, 255, 255, 1 - percentage);

            bgRaw.color = color;
            hourseRaw.color = color;

            for (int i = 0; i < lightRaw.Length; i++)
            {
                lightRaw[i].color = color;
            }
        }

        //等待说话结束
        IEnumerator TimeLapse(float _time)
        {
            speaktime = _time;
            WaitForEndOfFrame wait = new WaitForEndOfFrame();

            while (!isPlaying && speaktime >= 0)
            {
                speaktime -= Time.deltaTime;
                yield return wait;
            }
        }

        IEnumerator ChangeTextAndAlpha(float aimValue)
        {
            float curValue = 1 - bgRaw.color.a;
            float value = curValue;
            float deltaValue = aimValue - curValue;
            float totaltime = speaktime < 1 ? 1 : speaktime;
            float i = 0;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while(i <= totaltime)
            {
                i += Time.fixedDeltaTime;
                value = curValue + deltaValue * i / totaltime;
                
                ChangeAlpha(value);

                text.text = Math.Round(100 * value) + "";

                yield return wait;
            }

            text.text = "" + aimValue * 100;
            ChangeAlpha(aimValue);

            isPlaying = false;

            drags[(int)aimValue].GetComponent<SkeletonGraphic>().raycastTarget = true;

            if (isSun) isSunClicked = true;
            else isMoonClicked = true;

            if (isSunClicked && isMoonClicked) SoundManager.instance.ShowVoiceBtn(true);
        }

        //播放动画和语音
        IEnumerator SpeakAndAnimation(GameObject obj, string a = null, string b = null, string c = null, int index = -1, Action method = null)
        {
            float _time = 0f;
            if (a != null) _time = SpineManager.instance.DoAnimation(obj, a, false);
            yield return new WaitForSeconds(_time);

            float speakTime = 0f;
            if (index != -1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index));
                speakTime = SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, index);
            }

            float animationTime = 0f;
            if (b != null) animationTime = SpineManager.instance.DoAnimation(obj, b, false);

            _time = speakTime > animationTime ? speakTime : animationTime;
            yield return new WaitForSeconds(_time);

            _time = 0f;
            if (c != null) _time = SpineManager.instance.DoAnimation(obj, c, false);
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

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //协程:间隔播放动画
        IEnumerator WaitAnimation(GameObject obj, string init, int num, int total, Action method = null)
        {
            float _time;

            for (int i = 0; i < total; ++i)
            {
                _time = SpineManager.instance.DoAnimation(obj, init + num, false);

                ++num;

                yield return new WaitForSeconds(_time + 2f);
            }

            method?.Invoke();
        }
        #endregion
    }
}

