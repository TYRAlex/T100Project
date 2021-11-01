using System;
using System.Collections.Generic;
using System.IO;

using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace TDCreate {

    public class CardPart : BassPart
    {

        //[AssetList(Path = "HotFixPackage")]
        public SkeletonDataAsset SkeletonData;


        [InfoBox("点击开始的名字(小写)：a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z")]
        public string StartName = string.Empty;


        [OnValueChanged("PageTypeCallBack")]
        public PageType PageType = PageType.Null;

        private void PageTypeCallBack()
        {   
            PageNums = 0;
            PageInfos.Clear();

            MoreSmallPartNums = 0;
            MoreSmallPartPageNums = 0;

            MoreSmallPartInfos.Clear();
            MoreSmallPartPageInfos.Clear();
        }

        #region 单页

        [HideLabel]
        [InfoBox("单页卡牌数量(最小是1,最大是3)")]
        [ShowIf("PageType", PageType.OnePage)]
        public int OnePageOnClickNums;

        #endregion

        #region 多页单环节

        private bool _isInputPageNums;

        [HideLabel]
        [InfoBox("页数")]
        [ShowIf("PageType", PageType.MorePageOneSmallPart)]     
        [OnValueChanged("InputPageNumsCallBack")]
        public int PageNums = 0;


        private void InputPageNumsCallBack()
        {
            _isInputPageNums = PageNums == 0;

            if (!_isInputPageNums)
            {
                CheckMorePageInfos(PageNums, PageInfos);
            }
            else
            {
                PageInfos.Clear();
            }
        }


        [HideLabel]
        [ShowIf("PageType", PageType.MorePageOneSmallPart)]      
        [HideIf("_isInputPageNums")]
        [DictionaryDrawerSettings(KeyLabel = "页", ValueLabel = "点击数量(最小值校验1,最大值为3)")]
        public Dictionary<int, int> PageInfos = new Dictionary<int, int>();

        #endregion

        #region 多页多环节

        private bool _isInputMoreSmallPartNums;

        [HideLabel]
        [InfoBox("小环节数量")]
        [ShowIf("PageType", PageType.MorePageMoreSmallPart)]
        [OnValueChanged("InputMoreSmallPartNumsCallBack")]
        public int MoreSmallPartNums = 0;


        private void InputMoreSmallPartNumsCallBack()
        {
            _isInputMoreSmallPartNums = MoreSmallPartNums == 0;

            if (!_isInputMoreSmallPartNums)
            {
                CheckMoreSmallPartInfos(MoreSmallPartNums, MoreSmallPartInfos);
            }
            else
            {
                MoreSmallPartInfos.Clear();
            }
        }
     
        [HideLabel]
        [ShowIf("PageType", PageType.MorePageMoreSmallPart)]
        [HideIf("_isInputMoreSmallPartNums")]
        [DictionaryDrawerSettings(KeyLabel = "小环节Index", ValueLabel = "小环节数据")]
        public Dictionary<int, CardMoreSmallPart> MoreSmallPartInfos = new Dictionary<int, CardMoreSmallPart>();



        private bool _isInputMoreSmallPartPageNums;

        [HideLabel]
        [InfoBox("页数")]
        [ShowIf("PageType", PageType.MorePageMoreSmallPart)]
        [OnValueChanged("InputMoreSmallPartPageNumsCallBack")]
        public int MoreSmallPartPageNums = 0;


        private void InputMoreSmallPartPageNumsCallBack()
        {
            _isInputMoreSmallPartPageNums = MoreSmallPartPageNums == 0;

            if (!_isInputMoreSmallPartPageNums)
            {
                CheckMorePageInfos(MoreSmallPartPageNums, MoreSmallPartPageInfos);
            }
            else
            {
                MoreSmallPartPageInfos.Clear();
            }
        }


        [HideLabel]
        [ShowIf("PageType", PageType.MorePageMoreSmallPart)]
        [HideIf("_isInputMoreSmallPartPageNums")]
        [DictionaryDrawerSettings(KeyLabel = "页", ValueLabel = "点击数量(最小值校验1,最大值为3)")]
        public Dictionary<int, int> MoreSmallPartPageInfos = new Dictionary<int, int>();

        #endregion;

        public CardPart()
        {
            StartName = String.Empty;
            OnePageOnClickNums = 0;
            PageNums = 0;
            PageInfos.Clear();


            MoreSmallPartNums = 0;
            MoreSmallPartPageNums = 0;
        
            MoreSmallPartInfos.Clear();
            MoreSmallPartPageInfos.Clear();
          
        }


        protected override void CreateScript()
        {
            switch (PageType)
            {
                case PageType.OnePage:
                    CreateCsProject(Config.TemplateCardPartOnePageCsPath,(contents, createCsPath) => {                 
                        contents = contents.Replace("卡牌点击次数替换", string.Format("_recordOnClickCardNums ={0};", OnePageOnClickNums));
                        File.WriteAllText(createCsPath, contents);
                    });
                    break;
                case PageType.MorePageOneSmallPart:
                    CreateCsProject(Config.TemplateCardPartMorePageOneSmallPartCsPath, (contents, createCsPath) => {
                       
                        int tempNums = 0;
                        foreach (var item in PageInfos)
                            tempNums = item.Value + tempNums;

                        contents = contents.Replace("卡牌点击次数替换", string.Format("_recordOnClickCardNums ={0};", tempNums));
                        File.WriteAllText(createCsPath, contents);
                    });             
                    break;
                case PageType.MorePageMoreSmallPart:
                    CreateCsProject(Config.TemplateCardPartMorePageMoreSmallPartCsPath, (contents, createCsPath) => {
                      
                        contents = contents.Replace("替换初始值最大索引", string.Format("_cardPageMaxIndex ={0};", MoreSmallPartInfos[0].MaxIndex));
                        contents = contents.Replace("替换初始值点击次数", string.Format("_recordOnClickCardNums ={0};", MoreSmallPartInfos[0].OnClickNum));

                        string replace1 = string.Empty;

                        for (int i = 0; i < MoreSmallPartInfos.Count; i++)
                        {
                            replace1 = replace1+"false";
                            if (i == MoreSmallPartInfos.Count - 1)
                                continue;
                            replace1 = replace1 + ",";
                        }

                        contents = contents.Replace("替换小环节bool集合", replace1);

                        contents = contents.Replace("去下个卡牌小环节",string.Format("ToOthersCardSmallPart({0},{1},{2},{3});", "false", MoreSmallPartInfos[1].MinIndex, MoreSmallPartInfos[1].MaxIndex, MoreSmallPartInfos[1].OnClickNum));
                        contents = contents.Replace("去上个卡牌小环节",string.Format("ToOthersCardSmallPart({0},{1},{2},{3});", "true", MoreSmallPartInfos[0].MinIndex, MoreSmallPartInfos[0].MaxIndex, MoreSmallPartInfos[0].OnClickNum));

                        File.WriteAllText(createCsPath, contents);
                    });
                    break;
            }
           
            CreateCsprojProject();
            OpenCsFolder();
        }

        protected override void CreatePrefab()
        {
            var rootTra = base.CreatePartRoot();
            CreateBg(rootTra);
            switch (PageType)
            {               
                case PageType.OnePage:
                    CreateSpines(rootTra);
                    break;
                case PageType.MorePageOneSmallPart:
                    CreateParts(rootTra);                 
                    break;
                case PageType.MorePageMoreSmallPart:
                    CreateParts(rootTra);
                    CreateLastNextSmallPartBtn(rootTra);
                    break;

            }


            CreateLucencyMask(rootTra);
            CreateLeftBd(rootTra);
            SavePrefab(rootTra.gameObject);
        }


        private void CreateSpines(Transform parent)
        {
            GameObject parts = new GameObject("Spines");
            parts.transform.SetParent(parent);
            parts.AddRectTransform(Config.ConstV2);

            int tempNameIndex = FindNameStartIndex();

            for (int i = 0; i < OnePageOnClickNums; i++)
            {
                var name = Config.CardSpineNames[tempNameIndex];
                GameObject spine = new GameObject(name + "3");
                spine.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop,
                    AnchorPresetsType.LeftTop);
                spine.transform.SetParent(parts.transform);
                spine.SetAnchorPos(new Vector2(0, 0));

                if (SkeletonData != null)
                    spine.AddSkeletonGraphic(SkeletonData);

                GameObject onClick = new GameObject(name);
                onClick.transform.SetParent(parts.transform);
                onClick.AddEmpty4Ray();

                GameObject soundIndexGo = new GameObject("0");
                soundIndexGo.transform.SetParent(onClick.transform);
                soundIndexGo.AddRectTransform(new Vector2(100, 100));

              

                if (OnePageOnClickNums == 1)
                {
                    onClick.AddRectTransform(Config.ConstOneCardV2);
                    onClick.SetAnchorPos(new Vector2(0, 0));
                }
                else if (OnePageOnClickNums == 2)
                {
                    onClick.AddRectTransform(Config.ConstTwoCardV2);
                    onClick.SetAnchorPos(i == 0 ? Config.ConstTwoCardLeftPos : Config.ConstTwoCardRightPos);
                }
                else if (OnePageOnClickNums == 3)
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

        private void CreateParts(Transform parent)
        {
            GameObject parts = new GameObject("Parts");
            parts.transform.SetParent(parent);
            parts.AddRectTransform(Config.ConstV2);
           
            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(parts.transform);
            mask.AddRectTransform(new Vector2(1400, 1080), PivotPresetsType.CenterMiddle, AnchorPresetsType.CenterStretch);
            mask.AddMask();
            mask.AddImage();

            GameObject pages = new GameObject("Pages");
            pages.transform.SetParent(mask.transform);
            pages.AddRectTransform(Config.ConstV2);
            pages.AddGridLayoutGroup(Config.ConstV2, Vector2.zero, new RectOffset(0, 0, 0, 0));


            int tempNameIndex = FindNameStartIndex();

            switch (PageType)
            {
                case PageType.MorePageOneSmallPart:
                    CreateMorePage(pages.transform, PageNums, PageInfos, SkeletonData, true, tempNameIndex);
                    break;
                case PageType.MorePageMoreSmallPart:
                    CreateMorePage(pages.transform, MoreSmallPartPageNums, MoreSmallPartPageInfos, SkeletonData, true, tempNameIndex);
                    break;
            }
       
            CreateSwitch(parts.transform);     
        }

        private int FindNameStartIndex()
        {
            int tempNameIndex = 0;

            if (StartName == string.Empty)
            {
                StartName = "a";
            }
            else
            {
                StartName = StartName.ToLower();
                bool isContains = Config.CardSpineNames.Contains(StartName);

                if (isContains)
                {
                    for (int i = 0; i < Config.CardSpineNames.Count; i++)
                    {
                        var name = Config.CardSpineNames[i];
                        if (name == StartName)
                        {
                            tempNameIndex = i;
                            break;
                        }
                    }
                }
            }

            return tempNameIndex;
        }

    }


}



   
