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

        public const string CommonSpineRootPath = "Assets/HotFixPackage/TDAssets/Spines/";                               //田丁spine根目录
        public const string CommonTextureRootPath = "Assets/HotFixPackage/TDAssets/Textures/";                           //田丁texture根目录
        //角色
        public const string BDMiddleSpinePath  = CommonSpineRootPath + "role/role_buding/buding_M/buding-M_SkeletonData.asset";     //中大布丁
        public const string BDLeftSpinePath    = CommonSpineRootPath + "role/role_buding/buding_L/buding-L_SkeletonData.asset";     //左小布丁
        public const string XemMiddleSpinePath = CommonSpineRootPath + "role/role_xiaoemo/xem_SkeletonData.asset";                  //小恶魔

        public const string DingDingChildSpinePath = CommonSpineRootPath + "role/role_dingding/Child/dingding_SkeletonData.asset";  //丁丁小年龄
        public const string DingDingAdultSpinePath = CommonSpineRootPath + "role/role_dingding/Adult/dingding_SkeletonData.asset"; //丁丁大年龄
        public const string TianTianChildSpinePath = CommonSpineRootPath + "role/role_tiantian/Child/tiantian_SkeletonData.asset"; //田田小年龄
        public const string TianTianAdultSpinePath = CommonSpineRootPath + "role/role_tiantian/Adult/tiantian_SkeletonData.asset"; //田田大年龄

        //结果反馈
        public const string SpSpinePath = CommonSpineRootPath           + "result/succeed/sp_SkeletonData.asset";              //彩带
        public const string SuccessChildSpinePath = CommonSpineRootPath + "result/succeed/Successfully_SkeletonData.asset";    //成功小年龄
        public const string SuccessAdultSpinePath = CommonSpineRootPath + "result/succeed/Successfully_SkeletonData.asset";    //成功大年龄  
        public const string FailChildSpinePath = CommonSpineRootPath    + "result/fail/Child/Fail-Child_SkeletonData.asset";   //失败小年龄
        public const string FailAdultSpinePath = CommonSpineRootPath    + "result/fail/Adult/Fail-Adult_SkeletonData.asset";   //失败大年龄

        //按钮
        public const string BtnsSpinePath = CommonSpineRootPath + "btn/btn_start-ok-retry/btns_SkeletonData.asset";    //开始、重玩、Ok
        public const string LRSpinePath   = CommonSpineRootPath + "btn/btn_lr/LR_SkeletonData.asset";                 //左右箭头
                   
        //对话框
        public const string BdDialoguesSpinePath = CommonSpineRootPath            + "dialogues/dialogues_buding/bd_SkeletonData.asset";          //布丁对话框
        public const string XemDialoguesSpinePath = CommonSpineRootPath           + "dialogues/dialogues_xiaoemo/xem_SkeletonData.asset";     //小恶魔对话框
        public const string DingDingChildDialoguesSpinePath = CommonSpineRootPath + "dialogues/dialogues_dingding/Child/xd_SkeletonData.asset";  //对话框丁丁小年龄
        public const string DingDingAdultDialoguesSpinePath = CommonSpineRootPath + "dialogues/dialogues_dingding/Adult/dd_SkeletonData.asset";  //对话框丁丁大年龄
        public const string TianTianChildDialoguesSpinePath = CommonSpineRootPath + "dialogues/dialogues_tiantian/Child/xt_SkeletonData.asset";  //对话框田田小年龄
        public const string TianTianAdultDialoguesSpinePath = CommonSpineRootPath + "dialogues/dialogues_tiantian/Adult/dt_SkeletonData.asset"; //对话框田田大年龄

        public const string DialoguesFontPath = "Assets/ILFramework/CommonRes/Font/站酷快乐体2016.ttf";

        public const string LastBtnPath = CommonTextureRootPath+"SwitchBtn/Last1.png";    //上一环节png路径
        public const string NextBtnPath = CommonTextureRootPath+"SwitchBtn/Next1.png";    //下一环节png路径

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

        public static readonly Vector2 DingDingChildLeftPos = new Vector2(300, -70);
        public static readonly Vector2 DingDingChildLeftScale = new Vector2(1.2f, 1.2f);
        public static readonly Vector2 DingDingChildMiddlePos = new Vector2(0, -100);
        public static readonly Vector2 DingDingChildMiddleScale = new Vector2(1.6f, 1.6f);
        public static readonly Vector2 DingDingAdultLeftPos = new Vector2(270, -206);
        public static readonly Vector2 DingDingAdultLeftScale = new Vector2(1.2f, 1.2f);
        public static readonly Vector2 DingDingAdultMiddlePos = new Vector2(980, -239);
        public static readonly Vector2 DingDingAdultMiddleScale = new Vector2(1.5f, 1.5f);
        #endregion

        #region 田田相关


        public static readonly Vector2 TianTianChildLeftPos = new Vector2(300, -50);
        public static readonly Vector2 TianTianChildLeftScale = new Vector2(1.2f, 1.2f);
        public static readonly Vector2 TianTianChildMiddlePos = new Vector2(0, -70);
        public static readonly Vector2 TianTianChildMiddleScale = new Vector2(1.6f, 1.6f);

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

