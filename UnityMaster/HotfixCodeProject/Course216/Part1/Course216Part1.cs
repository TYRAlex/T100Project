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
    public class Course216Part1
    {
        private GameObject bell;
        private GameObject bellBg;
        private GameObject faCar;
        private GameObject moveImg;
        private GameObject moveTile;
        private ILDrager[] moveImgChild;
        private ILDroper[] faCarChild;
        private GameObject[] moveTileChild;
        private GameObject shield;
        private GameObject ligthBtn;
        private GameObject bg2;
        private GameObject btnBack;
        private GameObject spine_1;
        private GameObject spine_2;
        private GameObject spine_3;
        private GameObject ligth;
        private GameObject proBtn;
        private GameObject[] spines;
        private GameObject imgBtn;
        private GameObject[] ligthSpine;
        private GameObject panelImg;
        private GameObject img_rotate;
        private GameObject img_rotate2;
        private Vectrosity.VectorObject2D img_scale;
        private Vectrosity.VectorObject2D img_scale2;
        private GameObject imgSpine;
        private GameObject spotPos;
        private GameObject spotPos2;

        private int[] isSuccess;
        private int talkIndex;
        private float sideSize;
        private float sideSize2;
        private List<Vector2> posList;
        private List<Vector2> posList2;
        private Vector3 startWroldPos;
        private Vector3 startWroldPos2;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject btnTest;
        int temNum = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
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


            bellBg = curTrans.Find("Bell").gameObject;
            bell = curTrans.Find("Bell/BellSpine").gameObject;
            faCar = curTrans.Find("FaCar").gameObject;
            moveImg = curTrans.Find("MoveImg").gameObject;
            moveTile = curTrans.Find("MoveTile").gameObject;
            ligthBtn = curTrans.Find("LigthBtn").gameObject;
            shield = curTrans.Find("Shield").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            btnBack = curTrans.Find("bg2/Image (1)").gameObject;
            ligth = curTrans.Find("Ligths/Ligth").gameObject;
            proBtn = curTrans.Find("ProBtn").gameObject;
            spine_1 = curTrans.Find("Spines/Spine_1").gameObject;
            spine_2 = curTrans.Find("Spines/Spine_2").gameObject;
            spine_3 = curTrans.Find("Spines/Spine_3").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            panelImg = curTrans.Find("PanelImg").gameObject;
            img_scale = curTrans.Find("FaCar/Img_3_2/Image").GetComponent<Vectrosity.VectorObject2D>();
            img_rotate = curTrans.Find("FaCar/Img_3_2/Image2").gameObject;
            img_scale2 = curTrans.Find("FaCar/Img_3_1/Img_3_1").GetComponent<Vectrosity.VectorObject2D>();
            img_rotate2 = curTrans.Find("FaCar/Img_3_1/Image").gameObject;
            imgSpine = curTrans.Find("ImgSpine").gameObject;
            spotPos = img_rotate.transform.Find("Pos").gameObject;
            spotPos2 = img_rotate2.transform.Find("Pos2").gameObject;

            for (int i = 0; i < moveImg.transform.childCount; i++) moveImg.transform.GetChild(i).gameObject.SetActive(true);

            GameObject ligthSpineParent = curTrans.Find("Ligths/LigthSpine").gameObject;
            ligthSpine = new GameObject[ligthSpineParent.transform.childCount];
            for (int i = 0; i < ligthSpine.Length; i++)
            {
                ligthSpine[i] = ligthSpineParent.transform.GetChild(i).gameObject;
            }
            GameObject spines_test = curTrans.Find("Spines/Spines").gameObject;
            spines = new GameObject[spines_test.transform.childCount];
            for (int i = 0; i < spines.Length; i++)
            {
                spines[i] = spines_test.transform.GetChild(i).gameObject;
            }
            faCarChild = faCar.transform.GetComponentsInChildren<ILDroper>();
            moveImgChild = moveImg.transform.GetComponentsInChildren<ILDrager>();

            for (int i = 0; i < moveImgChild.Length; i++)
            {
                moveImgChild[i].SetDragCallback(StartDrag, Drag, EndDrag);
            }
            moveTileChild = new GameObject[moveTile.transform.childCount];
            for (int i = 0; i < moveTile.transform.childCount; i++)
            {
                moveTileChild[i] = moveTile.transform.GetChild(i).gameObject;
            }
            for (int i = 0; i < imgBtn.transform.childCount; i++)
            {
                GameObject newObj = imgBtn.transform.GetChild(i).gameObject;
                Util.AddBtnClick(newObj, DoImgBtnClick);
            }
            Util.AddBtnClick(ligthBtn.transform.Find("Btn").gameObject, DoLigthBtnClick);
            Util.AddBtnClick(proBtn, DoProBtnClick);

            Util.AddBtnClick(btnBack, OnClickBack);

            GameInit();
        }

        private void OnClickBack(GameObject obj)
        {
            if (temNum >= 4)
            {
                bg2.SetActive(false); imgBtn.SetActive(false);
                spine_3.SetActive(false);
                for (int i = 0, len = spines.Length; i < len; i++)
                {
                    spines[i].SetActive(false);
                }
            }
        }

        void GameInit()
        {
            temNum = 0;
            talkIndex = 1;
            startWroldPos = spotPos.transform.position;
            startWroldPos2 = spotPos2.transform.position;
            posList = new List<Vector2>();
            posList.Add(new Vector3(1030, 400));
            posList.Add(new Vector3(1030, 776));
            sideSize = Vector3.Distance(posList[0], posList[1]);
            posList2 = new List<Vector2>();
            posList2.Add(new Vector3(1142, 430));
            posList2.Add(new Vector3(1216, 730));
            sideSize2 = Vector3.Distance(posList2[0], posList2[1]);
            int[] isSuccess_test = { 0, 0, 0 };
            isSuccess = isSuccess_test;

            bell.SetActive(true);
            bellBg.SetActive(true);
            moveImg.SetActive(true);
            moveTile.SetActive(true);
            shield.SetActive(false);
            ligthBtn.SetActive(false);
            proBtn.SetActive(false);
            imgBtn.SetActive(false);
            bg2.SetActive(false);
            ligth.SetActive(false);
            spine_1.SetActive(false);
            spine_2.SetActive(false);
            spine_3.SetActive(false);
            panelImg.SetActive(true);
            imgSpine.SetActive(false);

            for (int i = 0; i < moveImgChild.Length; i++) moveImgChild[i].transform.localScale = Vector3.one;

            for (int i = 0; i < moveTileChild.Length; i++) moveTileChild[i].SetActive(true);
            for (int i = 0; i < faCarChild.Length; i++) faCarChild[i].gameObject.SetActive(false);
            for (int i = 0; i < spines.Length; i++) spines[i].SetActive(false);
            for (int i = 0; i < ligthSpine.Length; i++)
            {
                ligthSpine[i].transform.localScale = new Vector3(0.5f, 0.5f, 1);
                ligthSpine[i].transform.GetComponent<Image>().CrossFadeAlpha(1, 0, true);
                ligthSpine[i].SetActive(false);
            }

            ligth.transform.GetComponent<Image>().CrossFadeAlpha(1, 0, true);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            shield.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () =>
            {
                bellBg.SetActive(false);
                shield.SetActive(false);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () =>
            {
                bg2.SetActive(false); imgBtn.SetActive(false);
                spine_3.SetActive(false);
                for (int i = 0, len = spines.Length; i < len; i++)
                {
                    spines[i].SetActive(false);
                }
            }));
            talkIndex++;
        }

        void StartDrag(Vector3 pos, int index, int type)
        {
            mono.StartCoroutine(StartDragCoroutien(index));
        }

        IEnumerator StartDragCoroutien(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            moveTileChild[index - 1].SetActive(false);
            ligthSpine[index - 1].SetActive(true);
            moveImgChild[index - 1].transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            ligthSpine[index - 1].transform.DOScale(new Vector3(0.7f, 0.7f, 1), 0.5f);
            ligthSpine[index - 1].transform.GetComponent<Image>().CrossFadeAlpha(0.2f, 0.5f, true);
            yield return new WaitForSeconds(1);
            ligthSpine[index - 1].transform.localScale = new Vector3(0.5f, 0.5f, 1);
            ligthSpine[index - 1].transform.GetComponent<Image>().CrossFadeAlpha(1, 0, true);
            ligthSpine[index - 1].SetActive(false);
        }

        void Drag(Vector3 pos, int index, int type)
        {

        }

        void EndDrag(Vector3 pos, int index, int type, bool match)
        {
            if (match)
            {
                isSuccess[index - 1] = 1;
                for (int i = 1; i <= type; i++)
                {
                    faCar.transform.Find("Img_" + index + "_" + i).gameObject.SetActive(true);
                }
                moveImgChild[index - 1].DoReset();
                moveImgChild[index - 1].gameObject.SetActive(false);
                shield.SetActive(true);
                spine_1.SetActive(true);
                if (index <= 3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                }
                string spiName = "";
                if (index == 1) spiName = "animation3";
                else if (index == 2) spiName = "animation6";
                else if (index == 3) spiName = "animation2";
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(spine_1, spiName, false);
                if (isSuccess[0] + isSuccess[1] + isSuccess[2] == 3) panelImg.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index, () => { }, () =>
                {
                    shield.SetActive(false);
                    if (isSuccess[0] + isSuccess[1] + isSuccess[2] == 3)
                    {
                        mono.StartCoroutine(WaitLigth());
                    }
                }));
            }
            else
            {
                moveImgChild[index - 1].DoReset();
                moveTileChild[index - 1].SetActive(true);
                moveImgChild[index - 1].transform.localScale = Vector3.one;
            }
        }

        IEnumerator WaitLigth()
        {
            yield return new WaitForSeconds(0.5f);
            bellBg.SetActive(true);
            ligth.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            ligth.transform.DOScale(new Vector3(0.65f, 0.65f, 1), 1);
            ligth.transform.GetComponent<Image>().CrossFadeAlpha(0, 1, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () =>
              {
                  proBtn.SetActive(true);
              }));
        }

        void DoLigthBtnClick(GameObject obj)
        {
            ligthBtn.SetActive(false);
            spine_1.SetActive(true);
            mono.StartCoroutine(LigthBtnCoroutine());
        }

        IEnumerator LigthBtnCoroutine()
        {
            bool isEnd = false;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            float spiTime = SpineManager.instance.DoAnimation(spine_1, "animation2", false);
            yield return new WaitForSeconds(spiTime);
            spine_1.SetActive(false);
            bg2.SetActive(true);
            spine_3.SetActive(true);
            spiTime = SpineManager.instance.DoAnimation(spine_3, "cx", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => { }, () =>
             {
                 if (isEnd) imgBtn.SetActive(true);
                 else isEnd = true;
             }));
            yield return new WaitForSeconds(spiTime);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
            spiTime = SpineManager.instance.DoAnimation(spine_3, "yd", true);
            yield return new WaitForSeconds(2 * spiTime - 0.2f);
            SpineManager.instance.DoAnimation(spine_3, "animation", false);
            if (isEnd) imgBtn.SetActive(true);
            else isEnd = true;
        }

        void DoProBtnClick(GameObject obj)
        {
            ligth.SetActive(false);
            proBtn.SetActive(false);
            mono.StartCoroutine(ProBtnCoroutine());
            mono.StartCoroutine(ProBtnCoroutine2());
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () =>
            {
                ligthBtn.SetActive(true);
            }, () => { }, 6.8f));
        }

        //IEnumerator zzzzz()
        //{
        //    for(int i = 0; i < 7; i++)
        //    {
        //        yield return new WaitForSeconds(0.67f);
        //        Debug.Log("////////0.67f");
        //    }
        //}

        IEnumerator ProBtnCoroutine()
        {
            imgSpine.SetActive(true);
            faCar.transform.Find("Img_1_1").gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            SpineManager.instance.DoAnimation(imgSpine, "animation7", true);
            //mono.StartCoroutine(zzzzz());
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    img_rotate.transform.localEulerAngles = new Vector3(0, 0, i * 28 * (-12.85f) + j * (-12.85f));
                    DrawLinePos(1);
                    yield return new WaitForSeconds(0.02f);
                }
                //Debug.Log(".............................0.67");
                InitLinePos(1);
            }
            imgSpine.SetActive(false);
            faCar.transform.Find("Img_1_1").gameObject.SetActive(true);
        }
        IEnumerator ProBtnCoroutine2()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    img_rotate2.transform.localEulerAngles = new Vector3(0, 0, i * 28 * (-12.85f) + j * (-12.85f));
                    DrawLinePos(2);
                    yield return new WaitForSeconds(0.02f);
                }
                InitLinePos(2);
            }
        }

        void DrawLinePos(int idx)
        {
            if (idx == 1)
            {
                Vector3 pos_1 = spotPos.transform.position;
                posList[0] = new Vector2(posList[0].x + pos_1.x - startWroldPos.x, posList[0].y + pos_1.y - startWroldPos.y);
                startWroldPos = pos_1;
                double posList_1_y = Math.Abs(Math.Sqrt(sideSize * sideSize - (posList[1].x - posList[0].x) * (posList[1].x - posList[0].x)) + posList[0].y);
                if (posList_1_y > 776) posList_1_y = 776 + (posList_1_y - 776) / 5;
                else if (posList_1_y < 776) posList_1_y = 776 - (776 - posList_1_y) / 1.5f;
                posList[1] = new Vector2(posList[1].x, (float)posList_1_y);
                img_scale.vectorLine.points2 = posList;
                img_scale.vectorLine.Draw();
            }
            else
            {
                Vector3 pos_2 = spotPos2.transform.position;
                posList2[0] = new Vector2(posList2[0].x + pos_2.x - startWroldPos2.x, posList2[0].y + pos_2.y - startWroldPos2.y);
                startWroldPos2 = pos_2;
                double posList_2_y = Math.Abs(Math.Sqrt(sideSize2 * sideSize2 - (posList2[1].x - posList2[0].x) * (posList2[1].x - posList2[0].x)) + posList2[0].y);
                img_scale2.vectorLine.points2 = posList2;
                img_scale2.vectorLine.Draw();
            }
        }

        void InitLinePos(int idx)
        {
            if (idx == 1)
            {
                posList[0] = new Vector3(1030, 400);
                posList[1] = new Vector3(1030, 776);
                img_scale.vectorLine.points2 = posList;
                img_scale.vectorLine.Draw();
                img_rotate.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                posList2[0] = new Vector3(1142, 430);
                posList2[1] = new Vector3(1216, 730);
                img_scale2.vectorLine.points2 = posList2;
                img_scale2.vectorLine.Draw();
                img_rotate2.transform.localEulerAngles = Vector3.zero;
            }
        }


        void DoImgBtnClick(GameObject obj)
        {

            imgBtn.SetActive(false);
            int idx = int.Parse(obj.name);
          
            SoundManager.instance.ShowVoiceBtn(false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 6, () =>
            {
                spines[idx - 1].SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                SpineManager.instance.DoAnimation(spines[idx - 1], "" + idx, false);
            }, () =>
            {
                temNum++;
                imgBtn.SetActive(true);
                if (temNum >= 4)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }));
            //imgBtn.transform.GetComponent<Image>().CrossFadeAlpha()
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            if (method_2 != null)
            {
                method_2();
            }
        }
    }
}
