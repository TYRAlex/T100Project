using System.Collections.Generic;
using UnityEngine;


   namespace TDCreate
{

    public struct CardMoreSmallPart
    {
        public int MinIndex;
        public int MaxIndex;
        public int OnClickNum;
    }

    public enum PageType
    {
        Null,
        OnePage,
        MorePageOneSmallPart,
        MorePageMoreSmallPart,
    }

    public enum PartType
    {
        Null,
        KnowPartType,  //材料Type
        CardPartType,  //卡牌Type
        SpritePartType,//精灵Type
        GamePartType,  //游戏Type
        SumUpPartType, //总结Type           
    }


    public enum AgeType
    {
        None,
        Child,
        Adult,
    }




    public class Config
    {


        public static readonly List<string> CardSpineNames = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static readonly Vector2 ConstV2 = new Vector2(1920, 1080);
        public static readonly Vector2 ConstSpineV2 = new Vector2(100, 100);
        public static readonly Vector2 ConstSpineBtnV2 = new Vector2(240, 240);
        public static readonly Vector2 ConstReplayBtnPos = new Vector2(-240, 0);
        public static readonly Vector2 ConstOkBtnPos = new Vector2(240, 0);

        public static readonly Vector2 ConstArrowV2 = new Vector2(136, 175);
        public static readonly Vector2 ConstLeftArrowPos = new Vector2(187, -2.5f);
        public static readonly Vector2 ConstRightArrowPos = new Vector2(-187, -2.5f);


        public static readonly Vector2 ConstOneCardV2 = new Vector2(620, 460);
        public static readonly Vector2 ConstTwoCardV2 = new Vector2(640, 462);
        public static readonly Vector2 ConstThreeCardV2 = new Vector2(400, 300);

        public static readonly Vector2 ConstTwoCardLeftPos = new Vector2(-355, 0);
        public static readonly Vector2 ConstTwoCardRightPos = new Vector2(355, 0);

        public static readonly Vector2 ConstThreeCardLeftPos = new Vector2(-480, 0);
        public static readonly Vector2 ConstThreeCardMiddlePos = new Vector2(0, 0);
        public static readonly Vector2 ConstThreeCardRightPos = new Vector2(480, 0);

        public const string HotFixPackageRootPath = "Assets/HotFixPackage/";
        public const string HotFixCodePackageRootPath = "Assets/../HotfixCodeProject/";

        public const string CommonSpineRootPath = "Assets/ILFramework/CommonRes/Spine/";
        public const string BDMiddleSpinePath = CommonSpineRootPath + "buding_TDCommon/buding_SkeletonData.asset";
        public const string BDLeftSpinePath = CommonSpineRootPath + "buding_TD/buding_4-3_SkeletonData.asset";
        public const string SpSpinePath = CommonSpineRootPath + "ChengGong/sp_SkeletonData.asset";
        public const string LRSpinePath = CommonSpineRootPath + "LRBtn/LR_SkeletonData.asset";
        public const string BtnsSpinePath = CommonSpineRootPath + "OkAndRetry/PLAY_SkeletonData.asset";
        public const string SuccessChildSpinePath = CommonSpineRootPath + "ChengGong/3-5_SkeletonData.asset";
        public const string SuccessAdultSpinePath = CommonSpineRootPath + "ChengGong/6-12_SkeletonData.asset";
        public const string XemMiddleSpinePath = CommonSpineRootPath + "xiaoemo/xem_SkeletonData.asset";
        public const string DingDingChildSpinePath = CommonSpineRootPath + "dingding_Child/nan_SkeletonData.asset";
        public const string DingDingAdultSpinePath = CommonSpineRootPath + "dingding_Adult/dingding_SkeletonData.asset";
        public const string TianTianChildSpinePath = CommonSpineRootPath + "tiantian_Child/nv_SkeletonData.asset";
        public const string TianTianAdultSpinePath = CommonSpineRootPath + "tiantian_Adult/tiantian_SkeletonData.asset";

        public const string BdDialoguesSpinePath = CommonSpineRootPath + "Dialogues/bd/bd_SkeletonData.asset";
        public const string XemDialoguesSpinePath = CommonSpineRootPath + "Dialogues/devil/xem_SkeletonData.asset";
        public const string DingDingChildDialoguesSpinePath = CommonSpineRootPath + "Dialogues/dingding_Child/xd_SkeletonData.asset";
        public const string DingDingAdultDialoguesSpinePath = CommonSpineRootPath + "Dialogues/dingding_Adult/dd_SkeletonData.asset";
        public const string TianTianChildDialoguesSpinePath = CommonSpineRootPath + "Dialogues/tiantian_Child/xt_SkeletonData.asset";
        public const string TianTianAdultDialoguesSpinePath = CommonSpineRootPath + "Dialogues/tiantian_Adult/dt_SkeletonData.asset";

        public const string DialoguesFontPath = "Assets/ILFramework/CommonRes/Font/站酷快乐体2016.ttf";

        public const string LastBtnPath = "Assets/ILFramework/CommonRes/Textures/SwitchBtn/Last1.png";
        public const string NextBtnPath = "Assets/ILFramework/CommonRes/Textures/SwitchBtn/Next1.png";

        public const string TemplateKnowPartOnePageOneSmallPartCsPath = HotFixCodePackageRootPath + "Template/KnowPartOnePageOneSmallPartTemplate.txt";
        public const string TemplateKnowPartMorePageOneSmallPartCsPath = HotFixCodePackageRootPath + "Template/KnowPartMorePageOneSmallPartTemplate.txt";
        public const string TemplateKnowPartMorePageMoreSmallPartCsPath = HotFixCodePackageRootPath + "Template/KnowPartMorePageMoreSmallPartTemplate.txt";

        public const string TemplateGamePartCsPath = HotFixCodePackageRootPath + "Template/GamePartTemplate.txt";
       
        public const string TemplateCardPartOnePageCsPath = HotFixCodePackageRootPath + "Template/CardPartOnePageTemplate.txt";
        public const string TemplateCardPartMorePageOneSmallPartCsPath = HotFixCodePackageRootPath + "Template/CardPartMorePageOneSmallPartTemplate.txt";
        public const string TemplateCardPartMorePageMoreSmallPartCsPath = HotFixCodePackageRootPath + "Template/CardPartMorePageMoreSmallPartTemplate.txt";

        public const string TemplateSpritePartCsPath = HotFixCodePackageRootPath + "Template/SpritePartTemplate.txt";
        public const string TemplateSumUpPartCsPath = HotFixCodePackageRootPath + "Template/SumUpPartTemplate.cs";
        public const string TemplateCsprojPath = HotFixCodePackageRootPath + "Template/CsprojTemplate.csproj";
        public const string AudioPrefabDir = "Assets/Editor/Template/AudiosPrefab/";
        public const string BgmPart = AudioPrefabDir + "CourseBgm_htp.prefab";
        public const string SoundPart = AudioPrefabDir + "CourseSound_htp.prefab";
        public const string VoicePart = AudioPrefabDir + "CourseVoice_htp.prefab";

        public const string TDBatPart =  "/Template/TDBat.bat";

        #region 丁丁相关 

        public static readonly Vector2 DingDingChildLeftPos = new Vector2(248, -108);
        public static readonly Vector2 DingDingChildLeftScale = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 DingDingChildMiddlePos = new Vector2(980, -130);
        public static readonly Vector2 DingDingChildMiddleScale = new Vector2(0.65f, 0.65f);
        public static readonly Vector2 DingDingAdultLeftPos = new Vector2(270, -206);
        public static readonly Vector2 DingDingAdultLeftScale = new Vector2(1.2f, 1.2f);
        public static readonly Vector2 DingDingAdultMiddlePos = new Vector2(980, -239);
        public static readonly Vector2 DingDingAdultMiddleScale = new Vector2(1.5f, 1.5f);
        #endregion

        #region 田田相关


        public static readonly Vector2 TianTianChildLeftPos = new Vector2(255, -84);
        public static readonly Vector2 TianTianChildLeftScale = new Vector2(0.45f, 0.45f);
        public static readonly Vector2 TianTianChildMiddlePos = new Vector2(973, -101);
        public static readonly Vector2 TianTianChildMiddleScale = new Vector2(0.6f, 0.6f);
        public static readonly Vector2 TianTianAdultLeftPos = new Vector2(292, -204);
        public static readonly Vector2 TianTianAdultLeftScale = new Vector2(1.25f, 1.25f);
        public static readonly Vector2 TianTianAdultMiddlePos = new Vector2(973, -228);
        public static readonly Vector2 TianTianAdultMiddleScale = new Vector2(1.55f, 1.55f);

        #endregion

        #region 布丁相关


        public static readonly Vector2 BuDingLeftPos = new Vector2(300, 78);
        public static readonly Vector2 BuDingLeftScale = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 BuDingMiddlePos = new Vector2(960, 150);
        public static readonly Vector2 BuDingMiddleScale = new Vector2(0.65f, 0.65f);
        #endregion






    }

}

