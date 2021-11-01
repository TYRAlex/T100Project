using DG.Tweening;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseKingdomPart1
    {
        Dictionary<string,ChildGrid> GridDict;
        Dictionary<string,ColorBtn> BtnDict;

        GameObject Grids;
        GameObject Btns;

        GameObject curGo;
        string curGrid_Name;
        string curColor_Name;
        Object3DAction colorAction;
        Dictionary<string, Dictionary<string,Image>> datas;
        List<string> curNameList;
        GameObject tianding;
        bool isOver;
        void Start(object o)
        {
            curGo = (GameObject)o;
                      
            Grids = curGo.transform.Find("Grids").gameObject;
            Btns = curGo.transform.Find("Btns").gameObject;
            colorAction = curGo.transform.Find("GridColor").GetComponent<Object3DAction>();
            tianding = curGo.transform.Find("tiantian").gameObject;
            colorAction.OnMouseDownLua = OnMouseDown;
            curNameList = new List<string>();
            curGrid_Name = "";
            curColor_Name = "";
            isOver = false;
            InitData();
            InitClass();
            SpeakNpc();
        }
        void SpeakNpc()
        {
            SoundManager.instance.Speaking(tianding, "talk2", SoundManager.SoundType.SOUND, 0,
                delegate () { colorAction.gameObject.SetActive(false); },
                delegate () { colorAction.gameObject.SetActive(true); });
        }
        void RandomSound()
        {
            int numIndex = UnityEngine.Random.Range(1, 4);
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, numIndex, false);
            curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time));
        }
        void OverSound()
        {
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            curGo.GetComponent<MonoBehaviour>().StartCoroutine(wait(time));
        }
        IEnumerator wait(float time)
        {
            yield return new WaitForSeconds(time);
            SoundManager.instance.sheildGo.SetActive(false);
        }
        void OnMouseDown(int index)
        {
            //Debug.Log("-----------OnMouseDown------------------");
            if(SoundManager.instance.sheildGo.activeInHierarchy) return;
            Texture2D m_texture = (Texture2D)colorAction.GetComponent<RawImage>().texture;
            Color testColor = m_texture.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);
            //Debug.Log((int)Input.mousePosition.x + "!!!!" + (int)Input.mousePosition.y);
            //Debug.Log(testColor.r + "," + testColor.g + "," + testColor.b);
            if (testColor == Color.white)
            {
                curGrid_Name = "boy1";
            }
            else if (Input.mousePosition.x > 1188 && Input.mousePosition.x < 1574 && Input.mousePosition.y > 378 && Input.mousePosition.y < 646)
            {
                curGrid_Name = "house1";
            }
            else if (Input.mousePosition.x > 1597 && Input.mousePosition.x < 1907 && Input.mousePosition.y > 372 && Input.mousePosition.y < 896)
            {
                curGrid_Name = "house2";
            }
            else if (Input.mousePosition.x > 897 && Input.mousePosition.x < 1141 && Input.mousePosition.y > 141 && Input.mousePosition.y < 508)
            {
                curGrid_Name = "boy3";
            }
            else if (testColor == Color.black)
            {
                curGrid_Name = "boy2";
            }
            else if (testColor == Color.blue)
            {
                curGrid_Name = "mountain";
            }
            else if (testColor == Color.cyan)
            {
                curGrid_Name = "tree";
            }          
            else if (testColor == Color.red)
            {
                curGrid_Name = "sun";
            }
            else if (testColor == Color.green)
            {
                curGrid_Name = "tree_1";
            }
            else
            {
                return;
            }
            //Debug.Log(curGrid_Name);
            if (curGrid_Name != "")
            {
                OnClickBtnGrid();
                ChildGrid curChild = GridDict[curGrid_Name];
                curChild.PlayAnimation();
            }
        }
        void InitData()
        {
            GameObject go = curGo.transform.Find("Data").gameObject;
            datas = new Dictionary<string, Dictionary<string, Image>>();
            int count = go.transform.childCount;
            for(int i = 0;i < count;i++)
            {
                int index = 0;
                Transform cur = go.transform.GetChild(i);
                Dictionary<string, Image> cuDict = new Dictionary<string, Image>();

                while (index < cur.childCount)
                {
                    cuDict.Add(cur.GetChild(index).name,cur.GetChild(index).GetComponent<Image>());
                    index++;                  
                }
                datas.Add(cur.name, cuDict);
            }
        }
        void OnClickBtnGrid()
        {     
            foreach(var v in GridDict.Values)
            {
                v.InitState();
            }
            foreach (var v in BtnDict.Values)
            {
                v.Ischeck(true);
            }
        }
        void OnClickBtnColor(string name)
        {
            //Debug.Log("-----------OnClickBtnColor--------");
            foreach(var v in BtnDict.Values)
            {
                v.InitState();
            }          
            if(curGrid_Name != "")
            {
                curColor_Name = name;
                Image curImage = datas[curGrid_Name][curColor_Name];
                ChildGrid curChild = GridDict[curGrid_Name];
                curChild.SetColor(curImage);
                curChild.InitState();               
                bool isAdd = true;
                foreach (var v in BtnDict.Values)
                {
                    v.Ischeck(false);
                }
                if (isOver) SoundManager.instance.sheildGo.SetActive(false);
                else
                {
                    //Debug.Log("curNameList");
                    if (curNameList.Count > 0)
                    {
                        foreach (var v in curNameList)
                        {
                            Debug.Log(v);
                            if (v == curGrid_Name)
                            {
                                isAdd = false;
                                break;
                            }
                        }
                    }
                    if (isAdd)
                    {
                        curNameList.Add(curGrid_Name);
                    }
                    //Debug.Log("curNameList.Count --------- " + curNameList.Count);
                    if (curNameList.Count == 9)
                    {
                        isOver = true;
                        OverSound();
                    }
                    else
                    {
                        RandomSound();
                    }
                }
                curGrid_Name = "";
            }
        }
        void InitClass()
        {
            GridDict = new Dictionary<string, ChildGrid>();
            int count = Grids.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                string name = Grids.transform.GetChild(i).name;
                ChildGrid curChild;
                if (name == "mountain")
                {
                    curChild = new ChildGrid(curGo, Grids.transform.GetChild(i).gameObject, name, OnClickBtnGrid, datas[name]["white"].sprite);
                }
                else
                {
                    curChild = new ChildGrid(curGo, Grids.transform.GetChild(i).gameObject, name, OnClickBtnGrid);
                }                
                GridDict.Add(name, curChild);
            }
            BtnDict = new Dictionary<string, ColorBtn>();
            count = Btns.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                string name = Btns.transform.GetChild(i).name;
                ColorBtn curChild = new ColorBtn(Btns.transform.GetChild(i).gameObject, name, OnClickBtnColor);
                BtnDict.Add(name, curChild);
            }
        }
    }
    
    public class ChildGrid
    {
        public GameObject light;
        SkeletonAnimation _skeletonAnimation;
        string name;
        Action act;
        GameObject curGo;
        Sprite sprite;
        float timeEnd;
        bool ison;
        SkeletonAnimation _skeLight;
        public Shader curskeShader;
        public Shader curlightShader;
        public ChildGrid(GameObject _curGo, GameObject _selfGo,string _name,Action _act,Sprite _sprite = null)
        {
            Debug.Log(_selfGo.name);
            light = _selfGo.transform.GetChild(0).gameObject;
            _skeLight = light.GetComponent<SkeletonAnimation>();
            _skeletonAnimation = _selfGo.GetComponent<SkeletonAnimation>();
            name = _name;
            act = _act;
            curGo = _curGo;
            sprite = _sprite;
            ison = false;
            curskeShader = _skeletonAnimation.GetComponent<MeshRenderer>().material.shader;
            curlightShader = _skeLight.GetComponent<MeshRenderer>().material.shader;
            OnInit();
        }
        public void OnInit()
        {
            AtlasAsset atlas = _skeletonAnimation.skeletonDataAsset.atlasAssets[0];
            AtlasAsset atlas1ight = light.GetComponent<SkeletonAnimation>().skeletonDataAsset.atlasAssets[0];
            if(name == "mountain")
            {
               
                SpineManager.instance.CreateRegionAttachmentByTexture(_skeletonAnimation, name, sprite, curskeShader);       
            }
            else
            {
                SpineManager.instance.CreateRegionAttachmentByTexture(atlas, name, curGo, _skeletonAnimation.gameObject, curlightShader);
            }
            string curName = "";
            if(name == "boy1")
            {
                curName = "boy";
            }
            else if(name == "tree_1")
            {
                curName = "tree2";
            }
            else if (name == "tree")
            {
                curName = "tree1";
            }
            else
            {
                curName = name;
            }
            SpineManager.instance.CreateRegionAttachmentByTexture(atlas1ight, curName, curGo, null, curlightShader);
            InitState();
        }
        public void SetColor(Image raw)
        {
            //Debug.Log("-----------SetColor---------------");
            //Debug.Log("----------"+name);
            //Debug.Log("----------"+raw.sprite.name);
            SpineManager.instance.CreateRegionAttachmentByTexture(_skeletonAnimation, name, raw.sprite, curskeShader);
        }
        public void InitState()
        {
            SpineManager.instance.getAnimationState(_skeLight.gameObject).SetAnimation(0, "animation", false);
            light.transform.localScale = Vector3.zero ;
            timeEnd = _skeLight.state.GetCurrent(0).Animation.Duration;

            SpineManager.instance.DoAnimation(_skeletonAnimation.gameObject, name+"_stand", false);
        }
        public void lightAnimation()
        {
            light.transform.localScale = Vector3.one * 1f;
            SoundManager.instance.sheildGo.SetActive(false);
            SpineManager.instance.DoAnimation(light, "animation", true);
        }
        IEnumerator wait(Action act)
        {
            yield return new WaitForSeconds(1);
            act();
        }
        public void PlayAnimation()
        {
            curGo.GetComponent<MonoBehaviour>().StopAllCoroutines();
            SoundManager.instance.sheildGo.SetActive(true);           
            SpineManager.instance.DoAnimation(_skeletonAnimation.gameObject, name, false,()=> { lightAnimation();  });
        }
        public void OnMouseDown()
        {           
            act();
            PlayAnimation();          
        }
    }
    public class ColorBtn
    {
        GameObject ske;
        GameObject btn;
        string name;
        Action<string> act;
        public bool ischeck;
        public ColorBtn(GameObject curGo, string _name, Action<string> _act)
        {
            ske = curGo.transform.Find("spine").gameObject;
            btn = curGo.transform.Find("btn").gameObject;
            name = _name;
            act = _act;
            ischeck = false;
            OnInit();
        }
        public void Ischeck(bool _ischeck)
        {
            ischeck = _ischeck;
        }
        public void OnInit()
        {
            Util.AddBtnClick(btn.gameObject, OnClickBtn);
            InitState();
        }
        public void InitState()
        {
            ske.SetActive(false);
            btn.SetActive(true);
        }
        public void PlayAnimation()
        {
            SpineManager.instance.DoAnimation(ske, name, false, ()=>
            {
                InitState();               
            });
        }
        public void OnClickBtn(GameObject btn)
        {
            if(ischeck)
            {
                SoundManager.instance.sheildGo.SetActive(true);
                act(name);
                ske.SetActive(true);
                btn.SetActive(false);
                PlayAnimation();               
            }            
        }
    }

}
