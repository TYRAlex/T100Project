using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD5665Part1
    {
        #region 常用变量
        GameObject curCanvas;
        GameObject mask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;
        GameObject nextButton;

        Transform ddTra;
        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        int levelIndex;
        int flag;
        int failTimes;
        string initial;
        bool isFailed;

        Transform bottomTra;
        Transform frogLifesTra;
        Transform demonLifesTra;
        Transform bubbleTra;
        Transform fragmentTra;
        Transform imagesTra;
        Transform shadowTra;
        Transform tongueTra;
        Transform successImageTra;
        Transform failMaskTra;
        Transform insectMoveTra;
        Transform lightTra;
        Transform cracksTra;
        Transform drogersRangeTra;

        Text timeText;

        GameObject frog;
        GameObject demon;
        GameObject water;
        GameObject insect;
        GameObject cutdown;
        GameObject star;
        GameObject unDragableMask;
        GameObject end;

        RectTransform frameRect;
        RectTransform jugglesXRect;
        RectTransform jugglesYRect;

        mILDrager[] dragers;

        Vector2[] InitPoses;

        Coroutine cutdownCor;
        #endregion

        void Start(object o)
        {
            #region 通用初始化
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            #endregion

            LoadMask();

            LoadGame();//加载

            MaskInit();

            GameInit();

            MaskStart();
        }

        void LoadMask()
        {
            //unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;

            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;

            ddTra = maskTra.Find("DD");

            nextButton = maskTra.Find("NextButton").gameObject;

            successSpine = maskTra.Find("successSpine").gameObject;

            caidaiSpine = maskTra.Find("caidaiSpine").gameObject;

            btn01 = maskTra.Find("Btns/0").gameObject;
            btn02 = maskTra.Find("Btns/1").gameObject;
            btn03 = maskTra.Find("Btns/2").gameObject;

            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn02, Win);
            Util.AddBtnClick(btn03, GamePlay);
            //Util.AddBtnClick(nextButton, NextGame);
        }

        void MaskInit()
        {
            //unDragableMask.SetActive(false);
            mask.SetActive(true);

            ddTra.GetChild(0).gameObject.SetActive(false);
            ddTra.GetChild(1).gameObject.SetActive(false);

            nextButton.SetActive(false);
            successSpine.SetActive(false);
            caidaiSpine.SetActive(false);

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);
        }

        void LoadGame()
        {
            bottomTra = curCanvasTra.Find("Bottom");
            jugglesXRect = bottomTra.Find("jugglesX").GetRectTransform();
            jugglesYRect = jugglesXRect.Find("jugglesY").GetRectTransform();

            frogLifesTra = bottomTra.Find("frogHp/life");
            demonLifesTra = bottomTra.Find("Demon/hp/life");
            bubbleTra = curCanvasTra.Find("bubble");
            fragmentTra = curCanvasTra.Find("fragment");
            failMaskTra = curCanvasTra.Find("failMask");
            imagesTra = jugglesYRect.Find("frameMask/frame/images");
            cracksTra = jugglesYRect.Find("frameMask/frame/cracks");
            shadowTra = jugglesXRect.Find("shadow");
            successImageTra = jugglesYRect.Find("success/images");
            tongueTra = bottomTra.Find("frog/tongue");
            insectMoveTra = jugglesYRect.Find("insectMove");
            lightTra = jugglesYRect.Find("success/light");
            drogersRangeTra = bottomTra.Find("drogersRange");

            timeText = curCanvasTra.Find("clock/text").GetText();

            frog = bottomTra.Find("frog").gameObject;
            demon = bottomTra.Find("Demon").gameObject;
            water = jugglesYRect.Find("water").gameObject;
            insect = insectMoveTra.Find("insect").gameObject;

            star = jugglesYRect.Find("success/star").gameObject;
            cutdown = curCanvasTra.Find("cutdown").gameObject;
            unDragableMask = curCanvasTra.Find("unDragableMask").gameObject;
            end = curCanvasTra.Find("middle/end").gameObject;

            frameRect = jugglesYRect.Find("frameMask/frame").GetRectTransform();

            dragers = fragmentTra.GetComponentsInChildren<mILDrager>(true);
        }

        void GameInit()
        {
            isFailed = false;
            levelIndex = 0;
            failTimes = 0;

            LevelInit(levelIndex);

            InitSpine(cutdown, "", false);
            InitSpine(lightTra.GetChild(0).gameObject, "", false);
            InitSpine(star, "", false);
            InitSpine(bottomTra.Find("Bg2").gameObject, "ct");
            InitSpine(failMaskTra.GetChild(0).gameObject, "", false);
            InitLife(demonLifesTra);
            InitLife(frogLifesTra);

            dragers = Sort(dragers);

            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].SetDragCallback(DragStart, Drag, DragEnd);
                dragers[i].dragType = dragers[i].index;
            }

            //失败
            failMaskTra.GetChild(1).gameObject.SetActive(false);
            failMaskTra.gameObject.SetActive(false);
            Util.AddBtnClick(failMaskTra.GetChild(1).gameObject, Replay);

            demon.transform.GetRectTransform().anchoredPosition = new Vector2(-900, 1080);
            frog.transform.localScale = Vector2.one * 0.85f;
            end.SetActive(false);
        }

        //开始游戏
        void StartGame()
        {
            mask.SetActive(false);

            LevelStart(levelIndex);

            isPlaying = false;
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            InitSpine(btn03, "bf2", false);
        }

        #region 游戏方法
        //虫子飞进来动画
        IEnumerator InsectAnimation(Action method = null)
        {
            float time = 6f;
            int loopTimes = 4;

            jugglesXRect.DOAnchorPos(Vector2.up * 450, time).SetEase(Ease.Linear);
            jugglesYRect.DOAnchorPosY(150, time / loopTimes).SetEase(Ease.InOutSine).SetLoops(loopTimes, LoopType.Yoyo);
            shadowTra.DOScale(Vector2.one * 0.7f, time / loopTimes).SetEase(Ease.InOutSine).SetLoops(loopTimes, LoopType.Yoyo);
            shadowTra.GetRawImage().DOColor(new Color(1, 1, 1, 0.7f), time / loopTimes).SetEase(Ease.InOutSine).SetLoops(loopTimes, LoopType.Yoyo);

            yield return new WaitForSeconds(time - 1f);

            //泡泡
            mono.StartCoroutine(WaitForAnimation(bubbleTra, "pp"));

            //图片
            mono.StartCoroutine(WaitForAnimation(fragmentTra, "xx" + initial));

            yield return new WaitForSeconds(2.5f);

            method?.Invoke();
        }

        //初始化双方生命值
        void InitLife(Transform traParent)
        {
            for (int i = 0; i < traParent.childCount; i++)
            {
                Transform tra = traParent.GetChild(i);
                tra.gameObject.SetActive(true);
                tra.GetRawImage().color = Color.white;
            }

            Transform parent = traParent.parent;
            parent.GetChild(0).GetRawImage().color = Color.white;
            parent.GetChild(2).GetRawImage().color = Color.white;
        }

        //隐藏血条
        void HideLife(Transform traParent)
        {
            Color color = new Color(1, 1, 1, 0);

            for (int i = 0; i < traParent.childCount; i++)
            {
                Transform tra = traParent.GetChild(i);
                tra.GetRawImage().DOColor(color, 0.5f).SetEase(Ease.Linear);
            }

            Transform parent = traParent.parent;
            parent.GetChild(0).GetRawImage().DOColor(color, 0.5f).SetEase(Ease.Linear);
            parent.GetChild(2).GetRawImage().DOColor(color, 0.5f).SetEase(Ease.Linear);
        }

        //倒计时
        IEnumerator CutDown(int initTime)
        {
            GameObject clock = timeText.transform.parent.gameObject;

            for (int i = initTime - 1; i >= 0; --i)
            {
                float time = SpineManager.instance.DoAnimation(clock, "zzz1", false);

                yield return new WaitForSeconds(time / 2);

                timeText.text = "" + i;

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 12);

                yield return new WaitForSeconds(1f - time / 2);
            }

            SpineManager.instance.DoAnimation(clock, "zzz2");

            mono.StartCoroutine(FailAnimation());

            
        }

        //关卡初始化
        void LevelInit(int levelNum)
        {
            isFailed = false;

            flag = 0;
            timeText.text = "20";
            InitSpine(frog, "qw1");
            InitSpine(demon, "xem1");
            InitSpine(timeText.transform.parent.gameObject, "zzz3");
            InitSpine(insect.transform.parent.GetChild(0).gameObject, "", false);
            InitSpine(water, "", false);

            //泡泡
            for (int i = 0; i < bubbleTra.childCount; i++)
            {
                GameObject obj = bubbleTra.GetChild(i).gameObject;
                obj.SetActive(true);
                InitSpine(obj, "");
            }

            //泡泡中图片
            for (int i = 0; i < fragmentTra.childCount; i++)
            {
                GameObject obj = fragmentTra.GetChild(i).gameObject;
                obj.SetActive(true);
                InitSpine(obj, "");
            }

            //框
            frameRect.anchoredPosition = new Vector2(-610, 510);
            InitSpine(frameRect.gameObject, "k0", false);

            //阴影
            shadowTra.gameObject.SetActive(true);
            shadowTra.localScale = Vector3.one;
            shadowTra.GetRawImage().color = Color.white;

            //框里面的图片
            for (int i = 0; i < imagesTra.childCount; i++)
            {
                Transform traParent = imagesTra.GetChild(i);
                traParent.gameObject.SetActive(false);

                for (int j = 0; j < traParent.childCount; j++)
                {
                    Transform tra = traParent.GetChild(j);

                    tra.gameObject.SetActive(false);
                    tra.GetRawImage().color = Color.white;
                }
            }

            //juggles的X与Y
            jugglesXRect.anchoredPosition = new Vector2(-1200, 325);
            jugglesYRect.anchoredPosition = Vector2.zero;

            //成功图片
            for (int i = 0; i < successImageTra.childCount; i++)
            {
                RawImage raw = successImageTra.GetChild(i).GetRawImage();
                raw.gameObject.SetActive(false);
                raw.color = Color.white;
            }

            //初始位置
            if (InitPoses == null)
            {
                InitPoses = new Vector2[5]
                {
                    new Vector2(-300, 365),
                    new Vector2(0, 365),
                    new Vector2(300, 365),
                    new Vector2(-150, 115),
                    new Vector2(150, 115)
                };
            }

            //虫子和光 位置
            insectMoveTra.gameObject.SetActive(true);
            insectMoveTra.GetRectTransform().anchoredPosition = Vector2.up * 25;

            //虫子
            insect.transform.SetParent(insectMoveTra);
            insect.transform.GetRectTransform().anchoredPosition = Vector2.down * 25;
            insect.transform.localScale = Vector3.one;
            insect.SetActive(true);
            InitSpine(insect, "cz" + (levelNum + 1));
            insect.transform.rotation = Quaternion.Euler(0, 0, 0);
            Material material = insect.GetComponent<SkeletonGraphic>().material;
            material.SetColor("_Color", Color.white);

            //光
            material = insectMoveTra.Find("light").GetComponent<SkeletonGraphic>().material;
            material.SetColor("_Color", Color.white);

            BoneFollowerGraphic timeBone = timeText.GetComponent<BoneFollowerGraphic>();
            timeBone.Awake();
            timeBone.boneName = "I1";

            //拖拽层级
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].transform.SetAsLastSibling();
            }

            //显示裂痕和对应的放置区域
            ShowOneOnly(cracksTra, levelNum);
            ShowOneOnly(drogersRangeTra, levelNum);
        }

        //一关开始
        void LevelStart(int levelNum)
        {
            LevelInit(levelNum);
            unDragableMask.SetActive(true);
            imagesTra.GetChild(levelNum).gameObject.SetActive(true);
            initial = ASCIIToChar(97 + levelNum);

            //打乱位置
            Shuffle(InitPoses);
            for (int i = 0; i < InitPoses.Length; i++)
            {
                dragers[i].transform.GetRectTransform().anchoredPosition = InitPoses[i];
                bubbleTra.GetChild(i).GetRectTransform().anchoredPosition = InitPoses[i] + Vector2.up * 50;
            }

            mono.StartCoroutine(InsectAnimation(() =>
            {
                //倒计时
                string[] animations = new string[] { "3", "2", "1", "4" };
                mono.StartCoroutine(WaitAnimation(cutdown, animations, () =>
                {
                    InitSpine(cutdown, "", false);

                    //闹钟
                    unDragableMask.SetActive(false);
                    cutdownCor = mono.StartCoroutine(CutDown(20));
                }));
            }));
        }

        //成功动画
        IEnumerator SuccessAnimation()
        {
            mono.StopCoroutine(cutdownCor);

            //剩下的最后一关消失
            dragers[0].gameObject.SetActive(false);

            GameObject obj = bubbleTra.GetChild(0).gameObject;
            SpineManager.instance.DoAnimation(obj, "pp2", false, () =>
            {
                obj.SetActive(false);
            });

            yield return new WaitForSeconds(2f);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            //青蛙动画
            SpineManager.instance.DoAnimation(frog, "qw2", false, () =>
            {
                SpineManager.instance.DoAnimation(frog, "qw1");
                insect.SetActive(false);
            });

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

            //激活组件
            tongueTra.GetComponent<BoneFollowerGraphic>().Awake();
            yield return new WaitForSeconds(1 / 6f);

            //让虫子跟随舌头
            insect.transform.SetParent(tongueTra);
            yield return new WaitForSeconds(1 / 6f);

            //框破碎
            imagesTra.GetChild(levelIndex).gameObject.SetActive(false);
            cracksTra.GetChild(levelIndex).gameObject.SetActive(false);
            shadowTra.GetRawImage().DOColor(new Color(1, 1, 1, 0), 0.5f).SetEase(Ease.Linear);

            RawImage raw = successImageTra.GetChild(levelIndex).GetRawImage();
            raw.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(frameRect.gameObject, "k3", false);

            //光
            SpineManager.instance.DoAnimation(lightTra.GetChild(0).gameObject, "g1", false);
            SpineManager.instance.DoAnimation(star, "g2", false);

            //小恶魔
            float time = SpineManager.instance.DoAnimation(demon, "xem2");
            yield return new WaitForSeconds(time);           

            ColorDisPlay(raw, false);
            time = SpineManager.instance.DoAnimation(demon, "xem4");

            yield return new WaitForSeconds(0.5f);

            //生命值扣除
            RawImage lifeRaw = demonLifesTra.GetChild(levelIndex).GetRawImage();
            ColorDisPlay(lifeRaw, false);

            if (++levelIndex < 4)
            {
                yield return new WaitForSeconds(time - 0.5f);

                SpineManager.instance.DoAnimation(demon, "xem1");
                LevelStart(levelIndex);
            }
            else
            {
                //全部通关以后的动画
                HideLife(frogLifesTra);
                HideLife(demonLifesTra);

                //青蛙变大
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9);
                frog.transform.DOScale(Vector2.one, 1f).SetEase(Ease.InSine);

                yield return new WaitForSeconds(1.25f);
                
                time = SpineManager.instance.DoAnimation(frog, "qw3", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 10);

                yield return new WaitForSeconds(1/6f);

                //小恶魔被打飞
                demon.transform.GetRectTransform().DOAnchorPos(new Vector2(-1500, 1500), 0.3f).SetEase(Ease.OutQuint);

                yield return new WaitForSeconds(2f);

                mono.StartCoroutine(WhiteCurtainTransition(failMaskTra.GetRawImage(), ()=>
                {
                    end.SetActive(true);
                    SpineManager.instance.DoAnimation(end, "animation");
                }));

                yield return new WaitForSeconds(2f);

                GameEnd();
            }
        }

        //失败动画
        IEnumerator FailAnimation()
        {
            isFailed = true;

            yield return new WaitForSeconds(0.5f);

            //剩下泡泡消失
            for (int i = 0; i < bubbleTra.childCount; i++)
            {
                GameObject fragObj = dragers[i].gameObject;
                GameObject bubbleObj = bubbleTra.GetChild(i).gameObject;

                if (bubbleObj.activeSelf)
                {
                    fragObj.SetActive(false);

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

                    float time1 = InitSpine(bubbleObj, "pp2");

                    mono.StartCoroutine(WaitFor(time1, ()=>
                    {
                        bubbleObj.SetActive(false);
                    }));

                    yield return new WaitForSeconds(0.1f);
                }
            }

            //取消拖拽
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].canMove = false;
                dragers[i].DoReset();
            }

            //虫子
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
            RectTransform rect = insectMoveTra.GetRectTransform();
            insect.transform.DORotateQuaternion(Quaternion.Euler(0, -180, 0), 0.1f).SetEase(Ease.InOutSine);

            //框
            frameRect.DOAnchorPosY(150, 0.8f).SetEase(Ease.InSine);

            yield return new WaitForSeconds(0.1f);

            //小恶魔嘲笑
            SpineManager.instance.DoAnimation(demon, "xem3");

            //虫子移动 和 光
            SpineManager.instance.DoAnimation(rect.GetChild(0).gameObject, "gx1", false, ()=>
            {
                SpineManager.instance.DoAnimation(rect.GetChild(0).gameObject, "gx2");
            });
            rect.DOAnchorPos(new Vector2(-500, 250), 3f).SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(0.3f);

            //水掉落和影子
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            SpineManager.instance.DoAnimation(water, "s", false, () => InitSpine(water, "", false));
            shadowTra.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.1f);

            //青蛙
            float time = SpineManager.instance.DoAnimation(frog, "qw4", false);
            yield return new WaitForSeconds(0.5f);

            //扣除生命值
            RawImage raw = frogLifesTra.GetChild(failTimes).GetRawImage();
            ColorDisPlay(raw, false);

            yield return new WaitForSeconds(time - 0.5f);

            SpineManager.instance.DoAnimation(frog, "qw1");

            yield return new WaitForSeconds(2.6f - time);

            //透明消失
            Material material1 = rect.GetChild(0).GetComponent<SkeletonGraphic>().material;
            Material material2 = insect.GetComponent<SkeletonGraphic>().material;

            mono.StartCoroutine(ChangeSpineAlpha(material1, 0, 0.5f));
            mono.StartCoroutine(ChangeSpineAlpha(material2, 0, 0.5f));

            yield return new WaitForSeconds(0.5f);

            rect.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            if (++failTimes == 4)
            {
                //重来
                raw = failMaskTra.GetRawImage();
                raw.color = new Color(0, 0, 0, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(new Color(0, 0, 0, 0.8f), 0.5f).SetEase(Ease.Linear);

                SpineManager.instance.DoAnimation(failMaskTra.GetChild(0).gameObject, "pz", false, () =>
                {
                    failMaskTra.GetChild(1).gameObject.SetActive(true);
                });
            }
            else
            {
                SpineManager.instance.DoAnimation(demon, "xem1");
                LevelStart(levelIndex);
            }
        }
        #endregion

        #region 拖拽
        void DragStart(Vector3 position, int type, int index)
        {
            SoundManager.instance.PlayClip(9);

            //点击动画与排列层级
            GameObject obj = dragers[index].gameObject;
            dragers[index].canMove = true;
            obj.transform.SetAsLastSibling();
            obj.transform.position = Input.mousePosition;
            InitSpine(obj, "dxx-" + initial + obj.name, false);
        }

        void Drag(Vector3 position, int type, int index)
        {

        }

        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            if (!dragers[index].canMove || isFailed) return;

            if(isMatch && index > 0)
            {
                //不规则放置
                PolygonCollider2D collider2D = drogersRangeTra.GetChild(levelIndex).GetChild(index - 1).GetComponent<PolygonCollider2D>();
                isMatch = collider2D.OverlapPoint(Input.mousePosition);
            }
            else
            {
                isMatch = false;
            }

            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

                //框
                SpineManager.instance.DoAnimation(frameRect.gameObject, "k1", false, () =>
                SpineManager.instance.DoAnimation(frameRect.gameObject, "k0", false));

                //拼图
                RawImage raw = imagesTra.GetChild(levelIndex).GetChild(index - 1).GetRawImage();
                dragers[index].gameObject.SetActive(false);
                ColorDisPlay(raw);

                //泡泡破裂
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                GameObject obj = bubbleTra.GetChild(type).gameObject;
                SpineManager.instance.DoAnimation(obj, "pp2", false, () =>
                obj.SetActive(false));

                if (++flag == 4) mono.StartCoroutine(SuccessAnimation());
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 11);

                //重置
                dragers[index].transform.GetRectTransform().anchoredPosition = InitPoses[index];
                GameObject obj = dragers[index].gameObject;
                InitSpine(obj, "xx" + initial + obj.name);

                //泡泡
                obj = bubbleTra.GetChild(index).gameObject;
                SpineManager.instance.DoAnimation(obj, "pp1", false, () =>
                SpineManager.instance.DoAnimation(obj, "pp0"));

                //框
                SpineManager.instance.DoAnimation(frameRect.gameObject, "k2", false, () =>
                SpineManager.instance.DoAnimation(frameRect.gameObject, "k0", false));
            }
        }

        void FaillBack(int type)
        {
            
        }
        #endregion

        #region 游戏通用环节
        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                ddTra.GetChild(0).gameObject.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        btn03.SetActive(false);
                        ddTra.GetChild(0).gameObject.SetActive(false);

                        StartGame();
                    }));
                }));
            });

        }

        void GameEnd()
        {
            isPlaying = true;

            mono.StartCoroutine(WaitFor(2f, () =>
            {
                mask.SetActive(true);
                btn03.SetActive(false);
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

                        btn01.SetActive(true);
                        btn02.SetActive(true);

                        InitSpine(btn01, "fh2", false);
                        InitSpine(btn02, "ok2", false);

                        isPlaying = false;
                    });
                });
            }));
        }

        //重玩
        void Replay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            float time = 0;

            if (mask.activeSelf) time = SpineManager.instance.DoAnimation(btn01, "fh", false);

            mono.StartCoroutine(WaitFor(time, () =>
            {
                MaskInit();
                GameInit();
                StartGame();
            }));
        }

        //胜利
        void Win(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(1).gameObject, SoundManager.SoundType.VOICE, 2));

                ddTra.GetChild(1).gameObject.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }

        #endregion

        #region 通用方法
        //只显示一个子物件
        void ShowOneOnly(Transform _tra, int _num)
        {
            for (int i = 0; i < _tra.childCount; i++)
            {
                _tra.GetChild(i).gameObject.SetActive(false);
            }

            _tra.GetChild(_num).gameObject.SetActive(true);
        }

        //白幕转场
        IEnumerator WhiteCurtainTransition(RawImage _raw, Action method = null)
        {
            _raw.color = new Color(1, 1, 1, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.white, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.4f);

            _raw.DOColor(new Color(1, 1, 1, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

            method?.Invoke();
        }

        void Shuffle<T>(T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = (Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
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

        string ASCIIToChar(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return strCharacter;
            }
            else
            {
                return "";
            }
        }

        //错开播放
        IEnumerator WaitForAnimation(Transform _tra, string _animation, float _time = 0.2f, Action method = null)
        {
            WaitForSeconds wait = new WaitForSeconds(_time);

            for (int i = 0; i < _tra.childCount; i++)
            {
                GameObject obj = _tra.GetChild(i).gameObject;
                Material material = obj.GetComponent<SkeletonGraphic>().material;

                //渐变显示
                material.SetColor("_Color", new Color(1, 1, 1, 0));

                InitSpine(obj, _animation + obj.name);

                mono.StartCoroutine(ChangeSpineAlpha(material, 1, _time * 1.5f));

                yield return wait;
            }

            yield return wait;

            method?.Invoke();
        }

        //连续播放动画
        IEnumerator WaitAnimation(GameObject _obj, string[] _animations, Action _method = null)
        {
            for (int i = 0; i < _animations.Length; i++)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, i == 3 ? 0 : 1);

                float _time = SpineManager.instance.DoAnimation(_obj, _animations[i], false);

                yield return new WaitForSeconds(1f);
            }

            _method?.Invoke();
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
            if (!speaker) speaker = ddTra.GetChild(0).gameObject;

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
        float InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);

            if (animation == "") return 0f;
            else return _ske.AnimationState.Data.SkeletonData.FindAnimation(animation).Duration;
        }
        #endregion
    }
}
