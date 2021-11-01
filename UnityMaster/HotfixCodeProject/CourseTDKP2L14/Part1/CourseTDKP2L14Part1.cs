using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Spine;
using DG.Tweening;


namespace ILFramework.HotClass
{
    public class CourseTDKP2L14Part1
    {
        GameObject curGo, CoverMask;
        GameObject npc, CameraLight, Ribbon, BgAnim, Bg;
        Transform Buttom, DeepCar, LightCar, JumpPlatform, Btns, Stage;
        Button DeepBtn, LightBtn;
        List<string> DeepAnims;
        List<string> LightAnims;
        List<string> curAnims;
        string curAnim, targetType;
        float Origin_Deep_Car_JumpAnim_X, Origin_Light_Car_JumpAnim_X;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");
            CoverMask = Buttom.Find("CoverMask").gameObject;
            npc = Buttom.Find("Npc").gameObject;
            BgAnim = Buttom.Find("BgAnim").gameObject;
            Bg = Buttom.Find("Bg").gameObject;
            Stage = Buttom.Find("Stage");
            DeepCar = Stage.Find("DeepCar");
            LightCar = Stage.Find("LightCar");
            Ribbon = Stage.Find("Ribbon").gameObject;
            JumpPlatform = Stage.Find("Jumpplatform");
            Btns = Buttom.Find("Btns");
            CameraLight = Stage.Find("CameraLight").gameObject;
            DeepBtn = Btns.Find("DeepBtn").GetComponent<Button>();
            LightBtn = Btns.Find("LightBtn").GetComponent<Button>();
            DeepAnims = new List<string>();
            LightAnims = new List<string>();
            InitGame();
        }

        private void InitGame()
        {
            //PlayAnimation(BgAnim,"idle",false);

            Bg.SetActive(true);
            LogicManager.instance.ShowReplayBtn(false);
            ResetStageObjs();
            CoverMask.SetActive(false);
            CameraLight.SetActive(false);
            Ribbon.SetActive(false);
            JumpPlatform.Find("Normal").gameObject.SetActive(true);
            JumpPlatform.Find("Anim").gameObject.SetActive(false);

            DeepBtn.onClick.AddListener(ClickDeepBtn);
            LightBtn.onClick.AddListener(ClickLightBtn);

            DeepAnims.Clear();
            LightAnims.Clear();

            DeepAnims.AddRange(new string[] { "deep_1", "deep_2", "deep_3", "deep_4" });
            LightAnims.AddRange(new string[] { "light_1", "light_2", "light_3", "light_4" });

            SoundManager.instance.BgSoundPart2();
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, () =>
            {
                CoverMask.SetActive(true);
            }, () =>
            {
                CameraLight.SetActive(true);
                SetNewMonster();
            });
        }

        void ResetStageObjs()
        {
            Origin_Deep_Car_JumpAnim_X = -621f;
            Origin_Light_Car_JumpAnim_X = 660f;

            for (int i = 0; i < DeepCar.childCount; i++)
            {
                if (DeepCar.GetChild(i).name == "JumpAnim")
                {
                    DeepCar.GetChild(i).localPosition = new Vector3(Origin_Deep_Car_JumpAnim_X, -263f, 0f);
                }
                if (i == 1 || i == DeepCar.childCount - 2)
                {
                    DeepCar.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    DeepCar.GetChild(i).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < LightCar.childCount; i++)
            {
                if (LightCar.GetChild(i).name == "JumpAnim")
                {
                    LightCar.GetChild(i).localPosition = new Vector3(Origin_Light_Car_JumpAnim_X, -263f, 0f);
                }
                if (i == 1 || i == LightCar.childCount - 2)
                {
                    LightCar.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    LightCar.GetChild(i).gameObject.SetActive(false);
                }
            }
            JumpPlatform.Find("Normal").gameObject.SetActive(true);
            JumpPlatform.Find("Anim").gameObject.SetActive(false);

        }


        private void ClickLightBtn()
        {
            CommonBtnAnim(LightCar, "light");
            SetRightTarget(LightCar, "light", () =>
            {

            });
        }

        private void ClickDeepBtn()
        {
            CommonBtnAnim(DeepCar, "deep");
            SetRightTarget(DeepCar, "deep", () =>
            {

            });
        }

        void CommonBtnAnim(Transform ChooseCar, string type)
        {
            var car_1 = ChooseCar.Find("Car_1").gameObject;
            var car_2 = ChooseCar.Find("Car_2").gameObject;
            var behindAnim = ChooseCar.Find("BehindAnim").gameObject;
            var frontAnim = ChooseCar.Find("FrontAnim").gameObject;

            car_1.SetActive(false);
            car_2.SetActive(false);
            behindAnim.SetActive(true);
            frontAnim.SetActive(true);
            SpineManager.instance.DoAnimation(behindAnim, "car_" + type + "3", false);
            SpineManager.instance.DoAnimation(frontAnim, "car_" + type + "2", false, () =>
              {
                  behindAnim.SetActive(false);
                  frontAnim.SetActive(false);
                  car_1.SetActive(true);
                  car_2.SetActive(true);
              });
        }

        void SetNewMonster(Action endCallback = null)
        {
            curAnim = GetNewAnimName();
            if (curAnim == "")
            {
                Debug.Log("游戏结束");
                LogicManager.instance.ShowReplayBtn(true);
                LogicManager.instance.SetReplayEvent(InitGame);
            }
            else
            {
                PlayAnimation(BgAnim, "animation", false, 0, () =>
                {
                    PlayAnimation(BgAnim, "idle", true);
                });

                CameraLight.SetActive(true);
                PlayAnimation(CameraLight, "camera", false, 0f, () =>
                {

                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 0, null, () =>
                    {
                        var AnimNameArray = curAnim.Split('_');
                        targetType = "";
                        targetType = AnimNameArray[0];
                        var upAnim = JumpPlatform.Find("Anim").gameObject;
                        var normal = JumpPlatform.Find("Normal").gameObject;
                        upAnim.SetActive(true);
                        normal.SetActive(false);
                        PlayAnimation(upAnim, AnimNameArray[0] + AnimNameArray[1] + "_up", false, 0f, () =>
                        {
                            CameraLight.SetActive(false);
                            CoverMask.SetActive(false);
                            normal.SetActive(true);
                            PlayAnimation(upAnim, AnimNameArray[0] + AnimNameArray[1] + "_idle", true);
                            endCallback?.Invoke();
                        });
                    });

                });
            }
        }

        string GetNewAnimName()
        {
            int i = UnityEngine.Random.Range(0, 2);
            curAnims = i == 0 ? DeepAnims : LightAnims;
            if (curAnims.Count > 0)
            {
                int j = UnityEngine.Random.Range(0, curAnims.Count);
                var value = curAnims[j];
                return value;
            }
            else
            {
                if (curAnims == DeepAnims)
                {
                    curAnims = LightAnims;
                }
                else
                {
                    curAnims = DeepAnims;
                }
                if (DeepAnims.Count == 0 && LightAnims.Count == 0)
                {
                    return "";
                }
                else
                {
                    int j = UnityEngine.Random.Range(0, curAnims.Count);
                    var value = curAnims[j];
                    return value;
                }
            }

        }

        void SetRightTarget(Transform car, string carType, Action endCallbback = null)
        {
            if (targetType == "")
                return;
            CoverMask.SetActive(true);
            var AnimNameArray = curAnim.Split('_');
            if (carType != targetType)
            {
                var upAnim = JumpPlatform.Find("Anim").gameObject;
                PlayAnimation(upAnim, AnimNameArray[0] + AnimNameArray[1] + "_error", false, 0f, () =>
                {
                    upAnim.SetActive(false);
                    CoverMask.SetActive(false);
                    endCallbback?.Invoke();
                    SetNewMonster();
                });
            }
            else
            {
                curAnims.Remove(curAnim);
                var rightCar = targetType == "deep" ? DeepCar : LightCar;
                var upAnim = JumpPlatform.Find("Anim").gameObject;
                upAnim.SetActive(false);
                var targetName = AnimNameArray[0] + AnimNameArray[1];
                var jumpAnim = rightCar.Find("JumpAnim").gameObject;
                jumpAnim.SetActive(true);
                Ribbon.SetActive(true);
                PlayAnimation(Ribbon, "caidai", true);
                PlayAnimation(jumpAnim, targetName + "_right", false, 0f, () =>
                {
                    //PlayCarAndMonsterShake(rightCar, jumpAnim, targetName);
                    jumpAnim.SetActive(false);
                });

                //需要同时播放
                var shakeAnim = rightCar.Find("ShakeAnim").gameObject;
               
                PlayCarAndMonsterShake(rightCar, shakeAnim, targetName);
            }
        }

        void PlayCarAndMonsterShake(Transform rightCar, GameObject shakeAnim, string targetName)
        {
            shakeAnim.SetActive(true);
            PlayAnimation(shakeAnim, targetName + "_right", false, 0f);

            // //self shake
            // PlayAnimation(shakeAnim, targetName + "_incar", false);

            string type = rightCar == DeepCar ? "deep" : "light";
            // man shake
            for (int i = 1; i <= 4; i++)
            {
                var monster = rightCar.Find(type + i).gameObject;
                if (monster.activeSelf)
                {
                    PlayAnimation(monster, monster.name + "_incar", false);
                }
            }

            //car shake
            var car1 = rightCar.Find("Car_1").gameObject;
            var car2 = rightCar.Find("Car_2").gameObject;
            var behindAnim = rightCar.Find("BehindAnim").gameObject;
            var frontAnim = rightCar.Find("FrontAnim").gameObject;
            car1.SetActive(false);
            car2.SetActive(false);

            PlayAnimation(behindAnim, "car_" + type + 1, false, 0f);
            PlayAnimation(frontAnim, "car_" + type, false, 0f, () =>
            {
                car1.SetActive(true);
                car2.SetActive(true);
                behindAnim.SetActive(false);
                frontAnim.SetActive(false);

                var targetAnim = rightCar.Find(targetName).gameObject;
                shakeAnim.transform.DOLocalMoveX(targetAnim.transform.localPosition.x, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    targetAnim.SetActive(true);
                    PlayAnimation(targetAnim, targetName + "_incar", false, 42f, () =>
                    {
                        shakeAnim.SetActive(false);
                        if (type == "deep")
                        {
                            shakeAnim.transform.localPosition = new Vector3(Origin_Deep_Car_JumpAnim_X, -263f, 0f);
                        }
                        else
                        {
                            shakeAnim.transform.localPosition = new Vector3(Origin_Light_Car_JumpAnim_X, -263f, 0f);
                        }
                        Ribbon.SetActive(false);
                        SetNewMonster();
                    });
                });
            });
        }

        void PlayAnimation(GameObject animObj, string animName, bool isLoop, float startFrame = 0f, Action endCallBack = null)
        {
            var skt = animObj.GetComponent<SkeletonGraphic>();
            skt.AnimationState.SetEmptyAnimation(0, 0f);
            animObj.SetActive(true);
            var track = skt.AnimationState.SetAnimation(0, animName, isLoop);
            track.TrackTime = startFrame / 30f;
            track.Complete += (TrackEntry trackEntry) =>
            {
                endCallBack?.Invoke();
            };
        }

    }
}
