using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System;
using OneID;
using UnityEditor;
using UnityEngine.Networking;
using UObject = UnityEngine.Object;
using UnityEngine.Profiling;
using UnityEngine.Video;

namespace ILFramework
{

    public class ResourceManager : Manager<ResourceManager>
    {

        Dictionary<HotfixPackage, AssetBundle> dynamicResDic = new Dictionary<HotfixPackage, AssetBundle>(); // 课程资源AB包

        List<AssetBundle> _audioReslist = new List<AssetBundle>();  //课程音频AB包

        List<AssetBundle> _courseResList = new List<AssetBundle>(); //课程资源AB包
        [HideInInspector]
        public AssetBundle commonResList; // 公共资源AB包

        public void AddResourceAB(HotfixPackage package, AssetBundle asset)
        {
            if (!dynamicResDic.ContainsKey(package))
                dynamicResDic.Add(package, asset);
            else
                Debug.LogErrorFormat(" ResourceManager AddResourceAB package named: {0} is already exist !", package.CourseName);
        }

        public void AddAudioResAB(AssetBundle asset)
        {
            if (!_audioReslist.Contains(asset))
            {
                _audioReslist.Add(asset);
            }
        }

        public void AddCourseResAB(AssetBundle asset)
        {
            if (!_courseResList.Contains(asset))
            {
                _courseResList.Add(asset);
            }
        }

        /// <summary>
        /// 释放每节课的音频以及课程prefab资源
        /// </summary>
        public void ResetAudioAndCourseResAB()
        {
            //Debug.LogError("Enter ResetFunc");
            try
            {

                for (int i = 0; i < _courseResList.Count; i++)
                {
                    //Debug.Log("资源名称" + _courseResList[i].name);
                    if (_courseResList[i])
                    {
                        _courseResList[i].Unload(true);
                        _courseResList[i] = null;
                    }

                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                //Debug.Log("Enter ResetFunc33333");
                _courseResList.Clear();
            }

            try
            {

                foreach (Dictionary<SoundManager.SoundType, AudioClip[]> audioClipDic in SoundManager.instance
                    .audioClips.Values)
                {
                    foreach (AudioClip[] target in audioClipDic.Values)
                    {
                        for (int i = 0; i < target.Length; i++)
                        {
                            Resources.UnloadAsset(target[i]);
                            target[i] = null;
                        }
                    }
                }
                for (int i = 0; i < _audioReslist.Count; i++)
                {
                    _audioReslist[i].Unload(true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _audioReslist.Clear();
                SoundManager.instance.audioClips.Clear();
            }
            Resources.UnloadUnusedAssets();
            Debug.Log("Clear all Asset Success!");
            // Debug.Log("Force to Collect all Assets!");            
        }

        public void RemoveResourceAB(HotfixPackage package)
        {
            if (dynamicResDic.ContainsKey(package))
            {
                dynamicResDic[package].Unload(true);
                dynamicResDic.Remove(package);
            }
            else
                Debug.LogErrorFormat(" ResourceManager RemoveResourceAB package named: {0} is not exist !", package.CourseName);
        }

        /// <summary>
        /// 加载指定课程包资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="package"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadResourceAB<T>(HotfixPackage package, string resName) where T : UObject
        {
            if (!dynamicResDic.ContainsKey(package))
                return default;
            AssetBundle ab = dynamicResDic[package];
            Debug.LogFormat(" ResourceManager LoadCourseResource assetbundle: {0} , resName: {1} , packageName: {2}", ab, resName, package.Name);
            
            T obj = ab.LoadAsset<T>(resName);
            Debug.LogFormat(" LoadCourseResource Over, Obj: {0} ", obj);
            return obj;
        }

        public T LoadResourceAB<T>(string resName,string resourceType=".prefab") where T : UObject
        {
#if UNITY_EDITOR
            T dynamicRecourse =
                AssetDatabase.LoadAssetAtPath<T>("Assets/HotFixPackage/" + HotfixManager.instance.curShowPackage.Name + "/LoadResource/" + resName+resourceType);
            Debug.LogFormat(" LoadCourseResource Over, Obj: {0} ", dynamicRecourse);
            return dynamicRecourse;
#endif
            Debug.LogError("你在用编辑器模式下的加载方式，请检查！");
            return null;
        }



        /// <summary>
        /// 加载公共资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resName"></param>
        /// <returns></returns>
        public T LoadCommonResAB<T>(string resName) where T : UObject
        {
            // Debug.LogError("当前资源是否为空"+commonResList);
            // foreach (var temp in commonResList.GetAllAssetNames())
            // {
            //     Debug.LogError("名称:"+temp);
            // }
            return commonResList.LoadAsset<T>(resName);
        }


        public void LoadOneIDCommonPrefabAsyn(string resName)
        {
            
            AssetBundleRequest ar = commonResList.LoadAssetAsync<GameObject>(resName);
            ar.completed += (ass) =>
            {
                if (ass.isDone)
                {
                    GameObject go= ar.asset as GameObject;
                    GameObject realPrefab = Instantiate(go);
                    realPrefab.name = resName;
                    OneIDSceneManager.Instance.AddGameSceneObject(realPrefab);
                    VideoPlayer vp = realPrefab.GetComponent<VideoPlayer>();
                    //Debug.LogError("视频名称："+vp.clip.name); 
                    vp.Prepare();
                    vp.prepareCompleted += PrepardFinished;
                    //Debug.LogError("加载了视频："+resName);
                    // vp.prepareCompleted += (vpComplete) =>
                    // {
                    //     Debug.LogError("视频："+resName + "加载完成");
                    //     if (vp.isPrepared)
                    //     {
                    //         Debug.LogError("视频："+resName + "是否准备好"+vp.isPrepared);
                    //         Debug.LogError("视频："+resName + "准备隐藏");
                    //         vp.targetTexture.DiscardContents();
                    //         vp.prepareCompleted += PrepardFinished;
                    //         realPrefab.Hide();
                    //     }
                    // };
                }
            };
        }

        void PrepardFinished(VideoPlayer v)
        {
            
            if (v.isPrepared)
            {
                //Debug.LogError("视频："+v.gameObject.name + "是否准备好"+v.isPrepared);
                //Debug.LogError("视频："+v.gameObject.name + "准备隐藏");
                v.targetTexture.DiscardContents();
                v.gameObject.Hide();
                v.prepareCompleted -= PrepardFinished;
                //realPrefab.Hide();
            }
        }





        /// <summary>
        /// 释放动态资源
        /// </summary>
        public void ReloadAssetBundle()
        {
            // Debug.Log("Enter Reload -   ");
            // Debug.Log("DynamicResDic Lenth is:"+dynamicResDic.Count);
            foreach (var ab in dynamicResDic)
            {
                // Debug.Log("assetbundle Name:" + ab.Value.name);       
                ab.Value.Unload(false);
            }
            Debug.Log("All DynamicClear!");
            dynamicResDic.Clear();
        }

        private void OnDestroy()
        {
            //if (SoundManager.instance.bebo1_commonClips!=null)
            //{
            //    SoundManager.instance.bebo1_commonClips = null;
            //}
            if (commonResList)
            {
                commonResList.Unload(false);
                //foreach (var audioClipDic in SoundManager.instance
                //.commonClips)
                //{
                //    foreach (AudioClip target in audioClipDic.Value)
                //    {
                //        Resources.UnloadAsset(target);
                //    }
                //}
            }
                //Resources.UnloadUnusedAssets();
        }

            /////////  兼容 1.0 ///////////////
            public void LoadPrefab(string courseName, string[] assetName, LuaFunction luaCall = null)
            {
                HotfixPackage package = Util.GetHotfixPackage(courseName);
                if (package == null)
                {
                    Debug.LogErrorFormat(" ResourceManager LoadPrefab: package is null,  courseName: {0}", courseName);
                    return;
                }
                for (int i = 0; i < assetName.Length; i++)
                {
                    GameObject go = LoadResourceAB<GameObject>(package, assetName[i]);
                    Debug.LogFormat(" LoadPrefab go :{0} ", go);
                    if (luaCall != null)
                        luaCall.Call(Instantiate(go));
                }
            }

            public GameObject LoadCommonPrefab(string prefabName, LuaFunction luaCall = null)
            {
                GameObject go = LoadCommonResAB<GameObject>(prefabName);
                GameObject ins = Instantiate(go);
                if (luaCall != null)
                    luaCall.Call(ins);
                return ins;
            }
        }
    }
