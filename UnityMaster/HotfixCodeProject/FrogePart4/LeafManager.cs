using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ILFramework;

namespace FrogePart4
{
    class LeafManager
    {
        GameObjectPool pool;
        int pooSize = 10;

        float initInterval = 3;  // 荷叶产生间隔时间
        int initMax = 3;  // 一组荷叶最大数量
        int initMin = 1;

        public Transform leavesGroup;
        public Transform leafParent;
        public bool stop = true;
        public List<Leaf> leaves = new List<Leaf>();
        public LeafManager(GameObject leafItem)
        {
            //pool = ObjectPoolManager.instance.CreatePool(ConstVariable.leafPoolName, pooSize, pooSize, leafItem);
        }

        public void GenerateLeaves()
        {
            leaves.Clear();
            leavesGroup.gameObject.SetActive(true);
            for (int i = 0; i < leavesGroup.childCount; i++)
            {
                leaves.Add(new Leaf(leavesGroup.GetChild(i).gameObject));
            }
        }

        void CreateLeaves()
        {
            int num = ConstVariable.RandDistance(initMin, initMax);
            Debug.LogFormat(" LeafManager CreateLeaves num: {0} ", num);
            for (int i = 0; i < num; i++)
            {
                GameObject go = ObjectPoolManager.instance.GetPool(ConstVariable.leafPoolName).NextAvailableObject();
                go.transform.SetParent(leafParent);
                leaves.Add(new Leaf(go) { donwComplete = RealseLeaf });
            }
        }

        public void GenerateLeaves(MonoBehaviour mono)
        {
            mono.StartCoroutine(GenerateLeavesCo());
        }

        IEnumerator GenerateLeavesCo()
        {
            while (!stop)
            {
                CreateLeaves();
                yield return new WaitForSeconds(initInterval);
            }
        }

        public void RealseLeaf(Leaf leaf)
        {
            ObjectPoolManager.instance.GetPool(ConstVariable.leafPoolName).ReturnObjectToPool(ConstVariable.leafPoolName, leaf.leafGo);

            for (int i = 0; i < leaf.froges.Count; i++)
            {
                leaf.froges[i].Revive();
            }

            leaves.Remove(leaf);
        }

        public void RealseList()
        {
            leaves.Clear();
        }
    }
}
