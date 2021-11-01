
using Spine.Unity;
using UnityEditor;
using UnityEngine;

namespace TDCreate
{

    public static class EditorResourcesManager
    {

        //public static T Load<T>(string assetPath) where T : Object
        //{
        //    T obj = null;

        //    if (typeof(T)== typeof(SkeletonGraphic))
        //    {
        //        obj = (SkeletonGraphic) 
        //    }


        //    return obj;
        //}



        public static SkeletonDataAsset LoadSkeletonDataAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(path);
        }

        public static Font LoadFontAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Font>(path);
        }

        public static Sprite LoadSpriteAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }


    }

}







