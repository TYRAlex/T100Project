using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ILFramework
{
    public class BellLoom : MonoBehaviour
    {

        struct NoDelayItem
        {
            public Action<object> action;
            public object param;
        }

        static List<NoDelayItem> noDelayActions = new List<NoDelayItem>();
        static List<NoDelayItem> curNoDelayActions = new List<NoDelayItem>();

        public static void QueueActions(Action<object> ac, object pars)
        {
            lock (noDelayActions)
            {
                noDelayActions.Add(new NoDelayItem
                {
                    action = ac,
                    param = pars,
                });
            }
        }

        private void Update()
        {
            if (noDelayActions.Count > 0)
            {
                lock (noDelayActions)
                {
                    curNoDelayActions.Clear();
                    curNoDelayActions.AddRange(noDelayActions);
                    noDelayActions.Clear();
                }
                for (int i = 0; i < curNoDelayActions.Count; i++)
                {
                    curNoDelayActions[i].action(curNoDelayActions[i].param);
                }
            }
        }
    }
}
