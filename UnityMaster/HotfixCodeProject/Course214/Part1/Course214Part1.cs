using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course214Part1
    {
        private GameObject max;
        private GameObject bg2;
        private GameObject bg3;
        private GameObject tile;
        private GameObject problem;
        private GameObject spine;

        private int talkIndex;
        private Vector3[] clPos;
        private GameObject[] moveImg;
        private bool isLoop;

        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject btnTest;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }
        void ReStart(GameObject obj)
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            max = curTrans.Find("Max/Max").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            bg3 = curTrans.Find("bg3").gameObject;
            tile = curTrans.Find("Tile").gameObject;
            problem = curTrans.Find("Problem").gameObject;
            spine = curTrans.Find("Spine").gameObject;

            GameObject MoveImg = curTrans.Find("MoveImg").gameObject;
            moveImg = new GameObject[MoveImg.transform.childCount];
            for (int i = 0; i < MoveImg.transform.childCount; i++)
            {
                moveImg[i] = MoveImg.transform.GetChild(i).gameObject;
            }

            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            isLoop = true;

            max.SetActive(true);
            bg2.SetActive(false);
            bg3.SetActive(false);
            tile.SetActive(false);
            problem.SetActive(true);
            spine.SetActive(false);
            for (int i = 0; i < moveImg.Length; i++)
            {
                moveImg[i].gameObject.SetActive(false);
            }
            problem.transform.localScale = new Vector3(0, 1, 1);
            tile.transform.localScale = new Vector3(0, 1, 1);
            bg3.transform.localScale = new Vector3(1, 0, 1);
            Vector3[] clPos_test = { new Vector3(-186, 99, 0), new Vector3(40, -17, 0), new Vector3(570, 52, 0), new Vector3(380, -11, 0) };
            clPos = clPos_test;
            moveImg[1].transform.localPosition = clPos[0];
            moveImg[2].transform.localPosition = clPos[2];

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
            {
                problem.transform.DOScale(Vector3.one, 1);
            }, () => { mono.StartCoroutine(GameStartSpine()); }));

        }
        IEnumerator GameStartSpine()
        {
            yield return new WaitForSeconds(tem);
            problem.SetActive(false);
            bg2.SetActive(true);
            moveImg[1].gameObject.SetActive(true);
            moveImg[2].gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () => { bg2.SetActive(false);

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { }, () =>
                {
                    moveImg[0].gameObject.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    moveImg[1].transform.DOLocalMove(clPos[1], 1.4f);
                    moveImg[2].transform.DOLocalMove(clPos[3], 1.4f).OnComplete(()=> {
                        moveImg[4].gameObject.SetActive(true);
                        moveImg[5].gameObject.SetActive(true);
                        moveImg[3].gameObject.SetActive(true);


                        spine.SetActive(true);
                        for (int i = 0; i < moveImg.Length; i++)
                        {
                            moveImg[i].gameObject.SetActive(false);
                        }
                       
                        SpineManager.instance.DoAnimation(spine, "2", false,()=> { mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () => {
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () => {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                SpineManager.instance.DoAnimation(spine, "1", false, () => {
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => { }, () => {
                                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => {
                                            tile.SetActive(true);
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                                            tile.transform.DOScale(Vector3.one, 1); }, () => {
                                           
                                                tile.SetActive(false);
                                                spine.SetActive(false);
                                                bg3.SetActive(true);
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                                                bg3.transform.DOScale(Vector3.one, 0.5f).OnComplete(() => { mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () => { }, () => { })); });
                                            ;
                                        }));
                                    }));
                                });
                            }));
                           
                        })); });
                 
                    });
                }));
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }
        float tem = 0;
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            tem = clipLength;
            if (method_1 != null)
            {
                method_1();
            }
            yield return new WaitForSeconds(clipLength);
            SpineManager.instance.DoAnimation(max, "daiji");
            if (method_2 != null)
            {
                method_2();
            }
        }

        void OnDisable()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.Stop();
        }
    }
}
