using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Spine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class CourseTDG4P1L11Part1
    {
        GameObject curGo;
        Transform Buttom;
        Transform SwicthAnims;
        GameObject SwitchAnimPrafeb;
        GameObject SwitchAnimPrafeb_1;
        //GameObject SwitchAnimGameObject;
        Transform Anims;
        Transform Btns;
        GameObject CoverMask;
        List<GameObject> AnimArray;
        int LastAnimIndex;
        int CurAnimIndex;

        GameObject Npc;
        Transform Fonts;
        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Buttom = curTrans.Find("Content/Buttom");
            SwicthAnims = Buttom.Find("SwitchAnim");
            SwitchAnimPrafeb = Buttom.Find("SwitchAnimNext").gameObject;
            SwitchAnimPrafeb_1 = SwicthAnims.Find("SwitchAnimNext_1").gameObject;
            Anims = Buttom.Find("Anims");
            Btns = Buttom.Find("Btns");
            CoverMask = curTrans.Find("Content/CoverMask").gameObject;
            Npc = curTrans.Find("Content/Npc").gameObject;
            Fonts = Buttom.Find("Fonts");
            mono = curGo.GetComponent<MonoBehaviour>();

            InitGame();
        }

        void InitGame()
        {
            AnimArray = new List<GameObject>();
            CoverMask.SetActive(false);
            //SwitchAnimNext.SetActive(false);
            SwitchAnimPrafeb_1.SetActive(true);
            LastAnimIndex = 0;
            CurAnimIndex = 0;
            ResetAnims();
            DisableAnims();

            PlayRandomBG();
            SwitchAnimPrafeb_1.SetActive(false);

            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, () =>
            {
                CoverMask.SetActive(true);
            }, () =>
            {
                CoverMask.SetActive(false);
                // SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
                // {
                //     CoverMask.SetActive(false);
                // });
            });
        }

        private void PlayRandomBG()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            int index = UnityEngine.Random.Range(0, 2);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, true);
            if(index == 1)
                SoundManager.instance.bgmSource.volume = 0.2f;
            else
                SoundManager.instance.bgmSource.volume = 0.3f;
        }

        void ResetAnims()
        {
            for (int i = 0; i < Anims.childCount; i++)
            {
                var animObj = Anims.GetChild(i).gameObject;
                animObj.SetActive(false);
                AnimArray.Add(animObj);
            }

            for (int i = 1; i < Btns.childCount; i++)
            {
                var btnObj = Btns.GetChild(i).gameObject;
                var btnImg = btnObj.transform.Find("BtnImg").gameObject;
                var animObj = btnObj.transform.Find("Anim").gameObject;
                btnImg.SetActive(false);
                animObj.SetActive(true);
                SpineManager.instance.DoAnimation(animObj, "animation" + i + "_idle", false);
                var btnAction = btnObj.GetComponent<ILObject3DAction>();
                btnAction.index = i;
                btnAction.OnPointDownLua = OnBtnPointDown;
            }

            // if (SwitchAnimGameObject != null)
            // {
            //     GameObject.Destroy(SwitchAnimGameObject);
            // }
            for (int i = 0; i < Fonts.childCount; i++)
            {
                var font = Fonts.GetChild(i).gameObject;
                try
                {
                    var ske = font.GetComponent<SkeletonGraphic>();
                    ske.AnimationState.ClearTrack(0);
                }
                catch (System.Exception)
                {
                    continue;
                }
                font.SetActive(false);
            }

        }

        private void OnBtnPointDown(int index)
        {
            CurAnimIndex = index;
            var btnObj = Btns.GetChild(index);
            // var BtnImg = btnObj.Find("BtnImg").gameObject;
            var Anim = btnObj.Find("Anim").gameObject;
            // BtnImg.SetActive(false);
            // Anim.SetActive(true);

            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

            SpineManager.instance.DoAnimation(Anim, "animation" + index, false, () =>
            {
                // BtnImg.SetActive(true);
                // Anim.SetActive(false);
                CoverMask.SetActive(true);
                SpineManager.instance.DoAnimation(Anim, "animation" + index + "_idle", false);
                PlayAnimation();
            });
        }

        void PlayAnimation()
        {
            // Debug.LogFormat("curIndex : {0},lastIndex : {1}", CurAnimIndex, LastAnimIndex);

            //切换动画播放完
            if ((CurAnimIndex == 1 && LastAnimIndex == 0) ||
                (CurAnimIndex == 2 && LastAnimIndex == 0) ||
                (CurAnimIndex == 3 && LastAnimIndex == 0)
            )
            {
                //刚开始游戏
                PlayAnim(CurAnimIndex);
            }
            else if (CurAnimIndex == LastAnimIndex)
            {
                PlayFontAnim_Out(CurAnimIndex, () =>
                {
                    //自己再次重复播放
                    PlayAnim(CurAnimIndex);
                });

            }
            else
            {
                PlayFontAnim_Out(LastAnimIndex, () =>
                {
                    PlaySwitchAnim(LastAnimIndex + "+" + CurAnimIndex, () =>
                    {
                        LastAnimIndex = CurAnimIndex;
                        PlayFontAnim_In(CurAnimIndex);
                        PlayIdleAnim(CurAnimIndex);
                    });
                });
            }
            //PlayAnim(CurAnimIndex);
        }

        void PlaySwitchAnim(string AnimName, Action EndCallBack = null)
        {
            DisableAnims();
            SwitchAnimPrafeb_1.SetActive(true);

            SkeletonAnimation skt = SwitchAnimPrafeb_1.GetComponent<SkeletonAnimation>();
            var track = skt.AnimationState.SetAnimation(0, AnimName, false);
            track.TrackTime = 5f/30f;
            track.Complete += (TrackEntry trackEntry) =>
            {
                SwitchAnimPrafeb_1.SetActive(false);
                if (EndCallBack != null)
                    EndCallBack.Invoke();
            };
            // SpineManager.instance.DoAnimation(SwitchAnimPrafeb_1, AnimName, false, () =>
            // {
            //     SwitchAnimPrafeb_1.SetActive(false);
            //     if (EndCallBack != null)
            //         EndCallBack.Invoke();
            // });

            //mono.StartCoroutine(SwitchCallback());
        }


        void PlayAnim(int index)
        {
            DisableAnims();
            AnimArray[index - 1].SetActive(true);
            var skt = AnimArray[index - 1].GetComponent<SkeletonGraphic>();
            TrackEntry track = skt.AnimationState.SetAnimation(0, "animation", false);
            track.TrackTime = 0f;
            // mono.StopCoroutine(PlayIdAnim_1(track.TrackEnd/30f,index));

            LastAnimIndex = index;
            PlayFontAnim_In(index);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index - 1, true);
            CoverMask.SetActive(false);
            // SpineManager.instance.DoAnimation(AnimArray[index - 1], "animation", false, () =>
            // {
            //     PlayIdleAnim(index);
            //     LastAnimIndex = index;
            //     PlayFontAnim_In(index);
            // });
        }

        void PlayIdleAnim(int index)
        {
            DisableAnims();
            // SwitchAnimNext.SetActive(false);
            AnimArray[index - 1].SetActive(true);
            //AnimArray[index - 1].GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimations(0);
            //AnimArray[index - 1].GetComponent<SkeletonGraphic>().Update(0);
            SpineManager.instance.DoAnimation(AnimArray[index - 1], "idle", false, () =>
            {
                CoverMask.SetActive(false);
            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index - 1, true);

        }

        void DisableAnims()
        {
            for (int i = 0; i < AnimArray.Count; i++)
            {
                AnimArray[i].SetActive(false);
            }
        }

        void PlayFontAnim_In(int index)
        {
            for (int i = 0; i < Fonts.childCount; i++)
            {
                Fonts.GetChild(i).gameObject.SetActive(false);
            }

            var curfont = Fonts.GetChild(index - 1).gameObject;


            curfont.SetActive(true);
            // var ske = curfont.GetComponent<SkeletonGraphic>();
            // ske.AnimationState.ClearTrack(0);

            var animName = "";
            var idleAninName = "";
            if (index == 1)
            {
                animName = "PY_I";
                idleAninName = "PY_idle";
            }
            else if (index == 2)
            {
                animName = "GY_I";
                idleAninName = "GY_idle";
            }
            else if (index == 3)
            {
                animName = "SY_I";
                idleAninName = "SY_idle";
            }
            SpineManager.instance.DoAnimation(curfont, animName, false, () =>
            {
                SpineManager.instance.DoAnimation(curfont, idleAninName);
            });
        }

        void PlayFontAnim_Out(int index, Action EndCallBack = null)
        {
            for (int i = 0; i < Fonts.childCount; i++)
            {
                Fonts.GetChild(i).gameObject.SetActive(false);
            }

            var curfont = Fonts.GetChild(index - 1).gameObject;

            curfont.SetActive(true);
            // var ske = curfont.GetComponent<SkeletonGraphic>();
            // ske.AnimationState.ClearTrack(0);

            var animName = "";
            if (index == 1)
            {
                animName = "PY_O";
            }
            else if (index == 2)
            {
                animName = "GY_O";
            }
            else if (index == 3)
            {
                animName = "SY_O";
            }


            SpineManager.instance.DoAnimation(curfont, animName, false, () =>
            {
                if (EndCallBack != null)
                {
                    EndCallBack.Invoke();
                }
            });
        }


    }
}
