using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToolExtensions
{
    public static void Identity(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.identity;
    }

    public static GameObject[] GetChildren(this Transform trans, GameObject father)
    {
        GameObject[] children = new GameObject[father.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = father.transform.GetChild(i).gameObject;
        }

        return children;
    }

    public static T[] GetChildrenComponent<T>(this Transform trans, GameObject father)where T:Component
    {
        T[] children = new T[father.transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = father.transform.GetChild(i).GetComponent<T>();
        }
        
        return children;
    }

    public static void ShowGameObject(this GameObject obj, GameObject child)
    {
        GameObject father = child.transform.parent.gameObject;
        for (int i = 0; i < father.transform.childCount; i++)
        {
            GameObject go = father.transform.GetChild(i).gameObject;

            if (go != child)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);
            }
        }
    }
}
