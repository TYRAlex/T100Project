using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace ILFramework
{
    public class ILObject3DAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public int index = 0;
        public Action<Collision, int> OnCollisionEnterLua;
        public Action<Collision2D, int> OnCollisionEnter2DLua;
        public Action<Collision, int> OnCollisionExitLua;
        public Action<Collision2D, int> OnCollisionExit2DLua;
        public Action<Collision, int> OnCollisionStayLua;
        public Action<Collision2D, int> OnCollisionStay2DLua;

        public Action<int> OnMouseDownLua;
        public Action<int> OnMouseDragLua;
        public Action<int> OnMouseEnterLua;
        public Action<int> OnMouseExitLua;
        public Action<int> OnMouseOverLua;
        public Action<int> OnMouseUpLua;
        public Action<GameObject, int> OnParticleCollisionLua;
        public Action<int> OnParticleTriggerLua;

        public Action<Collider, int> OnTriggerEnterLua;
        public Action<Collider2D, int> OnTriggerEnter2DLua;
        public Action<Collider, int> OnTriggerStayLua;
        public Action<Collider2D, int> OnTriggerStay2DLua;
        public Action<Collider, int> OnTriggerExitLua;
        public Action<Collider2D, int> OnTriggerExit2DLua;

        public Action<int> OnPointUpLua;
        public Action<int> OnPointDownLua;
        // 当此碰撞器/刚体开始接触另一个刚体/碰撞器时，调用 OnCollisionEnter
        private void OnCollisionEnter(Collision collision)
        {
            if (OnCollisionEnterLua != null)
            {
                OnCollisionEnterLua.Invoke(collision, index);
            }
        }

        // 当此碰撞器 2D/刚体 2D 开始接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionEnter2D (仅限 2D 物理)
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (OnCollisionEnter2DLua != null)
            {
                OnCollisionEnter2DLua.Invoke(collision, index);
            }
        }

        // 当此碰撞器/刚体停止接触另一刚体/碰撞器时调用 OnCollisionExit
        private void OnCollisionExit(Collision collision)
        {
            if (OnCollisionExitLua != null)
            {
                OnCollisionExitLua.Invoke(collision, index);
            }
        }

        // 当此碰撞器 2D/刚体 2D 停止接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionExit2D (仅限 2D 物理)
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (OnCollisionExit2DLua != null)
            {
                OnCollisionExit2DLua.Invoke(collision, index);
            }
        }

        // 每当此碰撞器/刚体接触到刚体/碰撞器时，OnCollisionStay 将在每一帧被调用一次
        private void OnCollisionStay(Collision collision)
        {
            if (OnCollisionStayLua != null)
            {
                OnCollisionStayLua.Invoke(collision, index);
            }
        }

        // 每当碰撞器 2D/刚体 2D 接触到刚体 2D/碰撞器 2D 时，OnCollisionStay2D 将在每一帧被调用一次(仅限 2D 物理)
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (OnCollisionStay2DLua != null)
            {
                OnCollisionStay2DLua.Invoke(collision, index);
            }
        }

        // 当用户在 GUIElement 或碰撞器上按鼠标按钮时调用 OnMouseDown
        private void OnMouseDown()
        {
            if (OnMouseDownLua != null)
            {
                OnMouseDownLua.Invoke(index);
            }
        }

        // 当用户在 GUIElement 或碰撞器上单击鼠标并保持按住鼠标时调用 OnMouseDrag
        private void OnMouseDrag()
        {
            if (OnMouseDragLua != null)
            {
                OnMouseDragLua.Invoke(index);
            }
        }

        // 当鼠标进入 GUIElement 或碰撞器时调用 OnMouseEnter
        private void OnMouseEnter()
        {
            if (OnMouseEnterLua != null)
            {
                OnMouseEnterLua.Invoke(index);
            }
        }

        // 当鼠标不再停留在 GUIElement 或碰撞器上时调用 OnMouseExit
        private void OnMouseExit()
        {
            if (OnMouseExitLua != null)
            {
                OnMouseExitLua.Invoke(index);
            }
        }

        // 当鼠标停留在 GUIElement 或碰撞器上时每帧都调用 OnMouseOver
        private void OnMouseOver()
        {
            if (OnMouseOverLua != null)
            {
                OnMouseOverLua.Invoke(index);
            }
        }

        // 当用户松开鼠标按钮时调用 OnMouseUp
        private void OnMouseUp()
        {
            if (OnMouseUpLua != null)
            {
                OnMouseUpLua.Invoke(index);
            }
        }

        // 当粒子击中碰撞器时调用 OnParticleCollision
        private void OnParticleCollision(GameObject other)
        {
            if (OnParticleCollisionLua != null)
            {
                OnParticleCollisionLua.Invoke(other, index);
            }
        }

        // 当粒子系统中的任意粒子满足触发模块的条件时调用
        private void OnParticleTrigger()
        {
            if (OnParticleTriggerLua != null)
            {
                OnParticleTriggerLua.Invoke(index);
            }
        }

        // 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
        private void OnTriggerEnter(Collider other)
        {
            if (OnTriggerEnterLua != null)
            {
                OnTriggerEnterLua.Invoke(other, index);
            }
        }

        // 如果另一个碰撞器 2D 进入了触发器，则调用 OnTriggerEnter2D (仅限 2D 物理)
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (OnTriggerEnter2DLua != null)
            {
                OnTriggerEnter2DLua.Invoke(collision, index);
            }
        }

        // 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
        private void OnTriggerExit(Collider other)
        {
            if (OnTriggerExitLua != null)
            {
                OnTriggerExitLua.Invoke(other, index);
            }
        }

        // 如果另一个碰撞器 2D 停止接触触发器，则调用 OnTriggerExit2D (仅限 2D 物理)
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (OnTriggerExit2DLua != null)
            {
                OnTriggerExit2DLua.Invoke(collision, index);
            }
        }

        // 对于触动触发器的所有“另一个碰撞器”，OnTriggerStay 将在每一帧被调用一次
        private void OnTriggerStay(Collider other)
        {
            if (OnTriggerStayLua != null)
            {
                OnTriggerStayLua.Invoke(other, index);
            }
        }

        // 如果其他每个碰撞器 2D 接触触发器，OnTriggerStay2D 将在每一帧被调用一次(仅限 2D 物理)
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (OnTriggerStay2DLua != null)
            {
                OnTriggerStay2DLua.Invoke(collision, index);
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
           if(OnPointUpLua != null)
            {
                OnPointUpLua.Invoke(index);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(OnPointDownLua != null)
            {
                OnPointDownLua.Invoke(index);
            }
        }
    }
}
