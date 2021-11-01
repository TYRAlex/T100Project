using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public enum RianbowType
    {
        red, orange, yellow, green, cyan, blue, purple, brownness
    }

    public enum SelectColor { blue, red, yellow };


    public class CourseTDKP1L10Part1
    {
        GameObject curGo;
        Transform Buttom;

        //云背景动画
        GameObject CloudAnim_1;
        GameObject CloudAnim_2;

        GameObject Npc;
        GameObject Shield;

        //颜色框
        GameObject UIBar;
        GameObject SUIBar;
        Transform ContentOfSUIBar;
        //彩虹动画
        Transform RainbowLightAnims;
        Transform RainbowColorAndStarAnims;

        //游戏部分
        int GamePartIndex = 0;
        GameObject CurRianBowColor;
        bool IsFillingRianBow = false;
        float fillSpeed = 0f;
        GameObject RightIcon;
        GameObject Mask;
        GameObject EndPanel;

        //目标颜色
        List<SelectColor> TargetColors;
        //选择的颜色
        List<SelectColor> SelectColors;



        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");
            Npc = curTrans.Find("Content/Npc").gameObject;
            Shield = curTrans.Find("Content/Shield").gameObject;
            Mask = curTrans.Find("Content/Mask").gameObject;
            EndPanel = curTrans.Find("Content/EndPanel").gameObject;

            UIBar = Buttom.transform.Find("UIBar").gameObject;
            SUIBar = Buttom.transform.Find("SUIBar").gameObject;
            ContentOfSUIBar = SUIBar.transform.Find("Content");

            RightIcon = SUIBar.transform.Find("RightIcon").gameObject;


            //云动画
            CloudAnim_1 = Buttom.Find("CloudAnim_1").gameObject;
            CloudAnim_2 = Buttom.Find("CloudAnim_2").gameObject;

            RainbowLightAnims = Buttom.Find("RianbowLights");
            RainbowColorAndStarAnims = Buttom.Find("RianbowColos");

            TargetColors = new List<SelectColor>();
            SelectColors = new List<SelectColor>();

            InitGame();
        }


        void Update()
        {
            if (IsFillingRianBow)
            {
                CurRianBowColor.GetComponent<Image>().fillAmount += fillSpeed * Time.deltaTime;
                if (CurRianBowColor.GetComponent<Image>().fillAmount >= 1f)
                {
                    IsFillingRianBow = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    var star = CurRianBowColor.transform.Find("Star").gameObject;
                    star.SetActive(true);
                    SpineManager.instance.DoAnimation(star, "animation", false, () =>
                    {
                        GamePartIndex++;
                        star.SetActive(false);
                        ChangeGamePartIndex(GamePartIndex);
                    });
                }
            }
        }

        void InitGame()
        {
            
            InitUIBar();
            ResetRianBowColor();

            RightIcon.SetActive(false);

            TargetColors.Clear();
            SelectColors.Clear();

            CloudAnim_1.SetActive(true);
            CloudAnim_2.SetActive(true);

            Shield.SetActive(false);
            Mask.SetActive(false);
            EndPanel.SetActive(false);

            fillSpeed = 2f;

            SpineManager.instance.DoAnimation(CloudAnim_1, "animation");
            SpineManager.instance.DoAnimation(CloudAnim_2, "cloud");

            ResetRianBowLight();

            SoundManager.instance.BgSoundPart1();

            NpcSpeckAnim(0, null, () =>
            {
                Debug.Log("开始游戏");
                GamePartIndex = 1;
                ChangeGamePartIndex(GamePartIndex);
                // NpcIdleAnim(false);
            });
        }

        void InitUIBar()
        {
            SUIBar.SetActive(true);
            UIBar.SetActive(true);

            for (int i = 0; i < UIBar.transform.childCount; i++)
            {
                var colorBtn = UIBar.transform.GetChild(i).gameObject;
                colorBtn.SetActive(true);
                colorBtn.transform.Find("Anim").gameObject.SetActive(false);

                var objAction = colorBtn.GetComponent<ILObject3DAction>();
                objAction.index = i;
                objAction.OnPointDownLua = ClickColorBtn;
            }

            ResetColorIconAnim();
            ResetColorBtn();
        }


        void NpcUnHappyAnim(bool isLoop = false, Action endCallBack = null)
        {
            Npc.SetActive(true);
            SpineManager.instance.DoAnimation(Npc, "unhappy", isLoop, () =>
            {
                if (endCallBack != null)
                    endCallBack.Invoke();
                NpcIdleAnim();
            });
        }

        void NpcIdleAnim(bool IsLoop = true, Action endCallBack = null)
        {
            Npc.SetActive(true);
            SpineManager.instance.DoAnimation(Npc, "breath", IsLoop, endCallBack);
        }

        void NpcSpeckAnim(int voiceIndex, Action startCallback = null, Action endCallBack = null)
        {
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, voiceIndex, () =>
            {
                Shield.SetActive(true);
                if (startCallback != null)
                    startCallback.Invoke();

            }, () =>
            {
                Npc.SetActive(true);
                Shield.SetActive(false);
                if (endCallBack != null)
                    endCallBack.Invoke();
            });
        }

        void ChangeGamePartIndex(int index)
        {
            InitUIBar();
            ResetRianBowLight();
            switch (index)
            {
                case 1:
                    //红色
                    GamePart_1();
                    break;
                case 2:
                    //橙色(红+黄)
                    GamePart_2();
                    break;
                case 3:
                    //黄色
                    GamePart_3();
                    break;
                case 4:
                    GamePart_4();
                    break;
                case 5:
                    GamePart_5();
                    break;
                case 6:
                    GamePart_6();
                    break;
                case 7:
                    GamePart_7();
                    break;
                case 8:
                    GamePart_8();
                    break;
                case 9:
                    GameEndPart();
                    break;
            }

        }


        //游戏每个环节
        void GamePart_1()
        {
            Debug.Log("游戏第1部分");
            SetTargetColors(SelectColor.red);
            RianbowLight(RianbowType.red);
            NpcSpeckAnim(1, null, () =>
            {
                NpcIdleAnim();
            });

        }

        void GamePart_2()
        {
            Debug.Log("游戏第2部分");
            SetTargetColors(SelectColor.red, SelectColor.yellow);
            RianbowLight(RianbowType.orange);
            NpcSpeckAnim(2, null, () =>
            {
                NpcIdleAnim();
            });

        }

        void GamePart_3()
        {
            Debug.Log("游戏第3部分");
            SetTargetColors(SelectColor.yellow);
            RianbowLight(RianbowType.yellow);
            NpcSpeckAnim(3, null, () =>
            {
                NpcIdleAnim();
            });
        }

        void GamePart_4()
        {
            Debug.Log("游戏第4部分");
            SetTargetColors(SelectColor.yellow, SelectColor.blue);
            RianbowLight(RianbowType.green);
            NpcSpeckAnim(4, null, () =>
            {
                NpcIdleAnim();
            });
        }

        void GamePart_5()
        {
            Debug.Log("游戏第5部分");
            SetTargetColors(SelectColor.blue, SelectColor.blue,SelectColor.yellow);
            RianbowLight(RianbowType.cyan);
            NpcSpeckAnim(5, null, () =>
            {
                NpcIdleAnim();
            });
        }

        void GamePart_6()
        {
            Debug.Log("游戏第6部分");
            SetTargetColors(SelectColor.blue);
            RianbowLight(RianbowType.blue);
            NpcSpeckAnim(6, null, () =>
            {
                NpcIdleAnim();
            });
        }

        void GamePart_7()
        {
            Debug.Log("游戏第7部分");
            SetTargetColors(SelectColor.red,SelectColor.blue);
            RianbowLight(RianbowType.purple);
            NpcSpeckAnim(7, null, () =>
            {
                NpcIdleAnim();
            });
        }

        void GamePart_8()
        {
            Debug.Log("游戏第8部分");
            SetTargetColors(SelectColor.red, SelectColor.yellow,SelectColor.blue);
            RianbowLight(RianbowType.brownness);
            NpcSpeckAnim(8, null, () =>
            {
                NpcIdleAnim();
            });

        }

        void GameEndPart()
        {
            Debug.Log("游戏结束");
            Shield.SetActive(true);
            Mask.SetActive(true);
            EndPanel.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            SpineManager.instance.DoAnimation(EndPanel, "animation", false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                SpineManager.instance.DoAnimation(EndPanel, "idle");
            });
        }


        //彩虹动画部分
        void ResetRianBowLight()
        {
            for (int i = 0; i < RainbowLightAnims.childCount; i++)
            {
                RainbowLightAnims.GetChild(i).gameObject.SetActive(false);
            }
        }

        //彩虹框高亮动画
        void RianbowLight(RianbowType rianbowType)
        {
            Debug.Log("播放彩虹动画");
            ResetRianBowLight();
            CurRianBowColor = RainbowColorAndStarAnims.GetChild((int)rianbowType).gameObject;
            var curRianow = RainbowLightAnims.transform.GetChild((int)rianbowType).gameObject;
            curRianow.SetActive(true);
            SpineManager.instance.DoAnimation(curRianow, rianbowType.ToString() + "_l");

        }

        void ResetColorIconAnim()
        {
            //RightIcon.SetActive(false);
            for (int i = 0; i < ContentOfSUIBar.childCount; i++)
            {
                ContentOfSUIBar.GetChild(i).gameObject.SetActive(false);
            }
        }

        void SetTargetColors(params SelectColor[] colors)
        {
            TargetColors.Clear();
            for (int i = 0; i < colors.Length; i++)
            {
                TargetColors.Add(colors[i]);
            }
            //重置所有ColorIcon
            ResetColorIcon();
            SpineManager.instance.DoAnimation(SUIBar, "s_uibar", false, () =>
            {
                ColorIconAnim(TargetColors);
            });
        }

        #region  设置小图标颜色动画
        void ResetColorIcon()
        {
            for (int i = 0; i < ContentOfSUIBar.childCount; i++)
            {
                var icon = ContentOfSUIBar.GetChild(i).gameObject;
                icon.SetActive(false);
            }
        }

        void ColorIconAnim(List<SelectColor> colors)
        {
            if (colors.Count == 1)
            {
                var icon = ContentOfSUIBar.GetChild(0).gameObject;
                icon.SetActive(true);
                SpineManager.instance.DoAnimation(icon, "s_" + colors[0].ToString(), false);
            }
            else if (colors.Count == 2)
            {
                int colorIndex = 0;
                for (int i = 0; colorIndex < colors.Count; i += 2)
                {
                    var icon = ContentOfSUIBar.GetChild(i).gameObject;
                    icon.SetActive(true);
                    SpineManager.instance.DoAnimation(icon, "s_" + colors[colorIndex].ToString(), false);
                    colorIndex++;
                }

                var plusIcon = ContentOfSUIBar.GetChild(1).gameObject;
                plusIcon.SetActive(true);
                SpineManager.instance.DoAnimation(plusIcon, "+", false);

            }
            else if (colors.Count == 3)
            {
                int colorIndex = 0;
                for (int i = 0; colorIndex < colors.Count; i += 2)
                {
                    var icon = ContentOfSUIBar.GetChild(i).gameObject;
                    icon.SetActive(true);
                    SpineManager.instance.DoAnimation(icon, "s_" + colors[colorIndex].ToString(), false);
                    colorIndex++;
                }

                for (int i = 1; i < 4; i += 2)
                {
                    var plusIcon = ContentOfSUIBar.GetChild(i).gameObject;
                    plusIcon.SetActive(true);
                    SpineManager.instance.DoAnimation(plusIcon, "+", false);

                }
            }

        }
        #endregion

        //颜色按钮选中回调
        private void ClickColorBtn(int index)
        {
            Debug.Log("点击了按钮");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SelectColor color = (SelectColor)index;
            var btnAnim = UIBar.transform.GetChild(index).Find("Anim").gameObject;
            btnAnim.SetActive(true);
            btnAnim.transform.parent.GetComponent<Image>().enabled = false;
            SpineManager.instance.DoAnimation(btnAnim, color.ToString() + "_s", false,()=>{
                btnAnim.transform.parent.GetComponent<Image>().enabled = true;
            });

            SelectColors.Add(color);

            if (SelectColors.Count < TargetColors.Count)
            {
                Debug.Log("没选完");
                if (SelectColors.Count > 0)
                {
                    var lastColor = SelectColors[SelectColors.Count - 1];
                    var anim = UIBar.transform.Find(lastColor.ToString()).Find("Anim").gameObject;
                    anim.transform.parent.GetComponent<Image>().enabled = false;
                    SpineManager.instance.DoAnimation(anim, lastColor.ToString() + "_s", false, () =>
                    {
                        anim.transform.parent.GetComponent<Image>().enabled = true;
                    });
                }
            }
            else if (SelectColors.Count == TargetColors.Count)
            {
                //选完
                Debug.Log("选完了");
                bool result = GetRightResult();
                if (result)
                {
                    //全选对了
                    Debug.Log("选对了");
                    SetRianBowColorFill();
                }
                else
                {
                    //选错了
                    ResetColorBtn();
                    NpcUnHappyAnim();
                }
            }

        }

        void SetRianBowColorFill()
        {
            Shield.SetActive(true);
            //RightIcon.SetActive(true);
            IsFillingRianBow = true;
            // SpineManager.instance.DoAnimation(RightIcon, "ui_confirm", false, () =>
            // {
            //     IsFillingRianBow = true;
            // });

        }

        void ResetRianBowColor()
        {
            for (int i = 0; i < RainbowColorAndStarAnims.childCount; i++)
            {
                var rianBow = RainbowColorAndStarAnims.GetChild(i).gameObject;
                rianBow.GetComponent<Image>().fillAmount = 0f;
                rianBow.transform.Find("Star").gameObject.SetActive(false);
            }
        }


        void ResetColorBtn()
        {
            //清空所有选择，表示重选
            Shield.SetActive(true);
            SelectColors.Clear();
            for (int i = 0; i < UIBar.transform.childCount; i++)
            {
                UIBar.transform.GetChild(i).GetComponent<Image>().enabled = true;
                var animString = UIBar.transform.GetChild(i).name;
                var animGameObj = UIBar.transform.GetChild(i).Find("Anim").gameObject;
                SpineManager.instance.DoAnimation(animGameObj, animString, false);
            }
            Shield.SetActive(false);
            NpcIdleAnim();
        }


        private bool GetRightResult()
        {
            bool isRight = false;
            var targetColors = new List<SelectColor>(TargetColors);
            for (int i = 0; i < SelectColors.Count; i++)
            {
                if (targetColors.Contains(SelectColors[i]))
                {
                    targetColors.Remove(SelectColors[i]);
                }
            }

            isRight = targetColors.Count == 0 ? true : false;
            return isRight;
        }
    }
}
