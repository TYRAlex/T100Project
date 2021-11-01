
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace TDCreate {

    public class SpritePart : BassPart
    {
        [InfoBox("是否需要飘彩带")] public bool IsNeedRibbon;
        [InfoBox("讲几句话(默认是1句)")] public int SpeakNums = 1;

        protected override void CreateScript()
        {
            CreateCsProject(Config.TemplateSpritePartCsPath, (contents, createCsPath) =>
            {
                string talkReplace1 = string.Empty;
                string talkReplace2 = string.Empty;
                string talkReplace3 = string.Empty;
                string talkReplace4 = string.Empty;

                string spReplace1 = string.Empty;
                string spReplace2 = string.Empty;
                string spReplace3 = string.Empty;

                string bellSpeckReplace1 = string.Empty;
                string bgmReplace1 = string.Empty;

                bool isSpeakMore = SpeakNums != 1;

                if (isSpeakMore)
                {
                    talkReplace1 = "private int _talkIndex;";
                    talkReplace2 = "_talkIndex = 1;";
                    talkReplace3 = "SoundManager.instance.SetVoiceBtnEvent(TalkClick);";
                    talkReplace4 = @" 
        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(9);
            switch (_talkIndex)
            {
                 case 1:
                   BellSpeck(_dBD,1);
                 break;
            }
             _talkIndex++;
        } ";
                }

                contents = contents.Replace("talkReplace1", talkReplace1);
                contents = contents.Replace("talkReplace2", talkReplace2);
                contents = contents.Replace("talkReplace3", talkReplace3);
                contents = contents.Replace("talkReplace4", talkReplace4);
                bgmReplace1 = "PlayCommonBgm(6);";

                if (IsNeedRibbon)
                {
                    spReplace1 = "private GameObject _sP;";
                    string sp2 = string.Format("_sP = curTrans.GetGameObject(\"{0}\");", "SP");
                    spReplace2 = sp2;
                    string sp3 = string.Format("_sP.Show();" +
                        "_sP.GetComponent<SkeletonGraphic>().Initialize(true);" +
                        " PlaySpine(_sP, \"{0}\");" +
                        "PlaySpine(_sP, \"{1}\");",
                        "kong", "sp");
                    spReplace3 = sp3;
                    bgmReplace1 = "PlayCommonBgm(4);";
                }

                contents = contents.Replace("spReplace1", spReplace1);
                contents = contents.Replace("spReplace2", spReplace2);
                contents = contents.Replace("spReplace3", spReplace3);
                contents = contents.Replace("bgmReplace1", bgmReplace1);
                if (isSpeakMore)
                    bellSpeckReplace1 = string.Format("BellSpeck(_dBD, {0},null,{1});", 0, "()=>{ SoundManager.instance.ShowVoiceBtn(true);}");
                else
                    bellSpeckReplace1 = string.Format("BellSpeck(_dBD, {0});", 0);

                contents = contents.Replace("bellSpeckReplace1", bellSpeckReplace1);

                File.WriteAllText(createCsPath, contents);
            });

            CreateCsprojProject();
            OpenCsFolder();
        }

        protected override void CreatePrefab()
        {
            var rootTra = base.CreatePartRoot();
            CreateBg(rootTra);

            CreateMiddleBd(rootTra);
            if (IsNeedRibbon)
                CreateSp(rootTra);

            SavePrefab(rootTra.gameObject);
        }





        private void CreateSp(Transform parent)
        {
            GameObject sp = new GameObject("SP");
            sp.transform.SetParent(parent);
            sp.AddRectTransform(Config.ConstSpineV2, PivotPresetsType.LeftTop, AnchorPresetsType.LeftTop);
            sp.AddSkeletonGraphic(Config.SpSpinePath);
        }
    }
}

  
