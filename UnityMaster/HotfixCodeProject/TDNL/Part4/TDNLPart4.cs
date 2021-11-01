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
    public class TDNLPart4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private Transform groups;
        private Transform groups2;

        private GameObject xx;
        private GameObject sx;
        private GameObject xx2;
        private GameObject sx2;

        private Transform zhuangImg;

        private Transform totalNumPanel;

        private Image numTexture;

        private Text numText;

        private GameObject powerC;
        private GameObject endAnim;
        private GameObject mask;
        private GameObject successPanel;
        private GameObject caidai;
        bool isPlaying = false;


        int random = 0;
        int random2 = 0;

        private GameObject objGroup;
        private GameObject objGroup2;

        string[] ObjNums;

        int Timer = 0;


        Vector3[] temVector3;
        Vector3[] tem2Vector3;


        private Transform powerCPos;
        private Transform groupInPos;
        private Transform group2InPos;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            groups = curTrans.Find("groups");
            groups2 = curTrans.Find("groups2");
            groupInPos = curTrans.Find("groupInPos");
            group2InPos = curTrans.Find("group2InPos");

            xx = curTrans.Find("xx").gameObject;
            xx.SetActive(false);
            sx = curTrans.Find("sx").gameObject;
            sx.SetActive(false);
            xx2 = curTrans.Find("xx2").gameObject;
            xx2.SetActive(false);
            sx2 = curTrans.Find("sx2").gameObject;
            sx2.SetActive(false);

            zhuangImg = curTrans.Find("zhuangImg");
            for (int i = 0, len = zhuangImg.childCount; i < len; i++)
            {
                zhuangImg.GetChild(i).gameObject.SetActive(false);
            }
            totalNumPanel = curTrans.Find("totalNumPanel");
            numTexture = totalNumPanel.Find("Num").GetComponent<Image>();

            numText = totalNumPanel.Find("Text").GetComponent<Text>();

            powerC = curTrans.Find("powerC").gameObject;
            powerCPos = curTrans.Find("powerCPos");
            powerC.transform.position = powerCPos.position;
            powerC.SetActive(false);
            endAnim = curTrans.Find("endAnim").gameObject;
            endAnim.SetActive(false);
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);

            successPanel = curTrans.Find("successPanel").gameObject;
            successPanel.SetActive(false);
            caidai = curTrans.Find("caidai").gameObject;
            caidai.SetActive(false);

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
                    Util.AddBtnClick(groups.GetChild(i).GetChild(j).gameObject, onClickBtn);
                }
                groups.GetChild(i).position = curTrans.Find("groupStart").position;
            }
            for (int i = 0; i < groups2.childCount; i++)
            {
                groups2.GetChild(i).gameObject.SetActive(false);
                for (int j = 0; j < 3; j++)
                {
                    groups2.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    Util.AddBtnClick(groups2.GetChild(i).GetChild(j).gameObject, onClickBtn);
                }
                groups2.GetChild(i).position = curTrans.Find("groupStart").position;
            }
            ObjNums = new string[8] { "9", "6", "11", "8", "21", "30", "23", "24" };


            temVector3 = new Vector3[2] { curTrans.Find("groupStart").position, groupInPos.position };
            tem2Vector3 = new Vector3[2] { curTrans.Find("groupStart").position, group2InPos.position };
            Timer = 0;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }


        private void onClickBtn(GameObject obj)
        {
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
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13));
                SpineManager.instance.DoAnimation(obj, obj.name, false);
                if (int.Parse(obj.name) < 17)
                {
                    SpineManager.instance.SetFreeze(objGroup.transform.GetChild(objGroup.transform.childCount - 1).gameObject, false);
                    SpineManager.instance.DoAnimation(objGroup.transform.GetChild(objGroup.transform.childCount - 1).gameObject, objGroup.transform.GetChild(objGroup.transform.childCount - 1).name, false);
                    xx.SetActive(false);
                    sx.SetActive(true);
                    for (int i = 0; i < objGroup.transform.childCount; i++)
                    {
                        if (obj.name == objGroup.transform.GetChild(i).name)
                        {
                            temNum = i + 1;
                        }
                    }
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
                    mono.StartCoroutine(PlayElements(3, () => { objGroup.SetActive(false); objGroup.transform.SetParent(groupInPos); sx.SetActive(false); playGroupSpine(); isPlaying = false; }));
                }
                else
                {
                    SpineManager.instance.SetFreeze(objGroup2.transform.GetChild(objGroup2.transform.childCount - 1).gameObject, false);
                    SpineManager.instance.DoAnimation(objGroup2.transform.GetChild(objGroup2.transform.childCount - 1).gameObject, objGroup2.transform.GetChild(objGroup2.transform.childCount - 1).name, false);
                    xx2.SetActive(false);
                    sx2.SetActive(true);
                    for (int i = 0; i < objGroup2.transform.childCount; i++)
                    {
                        if (obj.name == objGroup2.transform.GetChild(i).name)
                        {
                            temNum = i + 4;
                        }
                    }
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
                    mono.StartCoroutine(PlayElements(3, () => { objGroup2.SetActive(false); objGroup2.transform.SetParent(group2InPos); sx2.SetActive(false); playGroup2Spine(); isPlaying = false; }));
                }

                mono.StartCoroutine(PlayElements(0.3f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                    SpineManager.instance.DoAnimation(zhuangImg.GetChild(Timer - 1).gameObject, "6", false, () =>
                    {
                        numTexture.sprite = numTexture.transform.GetComponent<BellSprites>().sprites[Timer];
                        numTexture.SetNativeSize();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                        SpineManager.instance.DoAnimation(powerC, "animation" + Timer * 2, false, () =>
                        {
                            SpineManager.instance.DoAnimation(powerC, "animation" + (Timer * 2 + 1), false, () =>
                            {
                                if (Timer >= 4)
                                {
                                    isPlaying = true;
                                    mono.StartCoroutine(PlayElements(1, () =>
                                    {
                                        mask.SetActive(true); successPanel.SetActive(true); caidai.SetActive(true);
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);

                                        SpineManager.instance.DoAnimation(successPanel, "c2", false, () =>
                                        {
                                            SpineManager.instance.DoAnimation(caidai, "animation", false);
                                            SpineManager.instance.DoAnimation(successPanel, "c", true);
                                        });
                                    }));
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
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4));
                SpineManager.instance.DoAnimation(obj, obj.name, false, () => { isPlaying = false; });
            }
        }

        void GameStart()
        {
            bell.SetActive(true);
            numTexture.sprite = numTexture.transform.GetComponent<BellSprites>().sprites[0];
            numTexture.SetNativeSize();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
            playGroupSpine();
            playGroup2Spine();
            isPlaying = true;
            //AudioSource source = SoundManager.instance.voiceSource;

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }



        IEnumerator PlayElements(float index, Action ac = null)
        {
            yield return new WaitForSeconds(index);
            ac?.Invoke();
        }

        IEnumerator PlayDevils(int index)
        {
            yield return new WaitForSeconds(index);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, false);
            zhuangImg.GetChild(index).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(zhuangImg.GetChild(index).gameObject, "1", false, () =>
            {
                SpineManager.instance.DoAnimation(zhuangImg.GetChild(index).gameObject, "2", true);
                if (index == (zhuangImg.childCount - 1))
                {
                    isPlaying = false;
                }
            });
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "2");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "1");
            SoundManager.instance.SetShield(true);

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    bell.SetActive(false);
                    powerC.SetActive(true);
                    SpineManager.instance.DoAnimation(powerC, "animation", false);
                    powerC.transform.DOMove(new Vector3(bell.transform.position.x, bell.transform.position.y + 218 * Screen.height / 1080, 0), 1).OnComplete(() =>
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

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        float temTime = 0;
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
            objGroup.transform.DOPath(temVector3, 1.5f, PathType.Linear, PathMode.TopDown2D, 5).OnComplete(() => { xx.SetActive(true); });
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

            objGroup2.transform.DOPath(tem2Vector3, 1.5f, PathType.Linear, PathMode.TopDown2D, 5).OnComplete(() => { xx2.SetActive(true); });

        }

    }
}
