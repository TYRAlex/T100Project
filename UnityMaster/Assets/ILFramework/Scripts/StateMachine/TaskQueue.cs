using ILFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ILFramework
{
    public class TaskQueue
    {
        private Queue<MethodInfo> m_TaskQueue;
        private MethodInfo[] m_TaskQueue_copy;
        private int m_TasksNum = 0;
        public Action OnStart = null;
        public Action OnFinish = null;
        public int currentIndex = 0;
        public TaskQueue()
        {
            m_TaskQueue = new Queue<MethodInfo>();
            m_TasksNum = 0;
        }
        public void AddTaskList(MethodInfo[] workList)
        {
            m_TaskQueue_copy = new MethodInfo[workList.Length];
            for (int i = 0; i < workList.Length; i++)
            {
                m_TaskQueue.Enqueue(workList[i]);
            }
            m_TaskQueue.CopyTo(m_TaskQueue_copy, 0);
        }

        public void CheckTask(TaskEventArgs e)
        {
            NextTask();
        }
        public void SkipTask( TaskEventArgs e)
        {
            m_TaskQueue.Clear();
            for (int i = e.task_id; i < m_TaskQueue_copy.Length; i++)
            {
                m_TaskQueue.Enqueue(m_TaskQueue_copy[i]);
            }
            currentIndex = e.task_id;
            NextTask();
        }
        private void NextTask()
        {
            if (m_TaskQueue.Count > 0)
            {
                MethodInfo info = m_TaskQueue.Dequeue();
                ClassCopyManager.instance.OnInvoke(currentIndex++);
            }
            else
            {
                if (OnFinish != null)
                {
                    OnFinish();
                }
                Destroy();
            }
        }
        public void Start()
        {
            m_TasksNum = m_TaskQueue.Count;
            if (OnStart != null)
            {
                OnStart();
            }
            NextTask();
        }
        public void End()
        {
            TaskEventArgs e = new TaskEventArgs();
            CheckTask(e);
        }
        public void SkipBackId(int _id)
        {
            TaskEventArgs e = new TaskEventArgs();
            e.task_id = _id;
            SkipTask(e);
        }
        public void Clear()
        {
            m_TaskQueue.Clear();
            m_TasksNum = 0;
            m_TaskQueue_copy = null;
            OnStart = null;
            OnFinish = null;
            currentIndex = 0;
        }
        public void Destroy()
        {
            Clear();
        }
    }
}
