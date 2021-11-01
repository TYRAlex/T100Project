using System.Collections.Generic;
using System.IO;

using Sirenix.OdinInspector;
using UnityEngine;

namespace TDCreate {

    public class GamePart : BassPart
    {
        [BoxGroup("年龄类型")]
        public AgeType AgeType = AgeType.None;


        [HorizontalGroup("角色")]
        [BoxGroup("角色/Left(角色)")]
        public bool LeftBuDing, LeftDingDing, LeftTianTian, LeftNull;

        [BoxGroup("角色/Middle(角色)")]
        public bool MiddleBuDing, MiddleDingDing, MiddleTianTian, MiddleXem;


        [BoxGroup("情景对话")]
        public bool IsDialogue;


        [BoxGroup("情景对话")]
        [ShowIf("IsDialogue")]
        public bool DialogueDingDing, DialogueTianTian, DialogueBd, DialogueXem;

        [BoxGroup("按钮")] public bool IsNeedNextBtn;


        public GamePart()
        {
            AgeType = AgeType.None;
            LeftBuDing = LeftDingDing = LeftTianTian = false;
            MiddleBuDing = MiddleDingDing = MiddleTianTian = MiddleXem = false;
            IsDialogue = false;
            DialogueDingDing = DialogueTianTian = DialogueBd = false;
            DialogueXem = true;
            IsNeedNextBtn = false;
        }


        protected override void CreateScript()
        {
            CreateCsProject(Config.TemplateGamePartCsPath, (contents, createCsPath) => {

                #region 下一步
             
                string nextSpineReplace1 = string.Empty;
              
                if (IsNeedNextBtn)
                {
                    contents = contents.Replace("private GameObject _start,_replay, _ok;",  "private GameObject _start,_replay, _ok,_next;");
                    nextSpineReplace1 = "_next = curTrans.GetGameObject(\"common/btns/next\"); //下一步";                  
                }          
                contents = contents.Replace("nextSpineReplace1", nextSpineReplace1);

                #endregion
                #region 角色

                string bDReplace1 = string.Empty;
                string bDReplace2 = string.Empty;
                string dDReplace1 = string.Empty;
                string dDReplace2 = string.Empty;
                string tTReplace1 = string.Empty;
                string tTReplace2 = string.Empty;
                string xemReplace = string.Empty;

                List<string> roles = new List<string>();

                if (LeftBuDing)
                { 
                    roles.Add("_sBD");
                    bDReplace1 = "_sBD = curTrans.GetGameObject(\"roles/sBD\");  //左下小布丁";
                }

                if (MiddleBuDing)
                {
                    roles.Add("_dBD");
                    bDReplace2 = "_dBD = curTrans.GetGameObject(\"roles/dBD\");  //中间大布丁";
                }

                if (LeftDingDing)
                {
                    roles.Add("_sDD");
                    dDReplace1 = "_sDD = curTrans.GetGameObject(\"roles/sDD\");  //左下小丁丁";
                }

                if (MiddleDingDing)
                { 
                    roles.Add("_dDD");
                    dDReplace2 = "_dDD = curTrans.GetGameObject(\"roles/dDD\");  //中间大丁丁";
                }

                if (LeftTianTian)
                { 
                    roles.Add("_sTT");
                    tTReplace1 = "_sTT = curTrans.GetGameObject(\"roles/sTT\");  //左下小田田";
                }

                if (MiddleTianTian)
                { 
                    roles.Add("_dTT");
                    tTReplace2 = "_dTT = curTrans.GetGameObject(\"roles/dTT\");  //中间大田田";
                }

                if (MiddleXem)
                { 
                    roles.Add("_xem");
                    xemReplace = "_xem = curTrans.GetGameObject(\"roles/xem\");  //中间大小恶魔";
                }

                bool isCountZero = roles.Count == 0;

                string roleReplace = string.Empty;

                if (!isCountZero)
                {
                    for (int i = 0; i < roles.Count-1; i++)
                        roleReplace += roles[i] + ",";
                    roleReplace += roles[roles.Count - 1];
                    roleReplace = string.Format("private GameObject {0};", roleReplace);
                }

                contents = contents.Replace("roleReplace", roleReplace);

                contents = contents.Replace("bDReplace1", bDReplace1);
                contents = contents.Replace("bDReplace2", bDReplace2);
                contents = contents.Replace("dDReplace1", dDReplace1);
                contents = contents.Replace("dDReplace2", dDReplace2);
                contents = contents.Replace("tTReplace1", tTReplace1);
                contents = contents.Replace("tTReplace2", tTReplace2);
                contents = contents.Replace("xemReplace", xemReplace);


                #endregion

                #region 对话框

                string diakogueReplace1 = string.Empty;
                string diakogueReplace2 = string.Empty;

                if (IsDialogue)
                {
                    diakogueReplace1 = "private GameObject _dialogue;  //对话框";
                    diakogueReplace2 = "_dialogue = curTrans.GetGameObject(\"dialogue\");  //对话框父物体GameObject";
                }

                contents = contents.Replace("diakogueReplace1", diakogueReplace1);
                contents = contents.Replace("diakogueReplace2", diakogueReplace2);

                #endregion

                File.WriteAllText(createCsPath, contents);

            });

            CreateCsprojProject();
            OpenCsFolder();
        }

        protected override void CreatePrefab()
        {
            var rootTra = base.CreatePartRoot();
            CreateBg(rootTra);
            CreateBlackMask(rootTra);
            CreateCommon(rootTra);
          
            if (IsDialogue)
                CreateDialogue(rootTra);

            CreateRole(rootTra);
            SavePrefab(rootTra.gameObject);
        }

        private void CreateCommon(Transform parent)
        {

            GameObject common = new GameObject("common");
            common.transform.SetParent(parent);
            common.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);

            GameObject btns = new GameObject("btns");
            btns.transform.SetParent(common.transform);
            btns.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);

            GameObject result = new GameObject("result");
            result.transform.SetParent(common.transform);
            result.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);


            GameObject start = new GameObject("start");
            start.transform.SetParent(btns.transform);
            start.AddRectTransform(Config.ConstSpineBtnV2);
            start.SetAnchorPos(Vector2.zero);
            start.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);

            GameObject replay = new GameObject("replay");
            replay.transform.SetParent(btns.transform);
            replay.AddRectTransform(Config.ConstSpineBtnV2);
            replay.SetAnchorPos(Config.ConstReplayBtnPos);
            replay.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);

            GameObject ok = new GameObject("ok");
            ok.transform.SetParent(btns.transform);
            ok.AddRectTransform(Config.ConstSpineBtnV2);
            ok.SetAnchorPos(Config.ConstOkBtnPos);
            ok.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);

            if (IsNeedNextBtn)
            {
                GameObject next = new GameObject("next");
                next.transform.SetParent(btns.transform);
                next.AddRectTransform(Config.ConstSpineBtnV2);
                next.SetAnchorPos(Vector2.zero);
                next.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);
            }

            CreateResultSpine(result.transform);
        }

        private void CreateResultSpine(Transform parent)
        {

            GameObject success = new GameObject("success");
            success.transform.SetParent(parent);
            success.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);

            GameObject fail = new GameObject("fail");
            fail.transform.SetParent(parent);
            fail.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);

            switch (AgeType)
            {
                case AgeType.Child:
                    success.AddSkeletonGraphic(Config.SuccessChildSpinePath);
                    fail.AddSkeletonGraphic(Config.FailChildSpinePath);
                    break;
                case AgeType.Adult:
                    success.AddSkeletonGraphic(Config.SuccessAdultSpinePath);
                    fail.AddSkeletonGraphic(Config.FailAdultSpinePath);
                    break;
            }


            GameObject sp = new GameObject("sp");
            sp.transform.SetParent(success.transform);
            sp.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
            sp.AddSkeletonGraphic(Config.SpSpinePath);
        }

        private void CreateDialogue(Transform parent)
        {
            GameObject dialogue = new GameObject("dialogue");
            dialogue.transform.SetParent(parent);
            dialogue.AddRectTransform(Config.ConstV2);

            GameObject roles = new GameObject("roles");
            roles.transform.SetParent(dialogue.transform);
            roles.AddRectTransform(Config.ConstV2);


            GameObject xem = new GameObject("xem");
            xem.transform.SetParent(roles.transform);
            xem.AddRectTransform(Config.ConstSpineV2);
            xem.SetAnchorPos(new Vector2(200, 540));
            xem.AddSkeletonGraphic(Config.XemDialoguesSpinePath, "dj", true);

            GameObject xemText = new GameObject("Text");
            xemText.transform.SetParent(xem.transform);
            xemText.AddRectTransform(new Vector2(500, 36), PivotPresetsType.CenterTop, AnchorPresetsType.CenterTop);
            xemText.SetAnchorPos(new Vector2(1305, -339));
            xemText.AddText();
            xemText.AddContentSizeFitter();

            if (DialogueBd)
            {
                GameObject bd = new GameObject("bd");
                bd.transform.SetParent(roles.transform);
                bd.AddRectTransform(Config.ConstSpineV2);
                bd.SetAnchorPos(new Vector2(-2170, 539));
                bd.AddSkeletonGraphic(Config.BdDialoguesSpinePath, "dj", true);

                GameObject bdText = new GameObject("Text");
                bdText.transform.SetParent(bd.transform);
                bdText.AddRectTransform(new Vector2(500, 36), PivotPresetsType.CenterTop, AnchorPresetsType.CenterTop);
                bdText.SetAnchorPos(new Vector2(700, -855));
                bdText.AddText();
                bdText.AddContentSizeFitter();
            }

            if (DialogueDingDing)
            {
                GameObject dingDing = new GameObject("dingDing");
                dingDing.transform.SetParent(roles.transform);
                dingDing.AddRectTransform(Config.ConstSpineV2);
                dingDing.SetAnchorPos(new Vector2(-2170, 539));

                switch (AgeType)
                {
                    case AgeType.Child:
                        dingDing.AddSkeletonGraphic(Config.DingDingChildDialoguesSpinePath, "dj", true);
                        break;
                    case AgeType.Adult:
                        dingDing.AddSkeletonGraphic(Config.DingDingAdultDialoguesSpinePath, "dj", true);
                        break;
                }


                GameObject dingDingText = new GameObject("Text");
                dingDingText.transform.SetParent(dingDing.transform);
                dingDingText.AddRectTransform(new Vector2(500, 36), PivotPresetsType.CenterTop, AnchorPresetsType.CenterTop);
                dingDingText.SetAnchorPos(new Vector2(700, -855));
                dingDingText.AddText();
                dingDingText.AddContentSizeFitter();
            }

            if (DialogueTianTian)
            {
                GameObject tianTian = new GameObject("tianTian");
                tianTian.transform.SetParent(roles.transform);
                tianTian.AddRectTransform(Config.ConstSpineV2);
                tianTian.SetAnchorPos(new Vector2(-2170, 539));


                switch (AgeType)
                {
                    case AgeType.Child:
                        tianTian.AddSkeletonGraphic(Config.TianTianChildDialoguesSpinePath, "dj", true);
                        break;
                    case AgeType.Adult:
                        tianTian.AddSkeletonGraphic(Config.TianTianAdultDialoguesSpinePath, "dj", true);
                        break;
                }

                GameObject tianTianText = new GameObject("Text");
                tianTianText.transform.SetParent(tianTian.transform);
                tianTianText.AddRectTransform(new Vector2(500, 36), PivotPresetsType.CenterTop, AnchorPresetsType.CenterTop);
                tianTianText.SetAnchorPos(new Vector2(700, -855));
                tianTianText.AddText();
                tianTianText.AddContentSizeFitter();
            }


            GameObject pos = new GameObject("pos");
            pos.transform.SetParent(dialogue.transform);
            pos.AddRectTransform(Config.ConstV2);

            GameObject xemStartPos = new GameObject("xemStartPos");
            xemStartPos.transform.SetParent(pos.transform);
            xemStartPos.AddRectTransform(Config.ConstSpineV2);
            xemStartPos.SetAnchorPos(new Vector2(2000,540));

            GameObject xemEndPos = new GameObject("xemEndPos");
            xemEndPos.transform.SetParent(pos.transform);
            xemEndPos.AddRectTransform(Config.ConstSpineV2);
            xemEndPos.SetAnchorPos(new Vector2(-994, 540));


            GameObject roleStartPos = new GameObject("roleStartPos");
            roleStartPos.transform.SetParent(pos.transform);
            roleStartPos.AddRectTransform(Config.ConstSpineV2);
            roleStartPos.SetAnchorPos(new Vector2(-2170, 539));

            GameObject roleEndPos = new GameObject("roleEndPos");
            roleEndPos.transform.SetParent(pos.transform);
            roleEndPos.AddRectTransform(Config.ConstSpineV2);
            roleEndPos.SetAnchorPos(new Vector2(-960, 539));


        }

        private void CreateRole(Transform parent)
        {
            GameObject roles = new GameObject("roles");
            roles.transform.SetParent(parent);
            roles.AddRectTransform(Config.ConstV2, PivotPresetsType.CenterMiddle, AnchorPresetsType.StretchAll);
            parent = roles.transform;
            if (LeftBuDing)
                CreateLeftBd(parent);

            if (MiddleBuDing)
                CreateMiddleBd(parent);

            if (MiddleXem)
                CreateMiddleXem(parent);


            switch (AgeType)
            {

                case AgeType.Child:

                    if (LeftDingDing)
                    {
                        var dingding = CreateRole(parent, "sDD", Config.DingDingChildLeftPos, Config.DingDingChildLeftScale);
                        dingding.AddSkeletonGraphic(Config.DingDingChildSpinePath);
                    }

                    if (MiddleDingDing)
                    {
                        var dingding = CreateRole(parent, "dDD", Config.DingDingChildMiddlePos, Config.DingDingChildMiddleScale,PivotPresetsType.CenterBottom,AnchorPresetsType.CenterBottom);
                        dingding.AddSkeletonGraphic(Config.DingDingChildSpinePath);
                    }

                    if (LeftTianTian)
                    {
                        var tiantian = CreateRole(parent, "sTT", Config.TianTianChildLeftPos, Config.TianTianChildLeftScale);
                        tiantian.AddSkeletonGraphic(Config.TianTianChildSpinePath);
                    }

                    if (MiddleTianTian)
                    {
                        var tiantian = CreateRole(parent, "dTT", Config.TianTianChildMiddlePos, Config.TianTianChildMiddleScale, PivotPresetsType.CenterBottom, AnchorPresetsType.CenterBottom);
                        tiantian.AddSkeletonGraphic(Config.TianTianChildSpinePath);
                    }

                    break;
                case AgeType.Adult:
                    if (LeftDingDing)
                    {
                        var dingding = CreateRole(parent, "sDD", Config.DingDingAdultLeftPos, Config.DingDingAdultLeftScale);
                        dingding.AddSkeletonGraphic(Config.DingDingAdultSpinePath);
                    }

                    if (MiddleDingDing)
                    {
                        var dingding = CreateRole(parent, "dDD", Config.DingDingAdultMiddlePos, Config.DingDingAdultMiddleScale);
                        dingding.AddSkeletonGraphic(Config.DingDingAdultSpinePath);
                    }

                    if (LeftTianTian)
                    {
                        var tiantian = CreateRole(parent, "sTT", Config.TianTianAdultLeftPos, Config.TianTianAdultLeftScale);
                        tiantian.AddSkeletonGraphic(Config.TianTianAdultSpinePath);
                    }

                    if (MiddleTianTian)
                    {
                        var tiantian = CreateRole(parent, "dTT", Config.TianTianAdultMiddlePos, Config.TianTianAdultMiddleScale);
                        tiantian.AddSkeletonGraphic(Config.TianTianAdultSpinePath);
                    }

                    break;

            }

        }

        private GameObject CreateRole(Transform parent, string name, Vector2 pos, Vector2 scale, PivotPresetsType pivotPresetsType = PivotPresetsType.LeftBottom , AnchorPresetsType anchorPresetsType = AnchorPresetsType.LeftBottom)
        {
            GameObject role = new GameObject(name);
            role.transform.SetParent(parent);
            role.AddRectTransform(Vector2.zero, pivotPresetsType, anchorPresetsType);
            role.SetAnchorPos(pos);
            role.SetScale(scale);

            return role;
        }
    }

}


   
