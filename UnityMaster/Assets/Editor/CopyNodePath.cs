using UnityEditor;

using UnityEngine;

public static class CopyNodePath
{
    [MenuItem("GameObject/CopyNodePath %q", false, 11)]
    private static void CopyPath()
    {
        Object[] objs = Selection.objects;

        if (objs.Length != 2)
            return;

        GameObject starGo = objs[0] as GameObject;
        GameObject endGo = objs[1] as GameObject;



        if (starGo == null || endGo == null)
        {
            Debug.LogError("StarNode or EndNode No Selection");
            return;
        }

        int endId = endGo.transform.GetInstanceID();

        string path = starGo.name;
        Transform parent = starGo.transform.parent;

        bool isEnd = false;

        while (parent)
        {
            if (isEnd)
                break;

            if (parent.GetInstanceID() == endId)
                isEnd = true;

            path = $"{parent.name}/{path}";
            parent = parent.parent;
        }

        Debug.Log(path);

        CopyString(path);
    }

    private static void CopyString(string str)
    {
        TextEditor textEditor = new TextEditor { text = str };
        textEditor.OnFocus();
        textEditor.Copy();
    }
}
