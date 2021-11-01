using ILFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

/// <summary>
/// 截图工具类,记得把代码挂在组件上才能运行
/// </summary>
public class ScreenToolManager:Manager<ScreenToolManager> 
{
    //private static ScreenToolManager _instance;
    //public static ScreenToolManager Instance
    //{
    //    get
    //    {            
    //        if (_instance == null)
    //            _instance = new ScreenToolManager();
    //        return _instance;
    //    }
    //}
    
    /// <summary>
    /// UnityEngine自带截屏Api，只能截全屏
    /// </summary>
    /// <param name="fileName">文件名</param>
    public void ScreenShotFile(string fileName)
    {
        UnityEngine.ScreenCapture.CaptureScreenshot(fileName);//截图并保存截图文件
        Debug.Log(string.Format("截取了一张图片: {0}", fileName));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif
    }
    /// <summary>
    /// UnityEngine自带截屏Api，只能截全屏
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="callBack">截图完成回调</param>
    /// <returns>协程</returns>
    public IEnumerator ScreenShotTex(string fileName, Action callBack = null)
    {
        yield return new WaitForEndOfFrame();//等到帧结束，不然会报错
        Texture2D tex = UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();//截图返回Texture2D对象
        byte[] bytes = tex.EncodeToPNG();//将纹理数据，转化成一个png图片
        System.IO.File.WriteAllBytes(fileName, bytes);//写入数据
        Debug.Log(string.Format("截取了一张图片: {0}", fileName));

        callBack?.Invoke();
        //StopCoroutine(ScreenShotTex(fileName, callBack));
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif
    }

    public void ScreenCapture(Rect rect, string fileName, Action startCallBack = null, Action endCallBack = null)
    {
        StartCoroutine(CaptureScreen(rect, fileName, startCallBack, endCallBack));
    }

    /// <summary>
    /// 截取游戏屏幕内的像素
    /// </summary>
    /// <param name="rect">截取区域：屏幕左下角为0点</param>
    /// <param name="fileName">文件名</param>
    /// <param name="callBack">截图完成回调</param>
    /// <returns></returns>
    public IEnumerator CaptureScreen(Rect rect, string fileName, Action startCallBack = null, Action endCallBack = null)
    {
        startCallBack?.Invoke();
        yield return new WaitForEndOfFrame();//等到帧结束，不然会报错
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);//新建一个Texture2D对象
        tex.ReadPixels(rect, 0, 0);//读取像素，屏幕左下角为0点
        tex.Apply();//保存像素信息

        byte[] bytes = tex.EncodeToPNG();//将纹理数据，转化成一个png图片
        System.IO.File.WriteAllBytes(fileName, bytes);//写入数据
        Debug.Log(string.Format("截取了一张图片: {0}", fileName));

        endCallBack?.Invoke();
        StopCoroutine(CaptureScreen(rect, fileName, startCallBack, endCallBack));
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif
    }
    /// <summary>
    /// 对相机拍摄区域进行截图，如果需要多个相机，可类比添加，可截取多个相机的叠加画面
    /// </summary>
    /// <param name="camera">待截图的相机</param>
    /// <param name="width">截取的图片宽度</param>
    /// <param name="height">截取的图片高度</param>
    /// <param name="fileName">文件名</param>
    /// <returns>返回Texture2D对象</returns>
    public Texture2D CameraCapture(Camera camera, Rect rect, string fileName)
    {
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, -1);//创建一个RenderTexture对象 

        camera.gameObject.SetActive(true);//启用截图相机
        camera.targetTexture = render;//设置截图相机的targetTexture为render
        camera.Render();//手动开启截图相机的渲染

        RenderTexture.active = render;//激活RenderTexture
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);//新建一个Texture2D对象
        tex.ReadPixels(rect, 0, 0);//读取像素
        tex.Apply();//保存像素信息

        camera.targetTexture = null;//重置截图相机的targetTexture
        RenderTexture.active = null;//关闭RenderTexture的激活状态
        UnityEngine.Object.Destroy(render);//删除RenderTexture对象

        byte[] bytes = tex.EncodeToPNG();//将纹理数据，转化成一个png图片
        System.IO.File.WriteAllBytes(fileName, bytes);//写入数据
        Debug.Log(string.Format("截取了一张图片: {0}", fileName));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif

        return tex;//返回Texture2D对象，方便游戏内展示和使用
    }
    /// <summary>
    /// 多个相机截图，画面为多个相机的叠加画面
    /// </summary>
    /// <param name="cameras">待截图的相机组</param>
    /// <param name="rect"></param>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
    public Texture2D CamerasCapture(Camera[] cameras, Rect rect, string fileName)
    {
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, -1);//创建一个RenderTexture对象 

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(true);//启用截图相机
            cameras[i].targetTexture = render;//设置截图相机的targetTexture为render
            cameras[i].Render();//手动开启截图相机的渲染
        }        

        RenderTexture.active = render;//激活RenderTexture
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);//新建一个Texture2D对象
        tex.ReadPixels(rect, 0, 0);//读取像素
        tex.Apply();//保存像素信息

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].targetTexture = null;//重置截图相机的targetTexture
        }
        
        RenderTexture.active = null;//关闭RenderTexture的激活状态
        UnityEngine.Object.Destroy(render);//删除RenderTexture对象

        byte[] bytes = tex.EncodeToPNG();//将纹理数据，转化成一个png图片
        System.IO.File.WriteAllBytes(fileName, bytes);//写入数据
        Debug.Log(string.Format("截取了一张图片: {0}", fileName));

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#endif

        return tex;//返回Texture2D对象，方便游戏内展示和使用
    }

    public void LoadImage(string path, Image icon, Action startCallBack = null, Action endCallBack = null)
    {
        StartCoroutine(LoadImageWebRequest(path, icon, startCallBack, endCallBack));
    }

    /// <summary>
    /// 对截图进行加载赋值给icon对象上，用UnityWebRequest
    /// </summary>
    /// <param name="path">加载路径</param>
    /// <param name="icon">赋值对象</param>
    /// <returns></returns>
    public IEnumerator LoadImageWebRequest(string path, Image icon, Action startCallBack = null, Action endCallBack = null)
    {
        startCallBack?.Invoke();
        string fileName = "file://" + path; //安卓运行需要在前面加file://
        yield return new WaitForEndOfFrame();//等到帧结束
        UnityWebRequest wr = new UnityWebRequest(fileName);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        if (!wr.isNetworkError)
        {
            Texture2D t = texDl.texture;// 得到texDl的贴图
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                                     Vector2.zero, 1f);//根据贴图创建精灵
            Debug.Log("Sprite:" + s);
            icon.sprite = s;//把精灵赋值给要显示的对象
        }
        else
        {
            Debug.Log("wr.error:" + wr.error);//没加载成功打印错误信息
        }
        Debug.Log("LoadPath:" + path);
        endCallBack?.Invoke();
        StopCoroutine(LoadImageWebRequest(path, icon));
    }

    public void LoadImageWWW(string path, Image icon, Action startCallBack = null, Action endCallBack = null)
    {
        StartCoroutine(WWWLoadImage(path, icon, startCallBack, endCallBack));
    }

    /// <summary>
    /// 对截图进行加载赋值给icon对象上，用WWW
    /// </summary>
    /// <param name="path">加载路径</param>
    /// <param name="icon">赋值对象</param>
    /// <returns></returns>
    public IEnumerator WWWLoadImage(string path, Image icon, Action startCallBack = null, Action endCallBack = null)
    {
        startCallBack?.Invoke();
        string fileName = "file://" + path;//安卓运行需要在前面加file://
        yield return new WaitForEndOfFrame();//等到帧结束
        WWW www = new WWW(fileName);
        yield return www;
        if (www.isDone)
        {           
            Texture2D t = www.texture;//得到texDl的贴图
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                                     Vector2.zero, 1f);//根据贴图创建精灵
            Debug.Log("Sprite:" + s.name);
            icon.sprite = s;//把精灵赋值给要显示的对象
        }
        else
        {
            Debug.Log("www.Done:" + www.error);//没加载成功打印错误信息
        }
        endCallBack?.Invoke();
        StopCoroutine(WWWLoadImage(path, icon));
    }
}
