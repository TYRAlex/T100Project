using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course9113Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private Transform Content;

        private Transform boundPanel;

        List<GameObject> listObj = null;
        ILDrager[] DragsList = null;
        List<ILDroper> DropsList = null;


        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            boundPanel = curTrans.Find("boundPanel");

            Content = curTrans.Find("BottomImg/Scroll View/Viewport/Content");


            DropsList = Bg.GetComponentsInChildren<ILDroper>(true).ToList();
            for (int i = 0, len = DropsList.Count; i < len; i++)
            {
                DropsList[i].SetDropCallBack(OnAfter);
                DropsList[i].gameObject.SetActive(false);
            }

            listObj = new List<GameObject>();
            for (int i = 0; i < Bg.transform.childCount; i++)
            {
                for (int j = 0; j < Bg.transform.GetChild(i).childCount; j++)
                {
                    Bg.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                    Util.AddBtnClick(Bg.transform.GetChild(i).GetChild(j).gameObject, OnSelectObj);
                    if (Bg.transform.GetChild(i).GetChild(j).childCount > 0)
                    {
                        for (int k = 0; k < Bg.transform.GetChild(i).GetChild(j).childCount; k++)
                        {
                            Bg.transform.GetChild(i).GetChild(j).GetChild(k).gameObject.SetActive(false);
                            for (int l = 0; l < Bg.transform.GetChild(i).GetChild(j).GetChild(k).childCount; l++)
                            {
                                Util.AddBtnClick(Bg.transform.GetChild(i).GetChild(j).GetChild(k).GetChild(l).gameObject, OnSelectObj);
                            }
                        }
                    }
                    listObj.Add(Bg.transform.GetChild(i).GetChild(j).gameObject);
                }
            }

            for (int i = 0; i < boundPanel.childCount; i++)
            {
                Util.AddBtnClick(Content.GetChild(i).gameObject, OnClick);
                boundPanel.GetChild(i).GetComponent<ILDrager>().AddDrops(listObj.ToArray());
            }
            DragsList = boundPanel.GetComponentsInChildren<ILDrager>();

            for (int i = 0, len = DragsList.Length; i < len; i++)
            {
                DragsList[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }

            for (int i = 0; i < boundPanel.childCount; i++)
            {
                boundPanel.GetChild(i).gameObject.SetActive(false);
            }
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private void OnSelectObj(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false);
            if (obj.transform.childCount == 1)
            {
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    obj.transform.GetChild(0).gameObject.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).name, false);
                });

            }
            else if (obj.transform.parent.childCount == 3)
            {
                obj.transform.parent.parent.gameObject.name = obj.transform.GetChild(0).name;
                SpineManager.instance.DoAnimation(obj.transform.parent.parent.gameObject, obj.name, false, () => { obj.transform.parent.gameObject.SetActive(false); });

            }


        }

        private void OnClick(GameObject obj)
        {
            BtnPlaySound();
            int index = obj.transform.GetSiblingIndex();

            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {

            });
            boundPanel.GetChild(index).gameObject.SetActive(true);
            boundPanel.GetChild(index).position = Input.mousePosition;

        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }


        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (DropsList[index])
            {
                DropsList[index].gameObject.SetActive(dragType == dropType);
            }

            if (dragType == dropType)
            {
                DropsList[index] = null;
            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            //boundPanel.GetChild(type).position = Input.mousePosition;
            boundPanel.GetChild(type).gameObject.SetActive(true);
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            boundPanel.GetChild(type).position = Input.mousePosition;

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {        
            if (!isMatch)
            {
                boundPanel.GetChild(type).transform.DOMove(Content.GetChild(type).position, 1).OnComplete(() => { boundPanel.GetChild(type).gameObject.SetActive(false); });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                boundPanel.GetChild(type).gameObject.SetActive(false);
            }


        }






        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
