using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseArtPart1
    {
        GameObject curGo;
        ILObject3DAction libtn;
        ILObject3DAction pingbtn;
        int[] dataList;
        Dictionary<int, List<Sprite>> dictImg;
        List<Sprite> matchList;
        Dictionary<Sprite,int> matchDict;
        Dictionary<string, List<string>> animationData;
        GameObject mask;
        int numli;
        int numping;
        int curIndex;
        string slot;
        GameObject tiantian;
        MonoBehaviour mono;
        //平面 1 ，立体 0
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            libtn = curTrans.Find("spineParent/Li").GetComponent<ILObject3DAction>();
            pingbtn = curTrans.Find("spineParent/Ping").GetComponent<ILObject3DAction>();
            libtn.OnMouseDownLua = OnMouseDown;
            pingbtn.OnMouseDownLua = OnMouseDown;
            numli = 0;
            numping = 0;
            curIndex = 0;
            mask = curTrans.Find("mask").gameObject;
            slot = "tabImg";
            tiantian = curTrans.Find("tiantian").gameObject;
            SoundManager.instance.BgSoundPart1();
            mono = curGo.GetComponent<MonoBehaviour>();
            //curGo.GetComponent<MonoBehaviour>().StartCoroutine(waitStart());
            OnInit();
            OnStart();
        }
        IEnumerator waitStart(float _time,Action act)
        {
            yield return new WaitForSeconds(_time);
            act();
        }
        public void OnStart()
        {
            MaskShow();
            CurAnimationImg();
            SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, 0, null, delegate ()
                 {
                     tiantian.SetActive(true);
                     SpineManager.instance.DoAnimation(tiantian, "breath", true);
                     playAnimation("Cl", () => { MaskHide(); });
                 });
        }
        public void CurAnimationImg()
        {
            GetSkeletonAnimation("TabLi").gameObject.SetActive(false);
            GetSkeletonAnimation("TabPing").gameObject.SetActive(false);
            SkeletonAnimation curSke;
            if (dataList[curIndex] == 0)
            {
                curSke = GetSkeletonAnimation("TabLi");
                curSke.gameObject.SetActive(true);
            }
            else
            {
                curSke = GetSkeletonAnimation("TabPing");
                curSke.gameObject.SetActive(true);
            }
            TabImgState(curSke);
        }
        public void OnMouseDown(int index)
        {
            IsCheckBtn(index);
        }
        public SkeletonAnimation GetSkeletonAnimation(string str)
        {
            GameObject go = curGo.transform.Find("spineParent").gameObject;
            SkeletonAnimation ske = go.transform.Find(str).GetComponent<SkeletonAnimation>();
            return ske;
        }
        public void playAnimation(string name ,Action act = null, bool isClose = false)//0开 false，1关 true
        {
            GameObject go = GetSkeletonAnimation(name).gameObject;
            if(isClose)
            {
                SpineManager.instance.DoAnimation(go, animationData[name][1],false,act);
            }
            else
            {
                SpineManager.instance.DoAnimation(go, animationData[name][0], false, act);
            }
        }
        public void SetAnimationState(string name)
        {
            SkeletonAnimation curske = GetSkeletonAnimation(name);
            string stateName = animationData[name][0];
            SpineManager.instance.PlayAnimationState(curske, stateName);
            Shader curskeShader = curske.skeletonDataAsset.atlasAssets[0].materials[0].shader;
            if (name != "TabImg")
            {
            }
            else
            {
                SpineManager.instance.CreateRegionAttachmentByTexture(curske, slot, matchList[curIndex], curskeShader);
            }
        }
        public void OnInit()
        {
            matchList = new List<Sprite>();
            matchDict = new Dictionary<Sprite, int>();
            dictImg = new Dictionary<int, List<Sprite>>();
            GameObject go = curGo.transform.Find("spineParent").gameObject;
            dataList = new int[] { 1, 0, 0, 1, 0, 1, 0, 1 };
            animationData = new Dictionary<string, List<string>>
            {
                { "Cl",new List<string>{"open_cl", "close_cl" } },
                { "Li",new List<string>{ "right_li","error_li" } },
                { "Ping",new List<string>{ "right_ping","error_ping" } },
                { "Star",new List<string>{"star" } },
                { "TabPing",new List<string>{"tab_image"} },
                { "TabLi",new List<string>{"tab_image"} }
            };
            go = curGo.transform.Find("data").gameObject;
            List<Sprite> curList1 = new List<Sprite>();
            List<Sprite> curList2 = new List<Sprite>();
            for (int i = 0;i<go.transform.childCount;i++)
            {
                if(i<go.transform.childCount/2)
                {
                    curList1.Add(go.transform.GetChild(i).GetComponent<Image>().sprite);
                }
                else
                {
                    curList2.Add(go.transform.GetChild(i).GetComponent<Image>().sprite);
                }
            }
            Debug.Log("curList1 : " + curList1.Count);
            Debug.Log("curList2 : " + curList2.Count);
            dictImg.Add(0, curList1);
            dictImg.Add(1, curList2);
            Debug.Log("dictImg : " + dictImg.Count);
            dataInit();
            foreach (var k in animationData.Keys)
            {
                SetAnimationState(k);
            }
        }
        public void dataInit()
        {
            for(int i = 0;i < dataList.Length;i++)
            {
                if(dataList[i] == 0)
                {
                    numli++;
                }
                else
                {
                    numping++;
                }
            }
            Debug.Log("numli:" + numli);
            Debug.Log("numping:" + numping);
            Debug.Log("dictImg[0]:" + dictImg[0].Count);
            Debug.Log("dictImg[1]:" + dictImg[1].Count);
            List<Sprite> curli = SortNum(dictImg[0], numli);
            Debug.Log("curli : "+ curli.Count);
            List<Sprite> curping = SortNum(dictImg[1], numping);
            Debug.Log("curping : " + curping.Count);
            numli = 0;
            numping = 0;
            for (int i = 0; i < dataList.Length; i++)
            {
                if (dataList[i] == 0)
                {
                    matchList.Add(curli[numli]);
                    matchDict.Add(curli[numli], dataList[i]);
                    numli++;
                }
                else
                {
                    matchList.Add(curping[numping]);
                    matchDict.Add(curping[numping],dataList[i]);
                    numping++;
                }
            }
        }
        public int GetSpriteType()
        {
            return matchDict[matchList[curIndex]];
        }
        public List<Sprite> SortNum(List<Sprite> _curlist, int _num)
        {
            int count = _curlist.Count;
            List<Sprite> curlist = new List<Sprite>();
            Sprite curSpr = null;
            if (_num > _curlist.Count) return null;
            for (int i = 0; i < _num; i++)
            {
                int index = UnityEngine.Random.Range(0, count);
                curlist.Add(_curlist[index]);
                curSpr = _curlist[index];
                _curlist[index] = _curlist[count - 1];
                _curlist[count-1] = curSpr;
                count--;
            }
            return curlist;
        }
        public void OnClickPing(bool b)
        {
            MaskShow();
            float time1;
            float time2;
            time2 = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            Action act = null;
            if (b == true)
            {
                act = () =>
                {
                    int num = UnityEngine.Random.Range(4, 7);
                    time1 = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, num, false);
                    SpineManager.instance.DoAnimation(tiantian, "talk", true);
                    mono.StartCoroutine(waitStart(time1, () => 
                    {
                        SpineManager.instance.DoAnimation(tiantian, "breath", true);
                        MaskHide();
                    }));
                };
                mono.StartCoroutine(waitStart(time2, act));
            }
            else
            {
                Debug.Log("next-ping");
                act = () =>
                {
                    Debug.Log("act-ping");
                    curIndex++;
                    int num = UnityEngine.Random.Range(1, 4);
                    OnEndSound(num);
                    playAnimation("Star");
                    playAnimation("Cl", () =>
                    {
                        OnEnd();
                    }, true);
                };
                mono.StartCoroutine(waitStart(time2, act));

            }
            playAnimation("Ping", null, b);
        }
        public void OnClickLi(bool b)
        {
            MaskShow();
            float time1;
            float time2;
            time2 = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            Action act = null;
            if (b == true)
            {
                act = () =>
                    {
                        int num = UnityEngine.Random.Range(4, 7);
                        time1 = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, num, false);
                        SpineManager.instance.DoAnimation(tiantian, "talk", true);
                        mono.StartCoroutine(waitStart(time1, () =>
                        {
                            SpineManager.instance.DoAnimation(tiantian, "breath", true);
                            MaskHide();
                        }));
                    };
                mono.StartCoroutine(waitStart(time2, act));
            }
            else
            {
                Debug.Log("next-Li");
                act = () =>
                {
                    Debug.Log("act-Li");
                    curIndex++;
                    int num = UnityEngine.Random.Range(1, 4);
                    OnEndSound(num);
                    playAnimation("Star");
                    playAnimation("Cl", () =>
                    {
                        OnEnd();
                    }, true);
                };
                mono.StartCoroutine(waitStart(time2, act));
            }
            playAnimation("Li", null, b);
        }
        public void TabImgState(SkeletonAnimation curske)
        {
            Debug.Log("----------TabImgState-------------");
            Shader curskeShader = curske.skeletonDataAsset.atlasAssets[0].materials[0].shader;
            SpineManager.instance.CreateRegionAttachmentByTexture(curske, slot, matchList[curIndex], curskeShader);
        }
        public void IsCheckBtn(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
            if (index == 0)
            {
                if (GetSpriteType() == 0)
                {
                    OnClickLi(false);
                }
                else
                {
                    OnClickLi(true);
                }
            }
            else
            {
                if (GetSpriteType() == 1)
                {
                    OnClickPing(false);
                }
                else
                {
                    OnClickPing(true);
                }
            }
        }
        public void MaskShow()
        {
            mask.SetActive(true);
        }
        public void MaskHide()
        {
            mask.SetActive(false);
        }
        public void OnEndSound(int num)
        {
            Debug.Log("------------------OnEndSound-----------------");
            float time = 0;
            SpineManager.instance.DoAnimation(tiantian, "talk", true);
            if (curIndex == dataList.Length)
            {
                time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
            }
            else
            {
                time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, num, false);
            }
            mono.StartCoroutine(waitStart( time,() => { SpineManager.instance.DoAnimation(tiantian, "breath", true); }));
        }
        public void OnEnd()
        {
            Debug.Log("---------------OnEnd-------------");
            if(curIndex == dataList.Length)
            {
                GetSkeletonAnimation("Li").gameObject.SetActive(false);
                GetSkeletonAnimation("Ping").gameObject.SetActive(false);
            }
            else
            {
                CurAnimationImg();
                playAnimation("Cl", () =>
                {
                    MaskHide();
                }, false);
            }
        }
    }
}
