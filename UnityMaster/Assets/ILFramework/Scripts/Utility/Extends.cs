using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

//扩展类
public  static partial class Extends 
{
    public static bool IsNull(this UnityEngine.Object o)
    {
        return o == null;
    }

    public static void Show(this GameObject self)
    {
        if (self != null && !self.activeSelf)
        {
            self.SetActive(true);
        }
    }

    public static void Hide(this GameObject self)
    {
        if (self != null && self.activeSelf)
        {
            self.SetActive(false);
        }
    }

    public static T AddScriptComponent<T>(this GameObject self) where T : Component
    {
        if (self.GetComponent<T>() == null)
            return self.AddComponent<T>();
        return self.GetComponent<T>();
    }

    public static void RemoveComponent<T>(this Component src) where T : Component
    {
        T component = src.GetComponent<T>();
        if (component != null)
        {
            Object.DestroyImmediate(component);
        }
    }

    public static void RemoveChildren(this Transform self)
    {
        for (int i = self.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(self.GetChild(i).gameObject);
        }
    }

    public static Text GetText(this Transform transform, string path = null)
    {
        if (path == null)
        {
            return transform.GetComponent<Text>();
        }

        return transform.Find(path).GetComponent<Text>();
    }

    public static Button GetButton(this Transform transform, string path = null)
    {
        if (path == null)
        {
            return transform.GetComponent<Button>();
        }

        return transform.Find(path).GetComponent<Button>();
    }

    public static RectTransform GetRectTransform(this Transform transform, string path = null)
    {
        if (path == null)
        {
            return transform.GetComponent<RectTransform>();
        }

        return transform.Find(path).GetComponent<RectTransform>();
    }

    public static RawImage GetRawImage(this Transform transform, string path = null)
    {
        if (path == null)
        {
            return transform.GetComponent<RawImage>();
        }

        return transform.Find(path).GetComponent<RawImage>();
    }

    public static Image GetImage(this Transform transform, string path = null)
    {
        if (path == null)
        {
            return transform.GetComponent<Image>();
        }

        return transform.Find(path).GetComponent<Image>();
    }

    public static Toggle GetToggle(this Transform transform, string path=null)
    {
        if (path ==null)
        {
            return transform.GetComponent<Toggle>();
        }

        return transform.Find(path).GetComponent<Toggle>();
    }

    public static GameObject GetGameObject(this Transform transform, string path=null)
    {

        if (path==null)
        {
            return transform.gameObject;
        }
        return transform.Find(path).gameObject;
    }

    /// <summary>
    /// 获取对象的组件
    /// </summary>
    /// <param name="transform">要获取对象的父物体</param>
    /// <param name="path">寻找路径</param>
    /// <typeparam name="T">对应组件</typeparam>
    /// <returns></returns>
    public static T GetTargetComponent<T>(this Transform transform, string path = null) where T : Component
    {
        if (path == null)
            return transform.GetComponent<T>();
        return transform.Find(path).GetComponent<T>();
    }

    public static Transform GetTransform(this Transform transform, string path=null)
    {
        if (path==null)
        {
            return transform;
        }

        return transform.Find(path);
    }

    public static CanvasSizeFitter GetCanvasSizeFitter(this Transform transform, string path = null)
    {
        if (path == null)
        {
            return transform.GetComponent<CanvasSizeFitter>();
        }

        return transform.Find(path).GetComponent<CanvasSizeFitter>();
    }

    public static Empty4Raycast GetEmpty4Raycast(this Transform transform, string path=null)
    {
        if (path==null)
        {
            return transform.GetComponent<Empty4Raycast>();
        }

        return transform.Find(path).GetComponent<Empty4Raycast>();
    }

    public static GameObject InstanceGameObject(GameObject prefab,Transform parent,bool worldPositionStays=false)
    {
        return Object.Instantiate(prefab, parent, worldPositionStays);
    }

  
}
