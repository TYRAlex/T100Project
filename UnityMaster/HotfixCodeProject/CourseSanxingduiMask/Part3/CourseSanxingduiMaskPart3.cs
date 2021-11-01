using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class CourseSanxingduiMaskPart3
    {
        GameObject curGo;
        GameObject ChooseIconObj = null;
        List<int> IconArr = null;
        string[] IconNameArr = null; 
        int CompelateIconNum = 0;
        int PlayEffectIndex = 0;
        float playTime = 0;
        float matchTime = 0;
        bool IsMatching = false;
        bool IsInShanShuo = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            ChooseIconObj = curTrans.Find("bg/PTPlane/ChooseIcon").gameObject;
            ILObject3DAction UIeventSc = ChooseIconObj.GetComponent<ILObject3DAction>();
            UIeventSc.OnPointDownLua = PointerDown;
            UIeventSc.OnPointUpLua = PointerUp;
            ChooseIconObj.SetActive(false);
            IconArr = new List<int>();
            IconNameArr = new string[] { "UI_jigsaw_13", "UI_jigsaw_14", "UI_jigsaw_15", "UI_jigsaw_16",
                                            "UI_jigsaw_9", "UI_jigsaw_10", "UI_jigsaw_11", "UI_jigsaw_12",
                                            "UI_jigsaw_5", "UI_jigsaw_6", "UI_jigsaw_7", "UI_jigsaw_8",
                                            "UI_jigsaw_1", "UI_jigsaw_2", "UI_jigsaw_3", "UI_jigsaw_4"};
            IconArr.Clear();
            for (int i = 0; i < 16; i++)
            {
                IconArr.Add(i);
            }

            for (int i = 0; i < 16; i++)
            {
                int j = UnityEngine.Random.Range(0, 16);
                int temp = IconArr[j];
                IconArr[j] = IconArr[i];
                IconArr[i] = temp;
                curTrans.Find("bg/PTPlane").GetChild(i).Find("JC_Image").GetComponent<Image>().color = new Color(1, 1, 1, 1 / 255.0f);
                curTrans.Find("bg/TiShiPlane").GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }

            for (int i = 0; i < 5; i++)
            {
                curTrans.Find("bg/PTPlane/TxPlane").GetChild(i).gameObject.SetActive(false);
            }

            CompelateIconNum = 0;
            
            SoundManager.instance.Speaking(curTrans.Find("bg/Dingding").gameObject, "talk", SoundManager.SoundType.VOICE, 4, null, ()=>
                { CreateNewIcon(); });
            SoundManager.instance.BgSoundPart1();
        }

        void CreateNewIcon()
        {
            matchTime = 0;
            IsMatching = true;
            Debug.Log("IconNameArr[CompelateIconNum]: " + IconNameArr[IconArr[CompelateIconNum]]);
            Sprite sp = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("CourseSanxingduiMaskPart3"), IconNameArr[IconArr[CompelateIconNum]]);
            if(sp == null)
            {
                Debug.Log("sp is null");
            }
            ChooseIconObj.GetComponent<Image>().sprite = sp;
            ChooseIconObj.GetComponent<Image>().SetNativeSize();
            ChooseIconObj.SetActive(true);
            ChooseIconObj.transform.DOKill(false);
            ChooseIconObj.transform.localPosition = new Vector3(-1370, 394, 0);
            ChooseIconObj.transform.localScale = Vector3.one * 0.5f;
            ChooseIconObj.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
            ChooseIconObj.GetComponent<Image>().raycastTarget = true;
        }

        void PointerDown(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            ChooseIconObj.transform.DOKill(false);
            ChooseIconObj.transform.DOScale(1.2f, 0.2f);
        }

        void PointerUp(int index)
        {
            ChooseIconObj.GetComponent<Image>().raycastTarget = false;
            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);

            bool IsRightArea = false;
            Sprite sp = null;
            Vector3 pos = Vector3.zero;
            int childIndex = 0;
            bool IsMovetoPTArea = false;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.name.Contains("JC_Image"))
                {
                    IsMovetoPTArea = true;
                    int a = SwitchIconNameToIndex(results[i].gameObject.GetComponent<Image>().sprite.name);
                    int b = SwitchIconNameToIndex(ChooseIconObj.GetComponent<Image>().sprite.name);
                    if (a == b)
                    {
                        sp = results[i].gameObject.GetComponent<Image>().sprite;
                        IsRightArea = true;
                        childIndex = SwitchIconNameToIndex(results[i].gameObject.GetComponent<Image>().sprite.name);
                        pos = curGo.transform.Find("bg/PTPlane").GetChild(childIndex - 1).localPosition;
                        break;
                    }
                }
            }

            if (IsRightArea)
            {
                IsMatching = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                ChooseIconObj.GetComponent<Image>().sprite = sp;
                ChooseIconObj.GetComponent<Image>().SetNativeSize();
                ChooseIconObj.transform.transform.DOScale(1, 0.25f);
                SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Dingding").gameObject, "fun", false);
                if(CompelateIconNum < 15)
                { 
                    int Musicindex = UnityEngine.Random.Range(0, 3);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, Musicindex, false);
                }
                ChooseIconObj.transform.DOLocalMove(pos, 0.25f).OnComplete(() =>
                {
                    ChooseIconObj.SetActive(false);
                    curGo.transform.Find("bg/PTPlane").GetChild(childIndex - 1).Find("JC_Image").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    CompelateIconNum++;
                    if (CompelateIconNum != 16)
                    {
                        CreateNewIcon();
                    }
                    else
                    {
                        SoundManager.instance.Speaking(curGo.transform.Find("bg/Dingding").gameObject, "talk", SoundManager.SoundType.VOICE, 3);
                    }
                });
                
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                if (IsMovetoPTArea)
                {
                    MonoBehaviour mono = curGo.GetComponent<MonoBehaviour>();
                    SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Dingding").gameObject, "unhappy", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                }
                ChooseIconObj.transform.DOLocalMove(new Vector3(-1370, 394), 0.25f).OnComplete(() =>
                {
                    ChooseIconObj.transform.DOScale(1, 0.2f).OnComplete(
                    () =>
                    {
                        ChooseIconObj.GetComponent<Image>().raycastTarget = true;
                    });
                });
                ChooseIconObj.GetComponent<Image>().raycastTarget = false;
            }
        }

        int SwitchIconNameToIndex(string name)
        {
            if (name.Contains("16"))
            {
                return 4;
            }
            else if (name.Contains("15"))
            {
                return 3;
            }
            else if (name.Contains("14"))
            {
                return 2;
            }
            else if (name.Contains("13"))
            {
                return 1;
            }
            else if (name.Contains("12"))
            {
                return 8;
            }
            else if (name.Contains("11"))
            {
                return 7;
            }
            else if (name.Contains("10"))
            {
                return 6;
            }
            else if (name.Contains("1"))
            {
                return 13;
            }
            else if (name.Contains("2"))
            {
                return 14;
            }
            else if (name.Contains("3"))
            {
                return 15;
            }
            else if (name.Contains("4"))
            {
                return 16;
            }
            else if (name.Contains("5"))
            {
                return 9;
            }
            else if (name.Contains("6"))
            {
                return 10;
            }
            else if (name.Contains("7"))
            {
                return 11;
            }
            else if (name.Contains("8"))
            {
                return 12;
            }
            else
            {
                return 5;
            }
        }

        void Update()
        {
            if(CompelateIconNum >= 16)
            {
                playTime -= Time.deltaTime;
                if (playTime <= 0)
                {
                    if(PlayEffectIndex == 0)
                    {
                        GameObject go = curGo.transform.Find("bg/PTPlane/TxPlane").GetChild(0).gameObject;
                        curGo.transform.Find("bg/PTPlane/TxPlane").GetChild(4).gameObject.SetActive(false);
                        go.SetActive(true);
                        SpineManager.instance.DoAnimation(go, "animation", false);
                    }
                    else
                    {
                        GameObject go = curGo.transform.Find("bg/PTPlane/TxPlane").GetChild(PlayEffectIndex).gameObject;
                        curGo.transform.Find("bg/PTPlane/TxPlane").GetChild(PlayEffectIndex - 1).gameObject.SetActive(false);
                        go.SetActive(true);
                        SpineManager.instance.DoAnimation(go, "animation", false);
                    }
                    PlayEffectIndex++;
                    PlayEffectIndex = PlayEffectIndex % 5;
                    playTime = 1.0f;
                }
            }

            if(IsMatching)
            {
                matchTime += Time.deltaTime;
                if(matchTime % 7 > 5 && IsInShanShuo == false)
                {
                    int b = SwitchIconNameToIndex(ChooseIconObj.GetComponent<Image>().sprite.name);
                    ShanShuo(curGo.transform.Find("bg/TiShiPlane/TiShi" + b).gameObject, 0);
                    IsInShanShuo = true;
                }
            }
        }

        void ShanShuo(GameObject go, int count)
        {
            if(count > 6)
            {
                IsInShanShuo = false;
                go.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                return;
            }
            float a = go.GetComponent<Image>().color.a;
            if(a > 0.5f)
            {
                go.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            else
            {
                go.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            go.transform.DOScale(1.01f, 0.3f).OnComplete(() =>
            {
                ShanShuo(go, count + 1);
            });
        }
    }
}
