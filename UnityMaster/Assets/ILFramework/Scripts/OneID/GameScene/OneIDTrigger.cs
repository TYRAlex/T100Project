using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneID
{
    public class OneIDTrigger : MonoBehaviour
    {
        public static event TriggerReceive CurrentTriggerReceive;


       

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.name == "TriggerDispear")
            {
                other.gameObject.Hide();
                return;
            }
            //Debug.LogError("22222");
            CurrentTriggerReceive?.Invoke(other.gameObject);
        }

        
    }
}