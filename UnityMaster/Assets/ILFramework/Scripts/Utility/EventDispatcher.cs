using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    int time = 0;
    /**碰撞检测*/
    public delegate void CollisionHandler2D(Collision2D c,int time);
    /**触发器检测*/
    public delegate void TriggerHandler2D(Collider2D other, int time);


    /**事件*/
    public delegate void EventHandler(GameObject e);
    /**碰撞检测*/
    public delegate void CollisionHandler(Collision c, int time);
    /**触发器检测*/
    public delegate void TriggerHandler(Collider other, int time);


    public event EventHandler MouseEnter;
    void OnMouseEnter()
    {
        MouseEnter?.Invoke(this.gameObject); //通过委托调用方法
    }

    public event EventHandler MouseOver;
    void OnMouseOver()
    {
        MouseOver?.Invoke(this.gameObject);
    }

    public event EventHandler MouseExit;
    void OnMouseExit()
    {
        MouseExit?.Invoke(this.gameObject);
    }

    public event EventHandler MouseDown;
    void OnMouseDown()
    {
        MouseDown?.Invoke(this.gameObject);
    }

    public event EventHandler MouseUp;
    void OnMouseUp()
    {
        MouseUp?.Invoke(this.gameObject);
    }

    public event EventHandler MouseDrag;
    void OnMouseDrag()
    {
        MouseDrag?.Invoke(this.gameObject);
    }

    /**当renderer(渲染器)在任何相机上可见时调用OnBecameVisible*/
    public event EventHandler BecameVisible;
    void OnBecameVisible()
    {
        BecameVisible?.Invoke(this.gameObject);
    }
    /**当renderer(渲染器)在任何相机上都不可见时调用OnBecameInvisible*/
    public event EventHandler BecameInvisible;
    void OnBecameInvisible()
    {
        BecameInvisible?.Invoke(this.gameObject);
    }
    public event EventHandler Enable;
    void OnEnable()
    {
        Enable?.Invoke(this.gameObject);
    }
    public event EventHandler Disable;
    void OnDisable()
    {
        time=0;
        TriggerEnter2D = null;
        TriggerEnter = null;
        TriggerExit2D = null;
        TriggerExit = null;
        TriggerStay2D = null;
        TriggerStay = null;

        CollisionEnter2D = null;
        CollisionEnter = null;
        CollisionExit2D = null;
        CollisionExit = null;
        CollisionStay2D = null;
        CollisionStay = null;

        Disable?.Invoke(this.gameObject);
    }
    public event EventHandler Destroy;
    void OnDestroy()
    {
        Destroy?.Invoke(this.gameObject);
    }
    /**在相机渲染场景之前调用*/
    public event EventHandler PreRender;
    void OnPreRender()
    {
        PreRender?.Invoke(this.gameObject);
    }
    /**在相机完成场景渲染之后调用*/
    public event EventHandler PostRender;
    void OnPostRender()
    {
        PostRender?.Invoke(this.gameObject);
    }
    /**在相机场景渲染完成后被调用*/
    public event EventHandler RenderObject;
    void OnRenderObject()
    {
        RenderObject?.Invoke(this.gameObject);
    }

    public event EventHandler ApplicationPause;
    void OnApplicationPause()
    {
        ApplicationPause?.Invoke(this.gameObject);
    }
    /**当玩家获得或失去焦点时发送给所有游戏物体*/
    public event EventHandler ApplicationFocus;
    void OnApplicationFocus()
    {
        ApplicationFocus?.Invoke(this.gameObject);
    }
    public event EventHandler ApplicationQuit;
    void OnApplicationQuit()
    {
        ApplicationQuit?.Invoke(this.gameObject);
    }

    public event TriggerHandler TriggerEnter;
    void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other, time++);
    }
    public event TriggerHandler TriggerExit;
    void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke(other, time++);
    }
    public event TriggerHandler TriggerStay;
    void OnTriggerStay(Collider other)
    {
        TriggerStay?.Invoke(other, time++);
    }


    public event CollisionHandler CollisionEnter;
    void OnCollisionEnter(Collision c)
    {
        CollisionEnter?.Invoke(c, time++);
    }
    public event CollisionHandler CollisionStay;
    void OnCollisionStay(Collision c)
    {
        CollisionStay?.Invoke(c, time++);
    }

    public event CollisionHandler CollisionExit;
    void OnCollisionExit(Collision c)
    {
        CollisionExit?.Invoke(c, time++);
    }


    public event TriggerHandler2D TriggerEnter2D;
    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2D?.Invoke(other, time++);
    }
    public event TriggerHandler2D TriggerExit2D;
    void OnTriggerExit2D(Collider2D other)
    {
        TriggerExit2D?.Invoke(other, time++);
    }
    public event TriggerHandler2D TriggerStay2D;
    void OnTriggerStay2D(Collider2D other)
    {
        TriggerStay2D?.Invoke(other, time++);
    }


    public event CollisionHandler2D CollisionEnter2D;
    void OnCollisionEnter2D(Collision2D c)
    {
        CollisionEnter2D?.Invoke(c, time++);
    }
    public event CollisionHandler2D CollisionStay2D;
    void OnCollisionStay2D(Collision2D c)
    {
        CollisionStay2D?.Invoke(c, time++);
    }

    public event CollisionHandler2D CollisionExit2D;
    void OnCollisionExit2D(Collision2D c)
    {
        CollisionExit2D?.Invoke(c, time++);
    }
}
