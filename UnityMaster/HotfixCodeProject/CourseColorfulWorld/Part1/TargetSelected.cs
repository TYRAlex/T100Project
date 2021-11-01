using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseColorfulWorldPart
{
    class TargetSelected
    {
        Transform parent;
        Dictionary<string, List<GameObject>> PointsDict;
        string[] slots;
        SkeletonAnimation target;
        List<string> frontName;
        Camera ui3d;
        public TargetSelected()
        {
            parent = CourseColorfulWorldPart1.curGo.transform.Find("UI3D/Size/zhuanpan01/SkeletonUtility-Root/root/bone/zp_0");
            PointsDict = new Dictionary<string, List<GameObject>>();
            slots = new string[parent.childCount];
            for (int i = 0;i < parent.childCount;i++)
            {
                string name = parent.GetChild(i).name;
                string curName = "";
                for(int j = 0;j < name.Length;j++)
                {
                    curName += name[j] + "_";
                }
                curName += "c";
                List<GameObject> curList = new List<GameObject>();
                for (int j = 0;j < parent.GetChild(i).childCount;j++)
                {
                    curList.Add(parent.GetChild(i).GetChild(j).gameObject);
                }
                PointsDict[curName] = curList;
                slots[i] = curName;
            }
            target = CourseColorfulWorldPart1.curGo.transform.Find("UI3D/Size/zhuanpan02").GetComponent<SkeletonAnimation>();
            ui3d = CourseColorfulWorldPart1.curGo.transform.Find("UI3D").GetComponent<Camera>();
        }
        public void ShowBlock()
        {
            FrontBlock();
            ShowSetlectdBlock();
        }
        //帅选在y轴正上方的块
        public void FrontBlock()
        {
            if(frontName == null)
            {
                frontName = new List<string>();
            }
            else
            {
                frontName.Clear();
            }
            foreach(var name in PointsDict.Keys)
            {
                Debug.Log("name----" + name);
                var curList = PointsDict[name];
                bool ison = false;
                for(int i = 0; i < curList.Count;i++)
                {
                    Vector2 front = new Vector2(0, 1);
                    Vector2 curPos = ui3d.WorldToScreenPoint(curList[i].transform.position);
                    curPos = curPos - ZhuanpanClass.center;
                    float angle = Vector2.Angle(front, curPos);
                    if(angle >= 90)
                    {
                        ison = true;
                        break;
                    }
                }
                if(ison == false)
                {
                    frontName.Add(name);
                }
            }
        }
        //通过计算在x轴方向的角度，左右两个点的取值范围包含90度是那一个块
        public string GetBlockName()
        {
            for(int i = 0;i < frontName.Count;i++)
            {
                List<float> angles = new List<float>();
                for(int j = 0;j < 2; j++)
                {                   
                    Vector2 front = new Vector2(1, 0);
                    Vector2 curPos = ui3d.WorldToScreenPoint(PointsDict[frontName[i]][j].transform.position);
                    curPos = curPos - ZhuanpanClass.center;
                    float angle = Vector2.Angle(front, curPos);
                    angles.Add(angle);
                }
                if ((angles[0] > 90 && angles[1] < 90)|| (angles[0] < 90 && angles[1] > 90))
                {
                    return frontName[i];
                }
            }
            return "";
        }
        public string ShowSetlectdBlock()
        {
            string curName = GetBlockName();
            if (curName == "") return "";
           
            for(int i = 0;i < slots.Length;i++)
            {
                if(slots[i] != curName)
                {
                    SpineManager.instance.HideSpineTexture(target, slots[i]);
                }
                else
                {
                    SpineManager.instance.ShowSpineTexture(target, slots[i]);
                }
            }
            target.transform.localScale = Vector3.one;
            curName = curName.Split('_')[0] == "l" ? "right" : "left";
            return curName;
        }
    }
}
