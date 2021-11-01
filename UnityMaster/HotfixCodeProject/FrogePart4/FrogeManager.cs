using System;
using System.Collections.Generic;
using UnityEngine;
using ILFramework;

namespace FrogePart4
{
    class FrogeManager
    {
        LeafManager leafManager;

        int landColor = 4; // 黄色卡片 跳上岸
        int landIndex = -1;  // 没有荷叶落下，跳岸上
        Vector3 landPos = new Vector3(0, 0, 0);  // 落岸位置
        public Dictionary<Vector3, bool> endPoints = new Dictionary<Vector3, bool>();
        Vector3 invaildPos = new Vector3(-1000, -1000, -1000);

        public FrogeManager(LeafManager m)
        {
            leafManager = m;
        }
        
        public Vector3 GetLandPos()
        {
            foreach (var item in endPoints)
            {
                if (!item.Value)
                {
                    endPoints[item.Key] = true;
                    return item.Key;
                }
            }
            return invaildPos;
        }

        public Leaf GetTakeOffPos(ConstVariable.LColor c, Froge froge)
        {
            Vector3 leafPos;
            Vector3 frogPos = froge.curLeaf == null ? froge.FrogeGo.transform.localPosition : froge.curLeaf.leafGo.transform.localPosition;

            Vector3 tempPos = invaildPos;
            Leaf leaf = null;

            var leaves = leafManager.leaves;

            for (int i = 0; i < leaves.Count; i++)
            {
                leafPos = leaves[i].Position;

                if (leafPos == frogPos)
                    continue;

                if (Vector3.Distance(frogPos, leafPos) <= froge.radius)
                {
                    if (leaves[i].LeafColor != c)
                        continue;
                    if (tempPos == invaildPos)
                    {
                        tempPos = leaves[i].Position;
                        leaf = leaves[i];
                    }
                    else
                    {
                        float tDis = Vector3.Distance(tempPos, frogPos);
                        float cDis = Vector3.Distance(leaves[i].Position, frogPos);
                        if (tDis > cDis)
                        {
                            tempPos = leaves[i].Position;
                            leaf = leaves[i];
                        }
                    }
                }
            }

            if (leaf != null)
            {
                if (leaf.froges.Count == leaf.maxFrogeNum)
                    leaf = null;
                else
                {
                    if (froge.curLeaf != null)
                        froge.curLeaf.froges.Remove(froge);
                    froge.curLeaf = leaf;
                    leaf.froges.Add(froge);
                }
            }

            return leaf;
        }

        int GetCloserLeafIndex(Vector3 frogPos, List<Leaf> leaves)
        {
            float dis, tempDis = 0;
            int minIndex = landIndex;

            for (int i = 0; i < leaves.Count; i++)
            {
                dis = leaves[i].Position.y - frogPos.y;
                //Debug.LogFormat(" GetCloserLeafIndex i: {0} , dis {1}", i, dis);
                if (dis <= 0)
                    continue;
                if (tempDis == 0)
                {
                    tempDis = dis;
                    minIndex = i;
                }
                else
                {
                    if (dis < tempDis)
                    {
                        tempDis = dis;
                        minIndex = i;
                    }
                }
            }
            return minIndex;
        }

        public Vector3 GetNextFrogPos(Froge frog)
        {
            int idx = GetCloserLeafIndex(frog.Position, leafManager.leaves);
            if (idx == landIndex)
            {
                Debug.LogFormat(" 青蛙: {0} 成功上岸！", frog.Name);
                return landPos;
            }
            else
            {
                Leaf l = leafManager.leaves[idx];
                if (l.froges.Count == l.maxFrogeNum)
                {
                    Debug.LogFormat(" 青蛙{0}落水: 满了, 坐不下了! ", frog.Name);
                    frog.fallDown = true;
                }
                else
                {
                    if (frog.curLeaf != null)
                        frog.curLeaf.froges.Remove(frog);

                    frog.curLeaf = l;
                    l.froges.Add(frog);
                }
                return l.Position;
            }
        }

        public bool VertifyColor(Froge frog, int color)
        {
            int idx = GetCloserLeafIndex(frog.Position, leafManager.leaves);
            if(idx == landIndex)
                return color == landColor;

            var c = ConstVariable.GetColorByInt(color);

            return leafManager.leaves[idx].LeafColor == c;
        }

    }
}
