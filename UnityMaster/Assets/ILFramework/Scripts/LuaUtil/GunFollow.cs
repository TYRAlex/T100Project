using System.Collections;
using System.Collections.Generic;
using LuaFramework;
using LuaInterface;
using Spine.Unity;
using UnityEngine;

public class GunFollow : MonoBehaviour
{

    private bool isOn = false;

    public LuaFunction TurnOnGunLua;
    public LuaFunction TurnOffGunLua;

    void Awake()
    {
        isClose();
    }

    // Update is called once per frame
    void Update ()
	{
	    transform.position =
	        Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.farClipPlane));
        if (Input.GetMouseButton(0))
	    {
            if (!isOn)
            {
                isOpen();
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            if (isOn)
            {
                isClose();
            }
        }
    }

    private void isOpen()
    {
        //fire 开启 elect 开启
        //fire.SetActive(true);
        //spineMsg.gameObject.SetActive(true);
        //spineMsg.AnimationName = "texiao";
        isOn = true;
        if(TurnOnGunLua!=null) TurnOnGunLua.Call();
    }

    private void isClose()
    {
        Debug.Log("关闭功能");
        //fire 关闭 elect 关闭
        //fire.SetActive(false);
        //spineMsg.gameObject.SetActive(false);
        isOn = false;
        if(TurnOffGunLua!=null) TurnOffGunLua.Call();
    }
}
