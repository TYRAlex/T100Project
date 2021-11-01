using System.Collections.Generic;

using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace TDCreate
{

    public class SumUpPart : BassPart
    {


        // [AssetList]
        public SkeletonDataAsset SkeletonData;


        private bool _isInputPageNums;

        [InfoBox("页数")]
        [OnValueChanged("InputPageNumsCallBack")]
        public int PageNums = 0;

        [HideLabel]
        [HideIf("_isInputPageNums")]
        [DictionaryDrawerSettings(KeyLabel = "页", ValueLabel = "点击数量(最小值校验1,最大值为3)")]
        public Dictionary<int, int> PageInfos = new Dictionary<int, int>();



        private void InputPageNumsCallBack()
        {
            _isInputPageNums = PageNums == 0;

            if (!_isInputPageNums)
                CheckMorePageInfos(PageNums, PageInfos);
            else
                PageInfos.Clear();
        }

        public SumUpPart()
        {

            _isInputPageNums = PageNums == 0;
        }

        protected override void CreateScript()
        {
            CreateCsProject(Config.TemplateSumUpPartCsPath);
            CreateCsprojProject();
            OpenCsFolder();
        }



        protected override void CreatePrefab()
        {

            var rootTra = base.CreatePartRoot();
            CreateBg(rootTra);
            CreateParts(rootTra);
            CreateLucencyMask(rootTra);
            SavePrefab(rootTra.gameObject);
        }




        private void CreateParts(Transform parent)
        {
            GameObject parts = new GameObject("Parts");
            parts.transform.SetParent(parent);
            parts.AddEmpty4Ray();
            parts.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.CenterMiddle);


            GameObject mask = new GameObject("Mask");
            mask.transform.SetParent(parts.transform);
            mask.AddRectTransform(new Vector2(1400, 1080), PivotPresetsType.CenterMiddle, AnchorPresetsType.CenterStretch);
            mask.AddImage();
            mask.AddMask();

            GameObject pages = new GameObject("Pages");
            pages.transform.SetParent(mask.transform);
            pages.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.CenterMiddle);
            pages.AddGridLayoutGroup(Config.ConstV2, Vector2.zero, new RectOffset(0, 0, 0, 0));


            CreateMorePage(pages.transform, PageNums, PageInfos, SkeletonData);

            CreateSwitch(parts.transform);
        }

    }

}

  
