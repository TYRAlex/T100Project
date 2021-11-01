using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ILFramework
{
    public class Manager<T>:Base, IManager
    {
        private static T _instance;
        public static T instance
        {
            get 
            {
                return _instance; 
            }
        }

        private void Awake()
        {
            _instance = GetComponent<T>();
        }
    }
}
