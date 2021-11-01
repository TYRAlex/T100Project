using UnityEditor;
using System.Globalization;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class SpriteScriptObject : ScriptableObject
{
    [SerializeField]
    public Sprite sprite;
    //public int Level;
}
public class CreateSpriteEditor
{
//    [MenuItem("Assets/AddSprites", true, 1)]
//    static void CreateSprites()
//    {
//        return true;
//    }
    [MenuItem("Assets/AddSprites", false, 1)]
    
    static void CreateSprites()
    {
        Object[] objs = Selection.objects;
        List<Texture2D> objLists = new List<Texture2D>();
        Debug.Log(Selection.objects.Length);
        Object[] arr = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);
        for (int i = 0; i < objs.Length; i++)
        {
            Texture2D tex = objs[i] as Texture2D;
            objLists.Add(tex);
        }
        Sort(objLists);
        EditorUtility.DisplayCancelableProgressBar("生成", "CreateSprites", 0);
        for (int i = 0; i < objLists.Count; i++)
        {
            Object curObj = null;
            for(int j = 0;j<arr.Length;j++)
            {
                if(arr[j].name == objLists[i].name)
                {
                    curObj = arr[j];
                    break;
                }
            }
            string assetPath = AssetDatabase.GetAssetPath(curObj);
            bool isEnd = CreateGameObject(objLists[i],assetPath, i, objLists.Count - 1);
            if (isEnd) break;
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("生成完毕！！！");
    }
    static void Sort(List<Texture2D> sortlist)
    {
        Comparer comparer = new Comparer(CultureInfo.CurrentCulture);
        bool isMacth = false;
        do
        {
            isMacth = false;
            for (int i = 0; i < sortlist.Count - 1; i++)
            {
                if (comparer.Compare(sortlist[i].name, sortlist[i + 1].name) > 0)
                {
                    var temp = sortlist[i];
                    sortlist[i] = sortlist[i + 1];
                    sortlist[i + 1] = temp;
                    isMacth = true;
                }
            }
        } while (isMacth);
    }
    static bool CreateGameObject(Texture2D tex,string assetPath, int count, int max)
    {
        GameObject go = new GameObject(tex.name);
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
        Undo.RegisterCreatedObjectUndo(go, tex.name);

        return EditorUtility.DisplayCancelableProgressBar("生成", "CreateSprites--->" + tex.name, (count + 1.0f) / (max + 1.0f));
    }
}
