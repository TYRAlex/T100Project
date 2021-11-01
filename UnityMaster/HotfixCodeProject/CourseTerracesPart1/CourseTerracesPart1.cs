using CourseTerracesPart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    class CourseTerracesPart1
    {
        GameObject curGo;
        public static RootILBehaviour rootILBehaviour;

        public static GameObject mask;
        public static bool isFirstOnclick;
        public string curColorIndex;
        public string curTargetIndex;
        public string curTerracesIndex;

        public List<TargetBtn> targetBtns;
        public List<terracesBtn> terracesBtns;
        public List<ColorBtn> colorBtns;
        public GameObject colorObj;

        public Dictionary<string, Sprite> dataBtn;
        public Dictionary<string, Sprite[]> dataColor;
        public Dictionary<string, Dictionary<string, Sprite>> dataTerraces;
        public List<string> onclikTian;
        public GameObject dingding;
        public GameObject btns;
        public GameObject color;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            rootILBehaviour = curGo.GetComponent<RootILBehaviour>();
            colorObj = curGo.transform.Find("UI3D/color").gameObject;
            mask = curTrans.Find("UI3D/mask").gameObject;
            btns = curTrans.Find("UI3D/btns").gameObject;
            color = curGo.transform.Find("UI3D/color").gameObject;
            color.transform.localScale = Vector3.zero;
            isFirstOnclick = true;
            curColorIndex = "";
            curTargetIndex = "";
            curTerracesIndex = "";
            OnInitDate();
            OnInitClass();
            onclikTian = new List<string>();
            StratSpreaking();
        }
        public void StratSpreaking()
        {
            dingding = curGo.transform.Find("dingding").gameObject;
            mask.SetActive(true);
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(dingding, "talk", true);
            curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time));
        }
        IEnumerator wait(float time)
        {
            yield return new WaitForSeconds(time);
            dingding.SetActive(false);
            btns.transform.localScale = Vector3.one;
            curTerracesIndex = "1";
            terracesBtns[0].BtnOnclick(null);
            mask.SetActive(false);
        }
        public void OnInitDate()
        {
            Transform data = curGo.transform.Find("data");
            Transform curTran = data.Find("databtn");
            int count = curTran.childCount;
            dataBtn = new Dictionary<string, Sprite>();
            dataColor = new Dictionary<string, Sprite[]>();
            for(int i = 0;i < count;i++)
            {
                Sprite[] curDataColor = new Sprite[4];
                dataBtn.Add(i + "", curTran.GetChild(i).GetComponent<Image>().sprite);
                int childCount = curTran.GetChild(i).childCount;
                for(int j = 0;j < childCount;j++)
                {
                    curDataColor[j] = curTran.GetChild(i).GetChild(j).GetComponent<Image>().sprite;
                }
                dataColor.Add(i + "", curDataColor);
            }
            curTran = data.Find("dataterraces");
            count = curTran.childCount;
            dataTerraces = new Dictionary<string, Dictionary<string, Sprite>>();
            for(int i = 0;i<count;i++)
            {
                Dictionary<string, Sprite> curDict = new Dictionary<string, Sprite>();
                int childCount = curTran.GetChild(i).childCount;
                for(int j = 0;j < childCount;j++)
                {
                    curDict.Add(curTran.GetChild(i).GetChild(j).name,
                        curTran.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>().sprite);
                }
                dataTerraces.Add(i + "", curDict);
            }
        }
        public void OnInitClass()
        {
            Transform ui3d = curGo.transform.Find("UI3D");
            GameObject go = ui3d.Find("btns").gameObject;
            int count = go.transform.childCount;
            targetBtns = new List<TargetBtn>();
            for(int i = 0;i<count;i++)
            {
                targetBtns.Add(new TargetBtn(go.transform.GetChild(i).gameObject, dataBtn[i + ""]));
            }
            go = ui3d.Find("color").gameObject;
            colorBtns = new List<ColorBtn>();
            count = go.transform.childCount;
            for(int i = 0;i < count;i++)
            {
                ColorBtn colorBtn = new ColorBtn(go.transform.GetChild(i).gameObject, null);
                colorBtn.OnInit();
                colorBtns.Add(colorBtn);
            }
            go = ui3d.Find("terraces").gameObject;
            terracesBtns = new List<terracesBtn>();
            count = go.transform.childCount;
            for(int i = 0;i <count;i++)
            {
                terracesBtns.Add(new terracesBtn(go.transform.GetChild(i).gameObject, null));
            }
        }
        public void OnMouseDown(int index)
        {
            //花纹点击
            if(index>0 && index < 50)
            {
                if (index + "" == curTargetIndex) return;
                curTargetIndex = index.ToString();
                PushColorBtn(dataColor[int.Parse(curTargetIndex) -1+""]);
                foreach(var v in targetBtns)
                {
                    v.OnInit();
                }
                targetBtns[int.Parse(curTargetIndex) - 1].BtnOnclick(() => { mask.SetActive(false); });
            }
            //颜色点击
            else if(index>50 && index<100)
            {
                if (index + "" == curColorIndex) return;
                
                curColorIndex = (index - 50).ToString();
                colorBtns[int.Parse(curColorIndex) - 1].BtnOnclick(PushTerraces);
                
            }
            //梯田点击
            else if(index>100 && index <150)
            {
                if (index + "" == curTerracesIndex) return;
                
                curTerracesIndex = (index - 100).ToString();
                foreach(var v in terracesBtns)
                {
                    v.HideLight();
                }
                terracesBtns[int.Parse(curTerracesIndex) - 1].BtnOnclick(null);
            }
        }
        public void PushTerraces()
        {
            string tian = "p" + curTerracesIndex + "_" + curTargetIndex + "_" + curTerracesIndex + "_" + curColorIndex;
            terracesBtns[int.Parse(curTerracesIndex) - 1].ChooseImage(dataTerraces[int.Parse(curTerracesIndex) -1+""][tian]);
            bool isAdd = true;
            foreach (string v in onclikTian)
            {
                if (v == curTerracesIndex)
                {
                    isAdd = false;
                }
            }
            if(isAdd)
            {
                onclikTian.Add(curTerracesIndex);
            }
            Debug.Log("onclikTian.Count-------" + onclikTian.Count);
            if (onclikTian.Count == 8)
            {
                terracesBtns[int.Parse(curTerracesIndex) - 1].HideLight();
                btns.gameObject.SetActive(false);
                color.SetActive(false);
            }
            else
            {
                mask.SetActive(false);
            }
        }
        public void PushColorBtn(Sprite[] sprites)
        {
            if (isFirstOnclick)
            {
                isFirstOnclick = false;
                colorObj.transform.localScale = Vector3.one;
            }
            //for (int i = 0; i < colorBtns.Count; i++)
            //{
            //    colorBtns[i].OnInit();
            //}
            for (int i = 0; i < colorBtns.Count; i++)
            {
                colorBtns[i].ChooseImage(sprites[i]);
            }
            curColorIndex = "";
        }
    }
}
