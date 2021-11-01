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
    public class Course919Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject max;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject jqrSpine;
        private Transform Imgs;
        private Transform drags;

        private Transform maxStartPos;
        private Transform maxEndPos;

        int temNum = 0;
        Color color;
        bool isPlaying = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            max = curTrans.Find("max").gameObject;
            maxStartPos = curTrans.Find("maxStartPos");
            max.transform.position = maxStartPos.position;
            maxEndPos = curTrans.Find("maxEndPos");
            maxEndPos.GetChild(0).gameObject.SetActive(false);
            max.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            jqrSpine = curTrans.Find("jqrSpine").gameObject;
            jqrSpine.SetActive(true);
            SpineManager.instance.DoAnimation(jqrSpine, "kong", false);
            Imgs = curTrans.Find("Imgs");
            Imgs.gameObject.SetActive(true);
            drags = curTrans.Find("drags");
            drags.gameObject.SetActive(true);

            for (int i = 0; i < jqrSpine.transform.childCount; i++)
            {
                Util.AddBtnClick(jqrSpine.transform.GetChild(i).gameObject, OnClickPlaySpine);
                jqrSpine.transform.GetChild(i).gameObject.SetActive(false);
            }

            color = new Color(1, 1, 1, 1);
            temNum = 0;
            for (int i = 0; i < drags.childCount; i++)
            {
                Imgs.GetChild(i).GetComponent<Image>().color = color;
                Imgs.GetChild(i).GetChild(0).GetComponent<Image>().color = color;
                Imgs.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>().color = color;
                Imgs.GetChild(i).gameObject.SetActive(true);
                Imgs.GetChild(i).GetChild(0).gameObject.SetActive(true);
                Imgs.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(true);

                drags.GetChild(i).gameObject.SetActive(true);
                drags.GetChild(i).GetComponent<ILDrager>().SetDragCallback(null, ImgDarg, null);
            }

            isPlaying = false;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        private void OnClickPlaySpine(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
           
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1,
                () =>
                {

                    SpineManager.instance.DoAnimation(jqrSpine, obj.name + "1", false,
             () =>
             {
                 Imgs.gameObject.SetActive(false);
                 drags.gameObject.SetActive(false);
                 Bg.GetComponent<RawImage>().texture = bellTextures.texture[obj.transform.GetSiblingIndex()];
                 obj.transform.GetChild(0).gameObject.SetActive(true);
                 SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "n", false, () =>
                 {
                     SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "cx", false, () =>
                     {
                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, obj.transform.GetSiblingIndex(), false);
                         SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).name, false,
                         () =>
                         {
                             SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).name + "2", false,
                     () =>
                     {
                         SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "xs", false, () =>
                         {
                             Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
                             Imgs.gameObject.SetActive(true);
                             drags.gameObject.SetActive(true);
                             //obj.SetActive(false);
                             SpineManager.instance.DoAnimation(jqrSpine, obj.name + "2", false,
                            () =>
                            {
                                isPlaying = false;

                                SoundManager.instance.ShowVoiceBtn(true);
                            });
                         });
                     });
                         });
                 });
             });
             });
                }, null, 3));

        }

        private void ImgDarg(Vector3 pos, int type, int index)
        {
            if (isPlaying)
                return;
            SetObjAlpha(Imgs.GetChild(index), index);
        }

        void SetObjAlpha(Transform tranf, int index)
        {
            color.a -= 0.05f;
            SoundManager.instance.ShowVoiceBtn(false);
            if (!tranf.GetChild(0).GetChild(0).gameObject.activeSelf)
            {
                tranf.GetChild(0).GetChild(0).GetComponent<Image>().color = color;

                if (!tranf.GetChild(0).gameObject.activeSelf)
                {
                    tranf.GetComponent<Image>().color = color;
                    if (color.a <= 0)
                    {
                        tranf.gameObject.SetActive(false);
                        isPlaying = true;
                        color.a = 1f;
                       
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index + 1,
                            () =>
                            {
                                jqrSpine.transform.GetChild(index).gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(jqrSpine, jqrSpine.transform.GetChild(index).name + "1", false,
                         () =>
                         {
                             Imgs.gameObject.SetActive(false);
                             drags.gameObject.SetActive(false);
                             jqrSpine.transform.GetChild(index).GetComponent<Empty4Raycast>().enabled = false;
                             Bg.GetComponent<RawImage>().texture = bellTextures.texture[index];

                             SpineManager.instance.DoAnimation(jqrSpine.transform.GetChild(index).GetChild(0).gameObject, "n", false, () =>
                             {
                                 SpineManager.instance.DoAnimation(jqrSpine.transform.GetChild(index).GetChild(0).gameObject, "cx", false, () =>
                                 {
                                     SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, false);
                                     SpineManager.instance.DoAnimation(jqrSpine.transform.GetChild(index).GetChild(0).gameObject, jqrSpine.transform.GetChild(index).GetChild(0).name, false,
                                     () =>
                                     {
                                         SpineManager.instance.DoAnimation(jqrSpine.transform.GetChild(index).GetChild(0).gameObject, jqrSpine.transform.GetChild(index).GetChild(0).name + "2", false,
                                 () =>
                                 {
                                     SpineManager.instance.DoAnimation(jqrSpine.transform.GetChild(index).GetChild(0).gameObject, "xs", false, () =>
                                     {
                                         Imgs.gameObject.SetActive(true);
                                         drags.gameObject.SetActive(true);
                                         Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
                                         //需要将状态先置空
                                         jqrSpine.transform.GetChild(index).GetComponent<Empty4Raycast>().enabled = true;
                                         jqrSpine.transform.GetChild(index).GetChild(0).gameObject.SetActive(false);
                                         SpineManager.instance.DoAnimation(jqrSpine, jqrSpine.transform.GetChild(index).name + "2", false,
                                        () =>
                                        {
                                            for (int i = 0; i < drags.childCount; i++)
                                            {
                                                if (Imgs.GetChild(i).gameObject.activeSelf)
                                                {
                                                    drags.GetChild(i).gameObject.SetActive(true);
                                                }
                                                else
                                                {
                                                    drags.GetChild(i).gameObject.SetActive(false);
                                                }
                                            }
                                            isPlaying = false;
                                            temNum++;
                                            if (temNum >= 3)
                                            {
                                                drags.gameObject.SetActive(false);
                                                SoundManager.instance.ShowVoiceBtn(true);
                                            }
                                        });
                                     });
                                 });
                                     });
                                 });
                             });
                         });
                            },null,3));

                    }
                }
                else
                {
                    tranf.GetChild(0).GetComponent<Image>().color = color;
                    if (color.a <= 0)
                    {
                        tranf.GetChild(0).gameObject.SetActive(false);
                        color.a = 1f;
                    }

                }
            }
            else
            {
                tranf.GetChild(0).GetChild(0).GetComponent<Image>().color = color;

                if (color.a <= 0)
                {
                    tranf.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    color.a = 1f;
                }

            }
        }

        void GameStart()
        {
            isPlaying = true;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[3];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { SpineManager.instance.DoAnimation(jqrSpine, "animation", false); }, () => { max.SetActive(false); isPlaying = false; }));

        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
                method_1?.Invoke();
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(max, "daijishuohua");

           

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                jqrSpine.SetActive(false);
                Imgs.gameObject.SetActive(false);
                drags.gameObject.SetActive(false);
                maxEndPos.GetChild(0).gameObject.SetActive(true);
                max.transform.position = maxEndPos.position;
                max.SetActive(true);
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[4];
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
