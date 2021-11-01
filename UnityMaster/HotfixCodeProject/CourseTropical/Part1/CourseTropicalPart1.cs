using DG.Tweening;
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
    public class CourseTropicalPart1
    {
        GameObject curGo;
        List<ILDrager> plant_dragers;
        List<ILDroper> dropers;
        GameObject dragers;
        GameObject kuang_anim;
        List<GameObject> plant_anim;
        GameObject startPoint;
        GameObject dragPoint;
        List<GameObject> data_plant;
        GameObject btn;
        GameObject btnClose;
        GameObject btnOpen;
        GameObject black;
        GameObject ComplateBtn;
        Transform plants;
        GameObject dingding;
        Transform child;
        float x;
        int curIndex;
        GameObject curPlant;
        List<ILDrager> canPlanting;
        List<ILDrager> noCanPlanting;
        Vector3 startPos;
        float maxDistance;
        bool isStartDrag;
        bool isOpenBox;
        bool isCanPlanting;
        GameObject mask;
        Dictionary<int, Vector3> dragChildPlants;
        Dictionary<int, Vector2> noDragChildPlants;
        float colorA;
        MonoBehaviour mono;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            plant_dragers = new List<ILDrager>();
            dropers = new List<ILDroper>();
            data_plant = new List<GameObject>();
            plant_anim = new List<GameObject>();
            noCanPlanting = new List<ILDrager>();
            canPlanting = new List<ILDrager>();
            dragChildPlants = new Dictionary<int, Vector3>();
            dragChildPlants[0] = new Vector3(50, -139);
            dragChildPlants[1] = new Vector3(-23, -123.5f);
            dragChildPlants[2] = new Vector3(40, -165.2f);
            dragChildPlants[4] = new Vector3(-17, -140.8f);
            dragChildPlants[5] = new Vector3(-7.2f, -256.6f);
            dragChildPlants[6] = new Vector3(9.4f, -141);

            noDragChildPlants = new Dictionary<int, Vector2>();
            dragers = curTrans.Find("dragers").gameObject;
            kuang_anim = curTrans.Find("kuang_anim").gameObject;
            startPoint = curTrans.Find("beginParent").gameObject;
            dragPoint = curTrans.Find("dragParent").gameObject;
            btn = curTrans.Find("btn").gameObject;
            btnClose = btn.transform.Find("btnClose").gameObject;
            Util.AddBtnClick(btnClose, BtnOnClickClose);
            btnOpen = btn.transform.Find("btnOpen").gameObject;
            Util.AddBtnClick(btnOpen, BtnOnClickOpen);
            black = curTrans.Find("black").gameObject;
            plants = curTrans.Find("beginParent/plants");
            dingding = curTrans.Find("dingding").gameObject;
            dragers.SetActive(false);
            child = kuang_anim.transform.Find("child");
            OnInit();

            isStartDrag = false;
            curIndex = -1;
            maxDistance = 10;
            isOpenBox = true;
            isCanPlanting = false;
            mask = curTrans.Find("mask").gameObject;
            SpineManager.instance.PlayAnimationState(kuang_anim.GetComponent<SkeletonGraphic>(), "kuang_click");
            ShowBtnState(1);
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            colorA = 0.6f;
            curGo.transform.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.VOICE, 0, () => black.SetActive(false), () =>
            {
                dingding.SetActive(true);
                SpineManager.instance.DoAnimation(dingding, "breath", true);
                ScrollRectMoveManager.instance.CreateManager(curGo.transform.Find("ScrollRect_Parent"), Speaking);
                curGo.transform.Find("ScrollRect_Parent").gameObject.SetActive(true);
                SoundManager.instance.SetVoiceBtnEvent(() =>
                {
                    Speak1();
                });
                //dingding.SetActive(true);
                //SpineManager.instance.DoAnimation(dingding, "breath");
                //mono.StartCoroutine(FadeImage(1));
            });
            mono = curGo.GetComponent<MonoBehaviour>();

            curGo.transform.Find("ScrollRect_Parent").gameObject.SetActive(false);

            ComplateBtn = curGo.transform.Find("ComplateBtn").gameObject;
            ComplateBtn.transform.localPosition = new Vector3(1150, -850);
            ComplateBtn.gameObject.SetActive(true);
            curGo.transform.Find("WinBg").gameObject.SetActive(false);

            ComplateBtn.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            ComplateBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                ComplateBtn.gameObject.SetActive(false);
                curGo.transform.Find("WinBg").gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(curGo.transform.Find("WinBg/Win").gameObject, "animation", false, ()=>
                {
                    //mono.StartCoroutine(Wait(3.25f, () =>
                    //{
                        SpineManager.instance.DoAnimation(curGo.transform.Find("WinBg/Win").gameObject, "idle", true);
                    //}));
                });
                mono.StartCoroutine(Wait(1.0f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                }));
            });
            x = curGo.transform.Find("ScrollRect_Parent/Viewport/Content").localPosition.x;
        }

        void Update()
        {
            if(Math.Abs(curGo.transform.Find("ScrollRect_Parent/Viewport/Content").localPosition.x - x) > 0.1f)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                x = curGo.transform.Find("ScrollRect_Parent/Viewport/Content").localPosition.x;
            }
        }

        IEnumerator Wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act.Invoke();
        }

        public void Speaking()
        {
            Debug.Log("ScrollRectMoveManager.instance.index + 1: " + (ScrollRectMoveManager.instance.index + 1));
            if (ScrollRectMoveManager.instance.index >= 8)
            {
                SoundManager.instance.SetVoiceBtnEvent(null);
                SoundManager.instance.ShowVoiceBtn(false);
                curGo.transform.Find("ScrollRect_Parent").gameObject.SetActive(false);
                SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.VOICE, ScrollRectMoveManager.instance.index + 1, () => 
                {
                    black.SetActive(false);
                }, () =>
                {
                    BtnOnClickOpen(null);
                });
            }
            else
            {
                Speak1();
            }
        }

        void Speak1()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.VOICE, ScrollRectMoveManager.instance.index + 1, () => { SoundManager.instance.ShowVoiceBtn(false); black.SetActive(true); }, () =>
            {
                if(ScrollRectMoveManager.instance.index < 8)
                {
                    SpineManager.instance.DoAnimation(dingding, "breath", true);
                    dingding.SetActive(true);
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                else
                {
                    SoundManager.instance.SetVoiceBtnEvent(null);
                    SoundManager.instance.ShowVoiceBtn(false);
                }
            });
        }

        
        IEnumerator FadeImage(int index)
        {
            yield return new WaitForSeconds(1.0f);
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.VOICE, index, () => { black.SetActive(true); }, null);
        }

        IEnumerator W1(Action act)
        {
            yield return new WaitForSeconds(1.0f);
            act.Invoke();
        }
        public void OnInit()
        {
            Transform go = curGo.transform.Find("dragers");
            for (int i = 0; i < go.childCount; i++)
            {
                plant_dragers.Add(go.GetChild(i).GetComponent<ILDrager>());
                plant_dragers[i].SetDragCallback(DragStart, DragEnter, DragEnd);
                plant_dragers[i].index = i;
                int index = i;
                GameObject curObj = plant_dragers[i].gameObject;
                noDragChildPlants[i] = new Vector2(curObj.GetComponent<RectTransform>().rect.width, curObj.GetComponent<RectTransform>().rect.height);
                curObj.GetComponent<ILObject3DAction>().OnPointDownLua = PlantOnClickBtn;
            }
            go = curGo.transform.Find("dropers");
            for (int i = 0; i < go.childCount; i++)
            {
                dropers.Add(go.GetChild(i).GetComponent<ILDroper>());
                dropers[i].SetDropCallBack(DropAfter);
            }
            go = curGo.transform.Find("data_plants");
            for (int i = 0; i < go.childCount; i++)
            {
                data_plant.Add(go.GetChild(i).gameObject);
                data_plant[i].GetComponent<ILDrager>().SetDragCallback();
            }
            go = curGo.transform.Find("kuang_anim/child");
            for (int i = 0; i < go.childCount; i++)
            {
                plant_anim.Add(go.GetChild(i).gameObject);
                SpineManager.instance.PlayAnimationState(plant_anim[i].GetComponent<SkeletonGraphic>(), "plant_" + (i + 1));
            }
        }
        public void PlantOnClickBtn(int index)
        {
            if (curIndex == index) return;
            Debug.Log("---------------PlantOnClickBtn-------------");
            mask.SetActive(true);
            curIndex = index;
            for (int i = 0; i < plant_anim.Count; i++)
            {
                if (i != index)
                {
                    SpineManager.instance.PlayAnimationState(plant_anim[i].GetComponent<SkeletonGraphic>(), "plant_" + (i + 1));
                }
            }
            SpineManager.instance.DoAnimation(plant_anim[index], "plant_" + (index + 1), false, () =>
            {
                mask.SetActive(false);
            });
        }

        //刚开始拖拽的时候
        public void DragStart(Vector3 point, int dragType, int index)
        {
            if (curIndex == index)
            {
                startPos = point;
                //if (index != 3 && index != 7)
                //{
                    //plant_dragers[index].GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
                //}
                Debug.Log("---------------------DragStart-------------------");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            }
        }
        //进行拖拽
        public void DragEnter(Vector3 point, int dragType, int index)
        {
            Debug.Log("mmmeee1");
            if (curIndex == index)
            {
                if (isStartDrag == false)
                {
                    float distance = Vector3.Distance(startPos, point);
                    if (distance > maxDistance)
                    {
                        isStartDrag = true;
                        CreatePlant(curIndex, point);
                        AutoBtnClose();
                    }
                    else
                    {
                        return;
                    }
                }
                /*
                if (index != 3 && index != 7)
                {
                    Vector2 v2 = new Vector2(point.x + dragChildPlants[index].x, point.y + dragChildPlants[index].y);
                    plant_dragers[index].transform.position = v2;
                }
                */
                //没有退出
                if (isOpenBox)
                {
                    curPlant.transform.SetParent(startPoint.transform);
                }
                else//打开面板退出了
                {
                    curPlant.transform.SetParent(dragPoint.transform);
                }
                curPlant.transform.GetChild(0).GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                curPlant.transform.position = new Vector3(point.x, point.y, 0);
            }
        }
        //结束拖拽
        public void DragEnd(Vector3 point, int dragType, int index, bool isMatch)
        {
            Debug.Log("DragEnd---------" + curIndex + "   index:" + index + "------");
            if (curIndex == index)
            {
                if (isMatch)
                {
                    canPlanting.Add(curPlant.GetComponent<ILDrager>());
                    int count = canPlanting.Count - 1;
                    canPlanting[count].index = count;
                    canPlanting[count].dragType = 0;
                    Debug.Log("DragEnd----------canPlanting-----------count:" + count);
                    canPlanting[count].SetDragCallback(DragStart1, DragEnter1, DragEnd1);
                    curPlant.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    curPlant.transform.GetChild(0).DOScaleY(1.25f, 0.15f).OnComplete(() =>
                    {
                        curPlant.transform.GetChild(0).DOScaleY(1f, 0.15f).OnComplete(() =>
                        {
                            curPlant.transform.GetChild(0).DOScaleY(1.05f, 0.15f).OnComplete(() =>
                            {
                                curPlant.transform.GetChild(0).DOScaleY(1f, 0.15f);
                            });
                        });
                    });

                    if (dragPoint.transform.childCount >= 8)
                    {
                        ComplateBtn.transform.DOLocalMove(new Vector3(820, -850, 0), 0.25f).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(ComplateBtn, "animation");
                        });
                    }
                }
                else
                {
                    noCanPlanting.Add(curPlant.GetComponent<ILDrager>());
                    int count = noCanPlanting.Count - 1;
                    noCanPlanting[count].index = count;
                    noCanPlanting[count].dragType = 1;
                    Debug.Log("DragEnd----------noCanPlanting-----------count:" + count);
                    noCanPlanting[count].SetDragCallback(DragStart1, DragEnter1, DragEnd1);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                }
                curPlant.transform.SetParent(dragPoint.transform);
                
                isCanPlanting = false;
                isStartDrag = false;
                plant_dragers[index].transform.position = startPos;
                /*
                if (index != 3 && index != 7)
                {
                    plant_dragers[index].GetComponent<RectTransform>().sizeDelta = noDragChildPlants[index];
                }
                */
                dragers.SetActive(false);

                
            }
        }
        public void DragStart1(Vector3 point, int dragType, int index)
        {
            startPos = point;
            RectTransform curRect = null;
            Debug.Log("----------DragStart1-----------" + index);
            if (dragType == 0)
            {
                curRect = canPlanting[index].GetComponent<RectTransform>();
            }
            else
            {
                curRect = noCanPlanting[index].GetComponent<RectTransform>();
            }
            curRect.sizeDelta = new Vector2(1, 1);
            curRect.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, colorA);
            Debug.Log("---------------DragStart1------------------");
        }
        public void DragEnter1(Vector3 point, int dragType, int index)
        {
            Transform curTrans = null;
            if (dragType == 0)
            {
                curTrans = canPlanting[index].transform;
            }
            else
            {
                curTrans = noCanPlanting[index].transform;
            }
            int _index = int.Parse(curTrans.name.Split('_')[1]) - 1;
            /*
            if (_index != 3 && _index != 7)
            {
                Vector2 v2 = new Vector2(point.x + dragChildPlants[_index].x, point.y + dragChildPlants[_index].y);
                curTrans.position = v2;
                curTrans.GetChild(0).position = point;
            }
            */
        }
        public void DragEnd1(Vector3 point, int dragType, int index, bool isMatch)
        {
            RectTransform curRect = null;
            if (dragType == 0)
            {
                curRect = canPlanting[index].GetComponent<RectTransform>();
            }
            else
            {
                curRect = noCanPlanting[index].GetComponent<RectTransform>();
            }
            int _index = int.Parse(curRect.name.Split('_')[1]) - 1;
            if (_index != 3 && _index != 7)
            {
                //Vector2 v2 = new Vector2(point.x - dragChildPlants[_index].x, point.y - dragChildPlants[_index].y);
                //curRect.sizeDelta = new Vector2(curRect.transform.GetChild(0).GetComponent<RectTransform>().rect.width,
                    //curRect.transform.GetChild(0).GetComponent<RectTransform>().rect.height);
                //curRect.transform.position = v2;
                //curRect.transform.GetChild(0).localPosition = Vector2.zero;
            }
            ILDrager curDrager = null;
            if (dragType == 0)
            {
                curDrager = canPlanting[index];
            }
            else
            {
                curDrager = noCanPlanting[index];
            }
            bool isCan = false;
            for (int i = 0; i < canPlanting.Count; i++)
            {
                if (canPlanting[i] == curDrager)
                {
                    isCan = true;
                    break;
                }
            }
            Debug.Log("111111111111111" + isMatch);
            if (isMatch)
            {
                if (isCan == false)//虚变实
                {
                    noCanPlanting.Remove(curDrager);
                    for (int i = 0; i < noCanPlanting.Count; i++)
                    {
                        noCanPlanting[i].index = i;
                    }
                    canPlanting.Add(curDrager);
                    int count = canPlanting.Count - 1;
                    canPlanting[count].index = count;
                    canPlanting[count].dragType = 0;
                    curDrager.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                curDrager.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);

                curDrager.transform.GetChild(0).DOScaleY(1.25f, 0.15f).OnComplete(() =>
                {
                    curDrager.transform.GetChild(0).DOScaleY(1f, 0.15f).OnComplete(() =>
                    {
                        curDrager.transform.GetChild(0).DOScaleY(1.05f, 0.15f).OnComplete(() =>
                        {
                            curDrager.transform.GetChild(0).DOScaleY(1f, 0.15f);
                        });
                    });
                });

                if (dragPoint.transform.childCount >= 8)
                {
                    ComplateBtn.transform.DOLocalMove(new Vector3(820, -850, 0), 0.25f).OnComplete(() =>
                    {
                        SpineManager.instance.DoAnimation(ComplateBtn, "animation");
                    });
                }
            }
            else
            {
                if (isCan)
                {
                    canPlanting.Remove(curDrager);
                    for (int i = 0; i < canPlanting.Count; i++)
                    {
                        canPlanting[i].index = i;
                    }
                    noCanPlanting.Add(curDrager);
                    int count = noCanPlanting.Count - 1;
                    noCanPlanting[count].index = count;
                    noCanPlanting[count].dragType = 1;
                    curDrager.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, colorA);
                }
            }
            Debug.Log("---------------DragEnd1------------------");
        }
        public void CreatePlant(int index, Vector3 point)
        {
            curPlant = GameObject.Instantiate<GameObject>(data_plant[index], new Vector3(point.x, point.y, 0), Quaternion.identity, plants.transform);
            curPlant.transform.localScale = Vector3.one;
            curPlant.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, colorA);
            curPlant.transform.name = data_plant[index].name;
        }
        public bool DropAfter(int type, int index, int dropType)
        {
            Debug.Log("DropAfter--------------index:" + index);
            if (index == 0)
            {
                isCanPlanting = true;
            }
            return isCanPlanting;
        }
        public void BtnOnClickOpen(GameObject go)
        {
            HideBtn();
            isOpenBox = true;
            if (noCanPlanting.Count > 0)
            {
                ClearNoCanPlanting();
            }
            SpineManager.instance.DoAnimation(kuang_anim, "kuang_click", false, () => ShowBtnState(0));
        }
        public void BtnOnClickClose(GameObject go)
        {
            child.transform.localScale = Vector3.zero;
            isOpenBox = true;
            HideBtn();
            SpineManager.instance.DoAnimation(kuang_anim, "kuang_click2", false, () =>
            {
                ShowBtnState(1);
                isOpenBox = false;
            });
        }
        public void ClearNoCanPlanting()
        {
            for (int i = 0; i < noCanPlanting.Count; i++)
            {
                GameObject.Destroy(noCanPlanting[i].gameObject);
            }
            noCanPlanting.Clear();
            noCanPlanting = new List<ILDrager>();
        }
        public void ShowBtnState(int index)
        {
            btn.SetActive(true);
            if (index == 0)//开
            {
                btnClose.SetActive(true);
                btnOpen.SetActive(false);
                dragers.SetActive(true);
                child.transform.localScale = Vector3.one;
                //plants.SetParent(startPoint.transform);
            }
            else if (index == 1)//关
            {
                btnClose.SetActive(false);
                btnOpen.SetActive(true);
                dragers.SetActive(false);
                child.transform.localScale = Vector3.zero;
            }
        }
        public void AutoBtnClose()
        {
            child.transform.localScale = Vector3.zero;
            isOpenBox = true;
            HideBtn();
            SpineManager.instance.DoAnimation(kuang_anim, "kuang_click2", false, () =>
            {
                btn.SetActive(true);
                isOpenBox = false;
                btnClose.SetActive(false);
                btnOpen.SetActive(true);
            });
        }
        public void HideBtn()
        {
            btn.SetActive(false);
        }

        void ChangeDescripPlant(int index)
        {
            Debug.Log("index: " + index);
            for(int i = 1; i < 9; i++)
            {
                if(i == index)
                {
                    curGo.transform.Find("DescripPlantPlane/Image" + i).localScale = Vector3.zero;
                    curGo.transform.Find("DescripPlantPlane/Image" + i).gameObject.SetActive(true);
                    curGo.transform.Find("DescripPlantPlane/Image" + i).DOScale(0.6f, 0.2f).SetEase(Ease.InOutBack);
                }
                else
                {
                    curGo.transform.Find("DescripPlantPlane/Image" + i).gameObject.SetActive(false);
                }
            }
        }
    }
}
