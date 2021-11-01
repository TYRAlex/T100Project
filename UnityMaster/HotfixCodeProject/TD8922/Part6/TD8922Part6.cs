using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD8922Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private Transform groups;
        private Transform groups2;

        private GameObject xx;
        private GameObject sx;
        private GameObject xx2;
        private GameObject sx2;

        private Transform zhuangImg;

        private GameObject spineMask;
        private Transform scorePanel;
        private int totalBlood = 4;

        private GameObject powerC;
        private GameObject endAnim;
        int random = 0;
        int random2 = 0;

        private GameObject objGroup;
        private GameObject objGroup2;

        string[] ObjNums;

       private int Timer = 0;


        Vector3[] temVector3;
        Vector3[] tem2Vector3;

        private Transform powerCPos;
        private Transform groupInPos;
        private Transform group2InPos;

        private string sz;
        bool isPlaying = false;
        private int flag = 0;
        bool isDeviling = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            groups = curTrans.Find("groups");
            groups2 = curTrans.Find("groups2");
            groupInPos = curTrans.Find("groupInPos");
            group2InPos = curTrans.Find("group2InPos");

            xx = curTrans.Find("xx").gameObject;
            xx.SetActive(false);
            sx = curTrans.Find("sx").gameObject;
            xx2 = curTrans.Find("xx2").gameObject;
            xx2.SetActive(false);
            sx2 = curTrans.Find("sx2").gameObject;

            zhuangImg = curTrans.Find("zhuangImg");
            for (int i = 0, len = zhuangImg.childCount; i < len; i++)
            {
                zhuangImg.GetChild(i).gameObject.SetActive(false);
            }
            scorePanel = curTrans.Find("scorePanel");

            powerC = curTrans.Find("powerC").gameObject;
            powerCPos = curTrans.Find("powerCPos");
            powerC.transform.position = powerCPos.position;
            powerC.SetActive(false);
            endAnim = curTrans.Find("endAnim").gameObject;
            SpineManager.instance.DoAnimation(endAnim, "kong", false);
            spineMask = curTrans.Find("spineMask").gameObject;
            spineMask.SetActive(false);
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            sz = "6-12-z";

            for (int i = 0; i < groups.childCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Util.AddBtnClick(groups.GetChild(i).GetChild(j).gameObject, onClickBtn);
                }
            }
            for (int i = 0; i < groups2.childCount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Util.AddBtnClick(groups2.GetChild(i).GetChild(j).gameObject, onClickBtn);
                }

            }
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        private void onClickBtn(GameObject obj)
        {
            if (Timer > 4)
                return;
            if (isPlaying)
                return;
            isPlaying = true;
            SpineManager.instance.SetFreeze(obj, false);

            bool isRight = false;
            int temNum = 0;
            for (int i = 0, len = ObjNums.Length; i < len; i++)
            {
                if (obj.name == ObjNums[i])
                {
                    isRight = true;
                }
            }

            if (isRight)
            {
                Timer++;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10));
                SpineManager.instance.DoAnimation(obj, obj.name, false);
                if (int.Parse(obj.name) < 17)
                {
                    SpineManager.instance.SetFreeze(objGroup.transform.GetChild(objGroup.transform.childCount - 1).gameObject, false);
                    SpineManager.instance.DoAnimation(objGroup.transform.GetChild(objGroup.transform.childCount - 1).gameObject, objGroup.transform.GetChild(objGroup.transform.childCount - 1).name, false);
                    xx.SetActive(false);
                    for (int i = 0; i < objGroup.transform.childCount; i++)
                    {
                        if (obj.name == objGroup.transform.GetChild(i).name)
                        {
                            temNum = i + 1;
                        }
                    }
                    if (temNum <= 0)
                    {
                        SpineManager.instance.DoAnimation(sx, "xj", false, () =>
                        {

                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(sx, "x" + temNum, false, () =>
                        {
                            for (int i = 0, len = objGroup.transform.childCount - 1; i < len; i++)
                            {
                                if (obj.name != objGroup.transform.GetChild(i).name)
                                {
                                    objGroup.transform.GetChild(i).gameObject.SetActive(false);
                                }
                            }
                        });
                    }
                    spineMask.SetActive(true);
                    mono.StartCoroutine(PlayElements(3, () =>
                    {
                        objGroup.SetActive(false); objGroup.transform.SetParent(groupInPos); SpineManager.instance.DoAnimation(sx, "xj", false, () =>
                        {
                            playGroupSpine();
                        });
                    }));
                }
                else
                {
                    SpineManager.instance.SetFreeze(objGroup2.transform.GetChild(objGroup2.transform.childCount - 1).gameObject, false);
                    SpineManager.instance.DoAnimation(objGroup2.transform.GetChild(objGroup2.transform.childCount - 1).gameObject, objGroup2.transform.GetChild(objGroup2.transform.childCount - 1).name, false);
                    xx2.SetActive(false);
                    for (int i = 0; i < objGroup2.transform.childCount; i++)
                    {
                        if (obj.name == objGroup2.transform.GetChild(i).name)
                        {
                            temNum = i + 4;
                        }
                    }
                    if (temNum <= 0)
                    {
                        SpineManager.instance.DoAnimation(sx2, "xj2", false, () =>
                        {

                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(sx2, "x" + temNum, false, () =>
                        {
                            for (int i = 0, len = objGroup2.transform.childCount - 1; i < len; i++)
                            {
                                if (obj.name != objGroup2.transform.GetChild(i).name)
                                {
                                    objGroup2.transform.GetChild(i).gameObject.SetActive(false);
                                }
                            }
                        });
                    }
                    spineMask.SetActive(true);
                    mono.StartCoroutine(PlayElements(3, () =>
                    {
                        objGroup2.SetActive(false); objGroup2.transform.SetParent(group2InPos); SpineManager.instance.DoAnimation(sx2, "xj2", false, () =>
                        {
                            playGroup2Spine();
                        }); 
                    }));
                }
                spineMask.SetActive(true);
                
                mono.StartCoroutine(PlayElements(0.3f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                    SpineManager.instance.DoAnimation(zhuangImg.GetChild(Timer - 1).gameObject, "si", false, () =>
                    {
                        scorePanel.GetChild(totalBlood - Timer).gameObject.SetActive(false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                        SpineManager.instance.DoAnimation(powerC, "animation" + Timer * 2, false, () =>
                        {
                            SpineManager.instance.DoAnimation(powerC, "animation" + (Timer * 2 + 1), false, () =>
                            {
                                if (Timer >= 4)
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
                                    isDeviling = false;                                  
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
                                    SpineManager.instance.DoAnimation(endAnim, "animation", false, () =>
                                    {
                                        mono.StartCoroutine(PlayElements(1, () =>
                                        {
                                            playSuccessSpine();
                                        }));
                                    });
                                    // endAnim.SetActive(true);
                                    //SpineManager.instance.DoAnimation(endAnim, "animation", false, () =>
                                    //{
                                    //    SpineManager.instance.DoAnimation(endAnim, "animation2", false, () =>
                                    //    {
                                    //        mono.StartCoroutine(PlayElements(1, () =>
                                    //        {
                                    //            mask.SetActive(true); successPanel.SetActive(true); caidai.SetActive(true);
                                    //            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);

                                    //            SpineManager.instance.DoAnimation(successPanel, "c2", false, () =>
                                    //            {
                                    //                SpineManager.instance.DoAnimation(caidai, "animation", false);
                                    //                SpineManager.instance.DoAnimation(successPanel, "c", true);
                                    //            });
                                    //        }));
                                    //    });
                                    //});
                                }

                            });
                        });

                    });
                }));


            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), () => { SpineManager.instance.DoAnimation(obj, obj.name, false); }, () => { isPlaying = false; }));

            }
        }

        IEnumerator PlayElements(float index, Action ac = null)
        {
            yield return new WaitForSeconds(index);
            ac?.Invoke();
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {              
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        SpineManager.instance.DoAnimation(powerC, "animation", false);
                        xx.SetActive(false);
                        xx2.SetActive(false);
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                        for (int i = 0; i < zhuangImg.childCount; i++)
                        {
                            mono.StartCoroutine(PlayDevils(i));
                        }
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { GameInit(); SpineManager.instance.DoAnimation(endAnim, "animation2", false); switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
                }

            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            Timer = 0;
            spineMask.SetActive(true);
            isDeviling = false;
            if (groups.childCount < 4)
            {
                for (int i = 0, len = groupInPos.childCount; i < len; i++)
                {
                    groupInPos.GetChild(0).SetParent(groups);
                }
            }
            if (groups2.childCount < 4)
            {
                for (int i = 0, len = group2InPos.childCount; i < len; i++)
                {
                    group2InPos.GetChild(0).SetParent(groups2);
                }
            }

            for (int i = 0; i < groups.childCount; i++)
            {
                groups.GetChild(i).gameObject.SetActive(false);
                for (int j = 0; j < 3; j++)
                {
                    groups.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
                groups.GetChild(i).position = curTrans.Find("groupStart").position;
            }
            for (int i = 0; i < groups2.childCount; i++)
            {
                groups2.GetChild(i).gameObject.SetActive(false);
                for (int j = 0; j < 3; j++)
                {
                    groups2.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
                groups2.GetChild(i).position = curTrans.Find("groupStart").position;
            }
            ObjNums = new string[8] { "9", "6", "11", "8", "21", "30", "23", "24" };


            temVector3 = new Vector3[2] { curTrans.Find("groupStart").position, groupInPos.position };
            tem2Vector3 = new Vector3[2] { curTrans.Find("groupStart").position, group2InPos.position };
          
            totalBlood = scorePanel.childCount;
            for (int i = 0; i < totalBlood; i++)
            {
                scorePanel.GetChild(i).gameObject.SetActive(true);
            }
            SpineManager.instance.DoAnimation(endAnim, "kong", false);
            sx.SetActive(true);
            sx2.SetActive(true);
            SpineManager.instance.DoAnimation(sx, "xj", false);
            SpineManager.instance.DoAnimation(sx2, "xj2", false);
            playGroupSpine();
            playGroup2Spine();

        }

        void GameStart()
        {
            bd.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }
        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);

            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () =>
                 {
                     mask.SetActive(false);
                     bd.SetActive(false);
                     powerC.SetActive(true);
                     SpineManager.instance.DoAnimation(powerC, "animation", false);
                     powerC.transform.DOMove(new Vector3(bd.transform.position.x, powerCPos.position.y, 0), 1).OnComplete(() =>
                     {
                         for (int i = 0; i < zhuangImg.childCount; i++)
                         {
                             mono.StartCoroutine(PlayDevils(i));
                         }
                     });

                 }));
            }
            talkIndex++;
        }

        IEnumerator PlayDevils(int index)
        {
            yield return new WaitForSeconds(index + index * 0.1f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, false);
            zhuangImg.GetChild(index).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(zhuangImg.GetChild(index).gameObject, "qf", false, () =>
            {
                SpineManager.instance.DoAnimation(zhuangImg.GetChild(index).gameObject, "xem", true);
                if (index == (zhuangImg.childCount - 1))
                {
                    isPlaying = false;
                    spineMask.SetActive(false);
                    isDeviling = true;
                }
            });
        }
        private void playGroupSpine()
        {
            if (groups.transform.childCount <= 0)
                return;
            random = Random.Range(0, groups.transform.childCount);
            objGroup = groups.GetChild(random).gameObject;
            for (int i = 0; i < objGroup.transform.childCount; i++)
            {
                SpineManager.instance.SetFreeze(objGroup.transform.GetChild(i).gameObject, true);
                //temTime = SpineManager.instance.DoAnimation(objGroup.transform.GetChild(i).gameObject, objGroup.transform.GetChild(i).name, false, () =>
                //{
                //    Debug.Log("@objGroup.transform.GetChild(i).name---------------:" + objGroup.transform.GetChild(i).name);
                //    SpineManager.instance.SetFreeze(objGroup.transform.GetChild(i).gameObject, true);
                //});
                //Debug.Log("@temTime:--"+ temTime);
            }
            objGroup.SetActive(true);
            objGroup.transform.DOPath(temVector3, 1.5f).OnComplete(() =>
            {
                xx.SetActive(true);
                isPlaying = false;
                if (isDeviling)
                {
                    spineMask.SetActive(false);
                }               
            });
        }
        private void playGroup2Spine()
        {
            if (groups2.transform.childCount <= 0)
                return;
            random2 = Random.Range(0, groups2.transform.childCount);
            objGroup2 = groups2.GetChild(random2).gameObject;
            for (int i = 0; i < objGroup2.transform.childCount; i++)
            {
                SpineManager.instance.SetFreeze(objGroup2.transform.GetChild(i).gameObject, true);

                //temTime = SpineManager.instance.DoAnimation(objGroup2.transform.GetChild(i).gameObject, objGroup2.transform.GetChild(i).name, false, () =>
                //{
                //    Debug.Log("@objGroup2.transform.GetChild(i).name---------------:" + objGroup2.transform.GetChild(i).name);
                //    SpineManager.instance.SetFreeze(objGroup2.transform.GetChild(i).gameObject, true);
                //});
                //Debug.Log("@temTime2:--" + temTime);
            }
            objGroup2.SetActive(true);
            objGroup2.transform.DOPath(tem2Vector3, 1.5f).OnComplete(() =>
            {
                xx2.SetActive(true);
                isPlaying = false;
                if (isDeviling)
                {
                    spineMask.SetActive(false);
                }
            });

        }


        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            spineMask.SetActive(false);
            isPlaying = false;
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                () =>
                {
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
                });
        }

        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }



    }
}
