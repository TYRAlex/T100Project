using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course215Part1
    {
        private GameObject _men;
        private GameObject bell;
        private GameObject robot;
        private GameObject spine;
        private GameObject blackBoard;
        private ILDrager drager;
        private GameObject moveSpot;
        private GameObject moveCube;
        private GameObject move;
        private GameObject bg2;
        private GameObject imgBtn;
        private Vectrosity.VectorObject2D vectorLine_1;
        private Vectrosity.VectorObject2D vectorLine_2;


        private int talkIndex;
        private Vector3 startPos;
        private double[] sidesR;
        private Vector3[] LinePos;
        private List<Vector2> posList;
        private string[] spiName;
        private int[] clipIndex;
        private bool isTalk;

        private Image _qubing;    //曲柄
        private Image _liangan;   //连杆
        private Image _huakuai;   //滑块
        private Image _jijia;     //机架

        private MonoBehaviour mono;
        GameObject curGo;

   
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            

            _men = curTrans.GetGameObject("bg/Image");
            bell = curTrans.Find("bell").gameObject;
            robot = curTrans.Find("Robot").gameObject;
            blackBoard = curTrans.Find("BlackBoard").gameObject;
            drager = curTrans.Find("Move/Drag").GetComponent<ILDrager>();
            vectorLine_1 = curTrans.Find("Move/VectorCanvas/Line_1").GetComponent<Vectrosity.VectorObject2D>();
            vectorLine_2 = curTrans.Find("Move/VectorCanvas/Line_2").GetComponent<Vectrosity.VectorObject2D>();
            moveSpot = curTrans.Find("Move/MoveSpot").gameObject;
            moveCube = curTrans.Find("Move/MoveCube").gameObject;
            move = curTrans.Find("Move").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            spine = curTrans.Find("Spine").gameObject;

            Button[] btn = imgBtn.transform.GetComponentsInChildren<Button>();
            for(int i = 0; i < btn.Length; i++)
            {
                Util.AddBtnClick(btn[i].gameObject, DoImgBtnClick);
            }

            drager.transform.GetComponent<ILDrager>().SetDragCallback(StartDrag, Drag, EndDrag);
            mono = curGo.GetComponent<MonoBehaviour>();

            _qubing = curTrans.GetImage("Spine/qubing");
            _liangan = curTrans.GetImage("Spine/liangan");
            _huakuai = curTrans.GetImage("Spine/huakuai");
            _jijia = curTrans.GetImage("Spine/jijia");

            _qubing.DOFade(0, 0.1F);
            _liangan.DOFade(0,0.1F);
            _huakuai.DOFade(0, 0.1F);
            _jijia.DOFade(0, 0.1F);

            GameInit();
        }


        void TxtAni()
        {
            
        }


        void GameInit()
        {

            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            talkIndex = 1;
            isTalk = false;
            _men.Show();
            bell.SetActive(true);
            robot.SetActive(true);
            blackBoard.SetActive(false);
            move.SetActive(false);
            spine.SetActive(false);
            bg2.SetActive(false);
            drager.isActived = false;
            Vector3[] LinePos_test = {new Vector3(700,570,0),new Vector3(1130,570,0)};
            LinePos = LinePos_test;
            string[] spiName_test = {"qb6","qb5","qb4","qb7" };
            spiName = spiName_test;
            int[] clipIndex_test = { 5, 6, 8, 7 };
            clipIndex = clipIndex_test;
            startPos = new Vector3(770, 706, 0);
            InitSidesR();
            posList = new List<Vector2>();
            posList.Add(new Vector2(770, 706));
            posList.Add(new Vector2(700, 570));

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void InitSidesR()
        {
            sidesR = new double[2];
            for (int i = 0; i < sidesR.Length; i++)
            {
                sidesR[i] = Vector3.Distance(startPos, LinePos[i]);
            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            SpineManager.instance.DoAnimation(robot, "jqr", true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }
        
        //右边说话按钮点击事件
        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                float time_0 = SpineManager.instance.DoAnimation(robot, "jqr2", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                {
                    SpineManager.instance.DoAnimation(robot, "jqr", true);                    
                }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }, time_0));
            }
            else if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                {
                    _men.Hide();
                    robot.SetActive(false);
                    blackBoard.SetActive(true);
                }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 4)
            {
                blackBoard.SetActive(false);
                spine.SetActive(true);
                bg2.SetActive(true);
                SpineManager.instance.DoAnimation(spine, "qb", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () =>
                {
                    imgBtn.SetActive(true);
                }));
            }
            else if (talkIndex == 5)
            {
                imgBtn.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, () => { }, () =>
                {
                    move.SetActive(true);
                    spine.SetActive(false);
                    drager.isActived = true;
                }));
            }else if(talkIndex == 6)
            {
                drager.isActived = false;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () => { }, () =>
                {
                    drager.isActived = true;
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 7)
            {
                drager.isActived = false;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () => { }, () =>
                {
                    drager.isActived = true;
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 8)
            {
                drager.isActived = false;
                spine.SetActive(true);
                move.SetActive(false);
                float spiTime = SpineManager.instance.DoAnimation(spine, "qbz", true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, () =>
                {
                    SpineManager.instance.DoAnimation(spine, "qb", true);
                }, () =>
                {
                    LineInit();
                    spine.SetActive(false);
                    move.SetActive(true);
                    drager.isActived = true;
                }, 2 * spiTime));
            }
            talkIndex++;
        }

        void StartDrag(Vector3 pos,int type,int index)
        {
        }

        void Drag(Vector3 pos,int type,int index)
        {
            float x = drager.transform.localPosition.x;
            float y = drager.transform.localPosition.y;
            Vector3 drgaPos = new Vector3(x + 965, y + 541, 0);
            GetTempPos(drgaPos);
        }

        void EndDrag(Vector3 pos,int type,int index,bool isMatch)
        {
            float x = moveSpot.transform.localPosition.x - 965;
            float y = moveSpot.transform.localPosition.y - 541;
            drager.transform.localPosition = new Vector3(x, y, 0);
            if (isTalk)
            {
                isTalk = false;
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        void DoImgBtnClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.SetShield(false);
            int idx = int.Parse(obj.name);
            imgBtn.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex[idx], () => {
                SpineManager.instance.DoAnimation(spine, spiName[idx], false);
                TextTwAni(idx, true);



            }, () => {
                TextTwAni(idx, false);
                imgBtn.SetActive(true);
                if (!isTalk)
                {
                    isTalk = true;
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                SoundManager.instance.SetShield(true);
            }));
            
        }


        void TextTwAni(int idx,bool isShow)
        {
            int endValue = 0;

            if (isShow)            
                endValue = 1;
            

            switch (idx)
            {
                case 0:
                    _qubing.DOFade(endValue, 0.3f);
                    break;
                case 1:
                    _liangan.DOFade(endValue, 0.3f);
                    break;
                case 2:
                    _jijia.DOFade(endValue, 0.3f);
                    break;
                case 3:
                    _huakuai.DOFade(endValue, 0.3f);
                    break;
            }
        }

        //计算线各个点的位置，用来画线
        void GetTempPos(Vector3 pos)
        {
            //求两线交点的位置
            double x = LinePos[0].x + (pos.x - LinePos[0].x) * (sidesR[0] / Math.Sqrt((pos.x - LinePos[0].x) * (pos.x - LinePos[0].x) + (pos.y - LinePos[0].y) * (pos.y - LinePos[0].y)));
            double y = LinePos[0].y + (pos.y - LinePos[0].y) * (sidesR[0] / Math.Sqrt((pos.x - LinePos[0].x) * (pos.x - LinePos[0].x) + (pos.y - LinePos[0].y) * (pos.y - LinePos[0].y)));
            startPos = new Vector3((float)x,(float)y,0);
            //求移动方块点的位置
            double sizeX = Math.Sqrt(sidesR[1] * sidesR[1] - (startPos.y - LinePos[1].y) * (startPos.y - LinePos[1].y));
            double x2 = startPos.x + sizeX;
            LinePos[1] = new Vector3((float)x2, 570, 0);
            //画线
            ReDrawPic();
            //移动圆点的位置
            moveSpot.transform.localPosition = startPos;
            //移动方块的位置
            moveCube.transform.localPosition = LinePos[1];
        }

        //画线
        void ReDrawPic()
        {
            posList[0] = new Vector2(startPos.x, startPos.y);
            posList[1] = new Vector2(LinePos[0].x, LinePos[0].y);
            vectorLine_2.vectorLine.points2 = posList;
            vectorLine_2.vectorLine.Draw();
            posList[1] = new Vector2(LinePos[1].x, LinePos[1].y);
            vectorLine_1.vectorLine.points2 = posList;
            vectorLine_1.vectorLine.Draw();
        }

        //图形初始化
        void LineInit()
        {
            moveCube.transform.localPosition = new Vector3(1130, 570, 0);
            moveSpot.transform.localPosition = new Vector3(770, 706, 0);
            drager.gameObject.transform.localPosition = new Vector3(-195, 165, 0);
            posList[0] = new Vector2(770, 706);
            posList[1] = new Vector2(700, 570);
            vectorLine_2.vectorLine.points2 = posList;
            vectorLine_2.vectorLine.Draw();
            posList[1] = new Vector2(1130, 570);
            vectorLine_1.vectorLine.points2 = posList;
            vectorLine_1.vectorLine.Draw();
        }

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
            SoundManager.instance.SetShield(true);

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
