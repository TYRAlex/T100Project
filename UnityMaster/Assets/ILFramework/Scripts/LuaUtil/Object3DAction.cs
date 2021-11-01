using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 添加Unity的一些3D的操作到Lua
/// </summary>
public class Object3DAction : MonoBehaviour
{

    public int index = 0;
    public LuaFunction OnCollisionEnterLua;
    public LuaFunction OnCollisionEnter2DLua;
    public LuaFunction OnCollisionExitLua;
    public LuaFunction OnCollisionExit2DLua;
    public LuaFunction OnCollisionStayLua;
    public LuaFunction OnCollisionStay2DLua;

    public LuaFunction OnMouseDownLua;
    public LuaFunction OnMouseDragLua;
    public LuaFunction OnMouseEnterLua;
    public LuaFunction OnMouseExitLua;
    public LuaFunction OnMouseOverLua;
    public LuaFunction OnMouseUpLua;
    public LuaFunction OnParticleCollisionLua;
    public LuaFunction OnParticleTriggerLua;

    public LuaFunction OnTriggerEnterLua;
    public LuaFunction OnTriggerEnter2DLua;
    public LuaFunction OnTriggerStayLua;
    public LuaFunction OnTriggerStay2DLua;
    public LuaFunction OnTriggerExitLua;
    public LuaFunction OnTriggerExit2DLua;

    // 当此碰撞器/刚体开始接触另一个刚体/碰撞器时，调用 OnCollisionEnter
    private void OnCollisionEnter(Collision collision)
    {
        if (OnCollisionEnterLua != null)
        {
            OnCollisionEnterLua.Call<Collision, int>(collision, index);
        }
    }

    // 当此碰撞器 2D/刚体 2D 开始接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionEnter2D (仅限 2D 物理)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (OnCollisionEnter2DLua != null)
        {
            OnCollisionEnter2DLua.Call<Collision2D, int>(collision, index);
        }
    }

    // 当此碰撞器/刚体停止接触另一刚体/碰撞器时调用 OnCollisionExit
    private void OnCollisionExit(Collision collision)
    {
        if (OnCollisionExitLua != null)
        {
            OnCollisionExitLua.Call<Collision, int>(collision, index);
        }
    }

    // 当此碰撞器 2D/刚体 2D 停止接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionExit2D (仅限 2D 物理)
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (OnCollisionExit2DLua != null)
        {
            OnCollisionExit2DLua.Call<Collision2D, int>(collision, index);
        }
    }

    // 每当此碰撞器/刚体接触到刚体/碰撞器时，OnCollisionStay 将在每一帧被调用一次
    private void OnCollisionStay(Collision collision)
    {
        if (OnCollisionStayLua != null)
        {
            OnCollisionStayLua.Call<Collision, int>(collision, index);
        }
    }

    // 每当碰撞器 2D/刚体 2D 接触到刚体 2D/碰撞器 2D 时，OnCollisionStay2D 将在每一帧被调用一次(仅限 2D 物理)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (OnCollisionStay2DLua != null)
        {
            OnCollisionStay2DLua.Call<Collision2D, int>(collision, index);
        }
    }

    // 当用户在 GUIElement 或碰撞器上按鼠标按钮时调用 OnMouseDown
    private void OnMouseDown()
    {
        if (OnMouseDownLua != null)
        {
            OnMouseDownLua.Call<int>(index);
        }
    }

    // 当用户在 GUIElement 或碰撞器上单击鼠标并保持按住鼠标时调用 OnMouseDrag
    private void OnMouseDrag()
    {
        if (OnMouseDragLua != null)
        {
            OnMouseDragLua.Call<int>(index);
        }
    }

    // 当鼠标进入 GUIElement 或碰撞器时调用 OnMouseEnter
    private void OnMouseEnter()
    {
        if (OnMouseEnterLua != null)
        {
            OnMouseEnterLua.Call<int>(index);
        }
    }

    // 当鼠标不再停留在 GUIElement 或碰撞器上时调用 OnMouseExit
    private void OnMouseExit()
    {
        if (OnMouseExitLua != null)
        {
            OnMouseExitLua.Call<int>(index);
        }
    }

    // 当鼠标停留在 GUIElement 或碰撞器上时每帧都调用 OnMouseOver
    private void OnMouseOver()
    {
        if (OnMouseOverLua != null)
        {
            OnMouseOverLua.Call<int>(index);
        }
    }

    // 当用户松开鼠标按钮时调用 OnMouseUp
    private void OnMouseUp()
    {
        if (OnMouseUpLua != null)
        {
            OnMouseUpLua.Call<int>(index);
        }
    }

    // 当粒子击中碰撞器时调用 OnParticleCollision
    private void OnParticleCollision(GameObject other)
    {
        if (OnParticleCollisionLua != null)
        {
            OnParticleCollisionLua.Call<GameObject, int>(other, index);
        }
    }

    // 当粒子系统中的任意粒子满足触发模块的条件时调用
    private void OnParticleTrigger()
    {
        if (OnParticleTriggerLua != null)
        {
            OnParticleTriggerLua.Call<int>(index);
        }
    }

    // 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (OnTriggerEnterLua != null)
        {
            OnTriggerEnterLua.Call<Collider, int>(other, index);
        }
    }

    // 如果另一个碰撞器 2D 进入了触发器，则调用 OnTriggerEnter2D (仅限 2D 物理)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnTriggerEnter2DLua != null)
        {
            OnTriggerEnter2DLua.Call<Collider2D, int>(collision, index);
        }
    }

    // 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (OnTriggerExitLua != null)
        {
            OnTriggerExitLua.Call<Collider, int>(other, index);
        }
    }

    // 如果另一个碰撞器 2D 停止接触触发器，则调用 OnTriggerExit2D (仅限 2D 物理)
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnTriggerExit2DLua != null)
        {
            OnTriggerExit2DLua.Call<Collider2D, int>(collision, index);
        }
    }

    // 对于触动触发器的所有“另一个碰撞器”，OnTriggerStay 将在每一帧被调用一次
    private void OnTriggerStay(Collider other)
    {
        if (OnTriggerStayLua != null)
        {
            OnTriggerStayLua.Call<Collider, int>(other, index);
        }
    }

    // 如果其他每个碰撞器 2D 接触触发器，OnTriggerStay2D 将在每一帧被调用一次(仅限 2D 物理)
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (OnTriggerStay2DLua != null)
        {
            OnTriggerStay2DLua.Call<Collider2D, int>(collision, index);
        }
    }

}
