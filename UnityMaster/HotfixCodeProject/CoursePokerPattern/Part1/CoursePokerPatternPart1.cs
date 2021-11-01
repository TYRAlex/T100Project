using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CoursePokerPatternPart1
    {
        Vector2 centerPos;
        Vector2 lastDir, currentDir;
        bool isClickImg;
        GameObject curGo, uiText, gamePart, pokerSpine, pokerImg, npc, Points;
        GameObject curPoker, curPoker_D, poker_J, poker_Q, poker_K;
        GameObject[] points;
        List<GameObject> pokers;
        int curPokerIndex = 0;
        Transform PokersPa;
        Transform Btns;
        GameObject LeftBtn, RightBtn;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            uiText = curTrans.Find("gameScene/uiText").gameObject;
            gamePart = curTrans.Find("gameScene/gamePart").gameObject;
            pokerSpine = gamePart.transform.Find("pokerSpine").gameObject;

            // pokerImg = gamePart.transform.Find("pokerImg").gameObject;
            PokersPa = gamePart.transform.Find("Pokers");
            poker_J = PokersPa.Find("Poker_J").gameObject;
            poker_Q = PokersPa.Find("Poker_Q").gameObject;
            poker_K = PokersPa.Find("Poker_K").gameObject;
            pokers = new List<GameObject>() { poker_J, poker_Q, poker_K };

            Btns = gamePart.transform.Find("Btns");
            LeftBtn = Btns.Find("Left").gameObject;
            RightBtn = Btns.Find("Right").gameObject;

            Points = gamePart.transform.Find("Points").gameObject;
            points = curTrans.GetChildren(Points);
            npc = gamePart.transform.Find("npc").gameObject;
            centerPos = poker_J.transform.position;
            isClickImg = true;
            //Util.AddBtnClick(uiText, SceneInit);

            InitGame();

        }

        void InitGame()
        {

            curPokerIndex = 0;
            curPoker = null;
            curPoker_D = null;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = 0.3f;
            ResetPokers();

            LeftBtn.SetActive(false);
            RightBtn.SetActive(false);
            LeftBtn.GetComponent<ILObject3DAction>().OnPointDownLua = LeftBtnCallBack;
            RightBtn.GetComponent<ILObject3DAction>().OnPointDownLua = RightBtnCallBack;


            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                PlayPokerAnimation(pokers[curPokerIndex]);
            });

        }

        private void RightBtnCallBack(int obj)
        {
            if (curPokerIndex < 2)
            {
                LeftBtn.SetActive(false);
                RightBtn.SetActive(false);
                ResetPokers();
                curPokerIndex++;
                PlayPokerAnimation(pokers[curPokerIndex], false);
            }
        }

        private void LeftBtnCallBack(int obj)
        {
            if (curPokerIndex > 0)
            {
                LeftBtn.SetActive(false);
                RightBtn.SetActive(false);
                ResetPokers();
                curPokerIndex--;
                PlayPokerAnimation(pokers[curPokerIndex], false);
            }
        }

        void ResetPokers()
        {
            for (int i = 0; i < PokersPa.childCount; i++)
            {
                var poker = PokersPa.GetChild(i).gameObject;
                poker.SetActive(false);
                Debug.Log(poker.name);
                var ALL = poker.transform.Find("ALL").gameObject;
                var ALL_D = poker.transform.Find("ALL_D").gameObject;
                var Poker_U = poker.transform.Find("Poker_U").gameObject;
                var Poker_D = poker.transform.Find("Poker_D").gameObject;

                ALL_D.transform.localEulerAngles = Vector3.zero;
                Poker_D.transform.localEulerAngles = Vector3.zero;

                ALL.SetActive(false);
                ALL_D.SetActive(false);
                Poker_U.SetActive(true);
                Poker_D.SetActive(true);

            }

        }

        void OpenALL_DPoker()
        {
            var ALL = curPoker.transform.Find("ALL").gameObject;
            var ALL_D = curPoker.transform.Find("ALL_D").gameObject;
            var Poker_U = curPoker.transform.Find("Poker_U").gameObject;
            var Poker_D = curPoker.transform.Find("Poker_D").gameObject;

            Poker_U.SetActive(false);
            Poker_D.SetActive(false);
            ALL.SetActive(true);
            ALL_D.SetActive(true);

            curPoker_D = ALL_D;
        }


        void PlayPokerAnimation(GameObject poker, bool isFirst = true)
        {
            curPoker = poker;
            if (isFirst)
            {
                SpineManager.instance.DoAnimation(pokerSpine, "animation", false, (Action)(() =>
                {
                    SpineManager.instance.DoAnimation(pokerSpine, "animation");
                    curPoker.SetActive(true);
                    curPoker_D = curPoker.transform.Find("Poker_D").gameObject;
                }));
            }
            else
            {
                curPoker.SetActive(true);
                curPoker_D = curPoker.transform.Find("Poker_D").gameObject;
            }
        }

        void ChangePoker(GameObject poker_D)
        {
            float rotation_Z = Mathf.Abs(poker_D.transform.rotation.z);
            // int z = Convert.ToInt32(rotation_Z);
            if (poker_D.name == "Poker_D")
            {
                //转全身牌
                if (rotation_Z >= 0.98f)
                {
                    OpenALL_DPoker();
                    // Debug.Log("转全身牌----" + rotation_Z);
                }

            }
            else if (poker_D.name == "ALL_D")
            {
                if (rotation_Z >= 0.98f)
                {
                    if (curPokerIndex == 0)
                    {
                        LeftBtn.SetActive(false);
                        RightBtn.SetActive(true);
                    }
                    else if (curPokerIndex == 2)
                    {
                        LeftBtn.SetActive(true);
                        RightBtn.SetActive(false);
                    }
                    else
                    {
                        LeftBtn.SetActive(true);
                        RightBtn.SetActive(true);
                    }
                }

                // Debug.Log("转下一张牌");
            }
        }

        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                if (isPointInRect(pos))
                {
                    isClickImg = true;
                    lastDir = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - centerPos;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                }
            }
            if (isClickImg && curPoker_D != null)
            {
                currentDir = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - centerPos;
                float d = Vector2.Dot(lastDir.normalized, currentDir.normalized);
                float degree = Mathf.Acos(d) * Mathf.Rad2Deg;
                Vector3 cross = Vector3.Cross(lastDir.normalized, currentDir.normalized);

                if (cross.z > 0)
                {
                    curPoker_D.transform.rotation *= Quaternion.Euler(0, 0, degree);
                }
                else if (cross.z < 0)
                {
                    curPoker_D.transform.rotation *= Quaternion.Euler(0, 0, -degree);
                }

                ChangePoker(curPoker_D);
                // Debug.Log(curPoker_D.transform.rotation);

                lastDir = currentDir;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isClickImg = false;
            }
        }

        bool isPointInRect(Vector2 pos)
        {
            Vector2 A = points[0].transform.position;
            Vector2 B = points[1].transform.position;
            Vector2 C = points[2].transform.position;
            Vector2 D = points[3].transform.position;
            float a = (B.x - A.x) * (pos.y - A.y) - (B.y - A.y) * (pos.x - A.x);
            float b = (C.x - B.x) * (pos.y - B.y) - (C.y - B.y) * (pos.x - B.x);
            float c = (D.x - C.x) * (pos.y - C.y) - (D.y - C.y) * (pos.x - C.x);
            float d = (A.x - D.x) * (pos.y - D.y) - (A.y - D.y) * (pos.x - D.x);
            if ((a > 0 && b > 0 && c > 0 && d > 0) || (a < 0 && b < 0 && c < 0 && d < 0))
            {
                return true;
            }

            //      AB X AP = (b.x - a.x, b.y - a.y) x (p.x - a.x, p.y - a.y) = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
            //      BC X BP = (c.x - b.x, c.y - b.y) x (p.x - b.x, p.y - b.y) = (c.x - b.x) * (p.y - b.y) - (c.y - b.y) * (p.x - b.x);
            return false;
        }
    }
}
