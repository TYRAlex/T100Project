using CourseVasePart;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseVasePart1
    {
        static public GameObject curGo;
        GameObject btnLeft;
        GameObject btnRight;
        Dictionary<string, List<SpriteRenderer>> dataImgs;
        Image titleImg;
        SpriteRenderer[] titleImgs;
        List<BtnColor> btnLists;
        int index;
        int maxIndex;
        BtnColor curColor;
        bool isFirst;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            btnLeft = curGo.transform.Find("btn/left").gameObject;
            btnRight = curGo.transform.Find("btn/right").gameObject;
            Util.AddBtnClick(btnLeft.transform.GetChild(0).gameObject, SwitchImageBtn);
            Util.AddBtnClick(btnRight.transform.GetChild(0).gameObject, SwitchImageBtn);
            SpineManager.instance.PlayAnimationState(btnLeft.GetComponent<SkeletonGraphic>(), btnLeft.name);
            SpineManager.instance.PlayAnimationState(btnRight.GetComponent<SkeletonGraphic>(), btnRight.name);
            titleImg = curGo.transform.Find("title").GetComponent<Image>();
            index = 1;
            GameObject go = curGo.transform.Find("data/titles").gameObject;
            titleImgs = new SpriteRenderer[go.transform.childCount];
            curGo.AddComponent<MesManager>();
            for (int i = 0;i<go.transform.childCount;i++)
            {
                titleImgs[i] = go.transform.GetChild(i).GetComponent<SpriteRenderer>();
            }
            InitClass();
            AddListener();
            SoundManager.instance.BgSoundPart1();
            GameObject TT = curGo.transform.Find("npc").gameObject;
            SoundManager.instance.Speaking(TT, "talk", SoundManager.SoundType.VOICE, 1, null, null);
        }
        public void AddListener()
        {
            MesManager.instance.Register("CourseVasePart1", 1, ClearAnimation);
        }
        public void InitClass()
        {
            Transform tran = curGo.transform.Find("data/Imgs");
            string[] strNames = new string[] { "blue", "green", "purple","red","yellow" , "prototype" };
            int count = -1;
            dataImgs = new Dictionary<string, List<SpriteRenderer>>();
            for (int i = 0;i<tran.childCount ;i++)
            {
                string[] str = tran.GetChild(i).name.Split('_');
                if(str[1] == "1")
                {
                    count++;
                    dataImgs[strNames[count]] = new List<SpriteRenderer>();
                }
                dataImgs[strNames[count]].Add(tran.GetChild(i).GetComponent<SpriteRenderer>());
            }
            tran = curGo.transform.Find("btn");
            btnLists = new List<BtnColor>();
            for (int i = 0;i < tran.childCount-1;i++)
            {
               
                BtnColor btnColor;
                if (i != tran.childCount - 2)
                {
                    GameObject go = tran.GetChild(i + 2).gameObject;
                    Debug.Log(go.name);
                    btnColor = new BtnColor(go.name, dataImgs[go.name].ToArray(), i);
                }
                else
                {
                    btnColor = new BtnColor(dataImgs[strNames[i]].ToArray(), i);
                }
                btnLists.Add(btnColor);
            }
            maxIndex = 2;
            CheckLeftOrRight();
            isFirst = true;
            SwitchImageBtn(btnLeft.transform.GetChild(0).gameObject);
            isFirst = false;
        }
        public void SwitchImageBtn(GameObject go)
        {
            SpineManager.instance.DoAnimation(go.transform.parent.gameObject, go.transform.parent.name, false);
            if(!isFirst)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            }
            if (go.transform.parent.name == btnLeft.name)
            {
                index--;
            }
            else
            {
                index++;
            }
            CheckLeftOrRight();
            Debug.Log(go.name+"--------"+index);
            titleImg.sprite = titleImgs[index].sprite;
            for(int i = 0;i < btnLists.Count;i++)
            {
                btnLists[i].index = index;
            }
            btnLists[btnLists.Count - 1].SetImage();
            ClearAnimation(null);
            //btnLists[btnLists.Count - 1].StateEnd();
        }
        public void ClearAnimation(object[] param)
        {
            for (int i = 0; i < btnLists.Count - 1; i++)
            {
                btnLists[i].StateStart();
            }
        }
        //点击完了检查
        public void CheckLeftOrRight()
        {
            if(index == 0)
            {
                btnLeft.transform.localScale = Vector3.zero;//.SetActive(false);
            }
            else if(index == maxIndex)
            {
                btnRight.transform.localScale = Vector3.zero;//.SetActive(false);
            }
            else
            {
                btnLeft.transform.localScale = Vector3.one;//.SetActive(true);
                btnRight.transform.localScale = Vector3.one;//.SetActive(true);
            }
        }
    }
}
