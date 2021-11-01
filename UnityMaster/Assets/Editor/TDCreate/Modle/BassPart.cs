using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

using Spine.Unity;
using System.Text;
using System.Xml;
using System.IO;
using ILFramework;

namespace TDCreate
{
    [Serializable]
    public class BassPart
    {
        [HideInInspector]
        public string CourseName;
        [HideInInspector]
        public string PathName;

        // [AssetList]
        public Sprite BgSprite;




        public virtual void Create()
        {
            CreatePrefab();
            CreateScript();
            AssetDatabase.Refresh();
        }

        protected virtual void CreateScript()
        {
        }

        protected virtual void CreatePrefab()
        {

        }

        protected void SavePrefab(GameObject go)
        {
            var assetPtah = Config.HotFixPackageRootPath + CourseName + "/" + PathName + "/" + go.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(go, assetPtah);
        }


        protected void CreateCsProject(string tempCsPath, Action<string, string> updateContentsCallBack = null)
        {
            string createCsPath = string.Format("{0}{1}/{2}/{3}.cs", Config.HotFixCodePackageRootPath, CourseName, PathName, CourseName + PathName);
            string contents = File.ReadAllText(tempCsPath);
            contents = contents.Replace("ClassName", CourseName + PathName);
            updateContentsCallBack?.Invoke(contents, createCsPath);

            if (updateContentsCallBack == null)
                File.WriteAllText(createCsPath, contents);
        }

        protected void CreateCsprojProject()
        {
            string tempCsprojPath = Config.TemplateCsprojPath;
            string createCsprojPath = string.Format("{0}{1}/{2}/{3}.csproj", Config.HotFixCodePackageRootPath, CourseName, PathName, CourseName + PathName);
            string unityEidtorDataPath = EditorApplication.applicationContentsPath;
            string managedPath = "/Managed/UnityEngine/";

            XmlDocument xml = new XmlDocument();
            xml.Load(tempCsprojPath);
            XmlElement xmlRoot = xml.DocumentElement; //DocumentElement获取文档的跟
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;


            string outStr = string.Format("{0}\\{1}", CourseName, PathName);
            XmlWriter xw = XmlWriter.Create(createCsprojPath, settings);

            foreach (XmlNode node3 in xmlRoot.ChildNodes)
            {
                if (node3.HasChildNodes)
                {
                    foreach (XmlNode node4 in node3.ChildNodes)
                    {
                        if (node4.Name == "RootNamespace")
                        {
                            XmlElement tmp = (XmlElement)node4;
                            tmp.InnerText = CourseName + PathName;
                        }
                        else if (node4.Name.Equals("AssemblyName"))
                        {
                            node4.InnerText = CourseName + PathName;
                        }
                        else if (node4.Name.Equals("OutputPath"))
                        {
                            string outPath = "..\\..\\..\\OutHotfixAssetPackage\\" + outStr;
                            node4.InnerText = outPath;
                        }
                        else if (node4.Name.Equals("Reference"))
                        {
                            string attributes = node4.Attributes[0].Value;
                            string preStr = "..\\..\\..\\";
                            switch (attributes)
                            {
                                case "Assembly-CSharp":
                                    node4.FirstChild.InnerText = preStr + "Library\\ScriptAssemblies\\Assembly-CSharp.dll";
                                    break;
                                case "UnityEngine.CoreModule":
                                    node4.FirstChild.InnerText = unityEidtorDataPath + managedPath + "UnityEngine.CoreModule.dll";
                                    break;
                                case "UnityEngine.Physics2DModule":
                                    node4.FirstChild.InnerText = unityEidtorDataPath + managedPath + "UnityEngine.Physics2DModule.dll";
                                    break;
                                case "UnityEngine.PhysicsModule":
                                    node4.FirstChild.InnerText = unityEidtorDataPath + managedPath + "UnityEngine.PhysicsModule.dll";
                                    break;
                                case "UnityEngine":
                                    node4.FirstChild.InnerText = unityEidtorDataPath + managedPath + "UnityEngine.dll";
                                    break;
                                case "UnityEngine.UI":
                                    node4.FirstChild.InnerText = unityEidtorDataPath + "/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll";
                                    break;
                                case "DOTween":
                                    node4.FirstChild.InnerText = preStr + "Assets/Thirds/Demigiant/DOTween/DOTween.dll";
                                    break;
                                case "DOTweenPro":
                                    node4.FirstChild.InnerText = preStr + "Assets/Thirds/Demigiant/DOTweenPro/DOTweenPro.dll";
                                    break;
                                case "DOTween46":
                                    node4.FirstChild.InnerText = preStr + "Assets/Thirds/Demigiant/DOTween/DOTween46.dll";
                                    break;
                                case "UnityEngine.AudioModule":
                                    node4.FirstChild.InnerText = unityEidtorDataPath + managedPath + "UnityEngine.AudioModule.dll";
                                    break;
                            }
                        }
                        else if (node4.Name.Equals("Compile"))
                        {
                            string csName = string.Format("{0}{1}.cs", CourseName, PathName);
                            node4.Attributes[0].Value = csName;
                        }
                    }
                }
            }

            xml.Save(xw);
            xw.Flush();
            xw.Close();
        }

        protected void OpenCsFolder()
        {
            string openURL = Path.Combine(Application.dataPath, "../", "HotfixCodeProject/" + CourseName + "/" + PathName);
            Application.OpenURL(openURL);
        }

        protected Transform CreatePartRoot()
        {
            GameObject root = new GameObject { name = $"{CourseName}{PathName}_htp" };
            root.AddCanvas();
            root.AddCanvasScaler();
            root.AddGraphicRaycaster();
            root.AddComponent<RootILBehaviour>();
            return root.GetComponent<Transform>();
        }

        protected void CreateOnePage(Transform parent, int onePageOnClickNums, SkeletonDataAsset skeletonData = null, bool isNeedSoundIndex = false, int tempNameIndex = 0)
        {           
            GameObject go = new GameObject("0");
            go.transform.SetParent(parent);
            go.AddRectTransform(Config.ConstV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);


            for (int i = 0; i < onePageOnClickNums; i++)
            {
                var name = Config.CardSpineNames[tempNameIndex];
                GameObject spine = new GameObject(name + "3");
                spine.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop,
                    AnchorPresetsType.LeftTop);
                spine.transform.SetParent(go.transform);
                spine.SetAnchorPos(new Vector2(0, 0));

                if (skeletonData != null)
                    spine.AddSkeletonGraphic(skeletonData);

                GameObject onClick = new GameObject(name);
                onClick.transform.SetParent(go.transform);
                onClick.AddEmpty4Ray();

                if (isNeedSoundIndex)
                {
                    GameObject soundIndexGo = new GameObject("0");
                    soundIndexGo.transform.SetParent(onClick.transform);
                    soundIndexGo.AddRectTransform(new Vector2(100, 100));
                }

                if (onePageOnClickNums == 1)
                {
                    onClick.AddRectTransform(Config.ConstOneCardV2);
                    onClick.SetAnchorPos(new Vector2(0, 0));
                }
                else if (onePageOnClickNums == 2)
                {
                    onClick.AddRectTransform(Config.ConstTwoCardV2);
                    onClick.SetAnchorPos(i == 0 ? Config.ConstTwoCardLeftPos : Config.ConstTwoCardRightPos);
                }
                else if (onePageOnClickNums == 3)
                {
                    onClick.AddRectTransform(Config.ConstThreeCardV2);
                    if (i == 0)
                        onClick.SetAnchorPos(Config.ConstThreeCardLeftPos);
                    else if (i == 1)
                        onClick.SetAnchorPos(Config.ConstThreeCardMiddlePos);
                    else
                        onClick.SetAnchorPos(Config.ConstThreeCardRightPos);
                }

                tempNameIndex++;
            }


        }

        protected void CreateMorePage(Transform parent, int pageNums, Dictionary<int, int> pageInfos, SkeletonDataAsset skeletonData = null,
            bool isNeedSoundIndex = false, int tempNameIndex = 0)
        {
            for (int i = 0; i < pageNums; i++)
            {
                GameObject go = new GameObject(i.ToString());
                go.transform.SetParent(parent);
                go.AddRectTransform(Config.ConstV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);

                var onClickNums = pageInfos[i];
                for (int j = 0; j < onClickNums; j++)
                {
                    var name = Config.CardSpineNames[tempNameIndex];
                    GameObject spine = new GameObject(name + "3");
                    spine.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
                    spine.transform.SetParent(go.transform);
                    spine.SetAnchorPos(new Vector2(0, 0));

                    if (skeletonData != null)
                        spine.AddSkeletonGraphic(skeletonData);


                    GameObject onClick = new GameObject(name);
                    onClick.transform.SetParent(go.transform);
                    onClick.AddEmpty4Ray();
                    if (isNeedSoundIndex)
                    {
                        GameObject soundIndexGo = new GameObject("0");
                        soundIndexGo.transform.SetParent(onClick.transform);
                        soundIndexGo.AddRectTransform(new Vector2(100, 100));
                    }

                    if (onClickNums == 1)
                    {
                        onClick.AddRectTransform(Config.ConstOneCardV2);
                        onClick.SetAnchorPos(new Vector2(0, 0));
                    }
                    else if (onClickNums == 2)
                    {
                        onClick.AddRectTransform(Config.ConstTwoCardV2);
                        onClick.SetAnchorPos(j == 0 ? Config.ConstTwoCardLeftPos : Config.ConstTwoCardRightPos);
                    }
                    else if (onClickNums == 3)
                    {
                        onClick.AddRectTransform(Config.ConstThreeCardV2);
                        if (j == 0)
                            onClick.SetAnchorPos(Config.ConstThreeCardLeftPos);
                        else if (j == 1)
                            onClick.SetAnchorPos(Config.ConstThreeCardMiddlePos);
                        else
                            onClick.SetAnchorPos(Config.ConstThreeCardRightPos);
                    }

                    tempNameIndex++;
                }
            }
        }

        protected void CheckMorePageInfos(int pageNums, Dictionary<int, int> pageInfos)
        {
            var isExceedDicCount = pageNums >= pageInfos.Count;

            if (isExceedDicCount)
            {
                for (int i = 0; i < pageNums; i++)
                {
                    var key = i;
                    var isKey = pageInfos.ContainsKey(key);

                    if (!isKey)
                    {
                        var value = 0;
                        pageInfos.Add(key, value);
                    }
                }
            }
            else
            {
                for (int i = pageInfos.Count - 1; i >= pageNums; i--)
                {
                    var key = i;
                    var isKey = pageInfos.ContainsKey(key);
                    if (isKey)
                        pageInfos.Remove(key);
                }
            }
        }

        protected void CheckMoreSmallPartInfos(int moreSmallPartNums, Dictionary<int, CardMoreSmallPart> moreSmallPartInfos)
        {
            var isExceedDicCount = moreSmallPartNums >= moreSmallPartInfos.Count;

            if (isExceedDicCount)
            {
                for (int i = 0; i < moreSmallPartNums; i++)
                {
                    var key = i;
                    var isKey = moreSmallPartInfos.ContainsKey(key);

                    if (!isKey)
                    {
                        var value =  new CardMoreSmallPart();
                        moreSmallPartInfos.Add(key, value);
                    }
                }
            }
            else
            {
                for (int i = moreSmallPartInfos.Count - 1; i >= moreSmallPartNums; i--)
                {
                    var key = i;
                    var isKey = moreSmallPartInfos.ContainsKey(key);
                    if (isKey)
                        moreSmallPartInfos.Remove(key);
                }
            }
        }



        protected void CreateSwitch(Transform parent)
        {
            GameObject sw = new GameObject("Switchs");
            sw.transform.SetParent(parent);
            sw.AddRectTransform(Config.ConstV2);

            GameObject l2 = new GameObject("L2");
            l2.transform.SetParent(sw.transform);
            l2.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
            l2.AddSkeletonGraphic(Config.LRSpinePath);

            GameObject r2 = new GameObject("R2");
            r2.transform.SetParent(sw.transform);
            r2.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
            r2.AddSkeletonGraphic(Config.LRSpinePath);

            GameObject l = new GameObject("L");
            l.transform.SetParent(sw.transform);
            l.AddRectTransform(Config.ConstArrowV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.LeftMiddle);
            l.SetAnchorPos(Config.ConstLeftArrowPos);
            l.AddEmpty4Ray();

            GameObject r = new GameObject("R");
            r.transform.SetParent(sw.transform);
            r.AddRectTransform(Config.ConstArrowV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.RightMiddle);
            r.SetAnchorPos(Config.ConstRightArrowPos);
            r.AddEmpty4Ray();
        }


        protected void CreateMiddleBd(Transform parent)
        {
            GameObject go = new GameObject("dBD");
            go.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftBottom, AnchorPresetsType.LeftBottom);
            go.SetAnchorPos(Config.BuDingMiddlePos);
            go.SetScale(Config.BuDingMiddleScale);
            go.transform.SetParent(parent);
            go.AddSkeletonGraphic(Config.BDMiddleSpinePath);
        }

        protected void CreateMiddleXem(Transform parent)
        {
            GameObject xem = new GameObject("xem");
            xem.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftBottom, AnchorPresetsType.LeftBottom);
            xem.SetAnchorPos(Config.BuDingMiddlePos);
            xem.SetScale(Config.BuDingMiddleScale);
            xem.transform.SetParent(parent);
            xem.AddSkeletonGraphic(Config.XemMiddleSpinePath, "daiji");
        }


        protected void CreateLeftBd(Transform parent)
        {
            GameObject go = new GameObject("sBD");
            go.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftBottom, AnchorPresetsType.LeftBottom);
            go.SetAnchorPos(Config.BuDingLeftPos);
            go.SetScale(Config.BuDingLeftScale);
            go.transform.SetParent(parent);
            go.AddSkeletonGraphic(Config.BDLeftSpinePath);
        }

        protected void CreateBg(Transform parent)
        {
            GameObject bG = new GameObject("BG");
            bG.transform.SetParent(parent);
            bG.AddEmpty4Ray();
            bG.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);


            GameObject bgChild = new GameObject("bg");
            bgChild.transform.SetParent(bG.transform);
            bgChild.AddRawImage(BgSprite);
            bgChild.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);

        }

        protected void CreateLucencyMask(Transform parent)
        {
            GameObject mask = new GameObject("mask");
            mask.transform.SetParent(parent);
            mask.AddEmpty4Ray();
            mask.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);
        }

        protected void CreateBlackMask(Transform parent)
        {
            GameObject mask = new GameObject("mask");
            mask.transform.SetParent(parent);
            mask.AddImage(new Color(0, 0, 0, 200 / 255f), true);

            mask.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);
        }

        protected void CreateLastNextSmallPartBtn(Transform parent)
        {
            GameObject last = new GameObject("LastBtn");
            last.transform.SetParent(parent);
            last.AddImage(true);
            last.AddRectTransform(new Vector2(226, 106), PivotPresetsType.LeftBottom, AnchorPresetsType.LeftBottom);
            last.GetComponent<UnityEngine.UI.Image>().sprite = EditorResourcesManager.LoadSpriteAsset(Config.LastBtnPath);

            GameObject next = new GameObject("NextBtn");
            next.transform.SetParent(parent);
            next.AddImage(true);
            next.AddRectTransform(new Vector2(226, 106), PivotPresetsType.RightBottom, AnchorPresetsType.RightBottom);
            next.GetComponent<UnityEngine.UI.Image>().sprite = EditorResourcesManager.LoadSpriteAsset(Config.NextBtnPath);
        }
    }
}


