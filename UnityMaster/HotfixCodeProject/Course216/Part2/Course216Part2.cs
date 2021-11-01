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
    public class Course216Part2
    {
        private GameObject bell;
        private GameObject imgBtn;
        private GameObject okBtn;
        private GameObject backBtn;
        private GameObject shield;
        private GameObject bg2;
        private GameObject bg3;
        private GameObject bg4;
        private GameObject spine_4;
        private GameObject shengli;
        private GameObject finger;//手指动画
        private GameObject arroe;//提示箭头
        private GameObject[] problems;
        private GameObject[] problemAns;
        private GameObject[] moveSpot_1;
        private GameObject[] moveSpot_2;

        private List<List<Vectrosity.VectorObject2D>> linesVec;//线的集合
        private List<List<ILDrager>> spotDar;//移动的点Drager的集合
        private List<Vector2> posList;
        private Vector3[][] lineStartPos;//各线点一集合
        private Vector3[][] lineEndPos;//各线点二集合
        private Vector3[] TargetStartPos;
        private Vector3[] TilePos_1;
        private Vector3[] TilePos_2;
        private double[] sidesR;//各线边长集合
        private int[] Ans_1Bool;
        private int[] Ans_2Bool;
        private System.Random radom;

        private float spot_3PosX;//记录点三的X值来判断是不否成功
        private int currentDawnPage;//当前点击的问题
        private int dragIndex;//拖动并播放语音的次数
        private int talkIndex;//语音键播放语音次数
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        GameObject btnTest;

        float screenRatioW;
        float screenRatioH;

        private GameObject okSpine;
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

        private void ReStart(GameObject obj)
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            bell = curTrans.Find("Bell/Bell").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            shield = curTrans.Find("Shield").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            bg3 = curTrans.Find("bg3").gameObject;
            bg4 = curTrans.Find("bg4").gameObject;
            spine_4 = curTrans.Find("Spine_4").gameObject;
            shengli = curTrans.Find("Shengli").gameObject;
            okSpine = curTrans.Find("okSpine").gameObject;
            okBtn = curTrans.Find("okSpine/animation2").gameObject;
            backBtn = curTrans.Find("BackBtn").gameObject;
            finger = curTrans.Find("Finger").gameObject;
            arroe = curTrans.Find("Arrow").gameObject;

            GameObject lineAndSpot = curTrans.Find("LineAndSpot").gameObject;
            problems = new GameObject[lineAndSpot.transform.childCount];
            for (int i = 0; i < lineAndSpot.transform.childCount; i++)
            {
                problems[i] = lineAndSpot.transform.GetChild(i).gameObject;
            }
            GameObject problemAns_test = curTrans.Find("ProblemAns").gameObject;
            problemAns = new GameObject[problemAns_test.transform.childCount];
            for (int i = 0; i < problemAns_test.transform.childCount; i++)
            {
                problemAns[i] = problemAns_test.transform.GetChild(i).gameObject;
            }
            linesVec = new List<List<Vectrosity.VectorObject2D>>();
            spotDar = new List<List<ILDrager>>();
            for (int i = 0; i < problems.Length; i++)
            {
                List<Vectrosity.VectorObject2D> list_test = new List<Vectrosity.VectorObject2D>();
                List<ILDrager> spotDar_test = new List<ILDrager>();
                for (int j = 1; j < 4; j++) list_test.Add(problems[i].transform.Find("Lines/Line_" + j).GetComponent<Vectrosity.VectorObject2D>());
                for (int j = 1; j < 3; j++) spotDar_test.Add(problems[i].transform.Find("SpotImg/Drager_" + j).GetComponent<ILDrager>());
                linesVec.Add(list_test);
                spotDar.Add(spotDar_test);
            }
            moveSpot_1 = new GameObject[problems.Length];
            moveSpot_2 = new GameObject[problems.Length];
            for (int i = 0; i < problems.Length; i++)
            {
                moveSpot_1[i] = problems[i].transform.Find("Spot_1").gameObject;
                moveSpot_2[i] = problems[i].transform.Find("Spot_2").gameObject;
            }
            for (int i = 0; i < spotDar.Count; i++) spotDar[i][0].SetDragCallback(StartDarg, Darg, EndDarg);
            for (int i = 0; i < imgBtn.transform.childCount; i++)
            {
                GameObject newObj = imgBtn.transform.GetChild(i).gameObject;
                Util.AddBtnClick(newObj, DoImgBtnClick);
            }
            Util.AddBtnClick(okBtn, DoOkBtnClick);
            Util.AddBtnClick(backBtn, DoBackBtnClick);
            screenRatioW = Screen.width / 1920f;
            screenRatioH = Screen.height / 1080f;
        
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            dragIndex = 1;
            currentDawnPage = 0;
            radom = new System.Random();
            int[] Ans_1Bool_test = { 0, 0, 0 };
            Ans_1Bool = Ans_1Bool_test;
            int[] Ans_2Bool_test = { 0, 0, 0 };
            Ans_2Bool = Ans_2Bool_test;
            Vector3[] TargetStartPos_test = { new Vector3(0, 0, 0), new Vector3(-64, -34, 0), new Vector3(49, -40, 0) };
            UpdateScreenRatio(TargetStartPos_test);
            TargetStartPos = TargetStartPos_test;
            Vector3[] TilePos_1_test = { new Vector3(588, 283, 0), new Vector3(513, 270, 0), new Vector3(605, 270, 0) };
            UpdateScreenRatio(TilePos_1_test);
            TilePos_1 = TilePos_1_test;
            Vector3[] TilePos_2_test = { new Vector3(527, -77,0), new Vector3(529, -138, 0), new Vector3(598, -137, 0) };
            UpdateScreenRatio(TilePos_2_test);
            TilePos_2 = TilePos_2_test;

            Vector3[][] lineStartPos_test = {
                new Vector3[]{ new Vector3(805, 418, 0), new Vector3(750, 538, 0), new Vector3(1086, 622, 0) },
                new Vector3[]{ new Vector3(805, 418, 0), new Vector3(750, 538, 0), new Vector3(1086, 622, 0) },
                new Vector3[]{ new Vector3(805, 418, 0), new Vector3(750, 538, 0), new Vector3(1086, 622, 0) }
            };
            for (int i = 0; i < lineStartPos_test.Length; i++)
            {
                UpdateScreenRatio(lineStartPos_test[i]);
            }
            lineStartPos = lineStartPos_test;
            Vector3[][] lineEndPos_test = {
                new Vector3[]{ new Vector3(750, 538, 0), new Vector3(1086, 622, 0), new Vector3(1113, 418, 0) },
                new Vector3[]{ new Vector3(750, 538, 0), new Vector3(1086, 622, 0), new Vector3(1113, 418, 0) },
                new Vector3[]{ new Vector3(750, 538, 0), new Vector3(1086, 622, 0), new Vector3(1113, 418, 0) }
            };
            for (int i = 0; i < lineEndPos_test.Length; i++)
            {
                UpdateScreenRatio(lineEndPos_test[i]);
            }
            lineEndPos = lineEndPos_test;
            InitSidesR(lineStartPos.Length);

            posList = new List<Vector2>();
            posList.Add(new Vector2(0, 0));
            posList.Add(new Vector2(0, 0));
            for (int i = 0; i < problems.Length; i++) ProblemInit(i);
            for (int i = 0; i < problems.Length; i++)
            {
                problems[i].SetActive(false);
                //spotDar[i][0].isActived = false;
            }
            for (int i = 0; i < problemAns.Length; i++)
            {
                problemAns[i].SetActive(false);
                problemAns[i].transform.Find("Ans_1").gameObject.SetActive(false);
                problemAns[i].transform.Find("Ans_2").gameObject.SetActive(false);
            }
            imgBtn.SetActive(false);
            shield.SetActive(false);
            bg2.SetActive(false);
            bg3.SetActive(false);
            bg4.SetActive(false);
            spine_4.SetActive(false);
            shengli.SetActive(false);
            okBtn.SetActive(false);
            backBtn.SetActive(false);
            finger.SetActive(false);
            arroe.SetActive(false);
            arroe.transform.GetComponent<Image>().CrossFadeAlpha(0, 0, true);
            okSpine.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void UpdateScreenRatio(Vector3[] v3)
        {
            for (int i = 0, len = v3.Length; i < len; i++)
            {
                v3[i].x *= screenRatioW;
                v3[i].y *= screenRatioH;
            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            shield.SetActive(true);
            bg2.SetActive(true);
            problems[0].SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void InitSidesR(int lineNum_1)
        {
            sidesR = new double[lineNum_1];
            for (int i = 0; i < sidesR.Length; i++)
            {
                sidesR[i] = Vector3.Distance(lineStartPos[0][i], lineEndPos[0][i]);
            }
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () =>
                {
                    finger.SetActive(true);
                    arroe.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    arroe.transform.GetComponent<Image>().CrossFadeAlpha(1, 0.5f, true);
                    SpineManager.instance.DoAnimation(finger, "a", true);
                    shield.SetActive(false);
                    //spotDar[0][0].isActived = true;
                }, 2));
            }
            else if (talkIndex == 2)
            {
                shield.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () =>
                {
                    shield.SetActive(false);
                }));
            }
            else if (talkIndex == 3)
            {
                problems[0].SetActive(false);
                bg2.SetActive(false);
                spine_4.SetActive(true);
                ProblemInit(0);
                //problems[0].SetActive(false);
                SpineManager.instance.DoAnimation(spine_4, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 4)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => { }, () =>
                {
                    imgBtn.SetActive(true);
                }));
            }
            talkIndex++;
        }

        //三个图片点击事件
        void DoImgBtnClick(GameObject obj)
        {
            shield.SetActive(true);
            imgBtn.SetActive(false);
            int idx = int.Parse(obj.name);
            currentDawnPage = idx;
            mono.StartCoroutine(ImgBtnCoroutine(idx));
        }

        IEnumerator ImgBtnCoroutine(int idx)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,false);
            float spiTime = SpineManager.instance.DoAnimation(spine_4, "" + (idx + 1), false);
            yield return new WaitForSeconds(spiTime);
            if (idx == 0) bg2.SetActive(true);
            else bg3.SetActive(true);
            problems[idx].SetActive(true);
            problemAns[idx].SetActive(true);
            //SpineManager.instance.DoAnimation(spine_4, "OK", false);
            spine_4.SetActive(false);
            okSpine.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () => { }, () =>
            {
                ProblemInit(currentDawnPage);
                shield.SetActive(false);
                okBtn.SetActive(true);
            }));
        }

        void DoOkBtnClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(9);
            SpineManager.instance.DoAnimation(okSpine, obj.name,false,()=> { mono.StartCoroutine(OkBtnCoroutine()); });       
        }

        //Ok按钮点击事件协程
        IEnumerator OkBtnCoroutine()
        {
            shield.SetActive(true);
            if (Ans_1Bool[currentDawnPage] == 0 && spot_3PosX <= 965.2f)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                problems[currentDawnPage].transform.DOLocalMove(TilePos_1[currentDawnPage], 1);
                problems[currentDawnPage].transform.DOScale(new Vector3(0.45f, 0.45f, 1), 1);
                yield return new WaitForSeconds(1);
                //problems[currentDawnPage].SetActive(false);

                Ans_1Bool[currentDawnPage] = 1;
                problemAns[currentDawnPage].transform.Find("Ans_1").gameObject.SetActive(true);
                if (Ans_2Bool[currentDawnPage] != 1) ProblemInit(currentDawnPage);
                shield.SetActive(false);
            }
            else if (Ans_2Bool[currentDawnPage] == 0 && spot_3PosX >= 1261)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                problems[currentDawnPage].transform.DOLocalMove(TilePos_2[currentDawnPage], 1);
                problems[currentDawnPage].transform.DOScale(new Vector3(0.45f, 0.45f, 1), 1);
                yield return new WaitForSeconds(1);
                //problems[currentDawnPage].SetActive(false);

                Ans_2Bool[currentDawnPage] = 1;
                problemAns[currentDawnPage].transform.Find("Ans_2").gameObject.SetActive(true);
                if (Ans_1Bool[currentDawnPage] != 1) ProblemInit(currentDawnPage);
                shield.SetActive(false);
            }
            else
            {
                int clip_idx = radom.Next(8, 11);
                Debug.Log("...............Random:    " + clip_idx);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clip_idx, () => { }, () =>
                {
                    shield.SetActive(false);
                    return;
                }));
            }
            if (Ans_1Bool[currentDawnPage] + Ans_2Bool[currentDawnPage] == 2)
            {
                Debug.Log("恭喜你全部完成");
                Ans_1Bool[currentDawnPage] = 0;
                Ans_2Bool[currentDawnPage] = 0;
                bg4.SetActive(true);
                shengli.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,3,false);
                float spiTime = SpineManager.instance.DoAnimation(shengli, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () =>
                {
                    SpineManager.instance.DoAnimation(shengli, "animation2", true);
                }, () =>
                {
                    backBtn.SetActive(true);
                    shield.SetActive(false);
                }, spiTime));
            }
        }

        //返回按钮点击事件
        void DoBackBtnClick(GameObject obj)
        {
            backBtn.SetActive(false);
            shengli.SetActive(false);
            okBtn.SetActive(false);
            spine_4.SetActive(true);
            okSpine.SetActive(false);
            problems[currentDawnPage].SetActive(false);
            problemAns[currentDawnPage].SetActive(false);
            problemAns[currentDawnPage].transform.Find("Ans_1").gameObject.SetActive(false);
            problemAns[currentDawnPage].transform.Find("Ans_2").gameObject.SetActive(false);
            ProblemInit(currentDawnPage);
            bg2.SetActive(false);
            bg3.SetActive(false);
            bg4.SetActive(false);
            SpineManager.instance.DoAnimation(spine_4, "animation", false);
            imgBtn.SetActive(true);
        }

        void StartDarg(Vector3 pos, int index, int type)
        {
            Debug.Log("开始拖动");
            if (talkIndex == 2)
            {
                finger.SetActive(false);
                arroe.transform.GetComponent<Image>().CrossFadeAlpha(0, 0.5f, true);
            }
        }
        void Darg(Vector3 pos, int index, int type)
        {
            Vector3 spotPos = spotDar[currentDawnPage][0].transform.localPosition;
            MovePostEvent(new Vector3(spotPos.x + 956, spotPos.y + 539.4f, 0));
        }

        void EndDarg(Vector3 pos, int index, int type, bool match)
        {
           spotDar[currentDawnPage][0].transform.localPosition = moveSpot_1[currentDawnPage].transform.localPosition;
            if (dragIndex == 1 && talkIndex == 2)
            {
                shield.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    shield.SetActive(false);
                }));
                dragIndex++;
            }
            else if (dragIndex == 2 && talkIndex == 3)
            {
                shield.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    shield.SetActive(false);
                }));
                dragIndex++;
            }
        }

        //线和点的移动
        void MovePostEvent(Vector3 pos)
        {
            //画第一条线
            double x = lineStartPos[currentDawnPage][0].x + (pos.x - lineStartPos[currentDawnPage][0].x) * (sidesR[0] / Math.Sqrt((pos.x - lineStartPos[currentDawnPage][0].x) * (pos.x - lineStartPos[currentDawnPage][0].x) + (pos.y - lineStartPos[currentDawnPage][0].y) * (pos.y - lineStartPos[currentDawnPage][0].y)));
            double y = lineStartPos[currentDawnPage][0].y + (pos.y - lineStartPos[currentDawnPage][0].y) * (sidesR[0] / Math.Sqrt((pos.x - lineStartPos[currentDawnPage][0].x) * (pos.x - lineStartPos[currentDawnPage][0].x) + (pos.y - lineStartPos[currentDawnPage][0].y) * (pos.y - lineStartPos[currentDawnPage][0].y)));
            ReDrawPic(linesVec[currentDawnPage][0], (float)x, (float)y, lineStartPos[currentDawnPage][0].x, lineStartPos[currentDawnPage][0].y);
            //移动第一个交点
            moveSpot_1[currentDawnPage].transform.localPosition = new Vector3((float)x - 956, (float)y - 539.4f, 0);
            //画第二条线
            Vector2 spotPos_2 = GetMoveSpot_2(x, y);
            ReDrawPic(linesVec[currentDawnPage][1], (float)x, (float)y, spotPos_2.x, spotPos_2.y);
            //移动第二个交点
            moveSpot_2[currentDawnPage].transform.localPosition = new Vector3(spotPos_2.x - 960, spotPos_2.y - 540, 0);
            //画第三条线
            ReDrawPic(linesVec[currentDawnPage][2], spotPos_2.x, spotPos_2.y, lineEndPos[currentDawnPage][2].x, lineEndPos[currentDawnPage][2].y);

            spot_3PosX = spotPos_2.x;
        }

        //求第二个动点
        Vector2 GetMoveSpot_2(double b_x, double b_y)
        {
            //求bd连线的长度
            double dis_x = b_x - lineEndPos[currentDawnPage][2].x;
            double dis_y = b_y - lineEndPos[currentDawnPage][2].y;
            double bd_Size = Math.Sqrt(dis_x * dis_x + dis_y * dis_y);
            //求角bdc的cos，用来计算点c垂直bd的交点e的坐标
            double cos_bdc = (sidesR[2] * sidesR[2] + bd_Size * bd_Size - sidesR[1] * sidesR[1]) / (2 * sidesR[2] * bd_Size);
            double propor = cos_bdc * sidesR[2] / bd_Size;//ed占bd的比例
            double e_x = lineEndPos[currentDawnPage][2].x - (lineEndPos[currentDawnPage][2].x - b_x) * propor;
            double e_y = lineEndPos[currentDawnPage][2].y - (lineEndPos[currentDawnPage][2].y - b_y) * propor;
            //计算ce向量的模长
            double ce_Size = Math.Abs(Math.Sqrt(sidesR[2] * sidesR[2] - (cos_bdc * sidesR[2]) * (cos_bdc * sidesR[2])));
            //通过垂直相向相乘等于0和模长公式求c点的坐标：X1 * X2 + Y1 * Y2 = 0  和  X1^2 + Y1^2 = Z^2
            double c_y = e_y + Math.Abs(Math.Sqrt((ce_Size * ce_Size) / (((lineEndPos[currentDawnPage][2].y - e_y) * (lineEndPos[currentDawnPage][2].y - e_y)) / ((lineEndPos[currentDawnPage][2].x - e_x) * (lineEndPos[currentDawnPage][2].x - e_x)) + 1)));
            double c_x = e_x - ((c_y - e_y) * (lineEndPos[currentDawnPage][2].y - e_y) / (lineEndPos[currentDawnPage][2].x - e_x));
            return new Vector2((float)c_x, (float)c_y);
        }

        //四边形的初始化
        void ProblemInit(int idx)
        {
            problems[idx].transform.localPosition = TargetStartPos[idx];
            problems[currentDawnPage].transform.localScale = Vector3.one;
            //problems[idx].SetActive(true);
            for (int i = 0; i < linesVec[idx].Count; i++)
            {
                ReDrawPic(linesVec[idx][i], lineStartPos[idx][i].x, lineStartPos[idx][i].y, lineEndPos[idx][i].x, lineEndPos[idx][i].y);
            }
            moveSpot_1[idx].transform.localPosition = new Vector3(-206, -1.4f, 0);
            moveSpot_2[idx].transform.localPosition = new Vector3(126.5f, 82, 0);
            spotDar[idx][0].gameObject.transform.localPosition = new Vector3(-206, -1.4f, 0);
            spot_3PosX = 1086;
        }

        //画线
        void ReDrawPic(Vectrosity.VectorObject2D line, float x1, float y1, float x2, float y2)
        {
            posList[0] = new Vector2(x1, y1);
            posList[1] = new Vector2(x2, y2);
            line.vectorLine.points2 = posList;
            line.vectorLine.Draw();
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            if (method_2 != null)
            {
                method_2();
            }
        }
    }
}
