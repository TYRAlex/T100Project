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
    public class TD6751Part5
    {
        #region 常用变量
        int talkIndex;
        int GameIndex;
        int flag;
        int moveDistance;
        float movetime;

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
        GameObject antSymbol;
        GameObject starSymbol;
        GameObject success;
        GameObject basketSymbol;
        GameObject homeSymbol;
        GameObject demonSymbol;

        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        mILDrager[] drag;
        mILDroper[] drop;

        bool isPlaying = false;

        string sAnimation;
        string aAnimation;
        #endregion

        #region Game01
        GameObject demon;
        GameObject game01;
        GameObject home;
        GameObject ant;
        GameObject appleSuccess;
        GameObject star;
        GameObject leaves;

        Transform game01Tra;
        Transform treeTra;
        Transform basketTra;

        mILDrager[] appleDra;
        mILDroper[] appleDro;
        #endregion

        #region Game02
        GameObject demon2;
        GameObject game02;
        GameObject home2;
        GameObject ant2;
        GameObject candySuccess;
        GameObject star2;

        Transform game02Tra;
        Transform candyBoxTra;
        Transform cbasketTra;

        mILDrager[] candyDra;
        mILDroper[] candyDro;
        #endregion

        #region Game03
        GameObject demon3;
        GameObject game03;
        GameObject home3;
        GameObject ant3;
        GameObject stoneSuccess;
        GameObject star3;

        Transform game03Tra;
        Transform stoneWallTra;
        Transform sbasketTra;

        mILDrager[] stoneDra;
        mILDroper[] stoneDro;
        #endregion

        #region Game04
        GameObject game04;
        GameObject stone;
        GameObject demon4;
        GameObject ant4;
        GameObject star4;

        Transform game04Tra;
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

            LoadGameProperty();//加载

            GameInit();

            MaskStart();
        }

        #region 加载
        //加载游戏物件
        void LoadGameProperty()
        {
            LoadMask();

            LoadGame01();

            LoadGame02();

            LoadGame03();

            LoadGame04();
        }

        //加载蒙版场景和一些常用变量
        void LoadMask()
        {
            unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;
            unDragableMask.SetActive(false);

            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;
            mask.SetActive(true);

            nextButton = maskTra.Find("NextButton").gameObject;
            nextButton.GetComponent<SkeletonGraphic>().Initialize(true);
            nextButton.SetActive(false);

            dd = maskTra.Find("DD").gameObject;
            dd.transform.GetRectTransform().anchoredPosition = new Vector2(270, -206);
            dd.transform.localScale = new Vector2(1.2f, 1.2f);
            dd.SetActive(false);

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

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);

            Util.AddBtnClick(nextButton, Second);
            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn03, GamePlay);
            Util.AddBtnClick(btn02, Win);
        }

        void LoadGame01()
        {
            game01Tra = curCanvasTra.Find("Game01");
            game01 = game01Tra.gameObject;
            game01.SetActive(true);

            treeTra = game01Tra.Find("Tree");
            basketTra = game01Tra.Find("Basket");
            basketTra.gameObject.SetActive(true);

            star = basketTra.Find("Star").gameObject;
            star.SetActive(false);

            ant = game01Tra.Find("Ant").gameObject;
            ant.SetActive(true);

            home = game01Tra.Find("Home").gameObject;
            home.SetActive(true);

            demon = game01Tra.Find("Demon").gameObject;
            demon.SetActive(false);

            appleSuccess = game01Tra.Find("Success").gameObject;
            appleSuccess.SetActive(false);

            appleDra = treeTra.GetComponentsInChildren<mILDrager>(true);
            appleDro = basketTra.GetComponentsInChildren<mILDroper>(true);

            appleDra = Sort(appleDra);

            for (int i = 0; i < appleDro.Length; ++i)
            {
                appleDra[i].SetDragCallback(DragStart, null, DragEnd);
                appleDra[i].gameObject.SetActive(false);
                appleDra[i].canMove = true;
                SpineManager.instance.DoAnimation(appleDra[i].gameObject, appleDra[i].gameObject.name, false);

                appleDro[i].transform.GetChild(0).gameObject.SetActive(false);
            }

            for (int i = appleDro.Length; i < appleDra.Length; ++i)
            {
                appleDra[i].SetDragCallback(DragStart, null, DragEnd2);
                appleDra[i].gameObject.SetActive(false);
                appleDra[i].canMove = true;
                SpineManager.instance.DoAnimation(appleDra[i].gameObject, appleDra[i].gameObject.name + "2", false);
            }

            leaves = treeTra.Find("Leaves").gameObject;
            leaves.transform.SetAsLastSibling();
        }

        void LoadGame02()
        {
            game02Tra = curCanvasTra.Find("Game02");
            game02 = game02Tra.gameObject;
            game02.SetActive(false);

            candyBoxTra = game02Tra.Find("CandyBox");
            cbasketTra = game02Tra.Find("Basket");
            cbasketTra.gameObject.SetActive(true);

            star2 = cbasketTra.Find("Star").gameObject;
            star2.SetActive(false);

            ant2 = game02Tra.Find("Ant").gameObject;
            ant2.SetActive(true);

            home2 = game02Tra.Find("Home").gameObject;
            home2.SetActive(true);

            demon2 = game02Tra.Find("Demon").gameObject;
            demon2.SetActive(false);

            candySuccess = game02Tra.Find("Success").gameObject;
            candySuccess.SetActive(false);

            candyDra = candyBoxTra.GetComponentsInChildren<mILDrager>(true);
            candyDro = cbasketTra.GetComponentsInChildren<mILDroper>(true);

            candyDra = Sort(candyDra);

            for (int i = 0; i < candyDro.Length; ++i)
            {
                candyDra[i].SetDragCallback(DragStart, null, DragEnd);
                candyDra[i].gameObject.SetActive(false);
                candyDra[i].canMove = true;
                SpineManager.instance.DoAnimation(candyDra[i].gameObject, candyDra[i].gameObject.name, false);

                candyDro[i].transform.GetChild(0).gameObject.SetActive(false);
            }

            for (int i = candyDro.Length; i < candyDra.Length; ++i)
            {
                candyDra[i].SetDragCallback(DragStart, null, DragEnd2);
                candyDra[i].gameObject.SetActive(false);
                candyDra[i].canMove = true;

                SpineManager.instance.DoAnimation(candyDra[i].gameObject, candyDra[i].gameObject.name + "2", false);
            }
        }

        void LoadGame03()
        {
            game03Tra = curCanvasTra.Find("Game03");
            game03 = game03Tra.gameObject;
            game03.SetActive(false);

            stoneWallTra = game03Tra.Find("ShoneWall");
            sbasketTra = game03Tra.Find("Basket");
            sbasketTra.gameObject.SetActive(true);

            star3 = sbasketTra.Find("Star").gameObject;

            ant3 = game03Tra.Find("Ant").gameObject;
            ant3.SetActive(true);

            home3 = game03Tra.Find("Home").gameObject;
            home3.SetActive(true);

            demon3 = game03Tra.Find("Demon").gameObject;
            demon3.SetActive(false);

            stoneSuccess = game03Tra.Find("Success").gameObject;
            stoneSuccess.SetActive(false);

            stoneDra = stoneWallTra.GetComponentsInChildren<mILDrager>(true);
            stoneDro = sbasketTra.GetComponentsInChildren<mILDroper>(true);

            stoneDra = Sort(stoneDra);

            for (int i = 0; i < stoneDro.Length; ++i)
            {
                stoneDra[i].SetDragCallback(DragStart, null, DragEnd);
                stoneDra[i].gameObject.SetActive(false);
                stoneDra[i].canMove = true;
                SpineManager.instance.DoAnimation(stoneDra[i].gameObject, stoneDra[i].gameObject.name, false);

                stoneDro[i].transform.GetChild(0).gameObject.SetActive(false);
            }

            for (int i = stoneDro.Length; i < stoneDra.Length; ++i)
            {
                stoneDra[i].SetDragCallback(DragStart, null, DragEnd2);
                stoneDra[i].gameObject.SetActive(false);
                stoneDra[i].canMove = true;
                
                SpineManager.instance.DoAnimation(stoneDra[i].gameObject, stoneDra[i].gameObject.name + "2", false);
            }
        }

        void LoadGame04()
        {
            game04Tra = curCanvasTra.Find("Game04");
            game04 = game04Tra.gameObject;
            game04.SetActive(false);

            ant4 = game04Tra.Find("Ant").gameObject;

            demon4 = game04Tra.Find("Demon").gameObject;
            demon4.GetComponent<SkeletonGraphic>().Initialize(true);
            demon4.SetActive(false);

            stone = game04Tra.Find("Stone").gameObject;
            SpineManager.instance.DoAnimation(stone, "shitou2", false);

            stone.SetActive(true);

            star4 = game04Tra.Find("Star").gameObject;
            star4.SetActive(false);
        }

        #endregion

        #region 初始化

        //游戏初始化位置
        void GameInit()
        {
            talkIndex = 1;
            GameIndex = 1;
            flag = 0;

            ant4.transform.GetRectTransform().anchoredPosition = new Vector2(-100, 120);
            ant4.GetComponent<SkeletonGraphic>().Initialize(true);
            SoundManager.instance.ShowVoiceBtn(false);

            demon4.transform.GetRectTransform().anchoredPosition = new Vector2(-600, -75);

            GameInit(ant, star, home, demon);
            GameInit(ant2, star2, home2, demon2);
            GameInit(ant3, star3, home3, demon3);

            SetSpineRay(treeTra, false);
            SetSpineRay(candyBoxTra, false);
            SetSpineRay(stoneWallTra, false);

            for (int i = 0; i < 3; ++i)
            {
                star4.transform.GetChild(i).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            }

            Transform shoneWallPos = game03Tra.Find("ShoneWallPos");
            Transform candyBoxPos = game02Tra.Find("CandyBoxPos");
            Transform treePos = game01Tra.Find("TreePos");


            for (int i = 0; i < appleDra.Length; ++i)
            {
                appleDra[i].gameObject.SetActive(true);
                appleDra[i].transform.position = treePos.GetChild(i).position;
            }

            for (int i = 0; i < candyDra.Length; ++i)
            {
                candyDra[i].gameObject.SetActive(true);
                candyDra[i].transform.position = candyBoxPos.GetChild(i).position;
            }

            for (int i = 0; i < stoneDra.Length; ++i)
            {
                stoneDra[i].gameObject.SetActive(true);
                stoneDra[i].transform.position = shoneWallPos.GetChild(i).position;
            }
        }

        void GameInit(GameObject _ant, GameObject _star, GameObject _home, GameObject _demon)
        {
            _ant.transform.GetRectTransform().anchoredPosition = new Vector2(450, 35);
            _ant.transform.rotation = Quaternion.Euler(0, 0, 0);
            _ant.GetComponent<SkeletonGraphic>().Initialize(true);

            _star.GetComponent<SkeletonGraphic>().Initialize(true);

            _home.GetComponent<SkeletonGraphic>().Initialize(true);

            _demon.transform.rotation = Quaternion.Euler(0, 0, 0);
            _demon.transform.GetRectTransform().anchoredPosition = new Vector2(900, 50);
        }
        #endregion

        #region 游戏开始

        //游戏开始
        void MaskStart()
        {
            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        void GameStart(GameObject _home, GameObject _ant, GameObject _star, GameObject _basketSymbol, GameObject _demon,
            mILDrager[] _drag, mILDroper[] _drop, GameObject _success, string _sAnimation, string _aAnimation, Action method = null)
        {
            //将该环节的变量赋给公共变量
            flag = 0;
            unDragableMask.SetActive(true);
            antSymbol = _ant;
            starSymbol = _star;
            demonSymbol = _demon;
            drag = _drag;
            drop = _drop;
            success = _success;
            sAnimation = _sAnimation;
            aAnimation = "my-" + _aAnimation;
            basketSymbol = _basketSymbol;
            homeSymbol = _home;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);

            //树桩开门
            SpineManager.instance.DoAnimation(_home, "shuzhuang", false, () =>
            {
                SpineManager.instance.DoAnimation(_home, "shuzhuang2", false);
            }
            );

            //蚂蚁移动
            SpineManager.instance.DoAnimation(_ant, "my2", true);

            mono.StartCoroutine(SetMoveAncPosX(_ant.transform, moveDistance, movetime, () =>
            {
                DemonStart();

                SpineManager.instance.DoAnimation(_ant, "my", true);


            }));

            _demon.transform.GetRectTransform().anchoredPosition = new Vector2(moveDistance + 200, 50);
            _demon.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        void GameEnd()
        {
            //蚂蚁回家
            SpineManager.instance.DoAnimation(ant4, "my-c3", true);
            dd.transform.GetRectTransform().anchoredPosition = new Vector2(980, -239);
            dd.transform.localScale = new Vector2(1.5f, 1.5f);

            mono.StartCoroutine(SetMoveAncPosX(ant4.transform, moveDistance, movetime, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                SpineManager.instance.DoAnimation(ant4, "my-c4", false, () =>
                {
                    mono.StartCoroutine(StarAnimation(3, 0.075f));

                    SpineManager.instance.DoAnimation(stone, "shitou", false, () =>
                    {
                        demon4.SetActive(true);
                        SpineManager.instance.DoAnimation(demon4, "xem4", false);

                        demon4.transform.GetRectTransform().DOAnchorPosY(demon4.transform.GetRectTransform().anchoredPosition.y + 100, 1).OnComplete(() =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9);

                            SpineManager.instance.DoAnimation(ant4, "my3", false, () =>
                            {
                                mono.StartCoroutine(Wait(2f, () =>
                                {
                                    mask.SetActive(true);
                                    btn03.SetActive(false);
                                    dd.SetActive(false);
                                    successSpine.SetActive(true);
                                    caidaiSpine.SetActive(true);

                                    SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

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
                            });

                            mono.StartCoroutine(Wait(0.25f, () => SpineManager.instance.DoAnimation(demon4, "xem-y", true)));
                        });
                    });
                });
            }));
        }
        #endregion

        #region 拖拽
        void DragStart(Vector3 position, int type, int index)
        {
            if (!drag[index].canMove) return;

            drag[index].transform.SetAsLastSibling();
            drag[index].transform.position = Input.mousePosition;

            SpineManager.instance.DoAnimation(drag[index].gameObject, drag[index].gameObject.name + "3", false);
        }

        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            if (!drag[index].canMove) return;

            GameObject temp = drag[index].gameObject;

            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);

                unDragableMask.SetActive(true);
                drag[index].gameObject.SetActive(false);


                drop[index].transform.GetChild(0).gameObject.SetActive(true);
                starSymbol.transform.position = drop[index].transform.position;
                starSymbol.SetActive(true);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);

                SpineManager.instance.DoAnimation(starSymbol, "star", false, () =>
                {
                    ++flag;
                    unDragableMask.SetActive(false);

                    if (flag == drop.Length) Success();
                });

            }
            else
            {
                drag[index].DoReset();
                if (leaves) leaves.transform.SetAsLastSibling();
                SpineManager.instance.DoAnimation(temp, temp.name + "2", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);

                DemonLaugh();
            }
        }

        void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            GameObject temp = drag[index].gameObject;
            drag[index].DoReset();
            SpineManager.instance.DoAnimation(temp, temp.name, false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            if (leaves) leaves.transform.SetAsLastSibling();

            DemonLaugh();
        }
        #endregion

        #region 通用方法
        //横向移动
        IEnumerator SetMoveAncPosX(Transform temp, float distance, float duration = 1f, Action callBack = null)
        {
            float i = 0;
            float time = duration;
            float curPosx = temp.GetRectTransform().anchoredPosition.x;
            float curPosy = temp.GetRectTransform().anchoredPosition.y;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                temp.GetRectTransform().anchoredPosition = new Vector2(curPosx + distance * i / time, curPosy);
                yield return wait;
                i += Time.fixedDeltaTime;
            }

            callBack?.Invoke();
        }

        IEnumerator Rotate(Transform temp, float angle, float duration = 1f, Action callBack = null)
        {
            float i = 0;
            float time = duration;
            float _x = temp.rotation.x;
            float _y = temp.rotation.y;

            float curAngle = temp.rotation.eulerAngles.z;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                temp.rotation = Quaternion.Euler(_x, _y, curAngle + angle * i / time);
                yield return wait;
                i += Time.fixedDeltaTime;
            }

            callBack?.Invoke();
        }

        //游戏开始
        void GamePlay(GameObject obj)
        {
            SetSpineRay(treeTra);
            SetSpineRay(candyBoxTra);
            SetSpineRay(stoneWallTra);

            unDragableMask.Show();

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                btn03.SetActive(false);
                dd.SetActive(true);

                switch (talkIndex)
                {
                    case 1:
                        mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 1, null, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, ++GameIndex, null, () =>
                            {
                                mask.SetActive(false);
                                dd.SetActive(false);

                                moveDistance = 700;
                                movetime = 2f;
                                GameStart(home, ant, star, basketTra.gameObject, demon, appleDra, appleDro, appleSuccess, "1-apple", "a");
                            }));
                        }));
                        break;
                }

                ++talkIndex;
            });
        }

        //开关Actived
        void SetSpineRay(Transform parent, bool isRay = true)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.GetComponent<mILDrager>())
                {
                    child.GetComponent<mILDrager>().isActived = isRay;
                }
            }
        }

        //小恶魔飞过来嘲笑
        void DemonStart()
        {
            demonSymbol.SetActive(true);

            SpineManager.instance.DoAnimation(demonSymbol, "xem3", true);

            demonSymbol.transform.GetRectTransform().DOAnchorPos(new Vector2(moveDistance + 200, -850), 2f).OnComplete(() =>
            {
                mono.StartCoroutine(Rotate(demonSymbol.transform, -20f, 0.75f, () =>
                {
                    DemonLaugh();
                }));
            });
        }

        //小恶魔嘲笑
        void DemonLaugh()
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

            SpineManager.instance.DoAnimation(demonSymbol, "xem-jx2", false, () =>
            {
                SpineManager.instance.DoAnimation(demonSymbol, "xem-jx2", false, () =>
                {
                    SpineManager.instance.DoAnimation(demonSymbol, "xem1", true);
                    unDragableMask.SetActive(false);
                });
            });
        }

        //拖拽通关
        void Success()
        {
            unDragableMask.SetActive(true);

            demonSymbol.SetActive(false);

            antSymbol.transform.rotation = Quaternion.Euler(0, -180, 0);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 10);

            SpineManager.instance.DoAnimation(antSymbol, "my4", false, () =>
             {
                 SpineManager.instance.DoAnimation(antSymbol, "my5", true);

                 success.SetActive(true);
                 SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                 SpineManager.instance.DoAnimation(success, sAnimation, false, () =>
                 {
                     success.SetActive(false);
                     basketSymbol.SetActive(false);

                     SpineManager.instance.DoAnimation(antSymbol, aAnimation + "1", false, () =>
                     {
                         SpineManager.instance.DoAnimation(antSymbol, aAnimation + "3");

                         mono.StartCoroutine(SetMoveAncPosX(antSymbol.transform, -moveDistance + 300, 1.5f * movetime, () =>
                         {
                             SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);

                             SpineManager.instance.DoAnimation(antSymbol, aAnimation + "2");
                             SpineManager.instance.DoAnimation(homeSymbol, "shuzhuang", false, () =>
                             {
                                 antSymbol.SetActive(false);
                                 antSymbol.transform.rotation = Quaternion.Euler(0, 0, 0);
                                 SpineManager.instance.DoAnimation(homeSymbol, "shuzhuang2", false);

                                 mask.SetActive(true);

                                 nextButton.SetActive(true);
                                 SpineManager.instance.DoAnimation(nextButton, "next2", false);
                                 unDragableMask.Hide();
                             });
                         }));
                     });
                 });
             });
        }

        //排列拖拽数组
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

        //下一关
        void Second(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

            moveDistance = 700;
            movetime = 2f;

            Util.AddBtnClick(nextButton, Third);

            SpineManager.instance.DoAnimation(obj, "next", false, () =>
            {
                dd.Show();

                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, ++GameIndex, null, () =>
                {
                    game02.SetActive(true);
                    game01.SetActive(false);
                    obj.SetActive(false);
                    mask.SetActive(false);

                    dd.Hide();

                    isPlaying = false;

                    GameStart(home2, ant2, star2, cbasketTra.gameObject, demon2, candyDra, candyDro, candySuccess, "2-tangguo", "b");
                }));
            });
        }

        void Third(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            moveDistance = 400;
            movetime = 1f;

            Util.AddBtnClick(nextButton, Fourth);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

            SpineManager.instance.DoAnimation(obj, "next", false, () =>
            {
                dd.Show();

                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, ++GameIndex, null, () =>
                {
                    game03.SetActive(true);
                    game02.SetActive(false);
                    obj.SetActive(false);
                    mask.SetActive(false);

                    dd.Hide();

                    isPlaying = false;

                    GameStart(home3, ant3, star3, sbasketTra.gameObject, demon3, stoneDra, stoneDro, stoneSuccess, "3-shitou", "c");
                }));
            });
        }

        void Fourth(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

            moveDistance = 1200;
            movetime = 5f;

            SpineManager.instance.DoAnimation(obj, "next", false, () =>
            {
                isPlaying = false;

                game04.SetActive(true);
                game03.SetActive(false);
                obj.SetActive(false);
                mask.SetActive(false);

                GameEnd();
            });
        }

        //重玩
        void Replay(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                LoadGameProperty();//加载

                GameInit();

                SetSpineRay(treeTra);
                SetSpineRay(candyBoxTra);
                SetSpineRay(stoneWallTra);

                mask.SetActive(false);
                dd.SetActive(false);

                moveDistance = 700;
                movetime = 2f;

                ++GameIndex;

                GameStart(home, ant, star, basketTra.gameObject, demon, appleDra, appleDro, appleSuccess, "1-apple", "a");
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 0));
                dd.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);
            });
        }

        //协程:播放丁丁说话语音
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            isPlaying = true;

            if (!speaker) speaker = dd;

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");

            isPlaying = false;

            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator Wait(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //协程:星星动画
        IEnumerator StarAnimation(int n, float _time)
        {
            star4.SetActive(true);

            for (int i = 0; i < n; ++i)
            {
                yield return new WaitForSeconds(_time);
                SpineManager.instance.DoAnimation(star4.transform.GetChild(i).gameObject, "star2", false);
            }
        }
        #endregion
    }
}
