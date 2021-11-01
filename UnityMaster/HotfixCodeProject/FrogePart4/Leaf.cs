using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ILFramework;

namespace FrogePart4
{
    class Leaf
    {
        public GameObject leafGo;

        public Rigidbody2D rigibody;
        public float speed = 30;
        public GameObject leafSpine;
        public CircleCollider2D leafCollider;

        private ConstVariable.LColor leafColor;
        public ConstVariable.LColor LeafColor // 荷叶颜色
        {
            set
            {
                leafColor = value;
                leafAnis = ConstVariable.GetSpineByColor(value);
                if (leafSpine != null)
                    SpineManager.instance.DoAnimation(leafSpine, leafAnis[0]);
                if (leafCollider != null)
                    leafCollider.radius = ConstVariable.GetCircleRadius(leafAnis[1]);
            }
            get { return leafColor; }
        }

        public Vector3 Position // 荷叶位置
        {
            set { leafGo.transform.localPosition = value; }
            get { return leafGo.transform.localPosition; }
        }

        public List<Froge> froges = new List<Froge>();
        //public int frogeIndex;    // 当前荷叶上青蛙索引
        public int maxFrogeNum = 1;   // 荷叶承受最多青蛙数量
        public string[] leafAnis;  // 当前荷叶的动画 0-待机动画 1-跳跃动画

        // 荷叶随机生成区域边界值
        Vector3 lPos = new Vector3(-500, 650, 0);
        Vector3 rPos = new Vector3(560, 650, 0);
        // 下降距离
        Vector3 downDis = new Vector3(0, 900, 0);
        // 下落持续时间
        float duration = 10;
        public Action<Leaf> donwComplete = null;

        public Leaf(GameObject leaf)
        {
            leafGo = leaf;
            rigibody = leaf.GetComponent<Rigidbody2D>();
            leafSpine = leaf.transform.Find("leafspine").gameObject;
            leafCollider = leaf.GetComponent<CircleCollider2D>();
            froges.Clear();
            //CreateLeaf();
            CreateLeaf2();
        }

        public void CreateLeaf2()
        {
            Vector2 dir = RandDirction();
            LeafColor = ConstVariable.RandColor();
            Debug.LogFormat(" StartMove dir {0} , color : {1}", dir, leafColor);
            rigibody.velocity = dir * speed;
        }

        public void CreateLeaf()
        {
            float x = ConstVariable.RandDistance((int)lPos.x, (int)rPos.x);
            LeafColor = ConstVariable.RandColor();
            Position = new Vector3(x, lPos.y, 0);
            Tweener moveCo = leafGo.transform.DOLocalMove(Position - downDis, duration).SetEase(Ease.Linear);

            moveCo.OnUpdate(() =>
            {
                for (int i = 0; i < froges.Count; i++)
                {
                    if (!froges[i].takeOff)
                        froges[i].FrogeGo.transform.localPosition = Position;
                }
            });

            moveCo.OnComplete(() =>
            {
                donwComplete?.Invoke(this);
            });
        }

        Color SwitchColor(ConstVariable.LColor color)
        {
            switch (color)
            {
                case ConstVariable.LColor.Blue:
                    return Color.blue;
                case ConstVariable.LColor.Green:
                    return Color.green;
                case ConstVariable.LColor.Red:
                    return Color.red;
                default:
                    Debug.LogErrorFormat(" color: {0} is invaild! ", color);
                    return Color.black;
            }
        }

        Vector2 RandDirction()
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            int num = UnityEngine.Random.Range(0, 8);
            switch (num)
            {
                case 0:
                    return Vector2.left;
                case 1:
                    return Vector2.right;
                case 2:
                    return Vector2.up;
                case 3:
                    return Vector2.down;
                case 4:
                    return Vector2.one;
                case 5:
                    return Vector2.one * -1;
                case 6:
                    return new Vector2(1, -1);
                case 7:
                    return new Vector2(-1, 1);
                default:
                    return Vector2.zero;
            }
        }
    }
}
