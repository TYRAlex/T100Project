using System.Collections.Generic;
using System.IO;

using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;


namespace TDCreate {


    public class KnowPart : BassPart
    {

        // [AssetList]
        public SkeletonDataAsset KnowSkeletonData;

        // [AssetList]
        public SkeletonDataAsset CardSkeletonData;


        private bool _isInputKnowNums;

        [OnValueChanged("InputKnowNumsCallBack")]
        [InfoBox("认识材料数量")]
        public int KnowNums;

        private void InputKnowNumsCallBack()
        {
            _isInputKnowNums = KnowNums == 0;
            if (_isInputKnowNums)
            {
                PageType = PageType.Null;
                OnePageOnClickNums = 0;
                PageNums = 0;
                PageInfos.Clear();

                MoreSmallPartNums = 0;
                MoreSmallPartPageNums = 0;

                MoreSmallPartInfos.Clear();
                MoreSmallPartPageInfos.Clear();
            }
        }


        [OnValueChanged("PageTypeCallBack")]
        [HideIf("_isInputKnowNums")]
        public PageType PageType = PageType.Null;

        private void PageTypeCallBack()
        {
            OnePageOnClickNums = 0;
            PageNums = 0;
            PageInfos.Clear();


            MoreSmallPartNums = 0;
            MoreSmallPartPageNums = 0;

            MoreSmallPartInfos.Clear();
            MoreSmallPartPageInfos.Clear();
        }

        #region 单页单环节

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


        public KnowPart()
        {
            KnowNums = 0;
            _isInputKnowNums = KnowNums == 0;
            PageNums = 0;
            OnePageOnClickNums = 0;
            _isInputPageNums = PageNums == 0;

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
                    CreateCsProject(Config.TemplateKnowPartOnePageOneSmallPartCsPath, (contents, createCsPath) => {
                        contents = contents.Replace("卡牌点击次数替换", string.Format("_recordOnClickCardNums ={0};", OnePageOnClickNums));
                        File.WriteAllText(createCsPath, contents);
                    });
                    break;
                case PageType.MorePageOneSmallPart:
                    CreateCsProject(Config.TemplateKnowPartMorePageOneSmallPartCsPath, (contents, createCsPath) => {
                        contents = contents.Replace("替换最大索引", string.Format("{0}", PageInfos.Count-1));

                        int tempNums = 0;
                        foreach (var item in PageInfos)
                            tempNums = item.Value + tempNums;

                        contents = contents.Replace("替换点击次数", string.Format("{0}", tempNums));

                        File.WriteAllText(createCsPath, contents);
                    });
                    break;
                case PageType.MorePageMoreSmallPart:

                    CreateCsProject(Config.TemplateKnowPartMorePageMoreSmallPartCsPath, (contents, createCsPath) => {

                        string replace1 = string.Empty;

                        for (int i = 0; i < MoreSmallPartInfos.Count; i++)
                        {
                            replace1 = replace1 + "false";
                            if (i == MoreSmallPartInfos.Count - 1)
                                continue;
                            replace1 = replace1 + ",";
                        }

                        contents = contents.Replace("替换小环节bool集合", replace1);

                        contents = contents.Replace("替换材料去卡1最大索引", string.Format("{0}",MoreSmallPartInfos[0].MaxIndex));
                        contents = contents.Replace("替换材料去卡1点击次数", string.Format("{0}", MoreSmallPartInfos[0].OnClickNum));

                        contents = contents.Replace("替换卡1去卡2开始索引", string.Format("{0}", MoreSmallPartInfos[1].MinIndex));
                        contents = contents.Replace("替换卡1去卡2最大索引", string.Format("{0}", MoreSmallPartInfos[1].MaxIndex));
                        contents = contents.Replace("替换卡1去卡2点击次数", string.Format("{0}", MoreSmallPartInfos[1].OnClickNum));

                        contents = contents.Replace("替换卡2去卡1开始索引", string.Format("{0}", MoreSmallPartInfos[0].MinIndex));
                        contents = contents.Replace("替换卡2去卡1最大索引", string.Format("{0}", MoreSmallPartInfos[0].MaxIndex));
                        contents = contents.Replace("替换卡2去卡1点击次数", string.Format("{0}", MoreSmallPartInfos[0].OnClickNum));


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

      
            CreateParts(rootTra);
            CreateLastNextSmallPartBtn(rootTra);
            CreateLucencyMask(rootTra);
            CreateLeftBd(rootTra);
            SavePrefab(rootTra.gameObject);
        }


        private void CreateParts(Transform parent)
        {
            GameObject parts = new GameObject("Parts");
            parts.transform.SetParent(parent);
            parts.AddRectTransform(Config.ConstV2);

            GameObject spineParent = new GameObject("1");
            spineParent.transform.SetParent(parts.transform);
            spineParent.AddRectTransform(Config.ConstV2);

            GameObject spine = new GameObject("0");
            spine.transform.SetParent(spineParent.transform);
            spine.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
            spine.AddSkeletonGraphic(KnowSkeletonData);


            for (int i = 0; i < KnowNums; i++)
            {
                GameObject go = new GameObject((i + 1).ToString());
                go.transform.SetParent(spineParent.transform);
                go.AddRectTransform(new Vector2(300, 400));
                go.AddEmpty4Ray();

                GameObject soundGo = new GameObject("0");
                soundGo.transform.SetParent(go.transform);
                soundGo.AddRectTransform(new Vector2(100, 100));
            }

            GameObject pagesRoot = new GameObject("2");
            pagesRoot.transform.SetParent(parts.transform);
            pagesRoot.AddRectTransform(Config.ConstV2);

            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(pagesRoot.transform);
            mask.AddRectTransform(new Vector2(1400, 1080), PivotPresetsType.CenterMiddle,
                AnchorPresetsType.CenterStretch);
            mask.AddImage();
            mask.AddMask();

            GameObject pages = new GameObject("Pages");
            pages.transform.SetParent(mask.transform);
            pages.AddRectTransform(Config.ConstV2);
            pages.AddGridLayoutGroup(Config.ConstV2, Vector2.zero, new RectOffset(0, 0, 0, 0));

            switch (PageType)
            {
                case PageType.Null:
                    break;
                case PageType.OnePage:
                    CreateOnePage(pages.transform, OnePageOnClickNums, CardSkeletonData, true);

                    break;
                case PageType.MorePageOneSmallPart:
               
                    CreateMorePage(pages.transform, PageNums, PageInfos, CardSkeletonData, true);
                    CreateSwitch(parts.transform);
                    break;
                case PageType.MorePageMoreSmallPart:
                    CreateMorePage(pages.transform, MoreSmallPartPageNums, MoreSmallPartPageInfos, CardSkeletonData, true);
                    CreateSwitch(parts.transform);
                    break;
            }
        }
    }
}

 
