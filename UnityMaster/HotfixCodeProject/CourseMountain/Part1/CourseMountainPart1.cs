using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public struct Face
    {
        public List<Sprite> faceList;
        public string name;
    }
    public class CourseMountainPart1
    {
        static public GameObject curGo;
        List<Face> faceDict;
        SkeletonAnimation targetAnim;
        ILObject3DAction onclick;
        GameObject light;
        Vector3 curLight;
        Vector3 nextLight;
        GameObject back;
        GameObject dingding;
        SkeletonAnimation targetError;
        List<string> slotsName;
        
        List<TargetBone> targetBones;
        bool isFinish;
        bool isEnd;
        GameObject mask;
        GameObject shadow;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Transform tran = curTrans.Find("data").transform;
            faceDict = new List<Face>();
            slotsName = new List<string>();
            for (int i = 0; i < tran.childCount;i++)
            {
                string name = tran.GetChild(i).name + "_0";
                Face face = new Face();
                face.faceList = new List<Sprite>();
                face.name = name;
                for (int j = 1;j < tran.GetChild(i).childCount - 1;j++)
                {
                    face.faceList.Add(tran.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>().sprite);
                }
                faceDict.Add(face);
            }
            onclick = curTrans.Find("UI3D/onclick").GetComponent<ILObject3DAction>();
            onclick.OnMouseDownLua = OnMouseDown;
            onclick.gameObject.SetActive(false);
            light = curTrans.Find("UI3D/light").gameObject;
            curLight = light.transform.localPosition;
            nextLight = new Vector3(curLight.x, curLight.y, 80);
            back = curTrans.Find("UI3D/black").gameObject;
            dingding = curTrans.Find("dingding").gameObject;
            targetError = curTrans.Find("UI3D/targetError").GetComponent<SkeletonAnimation>();
            curGo.AddComponent<MesManager>();
            isFinish = false;
            isEnd = false;
            mask = curTrans.Find("UI3D/mask").gameObject;
            mask.SetActive(true);
            shadow = curTrans.Find("UI3D/shadow").gameObject;
            shadow.SetActive(false);
            targetAnim = curTrans.Find("UI3D/targetParent").GetComponent<SkeletonAnimation>();
            AddListener();
            InitClass();
            ResetTargetError();

            SoundManager.instance.BgSoundPart1();
            Speaking();
            SpineManager.instance.DoAnimation(light, "animation", false);
        }
        public void Speaking()
        {
            SoundManager.instance.Speaking(dingding, "talk",SoundManager.SoundType.SOUND, 0, null, () => mask.SetActive(false));
        }
        public void AddListener()
        {
            MesManager.instance.Register("CourseMountainPart1", 1, AddFace);
        }
        public void InitClass()
        {
            targetBones = new List<TargetBone>();
            for(int i = 0;i < faceDict.Count;i++)
            {
                TargetBone targetBone = new TargetBone(faceDict[i].faceList, i);
                targetBones.Add(targetBone);
            }
        }
        private void AddFace(object[] param)
        {
            string name = param[0].ToString();
            bool ison = true;
            for (int i = 0; i < slotsName.Count; i++)
            {
                if (slotsName[i] == name)
                {
                    ison = false;
                    break;
                }
            }
            if (ison)
            {
                slotsName.Add(name);
            }
            FinishOnMouseDown();
        }

        private void FinishOnMouseDown()
        {
            Debug.Log("lotsName.Count --------->" + slotsName.Count);
            if (slotsName.Count == targetBones.Count)
            {
                shadow.SetActive(true);
                onclick.gameObject.SetActive(true);
                if (isFinish)
                {
                    ResetTargetError();
                    ShowTargetError();
                }
            }
        }

        public void OnMouseDown(int index)
        {
            mask.SetActive(true);
            SpineManager.instance.DoAnimation(onclick.gameObject, "button", false, () => mask.SetActive(false));
            isFinish = true;
            ResetTargetError();
            ShowTargetError();
            if(isEnd == true)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                IsGameOver();
            }
            else
            { 
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
            }
        }
        public void IsGameOver()
        {
            mask.SetActive(true);
            shadow.SetActive(false);
            light.transform.localPosition = nextLight;
            back.SetActive(true);
            SpineManager.instance.DoAnimation(targetAnim.gameObject, "end", false, () => { SpineManager.instance.DoAnimation(targetAnim.gameObject, "end2", true); });
        }
        public void ShowTargetError()
        {
            for(int i = 0; i < targetBones.Count;i++)
            {
                if(targetBones[i].CheckChoose() == false)
                {
                    isEnd = false;
                    SpineManager.instance.ShowSpineTexture(targetError, targetBones[i].slotName);
                }
            }
        }
        public void ResetTargetError()
        {
            isEnd = true;
            for (int i = 0; i < targetBones.Count; i++)
            {
                Debug.Log(targetBones[i].slotName);
                SpineManager.instance.HideSpineTexture(targetError, targetBones[i].slotName);
            }
        }
    }
    public class TargetBone
    {
        ILObject3DAction obj;
        public string slotName;
        SkeletonAnimation anim;
        List<Sprite> chooseColors;
        int curIndex;
        public TargetBone(List<Sprite> _chooseColors, int index)
        {
            anim = CourseMountainPart1.curGo.transform.Find("UI3D/targetParent").GetComponent<SkeletonAnimation>();
            obj = anim.transform.Find("SkeletonUtility-Root/root/bone").GetChild(index).GetComponent<ILObject3DAction>();
            obj.OnMouseDownLua = OnMouseDown;
            slotName = obj.name;
            chooseColors = _chooseColors;
            curIndex = -1;
        }
        public void OnMouseDown(int index)
        {
            curIndex = curIndex + 1 == chooseColors.Count? 0: curIndex + 1;
            Shader shader = anim.GetComponent<MeshRenderer>().material.shader;
            Debug.Log("chooseColors[curIndex].name: " + chooseColors[curIndex].name);
            //if(chooseColors[curIndex].name.Contains("_1"))
            //{
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            //}
            SpineManager.instance.CreateRegionAttachmentByTexture(anim, slotName, chooseColors[curIndex], shader);
            MesManager.instance.Dispatch("CourseMountainPart1", 1, new object[] { slotName });
        }
        public bool CheckChoose()
        {
           return curIndex == 0 ? true : false;
        }
    }
}
