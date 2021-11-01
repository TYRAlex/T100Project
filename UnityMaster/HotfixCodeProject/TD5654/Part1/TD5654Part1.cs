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
    public class TD5654Part1
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

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        int flag = 0;

        Transform bg1Tra;
        Transform bg2Tra;

        GameObject magnifier;
        GameObject bigMagnifier;
        GameObject demon;
        GameObject star;

        SkeletonGraphic[] targets;
        SkeletonGraphic[] spines1;
        SkeletonGraphic[] spines2;

        mILDrager[] drags1;
        mILDrager[] drags2;

        mILDroper[] drops;
        Material[] materials;
        #endregion

        void Start(object o)
        {
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
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
            Util.AddBtnClick(nextButton, NextGame);
        }

        void LoadGame()
        {
            bg1Tra = curCanvasTra.Find("Background");
            bg2Tra = curCanvasTra.Find("Background2");

            targets = bg1Tra.Find("Targets").GetComponentsInChildren<SkeletonGraphic>(true);
            spines1 = bg1Tra.Find("Frame").GetComponentsInChildren<SkeletonGraphic>(true);
            spines2 = bg2Tra.Find("Feather").GetComponentsInChildren<SkeletonGraphic>(true);

            drags1 = bg1Tra.Find("drags").GetComponentsInChildren<mILDrager>(true);
            drags2 = bg2Tra.Find("Frame").GetComponentsInChildren<mILDrager>(true);
            drops = bg2Tra.Find("Feather").GetComponentsInChildren<mILDroper>(true);

            

            bigMagnifier = bg1Tra.Find("bigMagnifier").gameObject;
            magnifier = bg1Tra.Find("magnifierFrame/magnifier").gameObject;
            demon = bg1Tra.Find("Demon").gameObject;
            star = bg2Tra.Find("star").gameObject;
        }

        void GameInit()
        {
            talkIndex = 1;
            flag = 0;

            bg1Tra.gameObject.SetActive(true);
            bg2Tra.gameObject.SetActive(false);
            unDragableMask.SetActive(false);
            magnifier.SetActive(true);
            
            bg2Tra.Find("LastFeather").gameObject.SetActive(false);
            bg2Tra.Find("light").gameObject.SetActive(false);
            bg2Tra.Find("Frame").gameObject.SetActive(true);
            bg2Tra.Find("Feather").gameObject.SetActive(true);

            foreach (var target in targets)
            {
                target.Initialize(true);
                SpineManager.instance.DoAnimation(target.gameObject, target.gameObject.name, false);
            }

            foreach (var spine in spines1)
            {
                spine.Initialize(true);
                SpineManager.instance.DoAnimation(spine.gameObject, "x" + spine.gameObject.name, false);
            }

            Vector2[] vector2s = new Vector2[4];
            vector2s[0] = new Vector2(167.9f, 385.99f);
            vector2s[1] = new Vector2(-629.2f, 392.5f);
            vector2s[2] = new Vector2(-188f, 164f);
            vector2s[3] = new Vector2(798f, 386f);

            int j = -1;

            foreach (var drag in drags1)
            {
                drag.gameObject.SetActive(true);

                Transform tra = drag.transform;
                tra.GetRectTransform().anchoredPosition = vector2s[++j];
                tra.GetChild(0).GetRawImage().color = new Color(255, 255, 255, 0);
                drag.isActived = false;
                drag.SetDragCallback(DragBegin1, null, DragEnd1);
            }

            drags1[1].transform.GetRectTransform().sizeDelta = new Vector2(395, 85);

            drags2 = Sort(drags2);

            for(int i = 0; i < drags2.Length; ++i)
            {
                drags2[i].transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(drags2[i].transform.GetChild(0).gameObject, "x" + drags2[i].transform.GetChild(0).gameObject.name, true);
                drags2[i].gameObject.SetActive(true);
                drags2[i].isActived = false;
                drags2[i].SetDragCallback(DragBegin2, null, DragEnd2);

                drags2[i].transform.GetRectTransform().anchoredPosition = Vector2.zero;
            }

            drops = Sort(drops);
            materials = new Material[drops.Length];
            for (int i = 0; i < drops.Length; i++)
            {
                materials[i] = drops[i].transform.parent.GetChild(1).GetComponent<SkeletonGraphic>().material;
            }

            InitSpine(bigMagnifier.transform, "");
            InitSpine(magnifier.transform, "fd2");
            InitSpine(demon.transform, "xem1");
            InitSpine(star.transform, "");

            bg1Tra.Find("Bg").GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            bg1Tra.Find("gizmos").GetChild(2).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            bg2Tra.Find("LastFeather").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            bg2Tra.Find("light").gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            bg2Tra.Find("Frame").GetRectTransform().anchoredPosition = Vector2.zero;

            dd.transform.GetRectTransform().anchoredPosition = new Vector2(248, -108);
            dd.transform.localScale = Vector2.one * 0.5f;

            Util.AddBtnClick(magnifier, MagnifierClick);
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            dd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                dd.SetActive(false);
                btn03.SetActive(true);
                SpineManager.instance.DoAnimation(btn03, "bf2", false);
            }));
        }

        void GameEnd()
        {
            isPlaying = true;

            mono.StartCoroutine(WaitFor(3f, () =>
            {
                mask.SetActive(true);
                btn03.SetActive(false);
                dd.SetActive(false);
                successSpine.SetActive(true);
                caidaiSpine.SetActive(true);

                SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);

                SpineManager.instance.DoAnimation(successSpine, "3-5-z", false, () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "3-5-z2", false, () =>
                    {
                        successSpine.SetActive(false);
                        caidaiSpine.SetActive(false);

                        SpineManager.instance.DoAnimation(btn01, "fh2", false);
                        SpineManager.instance.DoAnimation(btn02, "ok2", false);

                        btn01.SetActive(true);
                        btn02.SetActive(true);

                        isPlaying = false;
                    });
                });
            }));
        }

        //放大镜功能
        void MagnifierClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(magnifier, "fd", false, () =>
            {
                magnifier.SetActive(false);
                SpineManager.instance.DoAnimation(magnifier, "fd2", false);

                List<int> list = new List<int>();

                for (int i = 0; i < drags1.Length; ++i)
                {
                    if (drags1[i].gameObject.activeSelf) list.Add(i);
                }

                int num = list[Random.Range(0, list.Count)];

                SpineManager.instance.DoAnimation(targets[num].gameObject, targets[num].gameObject.name + "2", false);

                bigMagnifier.transform.position = targets[num].transform.GetChild(0).position;
                bigMagnifier.SetActive(true);

                SpineManager.instance.DoAnimation(bigMagnifier, "fd3", false, ()=>
                {
                    SpineManager.instance.DoAnimation(targets[num].gameObject, targets[num].gameObject.name, false);

                    bigMagnifier.SetActive(false);
                    magnifier.SetActive(true);

                    unDragableMask.SetActive(false);
                    isPlaying = false;
                });
            });
        }

        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0 , true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                dd.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    mask.SetActive(false);
                    btn03.SetActive(false);

                    isPlaying = false;

                    dd.SetActive(false);

                    foreach (var drag in drags1)
                    {
                        drag.isActived = true;
                    }

                    for (int i = 0; i < drags2.Length; ++i)
                    {
                        drags2[i].isActived = true;
                    }
                }));

                
            });

        }

        void NextGame(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            flag = 0;

            SpineManager.instance.DoAnimation(nextButton, "next", false, () =>
            {
                nextButton.SetActive(false);
            });

            dd.SetActive(true);

            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 2, null, () =>
            {
                dd.SetActive(false);

                bg2Tra.gameObject.SetActive(true);
                bg1Tra.gameObject.SetActive(false);

                unDragableMask.SetActive(false);
                mask.SetActive(false);
                
                isPlaying = false;

                foreach (var spine in spines2)
                {
                    spine.Initialize(true);
                    SpineManager.instance.DoAnimation(spine.gameObject, "y" + spine.gameObject.name);
                }

                foreach (var material in materials)
                {
                    material.SetColor("_Color", new Color(1, 1, 1, 0));
                }
            }));
        }

        //重玩
        void Replay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                LoadMask();

                GameInit();

                mask.SetActive(false);
                dd.SetActive(false);

                isPlaying = false;

                foreach (var drag in drags1)
                {
                    drag.isActived = true;
                }

                for (int i = 0; i < drags2.Length; ++i)
                {
                    drags2[i].isActived = true;
                }
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            dd.transform.GetRectTransform().anchoredPosition = new Vector2(980, -130);
            dd.transform.localScale = Vector2.one * 0.65f;

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 1));

                dd.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }

        #region 拖拽1
        void DragBegin1(Vector3 position, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

            drags1[index].transform.position = Input.mousePosition;
            ColorDisPlay(drags1[index].transform.GetChild(0).GetRawImage());

            if(index == 1)
            {
                drags1[1].transform.GetRectTransform().sizeDelta = new Vector2(220, 100);
            }
        }

        void DragEnd1(Vector3 position, int type, int index, bool isMatch)
        {
            unDragableMask.SetActive(true);

            if (!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);

                drags1[index].DoReset();
                drags1[index].transform.GetChild(0).gameObject.SetActive(false);

                SpineManager.instance.DoAnimation(demon, "xem-jx2", false, () =>
                {
                    SpineManager.instance.DoAnimation(demon, "xem-jx2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(demon, "xem1", true);

                        unDragableMask.SetActive(false);
                    });
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                ColorDisPlay(drags1[index].transform.GetChild(0).GetRawImage(), false, () =>
                {
                    drags1[index].gameObject.SetActive(false);
                });

                SpineManager.instance.DoAnimation(spines1[index].gameObject, "x" + spines1[index].gameObject.name + "2", false, () =>
                {
                    if (++flag == 4)
                    {
                        mono.StartCoroutine(WaitFor(2f, () =>
                        {
                            mask.SetActive(true);
                            nextButton.SetActive(true);
                        }));
                    }
                    else
                    {
                        unDragableMask.SetActive(false);
                    }
                });

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);
            }

            if(index == 1)
            {
                drags1[1].transform.GetRectTransform().sizeDelta = new Vector2(395, 85);
            }
        }
        #endregion

        #region 拖拽2
        void DragBegin2(Vector3 position, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            drags2[index].transform.position = Input.mousePosition;
            drags2[index].transform.parent.SetAsLastSibling();
        }

        void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            unDragableMask.SetActive(true);

           isMatch= drags2[index].drops[0].GetComponent<PolygonCollider2D>().OverlapPoint(Input.mousePosition);

            if (!isMatch)
            {
                float _time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

                drags2[index].DoReset();

                mono.StartCoroutine(WaitFor(_time, () =>
                {
                    unDragableMask.SetActive(false);
                }));
            }
            else
            {
                star.transform.position = drags2[index].transform.position;
                SpineManager.instance.DoAnimation(star, "star", false);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);

                SkeletonGraphic ske = drops[index].transform.parent.GetChild(1).GetComponent<SkeletonGraphic>();

                SpineManager.instance.DoAnimation(drags2[index].transform.GetChild(0).gameObject, "x" + drags2[index].transform.GetChild(0).gameObject.name + "2", false, ()=>
                {
                    drags2[index].gameObject.SetActive(false);
                    mono.StartCoroutine(ChangeSpineAlpha(materials[index], 1, 0.5f, ()=>
                    {
                        unDragableMask.SetActive(false);

                        if (++flag == 3)
                        {

                            //框下移
                            RectTransform frameRect = bg2Tra.Find("Frame").GetRectTransform();
                            frameRect.DOAnchorPosY(50, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
                            {
                                frameRect.DOAnchorPosY(-450, 1f).SetEase(Ease.InOutSine);
                            });

                            //羽毛
                            GameObject obj = bg2Tra.Find("LastFeather").gameObject;
                            obj.SetActive(true);

                            float _time = SpineManager.instance.DoAnimation(obj, "y", false);

                            bg2Tra.Find("Feather").gameObject.SetActive(false);

                            mono.StartCoroutine(WaitFor(3f, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                                bg2Tra.Find("light").gameObject.SetActive(true);

                                SpineManager.instance.DoAnimation(bg2Tra.Find("light").gameObject, "y-light", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(bg2Tra.Find("light").gameObject, "y-light2");
                                });

                                mono.StartCoroutine(WaitFor(2f, () =>
                                {
                                    GameEnd();
                                }));
                            }));
                        }
                    }));          
                });
            }
        }
        #endregion

        mILDrager[] Sort(mILDrager[] list)
        {
            int n = list.Length;
            mILDrager[] ret = new mILDrager[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }

        mILDroper[] Sort(mILDroper[] list)
        {
            int n = list.Length;
            mILDroper[] ret = new mILDroper[n];

            for (int i = 0; i < n; ++i)
            {
                ret[list[i].index] = list[i];
            }

            return ret;
        }

        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.1f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.InOutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        //协程:改变Spine透明度
        IEnumerator ChangeSpineAlpha(Material material, float aimAlpha, float time, Action method = null)
        {
            float i = 0;
            float curAlpha = material.GetColor("_Color").a;
            float deltaAlpha = aimAlpha - curAlpha;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                material.SetColor("_Color", new Color(1, 1, 1, curAlpha + deltaAlpha * i / time));

                yield return wait;
                i += Time.fixedDeltaTime;
            }

            method?.Invoke();
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = dd;

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");


            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //Spine初始化
        void InitSpine(Transform _tra, string animation)
        {
            SkeletonGraphic _ske = _tra.GetComponent<SkeletonGraphic>();
            _ske.startingAnimation = animation;
            _ske.Initialize(true);
        }
    }
}
