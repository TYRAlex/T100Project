using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD6752Part1
    {
        #region 常用变量
        int talkIndex;

        GameObject curCanvas;
        GameObject dd;
        GameObject mask;
        GameObject unDragableMask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;
        GameObject nextButton;

        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        //bool isPlaying = false;
        #endregion

        #region 游戏变量
        int imageIndex;

        GameObject milkyWay;
        GameObject moon;
        GameObject demon;
        //GameObject demonHand;
        GameObject dead;
        GameObject[] star;
        GameObject star2;
        GameObject hourse;

        BellSprites dragImage1;
        BellSprites dragImage2;

        RectTransform demonJPGTra;

        Image progress;

        mILDrager[] drag;
        List<mILDroper> drop;
        #endregion

        void Start(object o)
        {
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;

            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            LoadMask();

            LoadGame();//加载

            GameInit();

            MaskStart();
        }

        void LoadMask()
        {
            unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;
            unDragableMask.SetActive(false);

            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;
            mask.SetActive(true);

            dd = maskTra.Find("DD").gameObject;
            dd.transform.GetRectTransform().anchoredPosition = new Vector2(270, -206);
            dd.transform.localScale = new Vector2(1.2f, 1.2f);
            dd.SetActive(false);

            nextButton = maskTra.Find("NextButton").gameObject;
            nextButton.SetActive(false);

            successSpine = maskTra.Find("successSpine").gameObject;
            successSpine.SetActive(false);

            caidaiSpine = maskTra.Find("caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            btn01 = maskTra.Find("Btns/0").gameObject;
            btn02 = maskTra.Find("Btns/1").gameObject;
            btn03 = maskTra.Find("Btns/2").gameObject;

            btn01.GetComponent<SkeletonGraphic>().Initialize(true);
            btn02.GetComponent<SkeletonGraphic>().Initialize(true);
            btn03.GetComponent<SkeletonGraphic>().Initialize(true);
            nextButton.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(nextButton, "next2", false);
            SpineManager.instance.DoAnimation(btn01, "next2", false);

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);

            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn02, Win);
            Util.AddBtnClick(btn03, GamePlay);
            Util.AddBtnClick(nextButton, ShowDemon);
        }

        void LoadGame()
        {
            milkyWay = curCanvasTra.Find("BackGround/MilkyWay").gameObject;

            moon = curCanvasTra.Find("BackGround/Moon").gameObject;
            moon.GetComponent<SkeletonGraphic>().raycastTarget = true;

            demon = curCanvasTra.Find("Demon").gameObject;
            demon.SetActive(false);

            //demonHand = demon.transform.Find("Hand").gameObject;
            //demonHand.Hide();

            dead = demon.transform.Find("Dead").gameObject;
            dead.SetActive(false);

            curCanvasTra.Find("ProgressBar").gameObject.SetActive(false);

            progress = curCanvasTra.Find("ProgressBar/Progress").GetImage();
            progress.fillAmount = 0f;

            demonJPGTra = curCanvasTra.Find("ProgressBar/DemonJPG").GetRectTransform();
            demonJPGTra.anchoredPosition = Vector2.zero;

            hourse = curCanvasTra.Find("Hourse").gameObject;
            hourse.SetActive(false);

            int n = curCanvasTra.Find("BackGround/Star").childCount;

            star = new GameObject[n];
            for (int i = 0; i < n; ++i)
            {
                star[i] = curCanvasTra.Find("BackGround/Star").GetChild(i).gameObject;
            }

            Transform tra = curCanvasTra.Find("GameObject");
            drop = new List<mILDroper>();
            for (int i = 0, s = tra.childCount - 1; i < s; ++i)
            {
                drop.Add(tra.GetChild(i).GetChild(0).GetComponent<mILDroper>());
            }

            for (int i = 0, s = drop.Count; i < s; ++i)
            {
                drop[i].index = i;
                drop[i].transform.parent.gameObject.Hide();
            }

            star2 = tra.Find("Star").gameObject;

            drag = new mILDrager[2];
            drag[0] = demon.transform.Find("Photo1").GetComponent<mILDrager>();
            drag[1] = demon.transform.Find("Photo2").GetComponent<mILDrager>();

            drag[0].transform.GetRectTransform().anchoredPosition = new Vector2(-175, 50);
            drag[1].transform.GetRectTransform().anchoredPosition = new Vector2(175, 50);

            drag[0].SetDragCallback(DragBegin, null, DargEnd);
            drag[1].SetDragCallback(DragBegin, null, DargEnd);

            drag[0].gameObject.Show();
            drag[1].gameObject.Show();

            dragImage1 = drag[0].GetComponent<BellSprites>();
            dragImage2 = drag[1].GetComponent<BellSprites>();

            Util.AddBtnClick(moon, MoonClick);
        }

        void GameInit()
        {
            talkIndex = 1;

            milkyWay.GetComponent<SkeletonGraphic>().Initialize(true);
            moon.GetComponent<SkeletonGraphic>().Initialize(true);
            dead.GetComponent<SkeletonGraphic>().Initialize(true);
            demon.GetComponent<SkeletonGraphic>().Initialize(true);
            dd.GetComponent<SkeletonGraphic>().Initialize(true);
            star2.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(moon, "moon", false);
            SpineManager.instance.DoAnimation(milkyWay, "xh", false);

            dead.transform.GetRectTransform().anchoredPosition = new Vector2(50, 175);
            demon.transform.GetRectTransform().anchoredPosition = Vector2.zero;

            for (int i = 0, n = star.Length; i < n; ++i)
            {
                star[i].GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(star[i], "xing", false);
            }

            drop[0].transform.parent.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(drop[0].transform.parent.gameObject, "shu", false);

            for (int i = 1, n = drop.Count; i < n; ++i)
            {
                drop[i].transform.parent.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(drop[i].transform.parent.gameObject, "xuanxiang" + i, false);
            }
        }

        void MaskStart()
        {
            
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        void GamePlay(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                btn03.SetActive(false);
                dd.SetActive(true);

                switch (talkIndex)
                {
                    case 1:
                        mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 1, null, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 3, null, () =>
                            {
                                mask.SetActive(false);
                                dd.SetActive(false);
                                unDragableMask.Hide();
                            }));
                        }));
                        break;
                }

                ++talkIndex;
            });
        }

        void MoonClick(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

            SpineManager.instance.DoAnimation(moon, "moon2", false, () =>
            {
                SpineManager.instance.DoAnimation(moon, "moon3", true);
            });

            mono.StartCoroutine(WaitCor(2f, () =>
            {
                SpineManager.instance.DoAnimation(milkyWay, "xh2", false, () =>
                {
                    mono.StartCoroutine(StarAnimation(4, 0.4f, "xing2", false));

                    mono.StartCoroutine(WaitCor(4f, () =>
                    {
                        mask.SetActive(true);
                        nextButton.SetActive(true);

                        unDragableMask.SetActive(false);
                    }));

                    mono.StartCoroutine(WaitCor(20 / 3f, () =>
                    {
                        mono.StartCoroutine(StarAnimation(4, 0.4f, "xing3", true));
                    }));
                });
            }));

            moon.GetComponent<SkeletonGraphic>().raycastTarget = false;
        }

        #region 拖拽
        void DragBegin(Vector3 position, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

            drag[type].transform.position = Input.mousePosition;
            drag[type].transform.SetAsLastSibling();
        }

        void DargEnd(Vector3 position, int type, int index, bool isMatch)
        {
            unDragableMask.Show();
            drag[type].gameObject.Hide();

            if (type == 0 || !isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                SpineManager.instance.DoAnimation(demon, "xem-jx2", false, () =>
                {
                    SpineManager.instance.DoAnimation(demon, "xem-jx2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(demon, "xem2", true);

                        drag[type].DoReset();
                        drag[type].gameObject.Show();

                        unDragableMask.Hide();

                        RandomImage();
                    });
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);

                if (index == 0)
                {
                    mono.StartCoroutine(ProgressController(progress, 1 / 6f, 1f));
                    mono.StartCoroutine(ProgressController(demonJPGTra, -450 / 6f, 1f));     

                    SpineManager.instance.DoAnimation(drop[imageIndex].transform.parent.gameObject, "shu2", false, () =>
                    {
                        drag[type].DoReset();
                        drag[type].gameObject.Show();

                        unDragableMask.Hide();

                        SpineManager.instance.DoAnimation(drop[imageIndex].transform.parent.gameObject, "shu3", true);
                        drop.RemoveAt(imageIndex);

                        RandomImage();
                    });
                }
                else
                {
                    star2.transform.position = drop[imageIndex].transform.position;
                    SpineManager.instance.DoAnimation(star2, "star6", false);

                    mono.StartCoroutine(ProgressController(progress, 1 / 6f, 1f));
                    mono.StartCoroutine(ProgressController(demonJPGTra, -450 / 6f, 1f));

                    SpineManager.instance.DoAnimation(drop[imageIndex].transform.parent.gameObject, "xuanxiang" + (index + 5), false, () =>
                    {
                        drag[type].DoReset();
                        drag[type].gameObject.Show();

                        unDragableMask.Hide();

                        drop.RemoveAt(imageIndex);

                        RandomImage();
                    });
                }
            }
        }

        #endregion

        //恶魔出现
        void ShowDemon(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);

            SpineManager.instance.DoAnimation(nextButton, "next", false, () =>
            {
                dd.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 2, null, ()=>
                {
                    curCanvasTra.Find("ProgressBar").gameObject.SetActive(true);

                    mask.SetActive(false);
                    nextButton.SetActive(false);
                    dd.SetActive(false);

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);

                    for (int i = 0, s = drop.Count; i < s; ++i)
                    {
                        drop[i].transform.parent.gameObject.Show();
                    }

                    RandomImage();

                    demon.SetActive(true);
                    SpineManager.instance.DoAnimation(demon, "xem2", true);

                    demon.transform.GetRectTransform().DOAnchorPosY(-540, 2f);
                    mono.StartCoroutine(WaitCor(2f, ()=> unDragableMask.Hide()));
                }));
            });
        }

        //随机化小恶魔手中的正确和错误图片位置
        void RandomImagePos()
        {
            int i = Random.Range(0, 2);
            drag[i].transform.GetRectTransform().anchoredPosition = new Vector2(-175, 50);
            drag[1 - i].transform.GetRectTransform().anchoredPosition = new Vector2(175, 50);
        }

        //随机化小恶魔手中的图片
        void RandomImage()
        {
            if (drop.Count == 0)
            {
                hourse.SetActive(true);

                drag[0].gameObject.Hide();
                drag[1].gameObject.Hide();

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                SpineManager.instance.DoAnimation(demon, "xem-y3", true);

                mono.StartCoroutine(WaitCor(0.75f, () =>
                {
                    mono.StartCoroutine(SetMoveAncPosY(demon.transform, -300, 0.5f, () =>
                    {
                        mono.StartCoroutine(SetMoveAncPosY(demon.transform, 50, 0.2f, () =>
                        {
                            mono.StartCoroutine(SetMoveAncPosY(demon.transform, -50, 0.2f, () =>
                            {
                                dead.SetActive(true);
                                SpineManager.instance.DoAnimation(dead, "hun", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(dead, "hun2", true);
                                });

                                mono.StartCoroutine(SetMoveAncPosY(dead.transform, 50, 2f, () =>
                                {
                                    dead.SetActive(false);
                                    GameEnd();
                                }));
                            }));
                        }));
                    }));
                }));

                return;
            }

            RandomImagePos();

            imageIndex = Random.Range(0, drop.Count - 1);
            int s = drop[imageIndex].index;

            drag[0].transform.GetRawImage().texture = dragImage1.texture[s];
            drag[1].transform.GetRawImage().texture = dragImage2.texture[s];
            drag[1].index = s;
            drag[1].drops[0] = drop[imageIndex];
        }
        
        void GameEnd()
        {
            mono.StartCoroutine(WaitCor(2f, () =>
            {
                mask.SetActive(true);
                btn03.SetActive(false);
                dd.SetActive(false);
                successSpine.SetActive(true);
                caidaiSpine.SetActive(true);

                SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);

                SpineManager.instance.DoAnimation(successSpine, "6-12-z", false, () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "6-12-z2", false, () =>
                    {
                        successSpine.SetActive(false);
                        caidaiSpine.SetActive(false);

                        SpineManager.instance.DoAnimation(btn01, "fh2", false);
                        SpineManager.instance.DoAnimation(btn02, "ok2", false);

                        btn01.SetActive(true);
                        btn02.SetActive(true);

                        unDragableMask.SetActive(false);
                    });
                });
            }));
        }

        //重玩
        void Replay(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                LoadMask();

                LoadGame();//加载

                GameInit();

                mask.SetActive(false);
                dd.SetActive(false);
                unDragableMask.Hide();
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            unDragableMask.SetActive(true);

            dd.transform.GetRectTransform().anchoredPosition = new Vector2(980, -239);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 0));
                dd.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);
            });
        }

        IEnumerator SetMoveAncPosY(Transform temp, int distance, float duration = 1f, Action callBack = null)
        {
            float i = 0;
            float time = duration;
            float curPosx = temp.GetRectTransform().anchoredPosition.x;
            float curPosy = temp.GetRectTransform().anchoredPosition.y;


            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                temp.GetRectTransform().anchoredPosition = new Vector2(curPosx, curPosy + distance * i / time);
                yield return wait;
                i += Time.fixedDeltaTime;
            }

            callBack?.Invoke();
        }

        IEnumerator ProgressController(Image image, float offsetAomount, float duringTime = 1f)
        {
            float temp = 0;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            float totalTime =  duringTime;
            float curAomount = image.fillAmount;

            while (temp < totalTime)
            {
                image.fillAmount = curAomount + offsetAomount * temp / totalTime;
                temp += Time.fixedDeltaTime;
                yield return wait;
            }
        }

        IEnumerator ProgressController(RectTransform rectTransform, float offsetAomount, float duringTime = 1f)
        {
            float temp = 0;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            float totalTime = duringTime;
            float curAomountx = rectTransform.anchoredPosition.x;
            float curAomounty = rectTransform.anchoredPosition.y;

            while (temp < totalTime)
            {
                rectTransform.anchoredPosition = new Vector2(curAomountx, curAomounty + offsetAomount * temp / totalTime);
                temp += Time.fixedDeltaTime;
                yield return wait;
            }
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {

            if (!speaker) speaker = dd;

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");


            method_2?.Invoke();
        }

        IEnumerator WaitCor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        IEnumerator StarAnimation(int n, float _time, string animation, bool state, Action method = null)
        {
            for (int i = 0; i < n; ++i)
            {
                yield return new WaitForSeconds(_time);
                SpineManager.instance.DoAnimation(star[i], animation, state);
            }

            method?.Invoke();
        }
    }
}
