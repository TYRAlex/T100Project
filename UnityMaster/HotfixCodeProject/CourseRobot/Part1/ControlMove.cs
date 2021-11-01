using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseRobotPart
{
    class ControlMove
    {
        private Transform[] points;
        private Vector3 startPoint;
        private Transform target;
        private Transform targetPoint;
        private float speed = 2.0f;
        int index = 0;
        bool isLoop;
        bool isStart;
        public ControlMove(Transform parent,Transform _target,bool loop = true)
        {
            target = _target;
            points = new Transform[parent.childCount];
            for (int i = 0; i < parent.childCount;i++)
            {
                points[i] = parent.GetChild(i).transform;
            }
            targetPoint = points[index];
            startPoint = _target.position;
            index = 0;
            isStart = true;
            isLoop = loop;
        }
        public void Resat()
        {
            target.position = startPoint;
        }
        public void Update()
        {
            if(isStart)
            {
                target.position = Vector3.MoveTowards(target.position, targetPoint.position, speed * Time.deltaTime);
                if (target.position == targetPoint.position)
                {
                    index++;
                    if (!isLoop)
                    {
                        if(index == points.Length)
                        {
                            isStart = false;
                            return;
                        }
                    }
                    index = index == points.Length ? 0 : index;
                    targetPoint = points[index];
                }
            }
           
        }
    }
}
