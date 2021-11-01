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
    public class Course209Part1
    {
        private GameObject spine_1;
        private GameObject spine_2_start;
        private GameObject spine_2;
        private GameObject spine_3;
        private GameObject spine_3_2;
        private GameObject spine_stone_3;
        private GameObject spine_cheng;
        private GameObject daStone;
        private GameObject startBtn;
        private GameObject endBtn;
        private GameObject bj2;
        private GameObject imgBg;
        private GameObject imgBtn;
        private GameObject imgMT;
        private GameObject[] spines;
        private GameObject shitouSpine;
        private ILDrager stone;
        private Image[] stonePos;
        private Image[] downImg;

        private int idx;
        private int talkIndex;
        private string[] spineIndex;
        private int[] isDown;
        private string[] spineName;
        private Vector3 startPos;
        private Vector3 endPos_1;
        private Vector3 endPos_2;
        private Vector3 stoneStartPos;
        private Vector3[] daStonePos;
        private bool isSourrse;

        private float[] declineDistance;
        private float[] angle;
        private float[] stonePosX;
        private float stoneOldX;
        private float stoneNewX;
        private int stoneIndex;
        private float numxxx;

        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject reStart;
        private GameObject spines_test;

        void Start(object o)
        {
            curGo = (GameObject) o;
            Transform curTrans = curGo.transform;

            spine_1 = curTrans.Find("Spine/1").gameObject;
            spine_2_start = curTrans.Find("bg/ImgPos/Image/2").gameObject;
            spine_2 = curTrans.Find("Spine/2").gameObject;
            spine_3 = curTrans.Find("Spine/3").gameObject;
            spine_3_2 = curTrans.Find("Spine/3_2").gameObject;
            startBtn = curTrans.Find("ImgBtn/StartBtn").gameObject;
            endBtn = curTrans.Find("ImgBtn/BackBtn").gameObject;
            spine_stone_3 = curTrans.Find("bg/ImgPos/3").gameObject;
            spine_cheng = curTrans.Find("Spine/cheng").gameObject;
            stone = curTrans.Find("bg/ImgPos/Stone").GetComponent<ILDrager>();
            imgMT = curTrans.Find("bg/ImgPos/Image").gameObject;
            stonePos = curTrans.Find("bg/StonePos").GetComponentsInChildren<Image>();
            downImg = curTrans.Find("Spine/1").GetComponentsInChildren<Image>(true);
            daStone = curTrans.Find("bg/ImgPos/Image/DaStone").gameObject;
            bj2 = curTrans.Find("Spine/bj2").gameObject;
            imgBg = curTrans.Find("Spine/ImgBg").gameObject;
            imgBtn = curTrans.Find("ImgBtn/ImgBtn").gameObject;
            shitouSpine = curTrans.Find("Spine/shitouSpine").gameObject;
            reStart = curTrans.Find("bg/Button").gameObject;

            spines_test = curTrans.Find("Spine/Spines").gameObject;
            spines = new GameObject[spines_test.transform.childCount];
            for (int i = 0; i < spines_test.transform.childCount; i++)
            {
                spines[i] = spines_test.transform.GetChild(i).gameObject;
                spines[i].SetActive(false);
            }

            mono = curGo.GetComponent<MonoBehaviour>();

            stone.SetDragCallback(StartDrag, Drag, EndDrag);
            //reStart.Hide();
            GameInit(reStart);
            Util.AddBtnClick(reStart, GameInit);
        }


        void GameInit(GameObject gameObject)
        {
            talkIndex = 1;
            numxxx = 3;
            int[] isDown_test = {0, 0, 0, 0};
            isDown = isDown_test;
            string[] spineIndex_test = {"2", "5", "3", "4", "1"};
            spineIndex = spineIndex_test;
            float[] declineDistance_test = {11, -8, -10};
            declineDistance = declineDistance_test;
            float[] angle_test = {4, 9, -1};
            angle = angle_test;
            float[] stonePosX_test = {-324.8f, -256.2f, -88.2f, -13.6f};
            stonePosX = stonePosX_test;
            stone.isActived = true;
            isSourrse = true;
            stoneOldX = stone.transform.localPosition.x;
            startPos = new Vector3(-200, -462, 0);
            endPos_1 = new Vector3(-1702, -133, 0);
            endPos_2 = new Vector3(880, 200, 0);
            stoneStartPos = new Vector3(-172f, -393.5f, 0);


            
            spines_test.Hide();
            mono.StopAllCoroutines();
            Vector3[] daStonePos_test = PosArrage(100, new Vector3(-200, -462, 0), new Vector3(880, 300, 0)); 


            daStonePos = daStonePos_test;
            string[] spineName_test = {"ganggan", "zhidian", "zuli", "dongli", "zulibi", "donglibi"};
            spineName = spineName_test;

            imgMT.SetActive(true);
            daStone.SetActive(true);
            spine_stone_3.SetActive(true);
            stone.gameObject.SetActive(true);

            for (int i = 0; i < spine_1.transform.childCount; i++)
            {
                spine_1.transform.GetChild(i).gameObject.Show();
            }
            spine_1.SetActive(false);

            foreach (var VARIABLE in downImg)
            {
                VARIABLE.gameObject.Show();
            }

            for (int i = 0; i < spines.Length; i++)
            {
                spines[i].Show();
            }

            foreach (var VARIABLE in spines)
            {
                VARIABLE.Hide();
            }
            
            spine_2.SetActive(false);
            spine_2_start.SetActive(true);
            spine_3.SetActive(false);
            spine_3_2.SetActive(false);
            spine_cheng.SetActive(false);
            bj2.SetActive(false);
            startBtn.SetActive(false);
            endBtn.SetActive(false);
            imgBg.SetActive(false);
            imgBtn.SetActive(false);
            shitouSpine.SetActive(false);

            for (int i = 0; i < imgBtn.transform.childCount; i++)
            {
                GameObject obj = imgBtn.transform.GetChild(i).gameObject;
                obj.transform.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
                Util.AddBtnClick(obj, DoImgBtnClick);
            }

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Util.AddBtnClick(startBtn, DoStartBtnClick);
            Util.AddBtnClick(endBtn, DoBackBtnClick);


            spine_2.transform.localPosition = startPos;
            stone.gameObject.transform.localPosition = stoneStartPos;

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(spine_2, "daiji", true);
            SpineManager.instance.DoAnimation(spine_2_start, "daiji", true);
        }

        void StartDrag(Vector3 pos, int index, int type)
        {
            spine_stone_3.SetActive(false);
        }

        void Drag(Vector3 pos, int index, int type)
        {
            float x = stone.transform.localPosition.x;
            if (x < -324f)
            {
                stone.transform.localPosition = new Vector3(-324, -393.5f, 0);
                return;
            }
            else if (x > -14f)
            {
                stone.transform.localPosition = new Vector3(-13.6f, -393.5f, 0);
                return;
            }
            else
            {
                stone.transform.localPosition = new Vector3(x, -393.5f, 0);
            }

            if (x >= -88.2f) stoneIndex = 2;
            else if (x >= -259.2f) stoneIndex = 1;
            else stoneIndex = 0;
            int oldStoneIndex;
            if (stoneOldX >= -88.2f) oldStoneIndex = 2;
            else if (stoneOldX >= -259.2f) oldStoneIndex = 1;
            else oldStoneIndex = 0;
            stoneNewX = x;
            CalculationMothod(stoneIndex - oldStoneIndex);
            stoneOldX = stoneNewX;
        }

        void CalculationMothod(int idx)
        {
            if (idx == 0)
            {
                float num = (stoneNewX - stoneOldX) / (stonePosX[stoneIndex + 1] - stonePosX[stoneIndex]);
                numxxx = numxxx + angle[stoneIndex] * num;
                imgMT.transform.localEulerAngles = new Vector3(0, 0, numxxx);
                imgMT.transform.localPosition = new Vector3(-175,
                    imgMT.transform.localPosition.y + (declineDistance[stoneIndex] * num), 0);
            }
            else if (idx == 1 || idx == -1)
            {
                if (stoneNewX > stoneOldX)
                {
                    float middleNum;
                    if (stoneOldX >= -256.2f) middleNum = -88.2f;
                    else middleNum = -256.2f;
                    float num_1 = (stoneNewX - middleNum) / (stonePosX[stoneIndex + 1] - stonePosX[stoneIndex]);
                    float num_2 = (middleNum - stoneOldX) / (stonePosX[stoneIndex] - stonePosX[stoneIndex - 1]);
                    numxxx = numxxx + (angle[stoneIndex] * num_1 + angle[stoneIndex - 1] * num_2);
                    imgMT.transform.localEulerAngles = new Vector3(0, 0, numxxx);
                    imgMT.transform.localPosition = new Vector3(-175,
                        imgMT.transform.localPosition.y +
                        (declineDistance[stoneIndex] * num_1 + declineDistance[stoneIndex - 1] * num_2), 0);
                }
                else
                {
                    float middleNum;
                    if (stoneOldX >= -88.2f) middleNum = -88.2f;
                    else middleNum = -256.2f;
                    float num_1 = (stoneNewX - middleNum) / (stonePosX[stoneIndex + 1] - stonePosX[stoneIndex]);
                    float num_2 = (middleNum - stoneOldX) / (stonePosX[stoneIndex + 2] - stonePosX[stoneIndex + 1]);
                    numxxx = numxxx + (angle[stoneIndex] * num_1 + angle[stoneIndex + 1] * num_2);
                    imgMT.transform.localEulerAngles = new Vector3(0, 0, numxxx);
                    imgMT.transform.localPosition = new Vector3(-175,
                        imgMT.transform.localPosition.y +
                        (declineDistance[stoneIndex] * num_1 + declineDistance[stoneIndex + 1] * num_2), 0);
                }
            }
            else if (idx == 2 || idx == -2)
            {
                if (stoneNewX > stoneOldX)
                {
                    float middleNum_1 = -88.2f;
                    float middleNum_2 = -256.2f;
                    float num_1 = (stoneNewX - middleNum_1) / (stonePosX[stoneIndex + 1] - stonePosX[stoneIndex]);
                    float num_2 = (middleNum_2 - stoneOldX) / (stonePosX[stoneIndex - 1] - stonePosX[stoneIndex - 2]);
                    numxxx = numxxx +
                             (angle[stoneIndex] * num_1 + angle[stoneIndex - 1] + angle[stoneIndex - 2] * num_2);
                    imgMT.transform.localEulerAngles = new Vector3(0, 0, numxxx);
                    imgMT.transform.localPosition = new Vector3(-175,
                        imgMT.transform.localPosition.y +
                        (declineDistance[stoneIndex] * num_1 + declineDistance[stoneIndex - 1] +
                         declineDistance[stoneIndex - 2] * num_2), 0);
                }
                else
                {
                    float middleNum_1 = -256.2f;
                    float middleNum_2 = -88.2f;
                    float num_1 = (stoneNewX - middleNum_1) / (stonePosX[stoneIndex + 1] - stonePosX[stoneIndex]);
                    float num_2 = (middleNum_2 - stoneOldX) / (stonePosX[stoneIndex - 1] - stonePosX[stoneIndex - 2]);
                    numxxx = numxxx +
                             (angle[stoneIndex] * num_1 - angle[stoneIndex + 1] + angle[stoneIndex + 2] * num_2);
                    imgMT.transform.localEulerAngles = new Vector3(0, 0, numxxx);
                    imgMT.transform.localPosition = new Vector3(-175,
                        imgMT.transform.localPosition.y +
                        (declineDistance[stoneIndex] * num_1 - declineDistance[stoneIndex + 1] +
                         declineDistance[stoneIndex + 2] * num_2), 0);
                }
            }
        }

        void EndDrag(Vector3 pos, int index, int type, bool isMatch)
        {
            float x = stone.transform.localPosition.x;
            if (x >= -324.8f && x <= -13.6f) isMatch = true;
            //spine_stone_3.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                spine_3.SetActive(true);
                spine_3_2.SetActive(true);
                startBtn.SetActive(true);
                SpineManager.instance.DoAnimation(spine_3, "a", true);
                SpineManager.instance.DoAnimation(spine_3_2, "b", true);
                idx = -1;
                SpineManager.instance.DoAnimation(spine_stone_3, "c");
                if (x < -301)
                {
                    idx = 0;
                }
                else if (x < -201)
                {
                    idx = 1;
                }
                else if (x < -113)
                {
                    idx = 2;
                }
                else if (x < -25)
                {
                    idx = 3;
                }
                else
                {
                    idx = 4;
                }

                stone.gameObject.transform.localPosition = stonePos[idx].gameObject.transform.localPosition;
            }
            else
            {
                stone.DoReset();
            }
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                endBtn.SetActive(false);
                spines_test.Show();
                bj2.SetActive(true);
                spine_cheng.SetActive(false);
                imgBg.SetActive(true);
                spine_1.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { },
                    () => { imgBtn.SetActive(true); }));
            }

            talkIndex++;
        }

        void DoStartBtnClick(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            stone.isActived = false;
            startBtn.SetActive(false);
            mono.StartCoroutine(StoneFlight());
        }

        void DoBackBtnClick(GameObject obj)
        {
            idx = 2;
            numxxx = 3;
            stoneOldX = -172;
            stone.isActived = true;
            imgMT.transform.localEulerAngles = new Vector3(0, 0, 0);
            imgMT.transform.localPosition = new Vector3(-175, -344, 0);
            daStone.SetActive(true);
            shitouSpine.SetActive(false);
            stone.gameObject.SetActive(true);
            imgMT.SetActive(true);
            endBtn.SetActive(false);
            spine_cheng.SetActive(false);
            bj2.SetActive(false);
            spine_stone_3.SetActive(true);
            spine_2.transform.localPosition = startPos;
            stone.transform.localPosition = stoneStartPos;
            SpineManager.instance.DoAnimation(spine_stone_3, "c", true);
            SpineManager.instance.DoAnimation(spine_2, "daiji", true);
            spine_2.SetActive(false);
            spine_2_start.SetActive(true);
        }

        void DoImgBtnClick(GameObject obj)
        {
            int index = int.Parse(obj.name);
            Debug.Log("index:   " + index);
            isDown[index] = 1;
            int num = 0;
            for (int i = 0; i < 4; i++)
            {
                num = num + isDown[i];
            }

            if (index > 0)
            {
                downImg[index].gameObject.SetActive(false);
            }

            imgBtn.SetActive(false);
           
            spines[index].SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 10, false);
            SpineManager.instance.DoAnimation(spines[index], spineName[index], false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3 + index, () => { }, () =>
            {
                if (isSourrse && num >= 4)
                {
                    isSourrse = false;
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () =>
                    {
                        spines[4].SetActive(true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 10, false);
                        SpineManager.instance.DoAnimation(spines[4], spineName[4], false);
                    }, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, () =>
                        {
                            spines[5].SetActive(true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 10, false);
                            SpineManager.instance.DoAnimation(spines[5], spineName[5], false);
                        }, () => { imgBtn.SetActive(true); }));
                    }));
                }
                else
                {
                    imgBtn.SetActive(true);
                }
            }));
        }

        IEnumerator StoneFlight()
        {
            float downTime = SpineManager.instance.DoAnimation(spine_3, "a2", true);
            SpineManager.instance.DoAnimation(spine_3_2, "b2", true);
            yield return new WaitForSeconds(downTime);
            spine_3.SetActive(false);
            spine_3_2.SetActive(false);
            spine_2.SetActive(true);
            spine_2_start.SetActive(false);

            //ִʯͷ�ͷ���
            string[] Spine_2_spineName = {"2", "", "3", "4", "1"};
            if (idx == 1)
            {
                SpineManager.instance.DoAnimation(spine_2, "reng", false);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                SpineManager.instance.DoAnimation(spine_2, Spine_2_spineName[idx], false);
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
            //yield return new WaitForSeconds(0.5f);
            shitouSpine.SetActive(true);
            stone.gameObject.SetActive(false);
            imgMT.SetActive(false);
            daStone.SetActive(false);
            downTime = SpineManager.instance.DoAnimation(shitouSpine, spineIndex[idx], false);
            yield return new WaitForSeconds(downTime - 1);
            if (idx == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(spine_2, "fei", true);
                for (int i = 0; i < 100; i++)
                {
                    spine_2.transform.localPosition = daStonePos[i];
                    yield return new WaitForSeconds(0.6f / 100);
                }

                //spine_2.transform.DOLocalPath(daStonePos, 1.5f);
                //spine_2.transform.DOLocalMove(endPos_2, 2);
                //yield return new WaitForSeconds(1.5f);
                spine_2.transform.localPosition = endPos_2;
                SpineManager.instance.DoAnimation(spine_2, "daiji2", true);
            }

            if (idx == 0)
            {
                yield return new WaitForSeconds(1f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                yield return new WaitForSeconds(2f);
                SpineManager.instance.DoAnimation(spine_2, "yun", true);
            }

            yield return new WaitForSeconds(1);

            bj2.SetActive(true);
            spine_cheng.SetActive(true);
            mono.StartCoroutine(Recovery());
            string str = "chenggong";
            int num = 0;
            if (idx != 1)
            {
                str = "shibai";
                num = 1;
            }

            if (num == 0) SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9, false);
            SpineManager.instance.DoAnimation(spine_cheng, str, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, num, () => { }, () =>
            {
                if (idx == 1)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }

                endBtn.SetActive(true);
            }));
        }

        IEnumerator Recovery()
        {
            SpineManager.instance.DoAnimation(shitouSpine, "5", false);
            yield return new WaitForSeconds(0.22f);
            shitouSpine.SetActive(false);
        }

        Vector3[] PosArrage(int n, Vector3 startPos1, Vector3 endPos1)
        {
            Vector3[] stonePosArrage = new Vector3[n];
            float x = startPos1.x - endPos1.x;
            float y = startPos1.y - endPos1.y;
            float p = x * x / (-2 * y);

            float vec_x = 0;
            float vec_y = 0;
            for (int i = 0; i < n; i++)
            {
                vec_x = (x + i * ((endPos1.x - startPos1.x) / (n - 1)));
                vec_y = vec_x * vec_x / (-2 * p);
                stonePosArrage[i] = new Vector3(vec_x + endPos1.x, vec_y + endPos1.y);
            }

            return stonePosArrage;
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            //SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }

            yield return new WaitForSeconds(clipLength - len);
            //SpineManager.instance.DoAnimation(bell, "DAIJI");
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