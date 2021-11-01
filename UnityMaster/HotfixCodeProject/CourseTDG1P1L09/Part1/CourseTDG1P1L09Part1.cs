using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class CourseTDG1P1L09Part1
    {
        GameObject curGo;
        Transform Buttom;
        GameObject Npc;
        GameObject Part1;
        GameObject Part2;
        GameObject IdleAnim;
        //酸性水壶
        GameObject AcidKettle;
        //碱性水壶
        GameObject AlkaliKettle;
        GameObject Flower_1;
        GameObject Flower_2;
        GameObject Finger;
        GameObject Finger_2;
        GameObject Finger_3;
        GameObject Sunny;
        GameObject EndPanel;
        string curColor;
        Vector3 oldPosOfFlower;
        bool CanCiickEnd;
        GameObject CoverMask;

        int ClickCount;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");
            Npc = curTrans.Find("Content/Npc").gameObject;
            CoverMask = Buttom.Find("CoverMask").gameObject;

            Part1 = Buttom.Find("Part1").gameObject;
            Part2 = Buttom.Find("Part2").gameObject;
            Finger_2 = Part1.transform.Find("Finger_2").gameObject;

            Flower_1 = Part2.transform.Find("Flower_1").gameObject;
            Flower_2 = Part2.transform.Find("Flower_2").gameObject;

            AcidKettle = Part2.transform.Find("AcidKettle").gameObject;
            AlkaliKettle = Part2.transform.Find("AlkaliKettle").gameObject;

            IdleAnim = Part1.transform.Find("IdleAnim").gameObject;

            Finger = Part2.transform.Find("Finger").gameObject;
            Finger_3 = Part2.transform.Find("Finger3").gameObject;
            Sunny = Part2.transform.Find("Sunny").gameObject;

            EndPanel = Part2.transform.Find("EndPanel").gameObject;
            oldPosOfFlower = new Vector3(-35f, -540f, 0f);

            InitGame();
        }

        void InitGame()
        {
            Part1.SetActive(true);
            Part2.SetActive(false);
            Flower_1.transform.localPosition = oldPosOfFlower;
            Flower_2.transform.localPosition = oldPosOfFlower;

            Npc.SetActive(false);
            EndPanel.SetActive(false);
            Finger_2.SetActive(false);
            ResetEndPanel();
            ClickCount = 1;
            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM, 0.2f);
            curColor = "";
            CanCiickEnd = false;

            EndPanel.transform.Find("Bg").GetComponent<ILObject3DAction>().OnPointDownLua = ClickEndPanel;
            Part1.transform.Find("BG").GetComponent<ILObject3DAction>().OnPointDownLua = ClcikGamePart1;

            GamePart1();
        }

        private void ClcikGamePart1(int obj)
        {
            Debug.Log("养花游戏开始");
            Finger_2.SetActive(false);
            Part1.SetActive(false);
            GamePart2();
        }

        private void GamePart1()
        {
            SpineManager.instance.DoAnimation(IdleAnim, "animation", false, () =>
            {
                SpineManager.instance.DoAnimation(IdleAnim, "animation");
                SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, () =>
                {
                    CoverMask.SetActive(true);
                }, () =>
                {
                    ClickFingerAnim(Finger_2);
                    CoverMask.SetActive(false);
                });
            });
        }

        private void GamePart2()
        {
            Part2.SetActive(true);
            var action = AcidKettle.transform.Find("Touch").GetComponent<ILObject3DAction>();
            action.OnPointDownLua = ClickAcidKettleCallback;

            AcidKettle.SetActive(false);
            AlkaliKettle.SetActive(false);
            Finger.SetActive(false);
            Finger_3.SetActive(false);
            curColor = "blue";

            SpineManager.instance.DoAnimation(Sunny, "sunny");

            Flower_1.SetActive(true);
            Flower_2.SetActive(false);
            SpineManager.instance.DoAnimation(Flower_1, "1_idle");

            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 1, () =>
            {
                CoverMask.SetActive(true);
            }, () =>
            {
                ClickFingerAnim(Finger);
                AcidKettle.SetActive(true);
                var shuihu = AcidKettle.transform.Find("ShuiHu").gameObject;
                SpineManager.instance.DoAnimation(shuihu, "suanshuihu_1_i", false);
                CoverMask.SetActive(false);
            });
        }

        private void GamePart3()
        {
            var action = AlkaliKettle.transform.Find("Touch").GetComponent<ILObject3DAction>();
            action.OnPointDownLua = ClickAlkaliKettleCallback;

            curColor = "red";

            AlkaliKettle.SetActive(true);
            var shuihu = AlkaliKettle.transform.Find("ShuiHu").gameObject;
            SpineManager.instance.DoAnimation(shuihu, "suanshuihu_2_i", false);

            Flower_2.SetActive(true);
            var flower = Flower_2;
            SpineManager.instance.DoAnimation(flower, "1_idle");

            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 2, () =>
            {
                CoverMask.SetActive(true);
            }, () =>
            {
                ClickFingerAnim(Finger_3);
                CoverMask.SetActive(false);
            });
        }


        // 点击酸性水壶回调
        private void ClickAcidKettleCallback(int index)
        {
            Finger.SetActive(false);
            if (ClickCount >= 6)
                return;

            CoverMask.SetActive(true);
            var ShuiHuAnim = AcidKettle.transform.Find("ShuiHu").gameObject;
            var flower = Flower_1;
            SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_up", false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_jiaohua1", false, () =>
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    //植物发芽
                    PlantGermination(flower, "blue");
                    SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_down", false, () =>
                    {
                        SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_1_i", false);
                    });
                });

            });
        }


        private void ClickAlkaliKettleCallback(int index)
        {
            Finger_3.SetActive(false);
            if (ClickCount >= 6)
                return;
            CoverMask.SetActive(true);
            var ShuiHuAnim = AlkaliKettle.transform.Find("ShuiHu").gameObject;
            var flower = Flower_2.gameObject;
            SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_up2", false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_jiaohua2", false, () =>
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    //植物发芽
                    PlantGermination(flower, "red");
                    SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_down2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(ShuiHuAnim, "suanshuihu_2_i", false);
                    });
                });
            });
        }

        private void ClickEndPanel(int obj)
        {
            if (!CanCiickEnd)
                return;

            if (curColor == "blue")
            {
                Flower_1.SetActive(true);
                AcidKettle.SetActive(false);
                SpineManager.instance.DoAnimation(Flower_1, "blue_6idle");
                CoverMask.SetActive(true);
                ClickCount = 1;
                Flower_1.transform.DOLocalMoveX(468f, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    GamePart3();
                });
            }
            else
            {
                Flower_2.SetActive(true);
                AlkaliKettle.SetActive(false);
                Flower_2.transform.DOLocalMoveX(-468f, 1f).SetEase(Ease.Linear);
                SpineManager.instance.DoAnimation(Flower_2, "red_6idle");
            }
            EndPanel.SetActive(false);
            ResetEndPanel();
        }

        //发芽
        void PlantGermination(GameObject flower, string flowerColor)
        {
            if (ClickCount < 4)
            {
                Debug.Log("浇水抖动：" + ClickCount + "_1");
                SpineManager.instance.DoAnimation(flower, ClickCount + "_1", false, () =>
                {
                    ClickCount++;
                    Debug.Log("生长：" + "animation" + ClickCount);
                    SpineManager.instance.DoAnimation(flower, "animation" + ClickCount, false, () =>
                    {
                        CoverMask.SetActive(false);
                        SpineManager.instance.DoAnimation(flower, ClickCount + "_idle");
                        Debug.Log("待机：" + ClickCount + "_idle");
                        // ClickFingerAnim(CurFinger);
                    });
                });
            }
            else if (ClickCount == 4)
            {
                SpineManager.instance.DoAnimation(flower, ClickCount + "_1", false, () =>
                {
                    ClickCount++;
                    SpineManager.instance.DoAnimation(flower, flowerColor + "_animation" + ClickCount, false, () =>
                    {
                        CoverMask.SetActive(false);
                        SpineManager.instance.DoAnimation(flower, flowerColor + "_" + ClickCount + "idle");
                        // ClickFingerAnim(CurFinger);
                    });
                });
            }
            else if (ClickCount > 4 && ClickCount < 6)
            {
                Debug.Log("开花");
                //开花
                OpenFlower(flower, flowerColor);
            }
        }

        //开花
        private void OpenFlower(GameObject flower, string flowerColor)
        {
            SpineManager.instance.DoAnimation(flower, flowerColor + ClickCount + "_1", false, () =>
            {
                ClickCount++;
                SpineManager.instance.DoAnimation(flower, flowerColor + "_animation" + ClickCount, false, () =>
                {
                    CoverMask.SetActive(false);
                    SpineManager.instance.DoAnimation(flower, flowerColor + "_" + ClickCount + "idle");
                    // ClickFingerAnim(CurFinger);
                    if (ClickCount >= 6)
                    {
                        PlayEndAnim(flowerColor);
                    }
                });
            });
        }

        void ClickFingerAnim(GameObject finger)
        {
            finger.SetActive(true);
            SpineManager.instance.DoAnimation(finger, "animation");
        }

        void ResetEndPanel()
        {
            var endFlower_1 = EndPanel.transform.Find("EndFlower_1").gameObject;
            var endFlower_2 = EndPanel.transform.Find("EndFlower_2").gameObject;
            var starAnim = EndPanel.transform.Find("StarAnim").gameObject;

            endFlower_1.SetActive(false);
            endFlower_2.SetActive(false);
        }

        void PlayEndAnim(string flowerColor)
        {
            CanCiickEnd = false;
            ResetEndPanel();
            EndPanel.SetActive(true);
            var endFlower_1 = EndPanel.transform.Find("EndFlower_1").gameObject;
            var endFlower_2 = EndPanel.transform.Find("EndFlower_2").gameObject;
            var starAnim = EndPanel.transform.Find("StarAnim").gameObject;

            bool isBlue = flowerColor == "blue" ? true : false;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            if (isBlue)
            {
                Flower_1.SetActive(false);
                endFlower_1.SetActive(true);
                SpineManager.instance.DoAnimation(endFlower_1, "blue_6idle");
            }
            else
            {
                Flower_2.SetActive(false);
                endFlower_2.SetActive(true);
                SpineManager.instance.DoAnimation(endFlower_2, "red_6idle");
            }
            SpineManager.instance.DoAnimation(starAnim, "animation", false, () =>
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 1, null,()=>{
                    CanCiickEnd = true;
                });
                SpineManager.instance.DoAnimation(starAnim, "idle");
            });

        }
    }
}
