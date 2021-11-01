using CourseLeapOfSalmanPart1;
using DG.Tweening;
using LuaFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseLeapOfSalmanPart1
    {
        GameObject curGo;
        GameObject npc;
        GameObject[] level;

        SalmonPart1_Scene0 scene0;
        SalmonPart1_Scene1 scene1;
        SalmonPart1_Scene2 scene2;
        SalmonPart1_Scene3 scene3;
        SalmonPart1_Scene4 scene4;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            
            npc = curTrans.Find("npc").gameObject;
            level = GetChildren(curTrans.Find("gameScene").gameObject);
            Init();
            Level(0);
        }

        void Init()
        {
            SoundManager.instance.voiceBtn.SetActive(false);
        }

        void Update()
        {
            CheckLevel();
        }

        void CheckLevel()
        {
            if (scene0 !=null && scene0.isEnd)
            {
                Level(1);
                scene0.isEnd = false;
                scene0 = null;
            }
            else if (scene1 != null && scene1.isEnd)
            {
                Level(2);
                scene1.isEnd = false;
                scene1 = null;
            }
            else if (scene2 != null && scene2.isEnd)
            {
                Level(3);
                scene2.isEnd = false;
                scene2 = null;
            }
            else if (scene3 != null && scene3.isEnd)
            {
                Level(4);
                scene3.isEnd = false;
                scene3 = null;
            }
        }

        void Level(int level)
        {
            ShowLevel(level);

            switch (level)
            {
                case 0:
                    scene0 = new SalmonPart1_Scene0(curGo);
                    break;
                case 1:
                    scene1 = new SalmonPart1_Scene1(curGo);
                    break;
                case 2:
                    scene2 = new SalmonPart1_Scene2(curGo);
                    break;
                case 3:
                    scene3 = new SalmonPart1_Scene3(curGo);
                    break;
                case 4:
                    scene4 = new SalmonPart1_Scene4(curGo);
                    break;
            }
        }

        void ShowLevel(int iLevel)
        {
            for (int i = 0; i < level.Length; i++)
            {
                if (i == iLevel)
                {
                    level[i].SetActive(true);
                }
                else
                {
                    level[i].SetActive(false);
                }
            }
        }

        public GameObject[] GetChildren(GameObject father)
        {
            GameObject[] children = new GameObject[father.transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = father.transform.GetChild(i).gameObject;
            }
            return children;
        }


    }
}
