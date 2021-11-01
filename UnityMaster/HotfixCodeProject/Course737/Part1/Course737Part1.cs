using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course737Part1
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
        int flag = 0;
        bool[] isDraged;
        bool isRotated = false;

        Transform buttonTra;
        Transform animationTra;
        Transform carTra;

        GameObject hand;
        GameObject car;
        GameObject unDragableMask;
        mILDrager[] drags;

        Vector2 startPos;
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

            if(!buttonTra) LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            buttonTra = Bg.transform.Find("frame/Button");
            animationTra = Bg.transform.Find("carAnimation");
            carTra = Bg.transform.Find("car");

            hand = Bg.transform.Find("hand").gameObject;
            drags = Bg.transform.Find("drags").GetComponentsInChildren<mILDrager>(true);

            car = Bg.transform.Find("carDrag").gameObject;
            unDragableMask = curTrans.Find("unDragableMask").gameObject;
        }

         void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            isRotated = false;

            unDragableMask.SetActive(false);
            animationTra.gameObject.SetActive(true);
            car.SetActive(false);

            isDraged = new bool[5];
            for (int i = 0; i < isDraged.Length; ++i)
            {
                isDraged[i] = false;
            }

            buttonTra.GetRectTransform().anchoredPosition = Vector2.left * 520;

            hand.GetComponent<SkeletonGraphic>().Initialize(true);
            car.GetComponent<SkeletonGraphic>().Initialize(true);

            for (int i = 0; i < buttonTra.childCount; ++i)
            {
                SkeletonGraphic buttonSke = buttonTra.GetChild(i).GetComponent<SkeletonGraphic>();
                buttonSke.Initialize(true);
                buttonSke.raycastTarget = true;
                SpineManager.instance.DoAnimation(buttonSke.gameObject, "an-a" + int.Parse(buttonSke.gameObject.name));

                animationTra.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);

                carTra.GetChild(i).gameObject.SetActive(true);
                carTra.GetChild(i).GetRawImage().color = Color.white;
                carTra.GetChild(i).GetRectTransform().anchoredPosition =
                    new Vector2(550, (int.Parse(carTra.GetChild(i).gameObject.name) - 2) * -200);

                Util.AddBtnClick(buttonSke.gameObject, ClickButton);
            }

            drags[0].transform.parent.gameObject.SetActive(false);
            drags[0].SetDragCallback(DragStart, null, DragEnd);
            drags[1].SetDragCallback(DragStart, null, DragEnd);
            drags[2].SetDragCallback(DragStart, null, DragEnd);
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            { 
                Max.SetActive(false); 
                isPlaying = false; 
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);

            unDragableMask.SetActive(true);

            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, ()=>
                {
                    drags[0].transform.parent.gameObject.SetActive(true);
                   
                }));

                buttonTra.GetRectTransform().DOAnchorPosX(-490, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    buttonTra.GetRectTransform().DOAnchorPosX(-900, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        hand.transform.GetRectTransform().anchoredPosition = new Vector2(-165, 100);
                        hand.GetComponent<SkeletonGraphic>().Initialize(true);

                        mono.StartCoroutine(WaitAnimation(hand, "zhi", 3, 2, () =>
                        {
                            hand.transform.GetRectTransform().anchoredPosition = new Vector2(175, -125);
                            hand.GetComponent<SkeletonGraphic>().Initialize(true);

                            mono.StartCoroutine(WaitAnimation(hand, "zhi", 1, 2, ()=>
                            {
                                hand.transform.GetRectTransform().anchoredPosition = new Vector2(165, -230);
                                hand.GetComponent<SkeletonGraphic>().Initialize(true);

                                SpineManager.instance.DoAnimation(hand, "zhi2", false, () =>
                                {
                                    unDragableMask.SetActive(false);
                                });
                            }));
                        }));
                    });
                });
            }
            if (talkIndex == 2)
            {
                
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5));
            }

            talkIndex++;
        }

        #region 拖拽
        void DragStart(Vector3 position, int type, int index)
        {
            startPos = Input.mousePosition;
        }

        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            Vector2 endPos = Input.mousePosition;
            if (startPos == endPos) return;

            unDragableMask.SetActive(true);

            if (type == 0)
            {
                if (endPos.y - startPos.y > 100)
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                    SpineManager.instance.DoAnimation(car, "w1", false, () =>
                    {
                        EndAnimation(0);
                    });
                }
                else if (endPos.y - startPos.y < -100)
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                    SpineManager.instance.DoAnimation(car, "w2", false, () =>
                    {
                        EndAnimation(1);
                    });
                }
                else unDragableMask.SetActive(false);
            }
            else if (type == 1)
            {
                if (endPos.x - startPos.x > 100 && !isRotated)
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                    SpineManager.instance.DoAnimation(car, "w-z3", false, () =>
                    {
                        isRotated = true;
                        EndAnimation(2);
                    });
                }
                else if (endPos.x - startPos.x < -100 && isRotated)
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                    SpineManager.instance.DoAnimation(car, "w-z4", false, () =>
                    {
                        isRotated = false;
                        EndAnimation(3);
                    });
                }
                else unDragableMask.SetActive(false);
            }
            else if (type == 2)
            {
                if (endPos.x - startPos.x < -100)
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                    SpineManager.instance.DoAnimation(car, "w-run", false, () =>
                    {
                        EndAnimation(4);
                        unDragableMask.SetActive(false);
                    });
                }
                else unDragableMask.SetActive(false);
            }
        }
        #endregion

        void EndAnimation(int index)
        {
            isDraged[index] = true;
            unDragableMask.SetActive(false);
            bool istrue = true;

            for (int i = 0; i < isDraged.Length; ++i)
            {
                if (!isDraged[i]) istrue = false;
            }

            if (istrue) SoundManager.instance.ShowVoiceBtn(true);
        }

        //点击按钮
        void ClickButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

            obj.transform.GetComponent<SkeletonGraphic>().raycastTarget = false;

            SpineManager.instance.DoAnimation(obj, "an-a" + obj.name, false);

            int num = obj.transform.GetSiblingIndex();
            GameObject o = animationTra.GetChild(num).gameObject;

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, (num + 1), null, () =>
            {
                isPlaying = false;

                if (++flag == 3)
                {
                    car.SetActive(true);
                    SpineManager.instance.DoAnimation(car, "w0", false);

                    mono.StartCoroutine(WaitFor(0.5f, ()=>
                    {
                        animationTra.gameObject.SetActive(false);
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                }
            }));

            ColorDisPlay(carTra.GetChild(num).GetRawImage(), false, null, 1f);

            Vector2 aimPos = num == 1 ? new Vector2(200, -200) : Vector2.zero;
            carTra.GetChild(num).GetRectTransform().DOAnchorPos(aimPos, 1f);

            mono.StartCoroutine(WaitFor(0.75f, () =>
            {
                SpineManager.instance.DoAnimation(o, "w-c" + o.name, false);
            }));
        }

        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f, Ease ease = Ease.InOutSine)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(ease).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(ease).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        //协程:间隔播放动画
        IEnumerator WaitAnimation(GameObject obj, string init, int num, int total, Action method = null)
        {
            float _time;

            for (int i = 0; i < total; ++i)
            {
                _time = SpineManager.instance.DoAnimation(obj, init + num, false);

                ++num;

                yield return new WaitForSeconds(_time);
            }

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
    }
}
