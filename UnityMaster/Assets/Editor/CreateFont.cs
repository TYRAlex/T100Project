using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateFont : MonoBehaviour
{
    [MenuItem("Asset/CreateTheNewFont(创建艺术字体)")]
    public static void CreateTheNewFont()
    {
        if (Selection.objects == null || Selection.objects.GetType() != typeof(Texture2D)) { return; }

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            if (Selection.objects[i].GetType() == typeof(Texture2D))
                CreateImgFont((Texture2D)(Selection.objects[i]));
        }
    }


    public static void CreateImgFont(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("请传入贴图");
            return;
        }

        string texturePath = AssetDatabase.GetAssetPath(texture);   //贴图路径
        string textureExtension = Path.GetExtension(texturePath);   //贴图文件后缀
        Debug.Log(textureExtension);
        string filePath = texturePath.Remove(texturePath.Length - textureExtension.Length);

        Font newFont = AssetDatabase.LoadAssetAtPath<Font>(filePath + ".fontsettings");
        if (newFont == null)
        {
            Debug.Log("当前无字体设置，已新建");
            newFont = new Font();
            //设置材质
            Material newMat = new Material(Shader.Find("GUI/Text Shader"));
            newMat.SetTexture("_MainTex", texture);
            AssetDatabase.CreateAsset(newMat, filePath + ".mat");
            newFont.material = newMat;
            AssetDatabase.CreateAsset(newFont, filePath + ".fontsettings");
        }
        else
            Debug.Log("读取当前的字体设置");
        

        //设置字符信息
        Sprite[] allSprites = GetSpriteByPath(texturePath);
        if(allSprites.Length == 0)
        {
            Debug.LogError("请先进行字符切割操作");
            return;
        }

        CharacterInfo[] allInfos = new CharacterInfo[allSprites.Length];
        float texWidth = texture.width;
        float texHeight = texture.height;
        for (int i = 0; i < allInfos.Length; i++)
        {
            allInfos[i] = new CharacterInfo();
            //根据图片名设置Ascii码
            allInfos[i].index = int.Parse(allSprites[i].name);

            //设置字符uv
            Rect spriteRect = allSprites[i].rect;
            allInfos[i].uvBottomLeft = new Vector2(spriteRect.x / texWidth, spriteRect.y / texHeight);
            allInfos[i].uvBottomRight = new Vector2((spriteRect.x + +spriteRect.width) / texWidth, spriteRect.y / texHeight);
            allInfos[i].uvTopLeft = new Vector2(spriteRect.x / texWidth, (spriteRect.y + spriteRect.height) / texHeight);
            allInfos[i].uvTopRight = new Vector2((spriteRect.x + +spriteRect.width) / texWidth, (spriteRect.y + spriteRect.height) / texHeight);

            //设置字符的偏移值与宽高
            allInfos[i].minX = -(int)allSprites[i].pivot.x;
            allInfos[i].minY = -(int)allSprites[i].pivot.y;
            allInfos[i].maxX = (int)(spriteRect.width - allSprites[i].pivot.x);
            allInfos[i].maxY = (int)(spriteRect.height - allSprites[i].pivot.y);

            allInfos[i].advance = (int)spriteRect.width;
        }

        newFont.characterInfo = allInfos;

        //保存数据并刷新引擎中的引用
        EditorUtility.SetDirty(newFont);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static Sprite[] GetSpriteByPath(string path)
    {
        List<Sprite> sprites = new List<Sprite>();

        Object[] pathObjs = AssetDatabase.LoadAllAssetsAtPath(path);
        for (int i = 0; i < pathObjs.Length; i++)
        {
            if (pathObjs[i].GetType() == typeof(Sprite))
                sprites.Add((Sprite)pathObjs[i]);
        }

        return sprites.ToArray();
    }
}
