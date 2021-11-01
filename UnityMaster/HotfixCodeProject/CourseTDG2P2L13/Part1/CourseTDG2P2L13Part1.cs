using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;


namespace ILFramework.HotClass
{
    public class CourseTDG2P2L13Part1
    {
        GameObject curGo;
        Transform Buttom;
        GameObject UIBarAnim_1, UIBarAnim_2;
        GameObject Btns, CoverMask, Npc;
        GameObject Cotton, Tools, SunShine, EndAnim;
        int GamePartIndex, handIndex;
        MonoBehaviour mono;
        bool CanPlayPicking;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Buttom = curTrans.Find("Content/Buttom");
            Npc = curTrans.Find("Content/Npc").gameObject;

            UIBarAnim_1 = Buttom.Find("UIBarAnim_1").gameObject;
            UIBarAnim_2 = Buttom.Find("UIBarAnim_2").gameObject;

            Btns = Buttom.Find("Btns").gameObject;
            CoverMask = Buttom.Find("CoverMask").gameObject;
            Cotton = Buttom.Find("Cotton").gameObject;
            Tools = Buttom.Find("Tools").gameObject;
            SunShine = Buttom.Find("SunShine").gameObject;
            EndAnim = Buttom.Find("EndAnim").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            CanPlayPicking = false;
            InitGame();

        }

        private void InitGame()
        {
            Btns.SetActive(false);
            UIBarAnim_1.SetActive(false);
            UIBarAnim_2.SetActive(false);
            SunShine.SetActive(true);
            EndAnim.SetActive(false);
            CoverMask.SetActive(false);
            Cotton.SetActive(false);

            GamePartIndex = 0;
            handIndex = 0;
            //注册每个按钮的回调
            for (int i = 1; i <= Btns.transform.childCount; i++)
            {
                var btn = Btns.transform.GetChild(i - 1);
                var action = btn.GetComponent<ILObject3DAction>();
                action.index = i;
                action.OnPointDownLua = BtnClicked;
            }

            ResetAllAnims();

            LogicManager.instance.ShowReplayBtn(false);
            //Play Bgm
            SoundManager.instance.BgSoundPart2();

            SpineManager.instance.DoAnimation(SunShine, "sunny");

            //播放语音结束
            NpcTalk(0, null, () =>
            {
                UIBarAnim_1.SetActive(true);
                SpineManager.instance.DoAnimation(UIBarAnim_1, "animation", false, () =>
                {
                    UIBarAnim_1.SetActive(false);
                    UIBarAnim_2.SetActive(true);
                    Btns.SetActive(true);
                    SpineManager.instance.DoAnimation(UIBarAnim_2, "kuang_idle");

                    GamePartIndex = 1;
                    ChangeGamePart(GamePartIndex);
                });
            });

        }

        void ResetAllAnims()
        {
            for (int i = 0; i < Tools.transform.childCount; i++)
            {
                if (i < 7)
                {
                    var tool = Tools.transform.GetChild(i);
                    try
                    {
                        var skt = tool.GetComponent<SkeletonGraphic>();
                        skt.AnimationState.ClearTrack(0);
                        skt.AnimationState.SetEmptyAnimation(0,0);
                    }
                    catch (System.Exception)
                    {
                    }

                }
                else
                {
                    for (int j = 0; j < Tools.transform.GetChild(7).childCount; i++)
                    {
                        var tool = Tools.transform.GetChild(j);
                        try
                        {
                            var skt = tool.GetComponent<SkeletonGraphic>();
                            skt.AnimationState.ClearTrack(0);
                             skt.AnimationState.SetEmptyAnimation(0,0);
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                }

            }

            for (int i = 0; i < Cotton.transform.childCount; i++)
            {
                var cot = Cotton.transform.GetChild(i);
                try
                {
                    var skt = cot.GetComponent<SkeletonGraphic>();
                    skt.AnimationState.SetEmptyAnimations(0);
                }
                catch (System.Exception)
                {
                }
            }
        }


        private void BtnClicked(int index)
        {
            var btn = Btns.transform.Find(index.ToString());
            var normal = btn.Find("Normal").gameObject;
            var anim = btn.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "animation" + index.ToString(), false, () =>
            {
                normal.SetActive(true);
                anim.SetActive(false);

                //具体执行步骤
                ChangeGameActionBy(index);

            });
        }

        void NpcTalk(int soundIndex, Action StartAction = null, Action EndAction = null)
        {

            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, soundIndex, () =>
            {
                if (StartAction != null)
                {
                    StartAction.Invoke();
                }
                CoverMask.SetActive(true);

            }, () =>
            {
                if (EndAction != null)
                {
                    EndAction.Invoke();
                }
                CoverMask.SetActive(false);
            });
        }

        private void ChangeGamePart(int index)
        {
            NpcTalk(0, () =>
            {

            }, () =>
            {
                ActiveBtnBy(index);
            });
        }

        private void ChangeGameActionBy(int index)
        {
            switch (index)
            {
                case 1:
                    BtnAction_1(index);
                    break;
                case 2:
                    BtnAction_2(index);
                    break;
                case 3:
                    BtnAction_3(index);
                    break;
                case 4:
                    BtnAction_4(index);
                    break;
                case 5:
                    BtnAction_5(index);
                    break;
                case 6:
                    BtnAction_6(index);
                    break;
                case 7:
                    BtnAction_7(index);
                    break;
            }
        }

        void ActiveBtnBy(int index)
        {
            for (int i = 0; i < Btns.transform.childCount; i++)
            {
                Btns.transform.GetChild(i).GetComponent<Image>().enabled = false;
            }
            Btns.transform.Find(index.ToString()).GetComponent<Image>().enabled = true;
        }

        void NormalCottonAnim(int index, Action action = null)
        {
            for (int i = 0; i < Cotton.transform.childCount; i++)
            {
                var cot = Cotton.transform.GetChild(i).gameObject;
                cot.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimations(0);
                if (i == Cotton.transform.childCount - 1)
                {
                    SpineManager.instance.DoAnimation(cot, index.ToString(), false, () =>
                    {
                        GamePartIndex++;
                        ChangeGamePart(GamePartIndex);
                        if (action != null)
                        {
                            action.Invoke();
                        }

                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(cot, index.ToString(), false);
                }

            }
        }

        void BtnAction_1(int index)
        {
            CoverMask.SetActive(true);
            var tool = Tools.transform.Find(index.ToString()).gameObject;
            tool.SetActive(true);
            SpineManager.instance.DoAnimation(tool, index.ToString(), false, () =>
            {
                CoverMask.SetActive(false);
                GamePartIndex++;
                ChangeGamePart(GamePartIndex);
            });
        }

        void BtnAction_2(int index)
        {
            CoverMask.SetActive(true);
            var tool = Tools.transform.Find(index.ToString()).gameObject;
            var soil = Tools.transform.Find("1").gameObject;
            tool.SetActive(true);
            SpineManager.instance.DoAnimation(tool, index.ToString(), false, () =>
            {

                tool.SetActive(false);
                soil.SetActive(false);
                //所有种子出现
                Cotton.SetActive(true);
                NormalCottonAnim(index);
            });
        }


        private void BtnAction_3(int index)
        {
            CoverMask.SetActive(true);
            var tool = Tools.transform.Find(index.ToString()).gameObject;
            tool.SetActive(true);

            SpineManager.instance.DoAnimation(tool, index.ToString(), false, () =>
            {
                tool.SetActive(false);
                Cotton.SetActive(true);
                NormalCottonAnim(index);
            });
        }
        private void BtnAction_4(int index)
        {
            CoverMask.SetActive(true);
            var tool = Tools.transform.Find(index.ToString()).gameObject;
            tool.SetActive(true);

            SpineManager.instance.DoAnimation(tool, index.ToString(), false, () =>
            {
                Cotton.SetActive(true);
                tool.SetActive(false);
                NormalCottonAnim(index);
            });
        }

        private void BtnAction_5(int index)
        {
            CoverMask.SetActive(true);
            var tool = Tools.transform.Find(index.ToString()).gameObject;
            tool.SetActive(true);

            SpineManager.instance.DoAnimation(tool, index.ToString(), false, () =>
            {
                Cotton.SetActive(true);
                tool.SetActive(false);
            });

            NormalCottonAnim(index, () =>
            {
                //生虫
                for (int i = 0; i < Cotton.transform.childCount; i++)
                {
                    var cot = Cotton.transform.GetChild(i).gameObject;
                    SpineManager.instance.DoAnimation(cot, "6", false);
                }
            });
        }

        private void BtnAction_6(int index)
        {
            CoverMask.SetActive(true);
            var tool = Tools.transform.Find(index.ToString()).gameObject;
            tool.SetActive(true);

            SpineManager.instance.DoAnimation(tool, index.ToString(), false, () =>
            {
                Cotton.SetActive(true);
                tool.SetActive(false);
                NormalCottonAnim(index + 1, () =>
                {
                    //虫子掉后开始接棉花
                    for (int i = 0; i < Cotton.transform.childCount; i++)
                    {
                        var cot = Cotton.transform.GetChild(i).gameObject;
                        SpineManager.instance.DoAnimation(cot, "8", false);
                    }
                });
            });
        }

        private void BtnAction_7(int index)
        {
            //mono.StartCoroutine(BeginPickingCotton(index));
            BeginPickingCotton(index);
        }

        void BeginPickingCotton(int index)
        {
            CanPlayPicking = true;
        }

        private void Update()
        {
            if (CanPlayPicking)
            {
                CanPlayPicking = false;
                var tools = Tools.transform.Find("7").gameObject;
                tools.SetActive(true);
                CoverMask.SetActive(true);

                GameObject curHand = null;
                GameObject curCot = null;
                int length = tools.transform.childCount - 1;

                curHand = tools.transform.GetChild(handIndex).gameObject;
                if (handIndex < 4)
                {
                    curCot = Cotton.transform.GetChild(4 + handIndex).gameObject;
                }
                else
                {
                    curCot = Cotton.transform.GetChild(handIndex - 4).gameObject;
                }
                curHand.SetActive(true);
                SpineManager.instance.DoAnimation(curHand, "7", true);
                SpineManager.instance.DoAnimation(curCot, "9", false, () =>
                {

                    curHand.SetActive(false);
                    if (handIndex >= 7)
                    {
                        CanPlayPicking = false;
                        EndAnim.SetActive(true);
                        SpineManager.instance.DoAnimation(EndAnim, "end", false, () =>
                        {
                            LogicManager.instance.ShowReplayBtn(true);
                            LogicManager.instance.SetReplayEvent(() =>
                            {
                                InitGame();
                            });
                        });
                    }
                    else
                    {
                        CanPlayPicking = true;
                        handIndex++;
                    }
                });
            }
        }

    }
}
