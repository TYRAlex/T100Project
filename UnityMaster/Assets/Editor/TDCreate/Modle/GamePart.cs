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
                string nextSpineReplace2 = string.Empty;
                string nextSpineReplace3 = string.Empty;
                string nextSpineReplace4 = string.Empty;
                string nextSpineReplace5 = string.Empty;

                if (IsNeedNextBtn)
                {
                    nextSpineReplace1 = "private GameObject _nextSpine;";
                    nextSpineReplace2 = "_nextSpine = curTrans.GetGameObject(\"nextSpine\");";
                    nextSpineReplace3 = "_nextSpine.Hide();";
                    nextSpineReplace4 = "RemoveEvent(_nextSpine);";
                    #region nextSpineReplace5
                    nextSpineReplace5 = @"
		/// <summary>
        /// 游戏下一步
        /// </summary>
		private void GameNext()
		{
			_nextSpine.Show();
            PlaySpine(_nextSpine, ""next2"",()=> {
                AddEvent(_nextSpine, nextGo => {
                    PlayOnClickSound();
                    RemoveEvent(_nextSpine);
                    PlaySpine(_nextSpine, ""next"", () => {
                        _nextSpine.Hide();
                        //ToDo....                      
                    });
                });
            });
        }
";
                    #endregion
                }

                contents = contents.Replace("nextSpineReplace1", nextSpineReplace1);
                contents = contents.Replace("nextSpineReplace2", nextSpineReplace2);
                contents = contents.Replace("nextSpineReplace3", nextSpineReplace3);
                contents = contents.Replace("nextSpineReplace4", nextSpineReplace4);
                contents = contents.Replace("nextSpineReplace5", nextSpineReplace5);
                #endregion

                #region MiddleBd
                string dBDReplace1 = string.Empty;
                string dBDReplace2 = string.Empty;
                string dBDReplace3 = string.Empty;


                if (MiddleBuDing)
                {
                    dBDReplace1 = "  private GameObject _dBD;";
                    dBDReplace2 = "_dBD = curTrans.GetGameObject(\"dBD\");";
                    dBDReplace3 = " _dBD.Hide(); ";
                }
                contents = contents.Replace("dBDReplace1", dBDReplace1);
                contents = contents.Replace("dBDReplace2", dBDReplace2);
                contents = contents.Replace("dBDReplace3", dBDReplace3);
                #endregion

                #region MiddleDingDing
                string dDDReplace1 = string.Empty;
                string dDDReplace2 = string.Empty;
                string dDDReplace3 = string.Empty;

                if (MiddleDingDing)
                {
                    dDDReplace1 = "  private GameObject _dDD;";
                    dDDReplace2 = "_dDD = curTrans.GetGameObject(\"dDD\");";
                    dDDReplace3 = " _dDD.Hide(); ";

                }
                contents = contents.Replace("dDDReplace1", dDDReplace1);
                contents = contents.Replace("dDDReplace2", dDDReplace2);
                contents = contents.Replace("dDDReplace3", dDDReplace3);

                #endregion

                #region MiddleTianTian
                string dTTReplace1 = string.Empty;
                string dTTReplace2 = string.Empty;
                string dTTReplace3 = string.Empty;

                if (MiddleTianTian)
                {
                    dTTReplace1 = "  private GameObject _dTT;";
                    dTTReplace2 = "_dTT = curTrans.GetGameObject(\"dTT\");";
                    dTTReplace3 = "_dTT.Hide(); ";

                }
                contents = contents.Replace("dTTReplace1", dTTReplace1);
                contents = contents.Replace("dTTReplace2", dTTReplace2);
                contents = contents.Replace("dTTReplace3", dTTReplace3);
                #endregion

                #region MiddleXem
                string xemReplace1 = string.Empty;
                string xemReplace2 = string.Empty;
                string xemReplace3 = string.Empty;

                if (MiddleXem)
                {
                    xemReplace1 = "  private GameObject _xem;";
                    xemReplace2 = "_xem = curTrans.GetGameObject(\"xem\");";
                    xemReplace3 = "_xem.Hide(); ";

                }
                contents = contents.Replace("xemReplace1", xemReplace1);
                contents = contents.Replace("xemReplace2", xemReplace2);
                contents = contents.Replace("xemReplace3", xemReplace3);
                #endregion

                #region LfetBd
                string sBDReplace1 = string.Empty;
                string sBDReplace2 = string.Empty;
                string sBDReplace3 = string.Empty;

                if (LeftBuDing)
                {
                    sBDReplace1 = "  private GameObject _sBD;";
                    sBDReplace2 = "_sBD = curTrans.GetGameObject(\"sBD\");";
                    sBDReplace3 = "_sBD.Hide(); ";

                }
                contents = contents.Replace("sBDReplace1", sBDReplace1);
                contents = contents.Replace("sBDReplace2", sBDReplace2);
                contents = contents.Replace("sBDReplace3", sBDReplace3);
                #endregion

                #region LeftDingDing
                string sDDReplace1 = string.Empty;
                string sDDReplace2 = string.Empty;
                string sDDReplace3 = string.Empty;

                if (LeftDingDing)
                {
                    sDDReplace1 = "  private GameObject _sDD;";
                    sDDReplace2 = "_sDD = curTrans.GetGameObject(\"sDD\");";
                    sDDReplace3 = " _sDD.Hide(); ";

                }
                contents = contents.Replace("sDDReplace1", sDDReplace1);
                contents = contents.Replace("sDDReplace2", sDDReplace2);
                contents = contents.Replace("sDDReplace3", sDDReplace3);

                #endregion

                #region LeftTianTian
                string sTTReplace1 = string.Empty;
                string sTTReplace2 = string.Empty;
                string sTTReplace3 = string.Empty;

                if (LeftTianTian)
                {
                    sTTReplace1 = "  private GameObject _sTT;";
                    sTTReplace2 = "_sTT = curTrans.GetGameObject(\"sTT\");";
                    sTTReplace3 = "_sTT.Hide(); ";

                }
                contents = contents.Replace("sTTReplace1", sTTReplace1);
                contents = contents.Replace("sTTReplace2", sTTReplace2);
                contents = contents.Replace("sTTReplace3", sTTReplace3);
                #endregion

                #region 对话框

                string dialogueReplace1 = string.Empty;
                string dialogueReplace2 = string.Empty;
                string dialogueReplace3 = string.Empty;
                string dialogueReplace4 = string.Empty;
                string dialogueReplace5 = string.Empty;
                string dialogueReplace6 = string.Empty;
                string dialogueReplace7 = string.Empty;
                string posReplace1 = string.Empty;
                string posReplace2 = string.Empty;
                if (IsDialogue)
                {
                    dialogueReplace1 = " private GameObject _dialogue;";
                    dialogueReplace2 = " _dialogue = curTrans.GetGameObject(\"dialogue\");";
                    dialogueReplace3 = "_dialogue.Hide();";
                    dialogueReplace4 = "_dialogue.Show();";
                    dialogueReplace5 = "private List<string> _dialogues;";
                    dialogueReplace6 = " _dialogues = new List<string> {};";
                    #region dialogueReplace7
                    dialogueReplace7 = @"
        /// <summary>
        /// 开始对话
        /// </summary>
        private void StartDialogue()
        {
			 //ToDo...  
			 
            //测试代码记得删
			Delay(4,GameSuccess);			 
        }
";
                    #endregion
                    posReplace1 = " private Vector2 _roleStartPos,_roleEndPos,_enemyStartPos,_enemyEndPos;";
                    posReplace2 = "_roleStartPos = new Vector2(-2170, 539); _roleEndPos = new Vector2(-960, 539);_enemyStartPos = new Vector2(200, 540); _enemyEndPos = new Vector2(-994, 540);";
                }

                contents = contents.Replace("dialogueReplace1", dialogueReplace1);
                contents = contents.Replace("dialogueReplace2", dialogueReplace2);
                contents = contents.Replace("dialogueReplace3", dialogueReplace3);
                contents = contents.Replace("dialogueReplace4", dialogueReplace4);
                contents = contents.Replace("dialogueReplace5", dialogueReplace5);
                contents = contents.Replace("dialogueReplace6", dialogueReplace6);
                contents = contents.Replace("dialogueReplace7", dialogueReplace7);
                contents = contents.Replace("posReplace1", posReplace1);
                contents = contents.Replace("posReplace2", posReplace2);
                #endregion

                #region 布丁对话
                string bDDialogueReplace1 = string.Empty;
                string bDDialogueReplace2 = string.Empty;
                string bDDialogueReplace3 = string.Empty;
                string bDDialogueReplace4 = string.Empty;
                string bDDialogueReplace5 = string.Empty;
                string bDDialogueReplace6 = string.Empty;
                string bDDialogueReplace7 = string.Empty;
                if (DialogueBd)
                {
                    bDDialogueReplace1 = " private RectTransform _buDingRect;";
                    bDDialogueReplace2 = " private Text _buDingRectTxt;";
                    bDDialogueReplace3 = "  _buDingRect = curTrans.GetRectTransform(\"dialogue/buDing\");";
                    bDDialogueReplace4 = "_buDingRectTxt = curTrans.GetText(\"dialogue/buDing/Text\");";
                    #region bDDialogueReplace5
                    bDDialogueReplace5 = @"

        /// <summary>
        /// 布丁对话
        /// </summary>			
		private void BdDialogue(int dialogueIndex,int soundIndex,Action callBack)
		{
			
            SetMove(_buDingRect, _roleEndPos, 1.0f, () => {
                ShowDialogue(_dialogues[dialogueIndex], _buDingRectTxt);                  
                Delay(PlaySound(soundIndex), callBack);
            });
		}
";
                    #endregion
                    bDDialogueReplace6 = " _buDingRectTxt.text = string.Empty;";
                    bDDialogueReplace7 = " SetPos(_buDingRect, _roleStartPos);";
                }

                contents = contents.Replace("bDDialogueReplace1", bDDialogueReplace1);
                contents = contents.Replace("bDDialogueReplace2", bDDialogueReplace2);
                contents = contents.Replace("bDDialogueReplace3", bDDialogueReplace3);
                contents = contents.Replace("bDDialogueReplace4", bDDialogueReplace4);
                contents = contents.Replace("bDDialogueReplace5", bDDialogueReplace5);
                contents = contents.Replace("bDDialogueReplace6", bDDialogueReplace6);
                contents = contents.Replace("bDDialogueReplace7", bDDialogueReplace7);
                #endregion

                #region 小恶魔对话
                string devilDialogueReplace1 = string.Empty;
                string devilDialogueReplace2 = string.Empty;
                string devilDialogueReplace3 = string.Empty;
                string devilDialogueReplace4 = string.Empty;
                string devilDialogueReplace5 = string.Empty;
                string devilDialogueReplace6 = string.Empty;
                string devilDialogueReplace7 = string.Empty;
                if (IsDialogue && DialogueXem)
                {
                    devilDialogueReplace1 = "private RectTransform _devilRect;";
                    devilDialogueReplace2 = "private Text _devilRectTxt;";
                    devilDialogueReplace3 = "_devilRect = curTrans.GetRectTransform(\"dialogue/devil\");";
                    devilDialogueReplace4 = "_devilRectTxt = curTrans.GetText(\"dialogue/devil/Text\");";
                    #region devilDialogueReplace5
                    devilDialogueReplace5 = @"
        /// <summary>
        /// 小恶魔对话
        /// </summary>		
		private void XemDialogue(int dialogueIndex,int soundIndex,Action callBack)
		{			 
			 SetMove(_devilRect, _enemyEndPos, 1.0f, () => {
                ShowDialogue(_dialogues[dialogueIndex], _devilRectTxt);
                Delay(PlaySound(soundIndex), callBack);
            });
		}
";
                    #endregion
                    devilDialogueReplace6 = "_devilRectTxt.text = string.Empty;";
                    devilDialogueReplace7 = "SetPos(_devilRect, _enemyStartPos);";
                }

                contents = contents.Replace("devilDialogueReplace1", devilDialogueReplace1);
                contents = contents.Replace("devilDialogueReplace2", devilDialogueReplace2);
                contents = contents.Replace("devilDialogueReplace3", devilDialogueReplace3);
                contents = contents.Replace("devilDialogueReplace4", devilDialogueReplace4);
                contents = contents.Replace("devilDialogueReplace5", devilDialogueReplace5);
                contents = contents.Replace("devilDialogueReplace6", devilDialogueReplace6);
                contents = contents.Replace("devilDialogueReplace7", devilDialogueReplace7);
                #endregion

                #region 丁丁对话
                string dingDingDialogueReplace1 = string.Empty;
                string dingDingDialogueReplace2 = string.Empty;
                string dingDingDialogueReplace3 = string.Empty;
                string dingDingDialogueReplace4 = string.Empty;
                string dingDingDialogueReplace5 = string.Empty;
                string dingDingDialogueReplace6 = string.Empty;
                string dingDingDialogueReplace7 = string.Empty;
                if (DialogueDingDing)
                {
                    dingDingDialogueReplace1 = "private RectTransform _dingDingRect;";
                    dingDingDialogueReplace2 = "private Text _dingDingRectTxt;";
                    dingDingDialogueReplace3 = " _dingDingRect = curTrans.GetRectTransform(\"dialogue/dingDing\");";
                    dingDingDialogueReplace4 = " _dingDingRectTxt = curTrans.GetText(\"dialogue/dingDing/Text\");";
                    #region dingDingDialogueReplace5
                    dingDingDialogueReplace5 = @"
        /// <summary>
        /// 丁丁对话
        /// </summary>				
	    private void DingDingDialogue(int dialogueIndex,int soundIndex,Action callBack)
		{			
            SetMove(_dingDingRect, _roleEndPos, 1.0f, () => {
                ShowDialogue(_dialogues[dialogueIndex], _dingDingRectTxt);                  
                Delay(PlaySound(soundIndex), callBack);
            });
		}
";
                    #endregion
                    dingDingDialogueReplace6 = " _dingDingRectTxt.text =string.Empty;";
                    dingDingDialogueReplace7 = "SetPos(_dingDingRect, _enemyStartPos);";

                }

                contents = contents.Replace("dingDingDialogueReplace1", dingDingDialogueReplace1);
                contents = contents.Replace("dingDingDialogueReplace2", dingDingDialogueReplace2);
                contents = contents.Replace("dingDingDialogueReplace3", dingDingDialogueReplace3);
                contents = contents.Replace("dingDingDialogueReplace4", dingDingDialogueReplace4);
                contents = contents.Replace("dingDingDialogueReplace5", dingDingDialogueReplace5);
                contents = contents.Replace("dingDingDialogueReplace6", dingDingDialogueReplace6);
                contents = contents.Replace("dingDingDialogueReplace7", dingDingDialogueReplace7);
                #endregion

                #region 田田对话
                string tianTianDialogueReplace1 = string.Empty;
                string tianTianDialogueReplace2 = string.Empty;
                string tianTianDialogueReplace3 = string.Empty;
                string tianTianDialogueReplace4 = string.Empty;
                string tianTianDialogueReplace5 = string.Empty;
                string tianTianDialogueReplace6 = string.Empty;
                string tianTianDialogueReplace7 = string.Empty;
                if (DialogueTianTian)
                {
                    tianTianDialogueReplace1 = "private RectTransform _tianTianRect;";
                    tianTianDialogueReplace2 = "private Text _tianTianRectTxt;";
                    tianTianDialogueReplace3 = "_tianTianRect = curTrans.GetRectTransform(\"dialogue/tianTian\");";
                    tianTianDialogueReplace4 = "_tianTianRectTxt = curTrans.GetText(\"dialogue/tianTian/Text\");";
                    #region tianTianDialogueReplace5
                    tianTianDialogueReplace5 = @"
		/// <summary>
        /// 田田对话
        /// </summary>	
	    private void TianTianDialogue(int dialogueIndex,int soundIndex,Action callBack)
		{			
            SetMove(_tianTianRect, _roleEndPos, 1.0f, () => {
                ShowDialogue(_dialogues[dialogueIndex], _tianTianRectTxt);                  
                Delay(PlaySound(soundIndex), callBack);
            });
		}
";
                    #endregion
                    tianTianDialogueReplace6 = " _tianTianRectTxt.text =string.Empty;";
                    tianTianDialogueReplace7 = "SetPos(_tianTianRect, _enemyStartPos);";
                }

                contents = contents.Replace("tianTianDialogueReplace1", tianTianDialogueReplace1);
                contents = contents.Replace("tianTianDialogueReplace2", tianTianDialogueReplace2);
                contents = contents.Replace("tianTianDialogueReplace3", tianTianDialogueReplace3);
                contents = contents.Replace("tianTianDialogueReplace4", tianTianDialogueReplace4);
                contents = contents.Replace("tianTianDialogueReplace5", tianTianDialogueReplace5);
                contents = contents.Replace("tianTianDialogueReplace6", tianTianDialogueReplace6);
                contents = contents.Replace("tianTianDialogueReplace7", tianTianDialogueReplace7);
                #endregion

                #region 年龄结束动画

                string ageReplace1 = string.Empty;

                if (AgeType == AgeType.Adult)
                    ageReplace1 = " PlaySpine(_successSpine, \"6-12-z\", () => { PlaySpine(_successSpine, \"6-12-z2\"); });";
                else if (AgeType == AgeType.Child)
                    ageReplace1 = " PlaySpine(_successSpine, \"3-5-z\", () => { PlaySpine(_successSpine, \"3-5-z2\"); });";
                contents = contents.Replace("ageReplace1", ageReplace1);
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
            CreateBtn(rootTra);
            CreateSuccessSpine(rootTra);

            if (IsDialogue)
                CreateDialogue(rootTra);


            CreateRole(rootTra);
            SavePrefab(rootTra.gameObject);
        }

        private void CreateBtn(Transform parent)
        {
            GameObject startSpine = new GameObject("startSpine");
            startSpine.transform.SetParent(parent);
            startSpine.AddRectTransform(Config.ConstSpineBtnV2);
            startSpine.SetAnchorPos(Vector2.zero);
            startSpine.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);

            GameObject replaySpine = new GameObject("replaySpine");
            replaySpine.transform.SetParent(parent);
            replaySpine.AddRectTransform(Config.ConstSpineBtnV2);
            replaySpine.SetAnchorPos(Config.ConstReplayBtnPos);
            replaySpine.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);

            GameObject okSpine = new GameObject("okSpine");
            okSpine.transform.SetParent(parent);
            okSpine.AddRectTransform(Config.ConstSpineBtnV2);
            okSpine.SetAnchorPos(Config.ConstOkBtnPos);
            okSpine.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);

            if (IsNeedNextBtn)
            {
                GameObject nextSpine = new GameObject("nextSpine");
                nextSpine.transform.SetParent(parent);
                nextSpine.AddRectTransform(Config.ConstSpineBtnV2);
                nextSpine.SetAnchorPos(Vector2.zero);
                nextSpine.AddSkeletonGraphic(Config.BtnsSpinePath, "None", false, true);
            }
        }

        private void CreateSuccessSpine(Transform parent)
        {
            GameObject successSpine = new GameObject("successSpine");
            successSpine.transform.SetParent(parent);
            successSpine.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);

            switch (AgeType)
            {
                case AgeType.Child:
                    successSpine.AddSkeletonGraphic(Config.SuccessChildSpinePath);
                    break;
                case AgeType.Adult:
                    successSpine.AddSkeletonGraphic(Config.SuccessAdultSpinePath);
                    break;
            }


            GameObject sp = new GameObject("sp");
            sp.transform.SetParent(successSpine.transform);
            sp.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
            sp.AddSkeletonGraphic(Config.SpSpinePath);
        }

        private void CreateDialogue(Transform parent)
        {
            GameObject dialogue = new GameObject("dialogue");
            dialogue.transform.SetParent(parent);
            dialogue.AddRectTransform(Config.ConstV2);


            GameObject devil = new GameObject("devil");
            devil.transform.SetParent(dialogue.transform);
            devil.AddRectTransform(Config.ConstSpineV2);
            devil.SetAnchorPos(new Vector2(200, 540));
            devil.AddSkeletonGraphic(Config.XemDialoguesSpinePath, "dj", true);

            GameObject devilText = new GameObject("Text");
            devilText.transform.SetParent(devil.transform);
            devilText.AddRectTransform(new Vector2(500, 36), PivotPresetsType.CenterTop, AnchorPresetsType.CenterTop);
            devilText.SetAnchorPos(new Vector2(1305, -339));
            devilText.AddText();
            devilText.AddContentSizeFitter();

            if (DialogueBd)
            {
                GameObject bd = new GameObject("bd");
                bd.transform.SetParent(dialogue.transform);
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
                dingDing.transform.SetParent(dialogue.transform);
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
                tianTian.transform.SetParent(dialogue.transform);
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

        }

        private void CreateRole(Transform parent)
        {
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
                        var dingding = CreateRole(parent, "dDD", Config.DingDingChildMiddlePos, Config.DingDingChildMiddleScale);
                        dingding.AddSkeletonGraphic(Config.DingDingChildSpinePath);
                    }

                    if (LeftTianTian)
                    {
                        var tiantian = CreateRole(parent, "sTT", Config.TianTianChildLeftPos, Config.TianTianChildLeftScale);
                        tiantian.AddSkeletonGraphic(Config.TianTianChildSpinePath);
                    }

                    if (MiddleTianTian)
                    {
                        var tiantian = CreateRole(parent, "dTT", Config.TianTianChildMiddlePos, Config.TianTianChildMiddleScale);
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

        private GameObject CreateRole(Transform parent, string name, Vector2 pos, Vector2 scale)
        {
            GameObject role = new GameObject(name);
            role.transform.SetParent(parent);
            role.AddRectTransform(Vector2.zero, PivotPresetsType.LeftBottom, AnchorPresetsType.LeftBottom);
            role.SetAnchorPos(pos);
            role.SetScale(scale);

            return role;
        }
    }

}


   
